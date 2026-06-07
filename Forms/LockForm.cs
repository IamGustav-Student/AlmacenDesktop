using System;
using System.Diagnostics;
using System.Windows.Forms;
using AlmacenDesktop.Helpers;
using AlmacenDesktop.Services;

namespace AlmacenDesktop.Forms
{
    public partial class LockForm : Form
    {
        private readonly LicenseService _licenseService;

        public LockForm(string mensajeBloqueo)
        {
            InitializeComponent();
            _licenseService = new LicenseService();
            lblMensaje.Text = mensajeBloqueo;
        }

        private async void btnRevalidar_Click(object sender, EventArgs e)
        {
            var licencia = LicenseHelper.LeerLicenciaLocal();

            if (licencia == null)
            {
                // Si no hay licencia local, derivamos a la pantalla de activación
                using (var activationForm = new ActivationForm())
                {
                    if (activationForm.ShowDialog() == DialogResult.OK)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                return;
            }

            btnRevalidar.Enabled = false;
            btnRevalidar.Text = "COMPROBANDO PAGO...";
            Cursor = Cursors.WaitCursor;

            try
            {
                // Intentar validar online nuevamente con la clave guardada
                var (valido, mensaje) = await _licenseService.ValidarOnlineAsync(licencia.Email, licencia.Clave);

                if (valido)
                {
                    AudioHelper.PlayOk();
                    MessageBox.Show("¡Pago verificado! Su suscripción está al día. El sistema ha sido desbloqueado.", "Acceso Concedido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    AudioHelper.PlayError();
                    MessageBox.Show($"La suscripción no pudo ser reactivada: {mensaje}", "Suscripción Pendiente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show($"Error de conexión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRevalidar.Enabled = true;
                btnRevalidar.Text = "REINTENTAR COMPROBACIÓN";
                Cursor = Cursors.Default;
            }
        }

        private void lnkPagar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Constantes.API_LICENCIAS_URL, // URL de la landing de suscripción en Constantes
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
