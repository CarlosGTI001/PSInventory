using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
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
using PSData.Datos;
using PSData.Modelos;

namespace PSInventory
{
    public partial class Menu : MaterialForm
    {
        ColorMgr ColorMgr = new ColorMgr();
        LoadingHelper loadingHelper;

        // MaterialListViews
        private MaterialListView listViewCompras;
        private MaterialListView listViewArticulos;
        private MaterialListView listViewCategorias;

        // Paneles de botones
        private Panel panelBotonesCompras;
        private Panel panelBotonesArticulos;
        private Panel panelBotonesCategorias;

        // Botones
        private MaterialButton btnEditarCompra, btnEliminarCompra;
        private MaterialButton btnEditarArticulo, btnEliminarArticulo;
        private MaterialButton btnEditarCategoria, btnEliminarCategoria;

        public Menu()
        {
            InitializeComponent();
            ColorMgr.ManejarColor(this);
            loadingHelper = new LoadingHelper(this);
            
            InicializarListViewsModernos();
        }

        private void InicializarListViewsModernos()
        {
            // ===== COMPRAS =====
            listViewCompras = new MaterialListView
            {
                FullRowSelect = true,
                MultiSelect = false,
                View = View.Details,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill
            };
            listViewCompras.Columns.Add("ID", 60);
            listViewCompras.Columns.Add("Proveedor", 200);
            listViewCompras.Columns.Add("Factura", 100);
            listViewCompras.Columns.Add("Costo Total", 120);
            listViewCompras.Columns.Add("Fecha", 100);
            listViewCompras.Columns.Add("Estado", 100);
            listViewCompras.DoubleClick += ListViewCompras_DoubleClick;
            listViewCompras.SelectedIndexChanged += (s, e) => 
            {
                bool hay = listViewCompras.SelectedItems.Count > 0;
                btnEditarCompra.Enabled = hay;
                btnEliminarCompra.Enabled = hay;
            };

            panelBotonesCompras = CrearPanelBotones(out btnEditarCompra, out btnEliminarCompra);
            btnEditarCompra.Click += BtnEditarCompra_Click;
            btnEliminarCompra.Click += BtnEliminarCompra_Click;

            // Reemplazar DataGrid por ListView
            if (ComprasTab.Controls.Contains(dataGridCompras))
                ComprasTab.Controls.Remove(dataGridCompras);
            
            ComprasTab.Controls.Add(listViewCompras);
            ComprasTab.Controls.Add(panelBotonesCompras);

            // ===== ARTÍCULOS =====
            listViewArticulos = new MaterialListView
            {
                FullRowSelect = true,
                MultiSelect = false,
                View = View.Details,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill
            };
            listViewArticulos.Columns.Add("ID", 60);
            listViewArticulos.Columns.Add("Marca", 150);
            listViewArticulos.Columns.Add("Modelo", 150);
            listViewArticulos.Columns.Add("Categoría", 120);
            listViewArticulos.Columns.Add("Stock Disp.", 100);
            listViewArticulos.Columns.Add("Stock Mín.", 100);
            listViewArticulos.DoubleClick += ListViewArticulos_DoubleClick;
            listViewArticulos.SelectedIndexChanged += (s, e) =>
            {
                bool hay = listViewArticulos.SelectedItems.Count > 0;
                btnEditarArticulo.Enabled = hay;
                btnEliminarArticulo.Enabled = hay;
            };

            panelBotonesArticulos = CrearPanelBotones(out btnEditarArticulo, out btnEliminarArticulo);
            btnEditarArticulo.Click += BtnEditarArticulo_Click;
            btnEliminarArticulo.Click += BtnEliminarArticulo_Click;

            if (ArticulosTab.Controls.Contains(dataGridArticulos))
                ArticulosTab.Controls.Remove(dataGridArticulos);
            
            ArticulosTab.Controls.Add(listViewArticulos);
            ArticulosTab.Controls.Add(panelBotonesArticulos);

            // ===== CATEGORÍAS =====
            listViewCategorias = new MaterialListView
            {
                FullRowSelect = true,
                MultiSelect = false,
                View = View.Details,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill
            };
            listViewCategorias.Columns.Add("ID", 60);
            listViewCategorias.Columns.Add("Nombre", 200);
            listViewCategorias.Columns.Add("Descripción", 300);
            listViewCategorias.Columns.Add("Req. Serial", 100);
            listViewCategorias.DoubleClick += ListViewCategorias_DoubleClick;
            listViewCategorias.SelectedIndexChanged += (s, e) =>
            {
                bool hay = listViewCategorias.SelectedItems.Count > 0;
                btnEditarCategoria.Enabled = hay;
                btnEliminarCategoria.Enabled = hay;
            };

            panelBotonesCategorias = CrearPanelBotones(out btnEditarCategoria, out btnEliminarCategoria);
            btnEditarCategoria.Click += BtnEditarCategoria_Click;
            btnEliminarCategoria.Click += BtnEliminarCategoria_Click;

            if (CategoriasTab.Controls.Contains(dataGridCategorias))
                CategoriasTab.Controls.Remove(dataGridCategorias);
            
            CategoriasTab.Controls.Add(listViewCategorias);
            CategoriasTab.Controls.Add(panelBotonesCategorias);
        }

