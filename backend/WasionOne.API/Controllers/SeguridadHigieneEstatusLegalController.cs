using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/seguridad-higiene/estatus-legal")]
[Authorize]
public class SeguridadHigieneEstatusLegalController : ControllerBase
{
    private readonly IEstatusLegalPlantaService _servicio;
    private readonly IUsuarioContexto _usuarioContexto;

    // NOTA: esta clave de módulo todavía no existe en Models/Modulos.cs —
    // se agrega ahí de forma centralizada más adelante (ver
    // RESUMEN-GRUPO-B.md, constante propuesta Modulos.SegHigEstatusLegal).
    // Se usa el string literal directamente mientras tanto para no
    // depender de un archivo compartido que no se debe tocar aquí.
    private const string MODULO = "seguridadHigiene.estatusLegal";

    public SeguridadHigieneEstatusLegalController(IEstatusLegalPlantaService servicio, IUsuarioContexto usuarioContexto)
    {
        _servicio = servicio;
        _usuarioContexto = usuarioContexto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstatusLegalPlantaDto>>> ObtenerRegistros([FromQuery] int? areaUbicacionId)
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
    public async Task<ActionResult<EstatusLegalPlantaDto>> ObtenerRegistro(int id)
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
    public async Task<ActionResult<EstatusLegalPlantaDto>> CrearRegistro(EstatusLegalPlantaCrearDto dto)
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
        if (registro is null)
        {
            return Conflict("Ya existe un registro de Estatus legal para esa Planta, ese Requerimiento legal y esa Última fecha de realización. Edítalo desde la tabla en vez de crear uno nuevo.");
        }

        return CreatedAtAction(nameof(ObtenerRegistro), new { id = registro.Id }, registro);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EstatusLegalPlantaDto>> ActualizarRegistro(int id, EstatusLegalPlantaActualizarDto dto)
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
    public async Task<ActionResult<EstatusLegalPlantaImportarResultadoDto>> ImportarRegistros(IFormFile? archivo)
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
        return File(archivo, ExcelPlantillaUtils.MimeTypeXlsx, "plantilla-estatus-legal.xlsx");
    }
}
