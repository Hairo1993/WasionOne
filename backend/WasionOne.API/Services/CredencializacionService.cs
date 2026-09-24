using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WasionOne.API.Data;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;
using WasionOne.API.Models;

namespace WasionOne.API.Services;

public class CredencializacionService : ICredencializacionService
{
    // Fijo mientras solo exista Seguridad Patrimonial (AreaId = 1) en esta Área.
    private const int AREA_ID_SEGURIDAD_PATRIMONIAL = 1;

    private static readonly Dictionary<string, string> Columnas = new()
    {
        ["IdOrigen"] = "id",
        ["NombreCompleto"] = "nombrecompleto",
        ["Empresa"] = "empresa",
        ["TipoAcceso"] = "tipodeacceso",
        ["Ubicacion"] = "planta",
        ["Identificacion"] = "identificacion",
        ["PersonaQueVisita"] = "personaquevisita",
        ["MotivoVisita"] = "motivodevisita",
        ["AreaDeTrabajo"] = "areadetrabajo",
        ["FechaHoraEntrada"] = "fechahoraentrada",
        ["FechaHoraSalida"] = "fechahorasalida",
        ["Estado"] = "estado",
    };

    // Plantilla descargable (24/sep/2026): mismo orden y mismos campos que
    // "Columnas" de arriba, con el encabezado "bonito" y un valor de
    // ejemplo por columna.
    private static readonly IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> PlantillaColumnas = new List<ExcelPlantillaUtils.ColumnaPlantilla>
    {
        new("ID", "CRED-0001"),
        new("Nombre Completo", "Luis Hernández"),
        new("Empresa", "Proveedor ABC"),
        new("Tipo de Acceso", "Visitante"),
        new("Planta", "Planta 1"),
        new("Identificacion", "INE-12345678"),
        new("Persona que Visita", "Roberto Sánchez"),
        new("Motivo de Visita", "Revisión de equipo"),
        new("Área de Trabajo", "Almacén"),
        new("Fecha/Hora Entrada", "24/09/2026 09:00"),
        new("Fecha/Hora Salida", "24/09/2026 12:00"),
        new("Estado", "Dentro de instalaciones"),
    };

    private readonly ApplicationDbContext _contexto;

