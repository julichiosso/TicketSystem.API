namespace TicketSystem.Aplicacion.Interfaces;

public interface IServicioEmail
{
    Task EnviarEmailAsync(string destinatario, string asunto, string cuerpoHtml);
}
