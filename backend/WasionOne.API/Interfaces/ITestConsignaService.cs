using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface ITestConsignaService
{
    Task<IEnumerable<TestConsignaDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<TestConsignaDto?> ObtenerRegistroPorIdAsync(int id);

    Task<TestConsignaDto> CrearRegistroAsync(TestConsignaCrearDto dto);

    Task<TestConsignaDto?> ActualizarRegistroAsync(int id, TestConsignaActualizarDto dto);

    Task<TestConsignaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
