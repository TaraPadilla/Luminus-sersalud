using Database.Shared.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class IngestaExcretaController : Controller
    {
        private readonly IIngestaExcretaService _ingestaExcretaService;

        private readonly UserManager<User> _userManager;

        public IngestaExcretaController(IIngestaExcretaService ingestaExcretaService, UserManager<User> userManager)
        {
            _ingestaExcretaService = ingestaExcretaService;

            _userManager = userManager;
        }

        [HttpPost]
        public string Nuevo(IngestaExcreta2 entity)
        {

            try
            {
                entity.UserId = _userManager.GetUserId(HttpContext.User);

                _ingestaExcretaService.Add(entity);

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
                    resultado = "Error al crear una ingesta/excreta"

                });
            }

        }

        [HttpPost]
        public string ListaIngestaExcreta(int hospitalizacionId)
        {

            try
            {
                var result = _ingestaExcretaService.GetListByHospitalizacionId(hospitalizacionId);

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
                    mensaje = "Error al obtener las ingesta/Excreta"

                });
            }

        }
    }
}
