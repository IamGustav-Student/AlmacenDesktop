using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenDesktop.Modelos
{
    public class DetalleVenta
    {
        public int Id { get; set; }

        public int VentaId { get; set; }
        public virtual Venta Venta { get; set; }

        public int ProductoId { get; set; }
        public virtual Producto Producto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Range(0, 99999999)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Subtotal { get; set; }

        [NotMapped]
        public decimal SubtotalCalculado => Cantidad * PrecioUnitario;
    }
}