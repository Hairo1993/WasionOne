using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IEstacionamientoService
{
    Task<IEnumerable<EstacionamientoDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<EstacionamientoDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe un registro con ese No. Marbete (se debe
    // editar el existente, no duplicar).
    Task<EstacionamientoDto?> CrearRegistroAsync(EstacionamientoCrearDto dto);

    Task<EstacionamientoDto?> ActualizarRegistroAsync(int id, EstacionamientoActualizarDto dto);

    Task<EstacionamientoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
