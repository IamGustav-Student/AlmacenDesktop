using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    public class Proveedor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } 

        [StringLength(100)]
        public string Contacto { get; set; } 

        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        [StringLength(50)]
        public string Telefono { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        [StringLength(13, MinimumLength = 11, ErrorMessage = "El CUIT debe tener 11 dígitos (con o sin guiones).")]
        public string Cuit { get; set; }

        // Colección para navegación (opcional pero recomendada)
        public virtual ICollection<Producto> Productos { get; set; }
    }
}