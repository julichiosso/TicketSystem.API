using TicketSystem.Dominio.Entidades;

namespace TicketSystem.Aplicacion.Interfaces
{
    public interface ITokenService
    {
        string GenerarToken(Usuario usuario);
        string GenerarRefreshToken();
    }
}