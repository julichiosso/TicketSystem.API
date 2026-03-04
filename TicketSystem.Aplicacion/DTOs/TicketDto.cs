using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketSystem.Dominio.Enumeraciones;

namespace TicketSystem.Aplicacion.DTOs
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public PrioridadTicket Prioridad { get; set; }
        public EstadoTicket Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public DateTime? FechaLimite { get; set; }
        public bool SLACumplido { get; set; }
        public Guid UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public Guid? OperadorAsignadoId { get; set; }
        public string? OperadorAsignadoNombre { get; set; }
    }
}
