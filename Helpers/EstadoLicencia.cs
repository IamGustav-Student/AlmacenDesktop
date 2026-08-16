using System;

namespace AlmacenDesktop.Helpers
{
    /// <summary>
    /// Escalera de estados de la suscripción. Antes esto era un bool: funcionaba o
    /// no funcionaba. El problema de cortar de golpe en un POS es que el comercio
    /// se entera en el mostrador, con un cliente esperando, y encima no puede ni
    /// cerrar la caja del día ni sacar sus propios datos.
    ///
    /// Lo que realmente fuerza el pago es no poder VENDER, así que la restricción
    /// apunta ahí y deja disponible el resto.
    /// </summary>
    public enum EstadoLicencia
    {
        /// <summary>Todo normal.</summary>
        AlDia = 0,

        /// <summary>Vence pronto. Solo avisa, no restringe nada.</summary>
        PorVencer = 1,

        /// <summary>
        /// Venció hace poco. Sigue funcionando completo a propósito: un pago con
        /// MercadoPago puede tardar en acreditarse y cortar por 48 h de demora es
        /// garantía de cliente perdido.
        /// </summary>
        Gracia = 2,

        /// <summary>
        /// Venció y se agotó la gracia. Puede cerrar caja, consultar historial y
        /// exportar sus datos, pero no registrar ventas ni compras nuevas.
        /// </summary>
        Restringido = 3,

        /// <summary>
        /// Bloqueo total: suspensión/cancelación manual (contracargo, fraude),
        /// equipo distinto, gracia offline agotada o reloj manipulado. Sin período
        /// de gracia — no se lo ganó.
        /// </summary>
        Bloqueado = 4,
    }

    public class ResultadoLicencia
    {
        public EstadoLicencia Estado { get; set; }
        public string Mensaje { get; set; } = string.Empty;

        /// <summary>Negativo si ya venció.</summary>
        public int DiasRestantes { get; set; }

        /// <summary>Puede registrar ventas y compras nuevas.</summary>
        public bool PuedeOperar => Estado != EstadoLicencia.Restringido && Estado != EstadoLicencia.Bloqueado;

        /// <summary>Puede entrar al sistema (aunque sea en modo restringido).</summary>
        public bool PuedeEntrar => Estado != EstadoLicencia.Bloqueado;

        /// <summary>Amerita mostrarle algo al usuario, sin frenarlo.</summary>
        public bool AmeritaAviso => Estado == EstadoLicencia.PorVencer || Estado == EstadoLicencia.Gracia;
    }

    public static class LicenciaConfig
    {
        /// <summary>Días antes del vencimiento en que se empieza a avisar.</summary>
        public const int DiasAvisoPrevio = 7;

        /// <summary>Días después del vencimiento en que sigue funcionando completo.</summary>
        public const int DiasGracia = 7;

        /// <summary>Días que puede estar sin validar online antes de bloquear.</summary>
        public const int DiasGraciaOffline = 7;

        /// <summary>Cada cuánto revalida mientras la app está abierta.</summary>
        public static readonly TimeSpan IntervaloRevalidacion = TimeSpan.FromHours(6);
    }
}
