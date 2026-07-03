using System.Threading.Tasks;

namespace sistema.UtilidadesEmailWp.Services.IService
{
    public interface IWhatsAppService
    {
        /// <summary>Envía un mensaje de texto (Twilio o Meta según configuración).</summary>
        Task<bool> SendTextMessageAsync(string toPhoneNumber, string body);

        /// <summary>Envía una plantilla con documento PDF (Meta Cloud API o Twilio con media).</summary>
        Task<bool> SendTemplateDocumentAsync(
            string toPhoneNumber,
            string templateName,
            string documentUrl,
            string filename,
            string languageCode = "es");

        /// <summary>Envía una plantilla de solo texto (Meta Cloud API).</summary>
        Task<bool> SendTemplateTextAsync(
            string toPhoneNumber,
            string templateName,
            string[] bodyParameters,
            string languageCode = "es");
    }
}
