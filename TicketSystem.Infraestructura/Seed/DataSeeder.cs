using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Dominio.Enumeraciones;
using TicketSystem.Infraestructura.Datos;

namespace TicketSystem.Infraestructura.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(
        TicketSystemDbContext context,
        IPasswordHasher<Usuario> passwordHasher)
    {
        
        if (await context.Usuarios.AnyAsync())
            return;

        
        var admin = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = "Administrador",
            Email = "admin@ticketsystem.com",
            Rol = RolUsuario.Administrador
        };

        admin.PasswordHash =
            passwordHasher.HashPassword(admin, "Admin123");

        
        var operador = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = "Operador Demo",
            Email = "operador@ticketsystem.com",
            Rol = RolUsuario.Operador
        };

        operador.PasswordHash =
            passwordHasher.HashPassword(operador, "Operador123");

        
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = "Usuario Demo",
            Email = "usuario@ticketsystem.com",
            Rol = RolUsuario.Usuario
        };

        usuario.PasswordHash =
            passwordHasher.HashPassword(usuario, "Usuario123");

        
        await context.Usuarios.AddRangeAsync(admin, operador, usuario);
        await context.SaveChangesAsync();
    }
}