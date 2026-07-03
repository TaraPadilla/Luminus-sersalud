using System;
using Database.Shared;
using Database.Shared.SqlDataSeed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace farmamest.Utilidades
{
    /// <summary>
    /// Dev-only database bootstrap. Production servers keep their own DB and appsettings untouched.
    /// </summary>
    public static class DatabaseStartupBootstrap
    {
        public static void RunIfAllowed(IHost host)
        {
            var env = host.Services.GetRequiredService<IWebHostEnvironment>();
            var configuration = host.Services.GetRequiredService<IConfiguration>();
            var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseStartupBootstrap");

            if (!env.IsDevelopment())
            {
                logger.LogInformation(
                    "Database bootstrap skipped ({Environment}). Server DB and appsettings are not modified on startup.",
                    env.EnvironmentName);
                return;
            }

            var connectionString = ConnectionStringResolver.Resolve(configuration);
            if (ConnectionStringResolver.LooksLikeRemoteDatabase(connectionString))
            {
                logger.LogWarning(
                    "Database bootstrap skipped: connection string points to a remote/production host. " +
                    "Use a local database for Development, or set ASPNETCORE_ENVIRONMENT=Production on the server.");
                return;
            }

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<Context>();
                context.Database.EnsureCreated();
                PostgresStoredProcedureBootstrap.EnsureCreated(
                    context,
                    (ex, _) => logger.LogWarning(ex, "Could not apply a PostgreSQL stored procedure."));
                HospitalizacionSerSaludSchemaBootstrap.EnsureSchema(
                    context,
                    (ex, _) => logger.LogWarning(ex, "Could not apply a SerSalud hospitalization schema script."));
                CatalogLookupSeedBootstrap.EnsureRequiredLookups(context, logger);
                BodegaSeedBootstrap.EnsureDefaultBodegas(context, logger);
                CategoriaCuentaContableSeedBootstrap.EnsureDefaultCategories(context, logger);
                UsuarioEmpleadoSeedBootstrap.EnsureDefaultEmpleadoLinks(context, logger);
                ReporteVentasDemoSeedBootstrap.EnsureDemoVentas(context, logger);
                var webRoot = services.GetRequiredService<IWebHostEnvironment>().WebRootPath;
                ExpedienteHospitalizacionDemoSeedBootstrap.EnsureDemoExpedienteData(context, logger, webRoot);
                HospitalizacionClinicalCatalogSeedBootstrap.EnsureClinicalCatalogs(context, logger);
                logger.LogInformation("Development database bootstrap completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Development database bootstrap failed. Application will continue.");
            }
        }
    }
}
