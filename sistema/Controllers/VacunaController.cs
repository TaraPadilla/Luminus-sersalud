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
using Microsoft.AspNetCore.Identity;
using Wkhtmltopdf.NetCore;
using Database.Shared.Paginacion;
using System.Text.Json;
using Database.Shared.Enumeraciones;

namespace sistema.Controllers
{
	[Authorize]
	public class VacunaController : Controller
    {
		private readonly IVacunas _vacunasRepository = null;
		private readonly UserManager<User> _userManager = null;


		public VacunaController(
	    IVacunas vacunasRepository, 
		UserManager<User> userManager)
		{
			_vacunasRepository = vacunasRepository;
			_userManager = userManager;
		}

		public IActionResult ListaVacunas(VacunasViewModel viewModel)
		{
			if (viewModel.buscar != null)
			{
				viewModel.pageNumber = 1;
			}
			else
			{
				viewModel.buscar = viewModel.currentFilter;
			}

			ViewData["CurrentFilter"] = viewModel.buscar;
			

			viewModel.nombreVacunas = _vacunasRepository.PaginacionVacunas(null, viewModel.buscar, viewModel.pageNumber, 30);

			return View(viewModel);
		}

        public IActionResult ModificarVacuna(int? id)
        {
           
            var vacuna = _vacunasRepository.GetVacuna((int)id);

			var model = new VacunaViewModel()
			{
				Id = vacuna.Id,
				Nombre = vacuna.Nombre, 
				Preparacion = vacuna.Preparacion

            };

           
            return View(model);
        }


        [HttpPost]
        public async Task<string> ModificarVacuna(VacunaViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var vacuna = _vacunasRepository.GetVacuna((int)model.Id);
                vacuna.Nombre= model.Nombre;
                vacuna.Preparacion = model.Preparacion;
               
                
                _vacunasRepository.Update(vacuna);

                TempData["Message"] = "¡El registro se ha modificado con éxito.!";

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
                    Mensaje = "Error de servidor al modificar examen. " + ex.Message
                });
            }
        }


        public IActionResult CrearVacuna()
        {
            var model = new VacunaViewModel();
            return View(model);
        }

        [HttpPost]
        public string CrearVacuna(VacunaViewModel model)
        {
            try
            {

                var vacuna = new Vacuna
                {
                    Nombre = model.Nombre,
                    Preparacion = model.Preparacion

                };
                _vacunasRepository.Add(vacuna);
                TempData["Message"] = "El examen ha sido registrado";

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
                    Mensaje = "Error de servidor al crear examen. " + ex.Message
                });
            }
        }

        public IActionResult EliminarVacuna(int? id)
        {
            var vacuna = _vacunasRepository.GetVacuna((int)id);
            vacuna.Eliminado = true;

            _vacunasRepository.Update(vacuna);

            TempData["Message"] = "¡El registro se ha archivado con éxito.!";

            return RedirectToAction("ListaVacunas");
        }


    }
}
