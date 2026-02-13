using MaterialSkin.Controls;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSInventory
{
    public partial class AsignarItem : MaterialForm
    {
        LoadingHelper loadingHelper;

        public AsignarItem()
        {
            InitializeComponent();
            loadingHelper = new LoadingHelper(this);
            CargarDatosAsync();
        }

        private async void CargarDatosAsync()
        {
            await Task.Run(() =>
            {
                using (var db = new PSDatos())
                {
                    // Cargar solo items DISPONIBLES (en almacén)
                    var itemsDisponibles = db.Items.AsNoTracking()
                        .Where(i => i.Estado == "Disponible" && i.SucursalId == null)
                        .OrderBy(i => i.Serial)
                        .ToList();

                    // Cargar sucursales activas
                    var sucursales = db.Sucursales.AsNoTracking()
                        .Where(s => s.Activo)
                        .OrderBy(s => s.Nombre)
                        .ToList();

                    this.Invoke(new Action(() =>
                    {
                        // Llenar ComboBox de Items
                        cmbItem.Items.Clear();
                        foreach (var item in itemsDisponibles)
                        {
                            var articulo = db.Articulos.Find(item.ArticuloId);
                            string display = $"{item.Serial} - {articulo?.Marca} {articulo?.Modelo}";
                            cmbItem.Items.Add(new ComboBoxItem { Text = display, Value = item });
                        }

                        if (cmbItem.Items.Count > 0)
                            cmbItem.SelectedIndex = 0;
                        else
                        {
                            MaterialMessageBox.Show(
                                "No hay items disponibles en el almacén para asignar.",
                                "Sin Items",
                                MessageBoxButtons.OK,
                                false,
                                FlexibleMaterialForm.ButtonsPosition.Center
                            );
                        }

                        // Llenar ComboBox de Sucursales
                        cmbSucursal.DataSource = sucursales;
                        cmbSucursal.DisplayMember = "Nombre";
                        cmbSucursal.ValueMember = "Id";
                    }));
                }
            });
        }

        private async void btnAsignar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            loadingHelper.Show("Asignando item...");
            try
            {
                bool exito = await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        var itemSeleccionado = ((ComboBoxItem)cmbItem.SelectedItem).Value as Item;
                        var item = db.Items.Find(itemSeleccionado.Serial);

                        if (item != null)
                        {
                            // Actualizar item
                            item.SucursalId = cmbSucursal.SelectedValue.ToString();
                            item.Estado = "En Uso";
                            item.Ubicacion = txtUbicacion.Text.Trim();
                            item.ResponsableEmpleado = txtResponsable.Text.Trim();
                            item.FechaAsignacion = DateTime.Now;
                            item.FechaUltimaTransferencia = DateTime.Now;

                            // Registrar movimiento
                            db.MovimientosItem.Add(new MovimientoItem
                            {
                                ItemSerial = item.Serial,
                                SucursalOrigenId = null,  // Desde almacén
                                SucursalDestinoId = item.SucursalId,
                                FechaMovimiento = DateTime.Now,
                                UsuarioResponsable = Program.UserName,
                                Motivo = "Asignación Inicial",
                                Observaciones = txtObservaciones.Text.Trim(),
                                ResponsableRecepcion = txtResponsable.Text.Trim(),
                                FechaRecepcion = DateTime.Now
                            });

                            db.SaveChanges();
                            return true;
                        }
                    }
                    return false;
                });

                if (exito)
                {
                    MaterialMessageBox.Show(
                        "Item asignado exitosamente.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        false,
                        FlexibleMaterialForm.ButtonsPosition.Center
                    );
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            finally
            {
                loadingHelper.Hide();
            }
        }

        private bool ValidarCampos()
        {
            if (cmbItem.SelectedItem == null)
            {
                MaterialMessageBox.Show("Debe seleccionar un item", "Validación",
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                cmbItem.Focus();
                return false;
            }

            if (cmbSucursal.SelectedValue == null)
            {
                MaterialMessageBox.Show("Debe seleccionar una sucursal destino", "Validación",
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                cmbSucursal.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtResponsable.Text))
            {
                MaterialMessageBox.Show("Debe ingresar el nombre del responsable", "Validación",
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                txtResponsable.Focus();
                return false;
            }

            return true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbItem.SelectedItem != null)
            {
                var item = ((ComboBoxItem)cmbItem.SelectedItem).Value as Item;
                
                // Mostrar información del item seleccionado
                using (var db = new PSDatos())
                {
                    var articulo = db.Articulos.Find(item.ArticuloId);
                    var categoria = db.Categorias.Find(articulo?.CategoriaId);
                    
                    lblInfoItem.Text = $"Categoría: {categoria?.Nombre ?? "N/A"}\n" +
                                      $"Costo: {item.Costo:C2}\n" +
                                      $"Estado actual: {item.Estado}";
                }
            }
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() => Text;
        }
    }
}
