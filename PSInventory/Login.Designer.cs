namespace PSInventory
{
    partial class Login
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
            entrarBtn = new MaterialSkin.Controls.MaterialButton();
            materialTextBox21 = new MaterialSkin.Controls.MaterialTextBox2();
            materialTextBox22 = new MaterialSkin.Controls.MaterialTextBox2();
            pictureBox1 = new PictureBox();
            cerrarBtn = new MaterialSkin.Controls.MaterialButton();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // entrarBtn
            // 
            entrarBtn.AutoSize = false;
            entrarBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            entrarBtn.CharacterCasing = MaterialSkin.Controls.MaterialButton.CharacterCasingEnum.Normal;
            entrarBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            entrarBtn.Depth = 0;
            entrarBtn.Font = new Font("Segoe UI", 14.25F);
            entrarBtn.HighEmphasis = true;
            entrarBtn.Icon = null;
            entrarBtn.Location = new Point(40, 248);
            entrarBtn.Margin = new Padding(4, 6, 4, 6);
            entrarBtn.MouseState = MaterialSkin.MouseState.HOVER;
            entrarBtn.Name = "entrarBtn";
            entrarBtn.NoAccentTextColor = Color.Empty;
            entrarBtn.Size = new Size(250, 46);
            entrarBtn.TabIndex = 0;
            entrarBtn.Text = "Entrar";
            entrarBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            entrarBtn.UseAccentColor = false;
            entrarBtn.UseVisualStyleBackColor = true;
            entrarBtn.Click += entrarBtn_Click;
            // 
            // materialTextBox21
            // 
            materialTextBox21.AnimateReadOnly = false;
            materialTextBox21.BackgroundImageLayout = ImageLayout.None;
            materialTextBox21.CharacterCasing = CharacterCasing.Normal;
            materialTextBox21.Depth = 0;
            materialTextBox21.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTextBox21.HideSelection = true;
            materialTextBox21.Hint = "Usuario";
            materialTextBox21.LeadingIcon = null;
            materialTextBox21.Location = new Point(40, 102);
            materialTextBox21.MaxLength = 32767;
            materialTextBox21.MouseState = MaterialSkin.MouseState.OUT;
            materialTextBox21.Name = "materialTextBox21";
            materialTextBox21.PasswordChar = '\0';
            materialTextBox21.PrefixSuffixText = null;
            materialTextBox21.ReadOnly = false;
            materialTextBox21.RightToLeft = RightToLeft.No;
            materialTextBox21.SelectedText = "";
            materialTextBox21.SelectionLength = 0;
            materialTextBox21.SelectionStart = 0;
            materialTextBox21.ShortcutsEnabled = true;
            materialTextBox21.Size = new Size(250, 48);
            materialTextBox21.TabIndex = 1;
            materialTextBox21.TabStop = false;
            materialTextBox21.TextAlign = HorizontalAlignment.Left;
            materialTextBox21.TrailingIcon = null;
            materialTextBox21.UseSystemPasswordChar = false;
            // 
            // materialTextBox22
            // 
            materialTextBox22.AnimateReadOnly = false;
            materialTextBox22.BackgroundImageLayout = ImageLayout.None;
            materialTextBox22.CharacterCasing = CharacterCasing.Normal;
            materialTextBox22.Depth = 0;
            materialTextBox22.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTextBox22.HideSelection = true;
            materialTextBox22.Hint = "Contraseña";
            materialTextBox22.LeadingIcon = null;
            materialTextBox22.Location = new Point(40, 162);
            materialTextBox22.MaxLength = 32767;
            materialTextBox22.MouseState = MaterialSkin.MouseState.OUT;
            materialTextBox22.Name = "materialTextBox22";
            materialTextBox22.PasswordChar = '\0';
            materialTextBox22.PrefixSuffixText = null;
            materialTextBox22.ReadOnly = false;
            materialTextBox22.RightToLeft = RightToLeft.No;
            materialTextBox22.SelectedText = "";
            materialTextBox22.SelectionLength = 0;
            materialTextBox22.SelectionStart = 0;
            materialTextBox22.ShortcutsEnabled = true;
            materialTextBox22.Size = new Size(250, 48);
            materialTextBox22.TabIndex = 2;
            materialTextBox22.TabStop = false;
            materialTextBox22.TextAlign = HorizontalAlignment.Left;
            materialTextBox22.TrailingIcon = null;
            materialTextBox22.UseSystemPasswordChar = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Logo_Presidente_Sports;
            pictureBox1.Location = new Point(317, 69);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(279, 279);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // cerrarBtn
            // 
            cerrarBtn.AutoSize = false;
            cerrarBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cerrarBtn.CharacterCasing = MaterialSkin.Controls.MaterialButton.CharacterCasingEnum.Normal;
            cerrarBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            cerrarBtn.Depth = 0;
            cerrarBtn.Font = new Font("Segoe UI", 14.25F);
            cerrarBtn.HighEmphasis = true;
            cerrarBtn.Icon = null;
            cerrarBtn.Location = new Point(40, 300);
            cerrarBtn.Margin = new Padding(4, 6, 4, 6);
            cerrarBtn.MouseState = MaterialSkin.MouseState.HOVER;
            cerrarBtn.Name = "cerrarBtn";
            cerrarBtn.NoAccentTextColor = Color.Empty;
            cerrarBtn.Size = new Size(250, 46);
            cerrarBtn.TabIndex = 4;
            cerrarBtn.Text = "Cerrar";
            cerrarBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            cerrarBtn.UseAccentColor = true;
            cerrarBtn.UseVisualStyleBackColor = true;
            cerrarBtn.Click += cerrarBtn_Click;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(622, 364);
            Controls.Add(cerrarBtn);
            Controls.Add(pictureBox1);
            Controls.Add(materialTextBox22);
            Controls.Add(materialTextBox21);
            Controls.Add(entrarBtn);
            Name = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Inventario IT";
            Load += Login_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private MaterialSkin.Controls.MaterialButton entrarBtn;
        private MaterialSkin.Controls.MaterialTextBox2 materialTextBox21;
        private MaterialSkin.Controls.MaterialTextBox2 materialTextBox22;
        private PictureBox pictureBox1;
        private MaterialSkin.Controls.MaterialButton cerrarBtn;
    }
}