using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

// Lectura de catálogos: abierta a cualquier usuario autenticado (se usa en
// selectores por toda la aplicación). Altas/ediciones/bajas: restringidas a
// Superadmin (mismo criterio que UsuariosController; desde el 19/sep/2026
// CEO y Director ya no tienen esta capacidad).
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
    [Authorize(Roles = "Superadmin")]
    public async Task<ActionResult<DireccionDto>> CrearDireccion(DireccionCrearDto dto)
    {
        var direccion = await _servicio.CrearDireccionAsync(dto);
        return CreatedAtAction(nameof(ObtenerDireccion), new { id = direccion.Id }, direccion);
    }

    [HttpPut("direcciones/{id:int}")]
    [Authorize(Roles = "Superadmin")]
    public async Task<ActionResult<DireccionDto>> ActualizarDireccion(int id, DireccionActualizarDto dto)
    {
        var direccion = await _servicio.ActualizarDireccionAsync(id, dto);
        return direccion is null ? NotFound() : Ok(direccion);
    }

    [HttpDelete("direcciones/{id:int}")]
    [Authorize(Roles = "Superadmin")]
    public async Task<IActionResult> EliminarDireccion(int id)
    {
        var resultado = await _servicio.EliminarDireccionAsync(id);
        if (resultado.Exito)
        {
            return NoContent();
        }

        return resultado.Error is null ? NotFound() : Conflict(resultado.Error);
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
    [Authorize(Roles = "Superadmin")]
    public async Task<ActionResult<DepartamentoDto>> CrearDepartamento(DepartamentoCrearDto dto)
    {
        var departamento = await _servicio.CrearDepartamentoAsync(dto);
        return CreatedAtAction(nameof(ObtenerDepartamento), new { id = departamento.Id }, departamento);
    }

    [HttpPut("departamentos/{id:int}")]
    [Authorize(Roles = "Superadmin")]
    public async Task<ActionResult<DepartamentoDto>> ActualizarDepartamento(int id, DepartamentoActualizarDto dto)
    {
        var departamento = await _servicio.ActualizarDepartamentoAsync(id, dto);
        return departamento is null ? NotFound() : Ok(departamento);
    }

    [HttpDelete("departamentos/{id:int}")]
    [Authorize(Roles = "Superadmin")]
    public async Task<IActionResult> EliminarDepartamento(int id)
    {
        var resultado = await _servicio.EliminarDepartamentoAsync(id);
        if (resultado.Exito)
        {
            return NoContent();
        }

        return resultado.Error is null ? NotFound() : Conflict(resultado.Error);
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
    [Authorize(Roles = "Superadmin")]
    public async Task<ActionResult<AreaDto>> CrearArea(AreaCrearDto dto)
    {
        var area = await _servicio.CrearAreaAsync(dto);
        return CreatedAtAction(nameof(ObtenerArea), new { id = area.Id }, area);
    }

    [HttpPut("areas/{id:int}")]
    [Authorize(Roles = "Superadmin")]
    public async Task<ActionResult<AreaDto>> ActualizarArea(int id, AreaActualizarDto dto)
    {
        var area = await _servicio.ActualizarAreaAsync(id, dto);
        return area is null ? NotFound() : Ok(area);
    }

    [HttpDelete("areas/{id:int}")]
    [Authorize(Roles = "Superadmin")]
    public async Task<IActionResult> EliminarArea(int id)
    {
        var resultado = await _servicio.EliminarAreaAsync(id);
        if (resultado.Exito)
        {
            return NoContent();
        }

        return resultado.Error is null ? NotFound() : Conflict(resultado.Error);
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
    [Authorize(Roles = "Superadmin")]
    public async Task<ActionResult<UbicacionDto>> CrearUbicacion(UbicacionCrearDto dto)
    {
        var ubicacion = await _servicio.CrearUbicacionAsync(dto);
        return CreatedAtAction(nameof(ObtenerUbicacion), new { id = ubicacion.Id }, ubicacion);
    }

    [HttpPut("ubicaciones/{id:int}")]
    [Authorize(Roles = "Superadmin")]
    public async Task<ActionResult<UbicacionDto>> ActualizarUbicacion(int id, UbicacionActualizarDto dto)
    {
        var ubicacion = await _servicio.ActualizarUbicacionAsync(id, dto);
        return ubicacion is null ? NotFound() : Ok(ubicacion);
    }

    [HttpDelete("ubicaciones/{id:int}")]
    [Authorize(Roles = "Superadmin")]
    public async Task<IActionResult> EliminarUbicacion(int id)
    {
        var resultado = await _servicio.EliminarUbicacionAsync(id);
        if (resultado.Exito)
        {
            return NoContent();
        }

        return resultado.Error is null ? NotFound() : Conflict(resultado.Error);
    }

    // --- Área x Ubicación (nodo operativo) ---

    [HttpGet("area-ubicaciones")]
    public async Task<ActionResult<IEnumerable<AreaUbicacionDto>>> ObtenerAreaUbicaciones([FromQuery] int? areaId)
    {
        return Ok(await _servicio.ObtenerAreaUbicacionesAsync(areaId));
    }

    [HttpPost("area-ubicaciones")]
    [Authorize(Roles = "Superadmin")]
    public async Task<ActionResult<AreaUbicacionDto>> CrearAreaUbicacion(AreaUbicacionCrearDto dto)
    {
        var nodo = await _servicio.CrearAreaUbicacionAsync(dto);
        return Ok(nodo);
    }

    [HttpDelete("area-ubicaciones/{id:int}")]
    [Authorize(Roles = "Superadmin")]
    public async Task<IActionResult> EliminarAreaUbicacion(int id)
    {
        var resultado = await _servicio.EliminarAreaUbicacionAsync(id);
        if (resultado.Exito)
        {
            return NoContent();
        }

        return resultado.Error is null ? NotFound() : Conflict(resultado.Error);
    }

    // --- Módulos (catálogo fijo de pantallas de captura) ---

    [HttpGet("modulos")]
    public async Task<ActionResult<IEnumerable<ModuloDto>>> ObtenerModulos()
    {
        return Ok(await _servicio.ObtenerModulosAsync());
    }
}
