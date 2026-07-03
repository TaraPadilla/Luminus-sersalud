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
            PrepararVistaProveedor(modelo);
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Nuevo(ProveedorBaseViewModel model)
        {
            NormalizarBindingProveedor(model);
            AplicarDiasEntrega(model?.Proveedor);
            ValidarProveedor(model?.Proveedor);
            if (!ModelState.IsValid)
            {
                PrepararVistaProveedor(model);
                return View(model);
            }

            var proveedor = model.Proveedor;
            AplicarDiasEntrega(proveedor);
            AsignarTipoCompra(proveedor, model.SelectedTipoCompraId);

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

            var tipoCompraSeleccionado = _tipoCompraRepository.GetListByProveedorId((int)id);

            if (tipoCompraSeleccionado == null || tipoCompraSeleccionado.Count == 0 || tipoCompraSeleccionado.Count > 1)
            {
                model.SelectedTipoCompraId = 0;
            }
            else
            {
                model.SelectedTipoCompraId = tipoCompraSeleccionado[0].Id;
            }

            PrepararVistaProveedor(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modificar(ProveedorBaseViewModel model)
        {
            NormalizarBindingProveedor(model);
            AplicarDiasEntrega(model?.Proveedor);
            ValidarProveedor(model?.Proveedor);
            if (!ModelState.IsValid)
            {
                PrepararVistaProveedor(model);
                return View(model);
            }

            AplicarDiasEntrega(model.Proveedor);
            AsignarTipoCompra(model.Proveedor, model.SelectedTipoCompraId);
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

            if (proveedor == null)
            {
                return Json(new { success = false });
            }

            int tipoCompraId = 0;

            if (proveedor.TipoCompraProveedor == null
                || proveedor.TipoCompraProveedor.Count == 0
                || proveedor.TipoCompraProveedor.Count > 1)
            {
                tipoCompraId = 0;
            }
            else
            {
                tipoCompraId = proveedor.TipoCompraProveedor.FirstOrDefault().TipoCompraId;
            }

            return Json(new
            {
                success = true,
                diasCredito = proveedor.DiasCredito,
                tipoCompraId = tipoCompraId
            });
        }

        private void PrepararVistaProveedor(ProveedorBaseViewModel model)
        {
            if (model == null)
                model = new ProveedorBaseViewModel();

            var tipoCompra = _tipoCompraRepository.GetList();
            var items = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = "Ambas",
                    Selected = model.SelectedTipoCompraId == 0
                }
            };

            items.AddRange(tipoCompra.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Tipo,
                Selected = model.SelectedTipoCompraId == t.Id
            }));

            ViewBag.TipoCompra = new SelectList(items, "Value", "Text", model.SelectedTipoCompraId.ToString());
            model.Init(_proveedorRepository);
        }

        private void NormalizarBindingProveedor(ProveedorBaseViewModel model)
        {
            if (model == null)
                model = new ProveedorBaseViewModel();

            if (model.Proveedor == null)
                model.Proveedor = new Proveedor();

            var tipoCompraRaw = Request.Form["SelectedTipoCompraId"].ToString();
            model.SelectedTipoCompraId = string.IsNullOrWhiteSpace(tipoCompraRaw)
                ? 0
                : int.TryParse(tipoCompraRaw, out var tipoCompraId) ? tipoCompraId : 0;
            ModelState.Remove("SelectedTipoCompraId");

            NormalizarEnteroFormulario("Proveedor.DiasCredito", v => model.Proveedor.DiasCredito = v);
            NormalizarEnteroFormulario("Proveedor.PoliticasDevolucion", v => model.Proveedor.PoliticasDevolucion = v);
        }

        private void NormalizarEnteroFormulario(string key, Action<int> asignar)
        {
            var raw = Request.Form[key].ToString();
            if (string.IsNullOrWhiteSpace(raw))
            {
                asignar(0);
                ModelState.Remove(key);
                return;
            }

            if (int.TryParse(raw, out var valor))
            {
                asignar(valor);
                ModelState.Remove(key);
            }
        }

        private void AplicarDiasEntrega(Proveedor proveedor)
        {
            if (proveedor == null)
                return;

            var dias = Request.Form["DiasEntregaDias"];
            proveedor.DiasEntrega = dias.Count == 0
                ? null
                : string.Join(",", dias.ToArray());
        }

        private void ValidarProveedor(Proveedor proveedor)
        {
            if (proveedor == null)
            {
                ModelState.AddModelError(string.Empty, "Datos del proveedor inválidos.");
                return;
            }

            if (string.IsNullOrWhiteSpace(proveedor.Nombre))
            {
                ModelState.AddModelError("Proveedor.Nombre", "El nombre es obligatorio.");
            }

            if (proveedor.BancoId <= 0)
            {
                ModelState.AddModelError("Proveedor.BancoId", "Seleccione un banco.");
            }
        }

        private static void AsignarTipoCompra(Proveedor proveedor, int selectedTipoCompraId)
        {
            if (selectedTipoCompraId == 0)
            {
                proveedor.TipoCompraProveedor = new List<TipoCompraProveedor>
                {
                    new TipoCompraProveedor
                    {
                        Proveedor = proveedor,
                        ProveedorId = proveedor.Id,
                        TipoCompraId = (int)TipoCompraEnum.Credito
                    },
                    new TipoCompraProveedor
                    {
                        Proveedor = proveedor,
                        ProveedorId = proveedor.Id,
                        TipoCompraId = (int)TipoCompraEnum.Contado
                    }
                };
            }
            else
            {
                proveedor.TipoCompraProveedor = new List<TipoCompraProveedor>
                {
                    new TipoCompraProveedor
                    {
                        Proveedor = proveedor,
                        ProveedorId = proveedor.Id,
                        TipoCompraId = selectedTipoCompraId
                    }
                };
            }
        }
    }
}
