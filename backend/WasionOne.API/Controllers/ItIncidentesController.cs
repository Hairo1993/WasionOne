using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/it/incidentes")]
[Authorize]
public class ItIncidentesController : ControllerBase
{
    private readonly IIncidenteCriticoService _servicio;

    public ItIncidentesController(IIncidenteCriticoService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IncidenteCriticoDto>>> ObtenerIncidentes([FromQuery] int? areaUbicacionId)
    {
        return Ok(await _servicio.ObtenerIncidentesAsync(areaUbicacionId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IncidenteCriticoDto>> ObtenerIncidente(int id)
    {
        var incidente = await _servicio.ObtenerIncidentePorIdAsync(id);
        return incidente is null ? NotFound() : Ok(incidente);
    }

    [HttpPost]
    public async Task<ActionResult<IncidenteCriticoDto>> CrearIncidente(IncidenteCriticoCrearDto dto)
    {
        var incidente = await _servicio.CrearIncidenteAsync(dto);
        return CreatedAtAction(nameof(ObtenerIncidente), new { id = incidente.Id }, incidente);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<IncidenteCriticoDto>> ActualizarIncidente(int id, IncidenteCriticoActualizarDto dto)
    {
        var incidente = await _servicio.ActualizarIncidenteAsync(id, dto);
        return incidente is null ? NotFound() : Ok(incidente);
    }

    [HttpPost("importar")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<IncidenteCriticoImportarResultadoDto>> ImportarIncidentes(IFormFile? archivo)
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
