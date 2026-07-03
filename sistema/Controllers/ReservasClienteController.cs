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
using sistema.Json;
using System.Net;
using Database.Shared.Paginacion;
using Microsoft.AspNetCore.Authorization;
using Wkhtmltopdf.NetCore;
using System.Text.Json;

namespace sistema.Controllers
{
    [Authorize]
    public class ReservasClienteController : Controller
    {


        private readonly ICompra _compraRepository = null;
        private readonly IProveedor _proveedorRepository = null;
        private readonly IProducto _productoRepository = null;
        private readonly ICaja _cajaRepository = null;
        private readonly IEmpleado _empleadoRepository = null;
        private readonly IDespegablesProducto _categoriaRepository = null;

        public ReservasClienteController(ICompra compraRepository, IProveedor proveedorRepository, IProducto productoRepository,
            ICaja cajaRepository, IEmpleado empleadoRepository, IGeneratePdf generatepdf, IDespegablesProducto categoriaRepository)
        {
            _compraRepository = compraRepository;
            _proveedorRepository = proveedorRepository;
            _productoRepository = productoRepository;
            _cajaRepository = cajaRepository;
            _empleadoRepository = empleadoRepository;
            _categoriaRepository = categoriaRepository;
        }
        public IActionResult Nueva()
        {
            return View();
        }




    }
}
