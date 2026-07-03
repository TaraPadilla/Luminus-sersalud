using System;

namespace sistema.Utilidades
{
    public static class ExceptionHelper
    {
        public static string ObtenerMensajeRaiz(Exception ex)
        {
            if (ex == null)
                return string.Empty;

            while (ex.InnerException != null)
                ex = ex.InnerException;

            return ex.Message;
        }
    }
}
