using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AlmacenDesktop.Helpers
{
    public class LicenciaInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public DateTime FechaUltimaValidacionOnline { get; set; }
        public DateTime FechaVencimientoLocal { get; set; }
        public string Fingerprint { get; set; } = string.Empty;

        /// <summary>
        /// Último estado que informó el servidor (ACTIVO / SUSPENDIDO / CANCELADO).
        /// Permite distinguir "se venció" (merece período de gracia) de "lo
        /// suspendieron a mano" por contracargo o fraude (no lo merece). Vacío en
        /// licencias guardadas por versiones anteriores: se asume ACTIVO.
        /// </summary>
        public string EstadoServidor { get; set; } = string.Empty;

        /// <summary>
        /// Marca que el servidor rechazó la licencia explícitamente (403), a
        /// diferencia de no haber podido consultarlo (sin internet). Se persiste
        /// para que el bloqueo sobreviva a un reinicio de la app.
        /// </summary>
        public bool RechazadaPorServidor { get; set; }
    }

    public static class LicenseHelper
    {
        private static readonly string LicenciaFileName = "licencia.dat";
        private static readonly string PathLicencia = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LicenciaFileName);

        // Secreto criptográfico interno para hashing de redundancia local (HMAC)
        private static readonly string SaltLocal = "hexastrategy_pos_local_salt_redundancy";

        /// <summary>
        /// Guarda el token de licencia de forma segura en disco utilizando Windows DPAPI.
        /// </summary>
        public static bool GuardarLicenciaLocal(string email, string clave, DateTime vencimiento, string estadoServidor = "ACTIVO")
        {
            try
            {
                var info = new LicenciaInfo
                {
                    Email = email.Trim().ToLower(),
                    Clave = clave.Trim().ToUpper(),
                    FechaUltimaValidacionOnline = DateTime.Now,
                    FechaVencimientoLocal = vencimiento,
                    Fingerprint = HardwareHelper.ObtenerMachineFingerprint(),
                    EstadoServidor = string.IsNullOrWhiteSpace(estadoServidor) ? "ACTIVO" : estadoServidor.ToUpperInvariant(),
                    RechazadaPorServidor = false
                };

                string json = JsonSerializer.Serialize(info);
                byte[] rawData = Encoding.UTF8.GetBytes(json);

                // Cifrado DPAPI a nivel de usuario de Windows
                byte[] entropy = Encoding.UTF8.GetBytes(SaltLocal);
                byte[] encryptedData = ProtectedData.Protect(rawData, entropy, DataProtectionScope.CurrentUser);

                File.WriteAllBytes(PathLicencia, encryptedData);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar la licencia local: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Lee y descifra los datos de la licencia local.
        /// </summary>
        public static LicenciaInfo? LeerLicenciaLocal()
        {
            if (!File.Exists(PathLicencia))
            {
                return null;
            }

            try
            {
                byte[] encryptedData = File.ReadAllBytes(PathLicencia);
                byte[] entropy = Encoding.UTF8.GetBytes(SaltLocal);
                
                byte[] decryptedData = ProtectedData.Unprotect(encryptedData, entropy, DataProtectionScope.CurrentUser);
                string json = Encoding.UTF8.GetString(decryptedData);

                return JsonSerializer.Deserialize<LicenciaInfo>(json);
            }
            catch
            {
                // Si el archivo está corrupto o se cambió de usuario de Windows, se elimina para forzar reactivación
                EliminarLicenciaLocal();
                return null;
            }
        }

        /// <summary>
        /// Elimina el archivo de licencia local.
        /// </summary>
        public static void EliminarLicenciaLocal()
        {
            try
            {
                if (File.Exists(PathLicencia))
                {
                    File.Delete(PathLicencia);
                }
            }
            catch
            {
                // Ignorar
            }
        }

        /// <summary>
        /// Marca la licencia como rechazada por el servidor, para que el bloqueo
        /// no se pierda al reiniciar la app.
        /// </summary>
        public static void MarcarRechazadaPorServidor(string estadoServidor)
        {
            try
            {
                var licencia = LeerLicenciaLocal();
                if (licencia == null) return;

                licencia.RechazadaPorServidor = true;
                licencia.EstadoServidor = string.IsNullOrWhiteSpace(estadoServidor) ? "SUSPENDIDO" : estadoServidor.ToUpperInvariant();

                string json = JsonSerializer.Serialize(licencia);
                byte[] entropy = Encoding.UTF8.GetBytes(SaltLocal);
                byte[] encrypted = ProtectedData.Protect(Encoding.UTF8.GetBytes(json), entropy, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(PathLicencia, encrypted);
            }
            catch
            {
                // Si no se puede escribir, el bloqueo igual aplica en esta sesión.
            }
        }

        /// <summary>
        /// Evalúa el estado de la suscripción según la licencia guardada localmente.
        /// Ver <see cref="EstadoLicencia"/> para el criterio de cada nivel.
        /// </summary>
        public static ResultadoLicencia EvaluarLicenciaLocal()
        {
            var licencia = LeerLicenciaLocal();

            if (licencia == null)
            {
                return Bloqueo("El sistema no se encuentra activado. Ingrese su clave de activación.");
            }

            // 1. Vinculación de hardware — bloqueo duro, sin gracia.
            if (licencia.Fingerprint != HardwareHelper.ObtenerMachineFingerprint())
            {
                return Bloqueo("La clave de activación no coincide con el hardware de esta computadora.");
            }

            // 2. Reloj atrasado respecto de la última validación: manipulación.
            if (DateTime.Now < licencia.FechaUltimaValidacionOnline)
            {
                return Bloqueo("Se detectó una alteración en la hora del sistema. Corrija el reloj de Windows.");
            }

            // 3. Rechazo explícito del servidor (suspensión/cancelación manual).
            //    No hay período de gracia: un contracargo o un fraude no se lo ganó.
            if (licencia.RechazadaPorServidor ||
                (!string.IsNullOrEmpty(licencia.EstadoServidor) && licencia.EstadoServidor != "ACTIVO"))
            {
                return Bloqueo("Su licencia fue suspendida. Comuníquese para regularizar la situación.");
            }

            // 4. Gracia offline: hace demasiado que no se puede confirmar contra el
            //    servidor. Es distinto de "venció": acá no sabemos si pagó o no.
            double diasOffline = (DateTime.Now - licencia.FechaUltimaValidacionOnline).TotalDays;
            if (diasOffline > LicenciaConfig.DiasGraciaOffline)
            {
                return Bloqueo($"Pasaron más de {LicenciaConfig.DiasGraciaOffline} días sin poder verificar la suscripción. Conéctese a internet para continuar.");
            }

            // 5. Escalera por fecha de vencimiento.
            int diasRestantes = (int)Math.Floor((licencia.FechaVencimientoLocal.Date - DateTime.Now.Date).TotalDays);

            if (diasRestantes < -LicenciaConfig.DiasGracia)
            {
                return new ResultadoLicencia
                {
                    Estado = EstadoLicencia.Restringido,
                    DiasRestantes = diasRestantes,
                    Mensaje = $"Su suscripción venció el {licencia.FechaVencimientoLocal.ToShortDateString()}. " +
                              "Puede cerrar la caja, consultar el historial y exportar sus datos, " +
                              "pero no registrar ventas nuevas hasta regularizar el pago.",
                };
            }

            if (diasRestantes < 0)
            {
                int quedan = LicenciaConfig.DiasGracia + diasRestantes;
                return new ResultadoLicencia
                {
                    Estado = EstadoLicencia.Gracia,
                    DiasRestantes = diasRestantes,
                    Mensaje = $"Su suscripción venció el {licencia.FechaVencimientoLocal.ToShortDateString()}. " +
                              $"Puede seguir trabajando {quedan} día(s) más mientras regulariza el pago.",
                };
            }

            if (diasRestantes <= LicenciaConfig.DiasAvisoPrevio)
            {
                return new ResultadoLicencia
                {
                    Estado = EstadoLicencia.PorVencer,
                    DiasRestantes = diasRestantes,
                    Mensaje = diasRestantes == 0
                        ? "Su suscripción vence hoy. Renuévela para no interrumpir el servicio."
                        : $"Su suscripción vence en {diasRestantes} día(s).",
                };
            }

            return new ResultadoLicencia
            {
                Estado = EstadoLicencia.AlDia,
                DiasRestantes = diasRestantes,
                Mensaje = "Licencia vigente.",
            };
        }

        private static ResultadoLicencia Bloqueo(string mensaje) =>
            new ResultadoLicencia { Estado = EstadoLicencia.Bloqueado, Mensaje = mensaje, DiasRestantes = 0 };

        /// <summary>
        /// Compatibilidad con el contrato anterior (bool + mensaje). "Válido" acá
        /// significa que puede entrar al sistema, aunque sea en modo restringido.
        /// </summary>
        public static (bool valido, string mensaje) ValidarLicenciaLocal()
        {
            var r = EvaluarLicenciaLocal();
            return (r.PuedeEntrar, r.Mensaje);
        }
    }
}
