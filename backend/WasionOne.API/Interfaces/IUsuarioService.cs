using WasionOne.API.DTOs;

namespace WasionOne.API.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> ObtenerUsuariosAsync();

    Task<UsuarioDto?> ObtenerUsuarioPorIdAsync(int id);

    Task<UsuarioDto> CrearUsuarioAsync(UsuarioCrearDto dto);

    Task<UsuarioDto?> ActualizarUsuarioAsync(int id, UsuarioActualizarDto dto);

    Task<bool> CambiarPasswordAsync(int id, string nuevaPassword);
}
