using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSData.Modelos
{
    public class Compra
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Proveedor { get; set; }

        [Required]
        public decimal CostoTotal { get; set; }

        [Required]
        public DateTime FechaCompra { get; set; }

        [StringLength(100)]
        public string NumeroFactura { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; }  // "Solicitud", "Aprobada", "Completada", "Cancelada"

        [StringLength(500)]
        public string Observaciones { get; set; }

        [StringLength(500)]
        public string RutaFactura { get; set; }  // Ruta del archivo de factura escaneada

        // Nuevos campos para solicitudes
        public int? DepartamentoId { get; set; }
        public virtual Departamento? Departamento { get; set; }

        [StringLength(100)]
        public string? UsuarioSolicitante { get; set; }

        public DateTime? FechaSolicitud { get; set; }

        // Soft Delete
        public bool Eliminado { get; set; } = false;
        public DateTime? FechaEliminacion { get; set; }
        [StringLength(100)]
        public string? UsuarioEliminacion { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}

