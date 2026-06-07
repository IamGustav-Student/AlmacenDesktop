using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Http;
using System.Net; // Necesario para TLS
using System.IO;

namespace AlmacenDesktop.Services
{
    public class AfipAuthService
    {
        private const string URL_WSAA_HOMO = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms";
        private const string URL_WSAA_PROD = "https://wsaa.afip.gov.ar/ws/services/LoginCms";

        public class TicketAcceso
        {
            public string Token { get; set; }
            public string Sign { get; set; }
            public DateTime Expiracion { get; set; }
        }

        public async Task<TicketAcceso> ObtenerTicketAccesoAsync(string certPath, string certPass, long cuit, bool esProduccion)
        {
            // 1. CONFIGURACIÓN DE RED "NIVEL DIOS"
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // 2. Generación del CMS Firmado
            string cmsFirmadoBase64;
            try
            {
                cmsFirmadoBase64 = GenerarLoginTicketRequest(certPath, certPass, "wsfe");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generando firma criptográfica local:\n{ex.Message}");
            }

            // 3. Construcción del SOAP
            string soapXml = ConstruirSobreSoap(cmsFirmadoBase64);
            string url = esProduccion ? URL_WSAA_PROD : URL_WSAA_HOMO;

            // 4. Envío a AFIP con Diagnóstico de Respuesta
            string respuestaSoap = await EnviarSoapAsync(url, soapXml);

            // 5. Interpretación Inteligente
            return ParsearRespuestaWsaa(respuestaSoap);
        }

        private string GenerarLoginTicketRequest(string certificadoPath, string password, string servicio)
        {
            if (!File.Exists(certificadoPath)) throw new FileNotFoundException("No se encuentra el archivo .p12", certificadoPath);

            string traXml = GenerarTRA(servicio);

            X509Certificate2 certificado;
            try
            {
                certificado = new X509Certificate2(certificadoPath, password, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                throw new Exception("Contraseña incorrecta del certificado.");
            }

            return FirmarMensaje(traXml, certificado);
        }

        private string GenerarTRA(string servicio)
        {
            var uniqueId = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var generationTime = DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-ddTHH:mm:ss");
            var expirationTime = DateTime.Now.AddMinutes(+10).ToString("yyyy-MM-ddTHH:mm:ss");

            return $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                   $"<loginTicketRequest version=\"1.0\">" +
                   $"<header>" +
                   $"<uniqueId>{uniqueId}</uniqueId>" +
                   $"<generationTime>{generationTime}</generationTime>" +
                   $"<expirationTime>{expirationTime}</expirationTime>" +
                   $"</header>" +
                   $"<service>{servicio}</service>" +
                   $"</loginTicketRequest>";
        }

        private string FirmarMensaje(string mensajeXml, X509Certificate2 certificado)
        {
            byte[] msgBytes = Encoding.UTF8.GetBytes(mensajeXml);
            ContentInfo contentInfo = new ContentInfo(msgBytes);
            SignedCms signedCms = new SignedCms(contentInfo);
            CmsSigner signer = new CmsSigner(certificado);
            signer.IncludeOption = X509IncludeOption.EndCertOnly;
            signedCms.ComputeSignature(signer);
            return Convert.ToBase64String(signedCms.Encode());
        }

        private string ConstruirSobreSoap(string cmsBase64)
        {
            return "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:wsaa=\"http://wsaa.view.sua.dvadac.desein.afip.gov\">" +
                   "<soapenv:Header/>" +
                   "<soapenv:Body>" +
                   "<wsaa:loginCms>" +
                   $"<in0>{cmsBase64}</in0>" +
                   "</wsaa:loginCms>" +
                   "</soapenv:Body>" +
                   "</soapenv:Envelope>";
        }

        private async Task<string> EnviarSoapAsync(string url, string soapXml)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    client.DefaultRequestHeaders.Add("SOAPAction", "");

                    var content = new StringContent(soapXml, Encoding.UTF8, "text/xml");

                    HttpResponseMessage response = null;
                    string responseContent = "";

                    try
                    {
                        response = await client.PostAsync(url, content);
                        responseContent = await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error de RED al intentar conectar con AFIP ({url}): {ex.Message}");
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        // Si es un SOAP Fault, lo dejamos pasar para el análisis detallado
                        if (responseContent.Contains("Fault") || responseContent.Contains("fault") || responseContent.Contains("exception"))
                        {
                            return responseContent;
                        }
                        string errorLimpio = responseContent.Length > 300 ? responseContent.Substring(0, 300) + "..." : responseContent;
                        throw new Exception($"AFIP respondió con ERROR {response.StatusCode}.\nDetalle Técnico: {errorLimpio}");
                    }

                    return responseContent;
                }
            }
        }

        private TicketAcceso ParsearRespuestaWsaa(string xmlResponse)
        {
            var doc = new XmlDocument();
            try
            {
                doc.LoadXml(xmlResponse);
            }
            catch
            {
                throw new Exception($"La respuesta de AFIP no es un XML válido: {xmlResponse}");
            }

            // --- DETECCIÓN DE ERRORES INTELIGENTE ---
            var faultString = doc.SelectSingleNode("//faultstring");
            if (faultString != null)
            {
                string msg = faultString.InnerText;

                // 1. Error de Entorno (Testing vs Producción)
                if (msg.Contains("Certificado no emitido por AC de confianza"))
                {
                    throw new Exception("🛑 ERROR DE ENTORNO:\n\nEstás intentando usar un Certificado de TESTING en el entorno de PRODUCCIÓN.\n\nSOLUCIÓN:\n1. Vaya a Configuración AFIP.\n2. DESMARQUE la casilla 'Es Producción'.\n3. Guarde y vuelva a probar.");
                }

                // 2. Error de Vinculación
                if (msg.Contains("Computador no autorizado"))
                    throw new Exception("🛑 ERROR DE PERMISOS:\nEl certificado es válido, pero no autorizaste el servicio 'wsfe' en la web de AFIP (Administrador de Relaciones).");

                // 3. Error de Firma
                if (msg.Contains("CMS no es valido"))
                    throw new Exception("🛑 ERROR CRIPTOGRÁFICO:\nAFIP rechazó la firma del archivo.");

                throw new Exception($"AFIP RECHAZÓ EL PEDIDO:\n{msg}");
            }

            // Extraer Login Ticket si todo salió bien
            var loginTicketNode = doc.SelectSingleNode("//loginCmsReturn");
            if (loginTicketNode == null) throw new Exception("Respuesta inesperada de AFIP (Falta loginCmsReturn).");

            string innerXml = loginTicketNode.InnerText;
            var innerDoc = new XmlDocument();
            innerDoc.LoadXml(innerXml);

            var ticket = new TicketAcceso
            {
                Token = innerDoc.SelectSingleNode("//token")?.InnerText,
                Sign = innerDoc.SelectSingleNode("//sign")?.InnerText
            };

            string expStr = innerDoc.SelectSingleNode("//expirationTime")?.InnerText;
            if (DateTime.TryParse(expStr, out DateTime exp))
            {
                ticket.Expiracion = exp;
            }

            return ticket;
        }
    }
}