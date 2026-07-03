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
    public class ControlGlucometriaController : Controller
    {
        private readonly IControlGlucometria2Service _controlGlucometria2Service;
        private readonly UserManager<User> _userManager;
        private readonly IWebAuthnService _webAuthn;
        private readonly Context _db;

        public ControlGlucometriaController(
            IControlGlucometria2Service controlGlucometria2Service,
            UserManager<User> userManager,
            IWebAuthnService webAuthn,
            Context db)
        {
            _controlGlucometria2Service = controlGlucometria2Service;
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

                if (!root.TryGetProperty("entity", out var entityElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos de glucometría no encontrados." });

                var entity = JsonSerializer.Deserialize<ControlGlucometria2>(entityElement.GetRawText(), opts);

                if (entity == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos de glucometría inválidos." });

                if (entity.HospitalizacionId == 0 && root.TryGetProperty("hospitalizacionId", out var hospElement))
                    entity.HospitalizacionId = hospElement.GetInt32();

                if (entity.HospitalizacionId == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "HospitalizacionId inválido." });

                // Asignar usuario que crea (el actual)
                // entity.UserId = _userManager.GetUserId(HttpContext.User);
                entity.Autorizado = false;
                entity.UsuarioAutoriza = null;
                entity.FechaAutorizacion = null;

                // Crear los detalles según la dosis
                var detalles = new List<DetalleControlGlucometria2>();
                for (int i = 0; i < entity.Dosis; i++)
                {
                    detalles.Add(new DetalleControlGlucometria2
                    {
                        Aplicado = false,
                        Eliminado = false,
                        FechaAplicacion = null,
                        // ProfesionalId = entity.UserId,
                        UserId = null
                    });
                }
                entity.DetalleControlGlucometria2 = detalles;

                _controlGlucometria2Service.Add(entity);

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
        public string ListaControlGlucometria2(int hospitalizacionId)
        {
            try
            {
                var result = _controlGlucometria2Service.GetByHospitalizacionId(hospitalizacionId);
                return JsonSerializer.Serialize(new { exitoso = true, resultado = result });
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = true,
                    mensaje = "Error al obtener los controles de glucometria"
                });
            }
        }

        [HttpPost]
        public string AplicacionControlGlucometria2(int id)
        {
            try
            {
                string personaAplica = _userManager.GetUserId(HttpContext.User);
                _controlGlucometria2Service.AplicacionDetalleControlGlucometria2ById(id, personaAplica);
                return JsonSerializer.Serialize(new { exitoso = true, mensaje = "Se ha aplicado con exito" });
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = true,
                    mensaje = "Error al aplicar control glucometria"
                });
            }
        }


        [HttpPost]
        public async Task<string> AutorizarControlGlucometria()
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

                using var doc = JsonDocument.Parse(rawBody);
                var root = doc.RootElement;

                if (!root.TryGetProperty("controlGlucometriaId", out var idElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Id no encontrado." });
                var controlId = idElement.GetInt32();

                if (!root.TryGetProperty("huellaPayload", out var huellaElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Payload de huella no encontrado." });

                var huellaPayload = JsonSerializer.Deserialize<WebAuthnAssertionPayload>(huellaElement.GetRawText(), opts);
                if (huellaPayload == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Payload inválido." });

                // Verificar credencial
                var credIdString = Convert.ToBase64String(
                    Convert.FromBase64String(
                        huellaPayload.RawId.Replace('-', '+').Replace('_', '/')
                        + new string('=', (4 - huellaPayload.RawId.Length % 4) % 4)
                    )
                );
                var credencial = await _db.WebAuthnCredentials.FirstOrDefaultAsync(c => c.DescriptorId == credIdString);
                if (credencial == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Credencial no registrada." });

                var authorizerId = credencial.UserId;
                var verificacion = await _webAuthn.CompleteVerifyAsync(authorizerId, huellaPayload);
                if (!verificacion.Success)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = verificacion.UserMessage });

                // Obtener el registro
                var control = _controlGlucometria2Service.GetById(controlId);
                if (control == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Registro no encontrado." });

                // Validar permisos (médico asignado o secundario)
                var authorizerUser = await _userManager.FindByIdAsync(authorizerId);
                var esAdmin = await _userManager.IsInRoleAsync(authorizerUser, "Administrador");
                if (!esAdmin)
                {
                    var citasId = await _db.Consultas
                        .Where(c => c.HospitalizacionId == control.HospitalizacionId)
                        .Select(c => c.CitasId)
                        .FirstOrDefaultAsync();
                    if (citasId == null)
                        return JsonSerializer.Serialize(new { exitoso = false, resultado = "No se encontró la consulta." });

                    var cita = await _db.Citass
                        .Where(c => c.Id == citasId.Value)
                        .Select(c => new { c.EmpleadoId, c.MedicosSecundarios })
                        .FirstOrDefaultAsync();
                    if (cita == null)
                        return JsonSerializer.Serialize(new { exitoso = false, resultado = "No se encontró la cita." });

                    var empleadosAutorizados = new List<int>();
                    if (cita.EmpleadoId.HasValue) empleadosAutorizados.Add(cita.EmpleadoId.Value);
                    if (cita.MedicosSecundarios != null) empleadosAutorizados.AddRange(cita.MedicosSecundarios);

                    var userIdsAutorizados = await _db.Usuarios
                        .Where(u => u.EmpleadoId != null && empleadosAutorizados.Contains(u.EmpleadoId.Value))
                        .Select(u => u.Id)
                        .ToListAsync();

                    if (!userIdsAutorizados.Contains(authorizerId))
                        return JsonSerializer.Serialize(new { exitoso = false, resultado = "Usuario no autorizado." });
                }

                // Actualizar estado
                control.Autorizado = true;
                control.UsuarioAutoriza = authorizerId;
                control.FechaAutorizacion = DateTime.Now;
                control.Firma = authorizerUser?.Persona?.NombreYApellidos
                    ?? authorizerUser?.UserName
                    ?? "Autorizado";
                _controlGlucometria2Service.Update(control);

                return JsonSerializer.Serialize(new { exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, resultado = ex.Message });
            }
        }
    }
}