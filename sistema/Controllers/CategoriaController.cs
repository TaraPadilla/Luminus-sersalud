using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database.Shared.IRepository;
using sistema.Models;
using Database.Shared.Paginacion;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Database.Shared.Models;

namespace sisrest.Controllers
{
   
    public class CategoriaController : Controller
    {

        private readonly IDespegablesProducto _categoriasRepository = null;

        public CategoriaController(IDespegablesProducto categoriarepository)
        {

            _categoriasRepository = categoriarepository;

        }

        // via de administracion
        public IActionResult Nuevo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Nuevo(CategoriaBaseViewModel model)
        {

            if (ModelState.IsValid)
            {
                _categoriasRepository.Add(model.Categoria);
                TempData["Message"] = "¡La vía de administración se ha modificado con exito.!";

                return RedirectToAction("Lista");
            }

            return View(model);
        }
        [HttpPost]
        public string NuevaViaAdministracion(ViadminProductoViewModel model)
        {
            var viadmin = new Viadmin
            {
                NombreViadmin = model.NombreViaAdministracion,
                Eliminado = false
            };
            _categoriasRepository.Add(viadmin);
            return JsonSerializer.Serialize(new
            {
                Exitoso = true
            });
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

            var lista = _categoriasRepository.PaginacionCategoria(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult Modificar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");

            }

            var categoria = _categoriasRepository.Get((int)id);

            if (categoria == null)
            {
                return StatusCode(404);
            }

            var modelo = new CategoriaBaseViewModel()
            {
                Categoria = categoria,
            };

            return View(modelo);
        }


        [HttpPost]
        public IActionResult Modificar(CategoriaBaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                _categoriasRepository.Update(model.Categoria);
                TempData["Message"] = "¡La vía de administración se ha modificado con exito.!";

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

            model.Eliminado = true;

            _categoriasRepository.Update(model);
            TempData["Message"] = "¡La vía de administración se ha modificado con exito.!";

            return RedirectToAction("Lista");
        }



        // Tipo Producto
        public IActionResult NuevoTipoProducto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NuevoTipoProducto(TipoProductoViewModel model)
        {

            if (ModelState.IsValid)
            {
                _categoriasRepository.Add(model.TipoProducto);
                TempData["Message"] = "¡El nuevo Tipo de Producto se ha guardado con exito.!";

                return RedirectToAction("ListaTipoProductos");
            }

            return View(model);
        }

