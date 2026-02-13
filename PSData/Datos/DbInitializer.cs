using PSData.Modelos;

namespace PSData.Datos
{
    public static class DbInitializer
    {
        public static void Initialize(PSDatos context)
        {
            // Asegurar que la base de datos está creada
            context.Database.EnsureCreated();

            // Verificar si ya hay usuarios
            if (context.Usuarios.Any())
            {
                return; // La BD ya tiene datos
            }

            // Crear usuario administrador por defecto
            var adminUser = new Usuario
            {
                Id = Guid.NewGuid().ToString(),
                Nombre = "admin",
                Password = "admin123", // TODO: En producción usar hash BCrypt
                Email = "admin@psinventory.com",
                Rol = "Administrador"
            };

            // Crear usuario supervisor de ejemplo
            var supervisorUser = new Usuario
            {
                Id = Guid.NewGuid().ToString(),
                Nombre = "supervisor",
                Password = "supervisor123",
                Email = "supervisor@psinventory.com",
                Rol = "Supervisor"
            };

            context.Usuarios.AddRange(adminUser, supervisorUser);

            // Puedes agregar datos de prueba adicionales aquí
            // Ejemplo: Categorías de ejemplo
            var categorias = new[]
            {
                new Categoria { Nombre = "Computadoras", Descripcion = "Equipos de cómputo" },
                new Categoria { Nombre = "Periféricos", Descripcion = "Teclados, ratones, monitores" },
                new Categoria { Nombre = "Networking", Descripcion = "Switches, routers, access points" },
                new Categoria { Nombre = "Mobiliario", Descripcion = "Sillas, escritorios, archiveros" }
            };

            context.Categorias.AddRange(categorias);

            // Regiones de ejemplo
            var regiones = new[]
            {
                new Region { Nombre = "Norte", Descripcion = "Región Norte del país" },
                new Region { Nombre = "Sur", Descripcion = "Región Sur del país" },
                new Region { Nombre = "Centro", Descripcion = "Región Centro del país" }
            };

            context.Regiones.AddRange(regiones);

            context.SaveChanges();
        }
    }
}
