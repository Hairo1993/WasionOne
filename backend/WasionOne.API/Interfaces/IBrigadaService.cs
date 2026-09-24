using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

// PROPUESTO — pendiente de confirmar con el usuario (ver comentario en
// Models/Brigada.cs).
public interface IBrigadaService
{
    Task<IEnumerable<BrigadaDto>> ObtenerRegistrosAsync(int? areaUbicacionId);

    Task<BrigadaDto?> ObtenerRegistroPorIdAsync(int id);

    // Regresa null si ya existe un registro con ese Folio (se debe editar
    // el existente, no duplicar).
    Task<BrigadaDto?> CrearRegistroAsync(BrigadaCrearDto dto);

    Task<BrigadaDto?> ActualizarRegistroAsync(int id, BrigadaActualizarDto dto);

    Task<BrigadaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
