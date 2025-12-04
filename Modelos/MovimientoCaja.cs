using System;

namespace AlmacenDesktop.Modelos
{
    public class MovimientoCaja
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } // "INGRESO" (Entrada) o "EGRESO" (Salida/Gasto)
        public decimal Monto { get; set; }
        public string Descripcion { get; set; } // Ej: "Pago Proveedor Coca Cola"

        // Relaciones
        public int CajaId { get; set; }
        public Caja Caja { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}