using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface ITiempoRespuestaResolucionService
{
    Task<IEnumerable<TiempoRespuestaResolucionDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<TiempoRespuestaResolucionDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe un registro con ese Folio (se debe editar
    // el existente, no duplicar).
    Task<TiempoRespuestaResolucionDto?> CrearRegistroAsync(TiempoRespuestaResolucionCrearDto dto);

    Task<TiempoRespuestaResolucionDto?> ActualizarRegistroAsync(int id, TiempoRespuestaResolucionActualizarDto dto);

    Task<TiempoRespuestaResolucionImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
