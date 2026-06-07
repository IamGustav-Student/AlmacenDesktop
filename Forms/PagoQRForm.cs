using AlmacenDesktop.Helpers;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class PagoQRForm : Form
    {
        private decimal _total;
        private System.Windows.Forms.Timer _countdownTimer;
        private int _segundosRestantes = 60;
        private bool _pagoCompletado = false;

        public PagoQRForm(decimal total)
        {
            InitializeComponent();
            _total = total;
            
            _countdownTimer = new System.Windows.Forms.Timer();
            _countdownTimer.Interval = 1000;
            _countdownTimer.Tick += CountdownTimer_Tick;
        }

        private void PagoQRForm_Load(object sender, EventArgs e)
        {
            lblMonto.Text = $"Importe a Cobrar: {_total:C2}";
            GenerarCodigoQRPago();
            
            _countdownTimer.Start();
            SimularPollingMercadoPago();
        }

        private void GenerarCodigoQRPago()
        {
            try
            {
                // Generamos un QR real con el estándar de Mercado Pago / Coelsa interoperable simulado
                string payloadInteroperable = $"00020101021243650016com.mercadopago0215mock_pref_987655204000053030325412{_total:F2}5802AR5912VENDEMAX_POS6009ARGENTINA62070703123";
                
                picQr.Image = GenerarQR(payloadInteroperable, picQr.Width);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error QR: " + ex.Message);
                lblEstado.Text = "Fallo al generar QR";
                lblEstado.ForeColor = Color.Red;
            }
        }

        private Bitmap GenerarQR(string contenido, int size)
        {
            var escritor = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = size,
                    Width = size,
                    Margin = 1
                }
            };
            var pixelData = escritor.Write(contenido);
            var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
            return bitmap;
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            _segundosRestantes--;
            lblTimer.Text = $"El código expira en: {_segundosRestantes} seg.";

            if (_segundosRestantes <= 0)
            {
                _countdownTimer.Stop();
                if (!_pagoCompletado)
                {
                    AudioHelper.PlayError();
                    MessageBox.Show("El código de pago QR ha expirado. Por favor, reintente la operación.", "Código Expirado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
        }

        private async void SimularPollingMercadoPago()
        {
            // Simula el polling asíncrono consultando los servidores de Mercado Pago en segundo plano (Webhooks/API)
            string[] estados = {
                "Generando orden QR...",
                "Aguardando escaneo del cliente...",
                "Procesando transacción...",
                "Verificando saldo..."
            };

            try
            {
                for (int i = 0; i < estados.Length; i++)
                {
                    if (_pagoCompletado) return; // Si el programador clickea el botón de test, aborta el polling asíncrono
                    
                    lblEstado.Text = estados[i];
                    await Task.Delay(1800);
                }

                // Simular aprobación exitosa automática tras el ciclo de polling (UX fluida de demostración)
                AprobarPago();
            }
            catch
            {
                // Resguardar fallos asíncronos si el form se cierra rápido
            }
        }

        private void AprobarPago()
        {
            if (_pagoCompletado) return;
            
            _pagoCompletado = true;
            _countdownTimer.Stop();
            
            lblEstado.Text = "¡PAGO APROBADO!";
            lblEstado.ForeColor = Color.Green;
            picSuccess.Visible = true;
            picQr.Visible = false;

            AudioHelper.PlayCobro();
            
            // Retardo sutil de éxito antes de cerrar el form
            System.Windows.Forms.Timer delayClose = new System.Windows.Forms.Timer();
            delayClose.Interval = 1200;
            delayClose.Tick += (s, ev) =>
            {
                delayClose.Stop();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            delayClose.Start();
        }

        private void btnSimularAprobado_Click(object sender, EventArgs e)
        {
            AprobarPago();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            _countdownTimer.Stop();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
