using PSData.Modelos;
using System;
using System.Data.Entity;
using System.Linq;

namespace PSData.Datos
{
    public class PSDatos : DbContext
    {
        // El contexto se ha configurado para usar una cadena de conexión 'PSDatos' del archivo 
        // de configuración de la aplicación (App.config o Web.config). De forma predeterminada, 
        // esta cadena de conexión tiene como destino la base de datos 'PSData.Datos.PSDatos' de la instancia LocalDb. 
        // 
        // Si desea tener como destino una base de datos y/o un proveedor de base de datos diferente, 
        // modifique la cadena de conexión 'PSDatos'  en el archivo de configuración de la aplicación.
        public PSDatos()
            : base("name=PSDatos")
        { }
        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Region> Regiones { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        // Agregue un DbSet para cada tipo de entidad que desee incluir en el modelo. Para obtener más información 
        // sobre cómo configurar y usar un modelo Code First, vea http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}