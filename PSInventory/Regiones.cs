using MaterialSkin.Controls;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSInventory.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RegionModel = PSData.Modelos.Region;

namespace PSInventory
{
    public partial class Regiones : MaterialForm
    {
        LoadingHelper loadingHelper;
        private int? regionIdEditar = null;

        public Regiones()
        {
            InitializeComponent();
            loadingHelper = new LoadingHelper(this);
            chkActivo.Checked = true;
        }

        public Regiones(int regionId) : this()
        {
            regionIdEditar = regionId;
            CargarDatosRegionAsync(regionId);
        }

        private async void CargarDatosRegionAsync(int regionId)
        {
            loadingHelper.Show("Cargando región...");
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        var region = db.Regiones.AsNoTracking()
                            .FirstOrDefault(r => r.Id == regionId);

                        if (region != null)
                        {
                            this.Invoke(new Action(() =>
                            {
                                txtNombre.Text = region.Nombre;
                                txtDescripcion.Text = region.Descripcion;
                                chkActivo.Checked = region.Activo;
                                
                                this.Text = "Editar Región";
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

            loadingHelper.Show(regionIdEditar.HasValue ? "Actualizando región..." : "Guardando región...");
            try
            {
                bool exito = await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        if (regionIdEditar.HasValue)
                        {
                            var region = db.Regiones.Find(regionIdEditar.Value);
                            if (region != null)
                            {
                                region.Nombre = txtNombre.Text.Trim();
                                region.Descripcion = txtDescripcion.Text.Trim();
                                region.Activo = chkActivo.Checked;
                                db.SaveChanges();
                                return true;
                            }
                        }
                        else
                        {
                            bool existe = db.Regiones.AsNoTracking()
                                .Any(r => r.Nombre == txtNombre.Text.Trim());

                            if (existe)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    MaterialMessageBox.Show("Ya existe una región con ese nombre", 
                                        "Región Duplicada", MessageBoxButtons.OK, false, 
                                        FlexibleMaterialForm.ButtonsPosition.Center);
                                }));
                                return false;
                            }

                            db.Regiones.Add(new RegionModel
                            {
                                Nombre = txtNombre.Text.Trim(),
                                Descripcion = txtDescripcion.Text.Trim(),
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
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MaterialMessageBox.Show("Debe ingresar el nombre de la región", "Validación", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                txtNombre.Focus();
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
