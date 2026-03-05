using Microsoft.AspNetCore.Identity;
using TicketSystem.Aplicacion.DTOs;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Dominio.Entidades;

namespace TicketSystem.Aplicacion.Servicios
{
    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly IRepositorioUsuarios _repositorio;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public ServicioUsuarios(
            IRepositorioUsuarios repositorio,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _repositorio = repositorio;
            _passwordHasher = passwordHasher;
        }

        public async Task<Guid> CrearAsync(CrearUsuarioDto dto)
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre,
                Email = dto.Email,
                Rol = dto.Rol
            };

            await _repositorio.CrearAsync(usuario);
            return usuario.Id;
        }

        public async Task<IEnumerable<UsuarioDto>> ObtenerTodosAsync()
        {
            var usuarios = await _repositorio.ObtenerTodosAsync();

            return usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                Rol = u.Rol
            });
        }

        public async Task<UsuarioDto?> ObtenerPorIdAsync(Guid id)
        {
            var usuario = await _repositorio.ObtenerPorIdAsync(id);

            if (usuario == null)
                return null;

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol
            };
        }

        public async Task EliminarAsync(Guid id)
        {
            await _repositorio.EliminarAsync(id);
        }

        public async Task CambiarPasswordAsync(Guid usuarioId, CambiarPasswordDto dto)
        {
            if (dto.PasswordNueva != dto.ConfirmarPassword)
                throw new InvalidOperationException("Las contraseñas no coinciden.");

            if (dto.PasswordNueva.Length < 6)
                throw new InvalidOperationException("La contraseña debe tener al menos 6 caracteres.");

            var usuario = await _repositorio.ObtenerPorIdAsync(usuarioId);
            if (usuario == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario, usuario.PasswordHash, dto.PasswordActual);

            if (resultado == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("La contraseña actual es incorrecta.");

            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, dto.PasswordNueva);
            await _repositorio.ActualizarAsync(usuario);
        }
    }
}