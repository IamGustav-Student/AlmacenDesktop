using AlmacenDesktop.Modelos;
using System;
using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        public decimal Monto { get; set; }

        // Relación con Cliente (Quién pagó)
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        // Opcional: Notas (ej. "Pago parcial de la semana pasada")
        public string Notas { get; set; }
    }
}