using AlmacenDesktop.Modelos;
using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }

        public int VentaId { get; set; }
        public virtual Venta Venta { get; set; }

        public int ProductoId { get; set; }
        public virtual Producto Producto { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Propiedad calculada (no se guarda en DB, se calcula al vuelo)
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}