using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/it/respaldos")]
[Authorize]
public class ItRespaldosController : ControllerBase
{
    private readonly IRespaldoService _servicio;

    public ItRespaldosController(IRespaldoService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RespaldoDto>>> ObtenerRespaldos([FromQuery] int? areaUbicacionId)
    {
        return Ok(await _servicio.ObtenerRespaldosAsync(areaUbicacionId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RespaldoDto>> ObtenerRespaldo(int id)
    {
        var respaldo = await _servicio.ObtenerRespaldoPorIdAsync(id);
        return respaldo is null ? NotFound() : Ok(respaldo);
    }

    [HttpPost]
    public async Task<ActionResult<RespaldoDto>> CrearRespaldo(RespaldoCrearDto dto)
    {
        var respaldo = await _servicio.CrearRespaldoAsync(dto);
        return CreatedAtAction(nameof(ObtenerRespaldo), new { id = respaldo.Id }, respaldo);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RespaldoDto>> ActualizarRespaldo(int id, RespaldoActualizarDto dto)
    {
        var respaldo = await _servicio.ActualizarRespaldoAsync(id, dto);
        return respaldo is null ? NotFound() : Ok(respaldo);
    }

    [HttpPost("importar")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<RespaldoImportarResultadoDto>> ImportarRespaldos(IFormFile? archivo)
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
