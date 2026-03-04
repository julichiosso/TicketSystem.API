using TicketSystem.Dominio.Enumeraciones;

namespace TicketSystem.Aplicacion.DTOs;

public class FiltroTicketsDto
{
    public EstadoTicket? Estado { get; set; }
    public string? Titulo { get; set; }
    public PrioridadTicket? Prioridad { get; set; }
    public Guid? OperadorId { get; set; }
    public Guid? UsuarioId { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? SortBy { get; set; }
    public bool Descending { get; set; } = false;
}