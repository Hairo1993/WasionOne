using WasionOne.API.DTOs;
using WasionOne.API.Helpers;

namespace WasionOne.API.Interfaces;

public interface IRespaldoService
{
    Task<IEnumerable<RespaldoDto>> ObtenerRespaldosAsync(int? areaUbicacionId);

    Task<RespaldoDto?> ObtenerRespaldoPorIdAsync(int id);

    Task<RespaldoDto> CrearRespaldoAsync(RespaldoCrearDto dto);

    Task<RespaldoDto?> ActualizarRespaldoAsync(int id, RespaldoActualizarDto dto);

    Task<RespaldoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel, IReadOnlySet<int>? plantasPermitidas);

    // Plantilla descargable de Excel (24/sep/2026) — mismas columnas que
    // espera ImportarDesdeExcelAsync, con una fila de ejemplo.
    IReadOnlyList<ExcelPlantillaUtils.ColumnaPlantilla> ObtenerColumnasPlantilla();

    byte[] GenerarPlantillaExcel();
}
