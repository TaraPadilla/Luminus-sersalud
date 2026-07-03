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
using sistema.Json;
using Microsoft.AspNetCore.Authorization;

namespace sistema.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly ICliente _clienteRepository = null;

        public ClienteController(ICliente clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public IActionResult Lista(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _clienteRepository.PaginacionClientes(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult Nuevo()
        {
            var model = new ClientesBaseViewModel();
            return View(model);
        }
        public IActionResult NuevoProspecto()
        {
            var model = new ClienteProspectoViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Nuevo(ClientesBaseViewModel model)
        {

            if (ModelState.IsValid)
            {
                _clienteRepository.Add(model.Clientes);
                TempData["Message"] = "¡El cliente se ha guardado con exito.!";

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

            var cliente = _clienteRepository.Get((int)id);

            if (cliente == null)
            {
                return StatusCode(404);
            }

            var model = new ClientesBaseViewModel()
            {
                Clientes = cliente
            };


            return View(model);
        }

        [HttpPost]
        public IActionResult Modificar(ClientesBaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                _clienteRepository.Update(model.Clientes);
                TempData["Message"] = "¡El cliente se ha modificado con exito.!";
                return RedirectToAction("Lista");
            }

            return View(model.Clientes);
        }

        public IActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _clienteRepository.Get((int)id);


            if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado = true;

            _clienteRepository.Update(model);
            TempData["Message"] = "¡El cliente se ha eliminado con exito.!";

            return RedirectToAction("Lista");
        }

        public JsonResult RetornarCliente(string nombre)
        {
            var clientebuscado = _clienteRepository.GetClientePorNombre(nombre);

            // if(clientebuscado == null)
            // {
            //     return new JsonErrorResult(new { message = ""});
            // }

            return Json(clientebuscado);
        }

        public JsonResult RetornarClienteById(int id)
        {
            var clientebuscado = _clienteRepository.GetClientePorId(id);

            // if(clientebuscado == null)
            // {
            //     return new JsonErrorResult(new { message = ""});
            // }

            return Json(clientebuscado);
        }


    }
}
