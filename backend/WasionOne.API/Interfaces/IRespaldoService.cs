using WasionOne.API.DTOs;

namespace WasionOne.API.Interfaces;

public interface IRespaldoService
{
    Task<IEnumerable<RespaldoDto>> ObtenerRespaldosAsync(int? areaUbicacionId);

    Task<RespaldoDto?> ObtenerRespaldoPorIdAsync(int id);

    Task<RespaldoDto> CrearRespaldoAsync(RespaldoCrearDto dto);

    Task<RespaldoDto?> ActualizarRespaldoAsync(int id, RespaldoActualizarDto dto);

    Task<RespaldoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel);
}