        private Panel CrearPanelBotones(out MaterialButton btnEditar, out MaterialButton btnEliminar)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10)
            };

            btnEditar = new MaterialButton
            {
                Text = "✏️ Editar",
                Type = MaterialButton.MaterialButtonType.Contained,
                AutoSize = false,
                Size = new Size(120, 36),
                Location = new Point(10, 12),
                Enabled = false
            };

            btnEliminar = new MaterialButton
            {
                Text = "🗑️ Eliminar",
                Type = MaterialButton.MaterialButtonType.Outlined,
                AutoSize = false,
                Size = new Size(120, 36),
                Location = new Point(140, 12),
                Enabled = false
            };

            panel.Controls.Add(btnEditar);
            panel.Controls.Add(btnEliminar);

            return panel;
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            this.Text = "Bienvenido " + Program.UserName;
        }

        private void Menu_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            this.Dispose();
        }

        public void cerrarSesion()
        {
            MaterialDialog dialog = new MaterialDialog(this, "Cerrar Sesion", "¿Está seguro de que desea cerrar sesión?", "Aceptar", true, "Cancelar");
            var resultado = dialog.ShowDialog(this);
            if (resultado == DialogResult.OK)
            {
                Login login = new Login();
                login.Show();
                this.Hide();
                this.Dispose();
            }
        }

        private void OptionControls_SelectedIndexChanged(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Text == "Cerrar Sesion")
            {
                MaterialDialog dialog = new MaterialDialog(this, "Cerrar Aplicación", "¿Está seguro de que desea cerrar la aplicación?", "Aceptar", true, "Cancelar");
                var resultado = dialog.ShowDialog(this);
                if (resultado == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
        }

        private async void addBtn_Click(object sender, EventArgs e)
        {
            if (tabInventarios.SelectedIndex == 0)  // Compras
            {
                Compras compras = new Compras();
                var resultado = compras.ShowDialog(this);
                if (resultado == DialogResult.OK)
                {
                    await LoadComprasAsync();
                    MessageBox.Show("Compra registrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (tabInventarios.SelectedIndex == 1)  // Artículos
            {
                Articulos articulos = new Articulos();
                var resultado = articulos.ShowDialog(this);
                if (resultado == DialogResult.OK)
                {
                    await LoadArticulosAsync();
                    MessageBox.Show("Artículo agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (tabInventarios.SelectedIndex == 2)  // Categorías
            {
                Categorias categorias = new Categorias();
                var resultado = categorias.ShowDialog(this);
                if (resultado == DialogResult.OK)
                {
                    await LoadCategoriasAsync();
                    MessageBox.Show("Categoría agregada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void tabInventarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabInventarios.SelectedIndex)
            {
                case 0:
                    await LoadComprasAsync();
                    break;
                case 1:
                    await LoadArticulosAsync();
                    break;
                case 2:
                    await LoadCategoriasAsync();
                    break;
                default:
                    break;
            }
        }

        // ===== CARGA DE DATOS =====

        private async Task LoadComprasAsync()
        {
            loadingHelper.Show("Cargando compras...");
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        var compras = db.Compras.AsNoTracking()
                            .OrderByDescending(c => c.FechaCompra)
                            .ToList();

                        this.Invoke(new Action(() =>
                        {
                            listViewCompras.Items.Clear();

                            foreach (var compra in compras)
                            {
                                var item = new ListViewItem(compra.Id.ToString());
                                item.SubItems.Add(compra.Proveedor);
                                item.SubItems.Add(compra.NumeroFactura ?? "N/A");
                                item.SubItems.Add(compra.CostoTotal.ToString("C2"));
                                item.SubItems.Add(compra.FechaCompra.ToString("dd/MM/yyyy"));
                                item.SubItems.Add(compra.Estado);
                                item.Tag = compra;

                                // Colores según estado
                                switch (compra.Estado)
                                {
                                    case "Pendiente":
                                        item.ForeColor = Color.FromArgb(255, 152, 0); // Naranja
                                        break;
                                    case "Recibida":
                                        item.ForeColor = Color.FromArgb(76, 175, 80); // Verde
                                        break;
                                    case "Parcial":
                                        item.ForeColor = Color.FromArgb(33, 150, 243); // Azul
                                        break;
                                }

                                listViewCompras.Items.Add(item);
                            }
                        }));
                    }
                });
            }
            finally
            {
                loadingHelper.Hide();
            }
        }

        private async Task LoadArticulosAsync()
        {
            loadingHelper.Show("Cargando artículos...");
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        var articulos = db.Articulos.AsNoTracking()
                            .OrderBy(a => a.Marca).ThenBy(a => a.Modelo)
                            .ToList();

                        this.Invoke(new Action(() =>
                        {
                            listViewArticulos.Items.Clear();

                            foreach (var art in articulos)
                            {
                                var item = new ListViewItem(art.Id.ToString());
                                item.SubItems.Add(art.Marca);
                                item.SubItems.Add(art.Modelo);

                                // Obtener categoría
                                var categoria = db.Categorias.Find(art.CategoriaId);
                                item.SubItems.Add(categoria?.Nombre ?? "Sin categoría");

                                // Contar stock disponible
                                int stockDisponible = db.Items.Count(i => i.ArticuloId == art.Id && i.Estado == "Disponible");
                                item.SubItems.Add(stockDisponible.ToString());
                                item.SubItems.Add(art.StockMinimo.ToString());

                                item.Tag = art;

                                // ⚠️ ALERTA: Stock bajo
                                if (stockDisponible < art.StockMinimo)
                                {
                                    item.ForeColor = Color.FromArgb(244, 67, 54); // Rojo
                                    item.Font = new Font(listViewArticulos.Font, FontStyle.Bold);
                                }
                                else if (stockDisponible == 0)
                                {
                                    item.ForeColor = Color.Gray;
                                }
                                else
                                {
                                    item.ForeColor = Color.FromArgb(76, 175, 80); // Verde
                                }

                                listViewArticulos.Items.Add(item);
                            }
                        }));
                    }
                });
            }
            finally
            {
                loadingHelper.Hide();
            }
        }

        private async Task LoadCategoriasAsync()
        {
            loadingHelper.Show("Cargando categorías...");
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        var categorias = db.Categorias.AsNoTracking()
                            .OrderBy(c => c.Nombre)
                            .ToList();

                        this.Invoke(new Action(() =>
                        {
                            listViewCategorias.Items.Clear();

                            foreach (var cat in categorias)
                            {
                                var item = new ListViewItem(cat.Id.ToString());
                                item.SubItems.Add(cat.Nombre);
                                item.SubItems.Add(cat.Descripcion ?? "");
                                item.SubItems.Add(cat.RequiereNumeroSerie ? "Sí" : "No");
                                item.Tag = cat;

                                listViewCategorias.Items.Add(item);
                            }
                        }));
                    }
                });
            }
            finally
            {
                loadingHelper.Hide();
            }
        }

        // ===== EVENTOS DE DOBLE CLIC =====

        private async void ListViewCompras_DoubleClick(object sender, EventArgs e)
        {
            if (listViewCompras.SelectedItems.Count > 0)
            {
                var compra = (Compra)listViewCompras.SelectedItems[0].Tag;
                await EditarCompra(compra.Id);
            }
        }

        private async void ListViewArticulos_DoubleClick(object sender, EventArgs e)
        {
            if (listViewArticulos.SelectedItems.Count > 0)
            {
                var articulo = (Articulo)listViewArticulos.SelectedItems[0].Tag;
                await EditarArticulo(articulo.Id);
            }
        }

        private async void ListViewCategorias_DoubleClick(object sender, EventArgs e)
        {
            if (listViewCategorias.SelectedItems.Count > 0)
            {
                var categoria = (Categoria)listViewCategorias.SelectedItems[0].Tag;
                await EditarCategoria(categoria.Id);
            }
        }

        // ===== BOTONES EDITAR =====

        private async void BtnEditarCompra_Click(object sender, EventArgs e)
        {
            if (listViewCompras.SelectedItems.Count > 0)
            {
                var compra = (Compra)listViewCompras.SelectedItems[0].Tag;
                await EditarCompra(compra.Id);
            }
        }

        private async void BtnEditarArticulo_Click(object sender, EventArgs e)
        {
            if (listViewArticulos.SelectedItems.Count > 0)
            {
                var articulo = (Articulo)listViewArticulos.SelectedItems[0].Tag;
                await EditarArticulo(articulo.Id);
            }
        }

        private async void BtnEditarCategoria_Click(object sender, EventArgs e)
        {
            if (listViewCategorias.SelectedItems.Count > 0)
            {
                var categoria = (Categoria)listViewCategorias.SelectedItems[0].Tag;
                await EditarCategoria(categoria.Id);
            }
        }

        // ===== MÉTODOS DE EDICIÓN =====

        private async Task EditarCompra(int id)
        {
            Compras form = new Compras(id);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                await LoadComprasAsync();
                MessageBox.Show("Compra actualizada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task EditarArticulo(int id)
        {
            Articulos form = new Articulos(id);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                await LoadArticulosAsync();
                MessageBox.Show("Artículo actualizado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task EditarCategoria(int id)
        {
            // TODO: Cuando agregues constructor con ID a Categorias.cs
            MaterialMessageBox.Show("Funcionalidad de edición próximamente", "Info",
                MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
            // Descomentar cuando esté listo:
            // Categorias form = new Categorias(id);
            // if (form.ShowDialog(this) == DialogResult.OK)
            // {
            //     await LoadCategoriasAsync();
            //     MessageBox.Show("Categoría actualizada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // }
        }

        // ===== BOTONES ELIMINAR =====

        private async void BtnEliminarCompra_Click(object sender, EventArgs e)
        {
            if (listViewCompras.SelectedItems.Count > 0)
            {
                var compra = (Compra)listViewCompras.SelectedItems[0].Tag;
                await EliminarCompra(compra);
            }
        }

        private async void BtnEliminarArticulo_Click(object sender, EventArgs e)
        {
            if (listViewArticulos.SelectedItems.Count > 0)
            {
                var articulo = (Articulo)listViewArticulos.SelectedItems[0].Tag;
                await EliminarArticulo(articulo);
            }
        }

        private async void BtnEliminarCategoria_Click(object sender, EventArgs e)
        {
            if (listViewCategorias.SelectedItems.Count > 0)
            {
                var categoria = (Categoria)listViewCategorias.SelectedItems[0].Tag;
                await EliminarCategoria(categoria);
            }
        }

        // ===== MÉTODOS DE ELIMINACIÓN =====

        private async Task EliminarCompra(Compra compra)
        {
            var resultado = MaterialMessageBox.Show(
                $"¿Está seguro de eliminar la compra #{compra.Id} de {compra.Proveedor}?\n\n" +
                $"Costo: {compra.CostoTotal:C2}",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                false,
                FlexibleMaterialForm.ButtonsPosition.Center
            );

            if (resultado == DialogResult.Yes)
            {
                loadingHelper.Show("Eliminando compra...");
                try
                {
                    bool eliminado = false;
                    string error = "";

                    await Task.Run(() =>
                    {
                        using (var db = new PSDatos())
                        {
                            var comp = db.Compras.Find(compra.Id);
                            if (comp != null)
                            {
                                // Verificar items asociados
                                bool tieneItems = db.Items.Any(i => i.CompraId == compra.Id);
                                if (tieneItems)
                                {
                                    error = "No se puede eliminar la compra porque tiene items asociados.";
                                    return;
                                }

                                db.Compras.Remove(comp);
                                db.SaveChanges();
                                eliminado = true;
                            }
                        }
                    });

                    if (!string.IsNullOrEmpty(error))
                    {
                        MaterialMessageBox.Show(error, "Error", MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                    }
                    else if (eliminado)
                    {
                        await LoadComprasAsync();
                        MessageBox.Show("Compra eliminada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show($"Error al eliminar: {ex.Message}", "Error",
                        MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                finally
                {
                    loadingHelper.Hide();
                }
            }
        }

        private async Task EliminarArticulo(Articulo articulo)
        {
            var resultado = MaterialMessageBox.Show(
                $"¿Está seguro de eliminar el artículo?\n\n" +
                $"{articulo.Marca} {articulo.Modelo}",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                false,
                FlexibleMaterialForm.ButtonsPosition.Center
            );

            if (resultado == DialogResult.Yes)
            {
                loadingHelper.Show("Eliminando artículo...");
                try
                {
                    bool eliminado = false;
                    string error = "";

                    await Task.Run(() =>
                    {
                        using (var db = new PSDatos())
                        {
                            var art = db.Articulos.Find(articulo.Id);
                            if (art != null)
                            {
                                bool tieneItems = db.Items.Any(i => i.ArticuloId == articulo.Id);
                                if (tieneItems)
                                {
                                    error = "No se puede eliminar el artículo porque tiene items asociados.";
                                    return;
                                }

                                db.Articulos.Remove(art);
                                db.SaveChanges();
                                eliminado = true;
                            }
                        }
                    });

                    if (!string.IsNullOrEmpty(error))
                    {
                        MaterialMessageBox.Show(error, "Error", MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                    }
                    else if (eliminado)
                    {
                        await LoadArticulosAsync();
                        MessageBox.Show("Artículo eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show($"Error al eliminar: {ex.Message}", "Error",
                        MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                finally
                {
                    loadingHelper.Hide();
                }
            }
        }

        private async Task EliminarCategoria(Categoria categoria)
        {
            var resultado = MaterialMessageBox.Show(
                $"¿Está seguro de eliminar la categoría '{categoria.Nombre}'?",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                false,
                FlexibleMaterialForm.ButtonsPosition.Center
            );

            if (resultado == DialogResult.Yes)
            {
                loadingHelper.Show("Eliminando categoría...");
                try
                {
                    bool eliminado = false;
                    string error = "";

                    await Task.Run(() =>
                    {
                        using (var db = new PSDatos())
                        {
                            var cat = db.Categorias.Find(categoria.Id);
                            if (cat != null)
                            {
                                bool tieneArticulos = db.Articulos.Any(a => a.CategoriaId == categoria.Id);
                                if (tieneArticulos)
                                {
                                    error = "No se puede eliminar la categoría porque tiene artículos asociados.";
                                    return;
                                }

                                db.Categorias.Remove(cat);
                                db.SaveChanges();
                                eliminado = true;
                            }
                        }
                    });

                    if (!string.IsNullOrEmpty(error))
                    {
                        MaterialMessageBox.Show(error, "Error", MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                    }
                    else if (eliminado)
                    {
                        await LoadCategoriasAsync();
                        MessageBox.Show("Categoría eliminada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show($"Error al eliminar: {ex.Message}", "Error",
                        MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                }
                finally
                {
                    loadingHelper.Hide();
                }
            }
        }

        private void tabInventarios_Selected(object sender, TabControlEventArgs e)
        {
        }

        private void ArticulosTab_Click(object sender, EventArgs e)
        {
        }
    }
}
