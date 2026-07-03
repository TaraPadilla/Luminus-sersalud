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

namespace farmamest.Controllers
{
    public class SolicitudMedicamentoNacionalController : Controller
    {
        private readonly ISolicitudMedicamentoNacionalService _solicitudMedicamentoNacionalService;
        private readonly UserManager<User> _userManager;
        private readonly IUser _userRepository;
        private readonly IEmpleado _empleadoRepository;
        private readonly IProducto _productosRepository = null;

        public SolicitudMedicamentoNacionalController(
            ISolicitudMedicamentoNacionalService solicitudMedicamentoNacionalService,
            UserManager<User> userManager,
            IUser userRepository,
            IEmpleado empleadoRepository,
            IProducto productosRepository)
        {
            _solicitudMedicamentoNacionalService = solicitudMedicamentoNacionalService;
            _userManager = userManager;
            _userRepository = userRepository;
            _empleadoRepository = empleadoRepository;
            _productosRepository = productosRepository;
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
                var solicitudes = _solicitudMedicamentoNacionalService.GetAllSolicitudesMedicamento(hospitalizacionId);

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
                var solicitud = _solicitudMedicamentoNacionalService.GetSolicitudMedicamentoById(id);
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
        public async Task<ActionResult> Add([FromBody] SolicitudMedicamentoNacional solicitud)
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

                _solicitudMedicamentoNacionalService.AddSolicitudMedicamento(solicitud);

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
        public ActionResult Update(int id, [FromBody] SolicitudMedicamentoNacional solicitud)
        {
            try
            {
                if (solicitud == null || id != solicitud.Id)
                    return BadRequest(new { Exitoso = false, Mensaje = "Datos de solicitud inválidos." });

                var solicitudExistente = _solicitudMedicamentoNacionalService.GetSolicitudMedicamentoById(id);
                if (solicitudExistente == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Solicitud no encontrada." });

                _solicitudMedicamentoNacionalService.UpdateSolicitudMedicamento(solicitud);
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
                var solicitud = _solicitudMedicamentoNacionalService.GetSolicitudMedicamentoById(id);
                if (solicitud == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Solicitud no encontrada." });

                _solicitudMedicamentoNacionalService.DeleteSolicitudMedicamento(id);
                return Ok(new { Exitoso = true, Mensaje = "Solicitud eliminada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al eliminar la solicitud. {ex.Message}" });
            }
        }

        // ✅ 6. Obtener solicitudes por hospitalización
        [HttpGet]
        public ActionResult<List<SolicitudMedicamentoNacional>> GetByHospitalizacion(int hospitalizacionId)
        {
            try
            {
                return Ok(_solicitudMedicamentoNacionalService.GetSolicitudesByHospitalizacionId(hospitalizacionId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener solicitudes. {ex.Message}" });
            }
        }

        // 🔄 Marca una solicitud como "Despachado" o "Rechazado", registrando el usuario y la hora de despacho.
        [HttpPut]
        public ActionResult MarkAsDispensed(int id, [FromBody] bool refused)
        {
            try
            {
                // ✅ Obtener la solicitud de medicamento
                var solicitud = _solicitudMedicamentoNacionalService.GetSolicitudMedicamentoById(id);
                if (solicitud == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Solicitud no encontrada." });

                // ✅ Obtener el ID del usuario autenticado (quien marca como despachado)
                var userId = _userManager.GetUserId(HttpContext.User);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Exitoso = false, Mensaje = "Usuario no autenticado." });

                // ✅ Actualizar valores en la solicitud
                solicitud.UsuarioDespachanteId = userId;
                solicitud.FechaDespacho = DateTime.Now;
                solicitud.Estado = refused ? "Rechazado" : "Despachado";

                // ✅ Guardar cambios en la base de datos
                _solicitudMedicamentoNacionalService.UpdateSolicitudMedicamento(solicitud);

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
        public IActionResult SolicitudesMedicamentosNacionalView(int? hospitalizacionId = null)
        {
            try
            {
                var solicitudes = _solicitudMedicamentoNacionalService.GetAllSolicitudesMedicamento(hospitalizacionId);
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
        public ActionResult AprobarTodasSolicitudesPorHospitalizacion([FromQuery] int hospitalizacionId)
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Exitoso = false, Mensaje = "Usuario no autenticado." });

                Console.WriteLine($"HospitalizacionId recibido: {hospitalizacionId} (Tipo: {hospitalizacionId.GetType()})");

                var todasSolicitudes = _solicitudMedicamentoNacionalService.GetAllSolicitudesMedicamento(hospitalizacionId);
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

                    _solicitudMedicamentoNacionalService.UpdateSolicitudMedicamento(solicitud);
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
                var solicitud = _solicitudMedicamentoNacionalService.GetSolicitudMedicamentoById(id);
                if (solicitud == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Solicitud no encontrada." });

                // 2. Actualizar solo la propiedad EsRegistroHospitalizacion
                solicitud.EsRegistroHospitalizacion = true;

                // 3. Guardar la actualización en la base de datos
                _solicitudMedicamentoNacionalService.UpdateSolicitudMedicamento(solicitud);

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
                var categorias = _solicitudMedicamentoNacionalService.GetAllCategoriasHabitacion();


                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener categorías de habitación. {ex.Message}" });
            }
        }

    }


}
