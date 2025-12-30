using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenDesktop.Modelos
{
    public class Venta
    {
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Range(0, 99999999, ErrorMessage = "El total no puede ser negativo.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        [Required]
        public string MetodoPago { get; set; }

        [Required]
        public int CajaId { get; set; }
        // public virtual Caja Caja { get; set; } // Opcional

        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

        // --- CAMPOS AFIP (Esenciales para VENDEMAX) ---
        [StringLength(20)]
        public string TipoComprobante { get; set; } = "X";

        public int PuntoVenta { get; set; } = 0;
        public long NumeroFactura { get; set; } = 0;

        [StringLength(50)]
        public string CAE { get; set; }
        public DateTime? CAEVencimiento { get; set; }

        [StringLength(255)]
        public string ObservacionesAFIP { get; set; }
    }
}