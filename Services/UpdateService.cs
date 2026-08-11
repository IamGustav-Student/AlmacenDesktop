using AlmacenDesktop.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlmacenDesktop.Services
{
    public class UpdateInfo
    {
        public Version Version { get; set; } = new Version(0, 0, 0, 0);
        public string VersionTag { get; set; } = "";
        public string DownloadUrl { get; set; } = "";
        public string Notes { get; set; } = "";
        public long AssetSize { get; set; }
    }

    /// <summary>
    /// Actualizador automático: consulta el último release público de GitHub, y si es más
    /// nuevo que la versión actual, descarga el .exe y reemplaza el que está corriendo.
    /// No requiere instalador — la app es un único .exe self-contained, y la base SQLite
    /// vive aparte (junto al .exe), así que reemplazar el archivo no toca los datos.
    /// </summary>
    public class UpdateService
    {
        private static readonly HttpClient _http = CrearHttpClient();

        private static HttpClient CrearHttpClient()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            // La API de GitHub exige un User-Agent o rechaza el pedido.
            client.DefaultRequestHeaders.UserAgent.ParseAdd("VendemaxDesktop-UpdateChecker");
            return client;
        }

        /// <summary>
        /// Devuelve info de la actualización disponible, o null si ya está al día,
        /// no hay conexión, o cualquier otra falla — nunca tira excepción hacia afuera.
        /// </summary>
        public async Task<UpdateInfo?> BuscarActualizacionAsync()
        {
            try
            {
                using var response = await _http.GetAsync(Constantes.GITHUB_RELEASES_API);
                if (!response.IsSuccessStatusCode) return null;

                using var stream = await response.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                var root = doc.RootElement;

                string tag = root.TryGetProperty("tag_name", out var tagProp) ? tagProp.GetString() ?? "" : "";
                string cleanTag = tag.TrimStart('v', 'V');
                if (!Version.TryParse(cleanTag, out var remoteVersion)) return null;

                var current = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0, 0);
                if (remoteVersion.CompareTo(current) <= 0) return null;

                string downloadUrl = "";
                long size = 0;
                if (root.TryGetProperty("assets", out var assets))
                {
                    foreach (var asset in assets.EnumerateArray())
                    {
                        string name = asset.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
                        if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadUrl = asset.TryGetProperty("browser_download_url", out var u) ? u.GetString() ?? "" : "";
                            size = asset.TryGetProperty("size", out var s) ? s.GetInt64() : 0;
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(downloadUrl)) return null;

                string notes = root.TryGetProperty("body", out var bodyProp) ? bodyProp.GetString() ?? "" : "";

                return new UpdateInfo
                {
                    Version = remoteVersion,
                    VersionTag = tag,
                    DownloadUrl = downloadUrl,
                    Notes = notes,
                    AssetSize = size,
                };
            }
            catch
            {
                // Sin conexión, GitHub caído, rate limit, JSON inesperado — no interrumpe al usuario.
                return null;
            }
        }

        /// <summary>
        /// Descarga el nuevo .exe y deja armado el reemplazo: lanza un script auxiliar que
        /// espera a que este proceso termine, mueve el archivo nuevo sobre el actual, y
        /// vuelve a abrir la app. Al terminar, cierra la aplicación (vía Application.Exit,
        /// para que corra el backup automático de salida antes de irse).
        /// </summary>
        public async Task DescargarEInstalarAsync(string downloadUrl, IProgress<int>? progress = null)
        {
            string currentExePath = Process.GetCurrentProcess().MainModule?.FileName
                ?? throw new InvalidOperationException("No se pudo determinar la ruta del ejecutable actual.");
            string tempPath = currentExePath + ".update";

            using (var response = await _http.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                long total = response.Content.Headers.ContentLength ?? -1L;

                using var httpStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None);

                var buffer = new byte[81920];
                long totalRead = 0;
                int read;
                while ((read = await httpStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, read);
                    totalRead += read;
                    if (total > 0) progress?.Report((int)(totalRead * 100 / total));
                }
            }

            // Sanity check: un .exe self-contained real pesa decenas de MB — si vino
            // mucho más chico, algo salió mal (página de error, conexión cortada, etc.)
            var info = new FileInfo(tempPath);
            if (info.Length < 5_000_000)
            {
                File.Delete(tempPath);
                throw new Exception("El archivo descargado parece incompleto. Probá de nuevo más tarde.");
            }

            LanzarReemplazoYSalir(tempPath, currentExePath);
        }

        private void LanzarReemplazoYSalir(string newExePath, string currentExePath)
        {
            int pid = Process.GetCurrentProcess().Id;
            string batPath = Path.Combine(Path.GetTempPath(), $"vdmx_update_{pid}.bat");
            string workDir = Path.GetDirectoryName(currentExePath) ?? Path.GetTempPath();

            // Espera a que este proceso termine (por PID) y reemplaza el .exe. Con
            // reintentos ACOTADOS (no infinitos) porque el archivo puede quedar
            // bloqueado un rato por el antivirus escaneando el ejecutable recién
            // bajado — sin un límite, el script podía quedar reintentando para
            // siempre en silencio y la app nunca se volvía a abrir. Pase lo que
            // pase con el reemplazo, al final SIEMPRE se relanza algo: la versión
            // nueva si se pudo reemplazar, o la vieja si no, para que el usuario
            // nunca se quede con la app cerrada sin explicación.
            string script =
                "@echo off\r\n" +
                "setlocal enabledelayedexpansion\r\n" +
                $"cd /d \"{workDir}\"\r\n" +
                "set intentos=0\r\n" +
                ":waitproc\r\n" +
                "set /a intentos+=1\r\n" +
                "if !intentos! GTR 30 goto intentar_reemplazo\r\n" +
                $"tasklist /FI \"PID eq {pid}\" 2>NUL | find \"{pid}\" >NUL\r\n" +
                "if not errorlevel 1 (\r\n" +
                // "timeout" necesita una consola real y falla en un proceso oculto sin
                // ventana (CreateNoWindow) — "ping" da una espera de ~1s sin ese problema.
                "    ping -n 2 127.0.0.1 >NUL\r\n" +
                "    goto waitproc\r\n" +
                ")\r\n" +
                ":intentar_reemplazo\r\n" +
                "set intentos=0\r\n" +
                ":retrymove\r\n" +
                "set /a intentos+=1\r\n" +
                $"move /Y \"{newExePath}\" \"{currentExePath}\" >NUL 2>&1\r\n" +
                // No confiar en errorlevel de "move": en la práctica puede devolver 0
                // aunque haya fallado ("Acceso denegado"). Chequeamos el estado real
                // del archivo — si el origen ya no existe, el move sí funcionó.
                $"if not exist \"{newExePath}\" goto relanzar\r\n" +
                "if !intentos! LSS 25 (\r\n" +
                // "timeout" necesita una consola real y falla en un proceso oculto sin
                // ventana (CreateNoWindow) — "ping" da una espera de ~1s sin ese problema.
                "    ping -n 2 127.0.0.1 >NUL\r\n" +
                "    goto retrymove\r\n" +
                ")\r\n" +
                ":relanzar\r\n" +
                $"start \"\" \"{currentExePath}\"\r\n" +
                $"del \"{newExePath}\" >NUL 2>&1\r\n" +
                "del \"%~f0\"\r\n";

            File.WriteAllText(batPath, script);

            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{batPath}\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            Process.Start(psi);

            // Application.Exit (no Environment.Exit) para que el flujo normal de Program.cs
            // siga su curso y corra el backup automático antes de que el proceso termine.
            System.Windows.Forms.Application.Exit();
        }
    }
}
