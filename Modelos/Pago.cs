using AlmacenDesktop.Modelos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenDesktop.Modelos
{
    public class Pago
    {
        public int Id { get; set; }
        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        [Range(0.01, 99999999, ErrorMessage = "El monto del pago debe ser positivo.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Monto { get; set; }

        [StringLength(200)]
        public string Observaciones { get; set; }

        public int UsuarioId { get; set; } // Qué cajero recibió la plata
        public Usuario Usuario { get; set; }
    }
}


