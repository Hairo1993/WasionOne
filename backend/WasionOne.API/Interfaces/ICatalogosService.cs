using WasionOne.API.DTOs;

namespace WasionOne.API.Interfaces;

public interface ICatalogosService
{
    Task<IEnumerable<DireccionDto>> ObtenerDireccionesAsync();
    Task<DireccionDto?> ObtenerDireccionPorIdAsync(int id);
    Task<DireccionDto> CrearDireccionAsync(DireccionCrearDto dto);
    Task<DireccionDto?> ActualizarDireccionAsync(int id, DireccionActualizarDto dto);
    Task<ResultadoOperacionDto> EliminarDireccionAsync(int id);

    Task<IEnumerable<DepartamentoDto>> ObtenerDepartamentosAsync(int? direccionId);
    Task<DepartamentoDto?> ObtenerDepartamentoPorIdAsync(int id);
    Task<DepartamentoDto> CrearDepartamentoAsync(DepartamentoCrearDto dto);
    Task<DepartamentoDto?> ActualizarDepartamentoAsync(int id, DepartamentoActualizarDto dto);
    Task<ResultadoOperacionDto> EliminarDepartamentoAsync(int id);

    Task<IEnumerable<AreaDto>> ObtenerAreasAsync(int? departamentoId);
    Task<AreaDto?> ObtenerAreaPorIdAsync(int id);
    Task<AreaDto> CrearAreaAsync(AreaCrearDto dto);
    Task<AreaDto?> ActualizarAreaAsync(int id, AreaActualizarDto dto);
    Task<ResultadoOperacionDto> EliminarAreaAsync(int id);

    Task<IEnumerable<UbicacionDto>> ObtenerUbicacionesAsync();
    Task<UbicacionDto?> ObtenerUbicacionPorIdAsync(int id);
    Task<UbicacionDto> CrearUbicacionAsync(UbicacionCrearDto dto);
    Task<UbicacionDto?> ActualizarUbicacionAsync(int id, UbicacionActualizarDto dto);
    Task<ResultadoOperacionDto> EliminarUbicacionAsync(int id);

    Task<IEnumerable<AreaUbicacionDto>> ObtenerAreaUbicacionesAsync(int? areaId);
    Task<AreaUbicacionDto> CrearAreaUbicacionAsync(AreaUbicacionCrearDto dto);
    Task<ResultadoOperacionDto> EliminarAreaUbicacionAsync(int id);

    Task<IEnumerable<ModuloDto>> ObtenerModulosAsync();
}
