using PSData.Modelos;

namespace PSData.Datos
{
    public static class DbInitializer
    {
        public static void Initialize(PSDatos context)
        {
            // Asegurar que la base de datos está creada
            context.Database.EnsureCreated();

            // Usuarios - crear si no existen, y migrar contraseñas en texto plano a BCrypt
            if (!context.Usuarios.Any())
            {
                var usuarios = new[]
                {
                    new Usuario
                    {
                        Id = Guid.NewGuid().ToString(),
                        Nombre = "admin",
                        Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        Email = "admin@psinventory.com",
                        Rol = "Administrador"
                    },
                    new Usuario
                    {
                        Id = Guid.NewGuid().ToString(),
                        Nombre = "supervisor",
                        Password = BCrypt.Net.BCrypt.HashPassword("supervisor123"),
                        Email = "supervisor@psinventory.com",
                        Rol = "Supervisor"
                    }
                };
                context.Usuarios.AddRange(usuarios);
                context.SaveChanges();
            }
            else
            {
                // Migrar contraseñas en texto plano a BCrypt
                var usuariosPlain = context.Usuarios
                    .Where(u => !u.Eliminado && !u.Password.StartsWith("$2"))
                    .ToList();

                if (usuariosPlain.Any())
                {
                    foreach (var u in usuariosPlain)
                        u.Password = BCrypt.Net.BCrypt.HashPassword(u.Password);
                    context.SaveChanges();
                }
            }

            // Categorías
            if (!context.Categorias.Any())
            {
                var categorias = new[]
                {
                    new Categoria { Nombre = "Computadoras", Descripcion = "Equipos de cómputo" },
                    new Categoria { Nombre = "Periféricos", Descripcion = "Teclados, ratones, monitores" },
                    new Categoria { Nombre = "Networking", Descripcion = "Switches, routers, access points" },
                    new Categoria { Nombre = "Mobiliario", Descripcion = "Sillas, escritorios, archiveros" }
                };
                context.Categorias.AddRange(categorias);
                context.SaveChanges();
            }

            // Regiones
            if (!context.Regiones.Any())
            {
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
}
