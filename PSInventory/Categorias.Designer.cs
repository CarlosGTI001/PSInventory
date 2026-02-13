namespace PSInventory
{
    partial class Categorias
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
            categoriaTxt = new MaterialSkin.Controls.MaterialTextBox2();
            descripcionTxt = new MaterialSkin.Controls.MaterialMultiLineTextBox2();
            agregarBtn = new MaterialSkin.Controls.MaterialButton();
            SuspendLayout();
            // 
            // categoriaTxt
            // 
            categoriaTxt.AnimateReadOnly = false;
            categoriaTxt.BackgroundImageLayout = ImageLayout.None;
            categoriaTxt.CharacterCasing = CharacterCasing.Normal;
            categoriaTxt.Depth = 0;
            categoriaTxt.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            categoriaTxt.HideSelection = true;
            categoriaTxt.Hint = "Nombre Categoria";
            categoriaTxt.LeadingIcon = null;
            categoriaTxt.Location = new Point(15, 78);
            categoriaTxt.MaxLength = 32767;
            categoriaTxt.MouseState = MaterialSkin.MouseState.OUT;
            categoriaTxt.Name = "categoriaTxt";
            categoriaTxt.PasswordChar = '\0';
            categoriaTxt.PrefixSuffixText = null;
            categoriaTxt.ReadOnly = false;
            categoriaTxt.RightToLeft = RightToLeft.No;
            categoriaTxt.SelectedText = "";
            categoriaTxt.SelectionLength = 0;
            categoriaTxt.SelectionStart = 0;
            categoriaTxt.ShortcutsEnabled = true;
            categoriaTxt.Size = new Size(250, 48);
            categoriaTxt.TabIndex = 2;
            categoriaTxt.TabStop = false;
            categoriaTxt.TextAlign = HorizontalAlignment.Left;
            categoriaTxt.TrailingIcon = null;
            categoriaTxt.UseSystemPasswordChar = false;
            // 
            // descripcionTxt
            // 
            descripcionTxt.AnimateReadOnly = false;
            descripcionTxt.BackgroundImageLayout = ImageLayout.None;
            descripcionTxt.CharacterCasing = CharacterCasing.Normal;
            descripcionTxt.Depth = 0;
            descripcionTxt.HideSelection = true;
            descripcionTxt.Hint = "Descripcion";
            descripcionTxt.Location = new Point(15, 132);
            descripcionTxt.MaxLength = 32767;
            descripcionTxt.MouseState = MaterialSkin.MouseState.OUT;
            descripcionTxt.Name = "descripcionTxt";
            descripcionTxt.PasswordChar = '\0';
            descripcionTxt.ReadOnly = false;
            descripcionTxt.ScrollBars = ScrollBars.None;
            descripcionTxt.SelectedText = "";
            descripcionTxt.SelectionLength = 0;
            descripcionTxt.SelectionStart = 0;
            descripcionTxt.ShortcutsEnabled = true;
            descripcionTxt.Size = new Size(250, 100);
            descripcionTxt.TabIndex = 4;
            descripcionTxt.TabStop = false;
            descripcionTxt.TextAlign = HorizontalAlignment.Left;
            descripcionTxt.UseSystemPasswordChar = false;
            // 
            // agregarBtn
            // 
            agregarBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            agregarBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            agregarBtn.Depth = 0;
            agregarBtn.HighEmphasis = true;
            agregarBtn.Icon = null;
            agregarBtn.Location = new Point(99, 250);
            agregarBtn.Margin = new Padding(4, 6, 4, 6);
            agregarBtn.MouseState = MaterialSkin.MouseState.HOVER;
            agregarBtn.Name = "agregarBtn";
            agregarBtn.NoAccentTextColor = Color.Empty;
            agregarBtn.Size = new Size(88, 36);
            agregarBtn.TabIndex = 5;
            agregarBtn.Text = "Agregar";
            agregarBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            agregarBtn.UseAccentColor = false;
            agregarBtn.UseVisualStyleBackColor = true;
            agregarBtn.Click += agregarBtn_Click;
            // 
            // Categorias
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(279, 304);
            Controls.Add(agregarBtn);
            Controls.Add(descripcionTxt);
            Controls.Add(categoriaTxt);
            Name = "Categorias";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Agregar Categorias";
            Load += Categorias_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MaterialSkin.Controls.MaterialTextBox2 categoriaTxt;
        private MaterialSkin.Controls.MaterialMultiLineTextBox2 descripcionTxt;
        private MaterialSkin.Controls.MaterialButton agregarBtn;
    }
}