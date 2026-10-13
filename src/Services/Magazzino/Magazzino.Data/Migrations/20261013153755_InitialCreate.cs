using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magazzino.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stock",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantitaDisponibile = table.Column<int>(type: "integer", nullable: false),
                    QuantitaRiservata = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    UltimoAggiornamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock", x => x.ProductId);
                    table.CheckConstraint("CK_Stock_QuantitaDisponibile_NonNegativa", "\"QuantitaDisponibile\" >= 0");
                    table.CheckConstraint("CK_Stock_QuantitaRiservata_NonNegativa", "\"QuantitaRiservata\" >= 0");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock");
        }
    }
}
