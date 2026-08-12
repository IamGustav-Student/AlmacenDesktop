using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenDesktop.Modelos
{
    public class DetalleCompra
    {
        public int Id { get; set; }

        public int CompraId { get; set; }
        public virtual Compra Compra { get; set; }

        public int ProductoId { get; set; }
        public virtual Producto Producto { get; set; }

        [Range(0.001, 999999, ErrorMessage = "La cantidad comprada debe ser mayor a 0.")]
        public decimal Cantidad { get; set; }

        [Range(0, 99999999)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostoUnitario { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Subtotal { get; set; }
    }
}