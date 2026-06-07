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
        public static bool GuardarLicenciaLocal(string email, string clave, DateTime vencimiento)
        {
            try
            {
                var info = new LicenciaInfo
                {
                    Email = email.Trim().ToLower(),
                    Clave = clave.Trim().ToUpper(),
                    FechaUltimaValidacionOnline = DateTime.Now,
                    FechaVencimientoLocal = vencimiento,
                    Fingerprint = HardwareHelper.ObtenerMachineFingerprint()
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
        /// Comprueba si la licencia local es válida y está dentro del periodo de gracia offline.
        /// </summary>
        public static (bool valido, string mensaje) ValidarLicenciaLocal()
        {
            var licencia = LeerLicenciaLocal();

            if (licencia == null)
            {
                return (false, "El sistema no se encuentra activado. Ingrese su clave de activación.");
            }

            // 1. Validar vinculación de hardware
            string currentFingerprint = HardwareHelper.ObtenerMachineFingerprint();
            if (licencia.Fingerprint != currentFingerprint)
            {
                return (false, "La clave de activación no coincide con el hardware de esta computadora.");
            }

            // 2. Validar que la suscripción no haya vencido en el servidor
            if (DateTime.Now > licencia.FechaVencimientoLocal)
            {
                return (false, $"Su suscripción venció el {licencia.FechaVencimientoLocal.ToShortDateString()}. regularice su pago.");
            }

            // 3. Validar período de gracia offline (máximo 7 días consecutivos sin revalidar online)
            var diasOffline = (DateTime.Now - licencia.FechaUltimaValidacionOnline).TotalDays;
            if (diasOffline > 7)
            {
                return (false, "Se excedió el período de gracia offline de 7 días. Se requiere conexión a internet para revalidar la suscripción.");
            }

            // 4. Protección básica contra manipulación de reloj del sistema
            if (DateTime.Now < licencia.FechaUltimaValidacionOnline)
            {
                return (false, "Se detectó una alteración en la hora del sistema. Corrija el reloj de Windows.");
            }

            return (true, "Licencia local válida.");
        }
    }
}
