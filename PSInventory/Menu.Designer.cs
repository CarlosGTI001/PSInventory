namespace PSInventory
{
    partial class Menu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            OptionControls = new MaterialSkin.Controls.MaterialTabControl();
            inicioPage = new TabPage();
            pictureBox1 = new PictureBox();
            inventarioPage = new TabPage();
            menuInventario = new MaterialSkin.Controls.MaterialTabSelector();
            tabInventarios = new MaterialSkin.Controls.MaterialTabControl();
            ComprasTab = new TabPage();
            dataGridCompras = new DataGridView();
            idDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            tiendaDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            costoTotalDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            compraBindingSource = new BindingSource(components);
            ArticulosTab = new TabPage();
            dataGridArticulos = new DataGridView();
            idDataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            marcaDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            modeloDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            articuloBindingSource = new BindingSource(components);
            CategoriasTab = new TabPage();
            dataGridCategorias = new DataGridView();
            idDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            nombreDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            descripcionDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            categoriaBindingSource = new BindingSource(components);
            ingresarPage = new TabPage();
            salidaPage = new TabPage();
            sucursalesPage = new TabPage();
            regionesPage = new TabPage();
            cerrarSe = new TabPage();
            menuImg = new ImageList(components);
            MenuDrawer = new MaterialSkin.Controls.MaterialDrawer();
            compraBindingSource1 = new BindingSource(components);
            addBtn = new MaterialSkin.Controls.MaterialFloatingActionButton();
            OptionControls.SuspendLayout();
            inicioPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            inventarioPage.SuspendLayout();
            tabInventarios.SuspendLayout();
            ComprasTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridCompras).BeginInit();
            ((System.ComponentModel.ISupportInitialize)compraBindingSource).BeginInit();
            ArticulosTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridArticulos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)articuloBindingSource).BeginInit();
            CategoriasTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridCategorias).BeginInit();
            ((System.ComponentModel.ISupportInitialize)categoriaBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)compraBindingSource1).BeginInit();
            SuspendLayout();
            // 
            // OptionControls
            // 
            OptionControls.Controls.Add(inicioPage);
            OptionControls.Controls.Add(inventarioPage);
            OptionControls.Controls.Add(ingresarPage);
            OptionControls.Controls.Add(salidaPage);
            OptionControls.Controls.Add(sucursalesPage);
            OptionControls.Controls.Add(regionesPage);
            OptionControls.Controls.Add(cerrarSe);
            OptionControls.Depth = 0;
            // ImageList deshabilitado para evitar errores
            // OptionControls.ImageList = menuImg;
            OptionControls.Location = new Point(0, 64);
            OptionControls.MaximumSize = new Size(838, 511);
            OptionControls.MinimumSize = new Size(838, 511);
            OptionControls.MouseState = MaterialSkin.MouseState.HOVER;
            OptionControls.Multiline = true;
            OptionControls.Name = "OptionControls";
            OptionControls.Padding = new Point(0, 0);
            OptionControls.SelectedIndex = 0;
            OptionControls.Size = new Size(838, 511);
            OptionControls.TabIndex = 1;
            OptionControls.SelectedIndexChanged += OptionControls_SelectedIndexChanged_1;
            OptionControls.Click += OptionControls_Click;
            // 
            // inicioPage
            // 
            inicioPage.Controls.Add(pictureBox1);
            inicioPage.ImageKey = "dashboard.png";
            inicioPage.Location = new Point(4, 31);
            inicioPage.Name = "inicioPage";
            inicioPage.Padding = new Padding(3);
            inicioPage.Size = new Size(830, 476);
            inicioPage.TabIndex = 0;
            inicioPage.Text = "Inicio";
            inicioPage.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Logo_Presidente_Sports;
            pictureBox1.Location = new Point(230, 77);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(349, 337);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // inventarioPage
            // 
            inventarioPage.Controls.Add(menuInventario);
            inventarioPage.Controls.Add(tabInventarios);
            inventarioPage.ImageKey = "warehouse.png";
            inventarioPage.Location = new Point(4, 31);
            inventarioPage.Name = "inventarioPage";
            inventarioPage.Padding = new Padding(3);
            inventarioPage.Size = new Size(830, 476);
            inventarioPage.TabIndex = 1;
            inventarioPage.Text = "Inventario";
            inventarioPage.UseVisualStyleBackColor = true;
            // 
            // menuInventario
            // 
            menuInventario.BaseTabControl = tabInventarios;
            menuInventario.CharacterCasing = MaterialSkin.Controls.MaterialTabSelector.CustomCharacterCasing.Normal;
            menuInventario.Depth = 0;
            menuInventario.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            menuInventario.Location = new Point(40, -21);
            menuInventario.MouseState = MaterialSkin.MouseState.HOVER;
            menuInventario.Name = "menuInventario";
            menuInventario.Size = new Size(792, 49);
            menuInventario.TabIndex = 1;
            menuInventario.Text = "materialTabSelector1";
            // 
            // tabInventarios
            // 
            tabInventarios.Controls.Add(ComprasTab);
            tabInventarios.Controls.Add(ArticulosTab);
            tabInventarios.Controls.Add(CategoriasTab);
            tabInventarios.Depth = 0;
            tabInventarios.Dock = DockStyle.Bottom;
            tabInventarios.Location = new Point(3, 26);
            tabInventarios.MouseState = MaterialSkin.MouseState.HOVER;
            tabInventarios.Multiline = true;
            tabInventarios.Name = "tabInventarios";
            tabInventarios.SelectedIndex = 0;
            tabInventarios.Size = new Size(824, 447);
            tabInventarios.TabIndex = 0;
            tabInventarios.SelectedIndexChanged += tabInventarios_SelectedIndexChanged;
            tabInventarios.Selected += tabInventarios_Selected;
            // 
            // ComprasTab
            // 
            ComprasTab.Controls.Add(dataGridCompras);
            ComprasTab.Location = new Point(4, 24);
            ComprasTab.Name = "ComprasTab";
            ComprasTab.Padding = new Padding(3);
            ComprasTab.Size = new Size(816, 419);
            ComprasTab.TabIndex = 0;
            ComprasTab.Text = "Compras";
            ComprasTab.UseVisualStyleBackColor = true;
            ComprasTab.Click += Compras_Click;
            // 
            // dataGridCompras
            // 
            dataGridCompras.AllowUserToAddRows = false;
            dataGridCompras.AllowUserToDeleteRows = false;
            dataGridCompras.AutoGenerateColumns = false;
            dataGridCompras.BackgroundColor = Color.FromArgb(0, 46, 79);
            dataGridCompras.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridCompras.Columns.AddRange(new DataGridViewColumn[] { idDataGridViewTextBoxColumn, tiendaDataGridViewTextBoxColumn, costoTotalDataGridViewTextBoxColumn });
            dataGridCompras.DataSource = compraBindingSource;
            dataGridCompras.Location = new Point(35, 57);
            dataGridCompras.Name = "dataGridCompras";
            dataGridCompras.ReadOnly = true;
            dataGridCompras.Size = new Size(792, 359);
            dataGridCompras.TabIndex = 0;
            // 
            // idDataGridViewTextBoxColumn
            // 
            idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            idDataGridViewTextBoxColumn.HeaderText = "Codigo";
            idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            idDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tiendaDataGridViewTextBoxColumn
            // 
            tiendaDataGridViewTextBoxColumn.DataPropertyName = "Tienda";
            tiendaDataGridViewTextBoxColumn.HeaderText = "Tienda";
            tiendaDataGridViewTextBoxColumn.Name = "tiendaDataGridViewTextBoxColumn";
            tiendaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // costoTotalDataGridViewTextBoxColumn
            // 
            costoTotalDataGridViewTextBoxColumn.DataPropertyName = "costoTotal";
            costoTotalDataGridViewTextBoxColumn.HeaderText = "Costo Total";
            costoTotalDataGridViewTextBoxColumn.Name = "costoTotalDataGridViewTextBoxColumn";
            costoTotalDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // compraBindingSource
            // 
            compraBindingSource.DataSource = typeof(PSData.Modelos.Compra);
            // 
            // ArticulosTab
            // 
            ArticulosTab.Controls.Add(dataGridArticulos);
            ArticulosTab.Location = new Point(4, 24);
            ArticulosTab.Name = "ArticulosTab";
            ArticulosTab.Padding = new Padding(3);
            ArticulosTab.Size = new Size(816, 419);
            ArticulosTab.TabIndex = 1;
            ArticulosTab.Text = "Articulos";
            ArticulosTab.UseVisualStyleBackColor = true;
            ArticulosTab.Click += ArticulosTab_Click;
            // 
            // dataGridArticulos
            // 
            dataGridArticulos.AllowUserToAddRows = false;
            dataGridArticulos.AllowUserToDeleteRows = false;
            dataGridArticulos.AutoGenerateColumns = false;
            dataGridArticulos.BackgroundColor = SystemColors.ControlLightLight;
            dataGridArticulos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridArticulos.Columns.AddRange(new DataGridViewColumn[] { idDataGridViewTextBoxColumn2, marcaDataGridViewTextBoxColumn, modeloDataGridViewTextBoxColumn });
            dataGridArticulos.DataSource = articuloBindingSource;
            dataGridArticulos.Location = new Point(35, 60);
            dataGridArticulos.Name = "dataGridArticulos";
            dataGridArticulos.ReadOnly = true;
            dataGridArticulos.Size = new Size(792, 370);
            dataGridArticulos.TabIndex = 1;
            // 
            // idDataGridViewTextBoxColumn2
            // 
            idDataGridViewTextBoxColumn2.DataPropertyName = "Id";
            idDataGridViewTextBoxColumn2.HeaderText = "Codigo";
            idDataGridViewTextBoxColumn2.Name = "idDataGridViewTextBoxColumn2";
            idDataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // marcaDataGridViewTextBoxColumn
            // 
            marcaDataGridViewTextBoxColumn.DataPropertyName = "marca";
            marcaDataGridViewTextBoxColumn.HeaderText = "Marca";
            marcaDataGridViewTextBoxColumn.Name = "marcaDataGridViewTextBoxColumn";
            marcaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // modeloDataGridViewTextBoxColumn
            // 
            modeloDataGridViewTextBoxColumn.DataPropertyName = "modelo";
            modeloDataGridViewTextBoxColumn.HeaderText = "Modelo";
            modeloDataGridViewTextBoxColumn.Name = "modeloDataGridViewTextBoxColumn";
            modeloDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // articuloBindingSource
            // 
            articuloBindingSource.DataSource = typeof(PSData.Modelos.Articulo);
            // 
            // CategoriasTab
            // 
            CategoriasTab.Controls.Add(dataGridCategorias);
            CategoriasTab.Location = new Point(4, 24);
            CategoriasTab.Name = "CategoriasTab";
            CategoriasTab.Size = new Size(816, 419);
            CategoriasTab.TabIndex = 2;
            CategoriasTab.Text = "Categorias";
            CategoriasTab.UseVisualStyleBackColor = true;
            // 
            // dataGridCategorias
            // 
            dataGridCategorias.AllowUserToAddRows = false;
            dataGridCategorias.AllowUserToDeleteRows = false;
            dataGridCategorias.AutoGenerateColumns = false;
            dataGridCategorias.BackgroundColor = SystemColors.ControlLightLight;
            dataGridCategorias.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridCategorias.Columns.AddRange(new DataGridViewColumn[] { idDataGridViewTextBoxColumn1, nombreDataGridViewTextBoxColumn, descripcionDataGridViewTextBoxColumn });
            dataGridCategorias.DataSource = categoriaBindingSource;
            dataGridCategorias.Location = new Point(33, 57);
            dataGridCategorias.Name = "dataGridCategorias";
            dataGridCategorias.ReadOnly = true;
            dataGridCategorias.Size = new Size(796, 373);
            dataGridCategorias.TabIndex = 1;
            // 
            // idDataGridViewTextBoxColumn1
            // 
            idDataGridViewTextBoxColumn1.DataPropertyName = "Id";
            idDataGridViewTextBoxColumn1.HeaderText = "Id";
            idDataGridViewTextBoxColumn1.Name = "idDataGridViewTextBoxColumn1";
            idDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // nombreDataGridViewTextBoxColumn
            // 
            nombreDataGridViewTextBoxColumn.DataPropertyName = "Nombre";
            nombreDataGridViewTextBoxColumn.HeaderText = "Nombre";
            nombreDataGridViewTextBoxColumn.Name = "nombreDataGridViewTextBoxColumn";
            nombreDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // descripcionDataGridViewTextBoxColumn
            // 
            descripcionDataGridViewTextBoxColumn.DataPropertyName = "Descripcion";
            descripcionDataGridViewTextBoxColumn.HeaderText = "Descripcion";
            descripcionDataGridViewTextBoxColumn.Name = "descripcionDataGridViewTextBoxColumn";
            descripcionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // categoriaBindingSource
            // 
            categoriaBindingSource.DataSource = typeof(PSData.Modelos.Categoria);
            // 
            // ingresarPage
            // 
            ingresarPage.ImageKey = "login.png";
            ingresarPage.Location = new Point(4, 31);
            ingresarPage.Name = "ingresarPage";
            ingresarPage.Size = new Size(830, 476);
            ingresarPage.TabIndex = 2;
            ingresarPage.Text = "Entrada Inventario";
            ingresarPage.UseVisualStyleBackColor = true;
            // 
            // salidaPage
            // 
            salidaPage.ImageKey = "logout.png";
            salidaPage.ImeMode = ImeMode.On;
            salidaPage.Location = new Point(4, 31);
            salidaPage.Name = "salidaPage";
            salidaPage.Size = new Size(830, 476);
            salidaPage.TabIndex = 3;
            salidaPage.Text = "Salida Inventario";
            salidaPage.UseVisualStyleBackColor = true;
            // 
            // sucursalesPage
            // 
            sucursalesPage.ImageKey = "store.png";
            sucursalesPage.Location = new Point(4, 31);
            sucursalesPage.Name = "sucursalesPage";
            sucursalesPage.Size = new Size(830, 476);
            sucursalesPage.TabIndex = 4;
            sucursalesPage.Text = "Puntos de Venta";
            sucursalesPage.UseVisualStyleBackColor = true;
            // 
            // regionesPage
            // 
            regionesPage.ImageKey = "file_map_stack.png";
            regionesPage.Location = new Point(4, 31);
            regionesPage.Name = "regionesPage";
            regionesPage.Size = new Size(830, 476);
            regionesPage.TabIndex = 5;
            regionesPage.Text = "Regiones";
            regionesPage.UseVisualStyleBackColor = true;
            // 
            // cerrarSe
            // 
            cerrarSe.ImageKey = "account_circle_off.png";
            cerrarSe.Location = new Point(4, 31);
            cerrarSe.Name = "cerrarSe";
            cerrarSe.Size = new Size(830, 476);
            cerrarSe.TabIndex = 6;
            cerrarSe.Text = "Cerrar Sesion";
            cerrarSe.UseVisualStyleBackColor = true;
            cerrarSe.Enter += cerrarSe_Enter;
            // 
            // menuImg
            // 
            menuImg.ColorDepth = ColorDepth.Depth32Bit;
            menuImg.ImageStream = (ImageListStreamer)resources.GetObject("menuImg.ImageStream");
            menuImg.TransparentColor = Color.Transparent;
            // Comentado para evitar IndexOutOfRangeException
            // Solo descomentar si las imágenes existen en el ImageList
            /*
            menuImg.Images.SetKeyName(0, "cancel.png");
            menuImg.Images.SetKeyName(1, "logout.png");
            menuImg.Images.SetKeyName(2, "person.png");
            menuImg.Images.SetKeyName(3, "uberMoto.png");
            menuImg.Images.SetKeyName(4, "valija.png");
            menuImg.Images.SetKeyName(5, "valija1.png");
            menuImg.Images.SetKeyName(6, "warehouse.png");
            menuImg.Images.SetKeyName(7, "dashboard.png");
            menuImg.Images.SetKeyName(8, "file_map_stack.png");
            menuImg.Images.SetKeyName(9, "login.png");
            menuImg.Images.SetKeyName(10, "store.png");
            menuImg.Images.SetKeyName(11, "account_circle_off.png");
            */
            // 
            // MenuDrawer - Comentado para evitar errores de iconos
            /*
            MenuDrawer.AutoHide = false;
            MenuDrawer.AutoShow = false;
            MenuDrawer.BackgroundWithAccent = false;
            MenuDrawer.BaseTabControl = OptionControls;
            MenuDrawer.Depth = 0;
            MenuDrawer.HighlightWithAccent = true;
            MenuDrawer.IndicatorWidth = 0;
            MenuDrawer.IsOpen = false;
            MenuDrawer.Location = new Point(-250, 27);
            MenuDrawer.MouseState = MaterialSkin.MouseState.HOVER;
            MenuDrawer.Name = "MenuDrawer";
            MenuDrawer.ShowIconsWhenHidden = false;
            MenuDrawer.Size = new Size(250, 548);
            MenuDrawer.TabIndex = 2;
            MenuDrawer.UseColors = false;
            */
            // 
            // compraBindingSource1
            // 
            compraBindingSource1.DataSource = typeof(PSData.Modelos.Compra);
            // 
            // addBtn
            // 
            addBtn.Depth = 0;
            addBtn.Icon = null;
            addBtn.Location = new Point(771, 508);
            addBtn.MouseState = MaterialSkin.MouseState.HOVER;
            addBtn.Name = "addBtn";
            addBtn.Size = new Size(56, 56);
            addBtn.TabIndex = 3;
            addBtn.Text = "materialFloatingActionButton1";
            addBtn.UseVisualStyleBackColor = true;
            addBtn.Click += addBtn_Click;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(839, 575);
            Controls.Add(addBtn);
            // MenuDrawer deshabilitado temporalmente
            // Controls.Add(MenuDrawer);
            Controls.Add(OptionControls);
            // DrawerShowIconsWhenHidden = true;
            // DrawerTabControl = OptionControls;
            Name = "Menu";
            Padding = new Padding(0, 64, 3, 3);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Menu";
            FormClosed += Menu_FormClosed;
            Load += Menu_Load;
            OptionControls.ResumeLayout(false);
            inicioPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            inventarioPage.ResumeLayout(false);
            tabInventarios.ResumeLayout(false);
            ComprasTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridCompras).EndInit();
            ((System.ComponentModel.ISupportInitialize)compraBindingSource).EndInit();
            ArticulosTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridArticulos).EndInit();
            ((System.ComponentModel.ISupportInitialize)articuloBindingSource).EndInit();
            CategoriasTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridCategorias).EndInit();
            ((System.ComponentModel.ISupportInitialize)categoriaBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)compraBindingSource1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private MaterialSkin.Controls.MaterialTabControl OptionControls;
        private TabPage inicioPage;
        private TabPage inventarioPage;
        private MaterialSkin.Controls.MaterialDrawer MenuDrawer;
        private ImageList menuImg;
        private PictureBox pictureBox1;
        private TabPage ingresarPage;
        private TabPage salidaPage;
        private TabPage sucursalesPage;
        private TabPage regionesPage;
        private TabPage cerrarSe;
        private MaterialSkin.Controls.MaterialTabControl tabInventarios;
        private TabPage ComprasTab;
        private TabPage ArticulosTab;
        private MaterialSkin.Controls.MaterialTabSelector menuInventario;
        private DataGridView dataGridCompras;
        private BindingSource compraBindingSource;
        private TabPage CategoriasTab;
        private DataGridView dataGridArticulos;
        private DataGridView dataGridCategorias;
        private DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn tiendaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn costoTotalDataGridViewTextBoxColumn;
        private BindingSource compraBindingSource1;
        private BindingSource articuloBindingSource;
        private BindingSource categoriaBindingSource;
        private DataGridViewTextBoxColumn idDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn nombreDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn descripcionDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn idDataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn marcaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn modeloDataGridViewTextBoxColumn;
        private MaterialSkin.Controls.MaterialFloatingActionButton addBtn;
    }
}