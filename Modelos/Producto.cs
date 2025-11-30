using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string CodigoBarras { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public int Stock { get; set; }

        // Método de utilidad para el negocio
        public void ReducirStock(int cantidad)
        {
            if (Stock >= cantidad)
            {
                Stock -= cantidad;
            }
            else
            {
                // [Manejo de Excepciones] - Requisito del examen
                throw new InvalidOperationException("No hay suficiente stock para realizar esta operación.");
            }
        }
    }
}