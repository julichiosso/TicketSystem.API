using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Infraestructura.Migrations
{
    public partial class FixRefreshTokenColumnType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"Usuarios\" ALTER COLUMN \"RefreshTokenExpires\" TYPE timestamp with time zone USING \"RefreshTokenExpires\"::timestamp with time zone;"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"Usuarios\" ALTER COLUMN \"RefreshTokenExpires\" TYPE text USING \"RefreshTokenExpires\"::text;"
            );
        }
    }
}