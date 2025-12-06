using System.Media;

namespace AlmacenDesktop.Helpers
{
    public static class AudioHelper
    {
        // Sonido satisfactorio de "Escaneo Correcto"
        public static void PlayOk()
        {
            // Usamos sonidos del sistema para no depender de archivos .wav externos por ahora
            // En el futuro podés poner tu propio "bip.wav"
            SystemSounds.Asterisk.Play();
        }

        // Sonido de "Error / No Encontrado / Stock Bajo"
        public static void PlayError()
        {
            SystemSounds.Hand.Play();
        }

        // Sonido de "Caja Registradora" al cobrar
        public static void PlayCobro()
        {
            SystemSounds.Exclamation.Play();
        }
    }
}