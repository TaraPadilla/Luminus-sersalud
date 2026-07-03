using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sistema.UtilidadesEmailWp.Services.IService;
using farmamest.UtilidadesEmailWp;

namespace farmamest.UtilidadesEmailWp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WhatsAppMessagingController : ControllerBase
    {
        private readonly IWhatsAppService _whatsAppService;
        private readonly WhatsAppSettings _settings;

        public WhatsAppMessagingController(
            IWhatsAppService whatsAppService,
            Microsoft.Extensions.Options.IOptions<WhatsAppSettings> settings)
        {
            _whatsAppService = whatsAppService;
            _settings = settings.Value;
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok(new
            {
                enabled = _settings.Enabled,
                hasTwilio = _settings.HasTwilio,
                hasMeta = _settings.HasMetaCloudApi
            });
        }

        [HttpPost("send-text")]
        public async Task<IActionResult> SendText([FromBody] WhatsAppTextRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.To) || string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { success = false, message = "Teléfono y mensaje son requeridos." });

            var ok = await _whatsAppService.SendTextMessageAsync(request.To, request.Message);
            return ok
                ? Ok(new { success = true, message = "Mensaje enviado." })
                : BadRequest(new
                {
                    success = false,
                    message = "No se pudo enviar el mensaje. Verifique WhatsAppSettings (Enabled y credenciales)."
                });
        }

        [HttpPost("send-template-document")]
        public async Task<IActionResult> SendTemplateDocument([FromBody] WhatsAppTemplateDocumentRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.To) ||
                string.IsNullOrWhiteSpace(request.DocumentUrl))
            {
                return BadRequest(new { success = false, message = "Teléfono y URL del documento son requeridos." });
            }

            var templateName = string.IsNullOrWhiteSpace(request.TemplateName)
                ? _settings.TemplateLabResults
                : request.TemplateName;
            var filename = string.IsNullOrWhiteSpace(request.Filename)
                ? "Documento.pdf"
                : request.Filename;

            var ok = await _whatsAppService.SendTemplateDocumentAsync(
                request.To,
                templateName,
                request.DocumentUrl,
                filename,
                request.LanguageCode ?? "es");

            return ok
                ? Ok(new { success = true, message = "Mensaje enviado por WhatsApp." })
                : BadRequest(new
                {
                    success = false,
                    message = "No se pudo enviar el documento por WhatsApp. Verifique configuración y plantillas Meta/Twilio."
                });
        }

        [HttpPost("send-appointment-reminder")]
        public async Task<IActionResult> SendAppointmentReminder([FromBody] WhatsAppAppointmentReminderRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.To))
                return BadRequest(new { success = false, message = "Teléfono requerido." });

            var mensaje =
                $"Recordatorio de cita\nPaciente: {request.PatientName}\nFecha: {request.AppointmentDate}";

            if (_settings.HasMetaCloudApi &&
                !string.IsNullOrWhiteSpace(_settings.TemplateRecordatorioCita))
            {
                var okTemplate = await _whatsAppService.SendTemplateTextAsync(
                    request.To,
                    _settings.TemplateRecordatorioCita,
                    new[] { request.PatientName ?? "", request.AppointmentDate ?? "" });
                if (okTemplate)
                    return Ok(new { success = true, message = "Recordatorio enviado por WhatsApp." });
            }

            var okText = await _whatsAppService.SendTextMessageAsync(request.To, mensaje);
            return okText
                ? Ok(new { success = true, message = "Recordatorio enviado por WhatsApp." })
                : BadRequest(new { success = false, message = "No se pudo enviar el recordatorio." });
        }
    }

    public class WhatsAppTextRequest
    {
        public string To { get; set; }
        public string Message { get; set; }
    }

    public class WhatsAppTemplateDocumentRequest
    {
        public string To { get; set; }
        public string DocumentUrl { get; set; }
        public string TemplateName { get; set; }
        public string Filename { get; set; }
        public string LanguageCode { get; set; }
    }

    public class WhatsAppAppointmentReminderRequest
    {
        public string To { get; set; }
        public string PatientName { get; set; }
        public string AppointmentDate { get; set; }
    }
}
