using TicketSystem.Aplicacion.Common;
using TicketSystem.Aplicacion.DTOs;
using TicketSystem.Dominio.Enumeraciones;
using TicketSystem.Dominio.Entidades;

namespace TicketSystem.Aplicacion.Interfaces
{
    public interface IServicioTickets
    {
        Task<Guid> CrearAsync(CrearTicketDto dto);
        Task<IEnumerable<TicketDto>> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<TicketDto>> ObtenerPorOperadorAsync(Guid operadorId);
        Task CambiarEstadoAsync(Guid ticketId, EstadoTicket nuevoEstado, Guid actorId);
        Task<PagedResult<TicketDto>> ObtenerFiltradosAsync(FiltroTicketsDto filtro);
        Task EliminarAsync(Guid id, Guid actorId);
        Task AsignarOperadorAsync(Guid ticketId, Guid? operadorId, Guid actorId);

        Task<IEnumerable<ComentarioTicketDto>> ObtenerComentariosAsync(Guid ticketId, bool incluirInternos);
        Task<ComentarioTicketDto> AgregarComentarioAsync(Guid ticketId, Guid autorId, CrearComentarioTicketDto dto);
        Task<IEnumerable<AuditLog>> ObtenerAuditLogsAsync();
        Task<MetricasDto> ObtenerMetricasAsync();
    }
}
