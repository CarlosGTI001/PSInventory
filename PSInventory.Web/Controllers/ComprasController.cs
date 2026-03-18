using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador", "Jefe")]
    public class ComprasController : Controller
    {
        private readonly PSDatos _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComprasController(PSDatos context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Compras
        public async Task<IActionResult> Index(string q = "", int page = 1, int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 10 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Compras
                .Where(c => !c.Eliminado)
                .Include(c => c.Departamento)
                .Include(c => c.Lotes)
                    .ThenInclude(l => l.Items)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(c =>
                    (c.Proveedor != null && c.Proveedor.ToLower().Contains(term)) ||
                    (c.NumeroFactura != null && c.NumeroFactura.ToLower().Contains(term)) ||
                    (c.Estado != null && c.Estado.ToLower().Contains(term)) ||
                    (c.Departamento != null && c.Departamento.Nombre.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var compras = await query
                .OrderByDescending(c => c.FechaCompra)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Query = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            return View(compras);
        }

        // GET: Compras/Create
        public IActionResult Create()
        {
            return View(new Compra
            {
                FechaCompra = DateTime.Now,
                Estado = "Solicitud",
                UsuarioSolicitante = User.Identity?.Name // Default to current user
            });
        }

        // POST: Compras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Proveedor,FechaCompra,NumeroFactura,Estado,Observaciones,RutaFactura,UsuarioSolicitante")] Compra compra, IFormFile? facturaFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (facturaFile != null && facturaFile.Length > 0)
                {
                    var extension = Path.GetExtension(facturaFile.FileName).ToLowerInvariant();
                    var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("facturaFile", "Solo se permiten archivos PDF, JPG, JPEG o PNG.");
                        return View(compra);
                    }
                    if (facturaFile.Length > 10 * 1024 * 1024) // Max 10MB
                    {
                        ModelState.AddModelError("facturaFile", "El archivo no debe superar los 10 MB.");
                        return View(compra);
                    }

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "facturas");
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await facturaFile.CopyToAsync(stream);
                    }
                    compra.RutaFactura = $"/uploads/facturas/{fileName}";
                }

                _context.Add(compra);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Compra creada exitosamente. Ahora puedes agregar lotes.";
                return RedirectToAction("Details", new { id = compra.Id }); // Redirect to details to add lots
            }
            return View(compra);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound();
            }
            return View(compra);
        }

        // POST: Compras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Proveedor,FechaCompra,NumeroFactura,Estado,Observaciones,RutaFactura,UsuarioSolicitante")] Compra compra, IFormFile? facturaFile)
        {
            if (id != compra.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload
                    if (facturaFile != null && facturaFile.Length > 0)
                    {
                        var extension = Path.GetExtension(facturaFile.FileName).ToLowerInvariant();
                        var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("facturaFile", "Solo se permiten archivos PDF, JPG, JPEG o PNG.");
                            return View(compra);
                        }
                        if (facturaFile.Length > 10 * 1024 * 1024) // Max 10MB
                        {
                            ModelState.AddModelError("facturaFile", "El archivo no debe superar los 10 MB.");
                            return View(compra);
                        }

                        // Delete old file if exists
                        if (!string.IsNullOrEmpty(compra.RutaFactura))
                        {
                            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, compra.RutaFactura.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                        
                        var fileName = $"{Guid.NewGuid()}{extension}";
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "facturas");
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await facturaFile.CopyToAsync(stream);
                        }
                        compra.RutaFactura = $"/uploads/facturas/{fileName}";
                    }

                    _context.Update(compra);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Compra actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompraExists(compra.Id))
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
            return View(compra);
        }

        // GET: Compras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Lotes)
                .ThenInclude(l => l.Items)
                .ThenInclude(i => i.Articulo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // POST: Compras/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var compra = await _context.Compras
                .Where(c => !c.Eliminado)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (compra == null)
            {
                return Json(new { success = false, message = "Compra no encontrada" });
            }

            // Verificar si tiene lotes asociados activos
            var tieneLotes = await _context.Lotes.AnyAsync(l => l.CompraId == id && l.Items.Any(i => !i.Eliminado));
            if (tieneLotes)
            {
                return Json(new { success = false, message = "No se puede eliminar la compra porque tiene lotes con items asociados" });
            }

            // Soft delete
            compra.Eliminado = true;
            compra.FechaEliminacion = DateTime.Now;
            compra.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.Update(compra);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Compra eliminada exitosamente" });
        }

        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.Id == id && !e.Eliminado);
        }
    }
}
