using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class ReunionProveedorService : IReunionProveedorService
{
    // Fijo mientras solo exista Seguridad Patrimonial (AreaId = 1) en esta Área.
    private const int AREA_ID_SEGURIDAD_PATRIMONIAL = 1;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "id",
        ["Fecha"] = "fecha",
        ["Hora"] = "hora",
        ["Proveedor"] = "proveedor",
        ["Ubicacion"] = "planta",
        ["AsuntoMotivo"] = "asuntomotivo",
        ["Asistentes"] = "asistentes",
        ["MinutaAcuerdos"] = "minutaacuerdos",
        ["RegistradoPor"] = "registradopor",
        ["FechaRegistro"] = "fecharegistro",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("ID", "RPV-0001"),
        new("Fecha", "24/09/2026"),
        new("Hora", "10:00"),
        new("Proveedor", "Proveedor Ejemplo S.A."),
        new("Planta", "Planta 1"),
        new("Asunto / Motivo", "Revisión mensual de desempeño"),
        new("Asistentes", "Juan Pérez, María López"),
        new("Minuta / Acuerdos", "Se acuerda dar seguimiento en 30 días"),
        new("Registrado Por", "Ana Torres"),
        new("Fecha Registro", "24/09/2026"),
    };

    private readonly ApplicationDbContext _contexto;

    public ReunionProveedorService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Seg Reuniones Proveedor", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<ReunionProveedorDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.ReunionesProveedor.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(r => r.AreaUbicacionId == areaUbicacionId.Value);
        }

        var registros = await consulta
            .OrderByDescending(r => r.Fecha)
            .ToListAsync();

        return registros.Select(MapearDto);
    }

    public async Task<ReunionProveedorDto?> ObtenerRegistroPorIdAsync(int id)
    {
        var registro = await _contexto.ReunionesProveedor
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        return registro is null ? null : MapearDto(registro);
    }

    public async Task<ReunionProveedorDto> CrearRegistroAsync(ReunionProveedorCrearDto dto)
    {
        var registro = new ReunionProveedor
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Fecha = dto.Fecha,
            Hora = dto.Hora,
            Proveedor = dto.Proveedor,
            AsuntoMotivo = dto.AsuntoMotivo,
            Asistentes = dto.Asistentes,
            MinutaAcuerdos = dto.MinutaAcuerdos,
            RegistradoPor = dto.RegistradoPor,
            FechaRegistro = DateTime.UtcNow,
        };

        _contexto.ReunionesProveedor.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<ReunionProveedorDto?> ActualizarRegistroAsync(int id, ReunionProveedorActualizarDto dto)
    {
        var registro = await _contexto.ReunionesProveedor.FirstOrDefaultAsync(r => r.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Asistentes = dto.Asistentes;
        registro.MinutaAcuerdos = dto.MinutaAcuerdos;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<ReunionProveedorImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new ReunionProveedorImportarResultadoDto();

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

        var existentes = await _contexto.ReunionesProveedor
            .Where(r => r.IdOrigen != null)
            .ToDictionaryAsync(r => r.IdOrigen!, r => r);

        void MapearCampos(ReunionProveedor registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Fecha = lectora.Fecha("Fecha") ?? registro.Fecha;
            registro.Hora = lectora.Hora("Hora");
            registro.Proveedor = lectora.Texto("Proveedor");
            registro.AsuntoMotivo = lectora.Texto("AsuntoMotivo");
            registro.Asistentes = lectora.Texto("Asistentes");
            registro.MinutaAcuerdos = lectora.Texto("MinutaAcuerdos");
            registro.RegistradoPor = lectora.Texto("RegistradoPor");
            registro.FechaRegistro = lectora.Fecha("FechaRegistro") ?? registro.FechaRegistro;
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
                var nuevo = new ReunionProveedor
                {
                    IdOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.ReunionesProveedor.Add(nuevo);
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

    private static ReunionProveedorDto MapearDto(ReunionProveedor r) => new()
    {
        Id = r.Id,
        IdOrigen = r.IdOrigen,
        Fecha = r.Fecha,
        Hora = r.Hora,
        Proveedor = r.Proveedor,
        AreaUbicacionId = r.AreaUbicacionId,
        AsuntoMotivo = r.AsuntoMotivo,
        Asistentes = r.Asistentes,
        MinutaAcuerdos = r.MinutaAcuerdos,
        RegistradoPor = r.RegistradoPor,
        FechaRegistro = r.FechaRegistro,
        FechaImportacion = r.FechaImportacion,
    };
}
