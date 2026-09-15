using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WasionOne.API.DTOs;
using WasionOne.API.Interfaces;

namespace WasionOne.API.Controllers;

[ApiController]
[Route("api/it/tickets")]
[Authorize]
public class ItTicketsController : ControllerBase
{
    private readonly ITicketService _servicio;

    public ItTicketsController(ITicketService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketDto>>> ObtenerTickets([FromQuery] int? areaUbicacionId)
    {
        return Ok(await _servicio.ObtenerTicketsAsync(areaUbicacionId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketDto>> ObtenerTicket(int id)
    {
        var ticket = await _servicio.ObtenerTicketPorIdAsync(id);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> CrearTicket(TicketCrearDto dto)
    {
        var ticket = await _servicio.CrearTicketAsync(dto);
        return CreatedAtAction(nameof(ObtenerTicket), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TicketDto>> ActualizarTicket(int id, TicketActualizarDto dto)
    {
        var ticket = await _servicio.ActualizarTicketAsync(id, dto);
        return ticket is null ? NotFound() : Ok(ticket);
    }

    // Importación masiva desde el Excel exportado del sistema de mesa de
    // ayuda. Se envía como multipart/form-data con el archivo en el campo
    // "archivo".
    [HttpPost("importar")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<TicketImportarResultadoDto>> ImportarTickets(IFormFile? archivo)
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
