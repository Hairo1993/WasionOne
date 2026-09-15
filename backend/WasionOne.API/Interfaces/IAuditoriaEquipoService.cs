using WasionOne.API.DTOs;

namespace WasionOne.API.Interfaces;

public interface IAuditoriaEquipoService
{
    Task<IEnumerable<AuditoriaEquipoDto>> ObtenerAuditoriasAsync(int? areaUbicacionId);

    Task<AuditoriaEquipoDto?> ObtenerAuditoriaPorIdAsync(int id);

    Task<AuditoriaEquipoDto> CrearAuditoriaAsync(AuditoriaEquipoCrearDto dto);

    Task<AuditoriaEquipoDto?> ActualizarAuditoriaAsync(int id, AuditoriaEquipoActualizarDto dto);

    Task<AuditoriaEquipoImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel);
}
