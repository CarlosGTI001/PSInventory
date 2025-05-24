using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
using PSInventory.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using PSData.Modelos;
using PSData.Datos;

namespace PSInventory
{
    public partial class Login : MaterialForm
 // Add this line to resolve the CS0012 error.
    {
        ColorMgr ColorMgr = new ColorMgr();
        PSDatos _db = new PSDatos();
        public Login()
        {
            InitializeComponent();
            ColorMgr.ManejarColor(this);
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void entrarBtn_Click(object sender, EventArgs e)
        {
            // Buscar usuario por nombre
            var usuario = _db.Usuarios.FirstOrDefault(u => u.Nombre == usuarioTxt.Text);

            if (usuario == null)
            {
                MaterialMessageBox.Show("Esta combinación de usuario y contraseña no existe en el sistema", "Aviso", MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                return;
            }

            // Validar contraseña con BCrypt
            bool contrasenaValida = BCrypt.Net.BCrypt.Verify(contrasenaTxt.Text, usuario.Password);

            if (contrasenaValida)
            {
                var services = new ServiceCollection();
                using (ServiceProvider serviceProvider = services.BuildServiceProvider())
                {
                    Menu menu = new Menu();
                    menu.Show();
                    this.Hide();
                }
            }
            else
            {
                MaterialMessageBox.Show("La contraseña es incorrecta", "Aviso", MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
            }
        }


        private void cerrarBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MaterialMessageBox.Show("Con esta accion saldra del sistema!", "En realidad desea salir?", MessageBoxButtons.YesNo, false, FlexibleMaterialForm.ButtonsPosition.Center);

            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
