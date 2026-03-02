using PSData.Modelos;
using System.ComponentModel.DataAnnotations;

namespace PSInventory.Web.Models.ViewModels
{
    public class LoteViewModel
    {
        public int? Id { get; set; } // Nullable for new lots

        [Required(ErrorMessage = "El artículo es requerido.")]
        public int ArticuloId { get; set; }
        public string? ArticuloNombre { get; set; } // For display purposes

        [Required(ErrorMessage = "La cantidad es requerida.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El costo unitario es requerido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo unitario debe ser mayor a 0.")]
        public decimal CostoUnitario { get; set; }

        // Seriales ingresados antes de confirmar (solo para artículos con serial)
        public List<string> Seriales { get; set; } = new List<string>();
    }

    public class CompraViewModel
    {
        public int? Id { get; set; } // Nullable for new purchases

        [Required(ErrorMessage = "El proveedor es requerido.")]
        [StringLength(200, ErrorMessage = "El proveedor no puede exceder los 200 caracteres.")]
        public string Proveedor { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de compra es requerida.")]
        public DateTime FechaCompra { get; set; } = DateTime.Now;

        [StringLength(100, ErrorMessage = "El número de factura no puede exceder los 100 caracteres.")]
        public string? NumeroFactura { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        [StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres.")]
        public string Estado { get; set; } = "Solicitud"; // Default status

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres.")]
        public string? Observaciones { get; set; }

        public string? RutaFactura { get; set; }

        // Master-detail: Collection of Lotes
        public List<LoteViewModel> Lotes { get; set; } = new List<LoteViewModel>();
    }
}