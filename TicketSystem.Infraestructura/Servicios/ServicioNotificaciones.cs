using Microsoft.Extensions.Configuration;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Dominio.Enumeraciones;

namespace TicketSystem.Infraestructura.Servicios;

/// <summary>
/// Implementa las notificaciones por email para eventos del sistema de tickets.
/// Usa el IServicioEmail existente para el envío real.
/// </summary>
public class ServicioNotificaciones : IServicioNotificaciones
{
    private readonly IServicioEmail _servicioEmail;
    private readonly string _frontendBase;
    private readonly string _appNombre;

    public ServicioNotificaciones(IServicioEmail servicioEmail, IConfiguration configuration)
    {
        _servicioEmail = servicioEmail;
        _frontendBase  = configuration["AppSettings:FrontendBaseUrl"] ?? "http://localhost:5173";
        _appNombre     = configuration["AppSettings:NombreApp"]       ?? "TicketSystem";
    }

    // ─────────────────────────────────────────────────────────────
    // Ticket creado – confirmación al usuario
    // ─────────────────────────────────────────────────────────────
    public async Task NotificarTicketCreadoAsync(Ticket ticket, Usuario usuario)
    {
        var asunto = $"[{_appNombre}] Tu ticket #{ticket.Id.ToString()[..8].ToUpper()} fue recibido";
        var body   = ConstruirEmail(
            titulo:   "¡Ticket recibido!",
            intro:    $"Hola <strong>{usuario.Nombre}</strong>, registramos tu ticket correctamente.",
            contenido: $@"
                <table style='width:100%;border-collapse:collapse;margin:16px 0;'>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;width:140px;'>ID</td>
                        <td style='padding:8px;'>{ticket.Id.ToString()[..8].ToUpper()}</td></tr>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;'>Asunto</td>
                        <td style='padding:8px;'>{ticket.Titulo}</td></tr>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;'>Prioridad</td>
                        <td style='padding:8px;'>{TraducirPrioridad(ticket.Prioridad)}</td></tr>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;'>Estado</td>
                        <td style='padding:8px;'><span style='color:#2563eb;font-weight:bold;'>{TraducirEstado(ticket.Estado)}</span></td></tr>
                </table>
                <p style='color:#555;'>Nuestro equipo revisará tu solicitud a la brevedad. Te notificaremos ante cualquier cambio.</p>",
            botonUrl:   $"{_frontendBase}/mis-tickets/{ticket.Id}",
            botonTexto: "Ver mi ticket"
        );

        await EnviarConLogging(usuario.Email, asunto, body);
    }

    // ─────────────────────────────────────────────────────────────
    // Cambio de estado – aviso al usuario dueño del ticket
    // ─────────────────────────────────────────────────────────────
    public async Task NotificarCambioEstadoAsync(
        Ticket ticket, Usuario usuario,
        EstadoTicket estadoAnterior, EstadoTicket estadoNuevo)
    {
        var (colorNuevo, emoji) = ObtenerColorYEmojiEstado(estadoNuevo);

        var asunto = $"[{_appNombre}] {emoji} Tu ticket cambió a \"{TraducirEstado(estadoNuevo)}\"";
        var body   = ConstruirEmail(
            titulo:   $"{emoji} Estado actualizado",
            intro:    $"Hola <strong>{usuario.Nombre}</strong>, el estado de tu ticket fue actualizado.",
            contenido: $@"
                <div style='display:flex;gap:12px;align-items:center;margin:20px 0;'>
                    <div style='flex:1;padding:12px;background:#f3f4f6;border-radius:6px;text-align:center;'>
                        <div style='font-size:12px;color:#777;margin-bottom:4px;'>Anterior</div>
                        <div style='font-weight:bold;color:#555;'>{TraducirEstado(estadoAnterior)}</div>
                    </div>
                    <div style='font-size:20px;color:#9ca3af;'>→</div>
                    <div style='flex:1;padding:12px;background:{colorNuevo}20;border:2px solid {colorNuevo};border-radius:6px;text-align:center;'>
                        <div style='font-size:12px;color:#777;margin-bottom:4px;'>Nuevo</div>
                        <div style='font-weight:bold;color:{colorNuevo};'>{TraducirEstado(estadoNuevo)}</div>
                    </div>
                </div>
                <table style='width:100%;border-collapse:collapse;'>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;width:140px;'>Ticket</td>
                        <td style='padding:8px;'>{ticket.Titulo}</td></tr>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;'>ID</td>
                        <td style='padding:8px;'>{ticket.Id.ToString()[..8].ToUpper()}</td></tr>
                </table>",
            botonUrl:   $"{_frontendBase}/mis-tickets/{ticket.Id}",
            botonTexto: "Ver ticket"
        );

        await EnviarConLogging(usuario.Email, asunto, body);
    }

