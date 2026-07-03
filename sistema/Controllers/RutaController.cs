using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Database.Shared;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using sistema.Models;
using Database.Shared.IRepository;
using Database.Shared.Data;
using Database.Shared.Models;
using System.Web;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using sistema.Json;
using Microsoft.AspNetCore.Authorization;

namespace sistema.Controllers
{
    [Authorize]
    public class RutaController : Controller
    {
        private readonly IRuta _rutaRepository = null;


       // private string _dir;

        public RutaController(IRuta rutaRepository)
        {
            _rutaRepository = rutaRepository;
        }

        public IActionResult Lista(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
           
            
            if(buscar != null)
            {
                pageNumber = 1;
            }
            else 
            {
                buscar = currentFilter;
            }
            
            ViewData["CurrentFilter"] = buscar;

            var lista = _rutaRepository.PaginacionRutas(sortOrder, buscar, pageNumber, 2);

            return View(lista);
        }

        public IActionResult Nuevo()
        {

            // var cargarCategorias = _categoryRepository.ListarCategorias();

            // var modelo = new ServicioBaseViewModel()
            // {
            // };


            return View();
        }

        [HttpPost]
        public IActionResult Nuevo(Ruta model)//, IFormFile file)
        {


            if (ModelState.IsValid)
            {
                var ruta = model;

                _rutaRepository.Add(ruta);
                TempData["Message"] = "¡La ruta se ha guardado con exito.!";

                return RedirectToAction("Lista");
            }

            return View(model);
        }

        public IActionResult Modificar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var ruta = _rutaRepository.Get((int)id);

            if (ruta == null)
            {
                return StatusCode(404);
            }


            return View(ruta);
        }

        [HttpPost]
        public IActionResult Modificar(Ruta model)
        {
            if (ModelState.IsValid)
            {

                _rutaRepository.Update(model);
                TempData["Message"] = "¡La ruta se ha modificado con exito.!";
                return RedirectToAction("Lista");
            }

            return View(model);
        }

         public IActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _rutaRepository.Get((int)id);

             if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado=true;
                
            _rutaRepository.Update(model);
            TempData["Message"] = "¡La ruta se ha eliminado con exito.!";
               
            return RedirectToAction("Lista");
        }

        public JsonResult RetornarRutas(string id)
        {
            var rutaBuscada = _rutaRepository.Get(Convert.ToInt32(id));

            if(rutaBuscada == null)
            {
                return new JsonErrorResult(new { message = ""});
            }

            return Json(rutaBuscada);
        }


       

       
    }
}