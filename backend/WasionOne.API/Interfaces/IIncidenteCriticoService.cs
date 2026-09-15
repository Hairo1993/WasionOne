using WasionOne.API.DTOs;

namespace WasionOne.API.Interfaces;

public interface IIncidenteCriticoService
{
    Task<IEnumerable<IncidenteCriticoDto>> ObtenerIncidentesAsync(int? areaUbicacionId);

    Task<IncidenteCriticoDto?> ObtenerIncidentePorIdAsync(int id);

    Task<IncidenteCriticoDto> CrearIncidenteAsync(IncidenteCriticoCrearDto dto);

    Task<IncidenteCriticoDto?> ActualizarIncidenteAsync(int id, IncidenteCriticoActualizarDto dto);

    Task<IncidenteCriticoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel);
}
