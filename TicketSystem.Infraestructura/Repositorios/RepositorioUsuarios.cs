using Microsoft.EntityFrameworkCore;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Infraestructura.Datos;

namespace TicketSystem.Infraestructura.Repositorios
{
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly TicketSystemDbContext _context;

        public RepositorioUsuarios(TicketSystemDbContext context)
        {
            _context = context;
        }

        public async Task CrearAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> ObtenerPorRefreshTokenAsync(string refreshToken)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task AgregarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task EliminarAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }
}