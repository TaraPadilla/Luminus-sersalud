using System;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;

namespace farmamest.Utilidades
{
    public static class DatabaseExceptionHelper
    {
        public static bool IsConnectivityError(Exception ex)
        {
            while (ex != null)
            {
                if (ex is TimeoutException || ex is SocketException)
                    return true;

                var typeName = ex.GetType().FullName ?? "";
                if (typeName.IndexOf("RetryLimitExceeded", StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;

                if (typeName.IndexOf("Npgsql", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var msg = ex.Message ?? "";
                    if (msg.IndexOf("connect", StringComparison.OrdinalIgnoreCase) >= 0
                        || msg.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0
                        || msg.IndexOf("SSL", StringComparison.OrdinalIgnoreCase) >= 0
                        || msg.IndexOf("socket", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }

                ex = ex.InnerException;
            }

            return false;
        }

        public static string GetFriendlyMessage(Exception ex, bool isDevelopment, string connectionString = null)
        {
            var root = ex;
            while (root.InnerException != null)
                root = root.InnerException;

            var detail = root.Message ?? ex.Message ?? "";

            if (ConnectionStringResolver.IsPlaceholderConnectionString(connectionString))
            {
                return "La cadena de conexión en appsettings.json es una plantilla (Host=...) y no un servidor real. "
                     + "Copie el appsettings.json del servidor con ConnectionStrings:farmaowl correcta, "
                     + "o monte el archivo en /app/appsettings.json al ejecutar Docker.";
            }

            if (detail.IndexOf("No such host is known", StringComparison.OrdinalIgnoreCase) >= 0
                || detail.IndexOf("Name or service not known", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "No se pudo resolver el host de PostgreSQL ("
                     + ConnectionStringResolver.DescribeConnectionTarget(connectionString)
                     + "). Verifique ConnectionStrings:farmaowl en appsettings.json del servidor.";
            }

            if (isDevelopment && LooksLikeRemoteDatabase(connectionString))
            {
                return "No se pudo conectar a la base de datos PostgreSQL en DigitalOcean. "
                     + "La conexión remota puede tardar o estar bloqueada desde su red. "
                     + "Opciones: (1) verifique firewall/VPN y que su IP esté autorizada en DigitalOcean, "
                     + "(2) use PostgreSQL local con appsettings.Development.local.json, "
                     + "(3) ejecute con ASPNETCORE_ENVIRONMENT=Production si prueba contra el servidor del cliente.";
            }

            return "No se pudo conectar a la base de datos. "
                 + "Verifique que PostgreSQL esté disponible y que la cadena de conexión en appsettings.json sea correcta. "
                 + (isDevelopment ? "Detalle: " + detail : "");
        }

        public static bool LooksLikeRemoteDatabase(string connectionString)
        {
            return ConnectionStringResolver.LooksLikeRemoteDatabase(connectionString);
        }

        public static bool WantsJson(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            if (request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
                return true;

            var accept = request.Headers.Accept.ToString();
            return accept.IndexOf("application/json", StringComparison.OrdinalIgnoreCase) >= 0
                && accept.IndexOf("text/html", StringComparison.OrdinalIgnoreCase) < 0;
        }
    }
}
