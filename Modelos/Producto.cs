using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenDesktop.Modelos
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(255, ErrorMessage = "La descripción es demasiado larga.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El código de barras es obligatorio.")]
        [StringLength(50, ErrorMessage = "El código no puede exceder 50 caracteres.")]
        public string CodigoBarras { get; set; }

        [Range(0, 99999999, ErrorMessage = "El costo no puede ser negativo.")]
        public decimal Costo { get; set; }

        [Range(0, 99999999, ErrorMessage = "El precio no puede ser negativo.")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser positivo.")]
        public int StockMinimo { get; set; }

        public decimal Impuesto { get; set; }

        [Required]
        public int ProveedorId { get; set; }
        public virtual Proveedor Proveedor { get; set; }

        // --- MÉTODOS DE NEGOCIO ---

        public void AumentarStock(int cantidad)
        {
            if (cantidad < 0) throw new ArgumentException("La cantidad debe ser positiva.");
            Stock += cantidad;
        }

        public void ReducirStock(int cantidad)
        {
            if (cantidad < 0) throw new ArgumentException("La cantidad debe ser positiva.");
            if (Stock - cantidad < 0) throw new InvalidOperationException($"Stock insuficiente. Stock actual: {Stock}");
            Stock -= cantidad;
        }
    }
}