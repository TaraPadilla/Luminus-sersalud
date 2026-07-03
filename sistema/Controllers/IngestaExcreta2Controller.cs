using Database.Shared;
using Database.Shared.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema.Services.WebAuthn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace farmamest.Controllers
{
    public class IngestaExcreta2Controller : Controller
    {
        private readonly IIngestaExcretaService _ingestaExcretaService;
        private readonly UserManager<User> _userManager;
        private readonly IWebAuthnService _webAuthn;
        private readonly Context _db;

        public IngestaExcreta2Controller(
            IIngestaExcretaService ingestaExcretaService,
            UserManager<User> userManager,
            IWebAuthnService webAuthn,
            Context db)
        {
            _ingestaExcretaService = ingestaExcretaService;
            _userManager = userManager;
            _webAuthn = webAuthn;
            _db = db;
        }

        [HttpPost]
        public async Task<string> Nuevo()
        {
            try
            {
                string rawBody;
                using (var reader = new StreamReader(Request.Body))
                    rawBody = await reader.ReadToEndAsync();

                if (string.IsNullOrEmpty(rawBody))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Body vacío." });

                var opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                };

                using var doc = JsonDocument.Parse(rawBody);
                var root = doc.RootElement;

                // Extraer entity
                if (!root.TryGetProperty("entity", out var entityElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos de ingesta no encontrados." });

                var entity = JsonSerializer.Deserialize<IngestaExcreta2>(entityElement.GetRawText(), opts);

                if (entity == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos de ingesta inválidos." });

                if (entity.HospitalizacionId == 0)
                {
                    if (root.TryGetProperty("hospitalizacionId", out var hospFallback))
                        entity.HospitalizacionId = hospFallback.GetInt32();
                }

                if (entity.HospitalizacionId == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "HospitalizacionId inválido." });

                // Guardar como pendiente (sin autorización)
                entity.UserId = _userManager.GetUserId(HttpContext.User);
                entity.Autorizado = false;
                entity.UsuarioAutoriza = null;
                entity.FechaAutorizacion = null;
                entity.Hospitalizacion = null;
                entity.User = null;

                _ingestaExcretaService.Add(entity);

                return JsonSerializer.Serialize(new { exitoso = true, resultado = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                });
            }
        }
        [HttpPost]
        public string ListaIngestaExcreta(int hospitalizacionId)
        {
            try
            {
                var result = _ingestaExcretaService.GetListByHospitalizacionId(hospitalizacionId);
                return JsonSerializer.Serialize(new { exitoso = true, resultado = result });
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new { exitoso = true, mensaje = "Error al obtener las ingesta/Excreta" });
            }
        }


        [HttpPost]
        public async Task<string> AutorizarIngestaExcreta()
        {
            try
            {
                var opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                };

                string rawBody;
                using (var reader = new StreamReader(Request.Body))
                    rawBody = await reader.ReadToEndAsync();

                if (string.IsNullOrEmpty(rawBody))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Body vacío." });

                using var doc = JsonDocument.Parse(rawBody);
                var root = doc.RootElement;

                // Extraer ingestaExcretaId
                if (!root.TryGetProperty("ingestaExcretaId", out var idElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Id no encontrado." });
                var ingestaExcretaId = idElement.GetInt32();

                // Extraer huellaPayload
                if (!root.TryGetProperty("huellaPayload", out var huellaElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Payload de huella no encontrado." });

                var huellaPayload = JsonSerializer.Deserialize<WebAuthnAssertionPayload>(
                    huellaElement.GetRawText(), opts);

                if (huellaPayload == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Payload de huella inválido." });

                // Identificar credencial
                var credIdString = Convert.ToBase64String(
                    Convert.FromBase64String(
                        huellaPayload.RawId.Replace('-', '+').Replace('_', '/')
                        + new string('=', (4 - huellaPayload.RawId.Length % 4) % 4)
                    )
                );

                var credencial = await _db.WebAuthnCredentials
                    .FirstOrDefaultAsync(c => c.DescriptorId == credIdString);

                if (credencial == null)
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        resultado = "La credencial presentada no está registrada en el sistema."
                    });

                var authorizerId = credencial.UserId;

                // Verificar firma
                var verificacion = await _webAuthn.CompleteVerifyAsync(authorizerId, huellaPayload);

                if (!verificacion.Success)
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        resultado = verificacion.UserMessage,
                        errorCode = verificacion.ErrorCode?.ToString()
                    });

                // Obtener el registro de ingesta/excreta
                var ingesta = _ingestaExcretaService.GetById(ingestaExcretaId); // asegúrate de tener este método
                if (ingesta == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Registro no encontrado." });

                // Validar permisos (mismo que en Nuevo)
                var authorizerUser = await _userManager.FindByIdAsync(authorizerId);
                var esAdmin = await _userManager.IsInRoleAsync(authorizerUser, "Administrador");

                if (!esAdmin)
                {
                    var citasId = await _db.Consultas
                        .Where(c => c.HospitalizacionId == ingesta.HospitalizacionId)
                        .Select(c => c.CitasId)
                        .FirstOrDefaultAsync();

                    if (citasId == null)
                        return JsonSerializer.Serialize(new
                        {
                            exitoso = false,
                            resultado = "No se encontró la consulta de esta hospitalización."
                        });

                    var cita = await _db.Citass
                        .Where(c => c.Id == citasId.Value)
                        .Select(c => new { c.EmpleadoId, c.MedicosSecundarios })
                        .FirstOrDefaultAsync();

                    if (cita == null)
                        return JsonSerializer.Serialize(new
                        {
                            exitoso = false,
                            resultado = "No se encontró la cita de esta hospitalización."
                        });

                    var empleadosAutorizados = new List<int>();
                    if (cita.EmpleadoId.HasValue)
                        empleadosAutorizados.Add(cita.EmpleadoId.Value);
                    if (cita.MedicosSecundarios != null)
                        empleadosAutorizados.AddRange(cita.MedicosSecundarios);

                    var userIdsAutorizados = await _db.Usuarios
                        .Where(u => u.EmpleadoId != null
                                 && empleadosAutorizados.Contains(u.EmpleadoId.Value))
                        .Select(u => u.Id)
                        .ToListAsync();

                    if (!userIdsAutorizados.Contains(authorizerId))
                        return JsonSerializer.Serialize(new
                        {
                            exitoso = false,
                            resultado = "El autorizador no es el médico asignado ni un médico secundario de esta hospitalización."
                        });
                }

                // Actualizar el registro
                ingesta.Autorizado = true;
                ingesta.UsuarioAutoriza = authorizerId;
                ingesta.FechaAutorizacion = DateTime.Now;
                _ingestaExcretaService.Update(ingesta);

                return JsonSerializer.Serialize(new { exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                });
            }
        }
    }
}