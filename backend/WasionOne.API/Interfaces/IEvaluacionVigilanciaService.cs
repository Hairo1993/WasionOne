using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IEvaluacionVigilanciaService
{
    Task<IEnumerable<EvaluacionVigilanciaDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<EvaluacionVigilanciaDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe una evaluación para esa combinación
    // Planta + Proveedor + Fecha (se debe editar la existente, no duplicar).
    Task<EvaluacionVigilanciaDto?> CrearRegistroAsync(EvaluacionVigilanciaCrearDto dto);

    Task<EvaluacionVigilanciaDto?> ActualizarRegistroAsync(int id, EvaluacionVigilanciaActualizarDto dto);

    Task<EvaluacionVigilanciaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
