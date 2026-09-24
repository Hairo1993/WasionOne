using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IPlaticaService
{
    Task<IEnumerable<PlaticaDto>> ObtenerPlaticasAsync(int? areaUbicacionId);

    Task<PlaticaDto?> ObtenerPlaticaPorIdAsync(int id);

    Task<PlaticaDto> CrearPlaticaAsync(PlaticaCrearDto dto);

    Task<PlaticaDto?> ActualizarPlaticaAsync(int id, PlaticaActualizarDto dto);

    Task<PlaticaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
