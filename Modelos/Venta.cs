using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenDesktop.Modelos
{
    public class Venta
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        [Required]
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        // Totales
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        public string MetodoPago { get; set; } // Efectivo, Tarjeta, QR

        // Relación con Caja
        public int CajaId { get; set; }
        // public virtual Caja Caja { get; set; } // (Opcional si quieres navegación)

        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

        // --- CAMPOS FISCALES (NIVEL DIOS - AFIP) ---

        [StringLength(20)]
        public string TipoComprobante { get; set; } = "X"; // "FA" (Factura A), "FB", "FC", "X" (Presupuesto)

        public int PuntoVenta { get; set; } = 0; // Ej: 1, 2...

        public long NumeroFactura { get; set; } = 0; // El número correlativo que da AFIP

        [StringLength(50)]
        public string CAE { get; set; } // Código de Autorización Electrónico

        public DateTime? CAEVencimiento { get; set; } // Cuándo vence el CAE

        [StringLength(255)]
        public string ObservacionesAFIP { get; set; } // Si AFIP observa algo ("DNI inválido", etc.)
    }
}