using WasionOne.API.DTOs;

namespace WasionOne.API.Interfaces;

public interface ICatalogosService
{
    Task<IEnumerable<DireccionDto>> ObtenerDireccionesAsync();
    Task<DireccionDto?> ObtenerDireccionPorIdAsync(int id);
    Task<DireccionDto> CrearDireccionAsync(DireccionCrearDto dto);

    Task<IEnumerable<DepartamentoDto>> ObtenerDepartamentosAsync(int? direccionId);
    Task<DepartamentoDto?> ObtenerDepartamentoPorIdAsync(int id);
    Task<DepartamentoDto> CrearDepartamentoAsync(DepartamentoCrearDto dto);

    Task<IEnumerable<AreaDto>> ObtenerAreasAsync(int? departamentoId);
    Task<AreaDto?> ObtenerAreaPorIdAsync(int id);
    Task<AreaDto> CrearAreaAsync(AreaCrearDto dto);

    Task<IEnumerable<UbicacionDto>> ObtenerUbicacionesAsync();
    Task<UbicacionDto?> ObtenerUbicacionPorIdAsync(int id);
    Task<UbicacionDto> CrearUbicacionAsync(UbicacionCrearDto dto);

    Task<IEnumerable<AreaUbicacionDto>> ObtenerAreaUbicacionesAsync(int? areaId);
    Task<AreaUbicacionDto> CrearAreaUbicacionAsync(AreaUbicacionCrearDto dto);
}