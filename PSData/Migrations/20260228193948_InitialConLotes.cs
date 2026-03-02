using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSData.Migrations
{
    /// <inheritdoc />
    public partial class InitialConLotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiereNumeroSerie = table.Column<bool>(type: "bit", nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Responsable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regiones",
                columns: table => new
                {
                    RegionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regiones", x => x.RegionId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Articulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StockMinimo = table.Column<int>(type: "int", nullable: false),
                    Especificaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequiereSerial = table.Column<bool>(type: "bit", nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articulos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articulos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Proveedor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CostoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumeroFactura = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RutaFactura = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DepartamentoId = table.Column<int>(type: "int", nullable: true),
                    UsuarioSolicitante = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compras_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sucursales",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sucursales_Regiones_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regiones",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticuloId = table.Column<int>(type: "int", nullable: false),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lotes_Articulos_ArticuloId",
                        column: x => x.ArticuloId,
                        principalTable: "Articulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lotes_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Serial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    ArticuloId = table.Column<int>(type: "int", nullable: false),
                    SucursalId = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoteId = table.Column<int>(type: "int", nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResponsableEmpleado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaUltimaTransferencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaGarantiaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MesesGarantia = table.Column<int>(type: "int", nullable: true),
                    FechaGarantiaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Articulos_ArticuloId",
                        column: x => x.ArticuloId,
                        principalTable: "Articulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Lotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "Lotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MovimientosItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    SucursalOrigenId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SucursalDestinoId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioResponsable = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResponsableRecepcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaRecepcion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosItem_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovimientosItem_Sucursales_SucursalDestinoId",
                        column: x => x.SucursalDestinoId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosItem_Sucursales_SucursalOrigenId",
                        column: x => x.SucursalOrigenId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articulo_CategoriaId",
                table: "Articulos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_Nombre",
                table: "Categorias",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_Compra_DepartamentoId",
                table: "Compras",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Departamento_Nombre",
                table: "Departamentos",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_Item_ArticuloId",
                table: "Items",
                column: "ArticuloId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_Estado",
                table: "Items",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Item_LoteId",
                table: "Items",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_SucursalId",
                table: "Items",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_ArticuloId",
                table: "Lotes",
                column: "ArticuloId");

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_CompraId",
                table: "Lotes",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoItem_FechaMovimiento",
                table: "MovimientosItem",
                column: "FechaMovimiento");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoItem_ItemId",
                table: "MovimientosItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosItem_SucursalDestinoId",
                table: "MovimientosItem",
                column: "SucursalDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosItem_SucursalOrigenId",
                table: "MovimientosItem",
                column: "SucursalOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_Sucursal_RegionId",
                table: "Sucursales",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Nombre",
                table: "Usuarios",
                column: "Nombre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosItem");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Lotes");

            migrationBuilder.DropTable(
                name: "Sucursales");

            migrationBuilder.DropTable(
                name: "Articulos");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Regiones");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Departamentos");
        }
    }
}
