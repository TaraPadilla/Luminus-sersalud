using System;
using Microsoft.Extensions.Configuration;

namespace farmamest.Utilidades
{
    public static class ClienteConfigHelper
    {
        public static bool EsSerSalud(IConfiguration configuration)
        {
            return SerSaludUiHelper.EsSerSalud(configuration?["Cliente"]);
        }

        /// <summary>
        /// SerSalud siempre usa calendario de hospitalización (SOP), no consulta general.
        /// </summary>
        public static string ResolverModoCalendario(IConfiguration configuration, string modo)
        {
            if (EsSerSalud(configuration))
                return "hospitalizacion";

            return string.IsNullOrWhiteSpace(modo)
                ? "consulta"
                : modo.ToLowerInvariant().Trim();
        }
    }
}
