using AlmacenDesktop.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public decimal Total { get; set; }

        // --- NUEVO CAMPO ---
        [Required]
        [MaxLength(50)]
        public string MetodoPago { get; set; } // "Efectivo", "Transferencia", "Fiado"

        // Relación con Cliente
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        // Relación con Usuario (Vendedor)
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        public virtual List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}