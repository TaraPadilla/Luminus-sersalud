using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Shared;
using Database.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema.Services.WebAuthn;

namespace sistema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenesMedicasController : ControllerBase
    {
        private readonly Context dbContext;

        private readonly IWebAuthnService _webAuthn;
        private readonly Context _db;

        private readonly UserManager<User> _userManager;


        public OrdenesMedicasController(Context context, IWebAuthnService webAuthn, UserManager<User> userManager)
        {
            dbContext = context;
            _webAuthn = webAuthn;
            _userManager = userManager;
        }

        [HttpPost("AgregarOrdenMedica")]
        public IActionResult AgregarOrdenMedica([FromBody] OrdenesMedicas nuevaOrden)
        {
            var hospitalizacion = dbContext.Hospitalizaciones
                .Include(h => h.OrdenesMedicas)
                .FirstOrDefault(h => h.Id == nuevaOrden.HospitalizacionId);

            if (hospitalizacion == null || hospitalizacion.Finalizada)
            {
                return BadRequest("No se puede agregar órdenes médicas a una hospitalización finalizada o inexistente.");
            }
            nuevaOrden.FechaHora = DateTime.Now;
            nuevaOrden.Autorizado = false;
            hospitalizacion.OrdenesMedicas.Add(nuevaOrden);
            dbContext.SaveChanges();

            return Ok(new { id = nuevaOrden.Id });
        }

        // [HttpGet("ObtenerOrdenesPorHospitalizacion/{hospitalizacionId}")]
        // public IActionResult ObtenerOrdenesPorHospitalizacion(int hospitalizacionId)
        // {
        //     var ordenes = dbContext.OrdenesMedicas
        //         .Where(o => o.HospitalizacionId == hospitalizacionId)
        //         .OrderBy(o => o.Realizada) 
        //         .ThenByDescending(o => !o.Realizada ? o.FechaHora : DateTime.MinValue) 
        //         .ThenBy(o => !o.Realizada ? o.FechaHora : DateTime.MaxValue) 
        //         .ThenByDescending(o => o.Realizada ? o.FechaHora : DateTime.MinValue)
        //         .ToList();

        //     return Ok(ordenes);
        // }

        [HttpGet("ObtenerOrdenesPorHospitalizacion/{hospitalizacionId}")]
        public IActionResult ObtenerOrdenesPorHospitalizacion(int hospitalizacionId)
        {
            var hospitalizacion = dbContext.Hospitalizaciones
                .FirstOrDefault(h => h.Id == hospitalizacionId);

            if (hospitalizacion == null)
                return NotFound("Hospitalización no encontrada.");

            var ordenes = dbContext.OrdenesMedicas
                .Where(o => o.HospitalizacionId == hospitalizacionId)
                .OrderBy(o => o.Realizada)
                .ThenBy(o => o.FechaHora)
                .Select(o => new
                {
                    o.Id,
                    o.FechaHora,
                    o.Profesional,
                    o.Descripcion,
                    o.HospitalizacionId,
                    o.Realizada,
                    o.FechaRealizacion,
                    o.ProfesionalRealiza,
                    o.Examenes,
                    o.Dietas,
                    o.Observaciones,
                    o.Autorizado,
                    o.UsuarioAutoriza,
                    o.FechaAutorizacion
                })
                .ToList();

            return Ok(ordenes);
        }


        [HttpPut("ActualizarOrdenMedica/{id}")]
        public IActionResult ActualizarOrdenMedica(int id, [FromBody] OrdenesMedicas ordenActualizada)
        {
            var orden = dbContext.OrdenesMedicas.Include(o => o.Examenes).FirstOrDefault(o => o.Id == id);

            if (orden == null)
            {
                return NotFound("Orden médica no encontrada.");
            }

            orden.Profesional = ordenActualizada.Profesional;
            orden.Descripcion = ordenActualizada.Descripcion;
            orden.Examenes = ordenActualizada.Examenes;
            dbContext.SaveChanges();

            return Ok(orden);
        }

        [HttpPut("AplicarOrdenMedica/{id}")]
        public IActionResult AplicarOrdenMedica(int id, [FromBody] OrdenesMedicas ordenActualizada)
        {
            var orden = dbContext.OrdenesMedicas.FirstOrDefault(o => o.Id == id);

            if (orden == null)
            {
                return NotFound("Orden médica no encontrada.");
            }

            orden.Realizada = ordenActualizada.Realizada;
            orden.FechaRealizacion = DateTime.Now;
            orden.ProfesionalRealiza = ordenActualizada.ProfesionalRealiza;
            dbContext.SaveChanges();

            return Ok(orden);
        }

        [HttpDelete("EliminarOrdenMedica/{id}")]
        public IActionResult EliminarOrdenMedica(int id)
        {
            var orden = dbContext.OrdenesMedicas.FirstOrDefault(o => o.Id == id);

            if (orden == null)
            {
                return NotFound("Orden médica no encontrada.");
            }

            dbContext.OrdenesMedicas.Remove(orden);
            dbContext.SaveChanges();

            return Ok("Orden médica eliminada exitosamente.");
        }


        [HttpPost("AutorizarOrdenMedica")]
        public async Task<IActionResult> AutorizarOrdenMedica([FromBody] AutorizarOrdenRequest request)
        {
            try
            {
                if (request?.HuellaPayload == null)
                    return BadRequest(new { exitoso = false, resultado = "Payload de huella no encontrado." });

                // Validar credencial
                var credIdString = Convert.ToBase64String(
                    Convert.FromBase64String(
                        request.HuellaPayload.RawId.Replace('-', '+').Replace('_', '/')
                        + new string('=', (4 - request.HuellaPayload.RawId.Length % 4) % 4)
                    )
                );
                var credencial = await dbContext.WebAuthnCredentials
                    .FirstOrDefaultAsync(c => c.DescriptorId == credIdString);
                if (credencial == null)
                    return Unauthorized(new { exitoso = false, resultado = "Credencial no registrada." });

                var authorizerId = credencial.UserId;

                // Verificar firma
                var verificacion = await _webAuthn.CompleteVerifyAsync(authorizerId, request.HuellaPayload);
                if (!verificacion.Success)
                    return Unauthorized(new { exitoso = false, resultado = verificacion.UserMessage });

                // Obtener orden
                var orden = await dbContext.OrdenesMedicas.FindAsync(request.OrdenMedicaId);
                if (orden == null)
                    return NotFound(new { exitoso = false, resultado = "Orden no encontrada." });

                // Validar permisos
                var authorizerUser = await _userManager.FindByIdAsync(authorizerId);
                var esAdmin = await _userManager.IsInRoleAsync(authorizerUser, "Administrador");

                if (!esAdmin)
                {
                    var citasId = await dbContext.Consultas
                        .Where(c => c.HospitalizacionId == orden.HospitalizacionId)
                        .Select(c => c.CitasId)
                        .FirstOrDefaultAsync();

                    if (citasId == null)
                        return BadRequest(new { exitoso = false, resultado = "No se encontró la consulta de esta hospitalización." });

                    var cita = await dbContext.Citass
                        .Where(c => c.Id == citasId.Value)
                        .Select(c => new { c.EmpleadoId, c.MedicosSecundarios })
                        .FirstOrDefaultAsync();

                    if (cita == null)
                        return BadRequest(new { exitoso = false, resultado = "No se encontró la cita asociada." });

                    var empleadosAutorizados = new List<int>();
                    if (cita.EmpleadoId.HasValue)
                        empleadosAutorizados.Add(cita.EmpleadoId.Value);
                    if (cita.MedicosSecundarios != null)
                        empleadosAutorizados.AddRange(cita.MedicosSecundarios);

                    var userIdsAutorizados = await dbContext.Usuarios
                        .Where(u => u.EmpleadoId != null && empleadosAutorizados.Contains(u.EmpleadoId.Value))
                        .Select(u => u.Id)
                        .ToListAsync();

                    if (!userIdsAutorizados.Contains(authorizerId))
                        return Unauthorized(new { exitoso = false, resultado = "No tiene permisos para autorizar esta orden." });
                }

                // Autorizar
                orden.Autorizado = true;
                orden.UsuarioAutoriza = authorizerId;
                orden.FechaAutorizacion = DateTime.Now;
                await dbContext.SaveChangesAsync();

                return Ok(new { exitoso = true });
            }
            catch (Exception ex)
            {
                // Registrar el error en un log (puedes usar ILogger)
                Console.WriteLine($"Error en AutorizarOrdenMedica: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { exitoso = false, resultado = ex.Message });
            }
        }
        // DTO para recibir la solicitud
        public class AutorizarOrdenRequest
        {
            public int OrdenMedicaId { get; set; }
            public WebAuthnAssertionPayload HuellaPayload { get; set; }
        }
    }
}
