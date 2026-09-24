using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class AlcoholimetriaService : IAlcoholimetriaService
{
    // Fijo mientras solo exista Seguridad Patrimonial (AreaId = 1) en esta Área.
    private const int AREA_ID_SEGURIDAD_PATRIMONIAL = 1;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Fecha"] = "fecha",
        ["Ubicacion"] = "planta",
        ["Turno"] = "turno",
        ["Cantidad"] = "cantidad",
        ["Positivo"] = "positivo",
        ["Negativo"] = "negativo",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba, con el encabezado "bonito" y un valor de
    // ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Fecha", "24/09/2026"),
        new("Planta", "Planta 1"),
        new("Turno", "Turno 1"),
        new("Cantidad", "10"),
        new("Positivo", "0"),
        new("Negativo", "10"),
    };

    private readonly ApplicationDbContext _contexto;

    public AlcoholimetriaService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Seg Alcoholimetría", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    // Si no viene una cantidad explícita, se calcula como Positivo + Negativo.
    private static int CalcularCantidad(int? cantidadExplicita, int positivo, int negativo)
    {
        return cantidadExplicita ?? (positivo + negativo);
    }

    private static string NormalizarTurno(string turno) => turno.Trim();

    public async Task<IEnumerable<AlcoholimetriaDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.Alcoholimetrias.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(a => a.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(a => a.Fecha)
            .ThenBy(a => a.Turno)
            .Select(a => new AlcoholimetriaDto
            {
                Id = a.Id,
                AreaUbicacionId = a.AreaUbicacionId,
                Fecha = a.Fecha,
                Turno = a.Turno,
                Cantidad = a.Cantidad,
                Positivo = a.Positivo,
                Negativo = a.Negativo,
                FechaImportacion = a.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<AlcoholimetriaDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.Alcoholimetrias
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AlcoholimetriaDto
            {
                Id = a.Id,
                AreaUbicacionId = a.AreaUbicacionId,
                Fecha = a.Fecha,
                Turno = a.Turno,
                Cantidad = a.Cantidad,
                Positivo = a.Positivo,
                Negativo = a.Negativo,
                FechaImportacion = a.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AlcoholimetriaDto?> CrearRegistroAsync(AlcoholimetriaCrearDto dto)
    {
        var turno = NormalizarTurno(dto.Turno);

        var yaExiste = await _contexto.Alcoholimetrias.AnyAsync(a =>
            a.AreaUbicacionId == dto.AreaUbicacionId && a.Fecha == dto.Fecha && a.Turno == turno);
        if (yaExiste)
        {
            return null;
        }

        var registro = new Alcoholimetria
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Fecha = dto.Fecha,
            Turno = turno,
            Cantidad = CalcularCantidad(dto.Cantidad, dto.Positivo, dto.Negativo),
            Positivo = dto.Positivo,
            Negativo = dto.Negativo,
        };

        _contexto.Alcoholimetrias.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<AlcoholimetriaDto?> ActualizarRegistroAsync(int id, AlcoholimetriaActualizarDto dto)
    {
        var registro = await _contexto.Alcoholimetrias.FirstOrDefaultAsync(a => a.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Positivo = dto.Positivo;
        registro.Negativo = dto.Negativo;
        registro.Cantidad = CalcularCantidad(dto.Cantidad, dto.Positivo, dto.Negativo);

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<AlcoholimetriaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new AlcoholimetriaImportarResultadoDto();

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

        // Llave de deduplicación: no hay un "ID" de origen para este registro
        // agregado, así que la combinación Planta + Fecha + Turno hace las
        // veces de llave natural (reimportar el mismo día/turno actualiza en
        // vez de duplicar).
        var existentes = await _contexto.Alcoholimetrias.ToDictionaryAsync(
            a => (a.AreaUbicacionId, a.Fecha.Date, Turno: a.Turno.Trim().ToLowerInvariant()),
            a => a);

        void MapearCampos(Alcoholimetria registro, ExcelFilaLectora lectora, int areaUbicacionId, DateTime fecha, string turno)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Fecha = fecha.Date;
            registro.Turno = turno;
            var positivo = lectora.Entero("Positivo") ?? 0;
            var negativo = lectora.Entero("Negativo") ?? 0;
            registro.Positivo = positivo;
            registro.Negativo = negativo;
            registro.Cantidad = CalcularCantidad(lectora.Entero("Cantidad"), positivo, negativo);
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

            var fecha = lectora.Fecha("Fecha");
            var turnoTexto = lectora.Texto("Turno");
            if (fecha is null || turnoTexto is null)
            {
                resultado.Errores.Add($"Fila {numeroFila}: falta Fecha o Turno, se omitió.");
                resultado.Omitidos++;
                continue;
            }

            var turno = NormalizarTurno(turnoTexto);
            var llave = (areaUbicacionId.Value, fecha.Value.Date, Turno: turno.ToLowerInvariant());

            if (existentes.TryGetValue(llave, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value, fecha.Value, turno);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new Alcoholimetria
                {
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value, fecha.Value, turno);
                _contexto.Alcoholimetrias.Add(nuevo);
                existentes[llave] = nuevo;

                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static AlcoholimetriaDto MapearDto(Alcoholimetria a) => new()
    {
        Id = a.Id,
        AreaUbicacionId = a.AreaUbicacionId,
        Fecha = a.Fecha,
        Turno = a.Turno,
        Cantidad = a.Cantidad,
        Positivo = a.Positivo,
        Negativo = a.Negativo,
        FechaImportacion = a.FechaImportacion,
    };
}
