using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class DisponibilidadRedService : IDisponibilidadRedService
{
    private const int AREA_ID_IT = 3;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "id",
        ["Fecha"] = "fecha",
        ["Hora"] = "hora",
        ["Ubicacion"] = "planta",
        ["Dispositivo"] = "dispositivo",
        ["Ip"] = "ip",
        ["TipoDispositivo"] = "tipodedispositivo",
        ["Estado"] = "estado",
        ["LatenciaMs"] = "latenciams",
        ["PerdidaPaquetesPorcentaje"] = "perdidadepaquetes",
        ["TiempoCaidaMin"] = "tiempocaidamin",
        ["DisponibilidadPorcentaje"] = "disponibilidad",
        ["Responsable"] = "responsable",
        ["Observaciones"] = "observaciones",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con un
    // encabezado "bonito" y un valor de ejemplo por columna. Incluye "ID"
    // (IdOrigen): el texto de ayuda de la pantalla lo menciona aparte como
    // llave opcional de deduplicación en reimportaciones.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("ID", "1"),
        new("Fecha", "24/09/2026"),
        new("Hora", "08:30"),
        new("Planta", "Planta 1"),
        new("Dispositivo", "Switch Core 1"),
        new("IP", "10.0.1.1"),
        new("Tipo de dispositivo", "Switch"),
        new("Estado", "Disponible"),
        new("Latencia (ms)", "12"),
        new("Pérdida de paquetes (%)", "0"),
        new("Tiempo caída (min)", "0"),
        new("Disponibilidad (%)", "99.9"),
        new("Responsable", "Juan Pérez"),
        new("Observaciones", "Sin incidencias"),
    };

    private readonly ApplicationDbContext _contexto;

    public DisponibilidadRedService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("IT Disponibilidad Red", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<DisponibilidadRedDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.DisponibilidadRedes.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(r => r.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(r => r.Fecha)
            .ThenByDescending(r => r.Hora)
            .Select(r => new DisponibilidadRedDto
            {
                Id = r.Id,
                IdOrigen = r.IdOrigen,
                AreaUbicacionId = r.AreaUbicacionId,
                Fecha = r.Fecha,
                Hora = r.Hora,
                Dispositivo = r.Dispositivo,
                Ip = r.Ip,
                TipoDispositivo = r.TipoDispositivo,
                Estado = r.Estado,
                LatenciaMs = r.LatenciaMs,
                PerdidaPaquetesPorcentaje = r.PerdidaPaquetesPorcentaje,
                TiempoCaidaMin = r.TiempoCaidaMin,
                DisponibilidadPorcentaje = r.DisponibilidadPorcentaje,
                Responsable = r.Responsable,
                Observaciones = r.Observaciones,
                FechaImportacion = r.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<DisponibilidadRedDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.DisponibilidadRedes
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new DisponibilidadRedDto
            {
                Id = r.Id,
                IdOrigen = r.IdOrigen,
                AreaUbicacionId = r.AreaUbicacionId,
                Fecha = r.Fecha,
                Hora = r.Hora,
                Dispositivo = r.Dispositivo,
                Ip = r.Ip,
                TipoDispositivo = r.TipoDispositivo,
                Estado = r.Estado,
                LatenciaMs = r.LatenciaMs,
                PerdidaPaquetesPorcentaje = r.PerdidaPaquetesPorcentaje,
                TiempoCaidaMin = r.TiempoCaidaMin,
                DisponibilidadPorcentaje = r.DisponibilidadPorcentaje,
                Responsable = r.Responsable,
                Observaciones = r.Observaciones,
                FechaImportacion = r.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<DisponibilidadRedDto> CrearRegistroAsync(DisponibilidadRedCrearDto dto)
    {
        var registro = new DisponibilidadRed
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Fecha = dto.Fecha,
            Hora = dto.Hora,
            Dispositivo = dto.Dispositivo,
            Ip = dto.Ip,
            TipoDispositivo = dto.TipoDispositivo,
            Estado = string.IsNullOrWhiteSpace(dto.Estado) ? "Disponible" : dto.Estado,
            LatenciaMs = dto.LatenciaMs,
            PerdidaPaquetesPorcentaje = dto.PerdidaPaquetesPorcentaje,
            TiempoCaidaMin = dto.TiempoCaidaMin,
            DisponibilidadPorcentaje = dto.DisponibilidadPorcentaje,
            Responsable = dto.Responsable,
            Observaciones = dto.Observaciones,
        };

        _contexto.DisponibilidadRedes.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<DisponibilidadRedDto?> ActualizarRegistroAsync(int id, DisponibilidadRedActualizarDto dto)
    {
        var registro = await _contexto.DisponibilidadRedes.FirstOrDefaultAsync(r => r.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Estado = dto.Estado;
        registro.LatenciaMs = dto.LatenciaMs;
        registro.PerdidaPaquetesPorcentaje = dto.PerdidaPaquetesPorcentaje;
        registro.TiempoCaidaMin = dto.TiempoCaidaMin;
        registro.DisponibilidadPorcentaje = dto.DisponibilidadPorcentaje;
        registro.Responsable = dto.Responsable;
        registro.Observaciones = dto.Observaciones;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<DisponibilidadRedImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new DisponibilidadRedImportarResultadoDto();

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

        var existentes = await _contexto.DisponibilidadRedes
            .Where(r => r.IdOrigen != null)
            .ToDictionaryAsync(r => r.IdOrigen!, r => r);

        void MapearCampos(DisponibilidadRed registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Fecha = lectora.Fecha("Fecha") ?? registro.Fecha;
            registro.Hora = lectora.Hora("Hora") ?? registro.Hora;
            registro.Dispositivo = lectora.Texto("Dispositivo") ?? registro.Dispositivo;
            registro.Ip = lectora.Texto("Ip");
            registro.TipoDispositivo = lectora.Texto("TipoDispositivo");
            registro.Estado = lectora.Texto("Estado") ?? "Disponible";
            registro.LatenciaMs = lectora.Numero("LatenciaMs");
            registro.PerdidaPaquetesPorcentaje = lectora.Numero("PerdidaPaquetesPorcentaje");
            registro.TiempoCaidaMin = lectora.Entero("TiempoCaidaMin");
            registro.DisponibilidadPorcentaje = lectora.Numero("DisponibilidadPorcentaje") ?? registro.DisponibilidadPorcentaje;
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
                var nuevo = new DisponibilidadRed
                {
                    IdOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.DisponibilidadRedes.Add(nuevo);
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

    private static DisponibilidadRedDto MapearDto(DisponibilidadRed r) => new()
    {
        Id = r.Id,
        IdOrigen = r.IdOrigen,
        AreaUbicacionId = r.AreaUbicacionId,
        Fecha = r.Fecha,
        Hora = r.Hora,
        Dispositivo = r.Dispositivo,
        Ip = r.Ip,
        TipoDispositivo = r.TipoDispositivo,
        Estado = r.Estado,
        LatenciaMs = r.LatenciaMs,
        PerdidaPaquetesPorcentaje = r.PerdidaPaquetesPorcentaje,
        TiempoCaidaMin = r.TiempoCaidaMin,
        DisponibilidadPorcentaje = r.DisponibilidadPorcentaje,
        Responsable = r.Responsable,
        Observaciones = r.Observaciones,
        FechaImportacion = r.FechaImportacion,
    };
}
