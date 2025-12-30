using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Helpers; // Para AudioHelper
using System;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ConfiguracionForm : Form
    {
        public ConfiguracionForm()
        {
            InitializeComponent();

            // Habilitar teclas rápidas
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(ConfiguracionForm_KeyDown);
        }

        private void ConfiguracionForm_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var datos = context.DatosNegocio.FirstOrDefault();
                    if (datos != null)
                    {
                        txtNombre.Text = datos.NombreFantasia;
                        txtRazon.Text = datos.RazonSocial;
                        txtCuit.Text = datos.CUIT;
                        txtDireccion.Text = datos.Direccion;
                        txtTelefono.Text = datos.Telefono;
                        txtMensaje.Text = datos.MensajeTicket;
                    }
                    else
                    {
                        // Valores por defecto si es la primera vez
                        txtMensaje.Text = "¡Gracias por su compra!";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarDatos();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // --- Integración con AFIP ---
        private void btnConfigAfip_Click(object sender, EventArgs e)
        {
            // Abre el formulario de configuración de AFIP que creamos anteriormente
            var formAfip = new ConfiguracionAfipForm();
            formAfip.ShowDialog();
        }

        private void GuardarDatos()
        {
            // Validaciones básicas de negocio
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre de fantasía es obligatorio para el ticket.");
                txtNombre.Focus();
                return;
            }

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var datos = context.DatosNegocio.FirstOrDefault();
                    if (datos == null)
                    {
                        datos = new DatosNegocio();
                        context.DatosNegocio.Add(datos);
                    }

                    datos.NombreFantasia = txtNombre.Text.Trim();
                    datos.RazonSocial = txtRazon.Text.Trim();
                    datos.CUIT = txtCuit.Text.Trim();
                    datos.Direccion = txtDireccion.Text.Trim();
                    datos.Telefono = txtTelefono.Text.Trim();
                    datos.MensajeTicket = txtMensaje.Text.Trim();

                    context.SaveChanges();

                    AudioHelper.PlayOk();
                    MessageBox.Show("Configuración guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show("Error al guardar: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Atajos de Teclado ---
        private void ConfiguracionForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            if (e.KeyCode == Keys.F10) // Guardar con F10 (Estándar de sistemas de gestión)
            {
                GuardarDatos();
            }
        }
    }
}