using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IAlcoholimetriaService
{
    Task<IEnumerable<AlcoholimetriaDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<AlcoholimetriaDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe un registro para esa combinación de
    // Planta + Fecha + Turno (se debe editar el existente, no duplicar).
    Task<AlcoholimetriaDto?> CrearRegistroAsync(AlcoholimetriaCrearDto dto);

    Task<AlcoholimetriaDto?> ActualizarRegistroAsync(int id, AlcoholimetriaActualizarDto dto);

    Task<AlcoholimetriaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
