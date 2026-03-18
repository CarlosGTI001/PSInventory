using System.Collections.Generic;

namespace PSInventory.Web.Models.ViewModels
{
    public class SalidaSinRegistroInput
    {
        public string SucursalDestinoId { get; set; }
        public string ResponsableEmpleado { get; set; }
        public string Observaciones { get; set; }
        public List<ItemSinRegistroInput> Items { get; set; }
    }

    public class ItemSinRegistroInput
    {
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Descripcion { get; set; }
        public int CategoriaId { get; set; }
        public string Serial { get; set; }
        public int Cantidad { get; set; }
    }
}
