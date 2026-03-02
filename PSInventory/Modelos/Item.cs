using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSInventory.Modelos
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public string? Serial { get; set; }
        public int Cantidad { get; set; } = 1;
        [Required]
        public int ArticuloId { get; set; }

        [NotMapped]
        public Articulo? Articulo { get; set; }
        public string? SucursalId { get; set; }

        [NotMapped]
        public Sucursal? Sucursal { get; set; }
        public string? Estado { get; set; } 
        [Required]
        public int LoteId   { get; set; }

        [NotMapped]
        public Lote? Lote { get; set; }
        public string? Departamento { get; set; }
    }
}
