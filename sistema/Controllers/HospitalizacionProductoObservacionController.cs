using Database.Shared.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class HospitalizacionProductoObservacionController : Controller
    {
        private readonly IHospitalizacionProductoObservacionService _service;
        private readonly UserManager<User> _userManager;

        public HospitalizacionProductoObservacionController(
            IHospitalizacionProductoObservacionService service,
            UserManager<User> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        // Crear una nueva observación
        [HttpPost]
        public string Nuevo(HospitalizacionProductoObservacion entity)
        {
            try
            {
                // entity.UsuarioCreaId = _userManager.GetUserId(HttpContext.User);
                // entity.FechaCreacion = DateTime.Now;

                _service.Add(entity);

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
                    exitoso = false,
                    mensaje = "Error al crear la observación"
                });
            }
        }

        // Modificar una observación existente
        [HttpPost]
        public string Modificar(HospitalizacionProductoObservacion entity)
        {
            try
            {
                _service.Update(entity);

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
                    exitoso = false,
                    mensaje = "Error al modificar la observación"
                });
            }
        }

        // Obtener lista de observaciones por HospitalizacionProductoAplicacionId
        [HttpGet]
        public string ListaObservaciones(int hospitalizacionProductoAplicacionId)
        {
            try
            {
                var result = _service.GetByHospitalizacionProductoAplicacionId(hospitalizacionProductoAplicacionId);

                return JsonSerializer.Serialize(new
                {
                    exitoso = true,
                    resultado = result
                });
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    mensaje = "Error al obtener las observaciones"
                });
            }
        }

        // Eliminar (lógicamente) una observación
        [HttpPost]
        public string Eliminar(int id)
        {
            try
            {
                _service.Delete(id);

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
                    exitoso = false,
                    mensaje = "Error al eliminar la observación"
                });
            }
        }
    }
}
