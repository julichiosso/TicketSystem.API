using Microsoft.EntityFrameworkCore;
using TicketSystem.Dominio.Entidades;

namespace TicketSystem.Infraestructura.Datos;

public class TicketSystemDbContext : DbContext
{
    public TicketSystemDbContext(DbContextOptions<TicketSystemDbContext> options)
        : base(options) { }

    public DbSet<Ticket>           Tickets           { get; set; }
    public DbSet<Usuario>          Usuarios          { get; set; }
    public DbSet<ComentarioTicket> ComentariosTicket { get; set; }
    public DbSet<AuditLog>         AuditLogs         { get; set; }
    public DbSet<ArchivoAdjunto>   ArchivosAdjuntos  { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasOne(t => t.OperadorAsignado)
                .WithMany()
                .HasForeignKey(t => t.OperadorAsignadoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ComentarioTicket>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Mensaje).IsRequired().HasMaxLength(2000);

            entity.HasOne(c => c.Ticket)
                .WithMany(t => t.Comentarios)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Autor)
                .WithMany()
                .HasForeignKey(c => c.AutorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(c => c.Adjuntos)
                .WithOne(a => a.Comentario)
                .HasForeignKey(a => a.ComentarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ArchivoAdjunto>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.NombreOriginal).IsRequired().HasMaxLength(255);
            entity.Property(a => a.NombreAlmacenado).IsRequired().HasMaxLength(255);
            entity.Property(a => a.ContentType).IsRequired().HasMaxLength(100);
        });
    }
}