using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSData.Migrations
{
    /// <inheritdoc />
    public partial class columna : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RutaFactura",
                table: "Compras",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RutaFactura",
                table: "Compras");
        }
    }
}
