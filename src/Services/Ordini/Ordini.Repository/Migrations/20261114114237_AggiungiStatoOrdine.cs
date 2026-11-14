using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordini.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AggiungiStatoOrdine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MotivoAnnullamento",
                table: "Ordini",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stato",
                table: "Ordini",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "InAttesaPagamento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MotivoAnnullamento",
                table: "Ordini");

            migrationBuilder.DropColumn(
                name: "Stato",
                table: "Ordini");
        }
    }
}
