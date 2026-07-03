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
    public class DevolucionMedicamentoController : Controller
    {
        private readonly IDevolucionMedicamentoService _devolucionService;
        private readonly UserManager<User> _userManager;
        private readonly IUser _userRepository;
        private readonly IEmpleado _empleadoRepository;

        public DevolucionMedicamentoController(
            IDevolucionMedicamentoService devolucionService,
            UserManager<User> userManager,
            IUser userRepository,
            IEmpleado empleadoRepository)
        {
            _devolucionService = devolucionService;
            _userManager = userManager;
            _userRepository = userRepository;
            _empleadoRepository = empleadoRepository;
        }

        // --- Métodos comunes para obtener datos del usuario ---

        // Método para obtener el ID del usuario actual
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

        // Método para obtener el Nombre y Apellido de un usuario dado su ID (lógica interna)
        private string GetFullName(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return string.Empty;
            var user = _userRepository.GetbyId(userId);
            if (user == null)
                return string.Empty;
            var empleadoId = user.EmpleadoId;
            if (empleadoId == null)
                return "Admin";
            var empleado = _empleadoRepository.Get((int)empleadoId);
            if (empleado == null)
                return string.Empty;
            return empleado.NombreYApellidos;
        }

        // Endpoint para obtener el nombre completo mediante el ID (opcional, expuesto también)
        [HttpGet]
        public ActionResult<string> GetUserFullNameByEmployeeId(string userId)
        {
            try
            {
                var fullName = GetFullName(userId);
                if (string.IsNullOrEmpty(fullName))
                    return NotFound(new { Exitoso = false, Mensaje = "Usuario o empleado no encontrado." });
                return Ok(new { Exitoso = true, NombreCompleto = fullName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener el nombre del usuario. {ex.Message}" });
            }
        }

        // --- Endpoints de Devoluciones ---

        // Obtener todas las devoluciones (opcionalmente filtradas por hospitalización)
        [HttpGet]
        public ActionResult<List<DevolucionMedicamento>> GetAll(int? hospitalizacionId = null)
        {
            try
            {
                var devoluciones = _devolucionService.GetAllDevolucionesMedicamento(hospitalizacionId);
                // Transformar los datos para mostrar los nombres completos en lugar de IDs
                foreach (var d in devoluciones)
                {
                    d.UsuarioSolicitanteNombre = GetFullName(d.UsuarioSolicitanteId);
                    if (!string.IsNullOrEmpty(d.UsuarioAprobadorId))
                    {
                        // Se asume que se agregó la propiedad UsuarioAprobadorNombre en el modelo (o se puede setear en otro campo)
                        d.UsuarioAprobadorNombre = GetFullName(d.UsuarioAprobadorId);
                    }
                }
                return Ok(devoluciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener devoluciones: {ex.Message}" });
            }
        }

        // Obtener una devolución por ID
        [HttpGet]
        public ActionResult<DevolucionMedicamento> GetById(int id)
        {
            try
            {
                var devolucion = _devolucionService.GetDevolucionMedicamentoById(id);
                if (devolucion == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Devolución no encontrada." });

                devolucion.UsuarioSolicitanteNombre = GetFullName(devolucion.UsuarioSolicitanteId);
                if (!string.IsNullOrEmpty(devolucion.UsuarioAprobadorId))
                {
                    devolucion.UsuarioAprobadorNombre = GetFullName(devolucion.UsuarioAprobadorId);
                }
                return Ok(devolucion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener la devolución: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] DevolucionMedicamento devolucion)
        {
            try
            {
                Console.WriteLine("Iniciando método Add de DevolucionMedicamento...");

                if (devolucion == null)
                {
                    Console.WriteLine("Devolución recibida es null.");
                    return BadRequest(new { Exitoso = false, Mensaje = "Devolución inválida." });
                }
                
                // Mostrar datos recibidos para la depuración
                Console.WriteLine($"Datos recibidos: HospitalizacionId = {devolucion.HospitalizacionId}, ProductoId = {devolucion.ProductoId}, HospitalizacionProductoAplicacionId = {devolucion.HospitalizacionProductoAplicacionId}");

                // Obtener todas las devoluciones para la hospitalización
                var devoluciones = _devolucionService.GetDevolucionesByHospitalizacionId(devolucion.HospitalizacionId);
                Console.WriteLine($"Total de devoluciones encontradas para HospitalizacionId {devolucion.HospitalizacionId}: {devoluciones.Count}");

                // Log de cada devolución encontrada
                foreach (var d in devoluciones)
                {
                    Console.WriteLine($"Devolución existente - HospitalizacionProductoAplicacionId: {d.HospitalizacionProductoAplicacionId}, Estado: {d.Estado}, FechaSolicitud: {d.FechaSolicitud}");
                }
                
                // Verificar duplicados: se considera duplicado si para esa misma aplicación del producto ya existe una devolución
                // cuyo estado sea "En espera" o "Aprobada"
                bool exists = devoluciones.Any(d => d.HospitalizacionProductoAplicacionId == devolucion.HospitalizacionProductoAplicacionId 
                                                && (d.Estado == "En espera" || d.Estado == "Aprobada"));
                Console.WriteLine($"Verificación de duplicado para HospitalizacionProductoAplicacionId {devolucion.HospitalizacionProductoAplicacionId}: {exists}");
                
                if (exists)
                {
                    Console.WriteLine($"Ya existe una devolución para la aplicación del producto {devolucion.HospitalizacionProductoAplicacionId} en la hospitalización {devolucion.HospitalizacionId} con estado 'En espera' o 'Aprobada'. Abortando creación.");
                    return BadRequest(new { Exitoso = false, Mensaje = "Ya existe una devolución para este producto en la hospitalización." });
                }

                // Obtener el usuario autenticado que solicita la devolución
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    Console.WriteLine("Usuario no autenticado. Abortando creación de devolución.");
                    return Unauthorized(new { Exitoso = false, Mensaje = "Usuario no autenticado." });
                }
                Console.WriteLine($"Usuario autenticado: {user.Id}");

                devolucion.UsuarioSolicitanteId = user.Id;
                devolucion.UsuarioSolicitanteNombre = GetFullName(user.Id);
                devolucion.FechaSolicitud = DateTime.Now;
                Console.WriteLine($"Fecha de solicitud asignada: {devolucion.FechaSolicitud}");

                // Llamada al servicio para agregar la devolución
                _devolucionService.AddDevolucionMedicamento(devolucion);
                Console.WriteLine("Devolución agregada exitosamente en el servicio.");

                Console.WriteLine("Proceso Add completado exitosamente.");
                return Ok(new { Exitoso = true, Mensaje = "Devolución creada exitosamente.", FechaSolicitud = devolucion.FechaSolicitudFormatted });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en método Add: " + ex.Message);
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al crear la devolución: {ex.Message}" });
            }
        }

        // Actualizar una devolución
        [HttpPut]
        public ActionResult Update(int id, [FromBody] DevolucionMedicamento devolucion)
        {
            try
            {
                if (devolucion == null || id != devolucion.Id)
                    return BadRequest(new { Exitoso = false, Mensaje = "Datos de devolución inválidos." });

                var existente = _devolucionService.GetDevolucionMedicamentoById(id);
                if (existente == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Devolución no encontrada." });

                _devolucionService.UpdateDevolucionMedicamento(devolucion);
                return Ok(new { Exitoso = true, Mensaje = "Devolución actualizada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al actualizar la devolución: {ex.Message}" });
            }
        }

        // Eliminar una devolución
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                var devolucion = _devolucionService.GetDevolucionMedicamentoById(id);
                if (devolucion == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Devolución no encontrada." });

                _devolucionService.DeleteDevolucionMedicamento(id);
                return Ok(new { Exitoso = true, Mensaje = "Devolución eliminada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al eliminar la devolución: {ex.Message}" });
            }
        }

        // Procesar (aprobar o rechazar) una devolución
        [HttpPut]
        public async Task<ActionResult> ProcesarDevolucion(int id, [FromBody] bool rechazada)
        {
            try
            {
                // Obtener la devolución
                var devolucion = _devolucionService.GetDevolucionMedicamentoById(id);
                if (devolucion == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Devolución no encontrada." });

                // Obtener el usuario que procesa la devolución
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                    return Unauthorized(new { Exitoso = false, Mensaje = "Usuario no autenticado." });

                devolucion.UsuarioAprobadorId = user.Id;
                devolucion.FechaAprobacion = DateTime.Now;
                devolucion.Estado = rechazada ? "Rechazada" : "Aprobada";
                // Actualizar el nombre del aprobador para mejorar la respuesta
                devolucion.UsuarioAprobadorNombre = GetFullName(user.Id);

                _devolucionService.UpdateDevolucionMedicamento(devolucion);

                return Ok(new
                {
                    Exitoso = true,
                    Mensaje = $"Devolución procesada como '{devolucion.Estado}'.",
                    FechaAprobacion = devolucion.FechaAprobacion,
                    UsuarioAprobadorNombre = devolucion.UsuarioAprobadorNombre
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al procesar la devolución: {ex.Message}" });
            }
        }

        // Obtener devoluciones por hospitalización
        [HttpGet]
        public ActionResult<List<DevolucionMedicamento>> GetByHospitalizacion(int hospitalizacionId)
        {
            try
            {
                var devoluciones = _devolucionService.GetDevolucionesByHospitalizacionId(hospitalizacionId);
                foreach (var d in devoluciones)
                {
                    d.UsuarioSolicitanteNombre = GetFullName(d.UsuarioSolicitanteId);
                    if (!string.IsNullOrEmpty(d.UsuarioAprobadorId))
                    {
                        d.UsuarioAprobadorNombre = GetFullName(d.UsuarioAprobadorId);
                    }
                }
                return Ok(devoluciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener devoluciones: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult DevolucionesMedicamentosView(int? hospitalizacionId = null)
        {
            try
            {
                // Obtener la lista de devoluciones (puedes aplicar el filtro por hospitalización si es necesario)
                var devoluciones = _devolucionService.GetAllDevolucionesMedicamento(hospitalizacionId);

                // Opcional: Transformar los datos para incluir nombres completos
                foreach (var d in devoluciones)
                {
                    d.UsuarioSolicitanteNombre = GetFullName(d.UsuarioSolicitanteId);
                    if (!string.IsNullOrEmpty(d.UsuarioAprobadorId))
                    {
                        d.UsuarioAprobadorNombre = GetFullName(d.UsuarioAprobadorId);
                    }
                }

                // Pasar el hospitalizacionId a la vista mediante ViewBag, si se desea
                ViewBag.HospitalizacionId = hospitalizacionId;

                // Retornar la vista junto con la lista de devoluciones
                return View(devoluciones);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las devoluciones: {ex.Message}";
                return RedirectToAction("Error");
            }
        }
        
        [HttpGet]
        public ActionResult<List<DevolucionMedicamento>> GetAllDevolucionesMedicamento(int hospitalizacionId)
        {
            try
            {
                var devoluciones = _devolucionService.GetAllDevolucionesMedicamento(hospitalizacionId);
                return Ok(devoluciones);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al obtener devoluciones: {ex.Message}" });
            }
        }
    }
}
