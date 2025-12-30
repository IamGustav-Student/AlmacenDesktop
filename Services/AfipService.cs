using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AlmacenDesktop.Services
{
    public class AfipService
    {
        private const string URL_WSAA_HOMO = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms";
        private const string URL_WSFE_HOMO = "https://wswhomo.afip.gov.ar/wsfev1/service.asmx";
        private const string URL_WSAA_PROD = "https://wsaa.afip.gov.ar/ws/services/LoginCms";
        private const string URL_WSFE_PROD = "https://servicios1.afip.gov.ar/wsfev1/service.asmx";

        private ConfiguracionAfip _config;

        // Constructor 1: Lee de la Base de Datos (Uso normal en Ventas)
        public AfipService()
        {
            using (var db = new AlmacenDbContext())
            {
                // CORRECCIÓN: Intentamos leer, pero si no hay, no lanzamos excepción aquí.
                _config = db.ConfiguracionesAfip.FirstOrDefault();
            }
        }

        // Constructor 2: Recibe configuración manual (Uso en botón "Probar Conexión")
        public AfipService(ConfiguracionAfip configManual)
        {
            _config = configManual;
        }

        // 1. AUTENTICACIÓN
        public async Task AutenticarAsync()
        {
            // La validación se mueve aquí, donde realmente hace falta
            if (_config == null) throw new Exception("No se encontró configuración de AFIP en la base de datos. Vaya a Configuración > AFIP.");

            bool esTestManual = _config.Id == 0;

            if (!esTestManual && !string.IsNullOrEmpty(_config.Token) && _config.ExpiracionToken > DateTime.Now)
            {
                return;
            }

            if (!File.Exists(_config.CertificadoPath))
                throw new Exception($"El archivo de certificado no existe en la ruta:\n{_config.CertificadoPath}");

            string cmsFirmado = AfipAuthService.GenerarLoginTicketRequest(_config.CertificadoPath, _config.CertificadoPassword);

            string urlWsaa = _config.EsProduccion ? URL_WSAA_PROD : URL_WSAA_HOMO;
            string soapRequest = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://wsaa.view.sua.dvadac.desein.afip.gov"">
   <soapenv:Header/>
   <soapenv:Body>
      <ser:loginCms>
         <in0>{cmsFirmado}</in0>
      </ser:loginCms>
   </soapenv:Body>
</soapenv:Envelope>";

            string respuestaXml = await EnviarSoapAsync(urlWsaa, soapRequest, "loginCms");

            var doc = XDocument.Parse(respuestaXml);
            var loginCmsReturn = doc.Descendants().FirstOrDefault(n => n.Name.LocalName == "loginCmsReturn")?.Value;

            if (string.IsNullOrEmpty(loginCmsReturn))
            {
                var fault = doc.Descendants().FirstOrDefault(n => n.Name.LocalName == "faultstring")?.Value;
                throw new Exception($"Error AFIP WSAA: {fault ?? "Respuesta vacía o desconocida."}");
            }

            var docAuth = XDocument.Parse(loginCmsReturn);
            string token = docAuth.Descendants("token").First().Value;
            string sign = docAuth.Descendants("sign").First().Value;
            string expirationTime = docAuth.Descendants("expirationTime").First().Value;

            _config.Token = token;
            _config.Sign = sign;
            _config.ExpiracionToken = DateTime.Parse(expirationTime);

            if (!esTestManual)
            {
                using (var db = new AlmacenDbContext())
                {
                    var configDb = db.ConfiguracionesAfip.First();
                    configDb.Token = token;
                    configDb.Sign = sign;
                    configDb.ExpiracionToken = _config.ExpiracionToken;
                    db.SaveChanges();
                }
            }
        }

        public async Task<int> ObtenerUltimoComprobanteAsync(int puntoVenta, int tipoComprobante)
        {
            await AutenticarAsync();

            string urlWsfe = _config.EsProduccion ? URL_WSFE_PROD : URL_WSFE_HOMO;

            string request = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ar=""http://ar.gov.afip.dif.FEV1/"">
   <soapenv:Header/>
   <soapenv:Body>
      <ar:FECompUltimoAutorizado>
         <ar:Auth>
            <ar:Token>{_config.Token}</ar:Token>
            <ar:Sign>{_config.Sign}</ar:Sign>
            <ar:Cuit>{_config.CuitEmisor}</ar:Cuit>
         </ar:Auth>
         <ar:PtoVta>{puntoVenta}</ar:PtoVta>
         <ar:CbteTipo>{tipoComprobante}</ar:CbteTipo>
      </ar:FECompUltimoAutorizado>
   </soapenv:Body>
</soapenv:Envelope>";

            string xmlResp = await EnviarSoapAsync(urlWsfe, request, "http://ar.gov.afip.dif.FEV1/FECompUltimoAutorizado");
            var doc = XDocument.Parse(xmlResp);

            var errores = doc.Descendants().Where(n => n.Name.LocalName == "Err");
            if (errores.Any())
            {
                string msg = errores.First().Descendants().First(n => n.Name.LocalName == "Msg").Value;
                throw new Exception($"Error AFIP WSFE: {msg}");
            }

            string ultimo = doc.Descendants().FirstOrDefault(n => n.Name.LocalName == "CbteNro")?.Value;
            return string.IsNullOrEmpty(ultimo) ? 0 : int.Parse(ultimo);
        }

        public async Task<ResultadoAfip> FacturarVentaAsync(Venta venta)
        {
            if (_config == null) throw new Exception("Configure AFIP primero.");

            int tipoCbte = 11; // Factura C (Monotributo)
            int ptoVta = _config.PuntoVenta;

            int ultimoNro = await ObtenerUltimoComprobanteAsync(ptoVta, tipoCbte);
            int nuevoNro = ultimoNro + 1;

            string fecha = DateTime.Now.ToString("yyyyMMdd");
            double total = (double)venta.Total;
            long docNro = 0;
            int docTipo = 99;

            if (venta.Cliente != null && venta.Cliente.DniCuit != "-" && venta.Cliente.DniCuit.Length > 6)
            {
                string limpio = new string(venta.Cliente.DniCuit.Where(char.IsDigit).ToArray());
                if (long.TryParse(limpio, out docNro))
                {
                    if (docNro > 20000000000) docTipo = 80; // CUIT
                    else docTipo = 96; // DNI
                }
            }

            string request = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ar=""http://ar.gov.afip.dif.FEV1/"">
   <soapenv:Header/>
   <soapenv:Body>
      <ar:FECAESolicitar>
         <ar:Auth>
            <ar:Token>{_config.Token}</ar:Token>
            <ar:Sign>{_config.Sign}</ar:Sign>
            <ar:Cuit>{_config.CuitEmisor}</ar:Cuit>
         </ar:Auth>
         <ar:FeCAEReq>
            <ar:FeCabReq>
               <ar:CantReg>1</ar:CantReg>
               <ar:PtoVta>{ptoVta}</ar:PtoVta>
               <ar:CbteTipo>{tipoCbte}</ar:CbteTipo>
            </ar:FeCabReq>
            <ar:FeDetReq>
               <ar:FECAEDetRequest>
                  <ar:Concepto>1</ar:Concepto>
                  <ar:DocTipo>{docTipo}</ar:DocTipo>
                  <ar:DocNro>{docNro}</ar:DocNro>
                  <ar:CbteDesde>{nuevoNro}</ar:CbteDesde>
                  <ar:CbteHasta>{nuevoNro}</ar:CbteHasta>
                  <ar:CbteFch>{fecha}</ar:CbteFch>
                  <ar:ImpTotal>{total.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}</ar:ImpTotal>
                  <ar:ImpTotConc>0</ar:ImpTotConc>
                  <ar:ImpNeto>{total.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}</ar:ImpNeto>
                  <ar:ImpOpEx>0</ar:ImpOpEx>
                  <ar:ImpTrib>0</ar:ImpTrib>
                  <ar:ImpIVA>0</ar:ImpIVA>
                  <ar:MonId>PES</ar:MonId>
                  <ar:MonCotiz>1</ar:MonCotiz>
               </ar:FECAEDetRequest>
            </ar:FeDetReq>
         </ar:FeCAEReq>
      </ar:FECAESolicitar>
   </soapenv:Body>
</soapenv:Envelope>";

            string urlWsfe = _config.EsProduccion ? URL_WSFE_PROD : URL_WSFE_HOMO;
            string xmlResp = await EnviarSoapAsync(urlWsfe, request, "http://ar.gov.afip.dif.FEV1/FECAESolicitar");
            var doc = XDocument.Parse(xmlResp);

            var errores = doc.Descendants().Where(n => n.Name.LocalName == "Err");
            if (errores.Any())
            {
                string msg = errores.First().Descendants().First(n => n.Name.LocalName == "Msg").Value;
                return new ResultadoAfip { Exito = false, Error = msg };
            }

            string resultado = doc.Descendants().FirstOrDefault(n => n.Name.LocalName == "Resultado")?.Value;

            if (resultado == "A")
            {
                string cae = doc.Descendants().FirstOrDefault(n => n.Name.LocalName == "CAE")?.Value;
                string vto = doc.Descendants().FirstOrDefault(n => n.Name.LocalName == "CAEFchVto")?.Value;
                return new ResultadoAfip
                {
                    Exito = true,
                    CAE = cae,
                    Vencimiento = DateTime.ParseExact(vto, "yyyyMMdd", null),
                    NumeroComprobante = nuevoNro
                };
            }
            else
            {
                var obs = doc.Descendants().Where(n => n.Name.LocalName == "Obs");
                string msgObs = obs.Any() ? obs.First().Descendants().First(n => n.Name.LocalName == "Msg").Value : "Rechazado";
                return new ResultadoAfip { Exito = false, Error = "Rechazado: " + msgObs };
            }
        }

        private async Task<string> EnviarSoapAsync(string url, string xmlBody, string soapAction)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                using (var content = new StringContent(xmlBody, Encoding.UTF8, "text/xml"))
                {
                    if (!string.IsNullOrEmpty(soapAction)) content.Headers.Add("SOAPAction", soapAction);

                    try
                    {
                        var response = await client.PostAsync(url, content);
                        return await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error de red al conectar con AFIP ({url}): {ex.Message}");
                    }
                }
            }
        }
    }

    public class ResultadoAfip
    {
        public bool Exito { get; set; }
        public string CAE { get; set; }
        public long NumeroComprobante { get; set; }
        public DateTime Vencimiento { get; set; }
        public string Error { get; set; }
    }
}