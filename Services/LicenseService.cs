using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AlmacenDesktop.Helpers;

namespace AlmacenDesktop.Services
{
    public class LicenseValidationPayload
    {
        public bool Valido { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public string FechaVencimiento { get; set; } = string.Empty;
        public long Timestamp { get; set; }

        // Campos agregados del lado del servidor. Las versiones viejas del cliente
        // simplemente los ignoran (System.Text.Json descarta lo desconocido), por
        // eso se pueden sumar sin romper las instalaciones ya distribuidas.
        public string Estado { get; set; } = string.Empty;
        public int DiasRestantes { get; set; }
    }

    public class ResultadoValidacionOnline
    {
        public bool Valido { get; set; }
        public string Mensaje { get; set; } = string.Empty;

        /// <summary>
        /// El servidor respondió y rechazó la licencia (suspendida, cancelada,
        /// inexistente u otro equipo). Distinto de no haber podido consultarlo:
        /// un rechazo definitivo bloquea, una falla de red cae en gracia offline.
        /// </summary>
        public bool RechazoDefinitivo { get; set; }

        public string EstadoServidor { get; set; } = string.Empty;
    }

    public class LicenseService
    {
        private static readonly HttpClient HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
        // Debe coincidir con VENDEMAX_DESKTOP_VALIDATE_SECRET en ops-dashboard.
        private static readonly string SharedHmacSecret = "65c2093009ef55bda192bab51373e6af6a8ce6f8117594270ed962d30990f82e";

        /// <summary>
        /// Realiza la verificación de licencia online contra el Servidor de Licencias HEXASTRATEGY.
        /// </summary>
        public async Task<(bool valido, string mensaje)> ValidarOnlineAsync(string email, string clave)
        {
            var r = await ValidarOnlineDetalladoAsync(email, clave);
            return (r.Valido, r.Mensaje);
        }

        /// <summary>
        /// Igual que <see cref="ValidarOnlineAsync"/> pero informando si el rechazo
        /// vino del servidor o si simplemente no se pudo consultar.
        /// </summary>
        public async Task<ResultadoValidacionOnline> ValidarOnlineDetalladoAsync(string email, string clave)
        {
            string url = $"{Constantes.API_LICENCIAS_URL}/licencias/validar";
            string fingerprint = HardwareHelper.ObtenerMachineFingerprint();

            var requestBody = new
            {
                email = email.Trim().ToLower(),
                clave = clave.Trim().ToUpper(),
                machineFingerprint = fingerprint
            };

            try
            {
                string jsonRequest = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await HttpClient.PostAsync(url, content);
                string rawResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(rawResponse))
                    {
                        var root = doc.RootElement;
                        if (root.TryGetProperty("payload", out var payloadProp) && root.TryGetProperty("signature", out var sigProp))
                        {
                            string payloadStr = payloadProp.GetString() ?? string.Empty;
                            string receivedSignature = sigProp.GetString() ?? string.Empty;

                            // Verificar la autenticidad de la firma HMAC
                            if (VerificarFirmaHMAC(payloadStr, receivedSignature))
                            {
                                var payloadObj = JsonSerializer.Deserialize<LicenseValidationPayload>(payloadStr, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                                if (payloadObj != null && payloadObj.Valido)
                                {
                                    if (DateTime.TryParse(payloadObj.FechaVencimiento, out DateTime fechaVencimiento))
                                    {
                                        // Guardar localmente de forma cifrada mediante DPAPI
                                        string estado = string.IsNullOrWhiteSpace(payloadObj.Estado) ? "ACTIVO" : payloadObj.Estado;
                                        bool guardado = LicenseHelper.GuardarLicenciaLocal(email, clave, fechaVencimiento, estado);
                                        if (guardado)
                                        {
                                            return Ok("Licencia validada y registrada correctamente.", estado);
                                        }
                                        return Falla("Error interno al guardar los datos de licencia cifrados.");
                                    }
                                }
                            }
                            else
                            {
                                return Falla("Error de integridad: La firma digital del servidor no coincide.");
                            }
                        }
                    }
                    return Falla("Respuesta del servidor con formato inválido.");
                }
                else
                {
                    // 403 = suspendida/cancelada/otro equipo. 404 = no existe.
                    // Ambos son veredictos del servidor, no problemas de conexión.
                    bool definitivo = (int)response.StatusCode == 403 || (int)response.StatusCode == 404;

                    string mensaje = $"Error del servidor: {(int)response.StatusCode} {response.ReasonPhrase}";
                    try
                    {
                        using (JsonDocument doc = JsonDocument.Parse(rawResponse))
                        {
                            if (doc.RootElement.TryGetProperty("mensaje", out var msgProp))
                                mensaje = msgProp.GetString() ?? mensaje;
                        }
                    }
                    catch { /* respuesta no-JSON: queda el mensaje genérico */ }

                    return new ResultadoValidacionOnline
                    {
                        Valido = false,
                        Mensaje = mensaje,
                        RechazoDefinitivo = definitivo,
                        EstadoServidor = definitivo ? "SUSPENDIDO" : string.Empty,
                    };
                }
            }
            catch (Exception ex)
            {
                // Sin internet o servidor caído: NO es un rechazo. Que caiga en la
                // gracia offline en vez de bloquear a un comercio que sí pagó.
                return Falla($"Error de red: {ExceptionHelper.ObtenerMensaje(ex)}");
            }
        }

        private static ResultadoValidacionOnline Ok(string mensaje, string estado) =>
            new ResultadoValidacionOnline { Valido = true, Mensaje = mensaje, EstadoServidor = estado };

        private static ResultadoValidacionOnline Falla(string mensaje) =>
            new ResultadoValidacionOnline { Valido = false, Mensaje = mensaje, RechazoDefinitivo = false };

        private bool VerificarFirmaHMAC(string payload, string signature)
        {
            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(SharedHmacSecret);
                using (var hmac = new HMACSHA256(keyBytes))
                {
                    byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
                    byte[] calculatedHash = hmac.ComputeHash(payloadBytes);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < calculatedHash.Length; i++)
                    {
                        sb.Append(calculatedHash[i].ToString("x2"));
                    }

                    string calculatedSignature = sb.ToString();
                    return string.Equals(calculatedSignature, signature, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
