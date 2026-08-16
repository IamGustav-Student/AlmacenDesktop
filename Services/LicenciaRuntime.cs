using AlmacenDesktop.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmacenDesktop.Services
{
    /// <summary>
    /// Estado vigente de la suscripción durante la sesión, y su revalidación
    /// periódica.
    ///
    /// Antes esto no existía: se validaba una sola vez al arrancar y el resultado
    /// de la revalidación online se descartaba. En un POS de mostrador, que queda
    /// abierto días enteros, eso significaba que un vencimiento o una suspensión
    /// no tenían ningún efecto hasta que alguien reiniciara la aplicación.
    /// </summary>
    public static class LicenciaRuntime
    {
        private static System.Windows.Forms.Timer? _timer;

        public static ResultadoLicencia Estado { get; private set; } = new ResultadoLicencia
        {
            Estado = EstadoLicencia.AlDia,
            Mensaje = "Sin evaluar",
        };

        /// <summary>Se dispara cuando el estado cambia (para refrescar avisos en pantalla).</summary>
        public static event Action<ResultadoLicencia>? EstadoCambiado;

        public static bool PuedeOperar => Estado.PuedeOperar;

        public static void Refrescar()
        {
            var nuevo = LicenseHelper.EvaluarLicenciaLocal();
            bool cambio = nuevo.Estado != Estado.Estado;
            Estado = nuevo;
            if (cambio)
            {
                try { EstadoCambiado?.Invoke(nuevo); } catch { }
            }
        }

        /// <summary>
        /// Arranca la revalidación periódica. Se llama una sola vez, después del
        /// login. El timer es de WinForms a propósito: dispara en el hilo de UI,
        /// así se puede mostrar el bloqueo sin marshalling manual.
        /// </summary>
        public static void IniciarMonitoreo()
        {
            Refrescar();
            if (_timer != null) return;

            _timer = new System.Windows.Forms.Timer
            {
                Interval = (int)LicenciaConfig.IntervaloRevalidacion.TotalMilliseconds,
            };
            _timer.Tick += async (s, e) => await RevalidarAsync();
            _timer.Start();
        }

        public static void DetenerMonitoreo()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }

        /// <summary>
        /// Consulta al servidor y actualiza el estado. Nunca tira excepción: si no
        /// hay internet simplemente se mantiene el estado local y, si pasa mucho
        /// tiempo, la gracia offline termina bloqueando.
        /// </summary>
        public static async Task RevalidarAsync()
        {
            try
            {
                var licencia = LicenseHelper.LeerLicenciaLocal();
                if (licencia == null) return;

                var servicio = new LicenseService();
                var r = await servicio.ValidarOnlineDetalladoAsync(licencia.Email, licencia.Clave);

                // Un rechazo del servidor se persiste para que el bloqueo sobreviva
                // al reinicio. Una falla de red NO: el comercio pagó y puede estar
                // sin internet, para eso está la gracia offline.
                if (!r.Valido && r.RechazoDefinitivo)
                {
                    LicenseHelper.MarcarRechazadaPorServidor(r.EstadoServidor);
                }

                Refrescar();
                AplicarBloqueoSiCorresponde();
            }
            catch
            {
                // Nunca interrumpe la operación del comercio.
            }
        }

        /// <summary>
        /// Si el estado pasó a bloqueado con la app abierta, muestra la pantalla de
        /// bloqueo. Si el usuario no regulariza, se cierra la aplicación.
        /// </summary>
        private static void AplicarBloqueoSiCorresponde()
        {
            if (Estado.Estado != EstadoLicencia.Bloqueado) return;

            try
            {
                using (var lockForm = new Forms.LockForm(Estado.Mensaje))
                {
                    if (lockForm.ShowDialog() == DialogResult.OK)
                    {
                        Refrescar();
                        if (Estado.PuedeEntrar) return;
                    }
                }
                Application.Exit();
            }
            catch
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Barrera para las acciones que sí se cortan cuando no pagó: registrar
        /// ventas y compras. Devuelve false y explica si no puede operar.
        /// </summary>
        public static bool ExigirOperacionHabilitada(IWin32Window? duenio = null)
        {
            Refrescar();
            if (Estado.PuedeOperar) return true;

            AudioHelper.PlayError();
            var texto = Estado.Mensaje + "\n\n¿Desea abrir la página de pago ahora?";
            var r = MessageBox.Show(texto, "Suscripción vencida", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = Constantes.URL_CHECKOUT,
                        UseShellExecute = true,
                    });
                }
                catch { }
            }
            return false;
        }
    }
}
