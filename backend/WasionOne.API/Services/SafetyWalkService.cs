using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class SafetyWalkService : ISafetyWalkService
{

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Fecha"] = "fecha",
        ["Ubicacion"] = "planta",
        ["Area"] = "area",
        ["Cumplimiento"] = "cumplimiento",
        ["AsistioGerenteCalidad"] = "asistiogerentedecalidad",
        ["AsistioGerenteProduccion"] = "asistiogerentedeproduccion",
        ["AsistioGerenteLogistica"] = "asistiogerentedelogistica",
        ["AsistioGerenteSoporteTecnico"] = "asistiogerentedesoportetecnico",
        ["AsistioGerenteProyectos"] = "asistiogerentedeproyectos",
        ["AsistioGerenteCompras"] = "asistiogerentedecompras",
        ["AsistioGerenteRh"] = "asistiogerentederh",
        ["AsistioCoordinadorCsh"] = "asistiocoordinadorcsh",
        ["AsistioSecretario"] = "asistiosecretario",
        ["AsistioVocal1"] = "asistiovocal1",
        ["AsistioVocal2"] = "asistiovocal2",
        ["AsistioVocal3"] = "asistiovocal3",
        ["AsistioVocal4"] = "asistiovocal4",
        ["AsistioVocal5"] = "asistiovocal5",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" (etiquetas del Comité, igual que
    // SAFETY_WALK_ASISTENTES en el frontend) y un valor de ejemplo por
    // columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Fecha", "24/09/2026"),
        new("Planta", "Planta 1"),
        new("Área", "Línea de producción 1"),
        new("Cumplimiento (%)", "90"),
        new("Gerente de Calidad", "Sí"),
        new("Gerente de Producción", "Sí"),
        new("Gerente de Logística", "No"),
        new("Gerente de Soporte Técnico", "Sí"),
        new("Gerente de Proyectos", "No"),
        new("Gerente de Compras", "Sí"),
        new("Gerente de RH", "Sí"),
        new("Coordinador CSH", "Sí"),
        new("Secretario", "Sí"),
        new("Vocal 1", "Sí"),
        new("Vocal 2", "No"),
        new("Vocal 3", "Sí"),
        new("Vocal 4", "Sí"),
        new("Vocal 5", "No"),
    };

    private readonly ApplicationDbContext _contexto;

    public SafetyWalkService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("SegHig Safety Walks", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<SafetyWalkDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.SafetyWalks.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(s => s.AreaUbicacionId == areaUbicacionId.Value);
        }

        var registros = await consulta
            .OrderByDescending(s => s.Fecha)
            .ToListAsync();

        return registros.Select(MapearDto);
    }

    public async Task<SafetyWalkDto?> ObtenerRegistroPorIdAsync(int id)
    {
        var registro = await _contexto.SafetyWalks
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        return registro is null ? null : MapearDto(registro);
    }

    public async Task<SafetyWalkDto?> CrearRegistroAsync(SafetyWalkCrearDto dto)
    {
        var yaExiste = await _contexto.SafetyWalks
            .AnyAsync(s => s.Fecha == dto.Fecha && s.AreaUbicacionId == dto.AreaUbicacionId);
        if (yaExiste)
        {
            return null;
        }

        var registro = new SafetyWalk
        {
            Fecha = dto.Fecha,
            Area = dto.Area,
            AreaUbicacionId = dto.AreaUbicacionId,
            Cumplimiento = dto.Cumplimiento,
            AsistioGerenteCalidad = dto.AsistioGerenteCalidad,
            AsistioGerenteProduccion = dto.AsistioGerenteProduccion,
            AsistioGerenteLogistica = dto.AsistioGerenteLogistica,
            AsistioGerenteSoporteTecnico = dto.AsistioGerenteSoporteTecnico,
            AsistioGerenteProyectos = dto.AsistioGerenteProyectos,
            AsistioGerenteCompras = dto.AsistioGerenteCompras,
            AsistioGerenteRh = dto.AsistioGerenteRh,
            AsistioCoordinadorCsh = dto.AsistioCoordinadorCsh,
            AsistioSecretario = dto.AsistioSecretario,
            AsistioVocal1 = dto.AsistioVocal1,
            AsistioVocal2 = dto.AsistioVocal2,
            AsistioVocal3 = dto.AsistioVocal3,
            AsistioVocal4 = dto.AsistioVocal4,
            AsistioVocal5 = dto.AsistioVocal5,
        };

        _contexto.SafetyWalks.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<SafetyWalkDto?> ActualizarRegistroAsync(int id, SafetyWalkActualizarDto dto)
    {
        var registro = await _contexto.SafetyWalks.FirstOrDefaultAsync(s => s.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Area = dto.Area;
        registro.Cumplimiento = dto.Cumplimiento;
        registro.AsistioGerenteCalidad = dto.AsistioGerenteCalidad;
        registro.AsistioGerenteProduccion = dto.AsistioGerenteProduccion;
        registro.AsistioGerenteLogistica = dto.AsistioGerenteLogistica;
        registro.AsistioGerenteSoporteTecnico = dto.AsistioGerenteSoporteTecnico;
        registro.AsistioGerenteProyectos = dto.AsistioGerenteProyectos;
        registro.AsistioGerenteCompras = dto.AsistioGerenteCompras;
        registro.AsistioGerenteRh = dto.AsistioGerenteRh;
        registro.AsistioCoordinadorCsh = dto.AsistioCoordinadorCsh;
        registro.AsistioSecretario = dto.AsistioSecretario;
        registro.AsistioVocal1 = dto.AsistioVocal1;
        registro.AsistioVocal2 = dto.AsistioVocal2;
        registro.AsistioVocal3 = dto.AsistioVocal3;
        registro.AsistioVocal4 = dto.AsistioVocal4;
        registro.AsistioVocal5 = dto.AsistioVocal5;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<SafetyWalkImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new SafetyWalkImportarResultadoDto();

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

        // Llave compuesta (Fecha exacta, Planta) — no hay Folio en este módulo.
        var existentes = await _contexto.SafetyWalks
            .ToDictionaryAsync(s => (s.Fecha, s.AreaUbicacionId), s => s);

        void MapearCampos(SafetyWalk registro, ExcelFilaLectora lectora)
        {
            registro.Area = lectora.Texto("Area");
            registro.Cumplimiento = lectora.Numero("Cumplimiento") ?? registro.Cumplimiento;
            registro.AsistioGerenteCalidad = lectora.Booleano("AsistioGerenteCalidad");
            registro.AsistioGerenteProduccion = lectora.Booleano("AsistioGerenteProduccion");
            registro.AsistioGerenteLogistica = lectora.Booleano("AsistioGerenteLogistica");
            registro.AsistioGerenteSoporteTecnico = lectora.Booleano("AsistioGerenteSoporteTecnico");
            registro.AsistioGerenteProyectos = lectora.Booleano("AsistioGerenteProyectos");
            registro.AsistioGerenteCompras = lectora.Booleano("AsistioGerenteCompras");
            registro.AsistioGerenteRh = lectora.Booleano("AsistioGerenteRh");
            registro.AsistioCoordinadorCsh = lectora.Booleano("AsistioCoordinadorCsh");
            registro.AsistioSecretario = lectora.Booleano("AsistioSecretario");
            registro.AsistioVocal1 = lectora.Booleano("AsistioVocal1");
            registro.AsistioVocal2 = lectora.Booleano("AsistioVocal2");
            registro.AsistioVocal3 = lectora.Booleano("AsistioVocal3");
            registro.AsistioVocal4 = lectora.Booleano("AsistioVocal4");
            registro.AsistioVocal5 = lectora.Booleano("AsistioVocal5");
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var fecha = lectora.Fecha("Fecha");
            if (fecha is null)
            {
                resultado.Errores.Add($"Fila {numeroFila}: no trae 'Fecha', se omitió.");
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

            var llave = (fecha.Value, areaUbicacionId.Value);
            if (existentes.TryGetValue(llave, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new SafetyWalk
                {
                    Fecha = fecha.Value,
                    AreaUbicacionId = areaUbicacionId.Value,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora);
                _contexto.SafetyWalks.Add(nuevo);
                existentes[llave] = nuevo;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static SafetyWalkDto MapearDto(SafetyWalk s) => new()
    {
        Id = s.Id,
        Fecha = s.Fecha,
        Area = s.Area,
        AreaUbicacionId = s.AreaUbicacionId,
        Cumplimiento = s.Cumplimiento,
        AsistioGerenteCalidad = s.AsistioGerenteCalidad,
        AsistioGerenteProduccion = s.AsistioGerenteProduccion,
        AsistioGerenteLogistica = s.AsistioGerenteLogistica,
        AsistioGerenteSoporteTecnico = s.AsistioGerenteSoporteTecnico,
        AsistioGerenteProyectos = s.AsistioGerenteProyectos,
        AsistioGerenteCompras = s.AsistioGerenteCompras,
        AsistioGerenteRh = s.AsistioGerenteRh,
        AsistioCoordinadorCsh = s.AsistioCoordinadorCsh,
        AsistioSecretario = s.AsistioSecretario,
        AsistioVocal1 = s.AsistioVocal1,
        AsistioVocal2 = s.AsistioVocal2,
        AsistioVocal3 = s.AsistioVocal3,
        AsistioVocal4 = s.AsistioVocal4,
        AsistioVocal5 = s.AsistioVocal5,
        FechaImportacion = s.FechaImportacion,
    };
}
