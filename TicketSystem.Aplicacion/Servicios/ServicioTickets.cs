using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketSystem.Aplicacion.Common;
using TicketSystem.Aplicacion.DTOs;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Dominio.Enumeraciones;
using Microsoft.EntityFrameworkCore;

namespace TicketSystem.Aplicacion.Servicios
{
    public class ServicioTickets : IServicioTickets
    {
        private readonly IRepositorioTickets _repositorioTickets;
        private readonly IRepositorioUsuarios _repositorioUsuarios;

        public ServicioTickets(
            IRepositorioTickets repositorioTickets,
            IRepositorioUsuarios repositorioUsuarios)
        {
            _repositorioTickets = repositorioTickets;
            _repositorioUsuarios = repositorioUsuarios;
        }

        public async Task<Guid> CrearAsync(CrearTicketDto dto)
        {
            var usuario = await _repositorioUsuarios.ObtenerPorIdAsync(dto.UsuarioId);

            if (usuario == null)
                throw new Exception("El usuario no existe");

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Estado = EstadoTicket.Pendiente,
                Prioridad = dto.Prioridad,
                UsuarioId = dto.UsuarioId,
                FechaCreacion = DateTime.UtcNow
            };

            await _repositorioTickets.CrearAsync(ticket);
            return ticket.Id;
        }

        public async Task<IEnumerable<TicketDto>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            var tickets = await _repositorioTickets.ObtenerPorUsuarioAsync(usuarioId);

            return tickets.Select(t => new TicketDto
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                Estado = t.Estado,
                Prioridad = t.Prioridad,
                FechaCreacion = t.FechaCreacion
            });
        }
        private bool EsTransicionValida(EstadoTicket actual, EstadoTicket nuevo)
        {
            return (actual, nuevo) switch
            {
                (EstadoTicket.Pendiente, EstadoTicket.EnProceso) => true,
                (EstadoTicket.EnProceso, EstadoTicket.Resuelto) => true,
                _ => false
            };
        }

        public async Task CambiarEstadoAsync(Guid ticketId, EstadoTicket nuevoEstado)
        {
            var ticket = await _repositorioTickets.ObtenerPorIdAsync(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("El ticket no existe");

            if (!EsTransicionValida(ticket.Estado, nuevoEstado))
                throw new ArgumentException(
                    $"No se puede cambiar de {ticket.Estado} a {nuevoEstado}");

            ticket.Estado = nuevoEstado;

            await _repositorioTickets.GuardarCambiosAsync();
        }

        public async Task<PagedResult<TicketDto>> ObtenerFiltradosAsync(FiltroTicketsDto filtro)
        {
            var query = _repositorioTickets.ObtenerQueryable();

            
            if (filtro.Estado.HasValue)
                query = query.Where(t => t.Estado == filtro.Estado.Value);

            if (!string.IsNullOrWhiteSpace(filtro.Titulo))
                query = query.Where(t => t.Titulo.Contains(filtro.Titulo));

            var totalRecords = await query.CountAsync();

            var tickets = await query
                .OrderByDescending(t => t.FechaCreacion)
                .Skip((filtro.Page - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToListAsync();

            return new PagedResult<TicketDto>
            {
                Data = tickets.Select(t => new TicketDto
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    Estado = t.Estado,
                    Prioridad = t.Prioridad,
                    FechaCreacion = t.FechaCreacion
                }),
                Page = filtro.Page,
                PageSize = filtro.PageSize,
                TotalRecords = totalRecords
            };
        }
        public async Task EliminarAsync(Guid id)
        {
            var ticket = await _repositorioTickets.ObtenerPorIdAsync(id);

            if (ticket == null)
                throw new Exception("Ticket no encontrado");

            ticket.IsDeleted = true;

            await _repositorioTickets.GuardarCambiosAsync();
        }

    }
}
