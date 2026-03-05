using TicketSystem.Dominio.Entidades;
using TicketSystem.Dominio.Enumeraciones;

namespace TicketSystem.Aplicacion.Interfaces;

/// <summary>
/// Centraliza todas las notificaciones por email relacionadas a tickets.
/// </summary>
public interface IServicioNotificaciones
{
    /// <summary>Avisa al usuario que su ticket fue recibido y está pendiente.</summary>
    Task NotificarTicketCreadoAsync(Ticket ticket, Usuario usuario);

    /// <summary>Avisa al usuario que el estado de su ticket cambió.</summary>
    Task NotificarCambioEstadoAsync(Ticket ticket, Usuario usuario, EstadoTicket estadoAnterior, EstadoTicket estadoNuevo);

    /// <summary>Avisa al operador que le fue asignado un ticket.</summary>
    Task NotificarAsignacionOperadorAsync(Ticket ticket, Usuario operador);

    /// <summary>Avisa al creador del ticket que recibió un nuevo comentario.</summary>
    Task NotificarNuevoComentarioAsync(Ticket ticket, Usuario destinatario, ComentarioTicket comentario, Usuario autor);
}




