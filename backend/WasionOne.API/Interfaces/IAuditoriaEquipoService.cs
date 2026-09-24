using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IAuditoriaEquipoService
{
    Task<IEnumerable<AuditoriaEquipoDto>> ObtenerAuditoriasAsync(int? areaUbicacionId);

    Task<AuditoriaEquipoDto?> ObtenerAuditoriaPorIdAsync(int id);

    Task<AuditoriaEquipoDto> CrearAuditoriaAsync(AuditoriaEquipoCrearDto dto);

    Task<AuditoriaEquipoDto?> ActualizarAuditoriaAsync(int id, AuditoriaEquipoActualizarDto dto);

    Task<AuditoriaEquipoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
