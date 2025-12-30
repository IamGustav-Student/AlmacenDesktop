using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    // [POO - Herencia]
    // Usuario HEREDA de Persona (tiene Id, Nombre, Apellido, etc. gratis)
    public class Usuario : Persona
    {
      public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres.")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "La contraseña debe tener al menos 4 caracteres.")]
        public string Password { get; set; }

        public bool Activo { get; set; } = true;

        // [POO - Polimorfismo]
        // Implementación obligatoria del método abstracto del padre
        public override string ObtenerRol()
        {
            return "Operador del Sistema";
        }
    }
}