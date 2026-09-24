using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class AccidenteTrabajoService : IAccidenteTrabajoService
{

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Folio"] = "folio",
        ["Ubicacion"] = "planta",
        ["Area"] = "area",
        ["FechaReporte"] = "fechareporte",
        ["FechaOcurrido"] = "fechaocurrido",
        ["Nombre"] = "nombre",
        ["Compania"] = "compania",
        ["TipoIncidenteAccidente"] = "tipodeincidenteaccidente",
        ["AccidenteConDiasIncapacidad"] = "accidentecondiasdeincapacidad",
        ["Dias"] = "dias",
        ["Lesion"] = "lesion",
        ["ParteLesionada"] = "partelesionada",
        ["CausaRaiz"] = "causaraiz",
        ["EstatusAccion1"] = "estatusaccion1",
        ["EstatusAccion2"] = "estatusaccion2",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba, con el encabezado "bonito" y un valor de
    // ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Folio", "ACC-0001"),
        new("Planta", "Planta 1"),
        new("Área", "Producción"),
        new("Fecha Reporte", "24/09/2026"),
        new("Fecha Ocurrido", "23/09/2026"),
        new("Nombre", "Miguel Ángel Torres"),
        new("Compañía", "Wasion"),
        new("Tipo de Incidente/Accidente", "Accidente de trabajo"),
        new("Accidente con Días de Incapacidad", "Sí"),
        new("Días", "3"),
        new("Lesión", "Corte/Laceración"),
        new("Parte Lesionada", "Manos"),
        new("Causa Raíz", "Falta de EPP"),
        new("Estatus Acción 1", "Abierto"),
        new("Estatus Acción 2", "Abierto"),
    };

    private readonly ApplicationDbContext _contexto;

    public AccidenteTrabajoService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Seg Hig Accidentes", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<AccidenteTrabajoDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.AccidentesTrabajo.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(a => a.AreaUbicacionId == areaUbicacionId.Value);
        }

        var registros = await consulta
            .OrderByDescending(a => a.FechaOcurrido)
            .ToListAsync();

        return registros.Select(MapearDto);
    }

    public async Task<AccidenteTrabajoDto?> ObtenerRegistroPorIdAsync(int id)
    {
        var registro = await _contexto.AccidentesTrabajo
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        return registro is null ? null : MapearDto(registro);
    }

    public async Task<AccidenteTrabajoDto?> CrearRegistroAsync(AccidenteTrabajoCrearDto dto)
    {
        var yaExiste = await _contexto.AccidentesTrabajo.AnyAsync(a => a.Folio == dto.Folio);
        if (yaExiste)
        {
            return null;
        }

        var registro = new AccidenteTrabajo
        {
            Folio = dto.Folio,
            AreaUbicacionId = dto.AreaUbicacionId,
            Area = dto.Area,
            FechaReporte = dto.FechaReporte,
            FechaOcurrido = dto.FechaOcurrido,
            Nombre = dto.Nombre,
            Compania = dto.Compania,
            TipoIncidenteAccidente = dto.TipoIncidenteAccidente,
            AccidenteConDiasIncapacidad = dto.AccidenteConDiasIncapacidad,
            Dias = dto.Dias,
            Lesion = dto.Lesion,
            ParteLesionada = dto.ParteLesionada,
            CausaRaiz = dto.CausaRaiz,
            EstatusAccion1 = string.IsNullOrWhiteSpace(dto.EstatusAccion1) ? "Abierto" : dto.EstatusAccion1,
            EstatusAccion2 = string.IsNullOrWhiteSpace(dto.EstatusAccion2) ? "Abierto" : dto.EstatusAccion2,
        };

        _contexto.AccidentesTrabajo.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<AccidenteTrabajoDto?> ActualizarRegistroAsync(int id, AccidenteTrabajoActualizarDto dto)
    {
        var registro = await _contexto.AccidentesTrabajo.FirstOrDefaultAsync(a => a.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.AreaUbicacionId = dto.AreaUbicacionId;
        registro.Area = dto.Area;
        registro.FechaReporte = dto.FechaReporte;
        registro.FechaOcurrido = dto.FechaOcurrido;
        registro.Nombre = dto.Nombre;
        registro.Compania = dto.Compania;
        registro.TipoIncidenteAccidente = dto.TipoIncidenteAccidente;
        registro.AccidenteConDiasIncapacidad = dto.AccidenteConDiasIncapacidad;
        registro.Dias = dto.Dias;
        registro.Lesion = dto.Lesion;
        registro.ParteLesionada = dto.ParteLesionada;
        registro.CausaRaiz = dto.CausaRaiz;
        registro.EstatusAccion1 = dto.EstatusAccion1;
        registro.EstatusAccion2 = dto.EstatusAccion2;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<AccidenteTrabajoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new AccidenteTrabajoImportarResultadoDto();

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

        var areaUbicacionesSeguridadHigiene = await _contexto.AreaUbicaciones
            .Where(au => au.AreaId == Modulos.AreaIdSeguridadHigiene)
            .ToDictionaryAsync(au => au.UbicacionId, au => au.Id);

        var existentes = await _contexto.AccidentesTrabajo
            .ToDictionaryAsync(a => a.Folio, a => a);

        void MapearCampos(AccidenteTrabajo registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Area = lectora.Texto("Area");
            registro.FechaReporte = lectora.Fecha("FechaReporte") ?? registro.FechaReporte;
            registro.FechaOcurrido = lectora.Fecha("FechaOcurrido") ?? registro.FechaOcurrido;
            registro.Nombre = lectora.Texto("Nombre") ?? registro.Nombre;
            registro.Compania = lectora.Texto("Compania") ?? registro.Compania;
            registro.TipoIncidenteAccidente = lectora.Texto("TipoIncidenteAccidente") ?? registro.TipoIncidenteAccidente;
            registro.AccidenteConDiasIncapacidad = lectora.Booleano("AccidenteConDiasIncapacidad");
            registro.Dias = lectora.Entero("Dias");
            registro.Lesion = lectora.Texto("Lesion") ?? registro.Lesion;
            registro.ParteLesionada = lectora.Texto("ParteLesionada") ?? registro.ParteLesionada;
            registro.CausaRaiz = lectora.Texto("CausaRaiz") ?? registro.CausaRaiz;
            registro.EstatusAccion1 = lectora.Texto("EstatusAccion1") ?? "Abierto";
            registro.EstatusAccion2 = lectora.Texto("EstatusAccion2") ?? "Abierto";
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

            var plantaTexto = lectora.Texto("Ubicacion");
            int? areaUbicacionId = null;
            if (plantaTexto is not null
                && ubicaciones.TryGetValue(ExcelImportUtils.Normalizar(plantaTexto), out var ubicacionId)
                && areaUbicacionesSeguridadHigiene.TryGetValue(ubicacionId, out var areaUbicacionEncontrada))
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

            if (existentes.TryGetValue(folio, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new AccidenteTrabajo
                {
                    Folio = folio,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.AccidentesTrabajo.Add(nuevo);
                existentes[folio] = nuevo;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static AccidenteTrabajoDto MapearDto(AccidenteTrabajo a) => new()
    {
        Id = a.Id,
        Folio = a.Folio,
        AreaUbicacionId = a.AreaUbicacionId,
        Area = a.Area,
        FechaReporte = a.FechaReporte,
        FechaOcurrido = a.FechaOcurrido,
        Nombre = a.Nombre,
        Compania = a.Compania,
        TipoIncidenteAccidente = a.TipoIncidenteAccidente,
        AccidenteConDiasIncapacidad = a.AccidenteConDiasIncapacidad,
        Dias = a.Dias,
        Lesion = a.Lesion,
        ParteLesionada = a.ParteLesionada,
        CausaRaiz = a.CausaRaiz,
        EstatusAccion1 = a.EstatusAccion1,
        EstatusAccion2 = a.EstatusAccion2,
        FechaImportacion = a.FechaImportacion,
    };
}
