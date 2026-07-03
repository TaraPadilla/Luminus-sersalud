using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Mvc;
using sistema.Controllers;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class HospitalizacionUsuarioAccesoController : Controller
    {
        private readonly IHospitalizacionUsuarioAccesoService _hospitalizacionUsuarioAccesoService;

        public HospitalizacionUsuarioAccesoController(IHospitalizacionUsuarioAccesoService hospitalizacionUsuarioAccesoService)
        {
            _hospitalizacionUsuarioAccesoService = hospitalizacionUsuarioAccesoService;
        }

        [HttpPost]
        public string GetAllByIdHospitalizacion(int hospitalizacionId)
        {
            try
            {
                var result = _hospitalizacionUsuarioAccesoService.GetAllByHospitalizacionId(hospitalizacionId);
                return JsonSerializer.Serialize(new
                {
                    exitoso = true,
                    resultado = result
                });
            }
            catch (System.Exception)
            {

                return JsonSerializer.Serialize(new
                {
                    exitoso = false
                });
            }
        }

        [HttpPost]
        public string Agregar([FromBody] HospitalizacionUsuarioAcceso entity)
        {
            try
            {
                _hospitalizacionUsuarioAccesoService.Add(entity);
                return JsonSerializer.Serialize(new
                {
                    exitoso = true
                });
            }
            catch (System.Exception)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false
                });
            }
        }
        [HttpPost]
        public string ActualizarPermiso(HospitalizacionUsuarioAccesoViewModels model)
        {
            try
            {
                _hospitalizacionUsuarioAccesoService.UpdateUsuarioAcceso(model);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true
                });
            }
            catch
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false
                });
            }
        }
        [HttpPost]
        public string Eliminar(int id)
        {
            try
            {
                _hospitalizacionUsuarioAccesoService.Delete(id);
                return JsonSerializer.Serialize(new
                {
                    exitoso = true
                });
            }
            catch (System.Exception)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    mensaje = "Error al eliminar el usuario."
                });
            }
        }

    }
}