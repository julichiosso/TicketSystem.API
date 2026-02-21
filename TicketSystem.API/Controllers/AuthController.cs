using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

    public AuthController(
        IRepositorioUsuarios repositorioUsuarios,
        ITokenService tokenService,
        IPasswordHasher<Usuario> passwordHasher)
    {
        _repositorioUsuarios = repositorioUsuarios;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var usuario = await _repositorioUsuarios
            .ObtenerPorEmailAsync(request.Email);

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
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var usuarioExistente = await _repositorioUsuarios
            .ObtenerPorEmailAsync(request.Email);

        if (usuarioExistente != null)
            throw new ArgumentException("El usuario ya existe");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Email = request.Email,
            Rol = RolUsuario.Usuario
        };

        usuario.PasswordHash =
            _passwordHasher.HashPassword(usuario, request.Password);

        await _repositorioUsuarios.AgregarAsync(usuario);

        return Ok(new
        {
            message = "Usuario registrado correctamente"
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var rol = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            Id = id,
            Email = email,
            Rol = rol
        });
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

        usuario.PasswordHash =
            _passwordHasher.HashPassword(usuario, request.Password);

        await _repositorioUsuarios.AgregarAsync(usuario);

        return Ok(new { message = "Administrador creado correctamente" });
    }
}
