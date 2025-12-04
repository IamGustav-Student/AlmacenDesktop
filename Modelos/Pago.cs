using System;

namespace AlmacenDesktop.Modelos
{
    public class Pago
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }

        // Relaciones
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public int UsuarioId { get; set; } // Qué cajero recibió la plata
        public Usuario Usuario { get; set; }
    }
}