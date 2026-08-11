using System;

namespace AlmacenDesktop.Helpers
{
    /// <summary>
    /// Excepciones como DbUpdateException traen el mensaje real en InnerException
    /// ("An error occurred while saving the entity changes. See the inner exception
    /// for details." es el mensaje de afuera, inútil para diagnosticar) — este
    /// helper baja hasta la excepción más profunda para mostrar algo accionable.
    /// </summary>
    public static class ExceptionHelper
    {
        public static string ObtenerMensaje(Exception ex)
        {
            var actual = ex;
            while (actual.InnerException != null)
            {
                actual = actual.InnerException;
            }
            return actual.Message;
        }
    }
}