    public CredencializacionService(ApplicationDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla() => PlantillaColumnas;

    public byte[] GenerarPlantillaExcel()
    {
        using var libro = ExcelPlantillaUtils.GenerarLibro("Seg Credencialización", PlantillaColumnas);
        return ExcelPlantillaUtils.GuardarComoBytes(libro);
    }

    public async Task<IEnumerable<CredencializacionDto>> ObtenerRegistrosAsync(int? areaUbicacionId)
    {
        var consulta = _contexto.Credencializaciones.AsNoTracking().AsQueryable();

        if (areaUbicacionId.HasValue)
        {
            consulta = consulta.Where(c => c.AreaUbicacionId == areaUbicacionId.Value);
        }

        return await consulta
            .OrderByDescending(c => c.FechaHoraEntrada)
            .Select(c => new CredencializacionDto
            {
                Id = c.Id,
                IdOrigen = c.IdOrigen,
                NombreCompleto = c.NombreCompleto,
                Empresa = c.Empresa,
                TipoAcceso = c.TipoAcceso,
                AreaUbicacionId = c.AreaUbicacionId,
                Identificacion = c.Identificacion,
                PersonaQueVisita = c.PersonaQueVisita,
                MotivoVisita = c.MotivoVisita,
                AreaDeTrabajo = c.AreaDeTrabajo,
                FechaHoraEntrada = c.FechaHoraEntrada,
                FechaHoraSalida = c.FechaHoraSalida,
                Estado = c.Estado,
                FechaImportacion = c.FechaImportacion,
            })
            .ToListAsync();
    }

    public async Task<CredencializacionDto?> ObtenerRegistroPorIdAsync(int id)
    {
        return await _contexto.Credencializaciones
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CredencializacionDto
            {
                Id = c.Id,
                IdOrigen = c.IdOrigen,
                NombreCompleto = c.NombreCompleto,
                Empresa = c.Empresa,
                TipoAcceso = c.TipoAcceso,
                AreaUbicacionId = c.AreaUbicacionId,
                Identificacion = c.Identificacion,
                PersonaQueVisita = c.PersonaQueVisita,
                MotivoVisita = c.MotivoVisita,
                AreaDeTrabajo = c.AreaDeTrabajo,
                FechaHoraEntrada = c.FechaHoraEntrada,
                FechaHoraSalida = c.FechaHoraSalida,
                Estado = c.Estado,
                FechaImportacion = c.FechaImportacion,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CredencializacionDto> CrearRegistroAsync(CredencializacionCrearDto dto)
    {
        var registro = new Credencializacion
        {
            AreaUbicacionId = dto.AreaUbicacionId,
            NombreCompleto = dto.NombreCompleto,
            Empresa = dto.Empresa,
            TipoAcceso = dto.TipoAcceso,
            Identificacion = dto.Identificacion,
            PersonaQueVisita = dto.PersonaQueVisita,
            MotivoVisita = dto.MotivoVisita,
            AreaDeTrabajo = dto.AreaDeTrabajo,
            FechaHoraEntrada = dto.FechaHoraEntrada,
            FechaHoraSalida = dto.FechaHoraSalida,
            Estado = string.IsNullOrWhiteSpace(dto.Estado) ? "Dentro de instalaciones" : dto.Estado,
        };

        _contexto.Credencializaciones.Add(registro);
        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<CredencializacionDto?> ActualizarRegistroAsync(int id, CredencializacionActualizarDto dto)
    {
        var registro = await _contexto.Credencializaciones.FirstOrDefaultAsync(c => c.Id == id);
        if (registro is null)
        {
            return null;
        }

        registro.FechaHoraSalida = dto.FechaHoraSalida;
        registro.Estado = dto.Estado;

        await _contexto.SaveChangesAsync();

        return MapearDto(registro);
    }

    public async Task<CredencializacionImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas)
    {
        var resultado = new CredencializacionImportarResultadoDto();

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

        var existentes = await _contexto.Credencializaciones
            .Where(c => c.IdOrigen != null)
            .ToDictionaryAsync(c => c.IdOrigen!, c => c);

        void MapearCampos(Credencializacion registro, ExcelFilaLectora lectora, int areaUbicacionId)
        {
            registro.AreaUbicacionId = areaUbicacionId;
            registro.NombreCompleto = lectora.Texto("NombreCompleto") ?? registro.NombreCompleto;
            registro.Empresa = lectora.Texto("Empresa");
            registro.TipoAcceso = lectora.Texto("TipoAcceso");
            registro.Identificacion = lectora.Texto("Identificacion");
            registro.PersonaQueVisita = lectora.Texto("PersonaQueVisita");
            registro.MotivoVisita = lectora.Texto("MotivoVisita");
            registro.AreaDeTrabajo = lectora.Texto("AreaDeTrabajo");
            registro.FechaHoraEntrada = lectora.Fecha("FechaHoraEntrada") ?? registro.FechaHoraEntrada;
            registro.FechaHoraSalida = lectora.Fecha("FechaHoraSalida");
            registro.Estado = lectora.Texto("Estado") ?? "Dentro de instalaciones";
        }

        var numeroFila = 1; // fila 1 = encabezado
        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            numeroFila++;
            var lectora = new ExcelFilaLectora(fila, indicePorEncabezado, Columnas);

            var nombre = lectora.Texto("NombreCompleto");
            if (string.IsNullOrWhiteSpace(nombre))
            {
                resultado.Errores.Add($"Fila {numeroFila}: no trae 'Nombre Completo', se omitió.");
                resultado.Omitidos++;
                continue;
            }

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
                    $"Fila {numeroFila} ({nombre}): la Planta '{ubicacionTexto}' no coincide con ninguna ubicación del catálogo de Seguridad Patrimonial, se omitió.");
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
                var nuevo = new Credencializacion
                {
                    IdOrigen = idOrigen,
                    FechaImportacion = DateTime.UtcNow,
                };
                MapearCampos(nuevo, lectora, areaUbicacionId.Value);
                _contexto.Credencializaciones.Add(nuevo);
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

    private static CredencializacionDto MapearDto(Credencializacion c) => new()
    {
        Id = c.Id,
        IdOrigen = c.IdOrigen,
        NombreCompleto = c.NombreCompleto,
        Empresa = c.Empresa,
        TipoAcceso = c.TipoAcceso,
        AreaUbicacionId = c.AreaUbicacionId,
        Identificacion = c.Identificacion,
        PersonaQueVisita = c.PersonaQueVisita,
        MotivoVisita = c.MotivoVisita,
        AreaDeTrabajo = c.AreaDeTrabajo,
        FechaHoraEntrada = c.FechaHoraEntrada,
        FechaHoraSalida = c.FechaHoraSalida,
        Estado = c.Estado,
        FechaImportacion = c.FechaImportacion,
    };
}
