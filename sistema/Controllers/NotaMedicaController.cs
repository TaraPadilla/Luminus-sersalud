using Database.Shared;
using Database.Shared.Models;
using farmamest.Models;
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
    public class NotaMedica2Controller : Controller
    {
        private readonly INotaMedica2Service _notaMedicaService;
        private readonly UserManager<User> _userManager;
        private readonly IWebAuthnService _webAuthn;
        private readonly Context _db;

        public NotaMedica2Controller(
            INotaMedica2Service notaMedicaService,
            UserManager<User> userManager,
            IWebAuthnService webAuthn,
            Context db)
        {
            _notaMedicaService = notaMedicaService;
            _userManager = userManager;
            _webAuthn = webAuthn;
            _db = db;
        }

        [HttpPost]
        public string ObtenerNotasMedicas(int idHospitalizacion)
        {
            try
            {
                var notasMedicas = _notaMedicaService.GetListByIdHospitalizacion(idHospitalizacion);
                return JsonSerializer.Serialize(new
                {
                    exitoso = true,
                    resultado = notasMedicas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    mensaje = "Error al obtener las notas médicas: " + ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<string> AgregarNotaMedica2()
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
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos de nota no encontrados." });

                var viewModel = JsonSerializer.Deserialize<NotaMedica2ViewModel>(entityElement.GetRawText(), opts);
                if (viewModel == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos de nota inválidos." });

                var hospitalizacionId = viewModel.HospitalizacionId;
                if (hospitalizacionId == 0 && root.TryGetProperty("hospitalizacionId", out var hospFallback))
                    hospitalizacionId = hospFallback.GetInt32();

                if (hospitalizacionId == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "HospitalizacionId inválido." });

                var notaMedica = new NotaMedica2
                {
                    Diagnostico = viewModel.Diagnostico,
                    FechaRegistro = DateTime.Now,
                    HistoriaProblema = viewModel.HistoriaProblema,
                    Sintomas = viewModel.Sintomas,
                    ProfesionalId = _userManager.GetUserId(HttpContext.User),
                    HospitalizacionId = hospitalizacionId,
                    Autorizado = false,
                    UsuarioAutoriza = null,
                    FechaAutorizacion = null,
                    TipoNota = string.IsNullOrWhiteSpace(viewModel.TipoNota) ? null : viewModel.TipoNota.Trim()
                };

                _notaMedicaService.Add(notaMedica);

                return JsonSerializer.Serialize(new { exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, resultado = ex.Message });
            }
        }

        [HttpPost]
        public async Task<string> AutorizarNotaMedica2()
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

                if (!root.TryGetProperty("notaMedicaId", out var idElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Id no encontrado." });
                var notaId = idElement.GetInt32();

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
                    ));
                var credencial = await _db.WebAuthnCredentials.FirstOrDefaultAsync(c => c.DescriptorId == credIdString);
                if (credencial == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Credencial no registrada." });

                var authorizerId = credencial.UserId;
                var verificacion = await _webAuthn.CompleteVerifyAsync(authorizerId, huellaPayload);
                if (!verificacion.Success)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = verificacion.UserMessage });

                // Obtener la nota
                var nota = _notaMedicaService.GetById(notaId);
                if (nota == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Nota no encontrada." });

                // Validar permisos (médico asignado o secundario)
                var authorizerUser = await _userManager.FindByIdAsync(authorizerId);
                var esAdmin = await _userManager.IsInRoleAsync(authorizerUser, "Administrador");
                if (!esAdmin)
                {
                    var citasId = await _db.Consultas
                        .Where(c => c.HospitalizacionId == nota.HospitalizacionId)
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
                nota.Autorizado = true;
                nota.UsuarioAutoriza = authorizerId;
                nota.FechaAutorizacion = DateTime.Now;
                _notaMedicaService.Update(nota);

                return JsonSerializer.Serialize(new { exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, resultado = ex.Message });
            }
        }
        [HttpPost]
        public async Task<string> ActualizarNotaMedica2([FromBody] ActualizarNotaMedica2VM model)
        {
            try
            {
                var nota = _notaMedicaService.GetById(model.Id);
                if (nota == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Nota no encontrada." });

                if (nota.Autorizado)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "No se puede editar una nota ya autorizada." });

                nota.Diagnostico = model.Diagnostico;
                if (!string.IsNullOrWhiteSpace(model.TipoNota))
                    nota.TipoNota = model.TipoNota.Trim();
                _notaMedicaService.Update(nota);

                return JsonSerializer.Serialize(new { exitoso = true, resultado = "Nota actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, resultado = "Error: " + ex.Message });
            }
        }

        public class ActualizarNotaMedica2VM
        {
            public int Id { get; set; }
            public string Diagnostico { get; set; }
            public string TipoNota { get; set; }
        }
    }
}