using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class ValeSalidaService : IValeSalidaService
{
    // Fijo mientras solo exista Seguridad Patrimonial (AreaId = 1) en esta Área.
    private const int AREA_ID_SEGURIDAD_PATRIMONIAL = 1;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "id",
        ["Folio"] = "folio",
        ["Solicitante"] = "solicitante",
        ["Referencia"] = "referencia",
        ["ConceptoMotivo"] = "conceptomotivo",
        ["DetalleMotivo"] = "detallemotivo",
        ["ActivoFijo"] = "activofijo",
        ["Ubicacion"] = "planta",
        ["Estado"] = "estado",
        ["FechaVale"] = "fechadelvale",
        ["FechaSalida"] = "fechasalida",
        ["FechaEstimadaRetorno"] = "fechaestimadaretorno",
        ["FechaRealRetorno"] = "fecharealretorno",
        ["ArticulosMateriales"] = "articulosmateriales",
        ["RegistradoPor"] = "registradopor",
        ["CerradoPor"] = "cerradopor",
        ["FechaRegistro"] = "fecharegistro",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("ID", "VS-0001"),
        new("Folio", "VS-0001"),
        new("Solicitante", "Juan Pérez"),
        new("Referencia", "OT-1234"),
        new("Concepto / Motivo", "Préstamo de herramienta"),
        new("Detalle Motivo", "Salida de multímetro para mantenimiento"),
        new("¿Activo Fijo?", "Sí"),
        new("Planta", "Planta 1"),
        new("Estado", "Abierto"),
        new("Fecha del Vale", "24/09/2026"),
        new("Fecha Salida", "24/09/2026"),
        new("Fecha Estimada Retorno", "25/09/2026"),
        new("Fecha Real Retorno", "25/09/2026"),
        new("Artículos / Materiales", "1 multímetro digital"),
        new("Registrado Por", "Ana Torres"),
        new("Cerrado Por", "Ana Torres"),
        new("Fecha Registro", "24/09/2026"),
    };

    private readonly ApplicationDbContext _contexto;

    public ValeSalidaService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Seg Vales de Salida", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<ValeSalidaDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.ValesSalida.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(v => v.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(v => v.FechaVale)
            .Select(v => new ValeSalidaDto
            {
                Id = v.Id,
                IdOrigen = v.IdOrigen,
                Folio = v.Folio,
                Solicitante = v.Solicitante,
                Referencia = v.Referencia,
                ConceptoMotivo = v.ConceptoMotivo,
                DetalleMotivo = v.DetalleMotivo,
                ActivoFijo = v.ActivoFijo,
                AreaUbicacionId = v.AreaUbicacionId,
                Estado = v.Estado,
                FechaVale = v.FechaVale,
                FechaSalida = v.FechaSalida,
                FechaEstimadaRetorno = v.FechaEstimadaRetorno,
                FechaRealRetorno = v.FechaRealRetorno,
                ArticulosMateriales = v.ArticulosMateriales,
                RegistradoPor = v.RegistradoPor,
                CerradoPor = v.CerradoPor,
                FechaRegistro = v.FechaRegistro,
                FechaImportacion = v.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<ValeSalidaDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.ValesSalida
            .AsNoTracking()
            .Where(v => v.Id == id)
            .Select(v => new ValeSalidaDto
            {
                Id = v.Id,
                IdOrigen = v.IdOrigen,
                Folio = v.Folio,
                Solicitante = v.Solicitante,
                Referencia = v.Referencia,
                ConceptoMotivo = v.ConceptoMotivo,
                DetalleMotivo = v.DetalleMotivo,
                ActivoFijo = v.ActivoFijo,
                AreaUbicacionId = v.AreaUbicacionId,
                Estado = v.Estado,
                FechaVale = v.FechaVale,
                FechaSalida = v.FechaSalida,
                FechaEstimadaRetorno = v.FechaEstimadaRetorno,
                FechaRealRetorno = v.FechaRealRetorno,
                ArticulosMateriales = v.ArticulosMateriales,
                RegistradoPor = v.RegistradoPor,
                CerradoPor = v.CerradoPor,
                FechaRegistro = v.FechaRegistro,
                FechaImportacion = v.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ValeSalidaDto> CrearRegistroAsync(ValeSalidaCrearDto dto)
    {
        var registro = new ValeSalida
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Folio = dto.Folio,
            Solicitante = dto.Solicitante,
            Referencia = dto.Referencia,
            ConceptoMotivo = dto.ConceptoMotivo,
            DetalleMotivo = dto.DetalleMotivo,
            ActivoFijo = dto.ActivoFijo,
            Estado = string.IsNullOrWhiteSpace(dto.Estado) ? "Abierto" : dto.Estado,
            FechaVale = dto.FechaVale,
            FechaSalida = dto.FechaSalida,
            FechaEstimadaRetorno = dto.FechaEstimadaRetorno,
            ArticulosMateriales = dto.ArticulosMateriales,
            RegistradoPor = dto.RegistradoPor,
        };

        _contexto.ValesSalida.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<ValeSalidaDto?> ActualizarRegistroAsync(int id, ValeSalidaActualizarDto dto)
    {
        var registro = await _contexto.ValesSalida.FirstOrDefaultAsync(v => v.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Estado = dto.Estado;
        registro.FechaEstimadaRetorno = dto.FechaEstimadaRetorno;
        registro.FechaRealRetorno = dto.FechaRealRetorno;
        registro.ArticulosMateriales = dto.ArticulosMateriales;
        registro.CerradoPor = dto.CerradoPor;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<ValeSalidaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new ValeSalidaImportarResultadoDto();

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

        var existentes = await _contexto.ValesSalida
            .Where(v => v.IdOrigen != null)
            .ToDictionaryAsync(v => v.IdOrigen!, v => v);

        void MapearCampos(ValeSalida registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Folio = lectora.Texto("Folio");
            registro.Solicitante = lectora.Texto("Solicitante");
            registro.Referencia = lectora.Texto("Referencia");
            registro.ConceptoMotivo = lectora.Texto("ConceptoMotivo");
            registro.DetalleMotivo = lectora.Texto("DetalleMotivo");
            registro.ActivoFijo = lectora.Booleano("ActivoFijo");
            registro.Estado = lectora.Texto("Estado") ?? "Abierto";
            registro.FechaVale = lectora.Fecha("FechaVale") ?? registro.FechaVale;
            registro.FechaSalida = lectora.Fecha("FechaSalida");
            registro.FechaEstimadaRetorno = lectora.Fecha("FechaEstimadaRetorno");
            registro.FechaRealRetorno = lectora.Fecha("FechaRealRetorno");
            registro.ArticulosMateriales = lectora.Texto("ArticulosMateriales");
            registro.RegistradoPor = lectora.Texto("RegistradoPor");
            registro.CerradoPor = lectora.Texto("CerradoPor");
            registro.FechaRegistro = lectora.Fecha("FechaRegistro");
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var ubicacionTexto = lectora.Texto("Ubicacion");
            int? areaUbicacionId = null;
            if (ubicacionTexto is not null
                && ubicaciones.TryGetValue(ExcelImportUtils.Normalizar(ubicacionTexto), out var ubicacionId)
                && areaUbicacionesSeguridad.TryGetValue(ubicacionId, out var areaUbicacionEncontrada))
            {
                areaUbicacionId = areaUbicacionEncontrada;
            }

            if (areaUbicacionId is null)
            {
                resultado.Errores.Add(
                    $"Fila {numeroFila}: la Planta '{ubicacionTexto}' no coincide con ninguna ubicación del catálogo de Seguridad Patrimonial, se omitió.");
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
            if (idOrigen is not null && existentes.TryGetValue(idOrigen, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new ValeSalida
                {
                    IdOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.ValesSalida.Add(nuevo);
                if (idOrigen is not null)
                {
                    existentes[idOrigen] = nuevo;
                }

                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static ValeSalidaDto MapearDto(ValeSalida v) => new()
    {
        Id = v.Id,
        IdOrigen = v.IdOrigen,
        Folio = v.Folio,
        Solicitante = v.Solicitante,
        Referencia = v.Referencia,
        ConceptoMotivo = v.ConceptoMotivo,
        DetalleMotivo = v.DetalleMotivo,
        ActivoFijo = v.ActivoFijo,
        AreaUbicacionId = v.AreaUbicacionId,
        Estado = v.Estado,
        FechaVale = v.FechaVale,
        FechaSalida = v.FechaSalida,
        FechaEstimadaRetorno = v.FechaEstimadaRetorno,
        FechaRealRetorno = v.FechaRealRetorno,
        ArticulosMateriales = v.ArticulosMateriales,
        RegistradoPor = v.RegistradoPor,
        CerradoPor = v.CerradoPor,
        FechaRegistro = v.FechaRegistro,
        FechaImportacion = v.FechaImportacion,
    };
}
