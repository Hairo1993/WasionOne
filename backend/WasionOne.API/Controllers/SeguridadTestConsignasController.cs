using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/seguridad/test-consignas")]
[Authorize]
public class SeguridadTestConsignasController : ControllerBase
{
    private readonly ITestConsignaService _servicio;
    private readonly IUsuarioContexto _usuarioContexto;

    private const string MODULO = WasionOne.API.Models.Modulos.SegTestConsignas;

    public SeguridadTestConsignasController(ITestConsignaService servicio, IUsuarioContexto usuarioContexto)
    {
        _servicio = servicio;
        _usuarioContexto = usuarioContexto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestConsignaDto>>> ObtenerRegistros([FromQuery] int? areaUbicacionId)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        var registros = await _servicio.ObtenerRegistrosAsync(areaUbicacionId);
        return Ok(registros.Where(r => AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, r.AreaUbicacionId)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TestConsignaDto>> ObtenerRegistro(int id)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var registro = await _servicio.ObtenerRegistroPorIdAsync(id);
        if (registro is null)
        {
            return NotFound();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        if (!AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, registro.AreaUbicacionId))
        {
            return NotFound();
        }

        return Ok(registro);
    }

    [HttpPost]
    public async Task<ActionResult<TestConsignaDto>> CrearRegistro(TestConsignaCrearDto dto)
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

        var registro = await _servicio.CrearRegistroAsync(dto);
        return CreatedAtAction(nameof(ObtenerRegistro), new { id = registro.Id }, registro);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TestConsignaDto>> ActualizarRegistro(int id, TestConsignaActualizarDto dto)
    {
        if (!AutorizacionModuloHelper.TieneModuloAsignado(_usuarioContexto, MODULO))
        {
            return Forbid();
        }

        var existente = await _servicio.ObtenerRegistroPorIdAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        var plantasPermitidas = AutorizacionModuloHelper.ObtenerPlantasPermitidas(_usuarioContexto, MODULO);
        if (!AutorizacionModuloHelper.TienePlantaPermitida(plantasPermitidas, existente.AreaUbicacionId))
        {
            return NotFound();
        }

        var registro = await _servicio.ActualizarRegistroAsync(id, dto);
        return registro is null ? NotFound() : Ok(registro);
    }

    [HttpPost("importar")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<TestConsignaImportarResultadoDto>> ImportarRegistros(IFormFile? archivo)
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
        return File(archivo, ExcelPlantillaUtils.MimeTypeXlsx, "plantilla-test-consignas.xlsx");
    }
}
