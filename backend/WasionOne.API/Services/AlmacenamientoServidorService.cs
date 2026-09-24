using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class AlmacenamientoServidorService : IAlmacenamientoServidorService
{
    private const int AREA_ID_IT = 3;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "id",
        ["Fecha"] = "fecha",
        ["Hora"] = "hora",
        ["Ubicacion"] = "planta",
        ["Servidor"] = "servidor",
        ["Ip"] = "ip",
        ["Unidad"] = "unidad",
        ["CapacidadTotalGb"] = "capacidadtotalgb",
        ["EspacioUtilizadoGb"] = "espacioutilizadogb",
        ["EspacioDisponibleGb"] = "espaciodisponiblegb",
        ["AlmacenamientoUtilizadoPorcentaje"] = "almacenamientoutilizado",
        ["Umbral"] = "umbral",
        ["Estado"] = "estado",
        ["Responsable"] = "responsable",
        ["Observaciones"] = "observaciones",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" y un valor de ejemplo por columna. Incluye "ID"
    // (IdOrigen) aunque el texto de ayuda de la pantalla no lo liste
    // explícitamente entre "Columnas esperadas": el import sí lo lee como
    // llave opcional de deduplicación (igual que en Disponibilidad de
    // Red/Servidores), así que se agrega aquí por consistencia.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("ID", "1"),
        new("Fecha", "24/09/2026"),
        new("Hora", "08:30"),
        new("Planta", "Planta 1"),
        new("Servidor", "SRV-APP01"),
        new("IP", "10.0.0.10"),
        new("Unidad", "C:"),
        new("Capacidad total (GB)", "500"),
        new("Espacio utilizado (GB)", "350"),
        new("Espacio disponible (GB)", "150"),
        new("Almacenamiento utilizado (%)", "70"),
        new("Umbral", "80"),
        new("Estado", "Normal"),
        new("Responsable", "Juan Pérez"),
        new("Observaciones", "Sin incidencias"),
    };

    private readonly ApplicationDbContext _contexto;

    public AlmacenamientoServidorService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("IT Almacenamiento", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<AlmacenamientoServidorDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.AlmacenamientoServidores.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(r => r.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(r => r.Fecha)
            .ThenByDescending(r => r.Hora)
            .Select(r => new AlmacenamientoServidorDto
            {
                Id = r.Id,
                IdOrigen = r.IdOrigen,
                AreaUbicacionId = r.AreaUbicacionId,
                Fecha = r.Fecha,
                Hora = r.Hora,
                Servidor = r.Servidor,
                Ip = r.Ip,
                Unidad = r.Unidad,
                CapacidadTotalGb = r.CapacidadTotalGb,
                EspacioUtilizadoGb = r.EspacioUtilizadoGb,
                EspacioDisponibleGb = r.EspacioDisponibleGb,
                AlmacenamientoUtilizadoPorcentaje = r.AlmacenamientoUtilizadoPorcentaje,
                Umbral = r.Umbral,
                Estado = r.Estado,
                Responsable = r.Responsable,
                Observaciones = r.Observaciones,
                FechaImportacion = r.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<AlmacenamientoServidorDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.AlmacenamientoServidores
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new AlmacenamientoServidorDto
            {
                Id = r.Id,
                IdOrigen = r.IdOrigen,
                AreaUbicacionId = r.AreaUbicacionId,
                Fecha = r.Fecha,
                Hora = r.Hora,
                Servidor = r.Servidor,
                Ip = r.Ip,
                Unidad = r.Unidad,
                CapacidadTotalGb = r.CapacidadTotalGb,
                EspacioUtilizadoGb = r.EspacioUtilizadoGb,
                EspacioDisponibleGb = r.EspacioDisponibleGb,
                AlmacenamientoUtilizadoPorcentaje = r.AlmacenamientoUtilizadoPorcentaje,
                Umbral = r.Umbral,
                Estado = r.Estado,
                Responsable = r.Responsable,
                Observaciones = r.Observaciones,
                FechaImportacion = r.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AlmacenamientoServidorDto> CrearRegistroAsync(AlmacenamientoServidorCrearDto dto)
    {
        var registro = new AlmacenamientoServidor
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Fecha = dto.Fecha,
            Hora = dto.Hora,
            Servidor = dto.Servidor,
            Ip = dto.Ip,
            Unidad = dto.Unidad,
            CapacidadTotalGb = dto.CapacidadTotalGb,
            EspacioUtilizadoGb = dto.EspacioUtilizadoGb,
            EspacioDisponibleGb = dto.EspacioDisponibleGb,
            AlmacenamientoUtilizadoPorcentaje = dto.AlmacenamientoUtilizadoPorcentaje,
            Umbral = dto.Umbral,
            Estado = string.IsNullOrWhiteSpace(dto.Estado) ? "Normal" : dto.Estado,
            Responsable = dto.Responsable,
            Observaciones = dto.Observaciones,
        };

        _contexto.AlmacenamientoServidores.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<AlmacenamientoServidorDto?> ActualizarRegistroAsync(int id, AlmacenamientoServidorActualizarDto dto)
    {
        var registro = await _contexto.AlmacenamientoServidores.FirstOrDefaultAsync(r => r.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.EspacioUtilizadoGb = dto.EspacioUtilizadoGb;
        registro.EspacioDisponibleGb = dto.EspacioDisponibleGb;
        registro.AlmacenamientoUtilizadoPorcentaje = dto.AlmacenamientoUtilizadoPorcentaje;
        registro.Umbral = dto.Umbral;
        registro.Estado = dto.Estado;
        registro.Responsable = dto.Responsable;
        registro.Observaciones = dto.Observaciones;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<AlmacenamientoServidorImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new AlmacenamientoServidorImportarResultadoDto();

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

        var areaUbicacionesIt = await _contexto.AreaUbicaciones
            .Where(au => au.AreaId == AREA_ID_IT)
            .ToDictionaryAsync(au => au.UbicacionId, au => au.Id);

        var existentes = await _contexto.AlmacenamientoServidores
            .Where(r => r.IdOrigen != null)
            .ToDictionaryAsync(r => r.IdOrigen!, r => r);

        void MapearCampos(AlmacenamientoServidor registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Fecha = lectora.Fecha("Fecha") ?? registro.Fecha;
            registro.Hora = lectora.Hora("Hora") ?? registro.Hora;
            registro.Servidor = lectora.Texto("Servidor") ?? registro.Servidor;
            registro.Ip = lectora.Texto("Ip");
            registro.Unidad = lectora.Texto("Unidad");
            registro.CapacidadTotalGb = lectora.Numero("CapacidadTotalGb");
            registro.EspacioUtilizadoGb = lectora.Numero("EspacioUtilizadoGb");
            registro.EspacioDisponibleGb = lectora.Numero("EspacioDisponibleGb");
            registro.AlmacenamientoUtilizadoPorcentaje = lectora.Numero("AlmacenamientoUtilizadoPorcentaje") ?? registro.AlmacenamientoUtilizadoPorcentaje;
            registro.Umbral = lectora.Numero("Umbral");
            registro.Estado = lectora.Texto("Estado") ?? "Normal";
            registro.Responsable = lectora.Texto("Responsable");
            registro.Observaciones = lectora.Texto("Observaciones");
        }

        var numeroFila = 1;
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var ubicacionTexto = lectora.Texto("Ubicacion");
            int? areaUbicacionId = null;
            if (ubicacionTexto is not null
                && ubicaciones.TryGetValue(ExcelImportUtils.Normalizar(ubicacionTexto), out var ubicacionId)
                && areaUbicacionesIt.TryGetValue(ubicacionId, out var areaUbicacionEncontrada))
            {
                areaUbicacionId = areaUbicacionEncontrada;
            }

            if (areaUbicacionId is null)
            {
                resultado.Errores.Add(
                    $"Fila {numeroFila}: la Planta '{ubicacionTexto}' no coincide con ninguna ubicación del catálogo de IT, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            var idOrigen = lectora.Texto("IdOrigen");
            if (idOrigen is not null && existentes.TryGetValue(idOrigen, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new AlmacenamientoServidor
                {
                    IdOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.AlmacenamientoServidores.Add(nuevo);
                if (idOrigen is not null)
                {
                    existentes[idOrigen] = nuevo;
                }

                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static AlmacenamientoServidorDto MapearDto(AlmacenamientoServidor r) => new()
    {
        Id = r.Id,
        IdOrigen = r.IdOrigen,
        AreaUbicacionId = r.AreaUbicacionId,
        Fecha = r.Fecha,
        Hora = r.Hora,
        Servidor = r.Servidor,
        Ip = r.Ip,
        Unidad = r.Unidad,
        CapacidadTotalGb = r.CapacidadTotalGb,
        EspacioUtilizadoGb = r.EspacioUtilizadoGb,
        EspacioDisponibleGb = r.EspacioDisponibleGb,
        AlmacenamientoUtilizadoPorcentaje = r.AlmacenamientoUtilizadoPorcentaje,
        Umbral = r.Umbral,
        Estado = r.Estado,
        Responsable = r.Responsable,
        Observaciones = r.Observaciones,
        FechaImportacion = r.FechaImportacion,
    };
}
