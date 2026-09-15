using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/it/auditorias")]
[Authorize]
public class ItAuditoriasController : ControllerBase
{
    private readonly IAuditoriaEquipoService _servicio;

    public ItAuditoriasController(IAuditoriaEquipoService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditoriaEquipoDto>>> ObtenerAuditorias([FromQuery] int? areaUbicacionId)
    {
        return Ok(await _servicio.ObtenerAuditoriasAsync(areaUbicacionId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuditoriaEquipoDto>> ObtenerAuditoria(int id)
    {
        var auditoria = await _servicio.ObtenerAuditoriaPorIdAsync(id);
        return auditoria is null ? NotFound() : Ok(auditoria);
    }

    [HttpPost]
    public async Task<ActionResult<AuditoriaEquipoDto>> CrearAuditoria(AuditoriaEquipoCrearDto dto)
    {
        var auditoria = await _servicio.CrearAuditoriaAsync(dto);
        return CreatedAtAction(nameof(ObtenerAuditoria), new { id = auditoria.Id }, auditoria);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AuditoriaEquipoDto>> ActualizarAuditoria(int id, AuditoriaEquipoActualizarDto dto)
    {
        var auditoria = await _servicio.ActualizarAuditoriaAsync(id, dto);
        return auditoria is null ? NotFound() : Ok(auditoria);
    }

    [HttpPost("importar")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<AuditoriaEquipoImportarResultadoDto>> ImportarAuditorias(IFormFile? archivo)
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
