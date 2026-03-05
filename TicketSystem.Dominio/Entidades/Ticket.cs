using System;
using System.Collections.Generic;
using TicketSystem.Dominio.Enumeraciones;

namespace TicketSystem.Dominio.Entidades
{
    public class Ticket
    {
        public Guid Id { get; set; }

        public string Titulo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;

        public PrioridadTicket Prioridad { get; set; }
        public EstadoTicket Estado { get; set; } = EstadoTicket.Pendiente;

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public Guid? OperadorAsignadoId { get; set; }
        public Usuario? OperadorAsignado { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public DateTime? FechaLimite { get; set; }
        public bool SLACumplido { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public ICollection<ComentarioTicket> Comentarios { get; set; } = new List<ComentarioTicket>();
    }
}
