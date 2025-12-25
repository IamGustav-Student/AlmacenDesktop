using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    // Singleton en Base de Datos (Solo habrá 1 registro)
    public class ConfiguracionAfip
    {
        public int Id { get; set; }

        [Required]
        public long CuitEmisor { get; set; } // El CUIT del dueño del negocio

        [Required]
        public int PuntoVenta { get; set; } = 1;

        [Required]
        public string CertificadoPath { get; set; } // Ruta al archivo .p12

        public string CertificadoPassword { get; set; } // Contraseña del .p12 (si tiene)

        public bool EsProduccion { get; set; } = false; // False = Testing (Homologación), True = Real

        // Tokens temporales (Para no pedir login a AFIP en cada venta, duran 12hs)
        public string Token { get; set; }
        public string Sign { get; set; }
        public System.DateTime? ExpiracionToken { get; set; }
    }
}