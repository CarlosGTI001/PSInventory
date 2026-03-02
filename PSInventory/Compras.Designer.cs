namespace PSInventory
{
    partial class Compras
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
            this.txtProveedor = new MaterialSkin.Controls.MaterialTextBox();
            this.txtNumeroFactura = new MaterialSkin.Controls.MaterialTextBox();
            this.numCostoTotal = new System.Windows.Forms.NumericUpDown();
            this.dtpFechaCompra = new System.Windows.Forms.DateTimePicker();
            this.cmbEstado = new System.Windows.Forms.ComboBox();
            this.txtObservaciones = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.btnGuardar = new MaterialSkin.Controls.MaterialButton();
            this.btnCancelar = new MaterialSkin.Controls.MaterialButton();
            this.lblCostoTotal = new MaterialSkin.Controls.MaterialLabel();
            this.lblFechaCompra = new MaterialSkin.Controls.MaterialLabel();
            this.lblEstado = new MaterialSkin.Controls.MaterialLabel();
            this.btnGestionarLotes = new MaterialSkin.Controls.MaterialButton();
            ((System.ComponentModel.ISupportInitialize)(this.numCostoTotal)).BeginInit();
            this.SuspendLayout();
            // 
            // txtProveedor
            // 
            this.txtProveedor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtProveedor.Depth = 0;
            this.txtProveedor.Font = new System.Drawing.Font("Roboto", 12F);
            this.txtProveedor.Hint = "Nombre del proveedor";
            this.txtProveedor.Location = new System.Drawing.Point(20, 80);
            this.txtProveedor.MaxLength = 200;
            this.txtProveedor.MouseState = MaterialSkin.MouseState.OUT;
            this.txtProveedor.Name = "txtProveedor";
            this.txtProveedor.Size = new System.Drawing.Size(400, 50);
            this.txtProveedor.TabIndex = 0;
            this.txtProveedor.Text = "";
            // 
            // txtNumeroFactura
            // 
            this.txtNumeroFactura.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNumeroFactura.Depth = 0;
            this.txtNumeroFactura.Font = new System.Drawing.Font("Roboto", 12F);
            this.txtNumeroFactura.Hint = "Número de factura (opcional)";
            this.txtNumeroFactura.Location = new System.Drawing.Point(20, 140);
            this.txtNumeroFactura.MaxLength = 100;
            this.txtNumeroFactura.MouseState = MaterialSkin.MouseState.OUT;
            this.txtNumeroFactura.Name = "txtNumeroFactura";
            this.txtNumeroFactura.Size = new System.Drawing.Size(400, 50);
            this.txtNumeroFactura.TabIndex = 1;
            this.txtNumeroFactura.Text = "";
            // 
            // numCostoTotal
            // 
            this.numCostoTotal.DecimalPlaces = 2;
            this.numCostoTotal.Font = new System.Drawing.Font("Roboto", 12F);
            this.numCostoTotal.Location = new System.Drawing.Point(180, 210);
            this.numCostoTotal.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numCostoTotal.Name = "numCostoTotal";
            this.numCostoTotal.Size = new System.Drawing.Size(240, 32);
            this.numCostoTotal.TabIndex = 2;
            this.numCostoTotal.ThousandsSeparator = true;
            // 
            // dtpFechaCompra
            // 
            this.dtpFechaCompra.Font = new System.Drawing.Font("Roboto", 12F);
            this.dtpFechaCompra.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaCompra.Location = new System.Drawing.Point(180, 260);
            this.dtpFechaCompra.Name = "dtpFechaCompra";
            this.dtpFechaCompra.Size = new System.Drawing.Size(240, 32);
            this.dtpFechaCompra.TabIndex = 3;
            // 
            // cmbEstado
            // 
            this.cmbEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstado.Font = new System.Drawing.Font("Roboto", 12F);
            this.cmbEstado.FormattingEnabled = true;
            this.cmbEstado.Location = new System.Drawing.Point(180, 310);
            this.cmbEstado.Name = "cmbEstado";
            this.cmbEstado.Size = new System.Drawing.Size(240, 31);
            this.cmbEstado.TabIndex = 4;
            // 
            // txtObservaciones
            // 
            this.txtObservaciones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtObservaciones.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtObservaciones.Depth = 0;
            this.txtObservaciones.Font = new System.Drawing.Font("Roboto", 10F);
            this.txtObservaciones.Hint = "Observaciones (opcional)";
            this.txtObservaciones.Location = new System.Drawing.Point(20, 360);
            this.txtObservaciones.MaxLength = 500;
            this.txtObservaciones.MouseState = MaterialSkin.MouseState.OUT;
            this.txtObservaciones.Name = "txtObservaciones";
            this.txtObservaciones.Size = new System.Drawing.Size(400, 80);
            this.txtObservaciones.TabIndex = 5;
            this.txtObservaciones.Text = "";
            // 
            // btnGuardar
            // 
            this.btnGuardar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnGuardar.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnGuardar.Depth = 0;
            this.btnGuardar.HighEmphasis = true;
            this.btnGuardar.Icon = null;
            this.btnGuardar.Location = new System.Drawing.Point(220, 460);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnGuardar.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnGuardar.Size = new System.Drawing.Size(88, 36);
            this.btnGuardar.TabIndex = 6;
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
            this.btnCancelar.Location = new System.Drawing.Point(320, 460);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnCancelar.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnCancelar.Size = new System.Drawing.Size(96, 36);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.btnCancelar.UseAccentColor = false;
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // lblCostoTotal
            // 
            this.lblCostoTotal.AutoSize = true;
            this.lblCostoTotal.Depth = 0;
            this.lblCostoTotal.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblCostoTotal.Location = new System.Drawing.Point(20, 215);
            this.lblCostoTotal.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblCostoTotal.Name = "lblCostoTotal";
            this.lblCostoTotal.Size = new System.Drawing.Size(87, 19);
            this.lblCostoTotal.TabIndex = 8;
            this.lblCostoTotal.Text = "Costo Total:";
            // 
            // lblFechaCompra
            // 
            this.lblFechaCompra.AutoSize = true;
            this.lblFechaCompra.Depth = 0;
            this.lblFechaCompra.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblFechaCompra.Location = new System.Drawing.Point(20, 265);
            this.lblFechaCompra.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblFechaCompra.Name = "lblFechaCompra";
            this.lblFechaCompra.Size = new System.Drawing.Size(104, 19);
            this.lblFechaCompra.TabIndex = 9;
            this.lblFechaCompra.Text = "Fecha Compra:";
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Depth = 0;
            this.lblEstado.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblEstado.Location = new System.Drawing.Point(20, 315);
            this.lblEstado.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(53, 19);
            this.lblEstado.TabIndex = 10;
            this.lblEstado.Text = "Estado:";
            // 
            // btnGestionarLotes
            // 
            this.btnGestionarLotes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnGestionarLotes.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnGestionarLotes.Depth = 0;
            this.btnGestionarLotes.HighEmphasis = true;
            this.btnGestionarLotes.Icon = null;
            this.btnGestionarLotes.Location = new System.Drawing.Point(20, 460);
            this.btnGestionarLotes.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnGestionarLotes.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnGestionarLotes.Name = "btnGestionarLotes";
            this.btnGestionarLotes.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnGestionarLotes.Size = new System.Drawing.Size(156, 36);
            this.btnGestionarLotes.TabIndex = 11;
            this.btnGestionarLotes.Text = "Gestionar Lotes";
            this.btnGestionarLotes.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnGestionarLotes.UseAccentColor = false;
            this.btnGestionarLotes.UseVisualStyleBackColor = true;
            this.btnGestionarLotes.Visible = false; // Solo visible en modo edición
            this.btnGestionarLotes.Click += new System.EventHandler(this.btnGestionarLotes_Click);
            // 
            // Compras
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 520);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.btnGestionarLotes);
            this.Controls.Add(this.lblFechaCompra);
            this.Controls.Add(this.lblCostoTotal);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.txtObservaciones);
            this.Controls.Add(this.btnGestionarLotes);
            this.Controls.Add(this.dtpFechaCompra);
            this.Controls.Add(this.numCostoTotal);
            this.Controls.Add(this.txtNumeroFactura);
            this.Controls.Add(this.txtProveedor);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Compras";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nueva Compra";
            ((System.ComponentModel.ISupportInitialize)(this.numCostoTotal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialTextBox txtProveedor;
        private MaterialSkin.Controls.MaterialTextBox txtNumeroFactura;
        private System.Windows.Forms.NumericUpDown numCostoTotal;
        private System.Windows.Forms.DateTimePicker dtpFechaCompra;
        private System.Windows.Forms.ComboBox cmbEstado;
        private MaterialSkin.Controls.MaterialMultiLineTextBox txtObservaciones;
        private MaterialSkin.Controls.MaterialButton btnGuardar;
        private MaterialSkin.Controls.MaterialButton btnCancelar;
        private MaterialSkin.Controls.MaterialLabel lblCostoTotal;
        private MaterialSkin.Controls.MaterialLabel lblFechaCompra;
        private MaterialSkin.Controls.MaterialLabel lblEstado;
        private MaterialSkin.Controls.MaterialButton btnGestionarLotes;
    }
}
