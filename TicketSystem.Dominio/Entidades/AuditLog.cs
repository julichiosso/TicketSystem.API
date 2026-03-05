using System;

namespace TicketSystem.Dominio.Entidades
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid? TicketId { get; set; }
        public Guid? UsuarioId { get; set; }
        public string Accion { get; set; } = null!;
        public string Detalle { get; set; } = null!;
        public DateTime Fecha { get; set; }
    }
}
