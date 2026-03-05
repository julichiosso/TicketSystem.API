using TicketSystem.Dominio.Entidades;

namespace TicketSystem.Aplicacion.Interfaces
{
    public interface IRepositorioUsuarios
    {
        Task CrearAsync(Usuario usuario);
        Task<IEnumerable<Usuario>> ObtenerTodosAsync();
        Task<Usuario?> ObtenerPorIdAsync(Guid id);
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task<Usuario?> ObtenerPorRefreshTokenAsync(string refreshToken);
        Task AgregarAsync(Usuario usuario);
        Task EliminarAsync(Usuario usuario);
        Task EliminarAsync(Guid id);
        Task ActualizarAsync(Usuario usuario);
    }
}