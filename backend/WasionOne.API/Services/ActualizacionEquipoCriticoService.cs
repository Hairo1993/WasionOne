using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class ActualizacionEquipoCriticoService : IActualizacionEquipoCriticoService
{
    // Fijo mientras IT (AreaId = 3) sea el único consumidor de este módulo.
    private const int AREA_ID_IT = 3;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["Planta"] = "planta",
        ["Tipo"] = "tipo",
        ["Codigo"] = "codigo",
        ["Estado"] = "estado",
        ["TeniaActualizacion"] = "teniaactualizacion",
        ["SeAplico"] = "seaplico",
        ["Observaciones"] = "observaciones",
        ["Responsable"] = "responsable",
        ["FechaRegistro"] = "fechaderegistro",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba (fuente de verdad para el import), con el
    // encabezado "bonito" (el que ya se mostraba en el texto de ayuda de
    // la pantalla) y un valor de ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("Planta", "Planta 1"),
        new("Tipo", "Firmware"),
        new("Código", "EQ-0001"),
        new("Estado", "Actualizado"),
        new("¿Tenía actualización?", "Sí"),
        new("¿Se aplicó?", "Sí"),
        new("Observaciones", "Actualización aplicada sin incidencias"),
        new("Responsable", "Juan Pérez"),
        new("Fecha de registro", "24/09/2026"),
    };

    private readonly ApplicationDbContext _contexto;

    public ActualizacionEquipoCriticoService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("IT Actualizaciones Equipos", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    private static string NormalizarCodigo(string codigo) => codigo.Trim();

    public async Task<IEnumerable<ActualizacionEquipoCriticoDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.ActualizacionesEquiposCriticos.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(a => a.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(a => a.FechaRegistro)
            .ThenBy(a => a.Codigo)
            .Select(a => new ActualizacionEquipoCriticoDto
            {
                Id = a.Id,
                AreaUbicacionId = a.AreaUbicacionId,
                Tipo = a.Tipo,
                Codigo = a.Codigo,
                Estado = a.Estado,
                TeniaActualizacion = a.TeniaActualizacion,
                SeAplico = a.SeAplico,
                Observaciones = a.Observaciones,
                Responsable = a.Responsable,
                FechaRegistro = a.FechaRegistro,
                FechaImportacion = a.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<ActualizacionEquipoCriticoDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.ActualizacionesEquiposCriticos
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new ActualizacionEquipoCriticoDto
            {
                Id = a.Id,
                AreaUbicacionId = a.AreaUbicacionId,
                Tipo = a.Tipo,
                Codigo = a.Codigo,
                Estado = a.Estado,
                TeniaActualizacion = a.TeniaActualizacion,
                SeAplico = a.SeAplico,
                Observaciones = a.Observaciones,
                Responsable = a.Responsable,
                FechaRegistro = a.FechaRegistro,
                FechaImportacion = a.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ActualizacionEquipoCriticoDto?> CrearRegistroAsync(ActualizacionEquipoCriticoCrearDto dto)
    {
        var codigo = NormalizarCodigo(dto.Codigo);
        var fechaRegistro = dto.FechaRegistro.Date;

        var yaExiste = await _contexto.ActualizacionesEquiposCriticos.AnyAsync(a =>
            a.Codigo == codigo && a.FechaRegistro == fechaRegistro);
        if (yaExiste)
        {
            return null;
        }

        var registro = new ActualizacionEquipoCritico
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            Tipo = dto.Tipo,
            Codigo = codigo,
            Estado = dto.Estado,
            TeniaActualizacion = dto.TeniaActualizacion,
            SeAplico = dto.SeAplico,
            Observaciones = dto.Observaciones,
            Responsable = dto.Responsable,
            FechaRegistro = fechaRegistro,
        };

        _contexto.ActualizacionesEquiposCriticos.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<ActualizacionEquipoCriticoDto?> ActualizarRegistroAsync(int id, ActualizacionEquipoCriticoActualizarDto dto)
    {
        var registro = await _contexto.ActualizacionesEquiposCriticos.FirstOrDefaultAsync(a => a.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.Tipo = dto.Tipo;
        registro.Estado = dto.Estado;
        registro.TeniaActualizacion = dto.TeniaActualizacion;
        registro.SeAplico = dto.SeAplico;
        registro.Observaciones = dto.Observaciones;
        registro.Responsable = dto.Responsable;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<ActualizacionEquipoCriticoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new ActualizacionEquipoCriticoImportarResultadoDto();

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

        // Llave de deduplicación: el "Código" identifica al equipo, no al
        // registro (un mismo equipo tiene varias actualizaciones en fechas
        // distintas), así que la combinación Código + Fecha de registro es
        // la llave natural (reimportar la misma combinación actualiza en
        // vez de duplicar).
        var existentes = await _contexto.ActualizacionesEquiposCriticos.ToDictionaryAsync(
            a => (Codigo: a.Codigo.Trim().ToLowerInvariant(), a.FechaRegistro.Date),
            a => a);

        void MapearCampos(
            ActualizacionEquipoCritico registro,
            ExcelFilaLectora lectora,
            int areaUbicacionId,
            string codigo,
            DateTime fechaRegistro)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.Codigo = codigo;
            registro.FechaRegistro = fechaRegistro.Date;
            registro.Tipo = lectora.Texto("Tipo") ?? string.Empty;
            registro.Estado = lectora.Texto("Estado") ?? string.Empty;
            registro.TeniaActualizacion = lectora.Booleano("TeniaActualizacion");
            registro.SeAplico = lectora.Booleano("SeAplico");
            registro.Observaciones = lectora.Texto("Observaciones");
            registro.Responsable = lectora.Texto("Responsable");
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
                && areaUbicacionesIt.TryGetValue(ubicacionId, out var areaUbicacionEncontrada))
            {
                areaUbicacionId = areaUbicacionEncontrada;
            }

            if (areaUbicacionId is null)
            {
                resultado.Errores.Add(
                    $"Fila {numeroFila}: la Planta '{plantaTexto}' no coincide con ninguna ubicación del catálogo de IT, se omitió.");
                resultado.Omitidos++;
                continue;
            }
            if (plantasPermitidas is not null && !plantasPermitidas.Contains(areaUbicacionId.Value))
            {
                resultado = new();
                resultado.Errores.Add($"Fila {numeroFila}: la Planta de esta fila no está permitida para tu usuario en este módulo. Se rechazó el archivo completo, no se importó ningún registro.");
                return resultado;
            }

            var codigoTexto = lectora.Texto("Codigo");
            var fechaRegistro = lectora.Fecha("FechaRegistro");
            if (codigoTexto is null || fechaRegistro is null)
            {
                resultado.Errores.Add($"Fila {numeroFila}: falta Código o Fecha de registro, se omitió.");
                resultado.Omitidos++;
                continue;
            }

            var codigo = NormalizarCodigo(codigoTexto);
            var llave = (Codigo: codigo.ToLowerInvariant(), fechaRegistro.Value.Date);

            if (existentes.TryGetValue(llave, out var registroExistente))
            {
                MapearCampos(registroExistente, lectora, areaUbicacionId.Value, codigo, fechaRegistro.Value);
                resultado.Actualizados++;
            }
            else
            {
                var nuevo = new ActualizacionEquipoCritico
                {
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value, codigo, fechaRegistro.Value);
                _contexto.ActualizacionesEquiposCriticos.Add(nuevo);
                existentes[llave] = nuevo;

                resultado.Creados++;
            }
        }

        await _contexto.SaveChangesAsync();
        resultado.TotalFilas = numeroFila - 1;
        return resultado;
    }

    private static ActualizacionEquipoCriticoDto MapearDto(ActualizacionEquipoCritico a) => new()
    {
        Id = a.Id,
        AreaUbicacionId = a.AreaUbicacionId,
        Tipo = a.Tipo,
        Codigo = a.Codigo,
        Estado = a.Estado,
        TeniaActualizacion = a.TeniaActualizacion,
        SeAplico = a.SeAplico,
        Observaciones = a.Observaciones,
        Responsable = a.Responsable,
        FechaRegistro = a.FechaRegistro,
        FechaImportacion = a.FechaImportacion,
    };
}
