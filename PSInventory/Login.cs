using MaterialSkin;
using MaterialSkin.Controls;
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

namespace PSInventory
{
    public partial class Login : MaterialForm
    {
        ColorMgr ColorMgr = new ColorMgr();
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
