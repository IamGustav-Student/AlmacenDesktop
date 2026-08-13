using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression; // Opcional si quisieras comprimir, por ahora copia directa para velocidad
using System.Text;
using System.Windows.Forms;

namespace AlmacenDesktop.Services
{
    public class BackupService
    {
        private readonly string _dbName = "almacen.db";
        private readonly string _backupFolder;

        public BackupService()
        {
            // Carpeta "Backups" al lado del ejecutable
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            _backupFolder = Path.Combine(basePath, "Backups");

            if (!Directory.Exists(_backupFolder))
            {
                Directory.CreateDirectory(_backupFolder);
            }
        }

        public void RealizarBackupAutomatico()
        {
            string destPath = null;
            try
            {
                string sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _dbName);

                if (!File.Exists(sourcePath)) return; // No hay nada que salvar

                // Nombre formato: backup_2023-10-25_14-30-00.db
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string destFileName = $"backup_{timestamp}.db";
                destPath = Path.Combine(_backupFolder, destFileName);

                // Copia de seguridad
                File.Copy(sourcePath, destPath, true);

                // Verificación básica: si el tamaño no coincide, la copia quedó
                // incompleta (disco lleno, lock parcial, etc.) — no confiar en que
                // File.Copy sin excepción implica "backup completo".
                long tamanioOriginal = new FileInfo(sourcePath).Length;
                long tamanioCopia = new FileInfo(destPath).Length;
                if (tamanioCopia != tamanioOriginal)
                {
                    File.Delete(destPath); // No dejar un backup corrupto ocupando lugar
                    throw new IOException($"Backup incompleto: original {tamanioOriginal} bytes, copia {tamanioCopia} bytes.");
                }

                // MANTENIMIENTO: Borrar backups muy viejos (más de 30 días) para no llenar el disco
                LimpiarBackupsViejos();
            }
            catch (Exception ex)
            {
                // No interrumpimos el cierre de la app con un MessageBox, pero el error
                // queda en un log persistente — antes solo iba a Debug.WriteLine, que en
                // una instalación real (sin debugger adjunto) no se ve en ningún lado.
                LoguearErrorBackup(ex, destPath);
            }
        }

        private void LoguearErrorBackup(Exception ex, string destPath)
        {
            try
            {
                string logPath = Path.Combine(_backupFolder, "backup_errores.log");
                string linea = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Falló el backup automático" +
                               (destPath != null ? $" ({destPath})" : "") + $": {ex.Message}{Environment.NewLine}";
                File.AppendAllText(logPath, linea);
            }
            catch { /* Si ni el log se puede escribir, no hay nada más que hacer acá */ }
        }

