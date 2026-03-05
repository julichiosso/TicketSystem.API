using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TicketSystem.Aplicacion.DTOs;
using TicketSystem.Aplicacion.Interfaces;

namespace TicketSystem.API.Hubs;

[Authorize]
public class TicketHub : Hub
{
    private readonly IServicioTickets _servicioTickets;

    public TicketHub(IServicioTickets servicioTickets)
    {
        _servicioTickets = servicioTickets;
    }

    // El cliente se une al grupo del ticket cuando abre el modal
    public async Task UnirseATicket(string ticketId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"ticket-{ticketId}");
    }

    // El cliente sale del grupo cuando cierra el modal
    public async Task SalirDeTicket(string ticketId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ticket-{ticketId}");
    }

    // El cliente envía un mensaje
    public async Task EnviarMensaje(string ticketId, string mensaje, bool esInterno)
    {
        var userIdClaim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? Context.User?.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var autorId))
            throw new HubException("Token inválido.");

        if (!Guid.TryParse(ticketId, out var ticketGuid))
            throw new HubException("Ticket inválido.");

        var dto = new CrearComentarioTicketDto
        {
            Mensaje = mensaje,
            Interno = esInterno
        };

        var comentario = await _servicioTickets.AgregarComentarioAsync(ticketGuid, autorId, dto);

        // Transmitir a todos los que están viendo este ticket
        await Clients.Group($"ticket-{ticketId}").SendAsync("NuevoMensaje", comentario);
    }
}