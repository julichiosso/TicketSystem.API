using TicketSystem.Aplicacion.Common;
using TicketSystem.Aplicacion.DTOs;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Dominio.Enumeraciones;

namespace TicketSystem.Aplicacion.Servicios
{
    public class ServicioTickets : IServicioTickets
    {
        private readonly IRepositorioTickets _repositorioTickets;
        private readonly IRepositorioUsuarios _repositorioUsuarios;
        private readonly IServicioNotificaciones _notificaciones;

        public ServicioTickets(
            IRepositorioTickets repositorioTickets,
            IRepositorioUsuarios repositorioUsuarios,
            IServicioNotificaciones notificaciones)
        {
            _repositorioTickets  = repositorioTickets;
            _repositorioUsuarios = repositorioUsuarios;
            _notificaciones      = notificaciones;
        }

        // ─────────────────────────────────────────────────────────────
        // CREAR TICKET
        // ─────────────────────────────────────────────────────────────
        public async Task<Guid> CrearAsync(CrearTicketDto dto)
        {
            var usuario = await _repositorioUsuarios.ObtenerPorIdAsync(dto.UsuarioId)
                ?? throw new KeyNotFoundException("El usuario no existe");

            var fechaCreacion = DateTime.UtcNow;

            DateTime? fechaLimite = dto.Prioridad switch
            {
                PrioridadTicket.Critica => fechaCreacion.AddHours(2),
                PrioridadTicket.Alta    => fechaCreacion.AddHours(4),
                PrioridadTicket.Media   => fechaCreacion.AddHours(24),
                PrioridadTicket.Baja    => fechaCreacion.AddHours(48),
                _                       => (DateTime?)null
            };

            var ticket = new Ticket
            {
                Id            = Guid.NewGuid(),
                Titulo        = dto.Titulo,
                Descripcion   = dto.Descripcion,
                Estado        = EstadoTicket.Pendiente,
                Prioridad     = dto.Prioridad,
                UsuarioId     = dto.UsuarioId,
                FechaCreacion = fechaCreacion,
                FechaLimite   = fechaLimite,
                SLACumplido   = true
            };

            await _repositorioTickets.CrearAsync(ticket);
            await RegistrarAccionAsync(ticket.Id, dto.UsuarioId, "CREACIÓN", $"Ticket '{ticket.Titulo}' creado con prioridad {ticket.Prioridad}.");

            // Notificación: confirmación al usuario
            await _notificaciones.NotificarTicketCreadoAsync(ticket, usuario);

            return ticket.Id;
        }

        // ─────────────────────────────────────────────────────────────
        // OBTENER TICKETS
        // ─────────────────────────────────────────────────────────────
        public async Task<IEnumerable<TicketDto>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            var tickets = await _repositorioTickets.ObtenerPorUsuarioAsync(usuarioId);
            return MapToDto(tickets);
        }

        public async Task<IEnumerable<TicketDto>> ObtenerPorOperadorAsync(Guid operadorId)
        {
            var tickets = await _repositorioTickets.ObtenerPorOperadorAsync(operadorId);
            return MapToDto(tickets);
        }

        public async Task<PagedResult<TicketDto>> ObtenerFiltradosAsync(FiltroTicketsDto filtro)
        {
            var (tickets, total) = await _repositorioTickets.ObtenerFiltradosAsync(filtro);

            return new PagedResult<TicketDto>
            {
                Data         = MapToDto(tickets),
                Page         = filtro.Page,
                PageSize     = filtro.PageSize,
                TotalRecords = total
            };
        }

        // ─────────────────────────────────────────────────────────────
        // CAMBIAR ESTADO
        // ─────────────────────────────────────────────────────────────
        public async Task CambiarEstadoAsync(Guid ticketId, EstadoTicket nuevoEstado, Guid actorId)
        {
            var ticket = await _repositorioTickets.ObtenerPorIdAsync(ticketId)
                ?? throw new KeyNotFoundException("El ticket no existe");

            if (ticket.Estado == EstadoTicket.Cerrado)
                throw new ArgumentException("No se puede cambiar el estado de un ticket cerrado.");

            var estadoAnterior = ticket.Estado;
            ticket.Estado = nuevoEstado;

            if (nuevoEstado == EstadoTicket.Resuelto)
            {
                ticket.FechaResolucion = DateTime.UtcNow;
                if (ticket.FechaLimite.HasValue)
                    ticket.SLACumplido = ticket.FechaResolucion <= ticket.FechaLimite.Value;
            }

            await _repositorioTickets.ActualizarAsync(ticket);
            await RegistrarAccionAsync(ticketId, actorId, "ESTADO_CAMBIO", $"Cambio de estado de {estadoAnterior} a {nuevoEstado}.");

            // Notificación: aviso al dueño del ticket (solo si el actor no es él mismo)
            var duenio = await _repositorioUsuarios.ObtenerPorIdAsync(ticket.UsuarioId);
            if (duenio != null && duenio.Id != actorId)
                await _notificaciones.NotificarCambioEstadoAsync(ticket, duenio, estadoAnterior, nuevoEstado);
        }

