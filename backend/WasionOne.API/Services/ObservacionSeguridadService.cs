using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class ObservacionSeguridadService : IObservacionSeguridadService
{

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Folio"] = "folio",
        ["Fecha"] = "fecha",
        ["Usuario"] = "usuario",
        ["NNomina"] = "nnomina",
        ["PersonaObservada"] = "personaobservada",
        ["Empresa"] = "empresa",
        ["Area"] = "area",
        ["Tipo"] = "tipo",
        ["Descripcion"] = "descripcion",
        ["Categorias"] = "categorias",
        ["Estado"] = "estado",
        ["Ubicacion"] = "planta",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" (el que ya se mostraba en el texto de ayuda de
    // la pantalla) y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Folio", "OBS-0001"),
        new("Fecha", "24/09/2026"),
        new("Usuario", "Juan Pérez"),
        new("N. Nómina", "12345"),
        new("Persona Observada", "María López"),
        new("Empresa", "Wasion"),
        new("Área", "Producción"),
        new("Tipo", "Acto inseguro"),
        new("Descripción", "Trabajador sin casco en zona de andamios"),
        new("Categorías", "EPP"),
        new("Estado", "Abierto"),
        new("Planta", "Planta 1"),
    };

    private readonly ApplicationDbContext _contexto;

    public ObservacionSeguridadService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Observaciones SegHig", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<ObservacionSeguridadDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.ObservacionesSeguridad.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(o => o.AreaUbicacionId == areaUbicacionId.Value);
        }

        var registros = await consulta
            .OrderByDescending(o => o.Fecha)
            .ToListAsync();

        return registros.Select(MapearDto);
    }

    public async Task<ObservacionSeguridadDto?> ObtenerRegistroPorIdAsync(int id)
    {
        var registro = await _contexto.ObservacionesSeguridad
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        return registro is null ? null : MapearDto(registro);
    }

    public async Task<ObservacionSeguridadDto?> CrearRegistroAsync(ObservacionSeguridadCrearDto dto)
    {
        var yaExiste = await _contexto.ObservacionesSeguridad.AnyAsync(o => o.Folio == dto.Folio);
        if (yaExiste)
        {
            return null;
        }

        var registro = new ObservacionSeguridad
        {
            Folio = dto.Folio,
            Fecha = dto.Fecha,
            Usuario = dto.Usuario,
            NNomina = dto.NNomina,
            PersonaObservada = dto.PersonaObservada,
            Empresa = dto.Empresa,
            Area = dto.Area,
            Tipo = dto.Tipo,
            Descripcion = dto.Descripcion,
            Categorias = dto.Categorias,
            Estado = string.IsNullOrWhiteSpace(dto.Estado) ? "Abierto" : dto.Estado,
            AreaUbicacionId = dto.AreaUbicacionId,
        };

        _contexto.ObservacionesSeguridad.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<ObservacionSeguridadDto?> ActualizarRegistroAsync(int id, ObservacionSeguridadActualizarDto dto)
    {
        var registro = await _contexto.ObservacionesSeguridad.FirstOrDefaultAsync(o => o.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Fecha = dto.Fecha;
        registro.Usuario = dto.Usuario;
        registro.NNomina = dto.NNomina;
        registro.PersonaObservada = dto.PersonaObservada;
        registro.Empresa = dto.Empresa;
        registro.Area = dto.Area;
        registro.Tipo = dto.Tipo;
        registro.Descripcion = dto.Descripcion;
        registro.Categorias = dto.Categorias;
        registro.Estado = dto.Estado;
        registro.AreaUbicacionId = dto.AreaUbicacionId;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<ObservacionSeguridadImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new ObservacionSeguridadImportarResultadoDto();

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

        var areaUbicacionesSeguridadHigiene = await _contexto.AreaUbicaciones
            .Where(au => au.AreaId == Modulos.AreaIdSeguridadHigiene)
            .ToDictionaryAsync(au => au.UbicacionId, au => au.Id);

        var existentes = await _contexto.ObservacionesSeguridad
            .ToDictionaryAsync(o => o.Folio, o => o);

        void MapearCampos(ObservacionSeguridad registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Fecha = lectora.Fecha("Fecha") ?? registro.Fecha;
            registro.Usuario = lectora.Texto("Usuario") ?? registro.Usuario;
            registro.NNomina = lectora.Texto("NNomina");
            registro.PersonaObservada = lectora.Texto("PersonaObservada") ?? registro.PersonaObservada;
            registro.Empresa = lectora.Texto("Empresa") ?? registro.Empresa;
            registro.Area = lectora.Texto("Area");
            registro.Tipo = lectora.Texto("Tipo") ?? registro.Tipo;
            registro.Descripcion = lectora.Texto("Descripcion") ?? registro.Descripcion;
            registro.Categorias = lectora.Texto("Categorias") ?? registro.Categorias;
            registro.Estado = lectora.Texto("Estado") ?? "Abierto";
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

            var plantaTexto = lectora.Texto("Ubicacion");
            int? areaUbicacionId = null;
            if (plantaTexto is not null
                && ubicaciones.TryGetValue(ExcelImportUtils.Normalizar(plantaTexto), out var ubicacionId)
                && areaUbicacionesSeguridadHigiene.TryGetValue(ubicacionId, out var areaUbicacionEncontrada))
            {
                areaUbicacionId = areaUbicacionEncontrada;
            }

            if (areaUbicacionId is null)
            {
                resultado.Errores.Add(
                    $"Fila {numeroFila} (Folio {folio}): la Planta '{plantaTexto}' no coincide con ninguna ubicación del catálogo de Seguridad e Higiene, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            if (existentes.TryGetValue(folio, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new ObservacionSeguridad
                {
                    Folio = folio,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.ObservacionesSeguridad.Add(nuevo);
                existentes[folio] = nuevo;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static ObservacionSeguridadDto MapearDto(ObservacionSeguridad o) => new()
    {
        Id = o.Id,
        Folio = o.Folio,
        Fecha = o.Fecha,
        Usuario = o.Usuario,
        NNomina = o.NNomina,
        PersonaObservada = o.PersonaObservada,
        Empresa = o.Empresa,
        Area = o.Area,
        Tipo = o.Tipo,
        Descripcion = o.Descripcion,
        Categorias = o.Categorias,
        Estado = o.Estado,
        AreaUbicacionId = o.AreaUbicacionId,
        FechaImportacion = o.FechaImportacion,
    };
}
