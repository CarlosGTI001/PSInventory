using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSData.Modelos
{
    public class Sucursal
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string Telefono { get; set; }

        [StringLength(500)]
        public string Direccion { get; set; }

        [Required]
        public int RegionId { get; set; }

        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }

        public bool Activo { get; set; }

        // Soft Delete
        public bool Eliminado { get; set; } = false;
        public DateTime? FechaEliminacion { get; set; }
        [StringLength(100)]
        public string? UsuarioEliminacion { get; set; }

        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<MovimientoItem> MovimientosOrigen { get; set; }
        public virtual ICollection<MovimientoItem> MovimientosDestino { get; set; }
    }
}

