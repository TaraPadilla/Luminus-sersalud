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
    public class GrabacionesController : Controller
    {
        private readonly IGrabaciones _grabacionesRepository = null;

        public GrabacionesController(IGrabaciones grabacionesRepository)
        {
            _grabacionesRepository = grabacionesRepository;
        }

        public IActionResult Lista()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ConsultarGrabaciones()
        {
            try
            {
                var resultado = _grabacionesRepository.GetList();
                return Json(new
                {
                    Exitoso = true,
                    Resultado = resultado
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar grabaciones. " + ex.Message
                });
            }
        }
        public IActionResult Nueva()
        {
            var model = new GrabacionViewModel();
            return View(model);
        }
        [HttpPost]
        public JsonResult Nueva(GrabacionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var grabacion = new Grabacion
                    {
                        Numero = model.Numero,
                        Nombre = model.Nombre,
                        Categoria = model.Categoria,
                        PalabraClave = model.PalabraClave
                    };
                    _grabacionesRepository.Add(grabacion);
                    TempData["Message"] = "¡La grabación ha sido registrada!";
                    return Json(new { Exitoso = true });
                }
                catch (Exception ex)
                {
                    return Json(new { Exitoso = false, Mensaje = "Error al registrar grabación. " + ex.Message });
                }
            }
            return Json(new { Exitoso = false, Mensaje = "Asegúrese de diligenciar los campos obligatorios" });
        }
        public IActionResult Modificar(int? grabacionId)
        {
            if (grabacionId == null)
                return RedirectToAction("Lista");
            var grabacion = _grabacionesRepository.Get((int)grabacionId);
            if (grabacion == null)
                return RedirectToAction("Lista");
            var model = new GrabacionViewModel
            {
                GrabacionId = grabacion.Id,
                Nombre = grabacion.Nombre,
                Numero = grabacion.Numero,
                Categoria = grabacion.Categoria,
                PalabraClave = grabacion.PalabraClave
            };
            return View(model);
        }
        [HttpPost]
        public JsonResult Modificar(GrabacionViewModel model)
        {
            try
            {
                var grabacion = _grabacionesRepository.Get((int)model.GrabacionId);
                grabacion.Numero = model.Numero;
                grabacion.Nombre = model.Nombre;
                grabacion.Categoria = model.Categoria;
                grabacion.PalabraClave = model.PalabraClave;

                _grabacionesRepository.Update(grabacion);
                TempData["Message"] = "¡La grabación ha sido editada!";
                return Json(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al editar grabación. " + ex.Message });
            }
        }
        public IActionResult Eliminar(int? grabacionId)
        {
            if (grabacionId == null)
            {
                TempData["Message"] = "Error de dirección";
                return RedirectToAction("Lista");
            }
            var grabacion = _grabacionesRepository.Get((int)grabacionId);
            if (grabacion == null)
            {
                TempData["Message"] = "No se encontró ninguna grabción con este código";
                return RedirectToAction("Lista");
            }

            _grabacionesRepository.Delete((int)grabacionId);

            TempData["Message"] = "La grabación ha sido eliminada";
            return RedirectToAction("Lista");
        }

    }
}

