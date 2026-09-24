using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IEstatusLegalPlantaService
{
    Task<IEnumerable<EstatusLegalPlantaDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<EstatusLegalPlantaDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe un registro para esa combinación Planta +
    // Requerimiento legal + Última fecha de realización (se debe editar el
    // existente, no duplicar).
    Task<EstatusLegalPlantaDto?> CrearRegistroAsync(EstatusLegalPlantaCrearDto dto);

    Task<EstatusLegalPlantaDto?> ActualizarRegistroAsync(int id, EstatusLegalPlantaActualizarDto dto);

    Task<EstatusLegalPlantaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
