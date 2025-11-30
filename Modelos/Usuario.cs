using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    // [POO - Herencia]
    // Usuario HEREDA de Persona (tiene Id, Nombre, Apellido, etc. gratis)
    public class Usuario : Persona
    {
        [Required]
        public string NombreUsuario { get; set; } // Para el login

        [Required]
        public string Password { get; set; } // Guardaremos el hash, no el texto plano

        // [POO - Polimorfismo]
        // Implementación obligatoria del método abstracto del padre
        public override string ObtenerRol()
        {
            return "Operador del Sistema";
        }
    }
}