    // ─────────────────────────────────────────────────────────────
    // Asignación de operador – aviso al operador
    // ─────────────────────────────────────────────────────────────
    public async Task NotificarAsignacionOperadorAsync(Ticket ticket, Usuario operador)
    {
        var asunto = $"[{_appNombre}] 📋 Nuevo ticket asignado: {ticket.Titulo}";
        var body   = ConstruirEmail(
            titulo:   "📋 Te asignaron un ticket",
            intro:    $"Hola <strong>{operador.Nombre}</strong>, se te asignó un ticket para gestionar.",
            contenido: $@"
                <table style='width:100%;border-collapse:collapse;margin:16px 0;'>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;width:140px;'>ID</td>
                        <td style='padding:8px;'>{ticket.Id.ToString()[..8].ToUpper()}</td></tr>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;'>Asunto</td>
                        <td style='padding:8px;'>{ticket.Titulo}</td></tr>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;'>Prioridad</td>
                        <td style='padding:8px;'><strong style='color:{ObtenerColorPrioridad(ticket.Prioridad)};'>{TraducirPrioridad(ticket.Prioridad)}</strong></td></tr>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;'>Estado</td>
                        <td style='padding:8px;'>{TraducirEstado(ticket.Estado)}</td></tr>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;'>Fecha</td>
                        <td style='padding:8px;'>{ticket.FechaCreacion:dd/MM/yyyy HH:mm}</td></tr>
                </table>
                <p style='color:#555;'>Ingresá al panel para ver el detalle completo y gestionar el ticket.</p>",
            botonUrl:   $"{_frontendBase}/operador/tickets/{ticket.Id}",
            botonTexto: "Gestionar ticket"
        );

        await EnviarConLogging(operador.Email, asunto, body);
    }

    // ─────────────────────────────────────────────────────────────
    // Nuevo comentario – aviso al creador del ticket
    // ─────────────────────────────────────────────────────────────
    public async Task NotificarNuevoComentarioAsync(
        Ticket ticket, Usuario destinatario,
        ComentarioTicket comentario, Usuario autor)
    {
        // No notificar si el autor es el mismo que el destinatario
        if (autor.Id == destinatario.Id) return;

        var asunto = $"[{_appNombre}] 💬 Nuevo comentario en tu ticket: {ticket.Titulo}";
        var body   = ConstruirEmail(
            titulo:   "💬 Nuevo comentario",
            intro:    $"Hola <strong>{destinatario.Nombre}</strong>, hay una nueva respuesta en tu ticket.",
            contenido: $@"
                <div style='border-left:4px solid #2563eb;padding:12px 16px;background:#f0f7ff;border-radius:0 6px 6px 0;margin:16px 0;'>
                    <div style='font-size:12px;color:#777;margin-bottom:6px;'>
                        <strong>{autor.Nombre}</strong> · {comentario.FechaCreacion:dd/MM/yyyy HH:mm}
                    </div>
                    <div style='color:#333;white-space:pre-wrap;'>{EscaparHtml(comentario.Mensaje)}</div>
                </div>
                <table style='width:100%;border-collapse:collapse;margin-top:16px;'>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;width:140px;'>Ticket</td>
                        <td style='padding:8px;'>{ticket.Titulo}</td></tr>
                    <tr><td style='padding:8px;background:#f3f4f6;font-weight:bold;'>ID</td>
                        <td style='padding:8px;'>{ticket.Id.ToString()[..8].ToUpper()}</td></tr>
                </table>",
            botonUrl:   $"{_frontendBase}/mis-tickets/{ticket.Id}",
            botonTexto: "Responder"
        );

        await EnviarConLogging(destinatario.Email, asunto, body);
    }

