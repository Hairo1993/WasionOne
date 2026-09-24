using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class EstacionamientoService : IEstacionamientoService
{
    // Fijo mientras solo exista Seguridad Patrimonial (AreaId = 1) en esta Área.
    private const int AREA_ID_SEGURIDAD_PATRIMONIAL = 1;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "id",
        ["NoMarbete"] = "nomarbete",
        ["Colaborador"] = "colaborador",
        ["Area"] = "area",
        ["PlantaBase"] = "plantabase",
        ["MultiPlanta"] = "multiplanta",
        ["PlantasAdicionales"] = "plantasadicionales",
        ["MarcaVehiculo1"] = "marcavehiculo1",
        ["SubmarcaVehiculo1"] = "submarcavehiculo1",
        ["PlacasVehiculo1"] = "placasvehiculo1",
        ["MarcaVehiculo2"] = "marcavehiculo2",
        ["SubmarcaVehiculo2"] = "submarcavehiculo2",
        ["PlacasVehiculo2"] = "placasvehiculo2",
        ["EstatusDocumentacion"] = "estatusdocumentacion",
        ["Licencia"] = "licencia",
        ["VencimientoLicencia"] = "venclicencia",
        ["TarjetaCirculacion"] = "tarjetacirculacion",
        ["Seguro"] = "seguro",
        ["VencimientoSeguro"] = "vencseguro",
        ["RegistradoPor"] = "registradopor",
        ["FechaRegistro"] = "fecharegistro",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba, con el encabezado "bonito" y un valor de
    // ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("ID", "EST-0001"),
        new("No. Marbete", "MRB-001"),
        new("Colaborador", "Sofía Ramírez"),
        new("Área", "Logística"),
        new("Planta Base", "Planta 1"),
        new("¿Multi-Planta?", "Sí"),
        new("Plantas Adicionales", "Planta 2"),
        new("Marca Vehículo 1", "Nissan"),
        new("Submarca Vehículo 1", "Versa"),
        new("Placas Vehículo 1", "ABC-123"),
        new("Marca Vehículo 2", "Toyota"),
        new("Submarca Vehículo 2", "Corolla"),
        new("Placas Vehículo 2", "XYZ-789"),
        new("Estatus Documentación", "Completa"),
        new("Licencia", "LIC-556677"),
        new("Venc. Licencia", "24/09/2027"),
        new("Tarjeta Circulación", "TC-998877"),
        new("Seguro", "Póliza GNP-4455"),
        new("Venc. Seguro", "24/09/2027"),
        new("Registrado Por", "Recepción"),
        new("Fecha Registro", "24/09/2026"),
    };

    private readonly ApplicationDbContext _contexto;

    public EstacionamientoService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Seg Estacionamiento", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<EstacionamientoDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.Estacionamientos.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(e => e.AreaUbicacionId == areaUbicacionId.Value);
        }

        var registros = await consulta
            .OrderBy(e => e.Colaborador)
            .ToListAsync();

        return registros.Select(MapearDto);
    }

    public async Task<EstacionamientoDto?> ObtenerRegistroPorIdAsync(int id)
    {
        var registro = await _contexto.Estacionamientos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        return registro is null ? null : MapearDto(registro);
    }

    public async Task<EstacionamientoDto?> CrearRegistroAsync(EstacionamientoCrearDto dto)
    {
        var yaExiste = await _contexto.Estacionamientos
            .AnyAsync(e => e.NoMarbete == dto.NoMarbete);
        if (yaExiste)
        {
            return null;
        }

        var registro = new Estacionamiento
        {
            NoMarbete = dto.NoMarbete,
            Colaborador = dto.Colaborador,
            Area = dto.Area,
            AreaUbicacionId = dto.AreaUbicacionId,
            MultiPlanta = dto.MultiPlanta,
            PlantasAdicionales = dto.PlantasAdicionales,
            MarcaVehiculo1 = dto.MarcaVehiculo1,
            SubmarcaVehiculo1 = dto.SubmarcaVehiculo1,
            PlacasVehiculo1 = dto.PlacasVehiculo1,
            MarcaVehiculo2 = dto.MarcaVehiculo2,
            SubmarcaVehiculo2 = dto.SubmarcaVehiculo2,
            PlacasVehiculo2 = dto.PlacasVehiculo2,
            EstatusDocumentacion = dto.EstatusDocumentacion,
            Licencia = dto.Licencia,
            VencimientoLicencia = dto.VencimientoLicencia,
            TarjetaCirculacion = dto.TarjetaCirculacion,
            Seguro = dto.Seguro,
            VencimientoSeguro = dto.VencimientoSeguro,
            RegistradoPor = dto.RegistradoPor,
            FechaRegistro = DateTime.UtcNow,
        };

        _contexto.Estacionamientos.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<EstacionamientoDto?> ActualizarRegistroAsync(int id, EstacionamientoActualizarDto dto)
    {
        var registro = await _contexto.Estacionamientos.FirstOrDefaultAsync(e => e.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Colaborador = dto.Colaborador;
        registro.Area = dto.Area;
        registro.AreaUbicacionId = dto.AreaUbicacionId;
        registro.MultiPlanta = dto.MultiPlanta;
        registro.PlantasAdicionales = dto.PlantasAdicionales;
        registro.MarcaVehiculo1 = dto.MarcaVehiculo1;
        registro.SubmarcaVehiculo1 = dto.SubmarcaVehiculo1;
        registro.PlacasVehiculo1 = dto.PlacasVehiculo1;
        registro.MarcaVehiculo2 = dto.MarcaVehiculo2;
        registro.SubmarcaVehiculo2 = dto.SubmarcaVehiculo2;
        registro.PlacasVehiculo2 = dto.PlacasVehiculo2;
        registro.EstatusDocumentacion = dto.EstatusDocumentacion;
        registro.Licencia = dto.Licencia;
        registro.VencimientoLicencia = dto.VencimientoLicencia;
        registro.TarjetaCirculacion = dto.TarjetaCirculacion;
        registro.Seguro = dto.Seguro;
        registro.VencimientoSeguro = dto.VencimientoSeguro;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<EstacionamientoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new EstacionamientoImportarResultadoDto();

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

        var existentes = await _contexto.Estacionamientos
            .ToDictionaryAsync(e => e.NoMarbete, e => e);

        void MapearCampos(Estacionamiento registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Colaborador = lectora.Texto("Colaborador");
            registro.Area = lectora.Texto("Area");
            registro.MultiPlanta = lectora.Booleano("MultiPlanta");
            registro.PlantasAdicionales = lectora.Texto("PlantasAdicionales");
            registro.MarcaVehiculo1 = lectora.Texto("MarcaVehiculo1");
            registro.SubmarcaVehiculo1 = lectora.Texto("SubmarcaVehiculo1");
            registro.PlacasVehiculo1 = lectora.Texto("PlacasVehiculo1");
            registro.MarcaVehiculo2 = lectora.Texto("MarcaVehiculo2");
            registro.SubmarcaVehiculo2 = lectora.Texto("SubmarcaVehiculo2");
            registro.PlacasVehiculo2 = lectora.Texto("PlacasVehiculo2");
            registro.EstatusDocumentacion = lectora.Texto("EstatusDocumentacion");
            registro.Licencia = lectora.Texto("Licencia");
            registro.VencimientoLicencia = lectora.Fecha("VencimientoLicencia");
            registro.TarjetaCirculacion = lectora.Texto("TarjetaCirculacion");
            registro.Seguro = lectora.Texto("Seguro");
            registro.VencimientoSeguro = lectora.Fecha("VencimientoSeguro");
            registro.RegistradoPor = lectora.Texto("RegistradoPor");
            registro.FechaRegistro = lectora.Fecha("FechaRegistro") ?? registro.FechaRegistro;
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var noMarbete = lectora.Texto("NoMarbete");
            if (string.IsNullOrWhiteSpace(noMarbete))
            {
                resultado.Errores.Add($"Fila {numeroFila}: no trae 'No. Marbete', se omitió.");
                resultado.Omitidos++;
                continue;
            }

            var plantaTexto = lectora.Texto("PlantaBase");
            int? areaUbicacionId = null;
            if (plantaTexto is not null
                && ubicaciones.TryGetValue(ExcelImportUtils.Normalizar(plantaTexto), out var ubicacionId)
                && areaUbicacionesSeguridad.TryGetValue(ubicacionId, out var areaUbicacionEncontrada))
            {
                areaUbicacionId = areaUbicacionEncontrada;
            }

            if (areaUbicacionId is null)
            {
                resultado.Errores.Add(
                    $"Fila {numeroFila} (marbete {noMarbete}): la Planta Base '{plantaTexto}' no coincide con ninguna ubicación del catálogo de Seguridad Patrimonial, se omitió.");
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

            if (existentes.TryGetValue(noMarbete, out var registroExistente))
            {
                registroExistente.IdOrigen ??= idOrigen;
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new Estacionamiento
                {
                    IdOrigen = idOrigen,
                    NoMarbete = noMarbete,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.Estacionamientos.Add(nuevo);
                existentes[noMarbete] = nuevo;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static EstacionamientoDto MapearDto(Estacionamiento e) => new()
    {
        Id = e.Id,
        IdOrigen = e.IdOrigen,
        NoMarbete = e.NoMarbete,
        Colaborador = e.Colaborador,
        Area = e.Area,
        AreaUbicacionId = e.AreaUbicacionId,
        MultiPlanta = e.MultiPlanta,
        PlantasAdicionales = e.PlantasAdicionales,
        MarcaVehiculo1 = e.MarcaVehiculo1,
        SubmarcaVehiculo1 = e.SubmarcaVehiculo1,
        PlacasVehiculo1 = e.PlacasVehiculo1,
        MarcaVehiculo2 = e.MarcaVehiculo2,
        SubmarcaVehiculo2 = e.SubmarcaVehiculo2,
        PlacasVehiculo2 = e.PlacasVehiculo2,
        EstatusDocumentacion = e.EstatusDocumentacion,
        Licencia = e.Licencia,
        VencimientoLicencia = e.VencimientoLicencia,
        TarjetaCirculacion = e.TarjetaCirculacion,
        Seguro = e.Seguro,
        VencimientoSeguro = e.VencimientoSeguro,
        RegistradoPor = e.RegistradoPor,
        FechaRegistro = e.FechaRegistro,
        FechaImportacion = e.FechaImportacion,
    };
}
