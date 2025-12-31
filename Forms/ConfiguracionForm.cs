using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Helpers;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing; // Necesario para PrinterSettings

namespace AlmacenDesktop.Forms
{
    public partial class ConfiguracionForm : Form
    {
        public ConfiguracionForm()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(ConfiguracionForm_KeyDown);
        }

        private void ConfiguracionForm_Load(object sender, EventArgs e)
        {
            CargarImpresoras();
            CargarDatos();
        }

        private void CargarImpresoras()
        {
            cboImpresoras.Items.Clear();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                cboImpresoras.Items.Add(printer);
            }

            // Si hay impresora por defecto y no se ha guardado otra, seleccionarla
            if (cboImpresoras.Items.Count > 0)
            {
                // Intenta seleccionar la predeterminada si no hay config
                PrinterSettings settings = new PrinterSettings();
                cboImpresoras.SelectedItem = settings.PrinterName;
            }
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

                        // Cargar impresora guardada
                        if (!string.IsNullOrEmpty(datos.NombreImpresora))
                        {
                            if (cboImpresoras.Items.Contains(datos.NombreImpresora))
                                cboImpresoras.SelectedItem = datos.NombreImpresora;
                            else
                                cboImpresoras.Items.Add(datos.NombreImpresora); // Agregar aunque no esté (por si se desconectó)
                        }
                    }
                    else
                    {
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

        private void btnCancelar_Click(object sender, EventArgs e) => this.Close();

        private void btnConfigAfip_Click(object sender, EventArgs e)
        {
            var formAfip = new ConfiguracionAfipForm();
            formAfip.ShowDialog();
        }

        private void GuardarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre de fantasía es obligatorio.");
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

                    // Guardar Impresora
                    if (cboImpresoras.SelectedItem != null)
                        datos.NombreImpresora = cboImpresoras.SelectedItem.ToString();

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

        private void ConfiguracionForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
            if (e.KeyCode == Keys.F10) GuardarDatos();
        }
    }
}