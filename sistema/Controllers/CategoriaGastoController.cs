using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database.Shared.IRepository;
using Database.Shared.Models;
using sistema.Models;
using Database.Shared.Paginacion;
using Microsoft.AspNetCore.Authorization;

namespace sistema.Controllers
{
    [Authorize]
    public class CategoriaGastoController : Controller
    {

        private readonly ICategoriaGasto _categoriasRepository = null;

        public CategoriaGastoController(ICategoriaGasto categoriarepository)
        {

            _categoriasRepository = categoriarepository;

        }

        public IActionResult Nuevo()
        {


            return View();
        }

        [HttpPost]
        public IActionResult Nuevo(CategoriaGasto model)
        {

            if (ModelState.IsValid)
            {
                _categoriasRepository.Add(model);
                TempData["Message"] = "¡La categoria se ha guardado con exito.!";

                return RedirectToAction("Lista");
            }

            return View(model);
        }

         public IActionResult Lista(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ApellidoSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Apellido_desc" : "";
            ViewData["NombreSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Nombre_desc" : "";
            
            if(buscar != null)
            {
                pageNumber = 1;
            }
            else 
            {
                buscar = currentFilter;
            }
            
            ViewData["CurrentFilter"] = buscar;

            var lista = _categoriasRepository.PaginacionCategoria(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult Modificar(int? id)
        {
            if(id==null)
            {
              return BadRequest("request is incorrect");

            }

            var categoria = _categoriasRepository.Get((int)id);

            if (categoria == null)
            {
                return StatusCode(404);
            }

            return View(categoria);
        }


        [HttpPost]
        public IActionResult Modificar(CategoriaGasto model)
        {
            if (ModelState.IsValid)
            {
                _categoriasRepository.Update(model);
                TempData["Message"] = "¡La Categoria se ha modificado con exito.!";

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

            var model = _categoriasRepository.Get((int)id);

             if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado=true;
                
            _categoriasRepository.Update(model);
            TempData["Message"] = "¡La Categoria se ha eliminado con exito.!";
               
            return RedirectToAction("Lista");
        }



    }
}