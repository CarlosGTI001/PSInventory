using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador")]
    public class SucursalesController : Controller
    {
        private readonly PSDatos _context;

        public SucursalesController(PSDatos context)
        {
            _context = context;
        }

        // GET: Sucursales
        public async Task<IActionResult> Index(string q = "", int page = 1, int pageSize = 30)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 10 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Sucursales
                .Where(s => !s.Eliminado)
                .Include(s => s.Region)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(s =>
                    s.Nombre.ToLower().Contains(term) ||
                    (s.Direccion != null && s.Direccion.ToLower().Contains(term)) ||
                    (s.Telefono != null && s.Telefono.ToLower().Contains(term)) ||
                    (s.Region != null && s.Region.Nombre.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var sucursales = await query
                .OrderBy(s => s.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Query = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            return View(sucursales);
        }

        // GET: Sucursales/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Regiones = new SelectList(await _context.Regiones.Where(r => !r.Eliminado).OrderBy(r => r.Nombre).ToListAsync(), "RegionId", "Nombre");
            return View();
        }

        // POST: Sucursales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Direccion,Telefono,RegionId,Activo")] Sucursal sucursal)
        {
            // Validar que la región sea válida
            if (sucursal.RegionId <= 0)
            {
                ModelState.AddModelError("RegionId", "Debe seleccionar una región");
            }
            else
            {
                var regionExists = await _context.Regiones
                    .AnyAsync(r => r.RegionId == sucursal.RegionId && !r.Eliminado);
                
                if (!regionExists)
                {
                    ModelState.AddModelError("RegionId", "La región seleccionada no existe o está inactiva");
                }
            }
            
            if (ModelState.IsValid)
            {
                // Generar ID automáticamente: SUC-001, SUC-002, etc.
                var ultimoId = await _context.Sucursales
                    .Where(s => s.Id.StartsWith("SUC-"))
                    .OrderByDescending(s => s.Id)
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync();
                
                int siguienteNumero = 1;
                if (!string.IsNullOrEmpty(ultimoId))
                {
                    // Extraer el número del último ID (SUC-001 -> 001 -> 1)
                    var numeroStr = ultimoId.Substring(4); // después de "SUC-"
                    if (int.TryParse(numeroStr, out int numero))
                    {
                        siguienteNumero = numero + 1;
                    }
                }
                
                // Formatear con 3 dígitos: SUC-001, SUC-002, etc.
                sucursal.Id = $"SUC-{siguienteNumero:D3}";
                
                _context.Add(sucursal);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Sucursal creada exitosamente con código {sucursal.Id}";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Regiones = new SelectList(await _context.Regiones.Where(r => !r.Eliminado).OrderBy(r => r.Nombre).ToListAsync(), "RegionId", "Nombre", sucursal.RegionId);
            return View(sucursal);
        }

        // GET: Sucursales/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursales
                .Where(s => !s.Eliminado)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sucursal == null)
            {
                return NotFound();
            }
            ViewBag.Regiones = new SelectList(await _context.Regiones.Where(r => !r.Eliminado).OrderBy(r => r.Nombre).ToListAsync(), "RegionId", "Nombre", sucursal.RegionId);
            return View(sucursal);
        }

        // POST: Sucursales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nombre,Direccion,Telefono,RegionId,Activo")] Sucursal sucursal)
        {
            if (id != sucursal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sucursal);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sucursal actualizada exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SucursalExists(sucursal.Id))
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
            ViewBag.Regiones = new SelectList(await _context.Regiones.Where(r => !r.Eliminado).OrderBy(r => r.Nombre).ToListAsync(), "RegionId", "Nombre", sucursal.RegionId);
            return View(sucursal);
        }

        // POST: Sucursales/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var sucursal = await _context.Sucursales
                .Where(s => !s.Eliminado)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sucursal == null)
            {
                return Json(new { success = false, message = "Sucursal no encontrada" });
            }

            // Verificar si tiene items asignados activos
            var tieneItems = await _context.Items.AnyAsync(i => i.SucursalId == id && !i.Eliminado);
            if (tieneItems)
            {
                return Json(new { success = false, message = "No se puede eliminar la sucursal porque tiene items asignados" });
            }

            // Soft delete
            sucursal.Eliminado = true;
            sucursal.FechaEliminacion = DateTime.Now;
            sucursal.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.Update(sucursal);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Sucursal eliminada exitosamente" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearRapido([FromBody] QuickCreateLocationInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nombre) || !input.RegionId.HasValue || input.RegionId.Value <= 0)
            {
                return Json(new { success = false, message = "Debe ingresar nombre y región para la sucursal." });
            }

            var region = await _context.Regiones
                .Where(r => !r.Eliminado && r.RegionId == input.RegionId.Value)
                .FirstOrDefaultAsync();
            if (region == null)
            {
                return Json(new { success = false, message = "La región seleccionada no es válida." });
            }

            var nombre = input.Nombre.Trim();
            var existe = await _context.Sucursales
                .AnyAsync(s => !s.Eliminado && s.RegionId == input.RegionId.Value && s.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                return Json(new { success = false, message = "Ya existe una sucursal con ese nombre en esta región." });
            }

            var ultimoId = await _context.Sucursales
                .Where(s => s.Id.StartsWith("SUC-"))
                .OrderByDescending(s => s.Id)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
            var siguienteNumero = 1;
            if (!string.IsNullOrEmpty(ultimoId))
            {
                var numeroStr = ultimoId.Substring(4);
                if (int.TryParse(numeroStr, out int numero))
                {
                    siguienteNumero = numero + 1;
                }
            }

            var sucursal = new Sucursal
            {
                Id = $"SUC-{siguienteNumero:D3}",
                Nombre = nombre,
                RegionId = input.RegionId.Value,
                Activo = true
            };
            _context.Sucursales.Add(sucursal);
            await _context.SaveChangesAsync();

            return Json(new { success = true, option = new { value = sucursal.Id, text = sucursal.Nombre } });
        }

        private bool SucursalExists(string id)
        {
            return _context.Sucursales.Any(e => e.Id == id && !e.Eliminado);
        }
    }
}
