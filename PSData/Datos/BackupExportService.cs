using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;

namespace PSData.Backup
{
    /// <summary>
    /// Servicio para backup, exportación e importación de datos.
    /// Pensado para migrar el inventario al cambiar de computadora.
    /// Compatible con SQLite.
    /// </summary>
    public class BackupExportService
    {
        private const int FormatVersion = 1;
        private readonly PSDatos _context;
        private readonly string _connectionString;

        public BackupExportService(PSDatos context)
        {
            _context = context;
            _connectionString = context.Database.GetConnectionString() ?? "";
        }

        /// <summary>
        /// Exporta todos los datos a un archivo JSON portable.
        /// </summary>
        public async Task<byte[]> ExportToJsonAsync()
        {
            var data = new ExportData
            {
                Version = FormatVersion,
                ExportDate = DateTime.UtcNow,
                Regiones = await _context.Regiones.AsNoTracking().ToListAsync(),
                Categorias = await _context.Categorias.AsNoTracking().ToListAsync(),
                Departamentos = await _context.Departamentos.AsNoTracking().ToListAsync(),
                Usuarios = await _context.Usuarios.AsNoTracking().ToListAsync(),
                Sucursales = await _context.Sucursales.AsNoTracking().ToListAsync(),
                Articulos = await _context.Articulos.AsNoTracking().ToListAsync(),
                Compras = await _context.Compras.AsNoTracking().ToListAsync(),
                Lotes = await _context.Lotes.AsNoTracking().ToListAsync(),
                Items = await _context.Items.AsNoTracking().ToListAsync(),
                SolicitudesCompra = await _context.SolicitudesCompra.AsNoTracking().ToListAsync(),
                DetallesSolicitudCompra = await _context.DetallesSolicitudCompra.AsNoTracking().ToListAsync(),
                MovimientosItem = await _context.MovimientosItem.AsNoTracking().ToListAsync()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            return System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, options));
        }

