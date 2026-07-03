using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using sistema.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace farmamest.Controllers
{
    public class RecetaMenuController : Controller
    {
        private readonly IRecetas _recetasRepository;

        public RecetaMenuController(IRecetas recetasRepository)
        {
            _recetasRepository = recetasRepository;
        }

        // --------------------------
        // Helpers
        // --------------------------
        private void CargarCatalogos(RecetaMenuCrudViewModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var categorias = _recetasRepository.GetCategoriasRecetas()
                .Where(x => !x.Eliminado)
                .OrderBy(x => x.Nombre)
                .ToList();

            model.Categorias = categorias
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                })
                .ToList();
        }

        private void ValidarMenu(RecetaMenuCrudViewModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            if (!model.CategoriaId.HasValue || model.CategoriaId.Value <= 0)
                ModelState.AddModelError(nameof(model.CategoriaId), "La categoría es obligatoria.");

            if (string.IsNullOrWhiteSpace(model.Nombre))
                ModelState.AddModelError(nameof(model.Nombre), "El nombre es requerido.");
        }

        private static int CalcularDiasFlags(List<int> diasSeleccionados)
        {
            if (diasSeleccionados == null || diasSeleccionados.Count == 0) return 0;

            var distinct = diasSeleccionados.Distinct().ToList();
            var flags = 0;

            foreach (var d in distinct)
                flags |= d;

            return flags;
        }

        private static List<int> ExtraerDiasSeleccionados(int diasSemanaFlags)
        {
            var result = new List<int>();

            foreach (DiasSemanaFlags flag in Enum.GetValues(typeof(DiasSemanaFlags)))
            {
                if (flag == DiasSemanaFlags.Ninguno) continue;

                var val = (int)flag;
                if ((diasSemanaFlags & val) == val)
                    result.Add(val);
            }

            return result;
        }

        private static string FormatearDias(int diasSemanaFlags)
        {
            if (diasSemanaFlags == 0) return string.Empty;

            var partes = new List<string>();
            var flags = (DiasSemanaFlags)diasSemanaFlags;

            if (flags.HasFlag(DiasSemanaFlags.Lunes)) partes.Add("Lunes");
            if (flags.HasFlag(DiasSemanaFlags.Martes)) partes.Add("Martes");
            if (flags.HasFlag(DiasSemanaFlags.Miercoles)) partes.Add("Miércoles");
            if (flags.HasFlag(DiasSemanaFlags.Jueves)) partes.Add("Jueves");
            if (flags.HasFlag(DiasSemanaFlags.Viernes)) partes.Add("Viernes");
            if (flags.HasFlag(DiasSemanaFlags.Sabado)) partes.Add("Sábado");
            if (flags.HasFlag(DiasSemanaFlags.Domingo)) partes.Add("Domingo");

            return string.Join(", ", partes);
        }

        // --------------------------
        // Menús (CRUD)
        // --------------------------
        public IActionResult Lista()
        {
            var categorias = _recetasRepository.GetCategoriasRecetas()
                .Where(x => !x.Eliminado)
                .ToDictionary(x => x.Id, x => x.Nombre);

            var menus = _recetasRepository.GetRecetaMenus()
                .Where(x => !x.Eliminado)
                .OrderBy(x => x.Nombre)
                .Select(m => new RecetaMenuListItemViewModel
                {
                    Id = m.Id,
                    CategoriaNombre = categorias.ContainsKey(m.CategoriaId) ? categorias[m.CategoriaId] : $"#{m.CategoriaId}",
                    Nombre = m.Nombre,
                    Ingredientes = m.Ingredientes,
                    DiasTexto = FormatearDias(m.DiasSemana)
                })
                .ToList();

            return View(menus);
        }

        public IActionResult Nuevo()
        {
            var model = new RecetaMenuCrudViewModel();
            CargarCatalogos(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Nuevo(RecetaMenuCrudViewModel model)
        {
            model.DiasSeleccionados ??= new List<int>();

            ValidarMenu(model);

            if (!ModelState.IsValid)
            {
                CargarCatalogos(model);
                return View(model);
            }

            var menu = new RecetaMenu
            {
                CategoriaId = model.CategoriaId!.Value,
                Nombre = model.Nombre?.Trim(),
                Ingredientes = model.Ingredientes,
                DiasSemana = CalcularDiasFlags(model.DiasSeleccionados),
                FechaHoraCreada = DateTime.Now,
                Eliminado = false
            };

            _recetasRepository.AddRecetaMenu(menu);

            TempData["Message"] = "El menú se ha creado con éxito";
            return RedirectToAction(nameof(Lista));
        }

        public IActionResult Modificar(int menuId)
        {
            var menu = _recetasRepository.GetRecetaMenu(menuId);
            if (menu == null || menu.Eliminado)
                return RedirectToAction(nameof(Lista));

            var model = new RecetaMenuCrudViewModel
            {
                Id = menu.Id,
                CategoriaId = menu.CategoriaId,
                Nombre = menu.Nombre,
                Ingredientes = menu.Ingredientes,
                DiasSemana = menu.DiasSemana,
                DiasSeleccionados = ExtraerDiasSeleccionados(menu.DiasSemana)
            };

            CargarCatalogos(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modificar(RecetaMenuCrudViewModel model)
        {
            if (model.Id <= 0)
                return RedirectToAction(nameof(Lista));

            model.DiasSeleccionados ??= new List<int>();

            ValidarMenu(model);

            if (!ModelState.IsValid)
            {
                CargarCatalogos(model);
                return View(model);
            }

            var menu = _recetasRepository.GetRecetaMenu(model.Id);
            if (menu == null || menu.Eliminado)
                return RedirectToAction(nameof(Lista));

            menu.CategoriaId = model.CategoriaId!.Value;
            menu.Nombre = model.Nombre?.Trim();
            menu.Ingredientes = model.Ingredientes;
            menu.DiasSemana = CalcularDiasFlags(model.DiasSeleccionados);

            _recetasRepository.UpdateRecetaMenu(menu);

            TempData["Message"] = "El menú se ha modificado con éxito";
            return RedirectToAction(nameof(Lista));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int menuId)
        {
            _recetasRepository.SoftDeleteRecetaMenu(menuId);
            TempData["Message"] = "El menú se ha eliminado con éxito";
            return RedirectToAction(nameof(Lista));
        }
    }
}
