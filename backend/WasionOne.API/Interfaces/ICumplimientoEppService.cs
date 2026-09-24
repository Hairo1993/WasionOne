using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface ICumplimientoEppService
{
    Task<IEnumerable<CumplimientoEppDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<CumplimientoEppDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe un registro para ese IdEpp en esa Fecha de
    // registro (se debe editar el existente, no duplicar).
    Task<CumplimientoEppDto?> CrearRegistroAsync(CumplimientoEppCrearDto dto);

    Task<CumplimientoEppDto?> ActualizarRegistroAsync(int id, CumplimientoEppActualizarDto dto);

    Task<CumplimientoEppImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
