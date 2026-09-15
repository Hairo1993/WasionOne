using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class IncidenteCriticoService : IIncidenteCriticoService
{
    // Fijo mientras solo exista el piloto de IT (AreaId = 3).
    private const int AREA_ID_IT = 3;

    // Nombre lógico -> encabezado normalizado esperado en el Excel de origen.
    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdFalla"] = "iddefalla",
        ["Fecha"] = "fecha",
        ["HoraInicio"] = "horadeinicio",
        ["HoraFin"] = "horadefinalizacion",
        ["Duracion"] = "duracion",
        ["Planta"] = "planta",
        ["Departamento"] = "departamento",
        ["Area"] = "area",
        ["Linea"] = "linea",
        ["Severidad"] = "severidad",
        ["Tipo"] = "tipo",
        ["Descripcion"] = "descripcion",
        ["Responsable"] = "responsable",
        ["Causa"] = "causa",
        ["Detalles"] = "detalles",
        ["Contramedida"] = "contramedida",
        ["Estado"] = "estado",
    };

    private readonly ApplicationDbContext _contexto;

    public IncidenteCriticoService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<IEnumerable<IncidenteCriticoDto>> ObtenerIncidentesAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.IncidentesCriticos.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(i => i.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(i => i.Fecha)
            .Select(i => new IncidenteCriticoDto
            {
                Id = i.Id,
                IdFallaOrigen = i.IdFallaOrigen,
                AreaUbicacionId = i.AreaUbicacionId,
                Fecha = i.Fecha,
                HoraInicio = i.HoraInicio,
                HoraFin = i.HoraFin,
                DuracionHoras = i.DuracionHoras,
                DepartamentoId = i.DepartamentoId,
                Area = i.Area,
                Linea = i.Linea,
                Severidad = i.Severidad,
                Tipo = i.Tipo,
                Descripcion = i.Descripcion,
                Responsable = i.Responsable,
                Causa = i.Causa,
                Detalles = i.Detalles,
                Contramedida = i.Contramedida,
                Estado = i.Estado,
                FechaImportacion = i.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<IncidenteCriticoDto?> ObtenerIncidentePorIdAsync(int id)
    {
        return await _contexto.IncidentesCriticos
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new IncidenteCriticoDto
            {
                Id = i.Id,
                IdFallaOrigen = i.IdFallaOrigen,
                AreaUbicacionId = i.AreaUbicacionId,
                Fecha = i.Fecha,
                HoraInicio = i.HoraInicio,
                HoraFin = i.HoraFin,
                DuracionHoras = i.DuracionHoras,
                DepartamentoId = i.DepartamentoId,
                Area = i.Area,
                Linea = i.Linea,
                Severidad = i.Severidad,
                Tipo = i.Tipo,
                Descripcion = i.Descripcion,
                Responsable = i.Responsable,
                Causa = i.Causa,
                Detalles = i.Detalles,
                Contramedida = i.Contramedida,
                Estado = i.Estado,
                FechaImportacion = i.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IncidenteCriticoDto> CrearIncidenteAsync(IncidenteCriticoCrearDto dto)
    {
        var incidente = new IncidenteCritico
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Fecha = dto.Fecha,
            HoraInicio = dto.HoraInicio,
            HoraFin = dto.HoraFin,
            DuracionHoras = dto.DuracionHoras,
            DepartamentoId = dto.DepartamentoId,
            Area = dto.Area,
            Linea = dto.Linea,
            Severidad = dto.Severidad,
            Tipo = dto.Tipo,
            Descripcion = dto.Descripcion,
            Responsable = dto.Responsable,
            Causa = dto.Causa,
            Detalles = dto.Detalles,
            Contramedida = dto.Contramedida,
            Estado = "Abierto",
        };

        _contexto.IncidentesCriticos.Add(incidente);
        await _contexto.SaveChangesAsync();

        return MapearDto(incidente);
    }

    public async Task<IncidenteCriticoDto?> ActualizarIncidenteAsync(int id, IncidenteCriticoActualizarDto dto)
    {
        var incidente = await _contexto.IncidentesCriticos.FirstOrDefaultAsync(i => i.Id == id);
        if (incidente is null)
        {
            return null;
        }

        incidente.Estado = dto.Estado;
        incidente.Causa = dto.Causa;
        incidente.Detalles = dto.Detalles;
        incidente.Contramedida = dto.Contramedida;
        incidente.HoraFin = dto.HoraFin ?? incidente.HoraFin;
        incidente.DuracionHoras = dto.DuracionHoras ?? incidente.DuracionHoras;

        await _contexto.SaveChangesAsync();

        return MapearDto(incidente);
    }

    public async Task<IncidenteCriticoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel)
    {
        var resultado = new IncidenteCriticoImportarResultadoDto();

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

        var departamentos = await _contexto.Departamentos
            .ToDictionaryAsync(d => ExcelImportUtils.Normalizar(d.Nombre), d => d.Id);

        var existentes = await _contexto.IncidentesCriticos
            .Where(i => i.IdFallaOrigen != null)
            .ToDictionaryAsync(i => i.IdFallaOrigen!, i => i);

        void MapearCampos(IncidenteCritico incidente, ExcelFilaLectora lectora, int areaUbicacionId, int numeroFila, string idFalla)
        {
            incidente.AreaUbicacionId = areaUbicacionId;
            incidente.Fecha = lectora.Fecha("Fecha") ?? incidente.Fecha;
            incidente.HoraInicio = lectora.Hora("HoraInicio");
            incidente.HoraFin = lectora.Hora("HoraFin");
            incidente.DuracionHoras = lectora.Numero("Duracion");
            incidente.Area = lectora.Texto("Area");
            incidente.Linea = lectora.Texto("Linea");
            incidente.Severidad = lectora.Texto("Severidad");
            incidente.Tipo = lectora.Texto("Tipo");
            incidente.Descripcion = lectora.Texto("Descripcion") ?? string.Empty;
            incidente.Responsable = lectora.Texto("Responsable");
            incidente.Causa = lectora.Texto("Causa");
            incidente.Detalles = lectora.Texto("Detalles");
            incidente.Contramedida = lectora.Texto("Contramedida");
            incidente.Estado = lectora.Texto("Estado") ?? "Abierto";

            var departamentoTexto = lectora.Texto("Departamento");
            if (string.IsNullOrWhiteSpace(departamentoTexto) || ExcelImportUtils.Normalizar(departamentoTexto) == "todos")
            {
                incidente.DepartamentoId = null;
            }
            else if (departamentos.TryGetValue(ExcelImportUtils.Normalizar(departamentoTexto), out var departamentoId))
            {
                incidente.DepartamentoId = departamentoId;
            }
            else
            {
                incidente.DepartamentoId = null;
                resultado.Errores.Add(
                    $"Fila {numeroFila} (falla {idFalla}): el Departamento '{departamentoTexto}' no coincide con el catálogo, se dejó sin departamento asignado.");
            }
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var idFalla = lectora.Texto("IdFalla");
            if (string.IsNullOrWhiteSpace(idFalla))
            {
                resultado.Errores.Add($"Fila {numeroFila}: no trae 'ID de Falla', se omitió.");
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
                    $"Fila {numeroFila} (falla {idFalla}): la Planta '{plantaTexto}' no coincide con ninguna ubicación del catálogo de IT, se omitió.");
                resultado.Omitidos++;
                continue;
            }

            if (existentes.TryGetValue(idFalla, out var incidenteExistente))
            {
                MapearCampos(incidenteExistente, lectora, areaUbicacionId.Value, numeroFila, idFalla);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new IncidenteCritico
                {
                    IdFallaOrigen = idFalla,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value, numeroFila, idFalla);
                _contexto.IncidentesCriticos.Add(nuevo);
                existentes[idFalla] = nuevo;
                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static IncidenteCriticoDto MapearDto(IncidenteCritico i) => new()
    {
        Id = i.Id,
        IdFallaOrigen = i.IdFallaOrigen,
        AreaUbicacionId = i.AreaUbicacionId,
        Fecha = i.Fecha,
        HoraInicio = i.HoraInicio,
        HoraFin = i.HoraFin,
        DuracionHoras = i.DuracionHoras,
        DepartamentoId = i.DepartamentoId,
        Area = i.Area,
        Linea = i.Linea,
        Severidad = i.Severidad,
        Tipo = i.Tipo,
        Descripcion = i.Descripcion,
        Responsable = i.Responsable,
        Causa = i.Causa,
        Detalles = i.Detalles,
        Contramedida = i.Contramedida,
        Estado = i.Estado,
        FechaImportacion = i.FechaImportacion,
    };
}
