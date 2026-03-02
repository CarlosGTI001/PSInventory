using MaterialSkin.Controls;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Helpers;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSInventory
{
    public partial class Items : MaterialForm
    {
        LoadingHelper loadingHelper;
        private int? itemIdEditar = null;

        public Items()
        {
            InitializeComponent();
            loadingHelper = new LoadingHelper(this);
            InicializarEstados();
            CargarDatosAsync();
        }

        public Items(int id) : this()
        {
            itemIdEditar = id;
            CargarDatosItemAsync(id);
        }

        private void InicializarEstados()
        {
            cmbEstado.Items.Add("Disponible");
            cmbEstado.Items.Add("En Uso");
            cmbEstado.Items.Add("En Reparación");
            cmbEstado.Items.Add("Dañado");
            cmbEstado.Items.Add("Dado de Baja");
            cmbEstado.SelectedIndex = 0;
        }

        private async void CargarDatosAsync()
        {
            await Task.Run(() =>
            {
                using (var db = new PSDatos())
                {
                    // Cargar Artículos
                    var articulos = db.Articulos.AsNoTracking()
                        .OrderBy(a => a.Marca).ThenBy(a => a.Modelo)
                        .Select(a => new { 
                            a.Id, 
                            Nombre = a.Marca + " " + a.Modelo 
                        })
                        .ToList();

                    // Cargar Lotes
                    var lotes = db.Lotes.AsNoTracking()
                        .Include(l => l.Compra)
                        .Where(l => l.Compra.Estado != "Recibida")
                        .OrderByDescending(l => l.Compra.FechaCompra)
                        .Select(l => new { 
                            Id = l.Id, 
                            Nombre = $"Lote {l.Id} - " + l.Compra.Proveedor + " - " + l.Compra.FechaCompra.ToString("dd/MM/yyyy") 
                        })
                        .ToList();

                    // Cargar Sucursales (opcional)
                    var sucursales = db.Sucursales.AsNoTracking()
                        .Where(s => s.Activo)
                        .OrderBy(s => s.Nombre)
                        .Select(s => new { s.Id, s.Nombre })
                        .ToList();

                    this.Invoke(new Action(() =>
                    {
                        cmbArticulo.DataSource = articulos;
                        cmbArticulo.DisplayMember = "Nombre";
                        cmbArticulo.ValueMember = "Id";

                        cmbCompra.DataSource = lotes;
                        cmbCompra.DisplayMember = "Nombre";
                        cmbCompra.ValueMember = "Id";

                        // Sucursal es opcional, agregar opción vacía
                        sucursales.Insert(0, new { Id = "", Nombre = "(Sin asignar - En Almacén)" });
                        cmbSucursal.DataSource = sucursales;
                        cmbSucursal.DisplayMember = "Nombre";
                        cmbSucursal.ValueMember = "Id";
                    }));
                }
            });
        }

        private async void CargarDatosItemAsync(int id)
        {
            loadingHelper.Show("Cargando item...");
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        var item = db.Items.AsNoTracking()
                            .FirstOrDefault(i => i.Id == id);

                        if (item != null)
                        {
                            this.Invoke(new Action(() =>
                            {
                                txtSerial.Text = item.Serial;
                                txtSerial.Enabled = false;
                                cmbArticulo.SelectedValue = item.ArticuloId;
                                cmbCompra.SelectedValue = item.LoteId;
                                // numCosto.Value is removed or ignored since cost is per lote
                                txtUbicacion.Text = item.Ubicacion;
                                txtResponsable.Text = item.ResponsableEmpleado;
                                txtObservaciones.Text = item.Observaciones;

                                if (item.FechaGarantiaInicio.HasValue)
                                {
                                    dtpGarantiaInicio.Value = item.FechaGarantiaInicio.Value;
                                    numMesesGarantia.Value = item.MesesGarantia ?? 0;
                                }

                                this.Text = "Editar Item";
                                btnGuardar.Text = "Actualizar";
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

            loadingHelper.Show(itemIdEditar != null ? "Actualizando item..." : "Guardando item...");
            try
            {
                bool exito = await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        if (itemIdEditar != null)
                        {
                            var item = db.Items.Find(itemIdEditar);
                            if (item != null)
                            {
                                item.ArticuloId = (int)cmbArticulo.SelectedValue;
                                item.LoteId = (int)cmbCompra.SelectedValue;
                                // item.Costo is removed
                                item.Ubicacion = txtUbicacion.Text.Trim();
                                item.ResponsableEmpleado = txtResponsable.Text.Trim();
                                item.Observaciones = txtObservaciones.Text.Trim();
                                item.FechaGarantiaInicio = dtpGarantiaInicio.Value;
                                item.MesesGarantia = (int)numMesesGarantia.Value;
                                item.FechaGarantiaVencimiento = dtpGarantiaInicio.Value.AddMonths((int)numMesesGarantia.Value);
                                
                                db.SaveChanges();
                                return true;
                            }
                        }
                        else
                        {
                            string serialInput = txtSerial.Text.Trim().ToUpper();
                            string? finalSerial = string.IsNullOrEmpty(serialInput) ? null : serialInput;

                            if (finalSerial != null)
                            {
                                bool existe = db.Items.AsNoTracking()
                                    .Any(i => i.Serial == finalSerial);

                                if (existe)
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        MaterialMessageBox.Show("Ya existe un item con ese número de serie", 
                                            "Serial Duplicado", MessageBoxButtons.OK, false, 
                                            FlexibleMaterialForm.ButtonsPosition.Center);
                                    }));
                                    return false;
                                }
                            }

                            var nuevoItem = new Item
                            {
                                Serial = finalSerial,
                                Cantidad = 1, // To do: add UI for Cantidad
                                ArticuloId = (int)cmbArticulo.SelectedValue,
                                LoteId = (int)cmbCompra.SelectedValue,
                                // Costo removed
                                Ubicacion = txtUbicacion.Text.Trim(),
                                ResponsableEmpleado = txtResponsable.Text.Trim(),
                                Observaciones = txtObservaciones.Text.Trim(),
                                FechaGarantiaInicio = dtpGarantiaInicio.Value,
                                MesesGarantia = (int)numMesesGarantia.Value,
                                FechaGarantiaVencimiento = dtpGarantiaInicio.Value.AddMonths((int)numMesesGarantia.Value),
                                FechaAsignacion = DateTime.Now
                            };

                            db.Items.Add(nuevoItem);
                            db.SaveChanges();
                            return true;
                        }
                    }
                    return false;
                });

                if (exito)
                {
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
            if (cmbArticulo.SelectedValue == null)
            {
                MaterialMessageBox.Show("Debe seleccionar un artículo", "Validación", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                cmbArticulo.Focus();
                return false;
            }

            if (cmbCompra.SelectedValue == null)
            {
                MaterialMessageBox.Show("Debe seleccionar una compra", "Validación", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                cmbCompra.Focus();
                return false;
            }

            // numCosto validation removed

            return true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cmbEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Si el estado es "Disponible", limpiar sucursal y responsable
            if (cmbEstado.SelectedItem?.ToString() == "Disponible")
            {
                cmbSucursal.SelectedIndex = 0;  // Sin asignar
                txtResponsable.Text = "";
                txtUbicacion.Text = "Almacén Central";
            }
        }
    }
}
