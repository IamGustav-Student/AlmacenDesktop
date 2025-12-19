using System;
using System.IO;
using System.Windows.Forms; // Necesario para mostrar alertas visuales

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
                // 1. Verificar que la base de datos exista antes de intentar nada
                if (!File.Exists(NOMBRE_DB)) return;

                // 2. Crear carpeta si no existe
                if (!Directory.Exists(CARPETA_BACKUP))
                    Directory.CreateDirectory(CARPETA_BACKUP);

                // 3. Generar nombre único: "Backups\almacen_2023-12-01_14-30.bak"
                string fecha = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
                string nombreDestino = Path.Combine(CARPETA_BACKUP, $"almacen_{fecha}.bak");

                // 4. Copiar archivo (el 'true' permite sobrescribir si ya existe)
                File.Copy(NOMBRE_DB, nombreDestino, true);
                MessageBox.Show($"Backup creado en: {nombreDestino}", "Test Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);


                // 5. Mantenimiento: Borrar backups muy viejos para no llenar el disco
                LimpiarBackupsAntiguos();
            }
            catch (Exception ex)
            {
                // AHORA SÍ: Notificar al usuario si algo sale mal
                MessageBox.Show(
                    $"⚠️ No se pudo realizar la copia de seguridad automática.\n\nError: {ex.Message}\n\nPor favor, verifique el espacio en disco o los permisos.",
                    "Advertencia de Seguridad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private static void LimpiarBackupsAntiguos()
        {
            try
            {
                // Mantenemos solo los backups de los últimos 30 días
                var directorio = new DirectoryInfo(CARPETA_BACKUP);
                var archivos = directorio.GetFiles("*.bak");

                foreach (var archivo in archivos)
                {
                    // Si el archivo tiene más de 60 días de antigüedad
                    if (archivo.CreationTime < DateTime.Now.AddDays(-60))
                    {
                        archivo.Delete();
                    }
                }
            }
            catch
            {
                // Si falla la limpieza (ej. archivo en uso), no es crítico, lo ignoramos.
                // Aquí el silencio es aceptable porque no pone en riesgo los datos.
            }
        }
    }
}