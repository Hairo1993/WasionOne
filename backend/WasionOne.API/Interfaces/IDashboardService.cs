using WasionOne.API.DTOs;

namespace WasionOne.API.Interfaces;

public interface IDashboardService
{
    // ubicacionIds trae Ids de Ubicación física (catálogo Ubicacion, la
    // misma lista que catalogosService.obtenerUbicaciones() en el
    // frontend) — NO AreaUbicacionId. El servicio resuelve internamente,
    // por cada Área, los AreaUbicacionId específicos que le corresponden a
    // esas Ubicaciones. Null o vacío = sin filtro (todas las Ubicaciones).
    // Null si la Dirección no existe.
    Task<DashboardDireccionDto?> ObtenerDireccionAsync(int direccionId, DateTime? fechaDesde, DateTime? fechaHasta, List<int>? ubicacionIds);

    // Null si el Departamento no existe.
    Task<DashboardDepartamentoDto?> ObtenerDepartamentoAsync(int departamentoId, DateTime? fechaDesde, DateTime? fechaHasta, List<int>? ubicacionIds);

    // Usado por el Controller para autorizar a un Director sobre un
    // Departamento que no es el suyo (debe pertenecer a su misma
    // Dirección). Null si el Departamento no existe.
    Task<int?> ObtenerDireccionIdDeDepartamentoAsync(int departamentoId);
}
