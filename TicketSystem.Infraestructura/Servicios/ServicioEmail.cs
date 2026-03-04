using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using TicketSystem.Aplicacion.Interfaces;

namespace TicketSystem.Infraestructura.Servicios;

public class ServicioEmail : IServicioEmail
{
    private readonly IConfiguration _configuration;

    public ServicioEmail(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task EnviarEmailAsync(string destinatario, string asunto, string cuerpoHtml)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var host = emailSettings["Host"];
        var port = int.Parse(emailSettings["Port"] ?? "587");
        var username = emailSettings["Username"];
        var password = emailSettings["Password"];
        var enableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true");
        var displayName = emailSettings["DisplayName"] ?? "TicketSystem";

        using var client = new SmtpClient(host, port);
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(username, password);
        client.EnableSsl = enableSsl;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;

        var pickupDirectory = emailSettings["PickupDirectoryLocation"];
        if (!string.IsNullOrEmpty(pickupDirectory))
        {
            if (!Directory.Exists(pickupDirectory))
            {
                Directory.CreateDirectory(pickupDirectory);
            }
            client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            client.PickupDirectoryLocation = pickupDirectory;
        }

        var mailMessage = new MailMessage
        {
            From = new MailAddress(username!, displayName),
            Subject = asunto,
            Body = cuerpoHtml,
            IsBodyHtml = true
        };

        mailMessage.To.Add(destinatario);

        try {
            await client.SendMailAsync(mailMessage);
            Console.WriteLine($"[DEBUG] Email sent successfully to {destinatario} using {username}");
        } catch (Exception ex) {
            Console.WriteLine($"[ERROR] SMTP Failed to {destinatario} using {username}. Error: {ex.Message}");
            if (ex.InnerException != null) Console.WriteLine($"[ERROR] Inner Exception: {ex.InnerException.Message}");
            throw;
        }
    }
}
