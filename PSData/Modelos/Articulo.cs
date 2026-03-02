using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSData.Modelos
{
    public class Articulo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Marca { get; set; }

        [Required]
        [StringLength(100)]
        public string Modelo { get; set; }

        [Required]
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        public int StockMinimo { get; set; }

        [StringLength(2000)]
        public string Especificaciones { get; set; }

        public bool RequiereSerial { get; set; } = true; // Por defecto requiere serial

        // Soft Delete
        public bool Eliminado { get; set; } = false;
        public DateTime? FechaEliminacion { get; set; }
        [StringLength(100)]
        public string? UsuarioEliminacion { get; set; }

        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Lote> Lotes { get; set; }
    }
}
