using Microsoft.EntityFrameworkCore;
using PSInventory.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSInventory.Datos
{
    public class AppDbContext : DbContext
    {
        // Constructor que permite inyectar las opciones
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Aquí defines tus entidades (tablas)
        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Item> Items { get; set; }
        // Agrega más entidades según necesites

        // Configuración adicional del modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ejemplo de configuración manual de una tabla
            modelBuilder.Entity<Articulo>().ToTable("Articulo");
        }
    }
}
