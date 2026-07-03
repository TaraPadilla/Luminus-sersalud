using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wkhtmltopdf.NetCore;
using Wkhtmltopdf.NetCore.Options;

namespace farmamest.Utilidades
{
    public static class PdfGenerationHelper
    {
        public static async Task<IActionResult> GetPdfSafeAsync(
            IGeneratePdf generatePdf,
            IWebHostEnvironment environment,
            ILogger logger,
            string viewPath,
            object model,
            ConvertOptions convertOptions = null)
        {
            PdfEnvironmentHelper.EnsureLinuxRotativaBinary(environment.WebRootPath, logger);
            PdfEnvironmentHelper.EnsureWindowsRotativaBinary(
                environment.ContentRootPath,
                environment.WebRootPath,
                logger);

            if (!PdfEnvironmentHelper.IsWkhtmltopdfAvailable(environment.WebRootPath, environment.ContentRootPath))
            {
                logger.LogError("PDF rechazado: wkhtmltopdf no disponible para {View}", viewPath);
                return new ContentResult
                {
                    StatusCode = 503,
                    ContentType = "text/plain; charset=utf-8",
                    Content = PdfEnvironmentHelper.GetUnavailableMessage()
                };
            }

            try
            {
                if (convertOptions != null)
                    generatePdf.SetConvertOptions(convertOptions);

                return await generatePdf.GetPdf(viewPath, model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al generar PDF {View}", viewPath);
                return ErrorContentResult(ex);
            }
        }

        public static IActionResult ErrorContentResult(Exception ex)
        {
            var detail = ex?.Message ?? "";
            if (ex?.InnerException != null)
                detail += " " + ex.InnerException.Message;

            return new ContentResult
            {
                StatusCode = 500,
                ContentType = "text/plain; charset=utf-8",
                Content = "Error al generar el PDF. " + detail +
                          " Revise el log del servidor (wkhtmltopdf / permisos)."
            };
        }

        public static string GetFriendlyErrorMessage(Exception ex)
        {
            var msg = ex?.ToString() ?? string.Empty;
            if (msg.IndexOf("Npgsql", StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("SSL handshake", StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("SocketException", StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("transient failure", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "No se pudo conectar a la base de datos PostgreSQL. "
                     + "En desarrollo use la base local (appsettings.Development.json). "
                     + "En producción verifique red, firewall y la cadena de conexión del servidor.";
            }

            if (msg.IndexOf("wkhtmltopdf", StringComparison.OrdinalIgnoreCase) >= 0)
                return "No se pudo generar el PDF: wkhtmltopdf no está instalado o no tiene permisos de ejecución.";

            var detail = ex?.Message;
            if (ex?.InnerException != null)
                detail += " " + ex.InnerException.Message;
            return "Error al generar el reporte. " + (detail ?? "Revise el log del servidor.");
        }
    }
}
