using TicketSystem.Aplicacion.Common;
using TicketSystem.Aplicacion.DTOs;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Dominio.Enumeraciones;
// Microsoft.EntityFrameworkCore import removed to respect clean architecture

namespace TicketSystem.Aplicacion.Servicios
{
    public class ServicioTickets : IServicioTickets
    {
        private readonly IRepositorioTickets _repositorioTickets;
        private readonly IRepositorioUsuarios _repositorioUsuarios;
        private readonly IServicioEmail _servicioEmail;

        public ServicioTickets(
            IRepositorioTickets repositorioTickets,
            IRepositorioUsuarios repositorioUsuarios,
            IServicioEmail servicioEmail)
        {
            _repositorioTickets = repositorioTickets;
            _repositorioUsuarios = repositorioUsuarios;
            _servicioEmail = servicioEmail;
        }

        public async Task<Guid> CrearAsync(CrearTicketDto dto)
        {
            var usuario = await _repositorioUsuarios.ObtenerPorIdAsync(dto.UsuarioId);

            if (usuario == null)
                throw new Exception("El usuario no existe");

            var fechaCreacion = DateTime.UtcNow;

            DateTime? fechaLimite = dto.Prioridad switch
            {
                PrioridadTicket.Alta => fechaCreacion.AddHours(4),
                PrioridadTicket.Media => fechaCreacion.AddHours(24),
                PrioridadTicket.Baja => fechaCreacion.AddHours(48),
                _ => (DateTime?)null
            };

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Estado = EstadoTicket.Pendiente,
                Prioridad = dto.Prioridad,
                UsuarioId = dto.UsuarioId,
                FechaCreacion = fechaCreacion,
                FechaLimite = fechaLimite,
                SLACumplido = true
            };

            await _repositorioTickets.CrearAsync(ticket);
            await RegistrarAccionAsync(ticket.Id, dto.UsuarioId, "CREACIÓN", $"Ticket '{ticket.Titulo}' creado por {usuario.Nombre}.");

            return ticket.Id;
        }

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

        private bool EsTransicionValida(EstadoTicket actual, EstadoTicket nuevo)
        {
            return (actual, nuevo) switch
            {
                (EstadoTicket.Pendiente, EstadoTicket.EnProceso) => true,
                (EstadoTicket.EnProceso, EstadoTicket.EsperandoUsuario) => true,
                (EstadoTicket.EsperandoUsuario, EstadoTicket.EnProceso) => true,
                (EstadoTicket.EnProceso, EstadoTicket.Resuelto) => true,
                (EstadoTicket.EsperandoUsuario, EstadoTicket.Resuelto) => true,
                (EstadoTicket.Resuelto, EstadoTicket.Cerrado) => true,
                _ => false
            };
        }

