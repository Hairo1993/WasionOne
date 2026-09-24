using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface ILockerService
{
    Task<IEnumerable<LockerDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<LockerDto?> ObtenerRegistroPorIdAsync(int id);

    Task<LockerDto> CrearRegistroAsync(LockerCrearDto dto);

    Task<LockerDto?> ActualizarRegistroAsync(int id, LockerActualizarDto dto);

    Task<LockerImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
