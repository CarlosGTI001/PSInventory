using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSData.Migrations
{
    /// <inheritdoc />
    public partial class depto1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEliminacion",
                table: "Usuarios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Sucursales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Sucursales",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEliminacion",
                table: "Sucursales",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Regiones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Regiones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEliminacion",
                table: "Regiones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEliminacion",
                table: "Items",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Departamentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Departamentos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEliminacion",
                table: "Departamentos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Compras",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Compras",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEliminacion",
                table: "Compras",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Categorias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Categorias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEliminacion",
                table: "Categorias",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Articulos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Articulos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEliminacion",
                table: "Articulos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Sucursales");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Sucursales");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Sucursales");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Regiones");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Regiones");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Regiones");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Departamentos");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Departamentos");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Departamentos");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Articulos");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Articulos");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Articulos");
        }
    }
}
