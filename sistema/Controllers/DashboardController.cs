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

namespace sistema.Controllers
{
    [Authorize]

    public class DashboardController : Controller
    {
        private readonly IChart _chartRepository = null;

        public DashboardController(IChart chartRepository)
        {
            _chartRepository = chartRepository;
        }

        public IActionResult VentasPorMesesFarmacia()
        {
            var ventas = _chartRepository.TotalIngresoVentasPorMesFarmacia();

            var meses = ventas.Select(a => a.MonthName);
            var ingresoPorMes = ventas.Select(a => a.Total);

            var model = new ChartsBaseViewModel()
            {
                Meses = meses,
                IngresoPorMes = ingresoPorMes
            };

            return View(model);
        }

        public IActionResult VentasPorMesesClinica()
        {
            var ventas = _chartRepository.TotalIngresoVentasPorMesClinica();

            var meses = ventas.Select(a => a.MonthName);
            var ingresoPorMes = ventas.Select(a => a.Total);

            var model = new ChartsBaseViewModel()
            {
                Meses = meses,
                IngresoPorMes = ingresoPorMes
            };

            return View(model);
        }

    }
}