        public IActionResult ListaTipoProductos(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _categoriasRepository.PaginacionTipoProducto(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult ModificarTipoProducto(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");

            }

            var categoria = _categoriasRepository.GetTipoProducto((int)id);

            if (categoria == null)
            {
                return StatusCode(404);
            }

            var modelo = new TipoProductoViewModel()
            {
                TipoProducto = categoria,
            };

            return View(modelo);
        }


        [HttpPost]
        public IActionResult ModificarTipoProducto(TipoProductoViewModel model)
        {
            if (ModelState.IsValid)
            {
                _categoriasRepository.Update(model.TipoProducto);
                TempData["Message"] = "¡El tipo de Producto se ha modificado con exito.!";

                return RedirectToAction("ListaTipoProductos");
            }

            return View(model);
        }

        public IActionResult EliminarTipoProductos(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _categoriasRepository.GetTipoProducto((int)id);

            if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado = true;

            _categoriasRepository.Update(model);
            TempData["Message"] = "¡El Tipo de Producto se ha eliminado con exito.!";

            return RedirectToAction("ListaTipoProductos");
        }



        // Presentacion producto
        public IActionResult NuevoPresentacionProducto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NuevoPresentacionProducto(PresentacionProductoViewModel model)
        {

            if (ModelState.IsValid)
            {
                _categoriasRepository.Add(model.PresentacionProducto);
                TempData["Message"] = "¡La Presentacion se ha guardado con exito.!";

                return RedirectToAction("ListaPresentacionProducto");
            }

            return View(model);
        }
        [HttpPost]
        public string NuevaPresentacionProducto(PresentacionProductoViewModel model)
        {
            var presentacion = new PresentacionProducto
            {
                PresentProducto = model.NombrePresentacion,
                Eliminado = false
            };
            _categoriasRepository.Add(presentacion);
            return JsonSerializer.Serialize(new
            {
                Exitoso = true
            });
        }

        public IActionResult ListaPresentacionProducto(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _categoriasRepository.PaginacionPresentacionProducto(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult ModificarPresentacionProducto(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");

            }

            var categoria = _categoriasRepository.GetPresentacionProducto((int)id);

            if (categoria == null)
            {
                return StatusCode(404);
            }

            var modelo = new PresentacionProductoViewModel()
            {
                PresentacionProducto = categoria,
            };

            return View(modelo);
        }


        [HttpPost]
        public IActionResult ModificarPresentacionProducto(PresentacionProductoViewModel model)
        {
            if (ModelState.IsValid)
            {
                _categoriasRepository.Update(model.PresentacionProducto);
                TempData["Message"] = "¡La Presentacion se ha modificado con exito.!";

                return RedirectToAction("ListaPresentacionProducto");
            }

            return View(model);
        }

        public IActionResult EliminarPresentacionProducto(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _categoriasRepository.GetPresentacionProducto((int)id);

            if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado = true;

            _categoriasRepository.Update(model);
            TempData["Message"] = "¡La presentacion del producto se ha eliminado con exito.!";

            return RedirectToAction("ListaPresentacionProducto");
        }



        // GrupoT producto
        public IActionResult NuevoGrupoTProducto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NuevoGrupoTProducto(GrupoTViewModel model)
        {

            if (ModelState.IsValid)
            {
                _categoriasRepository.Add(model.GrupoTProducto);
                TempData["Message"] = "¡El grupo terapéutico se ha guardado con exito.!";

                return RedirectToAction("ListaGrupoT");
            }

            return View(model);
        }
        [HttpPost]
        public string NuevoGrupoT(GrupoTViewModel model)
        {
            var grupo = new GrupoTProducto
            {
                NombreGrupoT = model.NombreGrupoT,
                Eliminado = false
            };
            _categoriasRepository.Add(grupo);
            return JsonSerializer.Serialize(new
            {
                Exitoso = true
            });
        }

        public IActionResult ListaGrupoT(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _categoriasRepository.PaginacionGrupoTProducto(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult ModificarGrupoTProducto(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");

            }

            var categoria = _categoriasRepository.GetGrupoTProducto((int)id);

            if (categoria == null)
            {
                return StatusCode(404);
            }

            var modelo = new GrupoTViewModel()
            {
                GrupoTProducto = categoria,
            };

            return View(modelo);
        }


        [HttpPost]
        public IActionResult ModificarGrupoTProducto(GrupoTViewModel model)
        {
            if (ModelState.IsValid)
            {
                _categoriasRepository.Update(model.GrupoTProducto);
                TempData["Message"] = "¡EL grupo terapéutico se ha modificado con exito.!";

                return RedirectToAction("ListaGrupoT");
            }

            return View(model);
        }

        public IActionResult EliminarGrupoTProducto(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _categoriasRepository.GetGrupoTProducto((int)id);

            if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado = true;

            _categoriasRepository.Update(model);
            TempData["Message"] = "¡El grupo terapéutico se ha eliminado con exito.!";

            return RedirectToAction("ListaGrupoT");
        }


        // Laboratorio Producto
        public IActionResult NuevoLaboratorioProducto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NuevoLaboratorioProducto(LaboratorioProductoViewModel model)
        {

            if (ModelState.IsValid)
            {
                _categoriasRepository.Add(model.LaboratorioProducto);
                TempData["Message"] = "¡El Laboratorio se ha guardado con exito.!";

                return RedirectToAction("ListaLabProducto");
            }

            return View(model);
        }
        [HttpPost]
        public string NuevoLaboratorio(LaboratorioProductoViewModel model)
        {
            var laboratorio = new LaboratorioProducto
            {
                NombreLaboratorioProducto = model.NombreLaboratorio,
                Eliminado = false
            };
            _categoriasRepository.Add(laboratorio);
            return JsonSerializer.Serialize(new
            {
                Exitoso = true
            });
        }

        public IActionResult ListaLabProducto(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _categoriasRepository.PaginacionLaboratorioProducto(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult ModificarLabProducto(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");

            }

            var categoria = _categoriasRepository.GetLaboratorioProducto((int)id);

            if (categoria == null)
            {
                return StatusCode(404);
            }

            var modelo = new LaboratorioProductoViewModel()
            {
                LaboratorioProducto = categoria,
            };

            return View(modelo);
        }


        [HttpPost]
        public IActionResult ModificarLabProducto(LaboratorioProductoViewModel model)
        {
            if (ModelState.IsValid)
            {
                _categoriasRepository.Update(model.LaboratorioProducto);
                TempData["Message"] = "¡EL Laboratorio se ha modificado con exito.!";

                return RedirectToAction("ListaLabProducto");
            }

            return View(model);
        }

        public IActionResult EliminarLaboratorioProducto(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _categoriasRepository.GetLaboratorioProducto((int)id);

            if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado = true;

            _categoriasRepository.Update(model);
            TempData["Message"] = "¡EL laboratorio se ha eliminado con exito.!";

            return RedirectToAction("ListaLabProducto");
        }


        // insumos
        public IActionResult ListaCategorias(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

            var lista = _categoriasRepository.PaginacionCategorias(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }


        public IActionResult NuevaCategoria()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NuevaCategoria(CategoriaViewModel model)
        {

            if (ModelState.IsValid)
            {
                _categoriasRepository.Add(model.Categoria);
                TempData["Message"] = "¡La Categoria se ha guardado con exito.!";

                return RedirectToAction("ListaCategorias");
            }

            return View(model);
        }
        [HttpPost]
        public string NuevaCategoriaProducto(CategoriaViewModel model)
        {
            var categoria = new Categoria
            {
                NombreCategoria = model.NombreCategoria,
                Eliminado = false
            };
            _categoriasRepository.Add(categoria);
            return JsonSerializer.Serialize(new
            {
                Exitoso = true
            });
        }

        public IActionResult ModificarCategoria(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");

            }

            var categoria = _categoriasRepository.GetCategoria((int)id);

            if (categoria == null)
            {
                return StatusCode(404);
            }

            var modelo = new CategoriaViewModel()
            {
                Categoria = categoria,
            };

            return View(modelo);
        }

        [HttpPost]
        public IActionResult ModificarCategoria(CategoriaViewModel model)
        {
            if (ModelState.IsValid)
            {
                _categoriasRepository.Update(model.Categoria);
                TempData["Message"] = "¡La Categoria se ha modificado con exito.!";

                return RedirectToAction("ListaCategorias");
            }

            return View(model);
        }


        // marcas
        public IActionResult ListaMarcas(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

            var lista = _categoriasRepository.PaginacionMarcas(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }



        public IActionResult NuevaMarca()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NuevaMarca(MarcaViewModel model)
        {

            if (ModelState.IsValid)
            {
                _categoriasRepository.Add(model.Marca);
                TempData["Message"] = "¡La Marca se ha guardado con exito.!";

                return RedirectToAction("ListaMarcas");
            }

            return View(model);
        }
        [HttpPost]
        public string NuevaMarcaProducto(MarcaViewModel model)
        {
            var marca = new Marca
            {
                NombreMarca = model.NombreMarca,
                Eliminado = false
            };
            _categoriasRepository.Add(marca);

            return JsonSerializer.Serialize(new
            {
                Exitoso = true
            });
        }

        public IActionResult ModificarMarca(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");

            }

            var marca = _categoriasRepository.GetMarca((int)id);

            if (marca == null)
            {
                return StatusCode(404);
            }

            var modelo = new MarcaViewModel()
            {
                Marca = marca,
            };

            return View(modelo);
        }

        [HttpPost]
        public IActionResult ModificarMarca(MarcaViewModel model)
        {
            if (ModelState.IsValid)
            {
                _categoriasRepository.Update(model.Marca);
                TempData["Message"] = "¡La Marca se ha modificado con exito.!";

                return RedirectToAction("ListaMarcas");
            }

            return View(model);
        }


        public IActionResult ListaGrupos(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

            var lista = _categoriasRepository.PaginacionGrupos(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }


        public IActionResult NuevoGrupo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NuevoGrupo(GrupoViewModel model)
        {

            if (ModelState.IsValid)
            {
                _categoriasRepository.Add(model.Grupo);
                TempData["Message"] = "¡El Grupo se ha guardado con exito.!";

                return RedirectToAction("ListaGrupos");
            }

            return View(model);
        }

        public IActionResult ModificarGrupo(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");

            }

            var grupo = _categoriasRepository.GetGrupo((int)id);

            if (grupo == null)
            {
                return StatusCode(404);
            }

            var modelo = new GrupoViewModel()
            {
                Grupo = grupo,
            };

            return View(modelo);
        }

        [HttpPost]
        public IActionResult ModificarGrupo(GrupoViewModel model)
        {
            if (ModelState.IsValid)
            {
                _categoriasRepository.Update(model.Grupo);
                TempData["Message"] = "¡La Marca se ha modificado con exito.!";

                return RedirectToAction("ListaGrupos");
            }

            return View(model);
        }

        public IActionResult EliminarGrupo(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _categoriasRepository.GetGrupo((int)id);

            if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado = true;

            _categoriasRepository.Update(model);
            TempData["Message"] = "¡EL grupo se ha eliminado con exito.!";

            return RedirectToAction("ListaGrupos");
        }



    }
}