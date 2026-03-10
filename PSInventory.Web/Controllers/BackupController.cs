using Microsoft.AspNetCore.Mvc;
using PSData.Backup;
using PSData.Datos;
using PSInventory.Web.Filters;

namespace PSInventory.Web.Controllers
{
    [RequireAuth]
    [AuthorizeRole("Administrador")]
    public class BackupController : Controller
    {
        private readonly BackupExportService _backupService;

        public BackupController(PSDatos context)
        {
            _backupService = new BackupExportService(context);
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Exporta todos los datos a un archivo JSON descargable.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ExportarJson()
        {
            var bytes = await _backupService.ExportToJsonAsync();
            var nombre = $"PSInventory_Export_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            return File(bytes, "application/json", nombre);
        }

        /// <summary>
        /// Importa datos desde un archivo JSON.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ImportarJson(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo para importar.";
                return RedirectToAction(nameof(Index));
            }

            if (!archivo.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "El archivo debe ser de tipo JSON.";
                return RedirectToAction(nameof(Index));
            }

            await using var stream = archivo.OpenReadStream();
            var result = await _backupService.ImportFromJsonAsync(stream);

            if (result.Success)
            {
                TempData["Success"] = "Los datos se importaron correctamente.";
            }
            else
            {
                TempData["Error"] = $"Error al importar: {result.ErrorMessage}";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Crea un backup nativo de SQL Server (.bak).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CrearBackupSql([FromForm] string? rutaDestino)
        {
            var carpeta = string.IsNullOrWhiteSpace(rutaDestino)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PSInventoryBackups")
                : rutaDestino.Trim();

            try
            {
                Directory.CreateDirectory(carpeta);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudo crear la carpeta: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }

            var result = await _backupService.CreateSqlBackupAsync(carpeta);

            if (result.Success)
            {
                TempData["Success"] = $"Backup creado correctamente en: {result.BackupPath}";
            }
            else
            {
                TempData["Error"] = $"Error al crear backup: {result.ErrorMessage}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
