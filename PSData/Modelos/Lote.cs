using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSData.Modelos
{
    public class Lote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ArticuloId { get; set; }
        [ForeignKey("ArticuloId")]
        public virtual Articulo Articulo { get; set; }

        [Required]
        public int CompraId { get; set; }
        [ForeignKey("CompraId")]
        public virtual Compra Compra { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal CostoUnitario { get; set; }

        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
