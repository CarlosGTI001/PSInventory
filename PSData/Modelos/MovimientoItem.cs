using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSData.Modelos
{
    public class MovimientoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        [Required]
        public int Cantidad { get; set; } = 1; // Cantidad movida

        [StringLength(50)]
        public string SucursalOrigenId { get; set; }
        [ForeignKey("SucursalOrigenId")]
        public virtual Sucursal SucursalOrigen { get; set; }

        [Required]
        [StringLength(50)]
        public string SucursalDestinoId { get; set; }
        [ForeignKey("SucursalDestinoId")]
        public virtual Sucursal SucursalDestino { get; set; }

        [Required]
        public DateTime FechaMovimiento { get; set; }

        [Required]
        [StringLength(200)]
        public string UsuarioResponsable { get; set; }

        [Required]
        [StringLength(200)]
        public string Motivo { get; set; }  // "Asignación Inicial", "Transferencia", "Retorno para Reparación", "Devolución a Almacén"

        [StringLength(1000)]
        public string Observaciones { get; set; }

        [StringLength(200)]
        public string ResponsableRecepcion { get; set; }

        public DateTime? FechaRecepcion { get; set; }
    }
}
