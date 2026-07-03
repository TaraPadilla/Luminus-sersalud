using Database.Shared.Data;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;

namespace farmamest.Controllers
{
    public class EstadoResultadosController : Controller
    {
        public IActionResult Index()
        {
            var data = new CalendarioLinealViewModel();
            return View(data);
        }
    }
}
