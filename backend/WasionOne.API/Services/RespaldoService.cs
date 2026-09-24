using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class RespaldoService : IRespaldoService
{
    // Fijo mientras solo exista el piloto de IT (AreaId = 3).
    private const int AREA_ID_IT = 3;

    // Nombre lógico -> encabezado normalizado esperado en el Excel de origen.
    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "id",
        ["FechaRespaldo"] = "fechaderespaldo",
        ["SistemaAplicacion"] = "sistemaaplicacion",
        ["SoftwareUtilizado"] = "softwareutilizado",
        ["TipoRespaldo"] = "tipoderespaldo",
        ["Ubicacion"] = "ubicaciondelrespaldo",
        ["Responsable"] = "responsable",
        ["Estado"] = "estado",
        ["Observaciones"] = "observaciones",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba, con el encabezado "bonito" y un valor de
    // ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("ID", "RESP-0001"),
        new("Fecha de Respaldo", "24/09/2026"),
        new("Sistema / Aplicación", "ERP SAP"),
        new("Software Utilizado", "Veeam"),
        new("Tipo de Respaldo", "Completo"),
        new("Ubicación del Respaldo", "Planta 1"),
        new("Responsable", "Carlos Ruiz"),
        new("Estado", "Completado"),
        new("Observaciones", "Respaldo verificado correctamente"),
    };

    private readonly ApplicationDbContext _contexto;

    public RespaldoService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("IT Respaldos", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<RespaldoDto>> ObtenerRespaldosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.Respaldos.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(r => r.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(r => r.FechaRespaldo)
            .Select(r => new RespaldoDto
            {
                Id = r.Id,
                IdOrigen = r.IdOrigen,
                AreaUbicacionId = r.AreaUbicacionId,
                FechaRespaldo = r.FechaRespaldo,
                SistemaAplicacion = r.SistemaAplicacion,
                SoftwareUtilizado = r.SoftwareUtilizado,
                TipoRespaldo = r.TipoRespaldo,
                Responsable = r.Responsable,
                Estado = r.Estado,
                Observaciones = r.Observaciones,
                FechaImportacion = r.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<RespaldoDto?> ObtenerRespaldoPorIdAsync(int id)
    {
        return await _contexto.Respaldos
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new RespaldoDto
            {
                Id = r.Id,
                IdOrigen = r.IdOrigen,
                AreaUbicacionId = r.AreaUbicacionId,
                FechaRespaldo = r.FechaRespaldo,
                SistemaAplicacion = r.SistemaAplicacion,
                SoftwareUtilizado = r.SoftwareUtilizado,
                TipoRespaldo = r.TipoRespaldo,
                Responsable = r.Responsable,
                Estado = r.Estado,
                Observaciones = r.Observaciones,
                FechaImportacion = r.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<RespaldoDto> CrearRespaldoAsync(RespaldoCrearDto dto)
    {
        var respaldo = new Respaldo
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            FechaRespaldo = dto.FechaRespaldo,
            SistemaAplicacion = dto.SistemaAplicacion,
            SoftwareUtilizado = dto.SoftwareUtilizado,
            TipoRespaldo = dto.TipoRespaldo,
            Responsable = dto.Responsable,
            Estado = "Completado",
            Observaciones = dto.Observaciones,
        };

        _contexto.Respaldos.Add(respaldo);
        await _contexto.SaveChangesAsync();

        return MapearDto(respaldo);
    }

    public async Task<RespaldoDto?> ActualizarRespaldoAsync(int id, RespaldoActualizarDto dto)
    {
        var respaldo = await _contexto.Respaldos.FirstOrDefaultAsync(r => r.Id == id);
        if (respaldo is null)
        {
            return null;
        }

        respaldo.Estado = dto.Estado;
        respaldo.Observaciones = dto.Observaciones;

        await _contexto.SaveChangesAsync();

        return MapearDto(respaldo);
    }

    public async Task<RespaldoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new RespaldoImportarResultadoDto();

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

        var existentes = await _contexto.Respaldos
            .Where(r => r.IdOrigen != null)
            .ToDictionaryAsync(r => r.IdOrigen!, r => r);

        void MapearCampos(Respaldo respaldo, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            respaldo.AreaUbicacionId = areaUbicacionId;
            respaldo.FechaRespaldo = lectora.Fecha("FechaRespaldo") ?? respaldo.FechaRespaldo;
            respaldo.SistemaAplicacion = lectora.Texto("SistemaAplicacion") ?? string.Empty;
            respaldo.SoftwareUtilizado = lectora.Texto("SoftwareUtilizado");
            respaldo.TipoRespaldo = lectora.Texto("TipoRespaldo");
            respaldo.Responsable = lectora.Texto("Responsable");
            respaldo.Estado = lectora.Texto("Estado") ?? "Completado";
            respaldo.Observaciones = lectora.Texto("Observaciones");
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var idOrigen = lectora.Texto("IdOrigen");
            if (string.IsNullOrWhiteSpace(idOrigen))
            {
                resultado.Errores.Add($"Fila {numeroFila}: no trae 'ID', se omitió.");
                resultado.Omitidos++;
                continue;
            }

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
                    $"Fila {numeroFila} (id {idOrigen}): la Ubicación del Respaldo '{ubicacionTexto}' no coincide con ninguna ubicación del catálogo de IT, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            if (existentes.TryGetValue(idOrigen, out var respaldoExistente))
            {
                MapearCampos(respaldoExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new Respaldo
                {
                    IdOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.Respaldos.Add(nuevo);
                existentes[idOrigen] = nuevo;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static RespaldoDto MapearDto(Respaldo r) => new()
    {
        Id = r.Id,
        IdOrigen = r.IdOrigen,
        AreaUbicacionId = r.AreaUbicacionId,
        FechaRespaldo = r.FechaRespaldo,
        SistemaAplicacion = r.SistemaAplicacion,
        SoftwareUtilizado = r.SoftwareUtilizado,
        TipoRespaldo = r.TipoRespaldo,
        Responsable = r.Responsable,
        Estado = r.Estado,
        Observaciones = r.Observaciones,
        FechaImportacion = r.FechaImportacion,
    };
}
