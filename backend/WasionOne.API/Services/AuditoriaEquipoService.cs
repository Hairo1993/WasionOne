using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class AuditoriaEquipoService : IAuditoriaEquipoService
{
    // Fijo mientras solo exista el piloto de IT (AreaId = 3).
    private const int AREA_ID_IT = 3;

    // Nombre lógico -> encabezado normalizado esperado en el Excel de origen.
    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Folio"] = "folio",
        ["FechaProgramada"] = "fechaprogramada",
        ["FechaRealizada"] = "fecharealizada",
        ["Planta"] = "planta",
        ["Area"] = "area",
        ["Almacen"] = "almacen",
        ["CodigoActivo"] = "codigodeactivo",
        ["DescripcionActivo"] = "descripciondeactivo",
        ["Responsable"] = "responsable",
        ["Tipo"] = "tipo",
        ["Revisados"] = "revisados",
        ["Estado"] = "estado",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con un
    // encabezado "bonito" y un valor de ejemplo por columna. Este módulo
    // no tiene una lista de "Columnas esperadas" en su texto de ayuda, así
    // que los encabezados se derivaron de los nombres de propiedad en
    // Models/AuditoriaEquipo.cs / DTOs; "Estado" usa un valor real del
    // catálogo fijo del componente (estados = Programada/Realizada/
    // Pendiente).
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Folio", "AUD-0001"),
        new("Fecha Programada", "24/09/2026"),
        new("Fecha Realizada", "25/09/2026"),
        new("Planta", "Planta 1"),
        new("Área", "Sistemas"),
        new("Almacén", "Almacén Central"),
        new("Código de Activo", "ACT-0001"),
        new("Descripción de Activo", "Laptop Dell Latitude"),
        new("Responsable", "Juan Pérez"),
        new("Tipo", "Preventiva"),
        new("Revisados", "15"),
        new("Estado", "Realizada"),
    };

    private readonly ApplicationDbContext _contexto;

    public AuditoriaEquipoService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("IT Auditorias", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<AuditoriaEquipoDto>> ObtenerAuditoriasAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.AuditoriasEquipo.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(a => a.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(a => a.FechaProgramada)
            .Select(a => new AuditoriaEquipoDto
            {
                Id = a.Id,
                Folio = a.Folio,
                AreaUbicacionId = a.AreaUbicacionId,
                FechaProgramada = a.FechaProgramada,
                FechaRealizada = a.FechaRealizada,
                Area = a.Area,
                Almacen = a.Almacen,
                CodigoActivo = a.CodigoActivo,
                DescripcionActivo = a.DescripcionActivo,
                Responsable = a.Responsable,
                Tipo = a.Tipo,
                Revisados = a.Revisados,
                Estado = a.Estado,
                FechaImportacion = a.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<AuditoriaEquipoDto?> ObtenerAuditoriaPorIdAsync(int id)
    {
        return await _contexto.AuditoriasEquipo
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AuditoriaEquipoDto
            {
                Id = a.Id,
                Folio = a.Folio,
                AreaUbicacionId = a.AreaUbicacionId,
                FechaProgramada = a.FechaProgramada,
                FechaRealizada = a.FechaRealizada,
                Area = a.Area,
                Almacen = a.Almacen,
                CodigoActivo = a.CodigoActivo,
                DescripcionActivo = a.DescripcionActivo,
                Responsable = a.Responsable,
                Tipo = a.Tipo,
                Revisados = a.Revisados,
                Estado = a.Estado,
                FechaImportacion = a.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AuditoriaEquipoDto> CrearAuditoriaAsync(AuditoriaEquipoCrearDto dto)
    {
        var auditoria = new AuditoriaEquipo
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            FechaProgramada = dto.FechaProgramada,
            FechaRealizada = dto.FechaRealizada,
            Area = dto.Area,
            Almacen = dto.Almacen,
            CodigoActivo = dto.CodigoActivo,
            DescripcionActivo = dto.DescripcionActivo,
            Responsable = dto.Responsable,
            Tipo = dto.Tipo,
            Revisados = dto.Revisados,
            Estado = "Programada",
        };

        _contexto.AuditoriasEquipo.Add(auditoria);
        await _contexto.SaveChangesAsync();

        return MapearDto(auditoria);
    }

    public async Task<AuditoriaEquipoDto?> ActualizarAuditoriaAsync(int id, AuditoriaEquipoActualizarDto dto)
    {
        var auditoria = await _contexto.AuditoriasEquipo.FirstOrDefaultAsync(a => a.Id == id);
        if (auditoria is null)
        {
            return null;
        }

        auditoria.FechaRealizada = dto.FechaRealizada;
        auditoria.Responsable = dto.Responsable;
        auditoria.Revisados = dto.Revisados;
        auditoria.Estado = dto.Estado;

        await _contexto.SaveChangesAsync();

        return MapearDto(auditoria);
    }

    public async Task<AuditoriaEquipoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new AuditoriaEquipoImportarResultadoDto();

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

        var areaUbicacionesIt = await _contexto.AreaUbicaciones
            .Where(au => au.AreaId == AREA_ID_IT)
            .ToDictionaryAsync(au => au.UbicacionId, au => au.Id);

        var existentes = await _contexto.AuditoriasEquipo
            .Where(a => a.Folio != null)
            .ToDictionaryAsync(a => a.Folio!, a => a);

        void MapearCampos(AuditoriaEquipo auditoria, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            auditoria.AreaUbicacionId = areaUbicacionId;
            auditoria.FechaProgramada = lectora.Fecha("FechaProgramada");
            auditoria.FechaRealizada = lectora.Fecha("FechaRealizada");
            auditoria.Area = lectora.Texto("Area");
            auditoria.Almacen = lectora.Texto("Almacen");
            auditoria.CodigoActivo = lectora.Texto("CodigoActivo");
            auditoria.DescripcionActivo = lectora.Texto("DescripcionActivo");
            auditoria.Responsable = lectora.Texto("Responsable");
            auditoria.Tipo = lectora.Texto("Tipo");
            auditoria.Revisados = lectora.Entero("Revisados");
            auditoria.Estado = lectora.Texto("Estado") ?? "Programada";
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var folio = lectora.Texto("Folio");
            if (string.IsNullOrWhiteSpace(folio))
            {
                resultado.Errores.Add($"Fila {numeroFila}: no trae 'Folio', se omitió.");
                resultado.Omitidos++;
                continue;
            }

            var plantaTexto = lectora.Texto("Planta");
            int? areaUbicacionId = null;
            if (plantaTexto is not null
                && ubicaciones.TryGetValue(ExcelImportUtils.Normalizar(plantaTexto), out var ubicacionId)
                && areaUbicacionesIt.TryGetValue(ubicacionId, out var areaUbicacionEncontrada))
            {
                areaUbicacionId = areaUbicacionEncontrada;
            }

            if (areaUbicacionId is null)
            {
                resultado.Errores.Add(
                    $"Fila {numeroFila} (folio {folio}): la Planta '{plantaTexto}' no coincide con ninguna ubicación del catálogo de IT, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            if (existentes.TryGetValue(folio, out var auditoriaExistente))
            {
                MapearCampos(auditoriaExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nueva = new AuditoriaEquipo
                {
                    Folio = folio,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nueva, lectora, areaUbicacionId.Value);
                _contexto.AuditoriasEquipo.Add(nueva);
                existentes[folio] = nueva;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static AuditoriaEquipoDto MapearDto(AuditoriaEquipo a) => new()
    {
        Id = a.Id,
        Folio = a.Folio,
        AreaUbicacionId = a.AreaUbicacionId,
        FechaProgramada = a.FechaProgramada,
        FechaRealizada = a.FechaRealizada,
        Area = a.Area,
        Almacen = a.Almacen,
        CodigoActivo = a.CodigoActivo,
        DescripcionActivo = a.DescripcionActivo,
        Responsable = a.Responsable,
        Tipo = a.Tipo,
        Revisados = a.Revisados,
        Estado = a.Estado,
        FechaImportacion = a.FechaImportacion,
    };
}
