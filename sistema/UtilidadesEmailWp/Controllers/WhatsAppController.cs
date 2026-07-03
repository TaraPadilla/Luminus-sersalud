using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using sistema.UtilidadesEmailWp.Services.IService;

namespace farmamest.UtilidadesEmailWp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WhatsAppController : ControllerBase
    {
        private readonly IWhatsAppService _whatsAppService;

        public WhatsAppController(IWhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService;
        }

        [HttpPost("sendWhatsApp")]
        public async Task<IActionResult> SendWhatsApp([FromForm] WhatsAppRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.To) || string.IsNullOrWhiteSpace(request.Body))
                    return BadRequest("Teléfono y mensaje son requeridos.");

                var ok = await _whatsAppService.SendTextMessageAsync(request.To, request.Body);
                return ok
                    ? Ok("Mensaje enviado exitosamente.")
                    : StatusCode(StatusCodes.Status502BadGateway, "No se pudo enviar el mensaje de WhatsApp.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
    }

    public class WhatsAppRequest
    {
        public string Body { get; set; }
        public string To { get; set; }
        public IFormFile File { get; set; }
    }
}
