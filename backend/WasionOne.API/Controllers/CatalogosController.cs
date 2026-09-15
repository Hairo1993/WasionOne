using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/catalogos")]
[Authorize]
public class CatalogosController : ControllerBase
{
    private readonly ICatalogosService _servicio;

    public CatalogosController(ICatalogosService servicio)
    {
        _servicio = servicio;
    }

    // --- Direcciones ---

    [HttpGet("direcciones")]
    public async Task<ActionResult<IEnumerable<DireccionDto>>> ObtenerDirecciones()
    {
        return Ok(await _servicio.ObtenerDireccionesAsync());
    }

    [HttpGet("direcciones/{id:int}")]
    public async Task<ActionResult<DireccionDto>> ObtenerDireccion(int id)
    {
        var direccion = await _servicio.ObtenerDireccionPorIdAsync(id);
        return direccion is null ? NotFound() : Ok(direccion);
    }

    [HttpPost("direcciones")]
    public async Task<ActionResult<DireccionDto>> CrearDireccion(DireccionCrearDto dto)
    {
        var direccion = await _servicio.CrearDireccionAsync(dto);
        return CreatedAtAction(nameof(ObtenerDireccion), new { id = direccion.Id }, direccion);
    }

    // --- Departamentos ---

    [HttpGet("departamentos")]
    public async Task<ActionResult<IEnumerable<DepartamentoDto>>> ObtenerDepartamentos([FromQuery] int? direccionId)
    {
        return Ok(await _servicio.ObtenerDepartamentosAsync(direccionId));
    }

    [HttpGet("departamentos/{id:int}")]
    public async Task<ActionResult<DepartamentoDto>> ObtenerDepartamento(int id)
    {
        var departamento = await _servicio.ObtenerDepartamentoPorIdAsync(id);
        return departamento is null ? NotFound() : Ok(departamento);
    }

    [HttpPost("departamentos")]
    public async Task<ActionResult<DepartamentoDto>> CrearDepartamento(DepartamentoCrearDto dto)
    {
        var departamento = await _servicio.CrearDepartamentoAsync(dto);
        return CreatedAtAction(nameof(ObtenerDepartamento), new { id = departamento.Id }, departamento);
    }

    // --- Areas ---

    [HttpGet("areas")]
    public async Task<ActionResult<IEnumerable<AreaDto>>> ObtenerAreas([FromQuery] int? departamentoId)
    {
        return Ok(await _servicio.ObtenerAreasAsync(departamentoId));
    }

    [HttpGet("areas/{id:int}")]
    public async Task<ActionResult<AreaDto>> ObtenerArea(int id)
    {
        var area = await _servicio.ObtenerAreaPorIdAsync(id);
        return area is null ? NotFound() : Ok(area);
    }

    [HttpPost("areas")]
    public async Task<ActionResult<AreaDto>> CrearArea(AreaCrearDto dto)
    {
        var area = await _servicio.CrearAreaAsync(dto);
        return CreatedAtAction(nameof(ObtenerArea), new { id = area.Id }, area);
    }

    // --- Ubicaciones ---

    [HttpGet("ubicaciones")]
    public async Task<ActionResult<IEnumerable<UbicacionDto>>> ObtenerUbicaciones()
    {
        return Ok(await _servicio.ObtenerUbicacionesAsync());
    }

    [HttpGet("ubicaciones/{id:int}")]
    public async Task<ActionResult<UbicacionDto>> ObtenerUbicacion(int id)
    {
        var ubicacion = await _servicio.ObtenerUbicacionPorIdAsync(id);
        return ubicacion is null ? NotFound() : Ok(ubicacion);
    }

    [HttpPost("ubicaciones")]
    public async Task<ActionResult<UbicacionDto>> CrearUbicacion(UbicacionCrearDto dto)
    {
        var ubicacion = await _servicio.CrearUbicacionAsync(dto);
        return CreatedAtAction(nameof(ObtenerUbicacion), new { id = ubicacion.Id }, ubicacion);
    }

    // --- Área x Ubicación (nodo operativo) ---

    [HttpGet("area-ubicaciones")]
    public async Task<ActionResult<IEnumerable<AreaUbicacionDto>>> ObtenerAreaUbicaciones([FromQuery] int? areaId)
    {
        return Ok(await _servicio.ObtenerAreaUbicacionesAsync(areaId));
    }

    [HttpPost("area-ubicaciones")]
    public async Task<ActionResult<AreaUbicacionDto>> CrearAreaUbicacion(AreaUbicacionCrearDto dto)
    {
        var nodo = await _servicio.CrearAreaUbicacionAsync(dto);
        return Ok(nodo);
    }
}