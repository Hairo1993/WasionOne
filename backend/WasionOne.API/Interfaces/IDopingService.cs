using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IDopingService
{
    Task<IEnumerable<DopingDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<DopingDto?> ObtenerRegistroPorIdAsync(int id);

    Task<DopingDto> CrearRegistroAsync(DopingCrearDto dto);

    Task<DopingDto?> ActualizarRegistroAsync(int id, DopingActualizarDto dto);

    Task<DopingImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
