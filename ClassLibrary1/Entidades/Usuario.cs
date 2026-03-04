using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketSystem.Dominio.Enumeraciones;

namespace TicketSystem.Dominio.Entidades
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public RolUsuario Rol { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpires { get; set; }
    }
}
