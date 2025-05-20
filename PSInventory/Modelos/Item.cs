using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSInventory.Modelos
{
    internal class Item
    {
        [Key]
        [Required]
        public string serial { get; set; }
        public Articulo articulo { get; set; }
        public string? localidad { get; set; }
        public string? estado { get; set; } 
        public string? departamento { get; set; }
    }
}
