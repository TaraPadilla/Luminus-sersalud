using Database.Shared.Data;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text.Json;


namespace farmamest.Controllers
{
    public class UserController : Controller
    {
        private readonly IUser _userRepository;

        public UserController(IUser userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public string ConsultarUsuarios()
        {

            try
            {
                var lista = _userRepository.GetList();

                var result = lista.Select(x => new
                {
                    Id = x.Id,
                    Nombre = x.Persona != null ? x.Persona.NombreYApellidos : "-"
                });

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = result });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al consultar los pacientes. " + ex.Message });
            }
        }
    }
}
