using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        var tickets = await _servicioTickets.ObtenerPorUsuarioAsync(usuarioId);
        return Ok(tickets);
    }

    [Authorize(Roles = "Administrador,Operador")]
    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] int estado)
    {
        if (!Enum.IsDefined(typeof(EstadoTicket), estado))
            throw new ArgumentException("Estado inválido");

        var actorId = ObtenerUsuarioIdDelToken();
        await _servicioTickets.CambiarEstadoAsync(id, (EstadoTicket)estado, actorId);
        return NoContent();
    }

    [Authorize(Roles = "Administrador,Operador")]
    [HttpGet]
    public async Task<IActionResult> ObtenerFiltrados([FromQuery] FiltroTicketsDto filtro)
    {
        var resultado = await _servicioTickets.ObtenerFiltradosAsync(filtro);

        return Ok(new
        {
            success = true,
            data = resultado
        });
    }

    [Authorize(Roles = "Administrador,Operador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        var actorId = ObtenerUsuarioIdDelToken();
        await _servicioTickets.EliminarAsync(id, actorId);
        return NoContent();
    }

    [HttpGet("{id}/comments")]
    public async Task<IActionResult> ObtenerComentarios(Guid id)
    {
        var esAdminOOperador = User.IsInRole("Administrador") || User.IsInRole("Operador");
        var comentarios = await _servicioTickets.ObtenerComentariosAsync(id, incluirInternos: esAdminOOperador);

        return Ok(new
        {
            success = true,
            data = comentarios
        });
    }

    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AgregarComentario(Guid id, [FromBody] CrearComentarioTicketDto dto)
    {
        var autorId = ObtenerUsuarioIdDelToken();
        var comentario = await _servicioTickets.AgregarComentarioAsync(id, autorId, dto);

        return Ok(new
        {
            success = true,
            data = comentario
        });
    }

    [Authorize(Roles = "Operador,Administrador")]
    [HttpGet("operador/mis-tickets")]
    public async Task<IActionResult> ObtenerMisTicketsOperador()
    {
        var operadorId = ObtenerUsuarioIdDelToken();
        var tickets = await _servicioTickets.ObtenerPorOperadorAsync(operadorId);
        return Ok(new
        {
            success = true,
            data = tickets
        });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}/asignar")]
    public async Task<IActionResult> AsignarOperador(Guid id, [FromBody] AsignarOperadorDto dto)
    {
        var actorId = ObtenerUsuarioIdDelToken();
        await _servicioTickets.AsignarOperadorAsync(id, dto.OperadorId, actorId);
        return NoContent();
    }

    private Guid ObtenerUsuarioIdDelToken()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            throw new UnauthorizedAccessException("Usuario no autenticado");

        return Guid.Parse(claim.Value);
    }
}
