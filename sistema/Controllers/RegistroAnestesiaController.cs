using Database.Shared.Models;
using farmamest.Helpers;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace farmamest.Controllers
{
    public class RegistroAnestesiaController : Controller
    {
        private readonly IRegistroAnestesiaService _service;
        private readonly UserManager<User> _userManager;

        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        private static readonly JsonSerializerOptions _jsonInputOpts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public RegistroAnestesiaController(
            IRegistroAnestesiaService service,
            UserManager<User> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpPost]
        public string Obtener(int hospitalizacionId)
        {
            try
            {
                var registro = _service.GetByHospitalizacionId(hospitalizacionId);
                return JsonSerializer.Serialize(new { exitoso = true, resultado = registro }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, resultado = ex.Message }, _jsonOpts);
            }
        }

        [HttpPost]
        public async Task<string> Guardar()
        {
            try
            {
                var model = await HttpRequestJsonHelper.LeerCuerpoJsonAsync<RegistroAnestesiaInputVM>(
                    Request, this, _jsonInputOpts);

                if (model == null || model.HospitalizacionId == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "HospitalizacionId inválido." }, _jsonOpts);

                if (string.IsNullOrWhiteSpace(model.DatosJson))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "No hay datos para guardar." }, _jsonOpts);

                var userId = _userManager.GetUserId(HttpContext.User);
                var registro = _service.Guardar(model.HospitalizacionId, userId, model.DatosJson);

                return JsonSerializer.Serialize(new { exitoso = true, resultado = registro?.Id }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, resultado = ex.Message }, _jsonOpts);
            }
        }
    }

    public class RegistroAnestesiaInputVM
    {
        public int HospitalizacionId { get; set; }
        public string DatosJson { get; set; }
    }
}
