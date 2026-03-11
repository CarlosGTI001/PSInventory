using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSInventory.Web.Filters;
using System.Linq;

namespace PSInventory.Web.Controllers
{
    [RequireAuth]
    public class HomeController : Controller
    {
        private readonly PSDatos _context;

        public HomeController(PSDatos context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Verificar autenticación
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["UserName"] = userName;

            // Estadísticas para el dashboard (usando Sum(Cantidad) para soportar items sin serial)
            ViewBag.TotalItems        = _context.Items.Where(i => !i.Eliminado).Sum(i => (int?)i.Cantidad) ?? 0;
            ViewBag.ItemsDisponibles  = _context.Items.Where(i => !i.Eliminado && i.Estado == "Disponible").Sum(i => (int?)i.Cantidad) ?? 0;
            ViewBag.ItemsAsignados    = _context.Items.Where(i => !i.Eliminado && i.Estado == "Asignado").Sum(i => (int?)i.Cantidad) ?? 0;
            ViewBag.TotalSucursales   = _context.Sucursales.Count(s => s.Activo);
            ViewBag.TotalArticulos    = _context.Articulos.Count();
            ViewBag.ComprasPendientes = _context.Compras.Count(c => c.Estado == "Pendiente");
            
            // Items con stock bajo (comparar suma de cantidades contra stock mínimo)
            var articulosStockBajo = _context.Articulos
                .Where(a => a.StockMinimo > 0)
                .Select(a => new
                {
                    Articulo   = a,
                    StockActual = _context.Items
                        .Where(i => i.ArticuloId == a.Id && !i.Eliminado && i.Estado == "Disponible")
                        .Sum(i => (int?)i.Cantidad) ?? 0
                })
                .AsEnumerable()
                .Where(x => x.StockActual < x.Articulo.StockMinimo)
                .ToList();

            ViewBag.AlertasStockBajo = articulosStockBajo.Count();

            return View();
        }

        // API para Chart.js - Items por Estado
        [HttpGet]
        public IActionResult GetItemsPorEstado()
        {
            var data = _context.Items
                .Where(i => !i.Eliminado)
                .GroupBy(i => i.Estado)
                .Select(g => new { estado = g.Key, cantidad = g.Sum(i => i.Cantidad) })
                .ToList();

            var labels = data.Select(d => d.estado).ToList();
            var valores = data.Select(d => d.cantidad).ToList();
            var colores = labels.Select(l => l switch
            {
                "Disponible" => "#10b981",
                "Asignado" => "#047394",
                "En Reparación" => "#f59e0b",
                "Baja" => "#ef4444",
                _ => "#6b7280"
            }).ToList();

            return Json(new
            {
                labels = labels,
                datasets = new[]
                {
                    new
                    {
                        data = valores,
                        backgroundColor = colores,
                        borderWidth = 0
                    }
                }
            });
        }

        // API para Chart.js - Items por Categoría
        [HttpGet]
        public IActionResult GetItemsPorCategoria()
        {
            var data = _context.Items
                .Where(i => !i.Eliminado)
                .Include(i => i.Articulo)
                .ThenInclude(a => a.Categoria)
                .GroupBy(i => i.Articulo.Categoria.Nombre)
                .Select(g => new { categoria = g.Key, cantidad = g.Sum(i => i.Cantidad) })
                .OrderByDescending(x => x.cantidad)
                .Take(5)
                .ToList();

            var labels = data.Select(d => d.categoria).ToList();
            var valores = data.Select(d => d.cantidad).ToList();

            return Json(new
            {
                labels = labels,
                datasets = new[]
                {
                    new
                    {
                        label = "Items",
                        data = valores,
                        backgroundColor = "#047394",
                        borderColor = "#035a72",
                        borderWidth = 1
                    }
                }
            });
        }

        // API para Chart.js - Items por Sucursal
        [HttpGet]
        public IActionResult GetItemsPorSucursal()
        {
            var data = _context.Items
                .Where(i => !i.Eliminado && i.SucursalId != null)
                .Include(i => i.Sucursal)
                .GroupBy(i => i.Sucursal.Nombre)
                .Select(g => new { sucursal = g.Key, cantidad = g.Sum(i => i.Cantidad) })
                .OrderByDescending(x => x.cantidad)
                .ToList();

            var labels = data.Select(d => d.sucursal).ToList();
            var valores = data.Select(d => d.cantidad).ToList();

            return Json(new
            {
                labels = labels,
                datasets = new[]
                {
                    new
                    {
                        label = "Items Asignados",
                        data = valores,
                        backgroundColor = "#ff5c00",
                        borderColor = "#cc4a00",
                        borderWidth = 1,
                        barThickness = 30
                    }
                }
            });
        }

        // API para Chart.js - Compras por Mes
        [HttpGet]
        public async Task<IActionResult> GetComprasPorMes()
        {
            var fechaInicio = DateTime.Now.AddMonths(-5).Date;
            
            // SQLite no soporta Sum(decimal); traer a cliente y agregar
            var comprasFiltradas = await _context.Compras
                .Where(c => c.FechaCompra >= fechaInicio)
                .Select(c => new { c.FechaCompra, c.CostoTotal })
                .ToListAsync();

            var data = comprasFiltradas
                .GroupBy(c => new { c.FechaCompra.Year, c.FechaCompra.Month })
                .Select(g => new
                {
                    anio = g.Key.Year,
                    mes = g.Key.Month,
                    cantidad = g.Count(),
                    monto = (double)g.Sum(c => c.CostoTotal)
                })
                .OrderBy(x => x.anio).ThenBy(x => x.mes)
                .ToList();

            var labels = data.Select(d => $"{GetNombreMes(d.mes)} {d.anio}").ToList();
            var cantidades = data.Select(d => d.cantidad).ToList();
            var montos = data.Select(d => (double)d.monto).ToList();

            return Json(new
            {
                labels = labels,
                datasets = new object[]
                {
                    new
                    {
                        label = "Cantidad de Compras",
                        data = cantidades,
                        borderColor = "#047394",
                        backgroundColor = "rgba(4, 115, 148, 0.1)",
                        tension = 0.4,
                        fill = true,
                        yAxisID = "y"
                    },
                    new
                    {
                        label = "Monto Total ($)",
                        data = montos,
                        borderColor = "#ff5c00",
                        backgroundColor = "rgba(255, 92, 0, 0.1)",
                        tension = 0.4,
                        fill = true,
                        yAxisID = "y1"
                    }
                }
            });
        }

        private string GetNombreMes(int mes)
        {
            return mes switch
            {
                1 => "Ene",
                2 => "Feb",
                3 => "Mar",
                4 => "Abr",
                5 => "May",
                6 => "Jun",
                7 => "Jul",
                8 => "Ago",
                9 => "Sep",
                10 => "Oct",
                11 => "Nov",
                12 => "Dic",
                _ => ""
            };
        }
    }
}
