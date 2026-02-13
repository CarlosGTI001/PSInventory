using MaterialSkin.Controls;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSInventory.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSInventory
{
    public partial class Categorias : MaterialForm
    {
        LoadingHelper loadingHelper;
        private int? categoriaIdEditar = null;

        public Categorias()
        {
            InitializeComponent();
            loadingHelper = new LoadingHelper(this);
        }
        
        public Categorias(int categoriaId)
        {
            InitializeComponent();
            loadingHelper = new LoadingHelper(this);
            categoriaIdEditar = categoriaId;
            this.Text = "Editar Categoría";
            CargarDatosCategoria();
        }
        
        private void CargarDatosCategoria()
        {
            using (var db = new PSDatos())
            {
                var categoria = db.Categorias.Find(categoriaIdEditar);
                if (categoria != null)
                {
                    categoriaTxt.Text = categoria.Nombre;
                    descripcionTxt.Text = categoria.Descripcion;
                    // chkRequiereNumeroSerie no existe en el Designer actual
                    // Se debe agregar manualmente en Visual Studio si se necesita
                }
            }
        }

        private void Categorias_Load(object sender, EventArgs e)
        {

        }

        private async void agregarBtn_Click(object sender, EventArgs e)
        {
            loadingHelper.Show(categoriaIdEditar.HasValue ? "Actualizando categoría..." : "Guardando categoría...");
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        if (categoriaIdEditar.HasValue)
                        {
                            // Modo edición
                            var categoria = db.Categorias.Find(categoriaIdEditar.Value);
                            if (categoria != null)
                            {
                                categoria.Nombre = categoriaTxt.Text.Trim();
                                categoria.Descripcion = descripcionTxt.Text.Trim();
                                // RequiereNumeroSerie se mantiene sin cambios si no existe el control
                                db.SaveChanges();
                                
                                this.Invoke(new Action(() =>
                                {
                                    this.DialogResult = DialogResult.OK;
                                    this.Close();
                                }));
                            }
                        }
                        else
                        {
                            // Modo crear
                            bool exists = db.Categorias.AsNoTracking()
                                .Any(a => a.Nombre.Equals(categoriaTxt.Text.Trim()));

                            if (exists)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    MaterialMessageBox.Show("Esta categoria ya existe en el sistema", "Aviso", 
                                        MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                                    this.DialogResult = DialogResult.Cancel;
                                    this.Close();
                                }));
                            }
                            else
                            {
                                db.Categorias.Add(new PSData.Modelos.Categoria
                                {
                                    Nombre = categoriaTxt.Text.Trim(),
                                    Descripcion = descripcionTxt.Text.Trim(),
                                    RequiereNumeroSerie = false // Valor por defecto
                                });
                                db.SaveChanges();

                                this.Invoke(new Action(() =>
                                {
                                    this.DialogResult = DialogResult.OK;
                                    this.Close();
                                }));
                            }
                        }
                    }
                });
            }
            finally
            {
                loadingHelper.Hide();
            }
        }
    }
}
