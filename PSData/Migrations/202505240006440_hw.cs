namespace PSData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hw : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articuloes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        marca = c.String(nullable: false),
                        modelo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Compras",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tienda = c.String(nullable: false),
                        costoTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Serial = c.String(nullable: false, maxLength: 128),
                        ArticuloId = c.Int(nullable: false),
                        SucursalId = c.String(),
                        Estado = c.String(),
                        Costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompraId = c.Int(nullable: false),
                        Departamento = c.String(),
                    })
                .PrimaryKey(t => t.Serial);
            
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sucursals",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Nombre = c.String(nullable: false),
                        Telefono = c.String(),
                        Direccion = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Nombre = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Rol = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Usuarios");
            DropTable("dbo.Sucursals");
            DropTable("dbo.Regions");
            DropTable("dbo.Items");
            DropTable("dbo.Compras");
            DropTable("dbo.Articuloes");
        }
    }
}
