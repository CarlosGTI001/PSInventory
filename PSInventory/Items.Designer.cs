namespace PSInventory
{
    partial class Items
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.txtSerial = new MaterialSkin.Controls.MaterialTextBox();
            this.cmbArticulo = new System.Windows.Forms.ComboBox();
            this.cmbCompra = new System.Windows.Forms.ComboBox();
            this.cmbSucursal = new System.Windows.Forms.ComboBox();
            this.cmbEstado = new System.Windows.Forms.ComboBox();
            this.numCosto = new System.Windows.Forms.NumericUpDown();
            this.txtUbicacion = new MaterialSkin.Controls.MaterialTextBox();
            this.txtResponsable = new MaterialSkin.Controls.MaterialTextBox();
            this.txtObservaciones = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.dtpGarantiaInicio = new System.Windows.Forms.DateTimePicker();
            this.numMesesGarantia = new System.Windows.Forms.NumericUpDown();
            this.btnGuardar = new MaterialSkin.Controls.MaterialButton();
            this.btnCancelar = new MaterialSkin.Controls.MaterialButton();
            this.lblArticulo = new MaterialSkin.Controls.MaterialLabel();
            this.lblCompra = new MaterialSkin.Controls.MaterialLabel();
            this.lblSucursal = new MaterialSkin.Controls.MaterialLabel();
            this.lblEstado = new MaterialSkin.Controls.MaterialLabel();
            this.lblCosto = new MaterialSkin.Controls.MaterialLabel();
            this.lblGarantiaInicio = new MaterialSkin.Controls.MaterialLabel();
            this.lblMesesGarantia = new MaterialSkin.Controls.MaterialLabel();
            ((System.ComponentModel.ISupportInitialize)(this.numCosto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMesesGarantia)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSerial
            // 
            this.txtSerial.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSerial.Depth = 0;
            this.txtSerial.Font = new System.Drawing.Font("Roboto", 12F);
            this.txtSerial.Hint = "Número de Serie (único)";
            this.txtSerial.Location = new System.Drawing.Point(20, 80);
            this.txtSerial.MaxLength = 100;
            this.txtSerial.MouseState = MaterialSkin.MouseState.OUT;
            this.txtSerial.Name = "txtSerial";
            this.txtSerial.Size = new System.Drawing.Size(400, 50);
            this.txtSerial.TabIndex = 0;
            this.txtSerial.Text = "";
            // 
            // cmbArticulo
            // 
            this.cmbArticulo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbArticulo.Font = new System.Drawing.Font("Roboto", 10F);
            this.cmbArticulo.FormattingEnabled = true;
            this.cmbArticulo.Location = new System.Drawing.Point(120, 145);
            this.cmbArticulo.Name = "cmbArticulo";
            this.cmbArticulo.Size = new System.Drawing.Size(300, 28);
            this.cmbArticulo.TabIndex = 1;
            // 
            // cmbCompra
            // 
            this.cmbCompra.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCompra.Font = new System.Drawing.Font("Roboto", 10F);
            this.cmbCompra.FormattingEnabled = true;
            this.cmbCompra.Location = new System.Drawing.Point(120, 185);
            this.cmbCompra.Name = "cmbCompra";
            this.cmbCompra.Size = new System.Drawing.Size(300, 28);
            this.cmbCompra.TabIndex = 2;
            // 
            // cmbSucursal
            // 
            this.cmbSucursal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSucursal.Font = new System.Drawing.Font("Roboto", 10F);
            this.cmbSucursal.FormattingEnabled = true;
            this.cmbSucursal.Location = new System.Drawing.Point(120, 225);
            this.cmbSucursal.Name = "cmbSucursal";
            this.cmbSucursal.Size = new System.Drawing.Size(300, 28);
            this.cmbSucursal.TabIndex = 3;
            // 
            // cmbEstado
            // 
            this.cmbEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstado.Font = new System.Drawing.Font("Roboto", 10F);
            this.cmbEstado.FormattingEnabled = true;
            this.cmbEstado.Location = new System.Drawing.Point(120, 265);
            this.cmbEstado.Name = "cmbEstado";
            this.cmbEstado.Size = new System.Drawing.Size(300, 28);
            this.cmbEstado.TabIndex = 4;
            this.cmbEstado.SelectedIndexChanged += new System.EventHandler(this.cmbEstado_SelectedIndexChanged);
            // 
            // numCosto
            // 
            this.numCosto.DecimalPlaces = 2;
            this.numCosto.Font = new System.Drawing.Font("Roboto", 10F);
            this.numCosto.Location = new System.Drawing.Point(120, 305);
            this.numCosto.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numCosto.Name = "numCosto";
            this.numCosto.Size = new System.Drawing.Size(150, 28);
            this.numCosto.TabIndex = 5;
            this.numCosto.ThousandsSeparator = true;
            // 
            // txtUbicacion
            // 
            this.txtUbicacion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUbicacion.Depth = 0;
            this.txtUbicacion.Font = new System.Drawing.Font("Roboto", 10F);
            this.txtUbicacion.Hint = "Ubicación física (opcional)";
            this.txtUbicacion.Location = new System.Drawing.Point(20, 345);
            this.txtUbicacion.MaxLength = 200;
            this.txtUbicacion.MouseState = MaterialSkin.MouseState.OUT;
            this.txtUbicacion.Name = "txtUbicacion";
            this.txtUbicacion.Size = new System.Drawing.Size(400, 40);
            this.txtUbicacion.TabIndex = 6;
            this.txtUbicacion.Text = "";
            // 
            // txtResponsable
            // 
            this.txtResponsable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtResponsable.Depth = 0;
            this.txtResponsable.Font = new System.Drawing.Font("Roboto", 10F);
            this.txtResponsable.Hint = "Responsable/Empleado (opcional)";
            this.txtResponsable.Location = new System.Drawing.Point(20, 395);
            this.txtResponsable.MaxLength = 200;
            this.txtResponsable.MouseState = MaterialSkin.MouseState.OUT;
            this.txtResponsable.Name = "txtResponsable";
            this.txtResponsable.Size = new System.Drawing.Size(400, 40);
            this.txtResponsable.TabIndex = 7;
            this.txtResponsable.Text = "";
            // 
            // txtObservaciones
            // 
            this.txtObservaciones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtObservaciones.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtObservaciones.Depth = 0;
            this.txtObservaciones.Font = new System.Drawing.Font("Roboto", 10F);
            this.txtObservaciones.Hint = "Observaciones (opcional)";
            this.txtObservaciones.Location = new System.Drawing.Point(20, 545);
            this.txtObservaciones.MaxLength = 500;
            this.txtObservaciones.MouseState = MaterialSkin.MouseState.OUT;
            this.txtObservaciones.Name = "txtObservaciones";
            this.txtObservaciones.Size = new System.Drawing.Size(400, 80);
            this.txtObservaciones.TabIndex = 10;
            this.txtObservaciones.Text = "";
            // 
            // dtpGarantiaInicio
            // 
            this.dtpGarantiaInicio.Font = new System.Drawing.Font("Roboto", 10F);
            this.dtpGarantiaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpGarantiaInicio.Location = new System.Drawing.Point(180, 450);
            this.dtpGarantiaInicio.Name = "dtpGarantiaInicio";
            this.dtpGarantiaInicio.Size = new System.Drawing.Size(240, 28);
            this.dtpGarantiaInicio.TabIndex = 8;
            // 
            // numMesesGarantia
            // 
            this.numMesesGarantia.Font = new System.Drawing.Font("Roboto", 10F);
            this.numMesesGarantia.Location = new System.Drawing.Point(180, 495);
            this.numMesesGarantia.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numMesesGarantia.Name = "numMesesGarantia";
            this.numMesesGarantia.Size = new System.Drawing.Size(120, 28);
            this.numMesesGarantia.TabIndex = 9;
            this.numMesesGarantia.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // btnGuardar
            // 
            this.btnGuardar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnGuardar.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnGuardar.Depth = 0;
            this.btnGuardar.HighEmphasis = true;
            this.btnGuardar.Icon = null;
            this.btnGuardar.Location = new System.Drawing.Point(220, 645);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnGuardar.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnGuardar.Size = new System.Drawing.Size(88, 36);
            this.btnGuardar.TabIndex = 11;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnGuardar.UseAccentColor = false;
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancelar.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnCancelar.Depth = 0;
            this.btnCancelar.HighEmphasis = true;
            this.btnCancelar.Icon = null;
            this.btnCancelar.Location = new System.Drawing.Point(320, 645);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnCancelar.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnCancelar.Size = new System.Drawing.Size(96, 36);
            this.btnCancelar.TabIndex = 12;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.btnCancelar.UseAccentColor = false;
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // lblArticulo
            // 
            this.lblArticulo.AutoSize = true;
            this.lblArticulo.Depth = 0;
            this.lblArticulo.Font = new System.Drawing.Font("Roboto", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblArticulo.Location = new System.Drawing.Point(20, 150);
            this.lblArticulo.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblArticulo.Name = "lblArticulo";
            this.lblArticulo.Size = new System.Drawing.Size(53, 14);
            this.lblArticulo.TabIndex = 13;
            this.lblArticulo.Text = "Artículo:";
            // 
            // lblCompra
            // 
            this.lblCompra.AutoSize = true;
            this.lblCompra.Depth = 0;
            this.lblCompra.Font = new System.Drawing.Font("Roboto", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblCompra.Location = new System.Drawing.Point(20, 190);
            this.lblCompra.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblCompra.Name = "lblCompra";
            this.lblCompra.Size = new System.Drawing.Size(52, 14);
            this.lblCompra.TabIndex = 14;
            this.lblCompra.Text = "Compra:";
            // 
            // lblSucursal
            // 
            this.lblSucursal.AutoSize = true;
            this.lblSucursal.Depth = 0;
            this.lblSucursal.Font = new System.Drawing.Font("Roboto", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblSucursal.Location = new System.Drawing.Point(20, 230);
            this.lblSucursal.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblSucursal.Name = "lblSucursal";
            this.lblSucursal.Size = new System.Drawing.Size(57, 14);
            this.lblSucursal.TabIndex = 15;
            this.lblSucursal.Text = "Sucursal:";
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Depth = 0;
            this.lblEstado.Font = new System.Drawing.Font("Roboto", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblEstado.Location = new System.Drawing.Point(20, 270);
            this.lblEstado.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(45, 14);
            this.lblEstado.TabIndex = 16;
            this.lblEstado.Text = "Estado:";
            // 
            // lblCosto
            // 
            this.lblCosto.AutoSize = true;
            this.lblCosto.Depth = 0;
            this.lblCosto.Font = new System.Drawing.Font("Roboto", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblCosto.Location = new System.Drawing.Point(20, 310);
            this.lblCosto.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblCosto.Name = "lblCosto";
            this.lblCosto.Size = new System.Drawing.Size(40, 14);
            this.lblCosto.TabIndex = 17;
            this.lblCosto.Text = "Costo:";
            // 
            // lblGarantiaInicio
            // 
            this.lblGarantiaInicio.AutoSize = true;
            this.lblGarantiaInicio.Depth = 0;
            this.lblGarantiaInicio.Font = new System.Drawing.Font("Roboto", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblGarantiaInicio.Location = new System.Drawing.Point(20, 455);
            this.lblGarantiaInicio.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblGarantiaInicio.Name = "lblGarantiaInicio";
            this.lblGarantiaInicio.Size = new System.Drawing.Size(117, 14);
            this.lblGarantiaInicio.TabIndex = 18;
            this.lblGarantiaInicio.Text = "Inicio de Garantía:";
            // 
            // lblMesesGarantia
            // 
            this.lblMesesGarantia.AutoSize = true;
            this.lblMesesGarantia.Depth = 0;
            this.lblMesesGarantia.Font = new System.Drawing.Font("Roboto", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblMesesGarantia.Location = new System.Drawing.Point(20, 500);
            this.lblMesesGarantia.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblMesesGarantia.Name = "lblMesesGarantia";
            this.lblMesesGarantia.Size = new System.Drawing.Size(110, 14);
            this.lblMesesGarantia.TabIndex = 19;
            this.lblMesesGarantia.Text = "Meses Garantía:";
            // 
            // Items
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 700);
            this.Controls.Add(this.lblMesesGarantia);
            this.Controls.Add(this.lblGarantiaInicio);
            this.Controls.Add(this.lblCosto);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.lblSucursal);
            this.Controls.Add(this.lblCompra);
            this.Controls.Add(this.lblArticulo);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.numMesesGarantia);
            this.Controls.Add(this.dtpGarantiaInicio);
            this.Controls.Add(this.txtObservaciones);
            this.Controls.Add(this.txtResponsable);
            this.Controls.Add(this.txtUbicacion);
            this.Controls.Add(this.numCosto);
            this.Controls.Add(this.cmbEstado);
            this.Controls.Add(this.cmbSucursal);
            this.Controls.Add(this.cmbCompra);
            this.Controls.Add(this.cmbArticulo);
            this.Controls.Add(this.txtSerial);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Items";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nuevo Item";
            ((System.ComponentModel.ISupportInitialize)(this.numCosto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMesesGarantia)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialTextBox txtSerial;
        private System.Windows.Forms.ComboBox cmbArticulo;
        private System.Windows.Forms.ComboBox cmbCompra;
        private System.Windows.Forms.ComboBox cmbSucursal;
        private System.Windows.Forms.ComboBox cmbEstado;
        private System.Windows.Forms.NumericUpDown numCosto;
        private MaterialSkin.Controls.MaterialTextBox txtUbicacion;
        private MaterialSkin.Controls.MaterialTextBox txtResponsable;
        private MaterialSkin.Controls.MaterialMultiLineTextBox txtObservaciones;
        private System.Windows.Forms.DateTimePicker dtpGarantiaInicio;
        private System.Windows.Forms.NumericUpDown numMesesGarantia;
        private MaterialSkin.Controls.MaterialButton btnGuardar;
        private MaterialSkin.Controls.MaterialButton btnCancelar;
        private MaterialSkin.Controls.MaterialLabel lblArticulo;
        private MaterialSkin.Controls.MaterialLabel lblCompra;
        private MaterialSkin.Controls.MaterialLabel lblSucursal;
        private MaterialSkin.Controls.MaterialLabel lblEstado;
        private MaterialSkin.Controls.MaterialLabel lblCosto;
        private MaterialSkin.Controls.MaterialLabel lblGarantiaInicio;
        private MaterialSkin.Controls.MaterialLabel lblMesesGarantia;
    }
}
