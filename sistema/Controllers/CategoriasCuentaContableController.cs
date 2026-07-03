using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;
using System;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class CategoriasCuentaContableController : Controller
    {
        private readonly ICategoriaCuentaContable _categoriaCuentaContableRepository;

        public CategoriasCuentaContableController(ICategoriaCuentaContable categoriaCuentaContableRepository)
        {
            _categoriaCuentaContableRepository = categoriaCuentaContableRepository;
        }

        public IActionResult Nuevo()
        {
            var model = new CategoriaCuentaContableViewModel();
            return View(model);
        }
        [HttpPost]
        public string Nuevo(CategoriaCuentaContableViewModel model)
        {
            try
            {
                var categoria = new CategoriasCuentaContable
                {
                    Nombre = model.Nombre,
                    Especificacion = model.Especificacion,
                    Eliminado = false
                };

                _categoriaCuentaContableRepository.Add(categoria);
                TempData["Message"] = "La categoria se ha creado con éxito!";

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
                    Mensaje = "Error al registrar la categoria. " + ex.Message
                });
            }
        }
        public IActionResult Lista()
        {
            var categoria = (_categoriaCuentaContableRepository.GetAll());
            return View(categoria);
        }
        public IActionResult Modificar(int categoriaId)
        {
            var categoria = _categoriaCuentaContableRepository.GetById(categoriaId);
            var model = new CategoriaCuentaContableViewModel
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Especificacion = categoria.Especificacion
            };
            return View(model);
        }
        [HttpPost]
        public string ModificarCategoria(CategoriaCuentaContableViewModel model)
        {
            try
            {
                var categoria = _categoriaCuentaContableRepository.GetById(model.Id);
                categoria.Nombre = model.Nombre;
                categoria.Especificacion = model.Especificacion;

                _categoriaCuentaContableRepository.Update(categoria);

                TempData["Message"] = "La categoria se ha modificado con éxito";

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
                    Mensaje = "Error al modificar la categoria. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string EliminarCategoria(int? categoriaId)
        {
            try
            {
                _categoriaCuentaContableRepository.Delete((int)categoriaId);

                TempData["Message"] = "La categoria se ha eliminado con éxito";

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
                    Mensaje = "Error al eliminar la caategoria. " + ex.Message
                });
            }
        }
    }
}
