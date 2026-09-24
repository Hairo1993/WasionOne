using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IAccidenteTrabajoService
{
    Task<IEnumerable<AccidenteTrabajoDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<AccidenteTrabajoDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe un registro con ese Folio (se debe editar
    // el existente, no duplicar).
    Task<AccidenteTrabajoDto?> CrearRegistroAsync(AccidenteTrabajoCrearDto dto);

    Task<AccidenteTrabajoDto?> ActualizarRegistroAsync(int id, AccidenteTrabajoActualizarDto dto);

    Task<AccidenteTrabajoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
