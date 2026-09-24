using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class EstatusLegalPlantaService : IEstatusLegalPlantaService
{

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Planta"] = "planta",
        ["RequerimientoLegal"] = "requerimientolegal",
        ["Autoridad"] = "autoridad",
        ["Frecuencia"] = "frecuencia",
        ["UltimaFechaRealizacion"] = "ultimafechaderealizacion",
        ["Estatus"] = "estatus",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Planta", "Planta 1"),
        new("Requerimiento Legal", "Verificación de extintores"),
        new("Autoridad", "Protección Civil"),
        new("Frecuencia", "Anual"),
        new("Última Fecha de Realización", "24/09/2026"),
        new("Estatus", "Cumple"),
    };

    private readonly ApplicationDbContext _contexto;

    public EstatusLegalPlantaService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("SegHig Estatus Legal", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    private static string NormalizarRequerimiento(string requerimiento) => requerimiento.Trim();

    public async Task<IEnumerable<EstatusLegalPlantaDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.EstatusLegalesPlanta.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(e => e.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(e => e.UltimaFechaRealizacion)
            .ThenBy(e => e.RequerimientoLegal)
            .Select(e => new EstatusLegalPlantaDto
            {
                Id = e.Id,
                AreaUbicacionId = e.AreaUbicacionId,
                RequerimientoLegal = e.RequerimientoLegal,
                Autoridad = e.Autoridad,
                Frecuencia = e.Frecuencia,
                UltimaFechaRealizacion = e.UltimaFechaRealizacion,
                Estatus = e.Estatus,
                FechaImportacion = e.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<EstatusLegalPlantaDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.EstatusLegalesPlanta
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EstatusLegalPlantaDto
            {
                Id = e.Id,
                AreaUbicacionId = e.AreaUbicacionId,
                RequerimientoLegal = e.RequerimientoLegal,
                Autoridad = e.Autoridad,
                Frecuencia = e.Frecuencia,
                UltimaFechaRealizacion = e.UltimaFechaRealizacion,
                Estatus = e.Estatus,
                FechaImportacion = e.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EstatusLegalPlantaDto?> CrearRegistroAsync(EstatusLegalPlantaCrearDto dto)
    {
        var requerimiento = NormalizarRequerimiento(dto.RequerimientoLegal);
        var fecha = dto.UltimaFechaRealizacion.Date;

        var yaExiste = await _contexto.EstatusLegalesPlanta.AnyAsync(e =>
            e.AreaUbicacionId == dto.AreaUbicacionId && e.RequerimientoLegal == requerimiento && e.UltimaFechaRealizacion == fecha);
        if (yaExiste)
        {
            return null;
        }

        var registro = new EstatusLegalPlanta
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            RequerimientoLegal = requerimiento,
            Autoridad = dto.Autoridad,
            Frecuencia = dto.Frecuencia,
            UltimaFechaRealizacion = fecha,
            Estatus = dto.Estatus,
        };

        _contexto.EstatusLegalesPlanta.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<EstatusLegalPlantaDto?> ActualizarRegistroAsync(int id, EstatusLegalPlantaActualizarDto dto)
    {
        var registro = await _contexto.EstatusLegalesPlanta.FirstOrDefaultAsync(e => e.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Autoridad = dto.Autoridad;
        registro.Frecuencia = dto.Frecuencia;
        registro.Estatus = dto.Estatus;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<EstatusLegalPlantaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new EstatusLegalPlantaImportarResultadoDto();

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

        // Llave de deduplicación: Planta + Requerimiento legal + Última
        // fecha de realización (reimportar la misma combinación actualiza
        // el resto de campos en vez de duplicar).
        var existentes = await _contexto.EstatusLegalesPlanta.ToDictionaryAsync(
            e => (e.AreaUbicacionId, RequerimientoLegal: e.RequerimientoLegal.Trim().ToLowerInvariant(), e.UltimaFechaRealizacion.Date),
            e => e);

        void MapearCampos(EstatusLegalPlanta registro, ExcelFilaLectora lectora, int areaUbicacionId, string requerimiento, DateTime ultimaFecha)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.RequerimientoLegal = requerimiento;
            registro.UltimaFechaRealizacion = ultimaFecha.Date;
            registro.Autoridad = lectora.Texto("Autoridad") ?? string.Empty;
            registro.Frecuencia = lectora.Texto("Frecuencia") ?? string.Empty;
            registro.Estatus = lectora.Texto("Estatus") ?? string.Empty;
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

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
                    $"Fila {numeroFila}: la Planta '{plantaTexto}' no coincide con ninguna ubicación del catálogo de Seguridad e Higiene, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            var requerimientoTexto = lectora.Texto("RequerimientoLegal");
            var ultimaFecha = lectora.Fecha("UltimaFechaRealizacion");
            if (requerimientoTexto is null || ultimaFecha is null)
            {
                resultado.Errores.Add($"Fila {numeroFila}: falta Requerimiento legal o Última fecha de realización, se omitió.");
                resultado.Omitidos++;
                continue;
            }

            var requerimiento = NormalizarRequerimiento(requerimientoTexto);
            var llave = (areaUbicacionId.Value, RequerimientoLegal: requerimiento.ToLowerInvariant(), ultimaFecha.Value.Date);

            if (existentes.TryGetValue(llave, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value, requerimiento, ultimaFecha.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new EstatusLegalPlanta
                {
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value, requerimiento, ultimaFecha.Value);
                _contexto.EstatusLegalesPlanta.Add(nuevo);
                existentes[llave] = nuevo;

                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static EstatusLegalPlantaDto MapearDto(EstatusLegalPlanta e) => new()
    {
        Id = e.Id,
        AreaUbicacionId = e.AreaUbicacionId,
        RequerimientoLegal = e.RequerimientoLegal,
        Autoridad = e.Autoridad,
        Frecuencia = e.Frecuencia,
        UltimaFechaRealizacion = e.UltimaFechaRealizacion,
        Estatus = e.Estatus,
        FechaImportacion = e.FechaImportacion,
    };
}