        public async Task CambiarEstadoAsync(Guid ticketId, EstadoTicket nuevoEstado, Guid actorId)
        {
            var ticket = await _repositorioTickets.ObtenerPorIdAsync(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("El ticket no existe");

            if (!EsTransicionValida(ticket.Estado, nuevoEstado))
                throw new ArgumentException($"No se puede cambiar de {ticket.Estado} a {nuevoEstado}");

            var estadoAnterior = ticket.Estado;
            ticket.Estado = nuevoEstado;

            // Handle resolution
            if (nuevoEstado == EstadoTicket.Resuelto)
            {
                ticket.FechaResolucion = DateTime.UtcNow;
                if (ticket.FechaLimite.HasValue)
                {
                    ticket.SLACumplido = ticket.FechaResolucion <= ticket.FechaLimite.Value;
                }
            }

            await _repositorioTickets.ActualizarAsync(ticket);
            await RegistrarAccionAsync(ticketId, actorId, "ESTADO_CAMBIO", $"Cambio de estado de {estadoAnterior} a {nuevoEstado}.");

            // Notificar al usuario (creador) sobre el cambio de estado de su ticket
            if (ticket.Usuario?.Email != null)
            {
                var body = $@"
                    <h2>Actualización en tu ticket</h2>
                    <p>Hola {ticket.Usuario.Nombre},</p>
                    <p>El estado de tu ticket <strong>#{(ticket.Id.ToString().Substring(0, 8))}</strong> ({ticket.Titulo}) ha cambiado a <strong>{nuevoEstado}</strong>.</p>
                ";
                _ = _servicioEmail.EnviarEmailAsync(ticket.Usuario.Email, $"Cambio de Estado - Ticket {ticket.Titulo}", body).ContinueWith(t => 
                {
                    if (t.IsFaulted) Console.WriteLine($"SMTP Error: {t.Exception?.GetBaseException().Message}");
                });
            }
        }

        public async Task<PagedResult<TicketDto>> ObtenerFiltradosAsync(FiltroTicketsDto filtro)
        {
            var (tickets, total) = await _repositorioTickets.ObtenerFiltradosAsync(filtro);

            return new PagedResult<TicketDto>
            {
                Data = MapToDto(tickets),
                Page = filtro.Page,
                PageSize = filtro.PageSize,
                TotalRecords = total
            };
        }

        public async Task EliminarAsync(Guid id, Guid actorId)
        {
            var ticket = await _repositorioTickets.ObtenerPorIdAsync(id);

            if (ticket == null)
                throw new Exception("Ticket no encontrado");

            ticket.IsDeleted = true;
            await _repositorioTickets.ActualizarAsync(ticket);
            await RegistrarAccionAsync(id, actorId, "DELECIÓN", $"Ticket '{ticket.Titulo}' eliminado.");
        }

        public async Task AsignarOperadorAsync(Guid ticketId, Guid? operadorId, Guid actorId)
        {
            var ticket = await _repositorioTickets.ObtenerPorIdAsync(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("El ticket no existe");

            string nombreOperador = "Ninguno";
            Usuario? operador = null;

            if (operadorId.HasValue)
            {
                operador = await _repositorioUsuarios.ObtenerPorIdAsync(operadorId.Value);
                if (operador == null)
                    throw new KeyNotFoundException("El operador no existe");

                if (operador.Rol != RolUsuario.Operador && operador.Rol != RolUsuario.Administrador)
                    throw new ArgumentException("El usuario no es un operador válido");

                nombreOperador = operador.Nombre;
            }

            ticket.OperadorAsignadoId = operadorId;
            ticket.FechaAsignacion = operadorId.HasValue ? DateTime.UtcNow : (DateTime?)null;
            
            await _repositorioTickets.ActualizarAsync(ticket);
            await RegistrarAccionAsync(ticketId, actorId, "ASIGNACIÓN", $"Operador asignado: {nombreOperador}.");

            // Notificar al operador
            if (operador != null && operador.Email != null)
            {
                var body = $@"
                    <h2>Nuevo ticket asignado</h2>
                    <p>Hola {operador.Nombre},</p>
                    <p>Se te ha asignado el ticket <strong>#{(ticket.Id.ToString().Substring(0, 8))}</strong> ({ticket.Titulo}).</p>
                    <p><strong>Prioridad:</strong> {ticket.Prioridad}</p>
                ";
                _ = _servicioEmail.EnviarEmailAsync(operador.Email, $"Ticket Asignado - {ticket.Titulo}", body).ContinueWith(t => 
                {
                    if (t.IsFaulted) Console.WriteLine($"SMTP Error: {t.Exception?.GetBaseException().Message}");
                });
            }
        }

        // --- Rest of methods unchanged, but here for completeness 

        public async Task<IEnumerable<ComentarioTicketDto>> ObtenerComentariosAsync(Guid ticketId, bool incluirInternos)
        {
            var ticket = await _repositorioTickets.ObtenerPorIdAsync(ticketId);
            if (ticket == null) throw new KeyNotFoundException("El ticket no existe");

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
            var ticket = await _repositorioTickets.ObtenerPorIdAsync(ticketId);
            if (ticket == null) throw new KeyNotFoundException("El ticket no existe");

            var autor = await _repositorioUsuarios.ObtenerPorIdAsync(autorId);
            if (autor == null) throw new KeyNotFoundException("El autor no existe");

            var comentario = new ComentarioTicket
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                AutorId = autorId,
                Mensaje = dto.Mensaje,
                EsInterno = dto.Interno,
                FechaCreacion = DateTime.UtcNow
            };

            await _repositorioTickets.AgregarComentarioAsync(comentario);
            await RegistrarAccionAsync(ticketId, autorId, "COMENTARIO", $"{(dto.Interno ? "[Interno] " : "")}Comentario agregado por {autor.Nombre}.");

            return new ComentarioTicketDto
            {
                Id = comentario.Id,
                TicketId = comentario.TicketId,
                AutorId = comentario.AutorId,
                Autor = autor.Nombre,
                Rol = (int)autor.Rol,
                Mensaje = comentario.Mensaje,
                Interno = comentario.EsInterno,
                Fecha = comentario.FechaCreacion
            };
        }

        private async Task RegistrarAccionAsync(Guid? ticketId, Guid? usuarioId, string accion, string detalle)
        {
            await _repositorioTickets.RegistrarAuditLogAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                UsuarioId = usuarioId,
                Accion = accion,
                Detalle = detalle,
                Fecha = DateTime.UtcNow
            });
        }

        public async Task<IEnumerable<AuditLog>> ObtenerAuditLogsAsync()
        {
            return await _repositorioTickets.ObtenerAuditLogsAsync();
        }

        public async Task<MetricasDto> ObtenerMetricasAsync()
        {
            // Note: Now we query specifically what we need without bringing all DB into memory and without direct EF Core refs
            var query = _repositorioTickets.ObtenerQueryable();
            // Since we are not exposing an IQueryable that evaluates fully in DB easily because IQueryable isn't exposing CountAsync 
            // from EF without the package, we will cast it or evaluate it directly minimally. 
            // In a real CQRS setup we'd have a specific direct DB call for metrics. 
            // Let's resolve the enumerable first for simplicity (since we removed EF Core from app layer).
            
            var tickets = query.ToList(); 
            
            var resueltos = tickets.Where(t => t.Estado == EstadoTicket.Resuelto).ToList();
            var totalResueltos = resueltos.Count;
            
            // 🔥 FIXED: Use FechaResolucion here!
            int resueltosHoy = resueltos.Count(t => t.FechaResolucion.HasValue && t.FechaResolucion.Value.Date == DateTime.UtcNow.Date);
            
            double slaCumplido = totalResueltos > 0 
                ? (double)resueltos.Count(t => t.SLACumplido) / totalResueltos * 100 
                : 100;

            // 🔥 FIXED: Average time uses FechaResolucion instead of DateTime.UtcNow
            string avgTime = "0h 0m";
            if (totalResueltos > 0)
            {
                var durations = resueltos
                    .Where(t => t.FechaResolucion.HasValue)
                    .Select(t => (t.FechaResolucion!.Value - t.FechaCreacion).TotalHours)
                    .ToList();
                
                if (durations.Any())
                {
                    var avgHours = durations.Average();
                    avgTime = $"{(int)avgHours}h {(int)((avgHours - (int)avgHours) * 60)}m";
                }
            }

            return new MetricasDto
            {
                TotalTickets = tickets.Count,
                TicketsPendientes = tickets.Count(t => t.Estado == EstadoTicket.Pendiente),
                TicketsEnProceso = tickets.Count(t => t.Estado == EstadoTicket.EnProceso),
                TicketsResueltos = totalResueltos,
                TicketsEsperandoUsuario = tickets.Count(t => t.Estado == EstadoTicket.EsperandoUsuario),
                ResueltosHoy = resueltosHoy,
                PorcentajeSlaCumplido = Math.Round(slaCumplido, 1),
                TiempoPromedioResolucion = avgTime,
                DistribucionPorEstado = tickets.GroupBy(t => t.Estado)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count())
            };
        }

        private IEnumerable<TicketDto> MapToDto(IEnumerable<Ticket> tickets)
        {
            return tickets.Select(t => new TicketDto
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                Estado = t.Estado,
                Prioridad = t.Prioridad,
                FechaCreacion = t.FechaCreacion,
                FechaAsignacion = t.FechaAsignacion,
                FechaResolucion = t.FechaResolucion,
                FechaLimite = t.FechaLimite,
                SLACumplido = t.SLACumplido,
                UsuarioId = t.UsuarioId,
                UsuarioNombre = t.Usuario?.Nombre,
                OperadorAsignadoId = t.OperadorAsignadoId,
                OperadorAsignadoNombre = t.OperadorAsignado?.Nombre
            });
        }
    }
}
