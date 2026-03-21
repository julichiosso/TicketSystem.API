using TicketSystem.Aplicacion.Common;
using TicketSystem.Aplicacion.DTOs;
using TicketSystem.Aplicacion.Interfaces;
using TicketSystem.Dominio.Entidades;
using TicketSystem.Dominio.Enumeraciones;

namespace TicketSystem.Aplicacion.Servicios
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notifications;

        public TicketService(
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            INotificationService notifications)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _notifications = notifications;
        }

        public async Task<Guid> CreateAsync(CreateTicketDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId)
                ?? throw new KeyNotFoundException("User does not exist");

            var createdAt = DateTime.UtcNow;

            DateTime? deadline = dto.Priority switch
            {
                TicketPriority.Critical => createdAt.AddHours(2),
                TicketPriority.High => createdAt.AddHours(4),
                TicketPriority.Medium => createdAt.AddHours(24),
                TicketPriority.Low => createdAt.AddHours(48),
                _ => (DateTime?)null
            };

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Status = TicketStatus.Pending,
                Priority = dto.Priority,
                UserId = dto.UserId,
                CreatedAt = createdAt,
                Deadline = deadline,
                SlaComplied = true
            };

            await _ticketRepository.CreateAsync(ticket);
            await RegisterActionAsync(ticket.Id, dto.UserId, "CREATION", $"Ticket '{ticket.Title}' created with priority {ticket.Priority}.");

            await _notifications.NotifyTicketCreatedAsync(ticket, user);

            return ticket.Id;
        }

        public async Task<PagedResult<TicketDto>> GetByUserAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            var (tickets, total) = await _ticketRepository.GetByUserAsync(userId, page, pageSize);
            return new PagedResult<TicketDto>
            {
                Data = MapToDto(tickets),
                Page = page,
                PageSize = pageSize,
                TotalRecords = total
            };
        }

        public async Task<PagedResult<TicketDto>> GetByOperatorAsync(Guid operatorId, int page = 1, int pageSize = 10)
        {
            var (tickets, total) = await _ticketRepository.GetByOperatorAsync(operatorId, page, pageSize);
            return new PagedResult<TicketDto>
            {
                Data = MapToDto(tickets),
                Page = page,
                PageSize = pageSize,
                TotalRecords = total
            };
        }

        public async Task<PagedResult<TicketDto>> GetFilteredAsync(TicketFilterDto filter)
        {
            var (tickets, total) = await _ticketRepository.GetFilteredAsync(filter);

            return new PagedResult<TicketDto>
            {
                Data = MapToDto(tickets),
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalRecords = total
            };
        }

        public async Task ChangeStatusAsync(Guid ticketId, TicketStatus newStatus, Guid actorId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId)
                ?? throw new KeyNotFoundException("Ticket does not exist");

            if (ticket.Status == TicketStatus.Closed)
                throw new ArgumentException("Cannot change status of a closed ticket.");

            var oldStatus = ticket.Status;
            ticket.Status = newStatus;

            if (newStatus == TicketStatus.Resolved)
            {
                ticket.ResolvedAt = DateTime.UtcNow;
                if (ticket.Deadline.HasValue)
                    ticket.SlaComplied = ticket.ResolvedAt <= ticket.Deadline.Value;
            }

            await _ticketRepository.UpdateAsync(ticket);
            await RegisterActionAsync(ticketId, actorId, "STATUS_CHANGE", $"Status change from {oldStatus} to {newStatus}.");

            var owner = await _userRepository.GetByIdAsync(ticket.UserId);
            if (owner != null && owner.Id != actorId)
                await _notifications.NotifyStatusChangeAsync(ticket, owner, oldStatus, newStatus);
        }

        public async Task AssignOperatorAsync(Guid ticketId, Guid? operatorId, Guid actorId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId)
                ?? throw new KeyNotFoundException("Ticket does not exist");

            string operatorName = "None";
            User? @operator = null;

            if (operatorId.HasValue)
            {
                @operator = await _userRepository.GetByIdAsync(operatorId.Value)
                    ?? throw new KeyNotFoundException("Operator does not exist");

                if (@operator.Role != UserRole.Operator && @operator.Role != UserRole.Administrator)
                    throw new ArgumentException("User is not a valid operator.");

                operatorName = @operator.Name;
            }

            ticket.AssignedOperatorId = operatorId;
            ticket.AssignedAt = operatorId.HasValue ? DateTime.UtcNow : (DateTime?)null;

            await _ticketRepository.UpdateAsync(ticket);
            await RegisterActionAsync(ticketId, actorId, "ASSIGNMENT", $"Operator assigned: {operatorName}.");

            if (@operator != null)
                await _notifications.NotifyOperatorAssignmentAsync(ticket, @operator);
        }

        public async Task DeleteAsync(Guid id, Guid actorId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Ticket not found");

            ticket.IsDeleted = true;
            await _ticketRepository.UpdateAsync(ticket);
            await RegisterActionAsync(id, actorId, "DELETION", $"Ticket '{ticket.Title}' deleted.");
        }

        public async Task<IEnumerable<TicketCommentDto>> GetCommentsAsync(Guid ticketId, bool includeInternal)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId)
                ?? throw new KeyNotFoundException("Ticket does not exist");

            var comments = await _ticketRepository.GetCommentsByTicketAsync(ticketId, includeInternal);

            return comments.Select(c => new TicketCommentDto
            {
                Id = c.Id,
                TicketId = c.TicketId,
                AuthorId = c.AuthorId,
                Author = c.Author?.Name ?? "System",
                Role = (int)(c.Author?.Role ?? UserRole.User),
                Message = c.Message,
                IsInternal = c.IsInternal,
                CreatedAt = c.CreatedAt,
                Attachments = c.Attachments.Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    OriginalName = a.OriginalName,
                    ContentType = a.ContentType,
                    SizeBytes = a.SizeBytes,
                    Url = $"/uploads/{a.StoredName}"
                }).ToList()
            });
        }

        public async Task<TicketCommentDto> AddCommentAsync(Guid ticketId, Guid authorId, CreateTicketCommentDto dto)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId)
                ?? throw new KeyNotFoundException("Ticket does not exist");

            var author = await _userRepository.GetByIdAsync(authorId)
                ?? throw new KeyNotFoundException("Author does not exist");

            var comment = new TicketComment
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                AuthorId = authorId,
                Message = dto.Message,
                IsInternal = dto.IsInternal,
                CreatedAt = DateTime.UtcNow
            };

            await _ticketRepository.AddCommentAsync(comment);
            await RegisterActionAsync(ticketId, authorId, "COMMENT", $"{(dto.IsInternal ? "[Internal] " : "")}Comment added by {author.Name}.");

            if (!dto.IsInternal)
            {
                var owner = await _userRepository.GetByIdAsync(ticket.UserId);
                if (owner != null)
                    await _notifications.NotifyNewCommentAsync(ticket, owner, comment, author);
            }

            return new TicketCommentDto
            {
                Id = comment.Id,
                TicketId = comment.TicketId,
                AuthorId = comment.AuthorId,
                Author = author.Name,
                Role = (int)author.Role,
                Message = comment.Message,
                IsInternal = comment.IsInternal,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync()
        {
            return await _ticketRepository.GetAuditLogsAsync();
        }

        private async Task RegisterActionAsync(Guid? ticketId, Guid? userId, string action, string detail)
        {
            await _ticketRepository.RecordAuditLogAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                UserId = userId,
                Action = action,
                Detail = detail,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task<MetricsDto> GetMetricsAsync()
        {
            var tickets = _ticketRepository.GetQueryable().ToList();

            var resolved = tickets.Where(t => t.Status == TicketStatus.Resolved).ToList();
            var totalResolved = resolved.Count;

            int resolvedToday = resolved.Count(t =>
                t.ResolvedAt.HasValue &&
                t.ResolvedAt.Value.Date == DateTime.UtcNow.Date);

            double slaCompliance = tickets.Count > 0
                ? (double)resolved.Count(t => t.SlaComplied) / tickets.Count * 100
                : 100;

            string avgTime = "0h 0m";
            if (totalResolved > 0)
            {
                var durations = resolved
                    .Where(t => t.ResolvedAt.HasValue)
                    .Select(t => (t.ResolvedAt!.Value - t.CreatedAt).TotalHours)
                    .ToList();

                if (durations.Any())
                {
                    var avgHours = durations.Average();
                    avgTime = $"{(int)avgHours}h {(int)((avgHours - (int)avgHours) * 60)}m";
                }
            }

            return new MetricsDto
            {
                TotalTickets = tickets.Count,
                PendingTickets = tickets.Count(t => t.Status == TicketStatus.Pending),
                InProgressTickets = tickets.Count(t => t.Status == TicketStatus.InProgress),
                ResolvedTickets = totalResolved,
                WaitingForUserTickets = tickets.Count(t => t.Status == TicketStatus.WaitingForUser),
                ResolvedToday = resolvedToday,
                SlaComplianceRate = Math.Round(slaCompliance, 1),
                AverageResolutionTime = avgTime,
                StatusDistribution = tickets
                    .GroupBy(t => t.Status)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count())
            };
        }

        private static IEnumerable<TicketDto> MapToDto(IEnumerable<Ticket> tickets)
        {
            return tickets.Select(t => new TicketDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                CreatedAt = t.CreatedAt,
                AssignedAt = t.AssignedAt,
                ResolvedAt = t.ResolvedAt,
                Deadline = t.Deadline,
                SlaComplied = t.SlaComplied,
                UserId = t.UserId,
                UserName = t.User?.Name,
                AssignedOperatorId = t.AssignedOperatorId,
                AssignedOperatorName = t.AssignedOperator?.Name
            });
        }
    }
}