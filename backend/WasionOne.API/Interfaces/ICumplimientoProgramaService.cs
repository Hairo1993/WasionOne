using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface ICumplimientoProgramaService
{
    Task<IEnumerable<CumplimientoProgramaDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<CumplimientoProgramaDto?> ObtenerRegistroPorIdAsync(int id);

    Task<CumplimientoProgramaDto> CrearRegistroAsync(CumplimientoProgramaCrearDto dto);

    Task<CumplimientoProgramaDto?> ActualizarRegistroAsync(int id, CumplimientoProgramaActualizarDto dto);

    Task<CumplimientoProgramaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
