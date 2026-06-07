using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services;
using AlmacenDesktop.Helpers;
using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Drawing; // Necesario para cambiar colores de labels

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
                        chkProduccion.Checked = config.EsProduccion;

                        // LÓGICA DE RECUPERACIÓN DE CONTRASEÑA
                        if (!string.IsNullOrEmpty(config.CertificadoPassword))
                        {
                            string passDesencriptada = SecurityHelper.DesencriptarSecreto(config.CertificadoPassword);

                            if (passDesencriptada == null)
                            {
                                // CASO CRÍTICO: Había algo en la BD pero no se pudo desencriptar.
                                // Significa que es una contraseña vieja en texto plano.
                                txtPassword.Text = "";
                                txtPassword.BackColor = Color.LightYellow;
                                lblEstado.Text = "⚠️ Re-ingrese su contraseña";
                                lblEstado.ForeColor = Color.OrangeRed;
                                MessageBox.Show(
                                    "Actualización de Seguridad:\n\n" +
                                    "Se ha detectado una configuración antigua no segura.\n" +
                                    "Por favor, vuelva a escribir la contraseña de su certificado y guarde los cambios para encriptarla.",
                                    "Seguridad VENDEMAX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                // Todo OK, contraseña segura recuperada
                                txtPassword.Text = passDesencriptada;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al leer configuración: " + ex.Message);
            }
        }

        private void btnBuscarCertificado_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Certificados P12 (*.p12)|*.p12|Todos (*.*)|*.*";
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
                    config.EsProduccion = chkProduccion.Checked;

                    // ENCRIPTACIÓN OBLIGATORIA
                    // Si el usuario deja vacío, asumimos que no quiere cambiarla (si ya existía) 
                    // PERO si era nula por error de desencriptación, debe ingresarla sí o sí.

                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        // Si está vacío, solo permitimos guardar si NO estamos arreglando una pass rota
                        // Como es difícil saberlo aquí, mejor obligamos siempre a ponerla si está vacía.
                        MessageBox.Show("Por seguridad, debe ingresar la contraseña del certificado nuevamente.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txtPassword.Focus();
                        return;
                    }

                    config.CertificadoPassword = SecurityHelper.EncriptarSecreto(txtPassword.Text);

                    // Resetear tokens para forzar login limpio
                    config.Token = null;
                    config.Sign = null;
                    config.ExpiracionToken = null;

                    context.SaveChanges();

                    AudioHelper.PlayOk();
                    MessageBox.Show("Configuración guardada y ENCRIPTADA correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show("Error al guardar: " + ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnProbar_Click(object sender, EventArgs e)
        {
            // Validaciones previas
            if (!ValidarDatos()) return;
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Ingrese la contraseña para probar.");
                return;
            }
            if (!File.Exists(txtCertificadoPath.Text))
            {
                MessageBox.Show("El archivo .p12 no existe en la ruta indicada.");
                return;
            }

            // Prueba local de contraseña (rápida)
            try
            {
                new X509Certificate2(txtCertificadoPath.Text, txtPassword.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("La contraseña es incorrecta para este certificado.\n" + ex.Message, "Error Clave", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnProbar.Enabled = false;
            lblEstado.Text = "Conectando...";
            lblEstado.ForeColor = Color.Blue;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                // Creamos config temporal en memoria (con pass PLANA porque AfipService la encriptará internamente o usará el servicio Auth)
                // OJO: AfipService espera leer de BD la pass encriptada si le pasamos config NULL.
                // Si le pasamos config explícita, debemos ser consistentes.

                // MEJOR ESTRATEGIA: Guardar primero, luego probar.
                // Pero si queremos probar sin guardar, el AfipService debe manejar pass plana en el constructor temporal?
                // En mi implementación anterior de AfipService, NO usaba SecurityHelper si le pasabas el objeto config.
                // Vamos a usar una instancia directa de AfipAuthService para la prueba, es más directo.

                var auth = new AfipAuthService();
                var ticket = await auth.ObtenerTicketAccesoAsync(
                    txtCertificadoPath.Text,
                    txtPassword.Text, // Pasamos la pass plana que el usuario acaba de tipear
                    long.Parse(txtCuit.Text),
                    chkProduccion.Checked
                );

                AudioHelper.PlayOk();
                lblEstado.Text = "¡Conexión Exitosa!";
                lblEstado.ForeColor = Color.Green;
                MessageBox.Show($"¡Prueba Exitosa!\n\nToken recibido OK.\nExpiración: {ticket.Expiracion}", "AFIP Online", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                lblEstado.Text = "Fallo Conexión";
                lblEstado.ForeColor = Color.Red;
                MessageBox.Show("Error conectando a AFIP:\n" + ex.Message, "Fallo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnProbar.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtCuit.Text)) { MessageBox.Show("Falta CUIT"); return false; }
            if (string.IsNullOrWhiteSpace(txtCertificadoPath.Text)) { MessageBox.Show("Falta Certificado"); return false; }
            return true;
        }
    }
}