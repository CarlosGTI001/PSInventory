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
    public partial class Sucursales : MaterialForm
    {
        LoadingHelper loadingHelper;
        private string sucursalIdEditar = null;

        public Sucursales()
        {
            InitializeComponent();
            loadingHelper = new LoadingHelper(this);
            chkActivo.Checked = true;
            CargarRegionesAsync();
        }

        public Sucursales(string sucursalId) : this()
        {
            sucursalIdEditar = sucursalId;
            CargarDatosSucursalAsync(sucursalId);
        }

        private async void CargarRegionesAsync()
        {
            await Task.Run(() =>
            {
                using (var db = new PSDatos())
                {
                    var regiones = db.Regiones.AsNoTracking()
                        .Where(r => r.Activo)
                        .OrderBy(r => r.Nombre)
                        .ToList();
                    
                    this.Invoke(new Action(() =>
                    {
                        cmbRegion.DataSource = regiones;
                        cmbRegion.DisplayMember = "Nombre";
                        cmbRegion.ValueMember = "Id";
                    }));
                }
            });
        }

        private async void CargarDatosSucursalAsync(string sucursalId)
        {
            loadingHelper.Show("Cargando sucursal...");
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        var sucursal = db.Sucursales.AsNoTracking()
                            .FirstOrDefault(s => s.Id == sucursalId);

                        if (sucursal != null)
                        {
                            this.Invoke(new Action(() =>
                            {
                                txtId.Text = sucursal.Id;
                                txtId.Enabled = false;  // No se puede editar el ID
                                txtNombre.Text = sucursal.Nombre;
                                txtTelefono.Text = sucursal.Telefono;
                                txtDireccion.Text = sucursal.Direccion;
                                cmbRegion.SelectedValue = sucursal.RegionId;
                                chkActivo.Checked = sucursal.Activo;
                                
                                this.Text = "Editar Sucursal";
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

            loadingHelper.Show(sucursalIdEditar != null ? "Actualizando sucursal..." : "Guardando sucursal...");
            try
            {
                bool exito = await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        if (sucursalIdEditar != null)
                        {
                            var sucursal = db.Sucursales.Find(sucursalIdEditar);
                            if (sucursal != null)
                            {
                                sucursal.Nombre = txtNombre.Text.Trim();
                                sucursal.Telefono = txtTelefono.Text.Trim();
                                sucursal.Direccion = txtDireccion.Text.Trim();
                                sucursal.RegionId = (int)cmbRegion.SelectedValue;
                                sucursal.Activo = chkActivo.Checked;
                                db.SaveChanges();
                                return true;
                            }
                        }
                        else
                        {
                            bool existe = db.Sucursales.AsNoTracking()
                                .Any(s => s.Id == txtId.Text.Trim());

                            if (existe)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    MaterialMessageBox.Show("Ya existe una sucursal con ese código", 
                                        "Sucursal Duplicada", MessageBoxButtons.OK, false, 
                                        FlexibleMaterialForm.ButtonsPosition.Center);
                                }));
                                return false;
                            }

                            db.Sucursales.Add(new Sucursal
                            {
                                Id = txtId.Text.Trim().ToUpper(),
                                Nombre = txtNombre.Text.Trim(),
                                Telefono = txtTelefono.Text.Trim(),
                                Direccion = txtDireccion.Text.Trim(),
                                RegionId = (int)cmbRegion.SelectedValue,
                                Activo = chkActivo.Checked
                            });
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
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MaterialMessageBox.Show("Debe ingresar el código de la sucursal", "Validación", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                txtId.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MaterialMessageBox.Show("Debe ingresar el nombre de la sucursal", "Validación", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                txtNombre.Focus();
                return false;
            }

            if (cmbRegion.SelectedValue == null)
            {
                MaterialMessageBox.Show("Debe seleccionar una región", "Validación", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                cmbRegion.Focus();
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
