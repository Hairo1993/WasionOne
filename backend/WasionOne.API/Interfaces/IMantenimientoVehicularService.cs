using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IMantenimientoVehicularService
{
    Task<IEnumerable<MantenimientoVehicularDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<MantenimientoVehicularDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe un registro con ese VIN (se debe editar el
    // existente, no duplicar).
    Task<MantenimientoVehicularDto?> CrearRegistroAsync(MantenimientoVehicularCrearDto dto);

    Task<MantenimientoVehicularDto?> ActualizarRegistroAsync(int id, MantenimientoVehicularActualizarDto dto);

    Task<MantenimientoVehicularImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