    // ─────────────────────────────────────────────────────────────
    // Template base de email (reutilizable)
    // ─────────────────────────────────────────────────────────────
    private string ConstruirEmail(string titulo, string intro, string contenido, string botonUrl, string botonTexto)
    {
        return $@"
        <!DOCTYPE html>
        <html lang='es'>
        <head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'></head>
        <body style='margin:0;padding:0;background-color:#f4f5f7;font-family:Arial,sans-serif;'>
            <table width='100%' cellpadding='0' cellspacing='0' style='background:#f4f5f7;padding:32px 0;'>
                <tr><td align='center'>
                    <table width='600' cellpadding='0' cellspacing='0' style='max-width:600px;width:100%;background:#ffffff;border-radius:10px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,0.08);'>

                        <!-- Header -->
                        <tr>
                            <td style='background:linear-gradient(135deg,#1d4ed8,#2563eb);padding:28px 32px;'>
                                <h1 style='margin:0;color:white;font-size:22px;letter-spacing:0.5px;'>{_appNombre}</h1>
                                <p style='margin:6px 0 0;color:#bfdbfe;font-size:13px;'>Sistema de gestión de tickets</p>
                            </td>
                        </tr>

                        <!-- Título -->
                        <tr>
                            <td style='padding:28px 32px 8px;'>
                                <h2 style='margin:0;color:#1e293b;font-size:20px;'>{titulo}</h2>
                            </td>
                        </tr>

                        <!-- Intro -->
                        <tr>
                            <td style='padding:8px 32px;color:#475569;font-size:15px;line-height:1.6;'>
                                {intro}
                            </td>
                        </tr>

                        <!-- Contenido dinámico -->
                        <tr>
                            <td style='padding:12px 32px;'>
                                {contenido}
                            </td>
                        </tr>

                        <!-- Botón CTA -->
                        <tr>
                            <td style='padding:20px 32px 32px;text-align:center;'>
                                <a href='{botonUrl}'
                                   style='display:inline-block;background:#2563eb;color:white;padding:13px 32px;
                                          text-decoration:none;border-radius:6px;font-weight:bold;font-size:15px;
                                          letter-spacing:0.3px;'>
                                    {botonTexto}
                                </a>
                            </td>
                        </tr>

                        <!-- Footer -->
                        <tr>
                            <td style='background:#f8fafc;padding:16px 32px;border-top:1px solid #e2e8f0;text-align:center;'>
                                <p style='margin:0;color:#94a3b8;font-size:12px;'>
                                    Este es un mensaje automático de <strong>{_appNombre}</strong>. Por favor no respondas este correo.
                                </p>
                            </td>
                        </tr>

                    </table>
                    <p style='color:#94a3b8;font-size:11px;margin-top:16px;'>© {DateTime.UtcNow.Year} {_appNombre}. Todos los derechos reservados.</p>
                </td></tr>
            </table>
        </body>
        </html>";
    }

    // ─────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────
    private async Task EnviarConLogging(string destinatario, string asunto, string body)
    {
        try
        {
            await _servicioEmail.EnviarEmailAsync(destinatario, asunto, body);
        }
        catch (Exception ex)
        {
            // No lanzar excepción: las notificaciones nunca deben romper el flujo principal
            Console.WriteLine($"[NOTIFICACION ERROR] → {destinatario} | {asunto} | {ex.Message}");
        }
    }

    private static string TraducirEstado(EstadoTicket estado) => estado switch
    {
        EstadoTicket.Pendiente        => "Pendiente",
        EstadoTicket.EnProceso        => "En proceso",
        EstadoTicket.EsperandoUsuario => "Esperando usuario",
        EstadoTicket.Resuelto         => "Resuelto",
        EstadoTicket.Cerrado          => "Cerrado",
        _                        => estado.ToString()
    };

    private static string TraducirPrioridad(PrioridadTicket prioridad) => prioridad switch
    {
        PrioridadTicket.Baja    => "🟢 Baja",
        PrioridadTicket.Media   => "🟡 Media",
        PrioridadTicket.Alta    => "🟠 Alta",
        PrioridadTicket.Critica => "🔴 Crítica",
        _                       => prioridad.ToString()
    };

    private static (string color, string emoji) ObtenerColorYEmojiEstado(EstadoTicket estado) => estado switch
    {
        EstadoTicket.Pendiente        => ("#f59e0b", "⏳"),
        EstadoTicket.EnProceso        => ("#3b82f6", "🔄"),
        EstadoTicket.EsperandoUsuario => ("#a855f7", "⏸️"),
        EstadoTicket.Resuelto         => ("#10b981", "✅"),
        EstadoTicket.Cerrado          => ("#6b7280", "🔒"),
        _                       => ("#2563eb", "📋")
    };

    private static string ObtenerColorPrioridad(PrioridadTicket prioridad) => prioridad switch
    {
        PrioridadTicket.Baja    => "#10b981",
        PrioridadTicket.Media   => "#f59e0b",
        PrioridadTicket.Alta    => "#ef4444",
        PrioridadTicket.Critica => "#7c3aed",
        _                       => "#2563eb"
    };

    private static string EscaparHtml(string texto) =>
        texto
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;");
}