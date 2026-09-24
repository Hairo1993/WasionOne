using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class TestConsignaService : ITestConsignaService
{
    // Fijo mientras solo exista Seguridad Patrimonial (AreaId = 1) en esta Área.
    private const int AREA_ID_SEGURIDAD_PATRIMONIAL = 1;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "no",
        ["Fecha"] = "fecha",
        ["Ubicacion"] = "planta",
        ["ResultadoTest"] = "resultadodetest",
        ["AreaInvolucrada"] = "areainvolucrada",
        ["ProcedimientoInvolucrado"] = "procedimientoinvolucrado",
        ["Proveedor"] = "proveedor",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("No.", "TC-0001"),
        new("Fecha", "24/09/2026"),
        new("Planta", "Planta 1"),
        new("Resultado de Test", "Aprobado"),
        new("Área Involucrada", "Acceso principal"),
        new("Procedimiento Involucrado", "Protocolo de evacuación"),
        new("Proveedor", "Proveedor Ejemplo S.A."),
    };

    private readonly ApplicationDbContext _contexto;

    public TestConsignaService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Seg Test Consignas", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<TestConsignaDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.TestConsignas.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(t => t.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(t => t.Fecha)
            .Select(t => new TestConsignaDto
            {
                Id = t.Id,
                IdOrigen = t.IdOrigen,
                Fecha = t.Fecha,
                AreaUbicacionId = t.AreaUbicacionId,
                ResultadoTest = t.ResultadoTest,
                AreaInvolucrada = t.AreaInvolucrada,
                ProcedimientoInvolucrado = t.ProcedimientoInvolucrado,
                Proveedor = t.Proveedor,
                FechaImportacion = t.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<TestConsignaDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.TestConsignas
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TestConsignaDto
            {
                Id = t.Id,
                IdOrigen = t.IdOrigen,
                Fecha = t.Fecha,
                AreaUbicacionId = t.AreaUbicacionId,
                ResultadoTest = t.ResultadoTest,
                AreaInvolucrada = t.AreaInvolucrada,
                ProcedimientoInvolucrado = t.ProcedimientoInvolucrado,
                Proveedor = t.Proveedor,
                FechaImportacion = t.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TestConsignaDto> CrearRegistroAsync(TestConsignaCrearDto dto)
    {
        var registro = new TestConsigna
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Fecha = dto.Fecha,
            ResultadoTest = string.IsNullOrWhiteSpace(dto.ResultadoTest) ? "Aprobado" : dto.ResultadoTest,
            AreaInvolucrada = dto.AreaInvolucrada,
            ProcedimientoInvolucrado = dto.ProcedimientoInvolucrado,
            Proveedor = dto.Proveedor,
        };

        _contexto.TestConsignas.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<TestConsignaDto?> ActualizarRegistroAsync(int id, TestConsignaActualizarDto dto)
    {
        var registro = await _contexto.TestConsignas.FirstOrDefaultAsync(t => t.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.ResultadoTest = dto.ResultadoTest;
        registro.AreaInvolucrada = dto.AreaInvolucrada;
        registro.ProcedimientoInvolucrado = dto.ProcedimientoInvolucrado;
        registro.Proveedor = dto.Proveedor;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<TestConsignaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new TestConsignaImportarResultadoDto();

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

        var existentes = await _contexto.TestConsignas
            .Where(t => t.IdOrigen != null)
            .ToDictionaryAsync(t => t.IdOrigen!, t => t);

        void MapearCampos(TestConsigna registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Fecha = lectora.Fecha("Fecha") ?? registro.Fecha;
            registro.ResultadoTest = lectora.Texto("ResultadoTest") ?? "Aprobado";
            registro.AreaInvolucrada = lectora.Texto("AreaInvolucrada");
            registro.ProcedimientoInvolucrado = lectora.Texto("ProcedimientoInvolucrado");
            registro.Proveedor = lectora.Texto("Proveedor");
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
            if (idOrigen is not null && existentes.TryGetValue(idOrigen, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new TestConsigna
                {
                    IdOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.TestConsignas.Add(nuevo);
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

    private static TestConsignaDto MapearDto(TestConsigna t) => new()
    {
        Id = t.Id,
        IdOrigen = t.IdOrigen,
        Fecha = t.Fecha,
        AreaUbicacionId = t.AreaUbicacionId,
        ResultadoTest = t.ResultadoTest,
        AreaInvolucrada = t.AreaInvolucrada,
        ProcedimientoInvolucrado = t.ProcedimientoInvolucrado,
        Proveedor = t.Proveedor,
        FechaImportacion = t.FechaImportacion,
    };
}
