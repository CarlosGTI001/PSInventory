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
    public partial class Articulos : MaterialForm
    {
        LoadingHelper loadingHelper;
        private int? articuloIdEditar = null;

        public Articulos()
        {
            InitializeComponent();
            loadingHelper = new LoadingHelper(this);
            CargarCategoriasAsync();
        }

        public Articulos(int articuloId) : this()
        {
            articuloIdEditar = articuloId;
            CargarDatosArticuloAsync(articuloId);
        }

        private async void CargarCategoriasAsync()
        {
            await Task.Run(() =>
            {
                using (var db = new PSDatos())
                {
                    var categorias = db.Categorias.AsNoTracking().OrderBy(c => c.Nombre).ToList();
                    this.Invoke(new Action(() =>
                    {
                        cmbCategoria.DataSource = categorias;
                        cmbCategoria.DisplayMember = "Nombre";
                        cmbCategoria.ValueMember = "Id";
                    }));
                }
            });
        }

        private async void CargarDatosArticuloAsync(int articuloId)
        {
            loadingHelper.Show("Cargando artículo...");
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        var articulo = db.Articulos.AsNoTracking()
                            .FirstOrDefault(a => a.Id == articuloId);

                        if (articulo != null)
                        {
                            this.Invoke(new Action(() =>
                            {
                                txtMarca.Text = articulo.Marca;
                                txtModelo.Text = articulo.Modelo;
                                txtDescripcion.Text = articulo.Descripcion;
                                txtEspecificaciones.Text = articulo.Especificaciones;
                                numStockMinimo.Value = articulo.StockMinimo;
                                cmbCategoria.SelectedValue = articulo.CategoriaId;
                                
                                this.Text = "Editar Artículo";
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

            loadingHelper.Show(articuloIdEditar.HasValue ? "Actualizando artículo..." : "Guardando artículo...");
            try
            {
                bool exito = await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        if (articuloIdEditar.HasValue)
                        {
                            var articulo = db.Articulos.Find(articuloIdEditar.Value);
                            if (articulo != null)
                            {
                                articulo.Marca = txtMarca.Text.Trim();
                                articulo.Modelo = txtModelo.Text.Trim();
                                articulo.CategoriaId = (int)cmbCategoria.SelectedValue;
                                articulo.Descripcion = txtDescripcion.Text.Trim();
                                articulo.StockMinimo = (int)numStockMinimo.Value;
                                articulo.Especificaciones = txtEspecificaciones.Text.Trim();
                                db.SaveChanges();
                                return true;
                            }
                        }
                        else
                        {
                            bool existe = db.Articulos.AsNoTracking()
                                .Any(a => a.Marca == txtMarca.Text.Trim() && 
                                         a.Modelo == txtModelo.Text.Trim());

                            if (existe)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    MaterialMessageBox.Show("Ya existe un artículo con esa marca y modelo", 
                                        "Artículo Duplicado", MessageBoxButtons.OK, false, 
                                        FlexibleMaterialForm.ButtonsPosition.Center);
                                }));
                                return false;
                            }

                            db.Articulos.Add(new Articulo
                            {
                                Marca = txtMarca.Text.Trim(),
                                Modelo = txtModelo.Text.Trim(),
                                CategoriaId = (int)cmbCategoria.SelectedValue,
                                Descripcion = txtDescripcion.Text.Trim(),
                                StockMinimo = (int)numStockMinimo.Value,
                                Especificaciones = txtEspecificaciones.Text.Trim()
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
            if (string.IsNullOrWhiteSpace(txtMarca.Text))
            {
                MaterialMessageBox.Show("Debe ingresar la marca", "Validación", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                txtMarca.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtModelo.Text))
            {
                MaterialMessageBox.Show("Debe ingresar el modelo", "Validación", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                txtModelo.Focus();
                return false;
            }

            if (cmbCategoria.SelectedValue == null)
            {
                MaterialMessageBox.Show("Debe seleccionar una categoría", "Validación", 
                    MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                cmbCategoria.Focus();
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
