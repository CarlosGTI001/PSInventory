using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador", "Jefe")]
    public class DespachoController : Controller
    {
        private readonly PSDatos _context;

        public DespachoController(PSDatos context)
        {
            _context = context;
        }

        // GET: Despacho
        public async Task<IActionResult> Index()
        {
            ViewBag.Departamentos = await _context.Departamentos
                .Where(d => !d.Eliminado)
                .OrderBy(d => d.Nombre)
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nombre })
                .ToListAsync();

            ViewBag.Regiones = await _context.Regiones
                .Where(r => !r.Eliminado)
                .OrderBy(r => r.Nombre)
                .Select(r => new SelectListItem { Value = r.RegionId.ToString(), Text = r.Nombre })
                .ToListAsync();

            return View();
        }

        // GET: Despacho/SucursalesPorRegion?regionId=3
        [HttpGet]
        public async Task<IActionResult> SucursalesPorRegion(int regionId)
        {
            var sucursales = await _context.Sucursales
                .Where(s => !s.Eliminado && s.RegionId == regionId)
                .OrderBy(s => s.Nombre)
                .Select(s => new { value = s.Id, text = s.Nombre })
                .ToListAsync();

            return Json(sucursales);
        }

        // GET: Despacho/BuscarStock?q=laptop
        [HttpGet]
        public async Task<IActionResult> BuscarStock(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return Json(new List<object>());

            q = q.Trim().ToLower();

            // Obtener artículos que coincidan con la búsqueda
            var articulos = await _context.Articulos
                .Where(a => !a.Eliminado &&
                    (a.Marca.ToLower().Contains(q) ||
                     a.Modelo.ToLower().Contains(q) ||
                     a.Categoria.Nombre.ToLower().Contains(q) ||
                     (a.Descripcion != null && a.Descripcion.ToLower().Contains(q))))
                .Include(a => a.Categoria)
                .OrderBy(a => a.Marca).ThenBy(a => a.Modelo)
                .Take(15)
                .ToListAsync();

            if (!articulos.Any())
                return Json(new List<object>());

            var articuloIds = articulos.Select(a => a.Id).ToList();

            // Consulta directa de stock disponible agrupado por artículo
            // (sin filtered-include para evitar conteos incorrectos por change-tracking)
            var stockPorArticulo = await _context.Items
                .Where(i => articuloIds.Contains(i.ArticuloId) &&
                            !i.Eliminado &&
                            i.Estado == "Disponible" &&
                            i.SucursalId == null &&
                            i.Cantidad > 0)
                .GroupBy(i => i.ArticuloId)
                .Select(g => new
                {
                    ArticuloId   = g.Key,
                    TotalUnidades = g.Sum(i => i.Cantidad),  // para sin serial
                    TotalItems    = g.Count()                 // para con serial
                })
                .ToDictionaryAsync(x => x.ArticuloId);

            var resultado = articulos
                .Where(a => stockPorArticulo.ContainsKey(a.Id))
                .Select(a =>
                {
                    var stock = stockPorArticulo[a.Id];
                    return new
                    {
                        articuloId     = a.Id,
                        nombre         = $"{a.Marca} {a.Modelo}",
                        categoria      = a.Categoria?.Nombre,
                        requiereSerial = a.RequiereSerial,
                        disponible     = a.RequiereSerial
                            ? stock.TotalItems     // Con serial: contar items individuales
                            : stock.TotalUnidades  // Sin serial: sumar cantidades
                    };
                });

            return Json(resultado);
        }

        // GET: Despacho/BuscarSerial?articuloId=5&serial=ABC123
        [HttpGet]
        public async Task<IActionResult> BuscarSerial(int articuloId, string serial)
        {
            if (string.IsNullOrWhiteSpace(serial))
                return Json(new { found = false, mensaje = "Serial vacío." });

            var item = await _context.Items
                .FirstOrDefaultAsync(i =>
                    i.ArticuloId == articuloId &&
                    i.Serial == serial.Trim() &&
                    !i.Eliminado &&
                    i.Estado == "Disponible" &&
                    i.SucursalId == null);

            if (item == null)
                return Json(new { found = false, mensaje = "Serial no encontrado o no disponible." });

            return Json(new { found = true, itemId = item.Id, serial = item.Serial });
        }

        // POST: Despacho/Enviar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enviar([FromBody] DespachoInput input)
        {
            if (string.IsNullOrEmpty(input.SucursalDestinoId) ||
                string.IsNullOrEmpty(input.ResponsableEmpleado) ||
                input.Items == null || input.Items.Count == 0)
            {
                return Json(new { success = false, message = "Datos incompletos para el despacho." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            var procesados = 0;
            var errores = new List<string>();

            try
            {
                foreach (var entry in input.Items)
                {
                    // ── Caso 1: Con serial — item específico por ItemId ───────────────
                    if (entry.ItemId.HasValue)
                    {
                        var item = await _context.Items
                            .Include(i => i.Articulo)
                            .FirstOrDefaultAsync(i => i.Id == entry.ItemId.Value && !i.Eliminado);

                        if (item == null)
                        { errores.Add($"Item #{entry.ItemId} no encontrado."); continue; }

                        if (item.Estado != "Disponible" || item.SucursalId != null)
                        { errores.Add($"Item #{entry.ItemId} ya no está disponible."); continue; }

                        item.SucursalId          = input.SucursalDestinoId;
                        item.Estado              = "Asignado";
                        item.ResponsableEmpleado = input.ResponsableEmpleado;
                        item.FechaAsignacion     = DateTime.Now;
                        _context.Update(item);
                        await _context.SaveChangesAsync();

                        _context.MovimientosItem.Add(new MovimientoItem
                        {
                            ItemId               = item.Id,
                            Cantidad             = 1,
                            SucursalOrigenId     = null,
                            SucursalDestinoId    = input.SucursalDestinoId,
                            FechaMovimiento      = DateTime.Now,
                            UsuarioResponsable   = HttpContext.Session.GetString("UserName") ?? "Sistema",
                            Motivo               = "Despacho",
                            Observaciones        = input.Observaciones,
                            ResponsableRecepcion = input.ResponsableEmpleado,
                            FechaRecepcion       = DateTime.Now
                        });
                        await _context.SaveChangesAsync();
                        procesados++;
                    }
                    // ── Caso 2: Sin serial — distribuir cantidad entre stock disponible ─
                    else if (entry.ArticuloId.HasValue)
                    {
                        var pendiente = entry.Cantidad;
                        var itemsDisponibles = await _context.Items
                            .Where(i => i.ArticuloId == entry.ArticuloId.Value &&
                                        !i.Eliminado &&
                                        i.Estado == "Disponible" &&
                                        i.SucursalId == null &&
                                        i.Cantidad > 0)           // excluir items con cantidad = 0
                            .OrderBy(i => i.LoteId).ThenBy(i => i.Id)
                            .ToListAsync();

                        foreach (var it in itemsDisponibles)
                        {
                            if (pendiente <= 0) break;
                            var tomar = Math.Min(pendiente, it.Cantidad);
                            pendiente -= tomar;

                            Item itemAsignado;
                            if (tomar < it.Cantidad)
                            {
                                it.Cantidad -= tomar;
                                _context.Update(it);
                                itemAsignado = new Item
                                {
                                    ArticuloId               = it.ArticuloId,
                                    LoteId                   = it.LoteId,
                                    SucursalId               = input.SucursalDestinoId,
                                    Estado                   = "Asignado",
                                    Cantidad                 = tomar,
                                    ResponsableEmpleado      = input.ResponsableEmpleado,
                                    FechaAsignacion          = DateTime.Now,
                                    Serial                   = null,
                                    FechaGarantiaInicio      = it.FechaGarantiaInicio,
                                    MesesGarantia            = it.MesesGarantia,
                                    FechaGarantiaVencimiento = it.FechaGarantiaVencimiento
                                };
                                _context.Items.Add(itemAsignado);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                it.SucursalId          = input.SucursalDestinoId;
                                it.Estado              = "Asignado";
                                it.ResponsableEmpleado = input.ResponsableEmpleado;
                                it.FechaAsignacion     = DateTime.Now;
                                itemAsignado = it;
                                _context.Update(it);
                                await _context.SaveChangesAsync();
                            }

                            _context.MovimientosItem.Add(new MovimientoItem
                            {
                                ItemId               = itemAsignado.Id,
                                Cantidad             = tomar,
                                SucursalOrigenId     = null,
                                SucursalDestinoId    = input.SucursalDestinoId,
                                FechaMovimiento      = DateTime.Now,
                                UsuarioResponsable   = HttpContext.Session.GetString("UserName") ?? "Sistema",
                                Motivo               = "Despacho",
                                Observaciones        = input.Observaciones,
                                ResponsableRecepcion = input.ResponsableEmpleado,
                                FechaRecepcion       = DateTime.Now
                            });
                            await _context.SaveChangesAsync();
                        }

                        if (pendiente > 0)
                            errores.Add($"Stock insuficiente: faltan {pendiente} unidad(es) del artículo #{entry.ArticuloId}.");
                        else
                            procesados++;
                    }
                }

                await transaction.CommitAsync();

                var sucursal = await _context.Sucursales.FindAsync(input.SucursalDestinoId);
                var msg = $"{procesados} item(s) despachado(s) a {sucursal?.Nombre} exitosamente.";
                if (errores.Any())
                    msg += $" {errores.Count} error(es): {string.Join(" | ", errores)}";

                return Json(new { success = true, message = msg, procesados, errores });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = $"Error al procesar el despacho: {ex.Message}" });
            }
        }
    }
}
