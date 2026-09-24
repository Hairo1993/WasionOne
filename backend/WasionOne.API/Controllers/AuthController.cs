using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _servicio;
    private readonly IUsuarioContexto _usuarioContexto;

    public AuthController(IAuthService servicio, IUsuarioContexto usuarioContexto)
    {
        _servicio = servicio;
        _usuarioContexto = usuarioContexto;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginRespuestaDto>> Login(LoginDto credenciales)
    {
        var respuesta = await _servicio.ValidarCredencialesAsync(credenciales);
        return respuesta is null ? Unauthorized() : Ok(respuesta);
    }

    // Cambio de contraseña de autoservicio: cualquier usuario autenticado
    // (sin importar su rol) cambia su PROPIA contraseña, identificado por el
    // nombre de usuario que trae su JWT — no puede tocar la de nadie más.
    // Distinto de PUT api/usuarios/{id}/password, que es exclusivo de
    // Superadmin y sirve para restablecer la de otro usuario sin pedir la
    // anterior.
    [HttpPut("cambiar-password")]
    [Authorize]
    public async Task<IActionResult> CambiarMiPassword(CambiarMiPasswordDto dto)
    {
        var nombreUsuario = _usuarioContexto.NombreUsuario;
        if (nombreUsuario is null)
        {
            return Unauthorized();
        }

        var resultado = await _servicio.CambiarMiPasswordAsync(nombreUsuario, dto.PasswordActual, dto.PasswordNueva);
        return resultado.Exito ? NoContent() : BadRequest(resultado.Error);
    }
}
