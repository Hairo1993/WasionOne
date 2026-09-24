using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IReunionProveedorService
{
    Task<IEnumerable<ReunionProveedorDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<ReunionProveedorDto?> ObtenerRegistroPorIdAsync(int id);

    Task<ReunionProveedorDto> CrearRegistroAsync(ReunionProveedorCrearDto dto);

    Task<ReunionProveedorDto?> ActualizarRegistroAsync(int id, ReunionProveedorActualizarDto dto);

    Task<ReunionProveedorImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
