using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IObservacionSeguridadService
{
    Task<IEnumerable<ObservacionSeguridadDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<ObservacionSeguridadDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe un registro con ese Folio (se debe editar
    // el existente, no duplicar).
    Task<ObservacionSeguridadDto?> CrearRegistroAsync(ObservacionSeguridadCrearDto dto);

    Task<ObservacionSeguridadDto?> ActualizarRegistroAsync(int id, ObservacionSeguridadActualizarDto dto);

    Task<ObservacionSeguridadImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo. Expuesta
    // como lista (no solo como archivo ya armado) para que
    // ImportacionMasivaService pueda reutilizar las mismas columnas al
    // construir el archivo combinado con todos los indicadores.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