        // ─────────────────────────────────────────────────────────────
        // ASIGNAR OPERADOR
        // ─────────────────────────────────────────────────────────────
        public async Task AsignarOperadorAsync(Guid ticketId, Guid? operadorId, Guid actorId)
        {
            var ticket = await _repositorioTickets.ObtenerPorIdAsync(ticketId)
                ?? throw new KeyNotFoundException("El ticket no existe");

            string nombreOperador = "Ninguno";
            Usuario? operador     = null;

            if (operadorId.HasValue)
            {
                operador = await _repositorioUsuarios.ObtenerPorIdAsync(operadorId.Value)
                    ?? throw new KeyNotFoundException("El operador no existe");

                if (operador.Rol != RolUsuario.Operador && operador.Rol != RolUsuario.Administrador)
                    throw new ArgumentException("El usuario no es un operador válido.");

                nombreOperador = operador.Nombre;
            }

            ticket.OperadorAsignadoId = operadorId;
            ticket.FechaAsignacion    = operadorId.HasValue ? DateTime.UtcNow : (DateTime?)null;

            await _repositorioTickets.ActualizarAsync(ticket);
            await RegistrarAccionAsync(ticketId, actorId, "ASIGNACIÓN", $"Operador asignado: {nombreOperador}.");

            // Notificación: aviso al operador asignado
            if (operador != null)
                await _notificaciones.NotificarAsignacionOperadorAsync(ticket, operador);
        }

        // ─────────────────────────────────────────────────────────────
        // ELIMINAR TICKET (soft delete)
        // ─────────────────────────────────────────────────────────────
        public async Task EliminarAsync(Guid id, Guid actorId)
        {
            var ticket = await _repositorioTickets.ObtenerPorIdAsync(id)
                ?? throw new KeyNotFoundException("Ticket no encontrado");

            ticket.IsDeleted = true;
            await _repositorioTickets.ActualizarAsync(ticket);
            await RegistrarAccionAsync(id, actorId, "DELECIÓN", $"Ticket '{ticket.Titulo}' eliminado.");
        }

        // ─────────────────────────────────────────────────────────────
        // COMENTARIOS
        // ─────────────────────────────────────────────────────────────
        public async Task<IEnumerable<ComentarioTicketDto>> ObtenerComentariosAsync(Guid ticketId, bool incluirInternos)
        {
            var ticket = await _repositorioTickets.ObtenerPorIdAsync(ticketId)
                ?? throw new KeyNotFoundException("El ticket no existe");

            var comentarios = await _repositorioTickets.ObtenerComentariosPorTicketAsync(ticketId, incluirInternos);

            return comentarios.Select(c => new ComentarioTicketDto
            {
                Id       = c.Id,
                TicketId = c.TicketId,
                AutorId  = c.AutorId,
                Autor    = c.Autor?.Nombre ?? "Sistema",
                Rol      = (int)(c.Autor?.Rol ?? RolUsuario.Usuario),
                Mensaje  = c.Mensaje,
                Interno  = c.EsInterno,
                Fecha    = c.FechaCreacion,
                Adjuntos = c.Adjuntos.Select(a => new AdjuntoDto
                {
                    Id             = a.Id,
                    NombreOriginal = a.NombreOriginal,
                    ContentType    = a.ContentType,
                    TamañoBytes    = a.TamañoBytes,
                    Url            = $"/uploads/{a.NombreAlmacenado}"
                }).ToList()
            });
        }

