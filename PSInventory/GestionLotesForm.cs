using MaterialSkin.Controls;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity; // Required for Include

namespace PSInventory
{
    public partial class GestionLotesForm : MaterialForm
    {
        private int compraId;
        private LoadingHelper loadingHelper;

        public GestionLotesForm(int compraId)
        {
            InitializeComponent();
            this.compraId = compraId;
            loadingHelper = new LoadingHelper(this);
            CargarLotesAsync();
        }

        private async void CargarLotesAsync()
        {
            loadingHelper.Show("Cargando lotes...");
            try
            {
                await Task.Run(() =>
                {
                    using (var db = new PSDatos())
                    {
                        var lotes = db.Lotes
                            .Include(l => l.Articulo)
                            .Where(l => l.CompraId == compraId && !l.Compra.Eliminado)
                            .ToList();

                        this.Invoke(new Action(() =>
                        {
                            dgvLotes.DataSource = lotes.Select(l => new
                            {
                                l.Id,
                                Articulo = $"{l.Articulo.Marca} {l.Articulo.Modelo}",
                                CantidadLote = l.Cantidad,
                                CostoUnitario = l.CostoUnitario,
                                ItemsRegistrados = l.Items.Count(i => !i.Eliminado)
                            }).ToList();

                            dgvLotes.Columns["Id"].Visible = false; // Ocultar ID, se usa para referencia
                        }));
                    }
                });
            }
            finally
            {
                loadingHelper.Hide();
            }
        }

        private void btnAddLote_Click(object sender, EventArgs e)
        {
            var frmLote = new LoteForm(compraId);
            if (frmLote.ShowDialog() == DialogResult.OK)
            {
                CargarLotesAsync(); // Recargar la lista si se añadió un lote
            }
        }

        private void btnEditLote_Click(object sender, EventArgs e)
        {
            if (dgvLotes.SelectedRows.Count > 0)
            {
                int loteId = (int)dgvLotes.SelectedRows[0].Cells["Id"].Value;
                var frmLote = new LoteForm(compraId, loteId);
                if (frmLote.ShowDialog() == DialogResult.OK)
                {
                    CargarLotesAsync(); // Recargar la lista si se editó un lote
                }
            }
            else
            {
                MaterialMessageBox.Show("Seleccione un lote para editar.", "Advertencia");
            }
        }

        private async void btnDeleteLote_Click(object sender, EventArgs e)
        {
            if (dgvLotes.SelectedRows.Count > 0)
            {
                int loteId = (int)dgvLotes.SelectedRows[0].Cells["Id"].Value;
                DialogResult confirm = MaterialMessageBox.Show("¿Está seguro de eliminar este lote?", "Confirmar Eliminación", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    loadingHelper.Show("Eliminando lote...");
                    try
                    {
                        await Task.Run(() =>
                        {
                            using (var db = new PSDatos())
                            {
                                var lote = db.Lotes.Include(l => l.Items).FirstOrDefault(l => l.Id == loteId);
                                if (lote != null)
                                {
                                    if (lote.Items.Any(i => !i.Eliminado))
                                    {
                                        this.Invoke(new Action(() => MaterialMessageBox.Show("No se puede eliminar el lote porque tiene ítems asociados.", "Error")));
                                        return;
                                    }
                                    db.Lotes.Remove(lote);
                                    db.SaveChanges();
                                }
                            }
                        });
                        CargarLotesAsync();
                    }
                    finally
                    {
                        loadingHelper.Hide();
                    }
                }
            }
            else
            {
                MaterialMessageBox.Show("Seleccione un lote para eliminar.", "Advertencia");
            }
        }

        private void btnManageItems_Click(object sender, EventArgs e)
        {
            if (dgvLotes.SelectedRows.Count > 0)
            {
                int loteId = (int)dgvLotes.SelectedRows[0].Cells["Id"].Value;
                // Aquí abriremos el formulario para gestionar los ítems de este lote
                // Podríamos usar el mismo ItemsForm, pero pasándole el loteId
                var frmItems = new Items(loteId); // Suponiendo que ItemsForm tenga un constructor que acepte loteId
                frmItems.ShowDialog();
                CargarLotesAsync(); // Recargar para actualizar el contador de ítems registrados
            }
            else
            {
                MaterialMessageBox.Show("Seleccione un lote para gestionar sus ítems.", "Advertencia");
            }
        }
    }
}
