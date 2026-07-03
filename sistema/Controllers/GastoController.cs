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
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;

namespace sistema.Controllers
{
    [Authorize]
    public class GastoController : Controller
    {
        private readonly IGasto _gastoRepository = null;
        private readonly ICategoriaGasto _categoryGastoRepository = null;


       

        public GastoController(ICategoriaGasto categoryGastoRepository, IGasto gastoRepository)
        {
            _categoryGastoRepository = categoryGastoRepository;
            _gastoRepository = gastoRepository;
        }

        public IActionResult Lista(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ApellidoSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Apellido_desc" : "";
            ViewData["NombreSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Nombre_desc" : "";

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

            var lista = _gastoRepository.PaginacionGastos(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult Nuevo()
        {

            // var cargarCategorias = _categoryRepository.ListarCategorias();

            var modelo = new GastoBaseViewModel()
            {
                Modificar = true,
            };

            modelo.Init(_categoryGastoRepository);

            return View(modelo);
        }

        [HttpPost]
        public IActionResult Nuevo(GastoBaseViewModel model, IFormFile file)
        {
            

            if (ModelState.IsValid)
            {
                var gasto = model.Gasto;
                gasto.Fecha=DateTime.Now;

        
                _gastoRepository.Add(gasto);
                TempData["Message"] = "¡El gasto se ha guardado con exito.!";

                return RedirectToAction("Lista");
            }

            model.Init(_categoryGastoRepository);
            return View(model);
        }

        public IActionResult Modificar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var gasto = _gastoRepository.Get((int)id);

            if (gasto == null)
            {
                return StatusCode(404);
            }

            var modelo = new GastoBaseViewModel()
            {
                Gasto = gasto,
                Modificar = false,
            };

            modelo.Init(_categoryGastoRepository);

            return View(modelo);
        }

        [HttpPost]
        public IActionResult Modificar(GastoBaseViewModel model)
        {

             if (ModelState.IsValid)
            {    

                _gastoRepository.Update(model.Gasto);
        
                TempData["Message"] = "¡El gasto se ha modificado con exito.!";
                return RedirectToAction("Lista");
            }

            model.Init(_categoryGastoRepository);
            return View(model);
        }

        public IActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _gastoRepository.Get((int)id);

            if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado = true;

            _gastoRepository.Update(model);
            TempData["Message"] = "¡El gasto se ha eliminado con exito.!";


            return RedirectToAction("Lista");
        }



    }
}
