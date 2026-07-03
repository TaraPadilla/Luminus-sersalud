using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;
using System;
using System.Linq;

namespace farmamest.Controllers
{
    public class CategoriaRecetaController : Controller
    {
        private readonly IRecetas _recetasRepository;

        public CategoriaRecetaController(IRecetas recetasRepository)
        {
            _recetasRepository = recetasRepository;
        }

        private void ValidarCategoria(CategoriaRecetaCrudViewModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(model.Nombre))
                ModelState.AddModelError(nameof(model.Nombre), "El nombre es requerido.");
        }

        public IActionResult Lista()
        {
            var categorias = _recetasRepository.GetCategoriasRecetas()
                .Where(x => !x.Eliminado)
                .OrderBy(x => x.Nombre)
                .ToList();

            // Conteo de menús activos por categoría (sin asumir relación extra: ya existe RecetaMenu.CategoriaId)
            var menus = _recetasRepository.GetRecetaMenus()
                .Where(x => !x.Eliminado)
                .ToList();

            var model = categorias.Select(c => new CategoriaRecetaListItemViewModel
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                MenusActivos = menus.Count(m => m.CategoriaId == c.Id)
            }).ToList();

            return View(model);
        }

        public IActionResult Nuevo()
        {
            var model = new CategoriaRecetaCrudViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Nuevo(CategoriaRecetaCrudViewModel model)
        {
            ValidarCategoria(model);

            if (!ModelState.IsValid)
                return View(model);

            var entity = new CategoriaReceta
            {
                Nombre = model.Nombre?.Trim(),
                Descripcion = model.Descripcion,
                FechaHoraCreada = DateTime.Now,
                Eliminado = false
            };

            _recetasRepository.AddCategoriaReceta(entity);

            TempData["Message"] = "La categoría se ha creado con éxito";
            return RedirectToAction(nameof(Lista));
        }

        public IActionResult Modificar(int categoriaId)
        {
            var categoria = _recetasRepository.GetCategoriaReceta(categoriaId);
            if (categoria == null || categoria.Eliminado)
                return RedirectToAction(nameof(Lista));

            var model = new CategoriaRecetaCrudViewModel
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modificar(CategoriaRecetaCrudViewModel model)
        {
            if (model.Id <= 0)
                return RedirectToAction(nameof(Lista));

            ValidarCategoria(model);

            if (!ModelState.IsValid)
                return View(model);

            var categoria = _recetasRepository.GetCategoriaReceta(model.Id);
            if (categoria == null || categoria.Eliminado)
                return RedirectToAction(nameof(Lista));

            categoria.Nombre = model.Nombre?.Trim();
            categoria.Descripcion = model.Descripcion;

            _recetasRepository.UpdateCategoriaReceta(categoria);

            TempData["Message"] = "La categoría se ha modificado con éxito";
            return RedirectToAction(nameof(Lista));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int categoriaId)
        {
            _recetasRepository.SoftDeleteCategoriaReceta(categoriaId);
            TempData["Message"] = "La categoría se ha eliminado con éxito";
            return RedirectToAction(nameof(Lista));
        }
    }
}
