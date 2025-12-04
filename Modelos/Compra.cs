using System;
using System.Collections.Generic;

namespace AlmacenDesktop.Modelos
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        public int ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; }

        public int UsuarioId { get; set; } // Quién recibió la mercadería
        public Usuario Usuario { get; set; }

        public List<DetalleCompra> Detalles { get; set; }
    }
}