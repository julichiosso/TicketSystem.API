namespace TicketSystem.Dominio.Entidades;

public class ArchivoAdjunto
{
    public Guid   Id               { get; set; }
    public Guid   ComentarioId     { get; set; }
    public ComentarioTicket Comentario { get; set; } = null!;
    public string NombreOriginal   { get; set; } = null!;
    public string NombreAlmacenado { get; set; } = null!;
    public string ContentType      { get; set; } = null!;
    public long   TamañoBytes      { get; set; }
    public DateTime FechaSubida    { get; set; }
}