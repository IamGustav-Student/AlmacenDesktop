using System;
using System.IO;
using System.IO.Compression; // Opcional si quisieras comprimir, por ahora copia directa para velocidad
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