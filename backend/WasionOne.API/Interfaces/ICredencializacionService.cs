using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface ICredencializacionService
{
    Task<IEnumerable<CredencializacionDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<CredencializacionDto?> ObtenerRegistroPorIdAsync(int id);

    Task<CredencializacionDto> CrearRegistroAsync(CredencializacionCrearDto dto);

    Task<CredencializacionDto?> ActualizarRegistroAsync(int id, CredencializacionActualizarDto dto);

    Task<CredencializacionImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
