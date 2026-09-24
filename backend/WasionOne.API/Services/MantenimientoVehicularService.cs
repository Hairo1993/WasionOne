using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class MantenimientoVehicularService : IMantenimientoVehicularService
{
    // Fijo mientras solo exista Administración (AreaId de Administración)
    // en esta Área.
    // TODO AJUSTAR: -1 es un valor temporal. Reemplazar por el AreaId real
    // de la Área "Administración" (consultar GET /api/catalogos/areas o la
    // pantalla /admin/catalogos) antes de usar este módulo en producción.
    private const int AREA_ID_ADMINISTRACION = 11; // Area "Administracion" (AreaId real confirmado 22/sep/2026)

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Vin"] = "vin",
        ["Fecha"] = "fecha",
        ["VehiculoTipo"] = "vehiculotipo",
        ["KilometrajeUltimoServicio"] = "kilometrajedeultimoservicio",
        ["KilometrajeActual"] = "kilometrajeactual",
        ["ProximoServicio"] = "proximoservicio",
        ["Estatus"] = "estatus",
        ["Ubicacion"] = "planta",
        // "Km restantes" se lee y se descarta: siempre se recalcula.
        ["KmRestantes"] = "kmrestantes",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" (el que ya se mostraba en el texto de ayuda de
    // la pantalla) y un valor de ejemplo por columna. "KmRestantes" no se
    // incluye: se lee y se descarta en la importación (siempre se
    // recalcula), así que no aparece en "Columnas esperadas" del texto de
    // ayuda.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("VIN", "3N1AB7AP7KY123456"),
        new("Fecha", "24/09/2026"),
        new("Vehículo tipo", "Camioneta"),
        new("Kilometraje de ultimo servicio", "40000"),
        new("Kilometraje actual", "45000"),
        new("Próximo servicio", "50000"),
        new("Estatus", "Activo"),
        new("Planta", "Planta 1"),
    };

    private readonly ApplicationDbContext _contexto;

    public MantenimientoVehicularService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Adm Mantenimiento Vehicular", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    private static int? CalcularKmRestantes(int? proximoServicio, int? kilometrajeActual) =>
        proximoServicio.HasValue && kilometrajeActual.HasValue
            ? proximoServicio.Value - kilometrajeActual.Value
            : null;

    public async Task<IEnumerable<MantenimientoVehicularDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.MantenimientosVehiculares.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(m => m.AreaUbicacionId == areaUbicacionId.Value);
        }

        var registros = await consulta
            .OrderBy(m => m.Vin)
            .ToListAsync();

        return registros.Select(MapearDto);
    }

    public async Task<MantenimientoVehicularDto?> ObtenerRegistroPorIdAsync(int id)
    {
        var registro = await _contexto.MantenimientosVehiculares
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        return registro is null ? null : MapearDto(registro);
    }

    public async Task<MantenimientoVehicularDto?> CrearRegistroAsync(MantenimientoVehicularCrearDto dto)
    {
        var yaExiste = await _contexto.MantenimientosVehiculares.AnyAsync(m => m.Vin == dto.Vin);
        if (yaExiste)
        {
            return null;
        }

        var registro = new MantenimientoVehicular
        {
            Vin = dto.Vin,
            Fecha = dto.Fecha,
            VehiculoTipo = dto.VehiculoTipo,
            KilometrajeUltimoServicio = dto.KilometrajeUltimoServicio,
            KilometrajeActual = dto.KilometrajeActual,
            ProximoServicio = dto.ProximoServicio,
            Estatus = dto.Estatus,
            AreaUbicacionId = dto.AreaUbicacionId,
        };

        _contexto.MantenimientosVehiculares.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<MantenimientoVehicularDto?> ActualizarRegistroAsync(int id, MantenimientoVehicularActualizarDto dto)
    {
        var registro = await _contexto.MantenimientosVehiculares.FirstOrDefaultAsync(m => m.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Fecha = dto.Fecha;
        registro.VehiculoTipo = dto.VehiculoTipo;
        registro.KilometrajeUltimoServicio = dto.KilometrajeUltimoServicio;
        registro.KilometrajeActual = dto.KilometrajeActual;
        registro.ProximoServicio = dto.ProximoServicio;
        registro.Estatus = dto.Estatus;
        registro.AreaUbicacionId = dto.AreaUbicacionId;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<MantenimientoVehicularImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new MantenimientoVehicularImportarResultadoDto();

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

        var existentes = await _contexto.MantenimientosVehiculares
            .ToDictionaryAsync(m => m.Vin, m => m);

        void MapearCampos(MantenimientoVehicular registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Fecha = lectora.Fecha("Fecha") ?? registro.Fecha;
            registro.VehiculoTipo = lectora.Texto("VehiculoTipo");
            registro.KilometrajeUltimoServicio = lectora.Entero("KilometrajeUltimoServicio");
            registro.KilometrajeActual = lectora.Entero("KilometrajeActual");
            registro.ProximoServicio = lectora.Entero("ProximoServicio");
            registro.Estatus = lectora.Texto("Estatus");
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var vin = lectora.Texto("Vin");
            if (string.IsNullOrWhiteSpace(vin))
            {
                resultado.Errores.Add($"Fila {numeroFila}: no trae 'VIN', se omitió.");
                resultado.Omitidos++;
                continue;
            }

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
                    $"Fila {numeroFila} (VIN {vin}): la Planta '{plantaTexto}' no coincide con ninguna ubicación del catálogo de Administración, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            if (existentes.TryGetValue(vin, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new MantenimientoVehicular
                {
                    Vin = vin,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.MantenimientosVehiculares.Add(nuevo);
                existentes[vin] = nuevo;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static MantenimientoVehicularDto MapearDto(MantenimientoVehicular m) => new()
    {
        Id = m.Id,
        Vin = m.Vin,
        Fecha = m.Fecha,
        VehiculoTipo = m.VehiculoTipo,
        KilometrajeUltimoServicio = m.KilometrajeUltimoServicio,
        KilometrajeActual = m.KilometrajeActual,
        ProximoServicio = m.ProximoServicio,
        Estatus = m.Estatus,
        AreaUbicacionId = m.AreaUbicacionId,
        KmRestantes = CalcularKmRestantes(m.ProximoServicio, m.KilometrajeActual),
        FechaImportacion = m.FechaImportacion,
    };
}
