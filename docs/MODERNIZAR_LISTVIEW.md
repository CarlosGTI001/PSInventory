# Modernizar UI con MaterialListView

## 🎨 Problema Actual
Los **DataGridView** son antiguos y no siguen el diseño Material Design.

**MaterialSkin 2** ofrece **MaterialListView** que se ve mucho más moderno y es touch-friendly.

---

## ✨ MaterialListView vs DataGridView

### DataGridView (Antiguo)
```
┌─────────────────────────────────────────┐
│ ID  │ Nombre    │ Marca  │ Modelo      │
├─────────────────────────────────────────┤
│ 1   │ PC Gamer  │ Dell   │ Alienware   │
│ 2   │ Laptop    │ HP     │ Pavilion    │
└─────────────────────────────────────────┘
```

### MaterialListView (Moderno)
```
┌─────────────────────────────────────────┐
│ 🖥️  PC Gamer                            │
│    Dell Alienware                       │
│    Stock: 5 | Estado: Disponible       │
├─────────────────────────────────────────┤
│ 💻  Laptop                              │
│    HP Pavilion                          │
│    Stock: 3 | Estado: Disponible       │
└─────────────────────────────────────────┘
```

---

## 📝 Ejemplo: Reemplazo en Menu.cs

### 1. Reemplazar DataGridView por MaterialListView

**ANTES (DataGridView):**
```csharp
private DataGridView dataGridArticulos;
dataGridArticulos.DataSource = db.Articulos.ToList();
```

**DESPUÉS (MaterialListView):**
```csharp
private MaterialListView listViewArticulos;

// En InitializeComponent o constructor:
listViewArticulos = new MaterialListView
{
    FullRowSelect = true,
    AutoSize = false,
    View = System.Windows.Forms.View.Details,
    Dock = DockStyle.Fill,
    BorderStyle = BorderStyle.None
};

// Definir columnas
listViewArticulos.Columns.Add("ID", 60);
listViewArticulos.Columns.Add("Marca", 150);
listViewArticulos.Columns.Add("Modelo", 150);
listViewArticulos.Columns.Add("Categoría", 120);
listViewArticulos.Columns.Add("Stock", 80);
```

---

### 2. Cargar Datos en MaterialListView

**Método para cargar artículos:**
```csharp
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
                    .Include(a => a.Categoria)  // Cargar relación
                    .ToList();

                this.Invoke(new Action(() =>
                {
                    listViewArticulos.Items.Clear();
                    
                    foreach (var art in articulos)
                    {
                        var item = new ListViewItem(art.Id.ToString());
                        item.SubItems.Add(art.Marca);
                        item.SubItems.Add(art.Modelo);
                        item.SubItems.Add(art.Categoria?.Nombre ?? "Sin categoría");
                        
                        // Contar items disponibles
                        int stock = db.Items.Count(i => i.ArticuloId == art.Id && i.Estado == "Disponible");
                        item.SubItems.Add(stock.ToString());
                        
                        // Guardar el objeto completo en Tag
                        item.Tag = art;
                        
                        // Color de alerta si stock bajo
                        if (stock < art.StockMinimo)
                        {
                            item.ForeColor = Color.Red;
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
```

---

### 3. Eventos del MaterialListView

**Doble clic para editar:**
```csharp
private void listViewArticulos_DoubleClick(object sender, EventArgs e)
{
    if (listViewArticulos.SelectedItems.Count > 0)
    {
        var articulo = (Articulo)listViewArticulos.SelectedItems[0].Tag;
        
        Articulos formEditar = new Articulos(articulo.Id);
        if (formEditar.ShowDialog(this) == DialogResult.OK)
        {
            await LoadArticulosAsync();  // Recargar
        }
    }
}
```

**Clic derecho para menú contextual:**
```csharp
private void listViewArticulos_MouseClick(object sender, MouseEventArgs e)
{
    if (e.Button == MouseButtons.Right && listViewArticulos.SelectedItems.Count > 0)
    {
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Editar", null, MenuEditar_Click);
        contextMenu.Items.Add("Eliminar", null, MenuEliminar_Click);
        contextMenu.Items.Add("-");  // Separador
        contextMenu.Items.Add("Ver Items", null, MenuVerItems_Click);
        
        contextMenu.Show(listViewArticulos, e.Location);
    }
}

private async void MenuEditar_Click(object sender, EventArgs e)
{
    // Similar a doble clic
}

private async void MenuEliminar_Click(object sender, EventArgs e)
{
    if (listViewArticulos.SelectedItems.Count > 0)
    {
        var articulo = (Articulo)listViewArticulos.SelectedItems[0].Tag;
        
        var resultado = MaterialMessageBox.Show(
            $"¿Está seguro de eliminar {articulo.Marca} {articulo.Modelo}?",
            "Confirmar Eliminación",
            MessageBoxButtons.YesNo,
            false,
            FlexibleMaterialForm.ButtonsPosition.Center
        );
        
        if (resultado == DialogResult.Yes)
        {
            loadingHelper.Show("Eliminando...");
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        var art = db.Articulos.Find(articulo.Id);
                        if (art != null)
                        {
                            db.Articulos.Remove(art);
                            db.SaveChanges();
                        }
                    }
                });
                
                await LoadArticulosAsync();  // Recargar
                MessageBox.Show("Artículo eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show(
                    "No se puede eliminar porque tiene items asociados: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    false,
                    FlexibleMaterialForm.ButtonsPosition.Center
                );
            }
            finally
            {
                loadingHelper.Hide();
            }
        }
    }
}
```

