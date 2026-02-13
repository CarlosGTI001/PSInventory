using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSData.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Artículos
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

            // Categorías
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

            // Compras
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

            // Departamentos
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

            // Regiones
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

            // Sucursales
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

            // Usuarios
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

            // Crear índices para mejorar performance de consultas
            migrationBuilder.CreateIndex(
                name: "IX_Articulos_Eliminado",
                table: "Articulos",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Eliminado",
                table: "Categorias",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_Eliminado",
                table: "Compras",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_Eliminado",
                table: "Departamentos",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Regiones_Eliminado",
                table: "Regiones",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_Eliminado",
                table: "Sucursales",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Eliminado",
                table: "Usuarios",
                column: "Eliminado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar índices
            migrationBuilder.DropIndex(
                name: "IX_Articulos_Eliminado",
                table: "Articulos");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_Eliminado",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Compras_Eliminado",
                table: "Compras");

            migrationBuilder.DropIndex(
                name: "IX_Departamentos_Eliminado",
                table: "Departamentos");

            migrationBuilder.DropIndex(
                name: "IX_Regiones_Eliminado",
                table: "Regiones");

            migrationBuilder.DropIndex(
                name: "IX_Sucursales_Eliminado",
                table: "Sucursales");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Eliminado",
                table: "Usuarios");

            // Artículos
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Articulos");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Articulos");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Articulos");

            // Categorías
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Categorias");

            // Compras
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Compras");

            // Departamentos
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Departamentos");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Departamentos");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Departamentos");

            // Regiones
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Regiones");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Regiones");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Regiones");

            // Sucursales
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Sucursales");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Sucursales");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Sucursales");

            // Usuarios
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacion",
                table: "Usuarios");
        }
    }
}
