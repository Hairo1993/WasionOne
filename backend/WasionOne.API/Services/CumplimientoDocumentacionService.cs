using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class CumplimientoDocumentacionService : ICumplimientoDocumentacionService
{
    // Fijo mientras solo exista Administración (AreaId de Administración)
    // en esta Área.
    // TODO AJUSTAR: -1 es un valor temporal. Reemplazar por el AreaId real
    // de la Área "Administración" (consultar GET /api/catalogos/areas o la
    // pantalla /admin/catalogos) antes de usar este módulo en producción.
    private const int AREA_ID_ADMINISTRACION = 11; // Area "Administracion" (AreaId real confirmado 22/sep/2026)

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Code"] = "code",
        ["Proveedor"] = "proveedor",
        ["TipoContrato"] = "tipocontrato",
        ["Area"] = "area",
        ["Responsable"] = "resp",
        ["MontoIvaIncluido"] = "montoivaincluido",
        ["Moneda"] = "moneda",
        ["FirmaDireccion"] = "firmadireccion",
        ["FirmaLegal"] = "firmalegal",
        ["FirmaFinanzas"] = "firmafinanzas",
        ["AprobadoLeninLi"] = "aprobadoleninli",
        ["Firmado"] = "firmado",
        ["FechaInicio"] = "fechainicio",
        ["FechaVencimiento"] = "fechavencimiento",
        ["Renovacion"] = "renovacion",
        ["Estatus"] = "estatus",
        ["Carpeta"] = "carpeta",
        ["Ubicacion"] = "planta",
        // "Días por vencer" se lee y se descarta: siempre se recalcula.
        ["DiasPorVencer"] = "diasporvencer",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" (el que ya se mostraba en el texto de ayuda de
    // la pantalla) y un valor de ejemplo por columna. "DiasPorVencer" no
    // se incluye: se lee y se descarta en la importación (siempre se
    // recalcula), así que no aparece en "Columnas esperadas" del texto de
    // ayuda.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Code", "DOC-0001"),
        new("Proveedor", "Grupo ABC"),
        new("Tipo de Contrato", "Servicios"),
        new("Área", "Compras"),
        new("Resp", "Juan Pérez"),
        new("Monto (IVA incluido)", "50000"),
        new("Moneda", "MXN"),
        new("Firma Dirección", "Sí"),
        new("Firma Legal", "Sí"),
        new("Firma Finanzas", "No"),
        new("Aprobado Lenin/Li", "Sí"),
        new("Firmado", "Sí"),
        new("Fecha Inicio", "24/09/2026"),
        new("Fecha Vencimiento", "24/09/2027"),
        new("Renovación", "Anual"),
        new("Estatus", "Vigente"),
        new("Carpeta", "Carpeta 2026"),
        new("Planta", "Planta 1"),
    };

    private readonly ApplicationDbContext _contexto;

    public CumplimientoDocumentacionService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Adm Cumplimiento Documentacion", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    private static int CalcularDiasPorVencer(DateTime fechaVencimiento) =>
        (fechaVencimiento.Date - DateTime.Today).Days;

    public async Task<IEnumerable<CumplimientoDocumentacionDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.CumplimientosDocumentacion.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(c => c.AreaUbicacionId == areaUbicacionId.Value);
        }

        var registros = await consulta
            .OrderBy(c => c.FechaVencimiento)
            .ToListAsync();

        return registros.Select(MapearDto);
    }

    public async Task<CumplimientoDocumentacionDto?> ObtenerRegistroPorIdAsync(int id)
    {
        var registro = await _contexto.CumplimientosDocumentacion
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return registro is null ? null : MapearDto(registro);
    }

    public async Task<CumplimientoDocumentacionDto?> CrearRegistroAsync(CumplimientoDocumentacionCrearDto dto)
    {
        var yaExiste = await _contexto.CumplimientosDocumentacion.AnyAsync(c => c.Code == dto.Code);
        if (yaExiste)
        {
            return null;
        }

        var registro = new CumplimientoDocumentacion
        {
            Code = dto.Code,
            Proveedor = dto.Proveedor,
            TipoContrato = dto.TipoContrato,
            Area = dto.Area,
            Responsable = dto.Responsable,
            MontoIvaIncluido = dto.MontoIvaIncluido,
            Moneda = dto.Moneda,
            FirmaDireccion = dto.FirmaDireccion,
            FirmaLegal = dto.FirmaLegal,
            FirmaFinanzas = dto.FirmaFinanzas,
            AprobadoLeninLi = dto.AprobadoLeninLi,
            Firmado = dto.Firmado,
            FechaInicio = dto.FechaInicio,
            FechaVencimiento = dto.FechaVencimiento,
            Renovacion = dto.Renovacion,
            Estatus = dto.Estatus,
            Carpeta = dto.Carpeta,
            AreaUbicacionId = dto.AreaUbicacionId,
        };

        _contexto.CumplimientosDocumentacion.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<CumplimientoDocumentacionDto?> ActualizarRegistroAsync(int id, CumplimientoDocumentacionActualizarDto dto)
    {
        var registro = await _contexto.CumplimientosDocumentacion.FirstOrDefaultAsync(c => c.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Proveedor = dto.Proveedor;
        registro.TipoContrato = dto.TipoContrato;
        registro.Area = dto.Area;
        registro.Responsable = dto.Responsable;
        registro.MontoIvaIncluido = dto.MontoIvaIncluido;
        registro.Moneda = dto.Moneda;
        registro.FirmaDireccion = dto.FirmaDireccion;
        registro.FirmaLegal = dto.FirmaLegal;
        registro.FirmaFinanzas = dto.FirmaFinanzas;
        registro.AprobadoLeninLi = dto.AprobadoLeninLi;
        registro.Firmado = dto.Firmado;
        registro.FechaInicio = dto.FechaInicio;
        registro.FechaVencimiento = dto.FechaVencimiento;
        registro.Renovacion = dto.Renovacion;
        registro.Estatus = dto.Estatus;
        registro.Carpeta = dto.Carpeta;
        registro.AreaUbicacionId = dto.AreaUbicacionId;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<CumplimientoDocumentacionImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new CumplimientoDocumentacionImportarResultadoDto();

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

        var existentes = await _contexto.CumplimientosDocumentacion
            .ToDictionaryAsync(c => c.Code, c => c);

        void MapearCampos(CumplimientoDocumentacion registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Proveedor = lectora.Texto("Proveedor");
            registro.TipoContrato = lectora.Texto("TipoContrato");
            registro.Area = lectora.Texto("Area");
            registro.Responsable = lectora.Texto("Responsable");
            registro.MontoIvaIncluido = lectora.Numero("MontoIvaIncluido");
            registro.Moneda = lectora.Texto("Moneda");
            registro.FirmaDireccion = lectora.Booleano("FirmaDireccion");
            registro.FirmaLegal = lectora.Booleano("FirmaLegal");
            registro.FirmaFinanzas = lectora.Booleano("FirmaFinanzas");
            registro.AprobadoLeninLi = lectora.Booleano("AprobadoLeninLi");
            registro.Firmado = lectora.Booleano("Firmado");
            registro.FechaInicio = lectora.Fecha("FechaInicio") ?? registro.FechaInicio;
            registro.FechaVencimiento = lectora.Fecha("FechaVencimiento") ?? registro.FechaVencimiento;
            registro.Renovacion = lectora.Texto("Renovacion");
            registro.Estatus = lectora.Texto("Estatus");
            registro.Carpeta = lectora.Texto("Carpeta");
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var code = lectora.Texto("Code");
            if (string.IsNullOrWhiteSpace(code))
            {
                resultado.Errores.Add($"Fila {numeroFila}: no trae 'Code', se omitió.");
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
                    $"Fila {numeroFila} (code {code}): la Planta '{plantaTexto}' no coincide con ninguna ubicación del catálogo de Administración, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            if (existentes.TryGetValue(code, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new CumplimientoDocumentacion
                {
                    Code = code,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.CumplimientosDocumentacion.Add(nuevo);
                existentes[code] = nuevo;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static CumplimientoDocumentacionDto MapearDto(CumplimientoDocumentacion c) => new()
    {
        Id = c.Id,
        Code = c.Code,
        Proveedor = c.Proveedor,
        TipoContrato = c.TipoContrato,
        Area = c.Area,
        Responsable = c.Responsable,
        MontoIvaIncluido = c.MontoIvaIncluido,
        Moneda = c.Moneda,
        FirmaDireccion = c.FirmaDireccion,
        FirmaLegal = c.FirmaLegal,
        FirmaFinanzas = c.FirmaFinanzas,
        AprobadoLeninLi = c.AprobadoLeninLi,
        Firmado = c.Firmado,
        FechaInicio = c.FechaInicio,
        FechaVencimiento = c.FechaVencimiento,
        Renovacion = c.Renovacion,
        Estatus = c.Estatus,
        Carpeta = c.Carpeta,
        AreaUbicacionId = c.AreaUbicacionId,
        DiasPorVencer = CalcularDiasPorVencer(c.FechaVencimiento),
        FechaImportacion = c.FechaImportacion,
    };
}
