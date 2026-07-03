using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database.Shared.IRepository;
using sistema.Models;
using Database.Shared.Paginacion;
using System.Linq;
using Database.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Wkhtmltopdf.NetCore;

namespace sisrest.Controllers
{
 
    public class BufeteJuridicoMarcasController : Controller
    {
        private readonly ICaja _cajaRepository = null;
        private readonly UserManager<User> _userManager = null;
        private readonly IGeneratePdf _generatePdf;

        public BufeteJuridicoMarcasController(ICaja cajaRepository, UserManager<User> userManager, IGeneratePdf generatePdf)
        {
            _cajaRepository = cajaRepository;
            _userManager = userManager;
            _generatePdf = generatePdf;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Lista(bool? guardado)
        {
            if (guardado != null && (bool)guardado)
            {
                TempData["Message"] = "La marca ha sido registrada";
            }
            return View();
        }
        public IActionResult Nueva()
        {
            return View();
        }
    }


}