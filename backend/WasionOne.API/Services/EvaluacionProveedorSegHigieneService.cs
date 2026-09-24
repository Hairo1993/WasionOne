using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class EvaluacionProveedorSegHigieneService : IEvaluacionProveedorSegHigieneService
{

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Proveedor"] = "proveedor",
        ["Especialidad"] = "especialidad",
        ["Planta"] = "planta",
        ["Mes"] = "mes",
        ["Kpi"] = "kpi",
        ["Cumplimiento"] = "cumplimiento",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Proveedor", "Proveedor Ejemplo S.A."),
        new("Especialidad", "Seguridad Industrial"),
        new("Planta", "Planta 1"),
        new("Mes", "01/09/2026"),
        new("Kpi", "90"),
        new("Cumplimiento (%)", "85"),
    };

    private readonly ApplicationDbContext _contexto;

    public EvaluacionProveedorSegHigieneService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("SegHig Eval Proveedores", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    private static string NormalizarProveedor(string proveedor) => proveedor.Trim();

    // Primer día del mes evaluado, para que el mismo mes siempre normalice
    // a la misma fecha sin importar el día que traiga el Excel/formulario.
    private static DateTime PrimerDiaDelMes(DateTime fecha) => new(fecha.Year, fecha.Month, 1);

    public async Task<IEnumerable<EvaluacionProveedorSegHigieneDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.EvaluacionesProveedorSegHigiene.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(e => e.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(e => e.Mes)
            .ThenBy(e => e.Proveedor)
            .Select(e => new EvaluacionProveedorSegHigieneDto
            {
                Id = e.Id,
                Proveedor = e.Proveedor,
                Especialidad = e.Especialidad,
                AreaUbicacionId = e.AreaUbicacionId,
                Mes = e.Mes,
                Kpi = e.Kpi,
                Cumplimiento = e.Cumplimiento,
                FechaImportacion = e.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<EvaluacionProveedorSegHigieneDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.EvaluacionesProveedorSegHigiene
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EvaluacionProveedorSegHigieneDto
            {
                Id = e.Id,
                Proveedor = e.Proveedor,
                Especialidad = e.Especialidad,
                AreaUbicacionId = e.AreaUbicacionId,
                Mes = e.Mes,
                Kpi = e.Kpi,
                Cumplimiento = e.Cumplimiento,
                FechaImportacion = e.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EvaluacionProveedorSegHigieneDto?> CrearRegistroAsync(EvaluacionProveedorSegHigieneCrearDto dto)
    {
        var proveedor = NormalizarProveedor(dto.Proveedor);
        var mes = PrimerDiaDelMes(dto.Mes);

        var yaExiste = await _contexto.EvaluacionesProveedorSegHigiene.AnyAsync(e =>
            e.Proveedor == proveedor && e.AreaUbicacionId == dto.AreaUbicacionId && e.Mes == mes);
        if (yaExiste)
        {
            return null;
        }

        var registro = new EvaluacionProveedorSegHigiene
        {
            Proveedor = proveedor,
            Especialidad = dto.Especialidad,
            AreaUbicacionId = dto.AreaUbicacionId,
            Mes = mes,
            Kpi = dto.Kpi,
            Cumplimiento = dto.Cumplimiento,
        };

        _contexto.EvaluacionesProveedorSegHigiene.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<EvaluacionProveedorSegHigieneDto?> ActualizarRegistroAsync(int id, EvaluacionProveedorSegHigieneActualizarDto dto)
    {
        var registro = await _contexto.EvaluacionesProveedorSegHigiene.FirstOrDefaultAsync(e => e.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Especialidad = dto.Especialidad;
        registro.Kpi = dto.Kpi;
        registro.Cumplimiento = dto.Cumplimiento;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<EvaluacionProveedorSegHigieneImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new EvaluacionProveedorSegHigieneImportarResultadoDto();

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

        // Llave de deduplicación: Proveedor + Planta + Mes (reimportar el
        // mismo mes/proveedor/planta actualiza en vez de duplicar) — mismo
        // criterio que Evaluaciones de vigilancia.
        var existentes = await _contexto.EvaluacionesProveedorSegHigiene.ToDictionaryAsync(
            e => (Proveedor: e.Proveedor.Trim().ToLowerInvariant(), e.AreaUbicacionId, e.Mes.Date),
            e => e);

        void MapearCampos(EvaluacionProveedorSegHigiene registro, ExcelFilaLectora lectora, int areaUbicacionId, string proveedor, DateTime mes)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Proveedor = proveedor;
            registro.Mes = mes;
            registro.Especialidad = lectora.Texto("Especialidad") ?? string.Empty;
            registro.Kpi = lectora.Numero("Kpi");
            registro.Cumplimiento = lectora.Numero("Cumplimiento") ?? 0;
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

            var proveedorTexto = lectora.Texto("Proveedor");
            var mesLeido = lectora.Fecha("Mes");
            if (proveedorTexto is null || mesLeido is null)
            {
                resultado.Errores.Add($"Fila {numeroFila}: falta Proveedor o Mes, se omitió.");
                resultado.Omitidos++;
                continue;
            }

            var proveedor = NormalizarProveedor(proveedorTexto);
            var mes = PrimerDiaDelMes(mesLeido.Value);
            var llave = (Proveedor: proveedor.ToLowerInvariant(), areaUbicacionId.Value, mes.Date);

            if (existentes.TryGetValue(llave, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value, proveedor, mes);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new EvaluacionProveedorSegHigiene
                {
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value, proveedor, mes);
                _contexto.EvaluacionesProveedorSegHigiene.Add(nuevo);
                existentes[llave] = nuevo;

                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static EvaluacionProveedorSegHigieneDto MapearDto(EvaluacionProveedorSegHigiene e) => new()
    {
        Id = e.Id,
        Proveedor = e.Proveedor,
        Especialidad = e.Especialidad,
        AreaUbicacionId = e.AreaUbicacionId,
        Mes = e.Mes,
        Kpi = e.Kpi,
        Cumplimiento = e.Cumplimiento,
        FechaImportacion = e.FechaImportacion,
    };
}
