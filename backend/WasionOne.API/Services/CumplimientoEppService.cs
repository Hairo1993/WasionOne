using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class CumplimientoEppService : ICumplimientoEppService
{

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdEpp"] = "idepp",
        ["Descripcion"] = "descripcion",
        ["Unidad"] = "unidad",
        ["Talla"] = "talla",
        ["VidaUtilCantidad"] = "vidautilcantidad",
        ["VidaUtilUnidad"] = "vidautilunidad",
        ["Minimo"] = "minimo",
        ["Maximo"] = "maximo",
        ["Existencias"] = "existencias",
        ["Solicitud"] = "solicitud",
        ["AbastecimientoStock"] = "abastecimientostock",
        ["Planta"] = "planta",
        ["FechaRegistro"] = "fechaderegistro",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("IdEpp", "EPP-0001"),
        new("Descripción", "Casco de seguridad"),
        new("Unidad", "Pieza"),
        new("Talla", "Única"),
        new("Vida Útil (Cantidad)", "12"),
        new("Vida Útil (Unidad)", "Meses"),
        new("Mínimo", "10"),
        new("Máximo", "50"),
        new("Existencias", "30"),
        new("Solicitud", "5"),
        new("Abastecimiento / Stock", "25"),
        new("Planta", "Planta 1"),
        new("Fecha de Registro", "24/09/2026"),
    };

    private readonly ApplicationDbContext _contexto;

    public CumplimientoEppService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("SegHig Cumplimiento EPP", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    private static string NormalizarIdEpp(string idEpp) => idEpp.Trim();

    public async Task<IEnumerable<CumplimientoEppDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.CumplimientosEpp.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(c => c.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(c => c.FechaRegistro)
            .ThenBy(c => c.IdEpp)
            .Select(c => new CumplimientoEppDto
            {
                Id = c.Id,
                IdEpp = c.IdEpp,
                Descripcion = c.Descripcion,
                Unidad = c.Unidad,
                Talla = c.Talla,
                VidaUtilCantidad = c.VidaUtilCantidad,
                VidaUtilUnidad = c.VidaUtilUnidad,
                Minimo = c.Minimo,
                Maximo = c.Maximo,
                Existencias = c.Existencias,
                Solicitud = c.Solicitud,
                AbastecimientoStock = c.AbastecimientoStock,
                AreaUbicacionId = c.AreaUbicacionId,
                FechaRegistro = c.FechaRegistro,
                FechaImportacion = c.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<CumplimientoEppDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.CumplimientosEpp
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CumplimientoEppDto
            {
                Id = c.Id,
                IdEpp = c.IdEpp,
                Descripcion = c.Descripcion,
                Unidad = c.Unidad,
                Talla = c.Talla,
                VidaUtilCantidad = c.VidaUtilCantidad,
                VidaUtilUnidad = c.VidaUtilUnidad,
                Minimo = c.Minimo,
                Maximo = c.Maximo,
                Existencias = c.Existencias,
                Solicitud = c.Solicitud,
                AbastecimientoStock = c.AbastecimientoStock,
                AreaUbicacionId = c.AreaUbicacionId,
                FechaRegistro = c.FechaRegistro,
                FechaImportacion = c.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CumplimientoEppDto?> CrearRegistroAsync(CumplimientoEppCrearDto dto)
    {
        var idEpp = NormalizarIdEpp(dto.IdEpp);
        var fechaRegistro = dto.FechaRegistro.Date;

        var yaExiste = await _contexto.CumplimientosEpp.AnyAsync(c =>
            c.IdEpp == idEpp && c.FechaRegistro == fechaRegistro);
        if (yaExiste)
        {
            return null;
        }

        var registro = new CumplimientoEpp
        {
            IdEpp = idEpp,
            Descripcion = dto.Descripcion,
            Unidad = dto.Unidad,
            Talla = dto.Talla,
            VidaUtilCantidad = dto.VidaUtilCantidad,
            VidaUtilUnidad = dto.VidaUtilUnidad,
            Minimo = dto.Minimo,
            Maximo = dto.Maximo,
            Existencias = dto.Existencias,
            Solicitud = dto.Solicitud,
            AbastecimientoStock = dto.AbastecimientoStock,
            AreaUbicacionId = dto.AreaUbicacionId,
            FechaRegistro = fechaRegistro,
        };

        _contexto.CumplimientosEpp.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<CumplimientoEppDto?> ActualizarRegistroAsync(int id, CumplimientoEppActualizarDto dto)
    {
        var registro = await _contexto.CumplimientosEpp.FirstOrDefaultAsync(c => c.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Descripcion = dto.Descripcion;
        registro.Unidad = dto.Unidad;
        registro.Talla = dto.Talla;
        registro.VidaUtilCantidad = dto.VidaUtilCantidad;
        registro.VidaUtilUnidad = dto.VidaUtilUnidad;
        registro.Minimo = dto.Minimo;
        registro.Maximo = dto.Maximo;
        registro.Existencias = dto.Existencias;
        registro.Solicitud = dto.Solicitud;
        registro.AbastecimientoStock = dto.AbastecimientoStock;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<CumplimientoEppImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new CumplimientoEppImportarResultadoDto();

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

        // Llave de deduplicación: el "IdEpp" identifica al EPP, no al
        // registro (un mismo EPP tiene varias versiones en fechas
        // distintas), así que la combinación IdEpp + Fecha de registro es
        // la llave natural (reimportar la misma combinación actualiza en
        // vez de duplicar) — mismo criterio que Actualizaciones de equipos
        // críticos de IT.
        var existentes = await _contexto.CumplimientosEpp.ToDictionaryAsync(
            c => (IdEpp: c.IdEpp.Trim().ToLowerInvariant(), c.FechaRegistro.Date),
            c => c);

        void MapearCampos(
            CumplimientoEpp registro,
            ExcelFilaLectora lectora,
            int areaUbicacionId,
            string idEpp,
            DateTime fechaRegistro)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.IdEpp = idEpp;
            registro.FechaRegistro = fechaRegistro.Date;
            registro.Descripcion = lectora.Texto("Descripcion") ?? string.Empty;
            registro.Unidad = lectora.Texto("Unidad");
            registro.Talla = lectora.Texto("Talla");
            registro.VidaUtilCantidad = lectora.Entero("VidaUtilCantidad");
            registro.VidaUtilUnidad = lectora.Texto("VidaUtilUnidad");
            registro.Minimo = lectora.Entero("Minimo");
            registro.Maximo = lectora.Entero("Maximo");
            registro.Existencias = lectora.Entero("Existencias");
            registro.Solicitud = lectora.Entero("Solicitud");
            registro.AbastecimientoStock = lectora.Entero("AbastecimientoStock");
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

            var idEppTexto = lectora.Texto("IdEpp");
            var fechaRegistro = lectora.Fecha("FechaRegistro");
            if (idEppTexto is null || fechaRegistro is null)
            {
                resultado.Errores.Add($"Fila {numeroFila}: falta IdEpp o Fecha de registro, se omitió.");
                resultado.Omitidos++;
                continue;
            }

            var idEpp = NormalizarIdEpp(idEppTexto);
            var llave = (IdEpp: idEpp.ToLowerInvariant(), fechaRegistro.Value.Date);

            if (existentes.TryGetValue(llave, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value, idEpp, fechaRegistro.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new CumplimientoEpp
                {
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value, idEpp, fechaRegistro.Value);
                _contexto.CumplimientosEpp.Add(nuevo);
                existentes[llave] = nuevo;

                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static CumplimientoEppDto MapearDto(CumplimientoEpp c) => new()
    {
        Id = c.Id,
        IdEpp = c.IdEpp,
        Descripcion = c.Descripcion,
        Unidad = c.Unidad,
        Talla = c.Talla,
        VidaUtilCantidad = c.VidaUtilCantidad,
        VidaUtilUnidad = c.VidaUtilUnidad,
        Minimo = c.Minimo,
        Maximo = c.Maximo,
        Existencias = c.Existencias,
        Solicitud = c.Solicitud,
        AbastecimientoStock = c.AbastecimientoStock,
        AreaUbicacionId = c.AreaUbicacionId,
        FechaRegistro = c.FechaRegistro,
        FechaImportacion = c.FechaImportacion,
    };
}
