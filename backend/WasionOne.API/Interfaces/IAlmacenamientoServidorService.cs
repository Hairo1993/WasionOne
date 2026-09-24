using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IAlmacenamientoServidorService
{
    Task<IEnumerable<AlmacenamientoServidorDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<AlmacenamientoServidorDto?> ObtenerRegistroPorIdAsync(int id);

    Task<AlmacenamientoServidorDto> CrearRegistroAsync(AlmacenamientoServidorCrearDto dto);

    Task<AlmacenamientoServidorDto?> ActualizarRegistroAsync(int id, AlmacenamientoServidorActualizarDto dto);

    Task<AlmacenamientoServidorImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
