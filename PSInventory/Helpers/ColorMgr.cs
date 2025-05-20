using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSInventory.Helpers
{
    internal class ColorMgr
    {
        public void ManejarColor(MaterialForm form)
        {
            var skinManager = MaterialSkinManager.Instance;
            skinManager.AddFormToManage(form);

            // Tipo de tema: Light o Dark
            skinManager.Theme = MaterialSkinManager.Themes.DARK;

            // Define tu propia paleta de colores
            skinManager.ColorScheme = new ColorScheme(
                Color.FromArgb(0x04, 0x73, 0x94), // Primary: #047394
                    Color.FromArgb(0x03, 0x5a, 0x72), // Primary Darker (un poco más oscuro)
                    Color.FromArgb(0x5b, 0xc9, 0xe5), // Primary Light (un poco más claro)
                    Color.FromArgb(0xff, 0x5c, 0x00), // Accent: #ff5c00 (Secundario)
                    TextShade.WHITE                  // Texto claro sobre fondo oscuro
                );
        }
    }
}
