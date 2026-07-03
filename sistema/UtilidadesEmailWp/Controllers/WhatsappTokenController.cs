using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using farmamest.UtilidadesEmailWp;

namespace farmamest.UtilidadesEmailWp.Controllers
{
    /// <summary>
    /// Endpoint legacy. Preferir <see cref="WhatsAppMessagingController"/>.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WhatsappTokenController : ControllerBase
    {
        private readonly WhatsAppSettings _whatsAppSettings;

        public WhatsappTokenController(IOptions<WhatsAppSettings> options)
        {
            _whatsAppSettings = options.Value;
        }

        [HttpGet]
        public ActionResult<string> GetToken()
        {
            if (!_whatsAppSettings.Enabled)
                return BadRequest("WhatsApp deshabilitado.");

            var token = _whatsAppSettings.ResolveMetaAccessToken();
            if (string.IsNullOrWhiteSpace(token))
                return NotFound("Token Meta no configurado. Use WhatsAppSettings:MetaAccessToken.");

            return token;
        }
    }
}
