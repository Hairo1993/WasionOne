using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class TiempoRespuestaResolucionService : ITiempoRespuestaResolucionService
{
    // Fijo mientras solo exista Administración (AreaId de Administración)
    // en esta Área.
    // TODO AJUSTAR: -1 es un valor temporal. Reemplazar por el AreaId real
    // de la Área "Administración" (consultar GET /api/catalogos/areas o la
    // pantalla /admin/catalogos) antes de usar este módulo en producción.
    private const int AREA_ID_ADMINISTRACION = 11; // Area "Administracion" (AreaId real confirmado 22/sep/2026)

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Folio"] = "folio",
        ["FechaRecepcion"] = "fecharecepcion",
        ["HoraRecepcion"] = "horarecepcion",
        ["FechaCierre"] = "fechacierre",
        ["HoraCierre"] = "horacierre",
        ["Solicitante"] = "solicitante",
        ["ResponsableCompras"] = "responsablecompras",
        ["Tipo"] = "tipo",
        ["Prioridad"] = "prioridad",
        ["Estatus"] = "estatus",
        ["Solicitud"] = "solicitud",
        ["SlaCierreHoras"] = "slacierrehoras",
        ["Observaciones"] = "observaciones",
        ["Ubicacion"] = "planta",
        // "Mes recepción"/"Año recepción" son redundantes con
        // FechaRecepcion (se pueden derivar), no se guardan como columnas
        // separadas — no aparecen aquí, se ignoran si el archivo las trae.
        // Igual con cualquier columna de "Tiempo atención"/"Cumplimiento
        // SLA"/"Tiempo abierto": siempre se recalculan, nunca se leen.
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" (el que ya se mostraba en el texto de ayuda de
    // la pantalla) y un valor de ejemplo por columna. Tiempo de atención,
    // cumplimiento de SLA y tiempo abierto NO se incluyen: siempre se
    // recalculan en el servidor, no aparecen en "Columnas esperadas".
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Folio", "TKT-0001"),
        new("Fecha recepción", "24/09/2026"),
        new("Hora recepción", "08:30"),
        new("Fecha cierre", "25/09/2026"),
        new("Hora cierre", "17:00"),
        new("Solicitante", "Juan Pérez"),
        new("Responsable compras", "María López"),
        new("Tipo", "Compra"),
        new("Prioridad", "Alta"),
        new("Estatus", "Cerrado"),
        new("Solicitud", "Reposición de material de oficina"),
        new("SLA cierre (horas)", "24"),
        new("Observaciones", "Sin incidencias"),
        new("Planta", "Planta 1"),
    };

    private readonly ApplicationDbContext _contexto;

    public TiempoRespuestaResolucionService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Adm Tiempo Respuesta Resolucion", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    // Calcula (y persiste) TiempoAtencionHoras/TiempoAtencionDias/
    // CumplimientoSla una sola vez, en el momento en que se conoce
    // FechaCierre. Son horas de RELOJ (calendario), no horas hábiles: no
    // hay datos de turnos/festivos para calcular horas hábiles reales, así
    // que esto es una simplificación deliberada a comunicar al usuario.
    private static (decimal? TiempoAtencionHoras, decimal? TiempoAtencionDias, bool? CumplimientoSla) CalcularCamposCierre(
        DateTime fechaRecepcion, TimeSpan? horaRecepcion, DateTime? fechaCierre, TimeSpan? horaCierre, decimal? slaCierreHoras)
    {
        if (fechaCierre is null)
        {
            return (null, null, null);
        }

        var inicio = fechaRecepcion.Date + (horaRecepcion ?? TimeSpan.Zero);
        var fin = fechaCierre.Value.Date + (horaCierre ?? TimeSpan.Zero);
        var horas = (decimal)(fin - inicio).TotalHours;
        var dias = horas / 24m;
        bool? cumplimiento = slaCierreHoras.HasValue ? horas <= slaCierreHoras.Value : null;

        return (horas, dias, cumplimiento);
    }

    // NO se persiste: solo tiene sentido mientras el ticket sigue abierto y
    // cambia constantemente. Se calcula en vivo, solo para la lectura.
    private static decimal? CalcularTiempoAbiertoHoras(DateTime fechaRecepcion, TimeSpan? horaRecepcion, DateTime? fechaCierre)
    {
        if (fechaCierre is not null)
        {
            return null;
        }

        var inicio = fechaRecepcion.Date + (horaRecepcion ?? TimeSpan.Zero);
        return (decimal)(DateTime.Now - inicio).TotalHours;
    }

    public async Task<IEnumerable<TiempoRespuestaResolucionDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.TiemposRespuestaResolucion.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(t => t.AreaUbicacionId == areaUbicacionId.Value);
        }

        var registros = await consulta
            .OrderByDescending(t => t.FechaRecepcion)
            .ToListAsync();

        return registros.Select(MapearDto);
    }

    public async Task<TiempoRespuestaResolucionDto?> ObtenerRegistroPorIdAsync(int id)
    {
        var registro = await _contexto.TiemposRespuestaResolucion
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        return registro is null ? null : MapearDto(registro);
    }

    public async Task<TiempoRespuestaResolucionDto?> CrearRegistroAsync(TiempoRespuestaResolucionCrearDto dto)
    {
        var yaExiste = await _contexto.TiemposRespuestaResolucion.AnyAsync(t => t.Folio == dto.Folio);
        if (yaExiste)
        {
            return null;
        }

        var (tiempoAtencionHoras, tiempoAtencionDias, cumplimientoSla) = CalcularCamposCierre(
            dto.FechaRecepcion, dto.HoraRecepcion, dto.FechaCierre, dto.HoraCierre, dto.SlaCierreHoras);

        var registro = new TiempoRespuestaResolucion
        {
            Folio = dto.Folio,
            FechaRecepcion = dto.FechaRecepcion,
            HoraRecepcion = dto.HoraRecepcion,
            FechaCierre = dto.FechaCierre,
            HoraCierre = dto.HoraCierre,
            Solicitante = dto.Solicitante,
            ResponsableCompras = dto.ResponsableCompras,
            Tipo = dto.Tipo,
            Prioridad = dto.Prioridad,
            Estatus = dto.Estatus,
            Solicitud = dto.Solicitud,
            SlaCierreHoras = dto.SlaCierreHoras,
            Observaciones = dto.Observaciones,
            AreaUbicacionId = dto.AreaUbicacionId,
            TiempoAtencionHoras = tiempoAtencionHoras,
            TiempoAtencionDias = tiempoAtencionDias,
            CumplimientoSla = cumplimientoSla,
        };

        _contexto.TiemposRespuestaResolucion.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<TiempoRespuestaResolucionDto?> ActualizarRegistroAsync(int id, TiempoRespuestaResolucionActualizarDto dto)
    {
        var registro = await _contexto.TiemposRespuestaResolucion.FirstOrDefaultAsync(t => t.Id == id);
        if (registro is null)
        {
            return null;
        }

        var (tiempoAtencionHoras, tiempoAtencionDias, cumplimientoSla) = CalcularCamposCierre(
            dto.FechaRecepcion, dto.HoraRecepcion, dto.FechaCierre, dto.HoraCierre, dto.SlaCierreHoras);

        registro.FechaRecepcion = dto.FechaRecepcion;
        registro.HoraRecepcion = dto.HoraRecepcion;
        registro.FechaCierre = dto.FechaCierre;
        registro.HoraCierre = dto.HoraCierre;
        registro.Solicitante = dto.Solicitante;
        registro.ResponsableCompras = dto.ResponsableCompras;
        registro.Tipo = dto.Tipo;
        registro.Prioridad = dto.Prioridad;
        registro.Estatus = dto.Estatus;
        registro.Solicitud = dto.Solicitud;
        registro.SlaCierreHoras = dto.SlaCierreHoras;
        registro.Observaciones = dto.Observaciones;
        registro.AreaUbicacionId = dto.AreaUbicacionId;
        registro.TiempoAtencionHoras = tiempoAtencionHoras;
        registro.TiempoAtencionDias = tiempoAtencionDias;
        registro.CumplimientoSla = cumplimientoSla;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<TiempoRespuestaResolucionImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new TiempoRespuestaResolucionImportarResultadoDto();

        using var libro = new XLWorkbook(archivoExcel);
        var hoja = libro.Worksheets.First();
        var filaEncabezado = hoja.FirstRowUsed();
        if (filaEncabezado is null)
        {
            resultado.Errores.Add("El archivo está vacío.");
            return resultado;
        }

        var indicePorEncabezado = ExcelImportUtils.LeerIndicePorEncabezado(filaEncabezado);

        var ubicaciones = await _contexto.Ubicaciones
            .ToDictionaryAsync(u => ExcelImportUtils.Normalizar(u.Nombre), u => u.Id);

        var areaUbicacionesAdministracion = await _contexto.AreaUbicaciones
            .Where(au => au.AreaId == AREA_ID_ADMINISTRACION)
            .ToDictionaryAsync(au => au.UbicacionId, au => au.Id);

        var existentes = await _contexto.TiemposRespuestaResolucion
            .ToDictionaryAsync(t => t.Folio, t => t);

        void MapearCampos(TiempoRespuestaResolucion registro, ExcelFilaLectora lectora, int areaUbicacionId, DateTime fechaRecepcion)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.FechaRecepcion = fechaRecepcion;
            registro.HoraRecepcion = lectora.Hora("HoraRecepcion");
            registro.FechaCierre = lectora.Fecha("FechaCierre");
            registro.HoraCierre = lectora.Hora("HoraCierre");
            registro.Solicitante = lectora.Texto("Solicitante");
            registro.ResponsableCompras = lectora.Texto("ResponsableCompras");
            registro.Tipo = lectora.Texto("Tipo");
            registro.Prioridad = lectora.Texto("Prioridad");
            registro.Estatus = lectora.Texto("Estatus");
            registro.Solicitud = lectora.Texto("Solicitud");
            registro.SlaCierreHoras = lectora.Numero("SlaCierreHoras");
            registro.Observaciones = lectora.Texto("Observaciones");

            var (tiempoAtencionHoras, tiempoAtencionDias, cumplimientoSla) = CalcularCamposCierre(
                registro.FechaRecepcion, registro.HoraRecepcion, registro.FechaCierre, registro.HoraCierre, registro.SlaCierreHoras);
            registro.TiempoAtencionHoras = tiempoAtencionHoras;
            registro.TiempoAtencionDias = tiempoAtencionDias;
            registro.CumplimientoSla = cumplimientoSla;
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var folio = lectora.Texto("Folio");
            if (string.IsNullOrWhiteSpace(folio))
            {
                resultado.Errores.Add($"Fila {numeroFila}: no trae 'Folio', se omitió.");
                resultado.Omitidos++;
                continue;
            }

            var plantaTexto = lectora.Texto("Ubicacion");
            int? areaUbicacionId = null;
            if (plantaTexto is not null
                && ubicaciones.TryGetValue(ExcelImportUtils.Normalizar(plantaTexto), out var ubicacionId)
                && areaUbicacionesAdministracion.TryGetValue(ubicacionId, out var areaUbicacionEncontrada))
            {
                areaUbicacionId = areaUbicacionEncontrada;
            }

            if (areaUbicacionId is null)
            {
                resultado.Errores.Add(
                    $"Fila {numeroFila} (folio {folio}): la Planta '{plantaTexto}' no coincide con ninguna ubicación del catálogo de Administración, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            var fechaRecepcion = lectora.Fecha("FechaRecepcion");
            if (fechaRecepcion is null)
            {
                resultado.Errores.Add($"Fila {numeroFila} (folio {folio}): no trae Fecha de recepción, se omitió.");
                resultado.Omitidos++;
                continue;
            }

            if (existentes.TryGetValue(folio, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value, fechaRecepcion.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new TiempoRespuestaResolucion
                {
                    Folio = folio,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value, fechaRecepcion.Value);
                _contexto.TiemposRespuestaResolucion.Add(nuevo);
                existentes[folio] = nuevo;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static TiempoRespuestaResolucionDto MapearDto(TiempoRespuestaResolucion t) => new()
    {
        Id = t.Id,
        Folio = t.Folio,
        FechaRecepcion = t.FechaRecepcion,
        HoraRecepcion = t.HoraRecepcion,
        FechaCierre = t.FechaCierre,
        HoraCierre = t.HoraCierre,
        Solicitante = t.Solicitante,
        ResponsableCompras = t.ResponsableCompras,
        Tipo = t.Tipo,
        Prioridad = t.Prioridad,
        Estatus = t.Estatus,
        Solicitud = t.Solicitud,
        SlaCierreHoras = t.SlaCierreHoras,
        Observaciones = t.Observaciones,
        AreaUbicacionId = t.AreaUbicacionId,
        TiempoAtencionHoras = t.TiempoAtencionHoras,
        TiempoAtencionDias = t.TiempoAtencionDias,
        CumplimientoSla = t.CumplimientoSla,
        TiempoAbiertoHoras = CalcularTiempoAbiertoHoras(t.FechaRecepcion, t.HoraRecepcion, t.FechaCierre),
        FechaImportacion = t.FechaImportacion,
    };
}
