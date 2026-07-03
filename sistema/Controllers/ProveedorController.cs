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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.Enumeraciones;

namespace sistema.Controllers
{
    [Authorize]
    public class ProveedorController : Controller
    {


        private readonly IProveedor _proveedorRepository = null;
        private readonly ITipoCompra _tipoCompraRepository = null;

        public ProveedorController(IProveedor proveedorRepository, ITipoCompra tipoCompraRepository)
        {
            _proveedorRepository = proveedorRepository;
            _tipoCompraRepository = tipoCompraRepository;
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

            var lista = _proveedorRepository.PaginacionProveedores(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult Nuevo()
        {
            var modelo = new ProveedorBaseViewModel();
            var tipoCompra = _tipoCompraRepository.GetList();
            ViewBag.TipoCompra = new SelectList(tipoCompra, "Id", "Tipo");
            modelo.Init(_proveedorRepository);
            return View(modelo);
        }

        [HttpPost]
        public IActionResult Nuevo(ProveedorBaseViewModel model)
        {
            var proveedor = model.Proveedor;

            if (model.SelectedTipoCompraId == 0)
            {
                List<TipoCompraProveedor> tipoCompraProveedors = new List<TipoCompraProveedor>();

                // Se añaden los dos por que el Select es igual a 0
                tipoCompraProveedors.Add(new TipoCompraProveedor
                {
                    Proveedor = proveedor,
                    ProveedorId = proveedor.Id,
                    TipoCompraId = (int)TipoCompraEnum.Credito
                });
                tipoCompraProveedors.Add(new TipoCompraProveedor
                {
                    Proveedor = proveedor,
                    ProveedorId = proveedor.Id,
                    TipoCompraId = (int)TipoCompraEnum.Contado
                });
                proveedor.TipoCompraProveedor = tipoCompraProveedors;

            }
            else
            {
                // Crear la relación TipoCompraProveedor
                var tipoCompraProveedor = new TipoCompraProveedor
                {
                    Proveedor = proveedor,
                    ProveedorId = proveedor.Id,
                    TipoCompraId = model.SelectedTipoCompraId
                };
                proveedor.TipoCompraProveedor = new List<TipoCompraProveedor> { tipoCompraProveedor };

            }

            _proveedorRepository.Add(proveedor);
            TempData["Message"] = "¡El proveedor se ha guardado con exito.!";
            return RedirectToAction("Lista");
        }

        public IActionResult Modificar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var proveedor = _proveedorRepository.Get((int)id);

            if (proveedor == null)
            {
                return StatusCode(404);
            }

            var model = new ProveedorBaseViewModel()
            {
                Proveedor = proveedor,
            };

            //Tipo de compra seleccionado por el proveedor
            var tipoCompraSeleccionado = _tipoCompraRepository.GetListByProveedorId((int)id);

            //Tipo de compras que existen
            var tipoCompra = _tipoCompraRepository.GetList();

            ViewBag.TipoCompra = new SelectList(tipoCompra, "Id", "Tipo");
            if (tipoCompraSeleccionado.Count > 1 || tipoCompraSeleccionado is null || tipoCompraSeleccionado.Count == 0)
            {
                model.SelectedTipoCompraId = 0;
            }
            else
            {
                model.SelectedTipoCompraId = tipoCompraSeleccionado.FirstOrDefault().Id;
            }
            model.Init(_proveedorRepository);

            return View(model);
        }

        [HttpPost]
        public IActionResult Modificar(ProveedorBaseViewModel model)
        {
            _proveedorRepository.Update(model.Proveedor);
            TempData["Message"] = "¡El proveedor se ha modificado con exito.!";
            return RedirectToAction("Lista");
        }

        public IActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _proveedorRepository.Get((int)id);

            if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado = true;

            _proveedorRepository.Update(model);
            TempData["Message"] = "¡El proveedor se ha eliminado con exito.!";

            return RedirectToAction("Lista");
        }

        [HttpGet]
        public JsonResult GetProveedorDetails(string nombreProveedor)
        {
            var proveedor = _proveedorRepository.GetByNombre(nombreProveedor);

            int tipoCompraId = 0;

            if(proveedor.TipoCompraProveedor==null 
                || proveedor.TipoCompraProveedor.Count == 0
                || proveedor.TipoCompraProveedor.Count > 1)
            {
                tipoCompraId = 0;
            }
            else
            {
                tipoCompraId = proveedor.TipoCompraProveedor.FirstOrDefault().TipoCompraId;
            }

            if (proveedor != null)
            {
                return Json(new
                {
                    success = true,
                    diasCredito = proveedor.DiasCredito,
                    tipoCompraId = tipoCompraId
                });
            }
            return Json(new { success = false });
        }


    }
}
