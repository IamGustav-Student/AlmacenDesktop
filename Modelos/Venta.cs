using System;
using System.Collections.Generic;

namespace AlmacenDesktop.Modelos
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } // Efectivo, Tarjeta, etc.

        // Relaciones
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        // --- NUEVO FASE 3: VINCULO CON CAJA ---
        public int? CajaId { get; set; } // Puede ser nulo por compatibilidad con ventas viejas
        public Caja Caja { get; set; }

        public List<DetalleVenta> Detalles { get; set; }
    }
}