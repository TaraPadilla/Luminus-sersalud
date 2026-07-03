using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.IO;
using Database.Shared;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Hangfire;
using System.Globalization;
using farmamest.Service;

namespace farmamest.Controllers
{
    public class SolicitudMedicamentoController : Controller
    {
        private readonly ISolicitudMedicamentoService _solicitudMedicamentoService;
        private readonly UserManager<User> _userManager;
        private readonly IUser _userRepository;
        private readonly IEmpleado _empleadoRepository;
        private readonly IProducto _productosRepository = null;

        private readonly IHospitalizacion _hospitalizacionRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMedicamentoNotificacionService _medicamentoNotificacionService;

        private readonly Context _db;

        private static readonly string HospitalTimeZoneId = "America/Guatemala"; // o "America/Venezuela"
        private static readonly TimeZoneInfo HospitalTimeZone = TimeZoneInfo.FindSystemTimeZoneById(HospitalTimeZoneId);


        public SolicitudMedicamentoController(
            ISolicitudMedicamentoService solicitudMedicamentoService,
            UserManager<User> userManager,
            IUser userRepository,
            IEmpleado empleadoRepository,
            IProducto productosRepository,
            IHospitalizacion hospitalizacionRepository,
            Context db,
             IHttpClientFactory httpClientFactory,
             IMedicamentoNotificacionService medicamentoNotificacionService
)
        {
            _solicitudMedicamentoService = solicitudMedicamentoService;
            _userManager = userManager;
            _userRepository = userRepository;
            _empleadoRepository = empleadoRepository;
            _productosRepository = productosRepository;
            _hospitalizacionRepository = hospitalizacionRepository;
            _db = db;
            _httpClientFactory = httpClientFactory;
            _medicamentoNotificacionService = medicamentoNotificacionService;
        }

        // ✅ Método para obtener el ID del usuario actual
        [HttpGet]
        public ActionResult<string> GetCurrentUserId()
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                if (string.IsNullOrEmpty(userId))
                    return NotFound(new { Exitoso = false, Mensaje = "Usuario no autenticado." });

                return Ok(new { Exitoso = true, UsuarioId = userId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener ID del usuario. {ex.Message}" });
            }
        }

