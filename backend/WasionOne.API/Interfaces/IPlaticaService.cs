using WasionOne.API.DTOs;

namespace WasionOne.API.Interfaces;

public interface IPlaticaService
{
    Task<IEnumerable<PlaticaDto>> ObtenerPlaticasAsync(int? areaUbicacionId);

    Task<PlaticaDto?> ObtenerPlaticaPorIdAsync(int id);

    Task<PlaticaDto> CrearPlaticaAsync(PlaticaCrearDto dto);

    Task<PlaticaDto?> ActualizarPlaticaAsync(int id, PlaticaActualizarDto dto);

    Task<PlaticaImportarResultadoDto> ImportarDesdeExcelAsync(Stream archivoExcel);
}
