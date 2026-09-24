using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IEvaluacionProveedorSegHigieneService
{
    Task<IEnumerable<EvaluacionProveedorSegHigieneDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<EvaluacionProveedorSegHigieneDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe una evaluación para esa combinación
    // Proveedor + Planta + Mes (se debe editar la existente, no duplicar).
    Task<EvaluacionProveedorSegHigieneDto?> CrearRegistroAsync(EvaluacionProveedorSegHigieneCrearDto dto);

    Task<EvaluacionProveedorSegHigieneDto?> ActualizarRegistroAsync(int id, EvaluacionProveedorSegHigieneActualizarDto dto);

    Task<EvaluacionProveedorSegHigieneImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
