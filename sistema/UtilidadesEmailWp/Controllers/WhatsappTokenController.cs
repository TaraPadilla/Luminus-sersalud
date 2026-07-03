using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using farmamest.Models;
using farmamest.UtilidadesEmailWp;
namespace farmamest.UtilidadesEmailWp.Controllers
{
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
            return _whatsAppSettings.Token;
        }
    }
}
