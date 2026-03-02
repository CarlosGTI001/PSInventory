namespace PSInventory
{
    partial class GestionLotesForm
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
            this.dgvLotes = new System.Windows.Forms.DataGridView();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.btnAddLote = new MaterialSkin.Controls.MaterialButton();
            this.btnEditLote = new MaterialSkin.Controls.MaterialButton();
            this.btnDeleteLote = new MaterialSkin.Controls.MaterialButton();
            this.btnManageItems = new MaterialSkin.Controls.MaterialButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLotes)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLotes
            // 
            this.dgvLotes.AllowUserToAddRows = false;
            this.dgvLotes.AllowUserToDeleteRows = false;
            this.dgvLotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLotes.Location = new System.Drawing.Point(20, 100);
            this.dgvLotes.MultiSelect = false;
            this.dgvLotes.Name = "dgvLotes";
            this.dgvLotes.ReadOnly = true;
            this.dgvLotes.RowHeadersWidth = 51;
            this.dgvLotes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLotes.Size = new System.Drawing.Size(760, 340);
            this.dgvLotes.TabIndex = 0;
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel1.Location = new System.Drawing.Point(20, 70);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(126, 19);
            this.materialLabel1.TabIndex = 1;
            this.materialLabel1.Text = "Lotes de la Compra:";
            // 
            // btnAddLote
            // 
            this.btnAddLote.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddLote.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnAddLote.Depth = 0;
            this.btnAddLote.HighEmphasis = true;
            this.btnAddLote.Icon = null;
            this.btnAddLote.Location = new System.Drawing.Point(20, 460);
            this.btnAddLote.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnAddLote.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnAddLote.Name = "btnAddLote";
            this.btnAddLote.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnAddLote.Size = new System.Drawing.Size(109, 36);
            this.btnAddLote.TabIndex = 2;
            this.btnAddLote.Text = "Nuevo Lote";
            this.btnAddLote.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnAddLote.UseAccentColor = false;
            this.btnAddLote.UseVisualStyleBackColor = true;
            this.btnAddLote.Click += new System.EventHandler(this.btnAddLote_Click);
            // 
            // btnEditLote
            // 
            this.btnEditLote.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnEditLote.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnEditLote.Depth = 0;
            this.btnEditLote.HighEmphasis = false;
            this.btnEditLote.Icon = null;
            this.btnEditLote.Location = new System.Drawing.Point(140, 460);
            this.btnEditLote.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnEditLote.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnEditLote.Name = "btnEditLote";
            this.btnEditLote.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnEditLote.Size = new System.Drawing.Size(105, 36);
            this.btnEditLote.TabIndex = 3;
            this.btnEditLote.Text = "Editar Lote";
            this.btnEditLote.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnEditLote.UseAccentColor = false;
            this.btnEditLote.UseVisualStyleBackColor = true;
            this.btnEditLote.Click += new System.EventHandler(this.btnEditLote_Click);
            // 
            // btnDeleteLote
            // 
            this.btnDeleteLote.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDeleteLote.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnDeleteLote.Depth = 0;
            this.btnDeleteLote.HighEmphasis = false;
            this.btnDeleteLote.Icon = null;
            this.btnDeleteLote.Location = new System.Drawing.Point(260, 460);
            this.btnDeleteLote.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnDeleteLote.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnDeleteLote.Name = "btnDeleteLote";
            this.btnDeleteLote.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnDeleteLote.Size = new System.Drawing.Size(124, 36);
            this.btnDeleteLote.TabIndex = 4;
            this.btnDeleteLote.Text = "Eliminar Lote";
            this.btnDeleteLote.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnDeleteLote.UseAccentColor = true;
            this.btnDeleteLote.UseVisualStyleBackColor = true;
            this.btnDeleteLote.Click += new System.EventHandler(this.btnDeleteLote_Click);
            // 
            // btnManageItems
            // 
            this.btnManageItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManageItems.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnManageItems.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnManageItems.Depth = 0;
            this.btnManageItems.HighEmphasis = true;
            this.btnManageItems.Icon = null;
            this.btnManageItems.Location = new System.Drawing.Point(620, 460);
            this.btnManageItems.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnManageItems.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnManageItems.Name = "btnManageItems";
            this.btnManageItems.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnManageItems.Size = new System.Drawing.Size(160, 36);
            this.btnManageItems.TabIndex = 5;
            this.btnManageItems.Text = "Gestionar Ítems";
            this.btnManageItems.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnManageItems.UseAccentColor = false;
            this.btnManageItems.UseVisualStyleBackColor = true;
            this.btnManageItems.Click += new System.EventHandler(this.btnManageItems_Click);
            // 
            // GestionLotesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 520);
            this.Controls.Add(this.btnManageItems);
            this.Controls.Add(this.btnDeleteLote);
            this.Controls.Add(this.btnEditLote);
            this.Controls.Add(this.btnAddLote);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.dgvLotes);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GestionLotesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gestión de Lotes";
            ((System.ComponentModel.ISupportInitialize)(this.dgvLotes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLotes;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialButton btnAddLote;
        private MaterialSkin.Controls.MaterialButton btnEditLote;
        private MaterialSkin.Controls.MaterialButton btnDeleteLote;
        private MaterialSkin.Controls.MaterialButton btnManageItems;
    }
}