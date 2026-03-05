using System;

namespace TicketSystem.Aplicacion.DTOs
{
    public class ComentarioTicketDto
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid AutorId { get; set; }
        public string Autor { get; set; } = string.Empty;
        public int Rol { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public bool Interno { get; set; }
        public DateTime Fecha { get; set; }

        public List<AdjuntoDto> Adjuntos { get; set; } = new();

    }
}
