using System;
using System.Drawing;
using System.Windows.Forms;
using AlmacenDesktop.Modelos;

namespace AlmacenDesktop.Forms
{
    public partial class MenuPrincipal : Form
    {
        private Button currentButton;
        private Form activeForm;
        private Usuario _usuarioActual;

        public MenuPrincipal(Usuario usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;
            string nombreUsuario = _usuarioActual != null ? _usuarioActual.NombreUsuario : "Admin";
            this.Text = $"VENDEMAX - Usuario: {nombreUsuario}";
        }

        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currentButton != (Button)btnSender)
                {
                    DisableButton();
                    Color color = Color.FromArgb(0, 150, 136); // Color VENDEMAX
                    currentButton = (Button)btnSender;
                    currentButton.BackColor = color;
                    currentButton.ForeColor = Color.White;
                    currentButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
                }
            }
        }

        private void DisableButton()
        {
            foreach (Control previousBtn in panelMenu.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.FromArgb(51, 51, 76);
                    previousBtn.ForeColor = Color.Gainsboro;
                    previousBtn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                }
            }
        }

        private void OpenChildForm(Form childForm, object btnSender)
        {
            if (activeForm != null)
                activeForm.Close();

            ActivateButton(btnSender);
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.panelDesktop.Controls.Add(childForm);
            this.panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            lblBienvenida.Visible = false;
        }

        // --- EVENTOS PRINCIPALES ---

        private void btnVentas_Click(object sender, EventArgs e)
        {
            OpenChildForm(new VentasForm(_usuarioActual), sender);
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ProductosForm(), sender);
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ClientesForm(), sender); // Restaurado
        }

        private void btnProveedores_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ProveedoresForm(), sender); // Restaurado
        }

        private void btnCompras_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ComprasForm(_usuarioActual), sender); // Restaurado
        }

        private void btnCaja_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ControlCajaForm(_usuarioActual), sender);
        }

        private void btnCtaCte_Click(object sender, EventArgs e)
        {
            OpenChildForm(new CuentaCorrienteForm(), sender); // Restaurado
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ReporteGananciasForm(), sender);
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ConfiguracionForm(), sender);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}