        public string RealizarBackupManual(string carpetaDestino)
        {
            try
            {
                string sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _dbName);
                if (!File.Exists(sourcePath)) throw new FileNotFoundException("No se encuentra la base de datos.");

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
                string destPath = Path.Combine(carpetaDestino, $"VENDEMAX_Respaldo_{timestamp}.db");

                File.Copy(sourcePath, destPath, true);
                return destPath;
            }
            catch (Exception ex)
            {
                throw new Exception("Falló el respaldo manual: " + ex.Message);
            }
        }

        /// <summary>
        /// Restaura la base de datos a partir de un archivo de backup elegido por el
        /// usuario. Antes de tocar nada, guarda una copia de la base actual (por si
        /// eligió el backup equivocado) y valida que el archivo sea realmente un
        /// SQLite antes de reemplazar nada. El reemplazo real y el reinicio de la app
        /// corren en un script auxiliar — mismo patrón ya probado que usa el
        /// auto-actualizador (UpdateService): espera a que este proceso cierre (con
        /// reintentos acotados, no infinitos) y, pase lo que pase, siempre vuelve a
        /// abrir la app al final.
        /// </summary>
        public void RestaurarBackup(string archivoBackupElegido)
        {
            if (!File.Exists(archivoBackupElegido))
                throw new FileNotFoundException("No se encuentra el archivo de backup seleccionado.");

            // Chequeo básico de que sea realmente un SQLite y no cualquier otro
            // archivo que el usuario haya elegido por error (encabezado "SQLite format 3\0").
            byte[] header = new byte[16];
            using (var fs = File.OpenRead(archivoBackupElegido))
            {
                int leidos = fs.Read(header, 0, 16);
                if (leidos < 16 || Encoding.ASCII.GetString(header, 0, 15) != "SQLite format 3")
                {
                    throw new InvalidDataException("El archivo elegido no parece ser un backup válido de Vendemax Desktop.");
                }
            }

            string destinoDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _dbName);

            if (File.Exists(destinoDbPath))
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string preRestorePath = Path.Combine(_backupFolder, $"backup_pre-restauracion_{timestamp}.db");
                File.Copy(destinoDbPath, preRestorePath, true);
            }

            LanzarRestauracionYSalir(archivoBackupElegido, destinoDbPath);
        }

        private void LanzarRestauracionYSalir(string backupPath, string destinoDbPath)
        {
            string exePath = Process.GetCurrentProcess().MainModule?.FileName
                ?? throw new InvalidOperationException("No se pudo determinar la ruta del ejecutable actual.");
            int pid = Process.GetCurrentProcess().Id;
            string stagingPath = destinoDbPath + ".restaurando";
            string batPath = Path.Combine(Path.GetTempPath(), $"vdmx_restore_{pid}.bat");

            // 1) Copia el backup elegido a un archivo "staging" junto a almacen.db
            //    (no toca el archivo real todavía). 2) Espera a que este proceso
            // cierre. 3) Reintenta MOVER el staging sobre almacen.db (acotado, no
            // infinito — un antivirus o un handle que tarda en soltarse no debe
            // dejar el script reintentando para siempre en silencio). 4) Pase lo
            // que pase, siempre relanza la app al final.
            string script =
                "@echo off\r\n" +
                "setlocal enabledelayedexpansion\r\n" +
                $"copy /Y \"{backupPath}\" \"{stagingPath}\" >NUL 2>&1\r\n" +
                "set intentos=0\r\n" +
                ":waitproc\r\n" +
                "set /a intentos+=1\r\n" +
                "if !intentos! GTR 30 goto intentar_reemplazo\r\n" +
                $"tasklist /FI \"PID eq {pid}\" 2>NUL | find \"{pid}\" >NUL\r\n" +
                "if not errorlevel 1 (\r\n" +
                "    ping -n 2 127.0.0.1 >NUL\r\n" +
                "    goto waitproc\r\n" +
                ")\r\n" +
                ":intentar_reemplazo\r\n" +
                "set intentos=0\r\n" +
                ":retrymove\r\n" +
                "set /a intentos+=1\r\n" +
                $"move /Y \"{stagingPath}\" \"{destinoDbPath}\" >NUL 2>&1\r\n" +
                // Igual que en el updater: no confiar en el errorlevel de "move" —
                // chequeamos si el staging sigue existiendo (si sigue ahí, falló).
                $"if not exist \"{stagingPath}\" goto relanzar\r\n" +
                "if !intentos! LSS 25 (\r\n" +
                "    ping -n 2 127.0.0.1 >NUL\r\n" +
                "    goto retrymove\r\n" +
                ")\r\n" +
                ":relanzar\r\n" +
                $"start \"\" \"{exePath}\"\r\n" +
                $"del \"{stagingPath}\" >NUL 2>&1\r\n" +
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

            // Application.Exit (no Environment.Exit) para que el flujo normal de
            // Program.cs siga su curso antes de que el proceso termine.
            Application.Exit();
        }

        private void LimpiarBackupsViejos()
        {
            try
            {
                var directory = new DirectoryInfo(_backupFolder);
                var files = directory.GetFiles("backup_*.db");

                foreach (var file in files)
                {
                    if (file.CreationTime < DateTime.Now.AddDays(-30))
                    {
                        file.Delete();
                    }
                }
            }
            catch { /* Ignorar errores de limpieza */ }
        }
    }
}