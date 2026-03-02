using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSData.Modelos
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string? Serial { get; set; } // Opcional para items sin serial

        [Required]
        public int Cantidad { get; set; } = 1; // Por defecto 1 para items con serial

        [Required]
        public int ArticuloId { get; set; }
        [ForeignKey("ArticuloId")]
        public virtual Articulo Articulo { get; set; }

        public string SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; }  // "Disponible", "En Uso", "En Reparación", "Dañado", "Dado de Baja"

        [Required]
        public int LoteId { get; set; }
        [ForeignKey("LoteId")]
        public virtual Lote Lote { get; set; }

        [StringLength(200)]
        public string Ubicacion { get; set; }

        [StringLength(200)]
        public string ResponsableEmpleado { get; set; }

        public DateTime? FechaAsignacion { get; set; }

        public DateTime? FechaUltimaTransferencia { get; set; }

        public DateTime? FechaGarantiaInicio { get; set; }

        public int? MesesGarantia { get; set; }

        public DateTime? FechaGarantiaVencimiento { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }

        // Soft Delete
        public bool Eliminado { get; set; } = false;
        public DateTime? FechaEliminacion { get; set; }
        [StringLength(100)]
        public string? UsuarioEliminacion { get; set; }

        [NotMapped]
        public bool GarantiaVigente => FechaGarantiaVencimiento.HasValue && FechaGarantiaVencimiento.Value > DateTime.Now;

        [NotMapped]
        public bool Disponible => Estado == "Disponible" && SucursalId == null;
    }
}