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
        var refreshToken = _tokenService.GenerarRefreshToken();

        var refreshDays = _configuration.GetValue<int>("Jwt:RefreshExpiresInDays", 7);
        usuario.RefreshToken = refreshToken;
        usuario.RefreshTokenExpires = DateTime.UtcNow.AddDays(refreshDays);
        await _repositorioUsuarios.ActualizarAsync(usuario);

        return Ok(new
        {
            token,
            refreshToken,
            usuario = new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Email,
                usuario.Rol
            }
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var usuario = await _repositorioUsuarios.ObtenerPorRefreshTokenAsync(request.RefreshToken);

        if (usuario == null || usuario.RefreshTokenExpires < DateTime.UtcNow)
            return Unauthorized(new { message = "Refresh token inválido o expirado." });

        var nuevoToken = _tokenService.GenerarToken(usuario);
        var nuevoRefreshToken = _tokenService.GenerarRefreshToken();

        var refreshDays = _configuration.GetValue<int>("Jwt:RefreshExpiresInDays", 7);
        usuario.RefreshToken = nuevoRefreshToken;
        usuario.RefreshTokenExpires = DateTime.UtcNow.AddDays(refreshDays);
        await _repositorioUsuarios.ActualizarAsync(usuario);

        return Ok(new
        {
            token = nuevoToken,
            refreshToken = nuevoRefreshToken
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
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null) return Unauthorized();

        var usuario = await _repositorioUsuarios.ObtenerPorIdAsync(Guid.Parse(id));
        if (usuario != null)
        {
            usuario.RefreshToken = null;
            usuario.RefreshTokenExpires = null;
            await _repositorioUsuarios.ActualizarAsync(usuario);
        }

        return NoContent();
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
            return Ok(new { message = "Si el correo está registrado, recibirás un enlace de recuperación." });

        var tokenBytes = new byte[16];
        RandomNumberGenerator.Fill(tokenBytes);
        var token = Convert.ToHexString(tokenBytes).ToUpper();

        usuario.PasswordResetToken = token;
        usuario.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);
        await _repositorioUsuarios.ActualizarAsync(usuario);

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
                    <p style='color: #555;'>Tu código de recuperación es:</p>
                    <div style='background-color: #f3f4f6; padding: 15px; text-align: center; font-size: 20px; font-weight: bold; border-radius: 4px; margin: 20px 0; color: #2563eb; letter-spacing: 2px;'>
                        {token}
                    </div>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetUrl}' style='background-color: #2563eb; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Cambiar contraseña</a>
                    </div>
                    <p style='color: #777; font-size: 12px;'>El código expirará en 1 hora.</p>
                </div>
            </div>";

        try
        {
            await _servicioEmail.EnviarEmailAsync(request.Email, "Recuperación de Contraseña - TicketSystem", body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SMTP ERROR] {ex.Message}");
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