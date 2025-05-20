using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSInventory.Modelos
{
    internal class Articulo
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string marca { get; set; }

        [Required]
        public string modelo { get; set; }
    }
}
