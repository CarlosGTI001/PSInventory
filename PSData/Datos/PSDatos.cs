using Microsoft.EntityFrameworkCore;
using PSData.Modelos;
using System;
using System.Linq;

namespace PSData.Datos
{
    public class PSDatos : DbContext
    {
        // Constructor para ASP.NET Core (inyección de dependencias)
        public PSDatos(DbContextOptions<PSDatos> options)
            : base(options)
        {
        }

        // Constructor sin parámetros para Windows Forms
        public PSDatos()
            : base(GetOptions())
        {
        }

        private static DbContextOptions<PSDatos> GetOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<PSDatos>();
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PSInventory", "psinventory.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            return optionsBuilder.Options;
        }

        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Region> Regiones { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<MovimientoItem> MovimientosItem { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<SolicitudCompra> SolicitudesCompra { get; set; }
        public DbSet<DetalleSolicitudCompra> DetallesSolicitudCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Índices para mejorar performance
            modelBuilder.Entity<Item>()
                .HasIndex(i => i.ArticuloId)
                .HasDatabaseName("IX_Item_ArticuloId");

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.LoteId)
                .HasDatabaseName("IX_Item_LoteId");

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.SucursalId)
                .HasDatabaseName("IX_Item_SucursalId");

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.Estado)
                .HasDatabaseName("IX_Item_Estado");

            modelBuilder.Entity<Articulo>()
                .HasIndex(a => a.CategoriaId)
                .HasDatabaseName("IX_Articulo_CategoriaId");

            modelBuilder.Entity<Sucursal>()
                .HasIndex(s => s.RegionId)
                .HasDatabaseName("IX_Sucursal_RegionId");

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Nombre)
                .HasDatabaseName("IX_Usuario_Nombre");

            modelBuilder.Entity<Categoria>()
                .HasIndex(c => c.Nombre)
                .HasDatabaseName("IX_Categoria_Nombre");

            modelBuilder.Entity<MovimientoItem>()
                .HasIndex(m => m.ItemId)
                .HasDatabaseName("IX_MovimientoItem_ItemId");

            modelBuilder.Entity<MovimientoItem>()
                .HasIndex(m => m.FechaMovimiento)
                .HasDatabaseName("IX_MovimientoItem_FechaMovimiento");

            modelBuilder.Entity<Compra>()
                .HasIndex(c => c.DepartamentoId)
                .HasDatabaseName("IX_Compra_DepartamentoId");

            modelBuilder.Entity<Departamento>()
                .HasIndex(d => d.Nombre)
                .HasDatabaseName("IX_Departamento_Nombre");

            // Configurar relaciones de Sucursal con MovimientoItem
            modelBuilder.Entity<MovimientoItem>()
                .HasOne(m => m.SucursalOrigen)
                .WithMany(s => s.MovimientosOrigen)
                .HasForeignKey(m => m.SucursalOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovimientoItem>()
                .HasOne(m => m.SucursalDestino)
                .WithMany(s => s.MovimientosDestino)
                .HasForeignKey(m => m.SucursalDestinoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevenir error de múltiples rutas de cascada en SQL Server (Articulo -> Lote -> Item)
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Lote)
                .WithMany(l => l.Items)
                .HasForeignKey(i => i.LoteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Articulo)
                .WithMany(a => a.Items)
                .HasForeignKey(i => i.ArticuloId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lote>()
                .HasOne(l => l.Articulo)
                .WithMany(a => a.Lotes)
                .HasForeignKey(l => l.ArticuloId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lote>()
                .HasOne(l => l.Compra)
                .WithMany(c => c.Lotes)
                .HasForeignKey(l => l.CompraId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}