---

## 🎨 Personalización Avanzada

### ListView con Iconos
```csharp
// Crear ImageList
var imageList = new ImageList();
imageList.ImageSize = new Size(48, 48);
imageList.Images.Add("pc", Properties.Resources.IconoPC);
imageList.Images.Add("teclado", Properties.Resources.IconoTeclado);
imageList.Images.Add("mouse", Properties.Resources.IconoMouse);

listViewArticulos.SmallImageList = imageList;

// Al agregar items:
var item = new ListViewItem(art.Marca + " " + art.Modelo);
item.ImageKey = ObtenerIconoPorCategoria(art.CategoriaId);
```

### ListView tipo Cards (Vista grande)
```csharp
listViewArticulos.View = View.LargeIcon;
listViewArticulos.LargeImageList = imageList;

// Se verá como tarjetas con iconos grandes
```

### Colores alternados en filas
```csharp
int index = 0;
foreach (var art in articulos)
{
    var item = new ListViewItem(art.Id.ToString());
    // ... agregar subitems ...
    
    if (index % 2 == 0)
    {
        item.BackColor = Color.FromArgb(250, 250, 250);
    }
    
    listViewArticulos.Items.Add(item);
    index++;
}
```

---

## 📊 Comparativa Completa

| Característica | DataGridView | MaterialListView |
|----------------|--------------|------------------|
| Estilo Material | ❌ | ✅ |
| Touch-friendly | ❌ | ✅ |
| Iconos | Complicado | ✅ Fácil |
| Colores personalizados | ❌ Limitado | ✅ Total |
| Performance | Lento con >1000 filas | Rápido |
| Edición inline | ✅ | ❌ |
| Selección múltiple | ✅ | ✅ |
| Ordenar columnas | ✅ Automático | Manual |

---

## 🚀 Implementación Sugerida

### Opción 1: Reemplazo Total (Recomendado)
Reemplazar todos los DataGridView por MaterialListView en:
- Tab Compras
- Tab Artículos
- Tab Categorías

### Opción 2: Híbrido
- Usar MaterialListView para **listas de selección**
- Mantener DataGridView solo donde necesites **edición inline**

---

## 📝 Plantilla Completa

```csharp
// 1. Declarar en el form
private MaterialListView listView;

// 2. Inicializar
private void InicializarListView()
{
    listView = new MaterialListView
    {
        FullRowSelect = true,
        MultiSelect = false,
        View = View.Details,
        Dock = DockStyle.Fill,
        BorderStyle = BorderStyle.None
    };
    
    // Columnas
    listView.Columns.Add("ID", 60);
    listView.Columns.Add("Nombre", 200);
    listView.Columns.Add("Estado", 100);
    
    // Eventos
    listView.DoubleClick += ListView_DoubleClick;
    listView.MouseClick += ListView_MouseClick;
    
    // Agregar al contenedor
    panelContenedor.Controls.Add(listView);
}

// 3. Cargar datos
private async Task CargarDatos()
{
    await Task.Run(() =>
    {
        using (var db = new PSDatos())
        {
            var datos = db.Entidades.AsNoTracking().ToList();
            
            this.Invoke(new Action(() =>
            {
                listView.Items.Clear();
                foreach (var dato in datos)
                {
                    var item = new ListViewItem(dato.Id.ToString());
                    item.SubItems.Add(dato.Nombre);
                    item.SubItems.Add(dato.Estado);
                    item.Tag = dato;
                    listView.Items.Add(item);
                }
            }));
        }
    });
}

// 4. Eventos
private void ListView_DoubleClick(object sender, EventArgs e)
{
    if (listView.SelectedItems.Count > 0)
    {
        var entidad = (Entidad)listView.SelectedItems[0].Tag;
        // Abrir form editar
    }
}
```

---

**¿Quieres que implemente el MaterialListView en Menu.cs para algún tab específico como ejemplo?**
