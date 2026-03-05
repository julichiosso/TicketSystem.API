using TicketSystem.Aplicacion.DTOs;

namespace TicketSystem.Aplicacion.Interfaces
{
    public interface IServicioUsuarios
    {
        Task<Guid> CrearAsync(CrearUsuarioDto dto);
        Task<IEnumerable<UsuarioDto>> ObtenerTodosAsync();
        Task<UsuarioDto?> ObtenerPorIdAsync(Guid id);
        Task EliminarAsync(Guid id);
        Task CambiarPasswordAsync(Guid usuarioId, CambiarPasswordDto dto);
    }
}
