using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PSInventory.Helpers
{
    public class LoadingHelper
    {
        private Panel _loadingPanel;
        private ProgressBar _progressBar;
        private Label _loadingLabel;
        private Form _parentForm;

        public LoadingHelper(Form parentForm)
        {
            _parentForm = parentForm;
            InitializeLoadingUI();
        }

        private void InitializeLoadingUI()
        {
            _loadingPanel = new Panel
            {
                BackColor = Color.FromArgb(200, 55, 55, 55),
                Dock = DockStyle.Fill,
                Visible = false
            };

            _progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Size = new Size(300, 23),
                ForeColor = Color.FromArgb(255, 0, 122, 204)
            };

            _loadingLabel = new Label
            {
                Text = "Cargando datos...",
                ForeColor = Color.White,
                Font = new Font("Roboto", 12, FontStyle.Regular),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };

            _loadingPanel.Controls.Add(_progressBar);
            _loadingPanel.Controls.Add(_loadingLabel);

            _parentForm.Controls.Add(_loadingPanel);
            _loadingPanel.BringToFront();

            _parentForm.Resize += ParentForm_Resize;
            CenterControls();
        }

        private void ParentForm_Resize(object sender, EventArgs e)
        {
            CenterControls();
        }

        private void CenterControls()
        {
            if (_loadingPanel != null && _progressBar != null && _loadingLabel != null)
            {
                _progressBar.Location = new Point(
                    (_loadingPanel.Width - _progressBar.Width) / 2,
                    (_loadingPanel.Height - _progressBar.Height) / 2
                );

                _loadingLabel.Location = new Point(
                    (_loadingPanel.Width - _loadingLabel.Width) / 2,
                    _progressBar.Top - _loadingLabel.Height - 20
                );
            }
        }

        public void Show(string message = "Cargando datos...")
        {
            if (_parentForm.InvokeRequired)
            {
                _parentForm.Invoke(new Action(() => Show(message)));
                return;
            }

            _loadingLabel.Text = message;
            CenterControls();
            _loadingPanel.Visible = true;
            _parentForm.Enabled = false;
        }

        public void Hide()
        {
            if (_parentForm.InvokeRequired)
            {
                _parentForm.Invoke(new Action(Hide));
                return;
            }

            _loadingPanel.Visible = false;
            _parentForm.Enabled = true;
        }

        public void UpdateMessage(string message)
        {
            if (_parentForm.InvokeRequired)
            {
                _parentForm.Invoke(new Action(() => UpdateMessage(message)));
                return;
            }

            _loadingLabel.Text = message;
            CenterControls();
        }
    }
}
