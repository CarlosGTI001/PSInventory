using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;

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
        public async Task<IActionResult> Index()
        {
            var compras = await _context.Compras
                .Where(c => !c.Eliminado)
                .Include(c => c.Departamento)
                .Include(c => c.Items)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
            return View(compras);
        }

        // GET: Compras/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Departamentos = await _context.Departamentos
                .Where(d => d.Activo && !d.Eliminado)
                .OrderBy(d => d.Nombre)
                .ToListAsync();
                
            return View(new Compra { 
                FechaCompra = DateTime.Now,
                FechaSolicitud = DateTime.Now,
                Estado = "Solicitud",
                UsuarioSolicitante = User.Identity?.Name
            });
        }

        // POST: Compras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FechaCompra,FechaSolicitud,Proveedor,CostoTotal,NumeroFactura,Estado,Observaciones,DepartamentoId,UsuarioSolicitante")] Compra compra, IFormFile? facturaFile)
        {
            if (ModelState.IsValid)
            {
                // Guardar archivo de factura si se proporcionó
                if (facturaFile != null && facturaFile.Length > 0)
                {
                    // Validar tamaño máximo (10 MB)
                    const long maxFileSize = 10 * 1024 * 1024;
                    if (facturaFile.Length > maxFileSize)
                    {
                        ModelState.AddModelError("", "El archivo no debe superar los 10 MB");
                        return View(compra);
                    }
                    
                    var extension = Path.GetExtension(facturaFile.FileName).ToLowerInvariant();
                    var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                    
                    if (allowedExtensions.Contains(extension))
                    {
                        var fileName = $"factura_{DateTime.Now:yyyyMMddHHmmss}_{Path.GetRandomFileName()}{extension}";
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "facturas");
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await facturaFile.CopyToAsync(stream);
                        }
                        
                        compra.RutaFactura = $"/uploads/facturas/{fileName}";
                    }
                    else
                    {
                        ModelState.AddModelError("", "Solo se permiten archivos PDF, JPG, JPEG o PNG");
                        return View(compra);
                    }
                }
                
                _context.Add(compra);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Compra registrada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(compra);
        }

        // GET: Compras/Edit/5
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaCompra,Proveedor,CostoTotal,NumeroFactura,Estado,Observaciones,RutaFactura")] Compra compra, IFormFile? facturaFile)
        {
            if (id != compra.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Guardar nuevo archivo de factura si se proporcionó
                    if (facturaFile != null && facturaFile.Length > 0)
                    {
                        // Validar tamaño máximo (10 MB)
                        const long maxFileSize = 10 * 1024 * 1024;
                        if (facturaFile.Length > maxFileSize)
                        {
                            ModelState.AddModelError("", "El archivo no debe superar los 10 MB");
                            return View(compra);
                        }
                        
                        var extension = Path.GetExtension(facturaFile.FileName).ToLowerInvariant();
                        var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                        
                        if (allowedExtensions.Contains(extension))
                        {
                            // Eliminar archivo anterior si existe
                            if (!string.IsNullOrEmpty(compra.RutaFactura))
                            {
                                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, compra.RutaFactura.TrimStart('/'));
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }
                            }
                            
                            var fileName = $"factura_{DateTime.Now:yyyyMMddHHmmss}_{Path.GetRandomFileName()}{extension}";
                            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "facturas");
                            var filePath = Path.Combine(uploadsFolder, fileName);
                            
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await facturaFile.CopyToAsync(stream);
                            }
                            
                            compra.RutaFactura = $"/uploads/facturas/{fileName}";
                        }
                        else
                        {
                            ModelState.AddModelError("", "Solo se permiten archivos PDF, JPG, JPEG o PNG");
                            return View(compra);
                        }
                    }
                    
                    _context.Update(compra);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Compra actualizada exitosamente";
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
                .Include(c => c.Items)
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

            // Verificar si tiene items asociados activos
            var tieneItems = await _context.Items.AnyAsync(i => i.CompraId == id && !i.Eliminado);
            if (tieneItems)
            {
                return Json(new { success = false, message = "No se puede eliminar la compra porque tiene items asociados" });
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
