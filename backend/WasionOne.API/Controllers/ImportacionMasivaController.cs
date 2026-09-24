using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Helpers;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

// Importación masiva combinada (24/sep/2026, pantalla nueva dedicada
// "Importación masiva"): descarga un solo Excel con una pestaña por cada
// indicador que el usuario tiene asignado, y permite subirlo ya lleno de
// vuelta en una sola operación. No requiere ningún módulo asignado en
// particular — cualquier usuario autenticado puede entrar; lo que ve/pueda
// importar depende de sus propios módulos asignados (igual que en cada
// pantalla individual).
[ApiController]
[Route("api/importacion-masiva")]
[Authorize]
public class ImportacionMasivaController : ControllerBase
{
    private readonly IImportacionMasivaService _servicio;

    public ImportacionMasivaController(IImportacionMasivaService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet("modulos")]
    public ActionResult<IReadOnlyList<ModuloDisponibleDto>> ObtenerModulosDisponibles()
    {
        return Ok(_servicio.ObtenerModulosDisponibles());
    }

    [HttpGet("plantilla")]
    public IActionResult DescargarPlantillaCombinada()
    {
        var archivo = _servicio.GenerarPlantillaCombinada();
        return File(archivo, ExcelPlantillaUtils.MimeTypeXlsx, "plantilla-combinada-wasion-one.xlsx");
    }

    [HttpPost("importar")]
    [RequestSizeLimit(100_000_000)]
    public async Task<ActionResult<ImportacionMasivaResultadoDto>> ImportarCombinado(IFormFile? archivo)
    {
        if (archivo is null || archivo.Length == 0)
        {
            return BadRequest("Debes adjuntar un archivo Excel (.xlsx).");
        }

        using var flujo = archivo.OpenReadStream();
        var resultado = await _servicio.ImportarCombinadoAsync(flujo);
        return Ok(resultado);
    }
}
