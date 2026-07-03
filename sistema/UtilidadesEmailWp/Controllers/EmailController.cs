using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using farmamest.Service.IService;
using System;
using sistema.Service.IService;
using farmamest.Dtos;
using sistema.UtilidadesEmailWp.Services.IService;
using System.Collections.Generic;

namespace farmamest.UtilidadesEmailWp.Controllers
{
    [ApiController]
    [Route("api")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _messageService;

        public EmailController(IEmailService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("sendEmail")]
        public IActionResult GetStatus()
        {
            return Ok(new { message = "El servicio de Email está encendido." });
        }

        [HttpPost("sendEmail")]
        public async Task<IActionResult> SendEmail([FromForm] SendEmailRequest request)
        {
            // Creamos la lista que enviaremos al servicio
            var attachmentsList = new List<(string FileName, byte[] Data)>();

            // Recorremos los archivos si es que vienen en el request
            if (request.Attachments != null && request.Attachments.Count > 0)
            {
                foreach (var file in request.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            attachmentsList.Add((file.FileName, memoryStream.ToArray()));
                        }
                    }
                }
            }

            try
            {
                _messageService.SendEmail(request.Subject, request.Body, request.To, attachmentsList);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                return StatusCode(500, new { error = $"Error sending email: {ex.Message}" });
            }
        }
    }
}