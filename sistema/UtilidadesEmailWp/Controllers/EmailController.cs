using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using farmamest.Service.IService;
using System;
using sistema.Service.IService;
using farmamest.Dtos;
using sistema.UtilidadesEmailWp.Services.IService;
using System.Collections.Generic;
using System.Linq;

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

            if (request.Attachments != null && request.Attachments.Count > 0)
            {
                foreach (var file in request.Attachments)
                {
                    if (file != null && file.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            attachmentsList.Add((file.FileName, memoryStream.ToArray()));
                        }
                    }
                }
            }

            // Compatibilidad: algunos clientes envían "Attachment" en singular
            if (attachmentsList.Count == 0 && Request.HasFormContentType)
            {
                var legacyFiles = Request.Form.Files.Where(f =>
                    string.Equals(f.Name, "Attachment", StringComparison.OrdinalIgnoreCase));
                foreach (var file in legacyFiles)
                {
                    if (file.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        attachmentsList.Add((file.FileName, memoryStream.ToArray()));
                    }
                }
            }

            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.To))
                    return BadRequest(new { error = "Destinatario (To) es obligatorio." });

                _messageService.SendEmail(request.Subject, request.Body, request.To, attachmentsList);
                return Ok("Email sent successfully.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Email no configurado: {ex.Message}");
                return StatusCode(503, new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"Error al enviar el correo: {detail}");
                return StatusCode(500, new { error = detail });
            }
        }
    }
}