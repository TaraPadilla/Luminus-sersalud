using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Database.Shared;
using Microsoft.Extensions.Logging;
using Database.Shared.IRepository;
using sistema.Models;
using Microsoft.AspNetCore.Authorization;

namespace sistema.Controllers
{
    [Authorize]
    public class EspecialidadController : Controller
    {
        private readonly IEspecialidad _especialidadRepository = null;
        private readonly ILogger<EspecialidadController> _logger;

        public EspecialidadController(IEspecialidad especialidadRepository, ILogger<EspecialidadController> logger)
        {
            _especialidadRepository = especialidadRepository;
            _logger = logger;
        }

        public IActionResult Lista(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["EspecialidadSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Especialidad_desc" : "";
            ViewData["CodigoSortParam"] = sortOrder == "Codigo" ? "Codigo_desc" : "Codigo";

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

            var lista = _especialidadRepository.PaginacionEspecialidades(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult Nuevo()
        {
            var model = new EspecialidadBaseViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Nuevo(EspecialidadBaseViewModel model)
        {
            if (ModelState.IsValid)
            {   
                // Guardar la especialidad en la base de datos
                _especialidadRepository.Add(model.Especialidad);
                TempData["Message"] = "¡La especialidad se ha guardado con éxito!";
                return Json(new { success = true });
            }
            
            return View(model);
        }

        public IActionResult Modificar(int? id)
        {
            if (id == null)
            {
                return BadRequest("La solicitud es incorrecta");
            }

            var especialidad = _especialidadRepository.Get((int)id);

            if (especialidad == null)
            {
                return StatusCode(404);
            }

            var model = new EspecialidadBaseViewModel
            {
                Especialidad = especialidad,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Modificar(EspecialidadBaseViewModel model)
        {   
            if (ModelState.IsValid)
            {
                // Actualizar la especialidad en la base de datos
                _especialidadRepository.Update(model.Especialidad);
                TempData["Message"] = "¡La especialidad se ha actualizado con éxito!";
                return RedirectToAction("Lista");
            }

            return View(model);
        }

        public IActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return BadRequest("La solicitud es incorrecta");
            }

            var especialidad = _especialidadRepository.Get((int)id);

            if (especialidad == null)
            {
                return StatusCode(404);
            }

            especialidad.Eliminado = true;
            _especialidadRepository.Update(especialidad);
            TempData["Message"] = "¡La especialidad se ha eliminado con éxito!";

            return RedirectToAction("Lista");
        }
    }
}
