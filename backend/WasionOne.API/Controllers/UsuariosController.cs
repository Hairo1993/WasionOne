using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

// Solo CEO y Director pueden administrar usuarios. Más adelante (cuando se
// construyan las pantallas de gestión por Sub-área) se podrá afinar esto
// para permitir que un Responsable de Área/Sub-área gestione usuarios
// exclusivamente dentro de su propio nodo de la jerarquía.
[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "CEO,Director")]
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
}