using MaterialSkin.Controls;
using Microsoft.EntityFrameworkCore;
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
        private LoadingHelper loadingHelper;
        
        // MaterialListView declarations
        private MaterialSkin.Controls.MaterialListView listViewCompras;
        private MaterialSkin.Controls.MaterialListView listViewArticulos;
        private MaterialSkin.Controls.MaterialListView listViewCategorias;
        private MaterialSkin.Controls.MaterialListView listViewItems;
        private MaterialSkin.Controls.MaterialListView listViewRegiones;
        private MaterialSkin.Controls.MaterialListView listViewSucursales;
        
        // Buttons
        private MaterialButton btnEditarCompra;
        private MaterialButton btnEliminarCompra;
        private MaterialButton btnEditarArticulo;
        private MaterialButton btnEliminarArticulo;
        private MaterialButton btnEditarCategoria;
        private MaterialButton btnEliminarCategoria;
        private MaterialButton btnEditarItem;
        private MaterialButton btnEliminarItem;
        private MaterialButton btnEditarRegion;
        private MaterialButton btnEliminarRegion;
        private MaterialButton btnEditarSucursal;
        private MaterialButton btnEliminarSucursal;
        
        public Menu()
        {
            InitializeComponent();
            ColorMgr.ManejarColor(this);
            loadingHelper = new LoadingHelper(this);
            InicializarListViewsModernos();
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            this.Text = "Bienvenido " + Program.UserName;
            // Load data asynchronously
            Task.Run(() => CargarDatosIniciales());
        }
        
        private void InicializarListViewsModernos()
        {
            // Esta función se debe llamar después de que InitializeComponent() haya creado los controles
            // Si los ListViews ya existen en el Designer, solo configuramos las columnas
            // Si no existen, primero hay que agregarlos en el Designer
            
            // Configurar Compras ListView
            if (listViewCompras != null)
            {
                ConfigurarListViewCompras();
            }
            
            // Configurar Articulos ListView
            if (listViewArticulos != null)
            {
                ConfigurarListViewArticulos();
            }
            
            // Configurar Categorias ListView
            if (listViewCategorias != null)
            {
                ConfigurarListViewCategorias();
            }
            
            // Configurar Items ListView
            if (listViewItems != null)
            {
                ConfigurarListViewItems();
            }
            
            // Configurar Regiones ListView
            if (listViewRegiones != null)
            {
                ConfigurarListViewRegiones();
            }
            
            // Configurar Sucursales ListView
            if (listViewSucursales != null)
            {
                ConfigurarListViewSucursales();
            }
        }
        
        private void ConfigurarListViewCompras()
        {
            listViewCompras.View = View.Details;
            listViewCompras.FullRowSelect = true;
            listViewCompras.GridLines = true;
            listViewCompras.Columns.Clear();
            listViewCompras.Columns.Add("ID", 80);
            listViewCompras.Columns.Add("Proveedor", 200);
            listViewCompras.Columns.Add("Fecha", 120);
            listViewCompras.Columns.Add("Total", 120);
            listViewCompras.Columns.Add("Estado", 120);
        }
        
        private void ConfigurarListViewArticulos()
        {
            listViewArticulos.View = View.Details;
            listViewArticulos.FullRowSelect = true;
            listViewArticulos.GridLines = true;
            listViewArticulos.Columns.Clear();
            listViewArticulos.Columns.Add("ID", 80);
            listViewArticulos.Columns.Add("Nombre", 200);
            listViewArticulos.Columns.Add("Categoría", 150);
            listViewArticulos.Columns.Add("Stock", 100);
            listViewArticulos.Columns.Add("Mínimo", 100);
        }
        
        private void ConfigurarListViewCategorias()
        {
            listViewCategorias.View = View.Details;
            listViewCategorias.FullRowSelect = true;
            listViewCategorias.GridLines = true;
            listViewCategorias.Columns.Clear();
            listViewCategorias.Columns.Add("ID", 80);
            listViewCategorias.Columns.Add("Nombre", 250);
            listViewCategorias.Columns.Add("Requiere Serie", 150);
        }
        
        private void ConfigurarListViewItems()
        {
            listViewItems.View = View.Details;
            listViewItems.FullRowSelect = true;
            listViewItems.GridLines = true;
            listViewItems.Columns.Clear();
            listViewItems.Columns.Add("Número de Serie", 150);
            listViewItems.Columns.Add("Artículo", 180);
            listViewItems.Columns.Add("Estado", 120);
            listViewItems.Columns.Add("Sucursal", 150);
            listViewItems.Columns.Add("Garantía", 120);
        }
        
        private void ConfigurarListViewRegiones()
        {
            listViewRegiones.View = View.Details;
            listViewRegiones.FullRowSelect = true;
            listViewRegiones.GridLines = true;
            listViewRegiones.Columns.Clear();
            listViewRegiones.Columns.Add("ID", 80);
            listViewRegiones.Columns.Add("Nombre", 250);
            listViewRegiones.Columns.Add("Activo", 100);
        }
        
        private void ConfigurarListViewSucursales()
        {
            listViewSucursales.View = View.Details;
            listViewSucursales.FullRowSelect = true;
            listViewSucursales.GridLines = true;
            listViewSucursales.Columns.Clear();
            listViewSucursales.Columns.Add("ID", 80);
            listViewSucursales.Columns.Add("Nombre", 200);
            listViewSucursales.Columns.Add("Región", 150);
            listViewSucursales.Columns.Add("Activo", 100);
        }
        
        private async void CargarDatosIniciales()
        {
            try
            {
                this.Invoke((MethodInvoker)delegate { loadingHelper.Show("Cargando datos..."); });
                
                // Cargar todas las entidades
                await CargarComprasAsync();
                await CargarArticulosAsync();
                await CargarCategoriasAsync();
                await CargarItemsAsync();
                await CargarRegionesAsync();
                await CargarSucursalesAsync();
                
                this.Invoke((MethodInvoker)delegate { loadingHelper.Hide(); });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    loadingHelper.Hide();
                    MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }
        
        private async Task CargarComprasAsync()
        {
            await Task.Run(() =>
            {
                using (var context = new PSDatos())
                {
                    var compras = context.Compras.AsNoTracking().ToList();
                    
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (listViewCompras != null)
                        {
                            listViewCompras.Items.Clear();
                            foreach (var compra in compras)
                            {
                                var item = new ListViewItem(compra.Id.ToString());
                                item.SubItems.Add(compra.Proveedor ?? "N/A");
                                item.SubItems.Add(compra.FechaCompra.ToString("dd/MM/yyyy"));
                                item.SubItems.Add(compra.CostoTotal.ToString("C"));
                                item.SubItems.Add(compra.Estado ?? "N/A");
                                
                                // Color según estado
                                if (compra.Estado == "Recibida")
                                    item.ForeColor = Color.Green;
                                else if (compra.Estado == "Pendiente")
                                    item.ForeColor = Color.Orange;
                                    
                                item.Tag = compra.Id;
                                listViewCompras.Items.Add(item);
                            }
                        }
                    });
                }
            });
        }
        
        private async Task CargarArticulosAsync()
        {
            await Task.Run(() =>
            {
                using (var context = new PSDatos())
                {
                    var articulos = context.Articulos.AsNoTracking().ToList();
                    var categorias = context.Categorias.AsNoTracking().ToList();
                    var items = context.Items.AsNoTracking().ToList();
                    
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (listViewArticulos != null)
                        {
                            listViewArticulos.Items.Clear();
                            foreach (var articulo in articulos)
                            {
                                var categoria = categorias.FirstOrDefault(c => c.Id == articulo.CategoriaId);
                                var stockActual = items.Count(i => i.ArticuloId == articulo.Id && i.Estado == "Disponible");
                                
                                var item = new ListViewItem(articulo.Id.ToString());
                                item.SubItems.Add($"{articulo.Marca} {articulo.Modelo}");
                                item.SubItems.Add(categoria?.Nombre ?? "Sin categoría");
                                item.SubItems.Add(stockActual.ToString());
                                item.SubItems.Add(articulo.StockMinimo.ToString());
                                
                                // Color si está bajo stock
                                if (stockActual < articulo.StockMinimo)
                                {
                                    item.ForeColor = Color.Red;
                                    item.Font = new Font(item.Font, FontStyle.Bold);
                                }
                                
                                item.Tag = articulo.Id;
                                listViewArticulos.Items.Add(item);
                            }
                        }
                    });
                }
            });
        }
        
        private async Task CargarCategoriasAsync()
        {
            await Task.Run(() =>
            {
                using (var context = new PSDatos())
                {
                    var categorias = context.Categorias.AsNoTracking().ToList();
                    
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (listViewCategorias != null)
                        {
                            listViewCategorias.Items.Clear();
                            foreach (var categoria in categorias)
                            {
                                var item = new ListViewItem(categoria.Id.ToString());
                                item.SubItems.Add(categoria.Nombre);
                                item.SubItems.Add(categoria.RequiereNumeroSerie ? "Sí" : "No");
                                item.Tag = categoria.Id;
                                listViewCategorias.Items.Add(item);
                            }
                        }
                    });
                }
            });
        }
        
        private async Task CargarItemsAsync()
        {
            await Task.Run(() =>
            {
                using (var context = new PSDatos())
                {
                    var items = context.Items.AsNoTracking().ToList();
                    var articulos = context.Articulos.AsNoTracking().ToList();
                    var sucursales = context.Sucursales.AsNoTracking().ToList();
                    
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (listViewItems != null)
                        {
                            listViewItems.Items.Clear();
                            foreach (var itemData in items)
                            {
                                var articulo = articulos.FirstOrDefault(a => a.Id == itemData.ArticuloId);
                                var sucursal = !string.IsNullOrEmpty(itemData.SucursalId) ? 
                                    sucursales.FirstOrDefault(s => s.Id == itemData.SucursalId) : null;
                                
                                var item = new ListViewItem(itemData.Serial);
                                item.SubItems.Add(articulo != null ? $"{articulo.Marca} {articulo.Modelo}" : "N/A");
                                item.SubItems.Add(itemData.Estado ?? "N/A");
                                item.SubItems.Add(sucursal?.Nombre ?? "Almacén Central");
                                
                                var garantiaTexto = itemData.FechaGarantiaVencimiento.HasValue ? 
                                    (itemData.FechaGarantiaVencimiento.Value > DateTime.Now ? 
                                        $"Hasta {itemData.FechaGarantiaVencimiento.Value:dd/MM/yyyy}" : "Vencida") 
                                    : "N/A";
                                item.SubItems.Add(garantiaTexto);
                                
                                // Color según estado
                                if (itemData.Estado == "Disponible")
                                    item.ForeColor = Color.Green;
                                else if (itemData.Estado == "En Reparación")
                                    item.ForeColor = Color.Orange;
                                else if (itemData.Estado == "Dañado" || itemData.Estado == "Dado de Baja")
                                    item.ForeColor = Color.Red;
                                    
                                item.Tag = itemData.Serial;
                                listViewItems.Items.Add(item);
                            }
                        }
                    });
                }
            });
        }
        
        private async Task CargarRegionesAsync()
        {
            await Task.Run(() =>
            {
                using (var context = new PSDatos())
                {
                    var regiones = context.Regiones.AsNoTracking().ToList();
                    
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (listViewRegiones != null)
                        {
                            listViewRegiones.Items.Clear();
                            foreach (var region in regiones)
                            {
                                var item = new ListViewItem(region.Id.ToString());
                                item.SubItems.Add(region.Nombre);
                                item.SubItems.Add(region.Activo ? "Sí" : "No");
                                
                                if (!region.Activo)
                                    item.ForeColor = Color.Gray;
                                    
                                item.Tag = region.Id;
                                listViewRegiones.Items.Add(item);
                            }
                        }
                    });
                }
            });
        }
        
        private async Task CargarSucursalesAsync()
        {
            await Task.Run(() =>
            {
                using (var context = new PSDatos())
                {
                    var sucursales = context.Sucursales.AsNoTracking().ToList();
                    var regiones = context.Regiones.AsNoTracking().ToList();
                    
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (listViewSucursales != null)
                        {
                            listViewSucursales.Items.Clear();
                            foreach (var sucursal in sucursales)
                            {
                                var region = regiones.FirstOrDefault(r => r.Id == sucursal.RegionId);
                                
                                var item = new ListViewItem(sucursal.Id);
                                item.SubItems.Add(sucursal.Nombre);
                                item.SubItems.Add(region?.Nombre ?? "Sin región");
                                item.SubItems.Add(sucursal.Activo ? "Sí" : "No");
                                
                                if (!sucursal.Activo)
                                    item.ForeColor = Color.Gray;
                                    
                                item.Tag = sucursal.Id;
                                listViewSucursales.Items.Add(item);
                            }
                        }
                    });
                }
            });
        }

        private void Menu_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Login login = serviceProvider.GetRequiredService<Login>();
            //login.Show();

            this.Hide();
            this.Dispose();

        }

        private void materialButton1_Click(object sender, EventArgs e)
        {

        }

        public void cerrarSesion()
        {
            MaterialDialog dialog = new MaterialDialog(this, "Cerrar Sesion", "¿Está seguro de que desea cerrar sesión?", "Aceotar", true, "Cancelar");
            var resultado = dialog.ShowDialog(this);
            if (resultado == DialogResult.OK)
            {
                Login login = new Login();
                login.Show();
                this.Hide();
                this.Dispose();
            }

        }

        private void OptionControls_Click(object sender, EventArgs e)
        {

        }

        private void cerrarSe_Enter(object sender, EventArgs e)
        {

        }

        private void OptionControls_SelectedIndexChanged(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Text == "Cerrar Sesion")
            {
                //Question the user if they want to close the application
                MaterialDialog dialog = new MaterialDialog(this, "Cerrar Aplicación", "¿Está seguro de que desea cerrar la aplicación?", "Aceptar", true, "Cancelar");
                var resultado = dialog.ShowDialog(this);
                if (resultado == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
        }

        private void Compras_Click(object sender, EventArgs e)
        {

        }

        private void OptionControls_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void OptionControls_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            if (tabInventarios.SelectedIndex == 0) // Compras
            {
                Compras compras = new Compras();
                var resultado = compras.ShowDialog(this);
                if (resultado == DialogResult.OK)
                {
                    Task.Run(() => CargarComprasAsync());
                    MessageBox.Show("Compra agregada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (tabInventarios.SelectedIndex == 1) // Artículos
            {
                Articulos articulos = new Articulos();
                var resultado = articulos.ShowDialog(this);
                if (resultado == DialogResult.OK)
                {
                    Task.Run(() => CargarArticulosAsync());
                    MessageBox.Show("Artículo agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (tabInventarios.SelectedIndex == 2) // Categorías
            {
                Categorias categorias = new Categorias();
                var resultado = categorias.ShowDialog(this);
                if (resultado == DialogResult.OK)
                {
                    Task.Run(() => CargarCategoriasAsync());
                    MessageBox.Show("Categoría agregada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (tabInventarios.SelectedIndex == 3) // Items
            {
                Items items = new Items();
                var resultado = items.ShowDialog(this);
                if (resultado == DialogResult.OK)
                {
                    Task.Run(() => CargarItemsAsync());
                    MessageBox.Show("Item agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (tabInventarios.SelectedIndex == 4) // Regiones
            {
                Regiones regiones = new Regiones();
                var resultado = regiones.ShowDialog(this);
                if (resultado == DialogResult.OK)
                {
                    Task.Run(() => CargarRegionesAsync());
                    MessageBox.Show("Región agregada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (tabInventarios.SelectedIndex == 5) // Sucursales
            {
                Sucursales sucursales = new Sucursales();
                var resultado = sucursales.ShowDialog(this);
                if (resultado == DialogResult.OK)
                {
                    Task.Run(() => CargarSucursalesAsync());
                    MessageBox.Show("Sucursal agregada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void tabInventarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Los datos ya se cargan automáticamente al inicio
            // Este evento puede permanecer vacío o recargar si es necesario
        }

        private void tabInventarios_Selected(object sender, TabControlEventArgs e)
        {

        }

        private void ArticulosTab_Click(object sender, EventArgs e)
        {

        }
        
        // Event handlers para botones de Editar
        private void btnEditarCompra_Click(object sender, EventArgs e)
        {
            if (listViewCompras?.SelectedItems.Count > 0)
            {
                int id = (int)listViewCompras.SelectedItems[0].Tag;
                Compras form = new Compras(id);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Task.Run(() => CargarComprasAsync());
                }
            }
        }
        
        private void btnEditarArticulo_Click(object sender, EventArgs e)
        {
            if (listViewArticulos?.SelectedItems.Count > 0)
            {
                int id = (int)listViewArticulos.SelectedItems[0].Tag;
                Articulos form = new Articulos(id);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Task.Run(() => CargarArticulosAsync());
                }
            }
        }
        
        private void btnEditarCategoria_Click(object sender, EventArgs e)
        {
            if (listViewCategorias?.SelectedItems.Count > 0)
            {
                int id = (int)listViewCategorias.SelectedItems[0].Tag;
                Categorias form = new Categorias(id);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Task.Run(() => CargarCategoriasAsync());
                }
            }
        }
        
        private void btnEditarItem_Click(object sender, EventArgs e)
        {
            if (listViewItems?.SelectedItems.Count > 0)
            {
                string numSerie = (string)listViewItems.SelectedItems[0].Tag;
                Items form = new Items(numSerie);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Task.Run(() => CargarItemsAsync());
                }
            }
        }
        
        private void btnEditarRegion_Click(object sender, EventArgs e)
        {
            if (listViewRegiones?.SelectedItems.Count > 0)
            {
                int id = (int)listViewRegiones.SelectedItems[0].Tag;
                Regiones form = new Regiones(id);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Task.Run(() => CargarRegionesAsync());
                }
            }
        }
        
        private void btnEditarSucursal_Click(object sender, EventArgs e)
        {
            if (listViewSucursales?.SelectedItems.Count > 0)
            {
                int id = (int)listViewSucursales.SelectedItems[0].Tag;
                Sucursales form = new Sucursales(id.ToString());
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Task.Run(() => CargarSucursalesAsync());
                }
            }
        }
        
        // Event handlers para botones de Eliminar
        private void btnEliminarCompra_Click(object sender, EventArgs e)
        {
            if (listViewCompras?.SelectedItems.Count > 0)
            {
                int id = (int)listViewCompras.SelectedItems[0].Tag;
                var resultado = MessageBox.Show("¿Está seguro de eliminar esta compra?", "Confirmar", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultado == DialogResult.Yes)
                {
                    try
                    {
                        using (var context = new PSDatos())
                        {
                            var compra = context.Compras.Find(id);
                            if (compra != null)
                            {
                                context.Compras.Remove(compra);
                                context.SaveChanges();
                                Task.Run(() => CargarComprasAsync());
                                MessageBox.Show("Compra eliminada exitosamente.", "Éxito", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void btnEliminarArticulo_Click(object sender, EventArgs e)
        {
            if (listViewArticulos?.SelectedItems.Count > 0)
            {
                int id = (int)listViewArticulos.SelectedItems[0].Tag;
                
                // Verificar si tiene items asociados
                using (var context = new PSDatos())
                {
                    int itemsAsociados = context.Items.Count(i => i.ArticuloId == id);
                    if (itemsAsociados > 0)
                    {
                        MessageBox.Show($"No se puede eliminar. Este artículo tiene {itemsAsociados} items asociados.", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                
                var resultado = MessageBox.Show("¿Está seguro de eliminar este artículo?", "Confirmar", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultado == DialogResult.Yes)
                {
                    try
                    {
                        using (var context = new PSDatos())
                        {
                            var articulo = context.Articulos.Find(id);
                            if (articulo != null)
                            {
                                context.Articulos.Remove(articulo);
                                context.SaveChanges();
                                Task.Run(() => CargarArticulosAsync());
                                MessageBox.Show("Artículo eliminado exitosamente.", "Éxito", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void btnEliminarCategoria_Click(object sender, EventArgs e)
        {
            if (listViewCategorias?.SelectedItems.Count > 0)
            {
                int id = (int)listViewCategorias.SelectedItems[0].Tag;
                
                // Verificar si tiene artículos asociados
                using (var context = new PSDatos())
                {
                    int articulosAsociados = context.Articulos.Count(a => a.CategoriaId == id);
                    if (articulosAsociados > 0)
                    {
                        MessageBox.Show($"No se puede eliminar. Esta categoría tiene {articulosAsociados} artículos asociados.", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                
                var resultado = MessageBox.Show("¿Está seguro de eliminar esta categoría?", "Confirmar", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultado == DialogResult.Yes)
                {
                    try
                    {
                        using (var context = new PSDatos())
                        {
                            var categoria = context.Categorias.Find(id);
                            if (categoria != null)
                            {
                                context.Categorias.Remove(categoria);
                                context.SaveChanges();
                                Task.Run(() => CargarCategoriasAsync());
                                MessageBox.Show("Categoría eliminada exitosamente.", "Éxito", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void btnEliminarItem_Click(object sender, EventArgs e)
        {
            if (listViewItems?.SelectedItems.Count > 0)
            {
                string serial = (string)listViewItems.SelectedItems[0].Tag;
                
                // Verificar si tiene movimientos asociados
                using (var context = new PSDatos())
                {
                    int movimientos = context.MovimientosItem.Count(m => m.ItemSerial == serial);
                    if (movimientos > 0)
                    {
                        MessageBox.Show($"No se puede eliminar. Este item tiene {movimientos} movimientos registrados.", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                
                var resultado = MessageBox.Show("¿Está seguro de eliminar este item?", "Confirmar", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultado == DialogResult.Yes)
                {
                    try
                    {
                        using (var context = new PSDatos())
                        {
                            var item = context.Items.Find(serial);
                            if (item != null)
                            {
                                context.Items.Remove(item);
                                context.SaveChanges();
                                Task.Run(() => CargarItemsAsync());
                                MessageBox.Show("Item eliminado exitosamente.", "Éxito", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void btnEliminarRegion_Click(object sender, EventArgs e)
        {
            if (listViewRegiones?.SelectedItems.Count > 0)
            {
                int id = (int)listViewRegiones.SelectedItems[0].Tag;
                
                // Verificar si tiene sucursales asociadas
                using (var context = new PSDatos())
                {
                    int sucursalesAsociadas = context.Sucursales.Count(s => s.RegionId == id);
                    if (sucursalesAsociadas > 0)
                    {
                        MessageBox.Show($"No se puede eliminar. Esta región tiene {sucursalesAsociadas} sucursales asociadas.", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                
                var resultado = MessageBox.Show("¿Está seguro de eliminar esta región?", "Confirmar", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultado == DialogResult.Yes)
                {
                    try
                    {
                        using (var context = new PSDatos())
                        {
                            var region = context.Regiones.Find(id);
                            if (region != null)
                            {
                                context.Regiones.Remove(region);
                                context.SaveChanges();
                                Task.Run(() => CargarRegionesAsync());
                                MessageBox.Show("Región eliminada exitosamente.", "Éxito", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void btnEliminarSucursal_Click(object sender, EventArgs e)
        {
            if (listViewSucursales?.SelectedItems.Count > 0)
            {
                string id = (string)listViewSucursales.SelectedItems[0].Tag;
                
                // Verificar si tiene items asociados
                using (var context = new PSDatos())
                {
                    int itemsAsociados = context.Items.Count(i => i.SucursalId == id);
                    if (itemsAsociados > 0)
                    {
                        MessageBox.Show($"No se puede eliminar. Esta sucursal tiene {itemsAsociados} items asignados.", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                
                var resultado = MessageBox.Show("¿Está seguro de eliminar esta sucursal?", "Confirmar", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultado == DialogResult.Yes)
                {
                    try
                    {
                        using (var context = new PSDatos())
                        {
                            var sucursal = context.Sucursales.Find(id);
                            if (sucursal != null)
                            {
                                context.Sucursales.Remove(sucursal);
                                context.SaveChanges();
                                Task.Run(() => CargarSucursalesAsync());
                                MessageBox.Show("Sucursal eliminada exitosamente.", "Éxito", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
