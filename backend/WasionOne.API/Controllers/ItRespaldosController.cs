using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/it/respaldos")]
[Authorize]
public class ItRespaldosController : ControllerBase
{
    private readonly IRespaldoService _servicio;
    private readonly IUsuarioContexto _usuarioContexto;

    private const string MODULO = WasionOne.API.Models.Modulos.ItRespaldos;

    public ItRespaldosController(IRespaldoService servicio, IUsuarioContexto usuarioContexto)
    {
        _servicio = servicio;
        _usuarioContexto = usuarioContexto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RespaldoDto>>> ObtenerRespaldos([FromQuery] int? areaUbicacionId)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        var registros = await _servicio.ObtenerRespaldosAsync(areaUbicacionId);
        return Ok(registros.Where(r => AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, r.AreaUbicacionId)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RespaldoDto>> ObtenerRespaldo(int id)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var respaldo = await _servicio.ObtenerRespaldoPorIdAsync(id);
        if (respaldo is null)
        {
            return NotFound();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        if (!AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, respaldo.AreaUbicacionId))
        {
            return NotFound();
        }

        return Ok(respaldo);
    }

    [HttpPost]
    public async Task<ActionResult<RespaldoDto>> CrearRespaldo(RespaldoCrearDto dto)
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

        var respaldo = await _servicio.CrearRespaldoAsync(dto);
        return CreatedAtAction(nameof(ObtenerRespaldo), new { id = respaldo.Id }, respaldo);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RespaldoDto>> ActualizarRespaldo(int id, RespaldoActualizarDto dto)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var existente = await _servicio.ObtenerRespaldoPorIdAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        if (!AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, existente.AreaUbicacionId))
        {
            return NotFound();
        }

        var respaldo = await _servicio.ActualizarRespaldoAsync(id, dto);
        return respaldo is null ? NotFound() : Ok(respaldo);
    }

    [HttpPost("importar")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<RespaldoImportarResultadoDto>> ImportarRespaldos(IFormFile? archivo)
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
        return File(archivo, ExcelPlantillaUtils.MimeTypeXlsx, "plantilla-it-respaldos.xlsx");
    }
}
