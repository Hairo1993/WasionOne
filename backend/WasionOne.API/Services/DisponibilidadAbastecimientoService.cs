using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class DisponibilidadAbastecimientoService : IDisponibilidadAbastecimientoService
{
    // Fijo mientras solo exista Administración (AreaId de Administración)
    // en esta Área.
    // TODO AJUSTAR: -1 es un valor temporal. Reemplazar por el AreaId real
    // de la Área "Administración" (consultar GET /api/catalogos/areas o la
    // pantalla /admin/catalogos) antes de usar este módulo en producción.
    private const int AREA_ID_ADMINISTRACION = 11; // Area "Administracion" (AreaId real confirmado 22/sep/2026)

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["FechaEntrega"] = "fechaentrega",
        ["Departamento"] = "departamento",
        ["Material"] = "material",
        ["Especificar"] = "especificar",
        ["Unidad"] = "unidad",
        ["CantidadEntregada"] = "cantidadentregada",
        ["Comentarios"] = "comentarios",
        ["Ubicacion"] = "planta",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" (el que ya se mostraba en el texto de ayuda de
    // la pantalla) y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Fecha entrega", "24/09/2026"),
        new("Departamento", "Producción"),
        new("Material", "Guantes de nitrilo"),
        new("Especificar", "Talla M"),
        new("Unidad", "Caja"),
        new("Cantidad entregada", "10"),
        new("Comentarios", "Entrega completa"),
        new("Planta", "Planta 1"),
    };

    private readonly ApplicationDbContext _contexto;

    public DisponibilidadAbastecimientoService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Adm Disponibilidad Abastecimiento", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<DisponibilidadAbastecimientoDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.DisponibilidadesAbastecimiento.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(d => d.AreaUbicacionId == areaUbicacionId.Value);
        }

        var registros = await consulta
            .OrderByDescending(d => d.FechaEntrega)
            .ToListAsync();

        return registros.Select(MapearDto);
    }

    public async Task<DisponibilidadAbastecimientoDto?> ObtenerRegistroPorIdAsync(int id)
    {
        var registro = await _contexto.DisponibilidadesAbastecimiento
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        return registro is null ? null : MapearDto(registro);
    }

    public async Task<DisponibilidadAbastecimientoDto> CrearRegistroAsync(DisponibilidadAbastecimientoCrearDto dto)
    {
        var registro = new DisponibilidadAbastecimiento
        {
            FechaEntrega = dto.FechaEntrega,
            Departamento = dto.Departamento,
            Material = dto.Material,
            Especificar = dto.Especificar,
            Unidad = dto.Unidad,
            CantidadEntregada = dto.CantidadEntregada,
            Comentarios = dto.Comentarios,
            AreaUbicacionId = dto.AreaUbicacionId,
        };

        _contexto.DisponibilidadesAbastecimiento.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<DisponibilidadAbastecimientoDto?> ActualizarRegistroAsync(int id, DisponibilidadAbastecimientoActualizarDto dto)
    {
        var registro = await _contexto.DisponibilidadesAbastecimiento.FirstOrDefaultAsync(d => d.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.FechaEntrega = dto.FechaEntrega;
        registro.Departamento = dto.Departamento;
        registro.Material = dto.Material;
        registro.Especificar = dto.Especificar;
        registro.Unidad = dto.Unidad;
        registro.CantidadEntregada = dto.CantidadEntregada;
        registro.Comentarios = dto.Comentarios;
        registro.AreaUbicacionId = dto.AreaUbicacionId;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    // Este módulo no tiene llave de negocio ni "ID" de origen (ver Models/
    // DisponibilidadAbastecimiento.cs): cada fila importada SIEMPRE crea un
    // registro nuevo, no hay actualiza-o-crea aquí — es una simplificación
    // deliberada, no un descuido.
    public async Task<DisponibilidadAbastecimientoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new DisponibilidadAbastecimientoImportarResultadoDto();

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

            var material = lectora.Texto("Material");
            if (string.IsNullOrWhiteSpace(material))
            {
                resultado.Errores.Add($"Fila {numeroFila}: no trae 'Material', se omitió.");
                resultado.Omitidos++;
                continue;
            }

            var nuevo = new DisponibilidadAbastecimiento
            {
                AreaUbicacionId = areaUbicacionId.Value,
                FechaEntrega = lectora.Fecha("FechaEntrega") ?? DateTime.UtcNow.Date,
                Departamento = lectora.Texto("Departamento"),
                Material = material,
                Especificar = lectora.Texto("Especificar"),
                Unidad = lectora.Texto("Unidad"),
                CantidadEntregada = lectora.Numero("CantidadEntregada"),
                Comentarios = lectora.Texto("Comentarios"),
                FechaImportacion = DateTime.UtcNow,
            };
            _contexto.DisponibilidadesAbastecimiento.Add(nuevo);
            resultado.Creados++;
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static DisponibilidadAbastecimientoDto MapearDto(DisponibilidadAbastecimiento d) => new()
    {
        Id = d.Id,
        FechaEntrega = d.FechaEntrega,
        Departamento = d.Departamento,
        Material = d.Material,
        Especificar = d.Especificar,
        Unidad = d.Unidad,
        CantidadEntregada = d.CantidadEntregada,
        Comentarios = d.Comentarios,
        AreaUbicacionId = d.AreaUbicacionId,
        FechaImportacion = d.FechaImportacion,
    };
}
