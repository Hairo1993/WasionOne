using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class CumplimientoProgramaService : ICumplimientoProgramaService
{
    // Fijo mientras solo exista Administración (AreaId de Administración)
    // en esta Área.
    // TODO AJUSTAR: -1 es un valor temporal. Reemplazar por el AreaId real
    // de la Área "Administración" (consultar GET /api/catalogos/areas o la
    // pantalla /admin/catalogos) antes de usar este módulo en producción.
    private const int AREA_ID_ADMINISTRACION = 11; // Area "Administracion" (AreaId real confirmado 22/sep/2026)

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "id",
        ["AreaEvaluada"] = "areaevaluada",
        ["Ubicacion"] = "casaplanta",
        ["FechaReporte"] = "fechareporte",
        ["Hallazgo"] = "hallazgoobservacion",
        ["Seguimiento"] = "seguimientoaccionrealizada",
        ["FechaCierre"] = "fechacierre",
        ["Estatus"] = "estatus",
        ["Prioridad"] = "prioridad",
        ["Responsable"] = "responsable",
        ["Tipo"] = "tipo",
        ["CumplimientoGeneralPorcentaje"] = "cumplimientogeneralporcentaje",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" (el que ya se mostraba en el texto de ayuda de
    // la pantalla) y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("ID", "1001"),
        new("Área evaluada", "Seguridad"),
        new("Casa/Planta", "Planta 1"),
        new("Fecha reporte", "24/09/2026"),
        new("Hallazgo / observación", "Falta de señalización en pasillo"),
        new("Seguimiento / acción realizada", "Se instaló señalización"),
        new("Fecha cierre", "30/09/2026"),
        new("Estatus", "Cerrado"),
        new("Prioridad", "Alta"),
        new("Responsable", "Juan Pérez"),
        new("tipo", "Auditoría"),
        new("Cumplimiento general (%)", "85"),
    };

    private readonly ApplicationDbContext _contexto;

    public CumplimientoProgramaService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Adm Cumplimiento Programa", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<CumplimientoProgramaDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.CumplimientosPrograma.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(c => c.AreaUbicacionId == areaUbicacionId.Value);
        }

        var registros = await consulta
            .OrderByDescending(c => c.FechaReporte)
            .ToListAsync();

        return registros.Select(MapearDto);
    }

    public async Task<CumplimientoProgramaDto?> ObtenerRegistroPorIdAsync(int id)
    {
        var registro = await _contexto.CumplimientosPrograma
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return registro is null ? null : MapearDto(registro);
    }

    public async Task<CumplimientoProgramaDto> CrearRegistroAsync(CumplimientoProgramaCrearDto dto)
    {
        var registro = new CumplimientoPrograma
        {
            AreaEvaluada = dto.AreaEvaluada,
            AreaUbicacionId = dto.AreaUbicacionId,
            FechaReporte = dto.FechaReporte,
            Hallazgo = dto.Hallazgo,
            Seguimiento = dto.Seguimiento,
            FechaCierre = dto.FechaCierre,
            Estatus = dto.Estatus,
            Prioridad = dto.Prioridad,
            Responsable = dto.Responsable,
            Tipo = dto.Tipo,
            CumplimientoGeneralPorcentaje = dto.CumplimientoGeneralPorcentaje,
        };

        _contexto.CumplimientosPrograma.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<CumplimientoProgramaDto?> ActualizarRegistroAsync(int id, CumplimientoProgramaActualizarDto dto)
    {
        var registro = await _contexto.CumplimientosPrograma.FirstOrDefaultAsync(c => c.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.AreaEvaluada = dto.AreaEvaluada;
        registro.AreaUbicacionId = dto.AreaUbicacionId;
        registro.FechaReporte = dto.FechaReporte;
        registro.Hallazgo = dto.Hallazgo;
        registro.Seguimiento = dto.Seguimiento;
        registro.FechaCierre = dto.FechaCierre;
        registro.Estatus = dto.Estatus;
        registro.Prioridad = dto.Prioridad;
        registro.Responsable = dto.Responsable;
        registro.Tipo = dto.Tipo;
        registro.CumplimientoGeneralPorcentaje = dto.CumplimientoGeneralPorcentaje;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<CumplimientoProgramaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new CumplimientoProgramaImportarResultadoDto();

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

        var areaUbicacionesAdministracion = await _contexto.AreaUbicaciones
            .Where(au => au.AreaId == AREA_ID_ADMINISTRACION)
            .ToDictionaryAsync(au => au.UbicacionId, au => au.Id);

        var existentes = await _contexto.CumplimientosPrograma
            .Where(c => c.IdOrigen != null)
            .ToDictionaryAsync(c => c.IdOrigen!.Value, c => c);

        void MapearCampos(CumplimientoPrograma registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.AreaEvaluada = lectora.Texto("AreaEvaluada");
            registro.FechaReporte = lectora.Fecha("FechaReporte") ?? registro.FechaReporte;
            registro.Hallazgo = lectora.Texto("Hallazgo");
            registro.Seguimiento = lectora.Texto("Seguimiento");
            registro.FechaCierre = lectora.Fecha("FechaCierre");
            registro.Estatus = lectora.Texto("Estatus");
            registro.Prioridad = lectora.Texto("Prioridad");
            registro.Responsable = lectora.Texto("Responsable");
            registro.Tipo = lectora.Texto("Tipo");
            registro.CumplimientoGeneralPorcentaje = lectora.Numero("CumplimientoGeneralPorcentaje");
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var plantaTexto = lectora.Texto("Ubicacion");
            int? areaUbicacionId = null;
            if (plantaTexto is not null
                && ubicaciones.TryGetValue(ExcelImportUtils.Normalizar(plantaTexto), out var ubicacionId)
                && areaUbicacionesAdministracion.TryGetValue(ubicacionId, out var areaUbicacionEncontrada))
            {
                areaUbicacionId = areaUbicacionEncontrada;
            }

            if (areaUbicacionId is null)
            {
                resultado.Errores.Add(
                    $"Fila {numeroFila}: la Planta '{plantaTexto}' no coincide con ninguna ubicación del catálogo de Administración, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            var idOrigen = lectora.Entero("IdOrigen");
            if (idOrigen is not null && existentes.TryGetValue(idOrigen.Value, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new CumplimientoPrograma
                {
                    IdOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.CumplimientosPrograma.Add(nuevo);
                if (idOrigen is not null)
                {
                    existentes[idOrigen.Value] = nuevo;
                }

                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static CumplimientoProgramaDto MapearDto(CumplimientoPrograma c) => new()
    {
        Id = c.Id,
        IdOrigen = c.IdOrigen,
        AreaEvaluada = c.AreaEvaluada,
        AreaUbicacionId = c.AreaUbicacionId,
        FechaReporte = c.FechaReporte,
        Hallazgo = c.Hallazgo,
        Seguimiento = c.Seguimiento,
        FechaCierre = c.FechaCierre,
        Estatus = c.Estatus,
        Prioridad = c.Prioridad,
        Responsable = c.Responsable,
        Tipo = c.Tipo,
        CumplimientoGeneralPorcentaje = c.CumplimientoGeneralPorcentaje,
        FechaImportacion = c.FechaImportacion,
    };
}
