using Microsoft.EntityFrameworkCore;
using PSData.Modelos;

namespace PSData.Datos
{
    public static class DbInitializer
    {
        public static void Initialize(PSDatos context)
        {
            // Asegurar que la base de datos está creada
            context.Database.EnsureCreated();
            EnsureInfraestructuraSchema(context);

            // Usuarios - crear si no existen, y migrar contraseñas en texto plano a BCrypt
            if (!context.Usuarios.Any())
            {
                var usuarios = new[]
                {
                    new Usuario
                    {
                        Id = "admin",
                        Nombre = "admin",
                        Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        Email = "admin@psinventory.com",
                        Rol = "Administrador"
                    },
                    new Usuario
                    {
                        Id = "supervisor",
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

            SeedInfraestructuraCatalogos(context);
        }

        private static void SeedInfraestructuraCatalogos(PSDatos context)
        {
            if (!context.InfraSistemasOperativos.Any(so => !so.Eliminado))
            {
                context.InfraSistemasOperativos.AddRange(
                    new InfraSistemaOperativo { Nombre = "Windows 10 Pro", Activo = true },
                    new InfraSistemaOperativo { Nombre = "Windows 11 Pro", Activo = true },
                    new InfraSistemaOperativo { Nombre = "Ubuntu 22.04 LTS", Activo = true }
                );
            }

            if (!context.InfraTiposProcesador.Any(p => !p.Eliminado))
            {
                context.InfraTiposProcesador.AddRange(
                    new InfraTipoProcesador { Nombre = "Intel Core i5", Activo = true },
                    new InfraTipoProcesador { Nombre = "Intel Core i7", Activo = true },
                    new InfraTipoProcesador { Nombre = "AMD Ryzen 5", Activo = true }
                );
            }

            if (!context.InfraTiposRam.Any(r => !r.Eliminado))
            {
                context.InfraTiposRam.AddRange(
                    new InfraTipoRam { Nombre = "DDR3", Activo = true },
                    new InfraTipoRam { Nombre = "DDR4", Activo = true },
                    new InfraTipoRam { Nombre = "DDR5", Activo = true }
                );
            }

            if (!context.InfraTiposServicio.Any(s => !s.Eliminado))
            {
                context.InfraTiposServicio.AddRange(
                    new InfraTipoServicio { Nombre = "Internet", Activo = true },
                    new InfraTipoServicio { Nombre = "Telefonía", Activo = true },
                    new InfraTipoServicio { Nombre = "Internet + Telefonía", Activo = true }
                );
            }

            if (!context.InfraOperadoresServicio.Any(o => !o.Eliminado))
            {
                context.InfraOperadoresServicio.AddRange(
                    new InfraOperadorServicio { Nombre = "Claro", Activo = true },
                    new InfraOperadorServicio { Nombre = "Altice", Activo = true },
                    new InfraOperadorServicio { Nombre = "Wind", Activo = true }
                );
            }

            if (!context.InfraTiposAccesorio.Any(a => !a.Eliminado))
            {
                context.InfraTiposAccesorio.AddRange(
                    new InfraTipoAccesorio { Nombre = "UPS", Activo = true },
                    new InfraTipoAccesorio { Nombre = "DVR", Activo = true },
                    new InfraTipoAccesorio { Nombre = "Cámara", Activo = true }
                );
            }

            context.SaveChanges();
        }

        private static void EnsureInfraestructuraSchema(PSDatos context)
        {
            context.Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "InfraSistemasOperativos" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_InfraSistemasOperativos" PRIMARY KEY AUTOINCREMENT,
                    "Nombre" TEXT NOT NULL,
                    "Activo" INTEGER NOT NULL,
                    "Eliminado" INTEGER NOT NULL,
                    "FechaEliminacion" TEXT NULL,
                    "UsuarioEliminacion" TEXT NULL
                );
            """);

            context.Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "InfraTiposProcesador" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_InfraTiposProcesador" PRIMARY KEY AUTOINCREMENT,
                    "Nombre" TEXT NOT NULL,
                    "Activo" INTEGER NOT NULL,
                    "Eliminado" INTEGER NOT NULL,
                    "FechaEliminacion" TEXT NULL,
                    "UsuarioEliminacion" TEXT NULL
                );
            """);

            context.Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "InfraTiposRam" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_InfraTiposRam" PRIMARY KEY AUTOINCREMENT,
                    "Nombre" TEXT NOT NULL,
                    "Activo" INTEGER NOT NULL,
                    "Eliminado" INTEGER NOT NULL,
                    "FechaEliminacion" TEXT NULL,
                    "UsuarioEliminacion" TEXT NULL
                );
            """);

            context.Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "InfraTiposServicio" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_InfraTiposServicio" PRIMARY KEY AUTOINCREMENT,
                    "Nombre" TEXT NOT NULL,
                    "Activo" INTEGER NOT NULL,
                    "Eliminado" INTEGER NOT NULL,
                    "FechaEliminacion" TEXT NULL,
                    "UsuarioEliminacion" TEXT NULL
                );
            """);

