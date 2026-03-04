using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Security.Cryptography;
using TicketSystem.API.DTOs;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Dominio.Enumeraciones;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IRepositorioUsuarios _repositorioUsuarios;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly IServicioEmail _servicioEmail;
    private readonly IConfiguration _configuration;

    public AuthController(
        IRepositorioUsuarios repositorioUsuarios,
        ITokenService tokenService,
        IPasswordHasher<Usuario> passwordHasher,
        IServicioEmail servicioEmail,
        IConfiguration configuration)
    {
        _repositorioUsuarios = repositorioUsuarios;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _servicioEmail = servicioEmail;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var usuario = await _repositorioUsuarios.ObtenerPorEmailAsync(request.Email);

        if (usuario == null)
            throw new UnauthorizedAccessException("Credenciales inválidas");

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.PasswordHash,
            request.Password);

        if (resultado == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Credenciales inválidas");

        var token = _tokenService.GenerarToken(usuario);

        return Ok(new
        {
            token,
            usuario = new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Email,
                usuario.Rol
            }
        });
    }

    [HttpPost("register")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var usuarioExistente = await _repositorioUsuarios.ObtenerPorEmailAsync(request.Email);

        if (usuarioExistente != null)
            throw new ArgumentException("El usuario ya existe");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Email = request.Email,
            Rol = RolUsuario.Usuario
        };

        usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.Password);
        await _repositorioUsuarios.AgregarAsync(usuario);

        return Ok(new { message = "Usuario registrado correctamente" });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var nombre = User.FindFirst(ClaimTypes.Name)?.Value;
        var rol = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new { Id = id, Email = email, Nombre = nombre, Rol = rol });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
    {
        var existe = await _repositorioUsuarios.ObtenerPorEmailAsync(request.Email);
        if (existe != null)
            throw new ArgumentException("El usuario ya existe");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Email = request.Email,
            Rol = RolUsuario.Administrador
        };

        usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.Password);
        await _repositorioUsuarios.AgregarAsync(usuario);

        return Ok(new { message = "Administrador creado correctamente" });
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var usuario = await _repositorioUsuarios.ObtenerPorEmailAsync(request.Email);
        if (usuario == null)
        {
            // Security: don't reveal if user exists or not
            return Ok(new { message = "Si el correo está registrado, recibirás un enlace de recuperación." });
        }

        // Generate a cryptographically secure token (32 hex chars)
        var tokenBytes = new byte[16];
        RandomNumberGenerator.Fill(tokenBytes);
        var token = Convert.ToHexString(tokenBytes).ToUpper(); // 32 chars

        usuario.PasswordResetToken = token;
        usuario.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);
        await _repositorioUsuarios.ActualizarAsync(usuario);

        // Log token for development testing
        Console.WriteLine($"[DEBUG] Reset Token for {request.Email}: {token}");

        var frontendBase = _configuration["AppSettings:FrontendBaseUrl"] ?? "http://localhost:5173";
        var resetUrl = $"{frontendBase}/reset-password?email={Uri.EscapeDataString(request.Email)}&token={token}";

        var body = $@"
            <div style='font-family: sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                <div style='background-color: #2563eb; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                    <h1 style='margin: 0; font-size: 24px;'>TicketSystem</h1>
                </div>
                <div style='padding: 20px; background-color: #ffffff;'>
                    <h2 style='color: #333;'>Solicitud de cambio de contraseña</h2>
                    <p style='color: #555; line-height: 1.6;'>Has solicitado restablecer tu contraseña para tu cuenta en <strong>TicketSystem</strong>.</p>
                    <p style='color: #555; line-height: 1.6;'>Tu código de recuperación es:</p>
                    <div style='background-color: #f3f4f6; padding: 15px; text-align: center; font-size: 20px; font-weight: bold; border-radius: 4px; margin: 20px 0; color: #2563eb; letter-spacing: 2px; word-break: break-all;'>
                        {token}
                    </div>
                    <p style='color: #555; line-height: 1.6; text-align: center;'>O hacé clic en el siguiente botón para continuar:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetUrl}' style='background-color: #2563eb; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>Cambiar contraseña</a>
                    </div>
                    <p style='color: #777; font-size: 12px; margin-top: 30px;'>Si no solicitaste este cambio, podés ignorar este correo de forma segura. El código expirará en 1 hora.</p>
                </div>
                <div style='text-align: center; padding: 20px; color: #aaa; font-size: 11px;'>
                    &copy; {DateTime.Now.Year} TicketSystem. Todos los derechos reservados.
                </div>
            </div>";

        try
        {
            await _servicioEmail.EnviarEmailAsync(request.Email, "Recuperación de Contraseña - TicketSystem", body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SMTP ERROR] {ex.Message}");
            // Return success anyway in dev to allow testing via console log
            return Ok(new { message = "Si el correo está registrado, recibirás un enlace de recuperación. (Revisa la consola para el token en desarrollo)" });
        }

        return Ok(new { message = "Si el correo está registrado, recibirás un enlace de recuperación." });
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var usuario = await _repositorioUsuarios.ObtenerPorEmailAsync(request.Email);
        if (usuario == null)
            return BadRequest(new { message = "Enlace de recuperación inválido o expirado." });

        if (usuario.PasswordResetToken != request.Token)
            return BadRequest(new { message = "Token inválido." });

        if (usuario.PasswordResetTokenExpires < DateTime.UtcNow)
            return BadRequest(new { message = "El token ha expirado." });

        usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.NewPassword);
        usuario.PasswordResetToken = null;
        usuario.PasswordResetTokenExpires = null;

        await _repositorioUsuarios.ActualizarAsync(usuario);

        return Ok(new { message = "Contraseña restablecida correctamente." });
    }
}