        /// <summary>
        /// Importa datos desde un archivo JSON exportado previamente.
        /// Borra los datos actuales e inserta los importados.
        /// </summary>
        public async Task<ImportResult> ImportFromJsonAsync(Stream stream)
        {
            var result = new ImportResult();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            ExportData data;
            try
            {
                data = JsonSerializer.Deserialize<ExportData>(json, jsonOptions);
            }
            catch (JsonException ex)
            {
                result.Success = false;
                result.ErrorMessage = "Archivo JSON invalido: " + ex.Message;
                return result;
            }

            if (data == null)
            {
                result.Success = false;
                result.ErrorMessage = "El archivo esta vacio o es invalido.";
                return result;
            }

            await using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();

            var transaction = conn.BeginTransaction();
            try
            {
                await EjecutarSqlAsync(conn, transaction, "PRAGMA foreign_keys = OFF");

                await EjecutarSqlAsync(conn, transaction, "DELETE FROM MovimientosItem");
                await EjecutarSqlAsync(conn, transaction, "DELETE FROM DetallesSolicitudCompra");
                await EjecutarSqlAsync(conn, transaction, "DELETE FROM SolicitudesCompra");
                await EjecutarSqlAsync(conn, transaction, "DELETE FROM Items");
                await EjecutarSqlAsync(conn, transaction, "DELETE FROM Lotes");
                await EjecutarSqlAsync(conn, transaction, "DELETE FROM Compras");
                await EjecutarSqlAsync(conn, transaction, "DELETE FROM Articulos");
                await EjecutarSqlAsync(conn, transaction, "DELETE FROM Sucursales");
                await EjecutarSqlAsync(conn, transaction, "DELETE FROM Usuarios");
                await EjecutarSqlAsync(conn, transaction, "DELETE FROM Departamentos");
                await EjecutarSqlAsync(conn, transaction, "DELETE FROM Regiones");
                await EjecutarSqlAsync(conn, transaction, "DELETE FROM Categorias");

                await InsertarRegiones(conn, transaction, data.Regiones ?? new List<Region>());
                await InsertarCategorias(conn, transaction, data.Categorias ?? new List<Categoria>());
                await InsertarDepartamentos(conn, transaction, data.Departamentos ?? new List<Departamento>());
                await InsertarUsuarios(conn, transaction, data.Usuarios ?? new List<Usuario>());
                await InsertarSucursales(conn, transaction, data.Sucursales ?? new List<Sucursal>());
                await InsertarArticulos(conn, transaction, data.Articulos ?? new List<Articulo>());
                await InsertarCompras(conn, transaction, data.Compras ?? new List<Compra>());
                await InsertarLotes(conn, transaction, data.Lotes ?? new List<Lote>());
                await InsertarItems(conn, transaction, data.Items ?? new List<Item>());
                await InsertarSolicitudesCompra(conn, transaction, data.SolicitudesCompra ?? new List<SolicitudCompra>());
                await InsertarDetallesSolicitudCompra(conn, transaction, data.DetallesSolicitudCompra ?? new List<DetalleSolicitudCompra>());
                await InsertarMovimientosItem(conn, transaction, data.MovimientosItem ?? new List<MovimientoItem>());

                await EjecutarSqlAsync(conn, transaction, "PRAGMA foreign_keys = ON");
                transaction.Commit();
                result.Success = true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Crea una copia del archivo de base de datos SQLite (.db).
        /// </summary>
        public async Task<BackupResult> CreateSqlBackupAsync(string rutaDestino)
        {
            var result = new BackupResult();
            try
            {
                var dbPath = ObtenerRutaBaseDatos();
                if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
                {
                    result.Success = false;
                    result.ErrorMessage = "No se encontro el archivo de base de datos o aun no ha sido creado.";
                    return result;
                }

                Directory.CreateDirectory(rutaDestino);
                var archivoBackup = Path.Combine(rutaDestino, "PSInventory_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".db");

                await Task.Run(() => File.Copy(dbPath, archivoBackup, overwrite: true));

                result.Success = true;
                result.BackupPath = archivoBackup;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return await Task.FromResult(result);
        }

        private string ObtenerRutaBaseDatos()
        {
            var ds = "Data Source=";
            var idx = _connectionString.IndexOf(ds, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return "";

            var path = _connectionString.Substring(idx + ds.Length).Trim().TrimEnd(';');
            if (string.IsNullOrWhiteSpace(path)) return "";

            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
            }
            return Path.GetFullPath(path);
        }

        private static async Task EjecutarSqlAsync(SqliteConnection conn, SqliteTransaction tx, string sql)
        {
            await using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync();
        }

        private static string Q(string s) => s == null ? "NULL" : "'" + (s ?? "").Replace("'", "''") + "'";

        private static async Task InsertarRegiones(SqliteConnection conn, SqliteTransaction tx, List<Region> items)
        {
            if (items.Count == 0) return;
            foreach (var e in items)
            {
                await EjecutarSqlAsync(conn, tx, string.Format(@"INSERT INTO Regiones (RegionId, Nombre, Descripcion, Activo, Eliminado, FechaEliminacion, UsuarioEliminacion)
                    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})", e.RegionId, Q(e.Nombre), Q(e.Descripcion), e.Activo ? 1 : 0, e.Eliminado ? 1 : 0, e.FechaEliminacion.HasValue ? Q(e.FechaEliminacion.Value.ToString("yyyy-MM-dd")) : "NULL", Q(e.UsuarioEliminacion)));
            }
        }

        private static async Task InsertarCategorias(SqliteConnection conn, SqliteTransaction tx, List<Categoria> items)
        {
            if (items.Count == 0) return;
            foreach (var e in items)
            {
                await EjecutarSqlAsync(conn, tx, string.Format(@"INSERT INTO Categorias (Id, Nombre, Descripcion, RequiereNumeroSerie, Eliminado, FechaEliminacion, UsuarioEliminacion)
                    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})", e.Id, Q(e.Nombre), Q(e.Descripcion), e.RequiereNumeroSerie ? 1 : 0, e.Eliminado ? 1 : 0, e.FechaEliminacion.HasValue ? Q(e.FechaEliminacion.Value.ToString("yyyy-MM-dd")) : "NULL", Q(e.UsuarioEliminacion)));
            }
        }

        private static async Task InsertarDepartamentos(SqliteConnection conn, SqliteTransaction tx, List<Departamento> items)
        {
            if (items.Count == 0) return;
            foreach (var e in items)
            {
                await EjecutarSqlAsync(conn, tx, string.Format(@"INSERT INTO Departamentos (Id, Nombre, Descripcion, Responsable, Activo, Eliminado, FechaEliminacion, UsuarioEliminacion)
                    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})", e.Id, Q(e.Nombre), Q(e.Descripcion), Q(e.Responsable), e.Activo ? 1 : 0, e.Eliminado ? 1 : 0, e.FechaEliminacion.HasValue ? Q(e.FechaEliminacion.Value.ToString("yyyy-MM-dd")) : "NULL", Q(e.UsuarioEliminacion)));
            }
        }

        private static async Task InsertarUsuarios(SqliteConnection conn, SqliteTransaction tx, List<Usuario> items)
        {
            foreach (var e in items)
            {
                await EjecutarSqlAsync(conn, tx, string.Format(@"INSERT INTO Usuarios (Id, Nombre, Password, Email, Rol, Eliminado, FechaEliminacion, UsuarioEliminacion)
                    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})", Q(e.Id), Q(e.Nombre), Q(e.Password), Q(e.Email), Q(e.Rol), e.Eliminado ? 1 : 0, e.FechaEliminacion.HasValue ? Q(e.FechaEliminacion.Value.ToString("yyyy-MM-dd")) : "NULL", Q(e.UsuarioEliminacion)));
            }
        }

        private static async Task InsertarSucursales(SqliteConnection conn, SqliteTransaction tx, List<Sucursal> items)
        {
            foreach (var e in items)
            {
                await EjecutarSqlAsync(conn, tx, string.Format(@"INSERT INTO Sucursales (Id, Nombre, Telefono, Direccion, RegionId, Activo, Eliminado, FechaEliminacion, UsuarioEliminacion)
                    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})", Q(e.Id), Q(e.Nombre), Q(e.Telefono), Q(e.Direccion), e.RegionId, e.Activo ? 1 : 0, e.Eliminado ? 1 : 0, e.FechaEliminacion.HasValue ? Q(e.FechaEliminacion.Value.ToString("yyyy-MM-dd")) : "NULL", Q(e.UsuarioEliminacion)));
            }
        }

        private static async Task InsertarArticulos(SqliteConnection conn, SqliteTransaction tx, List<Articulo> items)
        {
            if (items.Count == 0) return;
            foreach (var e in items)
            {
                await EjecutarSqlAsync(conn, tx, string.Format(@"INSERT INTO Articulos (Id, Marca, Modelo, CategoriaId, Descripcion, StockMinimo, Especificaciones, RequiereSerial, Eliminado, FechaEliminacion, UsuarioEliminacion)
                    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})", e.Id, Q(e.Marca), Q(e.Modelo), e.CategoriaId, Q(e.Descripcion), e.StockMinimo, Q(e.Especificaciones), e.RequiereSerial ? 1 : 0, e.Eliminado ? 1 : 0, e.FechaEliminacion.HasValue ? Q(e.FechaEliminacion.Value.ToString("yyyy-MM-dd")) : "NULL", Q(e.UsuarioEliminacion)));
            }
        }

        private static async Task InsertarCompras(SqliteConnection conn, SqliteTransaction tx, List<Compra> items)
        {
            if (items.Count == 0) return;
            foreach (var e in items)
            {
                var deptId = e.DepartamentoId.HasValue ? e.DepartamentoId.ToString() : "NULL";
                var fechaSol = e.FechaSolicitud.HasValue ? Q(e.FechaSolicitud.Value.ToString("yyyy-MM-dd HH:mm:ss")) : "NULL";
                await EjecutarSqlAsync(conn, tx, string.Format(@"INSERT INTO Compras (Id, Proveedor, CostoTotal, FechaCompra, NumeroFactura, Estado, Observaciones, RutaFactura, DepartamentoId, UsuarioSolicitante, FechaSolicitud, Eliminado, FechaEliminacion, UsuarioEliminacion)
                    VALUES ({0}, {1}, {2}, '{3}', {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13})", e.Id, Q(e.Proveedor), e.CostoTotal, e.FechaCompra.ToString("yyyy-MM-dd HH:mm:ss"), Q(e.NumeroFactura), Q(e.Estado), Q(e.Observaciones), Q(e.RutaFactura), deptId, Q(e.UsuarioSolicitante), fechaSol, e.Eliminado ? 1 : 0, e.FechaEliminacion.HasValue ? Q(e.FechaEliminacion.Value.ToString("yyyy-MM-dd")) : "NULL", Q(e.UsuarioEliminacion)));
            }
        }

        private static async Task InsertarLotes(SqliteConnection conn, SqliteTransaction tx, List<Lote> items)
        {
            if (items.Count == 0) return;
            foreach (var e in items)
            {
                await EjecutarSqlAsync(conn, tx, string.Format("INSERT INTO Lotes (Id, ArticuloId, CompraId, Cantidad, CostoUnitario) VALUES ({0}, {1}, {2}, {3}, {4})", e.Id, e.ArticuloId, e.CompraId, e.Cantidad, e.CostoUnitario));
            }
        }

        private static async Task InsertarItems(SqliteConnection conn, SqliteTransaction tx, List<Item> items)
        {
            if (items.Count == 0) return;
            foreach (var e in items)
            {
                var sucursalId = e.SucursalId != null ? Q(e.SucursalId) : "NULL";
                await EjecutarSqlAsync(conn, tx, string.Format(@"INSERT INTO Items (Id, Serial, Cantidad, ArticuloId, SucursalId, Estado, LoteId, Ubicacion, ResponsableEmpleado, FechaAsignacion, FechaUltimaTransferencia, FechaGarantiaInicio, MesesGarantia, FechaGarantiaVencimiento, Observaciones, Eliminado, FechaEliminacion, UsuarioEliminacion)
                    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17})", e.Id, Q(e.Serial), e.Cantidad, e.ArticuloId, sucursalId, Q(e.Estado), e.LoteId, Q(e.Ubicacion), Q(e.ResponsableEmpleado), e.FechaAsignacion.HasValue ? Q(e.FechaAsignacion.Value.ToString("yyyy-MM-dd")) : "NULL", e.FechaUltimaTransferencia.HasValue ? Q(e.FechaUltimaTransferencia.Value.ToString("yyyy-MM-dd")) : "NULL", e.FechaGarantiaInicio.HasValue ? Q(e.FechaGarantiaInicio.Value.ToString("yyyy-MM-dd")) : "NULL", e.MesesGarantia.HasValue ? e.MesesGarantia.ToString() : "NULL", e.FechaGarantiaVencimiento.HasValue ? Q(e.FechaGarantiaVencimiento.Value.ToString("yyyy-MM-dd")) : "NULL", Q(e.Observaciones), e.Eliminado ? 1 : 0, e.FechaEliminacion.HasValue ? Q(e.FechaEliminacion.Value.ToString("yyyy-MM-dd")) : "NULL", Q(e.UsuarioEliminacion)));
            }
        }

        private static async Task InsertarSolicitudesCompra(SqliteConnection conn, SqliteTransaction tx, List<SolicitudCompra> items)
        {
            if (items.Count == 0) return;
            foreach (var e in items)
            {
                await EjecutarSqlAsync(conn, tx, string.Format(@"INSERT INTO SolicitudesCompra (Id, Titulo, Solicitante, FechaSolicitud, Estado, Observaciones, UsuarioRevisor, FechaRevision, NotasRevision, Eliminado)
                    VALUES ({0}, {1}, {2}, '{3}', {4}, {5}, {6}, {7}, {8}, {9})", e.Id, Q(e.Titulo), Q(e.Solicitante), e.FechaSolicitud.ToString("yyyy-MM-dd HH:mm:ss"), Q(e.Estado), Q(e.Observaciones), Q(e.UsuarioRevisor), e.FechaRevision.HasValue ? Q(e.FechaRevision.Value.ToString("yyyy-MM-dd HH:mm:ss")) : "NULL", Q(e.NotasRevision), e.Eliminado ? 1 : 0));
            }
        }

        private static async Task InsertarDetallesSolicitudCompra(SqliteConnection conn, SqliteTransaction tx, List<DetalleSolicitudCompra> items)
        {
            if (items.Count == 0) return;
            foreach (var e in items)
            {
                var artId = e.ArticuloId.HasValue ? e.ArticuloId.ToString() : "NULL";
                await EjecutarSqlAsync(conn, tx, string.Format(@"INSERT INTO DetallesSolicitudCompra (Id, SolicitudId, ArticuloId, DescripcionLibre, Cantidad, Observaciones)
                    VALUES ({0}, {1}, {2}, {3}, {4}, {5})", e.Id, e.SolicitudId, artId, Q(e.DescripcionLibre), e.Cantidad, Q(e.Observaciones)));
            }
        }

        private static async Task InsertarMovimientosItem(SqliteConnection conn, SqliteTransaction tx, List<MovimientoItem> items)
        {
            if (items.Count == 0) return;
            foreach (var e in items)
            {
                await EjecutarSqlAsync(conn, tx, string.Format(@"INSERT INTO MovimientosItem (Id, ItemId, Cantidad, SucursalOrigenId, SucursalDestinoId, FechaMovimiento, UsuarioResponsable, Motivo, Observaciones, ResponsableRecepcion, FechaRecepcion)
                    VALUES ({0}, {1}, {2}, {3}, {4}, '{5}', {6}, {7}, {8}, {9}, {10})", e.Id, e.ItemId, e.Cantidad, Q(e.SucursalOrigenId), Q(e.SucursalDestinoId), e.FechaMovimiento.ToString("yyyy-MM-dd HH:mm:ss"), Q(e.UsuarioResponsable), Q(e.Motivo), Q(e.Observaciones), Q(e.ResponsableRecepcion), e.FechaRecepcion.HasValue ? Q(e.FechaRecepcion.Value.ToString("yyyy-MM-dd")) : "NULL"));
            }
        }
    }

    internal class ExportData
    {
        public int Version { get; set; }
        public DateTime ExportDate { get; set; }
        public List<Region> Regiones { get; set; }
        public List<Categoria> Categorias { get; set; }
        public List<Departamento> Departamentos { get; set; }
        public List<Usuario> Usuarios { get; set; }
        public List<Sucursal> Sucursales { get; set; }
        public List<Articulo> Articulos { get; set; }
        public List<Compra> Compras { get; set; }
        public List<Lote> Lotes { get; set; }
        public List<Item> Items { get; set; }
        public List<SolicitudCompra> SolicitudesCompra { get; set; }
        public List<DetalleSolicitudCompra> DetallesSolicitudCompra { get; set; }
        public List<MovimientoItem> MovimientosItem { get; set; }
    }

    public class ImportResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class BackupResult
    {
        public bool Success { get; set; }
        public string BackupPath { get; set; }
        public string ErrorMessage { get; set; }
    }
}
