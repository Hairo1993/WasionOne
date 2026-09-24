using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

// 19/sep/2026: la administración de usuarios pasó a ser exclusiva del rol
// Superadmin (antes la tenían también CEO y Director). CEO y Director ya
// no pueden crear/editar usuarios ni cambiar contraseñas.
//
// Nota: se usa el literal "Superadmin" (en vez de interpolar la constante
// de Roles) para no depender de la evaluación de cadenas interpoladas
// como constantes en atributos.
[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "Superadmin")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _servicio;

    public UsuariosController(IUsuarioService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> ObtenerUsuarios()
    {
        return Ok(await _servicio.ObtenerUsuariosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> ObtenerUsuario(int id)
    {
        var usuario = await _servicio.ObtenerUsuarioPorIdAsync(id);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> CrearUsuario(UsuarioCrearDto dto)
    {
        var usuario = await _servicio.CrearUsuarioAsync(dto);
        return CreatedAtAction(nameof(ObtenerUsuario), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> ActualizarUsuario(int id, UsuarioActualizarDto dto)
    {
        var usuario = await _servicio.ActualizarUsuarioAsync(id, dto);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPut("{id:int}/password")]
    public async Task<IActionResult> CambiarPassword(int id, UsuarioCambiarPasswordDto dto)
    {
        var exito = await _servicio.CambiarPasswordAsync(id, dto.NuevaPassword);
        return exito ? NoContent() : NotFound();
    }
}
