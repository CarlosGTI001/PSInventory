using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSData.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartamentosAndUpdateCompras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartamentoId",
                table: "Compras",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaSolicitud",
                table: "Compras",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioSolicitante",
                table: "Compras",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Responsable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Compra_DepartamentoId",
                table: "Compras",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Departamento_Nombre",
                table: "Departamentos",
                column: "Nombre");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Departamentos_DepartamentoId",
                table: "Compras",
                column: "DepartamentoId",
                principalTable: "Departamentos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Departamentos_DepartamentoId",
                table: "Compras");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropIndex(
                name: "IX_Compra_DepartamentoId",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "DepartamentoId",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "FechaSolicitud",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "UsuarioSolicitante",
                table: "Compras");
        }
    }
}
