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
    }

    public class LicenseService
    {
        private static readonly HttpClient HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
        private static readonly string SharedHmacSecret = "hexastrategy_vendemax_secret_key_default";

        /// <summary>
        /// Realiza la verificación de licencia online contra el Servidor de Licencias HEXASTRATEGY.
        /// </summary>
        public async Task<(bool valido, string mensaje)> ValidarOnlineAsync(string email, string clave)
        {
            string url = $"{Constantes.API_LICENCIAS_URL}/api/licencia/validar";
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
                                        bool guardado = LicenseHelper.GuardarLicenciaLocal(email, clave, fechaVencimiento);
                                        if (guardado)
                                        {
                                            return (true, "Licencia validada y registrada correctamente.");
                                        }
                                        return (false, "Error interno al guardar los datos de licencia cifrados.");
                                    }
                                }
                            }
                            else
                            {
                                return (false, "Error de integridad: La firma digital del servidor no coincide.");
                            }
                        }
                    }
                    return (false, "Respuesta del servidor con formato inválido.");
                }
                else
                {
                    // Errores controlados (400, 403, 404, 500)
                    using (JsonDocument doc = JsonDocument.Parse(rawResponse))
                    {
                        var root = doc.RootElement;
                        if (root.TryGetProperty("mensaje", out var msgProp))
                        {
                            return (false, msgProp.GetString() ?? "Error al validar la licencia.");
                        }
                    }
                    return (false, $"Error del servidor: {(int)response.StatusCode} {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                // Fallas de red o servidor caído
                return (false, $"Error de red: {ex.Message}");
            }
        }

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
