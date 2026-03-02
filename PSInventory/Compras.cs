using MaterialSkin.Controls;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSInventory
{
    public partial class Compras : MaterialForm
    {
        LoadingHelper loadingHelper;
        private int? compraIdEditar = null;

        public Compras()
        {
            InitializeComponent();
            loadingHelper = new LoadingHelper(this);
            InicializarEstados();
        }

        public Compras(int compraId) : this()
        {
            compraIdEditar = compraId;
            CargarDatosCompraAsync(compraId);
            btnGestionarLotes.Visible = true; // Mostrar el botón si estamos editando
        }

        private void InicializarEstados()
        {
            cmbEstado.Items.Add("Pendiente");
            cmbEstado.Items.Add("Parcial");
            cmbEstado.Items.Add("Recibida");
            cmbEstado.SelectedIndex = 0;
        }

        private async void CargarDatosCompraAsync(int compraId)
        {
            loadingHelper.Show("Cargando compra...");
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        var compra = db.Compras.AsNoTracking()
                            .FirstOrDefault(c => c.Id == compraId);

                        if (compra != null)
                        {
                            this.Invoke(new Action(() =>
                            {
                                txtProveedor.Text = compra.Proveedor;
                                txtNumeroFactura.Text = compra.NumeroFactura;
                                numCostoTotal.Value = compra.CostoTotal;
                                dtpFechaCompra.Value = compra.FechaCompra;
                                cmbEstado.SelectedItem = compra.Estado;
                                txtObservaciones.Text = compra.Observaciones;
                                
                                this.Text = "Editar Compra";
                                btnGuardar.Text = "Actualizar";
                                btnGestionarLotes.Visible = true;
                            }));
                        }
                    }
                });
            }
            finally
            {
                loadingHelper.Hide();
            }
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            loadingHelper.Show(compraIdEditar.HasValue ? "Actualizando compra..." : "Guardando compra...");
            try
            {
                bool exito = await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        if (compraIdEditar.HasValue)
                        {
                            var compra = db.Compras.Find(compraIdEditar.Value);
                            if (compra != null)
                            {
                                compra.Proveedor = txtProveedor.Text.Trim();
                                compra.NumeroFactura = txtNumeroFactura.Text.Trim();
                                compra.CostoTotal = numCostoTotal.Value;
                                compra.FechaCompra = dtpFechaCompra.Value;
                                compra.Estado = cmbEstado.SelectedItem.ToString();
                                compra.Observaciones = txtObservaciones.Text.Trim();
                                db.SaveChanges();
                                return true;
                            }
                        }
                        else
                        {
                            var nuevaCompra = new Compra
                            {
                                Proveedor = txtProveedor.Text.Trim(),
                                NumeroFactura = txtNumeroFactura.Text.Trim(),
                                CostoTotal = numCostoTotal.Value,
                                FechaCompra = dtpFechaCompra.Value,
                                Estado = cmbEstado.SelectedItem.ToString(),
                                Observaciones = txtObservaciones.Text.Trim()
                            };
                            db.Compras.Add(nuevaCompra);
                            db.SaveChanges();
                            compraIdEditar = nuevaCompra.Id; // Asignar el ID de la nueva compra
                            return true;
                        }
                    }
                    return false;
                });

                if (exito)
                {
                    this.DialogResult = DialogResult.OK;
                    // Si es una nueva compra, mostrar el botón de gestionar lotes al guardar por primera vez
                    if (!compraIdEditar.HasValue) 
                    {
                        btnGestionarLotes.Visible = true;
                    }
                    this.Close();
                }
            }
            finally
            {
                loadingHelper.Hide();
            }
        }

        private void btnGestionarLotes_Click(object sender, EventArgs e)
        {
            if (compraIdEditar.HasValue)
            {
                // Aquí abriremos el nuevo formulario GestionLotesForm, pasándole el compraIdEditar
                var frmGestionLotes = new GestionLotesForm(compraIdEditar.Value);
                frmGestionLotes.ShowDialog();
                // Opcional: Recargar datos de la compra si los lotes afectan el costo total, etc.
                // CargarDatosCompraAsync(compraIdEditar.Value);
            }
            else
            {
                MaterialMessageBox.Show("Primero debe guardar la compra para poder gestionar sus lotes.", "Advertencia", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtProveedor.Text))
            {
                MaterialMessageBox.Show("Debe ingresar el proveedor", "Validación", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                txtProveedor.Focus();
                return false;
            }

            if (numCostoTotal.Value <= 0)
            {
                MaterialMessageBox.Show("El costo total debe ser mayor a 0", "Validación", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                numCostoTotal.Focus();
                return false;
            }

            if (cmbEstado.SelectedItem == null)
            {
                MaterialMessageBox.Show("Debe seleccionar un estado", "Validación", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                cmbEstado.Focus();
                return false;
            }

            return true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
