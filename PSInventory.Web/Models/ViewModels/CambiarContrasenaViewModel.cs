using System.ComponentModel.DataAnnotations;

namespace PSInventory.Web.Models.ViewModels
{
    public class CambiarContrasenaViewModel
    {
        [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
        [DataType(DataType.Password)]
        public string ContrasenaActual { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        public string NuevaContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes confirmar la nueva contraseña.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NuevaContrasena), ErrorMessage = "La confirmación no coincide con la nueva contraseña.")]
        public string ConfirmarNuevaContrasena { get; set; } = string.Empty;
    }
}
