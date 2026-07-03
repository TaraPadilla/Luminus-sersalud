using Database.Shared;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Service.IService;
using farmamest.Utilidades;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistema.Controllers;
using Sistema.Services.WebAuthn;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace farmamest.Controllers
{
    public class NotaEnfermeriaController : Controller
    {

        private readonly INotaEnfermeria2Service _notaEnfermeriaService;

        private readonly UserManager<User> _userManager;

        private readonly IWebHostEnvironment _env;
        private readonly IUser _usuarioRepository;
        private readonly IEmpleado _empleadoRepository;
        private readonly IWebAuthnService _webAuthn;
        private readonly Context _db;

        public NotaEnfermeriaController(
        INotaEnfermeria2Service notaEnfermeriaService,
        UserManager<User> userManager,
        IWebHostEnvironment env,
        IUser usuarioRepository,
        IEmpleado empleadoRepository,
        IWebAuthnService webAuthn,
        Context db)
        {
            _notaEnfermeriaService = notaEnfermeriaService;
            _userManager = userManager;
            _env = env;
            _usuarioRepository = usuarioRepository;
            _empleadoRepository = empleadoRepository;
            _webAuthn = webAuthn;
            _db = db;
        }
        [HttpPost]
        public string Nuevo(NotaEnfermeria2 entity)
        {

            try
            {
                entity.UserId = _userManager.GetUserId(HttpContext.User);

                _notaEnfermeriaService.AddNotaEnfermeria(entity);

                return JsonSerializer.Serialize(new
                {
                    exitoso = true,
                    resultado = true

                });
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = true,
                    resultado = "Error al crear una nota de enfermeria"

                });
            }

        }

        [HttpPost]
        public string ListaNotaEnfermeria(int hospitalizacionId)
        {
            try
            {
                var result = _notaEnfermeriaService.GetNotaEnfermeriaListByHospitalizacionId(hospitalizacionId);
                var userIds = result.Select(n => n.UserId).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
                var usuarios = _db.Users
                    .Include(u => u.Persona)
                    .Where(u => userIds.Contains(u.Id))
                    .ToList();

                foreach (var nota in result)
                {
                    if (string.IsNullOrWhiteSpace(nota.UserId)) continue;
                    var user = usuarios.FirstOrDefault(u => u.Id == nota.UserId);
                    var nombreEmpleado = PdfReportHelper.ObtenerNombreEmpleadoPorUser(user, _empleadoRepository);
                    if (!string.IsNullOrWhiteSpace(nombreEmpleado) && nombreEmpleado != "-")
                        nota.Profesional = nombreEmpleado;
                }

                return JsonSerializer.Serialize(new { exitoso = true, resultado = result });
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new { exitoso = false, mensaje = "Error al obtener las notas de enfermeria" });
            }
        }

        [HttpGet]
        public IActionResult ObtenerFirmaEmpleado()
        {
            try
            {
                var username = User.Identity?.Name;
                var usuario = _usuarioRepository.Get(username);
                if (usuario?.EmpleadoId == null)
                    return Json(new { exitoso = false, firmaUrl = (string)null });

                var empleado = _empleadoRepository.Get(usuario.EmpleadoId.Value);
                if (empleado == null || string.IsNullOrWhiteSpace(empleado.FirmaEmpleado))
                    return Json(new { exitoso = false, firmaUrl = (string)null });

                return Json(new { exitoso = true, firmaUrl = empleado.FirmaEmpleado });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, firmaUrl = (string)null, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> FirmarNotaEnfermeria()
        {
            try
            {
                string rawBody;
                using (var reader = new StreamReader(Request.Body))
                    rawBody = await reader.ReadToEndAsync();

                if (string.IsNullOrEmpty(rawBody))
                    return Json(new { exitoso = false, resultado = "Body vacío." });

                var opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                using var doc = JsonDocument.Parse(rawBody);
                var root = doc.RootElement;

                // 1. Extraer huellaPayload
                if (!root.TryGetProperty("huellaPayload", out var huellaElement))
                    return Json(new { exitoso = false, resultado = "Payload de huella no encontrado." });

                var huellaPayload = JsonSerializer.Deserialize<WebAuthnAssertionPayload>(
                    huellaElement.GetRawText(), opts);

                if (huellaPayload == null)
                    return Json(new { exitoso = false, resultado = "Payload de huella inválido." });

                // 2. Identificar credencial
                var credIdString = Convert.ToBase64String(
                    Convert.FromBase64String(
                        huellaPayload.RawId.Replace('-', '+').Replace('_', '/')
                        + new string('=', (4 - huellaPayload.RawId.Length % 4) % 4)
                    ));

                var credencial = await _db.WebAuthnCredentials
                    .FirstOrDefaultAsync(c => c.DescriptorId == credIdString);

                if (credencial == null)
                    return Json(new { exitoso = false, resultado = "Credencial no registrada." });

                var authorizerId = credencial.UserId;

                // 3. Verificar firma criptográfica
                var verificacion = await _webAuthn.CompleteVerifyAsync(authorizerId, huellaPayload);
                if (!verificacion.Success)
                    return Json(new { exitoso = false, resultado = verificacion.UserMessage });

                // 4. Extraer modelo de firma
                if (!root.TryGetProperty("model", out var modelElement))
                    return Json(new { exitoso = false, resultado = "Datos de firma no encontrados." });

                var model = JsonSerializer.Deserialize<FirmarNotaEnfermeriaInputVM>(
                    modelElement.GetRawText(), opts);

                if (model == null || model.NotaEnfermeriaId == 0)
                    return Json(new { exitoso = false, resultado = "Datos de firma inválidos." });

                // 5. Obtener la nota
                var nota = await _db.NotaEnfermeria2.FindAsync(model.NotaEnfermeriaId);
                if (nota == null)
                    return Json(new { exitoso = false, resultado = "Nota no encontrada." });

                if (nota.Firmado)
                    return Json(new { exitoso = false, resultado = "La nota ya está firmada." });

                // 6. Procesar la firma (imagen)
                string rutaFinal;
                if (!string.IsNullOrEmpty(model.FirmaExistenteUrl))
                {
                    rutaFinal = model.FirmaExistenteUrl;
                }
                else if (!string.IsNullOrEmpty(model.FirmaBase64))
                {
                    var userId = _userManager.GetUserId(User);
                    var carpeta = Path.Combine(_env.WebRootPath, "Firmas", "NotasEnfermeria");
                    Directory.CreateDirectory(carpeta);

                    var nombreArchivo = $"nota_enfermeria_{model.NotaEnfermeriaId}_{userId}_{DateTime.Now:yyyyMMddHHmmss}.png";
                    var rutaAbsoluta = Path.Combine(carpeta, nombreArchivo);

                    var base64Data = model.FirmaBase64.Contains(",")
                        ? model.FirmaBase64.Split(',')[1]
                        : model.FirmaBase64;

                    await System.IO.File.WriteAllBytesAsync(rutaAbsoluta, Convert.FromBase64String(base64Data));
                    rutaFinal = $"/Firmas/NotasEnfermeria/{nombreArchivo}";
                }
                else
                {
                    return Json(new { exitoso = false, resultado = "No se proporcionó firma." });
                }

                // 7. Guardar en la nota
                nota.FirmaRuta = rutaFinal;
                nota.Firmado = true;
                nota.FechaFirma = DateTime.Now;
                nota.UsuarioFirmaId = authorizerId;
                await _db.SaveChangesAsync();

                return Json(new { exitoso = true, firmaRuta = rutaFinal });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, resultado = "Error: " + ex.Message });
            }
        }

        public class FirmarNotaEnfermeriaInputVM
        {
            public int NotaEnfermeriaId { get; set; }
            public string FirmaBase64 { get; set; }
            public string FirmaExistenteUrl { get; set; }
        }



        [HttpPost]
        public async Task<IActionResult> FirmarTurnoEnfermeria([FromBody] FirmarTurnoRequest request)
        {
            try
            {
                // 1. Validar payload de huella
                if (request?.HuellaPayload == null)
                    return Json(new { exitoso = false, resultado = "Payload de huella no encontrado." });

                // 2. Identificar credencial
                var credIdString = Convert.ToBase64String(
                    Convert.FromBase64String(
                        request.HuellaPayload.RawId.Replace('-', '+').Replace('_', '/')
                        + new string('=', (4 - request.HuellaPayload.RawId.Length % 4) % 4)
                    ));
                var credencial = await _db.WebAuthnCredentials
                    .FirstOrDefaultAsync(c => c.DescriptorId == credIdString);
                if (credencial == null)
                    return Json(new { exitoso = false, resultado = "Credencial no registrada." });

                var authorizerId = credencial.UserId;

                // 3. Verificar firma criptográfica
                var verificacion = await _webAuthn.CompleteVerifyAsync(authorizerId, request.HuellaPayload);
                if (!verificacion.Success)
                    return Json(new { exitoso = false, resultado = verificacion.UserMessage });

                // 4. Obtener el turno
                var turno = await _db.TurnoEnfermeria.FindAsync(request.TurnoId);
                if (turno == null)
                    return Json(new { exitoso = false, resultado = "Turno no encontrado." });

                if (turno.Firmado)
                    return Json(new { exitoso = false, resultado = "El turno ya está firmado." });

                // 5. Procesar la firma (dibujada o existente)
                string rutaFinal;
                if (!string.IsNullOrEmpty(request.FirmaExistenteUrl))
                {
                    rutaFinal = request.FirmaExistenteUrl;
                }
                else if (!string.IsNullOrEmpty(request.FirmaBase64))
                {
                    var userId = _userManager.GetUserId(User);
                    var carpeta = Path.Combine(_env.WebRootPath, "Firmas", "TurnosEnfermeria");
                    Directory.CreateDirectory(carpeta);

                    var nombreArchivo = $"turno_{request.TurnoId}_{userId}_{DateTime.Now:yyyyMMddHHmmss}.png";
                    var rutaAbsoluta = Path.Combine(carpeta, nombreArchivo);

                    var base64Data = request.FirmaBase64.Contains(",")
                        ? request.FirmaBase64.Split(',')[1]
                        : request.FirmaBase64;

                    await System.IO.File.WriteAllBytesAsync(rutaAbsoluta, Convert.FromBase64String(base64Data));
                    rutaFinal = $"/Firmas/TurnosEnfermeria/{nombreArchivo}";
                }
                else
                {
                    return Json(new { exitoso = false, resultado = "No se proporcionó firma." });
                }

                // 6. Actualizar turno
                turno.Firmado = true;
                turno.FirmaRuta = rutaFinal;
                turno.FechaFirma = DateTime.Now;
                turno.UsuarioFirmaId = authorizerId;
                await _db.SaveChangesAsync();

                return Json(new { exitoso = true, firmaRuta = rutaFinal });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, resultado = "Error: " + ex.Message });
            }
        }

        // DTO para la solicitud
        public class FirmarTurnoRequest
        {
            public int TurnoId { get; set; }
            public string FirmaBase64 { get; set; }
            public string FirmaExistenteUrl { get; set; }
            public WebAuthnAssertionPayload HuellaPayload { get; set; }
        }


        [HttpPost]
        public async Task<string> ActualizarNotaEnfermeria([FromBody] ActualizarNotaEnfermeriaVM model)
        {
            try
            {
                var nota = await _db.NotaEnfermeria2.FindAsync(model.Id);
                if (nota == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Nota no encontrada." });

                if (nota.Firmado)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "No se puede editar una nota firmada." });

                nota.Diagnostico = model.Diagnostico;
                if (!string.IsNullOrWhiteSpace(model.TipoNota))
                    nota.TipoNota = model.TipoNota.Trim();
                nota.FechaRegistro = DateTime.Now; 
                await _db.SaveChangesAsync();

                return JsonSerializer.Serialize(new { exitoso = true, resultado = "Nota actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, resultado = "Error: " + ex.Message });
            }
        }

        // ViewModel auxiliar
        public class ActualizarNotaEnfermeriaVM
        {
            public int Id { get; set; }
            public string Diagnostico { get; set; }
            public int HospitalizacionId { get; set; }
            public string TipoNota { get; set; }
        }

    }


}
