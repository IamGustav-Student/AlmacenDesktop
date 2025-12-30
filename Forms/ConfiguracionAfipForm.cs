using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services;
using AlmacenDesktop.Helpers;
using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace AlmacenDesktop.Forms
{
    public partial class ConfiguracionAfipForm : Form
    {
        public ConfiguracionAfipForm()
        {
            InitializeComponent();
        }

        private void ConfiguracionAfipForm_Load(object sender, EventArgs e)
        {
            CargarConfiguracion();
        }

        private void CargarConfiguracion()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var config = context.ConfiguracionesAfip.FirstOrDefault();
                    if (config != null)
                    {
                        txtCuit.Text = config.CuitEmisor.ToString();
                        numPuntoVenta.Value = config.PuntoVenta;
                        txtCertificadoPath.Text = config.CertificadoPath;
                        txtPassword.Text = config.CertificadoPassword;
                        chkProduccion.Checked = config.EsProduccion;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar la configuración previa: " + ex.Message);
            }
        }

        private void btnBuscarCertificado_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Certificados Digitales (*.p12;*.pfx)|*.p12;*.pfx|Todos los archivos (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtCertificadoPath.Text = ofd.FileName;
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos()) return;

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var config = context.ConfiguracionesAfip.FirstOrDefault();
                    if (config == null)
                    {
                        config = new ConfiguracionAfip();
                        context.ConfiguracionesAfip.Add(config);
                    }

                    config.CuitEmisor = long.Parse(txtCuit.Text);
                    config.PuntoVenta = (int)numPuntoVenta.Value;
                    config.CertificadoPath = txtCertificadoPath.Text;
                    config.CertificadoPassword = txtPassword.Text;
                    config.EsProduccion = chkProduccion.Checked;

                    // Resetear token al cambiar configuración para forzar nueva autenticación
                    config.Token = null;
                    config.Sign = null;
                    config.ExpiracionToken = null;

                    context.SaveChanges();
                    AudioHelper.PlayOk();
                    MessageBox.Show("Configuración guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show("Error al guardar en base de datos: " + ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnProbar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos()) return;

            // Validar existencia física del certificado ANTES de llamar al servicio
            if (!File.Exists(txtCertificadoPath.Text))
            {
                MessageBox.Show("El archivo del certificado no existe en la ruta especificada.", "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validar que la contraseña abra el certificado (Check rápido local)
            try
            {
                new X509Certificate2(txtCertificadoPath.Text, txtPassword.Text);
            }
            catch (Exception exCert)
            {
                MessageBox.Show("La contraseña del certificado es incorrecta o el archivo está dañado.\nError: " + exCert.Message, "Certificado Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnProbar.Enabled = false;
            lblEstado.Text = "Conectando con AFIP...";
            lblEstado.ForeColor = System.Drawing.Color.Blue;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                // Configuración temporal en memoria para la prueba
                var configTemp = new ConfiguracionAfip
                {
                    Id = 0, // ID 0 indica modo test manual (no guardar token en BD)
                    CuitEmisor = long.Parse(txtCuit.Text),
                    PuntoVenta = (int)numPuntoVenta.Value,
                    CertificadoPath = txtCertificadoPath.Text,
                    CertificadoPassword = txtPassword.Text,
                    EsProduccion = chkProduccion.Checked
                };

                var servicio = new AfipService(configTemp); // Inyección de dependencia manual
                await servicio.AutenticarAsync(); // Intento de Login (WSAA)

                AudioHelper.PlayOk();
                lblEstado.Text = "¡Conexión Exitosa!";
                lblEstado.ForeColor = System.Drawing.Color.Green;
                MessageBox.Show("¡Conexión con AFIP exitosa!\n\nSe obtuvo Token y Sign correctamente.\nEl sistema está listo para facturar.", "Prueba OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                lblEstado.Text = "Error de Conexión";
                lblEstado.ForeColor = System.Drawing.Color.Red;

                string mensajeAyuda = "\n\nPosibles causas:\n" +
                                      "1. CUIT incorrecto (Debe coincidir con el certificado).\n" +
                                      "2. Certificado vencido o revocado.\n" +
                                      "3. Servicio de AFIP caído (intente más tarde).\n" +
                                      "4. Hora de la PC desincronizada.";

                MessageBox.Show("Fallo de conexión con AFIP:\n" + ex.Message + mensajeAyuda, "Error AFIP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnProbar.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtCuit.Text) || !long.TryParse(txtCuit.Text, out _))
            {
                MessageBox.Show("Ingrese un CUIT numérico válido (11 dígitos).");
                txtCuit.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCertificadoPath.Text))
            {
                MessageBox.Show("Seleccione el archivo .p12");
                btnBuscarCertificado.Focus();
                return false;
            }

            return true;
        }
    }
}