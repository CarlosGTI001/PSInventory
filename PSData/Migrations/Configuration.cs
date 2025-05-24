namespace PSData.Migrations
{
    using PSData.Modelos;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PSData.Datos.PSDatos>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PSData.Datos.PSDatos context)
        {
            context.Usuarios.AddOrUpdate(u => u.Id,
                new Usuario
                {
                    Id = "u001",
                    Nombre = "Administrador",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Email = "admin@ejemplo.com",
                    Rol = "Admin"
                },
                new Usuario
                {
                    Id = "u002",
                    Nombre = "Usuario Normal",
                    Password = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Email = "usuario@ejemplo.com",
                    Rol = "User"
                }
            );
        }
    }
}
