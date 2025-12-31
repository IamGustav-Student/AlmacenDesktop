using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    public class DatosNegocio
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string NombreFantasia { get; set; }

        [StringLength(100)]
        public string RazonSocial { get; set; }

        [StringLength(13)] // CUIT 11 digitos + guiones opcionales
        public string CUIT { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        [StringLength(50)]
        public string Telefono { get; set; }

        [StringLength(255)]
        public string MensajeTicket { get; set; }

        // --- NUEVO CAMPO: IMPRESORA SELECCIONADA ---
        [StringLength(100)]
        public string NombreImpresora { get; set; } // Ej: "Epson TM-T20II"
    }
}