        public async Task<ComentarioTicketDto> AgregarComentarioAsync(Guid ticketId, Guid autorId, CrearComentarioTicketDto dto)
        {
            var ticket = await _repositorioTickets.ObtenerPorIdAsync(ticketId)
                ?? throw new KeyNotFoundException("El ticket no existe");

            var autor = await _repositorioUsuarios.ObtenerPorIdAsync(autorId)
                ?? throw new KeyNotFoundException("El autor no existe");

            var comentario = new ComentarioTicket
            {
                Id            = Guid.NewGuid(),
                TicketId      = ticketId,
                AutorId       = autorId,
                Mensaje       = dto.Mensaje,
                EsInterno     = dto.Interno,
                FechaCreacion = DateTime.UtcNow
            };

            await _repositorioTickets.AgregarComentarioAsync(comentario);
            await RegistrarAccionAsync(ticketId, autorId, "COMENTARIO", $"{(dto.Interno ? "[Interno] " : "")}Comentario agregado por {autor.Nombre}.");

            // Notificación: aviso al dueño del ticket si el autor es distinto
            // Los comentarios internos no se notifican al usuario final
            if (!dto.Interno)
            {
                var duenio = await _repositorioUsuarios.ObtenerPorIdAsync(ticket.UsuarioId);
                if (duenio != null)
                    await _notificaciones.NotificarNuevoComentarioAsync(ticket, duenio, comentario, autor);
            }

            return new ComentarioTicketDto
            {
                Id       = comentario.Id,
                TicketId = comentario.TicketId,
                AutorId  = comentario.AutorId,
                Autor    = autor.Nombre,
                Rol      = (int)autor.Rol,
                Mensaje  = comentario.Mensaje,
                Interno  = comentario.EsInterno,
                Fecha    = comentario.FechaCreacion
            };
        }

        // ─────────────────────────────────────────────────────────────
        // AUDITORÍA
        // ─────────────────────────────────────────────────────────────
        public async Task<IEnumerable<AuditLog>> ObtenerAuditLogsAsync()
        {
            return await _repositorioTickets.ObtenerAuditLogsAsync();
        }

        private async Task RegistrarAccionAsync(Guid? ticketId, Guid? usuarioId, string accion, string detalle)
        {
            await _repositorioTickets.RegistrarAuditLogAsync(new AuditLog
            {
                Id        = Guid.NewGuid(),
                TicketId  = ticketId,
                UsuarioId = usuarioId,
                Accion    = accion,
                Detalle   = detalle,
                Fecha     = DateTime.UtcNow
            });
        }

        // ─────────────────────────────────────────────────────────────
        // MÉTRICAS
        // ─────────────────────────────────────────────────────────────
        public async Task<MetricasDto> ObtenerMetricasAsync()
        {
            var tickets = _repositorioTickets.ObtenerQueryable().ToList();

            var resueltos      = tickets.Where(t => t.Estado == EstadoTicket.Resuelto).ToList();
            var totalResueltos = resueltos.Count;

            int resueltosHoy = resueltos.Count(t =>
                t.FechaResolucion.HasValue &&
                t.FechaResolucion.Value.Date == DateTime.UtcNow.Date);

            double slaCumplido = totalResueltos > 0
                ? (double)resueltos.Count(t => t.SLACumplido) / totalResueltos * 100
                : 100;

            string avgTime = "0h 0m";
            if (totalResueltos > 0)
            {
                var duraciones = resueltos
                    .Where(t => t.FechaResolucion.HasValue)
                    .Select(t => (t.FechaResolucion!.Value - t.FechaCreacion).TotalHours)
                    .ToList();

                if (duraciones.Any())
                {
                    var avgHoras = duraciones.Average();
                    avgTime = $"{(int)avgHoras}h {(int)((avgHoras - (int)avgHoras) * 60)}m";
                }
            }

            return new MetricasDto
            {
                TotalTickets              = tickets.Count,
                TicketsPendientes         = tickets.Count(t => t.Estado == EstadoTicket.Pendiente),
                TicketsEnProceso          = tickets.Count(t => t.Estado == EstadoTicket.EnProceso),
                TicketsResueltos          = totalResueltos,
                TicketsEsperandoUsuario   = tickets.Count(t => t.Estado == EstadoTicket.EsperandoUsuario),
                ResueltosHoy              = resueltosHoy,
                PorcentajeSlaCumplido     = Math.Round(slaCumplido, 1),
                TiempoPromedioResolucion  = avgTime,
                DistribucionPorEstado     = tickets
                    .GroupBy(t => t.Estado)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count())
            };
        }

        // ─────────────────────────────────────────────────────────────
        // MAPPER
        // ─────────────────────────────────────────────────────────────
        private static IEnumerable<TicketDto> MapToDto(IEnumerable<Ticket> tickets)
        {
            return tickets.Select(t => new TicketDto
            {
                Id                     = t.Id,
                Titulo                 = t.Titulo,
                Descripcion            = t.Descripcion,
                Estado                 = t.Estado,
                Prioridad              = t.Prioridad,
                FechaCreacion          = t.FechaCreacion,
                FechaAsignacion        = t.FechaAsignacion,
                FechaResolucion        = t.FechaResolucion,
                FechaLimite            = t.FechaLimite,
                SLACumplido            = t.SLACumplido,
                UsuarioId              = t.UsuarioId,
                UsuarioNombre          = t.Usuario?.Nombre,
                OperadorAsignadoId     = t.OperadorAsignadoId,
                OperadorAsignadoNombre = t.OperadorAsignado?.Nombre
            });
        }
    }
}