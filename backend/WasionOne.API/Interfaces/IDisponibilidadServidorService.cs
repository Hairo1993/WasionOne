using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IDisponibilidadServidorService
{
    Task<IEnumerable<DisponibilidadServidorDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<DisponibilidadServidorDto?> ObtenerRegistroPorIdAsync(int id);

    Task<DisponibilidadServidorDto> CrearRegistroAsync(DisponibilidadServidorCrearDto dto);

    Task<DisponibilidadServidorDto?> ActualizarRegistroAsync(int id, DisponibilidadServidorActualizarDto dto);

    Task<DisponibilidadServidorImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
