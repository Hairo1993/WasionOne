using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/it/inventario")]
[Authorize]
public class ItInventarioController : ControllerBase
{
    private readonly IInventarioService _servicio;
    private readonly IUsuarioContexto _usuarioContexto;

    private const string MODULO = WasionOne.API.Models.Modulos.ItInventario;

    public ItInventarioController(IInventarioService servicio, IUsuarioContexto usuarioContexto)
    {
        _servicio = servicio;
        _usuarioContexto = usuarioContexto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventarioEquipoDto>>> ObtenerEquipos([FromQuery] int? areaUbicacionId)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        var registros = await _servicio.ObtenerEquiposAsync(areaUbicacionId);
        return Ok(registros.Where(r => AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, r.AreaUbicacionId)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InventarioEquipoDto>> ObtenerEquipo(int id)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var equipo = await _servicio.ObtenerEquipoPorIdAsync(id);
        if (equipo is null)
        {
            return NotFound();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        if (!AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, equipo.AreaUbicacionId))
        {
            return NotFound();
        }

        return Ok(equipo);
    }

    [HttpPost]
    public async Task<ActionResult<InventarioEquipoDto>> CrearEquipo(InventarioEquipoCrearDto dto)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        if (!AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, dto.AreaUbicacionId))
        {
            return Forbid();
        }

        var equipo = await _servicio.CrearEquipoAsync(dto);
        return CreatedAtAction(nameof(ObtenerEquipo), new { id = equipo.Id }, equipo);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<InventarioEquipoDto>> ActualizarEquipo(int id, InventarioEquipoActualizarDto dto)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var existente = await _servicio.ObtenerEquipoPorIdAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        if (!AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, existente.AreaUbicacionId))
        {
            return NotFound();
        }

        var equipo = await _servicio.ActualizarEquipoAsync(id, dto);
        return equipo is null ? NotFound() : Ok(equipo);
    }
}
