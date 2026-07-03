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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace farmamest.Controllers
{
    public class NotaOperatoriaController : Controller
    {
        private readonly INotaOperatoriaService _notaOperatoriaService;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IUser _usuarioRepository;
        private readonly IEmpleado _empleadoRepository;
        private readonly IWebAuthnService _webAuthn;
        private readonly Context _db;

        public NotaOperatoriaController(
            INotaOperatoriaService notaOperatoriaService,
            UserManager<User> userManager,
            IWebHostEnvironment env,
            IUser usuarioRepository,
            IEmpleado empleadoRepository,
            IWebAuthnService webAuthn,
            Context db)
        {
            _notaOperatoriaService = notaOperatoriaService;
            _userManager = userManager;
            _env = env;
            _usuarioRepository = usuarioRepository;
            _empleadoRepository = empleadoRepository;
            _webAuthn = webAuthn;
            _db = db;
        }

        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNameCaseInsensitive = true
        };

        // Nuevo método para agregar con huella (se llamará desde el frontend)
        [HttpPost]
        public async Task<string> AgregarNotaOperatoriaConHuella()
        {
            try
            {
                string rawBody;
                using (var reader = new StreamReader(Request.Body))
                    rawBody = await reader.ReadToEndAsync();

                if (string.IsNullOrEmpty(rawBody))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Body vacío." }, _jsonOpts);

                using var doc = JsonDocument.Parse(rawBody);
                var root = doc.RootElement;

                // 1. Extraer huellaPayload
                if (!root.TryGetProperty("huellaPayload", out var huellaElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Payload de huella no encontrado." }, _jsonOpts);

                var huellaPayload = JsonSerializer.Deserialize<WebAuthnAssertionPayload>(
                    huellaElement.GetRawText(), _jsonOpts);

                if (huellaPayload == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Payload de huella inválido." }, _jsonOpts);

                // 2. Identificar al autorizador por credentialId
                var credIdString = Convert.ToBase64String(
                    Convert.FromBase64String(
                        huellaPayload.RawId.Replace('-', '+').Replace('_', '/')
                        + new string('=', (4 - huellaPayload.RawId.Length % 4) % 4)
                    ));

                var credencial = await _db.WebAuthnCredentials
                    .FirstOrDefaultAsync(c => c.DescriptorId == credIdString);

                if (credencial == null)
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        resultado = "La credencial presentada no está registrada en el sistema."
                    }, _jsonOpts);

                var authorizerId = credencial.UserId;

                // 3. Verificar la firma
                var verificacion = await _webAuthn.CompleteVerifyAsync(authorizerId, huellaPayload);

                if (!verificacion.Success)
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        resultado = verificacion.UserMessage,
                        errorCode = verificacion.ErrorCode?.ToString()
                    }, _jsonOpts);

                // 4. Extraer entity (NotaOperatoriaInputVM)
                if (!root.TryGetProperty("entity", out var entityElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos de nota no encontrados." }, _jsonOpts);

                var model = JsonSerializer.Deserialize<NotaOperatoriaInputVM>(
                    entityElement.GetRawText(), _jsonOpts);

                if (model == null || model.HospitalizacionId == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos de nota inválidos o HospitalizacionId faltante." }, _jsonOpts);

                // 5. Validar permisos del autorizador (admin o médico asignado)
                var authorizerUser = await _userManager.FindByIdAsync(authorizerId);
                var esAdmin = await _userManager.IsInRoleAsync(authorizerUser, "Administrador");

                if (!esAdmin)
                {
                    var citasId = await _db.Consultas
                        .Where(c => c.HospitalizacionId == model.HospitalizacionId)
                        .Select(c => c.CitasId)
                        .FirstOrDefaultAsync();

                    if (citasId == null)
                        return JsonSerializer.Serialize(new { exitoso = false, resultado = "No se encontró la consulta de esta hospitalización." }, _jsonOpts);

                    var cita = await _db.Citass
                        .Where(c => c.Id == citasId.Value)
                        .Select(c => new { c.EmpleadoId, c.MedicosSecundarios })
                        .FirstOrDefaultAsync();

                    if (cita == null)
                        return JsonSerializer.Serialize(new { exitoso = false, resultado = "No se encontró la cita de esta hospitalización." }, _jsonOpts);

                    var empleadosAutorizados = new List<int>();
                    if (cita.EmpleadoId.HasValue)
                        empleadosAutorizados.Add(cita.EmpleadoId.Value);
                    if (cita.MedicosSecundarios != null)
                        empleadosAutorizados.AddRange(cita.MedicosSecundarios);

                    var userIdsAutorizados = await _db.Usuarios
                        .Where(u => u.EmpleadoId != null && empleadosAutorizados.Contains(u.EmpleadoId.Value))
                        .Select(u => u.Id)
                        .ToListAsync();

                    if (!userIdsAutorizados.Contains(authorizerId))
                        return JsonSerializer.Serialize(new
                        {
                            exitoso = false,
                            resultado = "El autorizador no es el médico asignado ni un médico secundario de esta hospitalización."
                        }, _jsonOpts);
                }

                // 6. Guardar la nota operatoria
                var nuevaNota = new NotaOperatoria
                {
                    Diagnostico = model.Diagnostico,
                    HospitalizacionId = model.HospitalizacionId,
                    FechaRegistro = DateTime.Now,
                    UserId = _userManager.GetUserId(HttpContext.User), // usuario logueado (puede diferir del autorizador)
                    FechaOperacion = model.FechaOperacion,
                    HoraComenzo = model.HoraComenzo,
                    HoraTermino = model.HoraTermino,
                    Cirujano = model.Cirujano,
                    PrimerAyudante = model.PrimerAyudante,
                    SegundoAyudante = model.SegundoAyudante,
                    Anestesista = model.Anestesista,
                    Instrumentista = model.Instrumentista,
                    Circulante = model.Circulante,
                    DiagnosticoPreOperatorio = model.DiagnosticoPreOperatorio,
                    DiagnosticoPostOperatorio = model.DiagnosticoPostOperatorio,
                    OperacionEfectuada = model.OperacionEfectuada,
                    HallazgosTransOperatorios = model.HallazgosTransOperatorios,
                };

                _notaOperatoriaService.Add(nuevaNota);

                return JsonSerializer.Serialize(new { exitoso = true, resultado = true }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                }, _jsonOpts);
            }
        }


        [HttpPost]
        public string AgregarNotaOperatoria([FromBody] NotaOperatoriaInputVM model)
        {
            try
            {
                if (model == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos inválidos." }, _jsonOpts);

                if (model.HospitalizacionId == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "HospitalizacionId inválido." }, _jsonOpts);

                var nuevaNota = new NotaOperatoria
                {
                    Diagnostico = model.Diagnostico,
                    HospitalizacionId = model.HospitalizacionId,
                    FechaRegistro = DateTime.Now,
                    UserId = _userManager.GetUserId(HttpContext.User),
                    FechaOperacion = model.FechaOperacion,
                    HoraComenzo = model.HoraComenzo,
                    HoraTermino = model.HoraTermino,
                    Cirujano = model.Cirujano,
                    PrimerAyudante = model.PrimerAyudante,
                    SegundoAyudante = model.SegundoAyudante,
                    Anestesista = model.Anestesista,
                    Instrumentista = model.Instrumentista,
                    Circulante = model.Circulante,
                    DiagnosticoPreOperatorio = model.DiagnosticoPreOperatorio,
                    DiagnosticoPostOperatorio = model.DiagnosticoPostOperatorio,
                    OperacionEfectuada = model.OperacionEfectuada,
                    HallazgosTransOperatorios = model.HallazgosTransOperatorios,
                };

                _notaOperatoriaService.Add(nuevaNota);

                return JsonSerializer.Serialize(new { exitoso = true, resultado = true }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                }, _jsonOpts);
            }
        }

        [HttpPost]
        public string ObtenerNotasOperatorias(int idHospitalizacion)
        {
            try
            {
                var result = _notaOperatoriaService.GetByHospitalizacionId(idHospitalizacion);
                return JsonSerializer.Serialize(new { exitoso = true, resultado = result }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    mensaje = "Error al obtener las notas operatorias: " + ex.Message
                }, _jsonOpts);
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


        // [HttpPost]
        // public string FirmarNotaOperatoria([FromBody] FirmarNotaOperatoriaInputVM model)
        // {
        //     try
        //     {
        //         if (model == null || model.NotaOperatoriaId == 0)
        //             return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos inválidos." }, _jsonOpts);

        //         string rutaFinal;

        //         if (!string.IsNullOrEmpty(model.FirmaExistenteUrl))
        //         {
        //             rutaFinal = model.FirmaExistenteUrl;
        //         }
        //         else if (!string.IsNullOrEmpty(model.FirmaBase64))
        //         {
        //             var userId = _userManager.GetUserId(HttpContext.User);
        //             var carpeta = Path.Combine(_env.WebRootPath, "Firmas", "NotasOperatorias");
        //             Directory.CreateDirectory(carpeta);

        //             var firmasAnteriores = Directory.GetFiles(carpeta, $"nota_*_{userId}_*.png");
        //             foreach (var firmaAnterior in firmasAnteriores)
        //             {
        //                 System.IO.File.Delete(firmaAnterior);
        //             }

        //             var nombreArchivo = $"nota_{model.NotaOperatoriaId}_{userId}_{DateTime.Now:yyyyMMddHHmmss}.png";
        //             var rutaAbsoluta = Path.Combine(carpeta, nombreArchivo);

        //             var base64Data = model.FirmaBase64.Contains(",")
        //                 ? model.FirmaBase64.Split(',')[1]
        //                 : model.FirmaBase64;

        //             System.IO.File.WriteAllBytes(rutaAbsoluta, Convert.FromBase64String(base64Data));
        //             rutaFinal = $"/Firmas/NotasOperatorias/{nombreArchivo}";
        //         }
        //         else
        //         {
        //             return JsonSerializer.Serialize(new { exitoso = false, resultado = "No se proporcionó firma." }, _jsonOpts);
        //         }

        //         _notaOperatoriaService.GuardarFirma(model.NotaOperatoriaId, rutaFinal);

        //         return JsonSerializer.Serialize(new { exitoso = true, firmaRuta = rutaFinal }, _jsonOpts);
        //     }
        //     catch (Exception ex)
        //     {
        //         return JsonSerializer.Serialize(new
        //         {
        //             exitoso = false,
        //             resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
        //         }, _jsonOpts);
        //     }
        // }

        // [HttpPost]
        // public async Task<string> FirmarNotaOperatoria()
        // {
        //     try
        //     {
        //         // 1. Leer el body completo
        //         string rawBody;
        //         using (var reader = new StreamReader(Request.Body))
        //             rawBody = await reader.ReadToEndAsync();

        //         if (string.IsNullOrEmpty(rawBody))
        //             return JsonSerializer.Serialize(new { exitoso = false, resultado = "Body vacío." }, _jsonOpts);

        //         var opts = new JsonSerializerOptions
        //         {
        //             PropertyNameCaseInsensitive = true,
        //             ReferenceHandler = ReferenceHandler.IgnoreCycles
        //         };

        //         using var doc = JsonDocument.Parse(rawBody);
        //         var root = doc.RootElement;

        //         // 2. Extraer huellaPayload
        //         if (!root.TryGetProperty("huellaPayload", out var huellaElement))
        //             return JsonSerializer.Serialize(new { exitoso = false, resultado = "Payload de huella no encontrado." }, _jsonOpts);

        //         var huellaPayload = JsonSerializer.Deserialize<WebAuthnAssertionPayload>(
        //             huellaElement.GetRawText(), opts);

        //         if (huellaPayload == null)
        //             return JsonSerializer.Serialize(new { exitoso = false, resultado = "Payload de huella inválido." }, _jsonOpts);

        //         // 3. Identificar al autorizador por credentialId
        //         var credIdString = Convert.ToBase64String(
        //             Convert.FromBase64String(
        //                 huellaPayload.RawId.Replace('-', '+').Replace('_', '/')
        //                 + new string('=', (4 - huellaPayload.RawId.Length % 4) % 4)
        //             ));

        //         var credencial = await _db.WebAuthnCredentials
        //             .FirstOrDefaultAsync(c => c.DescriptorId == credIdString);

        //         if (credencial == null)
        //             return JsonSerializer.Serialize(new
        //             {
        //                 exitoso = false,
        //                 resultado = "La credencial presentada no está registrada en el sistema."
        //             }, _jsonOpts);

        //         var authorizerId = credencial.UserId;

        //         // 4. Verificar la firma criptográfica
        //         var verificacion = await _webAuthn.CompleteVerifyAsync(authorizerId, huellaPayload);

        //         if (!verificacion.Success)
        //             return JsonSerializer.Serialize(new
        //             {
        //                 exitoso = false,
        //                 resultado = verificacion.UserMessage,
        //                 errorCode = verificacion.ErrorCode?.ToString()
        //             }, _jsonOpts);

        //         // 5. Extraer el modelo de firma
        //         if (!root.TryGetProperty("model", out var modelElement))
        //             return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos de firma no encontrados." }, _jsonOpts);

        //         var model = JsonSerializer.Deserialize<FirmarNotaOperatoriaInputVM>(
        //             modelElement.GetRawText(), opts);

        //         if (model == null || model.NotaOperatoriaId == 0)
        //             return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos de firma inválidos." }, _jsonOpts);

        //         // 6. Obtener la nota operatoria para validar permisos
        //         var nota = await _db.NotaOperatoria.FindAsync(model.NotaOperatoriaId);
        //         if (nota == null)
        //             return JsonSerializer.Serialize(new { exitoso = false, resultado = "Nota operatoria no encontrada." }, _jsonOpts);

        //         var hospitalizacionId = nota.HospitalizacionId;

        //         // 7. Validar que el autorizador tiene permiso (admin o médico asignado)
        //         var authorizerUser = await _userManager.FindByIdAsync(authorizerId);
        //         var esAdmin = await _userManager.IsInRoleAsync(authorizerUser, "Administrador");

        //         if (!esAdmin)
        //         {
        //             var citasId = await _db.Consultas
        //                 .Where(c => c.HospitalizacionId == hospitalizacionId)
        //                 .Select(c => c.CitasId)
        //                 .FirstOrDefaultAsync();

        //             if (citasId == null)
        //                 return JsonSerializer.Serialize(new
        //                 {
        //                     exitoso = false,
        //                     resultado = "No se encontró la consulta de esta hospitalización."
        //                 }, _jsonOpts);

        //             var cita = await _db.Citass
        //                 .Where(c => c.Id == citasId.Value)
        //                 .Select(c => new { c.EmpleadoId, c.MedicosSecundarios })
        //                 .FirstOrDefaultAsync();

        //             if (cita == null)
        //                 return JsonSerializer.Serialize(new
        //                 {
        //                     exitoso = false,
        //                     resultado = "No se encontró la cita de esta hospitalización."
        //                 }, _jsonOpts);

        //             var empleadosAutorizados = new List<int>();
        //             if (cita.EmpleadoId.HasValue)
        //                 empleadosAutorizados.Add(cita.EmpleadoId.Value);
        //             if (cita.MedicosSecundarios != null)
        //                 empleadosAutorizados.AddRange(cita.MedicosSecundarios);

        //             var userIdsAutorizados = await _db.Usuarios
        //                 .Where(u => u.EmpleadoId != null && empleadosAutorizados.Contains(u.EmpleadoId.Value))
        //                 .Select(u => u.Id)
        //                 .ToListAsync();

        //             if (!userIdsAutorizados.Contains(authorizerId))
        //                 return JsonSerializer.Serialize(new
        //                 {
        //                     exitoso = false,
        //                     resultado = "El autorizador no es el médico asignado ni un médico secundario de esta hospitalización."
        //                 }, _jsonOpts);
        //         }

        //         // 8. Procesar la firma (igual que antes)
        //         string rutaFinal;

        //         if (!string.IsNullOrEmpty(model.FirmaExistenteUrl))
        //         {
        //             rutaFinal = model.FirmaExistenteUrl;
        //         }
        //         else if (!string.IsNullOrEmpty(model.FirmaBase64))
        //         {
        //             var userId = _userManager.GetUserId(HttpContext.User);
        //             var carpeta = Path.Combine(_env.WebRootPath, "Firmas", "NotasOperatorias");
        //             Directory.CreateDirectory(carpeta);

        //             var firmasAnteriores = Directory.GetFiles(carpeta, $"nota_*_{userId}_*.png");
        //             foreach (var firmaAnterior in firmasAnteriores)
        //                 System.IO.File.Delete(firmaAnterior);

        //             var nombreArchivo = $"nota_{model.NotaOperatoriaId}_{userId}_{DateTime.Now:yyyyMMddHHmmss}.png";
        //             var rutaAbsoluta = Path.Combine(carpeta, nombreArchivo);

        //             var base64Data = model.FirmaBase64.Contains(",")
        //                 ? model.FirmaBase64.Split(',')[1]
        //                 : model.FirmaBase64;

        //             System.IO.File.WriteAllBytes(rutaAbsoluta, Convert.FromBase64String(base64Data));
        //             rutaFinal = $"/Firmas/NotasOperatorias/{nombreArchivo}";
        //         }
        //         else
        //         {
        //             return JsonSerializer.Serialize(new { exitoso = false, resultado = "No se proporcionó firma." }, _jsonOpts);
        //         }

        //         _notaOperatoriaService.GuardarFirma(model.NotaOperatoriaId, rutaFinal);

        //         return JsonSerializer.Serialize(new { exitoso = true, firmaRuta = rutaFinal }, _jsonOpts);
        //     }
        //     catch (Exception ex)
        //     {
        //         return JsonSerializer.Serialize(new
        //         {
        //             exitoso = false,
        //             resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
        //         }, _jsonOpts);
        //     }
        // }



        [HttpPost]
        public async Task<string> FirmarNotaOperatoria()
        {
            try
            {
                // 1. Leer el body completo
                string rawBody;
                using (var reader = new StreamReader(Request.Body))
                    rawBody = await reader.ReadToEndAsync();

                if (string.IsNullOrEmpty(rawBody))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Body vacío." }, _jsonOpts);

                var opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                using var doc = JsonDocument.Parse(rawBody);
                var root = doc.RootElement;

                // 2. Extraer huellaPayload
                if (!root.TryGetProperty("huellaPayload", out var huellaElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Payload de huella no encontrado." }, _jsonOpts);

                var huellaPayload = JsonSerializer.Deserialize<WebAuthnAssertionPayload>(
                    huellaElement.GetRawText(), opts);

                if (huellaPayload == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Payload de huella inválido." }, _jsonOpts);

                // 3. Identificar al autorizador por credentialId
                var credIdString = Convert.ToBase64String(
                    Convert.FromBase64String(
                        huellaPayload.RawId.Replace('-', '+').Replace('_', '/')
                        + new string('=', (4 - huellaPayload.RawId.Length % 4) % 4)
                    ));

                var credencial = await _db.WebAuthnCredentials
                    .FirstOrDefaultAsync(c => c.DescriptorId == credIdString);

                if (credencial == null)
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        resultado = "La credencial presentada no está registrada en el sistema."
                    }, _jsonOpts);

                var authorizerId = credencial.UserId;

                // 4. Verificar la firma criptográfica
                var verificacion = await _webAuthn.CompleteVerifyAsync(authorizerId, huellaPayload);

                if (!verificacion.Success)
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        resultado = verificacion.UserMessage,
                        errorCode = verificacion.ErrorCode?.ToString()
                    }, _jsonOpts);

                // 5. Extraer el modelo de firma
                if (!root.TryGetProperty("model", out var modelElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos de firma no encontrados." }, _jsonOpts);

                var model = JsonSerializer.Deserialize<FirmarNotaOperatoriaInputVM>(
                    modelElement.GetRawText(), opts);

                if (model == null || model.NotaOperatoriaId == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos de firma inválidos." }, _jsonOpts);

                // 6. Procesar la firma (sin ninguna validación adicional)
                string rutaFinal;

                if (!string.IsNullOrEmpty(model.FirmaExistenteUrl))
                {
                    rutaFinal = model.FirmaExistenteUrl;
                }
                else if (!string.IsNullOrEmpty(model.FirmaBase64))
                {
                    var carpeta = Path.Combine(_env.WebRootPath, "Firmas", "NotasOperatorias");
                    Directory.CreateDirectory(carpeta);

                    var firmasAnteriores = Directory.GetFiles(carpeta, $"nota_{model.NotaOperatoriaId}_{authorizerId}_*.png");
                    foreach (var firmaAnterior in firmasAnteriores)
                        System.IO.File.Delete(firmaAnterior);

                    var nombreArchivo = $"nota_{model.NotaOperatoriaId}_{authorizerId}_{DateTime.Now:yyyyMMddHHmmss}.png";
                    var rutaAbsoluta = Path.Combine(carpeta, nombreArchivo);

                    var base64Data = model.FirmaBase64.Contains(",")
                        ? model.FirmaBase64.Split(',')[1]
                        : model.FirmaBase64;

                    System.IO.File.WriteAllBytes(rutaAbsoluta, Convert.FromBase64String(base64Data));
                    rutaFinal = $"/Firmas/NotasOperatorias/{nombreArchivo}";
                }
                else
                {
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "No se proporcionó firma." }, _jsonOpts);
                }

                _notaOperatoriaService.GuardarFirma(model.NotaOperatoriaId, rutaFinal);

                return JsonSerializer.Serialize(new { exitoso = true, firmaRuta = rutaFinal }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                }, _jsonOpts);
            }
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerPersonalEnfermeria()
        {
            try
            {
                var usuariosEnfermeria = await _userManager.GetUsersInRoleAsync("Enfermeria");

                if (usuariosEnfermeria == null || !usuariosEnfermeria.Any())
                {
                    usuariosEnfermeria = await _userManager.GetUsersInRoleAsync("Enfermera");
                }

                var personalEnfermeria = new List<object>();

                foreach (var usuario in usuariosEnfermeria)
                {
                    if (usuario.EmpleadoId.HasValue)
                    {
                        var empleado = _empleadoRepository.Get(usuario.EmpleadoId.Value);
                        if (empleado != null && !empleado.Eliminado)
                        {
                            personalEnfermeria.Add(new
                            {
                                id = empleado.Id,
                                nombreCompleto = $"{empleado.Nombre} {empleado.Apellido}".Trim(),
                                nombre = empleado.Nombre,
                                apellido = empleado.Apellido,
                                tipoEmpleado = empleado.TipoEmpleado,
                                firmaUrl = empleado.FirmaEmpleado
                            });
                        }
                    }
                }

                return Json(new
                {
                    success = true,
                    data = personalEnfermeria.OrderBy(e => ((dynamic)e).nombreCompleto)
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al cargar el personal de enfermería."
                });
            }
        }


        [HttpGet]
        public string GetUltimaNotaOperatoria(int hospitalizacionId)
        {
            try
            {
                var notas = _notaOperatoriaService.GetByHospitalizacionId(hospitalizacionId);
                var ultima = notas?.FirstOrDefault();
                if (ultima == null)
                    return JsonSerializer.Serialize(new { exitoso = false }, _jsonOpts);

                return JsonSerializer.Serialize(new
                {
                    exitoso = true,
                    cirujano = ultima.Cirujano ?? "",
                    primerAyudante = ultima.PrimerAyudante ?? "",
                    segundoAyudante = ultima.SegundoAyudante ?? "",
                    anestesista = ultima.Anestesista ?? "",
                    instrumentista = ultima.Instrumentista ?? "",
                    circulante = ultima.Circulante ?? ""
                }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, mensaje = ex.Message }, _jsonOpts);
            }
        }

        [HttpPost]
        public string ActualizarNotaOperatoria([FromBody] NotaOperatoriaInputVM model)
        {
            try
            {
                if (model == null || model.Id == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Id invalido." }, _jsonOpts);

                var notaActualizada = new NotaOperatoria
                {
                    Id = model.Id,
                    FechaOperacion = model.FechaOperacion,
                    HoraComenzo = model.HoraComenzo,
                    HoraTermino = model.HoraTermino,
                    Cirujano = model.Cirujano,
                    PrimerAyudante = model.PrimerAyudante,
                    SegundoAyudante = model.SegundoAyudante,
                    Anestesista = model.Anestesista,
                    Instrumentista = model.Instrumentista,
                    Circulante = model.Circulante,
                    DiagnosticoPreOperatorio = model.DiagnosticoPreOperatorio,
                    DiagnosticoPostOperatorio = model.DiagnosticoPostOperatorio,
                    OperacionEfectuada = model.OperacionEfectuada,
                    HallazgosTransOperatorios = model.HallazgosTransOperatorios,
                    Diagnostico = model.Diagnostico
                };
                _notaOperatoriaService.ActualizarNotaOperatoria(notaActualizada);
                return JsonSerializer.Serialize(new { exitoso = true }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, resultado = "Error: " + ex.Message }, _jsonOpts);
            }
        }

    }

    public class FirmarNotaOperatoriaInputVM
    {
        public int NotaOperatoriaId { get; set; }
        public string FirmaBase64 { get; set; }
        public string FirmaExistenteUrl { get; set; }
    }

    public class NotaOperatoriaInputVM
    {
        public int Id { get; set; } // 0 = nuevo, >0 = editar
        public string Diagnostico { get; set; }
        public int HospitalizacionId { get; set; }

        // Datos del encabezado del formulario quirúrgico
        public DateTime? FechaOperacion { get; set; }
        public string HoraComenzo { get; set; }
        public string HoraTermino { get; set; }

        // Personal de quirófano
        public string Cirujano { get; set; }
        public string PrimerAyudante { get; set; }
        public string SegundoAyudante { get; set; }
        public string Anestesista { get; set; }
        public string Instrumentista { get; set; }
        public string Circulante { get; set; }

        // Diagnósticos y hallazgos
        public string DiagnosticoPreOperatorio { get; set; }
        public string DiagnosticoPostOperatorio { get; set; }
        public string OperacionEfectuada { get; set; }
        public string HallazgosTransOperatorios { get; set; }
    }
}