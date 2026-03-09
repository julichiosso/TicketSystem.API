using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Infraestructura.Migrations
{
    public partial class FixAuditLogsDeleteCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""AuditLogs"" DROP CONSTRAINT IF EXISTS ""FK_AuditLogs_Usuarios_UsuarioId"";
                ALTER TABLE ""AuditLogs"" ADD CONSTRAINT ""FK_AuditLogs_Usuarios_UsuarioId"" 
                    FOREIGN KEY (""UsuarioId"") REFERENCES ""Usuarios""(""Id"") ON DELETE SET NULL;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""AuditLogs"" DROP CONSTRAINT IF EXISTS ""FK_AuditLogs_Usuarios_UsuarioId"";
                ALTER TABLE ""AuditLogs"" ADD CONSTRAINT ""FK_AuditLogs_Usuarios_UsuarioId"" 
                    FOREIGN KEY (""UsuarioId"") REFERENCES ""Usuarios""(""Id"");
            ");
        }
    }
}