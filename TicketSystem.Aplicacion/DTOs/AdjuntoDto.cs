namespace TicketSystem.Aplicacion.DTOs;

public class AdjuntoDto
{
    public Guid   Id             { get; set; }
    public string NombreOriginal { get; set; } = null!;
    public string ContentType    { get; set; } = null!;
    public long   TamañoBytes    { get; set; }
    public string Url            { get; set; } = null!;
}