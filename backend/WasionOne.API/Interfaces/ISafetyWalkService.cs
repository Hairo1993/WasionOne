using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface ISafetyWalkService
{
    Task<IEnumerable<SafetyWalkDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<SafetyWalkDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe un registro con esa Fecha exacta + Planta
    // (se debe editar el existente, no duplicar).
    Task<SafetyWalkDto?> CrearRegistroAsync(SafetyWalkCrearDto dto);

    Task<SafetyWalkDto?> ActualizarRegistroAsync(int id, SafetyWalkActualizarDto dto);

    Task<SafetyWalkImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
