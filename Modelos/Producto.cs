using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenDesktop.Modelos
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(255, ErrorMessage = "La descripción es demasiado larga.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El código de barras es obligatorio.")]
        [StringLength(50, ErrorMessage = "El código de barras es muy largo.")]
        public string CodigoBarras { get; set; }

        [Range(0, 99999999, ErrorMessage = "El costo no puede ser negativo.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Costo { get; set; }

        [Range(0, 99999999, ErrorMessage = "El precio no puede ser negativo.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser positivo.")]
        public int StockMinimo { get; set; }

        [Range(0, 100, ErrorMessage = "El impuesto debe ser un porcentaje entre 0 y 100.")]
        public decimal Impuesto { get; set; }

        // Relación obligatoria: Todo producto debe tener un proveedor (aunque sea el Genérico)
        [Required(ErrorMessage = "Debe asignar un proveedor.")]
        public int ProveedorId { get; set; }
        public virtual Proveedor Proveedor { get; set; }

        // --- VALIDACIONES DE NEGOCIO ---

        public bool EsRentable()
        {
            return Precio >= Costo;
        }

        public void AumentarStock(int cantidad)
        {
            if (cantidad < 0) throw new ArgumentException("La cantidad a aumentar debe ser positiva.");
            Stock += cantidad;
        }

        public void ReducirStock(int cantidad)
        {
            if (cantidad < 0) throw new ArgumentException("La cantidad a reducir debe ser positiva.");
            if (Stock - cantidad < 0) throw new InvalidOperationException($"Stock insuficiente para {Nombre}. Stock actual: {Stock}");
            Stock -= cantidad;
        }
    }
}