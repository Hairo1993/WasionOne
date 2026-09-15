using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/it/platicas")]
[Authorize]
public class ItPlaticasController : ControllerBase
{
    private readonly IPlaticaService _servicio;

    public ItPlaticasController(IPlaticaService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlaticaDto>>> ObtenerPlaticas([FromQuery] int? areaUbicacionId)
    {
        return Ok(await _servicio.ObtenerPlaticasAsync(areaUbicacionId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlaticaDto>> ObtenerPlatica(int id)
    {
        var platica = await _servicio.ObtenerPlaticaPorIdAsync(id);
        return platica is null ? NotFound() : Ok(platica);
    }

    [HttpPost]
    public async Task<ActionResult<PlaticaDto>> CrearPlatica(PlaticaCrearDto dto)
    {
        var platica = await _servicio.CrearPlaticaAsync(dto);
        return CreatedAtAction(nameof(ObtenerPlatica), new { id = platica.Id }, platica);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PlaticaDto>> ActualizarPlatica(int id, PlaticaActualizarDto dto)
    {
        var platica = await _servicio.ActualizarPlaticaAsync(id, dto);
        return platica is null ? NotFound() : Ok(platica);
    }

    [HttpPost("importar")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<PlaticaImportarResultadoDto>> ImportarPlaticas(IFormFile? archivo)
    {
        if (archivo is null || archivo.Length == 0)
        {
            return BadRequest("Debes adjuntar un archivo Excel (.xlsx).");
        }

        using var flujo = archivo.OpenReadStream();
        var resultado = await _servicio.ImportarDesdeExcelAsync(flujo);
        return Ok(resultado);
    }
}
