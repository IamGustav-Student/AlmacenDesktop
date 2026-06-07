using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AlmacenDesktop.Helpers;
using AlmacenDesktop.Services;

namespace AlmacenDesktop.Forms
{
    public partial class ActivationForm : Form
    {
        private readonly LicenseService _licenseService;

        public ActivationForm()
        {
            InitializeComponent();
            _licenseService = new LicenseService();
            
            // Cargar datos de hardware
            lblHardware.Text = $"ID de Hardware (Fingerprint): {HardwareHelper.ObtenerMachineFingerprint().Substring(0, 16)}...";
        }

        private async void btnActivar_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string clave = txtClave.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(clave))
            {
                AudioHelper.PlayError();
                MessageBox.Show("Por favor complete todos los campos.", "Campos Incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnActivar.Enabled = false;
            btnActivar.Text = "VALIDANDO ONLINE...";
            Cursor = Cursors.WaitCursor;

            try
            {
                var (valido, mensaje) = await _licenseService.ValidarOnlineAsync(email, clave);

                if (valido)
                {
                    AudioHelper.PlayOk();
                    MessageBox.Show("¡VENDEMAX v2.0 ha sido activado exitosamente en este equipo!", "Activación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    AudioHelper.PlayError();
                    MessageBox.Show($"Error al activar: {mensaje}", "Error de Activación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show($"Ocurrió un error inesperado al conectar al servidor: {ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnActivar.Enabled = true;
                btnActivar.Text = "ACTIVAR SISTEMA";
                Cursor = Cursors.Default;
            }
        }

        private void lnkComprar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // Abrir la landing page en el navegador
                Process.Start(new ProcessStartInfo
                {
                    FileName = Constantes.API_LICENCIAS_URL, // En producción cambiar a la url real de la landing en Constantes
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el navegador: " + ex.Message);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
