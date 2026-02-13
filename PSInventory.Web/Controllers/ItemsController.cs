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
                .Include(i => i.Compra)
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
            
            ViewBag.Compras = new SelectList(await _context.Compras
                .Where(c => !c.Eliminado)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync(), "Id", "Proveedor");
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
        public async Task<IActionResult> Create([Bind("Serial,ArticuloId,CompraId,SucursalId,Costo,Estado,MesesGarantia,FechaGarantiaInicio,Observaciones")] Item item)
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
            
            ViewBag.Compras = new SelectList(await _context.Compras.OrderByDescending(c => c.FechaCompra).ToListAsync(), "Id", "Proveedor", item.CompraId);
            ViewBag.Sucursales = new SelectList(await _context.Sucursales.OrderBy(s => s.Nombre).ToListAsync(), "Id", "Nombre", item.SucursalId);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(string id)
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
            
            ViewBag.Compras = new SelectList(await _context.Compras.OrderByDescending(c => c.FechaCompra).ToListAsync(), "Id", "Proveedor", item.CompraId);
            ViewBag.Sucursales = new SelectList(await _context.Sucursales.OrderBy(s => s.Nombre).ToListAsync(), "Id", "Nombre", item.SucursalId);
            return View(item);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Serial,ArticuloId,CompraId,SucursalId,Costo,Estado,MesesGarantia,FechaGarantiaInicio,Observaciones")] Item item)
        {
            if (id != item.Serial)
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
                    if (!ItemExists(item.Serial))
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
            
            ViewBag.Compras = new SelectList(await _context.Compras.OrderByDescending(c => c.FechaCompra).ToListAsync(), "Id", "Proveedor", item.CompraId);
            ViewBag.Sucursales = new SelectList(await _context.Sucursales.OrderBy(s => s.Nombre).ToListAsync(), "Id", "Nombre", item.SucursalId);
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _context.Items
                .Where(i => !i.Eliminado)
                .FirstOrDefaultAsync(i => i.Serial == id);
            if (item == null)
            {
                return Json(new { success = false, message = "Item no encontrado" });
            }

            // Soft delete
            item.Eliminado = true;
            item.FechaEliminacion = DateTime.Now;
            item.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.Update(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Item eliminado exitosamente" });
        }

        // GET: Items/Asignar/5
        public async Task<IActionResult> Asignar(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Articulo)
                .ThenInclude(a => a.Categoria)
                .Include(i => i.Compra)
                .FirstOrDefaultAsync(m => m.Serial == id);

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
        public async Task<IActionResult> Asignar(string id, string sucursalId, string responsableEmpleado, string observaciones)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(sucursalId) || string.IsNullOrEmpty(responsableEmpleado))
            {
                TempData["Error"] = "Debe proporcionar Sucursal y Responsable";
                return RedirectToAction(nameof(Asignar), new { id });
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            if (item.Estado != "Disponible" || !string.IsNullOrEmpty(item.SucursalId))
            {
                TempData["Error"] = "El item ya no está disponible para asignación";
                return RedirectToAction(nameof(Index));
            }

            // Actualizar item
            item.SucursalId = sucursalId;
            item.Estado = "Asignado";
            item.ResponsableEmpleado = responsableEmpleado;
            item.FechaAsignacion = DateTime.Now;

            // Crear registro de movimiento
            var movimiento = new MovimientoItem
            {
                ItemSerial = item.Serial,
                SucursalOrigenId = null, // Desde almacén (sin origen)
                SucursalDestinoId = sucursalId,
                FechaMovimiento = DateTime.Now,
                UsuarioResponsable = User.Identity?.Name ?? "Sistema",
                Motivo = "Asignación Inicial",
                Observaciones = observaciones,
                ResponsableRecepcion = responsableEmpleado,
                FechaRecepcion = DateTime.Now
            };

            _context.MovimientosItem.Add(movimiento);
            _context.Update(item);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Item {item.Serial} asignado exitosamente a {_context.Sucursales.Find(sucursalId)?.Nombre}";
            return RedirectToAction(nameof(Index));
        }

        // GET: Items/Transferir/5
        public async Task<IActionResult> Transferir(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Articulo)
                .ThenInclude(a => a.Categoria)
                .Include(i => i.Sucursal)
                .FirstOrDefaultAsync(m => m.Serial == id);

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
        public async Task<IActionResult> Transferir(string id, string sucursalDestinoId, string responsableRecepcion, string motivo, string observaciones)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(sucursalDestinoId) || string.IsNullOrEmpty(motivo))
            {
                TempData["Error"] = "Debe proporcionar Sucursal Destino y Motivo de Transferencia";
                return RedirectToAction(nameof(Transferir), new { id });
            }

            var item = await _context.Items.FindAsync(id);
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

            var sucursalOrigenId = item.SucursalId;
            var sucursalOrigenNombre = _context.Sucursales.Find(sucursalOrigenId)?.Nombre;
            var sucursalDestinoNombre = _context.Sucursales.Find(sucursalDestinoId)?.Nombre;

            // Actualizar item
            item.SucursalId = sucursalDestinoId;
            item.FechaUltimaTransferencia = DateTime.Now;
            if (!string.IsNullOrEmpty(responsableRecepcion))
            {
                item.ResponsableEmpleado = responsableRecepcion;
            }

            // Crear registro de movimiento
            var movimiento = new MovimientoItem
            {
                ItemSerial = item.Serial,
                SucursalOrigenId = sucursalOrigenId,
                SucursalDestinoId = sucursalDestinoId,
                FechaMovimiento = DateTime.Now,
                UsuarioResponsable = User.Identity?.Name ?? "Sistema",
                Motivo = motivo,
                Observaciones = observaciones,
                ResponsableRecepcion = responsableRecepcion,
                FechaRecepcion = !string.IsNullOrEmpty(responsableRecepcion) ? DateTime.Now : null
            };

            _context.MovimientosItem.Add(movimiento);
            _context.Update(item);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Item {item.Serial} transferido exitosamente de {sucursalOrigenNombre} a {sucursalDestinoNombre}";
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(string serial)
        {
            return _context.Items.Any(e => e.Serial == serial && !e.Eliminado);
        }
    }
}
