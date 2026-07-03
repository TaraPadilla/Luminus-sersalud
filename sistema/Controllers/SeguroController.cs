using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class SeguroController : Controller
    {
        private readonly ISeguro _seguroRepository;

        public SeguroController
        (
            ISeguro seguroRepository
        )
        {
            _seguroRepository = seguroRepository;
        }

        public IActionResult Nuevo()
        {
            var model = new SeguroViewModel();
            return View(model);
        }
        [HttpPost]
        public string Nuevo(SeguroViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.Nombre))
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "El nombre del seguro es obligatorio."
                });
            }

            try
            {
                var seguro = new Seguro
                {
                    Nombre = model.Nombre,
                    Direccion = model.Direccion,
                    Telefono = model.Telefono,
                    Web = model.Web,
                    Proveedores = null

                };

                _seguroRepository.AddSeguro(seguro);
                TempData["Message"] = "El seguro se ha creado con éxito!";

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al registrar el seguro. " + ex.Message
                });
            }
        }
        public IActionResult Lista()
        {
            var seguros = (_seguroRepository.GetAll());
            Console.WriteLine("Seguros Count: " + seguros.Count);
            return View(seguros);
        }

        public IActionResult Modificar(int seguroId)
        {
            var seguro = _seguroRepository.GetSeguro(seguroId);
            var model = new SeguroViewModel
            {
                Id = seguro.Id,
                Nombre = seguro.Nombre,
                Direccion = seguro.Direccion,
                Telefono = seguro.Telefono,
                Web = seguro.Web
            };
            return View(model);
        }
        [HttpPost]
        public string ModificarSeguro(SeguroViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.Nombre))
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "El nombre del seguro es obligatorio."
                });
            }

            try
            {
                var seguro = _seguroRepository.GetSeguro(model.Id);
                seguro.Nombre = model.Nombre;
                seguro.Direccion = model.Direccion;
                seguro.Telefono = model.Telefono;
                seguro.Web = model.Web;

                _seguroRepository.UpdateSeguro(seguro);

                TempData["Message"] = "El seguro se ha modificado con éxito";

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al modificar el seguro. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string EliminarSeguro(int? seguroId)
        {
            try
            {
                _seguroRepository.DeleteSeguro((int)seguroId, HttpContext.User);

                TempData["Message"] = "El seguro se ha eliminado con éxito";

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al eliminar el seguro. " + ex.Message
                });
            }
        }
    }
}
