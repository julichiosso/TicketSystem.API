using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Aplicacion.DTOs;
using TicketSystem.Dominio.Enumeraciones;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IRepositorioUsuarios _repositorioUsuarios;
    private readonly IServicioUsuarios _servicioUsuarios;

    public UsuariosController(
        IRepositorioUsuarios repositorioUsuarios,
        IServicioUsuarios servicioUsuarios)
    {
        _repositorioUsuarios = repositorioUsuarios;
        _servicioUsuarios = servicioUsuarios;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GetUsuarios()
    {
        var usuarios = await _repositorioUsuarios.ObtenerTodosAsync();
        return Ok(new
        {
            success = true,
            data = usuarios.Select(u => new { u.Id, u.Nombre, u.Email, u.Rol })
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GetUsuario(Guid id)
    {
        var usuario = await _repositorioUsuarios.ObtenerPorIdAsync(id);
        if (usuario == null)
            throw new KeyNotFoundException("Usuario no encontrado");

        return Ok(new
        {
            success = true,
            data = new { usuario.Id, usuario.Nombre, usuario.Email, usuario.Rol }
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DeleteUsuario(Guid id)
    {
        var usuario = await _repositorioUsuarios.ObtenerPorIdAsync(id);
        if (usuario == null)
            throw new KeyNotFoundException("Usuario no encontrado");
    
        // Evitar que el admin se elimine a sí mismo
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == id.ToString())
            return BadRequest(new { message = "No podés eliminarte a vos mismo." });
    
        await _repositorioUsuarios.EliminarAsync(usuario);
        return NoContent();
    }


    [HttpPut("roles")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> UpdateRoles([FromBody] List<ActualizarRolDto> cambios)
    {
        if (cambios == null || cambios.Count == 0)
            return BadRequest(new { message = "No se proporcionaron cambios." });

        foreach (var cambio in cambios)
        {
            var usuario = await _repositorioUsuarios.ObtenerPorIdAsync(cambio.Id);
            if (usuario == null)
                return NotFound(new { message = $"Usuario {cambio.Id} no encontrado." });

            if (!Enum.IsDefined(typeof(RolUsuario), cambio.Rol))
                return BadRequest(new { message = $"Rol inválido para usuario {cambio.Id}." });

            usuario.Rol = (RolUsuario)cambio.Rol;
            await _repositorioUsuarios.ActualizarAsync(usuario);
        }

        return Ok(new { success = true, message = "Roles actualizados correctamente." });
    }

    // ── CAMBIAR CONTRASEÑA (cualquier usuario autenticado) ──────────────────
    [HttpPut("cambiar-password")]
    public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var usuarioId))
            return Unauthorized(new { message = "Token inválido." });

        try
        {
            await _servicioUsuarios.CambiarPasswordAsync(usuarioId, dto);
            return Ok(new { success = true, message = "Contraseña actualizada correctamente." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}