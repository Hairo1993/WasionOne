using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _servicio;

    public AuthController(IAuthService servicio)
    {
        _servicio = servicio;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginRespuestaDto>> Login(LoginDto credenciales)
    {
        var respuesta = await _servicio.ValidarCredencialesAsync(credenciales);
        return respuesta is null ? Unauthorized() : Ok(respuesta);
    }
}