using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utenti.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "utenti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cognome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Ruolo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DataRegistrazione = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utenti", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_utenti_Email",
                table: "utenti",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "utenti");
        }
    }
}
