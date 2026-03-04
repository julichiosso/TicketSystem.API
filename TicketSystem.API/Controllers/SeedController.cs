using Microsoft.AspNetCore.Mvc;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Dominio.Enumeraciones;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly IRepositorioUsuarios _repositorioUsuarios;

    public SeedController(IRepositorioUsuarios repositorioUsuarios)
    {
        _repositorioUsuarios = repositorioUsuarios;
    }

    [HttpPost("crear-operador")]
    public async Task<IActionResult> CrearOperador([FromBody] CrearOperadorRequest request, [FromServices] IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
            return NotFound();

        try
        {
            var operador = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = request.Nombre,
                Email = request.Email,
                Rol = RolUsuario.Operador,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _repositorioUsuarios.CrearAsync(operador);

            return Ok(new
            {
                success = true,
                message = $"Operador {operador.Nombre} creado exitosamente",
                id = operador.Id
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }
}

public class CrearOperadorRequest
{
    public required string Nombre { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
