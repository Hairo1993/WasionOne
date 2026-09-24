using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

// PROPUESTO — pendiente de confirmar con el usuario (ver comentario en
// Models/Brigada.cs).
public class BrigadaService : IBrigadaService
{

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Folio"] = "folio",
        ["Fecha"] = "fecha",
        ["Planta"] = "planta",
        ["TipoBrigada"] = "tipodebrigada",
        ["NombreBrigadista"] = "nombredelbrigadista",
        ["PuestoBrigada"] = "puestoenlabrigada",
        ["Estado"] = "estado",
        ["FechaUltimaCapacitacion"] = "fechadeultimacapacitacion",
        ["Observaciones"] = "observaciones",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Folio", "BRG-0001"),
        new("Fecha", "24/09/2026"),
        new("Planta", "Planta 1"),
        new("Tipo de Brigada", "Evacuación"),
        new("Nombre del Brigadista", "Juan Pérez"),
        new("Puesto en la Brigada", "Brigadista"),
        new("Estado", "Activo"),
        new("Fecha Última Capacitación", "24/09/2026"),
        new("Observaciones", "Curso de reinducción pendiente"),
    };

    private readonly ApplicationDbContext _contexto;

    public BrigadaService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("SegHig Brigadas", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    private static string NormalizarFolio(string folio) => folio.Trim();

    public async Task<IEnumerable<BrigadaDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.Brigadas.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(b => b.AreaUbicacionId == areaUbicacionId.Value);
        }

        var registros = await consulta
            .OrderByDescending(b => b.Fecha)
            .ThenBy(b => b.NombreBrigadista)
            .ToListAsync();

        return registros.Select(MapearDto);
    }

    public async Task<BrigadaDto?> ObtenerRegistroPorIdAsync(int id)
    {
        var registro = await _contexto.Brigadas
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        return registro is null ? null : MapearDto(registro);
    }

    public async Task<BrigadaDto?> CrearRegistroAsync(BrigadaCrearDto dto)
    {
        var folio = NormalizarFolio(dto.Folio);

        var yaExiste = await _contexto.Brigadas.AnyAsync(b => b.Folio == folio);
        if (yaExiste)
        {
            return null;
        }

        var registro = new Brigada
        {
            Folio = folio,
            Fecha = dto.Fecha,
            AreaUbicacionId = dto.AreaUbicacionId,
            TipoBrigada = dto.TipoBrigada,
            NombreBrigadista = dto.NombreBrigadista,
            PuestoBrigada = dto.PuestoBrigada,
            Estado = dto.Estado,
            FechaUltimaCapacitacion = dto.FechaUltimaCapacitacion,
            Observaciones = dto.Observaciones,
        };

        _contexto.Brigadas.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<BrigadaDto?> ActualizarRegistroAsync(int id, BrigadaActualizarDto dto)
    {
        var registro = await _contexto.Brigadas.FirstOrDefaultAsync(b => b.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Fecha = dto.Fecha;
        registro.AreaUbicacionId = dto.AreaUbicacionId;
        registro.TipoBrigada = dto.TipoBrigada;
        registro.NombreBrigadista = dto.NombreBrigadista;
        registro.PuestoBrigada = dto.PuestoBrigada;
        registro.Estado = dto.Estado;
        registro.FechaUltimaCapacitacion = dto.FechaUltimaCapacitacion;
        registro.Observaciones = dto.Observaciones;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<BrigadaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new BrigadaImportarResultadoDto();

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

        var areaUbicacionesSegHigiene = await _contexto.AreaUbicaciones
            .Where(au => au.AreaId == Modulos.AreaIdSeguridadHigiene)
            .ToDictionaryAsync(au => au.UbicacionId, au => au.Id);

        var existentes = await _contexto.Brigadas
            .ToDictionaryAsync(b => b.Folio, b => b);

        void MapearCampos(Brigada registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Fecha = lectora.Fecha("Fecha") ?? registro.Fecha;
            registro.TipoBrigada = lectora.Texto("TipoBrigada") ?? string.Empty;
            registro.NombreBrigadista = lectora.Texto("NombreBrigadista") ?? string.Empty;
            registro.PuestoBrigada = lectora.Texto("PuestoBrigada") ?? string.Empty;
            registro.Estado = lectora.Texto("Estado") ?? "Activo";
            registro.FechaUltimaCapacitacion = lectora.Fecha("FechaUltimaCapacitacion");
            registro.Observaciones = lectora.Texto("Observaciones");
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

            var plantaTexto = lectora.Texto("Planta");
            int? areaUbicacionId = null;
            if (plantaTexto is not null
                && ubicaciones.TryGetValue(ExcelImportUtils.Normalizar(plantaTexto), out var ubicacionId)
                && areaUbicacionesSegHigiene.TryGetValue(ubicacionId, out var areaUbicacionEncontrada))
            {
                areaUbicacionId = areaUbicacionEncontrada;
            }

            if (areaUbicacionId is null)
            {
                resultado.Errores.Add(
                    $"Fila {numeroFila} (Folio {folio}): la Planta '{plantaTexto}' no coincide con ninguna ubicación del catálogo de Seguridad e Higiene, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            var folioNormalizado = NormalizarFolio(folio);

            if (existentes.TryGetValue(folioNormalizado, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new Brigada
                {
                    Folio = folioNormalizado,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.Brigadas.Add(nuevo);
                existentes[folioNormalizado] = nuevo;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static BrigadaDto MapearDto(Brigada b) => new()
    {
        Id = b.Id,
        Folio = b.Folio,
        Fecha = b.Fecha,
        AreaUbicacionId = b.AreaUbicacionId,
        TipoBrigada = b.TipoBrigada,
        NombreBrigadista = b.NombreBrigadista,
        PuestoBrigada = b.PuestoBrigada,
        Estado = b.Estado,
        FechaUltimaCapacitacion = b.FechaUltimaCapacitacion,
        Observaciones = b.Observaciones,
        FechaImportacion = b.FechaImportacion,
    };
}
