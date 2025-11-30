using System;
using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    // [POO - Abstracción]
    // Definimos la clase como 'abstract' porque no queremos crear objetos "Persona" genéricos,
    // solo queremos crear "Usuarios" o "Clientes".
    public abstract class Persona
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Telefono { get; set; }

        // [POO - Polimorfismo]
        // Declaramos un método abstracto. Esto OBLIGA a las clases hijas (Usuario, Cliente)
        // a implementar su propia versión de este método.
        public abstract string ObtenerRol();

        // Método virtual: Las hijas PUEDEN sobrescribirlo si quieren, pero ya tiene una lógica base.
        public virtual string NombreCompleto
        {
            get { return $"{Apellido}, {Nombre}"; }
        }
    }
}