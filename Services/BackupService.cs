using System;
using System.IO;

namespace AlmacenDesktop.Services
{
    public static class BackupService
    {
        private const string NOMBRE_DB = "almacen.db";
        private const string CARPETA_BACKUP = "Backups";

        public static void RealizarBackupAutomatico()
        {
            try
            {
                if (!Directory.Exists(CARPETA_BACKUP)) Directory.CreateDirectory(CARPETA_BACKUP);
                if (!File.Exists(NOMBRE_DB)) return;

                // Formato: almacen_2023-10-25_14-30.bak
                string fecha = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
                string nombreDestino = Path.Combine(CARPETA_BACKUP, $"almacen_{fecha}.bak");

                // Copia segura (sobrescribe si existe, aunque por la fecha es difícil)
                File.Copy(NOMBRE_DB, nombreDestino, true);
            }
            catch
            {
                // Fallo silencioso para no molestar al usuario
            }
        }
    }
}
