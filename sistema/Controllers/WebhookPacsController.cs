// Sistema/Controllers/WebhookPacsController.cs
using Database.Shared;
using Database.Shared.Models;
using farmamest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Sistema.Controllers
{
    [Route("api/pacs")]
    [ApiController]
    public class WebhookPacsController : ControllerBase
    {
        private readonly string _authToken;
        private readonly Context _context;

        public WebhookPacsController(IConfiguration configuration, Context context)
        {
            _authToken = configuration["WebhookEden:Token"];
            _context = context;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveNotification(
            [FromBody] WebhookNotification notification,
            [FromHeader(Name = "X-Custom-Auth-Token")] string authToken)
        {
            if (authToken != _authToken)
            {
                return Unauthorized();
            }

            if (notification?.Data == null)
            {
                return BadRequest("El campo 'data' es requerido.");
            }

            // Verificar que el ID del examen está en el campo correcto
            if (!int.TryParse(notification.Id, out int examId))
            {
                return BadRequest("El ID del examen no es válido.");
            }

            // Convertir el dynamic Data a un objeto JObject para acceder a las propiedades
            var data = (dynamic)notification.Data;

            string status = data.status;
            string pdfUrl = data.reports != null ? data.reports[0].pdf_url : null; // Acceder a pdf_url

            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("El campo 'status' es requerido y debe ser una cadena dentro del objeto 'data'.");
            }

            // Buscar el examen correspondiente y actualizar el estado
            var exam = await _context.Examenes.FindAsync(examId);

            if (exam != null)
            {
                exam.EstadoEstudio = status;
                exam.UrlEstudio = pdfUrl; // Guardar la URL del PDF

                _context.Examenes.Update(exam);
            }
            else
            {
                return NotFound($"No se encontró ningún examen con el ID: {examId}");
            }

            await _context.SaveChangesAsync();

            // Procesar la notificación y obtener la respuesta
            var responseData = ProcessNotification(notification);

            return Ok(new
            {
                message = "Notificación recibida y estado actualizado con éxito",
                data = responseData
            });
        }


        private object ProcessNotification(WebhookNotification notification)
        {
            // Retornar los datos de la notificación para la respuesta
            return new
            {
                Id = notification.Id,
                EventType = notification.EventType,
                CreatedAt = notification.CreatedAt,
                UpdatedAt = notification.UpdatedAt,
                Data = notification.Data
            };
        }

    }
}