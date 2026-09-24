using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/it/incidentes")]
[Authorize]
public class ItIncidentesController : ControllerBase
{
    private readonly IIncidenteCriticoService _servicio;
    private readonly IUsuarioContexto _usuarioContexto;

    private const string MODULO = WasionOne.API.Models.Modulos.ItIncidentes;

    public ItIncidentesController(IIncidenteCriticoService servicio, IUsuarioContexto usuarioContexto)
    {
        _servicio = servicio;
        _usuarioContexto = usuarioContexto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IncidenteCriticoDto>>> ObtenerIncidentes([FromQuery] int? areaUbicacionId)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        var registros = await _servicio.ObtenerIncidentesAsync(areaUbicacionId);
        return Ok(registros.Where(r => AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, r.AreaUbicacionId)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IncidenteCriticoDto>> ObtenerIncidente(int id)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var incidente = await _servicio.ObtenerIncidentePorIdAsync(id);
        if (incidente is null)
        {
            return NotFound();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        if (!AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, incidente.AreaUbicacionId))
        {
            return NotFound();
        }

        return Ok(incidente);
    }

    [HttpPost]
    public async Task<ActionResult<IncidenteCriticoDto>> CrearIncidente(IncidenteCriticoCrearDto dto)
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

        var incidente = await _servicio.CrearIncidenteAsync(dto);
        return CreatedAtAction(nameof(ObtenerIncidente), new { id = incidente.Id }, incidente);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<IncidenteCriticoDto>> ActualizarIncidente(int id, IncidenteCriticoActualizarDto dto)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var existente = await _servicio.ObtenerIncidentePorIdAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        if (!AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, existente.AreaUbicacionId))
        {
            return NotFound();
        }

        var incidente = await _servicio.ActualizarIncidenteAsync(id, dto);
        return incidente is null ? NotFound() : Ok(incidente);
    }

    [HttpPost("importar")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<IncidenteCriticoImportarResultadoDto>> ImportarIncidentes(IFormFile? archivo)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        if (archivo is null || archivo.Length == 0)
        {
            return BadRequest("Debes adjuntar un archivo Excel (.xlsx).");
        }

        using var flujo = archivo.OpenReadStream();
        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        var resultado = await _servicio.ImportarDesdeExcelAsync(flujo, plantasPermitidas);
        return Ok(resultado);
    }

    // Plantilla de Excel descargable (24/sep/2026): mismas columnas que
    // espera "importar", con una fila de ejemplo.
    [HttpGet("plantilla")]
    public IActionResult DescargarPlantilla()
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var archivo = _servicio.GenerarPlantillaExcel();
        return File(archivo, ExcelPlantillaUtils.MimeTypeXlsx, "plantilla-it-incidentes.xlsx");
    }
}
