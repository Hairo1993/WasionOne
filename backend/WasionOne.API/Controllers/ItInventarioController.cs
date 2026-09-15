using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/it/inventario")]
[Authorize]
public class ItInventarioController : ControllerBase
{
    private readonly IInventarioService _servicio;

    public ItInventarioController(IInventarioService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventarioEquipoDto>>> ObtenerEquipos([FromQuery] int? areaUbicacionId)
    {
        return Ok(await _servicio.ObtenerEquiposAsync(areaUbicacionId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InventarioEquipoDto>> ObtenerEquipo(int id)
    {
        var equipo = await _servicio.ObtenerEquipoPorIdAsync(id);
        return equipo is null ? NotFound() : Ok(equipo);
    }

    [HttpPost]
    public async Task<ActionResult<InventarioEquipoDto>> CrearEquipo(InventarioEquipoCrearDto dto)
    {
        var equipo = await _servicio.CrearEquipoAsync(dto);
        return CreatedAtAction(nameof(ObtenerEquipo), new { id = equipo.Id }, equipo);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<InventarioEquipoDto>> ActualizarEquipo(int id, InventarioEquipoActualizarDto dto)
    {
        var equipo = await _servicio.ActualizarEquipoAsync(id, dto);
        return equipo is null ? NotFound() : Ok(equipo);
    }
}
