using Database.Shared.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sistema.Controllers;
using System;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class NotaEvolucionController : Controller
    {

        private readonly INotaEvolucionService _notaEvolucionService;

        private readonly UserManager<User> _userManager;

        public NotaEvolucionController(INotaEvolucionService notaEvolucionService, UserManager<User> userManager)
        {
            _notaEvolucionService = notaEvolucionService;

            _userManager = userManager;
        }

        [HttpPost]
        public string Nuevo(NotaEvolucion entity)
        {

            try
            {
                entity.UserId = _userManager.GetUserId(HttpContext.User);

                _notaEvolucionService.AddNotaEvolucion(entity);

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
                    resultado = "Error al crear una nota de evolucion"

                });
            }

        }

        [HttpPost]
        public string ListaNotaEvolucion(int hospitalizacionId)
        {

            try
            {
                var result = _notaEvolucionService.GetNotaEvolucionListByHospitalizacionId(hospitalizacionId);

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
                    exitoso = true,
                    mensaje = "Error al obtener las notas de evolución"

                });
            }

        }



    }
}
