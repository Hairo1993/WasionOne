using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class DopingService : IDopingService
{
    // Fijo mientras solo exista Seguridad Patrimonial (AreaId = 1) en esta Área.
    private const int AREA_ID_SEGURIDAD_PATRIMONIAL = 1;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "id",
        ["Fecha"] = "fecha",
        ["Ubicacion"] = "planta",
        ["Turno"] = "turno",
        ["NoNomina"] = "nonomina",
        ["Nombre"] = "nombre",
        ["Area"] = "area",
        ["Resultado"] = "resultado",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba, con el encabezado "bonito" y un valor de
    // ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("ID", "DOP-0001"),
        new("Fecha", "24/09/2026"),
        new("Planta", "Planta 1"),
        new("Turno", "Turno 1"),
        new("No. Nómina", "12345"),
        new("Nombre", "Pedro Torres"),
        new("Área", "Producción"),
        new("Resultado", "Negativo"),
    };

    private readonly ApplicationDbContext _contexto;

    public DopingService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Seg Dopings", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<DopingDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.Dopings.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(d => d.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(d => d.Fecha)
            .Select(d => new DopingDto
            {
                Id = d.Id,
                IdOrigen = d.IdOrigen,
                AreaUbicacionId = d.AreaUbicacionId,
                Fecha = d.Fecha,
                Turno = d.Turno,
                NoNomina = d.NoNomina,
                Nombre = d.Nombre,
                Area = d.Area,
                Resultado = d.Resultado,
                FechaImportacion = d.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<DopingDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.Dopings
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DopingDto
            {
                Id = d.Id,
                IdOrigen = d.IdOrigen,
                AreaUbicacionId = d.AreaUbicacionId,
                Fecha = d.Fecha,
                Turno = d.Turno,
                NoNomina = d.NoNomina,
                Nombre = d.Nombre,
                Area = d.Area,
                Resultado = d.Resultado,
                FechaImportacion = d.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<DopingDto> CrearRegistroAsync(DopingCrearDto dto)
    {
        var registro = new Doping
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Fecha = dto.Fecha,
            Turno = dto.Turno,
            NoNomina = dto.NoNomina,
            Nombre = dto.Nombre,
            Area = dto.Area,
            Resultado = string.IsNullOrWhiteSpace(dto.Resultado) ? "Negativo" : dto.Resultado,
        };

        _contexto.Dopings.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<DopingDto?> ActualizarRegistroAsync(int id, DopingActualizarDto dto)
    {
        var registro = await _contexto.Dopings.FirstOrDefaultAsync(d => d.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Resultado = dto.Resultado;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<DopingImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new DopingImportarResultadoDto();

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

        var existentes = await _contexto.Dopings
            .Where(d => d.IdOrigen != null)
            .ToDictionaryAsync(d => d.IdOrigen!, d => d);

        void MapearCampos(Doping registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Fecha = lectora.Fecha("Fecha") ?? registro.Fecha;
            registro.Turno = lectora.Texto("Turno");
            registro.NoNomina = lectora.Texto("NoNomina");
            registro.Nombre = lectora.Texto("Nombre");
            registro.Area = lectora.Texto("Area");
            registro.Resultado = lectora.Texto("Resultado") ?? "Negativo";
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
                var nuevo = new Doping
                {
                    IdOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.Dopings.Add(nuevo);
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

    private static DopingDto MapearDto(Doping d) => new()
    {
        Id = d.Id,
        IdOrigen = d.IdOrigen,
        AreaUbicacionId = d.AreaUbicacionId,
        Fecha = d.Fecha,
        Turno = d.Turno,
        NoNomina = d.NoNomina,
        Nombre = d.Nombre,
        Area = d.Area,
        Resultado = d.Resultado,
        FechaImportacion = d.FechaImportacion,
    };
}
