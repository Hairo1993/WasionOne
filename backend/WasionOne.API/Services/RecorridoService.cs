using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class RecorridoService : IRecorridoService
{
    // Fijo mientras solo exista Seguridad Patrimonial (AreaId = 1) en esta Área.
    private const int AREA_ID_SEGURIDAD_PATRIMONIAL = 1;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "id",
        ["Fecha"] = "fecha",
        ["Operador"] = "operadorguardia",
        ["Ubicacion"] = "planta",
        ["AreaTipo"] = "areatipo",
        ["Estado"] = "estado",
        ["HoraInicio"] = "horainicio",
        ["HoraFin"] = "horafin",
        ["DuracionMinutos"] = "duracion",
        ["Hallazgos"] = "hallazgosnotas",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("ID", "REC-0001"),
        new("Fecha", "24/09/2026"),
        new("Operador / Guardia", "Carlos Ramírez"),
        new("Planta", "Planta 1"),
        new("Área / Tipo", "Perímetro externo"),
        new("Estado", "Completado"),
        new("Hora Inicio", "08:00"),
        new("Hora Fin", "08:30"),
        new("Duración", "30"),
        new("Hallazgos / Notas", "Sin hallazgos"),
    };

    private readonly ApplicationDbContext _contexto;

    public RecorridoService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Seg Recorridos", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    // Si no viene una duración explícita pero sí ambas horas, se calcula.
    private static int? CalcularDuracion(int? duracionExplicita, TimeSpan? inicio, TimeSpan? fin)
    {
        if (duracionExplicita.HasValue)
        {
            return duracionExplicita;
        }

        if (inicio.HasValue && fin.HasValue)
        {
            var minutos = (fin.Value - inicio.Value).TotalMinutes;
            return minutos >= 0 ? (int)minutos : null;
        }

        return null;
    }

    public async Task<IEnumerable<RecorridoDto>> ObtenerRecorridosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.Recorridos.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(r => r.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(r => r.Fecha)
            .ThenByDescending(r => r.HoraInicio)
            .Select(r => new RecorridoDto
            {
                Id = r.Id,
                IdOrigen = r.IdOrigen,
                AreaUbicacionId = r.AreaUbicacionId,
                Fecha = r.Fecha,
                Operador = r.Operador,
                AreaTipo = r.AreaTipo,
                Estado = r.Estado,
                HoraInicio = r.HoraInicio,
                HoraFin = r.HoraFin,
                DuracionMinutos = r.DuracionMinutos,
                Hallazgos = r.Hallazgos,
                FechaImportacion = r.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<RecorridoDto?> ObtenerRecorridoPorIdAsync(int id)
    {
        return await _contexto.Recorridos
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new RecorridoDto
            {
                Id = r.Id,
                IdOrigen = r.IdOrigen,
                AreaUbicacionId = r.AreaUbicacionId,
                Fecha = r.Fecha,
                Operador = r.Operador,
                AreaTipo = r.AreaTipo,
                Estado = r.Estado,
                HoraInicio = r.HoraInicio,
                HoraFin = r.HoraFin,
                DuracionMinutos = r.DuracionMinutos,
                Hallazgos = r.Hallazgos,
                FechaImportacion = r.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<RecorridoDto> CrearRecorridoAsync(RecorridoCrearDto dto)
    {
        var recorrido = new Recorrido
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Fecha = dto.Fecha,
            Operador = dto.Operador,
            AreaTipo = dto.AreaTipo,
            Estado = string.IsNullOrWhiteSpace(dto.Estado) ? "Completado" : dto.Estado,
            HoraInicio = dto.HoraInicio,
            HoraFin = dto.HoraFin,
            DuracionMinutos = CalcularDuracion(dto.DuracionMinutos, dto.HoraInicio, dto.HoraFin),
            Hallazgos = dto.Hallazgos,
        };

        _contexto.Recorridos.Add(recorrido);
        await _contexto.SaveChangesAsync();

        return MapearDto(recorrido);
    }

    public async Task<RecorridoDto?> ActualizarRecorridoAsync(int id, RecorridoActualizarDto dto)
    {
        var recorrido = await _contexto.Recorridos.FirstOrDefaultAsync(r => r.Id == id);
        if (recorrido is null)
        {
            return null;
        }

        recorrido.Estado = dto.Estado;
        recorrido.HoraInicio = dto.HoraInicio;
        recorrido.HoraFin = dto.HoraFin;
        recorrido.DuracionMinutos = CalcularDuracion(dto.DuracionMinutos, dto.HoraInicio, dto.HoraFin);
        recorrido.Hallazgos = dto.Hallazgos;

        await _contexto.SaveChangesAsync();

        return MapearDto(recorrido);
    }

    public async Task<RecorridoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new RecorridoImportarResultadoDto();

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

        var existentes = await _contexto.Recorridos
            .Where(r => r.IdOrigen != null)
            .ToDictionaryAsync(r => r.IdOrigen!, r => r);

        void MapearCampos(Recorrido recorrido, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            recorrido.AreaUbicacionId = areaUbicacionId;
            recorrido.Fecha = lectora.Fecha("Fecha") ?? recorrido.Fecha;
            recorrido.Operador = lectora.Texto("Operador");
            recorrido.AreaTipo = lectora.Texto("AreaTipo");
            recorrido.Estado = lectora.Texto("Estado") ?? "Completado";
            recorrido.HoraInicio = lectora.Hora("HoraInicio");
            recorrido.HoraFin = lectora.Hora("HoraFin");
            recorrido.DuracionMinutos = CalcularDuracion(
                lectora.Entero("DuracionMinutos"), recorrido.HoraInicio, recorrido.HoraFin);
            recorrido.Hallazgos = lectora.Texto("Hallazgos");
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

            var idOrigen = lectora.Texto("IdOrigen");
            if (idOrigen is not null && existentes.TryGetValue(idOrigen, out var recorridoExistente))
            {
                MapearCampos(recorridoExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new Recorrido
                {
                    IdOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.Recorridos.Add(nuevo);
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

    private static RecorridoDto MapearDto(Recorrido r) => new()
    {
        Id = r.Id,
        IdOrigen = r.IdOrigen,
        AreaUbicacionId = r.AreaUbicacionId,
        Fecha = r.Fecha,
        Operador = r.Operador,
        AreaTipo = r.AreaTipo,
        Estado = r.Estado,
        HoraInicio = r.HoraInicio,
        HoraFin = r.HoraFin,
        DuracionMinutos = r.DuracionMinutos,
        Hallazgos = r.Hallazgos,
        FechaImportacion = r.FechaImportacion,
    };
}
