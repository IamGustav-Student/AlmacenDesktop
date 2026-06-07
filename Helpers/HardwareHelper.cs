using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace AlmacenDesktop.Helpers
{
    public static class HardwareHelper
    {
        private static string _cachedFingerprint = null;

        /// <summary>
        /// Obtiene una firma criptográfica única de hardware (Machine Fingerprint) para esta PC.
        /// </summary>
        public static string ObtenerMachineFingerprint()
        {
            if (!string.IsNullOrEmpty(_cachedFingerprint))
            {
                return _cachedFingerprint;
            }

            StringBuilder sb = new StringBuilder();

            // 1. Obtener identificador del Procesador (CPU ID)
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                using (var collection = searcher.Get())
                {
                    foreach (var obj in collection)
                    {
                        var val = obj["ProcessorId"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            sb.Append(val.Trim());
                            break; // Tomar el primero
                        }
                    }
                }
            }
            catch
            {
                sb.Append("CPU_UNKNOWN_ID");
            }

            sb.Append("-");

            // 2. Obtener Número de Serie de la Placa Madre (Motherboard)
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                using (var collection = searcher.Get())
                {
                    foreach (var obj in collection)
                    {
                        var val = obj["SerialNumber"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(val) && !val.Equals("None", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append(val.Trim());
                            break;
                        }
                    }
                }
            }
            catch
            {
                sb.Append("MB_UNKNOWN_SERIAL");
            }

            // Fallback en caso de que ambos fallen o devuelvan datos genéricos (ej. máquinas virtuales)
            if (sb.ToString() == "CPU_UNKNOWN_ID-MB_UNKNOWN_SERIAL" || sb.Length < 10)
            {
                // Agregamos un identificador basado en el nombre de la máquina y variables del sistema
                sb.Append($"-{Environment.MachineName}-{Environment.UserName}");
            }

            // Calcular SHA-256 para estandarizar el tamaño de la huella a 64 caracteres
            _cachedFingerprint = CalcularHashSHA256(sb.ToString());
            return _cachedFingerprint;
        }

        private static string CalcularHashSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
