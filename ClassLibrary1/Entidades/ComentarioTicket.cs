using System;

namespace TicketSystem.Dominio.Entidades
{
    public class ComentarioTicket
    {
        public Guid Id { get; set; }

        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;

        public Guid AutorId { get; set; }
        public Usuario Autor { get; set; } = null!;

        public string Mensaje { get; set; } = null!;
        public bool EsInterno { get; set; }

        public DateTime FechaCreacion { get; set; }
        
        public ICollection<ArchivoAdjunto> Adjuntos { get; set; } = new List<ArchivoAdjunto>();

    }
}
