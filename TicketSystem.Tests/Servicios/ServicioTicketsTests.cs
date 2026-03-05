using Moq;
using TicketSystem.Aplicacion.DTOs;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Aplicacion.Servicios;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Dominio.Enumeraciones;
using TicketSystem.Aplicacion.Common;
using Xunit;

namespace TicketSystem.Tests.Servicios
{
    public class ServicioTicketsTests
    {
        // ─── Mocks compartidos por todos los tests ───────────────────────────────
        private readonly Mock<IRepositorioTickets>      _repoTickets;
        private readonly Mock<IRepositorioUsuarios>     _repoUsuarios;
        private readonly Mock<IServicioNotificaciones>  _notificaciones;
        private readonly ServicioTickets                _servicio;

        // Datos de prueba reutilizables
        private readonly Guid      _usuarioId  = Guid.NewGuid();
        private readonly Guid      _operadorId = Guid.NewGuid();
        private readonly Guid      _ticketId   = Guid.NewGuid();
        private readonly Usuario   _usuario;
        private readonly Usuario   _operador;
        private readonly Ticket    _ticket;

        public ServicioTicketsTests()
        {
            _repoTickets    = new Mock<IRepositorioTickets>();
            _repoUsuarios   = new Mock<IRepositorioUsuarios>();
            _notificaciones = new Mock<IServicioNotificaciones>();

            _servicio = new ServicioTickets(
                _repoTickets.Object,
                _repoUsuarios.Object,
                _notificaciones.Object);

            _usuario = new Usuario
            {
                Id     = _usuarioId,
                Nombre = "Juan Pérez",
                Email  = "juan@empresa.com",
                Rol    = RolUsuario.Usuario
            };

            _operador = new Usuario
            {
                Id     = _operadorId,
                Nombre = "Ana López",
                Email  = "ana@empresa.com",
                Rol    = RolUsuario.Operador
            };

            _ticket = new Ticket
            {
                Id            = _ticketId,
                Titulo        = "No puedo iniciar sesión",
                Descripcion   = "Me dice contraseña incorrecta",
                Estado        = EstadoTicket.Pendiente,
                Prioridad     = PrioridadTicket.Alta,
                UsuarioId     = _usuarioId,
                Usuario       = _usuario,
                FechaCreacion = DateTime.UtcNow,
                FechaLimite   = DateTime.UtcNow.AddHours(4),
                SLACumplido   = true
            };
        }

        // ═════════════════════════════════════════════════════════════════════════
        // CREAR TICKET
        // ═════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task CrearAsync_UsuarioValido_RetornaGuid()
        {
            // Arrange
            var dto = new CrearTicketDto
            {
                Titulo      = "Problema con impresora",
                Descripcion = "No imprime en color",
                Prioridad   = PrioridadTicket.Media,
                UsuarioId   = _usuarioId
            };
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_usuarioId)).ReturnsAsync(_usuario);
            _repoTickets.Setup(r => r.CrearAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _servicio.CrearAsync(dto);

            // Assert
            Assert.NotEqual(Guid.Empty, resultado);
        }

