using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class EvaluacionVigilanciaService : IEvaluacionVigilanciaService
{
    // Fijo mientras solo exista Seguridad Patrimonial (AreaId = 1) en esta Área.
    private const int AREA_ID_SEGURIDAD_PATRIMONIAL = 1;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Fecha"] = "fecha",
        ["Proveedor"] = "proveedor",
        ["Ubicacion"] = "planta",
        ["Cobertura"] = "cobertura",
        ["Expedientes"] = "expedientes",
        ["Uniformidad"] = "uniformidad",
        ["Reuniones"] = "reuniones",
        ["Equipamiento"] = "equipamiento",
        ["Supervision"] = "supervision",
        ["CambiosSolicitados"] = "cambiossolicitados",
        ["Procedimientos"] = "procedimientos",
        ["Capacitacion"] = "capacitacion",
        ["Acuerdos"] = "acuerdos",
        ["Observaciones"] = "observaciones",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba, con el encabezado "bonito" y un valor de
    // ejemplo por columna. No incluye "Total": el sistema siempre lo
    // calcula, nunca se captura ni se acepta del Excel.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Fecha", "24/09/2026"),
        new("Proveedor", "Vigilancia Total S.A."),
        new("Planta", "Planta 1"),
        new("Cobertura", "9"),
        new("Expedientes", "8"),
        new("Uniformidad", "10"),
        new("Reuniones", "9"),
        new("Equipamiento", "8"),
        new("Supervisión", "9"),
        new("Cambios solicitados", "7"),
        new("Procedimientos", "9"),
        new("Capacitación", "8"),
        new("Acuerdos", "9"),
        new("Observaciones", "Cumplimiento satisfactorio en el mes"),
    };

    private readonly ApplicationDbContext _contexto;

    public EvaluacionVigilanciaService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Seg Eval Vigilancia", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    private static string NormalizarProveedor(string proveedor) => proveedor.Trim();

    // Total siempre se calcula aquí — nunca se acepta del cliente ni del
    // Excel de origen, aunque el archivo traiga su propia columna Total.
    private static decimal CalcularTotal(
        decimal? cobertura,
        decimal? expedientes,
        decimal? uniformidad,
        decimal? reuniones,
        decimal? equipamiento,
        decimal? supervision,
        decimal? cambiosSolicitados,
        decimal? procedimientos,
        decimal? capacitacion,
        decimal? acuerdos)
    {
        return (cobertura ?? 0)
            + (expedientes ?? 0)
            + (uniformidad ?? 0)
            + (reuniones ?? 0)
            + (equipamiento ?? 0)
            + (supervision ?? 0)
            + (cambiosSolicitados ?? 0)
            + (procedimientos ?? 0)
            + (capacitacion ?? 0)
            + (acuerdos ?? 0);
    }

    public async Task<IEnumerable<EvaluacionVigilanciaDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.EvaluacionesVigilancia.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(e => e.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(e => e.Fecha)
            .ThenBy(e => e.Proveedor)
            .Select(e => new EvaluacionVigilanciaDto
            {
                Id = e.Id,
                Fecha = e.Fecha,
                Proveedor = e.Proveedor,
                AreaUbicacionId = e.AreaUbicacionId,
                Cobertura = e.Cobertura,
                Expedientes = e.Expedientes,
                Uniformidad = e.Uniformidad,
                Reuniones = e.Reuniones,
                Equipamiento = e.Equipamiento,
                Supervision = e.Supervision,
                CambiosSolicitados = e.CambiosSolicitados,
                Procedimientos = e.Procedimientos,
                Capacitacion = e.Capacitacion,
                Acuerdos = e.Acuerdos,
                Total = e.Total,
                Observaciones = e.Observaciones,
                FechaImportacion = e.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<EvaluacionVigilanciaDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.EvaluacionesVigilancia
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EvaluacionVigilanciaDto
            {
                Id = e.Id,
                Fecha = e.Fecha,
                Proveedor = e.Proveedor,
                AreaUbicacionId = e.AreaUbicacionId,
                Cobertura = e.Cobertura,
                Expedientes = e.Expedientes,
                Uniformidad = e.Uniformidad,
                Reuniones = e.Reuniones,
                Equipamiento = e.Equipamiento,
                Supervision = e.Supervision,
                CambiosSolicitados = e.CambiosSolicitados,
                Procedimientos = e.Procedimientos,
                Capacitacion = e.Capacitacion,
                Acuerdos = e.Acuerdos,
                Total = e.Total,
                Observaciones = e.Observaciones,
                FechaImportacion = e.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EvaluacionVigilanciaDto?> CrearRegistroAsync(EvaluacionVigilanciaCrearDto dto)
    {
        var proveedor = NormalizarProveedor(dto.Proveedor);

        var yaExiste = await _contexto.EvaluacionesVigilancia.AnyAsync(e =>
            e.AreaUbicacionId == dto.AreaUbicacionId && e.Fecha == dto.Fecha && e.Proveedor == proveedor);
        if (yaExiste)
        {
            return null;
        }

        var registro = new EvaluacionVigilancia
        {
            Fecha = dto.Fecha,
            Proveedor = proveedor,
            AreaUbicacionId = dto.AreaUbicacionId,
            Cobertura = dto.Cobertura,
            Expedientes = dto.Expedientes,
            Uniformidad = dto.Uniformidad,
            Reuniones = dto.Reuniones,
            Equipamiento = dto.Equipamiento,
            Supervision = dto.Supervision,
            CambiosSolicitados = dto.CambiosSolicitados,
            Procedimientos = dto.Procedimientos,
            Capacitacion = dto.Capacitacion,
            Acuerdos = dto.Acuerdos,
            Observaciones = dto.Observaciones,
            Total = CalcularTotal(
                dto.Cobertura, dto.Expedientes, dto.Uniformidad, dto.Reuniones, dto.Equipamiento,
                dto.Supervision, dto.CambiosSolicitados, dto.Procedimientos, dto.Capacitacion, dto.Acuerdos),
        };

        _contexto.EvaluacionesVigilancia.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<EvaluacionVigilanciaDto?> ActualizarRegistroAsync(int id, EvaluacionVigilanciaActualizarDto dto)
    {
        var registro = await _contexto.EvaluacionesVigilancia.FirstOrDefaultAsync(e => e.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Cobertura = dto.Cobertura;
        registro.Expedientes = dto.Expedientes;
        registro.Uniformidad = dto.Uniformidad;
        registro.Reuniones = dto.Reuniones;
        registro.Equipamiento = dto.Equipamiento;
        registro.Supervision = dto.Supervision;
        registro.CambiosSolicitados = dto.CambiosSolicitados;
        registro.Procedimientos = dto.Procedimientos;
        registro.Capacitacion = dto.Capacitacion;
        registro.Acuerdos = dto.Acuerdos;
        registro.Observaciones = dto.Observaciones;
        registro.Total = CalcularTotal(
            dto.Cobertura, dto.Expedientes, dto.Uniformidad, dto.Reuniones, dto.Equipamiento,
            dto.Supervision, dto.CambiosSolicitados, dto.Procedimientos, dto.Capacitacion, dto.Acuerdos);

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<EvaluacionVigilanciaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new EvaluacionVigilanciaImportarResultadoDto();

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

        var areaUbicacionesSeguridad = await _contexto.AreaUbicaciones
            .Where(au => au.AreaId == AREA_ID_SEGURIDAD_PATRIMONIAL)
            .ToDictionaryAsync(au => au.UbicacionId, au => au.Id);

        // Llave de deduplicación: no hay un "ID" de origen en el Excel, así
        // que la combinación Planta + Proveedor + Fecha hace las veces de
        // llave natural (reimportar el mismo mes/proveedor actualiza en vez
        // de duplicar) — mismo criterio que Alcoholimetría.
        var existentes = await _contexto.EvaluacionesVigilancia.ToDictionaryAsync(
            e => (e.AreaUbicacionId, e.Fecha.Date, Proveedor: e.Proveedor.Trim().ToLowerInvariant()),
            e => e);

        void MapearCampos(EvaluacionVigilancia registro, ExcelFilaLectora lectora, int areaUbicacionId, DateTime fecha, string proveedor)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Fecha = fecha.Date;
            registro.Proveedor = proveedor;
            registro.Cobertura = lectora.Numero("Cobertura");
            registro.Expedientes = lectora.Numero("Expedientes");
            registro.Uniformidad = lectora.Numero("Uniformidad");
            registro.Reuniones = lectora.Numero("Reuniones");
            registro.Equipamiento = lectora.Numero("Equipamiento");
            registro.Supervision = lectora.Numero("Supervision");
            registro.CambiosSolicitados = lectora.Numero("CambiosSolicitados");
            registro.Procedimientos = lectora.Numero("Procedimientos");
            registro.Capacitacion = lectora.Numero("Capacitacion");
            registro.Acuerdos = lectora.Numero("Acuerdos");
            registro.Observaciones = lectora.Texto("Observaciones");
            registro.Total = CalcularTotal(
                registro.Cobertura, registro.Expedientes, registro.Uniformidad, registro.Reuniones,
                registro.Equipamiento, registro.Supervision, registro.CambiosSolicitados,
                registro.Procedimientos, registro.Capacitacion, registro.Acuerdos);
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var ubicacionTexto = lectora.Texto("Ubicacion");
            int? areaUbicacionId = null;
            if (ubicacionTexto is not null
                && ubicaciones.TryGetValue(ExcelImportUtils.Normalizar(ubicacionTexto), out var ubicacionId)
                && areaUbicacionesSeguridad.TryGetValue(ubicacionId, out var areaUbicacionEncontrada))
            {
                areaUbicacionId = areaUbicacionEncontrada;
            }

            if (areaUbicacionId is null)
            {
                resultado.Errores.Add(
                    $"Fila {numeroFila}: la Planta '{ubicacionTexto}' no coincide con ninguna ubicación del catálogo de Seguridad Patrimonial, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            var fecha = lectora.Fecha("Fecha");
            var proveedorTexto = lectora.Texto("Proveedor");
            if (fecha is null || proveedorTexto is null)
            {
                resultado.Errores.Add($"Fila {numeroFila}: falta Fecha o Proveedor, se omitió.");
                resultado.Omitidos++;
                continue;
            }

            var proveedor = NormalizarProveedor(proveedorTexto);
            var llave = (areaUbicacionId.Value, fecha.Value.Date, Proveedor: proveedor.ToLowerInvariant());

            if (existentes.TryGetValue(llave, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value, fecha.Value, proveedor);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new EvaluacionVigilancia
                {
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value, fecha.Value, proveedor);
                _contexto.EvaluacionesVigilancia.Add(nuevo);
                existentes[llave] = nuevo;

                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static EvaluacionVigilanciaDto MapearDto(EvaluacionVigilancia e) => new()
    {
        Id = e.Id,
        Fecha = e.Fecha,
        Proveedor = e.Proveedor,
        AreaUbicacionId = e.AreaUbicacionId,
        Cobertura = e.Cobertura,
        Expedientes = e.Expedientes,
        Uniformidad = e.Uniformidad,
        Reuniones = e.Reuniones,
        Equipamiento = e.Equipamiento,
        Supervision = e.Supervision,
        CambiosSolicitados = e.CambiosSolicitados,
        Procedimientos = e.Procedimientos,
        Capacitacion = e.Capacitacion,
        Acuerdos = e.Acuerdos,
        Total = e.Total,
        Observaciones = e.Observaciones,
        FechaImportacion = e.FechaImportacion,
    };
}
