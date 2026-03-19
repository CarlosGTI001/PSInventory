using System.ComponentModel.DataAnnotations;

namespace PSInventory.Web.Models.ViewModels
{
    public class QuickCreateLocationInput
    {
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        public int? RegionId { get; set; }
    }
}
