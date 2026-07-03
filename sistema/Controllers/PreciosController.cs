using System.ComponentModel.Design;
using System.IO.MemoryMappedFiles;
using System.Linq.Expressions;
using System.Reflection;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using sistema.Models;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Http;
using sistema.Json;
using Wkhtmltopdf.NetCore;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Rotativa.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace sistema.Controllers
{
    public class PreciosController : Controller
    {
        private readonly IPrecios _preciosRepository = null;

        public PreciosController(IPrecios preciosRepository)
        {
            _preciosRepository = preciosRepository;
        }

        public IActionResult Lista()
        {
            return View();
        }
        [HttpPost]
        public string ConsultarPrecios()
        {
            try
            {
                var preciosConsultados = new List<PrecioViewModel>();
                var preciosBd = _preciosRepository.GetList();
                if (preciosBd != null)
                {
                    foreach (var precio in preciosBd)
                    {
                        preciosConsultados.Add(new PrecioViewModel
                        {
                            PrecioId = precio.Id,
                            NombrePrecio = precio.NombrePrecio
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = preciosConsultados
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar precios. " + ex.Message
                });
            }
        }
        public IActionResult Nuevo()
        {
            var model = new PrecioViewModel();
            return View(model);
        }
        [HttpPost]
        public string Nuevo(PrecioViewModel model)
        {
            try
            {
                var precio = new Precio
                {
                    NombrePrecio = model.NombrePrecio,
                    Eliminado = false
                };
                _preciosRepository.Add(precio);
                TempData["Message"] = "El precio se ha registrado";
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
                    Mensaje = "Error al registrar precio. " + ex.Message
                });
            }
        }
        public IActionResult Modificar(int? precioId)
        {
            if (precioId == null)
            {
                TempData["Message"] = "Error de navegación";
                return RedirectToAction("Lista");
            }
            var precio = _preciosRepository.Get((int)precioId);
            if (precio == null)
            {
                TempData["Message"] = "El precio no existe";
                return RedirectToAction("Lista");
            }
            var model = new PrecioViewModel
            {
                PrecioId = precio.Id,
                NombrePrecio = precio.NombrePrecio
            };
            return View(model);
        }
        [HttpPost]
        public string Modificar(PrecioViewModel model)
        {
            try
            {
                var precio = _preciosRepository.Get((int)model.PrecioId);
                precio.NombrePrecio = model.NombrePrecio;

                _preciosRepository.Update(precio);
                TempData["Message"] = "¡El precio ha sido editado!";
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
                    Mensaje = "Error de servidor al editar precio. " + ex.Message
                });
            }
        }
        public IActionResult Eliminar(int? precioId)
        {
            if (precioId == null)
            {
                TempData["Message"] = "Error de navegación";
                return RedirectToAction("Lista");
            }
            var precio = _preciosRepository.Get((int)precioId);
            if (precio == null)
            {
                TempData["Message"] = "El precio no existe";
                return RedirectToAction("Lista");
            }

            _preciosRepository.Delete((int)precioId);

            TempData["Message"] = "El precio ha sido eliminado";
            return RedirectToAction("Lista");
        }

        [HttpGet]
        public IActionResult ObtenerPrecioPorSeguro(string tipo, string nombre, string seguro)
        {
            var precioSeguro = _preciosRepository.ObtenerPrecioPorSeguro(tipo, nombre, seguro);
            if (precioSeguro == null)
            {
                return Json(new { exitoso = false, mensaje = "Precio no encontrado." });
            }

            return Json(new { exitoso = true, precio = precioSeguro });
        }
    }
}

