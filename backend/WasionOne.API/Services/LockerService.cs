using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class LockerService : ILockerService
{
    // Fijo mientras solo exista Seguridad Patrimonial (AreaId = 1) en esta Área.
    private const int AREA_ID_SEGURIDAD_PATRIMONIAL = 1;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "id",
        ["Fecha"] = "fecha",
        ["Ubicacion"] = "planta",
        ["NumeroLocker"] = "numerodelocker",
        ["NoNomina"] = "nonomina",
        ["Nombre"] = "nombre",
        ["HoraInicio"] = "horainicio",
        ["HoraTermino"] = "horatermino",
        ["Resultado"] = "resultado",
        ["Detalles"] = "detalles",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("ID", "LOC-0001"),
        new("Fecha", "24/09/2026"),
        new("Planta", "Planta 1"),
        new("Número de Locker", "125"),
        new("No. Nómina", "45678"),
        new("Nombre", "Juan Pérez"),
        new("Hora Inicio", "08:30"),
        new("Hora Término", "08:45"),
        new("Resultado", "Sin novedad"),
        new("Detalles", "Revisión rutinaria sin hallazgos"),
    };

    private readonly ApplicationDbContext _contexto;

    public LockerService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Seg Lockers", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<LockerDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.Lockers.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(l => l.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(l => l.Fecha)
            .Select(l => new LockerDto
            {
                Id = l.Id,
                IdOrigen = l.IdOrigen,
                Fecha = l.Fecha,
                NumeroLocker = l.NumeroLocker,
                NoNomina = l.NoNomina,
                Nombre = l.Nombre,
                AreaUbicacionId = l.AreaUbicacionId,
                HoraInicio = l.HoraInicio,
                HoraTermino = l.HoraTermino,
                Resultado = l.Resultado,
                Detalles = l.Detalles,
                FechaImportacion = l.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<LockerDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.Lockers
            .AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new LockerDto
            {
                Id = l.Id,
                IdOrigen = l.IdOrigen,
                Fecha = l.Fecha,
                NumeroLocker = l.NumeroLocker,
                NoNomina = l.NoNomina,
                Nombre = l.Nombre,
                AreaUbicacionId = l.AreaUbicacionId,
                HoraInicio = l.HoraInicio,
                HoraTermino = l.HoraTermino,
                Resultado = l.Resultado,
                Detalles = l.Detalles,
                FechaImportacion = l.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<LockerDto> CrearRegistroAsync(LockerCrearDto dto)
    {
        var registro = new Locker
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Fecha = dto.Fecha,
            NumeroLocker = dto.NumeroLocker,
            NoNomina = dto.NoNomina,
            Nombre = dto.Nombre,
            HoraInicio = dto.HoraInicio,
            HoraTermino = dto.HoraTermino,
            Resultado = string.IsNullOrWhiteSpace(dto.Resultado) ? "Sin novedad" : dto.Resultado,
            Detalles = dto.Detalles,
        };

        _contexto.Lockers.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<LockerDto?> ActualizarRegistroAsync(int id, LockerActualizarDto dto)
    {
        var registro = await _contexto.Lockers.FirstOrDefaultAsync(l => l.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.HoraInicio = dto.HoraInicio;
        registro.HoraTermino = dto.HoraTermino;
        registro.Resultado = dto.Resultado;
        registro.Detalles = dto.Detalles;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<LockerImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new LockerImportarResultadoDto();

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

        var existentes = await _contexto.Lockers
            .Where(l => l.IdOrigen != null)
            .ToDictionaryAsync(l => l.IdOrigen!, l => l);

        void MapearCampos(Locker registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Fecha = lectora.Fecha("Fecha") ?? registro.Fecha;
            registro.NumeroLocker = lectora.Texto("NumeroLocker");
            registro.NoNomina = lectora.Texto("NoNomina");
            registro.Nombre = lectora.Texto("Nombre");
            registro.HoraInicio = lectora.Hora("HoraInicio");
            registro.HoraTermino = lectora.Hora("HoraTermino");
            registro.Resultado = lectora.Texto("Resultado") ?? "Sin novedad";
            registro.Detalles = lectora.Texto("Detalles");
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
                var nuevo = new Locker
                {
                    IdOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.Lockers.Add(nuevo);
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

    private static LockerDto MapearDto(Locker l) => new()
    {
        Id = l.Id,
        IdOrigen = l.IdOrigen,
        Fecha = l.Fecha,
        NumeroLocker = l.NumeroLocker,
        NoNomina = l.NoNomina,
        Nombre = l.Nombre,
        AreaUbicacionId = l.AreaUbicacionId,
        HoraInicio = l.HoraInicio,
        HoraTermino = l.HoraTermino,
        Resultado = l.Resultado,
        Detalles = l.Detalles,
        FechaImportacion = l.FechaImportacion,
    };
}
