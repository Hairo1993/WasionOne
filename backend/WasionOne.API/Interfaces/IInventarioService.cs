using WasionOne.API.DTOs;

namespace WasionOne.API.Interfaces;

public interface IInventarioService
{
    Task<IEnumerable<InventarioEquipoDto>> ObtenerEquiposAsync(int? areaUbicacionId);

    Task<InventarioEquipoDto?> ObtenerEquipoPorIdAsync(int id);

    Task<InventarioEquipoDto> CrearEquipoAsync(InventarioEquipoCrearDto dto);

    Task<InventarioEquipoDto?> ActualizarEquipoAsync(int id, InventarioEquipoActualizarDto dto);
}
