using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace AlmacenDesktop.Services
{
    // SERVICIO NIVEL DIOS: MANEJO CRIPTOGRÁFICO DE AFIP (WSAA)
    public class AfipAuthService
    {
        // Genera el XML firmado (CMS) para solicitar acceso al Web Service de Factura Electrónica (wsfe)
        public static string GenerarLoginTicketRequest(string certificadoPath, string password, string servicio = "wsfe")
        {
            // 1. Generar el XML de Solicitud (TRA)
            string traXml = GenerarTRA(servicio);

            // 2. Leer el Certificado .p12
            X509Certificate2 certificado;
            try
            {
                // X509KeyStorageFlags.Exportable es vital para poder usar la clave privada
                certificado = new X509Certificate2(certificadoPath, password, X509KeyStorageFlags.Exportable);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al leer el certificado: {ex.Message}. Verifique ruta y contraseña.");
            }

            // 3. Firmar el XML (CMS - Cryptographic Message Syntax)
            return FirmarMensaje(traXml, certificado);
        }

        private static string GenerarTRA(string servicio)
        {
            var uniqueId = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var generationTime = DateTime.Now.AddMinutes(-10).ToString("s"); // -10 min por clock skew
            var expirationTime = DateTime.Now.AddMinutes(+10).ToString("s"); // +10 min de validez para el pedido

            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                         "<loginTicketRequest version=\"1.0\">" +
                            "<header>" +
                                $"<uniqueId>{uniqueId}</uniqueId>" +
                                $"<generationTime>{generationTime}</generationTime>" +
                                $"<expirationTime>{expirationTime}</expirationTime>" +
                            "</header>" +
                            $"<service>{servicio}</service>" +
                         "</loginTicketRequest>";
            return xml;
        }

        private static string FirmarMensaje(string mensajeXml, X509Certificate2 certificado)
        {
            // Encoding UTF8
            byte[] msgBytes = Encoding.UTF8.GetBytes(mensajeXml);

            // Objeto ContentInfo
            ContentInfo contentInfo = new ContentInfo(msgBytes);

            // Objeto SignedCms
            SignedCms signedCms = new SignedCms(contentInfo);

            // CmsSigner con el certificado
            CmsSigner signer = new CmsSigner(certificado);
            signer.IncludeOption = X509IncludeOption.EndCertOnly; // Solo incluir el certificado final

            // Computar la firma
            signedCms.ComputeSignature(signer);

            // Codificar a Base64
            byte[] encoded = signedCms.Encode();
            return Convert.ToBase64String(encoded);
        }
    }
}