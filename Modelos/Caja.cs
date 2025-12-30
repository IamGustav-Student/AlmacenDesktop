using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenDesktop.Modelos
{
    public class Caja
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        public DateTime FechaApertura { get; set; } = DateTime.Now;

        public DateTime? FechaCierre { get; set; }

        [Range(0, 99999999, ErrorMessage = "El saldo inicial no puede ser negativo.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SaldoInicial { get; set; }

        [Range(0, 99999999)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalVentasEfectivo { get; set; }

        [Range(0, 99999999)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalVentasOtros { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SaldoFinalSistema { get; set; }

        [Range(0, 99999999, ErrorMessage = "El saldo real no puede ser negativo.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SaldoFinalReal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Diferencia { get; set; }

        public bool EstaAbierta { get; set; } = true;

        public List<Venta> Ventas { get; set; }

        // --- VALIDACIÓN LÓGICA ---
        public bool EsFechaValida()
        {
            if (FechaCierre.HasValue)
            {
                return FechaCierre.Value > FechaApertura;
            }
            return true;
        }
    }
}