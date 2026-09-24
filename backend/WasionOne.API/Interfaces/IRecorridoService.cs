using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IRecorridoService
{
    Task<IEnumerable<RecorridoDto>> ObtenerRecorridosAsync(int? areaUbicacionId);

    Task<RecorridoDto?> ObtenerRecorridoPorIdAsync(int id);

    Task<RecorridoDto> CrearRecorridoAsync(RecorridoCrearDto dto);

    Task<RecorridoDto?> ActualizarRecorridoAsync(int id, RecorridoActualizarDto dto);

    Task<RecorridoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
