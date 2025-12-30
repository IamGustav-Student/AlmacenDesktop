using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenDesktop.Modelos
{
    public class Compra
    {
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        public int ProveedorId { get; set; }
        public virtual Proveedor Proveedor { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Range(0, 99999999)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        [StringLength(200)]
        public string Observaciones { get; set; }

        public List<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>();
    }
}