            context.Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "InfraOperadoresServicio" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_InfraOperadoresServicio" PRIMARY KEY AUTOINCREMENT,
                    "Nombre" TEXT NOT NULL,
                    "Activo" INTEGER NOT NULL,
                    "Eliminado" INTEGER NOT NULL,
                    "FechaEliminacion" TEXT NULL,
                    "UsuarioEliminacion" TEXT NULL
                );
            """);

            context.Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "InfraTiposAccesorio" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_InfraTiposAccesorio" PRIMARY KEY AUTOINCREMENT,
                    "Nombre" TEXT NOT NULL,
                    "Activo" INTEGER NOT NULL,
                    "Eliminado" INTEGER NOT NULL,
                    "FechaEliminacion" TEXT NULL,
                    "UsuarioEliminacion" TEXT NULL
                );
            """);

            context.Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "InfraEquiposComputo" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_InfraEquiposComputo" PRIMARY KEY AUTOINCREMENT,
                    "CodigoActivo" TEXT NULL,
                    "SucursalId" TEXT NOT NULL,
                    "NombreEquipo" TEXT NOT NULL,
                    "Marca" TEXT NULL,
                    "Modelo" TEXT NULL,
                    "Serial" TEXT NOT NULL,
                    "SistemaOperativoId" INTEGER NULL,
                    "TipoProcesadorId" INTEGER NULL,
                    "CpuDetalle" TEXT NULL,
                    "TipoRamId" INTEGER NULL,
                    "RamCantidadGb" INTEGER NULL,
                    "Almacenamiento" TEXT NULL,
                    "DireccionIp" TEXT NULL,
                    "Activo" INTEGER NOT NULL,
                    "Observaciones" TEXT NULL,
                    "Eliminado" INTEGER NOT NULL,
                    "FechaEliminacion" TEXT NULL,
                    "UsuarioEliminacion" TEXT NULL,
                    CONSTRAINT "FK_InfraEquiposComputo_Sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES "Sucursales" ("Id") ON DELETE RESTRICT,
                    CONSTRAINT "FK_InfraEquiposComputo_InfraSistemasOperativos_SistemaOperativoId" FOREIGN KEY ("SistemaOperativoId") REFERENCES "InfraSistemasOperativos" ("Id") ON DELETE RESTRICT,
                    CONSTRAINT "FK_InfraEquiposComputo_InfraTiposProcesador_TipoProcesadorId" FOREIGN KEY ("TipoProcesadorId") REFERENCES "InfraTiposProcesador" ("Id") ON DELETE RESTRICT,
                    CONSTRAINT "FK_InfraEquiposComputo_InfraTiposRam_TipoRamId" FOREIGN KEY ("TipoRamId") REFERENCES "InfraTiposRam" ("Id") ON DELETE RESTRICT
                );
            """);

            context.Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "InfraEquiposDepartamentos" (
                    "InfraEquipoComputoId" INTEGER NOT NULL,
                    "DepartamentoId" INTEGER NOT NULL,
                    CONSTRAINT "PK_InfraEquiposDepartamentos" PRIMARY KEY ("InfraEquipoComputoId", "DepartamentoId"),
                    CONSTRAINT "FK_InfraEquiposDepartamentos_InfraEquiposComputo_InfraEquipoComputoId" FOREIGN KEY ("InfraEquipoComputoId") REFERENCES "InfraEquiposComputo" ("Id") ON DELETE CASCADE,
                    CONSTRAINT "FK_InfraEquiposDepartamentos_Departamentos_DepartamentoId" FOREIGN KEY ("DepartamentoId") REFERENCES "Departamentos" ("Id") ON DELETE RESTRICT
                );
            """);

            context.Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "InfraServiciosSucursal" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_InfraServiciosSucursal" PRIMARY KEY AUTOINCREMENT,
                    "SucursalId" TEXT NOT NULL,
                    "TipoServicioId" INTEGER NOT NULL,
                    "OperadorServicioId" INTEGER NOT NULL,
                    "NumeroServicio" TEXT NULL,
                    "VelocidadBajadaMbps" REAL NULL,
                    "VelocidadSubidaMbps" REAL NULL,
                    "Activo" INTEGER NOT NULL,
                    "Observaciones" TEXT NULL,
                    "Eliminado" INTEGER NOT NULL,
                    "FechaEliminacion" TEXT NULL,
                    "UsuarioEliminacion" TEXT NULL,
                    CONSTRAINT "FK_InfraServiciosSucursal_Sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES "Sucursales" ("Id") ON DELETE RESTRICT,
                    CONSTRAINT "FK_InfraServiciosSucursal_InfraTiposServicio_TipoServicioId" FOREIGN KEY ("TipoServicioId") REFERENCES "InfraTiposServicio" ("Id") ON DELETE RESTRICT,
                    CONSTRAINT "FK_InfraServiciosSucursal_InfraOperadoresServicio_OperadorServicioId" FOREIGN KEY ("OperadorServicioId") REFERENCES "InfraOperadoresServicio" ("Id") ON DELETE RESTRICT
                );
            """);

            context.Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "InfraSucursalesAccesorio" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_InfraSucursalesAccesorio" PRIMARY KEY AUTOINCREMENT,
                    "SucursalId" TEXT NOT NULL,
                    "TipoAccesorioId" INTEGER NOT NULL,
                    "Cantidad" INTEGER NOT NULL,
                    "Especificaciones" TEXT NULL,
                    "Activo" INTEGER NOT NULL,
                    "Eliminado" INTEGER NOT NULL,
                    "FechaEliminacion" TEXT NULL,
                    "UsuarioEliminacion" TEXT NULL,
                    CONSTRAINT "FK_InfraSucursalesAccesorio_Sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES "Sucursales" ("Id") ON DELETE RESTRICT,
                    CONSTRAINT "FK_InfraSucursalesAccesorio_InfraTiposAccesorio_TipoAccesorioId" FOREIGN KEY ("TipoAccesorioId") REFERENCES "InfraTiposAccesorio" ("Id") ON DELETE RESTRICT
                );
            """);

            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraSistemaOperativo_Nombre ON InfraSistemasOperativos (Nombre);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraTipoProcesador_Nombre ON InfraTiposProcesador (Nombre);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraTipoRam_Nombre ON InfraTiposRam (Nombre);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraTipoServicio_Nombre ON InfraTiposServicio (Nombre);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraOperadorServicio_Nombre ON InfraOperadoresServicio (Nombre);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraTipoAccesorio_Nombre ON InfraTiposAccesorio (Nombre);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraEquipoComputo_Serial ON InfraEquiposComputo (Serial);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraEquipoComputo_SucursalId ON InfraEquiposComputo (SucursalId);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraEquiposDepartamentos_DepartamentoId ON InfraEquiposDepartamentos (DepartamentoId);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraServicioSucursal_SucursalId ON InfraServiciosSucursal (SucursalId);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraServiciosSucursal_TipoServicioId ON InfraServiciosSucursal (TipoServicioId);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraServiciosSucursal_OperadorServicioId ON InfraServiciosSucursal (OperadorServicioId);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraSucursalAccesorio_SucursalId ON InfraSucursalesAccesorio (SucursalId);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_InfraSucursalesAccesorio_TipoAccesorioId ON InfraSucursalesAccesorio (TipoAccesorioId);");
        }
    }
}
