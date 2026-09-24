using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IDisponibilidadAbastecimientoService
{
    Task<IEnumerable<DisponibilidadAbastecimientoDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<DisponibilidadAbastecimientoDto?> ObtenerRegistroPorIdAsync(int id);

    Task<DisponibilidadAbastecimientoDto> CrearRegistroAsync(DisponibilidadAbastecimientoCrearDto dto);

    Task<DisponibilidadAbastecimientoDto?> ActualizarRegistroAsync(int id, DisponibilidadAbastecimientoActualizarDto dto);

    Task<DisponibilidadAbastecimientoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
