namespace TicketSystem.Aplicacion.DTOs;

public class CambiarPasswordDto
{
    public string PasswordActual { get; set; } = null!;
    public string PasswordNueva { get; set; } = null!;
    public string ConfirmarPassword { get; set; } = null!;
}