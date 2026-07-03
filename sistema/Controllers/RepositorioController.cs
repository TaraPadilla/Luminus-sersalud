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
using Database.Shared.Enumeraciones;

namespace sistema.Controllers
{
    [Authorize]
    public class RepositorioController : Controller
    {
        private readonly IPersonas _personasRepository = null;
        private readonly IPacientes _pacientesRepository = null;

        public RepositorioController(IPersonas personasRepository, IPacientes pacientesRepository)
        {
            _personasRepository = personasRepository;
            _pacientesRepository = pacientesRepository;
        }
        public IActionResult Archivos()
        {
            return View();
        }
        public IActionResult Carpetas()
        {
            return View();
        }
        public IActionResult ArchivosCarpeta(int? carpetaId)
        {
            //if(carpetaId == null)
            //{
            //    TempData["Message"] = "Error de ruta";
            //}
            var archivosCarpeta = new List<RepositorioArchivo>();
            return View();
        }
        public IActionResult SubirArchivos()
        {
            return View();
        }
    }
}
