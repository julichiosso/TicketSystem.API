using System;
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

        // Password Reset
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpires { get; set; }

        // Refresh Token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpires { get; set; }
    }
}