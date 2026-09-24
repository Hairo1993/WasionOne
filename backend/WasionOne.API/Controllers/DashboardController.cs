using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

// Dashboard de Dirección / Dashboard de Departamento (22/sep/2026): vista
// agregada de SOLO LECTURA sobre los 24 módulos de captura ya existentes.
// No expone ningún endpoint de escritura — todo se sigue capturando desde
// las pantallas de cada módulo.
[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    // [Authorize(Roles = "...")] exige una constante en tiempo de
    // compilación; concatenar campos const con '+' sigue siéndolo.
    private const string ROLES_DASHBOARD_DIRECCION =
        WasionOne.API.Models.Roles.Director + "," + WasionOne.API.Models.Roles.Ceo + "," + WasionOne.API.Models.Roles.Superadmin;
    private const string ROLES_DASHBOARD_DEPARTAMENTO =
        WasionOne.API.Models.Roles.Director + "," + WasionOne.API.Models.Roles.ResponsableDepartamento + "," +
        WasionOne.API.Models.Roles.Ceo + "," + WasionOne.API.Models.Roles.Superadmin;

    private readonly IDashboardService _servicio;
    private readonly IUsuarioContexto _usuarioContexto;

    public DashboardController(IDashboardService servicio, IUsuarioContexto usuarioContexto)
    {
        _servicio = servicio;
        _usuarioContexto = usuarioContexto;
    }

    [HttpGet("direccion")]
    [Authorize(Roles = ROLES_DASHBOARD_DIRECCION)]
    public async Task<ActionResult<DashboardDireccionDto>> ObtenerDireccion(
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] List<int>? ubicacionIds,
        [FromQuery] int? direccionId)
    {
        int direccionIdEfectiva;

        if (_usuarioContexto.Rol == WasionOne.API.Models.Roles.Director)
        {
            // Decisión de diseño: para Director este endpoint SIEMPRE usa su
            // propia Dirección (la del JWT) e ignora cualquier direccionId
            // que mande en el query, en vez de regresar 403 si no coincide.
            // Es más simple y a prueba de errores del frontend — un
            // Director nunca puede ver otra Dirección de todas formas, así
            // que "ignorar" y "rechazar" llevan al mismo resultado visible;
            // ignorar evita que un query mal armado (ej. quedó un
            // direccionId viejo en la URL) tire un error sin necesidad.
            if (_usuarioContexto.DireccionId is null)
            {
                return Forbid();
            }

            direccionIdEfectiva = _usuarioContexto.DireccionId.Value;
        }
        else
        {
            // CEO/Superadmin no tienen una Dirección propia en la jerarquía,
            // así que el parámetro es obligatorio para ellos.
            if (!direccionId.HasValue)
            {
                return BadRequest("El parámetro direccionId es obligatorio para este rol.");
            }

            direccionIdEfectiva = direccionId.Value;
        }

        var resultado = await _servicio.ObtenerDireccionAsync(direccionIdEfectiva, fechaDesde, fechaHasta, ubicacionIds);
        return resultado is null ? NotFound() : Ok(resultado);
    }

    [HttpGet("departamento/{id:int}")]
    [Authorize(Roles = ROLES_DASHBOARD_DEPARTAMENTO)]
    public async Task<ActionResult<DashboardDepartamentoDto>> ObtenerDepartamento(
        int id,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] List<int>? ubicacionIds)
    {
        var rol = _usuarioContexto.Rol;

        if (rol == WasionOne.API.Models.Roles.ResponsableDepartamento)
        {
            if (_usuarioContexto.DepartamentoId != id)
            {
                return Forbid();
            }
        }
        else if (rol == WasionOne.API.Models.Roles.Director)
        {
            // El Director puede ver el drill-down de cualquier Departamento
            // de SU Dirección, no solo el suyo propio (no tiene uno "propio"
            // en sí — ve toda su Dirección). Se valida contra la BD porque,
            // a diferencia de direccionId arriba, aquí sí hace falta saber
            // a qué Dirección pertenece el Departamento pedido.
            var direccionIdDelDepartamento = await _servicio.ObtenerDireccionIdDeDepartamentoAsync(id);
            if (direccionIdDelDepartamento is null)
            {
                return NotFound();
            }

            if (_usuarioContexto.DireccionId != direccionIdDelDepartamento)
            {
                return Forbid();
            }
        }
        // CEO y Superadmin: sin restricción adicional, pueden pedir
        // cualquier Departamento.

        var resultado = await _servicio.ObtenerDepartamentoAsync(id, fechaDesde, fechaHasta, ubicacionIds);
        return resultado is null ? NotFound() : Ok(resultado);
    }
}
