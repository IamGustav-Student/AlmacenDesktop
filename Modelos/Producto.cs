using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenDesktop.Modelos
{
    public class Producto
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string CodigoBarras { get; set; }

        public decimal Costo { get; set; }

        public decimal Precio { get; set; }

        public int Stock { get; set; }

        // Margen de ganancia
        public decimal Impuesto { get; set; }

        public int ProveedorId { get; set; }
        public virtual Proveedor Proveedor { get; set; }

        // --- MÉTODOS CON LÓGICA DE NEGOCIO ---

        public void AumentarStock(int cantidad)
        {
            if (cantidad < 0) throw new ArgumentException("La cantidad a aumentar no puede ser negativa.");
            Stock += cantidad;
        }

        public void ReducirStock(int cantidad)
        {
            if (cantidad < 0) throw new ArgumentException("La cantidad a reducir no puede ser negativa.");

            // VALIDACIÓN: No permitir stock negativo
            if (Stock - cantidad < 0)
            {
                throw new InvalidOperationException($"No hay stock suficiente de '{Nombre}'. Stock actual: {Stock}, Solicitado: {cantidad}");
            }

            Stock -= cantidad;
        }
    }
}