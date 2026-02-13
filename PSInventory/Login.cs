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
using PSData.Modelos;
using PSData.Datos;

namespace PSInventory
{
    public partial class Login : MaterialForm
    {
        ColorMgr ColorMgr = new ColorMgr();
        LoadingHelper loadingHelper;

        public Login()
        {
            InitializeComponent();
            ColorMgr.ManejarColor(this);
            loadingHelper = new LoadingHelper(this);
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private async void entrarBtn_Click(object sender, EventArgs e)
        {
            loadingHelper.Show("Validando credenciales...");
            try
            {
                bool loginExitoso = await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        var usuario = db.Usuarios.AsNoTracking()
                            .FirstOrDefault(u => u.Nombre == usuarioTxt.Text);

                        if (usuario == null)
                        {
                            return false;
                        }

                        Program.UserName = usuario.Nombre;
                        return BCrypt.Net.BCrypt.Verify(contrasenaTxt.Text, usuario.Password);
                    }
                });

                if (loginExitoso)
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
                    MaterialMessageBox.Show("Esta combinación de usuario y contraseña no existe en el sistema", 
                        "Aviso", MessageBoxButtons.OK, false, FlexibleMaterialForm.ButtonsPosition.Center);
                }
            }
            finally
            {
                loadingHelper.Hide();
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