        // ✅ Obtener el Nombre y Apellido de un usuario mediante su ID (buscando su EmpleadoId)
        [HttpGet]
        public ActionResult<string> GetUserFullNameByEmployeeId(string userId)
        {
            try
            {
                var user = _userRepository.GetbyId(userId);
                if (user == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Usuario no encontrado." });

                var empleadoId = user.EmpleadoId;
                if (empleadoId == null)
                    return Ok(new { Exitoso = true, NombreCompleto = "Admin" });

                var empleado = _empleadoRepository.Get((int)empleadoId);
                if (empleado == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Empleado no encontrado." });

                var nombreCompleto = empleado.NombreYApellidos;
                return Ok(new { Exitoso = true, NombreCompleto = nombreCompleto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener el nombre del usuario. {ex.Message}" });
            }
        }

        // ✅ 1. Obtener todas las solicitudes de medicamento
        [HttpGet]
        public ActionResult<List<SolicitudMedicamento>> GetAll(int? hospitalizacionId = null, int? categoriaId = null)
        {
            try
            {
                var solicitudes = _solicitudMedicamentoService.GetAllSolicitudesMedicamento(hospitalizacionId);

                // Filtrar por categoría si se proporciona
                if (categoriaId.HasValue)
                {
                    solicitudes = solicitudes.Where(s => s.Hospitalizacion?.Habitacion?.CategoriaHabitacionId == categoriaId).ToList();
                }

                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener solicitudes. {ex.Message}" });
            }
        }


        // ✅ 2. Obtener una solicitud de medicamento por ID
        [HttpGet]
        public ActionResult<SolicitudMedicamento> GetById(int id)
        {
            try
            {
                var solicitud = _solicitudMedicamentoService.GetSolicitudMedicamentoById(id);
                if (solicitud == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Solicitud no encontrada." });

                return Ok(solicitud);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener la solicitud. {ex.Message}" });
            }
        }

        // ✅ 3. Crear una nueva solicitud de medicamento
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] SolicitudMedicamento solicitud)
        {
            try
            {
                if (solicitud == null)
                    return BadRequest(new { Exitoso = false, Mensaje = "Solicitud inválida." });

                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                    return Unauthorized(new { Exitoso = false, Mensaje = "Usuario no autenticado." });

                solicitud.UsuarioSolicitanteId = user.Id;
                solicitud.UsuarioSolicitanteNombre = user.Email;
                solicitud.FechaSolicitud = DateTime.Now;
                solicitud.TipoProducto = "Medicamento";

                var producto = _productosRepository.GetProdutoById(solicitud.ProductoId);
                if (producto != null)
                {
                    solicitud.NombreProducto = producto.NombreProducto;
                }

                // ✅ Si la solicitud es directa, se marca como despachada
                if (solicitud.Directa)
                {
                    solicitud.Estado = "Despachado";
                    solicitud.UsuarioDespachanteId = user.Id;
                    solicitud.FechaDespacho = DateTime.Now;
                }

                _solicitudMedicamentoService.AddSolicitudMedicamento(solicitud);

                return Ok(new
                {
                    Exitoso = true,
                    Mensaje = solicitud.Directa ? "Solicitud directa despachada exitosamente." : "Solicitud creada exitosamente.",
                    FechaSolicitud = solicitud.FechaSolicitudFormatted,
                    NombreProducto = solicitud.NombreProducto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al agregar solicitud. {ex.Message}" });
            }
        }



        // ✅ 4. Actualizar una solicitud de medicamento
        [HttpPut]
        public ActionResult Update(int id, [FromBody] SolicitudMedicamento solicitud)
        {
            try
            {
                if (solicitud == null || id != solicitud.Id)
                    return BadRequest(new { Exitoso = false, Mensaje = "Datos de solicitud inválidos." });

                var solicitudExistente = _solicitudMedicamentoService.GetSolicitudMedicamentoById(id);
                if (solicitudExistente == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Solicitud no encontrada." });

                _solicitudMedicamentoService.UpdateSolicitudMedicamento(solicitud);
                return Ok(new { Exitoso = true, Mensaje = "Solicitud actualizada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al actualizar la solicitud. {ex.Message}" });
            }
        }

        // ✅ 5. Eliminar una solicitud de medicamento
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                var solicitud = _solicitudMedicamentoService.GetSolicitudMedicamentoById(id);
                if (solicitud == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Solicitud no encontrada." });

                _solicitudMedicamentoService.DeleteSolicitudMedicamento(id);
                return Ok(new { Exitoso = true, Mensaje = "Solicitud eliminada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al eliminar la solicitud. {ex.Message}" });
            }
        }

        // ✅ 6. Obtener solicitudes por hospitalización
        [HttpGet]
        public ActionResult<List<SolicitudMedicamento>> GetByHospitalizacion(int hospitalizacionId)
        {
            try
            {
                return Ok(_solicitudMedicamentoService.GetSolicitudesByHospitalizacionId(hospitalizacionId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener solicitudes. {ex.Message}" });
            }
        }


        // [HttpPut]
        // public async Task<ActionResult> MarkAsDispensed(int id, [FromBody] bool refused)
        // {
        //     try
        //     {
        //         var solicitud = _solicitudMedicamentoService.GetSolicitudMedicamentoById(id);
        //         if (solicitud == null)
        //             return NotFound(new { Exitoso = false, Mensaje = "Solicitud no encontrada." });

        //         var userId = _userManager.GetUserId(HttpContext.User);
        //         if (string.IsNullOrEmpty(userId))
        //             return Unauthorized(new { Exitoso = false, Mensaje = "Usuario no autenticado." });

        //         solicitud.UsuarioDespachanteId = userId;
        //         solicitud.FechaDespacho = DateTime.UtcNow; // Guardar en UTC
        //         solicitud.Estado = refused ? "Rechazado" : "Despachado";
        //         _solicitudMedicamentoService.UpdateSolicitudMedicamento(solicitud);

        //         if (!refused && solicitud.HospitalizacionId > 0)
        //         {
        //             // Crear HospitalizacionProducto
        //             var hospitalizacionProducto = _hospitalizacionRepository.AddMedicamento(new HospitalizacionProducto
        //             {
        //                 HospitalizacionId = solicitud.HospitalizacionId,
        //                 ProductoId = solicitud.ProductoId,
        //                 UnidadMedidaVentaId = solicitud.UnidadMedidaVentaId,
        //                 PrecioId = solicitud.PrecioId,
        //                 PrecioValor = solicitud.Precio,
        //                 Cantidad = (int)solicitud.Cantidad,
        //                 Indicaciones = solicitud.Indicaciones ?? "",
        //                 ViaAdministracion = solicitud.ViaAdministracion ?? "",
        //                 FrecuenciaAdministracion = solicitud.FrecuenciaAdministracion ?? "",
        //                 FechaHoraAplicacionManual = solicitud.FechaHoraAplicacionManual
        //             });

        //             for (var i = 1; i <= hospitalizacionProducto.Cantidad; i++)
        //             {
        //                 _hospitalizacionRepository.AddProductoAplicacion(new HospitalizacionProductoAplicacion
        //                 {
        //                     HospitalizacionProductoId = hospitalizacionProducto.Id,
        //                     Cantidad = 1,
        //                     Aplicado = false,
        //                     UsuarioCreaId = userId
        //                 });
        //             }

        //             solicitud.EsRegistroHospitalizacion = true;
        //             _solicitudMedicamentoService.UpdateSolicitudMedicamento(solicitud);

        //             // ==================== ENVÍO DE CORREOS CON MANEJO DE ZONA HORARIA ====================
        //             var emails = ObtenerEmailsMedicosDesdeCita(solicitud.HospitalizacionId);
        //             if (emails.Any())
        //             {
        //                 var pacienteNombre = _db.Hospitalizaciones
        //                     .Where(h => h.Id == solicitud.HospitalizacionId)
        //                     .Select(h => h.Paciente.Nombre)
        //                     .FirstOrDefault() ?? "Paciente";

        //                 var baseUrl = $"{Request.Scheme}://{Request.Host}";

        //                 Console.WriteLine($"[DEBUG] FechaHoraAplicacionManual RAW: '{solicitud.FechaHoraAplicacionManual}'");

        //                 // ----- Parsear y convertir la fecha/hora programada -----
        //                 DateTime? utcScheduledTime = ParseFechaHoraAplicacion(solicitud.FechaHoraAplicacionManual, HospitalTimeZone);
        //                 if (utcScheduledTime.HasValue)
        //                     Console.WriteLine($"[DEBUG] Hora UTC calculada: {utcScheduledTime:yyyy-MM-dd HH:mm:ss}");
        //                 else
        //                     Console.WriteLine("[DEBUG] No se pudo interpretar la fecha/hora.");

        //                 // ----- Calcular retraso inicial (UTC) -----
        //                 TimeSpan initialDelay = TimeSpan.Zero;
        //                 DateTime nowUtc = DateTime.UtcNow;

        //                 if (utcScheduledTime.HasValue)
        //                 {
        //                     if (utcScheduledTime.Value > nowUtc)
        //                     {
        //                         initialDelay = utcScheduledTime.Value - nowUtc;
        //                         Console.WriteLine($"[TIMING] Primera dosis programada para UTC {utcScheduledTime:yyyy-MM-dd HH:mm:ss}, retraso de {initialDelay.TotalMinutes:F2} minutos.");
        //                     }
        //                     else
        //                     {
        //                         // Hora pasada: si es recurrente, reprogramar para mañana
        //                         if (solicitud.FrecuenciaAdministracion.StartsWith("Cada", StringComparison.OrdinalIgnoreCase))
        //                         {
        //                             // Obtener la hora local original, sumar un día, convertir a UTC
        //                             var localOriginal = TimeZoneInfo.ConvertTimeFromUtc(utcScheduledTime.Value, HospitalTimeZone);
        //                             var nextLocal = localOriginal.AddDays(1);
        //                             utcScheduledTime = TimeZoneInfo.ConvertTimeToUtc(nextLocal, HospitalTimeZone);
        //                             initialDelay = utcScheduledTime.Value - nowUtc;
        //                             Console.WriteLine($"[TIMING] Hora pasada y recurrente → reprogramado para mañana: Local {nextLocal:yyyy-MM-dd HH:mm:ss} -> UTC {utcScheduledTime:yyyy-MM-dd HH:mm:ss}, retraso {initialDelay.TotalMinutes:F2} min.");
        //                         }
        //                         else
        //                         {
        //                             Console.WriteLine("[TIMING] Hora pasada y no recurrente → se enviará inmediatamente.");
        //                             utcScheduledTime = null; // Para forzar envío inmediato
        //                         }
        //                     }
        //                 }

        //                 // ----- Lógica según frecuencia -----
        //                 if (solicitud.FrecuenciaAdministracion == "STAT")
        //                 {
        //                     if (initialDelay > TimeSpan.Zero)
        //                     {
        //                         BackgroundJob.Schedule(() =>
        //                             EnviarCorreoBaseAsync(
        //                                 emails,
        //                                 $"Nueva orden médica - {solicitud.NombreProducto} para {pacienteNombre}",
        //                                 ObtenerCuerpoCorreo(solicitud, pacienteNombre, null, null),
        //                                 baseUrl,
        //                                 pacienteNombre,             // ✅
        //                                 solicitud.NombreProducto),  // ✅
        //                             initialDelay);
        //                         Console.WriteLine($"[SCHEDULE] STAT programado con retraso de {initialDelay.TotalMinutes:F2} min.");
        //                     }
        //                     else
        //                     {
        //                         await EnviarCorreoBaseAsync(
        //                             emails,
        //                             $"Nueva orden médica - {solicitud.NombreProducto} para {pacienteNombre}",
        //                             ObtenerCuerpoCorreo(solicitud, pacienteNombre, null, null),
        //                             baseUrl,
        //                             pacienteNombre,             // ✅
        //                             solicitud.NombreProducto);  // ✅
        //                         Console.WriteLine("[SCHEDULE] STAT enviado inmediatamente.");
        //                     }
        //                 }
        //                 else if (solicitud.FrecuenciaAdministracion.Contains("Cada"))
        //                 {
        //                     var match = Regex.Match(solicitud.FrecuenciaAdministracion, @"\d+");
        //                     if (match.Success)
        //                     {
        //                         int valor = int.Parse(match.Value);
        //                         string frecuenciaLower = solicitud.FrecuenciaAdministracion.ToLower();
        //                         int intervaloMinutos = 0;
        //                         if (frecuenciaLower.Contains("minuto"))
        //                             intervaloMinutos = valor;
        //                         else if (frecuenciaLower.Contains("hora"))
        //                             intervaloMinutos = valor * 60;
        //                         else
        //                             intervaloMinutos = valor * 60;

        //                         int cantidad = (int)solicitud.Cantidad;
        //                         if (intervaloMinutos > 0 && cantidad > 0)
        //                         {
        //                             // Primera dosis
        //                             if (initialDelay > TimeSpan.Zero)
        //                             {
        //                                 BackgroundJob.Schedule(() =>
        //                                     EnviarCorreoBaseAsync(
        //                                         emails,
        //                                         $"Nueva orden médica - {solicitud.NombreProducto} para {pacienteNombre}",
        //                                         ObtenerCuerpoCorreo(solicitud, pacienteNombre, 1, cantidad),
        //                                         baseUrl,
        //                                         pacienteNombre,             // ✅
        //                                         solicitud.NombreProducto),  // ✅
        //                                     initialDelay);
        //                                 Console.WriteLine($"[SCHEDULE] Dosis 1/{cantidad} programada con retraso {initialDelay.TotalMinutes:F2} min.");
        //                             }
        //                             else
        //                             {
        //                                 await EnviarCorreoBaseAsync(
        //                                     emails,
        //                                     $"Nueva orden médica - {solicitud.NombreProducto} para {pacienteNombre}",
        //                                     ObtenerCuerpoCorreo(solicitud, pacienteNombre, 1, cantidad),
        //                                     baseUrl,
        //                                     pacienteNombre,             // ✅
        //                                     solicitud.NombreProducto);  // ✅
        //                                 Console.WriteLine($"[SCHEDULE] Dosis 1/{cantidad} enviada inmediatamente.");
        //                             }

        //                             // Programar siguientes dosis
        //                             if (cantidad > 1)
        //                             {
        //                                 TimeSpan delayForSecond = initialDelay + TimeSpan.FromMinutes(intervaloMinutos);
        //                                 BackgroundJob.Schedule(() =>
        //                                     EnviarRecordatorioMedicamento(solicitud.Id, 2, cantidad, intervaloMinutos, baseUrl),
        //                                     delayForSecond);
        //                                 Console.WriteLine($"[SCHEDULE] Siguientes dosis programadas cada {intervaloMinutos} minutos, primera repetición en {delayForSecond.TotalMinutes:F2} min.");
        //                             }
        //                         }
        //                         else
        //                         {
        //                             Console.WriteLine("[SCHEDULE] Intervalo o cantidad inválidos, no se programaron correos.");
        //                         }
        //                     }
        //                     else
        //                     {
        //                         Console.WriteLine($"[SCHEDULE] No se pudo extraer número de la frecuencia: '{solicitud.FrecuenciaAdministracion}'.");
        //                     }
        //                 }
        //                 else
        //                 {
        //                     Console.WriteLine($"[SCHEDULE] Frecuencia no reconocida: '{solicitud.FrecuenciaAdministracion}'. No se enviaron correos.");
        //                 }
        //             }
        //             else
        //             {
        //                 Console.WriteLine("[EMAILS] No hay médicos asociados a la cita, no se enviarán correos.");
        //             }
        //         }

        //         return Ok(new
        //         {
        //             Exitoso = true,
        //             Mensaje = $"Solicitud marcada como '{solicitud.Estado}' exitosamente.",
        //             FechaDespacho = solicitud.FechaDespacho,
        //             UsuarioDespachanteId = solicitud.UsuarioDespachanteId
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"[ERROR FATAL] MarkAsDispensed: {ex.Message}");
        //         Console.WriteLine(ex.StackTrace);
        //         return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al actualizar solicitud. {ex.Message}" });
        //     }
        // }

        [HttpPut]
        public async Task<ActionResult> MarkAsDispensed(int id, [FromBody] bool refused)
        {
            try
            {
                var solicitud = _solicitudMedicamentoService.GetSolicitudMedicamentoById(id);
                if (solicitud == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Solicitud no encontrada." });

                var userId = _userManager.GetUserId(HttpContext.User);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Exitoso = false, Mensaje = "Usuario no autenticado." });

                solicitud.UsuarioDespachanteId = userId;
                solicitud.FechaDespacho = DateTime.UtcNow;
                solicitud.Estado = refused ? "Rechazado" : "Despachado";
                _solicitudMedicamentoService.UpdateSolicitudMedicamento(solicitud);

                if (!refused && solicitud.HospitalizacionId > 0)
                {
                    await RegistrarMedicamentoDesdeSolicitudAsync(solicitud, userId);
                    solicitud.EsRegistroHospitalizacion = true;
                    _solicitudMedicamentoService.UpdateSolicitudMedicamento(solicitud);
                }

                return Ok(new
                {
                    Exitoso = true,
                    Mensaje = $"Solicitud marcada como '{solicitud.Estado}' exitosamente.",
                    FechaDespacho = solicitud.FechaDespacho,
                    UsuarioDespachanteId = solicitud.UsuarioDespachanteId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al actualizar solicitud. {ex.Message}" });
            }
        }

        // 📄 Muestra la vista con todas las solicitudes de medicamentos.
        [HttpGet]
        public IActionResult SolicitudesMedicamentosView(int? hospitalizacionId = null)
        {
            try
            {
                var solicitudes = _solicitudMedicamentoService.GetAllSolicitudesMedicamento(hospitalizacionId);
                ViewBag.HospitalizacionId = hospitalizacionId; // Opcional para debug en la vista
                return View(solicitudes);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las solicitudes: {ex.Message}";
                return RedirectToAction("Error");
            }
        }


        // ✅ Endpoint para aprobar todas las solicitudes de una hospitalización específica

        [HttpPut]
        public async Task<ActionResult> AprobarTodasSolicitudesPorHospitalizacion([FromQuery] int hospitalizacionId)
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Exitoso = false, Mensaje = "Usuario no autenticado." });

                Console.WriteLine($"HospitalizacionId recibido: {hospitalizacionId} (Tipo: {hospitalizacionId.GetType()})");

                var todasSolicitudes = _solicitudMedicamentoService.GetAllSolicitudesMedicamento(hospitalizacionId);
                Console.WriteLine($"Total de solicitudes en la base de datos: {todasSolicitudes.Count}");

                var solicitudes = todasSolicitudes
                    .Where(s => s.Estado.Trim().ToLower() == "en espera" && s.HospitalizacionId == hospitalizacionId)
                    .ToList();

                Console.WriteLine($"Total de solicitudes encontradas para hospitalizacionId {hospitalizacionId}: {solicitudes.Count}");

                if (solicitudes == null || solicitudes.Count == 0)
                {
                    return NotFound(new { Exitoso = false, Mensaje = "No hay solicitudes en espera para aprobar." });
                }

                foreach (var solicitud in solicitudes)
                {
                    solicitud.Estado = "Despachado";
                    solicitud.UsuarioDespachanteId = userId;
                    solicitud.FechaDespacho = DateTime.Now;
                    _solicitudMedicamentoService.UpdateSolicitudMedicamento(solicitud);

                    if (solicitud.HospitalizacionId > 0 && !solicitud.EsRegistroHospitalizacion)
                    {
                        await RegistrarMedicamentoDesdeSolicitudAsync(solicitud, userId);
                        solicitud.EsRegistroHospitalizacion = true;
                        _solicitudMedicamentoService.UpdateSolicitudMedicamento(solicitud);
                    }
                }

                return Ok(new
                {
                    Exitoso = true,
                    Mensaje = "Todas las solicitudes han sido aprobadas exitosamente.",
                    TotalAprobadas = solicitudes.Count,
                    Solicitudes = solicitudes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al aprobar las solicitudes. {ex.Message}" });
            }
        }


        [HttpPut]
        public ActionResult ActualizarRegistroHospitalizacion(int id)
        {
            try
            {
                // 1. Obtener la solicitud de medicamento por ID
                var solicitud = _solicitudMedicamentoService.GetSolicitudMedicamentoById(id);
                if (solicitud == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Solicitud no encontrada." });

                // 2. Actualizar solo la propiedad EsRegistroHospitalizacion
                solicitud.EsRegistroHospitalizacion = true;

                // 3. Guardar la actualización en la base de datos
                _solicitudMedicamentoService.UpdateSolicitudMedicamento(solicitud);

                return Ok(new { Exitoso = true, Mensaje = "Solicitud actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al actualizar la solicitud. {ex.Message}" });
            }
        }
        [HttpGet]
        public ActionResult<List<CategoriaHabitacion>> GetAllCategoriasHabitacion()
        {
            try
            {
                var categorias = _solicitudMedicamentoService.GetAllCategoriasHabitacion();


                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener categorías de habitación. {ex.Message}" });
            }
        }




        private async Task RegistrarMedicamentoDesdeSolicitudAsync(SolicitudMedicamento solicitud, string userId)
        {
            var hospitalizacionProducto = _hospitalizacionRepository.AddMedicamento(new HospitalizacionProducto
            {
                HospitalizacionId = solicitud.HospitalizacionId,
                ProductoId = solicitud.ProductoId,
                UnidadMedidaVentaId = solicitud.UnidadMedidaVentaId,
                PrecioId = solicitud.PrecioId,
                PrecioValor = solicitud.Precio,
                Cantidad = (int)solicitud.Cantidad,
                Indicaciones = solicitud.Indicaciones ?? "",
                ViaAdministracion = solicitud.ViaAdministracion ?? "",
                FrecuenciaAdministracion = solicitud.FrecuenciaAdministracion ?? "",
                FechaHoraAplicacionManual = solicitud.FechaHoraAplicacionManual
            });

            MedicamentoAplicacionHelper.CrearAplicacionesConHorario(
                _hospitalizacionRepository,
                hospitalizacionProducto.Id,
                hospitalizacionProducto.Cantidad,
                solicitud.FrecuenciaAdministracion,
                solicitud.FechaHoraAplicacionManual,
                userId);

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            await _medicamentoNotificacionService.ProgramarNotificacionesAsync(
                solicitud.HospitalizacionId,
                solicitud.NombreProducto ?? "Medicamento",
                solicitud.Cantidad,
                solicitud.Indicaciones ?? "",
                solicitud.ViaAdministracion ?? "",
                solicitud.FrecuenciaAdministracion ?? "",
                solicitud.FechaHoraAplicacionManual,
                baseUrl,
                userId);
        }

        private async Task EnviarNotificacionMedicosAsync(SolicitudMedicamento solicitud, string pacienteNombre, List<string> emailsDestino, HttpRequest request)
        {
            try
            {
                Console.WriteLine($"[EMAIL] Iniciando envío a {emailsDestino.Count} destinatarios");
                if (emailsDestino == null || emailsDestino.Count == 0)
                {
                    Console.WriteLine("[EMAIL] Lista de emails vacía, cancelando envío");
                    return;
                }

                var subject = $"Nueva orden médica - {solicitud.NombreProducto} para {pacienteNombre}";
                var body = $@"
<div style='font-family: Arial, sans-serif;'>
    <h3 style='color: #2c3e50;'>Orden de medicamento aprobada</h3>
    <p><strong>Paciente:</strong> {pacienteNombre}</p>
    <p><strong>Medicamento:</strong> {solicitud.NombreProducto}</p>
    <p><strong>Cantidad:</strong> {solicitud.Cantidad}</p>
    <p><strong>Vía administración:</strong> {solicitud.ViaAdministracion}</p>
    <p><strong>Frecuencia:</strong> {solicitud.FrecuenciaAdministracion}</p>
    <p><strong>Indicaciones:</strong> {solicitud.Indicaciones}</p>
    <hr />
    <small>Este mensaje es automático, por favor no responder.</small>
</div>";

                var baseUrl = $"{request.Scheme}://{request.Host}";
                Console.WriteLine($"[EMAIL] URL base: {baseUrl}");

                using var client = _httpClientFactory.CreateClient();
                using var form = new MultipartFormDataContent();

                form.Add(new StringContent(subject), "Subject");
                form.Add(new StringContent(body), "Body");
                foreach (var email in emailsDestino)
                    form.Add(new StringContent(email), "To");

                Console.WriteLine("[EMAIL] Enviando petición a /api/SendEmail...");
                var response = await client.PostAsync($"{baseUrl}/api/SendEmail", form);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[EMAIL] Código respuesta: {(int)response.StatusCode} - {response.StatusCode}");
                Console.WriteLine($"[EMAIL] Contenido respuesta: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[EMAIL] Error al enviar correo: {responseContent}");
                }
                else
                {
                    Console.WriteLine("[EMAIL] Correo enviado exitosamente");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL] EXCEPCIÓN NO CAPTURADA: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                // No relanzamos la excepción para que no falle el flujo principal
            }
        }



        private Citas ObtenerCitaAsociada(int hospitalizacionId)
        {
            var hospitalizacion = _db.Hospitalizaciones
                .Include(h => h.Paciente)
                .FirstOrDefault(h => h.Id == hospitalizacionId);
            if (hospitalizacion == null) return null;

            var citasDelPaciente = _db.Citass
                .Where(c => c.PacienteId == hospitalizacion.PacienteId && !c.Eliminado)
                .ToList();

            if (!citasDelPaciente.Any()) return null;

            var citaMasCercana = citasDelPaciente
                .Select(c => new
                {
                    Cita = c,
                    Diferencia = c.FechaInicio.HasValue
                        ? Math.Abs((c.FechaInicio.Value - hospitalizacion.FechaInicio).TotalMinutes)
                        : double.MaxValue
                })
                .OrderBy(x => x.Diferencia)
                .FirstOrDefault()?.Cita;

            return citaMasCercana;
        }



        private List<string> ObtenerEmailsMedicosDesdeCita(int hospitalizacionId)
        {
            Console.WriteLine($"[EMAILS] Buscando médicos para hospitalización {hospitalizacionId}");
            var cita = ObtenerCitaAsociada(hospitalizacionId);
            if (cita == null)
            {
                Console.WriteLine("[EMAILS] No se encontró cita asociada.");
                return new List<string>();
            }

            var emailsConRol = new List<string>(); // Lista sin deduplicar

            if (cita.EmpleadoId.HasValue)
            {
                var empleado = _db.Empleados.FirstOrDefault(e => e.Id == cita.EmpleadoId.Value);
                if (empleado != null && !string.IsNullOrEmpty(empleado.Email))
                {
                    emailsConRol.Add(empleado.Email);
                    Console.WriteLine($"  >> Médico Asignado: {empleado.Email}");
                }
            }

            if (cita.PrimerAyudanteId.HasValue)
            {
                var empleado = _db.Empleados.FirstOrDefault(e => e.Id == cita.PrimerAyudanteId.Value);
                if (empleado != null && !string.IsNullOrEmpty(empleado.Email))
                {
                    emailsConRol.Add(empleado.Email);
                    Console.WriteLine($"  >> Primer Ayudante: {empleado.Email}");
                }
            }

            if (cita.SegundoAyudanteId.HasValue)
            {
                var empleado = _db.Empleados.FirstOrDefault(e => e.Id == cita.SegundoAyudanteId.Value);
                if (empleado != null && !string.IsNullOrEmpty(empleado.Email))
                {
                    emailsConRol.Add(empleado.Email);
                    Console.WriteLine($"  >> Segundo Ayudante: {empleado.Email}");
                }
            }

            if (cita.AnestesistaId.HasValue)
            {
                var empleado = _db.Empleados.FirstOrDefault(e => e.Id == cita.AnestesistaId.Value);
                if (empleado != null && !string.IsNullOrEmpty(empleado.Email))
                {
                    emailsConRol.Add(empleado.Email);
                    Console.WriteLine($"  >> Anestesista: {empleado.Email}");
                }
            }

            if (cita.InstrumentistaId.HasValue)
            {
                var empleado = _db.Empleados.FirstOrDefault(e => e.Id == cita.InstrumentistaId.Value);
                if (empleado != null && !string.IsNullOrEmpty(empleado.Email))
                {
                    emailsConRol.Add(empleado.Email);
                    Console.WriteLine($"  >> Instrumentista: {empleado.Email}");
                }
            }

            if (cita.CirculanteId.HasValue)
            {
                var empleado = _db.Empleados.FirstOrDefault(e => e.Id == cita.CirculanteId.Value);
                if (empleado != null && !string.IsNullOrEmpty(empleado.Email))
                {
                    emailsConRol.Add(empleado.Email);
                    Console.WriteLine($"  >> Circulante: {empleado.Email}");
                }
            }

            Console.WriteLine($"[EMAILS] Total de correos a enviar (sin deduplicar): {emailsConRol.Count}");
            return emailsConRol;
        }


        public async Task EnviarRecordatorioMedicamento(int solicitudId, int dosisActual, int cantidadTotal, int intervaloMinutos, string baseUrl)
        {
            try
            {
                var solicitud = _solicitudMedicamentoService.GetSolicitudMedicamentoById(solicitudId);
                if (solicitud == null || solicitud.Estado != "Despachado")
                    return;

                var emails = ObtenerEmailsMedicosDesdeCita(solicitud.HospitalizacionId);
                if (emails.Count == 0) return;

                var pacienteNombre = _db.Hospitalizaciones
                    .Where(h => h.Id == solicitud.HospitalizacionId)
                    .Select(h => h.Paciente.Nombre)
                    .FirstOrDefault() ?? "Paciente";

                var subject = $"Recordatorio de medicación ({dosisActual}/{cantidadTotal}) - {solicitud.NombreProducto} para {pacienteNombre}";
                var body = ObtenerCuerpoCorreo(solicitud, pacienteNombre, dosisActual, cantidadTotal);

                await EnviarCorreoBaseAsync(emails, subject, body, baseUrl, pacienteNombre, solicitud.NombreProducto);

                // Programar siguiente dosis si aún quedan
                if (dosisActual < cantidadTotal)
                {
                    BackgroundJob.Schedule(() =>
                        EnviarRecordatorioMedicamento(solicitudId, dosisActual + 1, cantidadTotal, intervaloMinutos, baseUrl),
                        TimeSpan.FromMinutes(intervaloMinutos));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] EnviarRecordatorioMedicamento: {ex.Message}");
            }
        }
        public async Task EnviarCorreoBaseAsync(
           List<string> emailsDestino,
           string subject,
           string body,
           string baseUrl,
           string pacienteNombre = null,
           string medicamento = null)
        {
            if (emailsDestino == null || emailsDestino.Count == 0)
            {
                Console.WriteLine("[EMAIL] Lista de emails vacía, cancelando envío");
                return;
            }

            int contador = 0;
            foreach (var email in emailsDestino)
            {
                contador++;
                using var client = _httpClientFactory.CreateClient();
                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(subject), "Subject");
                form.Add(new StringContent(body), "Body");
                form.Add(new StringContent(email), "To");

                try
                {
                    Console.WriteLine($"[EMAIL] Enviando correo #{contador} a: {email}");
                    var response = await client.PostAsync($"{baseUrl}/api/SendEmail", form);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[EMAIL] Respuesta para {email}: {(int)response.StatusCode} - {response.StatusCode}");
                    if (!response.IsSuccessStatusCode)
                        Console.WriteLine($"[EMAIL] Error para {email}: {responseContent}");
                    else
                        Console.WriteLine($"[EMAIL] Correo #{contador} enviado exitosamente a {email}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EMAIL] Excepción al enviar a {email}: {ex.Message}");
                }
            }

            // ✅ Guardar notificación en BD (solo una vez por envío, no por cada email)
            if (!string.IsNullOrEmpty(pacienteNombre) && !string.IsNullOrEmpty(medicamento))
            {
                try
                {
                    var horaGuatemala = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, HospitalTimeZone);
                    _db.Notificaciones.Add(new Notificacion
                    {
                        Titulo = "Recordatorio de medicación",
                        Mensaje = $"Administrar {medicamento} al paciente {pacienteNombre}",
                        FechaCreacion = horaGuatemala,
                        Leida = false
                    });
                    await _db.SaveChangesAsync();
                    Console.WriteLine($"[NOTIFICACION] Guardada: {medicamento} para {pacienteNombre}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NOTIFICACION] Error al guardar: {ex.Message}");
                }
            }
        }
        private string ObtenerCuerpoCorreo(SolicitudMedicamento solicitud, string pacienteNombre, int? dosisActual, int? cantidadTotal)
        {
            string dosisInfo = "";
            if (dosisActual.HasValue && cantidadTotal.HasValue)
                dosisInfo = $"<p><strong>Dosis:</strong> {dosisActual} de {cantidadTotal}</p>";

            return $@"
<div style='font-family: Arial, sans-serif;'>
    <h3 style='color: #2c3e50;'>Orden de medicamento aprobada</h3>
    <p><strong>Paciente:</strong> {pacienteNombre}</p>
    {dosisInfo}
    <p><strong>Medicamento:</strong> {solicitud.NombreProducto}</p>
    <p><strong>Cantidad:</strong> {solicitud.Cantidad}</p>
    <p><strong>Vía administración:</strong> {solicitud.ViaAdministracion}</p>
    <p><strong>Frecuencia:</strong> {solicitud.FrecuenciaAdministracion}</p>
    <p><strong>Indicaciones:</strong> {solicitud.Indicaciones}</p>
    <hr />
    <small>Este mensaje es automático, por favor no responder.</small>
</div>";
        }




        private DateTime? ParseFechaHoraAplicacion(string rawDate, TimeZoneInfo hospitalZone)
        {
            if (string.IsNullOrWhiteSpace(rawDate)) return null;

            // Normalizar formatos AM/PM en español
            rawDate = rawDate.Trim()
                .Replace("p. m.", "PM", StringComparison.OrdinalIgnoreCase)
                .Replace("a. m.", "AM", StringComparison.OrdinalIgnoreCase)
                .Replace("p.m.", "PM", StringComparison.OrdinalIgnoreCase)
                .Replace("a.m.", "AM", StringComparison.OrdinalIgnoreCase);

            if (rawDate.Contains("T"))
            {
                string[] isoFormats = { "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm" };
                if (DateTime.TryParseExact(rawDate, isoFormats, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var utcDirect))
                {
                    // Convertir UTC → hora local Guatemala para log
                    var localCheck = TimeZoneInfo.ConvertTimeFromUtc(utcDirect, hospitalZone);
                    Console.WriteLine($"[DEBUG] ISO recibido: {rawDate} → UTC: {utcDirect:yyyy-MM-dd HH:mm:ss} → Guatemala: {localCheck:yyyy-MM-dd HH:mm:ss}");
                    return utcDirect;
                }
            }

            string[] localFormats = {
        "dd/MM/yyyy HH:mm",
        "dd/MM/yyyy H:mm",
        "dd/MM/yyyy hh:mm tt",
        "dd/MM/yyyy h:mm tt",
    };
            var cultures = new[] { CultureInfo.InvariantCulture, new CultureInfo("es-GT") };
            foreach (var culture in cultures)
            {
                if (DateTime.TryParseExact(rawDate, localFormats, culture, DateTimeStyles.None, out var parsedLocalFull))
                {
                    var utc = TimeZoneInfo.ConvertTimeToUtc(parsedLocalFull, hospitalZone);
                    Console.WriteLine($"[DEBUG] Fecha+hora local Guatemala: {parsedLocalFull:yyyy-MM-dd HH:mm:ss} → UTC: {utc:yyyy-MM-dd HH:mm:ss}");
                    return utc;
                }
            }

            string[] timeOnlyFormats = {
        "HH:mm",
        "H:mm",
        "hh:mm tt",
        "h:mm tt",
        "HH:mm:ss",
    };
            foreach (var culture in cultures)
            {
                if (DateTime.TryParseExact(rawDate, timeOnlyFormats, culture, DateTimeStyles.None, out var parsedTime))
                {
                    var todayGuatemala = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hospitalZone).Date;
                    var localDateTime = todayGuatemala.Add(parsedTime.TimeOfDay);
                    var utc = TimeZoneInfo.ConvertTimeToUtc(localDateTime, hospitalZone);
                    Console.WriteLine($"[DEBUG] Solo hora: {rawDate} → Local Guatemala: {localDateTime:yyyy-MM-dd HH:mm:ss} → UTC: {utc:yyyy-MM-dd HH:mm:ss}");
                    return utc;
                }
            }

            Console.WriteLine($"[DEBUG] No se pudo parsear: '{rawDate}'");
            return null;
        }


        [HttpGet]
        public ActionResult GetNotificacionesNoLeidas()
        {
            var notificaciones = _db.Notificaciones
                .Where(n => !n.Leida)
                .OrderByDescending(n => n.FechaCreacion)
                .Select(n => new
                {
                    n.Id,
                    n.Titulo,
                    n.Mensaje,
                    Hora = n.FechaCreacion.ToString("hh:mm tt")
                })
                .ToList();

            return Ok(notificaciones);
        }

        [HttpPost]
        public ActionResult MarcarNotificacionLeida(int id)
        {
            var notif = _db.Notificaciones.Find(id);
            if (notif != null)
            {
                notif.Leida = true;
                _db.SaveChanges();
            }
            return Ok();
        }

    }






}
