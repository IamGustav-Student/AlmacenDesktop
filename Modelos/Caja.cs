using System;
using System.Collections.Generic;

namespace AlmacenDesktop.Modelos
{
    public class Caja
    {
        public int Id { get; set; }

        // Quién abrió la caja
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }

        public decimal SaldoInicial { get; set; } // Cambio con el que se arrancó
        public decimal TotalVentasEfectivo { get; set; } // Se calcula al cerrar
        public decimal TotalVentasOtros { get; set; } // Tarjetas, transferencias
        public decimal SaldoFinalSistema { get; set; } // Inicial + Ventas Efete
        public decimal SaldoFinalReal { get; set; } // Lo que contó el cajero
        public decimal Diferencia { get; set; } // Real - Sistema

        public bool EstaAbierta { get; set; } = true;

        // Relación: Una sesión de caja tiene muchas ventas
        public List<Venta> Ventas { get; set; }
    }
}