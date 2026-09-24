using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IIncidenteCriticoService
{
    Task<IEnumerable<IncidenteCriticoDto>> ObtenerIncidentesAsync(int? areaUbicacionId);

    Task<IncidenteCriticoDto?> ObtenerIncidentePorIdAsync(int id);

    Task<IncidenteCriticoDto> CrearIncidenteAsync(IncidenteCriticoCrearDto dto);

    Task<IncidenteCriticoDto?> ActualizarIncidenteAsync(int id, IncidenteCriticoActualizarDto dto);

    Task<IncidenteCriticoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
