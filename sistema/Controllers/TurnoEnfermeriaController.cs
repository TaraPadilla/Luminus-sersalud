using Database.Shared;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema.Services.WebAuthn;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace farmamest.Controllers
{
    public class TurnoEnfermeriaController : Controller
    {
        private readonly ITurnoEnfermeriaService _turnoEnfermeriaService;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IUser _usuarioRepository;
        private readonly IEmpleado _empleadoRepository;
        private readonly IWebAuthnService _webAuthn;
        private readonly Context _db;

        public TurnoEnfermeriaController(
            ITurnoEnfermeriaService turnoEnfermeriaService,
            UserManager<User> userManager,
            IWebHostEnvironment env,
            IUser usuarioRepository,
            IEmpleado empleadoRepository,
            IWebAuthnService webAuthn,
            Context db)
        {
            _turnoEnfermeriaService = turnoEnfermeriaService;
            _userManager = userManager;
            _env = env;
            _usuarioRepository = usuarioRepository;
            _empleadoRepository = empleadoRepository;
            _webAuthn = webAuthn;
            _db = db;
        }

        [HttpPost]
        public string Nuevo(TurnoEnfermeria entity)
        {
            try
            {
                entity.UserId = _userManager.GetUserId(HttpContext.User);
                _turnoEnfermeriaService.AddTurnoEnfermeria(entity);
                return JsonSerializer.Serialize(new { exitoso = true, resultado = true });
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new { exitoso = false, resultado = "Error al crear un turno de enfermería" });
            }
        }

        [HttpPost]
        public string ListaTurnoEnfermeria(int hospitalizacionId)
        {
            try
            {
                var turnos = _turnoEnfermeriaService.GetTurnosByHospitalizacionId(hospitalizacionId);

                // Resolver nombres desde _userManager sin depender de propiedad de navegación
                var userIds = turnos.Select(t => t.UserId).Distinct().ToList();
                var usuarios = _userManager.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.UserName })
                    .ToList();

                var resultado = turnos.Select(t => new
                {
                    t.Id,
                    t.FechaRegistro,
                    t.NumeroTurno,
                    t.NombreTurno,
                    t.HospitalizacionId,
                    t.UserId,
                    t.Firmado,
                    FirmaRuta = t.FirmaRuta ?? "",
                    Profesional = usuarios.FirstOrDefault(u => u.Id == t.UserId)?.UserName ?? ""
                }).ToList();

                var opts = new JsonSerializerOptions { PropertyNamingPolicy = null };
                return JsonSerializer.Serialize(new { exitoso = true, resultado = resultado }, opts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, mensaje = "Error al obtener los turnos: " + ex.Message });
            }
        }

        [HttpPost]
        public string FirmarTurno(int turnoId)
        {
            try
            {
                _turnoEnfermeriaService.MarkTurnoAsFirmado(turnoId);
                return JsonSerializer.Serialize(new { exitoso = true, resultado = "Turno firmado exitosamente" });
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new { exitoso = false, mensaje = "Error al firmar el turno" });
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
        public async Task<IActionResult> FirmarTurnoEnfermeria([FromBody] FirmarTurnoRequest request)
        {
            try
            {
                if (request?.HuellaPayload == null)
                    return Json(new { exitoso = false, resultado = "Payload de huella no encontrado." });

                // 1. Identificar credencial
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

                // 2. Verificar huella criptográfica
                var verificacion = await _webAuthn.CompleteVerifyAsync(authorizerId, request.HuellaPayload);
                if (!verificacion.Success)
                    return Json(new { exitoso = false, resultado = verificacion.UserMessage });

                // 3. Obtener el turno
                var turno = await _db.TurnoEnfermeria.FindAsync(request.TurnoId);
                if (turno == null)
                    return Json(new { exitoso = false, resultado = "Turno no encontrado." });

                if (turno.Firmado)
                    return Json(new { exitoso = false, resultado = "El turno ya está firmado." });

                // 4. Procesar imagen de firma
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

                // 5. Guardar en BD
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

        public class FirmarTurnoRequest
        {
            public int TurnoId { get; set; }
            public string FirmaBase64 { get; set; }
            public string FirmaExistenteUrl { get; set; }
            public WebAuthnAssertionPayload HuellaPayload { get; set; }
        }
    }
}