using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace farmamest.Utilidades
{
    /// <summary>
    /// Validates server appsettings at startup with clear messages for production deploy.
    /// </summary>
    public static class ServerConfigurationValidator
    {
        public const string ConnectionStringName = "farmaowl";

        public static void ValidateOrFail(
            IWebHostEnvironment environment,
            IConfiguration configuration,
            ILogger logger)
        {
            var root = environment.ContentRootPath;
            var appsettingsPath = Path.Combine(root, "appsettings.json");
            var hasErrors = false;

            logger.LogInformation(
                "Validando configuracion del servidor. ContentRoot={ContentRoot}, Environment={Environment}",
                root,
                environment.EnvironmentName);

            if (!File.Exists(appsettingsPath))
            {
                hasErrors = true;
                logger.LogError(
                    "FALTA appsettings.json en la carpeta de la aplicacion ({Path}). " +
                    "El publish y la imagen Docker deben incluir appsettings.json " +
                    "(desde su appsettings local o desde appsettings.ESTRUCTURA-SERVIDOR.json). " +
                    "Vuelva a publicar o monte el archivo en /app/appsettings.json.",
                    appsettingsPath);
            }
            else
            {
                logger.LogInformation("appsettings.json encontrado: {Path}", appsettingsPath);
            }

            var connectionString = ConnectionStringResolver.Resolve(configuration);
            var isPublishedDeploy = !File.Exists(Path.Combine(root, "farmamest.csproj"));

            if (environment.IsProduction() && isPublishedDeploy)
            {
                var devPath = Path.Combine(root, "appsettings.Development.json");
                if (File.Exists(devPath))
                {
                    hasErrors = true;
                    logger.LogError(
                        "PRODUCCION: existe appsettings.Development.json ({Path}). " +
                        "ELIMINE este archivo del servidor. Solo debe existir appsettings.json.",
                        devPath);
                }

                foreach (var extra in Directory.GetFiles(root, "appsettings.*.json"))
                {
                    var name = Path.GetFileName(extra);
                    if (string.Equals(name, "appsettings.json", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (string.Equals(name, "appsettings.ESTRUCTURA-SERVIDOR.json", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (string.Equals(name, "appsettings.local.json", StringComparison.OrdinalIgnoreCase))
                        continue;

                    hasErrors = true;
                    logger.LogError(
                        "PRODUCCION: archivo de configuracion no permitido: {File}. " +
                        "El servidor debe usar UN solo appsettings.json.",
                        name);
                }
            }
            else if (environment.IsProduction())
            {
                foreach (var extra in Directory.GetFiles(root, "appsettings.*.json"))
                {
                    var name = Path.GetFileName(extra);
                    if (string.Equals(name, "appsettings.json", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (string.Equals(name, "appsettings.ESTRUCTURA-SERVIDOR.json", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (string.Equals(name, "appsettings.local.json", StringComparison.OrdinalIgnoreCase))
                        continue;

                    logger.LogWarning(
                        "Prueba local en Production: existe {File}. En el servidor publicado no debe haber archivos appsettings extra.",
                        name);
                }
            }
            else if (!environment.IsDevelopment())
            {
                var devPath = Path.Combine(root, "appsettings.Development.json");
                if (File.Exists(devPath))
                {
                    logger.LogWarning(
                        "Existe appsettings.Development.json ({Path}). " +
                        "RENOMBRE o ELIMINE ese archivo antes de produccion.",
                        devPath);
                }

                foreach (var extra in Directory.GetFiles(root, "appsettings.*.json"))
                {
                    var name = Path.GetFileName(extra);
                    if (string.Equals(name, "appsettings.json", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (string.Equals(name, "appsettings.ESTRUCTURA-SERVIDOR.json", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (string.Equals(name, "appsettings.local.json", StringComparison.OrdinalIgnoreCase))
                        continue;

                    logger.LogWarning(
                        "Archivo de configuracion extra detectado: {File}. " +
                        "En produccion use solo appsettings.json.",
                        name);
                }
            }

            if (environment.IsDevelopment() && LooksLikeRemoteDatabase(connectionString))
            {
                logger.LogWarning(
                    "ASPNETCORE_ENVIRONMENT=Development con ConnectionStrings:{Name} apuntando a BD remota. " +
                    "Para pruebas identicas a produccion use ASPNETCORE_ENVIRONMENT=Production. " +
                    "Los seeds de desarrollo no se ejecutaran contra BD remota.",
                    ConnectionStringName);
            }

            var cliente = configuration["Cliente"];
            if (string.IsNullOrWhiteSpace(cliente))
            {
                logger.LogWarning(
                    "La clave 'Cliente' no esta en appsettings. " +
                    "El layout puede ser incorrecto. Ejemplo: \"Cliente\": \"AVM\" o \"SS\".");
            }
            else
            {
                logger.LogInformation("Cliente configurado: {Cliente}", cliente.Trim());

                var pestana = configuration["EstablecimientoPestana"] ?? "";
                if (cliente.Trim().Equals("SS", StringComparison.OrdinalIgnoreCase)
                    && pestana.IndexOf("AVM", StringComparison.OrdinalIgnoreCase) >= 0
                    && pestana.IndexOf("SerSalud", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    logger.LogWarning(
                        "Cliente=SS pero EstablecimientoPestana='{Pestana}' parece de otro tenant. " +
                        "Revise appsettings.json del servidor: diseño y menus dependen de Cliente y estas claves.",
                        pestana);
                }
            }

            var envOverlay = Path.Combine(root, $"appsettings.{environment.EnvironmentName}.json");
            if (File.Exists(envOverlay))
            {
                logger.LogInformation(
                    "Capa de configuracion activa: {Overlay} (Environment={Environment}). " +
                    "En produccion use solo appsettings.json del servidor.",
                    Path.GetFileName(envOverlay),
                    environment.EnvironmentName);
            }
            else if (!environment.IsProduction())
            {
                logger.LogInformation(
                    "Sin archivo appsettings.{Environment}.json; se usa unicamente appsettings.json.",
                    environment.EnvironmentName);
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                hasErrors = true;
                logger.LogError(
                    "Falta ConnectionStrings:{Name} en appsettings.json del servidor. " +
                    "Sin esta cadena la aplicacion no puede conectar a PostgreSQL.",
                    ConnectionStringName);
            }
            else if (ConnectionStringResolver.IsPlaceholderConnectionString(connectionString))
            {
                hasErrors = true;
                logger.LogError(
                    "ConnectionStrings:{Name} contiene valores plantilla (Host=..., etc.). " +
                    "El contenedor/publish uso appsettings.ESTRUCTURA-SERVIDOR.json en lugar del appsettings.json real. " +
                    "Incluya sistema/appsettings.json en el build Docker o monte /app/appsettings.json en el servidor. " +
                    "Destino actual: {Target}",
                    ConnectionStringName,
                    ConnectionStringResolver.DescribeConnectionTarget(connectionString));
            }
            else
            {
                var resolvedName = ConnectionStringResolver.ResolveName(configuration);
                logger.LogInformation(
                    "ConnectionStrings:{Name} configurada (resuelta desde '{Resolved}'). Destino={Target}. Cliente={Cliente}",
                    ConnectionStringName,
                    resolvedName,
                    ConnectionStringResolver.DescribeConnectionTarget(connectionString),
                    string.IsNullOrWhiteSpace(cliente) ? "(sin Cliente)" : cliente.Trim());
            }

            var webRoot = Path.Combine(root, "wwwroot");
            PdfEnvironmentHelper.EnsureWindowsRotativaBinary(root, webRoot, logger);
            var wkhtmlEngine = PdfEnvironmentHelper.GetEngineWindowsPath();
            var wkhtmlWww = PdfEnvironmentHelper.GetWebRootWindowsPath(webRoot);
            if (!environment.IsDevelopment()
                && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!File.Exists(wkhtmlEngine) && !File.Exists(wkhtmlWww))
                {
                    logger.LogWarning(
                        "PDF: no se encontro wkhtmltopdf.exe en {EnginePath} ni en {WwwPath}.",
                        wkhtmlEngine,
                        wkhtmlWww);
                }
            }

            if (hasErrors)
            {
                throw new InvalidOperationException(
                    "Configuracion del servidor incompleta. Revise el log de inicio: " +
                    "debe existir appsettings.json con ConnectionStrings:farmaowl. " +
                    "Incluya appsettings.json en el publish o montelo en /app/appsettings.json (Docker). " +
                    "En produccion use ASPNETCORE_ENVIRONMENT=Production.");
            }
        }

        private static bool LooksLikeRemoteDatabase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return false;

            var lower = connectionString.ToLowerInvariant();
            if (lower.Contains("127.0.0.1") || lower.Contains("localhost"))
                return false;

            return lower.Contains("ondigitalocean.com")
                || lower.Contains("amazonaws.com")
                || lower.Contains("azure.com")
                || (lower.Contains("host=") && !lower.Contains("host=127.0.0.1") && !lower.Contains("host=localhost"))
                || (lower.Contains("server=") && !lower.Contains("server=127.0.0.1") && !lower.Contains("server=localhost"));
        }
    }
}
