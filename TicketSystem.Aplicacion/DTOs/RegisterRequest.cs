namespace TicketSystem.API.DTOs;

public class RegisterRequest
{
    public string Nombre { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
