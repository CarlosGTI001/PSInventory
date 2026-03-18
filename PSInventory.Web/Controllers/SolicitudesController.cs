using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;
using PSInventory.Web.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace PSInventory.Web.Controllers
{
    [RequireAuth]
    public class SolicitudesController : Controller
    {
        private readonly PSDatos _context;
        private string UsuarioActual => HttpContext.Session.GetString("UserName") ?? "Sistema";

        public SolicitudesController(PSDatos context) => _context = context;

        // GET: Solicitudes
        public async Task<IActionResult> Index(string q = "", int page = 1, int pageSize = 25)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 10 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.SolicitudesCompra
                .Where(s => !s.Eliminado)
                .Include(s => s.Detalles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(s =>
                    (s.Titulo != null && s.Titulo.ToLower().Contains(term)) ||
                    (s.Solicitante != null && s.Solicitante.ToLower().Contains(term)) ||
                    (s.Estado != null && s.Estado.ToLower().Contains(term)) ||
                    (s.Observaciones != null && s.Observaciones.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var solicitudes = await query
                .OrderByDescending(s => s.FechaSolicitud)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Query = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            return View(solicitudes);
        }

        // GET: Solicitudes/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Articulos = await _context.Articulos
                .Where(a => !a.Eliminado)
                .OrderBy(a => a.Marca).ThenBy(a => a.Modelo)
                .Select(a => new { a.Id, Nombre = a.Marca + " " + a.Modelo })
                .ToListAsync();
            return View();
        }

        // POST: Solicitudes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string titulo,
            string? observaciones,
            List<int?> articuloIds,
            List<string?> descripciones,
            List<int> cantidades,
            List<string?> obsDetalles)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "El título es requerido.";
                return RedirectToAction(nameof(Create));
            }

            bool hayLineas = false;
            for (int i = 0; i < articuloIds.Count; i++)
            {
                if (articuloIds[i].HasValue || !string.IsNullOrWhiteSpace(descripciones.ElementAtOrDefault(i)))
                { hayLineas = true; break; }
            }
            if (!hayLineas)
            {
                TempData["Error"] = "Agrega al menos un artículo a la solicitud.";
                return RedirectToAction(nameof(Create));
            }

            var solicitud = new SolicitudCompra
            {
                Titulo         = titulo.Trim(),
                Solicitante    = UsuarioActual,
                FechaSolicitud = DateTime.Now,
                Estado         = "Pendiente",
                Observaciones  = observaciones?.Trim()
            };
            _context.SolicitudesCompra.Add(solicitud);
            await _context.SaveChangesAsync();

            for (int i = 0; i < articuloIds.Count; i++)
            {
                var artId = articuloIds.ElementAtOrDefault(i);
                var desc  = descripciones.ElementAtOrDefault(i)?.Trim();
                var cant  = cantidades.ElementAtOrDefault(i);
                if (cant < 1) cant = 1;

                if (!artId.HasValue && string.IsNullOrWhiteSpace(desc)) continue;

                _context.DetallesSolicitudCompra.Add(new DetalleSolicitudCompra
                {
                    SolicitudId      = solicitud.Id,
                    ArticuloId       = artId,
                    DescripcionLibre = artId.HasValue ? null : desc,
                    Cantidad         = cant,
                    Observaciones    = obsDetalles.ElementAtOrDefault(i)?.Trim()
                });
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = "Solicitud creada exitosamente.";
            return RedirectToAction(nameof(Details), new { id = solicitud.Id });
        }

        // GET: Solicitudes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var solicitud = await _context.SolicitudesCompra
                .Where(s => !s.Eliminado && s.Id == id)
                .Include(s => s.Detalles).ThenInclude(d => d.Articulo)
                .FirstOrDefaultAsync();
            if (solicitud == null) return NotFound();
            return View(solicitud);
        }

        // POST: Solicitudes/CambiarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado, string? notasRevision)
        {
            var solicitud = await _context.SolicitudesCompra.FindAsync(id);
            if (solicitud == null || solicitud.Eliminado) return NotFound();

            var estadosValidos = new[] { "Pendiente", "En Revisión", "Aprobada", "Rechazada" };
            if (!estadosValidos.Contains(nuevoEstado)) return BadRequest();

            solicitud.Estado        = nuevoEstado;
            solicitud.UsuarioRevisor = UsuarioActual;
            solicitud.FechaRevision  = DateTime.Now;
            solicitud.NotasRevision  = notasRevision?.Trim();

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Estado actualizado a «{nuevoEstado}».";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Solicitudes/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var solicitud = await _context.SolicitudesCompra.FindAsync(id);
            if (solicitud == null) return NotFound();
            if (solicitud.Estado != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden eliminar solicitudes en estado Pendiente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            solicitud.Eliminado = true;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Solicitud eliminada.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Solicitudes/DescargarPdf/5
        public async Task<IActionResult> DescargarPdf(int? id)
        {
            if (id == null) return NotFound();
            var solicitud = await _context.SolicitudesCompra
                .Where(s => !s.Eliminado && s.Id == id)
                .Include(s => s.Detalles).ThenInclude(d => d.Articulo)
                .FirstOrDefaultAsync();
            if (solicitud == null) return NotFound();

            var usuario = UsuarioActual;
            var estadoColor = solicitud.Estado switch {
                "Aprobada"    => "#4caf50",
                "Rechazada"   => "#d32f2f",
                "En Revisión" => "#e65100",
                _             => "#047394"
            };

            var pdf = Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(c => PdfReportService.GenerarHeader(c, "Solicitud de Compra", usuario));

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        // Info de la solicitud
                        col.Item().PaddingBottom(12).Row(row =>
                        {
                            row.RelativeItem().Column(info =>
                            {
                                info.Item().Text(solicitud.Titulo)
                                    .FontSize(14).Bold().FontColor("#047394");
                                info.Item().PaddingTop(4).Text($"Solicitud #{solicitud.Id}")
                                    .FontSize(10).FontColor(Colors.Grey.Darken2);
                            });
                            row.ConstantItem(130).Column(info =>
                            {
                                info.Item().AlignRight()
                                    .Background(estadoColor).Padding(6)
                                    .Text(solicitud.Estado).FontSize(11).Bold().FontColor(Colors.White);
                                info.Item().AlignRight().PaddingTop(4)
                                    .Text($"Solicitado: {solicitud.FechaSolicitud:dd/MM/yyyy HH:mm}")
                                    .FontSize(9).FontColor(Colors.Grey.Darken2);
                                info.Item().AlignRight()
                                    .Text($"Solicitante: {solicitud.Solicitante}")
                                    .FontSize(9).FontColor(Colors.Grey.Darken2);
                            });
                        });

                        col.Item().BorderBottom(1).BorderColor("#047394").PaddingBottom(2).Text("");

                        // Tabla de artículos
                        col.Item().PaddingTop(12).Text("Artículos Solicitados")
                            .FontSize(11).Bold().FontColor(Colors.Grey.Darken3);

                        col.Item().PaddingTop(8).Element(c =>
                            PdfReportService.GenerarTablaSimple(c,
                                new List<string> { "#", "Artículo / Descripción", "Cantidad", "Observación" },
                                solicitud.Detalles.Select((d, i) => new List<string>
                                {
                                    (i + 1).ToString(),
                                    d.Articulo != null ? $"{d.Articulo.Marca} {d.Articulo.Modelo}" : (d.DescripcionLibre ?? ""),
                                    d.Cantidad.ToString(),
                                    d.Observaciones ?? "—"
                                }).ToList()
                            )
                        );

                        // Total
                        col.Item().PaddingTop(10).AlignRight()
                            .Text($"Total de artículos: {solicitud.Detalles.Sum(d => d.Cantidad)}")
                            .FontSize(10).Bold().FontColor("#047394");

                        // Observaciones generales
                        if (!string.IsNullOrEmpty(solicitud.Observaciones))
                        {
                            col.Item().PaddingTop(16).Column(obs =>
                            {
                                obs.Item().Text("Observaciones Generales")
                                    .FontSize(11).Bold().FontColor(Colors.Grey.Darken3);
                                obs.Item().PaddingTop(6).Background(Colors.Grey.Lighten4).Padding(10)
                                    .Text(solicitud.Observaciones).FontSize(9).FontColor(Colors.Grey.Darken2);
                            });
                        }

                        // Notas de revisión
                        if (!string.IsNullOrEmpty(solicitud.NotasRevision))
                        {
                            col.Item().PaddingTop(16).Column(rev =>
                            {
                                rev.Item().Text("Notas de Revisión")
                                    .FontSize(11).Bold().FontColor(Colors.Grey.Darken3);
                                rev.Item().PaddingTop(6).Background(Colors.Grey.Lighten4).Padding(10).Column(inner =>
                                {
                                    inner.Item().Text(solicitud.NotasRevision!)
                                        .FontSize(9).FontColor(Colors.Grey.Darken2);
                                    inner.Item().PaddingTop(4)
                                        .Text($"Revisado por: {solicitud.UsuarioRevisor} · {solicitud.FechaRevision:dd/MM/yyyy HH:mm}")
                                        .FontSize(8).FontColor(Colors.Grey.Medium);
                                });
                            });
                        }

                        // Espacio para firmas
                        col.Item().PaddingTop(50).Row(row =>
                        {
                            row.RelativeItem().Column(f =>
                            {
                                f.Item().Width(160).BorderBottom(1).BorderColor(Colors.Grey.Medium).Text("");
                                f.Item().PaddingTop(4).Text("Solicitante").FontSize(9).FontColor(Colors.Grey.Darken2);
                            });
                            row.RelativeItem().Column(f =>
                            {
                                f.Item().AlignRight().Width(160).BorderBottom(1).BorderColor(Colors.Grey.Medium).Text("");
                                f.Item().AlignRight().PaddingTop(4).Text("Aprobado por").FontSize(9).FontColor(Colors.Grey.Darken2);
                            });
                        });
                    });

                    page.Footer().Element(c => PdfReportService.GenerarFooter(c, usuario));
                });
            }).GeneratePdf();

            var fileName = $"Solicitud_{solicitud.Id}_{solicitud.FechaSolicitud:yyyyMMdd}.pdf";
            return File(pdf, "application/pdf", fileName);
        }
    }
}
