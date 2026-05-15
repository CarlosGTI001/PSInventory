using Microsoft.EntityFrameworkCore;
using PSData.Modelos;
using System;
using System.Linq;
using System.Runtime.InteropServices;

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

            string dbPath;

            if (OperatingSystem.IsWindows())
            {
                dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "PSInventory",
                    "psinventory.db"
                );
            }
            else
            {
                dbPath = "/var/psinventory/psinventory.db";
            }

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
        public DbSet<InfraSistemaOperativo> InfraSistemasOperativos { get; set; }
        public DbSet<InfraTipoProcesador> InfraTiposProcesador { get; set; }
        public DbSet<InfraTipoRam> InfraTiposRam { get; set; }
        public DbSet<InfraEquipoComputo> InfraEquiposComputo { get; set; }
        public DbSet<InfraEquipoDepartamento> InfraEquiposDepartamentos { get; set; }
        public DbSet<InfraTipoServicio> InfraTiposServicio { get; set; }
        public DbSet<InfraOperadorServicio> InfraOperadoresServicio { get; set; }
        public DbSet<InfraServicioSucursal> InfraServiciosSucursal { get; set; }
        public DbSet<InfraTipoAccesorio> InfraTiposAccesorio { get; set; }
        public DbSet<InfraSucursalAccesorio> InfraSucursalesAccesorio { get; set; }

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
            
            modelBuilder.Entity<InfraSistemaOperativo>()
                .HasIndex(s => s.Nombre)
                .HasDatabaseName("IX_InfraSistemaOperativo_Nombre");

            modelBuilder.Entity<InfraTipoProcesador>()
                .HasIndex(p => p.Nombre)
                .HasDatabaseName("IX_InfraTipoProcesador_Nombre");

            modelBuilder.Entity<InfraTipoRam>()
                .HasIndex(r => r.Nombre)
                .HasDatabaseName("IX_InfraTipoRam_Nombre");

            modelBuilder.Entity<InfraTipoServicio>()
                .HasIndex(s => s.Nombre)
                .HasDatabaseName("IX_InfraTipoServicio_Nombre");

            modelBuilder.Entity<InfraOperadorServicio>()
                .HasIndex(o => o.Nombre)
                .HasDatabaseName("IX_InfraOperadorServicio_Nombre");

            modelBuilder.Entity<InfraTipoAccesorio>()
                .HasIndex(a => a.Nombre)
                .HasDatabaseName("IX_InfraTipoAccesorio_Nombre");

            modelBuilder.Entity<InfraEquipoComputo>()
                .HasIndex(e => e.Serial)
                .HasDatabaseName("IX_InfraEquipoComputo_Serial");

            modelBuilder.Entity<InfraEquipoComputo>()
                .HasIndex(e => e.SucursalId)
                .HasDatabaseName("IX_InfraEquipoComputo_SucursalId");

            modelBuilder.Entity<InfraServicioSucursal>()
                .HasIndex(s => s.SucursalId)
                .HasDatabaseName("IX_InfraServicioSucursal_SucursalId");

            modelBuilder.Entity<InfraSucursalAccesorio>()
                .HasIndex(a => a.SucursalId)
                .HasDatabaseName("IX_InfraSucursalAccesorio_SucursalId");

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

            modelBuilder.Entity<InfraEquipoDepartamento>()
                .HasKey(ed => new { ed.InfraEquipoComputoId, ed.DepartamentoId });

            modelBuilder.Entity<InfraEquipoDepartamento>()
                .HasOne(ed => ed.InfraEquipoComputo)
                .WithMany(e => e.EquiposDepartamentos)
                .HasForeignKey(ed => ed.InfraEquipoComputoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InfraEquipoDepartamento>()
                .HasOne(ed => ed.Departamento)
                .WithMany(d => d.InfraEquiposDepartamentos)
                .HasForeignKey(ed => ed.DepartamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InfraEquipoComputo>()
                .HasOne(e => e.Sucursal)
                .WithMany(s => s.InfraEquiposComputo)
                .HasForeignKey(e => e.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InfraEquipoComputo>()
                .HasOne(e => e.SistemaOperativo)
                .WithMany(s => s.EquiposComputo)
                .HasForeignKey(e => e.SistemaOperativoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InfraEquipoComputo>()
                .HasOne(e => e.TipoProcesador)
                .WithMany(p => p.EquiposComputo)
                .HasForeignKey(e => e.TipoProcesadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InfraEquipoComputo>()
                .HasOne(e => e.TipoRam)
                .WithMany(r => r.EquiposComputo)
                .HasForeignKey(e => e.TipoRamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InfraServicioSucursal>()
                .HasOne(s => s.Sucursal)
                .WithMany(su => su.InfraServiciosSucursal)
                .HasForeignKey(s => s.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InfraServicioSucursal>()
                .HasOne(s => s.TipoServicio)
                .WithMany(t => t.ServiciosSucursal)
                .HasForeignKey(s => s.TipoServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InfraServicioSucursal>()
                .HasOne(s => s.OperadorServicio)
                .WithMany(o => o.ServiciosSucursal)
                .HasForeignKey(s => s.OperadorServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InfraSucursalAccesorio>()
                .HasOne(a => a.Sucursal)
                .WithMany(s => s.InfraSucursalesAccesorio)
                .HasForeignKey(a => a.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InfraSucursalAccesorio>()
                .HasOne(a => a.TipoAccesorio)
                .WithMany(t => t.SucursalesAccesorio)
                .HasForeignKey(a => a.TipoAccesorioId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}