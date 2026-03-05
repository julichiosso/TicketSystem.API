using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Infraestructura.Datos;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ArchivosController : ControllerBase
{
    private readonly TicketSystemDbContext _context;
    private readonly IWebHostEnvironment   _env;

    private static readonly HashSet<string> TiposPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "image/webp",
        "application/pdf",
        "text/plain",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    };

    private const long MaxBytes = 10 * 1024 * 1024; // 10 MB

    public ArchivosController(TicketSystemDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env     = env;
    }

    [HttpPost("comentario/{comentarioId}")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> SubirArchivo(Guid comentarioId, IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest(new { message = "No se recibió ningún archivo." });

        if (archivo.Length > MaxBytes)
            return BadRequest(new { message = "El archivo supera el límite de 10 MB." });

        if (!TiposPermitidos.Contains(archivo.ContentType))
            return BadRequest(new { message = "Tipo de archivo no permitido." });

        var comentario = await _context.ComentariosTicket.FindAsync(comentarioId);
        if (comentario == null)
            return NotFound(new { message = "Comentario no encontrado." });

        // Guardar en wwwroot/uploads
        var uploadsPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsPath);

        var ext              = Path.GetExtension(archivo.FileName);
        var nombreAlmacenado = $"{Guid.NewGuid()}{ext}";
        var filePath         = Path.Combine(uploadsPath, nombreAlmacenado);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await archivo.CopyToAsync(stream);

        var adjunto = new ArchivoAdjunto
        {
            Id               = Guid.NewGuid(),
            ComentarioId     = comentarioId,
            NombreOriginal   = archivo.FileName,
            NombreAlmacenado = nombreAlmacenado,
            ContentType      = archivo.ContentType,
            TamañoBytes      = archivo.Length,
            FechaSubida      = DateTime.UtcNow
        };

        _context.ArchivosAdjuntos.Add(adjunto);
        await _context.SaveChangesAsync();

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        return Ok(new
        {
            success = true,
            data = new
            {
                adjunto.Id,
                adjunto.NombreOriginal,
                adjunto.ContentType,
                adjunto.TamañoBytes,
                Url = $"{baseUrl}/uploads/{nombreAlmacenado}"
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> DescargarArchivo(Guid id)
    {
        var adjunto = await _context.ArchivosAdjuntos.FindAsync(id);
        if (adjunto == null) return NotFound();

        var filePath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", adjunto.NombreAlmacenado);
        if (!System.IO.File.Exists(filePath)) return NotFound();

        var stream = System.IO.File.OpenRead(filePath);
        return File(stream, adjunto.ContentType, adjunto.NombreOriginal);
    }
}