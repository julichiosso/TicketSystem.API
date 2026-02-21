using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketSystem.Aplicacion.DTOs;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Dominio.Enumeraciones;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IServicioTickets _servicioTickets;

    public TicketsController(IServicioTickets servicioTickets)
    {
        _servicioTickets = servicioTickets;
    }

   
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearTicketDto dto)
    {
        var usuarioId = ObtenerUsuarioIdDelToken();
        dto.UsuarioId = usuarioId;

        var id = await _servicioTickets.CrearAsync(dto);

        return Ok(new { ticketId = id });
    }

    
    [HttpGet("mis-tickets")]
    public async Task<IActionResult> ObtenerMisTickets()
    {
        var usuarioId = ObtenerUsuarioIdDelToken();

        var tickets = await _servicioTickets
            .ObtenerPorUsuarioAsync(usuarioId);

        return Ok(tickets);
    }

    
    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(
        Guid id,
        [FromBody] int estado)
    {
        if (!Enum.IsDefined(typeof(EstadoTicket), estado))
            throw new ArgumentException("Estado inválido");

        await _servicioTickets
            .CambiarEstadoAsync(id, (EstadoTicket)estado);

        return NoContent();
    }

    
    [Authorize(Roles = "Administrador,Operador")]
    [HttpGet]
    public async Task<IActionResult> ObtenerFiltrados(
        [FromQuery] FiltroTicketsDto filtro)
    {
        var resultado = await _servicioTickets
            .ObtenerFiltradosAsync(filtro);

        return Ok(new
        {
            success = true,
            data = resultado
        });
    }

    private Guid ObtenerUsuarioIdDelToken()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null)
            throw new UnauthorizedAccessException("Usuario no autenticado");

        return Guid.Parse(claim.Value);
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        await _servicioTickets.EliminarAsync(id);
        return NoContent();
    }


}
