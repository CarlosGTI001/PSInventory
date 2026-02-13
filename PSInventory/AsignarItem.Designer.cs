namespace PSInventory
{
    partial class AsignarItem
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
            this.cmbItem = new System.Windows.Forms.ComboBox();
            this.cmbSucursal = new System.Windows.Forms.ComboBox();
            this.txtUbicacion = new MaterialSkin.Controls.MaterialTextBox();
            this.txtResponsable = new MaterialSkin.Controls.MaterialTextBox();
            this.txtObservaciones = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.lblInfoItem = new System.Windows.Forms.Label();
            this.btnAsignar = new MaterialSkin.Controls.MaterialButton();
            this.btnCancelar = new MaterialSkin.Controls.MaterialButton();
            this.lblItem = new System.Windows.Forms.Label();
            this.lblSucursal = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmbItem
            // 
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.Font = new System.Drawing.Font("Roboto", 11F);
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Location = new System.Drawing.Point(20, 100);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(500, 30);
            this.cmbItem.TabIndex = 0;
            this.cmbItem.SelectedIndexChanged += new System.EventHandler(this.cmbItem_SelectedIndexChanged);
            // 
            // cmbSucursal
            // 
            this.cmbSucursal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSucursal.Font = new System.Drawing.Font("Roboto", 11F);
            this.cmbSucursal.FormattingEnabled = true;
            this.cmbSucursal.Location = new System.Drawing.Point(20, 220);
            this.cmbSucursal.Name = "cmbSucursal";
            this.cmbSucursal.Size = new System.Drawing.Size(500, 30);
            this.cmbSucursal.TabIndex = 1;
            // 
            // txtUbicacion
            // 
            this.txtUbicacion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUbicacion.Depth = 0;
            this.txtUbicacion.Font = new System.Drawing.Font("Roboto", 11F);
            this.txtUbicacion.Hint = "Ubicación física en la sucursal (opcional)";
            this.txtUbicacion.Location = new System.Drawing.Point(20, 270);
            this.txtUbicacion.MaxLength = 200;
            this.txtUbicacion.MouseState = MaterialSkin.MouseState.OUT;
            this.txtUbicacion.Name = "txtUbicacion";
            this.txtUbicacion.Size = new System.Drawing.Size(500, 50);
            this.txtUbicacion.TabIndex = 2;
            this.txtUbicacion.Text = "";
            // 
            // txtResponsable
            // 
            this.txtResponsable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtResponsable.Depth = 0;
            this.txtResponsable.Font = new System.Drawing.Font("Roboto", 11F);
            this.txtResponsable.Hint = "Empleado responsable (opcional)";
            this.txtResponsable.Location = new System.Drawing.Point(20, 330);
            this.txtResponsable.MaxLength = 200;
            this.txtResponsable.MouseState = MaterialSkin.MouseState.OUT;
            this.txtResponsable.Name = "txtResponsable";
            this.txtResponsable.Size = new System.Drawing.Size(500, 50);
            this.txtResponsable.TabIndex = 3;
            this.txtResponsable.Text = "";
            // 
            // txtObservaciones
            // 
            this.txtObservaciones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtObservaciones.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtObservaciones.Depth = 0;
            this.txtObservaciones.Font = new System.Drawing.Font("Roboto", 11F);
            this.txtObservaciones.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.txtObservaciones.Hint = "Observaciones (opcional)";
            this.txtObservaciones.Location = new System.Drawing.Point(20, 390);
            this.txtObservaciones.MaxLength = 1000;
            this.txtObservaciones.MouseState = MaterialSkin.MouseState.OUT;
            this.txtObservaciones.Name = "txtObservaciones";
            this.txtObservaciones.Size = new System.Drawing.Size(500, 80);
            this.txtObservaciones.TabIndex = 4;
            this.txtObservaciones.Text = "";
            // 
            // lblInfoItem
            // 
            this.lblInfoItem.AutoSize = true;
            this.lblInfoItem.Font = new System.Drawing.Font("Roboto", 9F);
            this.lblInfoItem.ForeColor = System.Drawing.Color.Gray;
            this.lblInfoItem.Location = new System.Drawing.Point(20, 135);
            this.lblInfoItem.MaximumSize = new System.Drawing.Size(500, 0);
            this.lblInfoItem.Name = "lblInfoItem";
            this.lblInfoItem.Size = new System.Drawing.Size(0, 18);
            this.lblInfoItem.TabIndex = 5;
            // 
            // btnAsignar
            // 
            this.btnAsignar.AutoSize = false;
            this.btnAsignar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAsignar.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnAsignar.Depth = 0;
            this.btnAsignar.HighEmphasis = true;
            this.btnAsignar.Icon = null;
            this.btnAsignar.Location = new System.Drawing.Point(300, 490);
            this.btnAsignar.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnAsignar.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnAsignar.Name = "btnAsignar";
            this.btnAsignar.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnAsignar.Size = new System.Drawing.Size(110, 40);
            this.btnAsignar.TabIndex = 5;
            this.btnAsignar.Text = "Asignar";
            this.btnAsignar.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnAsignar.UseAccentColor = false;
            this.btnAsignar.UseVisualStyleBackColor = true;
            this.btnAsignar.Click += new System.EventHandler(this.btnAsignar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.AutoSize = false;
            this.btnCancelar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancelar.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnCancelar.Depth = 0;
            this.btnCancelar.HighEmphasis = true;
            this.btnCancelar.Icon = null;
            this.btnCancelar.Location = new System.Drawing.Point(420, 490);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnCancelar.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnCancelar.Size = new System.Drawing.Size(100, 40);
            this.btnCancelar.TabIndex = 6;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.btnCancelar.UseAccentColor = false;
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Font = new System.Drawing.Font("Roboto", 11F, System.Drawing.FontStyle.Bold);
            this.lblItem.Location = new System.Drawing.Point(20, 75);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(249, 23);
            this.lblItem.TabIndex = 7;
            this.lblItem.Text = "Seleccione el item a asignar:";
            // 
            // lblSucursal
            // 
            this.lblSucursal.AutoSize = true;
            this.lblSucursal.Font = new System.Drawing.Font("Roboto", 11F, System.Drawing.FontStyle.Bold);
            this.lblSucursal.Location = new System.Drawing.Point(20, 195);
            this.lblSucursal.Name = "lblSucursal";
            this.lblSucursal.Size = new System.Drawing.Size(197, 23);
            this.lblSucursal.TabIndex = 8;
            this.lblSucursal.Text = "Sucursal de destino:";
            // 
            // AsignarItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 550);
            this.Controls.Add(this.lblSucursal);
            this.Controls.Add(this.lblItem);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAsignar);
            this.Controls.Add(this.lblInfoItem);
            this.Controls.Add(this.txtObservaciones);
            this.Controls.Add(this.txtResponsable);
            this.Controls.Add(this.txtUbicacion);
            this.Controls.Add(this.cmbSucursal);
            this.Controls.Add(this.cmbItem);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AsignarItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Asignar Item a Sucursal";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbItem;
        private System.Windows.Forms.ComboBox cmbSucursal;
        private MaterialSkin.Controls.MaterialTextBox txtUbicacion;
        private MaterialSkin.Controls.MaterialTextBox txtResponsable;
        private MaterialSkin.Controls.MaterialMultiLineTextBox txtObservaciones;
        private System.Windows.Forms.Label lblInfoItem;
        private MaterialSkin.Controls.MaterialButton btnAsignar;
        private MaterialSkin.Controls.MaterialButton btnCancelar;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.Label lblSucursal;
    }
}
