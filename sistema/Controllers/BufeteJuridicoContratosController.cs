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
    [Authorize]
    public class BufeteJuridicoContratosController : Controller
    {
        private readonly ICaja _cajaRepository = null;
        private readonly UserManager<User> _userManager = null;
        private readonly IGeneratePdf _generatePdf;

        public BufeteJuridicoContratosController(ICaja cajaRepository, UserManager<User> userManager, IGeneratePdf generatePdf)
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
                TempData["Message"] = "El contrato ha sido registrado";
            }
            return View();
        }
        public IActionResult Nuevo()
        {
            return View();
        }
    }


}