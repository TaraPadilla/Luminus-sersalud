using Database.Shared;
using Database.Shared.SqlDataSeed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace farmamest.Utilidades
{
    /// <summary>
    /// Demo/schema seeds only in Development. Production PDFs and requests must not alter the client database.
    /// </summary>
    public static class DevelopmentDemoSeedGuard
    {
        public static void EnsureExpedienteHospitalizacion(
            IWebHostEnvironment env,
            Context context,
            int hospitalizacionId,
            ILogger logger = null)
        {
            if (env == null || !env.IsDevelopment())
                return;

            ExpedienteHospitalizacionDemoSeedBootstrap.EnsureDemoExpedienteDataForHospitalizacion(
                context, hospitalizacionId, logger);
        }

        public static void EnsureExpedientePdfAssets(
            IWebHostEnvironment env,
            string webRoot,
            ILogger logger = null)
        {
            if (env == null || !env.IsDevelopment())
                return;

            ExpedienteHospitalizacionDemoSeedBootstrap.EnsureDemoExpedientePdfAssets(webRoot, logger);
        }

        public static void EnsureCatalogLookups(
            IWebHostEnvironment env,
            Context context,
            ILogger logger = null)
        {
            if (env == null || !env.IsDevelopment())
                return;

            CatalogLookupSeedBootstrap.EnsureRequiredLookups(context, logger);
        }
    }
}
