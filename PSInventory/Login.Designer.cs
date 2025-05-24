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
            usuarioTxt = new MaterialSkin.Controls.MaterialTextBox2();
            contrasenaTxt = new MaterialSkin.Controls.MaterialTextBox2();
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
            // usuarioTxt
            // 
            usuarioTxt.AnimateReadOnly = false;
            usuarioTxt.BackgroundImageLayout = ImageLayout.None;
            usuarioTxt.CharacterCasing = CharacterCasing.Normal;
            usuarioTxt.Depth = 0;
            usuarioTxt.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            usuarioTxt.HideSelection = true;
            usuarioTxt.Hint = "Usuario";
            usuarioTxt.LeadingIcon = null;
            usuarioTxt.Location = new Point(40, 102);
            usuarioTxt.MaxLength = 32767;
            usuarioTxt.MouseState = MaterialSkin.MouseState.OUT;
            usuarioTxt.Name = "usuarioTxt";
            usuarioTxt.PasswordChar = '\0';
            usuarioTxt.PrefixSuffixText = null;
            usuarioTxt.ReadOnly = false;
            usuarioTxt.RightToLeft = RightToLeft.No;
            usuarioTxt.SelectedText = "";
            usuarioTxt.SelectionLength = 0;
            usuarioTxt.SelectionStart = 0;
            usuarioTxt.ShortcutsEnabled = true;
            usuarioTxt.Size = new Size(250, 48);
            usuarioTxt.TabIndex = 1;
            usuarioTxt.TabStop = false;
            usuarioTxt.TextAlign = HorizontalAlignment.Left;
            usuarioTxt.TrailingIcon = null;
            usuarioTxt.UseSystemPasswordChar = false;
            // 
            // contrasenaTxt
            // 
            contrasenaTxt.AnimateReadOnly = false;
            contrasenaTxt.BackgroundImageLayout = ImageLayout.None;
            contrasenaTxt.CharacterCasing = CharacterCasing.Normal;
            contrasenaTxt.Depth = 0;
            contrasenaTxt.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            contrasenaTxt.HideSelection = true;
            contrasenaTxt.Hint = "Contraseña";
            contrasenaTxt.LeadingIcon = null;
            contrasenaTxt.Location = new Point(40, 162);
            contrasenaTxt.MaxLength = 32767;
            contrasenaTxt.MouseState = MaterialSkin.MouseState.OUT;
            contrasenaTxt.Name = "contrasenaTxt";
            contrasenaTxt.PasswordChar = '•';
            contrasenaTxt.PrefixSuffixText = null;
            contrasenaTxt.ReadOnly = false;
            contrasenaTxt.RightToLeft = RightToLeft.No;
            contrasenaTxt.SelectedText = "";
            contrasenaTxt.SelectionLength = 0;
            contrasenaTxt.SelectionStart = 0;
            contrasenaTxt.ShortcutsEnabled = true;
            contrasenaTxt.Size = new Size(250, 48);
            contrasenaTxt.TabIndex = 2;
            contrasenaTxt.TabStop = false;
            contrasenaTxt.TextAlign = HorizontalAlignment.Left;
            contrasenaTxt.TrailingIcon = null;
            contrasenaTxt.UseSystemPasswordChar = false;
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
            Controls.Add(contrasenaTxt);
            Controls.Add(usuarioTxt);
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
        private MaterialSkin.Controls.MaterialTextBox2 usuarioTxt;
        private MaterialSkin.Controls.MaterialTextBox2 contrasenaTxt;
        private PictureBox pictureBox1;
        private MaterialSkin.Controls.MaterialButton cerrarBtn;
    }
}