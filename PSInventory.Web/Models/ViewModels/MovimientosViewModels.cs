using System;
using System.ComponentModel.DataAnnotations;

namespace PSInventory.Web.Models.ViewModels
{
    public class MovimientoEditViewModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemCodigo { get; set; } = string.Empty;
        public string ArticuloNombre { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public DateTime FechaMovimiento { get; set; }
        public bool EsUltimoMovimientoDelItem { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una sucursal destino.")]
        [StringLength(50)]
        public string SucursalDestinoId { get; set; } = string.Empty;

        [StringLength(200)]
        public string ResponsableRecepcion { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Observaciones { get; set; } = string.Empty;

        public DateTime? FechaRecepcion { get; set; }
    }
}
