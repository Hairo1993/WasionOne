using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IDisponibilidadRedService
{
    Task<IEnumerable<DisponibilidadRedDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<DisponibilidadRedDto?> ObtenerRegistroPorIdAsync(int id);

    Task<DisponibilidadRedDto> CrearRegistroAsync(DisponibilidadRedCrearDto dto);

    Task<DisponibilidadRedDto?> ActualizarRegistroAsync(int id, DisponibilidadRedActualizarDto dto);

    Task<DisponibilidadRedImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
