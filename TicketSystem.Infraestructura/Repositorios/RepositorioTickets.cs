using Microsoft.EntityFrameworkCore;
using TicketSystem.Aplicacion.DTOs;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Infraestructura.Datos;

namespace TicketSystem.Infraestructura.Repositorios;

public class RepositorioTickets : IRepositorioTickets
{
    private readonly TicketSystemDbContext _context;

    public RepositorioTickets(TicketSystemDbContext context)
    {
        _context = context;
    }

    public async Task CrearAsync(Ticket ticket)
    {
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Ticket>> ObtenerPorUsuarioAsync(Guid usuarioId)
    {
        return await _context.Tickets
            .Include(t => t.Usuario)
            .Include(t => t.OperadorAsignado)
            .Where(t => t.UsuarioId == usuarioId && !t.IsDeleted)
            .ToListAsync();
    }

    public async Task<Ticket?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Tickets
            .Include(t => t.Usuario)
            .Include(t => t.OperadorAsignado)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Ticket> Tickets, int Total)> ObtenerFiltradosAsync(FiltroTicketsDto filtro)
    {
        var query = _context.Tickets
            .Include(t => t.Usuario)
            .Include(t => t.OperadorAsignado)
            .Where(t => !t.IsDeleted)
            .AsQueryable();

        if (filtro.Estado.HasValue)
            query = query.Where(t => t.Estado == filtro.Estado.Value);

        if (filtro.Prioridad.HasValue)
            query = query.Where(t => t.Prioridad == filtro.Prioridad.Value);

        if (filtro.UsuarioId.HasValue)
            query = query.Where(t => t.UsuarioId == filtro.UsuarioId.Value);

        if (filtro.OperadorId.HasValue)
            query = query.Where(t => t.OperadorAsignadoId == filtro.OperadorId.Value);

        if (filtro.FechaDesde.HasValue)
            query = query.Where(t => t.FechaCreacion >= filtro.FechaDesde.Value);

        if (filtro.FechaHasta.HasValue)
            query = query.Where(t => t.FechaCreacion <= filtro.FechaHasta.Value);

        if (!string.IsNullOrEmpty(filtro.Titulo))
            query = query.Where(t => t.Titulo.Contains(filtro.Titulo));

        var total = await query.CountAsync();

        query = filtro.SortBy?.ToLower() switch
        {
            "titulo" => filtro.Descending ? query.OrderByDescending(t => t.Titulo) : query.OrderBy(t => t.Titulo),
            "estado" => filtro.Descending ? query.OrderByDescending(t => t.Estado) : query.OrderBy(t => t.Estado),
            "prioridad" => filtro.Descending ? query.OrderByDescending(t => t.Prioridad) : query.OrderBy(t => t.Prioridad),
            "fechacreacion" => filtro.Descending ? query.OrderByDescending(t => t.FechaCreacion) : query.OrderBy(t => t.FechaCreacion),
            _ => query.OrderByDescending(t => t.FechaCreacion)
        };

        var tickets = await query
            .Skip((filtro.Page - 1) * filtro.PageSize)
            .Take(filtro.PageSize)
            .ToListAsync();

        return (tickets, total);
    }

    public IQueryable<Ticket> ObtenerQueryable()
    {
        return _context.Tickets.Where(t => !t.IsDeleted).AsQueryable();
    }

    public async Task ActualizarAsync(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Ticket>> ObtenerPorOperadorAsync(Guid operadorId)
    {
        return await _context.Tickets
            .Include(t => t.Usuario)
            .Include(t => t.OperadorAsignado)
            .Where(t => t.OperadorAsignadoId == operadorId && !t.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<ComentarioTicket>> ObtenerComentariosPorTicketAsync(Guid ticketId, bool incluirInternos)
    {
        var query = _context.ComentariosTicket
            .Include(c => c.Autor)
            .Where(c => c.TicketId == ticketId);

        if (!incluirInternos)
            query = query.Where(c => !c.EsInterno);

        return await query
            .OrderBy(c => c.FechaCreacion)
            .ToListAsync();
    }

    public async Task AgregarComentarioAsync(ComentarioTicket comentario)
    {
        await _context.ComentariosTicket.AddAsync(comentario);
        await _context.SaveChangesAsync();
    }

    public async Task RegistrarAuditLogAsync(AuditLog log)
    {
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditLog>> ObtenerAuditLogsAsync()
    {
        return await _context.AuditLogs
            .OrderByDescending(l => l.Fecha)
            .Take(100)
            .ToListAsync();
    }
}
