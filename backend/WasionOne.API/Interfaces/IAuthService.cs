using WasionOne.API.DTOs;

namespace WasionOne.API.Interfaces;

public interface IAuthService
{
    Task<LoginRespuestaDto?> ValidarCredencialesAsync(LoginDto credenciales);

    // Cambio de contraseña de autoservicio: el propio usuario autenticado
    // cambia su contraseña (por eso exige la actual) — distinto del
    // restablecimiento que hace un Superadmin desde /admin/usuarios sobre
    // cualquier otro usuario (ese no pide la anterior).
    Task<ResultadoOperacionDto> CambiarMiPasswordAsync(string nombreUsuario, string passwordActual, string passwordNueva);
}
