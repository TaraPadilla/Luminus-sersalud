using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace farmamest.Utilidades
{
    public static class DatabaseConnectivityProbe
    {
        public static async Task ProbeAsync(IConfiguration configuration, ILogger logger)
        {
            var connectionString = ConnectionStringResolver.Resolve(configuration);
            if (string.IsNullOrWhiteSpace(connectionString))
                return;

            var sw = Stopwatch.StartNew();
            try
            {
                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();
                sw.Stop();
                logger.LogInformation(
                    "PostgreSQL conectado en {Ms} ms ({Host}).",
                    sw.ElapsedMilliseconds,
                    MaskHost(connectionString));

                if (sw.ElapsedMilliseconds > 5000)
                {
                    logger.LogWarning(
                        "La conexion a PostgreSQL tardo {Ms} ms. Si ve timeouts, use appsettings.Development.local.json con BD local o aumente Timeout en la cadena.",
                        sw.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                logger.LogError(
                    ex,
                    "No se pudo conectar a PostgreSQL tras {Ms} ms ({Host}). {Hint}",
                    sw.ElapsedMilliseconds,
                    MaskHost(connectionString),
                    ConnectionStringResolver.LooksLikeRemoteDatabase(connectionString)
                        ? "Verifique firewall/VPN e IP autorizada en DigitalOcean, o copie appsettings.Development.local.json.example a appsettings.Development.local.json con PostgreSQL local."
                        : "Verifique que el servicio PostgreSQL local este en ejecucion.");
            }
        }

        private static string MaskHost(string connectionString)
        {
            try
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                return $"{builder.Host}:{builder.Port}/{builder.Database}";
            }
            catch
            {
                return "(cadena invalida)";
            }
        }
    }
}
