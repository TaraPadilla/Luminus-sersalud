using Database.Shared.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sistema.Controllers;
using System;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class InfoIngestaController : Controller
    {
        private readonly IInfoIngestaService _infoIngestaService;
        private readonly UserManager<User> _userManager;

        public InfoIngestaController(IInfoIngestaService infoIngestaService, UserManager<User> userManager)
        {
            _infoIngestaService = infoIngestaService;
            _userManager = userManager;
        }

        // Método para crear un nuevo registro de InfoIngesta
        [HttpPost]
        public string Nuevo(InfoIngesta entity)
        {
            try
            {
                // Asignar el UserId del profesional (usuario actual)
                entity.UserId = _userManager.GetUserId(HttpContext.User);

                // Llamar al servicio para agregar el nuevo registro de InfoIngesta
                _infoIngestaService.AddInfoIngesta(entity);

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
                    resultado = "Error al crear un registro de InfoIngesta"
                });
            }
        }

        // Método para obtener los registros de InfoIngesta por IngestaExcretaId
        [HttpPost]
        public string ListaInfoIngesta(int ingestaExcretaId)
        {
            try
            {
                // Obtener los registros de InfoIngesta según el IngestaExcretaId
                var result = _infoIngestaService.GetInfoIngestaByIngestaExcretaId(ingestaExcretaId);

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
                    mensaje = "Error al obtener los registros de InfoIngesta"
                });
            }
        }
    }
}
