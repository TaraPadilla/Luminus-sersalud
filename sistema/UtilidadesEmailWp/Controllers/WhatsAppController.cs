using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using sistema.UtilidadesEmailWp.Services.IService;

namespace farmamest.UtilidadesEmailWp.Controllers
{
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
                //using (var fileStream = request.File.OpenReadStream())
                //{
                //    await _whatsAppService.SendMessageAsync(request.To, request.Body, fileStream);
                //}
                return Ok("Mensaje enviado exitosamente.");
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
