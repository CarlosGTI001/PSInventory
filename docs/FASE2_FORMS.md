# Fase 2: Formularios CRUD - Documentación

## ✅ Formularios Creados

### 1. **Articulos.cs** ⭐
**Propósito:** Gestión del catálogo de productos (PCs, teclados, mouse, etc.)

**Características:**
- ✅ Crear nuevos artículos
- ✅ Editar artículos existentes
- ✅ Validación de duplicados (marca + modelo)
- ✅ Selección de categoría (ComboBox)
- ✅ Stock mínimo para alertas
- ✅ Especificaciones técnicas en texto largo
- ✅ Loading asíncrono

**Campos:**
- Marca (requerido)
- Modelo (requerido)
- Categoría (requerido)
- Descripción (opcional)
- Stock Mínimo (numérico)
- Especificaciones (multiline, opcional)

**Uso:**
```csharp
// Crear nuevo
Articulos formArticulo = new Articulos();
formArticulo.ShowDialog();

// Editar existente
Articulos formArticulo = new Articulos(articuloId);
formArticulo.ShowDialog();
```

---

### 2. **Compras.cs** 💰
**Propósito:** Registro de compras a proveedores

**Características:**
- ✅ Crear nuevas compras
- ✅ Editar compras existentes
- ✅ Estados: Pendiente, Parcial, Recibida
- ✅ Validación de costo > 0
- ✅ Fecha de compra con DatePicker
- ✅ Loading asíncrono

**Campos:**
- Proveedor (requerido)
- Número de Factura (opcional)
- Costo Total (decimal, requerido)
- Fecha Compra (requerido)
- Estado (ComboBox, requerido)
- Observaciones (opcional)

**Estados:**
- **Pendiente:** Orden realizada pero no recibida
- **Parcial:** Recibida solo parte de la orden
- **Recibida:** Completamente recibida

**Uso:**
```csharp
// Crear nueva compra
Compras formCompra = new Compras();
formCompra.ShowDialog();

// Editar compra existente
Compras formCompra = new Compras(compraId);
formCompra.ShowDialog();
```

---

### 3. **Regiones.cs** 🌍
**Propósito:** Gestión de regiones geográficas

**Características:**
- ✅ Crear nuevas regiones
- ✅ Editar regiones existentes
- ✅ Validación de duplicados (nombre)
- ✅ Campo Activo (checkbox)
- ✅ Loading asíncrono

**Campos:**
- Nombre (requerido)
- Descripción (opcional)
- Activo (checkbox)

**Uso:**
```csharp
// Crear nueva región
Regiones formRegion = new Regiones();
formRegion.ShowDialog();

// Editar región existente
Regiones formRegion = new Regiones(regionId);
formRegion.ShowDialog();
```

**Ejemplos de regiones:**
- Centro
- Norte
- Sur
- Este
- Oeste

---

## 🔗 Integración con Menu.cs

### Botón "Agregar" (`addBtn_Click`)
Ya está conectado para:
- ✅ **Tab 0 (Compras):** Abre formulario `Compras`
- ✅ **Tab 1 (Artículos):** Abre formulario `Articulos`
- ✅ **Tab 2 (Categorías):** Abre formulario `Categorias`

Después de agregar, recarga automáticamente la lista con `LoadXxxAsync()`.

---

## 📝 PENDIENTE

### Formularios Faltantes:
1. **Sucursales.cs** - Para gestionar sucursales
2. **Items.cs** - Para dar de alta items individuales (con serial)

### Funcionalidades Faltantes:
1. **Botón Editar** en DataGrids
2. **Botón Eliminar** en DataGrids
3. **Doble clic para editar** en DataGrids

---

## 🎨 Patrón de Diseño Usado

Todos los formularios siguen el mismo patrón:

```csharp
public partial class FormularioX : MaterialForm
{
    LoadingHelper loadingHelper;
    private int? idEditar = null;  // NULL = crear, con valor = editar

    // Constructor para CREAR
    public FormularioX()
    {
        InitializeComponent();
        loadingHelper = new LoadingHelper(this);
    }

    // Constructor para EDITAR
    public FormularioX(int id) : this()
    {
        idEditar = id;
        CargarDatosAsync(id);
    }

    private async void btnGuardar_Click(object sender, EventArgs e)
    {
        if (!ValidarCampos()) return;
        
        loadingHelper.Show("Guardando...");
        try
        {
            bool exito = await Task.Run(() => {
                using (var db = new PSDatos())
                {
                    if (idEditar.HasValue)
                    {
                        // Modo EDITAR
                        var entidad = db.Entidades.Find(idEditar.Value);
                        entidad.Campo = txtCampo.Text;
                        db.SaveChanges();
                    }
                    else
                    {
                        // Modo CREAR
                        db.Entidades.Add(new Entidad { ... });
                        db.SaveChanges();
                    }
                    return true;
                }
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
}
```

---

## ✨ Ventajas de este Diseño

1. **Reutilización:** Mismo form para Crear y Editar
2. **Asíncrono:** No congela la UI
3. **Loading visual:** Usuario sabe que está trabajando
4. **Using pattern:** Libera conexiones a BD inmediatamente
5. **AsNoTracking:** Consultas de lectura optimizadas
6. **Validaciones:** Antes de guardar
7. **Material Design:** Consistencia visual

---

## 🚀 Siguiente Paso

Crear formularios de:
1. **Sucursales** (con FK a Región)
2. **Items** (con FK a Artículo, Compra, opcional Sucursal)

¿Continúo con estos o prefieres que agregue primero las funcionalidades de Editar/Eliminar en los DataGrids?
