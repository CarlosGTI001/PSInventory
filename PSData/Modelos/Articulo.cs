using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string marca { get; set; }

        [Required]
        public string modelo { get; set; }
    }
}
