using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class PlaticaService : IPlaticaService
{
    // Nombre lógico -> encabezado normalizado esperado en el Excel de origen.
    // El origen no trae columna de ubicación: todo lo importado entra como
    // "toda la empresa" (AreaUbicacionId = null).
    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "id",
        ["FechaEnvio"] = "fechadeenvio",
        ["TemaPolitica"] = "temadelapolitica",
        ["ResponsableEnvio"] = "responsabledeenvio",
        ["MedioDifusion"] = "mediodedifusion",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba, con el encabezado "bonito" y un valor de
    // ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("ID", "PLAT-0001"),
        new("Fecha de Envío", "24/09/2026"),
        new("Tema de la Política", "Uso seguro de contraseñas"),
        new("Responsable de Envío", "Ana García"),
        new("Medio de Difusión", "Correo electrónico"),
    };

    private readonly ApplicationDbContext _contexto;

    public PlaticaService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("IT Pláticas", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<PlaticaDto>> ObtenerPlaticasAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.Platicas.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(p => p.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(p => p.FechaEnvio)
            .Select(p => new PlaticaDto
            {
                Id = p.Id,
                IdOrigen = p.IdOrigen,
                AreaUbicacionId = p.AreaUbicacionId,
                FechaEnvio = p.FechaEnvio,
                TemaPolitica = p.TemaPolitica,
                ResponsableEnvio = p.ResponsableEnvio,
                MedioDifusion = p.MedioDifusion,
                FechaImportacion = p.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<PlaticaDto?> ObtenerPlaticaPorIdAsync(int id)
    {
        return await _contexto.Platicas
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PlaticaDto
            {
                Id = p.Id,
                IdOrigen = p.IdOrigen,
                AreaUbicacionId = p.AreaUbicacionId,
                FechaEnvio = p.FechaEnvio,
                TemaPolitica = p.TemaPolitica,
                ResponsableEnvio = p.ResponsableEnvio,
                MedioDifusion = p.MedioDifusion,
                FechaImportacion = p.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PlaticaDto> CrearPlaticaAsync(PlaticaCrearDto dto)
    {
        var platica = new Platica
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            FechaEnvio = dto.FechaEnvio,
            TemaPolitica = dto.TemaPolitica,
            ResponsableEnvio = dto.ResponsableEnvio,
            MedioDifusion = dto.MedioDifusion,
        };

        _contexto.Platicas.Add(platica);
        await _contexto.SaveChangesAsync();

        return MapearDto(platica);
    }

    public async Task<PlaticaDto?> ActualizarPlaticaAsync(int id, PlaticaActualizarDto dto)
    {
        var platica = await _contexto.Platicas.FirstOrDefaultAsync(p => p.Id == id);
        if (platica is null)
        {
            return null;
        }

        platica.AreaUbicacionId = dto.AreaUbicacionId;
        platica.FechaEnvio = dto.FechaEnvio;
        platica.TemaPolitica = dto.TemaPolitica;
        platica.ResponsableEnvio = dto.ResponsableEnvio;
        platica.MedioDifusion = dto.MedioDifusion;

        await _contexto.SaveChangesAsync();

        return MapearDto(platica);
    }

    public async Task<PlaticaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new PlaticaImportarResultadoDto();

        if (plantasPermitidas is not null)
        {
            resultado.Errores.Add("Este módulo no maneja Plantas específicas (aplica a toda la empresa); un usuario con captura restringida por Planta no puede importar aquí. Quita la restricción de Planta para este módulo si necesitas que esta persona importe.");
            return resultado;
        }

        using var libro = new XLWorkbook(archivoExcel);
        var hoja = libro.Worksheets.First();
        var filaEncabezado = hoja.FirstRowUsed();
        if (filaEncabezado is null)
        {
            resultado.Errores.Add("El archivo está vacío.");
            return resultado;
        }

        var indicePorEncabezado = ExcelImportUtils.LeerIndicePorEncabezado(filaEncabezado);

        var existentes = await _contexto.Platicas
            .Where(p => p.IdOrigen != null)
            .ToDictionaryAsync(p => p.IdOrigen!, p => p);

        void MapearCampos(Platica platica, ExcelFilaLectora lectora)
        {
            // El origen no trae ubicación: siempre "toda la empresa".
            platica.AreaUbicacionId = null;
            platica.FechaEnvio = lectora.Fecha("FechaEnvio") ?? platica.FechaEnvio;
            platica.TemaPolitica = lectora.Texto("TemaPolitica") ?? string.Empty;
            platica.ResponsableEnvio = lectora.Texto("ResponsableEnvio");
            platica.MedioDifusion = lectora.Texto("MedioDifusion");
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

            if (existentes.TryGetValue(idOrigen, out var platicaExistente))
            {
                MapearCampos(platicaExistente, lectora);
                resultado.Actualizados++;
            }
            else
            {
                var nueva = new Platica
                {
                    IdOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nueva, lectora);
                _contexto.Platicas.Add(nueva);
                existentes[idOrigen] = nueva;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static PlaticaDto MapearDto(Platica p) => new()
    {
        Id = p.Id,
        IdOrigen = p.IdOrigen,
        AreaUbicacionId = p.AreaUbicacionId,
        FechaEnvio = p.FechaEnvio,
        TemaPolitica = p.TemaPolitica,
        ResponsableEnvio = p.ResponsableEnvio,
        MedioDifusion = p.MedioDifusion,
        FechaImportacion = p.FechaImportacion,
    };
}
