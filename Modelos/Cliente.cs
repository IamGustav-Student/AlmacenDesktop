using AlmacenDesktop.Modelos;
using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    // [POO - Herencia]
    // Cliente también HEREDA de Persona
    public class Cliente : Persona
    {
        [Required]
        [MaxLength(20)]
        public string DniCuit { get; set; }

        public string Direccion { get; set; }

        // [POO - Polimorfismo]
        public override string ObtenerRol()
        {
            return "Cliente Final";
        }
    }
}