using System;

namespace AlmacenDesktop.Modelos
{
    // Esta clase sirve para mostrar datos en las grillas de cuenta corriente y fiados.
    // Al sacarla de los Forms, evitamos duplicar código.
    public class MovimientoCtaCte
    {
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } // Ejemplo: "COMPRA FIADA", "PAGO A CUENTA"
        public string Descripcion { get; set; } // Ejemplo: "Venta #123"
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal Saldo { get; set; } // Saldo acumulado en ese momento
        public decimal SaldoParcial { get; set; } // A veces lo llamaste así en otro form, unificamos

        // Propiedad opcional para vincular con una venta y ver detalles
        public int? VentaId { get; set; }
    }
}