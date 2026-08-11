using AlmacenDesktop.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ActualizacionForm : Form
    {
        private readonly UpdateInfo _info;
        private readonly UpdateService _updateService = new UpdateService();

        public ActualizacionForm(UpdateInfo info)
        {
            _info = info;
            InitializeComponent();
            lblTitulo.Text = $"🚀 Nueva versión disponible: {_info.VersionTag}";
            txtNotas.Text = string.IsNullOrWhiteSpace(_info.Notes) ? "(Sin notas de la versión)" : _info.Notes;
        }

        private async void btnActualizar_Click(object sender, EventArgs e)
        {
            btnActualizar.Enabled = false;
            btnMasTarde.Enabled = false;
            progressBar.Visible = true;
            lblEstado.Visible = true;
            lblEstado.Text = "Descargando…";

            var progress = new Progress<int>(p =>
            {
                progressBar.Value = Math.Min(100, Math.Max(0, p));
                lblEstado.Text = $"Descargando… {p}%";
            });

            try
            {
                await _updateService.DescargarEInstalarAsync(_info.DownloadUrl, progress);
                // Si llegó hasta acá sin excepción, DescargarEInstalarAsync ya llamó a
                // Application.Exit() — la app se está cerrando para reemplazarse sola.
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo completar la actualización:\n{ex.Message}\n\nPodés reintentar más tarde.",
                    "Error de actualización", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnActualizar.Enabled = true;
                btnMasTarde.Enabled = true;
                progressBar.Visible = false;
                lblEstado.Visible = false;
            }
        }

        private void btnMasTarde_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    // DISEÑO VISUAL
    partial class ActualizacionForm
    {
        private System.ComponentModel.IContainer? components = null;
        private Label lblTitulo = null!;
        private Label lblSubtitulo = null!;
        private TextBox txtNotas = null!;
        private ProgressBar progressBar = null!;
        private Label lblEstado = null!;
        private Button btnActualizar = null!;
        private Button btnMasTarde = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.Size = new Size(480, 400);
            this.Text = "Actualización disponible";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblTitulo = new Label
            {
                Text = "🚀 Nueva versión disponible",
                Location = new Point(20, 20),
                Size = new Size(440, 30),
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(124, 58, 237),
            };

            lblSubtitulo = new Label
            {
                Text = "Se recomienda actualizar para tener las últimas mejoras. Tus ventas, productos y clientes no se ven afectados.",
                Location = new Point(20, 55),
                Size = new Size(440, 40),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.DimGray,
            };

            txtNotas = new TextBox
            {
                Location = new Point(20, 100),
                Size = new Size(440, 180),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 9.5F),
            };

            lblEstado = new Label
            {
                Location = new Point(20, 290),
                Size = new Size(440, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.SteelBlue,
                Visible = false,
            };

            progressBar = new ProgressBar
            {
                Location = new Point(20, 312),
                Size = new Size(440, 20),
                Minimum = 0,
                Maximum = 100,
                Visible = false,
            };

            btnMasTarde = new Button
            {
                Text = "Más tarde",
                Location = new Point(240, 345),
                Size = new Size(100, 35),
            };
            btnMasTarde.Click += btnMasTarde_Click;

            btnActualizar = new Button
            {
                Text = "Actualizar ahora",
                Location = new Point(350, 345),
                Size = new Size(110, 35),
                BackColor = Color.FromArgb(20, 184, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
            };
            btnActualizar.Click += btnActualizar_Click;

            this.Controls.AddRange(new Control[]
            {
                lblTitulo, lblSubtitulo, txtNotas, lblEstado, progressBar, btnMasTarde, btnActualizar,
            });
        }
    }
}
