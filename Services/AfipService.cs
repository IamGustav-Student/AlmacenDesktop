using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace AlmacenDesktop.Services
{
    public class AfipService
    {
        private ConfiguracionAfip _config;
        private readonly AfipAuthService _authService;
        private readonly AfipFacturacionService _facturacionService;

        // Constructor flexible: acepta configuración inyectada (para pruebas) o la carga de la BD
        public AfipService(ConfiguracionAfip config = null)
        {
            if (config != null)
            {
                _config = config;
            }
            else
            {
                // Carga 'Lazy' de la configuración si no se pasa
                using (var context = new AlmacenDbContext())
                {
                    _config = context.ConfiguracionesAfip.FirstOrDefault();
                }
            }

            _authService = new AfipAuthService();
            _facturacionService = new AfipFacturacionService();
        }

        public async Task AutenticarAsync()
        {
            if (_config == null) throw new Exception("No hay configuración de AFIP guardada.");

            // 1. Verificar si el Token actual sigue siendo válido (duran 12hs aprox)
            if (!string.IsNullOrEmpty(_config.Token) &&
                !string.IsNullOrEmpty(_config.Sign) &&
                _config.ExpiracionToken.HasValue &&
                _config.ExpiracionToken.Value > DateTime.Now)
            {
                return; // Ya estamos autenticados, no hacemos nada.
            }

            // 2. Si venció o no existe, iniciamos Login (WSAA)

            // --- CORRECCIÓN DEL ERROR DE COMPILACIÓN AQUÍ ---
            // Recuperamos la contraseña desencriptándola con DPAPI
            string certPassword = SecurityHelper.DesencriptarSecreto(_config.CertificadoPassword);

            // Validación de seguridad: si devuelve null, es que la encriptación falló (ej: cambio de PC)
            // o que la contraseña estaba vacía.
            if (certPassword == null && !string.IsNullOrEmpty(_config.CertificadoPassword))
            {
                throw new Exception("Error de Seguridad: No se pudo desencriptar la contraseña del certificado. Por favor, vuelva a configurarla en el menú 'Configuración AFIP'.");
            }

            // Si era null/vacío originalmente, usamos cadena vacía.
            if (certPassword == null) certPassword = "";

            // 3. Obtener Ticket de Acceso (TA)
            // Nota: GenerarLoginTicketRequest ahora debe recibir la pass real (plana) que acabamos de desencriptar
            var loginTicket = await _authService.ObtenerTicketAccesoAsync(_config.CertificadoPath, certPassword, _config.CuitEmisor, _config.EsProduccion);

            // 4. Actualizar la configuración en memoria y BD
            _config.Token = loginTicket.Token;
            _config.Sign = loginTicket.Sign;
            _config.ExpiracionToken = loginTicket.Expiracion;

            // Solo guardamos en BD si es una configuración persistente (tiene ID)
            if (_config.Id > 0)
            {
                using (var context = new AlmacenDbContext())
                {
                    var confDb = context.ConfiguracionesAfip.Find(_config.Id);
                    if (confDb != null)
                    {
                        confDb.Token = loginTicket.Token;
                        confDb.Sign = loginTicket.Sign;
                        confDb.ExpiracionToken = loginTicket.Expiracion;
                        context.SaveChanges();
                    }
                }
            }
        }

        public async Task<ResultadoAfip> FacturarVentaAsync(Venta venta)
        {
            // 1. Asegurar autenticación (Token válido)
            await AutenticarAsync();

            // 2. Delegar la facturación al servicio específico usando el Token válido
            return await _facturacionService.EmitirFacturaAsync(_config, venta);
        }
    }

    // Clase auxiliar para devolver resultados limpios a la UI
    public class ResultadoAfip
    {
        public bool Exito { get; set; }
        public string CAE { get; set; }
        public DateTime? Vencimiento { get; set; }
        public string NumeroComprobante { get; set; }
        public string Error { get; set; }
    }
}