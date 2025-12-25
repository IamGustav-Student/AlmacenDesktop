using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    public class Proveedor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre es muy largo.")]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string Contacto { get; set; }

        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        [StringLength(50)]
        public string Telefono { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        [StringLength(20)]
        // Aquí podríamos agregar una validación personalizada de CUIT más adelante
        public string Cuit { get; set; }

        // Relación inversa (opcional pero recomendada para navegación)
        public virtual ICollection<Producto> Productos { get; set; }
    }
}