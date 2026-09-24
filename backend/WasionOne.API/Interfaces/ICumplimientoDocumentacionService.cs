using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface ICumplimientoDocumentacionService
{
    Task<IEnumerable<CumplimientoDocumentacionDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<CumplimientoDocumentacionDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe un registro con ese Code (se debe editar
    // el existente, no duplicar).
    Task<CumplimientoDocumentacionDto?> CrearRegistroAsync(CumplimientoDocumentacionCrearDto dto);

    Task<CumplimientoDocumentacionDto?> ActualizarRegistroAsync(int id, CumplimientoDocumentacionActualizarDto dto);

    Task<CumplimientoDocumentacionImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
