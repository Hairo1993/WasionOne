using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IValeSalidaService
{
    Task<IEnumerable<ValeSalidaDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<ValeSalidaDto?> ObtenerRegistroPorIdAsync(int id);

    Task<ValeSalidaDto> CrearRegistroAsync(ValeSalidaCrearDto dto);

    Task<ValeSalidaDto?> ActualizarRegistroAsync(int id, ValeSalidaActualizarDto dto);

    Task<ValeSalidaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
