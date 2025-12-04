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

        // NUEVO CAMPO: Aquí guardamos el % de ganancia o impuesto
        // Ejemplo: 30 para un 30%
        public decimal Impuesto { get; set; }

        public int ProveedorId { get; set; }
        public virtual Proveedor Proveedor { get; set; }

        // Métodos auxiliares
        public void AumentarStock(int cantidad)
        {
            Stock += cantidad;
        }

        public void ReducirStock(int cantidad)
        {
            Stock -= cantidad;
        }
    }
}