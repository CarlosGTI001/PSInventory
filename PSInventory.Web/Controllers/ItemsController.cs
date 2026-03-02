using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador", "Jefe")]
    public class ItemsController : Controller
    {
        private readonly PSDatos _context;

        public ItemsController(PSDatos context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index(string estado = "")
        {
            var query = _context.Items
                .Where(i => !i.Eliminado)
                .Include(i => i.Articulo)
                .ThenInclude(a => a.Categoria)
                .Include(i => i.Lote)
                .ThenInclude(l => l.Compra)
                .Include(i => i.Sucursal)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(i => i.Estado == estado);
            }

            var items = await query.OrderByDescending(i => i.Serial).ToListAsync();
            ViewBag.EstadoFiltro = estado;
            return View(items);
        }

        // GET: Items/Create
        public async Task<IActionResult> Create()
        {
            var articulos = await _context.Articulos
                .Where(a => !a.Eliminado)
                .Include(a => a.Categoria)
                .OrderBy(a => a.Marca)
                .ThenBy(a => a.Modelo)
                .ToListAsync();
            ViewBag.Articulos = new SelectList(articulos.Select(a => new { 
                Id = a.Id, 
                Display = $"{a.Marca} {a.Modelo}" 
            }), "Id", "Display");
            
            var lotes = await _context.Lotes
                .Include(l => l.Compra)
                .Include(l => l.Articulo)
                .Where(l => !l.Compra.Eliminado)
                .OrderByDescending(l => l.Compra.FechaCompra)
                .ToListAsync();
            ViewBag.Lotes = new SelectList(lotes.Select(l => new {
                Id = l.Id,
                Display = $"Lote {l.Id} - {l.Compra.Proveedor} ({l.Articulo.Modelo})"
            }), "Id", "Display");
            ViewBag.Sucursales = new SelectList(await _context.Sucursales
                .Where(s => !s.Eliminado)
                .OrderBy(s => s.Nombre)
                .ToListAsync(), "Id", "Nombre");
            
            return View(new Item { 
                Estado = "Disponible",
                FechaGarantiaInicio = DateTime.Now
            });
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Serial,ArticuloId,LoteId,SucursalId,Estado,MesesGarantia,FechaGarantiaInicio,Observaciones")] Item item)
        {
            if (ModelState.IsValid)
            {
                // Calcular fecha de vencimiento de garantía
                if (item.MesesGarantia.HasValue && item.FechaGarantiaInicio.HasValue)
                {
                    item.FechaGarantiaVencimiento = item.FechaGarantiaInicio.Value.AddMonths(item.MesesGarantia.Value);
                }
                
                _context.Add(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Item registrado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            
            var articulos = await _context.Articulos.Include(a => a.Categoria).OrderBy(a => a.Marca).ThenBy(a => a.Modelo).ToListAsync();
            ViewBag.Articulos = new SelectList(articulos.Select(a => new { 
                Id = a.Id, 
                Display = $"{a.Marca} {a.Modelo}" 
            }), "Id", "Display", item.ArticuloId);
            
            var lotes = await _context.Lotes.Include(l => l.Compra).Include(l => l.Articulo).OrderByDescending(l => l.Compra.FechaCompra).ToListAsync();
            ViewBag.Lotes = new SelectList(lotes.Select(l => new { Id = l.Id, Display = $"Lote {l.Id} - {l.Compra.Proveedor} ({l.Articulo.Modelo})" }), "Id", "Display", item.LoteId);
            ViewBag.Sucursales = new SelectList(await _context.Sucursales.OrderBy(s => s.Nombre).ToListAsync(), "Id", "Nombre", item.SucursalId);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            
            var articulos = await _context.Articulos.Include(a => a.Categoria).OrderBy(a => a.Marca).ThenBy(a => a.Modelo).ToListAsync();
            ViewBag.Articulos = new SelectList(articulos.Select(a => new { 
                Id = a.Id, 
                Display = $"{a.Marca} {a.Modelo}" 
            }), "Id", "Display", item.ArticuloId);
            
            var lotes = await _context.Lotes.Include(l => l.Compra).Include(l => l.Articulo).OrderByDescending(l => l.Compra.FechaCompra).ToListAsync();
            ViewBag.Lotes = new SelectList(lotes.Select(l => new { Id = l.Id, Display = $"Lote {l.Id} - {l.Compra.Proveedor} ({l.Articulo.Modelo})" }), "Id", "Display", item.LoteId);
            ViewBag.Sucursales = new SelectList(await _context.Sucursales.OrderBy(s => s.Nombre).ToListAsync(), "Id", "Nombre", item.SucursalId);
            return View(item);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Serial,ArticuloId,LoteId,SucursalId,Estado,Cantidad,MesesGarantia,FechaGarantiaInicio,Observaciones")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Recalcular fecha de vencimiento de garantía
                    if (item.MesesGarantia.HasValue && item.FechaGarantiaInicio.HasValue)
                    {
                        item.FechaGarantiaVencimiento = item.FechaGarantiaInicio.Value.AddMonths(item.MesesGarantia.Value);
                    }
                    
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Item actualizado exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            var articulos = await _context.Articulos.Include(a => a.Categoria).OrderBy(a => a.Marca).ThenBy(a => a.Modelo).ToListAsync();
            ViewBag.Articulos = new SelectList(articulos.Select(a => new { 
                Id = a.Id, 
                Display = $"{a.Marca} {a.Modelo}" 
            }), "Id", "Display", item.ArticuloId);
            
            var lotes = await _context.Lotes.Include(l => l.Compra).Include(l => l.Articulo).OrderByDescending(l => l.Compra.FechaCompra).ToListAsync();
            ViewBag.Lotes = new SelectList(lotes.Select(l => new { Id = l.Id, Display = $"Lote {l.Id} - {l.Compra.Proveedor} ({l.Articulo.Modelo})" }), "Id", "Display", item.LoteId);
            ViewBag.Sucursales = new SelectList(await _context.Sucursales.OrderBy(s => s.Nombre).ToListAsync(), "Id", "Nombre", item.SucursalId);
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Items
                .Where(i => !i.Eliminado)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return Json(new { success = false, message = "Item no encontrado" });
            }

            // Soft delete
            item.Eliminado = true;
            item.FechaEliminacion = DateTime.Now;
            item.UsuarioEliminacion = HttpContext.Session.GetString("UserName") ?? "Sistema";

            _context.Update(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Item eliminado exitosamente" });
        }

        // GET: Items/Asignar/5
        public async Task<IActionResult> Asignar(int id)
        {

            var item = await _context.Items
                .Include(i => i.Articulo)
                .ThenInclude(a => a.Categoria)
                .Include(i => i.Lote)
                .ThenInclude(l => l.Compra)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            // Verificar que el item esté disponible
            if (item.Estado != "Disponible" || !string.IsNullOrEmpty(item.SucursalId))
            {
                TempData["Error"] = "Solo se pueden asignar items que estén en estado Disponible y sin sucursal asignada";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Sucursales = new SelectList(await _context.Sucursales.OrderBy(s => s.Nombre).ToListAsync(), "Id", "Nombre");
            return View(item);
        }

        // POST: Items/Asignar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignar(int id, string sucursalId, string responsableEmpleado, string observaciones, int cantidadAsignar = 1)
        {
            if (string.IsNullOrEmpty(sucursalId) || string.IsNullOrEmpty(responsableEmpleado) || cantidadAsignar <= 0)
            {
                TempData["Error"] = "Debe proporcionar Sucursal, Responsable y una cantidad válida";
                return RedirectToAction(nameof(Asignar), new { id });
            }

            var item = await _context.Items.Include(i => i.Articulo).FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            if (item.Estado != "Disponible" || !string.IsNullOrEmpty(item.SucursalId))
            {
                TempData["Error"] = "El item ya no está disponible para asignación";
                return RedirectToAction(nameof(Index));
            }
            
            if (cantidadAsignar > item.Cantidad)
            {
                TempData["Error"] = "La cantidad a asignar no puede ser mayor que la cantidad disponible";
                return RedirectToAction(nameof(Asignar), new { id });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Item itemAsignado;
                
                // Si asignamos menos de lo que hay y el artículo no requiere serial (es por lote)
                if (cantidadAsignar < item.Cantidad && !item.Articulo.RequiereSerial)
                {
                    // 1. Reducir la cantidad del item original en el almacén
                    item.Cantidad -= cantidadAsignar;
                    _context.Update(item);
                    
                    // 2. Crear un nuevo registro de Item para la sucursal
                    itemAsignado = new Item
                    {
                        ArticuloId = item.ArticuloId,
                        LoteId = item.LoteId,
                        SucursalId = sucursalId,
                        Estado = "Asignado",
                        Cantidad = cantidadAsignar,
                        ResponsableEmpleado = responsableEmpleado,
                        FechaAsignacion = DateTime.Now,
                        Serial = null,
                        FechaGarantiaInicio = item.FechaGarantiaInicio,
                        MesesGarantia = item.MesesGarantia,
                        FechaGarantiaVencimiento = item.FechaGarantiaVencimiento
                    };
                    _context.Items.Add(itemAsignado);
                    await _context.SaveChangesAsync(); // Guardar para obtener el nuevo ID
                }
                else
                {
                    // Mover el registro completo
                    item.SucursalId = sucursalId;
                    item.Estado = "Asignado";
                    item.ResponsableEmpleado = responsableEmpleado;
                    item.FechaAsignacion = DateTime.Now;
                    itemAsignado = item;
                    _context.Update(item);
                }

                // Crear registro de movimiento
                var movimiento = new MovimientoItem
                {
                    ItemId = itemAsignado.Id,
                    Cantidad = cantidadAsignar,
                    SucursalOrigenId = null, // Desde almacén (sin origen)
                    SucursalDestinoId = sucursalId,
                    FechaMovimiento = DateTime.Now,
                    UsuarioResponsable = HttpContext.Session.GetString("UserName") ?? "Sistema",
                    Motivo = "Asignación Inicial",
                    Observaciones = observaciones,
                    ResponsableRecepcion = responsableEmpleado,
                    FechaRecepcion = DateTime.Now
                };

                _context.MovimientosItem.Add(movimiento);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = $"Item(s) asignado(s) exitosamente a {_context.Sucursales.Find(sucursalId)?.Nombre}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Ocurrió un error al procesar la asignación.";
                return RedirectToAction(nameof(Asignar), new { id });
            }
        }

        // GET: Items/Transferir/5
        public async Task<IActionResult> Transferir(int id)
        {

            var item = await _context.Items
                .Include(i => i.Articulo)
                .ThenInclude(a => a.Categoria)
                .Include(i => i.Sucursal)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            // Verificar que el item esté asignado
            if (item.Estado != "Asignado" || string.IsNullOrEmpty(item.SucursalId))
            {
                TempData["Error"] = "Solo se pueden transferir items que estén asignados a una sucursal";
                return RedirectToAction(nameof(Index));
            }

            // Cargar sucursales excluyendo la actual
            ViewBag.Sucursales = new SelectList(
                await _context.Sucursales
                    .Where(s => s.Id != item.SucursalId)
                    .OrderBy(s => s.Nombre)
                    .ToListAsync(), 
                "Id", 
                "Nombre"
            );

            return View(item);
        }

        // POST: Items/Transferir/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transferir(int id, string sucursalDestinoId, string responsableRecepcion, string motivo, string observaciones, int cantidadTransferir = 1)
        {
            if (string.IsNullOrEmpty(sucursalDestinoId) || string.IsNullOrEmpty(motivo) || cantidadTransferir <= 0)
            {
                TempData["Error"] = "Debe proporcionar Sucursal Destino, Motivo y Cantidad válida";
                return RedirectToAction(nameof(Transferir), new { id });
            }

            var item = await _context.Items.Include(i => i.Articulo).FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            if (item.Estado != "Asignado" || string.IsNullOrEmpty(item.SucursalId))
            {
                TempData["Error"] = "El item no está en condiciones de ser transferido";
                return RedirectToAction(nameof(Index));
            }

            if (item.SucursalId == sucursalDestinoId)
            {
                TempData["Error"] = "La sucursal destino debe ser diferente a la actual";
                return RedirectToAction(nameof(Transferir), new { id });
            }
            
            if (cantidadTransferir > item.Cantidad)
            {
                TempData["Error"] = "La cantidad a transferir no puede ser mayor a la cantidad actual en la sucursal";
                return RedirectToAction(nameof(Transferir), new { id });
            }

            var sucursalOrigenId = item.SucursalId;
            var sucursalOrigenNombre = _context.Sucursales.Find(sucursalOrigenId)?.Nombre;
            var sucursalDestinoNombre = _context.Sucursales.Find(sucursalDestinoId)?.Nombre;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Item itemTransferido;

                // Transferencia parcial
                if (cantidadTransferir < item.Cantidad && !item.Articulo.RequiereSerial)
                {
                    // 1. Reducir cantidad en sucursal origen
                    item.Cantidad -= cantidadTransferir;
                    _context.Update(item);
                    
                    // 2. Crear nuevo registro para la sucursal destino
                    itemTransferido = new Item
                    {
                        ArticuloId = item.ArticuloId,
                        LoteId = item.LoteId,
                        SucursalId = sucursalDestinoId,
                        Estado = "Asignado",
                        Cantidad = cantidadTransferir,
                        ResponsableEmpleado = !string.IsNullOrEmpty(responsableRecepcion) ? responsableRecepcion : item.ResponsableEmpleado,
                        FechaUltimaTransferencia = DateTime.Now,
                        Serial = null,
                        FechaGarantiaInicio = item.FechaGarantiaInicio,
                        MesesGarantia = item.MesesGarantia,
                        FechaGarantiaVencimiento = item.FechaGarantiaVencimiento
                    };
                    _context.Items.Add(itemTransferido);
                    await _context.SaveChangesAsync(); // Obtener el nuevo Id
                }
                else
                {
                    // Transferencia total
                    item.SucursalId = sucursalDestinoId;
                    item.FechaUltimaTransferencia = DateTime.Now;
                    if (!string.IsNullOrEmpty(responsableRecepcion))
                    {
                        item.ResponsableEmpleado = responsableRecepcion;
                    }
                    itemTransferido = item;
                    _context.Update(item);
                }

                // Crear registro de movimiento
                var movimiento = new MovimientoItem
                {
                    ItemId = itemTransferido.Id,
                    Cantidad = cantidadTransferir,
                    SucursalOrigenId = sucursalOrigenId,
                    SucursalDestinoId = sucursalDestinoId,
                    FechaMovimiento = DateTime.Now,
                    UsuarioResponsable = HttpContext.Session.GetString("UserName") ?? "Sistema",
                    Motivo = motivo,
                    Observaciones = observaciones,
                    ResponsableRecepcion = responsableRecepcion,
                    FechaRecepcion = !string.IsNullOrEmpty(responsableRecepcion) ? DateTime.Now : null
                };

                _context.MovimientosItem.Add(movimiento);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = $"Item(s) transferido(s) exitosamente de {sucursalOrigenNombre} a {sucursalDestinoNombre}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Error al procesar la transferencia.";
                return RedirectToAction(nameof(Transferir), new { id });
            }
        }

        // POST: Items/AsignarDesde  (non-serial: auto-pick from lote)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarDesde(int loteId, string sucursalId, string responsableEmpleado, int cantidad, string? observaciones)
        {
            if (string.IsNullOrEmpty(sucursalId) || string.IsNullOrEmpty(responsableEmpleado) || cantidad <= 0)
            {
                TempData["Error"] = "Debe proporcionar Sucursal, Responsable y una cantidad válida.";
                return RedirectToAction("Details", "Lotes", new { id = loteId });
            }

            var item = await _context.Items
                .Include(i => i.Articulo)
                .Where(i => i.LoteId == loteId && !i.Eliminado && i.Estado == "Disponible" && i.SucursalId == null)
                .FirstOrDefaultAsync();

            if (item == null)
            {
                TempData["Error"] = "No hay stock disponible en este lote.";
                return RedirectToAction("Details", "Lotes", new { id = loteId });
            }

            if (cantidad > item.Cantidad)
            {
                TempData["Error"] = $"Solo hay {item.Cantidad} unidades disponibles en este lote.";
                return RedirectToAction("Details", "Lotes", new { id = loteId });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Item itemAsignado;

                if (cantidad < item.Cantidad)
                {
                    item.Cantidad -= cantidad;
                    _context.Update(item);

                    itemAsignado = new Item
                    {
                        ArticuloId  = item.ArticuloId,
                        LoteId      = item.LoteId,
                        SucursalId  = sucursalId,
                        Estado      = "Asignado",
                        Cantidad    = cantidad,
                        ResponsableEmpleado  = responsableEmpleado,
                        FechaAsignacion      = DateTime.Now,
                        Serial               = null,
                        FechaGarantiaInicio  = item.FechaGarantiaInicio,
                        MesesGarantia        = item.MesesGarantia,
                        FechaGarantiaVencimiento = item.FechaGarantiaVencimiento
                    };
                    _context.Items.Add(itemAsignado);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    item.SucursalId         = sucursalId;
                    item.Estado             = "Asignado";
                    item.ResponsableEmpleado = responsableEmpleado;
                    item.FechaAsignacion    = DateTime.Now;
                    itemAsignado = item;
                    _context.Update(item);
                }

                _context.MovimientosItem.Add(new MovimientoItem
                {
                    ItemId              = itemAsignado.Id,
                    Cantidad            = cantidad,
                    SucursalOrigenId    = null,
                    SucursalDestinoId   = sucursalId,
                    FechaMovimiento     = DateTime.Now,
                    UsuarioResponsable  = HttpContext.Session.GetString("UserName") ?? "Sistema",
                    Motivo              = "Asignación Inicial",
                    Observaciones       = observaciones,
                    ResponsableRecepcion = responsableEmpleado,
                    FechaRecepcion      = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var sucursalNombre = (await _context.Sucursales.FindAsync(sucursalId))?.Nombre;
                TempData["Success"] = $"{cantidad} unidad(es) asignada(s) a {sucursalNombre} exitosamente.";
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Ocurrió un error al procesar la asignación.";
            }

            return RedirectToAction("Details", "Lotes", new { id = loteId });
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id && !e.Eliminado);
        }
    }
}
