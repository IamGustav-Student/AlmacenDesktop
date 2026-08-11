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
            txtNotas.Text = string.IsNullOrWhiteSpace(_info.Notes) ? "(Sin notas de la versión)" : FormatearNotas(_info.Notes);
            txtNotas.SelectionStart = 0;
            txtNotas.SelectionLength = 0;
        }

        // El body del release de GitHub viene en Markdown con saltos de línea "\n" —
        // el TextBox nativo de Windows necesita "\r\n" para cortar renglones, si no
        // el texto se ve todo corrido y sin separaciones. De paso saca la sintaxis
        // Markdown básica (#, **) porque esto es una caja de texto plana, no un
        // renderizador de Markdown.
        private static string FormatearNotas(string notas)
        {
            string texto = notas.Replace("\r\n", "\n").Replace("\n", "\r\n");
            texto = System.Text.RegularExpressions.Regex.Replace(texto, @"^#{1,6}\s*", "", System.Text.RegularExpressions.RegexOptions.Multiline);
            texto = texto.Replace("**", "");
            return texto;
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
            // AutoScaleMode.None a propósito: con Font (o sin setear nada) el escalado
            // por DPI/tamaño de texto de Windows corría todas las posiciones en Y y
            // terminaba empujando los botones fuera del área visible del diálogo —
            // reportado con una foto real de pantalla donde "Actualizar ahora" quedaba
            // cortado abajo sin scroll para verlo. Con None, los píxeles que se piden
            // acá son los píxeles que se dibujan, sin sorpresas.
            this.AutoScaleMode = AutoScaleMode.None;
            this.Size = new Size(500, 480);
            this.Text = "Actualización disponible";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblTitulo = new Label
            {
                Text = "🚀 Nueva versión disponible",
                Location = new Point(20, 20),
                Size = new Size(460, 30),
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(124, 58, 237),
            };

            lblSubtitulo = new Label
            {
                Text = "Se recomienda actualizar para tener las últimas mejoras. Tus ventas, productos y clientes no se ven afectados.",
                Location = new Point(20, 55),
                Size = new Size(460, 45),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.DimGray,
            };

            txtNotas = new TextBox
            {
                Location = new Point(20, 105),
                Size = new Size(460, 190),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 9.5F),
                TabStop = false,
            };

            lblEstado = new Label
            {
                Location = new Point(20, 305),
                Size = new Size(460, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.SteelBlue,
                Visible = false,
            };

            progressBar = new ProgressBar
            {
                Location = new Point(20, 328),
                Size = new Size(460, 20),
                Minimum = 0,
                Maximum = 100,
                Visible = false,
            };

            // Margen generoso debajo de los botones (form termina en 480, botones
            // terminan en 433) a propósito, para que sobren ~45px pase lo que pase.
            btnMasTarde = new Button
            {
                Text = "Más tarde",
                Location = new Point(255, 395),
                Size = new Size(105, 38),
            };
            btnMasTarde.Click += btnMasTarde_Click;

            btnActualizar = new Button
            {
                Text = "Actualizar ahora",
                Location = new Point(370, 395),
                Size = new Size(115, 38),
                BackColor = Color.FromArgb(20, 184, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
            };
            btnActualizar.Click += btnActualizar_Click;

            this.Controls.AddRange(new Control[]
            {
                lblTitulo, lblSubtitulo, txtNotas, lblEstado, progressBar, btnMasTarde, btnActualizar,
            });

            this.AcceptButton = btnActualizar;
            this.ActiveControl = btnMasTarde;
        }
    }
}