        [Fact]
        public async Task CrearAsync_UsuarioValido_LlamaNotificacion()
        {
            // Arrange
            var dto = new CrearTicketDto
            {
                Titulo      = "Ticket de prueba",
                Descripcion = "Descripción",
                Prioridad   = PrioridadTicket.Baja,
                UsuarioId   = _usuarioId
            };
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_usuarioId)).ReturnsAsync(_usuario);
            _repoTickets.Setup(r => r.CrearAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);

            // Act
            await _servicio.CrearAsync(dto);

            // Assert: debe haber enviado la notificación al usuario
            _notificaciones.Verify(
                n => n.NotificarTicketCreadoAsync(It.IsAny<Ticket>(), _usuario),
                Times.Once);
        }

        [Fact]
        public async Task CrearAsync_UsuarioInexistente_LanzaExcepcion()
        {
            // Arrange
            var dto = new CrearTicketDto { UsuarioId = Guid.NewGuid() };
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _servicio.CrearAsync(dto));
        }

        [Theory]
        [InlineData(PrioridadTicket.Critica, 2)]
        [InlineData(PrioridadTicket.Alta,    4)]
        [InlineData(PrioridadTicket.Media,   24)]
        [InlineData(PrioridadTicket.Baja,    48)]
        public async Task CrearAsync_AsignaFechaLimiteSegunPrioridad(PrioridadTicket prioridad, int horasEsperadas)
        {
            // Arrange
            var dto = new CrearTicketDto
            {
                Titulo      = "Test SLA",
                Descripcion = "Test",
                Prioridad   = prioridad,
                UsuarioId   = _usuarioId
            };
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_usuarioId)).ReturnsAsync(_usuario);

            Ticket? ticketCapturado = null;
            _repoTickets
                .Setup(r => r.CrearAsync(It.IsAny<Ticket>()))
                .Callback<Ticket>(t => ticketCapturado = t)
                .Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);

            // Act
            await _servicio.CrearAsync(dto);

            // Assert
            Assert.NotNull(ticketCapturado!.FechaLimite);
            var horasReales = (ticketCapturado.FechaLimite!.Value - ticketCapturado.FechaCreacion).TotalHours;
            Assert.Equal(horasEsperadas, horasReales, precision: 0);
        }

        // ═════════════════════════════════════════════════════════════════════════
        // CAMBIAR ESTADO
        // ═════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task CambiarEstadoAsync_TicketValido_ActualizaEstado()
        {
            // Arrange
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);
            _repoTickets.Setup(r => r.ActualizarAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_usuarioId)).ReturnsAsync(_usuario);

            // Act
            await _servicio.CambiarEstadoAsync(_ticketId, EstadoTicket.EnProceso, _operadorId);

            // Assert
            Assert.Equal(EstadoTicket.EnProceso, _ticket.Estado);
        }

        [Fact]
        public async Task CambiarEstadoAsync_TicketCerrado_LanzaExcepcion()
        {
            // Arrange
            _ticket.Estado = EstadoTicket.Cerrado;
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _servicio.CambiarEstadoAsync(_ticketId, EstadoTicket.EnProceso, _operadorId));
        }

        [Fact]
        public async Task CambiarEstadoAsync_TicketInexistente_LanzaExcepcion()
        {
            // Arrange
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Ticket?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _servicio.CambiarEstadoAsync(Guid.NewGuid(), EstadoTicket.EnProceso, _operadorId));
        }

        [Fact]
        public async Task CambiarEstadoAsync_AResuelto_AsignaFechaResolucion()
        {
            // Arrange
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);
            _repoTickets.Setup(r => r.ActualizarAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_usuarioId)).ReturnsAsync(_usuario);

            // Act
            await _servicio.CambiarEstadoAsync(_ticketId, EstadoTicket.Resuelto, _operadorId);

            // Assert
            Assert.NotNull(_ticket.FechaResolucion);
        }

        [Fact]
        public async Task CambiarEstadoAsync_ActorEsDuenio_NoEnviaNotificacion()
        {
            // El actor es el mismo usuario dueño del ticket → no notificar
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);
            _repoTickets.Setup(r => r.ActualizarAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_usuarioId)).ReturnsAsync(_usuario);

            // Act: el actor ES el dueño del ticket
            await _servicio.CambiarEstadoAsync(_ticketId, EstadoTicket.EnProceso, _usuarioId);

            // Assert: no debe notificar
            _notificaciones.Verify(
                n => n.NotificarCambioEstadoAsync(It.IsAny<Ticket>(), It.IsAny<Usuario>(), It.IsAny<EstadoTicket>(), It.IsAny<EstadoTicket>()),
                Times.Never);
        }

        [Fact]
        public async Task CambiarEstadoAsync_ActorEsOperador_NotificaAlDuenio()
        {
            // Arrange
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);
            _repoTickets.Setup(r => r.ActualizarAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_usuarioId)).ReturnsAsync(_usuario);

            // Act: el actor es el operador, distinto al dueño
            await _servicio.CambiarEstadoAsync(_ticketId, EstadoTicket.EnProceso, _operadorId);

            // Assert
            _notificaciones.Verify(
                n => n.NotificarCambioEstadoAsync(_ticket, _usuario, EstadoTicket.Pendiente, EstadoTicket.EnProceso),
                Times.Once);
        }

        // ═════════════════════════════════════════════════════════════════════════
        // ASIGNAR OPERADOR
        // ═════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task AsignarOperadorAsync_OperadorValido_AsignaYNotifica()
        {
            // Arrange
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);
            _repoTickets.Setup(r => r.ActualizarAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_operadorId)).ReturnsAsync(_operador);

            // Act
            await _servicio.AsignarOperadorAsync(_ticketId, _operadorId, Guid.NewGuid());

            // Assert: el ticket tiene el operador asignado
            Assert.Equal(_operadorId, _ticket.OperadorAsignadoId);

            // Assert: se notificó al operador
            _notificaciones.Verify(
                n => n.NotificarAsignacionOperadorAsync(_ticket, _operador),
                Times.Once);
        }

        [Fact]
        public async Task AsignarOperadorAsync_UsuarioNoEsOperador_LanzaExcepcion()
        {
            // Arrange: intentar asignar un usuario con rol Usuario (no operador)
            var usuarioComun = new Usuario { Id = Guid.NewGuid(), Rol = RolUsuario.Usuario };
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(usuarioComun.Id)).ReturnsAsync(usuarioComun);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _servicio.AsignarOperadorAsync(_ticketId, usuarioComun.Id, Guid.NewGuid()));
        }

        [Fact]
        public async Task AsignarOperadorAsync_OperadorNull_DesasignaOperador()
        {
            // Arrange
            _ticket.OperadorAsignadoId = _operadorId;
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);
            _repoTickets.Setup(r => r.ActualizarAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);

            // Act: asignar null = desasignar
            await _servicio.AsignarOperadorAsync(_ticketId, null, Guid.NewGuid());

            // Assert
            Assert.Null(_ticket.OperadorAsignadoId);
            Assert.Null(_ticket.FechaAsignacion);
        }

        // ═════════════════════════════════════════════════════════════════════════
        // ELIMINAR TICKET
        // ═════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task EliminarAsync_TicketExistente_MarcaComoEliminado()
        {
            // Arrange
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);
            _repoTickets.Setup(r => r.ActualizarAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);

            // Act
            await _servicio.EliminarAsync(_ticketId, _usuarioId);

            // Assert: soft delete
            Assert.True(_ticket.IsDeleted);
        }

        [Fact]
        public async Task EliminarAsync_TicketInexistente_LanzaExcepcion()
        {
            // Arrange
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Ticket?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _servicio.EliminarAsync(Guid.NewGuid(), _usuarioId));
        }

        // ═════════════════════════════════════════════════════════════════════════
        // COMENTARIOS
        // ═════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task AgregarComentarioAsync_ComentarioPublico_NotificaAlDuenio()
        {
            // Arrange
            var dto = new CrearComentarioTicketDto { Mensaje = "Estamos revisando tu caso.", Interno = false };
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_operadorId)).ReturnsAsync(_operador);
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_usuarioId)).ReturnsAsync(_usuario);
            _repoTickets.Setup(r => r.AgregarComentarioAsync(It.IsAny<ComentarioTicket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);

            // Act
            await _servicio.AgregarComentarioAsync(_ticketId, _operadorId, dto);

            // Assert: el dueño del ticket recibe notificación
            _notificaciones.Verify(
                n => n.NotificarNuevoComentarioAsync(_ticket, _usuario, It.IsAny<ComentarioTicket>(), _operador),
                Times.Once);
        }

        [Fact]
        public async Task AgregarComentarioAsync_ComentarioInterno_NoNotifica()
        {
            // Los comentarios internos son solo para el equipo, no van al usuario
            var dto = new CrearComentarioTicketDto { Mensaje = "Nota interna del equipo.", Interno = true };
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_operadorId)).ReturnsAsync(_operador);
            _repoTickets.Setup(r => r.AgregarComentarioAsync(It.IsAny<ComentarioTicket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);

            // Act
            await _servicio.AgregarComentarioAsync(_ticketId, _operadorId, dto);

            // Assert: no se notifica al usuario
            _notificaciones.Verify(
                n => n.NotificarNuevoComentarioAsync(It.IsAny<Ticket>(), It.IsAny<Usuario>(), It.IsAny<ComentarioTicket>(), It.IsAny<Usuario>()),
                Times.Never);
        }

        [Fact]
        public async Task AgregarComentarioAsync_TicketInexistente_LanzaExcepcion()
        {
            // Arrange
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Ticket?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _servicio.AgregarComentarioAsync(Guid.NewGuid(), _operadorId, new CrearComentarioTicketDto()));
        }

        // ═════════════════════════════════════════════════════════════════════════
        // SLA
        // ═════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task CambiarEstadoAsync_ResueltoDentroDelSLA_SLACumplidoTrue()
        {
            // Arrange: fecha límite en el futuro → SLA cumplido
            _ticket.FechaLimite = DateTime.UtcNow.AddHours(2);
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);
            _repoTickets.Setup(r => r.ActualizarAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_usuarioId)).ReturnsAsync(_usuario);

            // Act
            await _servicio.CambiarEstadoAsync(_ticketId, EstadoTicket.Resuelto, _operadorId);

            // Assert
            Assert.True(_ticket.SLACumplido);
        }

        [Fact]
        public async Task CambiarEstadoAsync_ResueltoPasadoElSLA_SLACumplidoFalse()
        {
            // Arrange: fecha límite ya pasó → SLA no cumplido
            _ticket.FechaLimite = DateTime.UtcNow.AddHours(-1);
            _repoTickets.Setup(r => r.ObtenerPorIdAsync(_ticketId)).ReturnsAsync(_ticket);
            _repoTickets.Setup(r => r.ActualizarAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _repoTickets.Setup(r => r.RegistrarAuditLogAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
            _repoUsuarios.Setup(r => r.ObtenerPorIdAsync(_usuarioId)).ReturnsAsync(_usuario);

            // Act
            await _servicio.CambiarEstadoAsync(_ticketId, EstadoTicket.Resuelto, _operadorId);

            // Assert
            Assert.False(_ticket.SLACumplido);
        }
    }
}