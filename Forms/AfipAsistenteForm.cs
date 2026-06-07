using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Helpers;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class AfipAsistenteForm : Form
    {
        private string _tempPrivateKeyPath = "";

        public AfipAsistenteForm()
        {
            InitializeComponent();
            _tempPrivateKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_private.key");
        }

        private void AfipAsistenteForm_Load(object sender, EventArgs e)
        {
            lblPaso1Status.Text = "Esperando generación...";
            lblPaso1Status.ForeColor = Color.DimGray;
            lblPaso3Status.Text = "Esperando certificado...";
            lblPaso3Status.ForeColor = Color.DimGray;
        }

        private void btnGenerarCSR_Click(object sender, EventArgs e)
        {
            string cuitText = txtCuit.Text.Trim();
            if (string.IsNullOrWhiteSpace(cuitText) || cuitText.Length != 11 || !long.TryParse(cuitText, out long cuit))
            {
                AudioHelper.PlayError();
                MessageBox.Show("Por favor, ingrese un CUIT válido de 11 dígitos sin guiones.", "CUIT Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCuit.Focus();
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Solicitud de Certificado (*.csr)|*.csr";
                sfd.FileName = $"pedido_afip_{cuit}.csr";
                sfd.Title = "Guardar Solicitud de Certificado (CSR) para AFIP";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    btnGenerarCSR.Enabled = false;
                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        // 1. Generar par de claves RSA 2048 de forma 100% nativa (.NET Core)
                        using (RSA rsa = RSA.Create(2048))
                        {
                            // Exportar clave privada en formato PEM PKCS#8
                            string privateKeyPem = rsa.ExportPkcs8PrivateKeyPem();
                            File.WriteAllText(_tempPrivateKeyPath, privateKeyPem, Encoding.UTF8);

                            // 2. Construir la solicitud de certificado (CSR / PKCS#10)
                            var subject = new X500DistinguishedName($"C=AR, O=VENDEMAX, CN=VENDEMAX_{cuit}, SerialNumber=CUIT {cuit}");
                            var request = new CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                            // Codificación DER
                            byte[] derEncoded = request.CreateSigningRequest();
                            
                            // Convertir a PEM
                            string csrPem = ConvertirAPem("CERTIFICATE REQUEST", derEncoded);
                            
                            File.WriteAllText(sfd.FileName, csrPem, Encoding.UTF8);

                            AudioHelper.PlayOk();
                            lblPaso1Status.Text = "¡Claves y CSR generados con éxito!";
                            lblPaso1Status.ForeColor = Color.Green;
                            txtLogCSR.Text = $"CSR guardado en:\r\n{sfd.FileName}\r\n\r\nClave privada temporal creada en el directorio base.\r\n\r\nYa puede subir el archivo .csr a la web de AFIP.";
                            
                            MessageBox.Show($"¡Solicitud generada con éxito!\r\n\r\nSe ha creado el archivo:\r\n{Path.GetFileName(sfd.FileName)}\r\n\r\nContinúe con el Paso 2 en la web de AFIP.", "Operación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        AudioHelper.PlayError();
                        MessageBox.Show("Fallo al generar claves criptográficas: " + ex.Message, "Error Cripto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        btnGenerarCSR.Enabled = true;
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private string ConvertirAPem(string cabecera, byte[] derBytes)
        {
            string base64 = Convert.ToBase64String(derBytes, Base64FormattingOptions.InsertLineBreaks);
            return $"-----BEGIN {cabecera}-----\r\n{base64}\r\n-----END {cabecera}-----";
        }

        private void btnBuscarCrt_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Certificado AFIP (*.crt;*.txt)|*.crt;*.txt|Todos (*.*)|*.*";
                ofd.Title = "Seleccionar Certificado emitido por AFIP";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtCrtPath.Text = ofd.FileName;
                }
            }
        }

        private void btnAsociar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCrtPath.Text) || !File.Exists(txtCrtPath.Text))
            {
                MessageBox.Show("Por favor, seleccione el archivo de certificado (.crt) descargado de AFIP.", "Falta Certificado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(_tempPrivateKeyPath))
            {
                AudioHelper.PlayError();
                MessageBox.Show("No se encontró la clave privada temporal generada en el Paso 1. Debe generar el CSR primero en este mismo equipo.", "Falta Clave Privada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string password = txtCertPassword.Text.Trim();
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor, cree una contraseña para proteger su nuevo contenedor criptográfico P12.", "Falta Contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCertPassword.Focus();
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Certificado Personal PKCS12 (*.p12)|*.p12";
                sfd.FileName = "certificado_afip_produccion.p12";
                sfd.Title = "Guardar Certificado Integrado (.p12)";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    btnAsociar.Enabled = false;
                    this.Cursor = Cursors.WaitCursor;

                    try
                    {
                        // 1. Cargar el Certificado X509 (.crt)
                        byte[] certBytes = File.ReadAllBytes(txtCrtPath.Text);
                        using (var certificado = new X509Certificate2(certBytes))
                        {
                            // 2. Leer la clave privada temporal
                            string keyPem = File.ReadAllText(_tempPrivateKeyPath, Encoding.UTF8);
                            using (var rsa = RSA.Create())
                            {
                                rsa.ImportFromPem(keyPem);

                                // 3. Asociar certificado y clave privada
                                using (var certificadoConClave = certificado.CopyWithPrivateKey(rsa))
                                {
                                    // 4. Exportar como contenedor PKCS12 (.p12) protegido con clave
                                    byte[] p12Bytes = certificadoConClave.Export(X509ContentType.Pkcs12, password);
                                    File.WriteAllBytes(sfd.FileName, p12Bytes);

                                    // 5. Guardar en la configuración de la Base de Datos
                                    using (var context = new AlmacenDbContext())
                                    {
                                        var config = context.ConfiguracionesAfip.FirstOrDefault();
                                        if (config == null)
                                        {
                                            config = new ConfiguracionAfip();
                                            context.ConfiguracionesAfip.Add(config);
                                        }

                                        config.CuitEmisor = long.Parse(txtCuit.Text.Trim());
                                        config.CertificadoPath = sfd.FileName;
                                        config.CertificadoPassword = SecurityHelper.EncriptarSecreto(password);
                                        config.EsProduccion = true; // Por defecto es producción tras el trámite

                                        // Resetear credenciales de sesión activas para forzar inicio limpio con el nuevo cert
                                        config.Token = null;
                                        config.Sign = null;
                                        config.ExpiracionToken = null;

                                        context.SaveChanges();
                                    }

                                    // Borrar clave privada temporal del disco por máxima seguridad
                                    File.Delete(_tempPrivateKeyPath);

                                    AudioHelper.PlayOk();
                                    lblPaso3Status.Text = "¡Instalación Completada y Encriptada!";
                                    lblPaso3Status.ForeColor = Color.Green;

                                    MessageBox.Show("¡Excelente!\r\n\r\nEl certificado ha sido integrado, guardado y cifrado localmente de forma segura en la base de datos mediante Windows DPAPI.\r\n\r\nEl módulo de Facturación Electrónica está listo para emitir Facturas Oficiales en Producción.", "Operación Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    this.Close();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AudioHelper.PlayError();
                        MessageBox.Show("Fallo al integrar certificado con clave privada: " + ex.Message, "Error Integración", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        btnAsociar.Enabled = true;
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }
    }
}
