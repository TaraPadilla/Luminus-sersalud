using System;
using Microsoft.AspNetCore.Mvc;
using Database.Shared.IRepository;
using sistema.Models;
using System.Linq;
using Database.Shared.Models;
using Rotativa.AspNetCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Wkhtmltopdf.NetCore;
using Database.Shared.Enumeraciones;
using farmamest.Service.IService;
using farmamest.Models;
using Microsoft.Extensions.Configuration;
using Wkhtmltopdf.NetCore.Options;
using System.Text.Json;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using Database.Shared;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using static farmamest.Models.ConsentimientoHospiVM;
using fmModels = farmamest.Models;
using Microsoft.AspNetCore.Authorization;
using farmamest.Service;
using farmamest.Service.IService;
using farmamest.Utilidades;
using Database.Shared.SqlDataSeed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace sistema.Controllers
{
    public class CrearPdfController : Controller
    {
        private readonly Context _context;
        private readonly IConfiguration _configuration;
        private readonly ICitas _citasRepository;
        private readonly IServicio _serviciosRepository;
        private readonly IGeneratePdf _generatePdf;
        private readonly IVenta _ventaRepository;
        private readonly ICotizacion _cotizacionRepository;
        private readonly IProducto _productoRepository;
        private readonly IConsultas _consultasRepository;
        private readonly IEmpleado _empleadoRepository;
        private readonly IProveedor _proveedorRepository;
        private readonly IHospitalizacion _hospitalizacionRepository;
        private readonly ICliente _clienteRepository;
        private readonly ICompra _compraRepository;
        private readonly IServicio _servicioRepository;
        private readonly IRuta _rutaRepository;
        private readonly IGasto _gastoRepository;
        private readonly ICaja _cajaRepository;
        private readonly IEnvio _envioRepository;
        private readonly IDespegablesProducto _categoriaRepository;
        private readonly ICategoriaGasto _categoriaGastoRepository;
        private readonly IUser _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILaboratorioClinico _laboratorioClinico;
        private readonly IPacientes _pacientesRepository;
        private readonly ICuentasPorCobrar _cuentasPorCobrarRepository;
        private readonly IProductosService _productosService;
        private readonly IPrecios _precioRepository;
        private readonly ISucursal _sucursalRepository;
        private readonly IConsentimientoHospiService _consentimientoHospiService;

        private readonly IRequision _requisicionRepository;

        private readonly IDevolucionNacional _devolucionRepository;

        private readonly INotaOperatoriaService _notaOperatoriaService;
        private readonly IMedicamentoNoControladoRepository _medicamentoNoControladoRepository;
        private readonly ICuestionarioPreAnestesicoService _cuestionarioPreAnestesicoService;
        private readonly IRegistroAnestesiaService _registroAnestesiaService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<CrearPdfController> _logger;


        public CrearPdfController(
            Context context,
            IConfiguration configuration,
            ICitas citasRepository,
            IServicio serviciosRepository,
            IGeneratePdf generatePdf,
            IVenta ventaRepository,
            ICotizacion cotizacionRepository,
            IProducto productoRepository,
            IConsultas consultasRepository,
            IEmpleado empleadoRepository,
            IProveedor proveedorRepository,
            IHospitalizacion hospitalizacionRepository,
            ICliente clienteRepository,
            ICompra compraRepository,
            IServicio servicioRepository,
            IRuta rutaRepository,
            IGasto gastoRepository,
            ICaja cajaRepository,
            IEnvio envioRepository,
            IDespegablesProducto categoriaRepository,
            ICategoriaGasto categoriaGastoRepository,
            IUser userRepository,
            UserManager<User> userManager,
            ILaboratorioClinico laboratorioClinico,
            IPacientes pacientesRepository,
            ICuentasPorCobrar cuentasPorCobrarRepository,
            IProductosService productosService,
            IPrecios precioRepository,
            ISucursal sucursalRepository,
            IConsentimientoHospiService consentimientoHospiService,
            IRequision requisicionRepository,
            IDevolucionNacional devolucionRepository,
            INotaOperatoriaService notaOperatoriaService,
            IMedicamentoNoControladoRepository medicamentoNoControladoRepository,
            ICuestionarioPreAnestesicoService cuestionarioPreAnestesicoService,
            IRegistroAnestesiaService registroAnestesiaService,
            IWebHostEnvironment webHostEnvironment,
            ILogger<CrearPdfController> logger
        )
        {
            _context = context;
            _configuration = configuration;
            _citasRepository = citasRepository;
            _serviciosRepository = serviciosRepository;
            _generatePdf = generatePdf;
            _ventaRepository = ventaRepository;
            _cotizacionRepository = cotizacionRepository;
            _productoRepository = productoRepository;
            _consultasRepository = consultasRepository;
            _empleadoRepository = empleadoRepository;
            _proveedorRepository = proveedorRepository;
            _hospitalizacionRepository = hospitalizacionRepository;
            _clienteRepository = clienteRepository;
            _compraRepository = compraRepository;
            _servicioRepository = servicioRepository;
            _rutaRepository = rutaRepository;
            _gastoRepository = gastoRepository;
            _cajaRepository = cajaRepository;
            _envioRepository = envioRepository;
            _categoriaRepository = categoriaRepository;
            _categoriaGastoRepository = categoriaGastoRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _laboratorioClinico = laboratorioClinico;
            _pacientesRepository = pacientesRepository;
            _cuentasPorCobrarRepository = cuentasPorCobrarRepository;
            _productosService = productosService;
            _precioRepository = precioRepository;
            _sucursalRepository = sucursalRepository;
            _consentimientoHospiService = consentimientoHospiService;
            _requisicionRepository = requisicionRepository;
            _devolucionRepository = devolucionRepository;
            _notaOperatoriaService = notaOperatoriaService;
            _medicamentoNoControladoRepository = medicamentoNoControladoRepository;
            _cuestionarioPreAnestesicoService = cuestionarioPreAnestesicoService;
            _registroAnestesiaService = registroAnestesiaService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;

        }

        private string ContentRoot =>
            _webHostEnvironment?.ContentRootPath ?? Directory.GetCurrentDirectory();

        private Task<IActionResult> RenderPdfAsync(
            string viewPath,
            object model,
            ConvertOptions convertOptions = null)
        {
            return PdfGenerationHelper.GetPdfSafeAsync(
                _generatePdf,
                _webHostEnvironment,
                _logger,
                viewPath,
                model,
                convertOptions);
        }

        // public async Task<IActionResult> ReciboVentaPdf(int? id)
        // {
        //     if (id == null)
        //     {
        //         return StatusCode(404);
        //     }

        //     var venta = _ventaRepository.Get((int)id);

        //     if (venta == null)
        //     {
        //         return StatusCode(400);
        //     }


        //     var options = new ConvertOptions
        //     {
        //         PageMargins = { Top = 0, Left = 0, Right = 0, Bottom = 0 },
        //         PageWidth = 58.0,
        //         PageHeight = 600,
        //     };



        //     _generatePdf.SetConvertOptions(options);

        //     return await RenderPdfAsync("Views/CrearPDF/ReciboVentaPdf.cshtml", venta);
        // }




        public async Task<IActionResult> ReciboVentaPdf(int? id)
        {
            if (id == null) return StatusCode(404);

            var venta = _ventaRepository.Get((int)id);
            if (venta == null) return StatusCode(400);

            if (venta.TipoVenta == "Hospitalizacion")
            {
                var detalleCaja = _context.DetalleCajas
                    .Include(d => d.CuentaPorCobrar)
                        .ThenInclude(c => c.DetallesCuentaPorCobrar)
                    .FirstOrDefault(d => d.VentaId == venta.Id && d.CuentaPorCobrarId != null);

                if (detalleCaja?.CuentaPorCobrar?.DetallesCuentaPorCobrar != null)
                {
                    var hospitalizacionId = detalleCaja.CuentaPorCobrar.DetallesCuentaPorCobrar
                        .Where(d => d.HospitalizacionId != null)
                        .Select(d => d.HospitalizacionId)
                        .FirstOrDefault();

                    if (hospitalizacionId != null)
                    {
                        var hospi = _hospitalizacionRepository.Get(
                            (int)hospitalizacionId,
                            true,   // includeMedicamentos  ← productos/insumos
                            true,   // includeServicios      ← servicios clínicos
                            false,  // includeExamenes
                            true,   // includePaquetes
                            false,  // includeConsultas
                            false); // includeOrdenesMedicas

                        if (hospi != null)
                        {
                            // ── DEBUG: ver qué trae el repositorio ──
                            Console.WriteLine($"[HOSPI {hospi.Id}] Productos: {hospi.HospitalizacionesProductos?.Count ?? 0}");
                            Console.WriteLine($"[HOSPI {hospi.Id}] Servicios: {hospi.HospitalizacionesServicios?.Count ?? 0}");
                            Console.WriteLine($"[HOSPI {hospi.Id}] Paquetes: {hospi.HospitalizacionesPaquetesHospitalizacion?.Count ?? 0}");
                            Console.WriteLine($"[HOSPI {hospi.Id}] CategoriaHabitacionTarifa: {hospi.CategoriaHabitacionTarifa?.ValorTarifa}");

                            var detallesGenerados = new List<DetalleVenta>();

                            // ── 1. Estadía / habitación ──────────────────────────────
                            if (hospi.CategoriaHabitacionTarifa != null)
                            {
                                var fechaFin = DateTime.Now.Date;
                                var fechaIni = hospi.FechaInicio.Date;
                                var noches = (fechaIni == fechaFin) ? 1 : (fechaFin - fechaIni).Days;
                                var tarifa = Convert.ToDecimal(hospi.CategoriaHabitacionTarifa.ValorTarifa);
                                var subtotal = Math.Round(tarifa * noches, 2);

                                detallesGenerados.Add(new DetalleVenta
                                {
                                    BienOServicio = "S",
                                    Cantidad = noches,
                                    Precio = tarifa,
                                    Subtotal = subtotal,
                                    Total = subtotal,
                                    Descuento = 0,
                                    Servicio = new Servicio
                                    {
                                        NombreServicio = $"Estadía en habitación ({noches} noche(s))"
                                    }
                                });
                            }

                            // ── 2. Paquetes ──────────────────────────────────────────
                            if (hospi.HospitalizacionesPaquetesHospitalizacion != null)
                            {
                                foreach (var paq in hospi.HospitalizacionesPaquetesHospitalizacion
                                                         .Where(p => !p.Eliminado))
                                {
                                    var precio = paq.PaqueteHospitalizacion?.Precio ?? 0;
                                    var subtotal = Math.Round(precio, 2);

                                    detallesGenerados.Add(new DetalleVenta
                                    {
                                        BienOServicio = "S",
                                        Cantidad = 1,
                                        Precio = precio,
                                        Subtotal = subtotal,
                                        Total = subtotal,
                                        Descuento = 0,
                                        Servicio = new Servicio
                                        {
                                            NombreServicio = paq.PaqueteHospitalizacion?.NombrePaquete ?? "Paquete"
                                        }
                                    });
                                }
                            }

                            // ── 3. Medicamentos / insumos aplicados ──────────────────
                            if (hospi.HospitalizacionesProductos != null)
                            {
                                foreach (var prod in hospi.HospitalizacionesProductos)
                                {
                                    var aplicadas = HospitalizacionCargosHelper
                                        .AplicacionesVigentes(prod.HospitalizacionesProductosAplicaciones)
                                        .ToList();
                                    if (!aplicadas.Any()) continue;

                                    var cantidad = aplicadas.Sum(a => a.Cantidad);
                                    if (cantidad <= 0) continue;

                                    var subtotal = Math.Round(cantidad * prod.PrecioValor, 2);
                                    detallesGenerados.Add(new DetalleVenta
                                    {
                                        BienOServicio = "B",
                                        Cantidad = cantidad,
                                        Precio = Math.Round(prod.PrecioValor, 2),
                                        Subtotal = subtotal,
                                        Total = subtotal,
                                        Descuento = 0,
                                        Producto = new Producto
                                        {
                                            NombreProducto = prod.Producto?.NombreProducto ?? "Medicamento/Insumo"
                                        }
                                    });
                                }
                            }

                            // ── 3b. Control de insumos directos (hospitalización) ──
                            if (hospi.HospitalizacionInsumosDirectos != null)
                            {
                                foreach (var insumo in hospi.HospitalizacionInsumosDirectos.Where(i => !i.Eliminado))
                                {
                                    var aplicadas = HospitalizacionCargosHelper
                                        .AplicacionesVigentesInsumoDirecto(insumo.Aplicaciones)
                                        .ToList();
                                    if (!aplicadas.Any()) continue;

                                    var cantidad = aplicadas.Sum(a => a.Cantidad);
                                    if (cantidad <= 0) continue;

                                    var subtotal = Math.Round(aplicadas.Sum(a => a.Cantidad * insumo.PrecioValor), 2);
                                    detallesGenerados.Add(new DetalleVenta
                                    {
                                        BienOServicio = "B",
                                        Cantidad = cantidad,
                                        Precio = Math.Round(insumo.PrecioValor, 2),
                                        Subtotal = subtotal,
                                        Total = subtotal,
                                        Descuento = 0,
                                        Producto = new Producto
                                        {
                                            NombreProducto = insumo.Producto?.NombreProducto ?? "Insumo/Medicamento"
                                        }
                                    });
                                }
                            }

                            // ── 4. Servicios clínicos ────────────────────────────────
                            if (hospi.HospitalizacionesServicios != null)
                            {
                                foreach (var svc in hospi.HospitalizacionesServicios
                                                         .Where(s => !s.Eliminado)) // ✅ sin filtro de Aplicado
                                {
                                    decimal precioSvc = ObtenerPrecioUnitarioHospitalizacionServicio(svc);
                                    var subtotal = Math.Round(svc.Cantidad * precioSvc, 2);

                                    detallesGenerados.Add(new DetalleVenta
                                    {
                                        BienOServicio = "S",
                                        Cantidad = (int)Math.Round(svc.Cantidad, 0),
                                        Precio = Math.Round(precioSvc, 2),
                                        Subtotal = subtotal,
                                        Total = subtotal,
                                        Descuento = 0,
                                        Servicio = new Servicio
                                        {
                                            NombreServicio = svc.Servicio?.NombreServicio ?? "Servicio clínico"
                                        }
                                    });
                                }
                            }
                            // ── 5. Inyectar en la venta ──────────────────────────────
                            venta.DetalleVenta = detallesGenerados;

                            if (venta.Clientes == null)
                            {
                                Console.WriteLine("Venta sin cliente asignado. Buscando nombre del paciente para el recibo...");
                                string nombrePacienteRecibo = "Consumidor Final";

                                if (venta.PacienteId != null)
                                {
                                    var pacienteBd = _context.Pacientes
                                        .FirstOrDefault(p => p.Id == venta.PacienteId);
                                    Console.WriteLine($"Paciente encontrado: {pacienteBd?.Nombre}");
                                    if (pacienteBd != null)
                                        nombrePacienteRecibo = pacienteBd.Nombre;

                                }

                                Console.WriteLine($"Nombre para el recibo: {nombrePacienteRecibo}");

                                venta.Clientes = new Clientes
                                {
                                    Nombre = nombrePacienteRecibo,
                                    Nit = venta.Nit ?? "CF",
                                    Direccion = venta.Direccion ?? "N/A",
                                };
                            }

                            // ── 7. MontoPago = lo efectivamente cobrado en este pago ─
                            venta.MontoPago = venta.Pagos?.Sum(p => p.Monto)
                                              ?? detallesGenerados.Sum(d => d.Total);
                        }
                    }
                }
            }

            // ── Altura dinámica ──────────────────────────────────────────────────────
            var items = venta.DetalleVenta?.Count ?? 0;
            var pagos = venta.Pagos?.Count ?? 0;

            double altoMm = 130;
            altoMm += items * 7.0;
            altoMm += pagos * 6.0;
            altoMm += 10;
            if (altoMm < 140) altoMm = 140;
            if (altoMm > 400) altoMm = 400;

            var options = new ConvertOptions
            {
                PageMargins = { Top = 0, Left = 0, Right = 0, Bottom = 0 },
                PageWidth = 58.0,
                PageHeight = altoMm
            };

            _generatePdf.SetConvertOptions(options);

            return await RenderPdfAsync("Views/CrearPDF/ReciboVentaPdf.cshtml", venta);
        }


        public async Task<IActionResult> TicketPdf(int? id) //obtenemos la informacion para el turno del ticket  y lo enviamos a la vista turnoPDF
        {
            if (id == null)
            {
                return StatusCode(404);
            }

            var cita = _citasRepository.GetCita((int)id);

            if (cita == null)
            {
                return StatusCode(400);
            }

            var options = new ConvertOptions
            {
                PageMargins = { Top = 0, Left = 0, Right = 0, Bottom = 0 },
                PageWidth = 58.0,
                PageHeight = 600,
            };




            _generatePdf.SetConvertOptions(options);

            return await RenderPdfAsync("Views/CrearPDF/TurnoPDF.cshtml", cita);
        }



        [HttpPost]
        public async Task<IActionResult> CotizacionPDF([FromBody] CotizacionViewModel model)
        {
            if (model == null || model.Productos == null || model.Proveedores == null)
            {
                return BadRequest("Datos de cotización no válidos.");
            }

            // Aseguramos colecciones
            if (model.ProveedorPrincipalPorItem == null)
                model.ProveedorPrincipalPorItem = new Dictionary<string, string>();

            if (model.Productos == null)
                model.Productos = new List<ProductoTrasladoViewModel>();

            if (model.Proveedores == null)
                model.Proveedores = new List<string>();

            // 1. Calcular proveedor principal por producto (cantidad > 0)
            foreach (var producto in model.Productos)
            {
                // Clave robusta del item
                var itemKey = !string.IsNullOrWhiteSpace(producto.ProductoId)
                    ? producto.ProductoId
                    : (!string.IsNullOrWhiteSpace(producto.CodigoReferencia)
                        ? producto.CodigoReferencia
                        : producto.ProductoNombre);

                if (string.IsNullOrWhiteSpace(itemKey))
                    continue;

                if (producto.CantidadesProveedores == null)
                    producto.CantidadesProveedores = new Dictionary<string, int>();

                // Si ya hay principal para este item, lo respetamos
                if (model.ProveedorPrincipalPorItem.ContainsKey(itemKey))
                    continue;

                // Candidatos con cantidad > 0
                var candidatos = producto.CantidadesProveedores
                    .Where(kv => kv.Value > 0)
                    .ToList();

                if (!candidatos.Any())
                    continue; // no hay ganador claro

                // Ganador = proveedor con mayor cantidad
                var ganador = candidatos
                    .OrderByDescending(kv => kv.Value)
                    .First()
                    .Key;

                if (!string.IsNullOrWhiteSpace(ganador))
                {
                    model.ProveedorPrincipalPorItem[itemKey] = ganador;
                }
            }

            // 2. Detectar si es compra dividida o proveedor único (en base a cantidades > 0)
            bool hayCompraDividida = model.Productos.Any(p =>
                p.CantidadesProveedores != null &&
                p.CantidadesProveedores.Count(kv => kv.Value > 0) > 1);

            ViewBag.EsCompraDividida = hayCompraDividida;
            ViewBag.EsCompraUnica = !hayCompraDividida;
            ViewBag.TipoCompra = hayCompraDividida ? "CompraDividida" : "ProveedorUnico";

            // 3. Opciones de PDF
            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };

            _generatePdf.SetConvertOptions(options);

            // 4. Generar el PDF con la vista actual (comparativa)
            return await RenderPdfAsync("Views/CrearPDF/CotizacionPDF.cshtml", model);
        }


        public async Task<IActionResult> CuentasPorCobrarReciboPagoPDF(int? id)
        {
            if (id == null)
            {
                return StatusCode(404);
            }

            var venta = _ventaRepository.Get((int)id);

            if (venta == null)
            {
                return StatusCode(400);
            }

            // var model = new VentaBaseViewModel(){

            //     Venta = venta,
            // };

            // int tam = model.Venta.DetalleVenta.Count();

            // var view = new ViewAsPdf("ReciboVentaPdf", model)
            // {
            //     PageMargins = { Left = 5, Bottom = 2, Right = 5, Top = 2 }, 
            // };

            // view.PageWidth = 40;
            // view.PageHeight =(93 +(tam*5)); 

            var options = new ConvertOptions
            {
                PageMargins = { Top = 0, Left = 0, Right = 0, Bottom = 0 },
                // PageSize = Wkhtmltopdf.NetCore.Options.Size.B9,
                PageWidth = 58.0,
                PageHeight = 600,
            };

            // return new ViewAsPdf("ReciboVentaPdf",model)
            //  {  
            //   PageWidth: 600,
            //  // PageSize=Rotativa.Options.Size.A4, 

            //  };

            _generatePdf.SetConvertOptions(options);

            return await RenderPdfAsync("Views/CrearPDF/CuentasPorCobrarReciboPagoPDF.cshtml", venta);
        }

        //public async Task<IActionResult> ReciboServicios(int? id)
        //{
        //    if (id == null)
        //    {
        //        return StatusCode(404);
        //    }

        //    var ventaServicio = _ventaServicioRepository.Get((int)id);

        //    if (ventaServicio == null)
        //    {
        //        return StatusCode(400);
        //    }

        //    var options = new ConvertOptions
        //    {
        //        PageMargins = { Top = 0, Left = 0, Right = 0, Bottom = 0 },
        //        // PageSize = Wkhtmltopdf.NetCore.Options.Size.B9,
        //        PageWidth = 58.0,
        //        PageHeight = 600,
        //    };

        //    _generatePdf.SetConvertOptions(options);

        //    return await RenderPdfAsync("Views/CrearPDF/ReciboServicios.cshtml", ventaServicio);
        //}

        public IActionResult Utilidad()
        {

            var ventas = _ventaRepository.GetListado();

            var model = new VentaBaseViewModel(0)
            {

                ListaVentas = ventas,
            };

            return new ViewAsPdf("Utilidad", model);

        }

        public IActionResult Cotizacion(int id)
        {

            var cotizacion = _cotizacionRepository.Get(id);


            //return View(cotizacion);
            return new ViewAsPdf("Cotizacion", cotizacion);
        }

        //      public IActionResult FaltantesPdf()
        //      {

        //         var productos = _productoRepository.GetListadoFaltantes();



        //             return new ViewAsPdf("FaltantesPdf",productos)
        //          {  
        //     //CustomSwitches = "--disable-smart-shrinking",
        //     CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12",
        //    // PageMargins = new Rotativa.Options.Margins(40, 10, 10, 10),
        //    PageMargins = { Left = 5, Bottom = 8, Right = 5, Top = 5 }, 
        //   //PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
        //   // PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,

        //          }; 
        //     }

        public async Task<IActionResult> ProveedoresPdf()
        {

            var proveedores = _proveedorRepository.GetList();

            return await RenderPdfAsync("Views/CrearPDF/ProveedoresPdf.cshtml", proveedores);
        }

        public async Task<IActionResult> EmpleadosPdf()
        {

            var empleados = _empleadoRepository.GetList();

            return await RenderPdfAsync("Views/CrearPDF/EmpleadosPdf.cshtml", empleados);
        }

        public async Task<IActionResult> MedicosPdf()
        {

            var medicos = _empleadoRepository.GetList();

            return await RenderPdfAsync("Views/CrearPDF/MedicosPdf.cshtml", medicos);
        }

        public async Task<IActionResult> ProductosPdf()
        {

            var producto = _productoRepository.GetListPdf();

            return await RenderPdfAsync("Views/CrearPDF/ProductosPdf.cshtml", producto);
        }

        public async Task<IActionResult> ClientesPdf()
        {

            var clientes = _clienteRepository.GetList(); // Asegúrate de que esto devuelve List<Clientes>
            return await RenderPdfAsync("Views/CrearPDF/ClientesPdf.cshtml", clientes);

        }

        public IActionResult ImprimirPDFVentas(IList<Venta> venta)
        {
            return new ViewAsPdf("ImprimirPDFVentas", venta);
        }



        public IActionResult ComprasPdf(string fecha, int? empleadoid)
        {
            if (string.IsNullOrWhiteSpace(fecha))
                return BadRequest("Debe enviar la fecha o el rango de fechas.");

            // Normaliza posibles guiones largos y espacios
            var normalized = fecha.Replace('—', '-').Replace('–', '-');
            var partes = normalized.Split('-', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            // Soporta tu formato original y otros comunes (con/sin hora)
            var formatos = new[]
            {
                "MM/dd/yyyy hh:mm tt", // tu formato original (12h con AM/PM)
                "MM/dd/yyyy HH:mm",    // 24h
                "MM/dd/yyyy",          // solo fecha
                "dd/MM/yyyy HH:mm",
                "dd/MM/yyyy",
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-dd"
            };

            // Usamos Invariant para números/AM-PM; estos formatos numéricos no dependen de cultura
            var cultura = CultureInfo.InvariantCulture;

            bool ParseFecha(string s, out DateTime result)
                => DateTime.TryParseExact(s, formatos, cultura, DateTimeStyles.None, out result);

            DateTime inicio, fin;

            if (partes.Length == 2)
            {
                // Rango
                if (!ParseFecha(partes[0], out var d1) || !ParseFecha(partes[1], out var d2))
                    return BadRequest("Formato de rango de fechas inválido. Ej: 11/01/2025 12:00 AM - 11/04/2025 11:59 PM");

                // Asegura orden
                if (d1 > d2) (d1, d2) = (d2, d1);

                // Tomamos días completos: desde el inicio del primer día hasta el final del último día
                inicio = d1.Date;
                fin = d2.Date.AddDays(1); // exclusivo (incluye todo el último día)
            }
            else
            {
                // Un solo día
                if (!ParseFecha(partes[0], out var d))
                    return BadRequest("Formato de fecha inválido. Ej: 11/01/2025 o 11/01/2025 12:00 AM");

                inicio = d.Date;
                fin = d.Date.AddDays(1); // exclusivo
            }

            // Llama a tus repos según haya empleado o no
            var compras = (empleadoid == null)
                ? _compraRepository.GetListadoFecha(inicio, fin)
                : _compraRepository.GetListadoFechaEmpleado(inicio, fin, empleadoid);

            // Genera el PDF con tu vista existente
            return new ViewAsPdf("ComprasPdf", compras);
        }


        //public IActionResult VentasServiciosPdf(string fecha, int? empleadoid)
        //{

        //    var fechas = fecha.Split('-');
        //    var ventas = new List<VentaServicio>();

        //    if (empleadoid == null)
        //    {

        //        ventas = _ventaServicioRepository.GetListadoFecha(Convert.ToDateTime(fechas[0]), Convert.ToDateTime(fechas[1]).AddDays(1));
        //        //return Json (ventas);
        //    }
        //    else
        //    {
        //        ventas = _ventaServicioRepository.GetListadoFechaEmpleado(Convert.ToDateTime(fechas[0]), Convert.ToDateTime(fechas[1]).AddDays(1), empleadoid);
        //        //return Json (ventas);
        //    }

        //    return new ViewAsPdf("VentasServiciosPdf", ventas);
        //}

        public IActionResult ServiciosPdf()
        {

            var servicios = _servicioRepository.GetList();



            return new ViewAsPdf("ServiciosPdf", servicios);
        }

        public IActionResult RutasPdf()
        {

            var servicios = _rutaRepository.GetList();


            return new ViewAsPdf("RutasPdf", servicios);
        }

        public IActionResult GastosPdf()
        {

            var gastos = _gastoRepository.GetList();


            return new ViewAsPdf("GastosPdf", gastos);
        }

        public IActionResult CategoriasPdf()
        {

            var categorias = _categoriaRepository.ListarCategorias();


            return new ViewAsPdf("CategoriasPdf", categorias);
        }

        public IActionResult CategoriasGastosPdf()
        {

            var categorias = _categoriaGastoRepository.ListarCategorias();


            return new ViewAsPdf("CategoriasGastosPdf", categorias);
        }
        // public IActionResult UsuariosPdf()
        //  {
        //    var usuarios = new LogoutModel(_signInManager, _logger) { };

        //     usuarios.Init(_userRepository);

        //     return new ViewAsPdf("UsuariosPdf",usuarios);
        // }

        public async Task<IActionResult> CajasPdf(string fecha)
        {

            var fechas = fecha.Split('-');


            var cajas = _cajaRepository.GetListadoFecha(Convert.ToDateTime(fechas[0]), Convert.ToDateTime(fechas[1]).AddDays(1),
                (int)AmbienteEnum.Global);

            var model = new CajaBaseViewModel()
            {
                ListaCajas = cajas
            };
            //return Json (ventas);

            return await RenderPdfAsync("Views/CrearPDF/CajasPdf.cshtml", model);
        }


        //public IActionResult CajaDetallePdfClinica(int? id)
        //{
        //    if (id == null)
        //    {
        //        return BadRequest("request is incorrect");
        //    }

        //    var caja = _cajaClinicaRepository.GetCaja((int)id);

        //    if (caja == null)
        //    {
        //        return StatusCode(404);
        //    }

        //    var model = new CajaClinicaBaseViewModel()
        //    {
        //        CajaClinica = caja
        //    };

        //    return new ViewAsPdf("CajaDetalleClinicaPDF", model);
        //}

        public async Task<IActionResult> EnviosPdf(string fecha)
        {
            var fechas = fecha.Split('-');
            var inicio = DateTime.ParseExact(fechas[0].Trim(), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            var fin = DateTime.ParseExact(fechas[1].Trim(), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture).AddDays(1);
            var envios = _envioRepository.GetListadoFecha(inicio, fin);

            return await RenderPdfAsync("Views/CrearPDF/EnviosPdf.cshtml", envios);
        }

        public async Task<IActionResult> CategoriasLabClinico()
        {

            var lista = _laboratorioClinico.GetListCategoriasLab();
            return await RenderPdfAsync("Views/CrearPDF/CategoriasLabClinicoPDF.cshtml", lista);

        }

        public async Task<IActionResult> GenerarReporteExamen(int id)
        {
            var lista = _laboratorioClinico.GetExamenRealizado(id);

            return await RenderPdfAsync("Views/CrearPDF/GenerarReporteExamen.cshtml", lista);
        }


        public async Task<IActionResult> OrdenCompraPDF(int ordenCompraId)
        {
            var ordenCompraBd = _compraRepository.Get(ordenCompraId);
            var proveedor = ordenCompraBd.Proveedor ?? new Proveedor();
            var empleado = ordenCompraBd.Empleado ?? new Empleado();
            var tipoCompra = ordenCompraBd.TipoCompra ?? new TipoCompra();
            string fechaLimite = null;
            if (ordenCompraBd.FechaLimite != null)
            {
                fechaLimite = ((DateTime)ordenCompraBd.FechaLimite).ToString("dd-MM-yyyy");
            }
            var ordenCompraDetalles = new CompraBaseViewModel
            {
                EstablecimientoDireccion = _configuration["EstablecimientoDireccion"],
                EncabezadoProveedor = proveedor.Nombre,
                EncabezadoEmpleadoNombre = empleado.NombreYApellidos,
                EncabezadoFechaLimite = fechaLimite,
                EncabezadoTipoCompraNombre = tipoCompra.Tipo,
                ProductosComprados = new List<CompraProductoCompradoViewModel>()
            };
            #region PRODUCTOS COMPRADOS
            if (ordenCompraBd.DetalleCompras != null)
            {
                foreach (var producto in ordenCompraBd.DetalleCompras)
                {
                    var productoBd = producto.Producto ?? new Producto();
                    ordenCompraDetalles.ProductosComprados.Add(new CompraProductoCompradoViewModel
                    {
                        ProductoCodigo = productoBd.CodigoReferencia,
                        NombreProducto = productoBd.NombreProducto,
                        Cantidad = producto.Cantidad,
                        PrecioCompra = producto.Precio
                    });
                }
            }
            #endregion

            return await RenderPdfAsync("Views/CrearPDF/OrdenCompraPDF.cshtml", ordenCompraDetalles);
        }

        public async Task<IActionResult> PaqueteCotizacionPDF(int paqueteId, int? pacienteId)
        {
            var paquete = _hospitalizacionRepository.GetPaqueteHospitalizacion(paqueteId);
            if (paquete == null)
                return NotFound($"No existe el paquete con id {paqueteId}.");

            var pacienteNombre = "-";
            var pacienteDpi = "-";

            // Si luego reactivas la lectura de paciente, valida nulls igual que con paquete.

            var model = new HospitalizacionPaqueteCotizacionPdfViewModel
            {
                Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                PaqueteId = paquete.Id,                    // si tu entidad tiene Id
                Nombre = paquete.NombrePaquete ?? "-",
                Descripcion = paquete.Descripcion ?? "-",
                CodigoInterno = paquete.CodigoInterno ?? "-",
                PacienteNombre = pacienteNombre,
                PacienteDpi = pacienteDpi,
                PrecioPaquete = paquete.Precio ?? 0m,
                Productos = new List<HospitalizacionPaqueteProductoAgregadoViewModel>(),
                Servicios = new List<HospitalizacionPaqueteServicioAgregadoViewModel>(),
                Laboratorios = new List<HospitalizacionPaqueteLaboratorioAgregadoViewModel>()
            };

            // Detalles del paquete (null-safe)
            var detalles = (paquete.DetallePaquetesHospitalizacion ?? Enumerable.Empty<DetallePaqueteHospitalizacion>())
    .Where(d => !d.Eliminado);
            // var detalles = paquete.DetallePaquetesHospitalizacion ?? Enumerable.Empty<DetallePaqueteHospitalizacion>();
            foreach (var detalle in detalles)
            {
                if (detalle == null) continue;

                var cantidad = (int)(detalle.Cantidad);

                // Productos
                if (detalle.ProductoId != null)
                {
                    var producto = detalle.Producto; // puede ser null
                    var unidad = detalle.UnidadMedidaVenta;
                    model.Productos.Add(new HospitalizacionPaqueteProductoAgregadoViewModel
                    {
                        ProductoNombre = producto?.NombreProducto ?? "-",
                        UnidadMedidaVentaNombre = unidad?.Nombre ?? "UN",
                        Cantidad = cantidad
                    });
                }

                // Servicios
                if (detalle.ServicioId != null)
                {
                    var servicio = detalle.Servicio;
                    model.Servicios.Add(new HospitalizacionPaqueteServicioAgregadoViewModel
                    {
                        ServicioCodigo = servicio?.CodigoInterno ?? "-",
                        ServicioNombre = servicio?.NombreServicio ?? "-",
                        Cantidad = cantidad
                    });
                }

                // Laboratorios
                if (detalle.LaboratorioId != null)
                {
                    var laboratorio = detalle.Laboratorio;
                    model.Laboratorios.Add(new HospitalizacionPaqueteLaboratorioAgregadoViewModel
                    {
                        Id = laboratorio?.Id ?? 0,
                        NombreExamen = laboratorio?.NombreExamen ?? "-",
                        Cantidad = cantidad
                    });
                }
            }

            return await RenderPdfAsync("Views/CrearPDF/PaqueteCotizacionPDF.cshtml", model);
        }


        private (Empleado medico, string firmaEmpleado, string telefonoMedico) ObtenerMedicoFirma(int? medicoId)
        {
            if (!medicoId.HasValue)
                throw new Exception("No se encontró el médico asociado.");

            var medico = _empleadoRepository.Get(medicoId.Value);
            if (medico == null)
                throw new Exception("No se pudo encontrar el médico.");

            // Devuelve el objeto 'medico', su firma (o un guion si es nula), y el teléfono del médico.
            return (medico, medico.FirmaEmpleado ?? "", medico.Telefono ?? "");
        }


        private string CalcularEdad(DateTime? fechaNacimiento)
        {
            if (!fechaNacimiento.HasValue)
                return "-";

            var birth = fechaNacimiento.Value.Date;
            var today = DateTime.Today;

            int years = today.Year - birth.Year;
            int months = today.Month - birth.Month;
            int days = today.Day - birth.Day;

            if (days < 0)
            {
                months--;
                var prevMonth = today.AddMonths(-1);
                days += DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            string yearText = years == 1 ? "Año" : "Años";
            string monthText = months == 1 ? "Mes" : "Meses";
            string dayText = days == 1 ? "Día" : "Días";

            return $"{years} {yearText} {months} {monthText} y {days} {dayText}";
        }

        private NotaOperatoriaVM MapNotaOperatoriaToPdfVm(
            NotaOperatoria notaOperatoria,
            string contentRoot,
            (string Nombre, string Especialidad, string Colegiado, string FirmaBase64)? medicoTratante = null)
        {
            if (notaOperatoria == null)
                return null;

            string empleadoText = notaOperatoria.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.EmpleadoText ?? "Sin asignar";

            string colegioEmpleado = notaOperatoria.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.Empleado?.Colegiado ?? "No disponible";

            string nombreProfesional = "Sin asignar";
            if (notaOperatoria.User?.Persona != null)
                nombreProfesional = notaOperatoria.User.Persona.NombreYApellidos;

            string firmaBase64 = PdfReportHelper.ObtenerFirmaEmpleadoPorUser(notaOperatoria.User, _empleadoRepository, contentRoot);
            if (string.IsNullOrEmpty(firmaBase64) && !string.IsNullOrWhiteSpace(notaOperatoria.Cirujano))
            {
                var empCirujano = _empleadoRepository.GetList()
                    .FirstOrDefault(e => string.Equals(e.NombreYApellidos, notaOperatoria.Cirujano, StringComparison.OrdinalIgnoreCase));
                if (empCirujano != null)
                    firmaBase64 = PdfReportHelper.ObtenerFirmaBase64Local(empCirujano.FirmaEmpleado, contentRoot);
            }

            if (string.IsNullOrEmpty(firmaBase64) && medicoTratante.HasValue && !string.IsNullOrEmpty(medicoTratante.Value.FirmaBase64))
                firmaBase64 = medicoTratante.Value.FirmaBase64;

            if (string.IsNullOrEmpty(firmaBase64) && notaOperatoria.Hospitalizacion != null)
            {
                var medicoFallback = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                    notaOperatoria.Hospitalizacion, _empleadoRepository, contentRoot);
                firmaBase64 = medicoFallback.FirmaBase64;
                if (string.IsNullOrWhiteSpace(empleadoText) || empleadoText == "Sin asignar")
                    empleadoText = medicoFallback.Nombre;
            }

            var nombreCirujano = !string.IsNullOrWhiteSpace(notaOperatoria.Cirujano)
                ? notaOperatoria.Cirujano
                : nombreProfesional;

            empleadoText = PdfReportHelper.ResolverNombreMedicoPdf(empleadoText, nombreCirujano, empleadoText);
            if (string.IsNullOrEmpty(firmaBase64) && !string.IsNullOrWhiteSpace(nombreCirujano))
                firmaBase64 = PdfReportHelper.ObtenerFirmaEmpleadoPorNombre(nombreCirujano, _empleadoRepository, contentRoot);

            if ((colegioEmpleado == "No disponible" || string.IsNullOrWhiteSpace(colegioEmpleado)) && !string.IsNullOrWhiteSpace(empleadoText))
                colegioEmpleado = PdfReportHelper.ObtenerColegiadoPorNombre(empleadoText, _empleadoRepository, colegioEmpleado);

            if ((colegioEmpleado == "No disponible" || string.IsNullOrWhiteSpace(colegioEmpleado))
                && medicoTratante.HasValue
                && !string.IsNullOrWhiteSpace(medicoTratante.Value.Colegiado))
                colegioEmpleado = medicoTratante.Value.Colegiado;

            return new NotaOperatoriaVM
            {
                Id = notaOperatoria.Id,
                HospitalizacionId = notaOperatoria.HospitalizacionId,
                DiagnosticoPreOperatorio = notaOperatoria.DiagnosticoPreOperatorio ?? notaOperatoria.Diagnostico,
                DiagnosticoPostOperatorio = notaOperatoria.DiagnosticoPostOperatorio,
                OperacionEfectuada = notaOperatoria.OperacionEfectuada ?? notaOperatoria.Evolucion,
                HallazgosTransOperatorios = notaOperatoria.HallazgosTransOperatorios,
                FechaOperacion = notaOperatoria.FechaOperacion?.ToString("dd/MM/yyyy"),
                HoraComenzo = notaOperatoria.HoraComenzo,
                HoraTermino = notaOperatoria.HoraTermino,
                Cirujano = nombreCirujano,
                PrimerAyudante = notaOperatoria.PrimerAyudante,
                SegundoAyudante = notaOperatoria.SegundoAyudante,
                Anestesista = notaOperatoria.Anestesista,
                Instrumentista = notaOperatoria.Instrumentista,
                Circulante = notaOperatoria.Circulante,
                FechaRegistro = HospitalTimeHelper.ToGuatemalaDisplay(notaOperatoria.FechaRegistro).ToString("dd/MM/yyyy hh:mm tt"),
                Profesional = nombreCirujano,
                PacienteNombre = notaOperatoria.Hospitalizacion?.Paciente?.Nombre ?? "Sin asignar",
                PacienteEdad = CalcularEdad(notaOperatoria.Hospitalizacion?.Paciente?.FechaNacimiento),
                PacienteSexoText = notaOperatoria.Hospitalizacion?.Paciente?.sexoText ?? "Sin asignar",
                Colegiado = colegioEmpleado,
                FirmaBase64 = firmaBase64
            };
        }

        public async Task<IActionResult> ConsultaExamenesPDF(int consultaId)
        {
            var consulta = _consultasRepository.GetConsulta(consultaId);

            var firma = ObtenerMedicoFirma(consulta.Citas.EmpleadoId);

            var examenes = _consultasRepository.GetExamenesAgregadosConsulta(consultaId);

            var model = new ConsultasViewModel
            {
                ExamenesAgregados = new List<ConsultaExamenAgregadoViewModel>()
            };

            if (examenes != null)
            {
                foreach (var examen in examenes)
                {
                    var examenLabClinico = examen.ExamenLabClinico ?? new ExamenLabClinico();
                    model.ExamenesAgregados.Add(new ConsultaExamenAgregadoViewModel
                    {
                        ExamenNombre = examenLabClinico.NombreExamen,
                        ExamenCodigo = examenLabClinico.CodigoInterno,
                        Cantidad = examen.Cantidad
                    });
                }
            }


            string directorio = ContentRoot;
            string rutaRelativa = firma.firmaEmpleado.TrimStart('/');

            string rutaFinal = Path.Combine(directorio, "wwwroot", rutaRelativa);


            if (System.IO.File.Exists(rutaFinal))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(rutaFinal);
                model.EstablecimientoImagenFirmaMedico = $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
            }

            model.EstablecimientoImagenFirma = _configuration["EstablecimientoImagenFirma"];
            model.EstablecimientoImagenLogo = _configuration["ImagenLogoBase64"];
            model.EstablecimientoDireccion = _configuration["EstablecimientoDireccion"];
            model.EstablecimientoTelefono = _configuration["EstablecimientoTelefono"];
            model.EstablecimientoCorreoElectronico = _configuration["EstablecimientoCorreoElectronico"];

            return await RenderPdfAsync("Views/CrearPDF/ConsultaExamenesPDF.cshtml", model);
        }

        public async Task<IActionResult> generarExamenesSolicitarPdf(int prescripcionId)
        {
            var prescripcion = _consultasRepository.GetPrescripcion(prescripcionId);

            var paciente = new Paciente();
            var cita = new Cita();
            if (prescripcion.Citas != null)
            {
                paciente = prescripcion.Citas.Paciente;

            }
            var model = new PrescripcionPdfViewModel
            {
                PacienteNombre = paciente != null ? paciente.Nombre : "-",
                PacienteEdad = paciente != null ? paciente.Edad.ToString() : "-",
                DetallesPrescripcion = new List<ConsultaPrescripcionViewModel>()
            };

            if (prescripcion.DetallePrescripcion != null)
            {
                foreach (var elemento in prescripcion.DetallePrescripcion)
                {
                    model.DetallesPrescripcion.Add(new ConsultaPrescripcionViewModel
                    {
                        Item = elemento.Item,
                        Medicamento = elemento.Medicine,
                        Cantidad = elemento.Cantidad,
                        Observaciones = elemento.Indications
                    });
                }
            }

            return await RenderPdfAsync("Views/CrearPDF/generarExamenesSolicitarPdf.cshtml", model);
        }
        public async Task<IActionResult> generarExamenesSolicitarAlergologiaPdf(int prescripcionId)
        {

            var prescripcion = _consultasRepository.GetPrescripcion(prescripcionId);
            var paciente = new Paciente();

            if (prescripcion.Citas != null)
            {
                paciente = prescripcion.Citas.Paciente;
            }
            var model = new PrescripcionPdfViewModel
            {
                PacienteNombre = paciente != null ? paciente.Nombre : "-",
                PacienteEdad = paciente != null ? paciente.Edad.ToString() : "-",
                PacienteSexo = paciente != null ? paciente.SexoId : 0,
                DetallesPrescripcion = new List<ConsultaPrescripcionViewModel>()
            };

            if (prescripcion.DetallePrescripcion != null)
            {
                var categoria = "";
                foreach (var elemento in prescripcion.DetallePrescripcion)
                {
                    if (elemento.Item >= 1000)
                    {
                        var codigo = elemento.Indications;
                        categoria = _consultasRepository.GetExamenLabClincos(codigo).CategoriaLabClinico.Nombre;

                    }
                    model.DetallesPrescripcion.Add(new ConsultaPrescripcionViewModel
                    {
                        Item = elemento.Item,
                        Medicamento = elemento.Medicine,
                        Cantidad = elemento.Cantidad,
                        Observaciones = elemento.Indications,
                        Categoria = categoria
                    });

                }
            }

            return await RenderPdfAsync("Views/CrearPDF/generarExamenesSolicitarAPdf.cshtml", model);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult FooterResultados()
        {
            return View("_FooterResultados");
        }
        public async Task<IActionResult> GenerarResultados(int id)
        {
            try
            {
                var examen = _laboratorioClinico.GetExamenResultados(id);
                var model = new ExamenResultadosVM
                {
                    DetalleExamenes = examen.DetalleExamenes,
                    EstadoExamen = examen.EstadoExamen,
                    Medicos = examen.Medicos,
                    Paciente = examen.Paciente,
                    Clinicas = examen.Clinicas,
                    FechaRealizacion = examen.FechaRealizacion
                };

                var usuarioIngresa = _userRepository.GetbyId(examen.UsuarioIngresa, false);
                var usuarioSolicita = _userRepository.GetbyId(examen.UsuarioSolicita, false);

                if (usuarioSolicita != null)
                {
                    model.UsuarioSolicita = usuarioSolicita.Email;
                }
                if (usuarioIngresa != null)
                {
                    model.UsuarioIngresa = usuarioIngresa.Email;
                }

                string footerUrl = $"{Request.Scheme}://{Request.Host}/CrearPDF/FooterResultados";

                _generatePdf.SetConvertOptions(new ConvertOptions
                {
                    FooterHtml = footerUrl,
                    FooterSpacing = 5,
                    PageMargins = new Margins { Bottom = 50, Left = 10, Right = 10, Top = 10 },
                    PageSize = Wkhtmltopdf.NetCore.Options.Size.A4
                });

                return await RenderPdfAsync("Views/CrearPDF/GenerarResultados.cshtml", model);
            }
            catch (Exception ex)
            {
                // Manejar el error
                return StatusCode(500, $"Ocurrió un error: {ex.Message}");
            }
        }


        public async Task<IActionResult> HospitalizacionNotaMedica2PDF(int notaMedicaId)
        {
            var notaMedica = _hospitalizacionRepository.GetNotaMedica2(notaMedicaId);

            if (notaMedica == null)
            {
                return NotFound("No se encontró la nota médica solicitada.");
            }

            var contentRoot = ContentRoot;
            var medicoTratante = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                notaMedica.Hospitalizacion, _empleadoRepository, contentRoot);

            var consentimientoNm = ResolverConsentimientoHospitalizacion(notaMedica.Hospitalizacion);
            medicoTratante = PdfReportHelper.ComplementarMedicoTratante(medicoTratante, consentimientoNm, null, null);

            User autorizador = null;
            if (notaMedica.Autorizado && !string.IsNullOrWhiteSpace(notaMedica.UsuarioAutoriza))
            {
                autorizador = _context.Users.AsNoTracking()
                    .FirstOrDefault(u => u.Id == notaMedica.UsuarioAutoriza);
            }

            var model = new NotaMedica2ViewModel
            {
                HistoriaProblema = notaMedica?.HistoriaProblema ?? "No disponible",
                Sintomas = notaMedica?.Sintomas ?? "No disponibles",
                Diagnostico = notaMedica?.Diagnostico ?? "No disponible",
                FechaRegistro = HospitalTimeHelper.ToGuatemalaDisplay(notaMedica.FechaRegistro).ToString("dd/MM/yyyy hh:mm tt"),
                PacienteNombre = notaMedica?.Hospitalizacion?.Paciente?.Nombre ?? "Sin asignar",
                PacienteEdad = CalcularEdad(notaMedica?.Hospitalizacion?.Paciente?.FechaNacimiento),
                PacienteSexoText = notaMedica?.Hospitalizacion?.Paciente?.sexoText ?? "Sin asignar",
                Autorizado = notaMedica.Autorizado,
            };

            PdfReportHelper.CompletarProfesionalNotaMedicaPdf(
                model,
                notaMedica.Profesional,
                medicoTratante,
                notaMedica.Autorizado,
                notaMedica.UsuarioAutoriza,
                autorizador == null
                    ? null
                    : new Dictionary<string, User> { { autorizador.Id, autorizador } },
                _empleadoRepository,
                contentRoot);

            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };

            return await RenderPdfAsync("Views/CrearPDF/HospitalizacionNotaMedicaPDF.cshtml", model, options);
        }

        public async Task<IActionResult> HospitalizacionNotaMedicaPDFGetAll(int hospitalizacionId)
        {
            var notasMedicas = _hospitalizacionRepository.GetNotasMedicasByHospitalizacion(hospitalizacionId);

            if (notasMedicas == null || !notasMedicas.Any())
            {
                return NotFound("No se encontraron notas médicas para la hospitalización solicitada.");
            }

            var paciente = notasMedicas.FirstOrDefault()?.Hospitalizacion?.Paciente;
            var hospitalizacion = notasMedicas.FirstOrDefault()?.Hospitalizacion;

            var empleadoText = notasMedicas.FirstOrDefault()?.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.EmpleadoText ?? "Sin asignar";

            var empleadoEspecialidad = notasMedicas.FirstOrDefault()?.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.Empleado?.Especialidad?.NombreEspecialidad ?? "Sin asignar";


            var colegioEmpleado = notasMedicas.FirstOrDefault()?.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.Empleado?.Colegiado ?? "No disponible";

            var contentRoot = ContentRoot;
            var medicoTratante = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                hospitalizacion, _empleadoRepository, contentRoot);

            var consentimientoNm = ResolverConsentimientoHospitalizacion(hospitalizacion);
            medicoTratante = PdfReportHelper.ComplementarMedicoTratante(medicoTratante, consentimientoNm, null, null);

            var autorizadorIds = notasMedicas
                .Where(n => n.Autorizado && !string.IsNullOrWhiteSpace(n.UsuarioAutoriza))
                .Select(n => n.UsuarioAutoriza)
                .Distinct()
                .ToList();
            var autorizadores = autorizadorIds.Any()
                ? _context.Users.AsNoTracking()
                    .Where(u => autorizadorIds.Contains(u.Id))
                    .ToDictionary(u => u.Id)
                : new Dictionary<string, User>();

            var model = new NotaMedica2ListaViewModel
            {
                PacienteNombre = paciente?.Nombre ?? "Sin asignar",
                PacienteEdad = CalcularEdad(paciente?.FechaNacimiento),
                PacienteSexoText = paciente?.Sexo?.DescripcionSexo ?? paciente?.sexoText ?? "Sin asignar",
                EmpleadoText = medicoTratante.Nombre,
                EmpleadoEspecialidad = empleadoEspecialidad != "Sin asignar" ? empleadoEspecialidad : medicoTratante.Especialidad,
                ColegioEmpleado = medicoTratante.Colegiado,
                NotasMedicas = notasMedicas.Select(nm =>
                {
                    User autorizador = null;
                    if (nm.Autorizado && !string.IsNullOrWhiteSpace(nm.UsuarioAutoriza))
                        autorizadores.TryGetValue(nm.UsuarioAutoriza, out autorizador);

                    var notaVm = new NotaMedica2ViewModel
                    {
                        Id = nm.Id,
                        Diagnostico = nm.Diagnostico ?? "No disponible",
                        FechaRegistro = HospitalTimeHelper.ToGuatemalaDisplay(nm.FechaRegistro).ToString("dd/MM/yyyy hh:mm tt"),
                        Autorizado = nm.Autorizado,
                    };

                    PdfReportHelper.CompletarProfesionalNotaMedicaPdf(
                        notaVm,
                        nm.Profesional,
                        medicoTratante,
                        nm.Autorizado,
                        nm.UsuarioAutoriza,
                        autorizador == null ? null : new Dictionary<string, User> { { autorizador.Id, autorizador } },
                        _empleadoRepository,
                        contentRoot);

                    return notaVm;
                }).ToList()
            };

            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };

            return await RenderPdfAsync("Views/CrearPDF/HospitalizacionNotasMedicasPDFGetAll.cshtml", model, options);
        }

        [HttpGet]
        public async Task<IActionResult> HospitalizacionNotaOperatoria2PDF(int notaOperatoriaId)
        {
            var notaOperatoria = _hospitalizacionRepository.GetNotaOperatoria(notaOperatoriaId);

            if (notaOperatoria == null)
            {
                return NotFound("No se encontró la nota operatoria solicitada.");
            }

            var contentRoot = ContentRoot;
            var model = MapNotaOperatoriaToPdfVm(notaOperatoria, contentRoot);

            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };

            return await RenderPdfAsync("Views/CrearPDF/HospitalizacionNotaOperatoriaPDF.cshtml", model, options);
        }

        [HttpGet]
        public async Task<IActionResult> HospitalizacionNotaOperatoriaPDFGetAll(int hospitalizacionId)
        {
            var notas = _hospitalizacionRepository.GetNotasOperatoriasByHospitalizacion(hospitalizacionId)
                ?.OrderBy(n => n.FechaRegistro)
                .ToList();

            if (notas == null || !notas.Any())
            {
                return NotFound("No se encontraron notas operatorias para esta hospitalización.");
            }

            var primeraNota = notas.First();
            var paciente = primeraNota.Hospitalizacion?.Paciente;
            var consultaBase = primeraNota.Hospitalizacion?.Consultas?.FirstOrDefault();

            var contentRoot = ContentRoot;
            var medicoTratante = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                primeraNota.Hospitalizacion, _empleadoRepository, contentRoot);

            var consentimientoOp = ResolverConsentimientoHospitalizacion(primeraNota.Hospitalizacion);
            medicoTratante = PdfReportHelper.ComplementarMedicoTratante(medicoTratante, consentimientoOp, null, null);

            var model = new NotaOperatoriaListaViewModel
            {
                PacienteNombre = paciente?.Nombre ?? "Sin asignar",
                PacienteEdad = CalcularEdad(paciente?.FechaNacimiento),
                PacienteSexoText = paciente?.Sexo?.DescripcionSexo ?? paciente?.sexoText ?? "Sin asignar",
                EmpleadoText = medicoTratante.Nombre,
                EmpleadoEspecialidad = consultaBase?.Citas?.Empleado?.Especialidad?.NombreEspecialidad
                    ?? medicoTratante.Especialidad
                    ?? "Sin asignar",
                Colegiado = medicoTratante.Colegiado,
                Notas = notas
                    .Select(n => MapNotaOperatoriaToPdfVm(n, contentRoot, medicoTratante))
                    .Where(n => n != null)
                    .ToList()
            };

            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };

            return await RenderPdfAsync("Views/CrearPDF/HospitalizacionNotasOperatoriasPDFGetAll.cshtml", model, options);
        }

        public async Task<IActionResult> HospitalizacionOrdenMedicaPDF(int ordenMedicaId)
        {
            var ordenMedica = _hospitalizacionRepository.GetOrdenMedica(ordenMedicaId);
            if (ordenMedica == null)
            {
                return NotFound("No se encontró la orden médica solicitada.");
            }

            string empleadoText = ordenMedica.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.EmpleadoText ?? "Sin asignar";

            string colegioEmpleado = ordenMedica?.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.Empleado?.Colegiado ?? "No disponible";

            var contentRoot = ContentRoot;
            var medicoTratante = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                ordenMedica.Hospitalizacion, _empleadoRepository, contentRoot);

            var registradoPorOrden = ordenMedica?.Profesional ?? "Sin asignar";

            string firmaOrden = "";
            string nombreFirmante = registradoPorOrden;
            string autorizadoPor = null;

            User profesionalOrden = null;
            if (!string.IsNullOrWhiteSpace(ordenMedica.Profesional))
            {
                profesionalOrden = _context.Users.AsNoTracking()
                    .Include(u => u.Persona)
                    .FirstOrDefault(u => u.UserName == ordenMedica.Profesional || u.Email == ordenMedica.Profesional);
            }

            User autorizador = null;
            if (ordenMedica.Autorizado && !string.IsNullOrWhiteSpace(ordenMedica.UsuarioAutoriza))
            {
                autorizador = _context.Users.AsNoTracking()
                    .Include(u => u.Persona)
                    .FirstOrDefault(u => u.Id == ordenMedica.UsuarioAutoriza);
            }

            var firmante = PdfReportHelper.ResolverFirmanteClinico(
                ordenMedica.Autorizado,
                ordenMedica.UsuarioAutoriza,
                autorizador ?? profesionalOrden,
                autorizador == null
                    ? null
                    : new Dictionary<string, User> { { autorizador.Id, autorizador } },
                _empleadoRepository,
                medicoTratante.FirmaBase64,
                contentRoot,
                registradoPorOrden);
            firmaOrden = firmante.FirmaBase64;
            nombreFirmante = firmante.NombreFirmante;
            autorizadoPor = firmante.AutorizadoPor;

            var nombreMedicoAsignado = PdfReportHelper.ResolverNombreMedicoPdf(empleadoText, registradoPorOrden, medicoTratante.Nombre);
            var colegiadoMedico = colegioEmpleado;
            if (colegiadoMedico == "No disponible" || string.IsNullOrWhiteSpace(colegiadoMedico) || colegiadoMedico == "-")
                colegiadoMedico = PdfReportHelper.ObtenerColegiadoPorNombre(nombreMedicoAsignado, _empleadoRepository, medicoTratante.Colegiado);

            var model = new OrdenMedicaViewModel
            {
                FechaHora = HospitalTimeHelper.ToGuatemalaDisplay(ordenMedica.FechaHora).ToString("dd/MM/yyyy hh:mm tt"),
                Profesional = registradoPorOrden,
                Descripcion = PdfReportHelper.NormalizarHtmlParaPdf(ordenMedica?.Descripcion ?? "No disponible"),
                PacienteNombre = ordenMedica?.Hospitalizacion?.Paciente?.Nombre ?? "Sin asignar",
                PacienteEdad = CalcularEdad(ordenMedica?.Hospitalizacion?.Paciente?.FechaNacimiento),
                PacienteSexoText = ordenMedica?.Hospitalizacion?.Paciente?.sexoText ?? "Sin asignar",
                EmpleadoText = nombreMedicoAsignado,
                ColegioEmpleado = colegiadoMedico,
                Realizada = ordenMedica.Realizada ? "Sí" : "No",
                Autorizado = ordenMedica.Autorizado,
                FirmaBase64 = firmaOrden,
                NombreFirmante = nombreFirmante,
                AutorizadoPor = autorizadoPor
            };


            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };

            return await RenderPdfAsync("Views/CrearPDF/HospitalizacionOrdenMedicaPDF.cshtml", model, options);
        }


        public async Task<IActionResult> PrescripcionPDF(int prescripcionId, int pacienteId)
        {
            var prescripcion = _consultasRepository.GetPrescripcion(prescripcionId);
            if (prescripcion == null)
                return NotFound("No se encontró una prescripción para esta consulta.");

            var paciente = _pacientesRepository.GetPacientePorId(pacienteId);

            var citas = prescripcion.Citas ?? throw new Exception("No se encontraron citas asociadas.");

            var consulta = citas.Consultas.FirstOrDefault();
            if (consulta == null)
                return NotFound("No se encontró la consulta asociada.");

            var fechaProximaConsulta = consulta.FechaProximaConsulta;

            var medicamentosOtros = _context.MedicamentosOtrosConsulta
            .Where(m => m.ConsultaId == consulta.Id)
            .OrderBy(m => m.Id)
            .Select(m => new MedicamentoOtro
            {
                Id = m.Id,
                Nombre = m.Nombre,
                Indicaciones = m.Indicaciones,
                Cantidad = m.Cantidad,
                FechaPrescripcion = m.FechaPrescripcion
            })
            .ToList();

            Console.WriteLine($"=== [PrescripcionPDF] MedicamentosOtros para ConsultaId: {consulta.Id} ===");
            foreach (var m in medicamentosOtros)
            {
                Console.WriteLine($"  -> Id: {m.Id} | Nombre: {m.Nombre} | Cantidad: {m.Cantidad} | Indicaciones: {m.Indicaciones} | Fecha: {m.FechaPrescripcion}");
            }

            var (medico, firmaEmpleado, telefonoMedico) = ObtenerMedicoFirma(citas.EmpleadoId);

            var model = new PrescripcionPdfViewModel
            {
                PacienteNombre = paciente.Nombre ?? "-",
                PacienteEdad = CalcularEdad(paciente.FechaNacimiento),
                PacientePeso = paciente.Peso ?? "-",
                MedicoNombre = citas.EmpleadoText ?? medico.NombreYApellidos ?? "Médico no especificado",
                FirmaEmpleado = firmaEmpleado,
                MedicoCelular = telefonoMedico,
                DetallesPrescripcion = prescripcion.DetallePrescripcion?.Select((elemento, index) => new ConsultaPrescripcionViewModel
                {
                    Item = index + 1,
                    Medicamento = elemento.Producto?.NombreProducto ?? elemento.Medicine ?? "Producto no especificado",
                    UnidadMedidaVentaNombre = elemento.UnidadMedidaVenta?.Nombre ?? "-",
                    Cantidad = elemento.Cantidad,
                    Observaciones = elemento.Indications ?? "-",
                    Color = elemento.Color
                }).ToList(),
                MedicamentosOtros = medicamentosOtros,
                ImagenLogoBase64 = _configuration["ImagenLogoBase64"] ?? "Logo no disponible",
                EstablecimientoDireccion = _configuration["EstablecimientoDireccion"] ?? "Dirección no disponible",
                EstablecimientoTelefono = _configuration["EstablecimientoTelefono"] ?? "Teléfono no disponible",
                EstablecimientoCorreoElectronico = _configuration["EstablecimientoCorreoElectronico"] ?? "Correo no disponible",
                FechaProximaConsulta = fechaProximaConsulta
            };

            return await RenderPdfAsync("Views/CrearPDF/PrescripcionPDF.cshtml", model);
        }


        // public async Task<IActionResult> PrescripcionPDF(int prescripcionId, int pacienteId)
        // {
        //     var prescripcion = _consultasRepository.GetPrescripcion(prescripcionId);
        //     if (prescripcion == null)
        //         return NotFound("No se encontró una prescripción para esta consulta.");

        //     var paciente = _pacientesRepository.GetPacientePorId(pacienteId);

        //     var citas = prescripcion.Citas ?? throw new Exception("No se encontraron citas asociadas.");

        //     // Obtener la primera consulta asociada (o la consulta relevante según tu lógica)
        //     var consulta = citas.Consultas.FirstOrDefault();
        //     if (consulta == null)
        //         return NotFound("No se encontró la consulta asociada.");
        //     var fechaProximaConsulta = consulta.FechaProximaConsulta;


        //     var (medico, firmaEmpleado, telefonoMedico) = ObtenerMedicoFirma(citas.EmpleadoId);

        //     var model = new PrescripcionPdfViewModel
        //     {
        //         PacienteNombre = paciente.Nombre ?? "-",
        //         PacienteEdad = CalcularEdad(paciente.FechaNacimiento),
        //         PacientePeso = paciente.Peso ?? "-",
        //         MedicoNombre = citas.EmpleadoText ?? medico.NombreYApellidos ?? "Médico no especificado",
        //         FirmaEmpleado = firmaEmpleado,
        //         MedicoCelular = telefonoMedico, // Agregando el teléfono al modelo
        //         DetallesPrescripcion = prescripcion.DetallePrescripcion?.Select((elemento, index) => new ConsultaPrescripcionViewModel
        //         {
        //             Item = index + 1,
        //             Medicamento = elemento.Producto?.NombreProducto ?? elemento.Medicine ?? "Producto no especificado",
        //             UnidadMedidaVentaNombre = elemento.UnidadMedidaVenta?.Nombre ?? "-",
        //             Cantidad = elemento.Cantidad,
        //             Observaciones = elemento.Indications ?? "-",
        //             Color = elemento.Color // Aquí se agrega el color

        //         }).ToList(),
        //         ImagenLogoBase64 = _configuration["ImagenLogoBase64"] ?? "Logo no disponible",
        //         EstablecimientoDireccion = _configuration["EstablecimientoDireccion"] ?? "Dirección no disponible",
        //         EstablecimientoTelefono = _configuration["EstablecimientoTelefono"] ?? "Teléfono no disponible",
        //         EstablecimientoCorreoElectronico = _configuration["EstablecimientoCorreoElectronico"] ?? "Correo no disponible",
        //         FechaProximaConsulta = fechaProximaConsulta // Asignar la fecha de la próxima consulta

        //     };

        //     return await RenderPdfAsync("Views/CrearPDF/PrescripcionPDF.cshtml", model);
        // }

        public async Task<IActionResult> GinecologiaConsultaPDF(int consultaId)
        {
            // Obtener la consulta y el paciente
            var consulta = _consultasRepository.GetConsulta(consultaId);
            var paciente = consulta?.Citas?.Paciente ?? new Paciente();
            string edadPaciente = CalcularEdad(paciente.FechaNacimiento);

            // Obtener el ID del médico
            var (medico, firmaEmpleado, telefonoMedico) = ObtenerMedicoFirma(consulta.Citas.EmpleadoId);

            var gineAntNoPatologicos = consulta.ConsultaAntNoPatologicosGinecologia ?? new ConsultaAntNoPatologicosGinecologia();
            var gineAntPatologicos = consulta.ConsultaAntPatologicosGinecologia ?? new ConsultaAntPatologicosGinecologia();
            var examenFisicoGinecologia = consulta.ConsultaExamenFisicoGinecologia ?? new ConsultaExamenFisicoGinecologia();

            var model = new ConsultasViewModel
            {
                // Datos PDF
                EstablecimientoCorreoElectronico = _configuration["EstablecimientoCorreoElectronico"],
                EstablecimientoDireccion = _configuration["EstablecimientoDireccion"],
                EstablecimientoImagenLogo = _configuration["ImagenLogoBase64"],
                EstablecimientoTelefono = _configuration["EstablecimientoTelefono"],

                PacienteNombre = paciente.Nombre ?? "-",
                PacienteEdad = edadPaciente,

                MedicoNombre = medico.NombreYApellidos ?? "Médico no especificado",
                FirmaEmpleado = firmaEmpleado,
                MedicoCelular = telefonoMedico, // Agregando el teléfono al modelo


                // Añadir el motivo de consulta
                GinecologiaConsultaMotivo = consulta?.GinecologiaConsultaMotivo ?? "-",

                #region GINECOLOGIA - ANT NO PATOLOGICOS

                GinecologiaAntNoPatologicosAbortos = gineAntNoPatologicos.Abortos ?? "-",
                GinecologiaAntNoPatologicosCesareas = gineAntNoPatologicos.Cesareas ?? "-",
                GinecologiaAntNoPatologicosCicloMenstrual = gineAntNoPatologicos.CicloMenstrual ?? "-",
                GinecologiaAntNoPatologicosFechaUltimaRegla = gineAntNoPatologicos.FechaUltimaRegla ?? "-",
                GinecologiaAntNoPatologicosGestas = gineAntNoPatologicos.Gestas ?? "-",
                GinecologiaAntNoPatologicosHijosMuertos = gineAntNoPatologicos.HijosMuertos ?? "-",
                GinecologiaAntNoPatologicosHijosVivos = gineAntNoPatologicos.HijosVivos ?? "-",
                GinecologiaAntNoPatologicosLactanciaMaterna = gineAntNoPatologicos.LactanciaMaterna ?? "-",
                GinecologiaAntNoPatologicosMenarquia = gineAntNoPatologicos.Menarquia ?? "-",
                GinecologiaAntNoPatologicosMetodoAnticonceptivo = gineAntNoPatologicos.MetodoAnticonceptivo ?? "-",
                GinecologiaAntNoPatologicosOtros = gineAntNoPatologicos.Otros ?? "-",
                GinecologiaAntNoPatologicosPartos = gineAntNoPatologicos.Partos ?? "-",

                #endregion

                #region GINECOLOGIA - ANT PATOLOGICOS

                GinecologiaAntPatologicosEts = gineAntPatologicos.Ets ?? "-",
                GinecologiaAntPatologicosOtros = gineAntPatologicos.Otros ?? "-",
                GinecologiaAntPatologicosInfecciones = gineAntPatologicos.Infecciones ?? "-",
                GinecologiaAntPatologicosPapanicolau = gineAntPatologicos.Papanicolau ?? "-",

                #endregion

                #region GINECOLOGIA - EXAMEN FISICO

                GinecologiaExamenFisicoEspeculoscopia = examenFisicoGinecologia.Especuloscopia ?? "-",
                GinecologiaExamenFisicoMamas = examenFisicoGinecologia.Mamas ?? "-",
                GinecologiaExamenFisicoTactoRectal = examenFisicoGinecologia.TactoRectal ?? "-",
                GinecologiaExamenFisicoTactoVaginal = examenFisicoGinecologia.TactoVaginal ?? "-",
                GinecologiaExamenFisicoVulvaVagina = examenFisicoGinecologia.VulvaVagina ?? "-",

                #endregion

                #region ECOGRAFIA ENDOCAVITARIA

                EcografiaEndocavitariaUtero = consulta.EcografiaEndocavitariaUtero ?? "-",
                EcografiaEndocavitariaLongitudinal = consulta.EcografiaEndocavitariaLongitudinal ?? "-",
                EcografiaEndocavitariaTransverso = consulta.EcografiaEndocavitariaTransverso ?? "-",
                EcografiaEndocavitariaEndometrio = consulta.EcografiaEndocavitariaEndometrio ?? "-",
                EcografiaEndocavitariaOvarioDerecho = consulta.EcografiaEndocavitariaOvarioDerecho ?? "-",
                EcografiaEndocavitariaOvarioIzquierdo = consulta.EcografiaEndocavitariaOvarioIzquierdo ?? "-",
                EcografiaEndocavitariaFondoSaco = consulta.EcografiaEndocavitariaFondoSaco ?? "-",
                EcografiaEndocavitariaImpresionClinica = consulta.EcografiaEndocavitariaImpresionClinica ?? "-",
                EcografiaEndocavitariaComentario = consulta.EcografiaEndocavitariaComentario ?? "-",

                #endregion
            };

            return await RenderPdfAsync("Views/CrearPDF/GenerarGinecologiaConsultaPDF.cshtml", model);
        }

        public async Task<IActionResult> generarObstetricaPDF(int consultaId)
        {
            var consulta = _consultasRepository.GetConsulta(consultaId);
            var paciente = consulta?.Citas?.Paciente ?? new Paciente();
            string edadPaciente = CalcularEdad(paciente.FechaNacimiento);

            var (medico, firmaEmpleado, telefonoMedico) = ObtenerMedicoFirma(consulta.Citas.EmpleadoId);
            var obsteAntNoPatologicos = consulta.ConsultaAntNoPatologicosObstetricia ?? new ConsultaAntNoPatologicosObstetricia();

            var model = new ConsultasViewModel
            {
                #region DATOS ENCABEZADO PDF

                EstablecimientoDireccion = _configuration["EstablecimientoDireccion"],
                EstablecimientoCorreoElectronico = _configuration["EstablecimientoCorreoElectronico"],
                EstablecimientoTelefono = _configuration["EstablecimientoTelefono"],
                EstablecimientoImagenLogo = _configuration["ImagenLogoBase64"],

                // Asignar la firma del médico


                #endregion

                PacienteNombre = paciente.Nombre ?? "-",
                PacienteEdad = edadPaciente,

                MedicoNombre = medico.NombreYApellidos ?? "Médico no especificado",
                FirmaEmpleado = firmaEmpleado,
                MedicoCelular = telefonoMedico, // Agregando el teléfono al modelo

                #region OBSTETRICIA - ANTECEDENTES

                ObstetriciaAntNoPatologicosAbortos = obsteAntNoPatologicos.Abortos ?? "-",
                ObstetriciaAntNoPatologicosCesareas = obsteAntNoPatologicos.Cesareas ?? "-",
                ObstetriciaAntNoPatologicosFechaProbableParto = obsteAntNoPatologicos.FechaProbableParto != null
                            ? ((DateTime)obsteAntNoPatologicos.FechaProbableParto).ToString("yyyy-MM-dd")
                            : "-",
                ObstetriciaAntNoPatologicosFechaUltimaRegla = obsteAntNoPatologicos.FechaUltimaRegla != null
                            ? ((DateTime)obsteAntNoPatologicos.FechaUltimaRegla).ToString("yyyy-MM-dd")
                            : "-",
                ObstetriciaAntNoPatologicosGestas = obsteAntNoPatologicos.Gestas ?? "-",
                ObstetriciaAntNoPatologicosHijosMuertos = obsteAntNoPatologicos.HijosMuertos ?? "-",
                ObstetriciaAntNoPatologicosHijosVivos = obsteAntNoPatologicos.HijosVivos ?? "-",
                ObstetriciaAntNoPatologicosPartos = obsteAntNoPatologicos.Partos ?? "-",

                #endregion

                #region OBSTETRICIA - ECOGRAFIA OBSTETRICA

                EcografiaObstetricaFeto = consulta.EcografiaObstetricaFeto ?? "-",
                EcografiaObstetricaSituacion = consulta.EcografiaObstetricaSituacion ?? "-",
                EcografiaObstetricaEstado = consulta.EcografiaObstetricaEstado ?? "-",
                EcografiaObstetricaPresentacion = consulta.EcografiaObstetricaPresentacion ?? "-",
                EcografiaObstetricaPosicion = consulta.EcografiaObstetricaPosicion ?? "-",
                EcografiaObstetricaDorso = consulta.EcografiaObstetricaDorso ?? "-",

                #endregion

                #region OBSTETRICIA - BIOMETRIA

                NumeroBebes = consulta.NumeroBebes.HasValue ? consulta.NumeroBebes.Value : 0,

                // Primer conjunto de propiedades
                ObsteBiometriaRlc = consulta.ObsteBiometriaRlc ?? "-",
                ObsteBiometriaSg = consulta.ObsteBiometriaSg ?? "-",
                ObsteBiometriaW = consulta.ObsteBiometriaW ?? "-",
                ObsteBiometriaDbp = consulta.ObsteBiometriaDbp ?? "-",
                ObsteBiometriaHc = consulta.ObsteBiometriaHc ?? "-",
                ObsteBiometriaAc = consulta.ObsteBiometriaAc ?? "-",
                ObsteBiometriaLf = consulta.ObsteBiometriaLf ?? "-",
                ObsteBiometriaEg = consulta.ObsteBiometriaEg ?? "-",
                ObsteBiometriaFcf = consulta.ObsteBiometriaFcf ?? "-",
                ObsteBiometriaPlacenta = consulta.ObsteBiometriaPlacenta ?? "-",
                ObsteBiometriaGrado = consulta.ObsteBiometriaGrado ?? "-",
                ObsteBiometriaIla = consulta.ObsteBiometriaIla ?? "-",
                ObsteBiometriaMalformaciones = consulta.ObsteBiometriaMalformaciones ?? "-",
                ObsteBiometriaPeso = consulta.ObsteBiometriaPeso ?? "-",
                ObsteBiometriaSexo = consulta.ObsteBiometriaSexo ?? "-",
                ObsteBiometriaFechaParto = consulta.ObsteBiometriaFechaParto ?? "-",
                ObsteBiometriaComentario = consulta.ObsteBiometriaComentario ?? "-",

                // Segundo conjunto de propiedades (si aplica)
                ObsteBiometriaRlc2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaRlc2 ?? "-" : null,
                ObsteBiometriaSg2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaSg2 ?? "-" : null,
                ObsteBiometriaDbp2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaDbp2 ?? "-" : null,
                ObsteBiometriaHc2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaHc2 ?? "-" : null,
                ObsteBiometriaAc2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaAc2 ?? "-" : null,
                ObsteBiometriaLf2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaLf2 ?? "-" : null,
                ObsteBiometriaEg2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaEg2 ?? "-" : null,
                ObsteBiometriaFcf2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaFcf2 ?? "-" : null,
                ObsteBiometriaPlacenta2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaPlacenta2 ?? "-" : null,
                ObsteBiometriaGrado2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaGrado2 ?? "-" : null,
                ObsteBiometriaIla2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaIla2 ?? "-" : null,
                ObsteBiometriaMalformaciones2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaMalformaciones2 ?? "-" : null,
                ObsteBiometriaPeso2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaPeso2 ?? "-" : null,
                ObsteBiometriaSexo2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaSexo2 ?? "-" : null,
                ObsteBiometriaFechaParto2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaFechaParto2 ?? "-" : null,
                ObsteBiometriaComentario2 = consulta.NumeroBebes >= 2 ? consulta.ObsteBiometriaComentario2 ?? "-" : null,

                // Tercer conjunto de propiedades (si aplica)
                ObsteBiometriaRlc3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaRlc3 ?? "-" : null,
                ObsteBiometriaSg3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaSg3 ?? "-" : null,
                ObsteBiometriaDbp3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaDbp3 ?? "-" : null,
                ObsteBiometriaHc3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaHc3 ?? "-" : null,
                ObsteBiometriaAc3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaAc3 ?? "-" : null,
                ObsteBiometriaLf3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaLf3 ?? "-" : null,
                ObsteBiometriaEg3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaEg3 ?? "-" : null,
                ObsteBiometriaFcf3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaFcf3 ?? "-" : null,
                ObsteBiometriaPlacenta3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaPlacenta3 ?? "-" : null,
                ObsteBiometriaGrado3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaGrado3 ?? "-" : null,
                ObsteBiometriaIla3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaIla3 ?? "-" : null,
                ObsteBiometriaMalformaciones3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaMalformaciones3 ?? "-" : null,
                ObsteBiometriaPeso3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaPeso3 ?? "-" : null,
                ObsteBiometriaSexo3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaSexo3 ?? "-" : null,
                ObsteBiometriaFechaParto3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaFechaParto3 ?? "-" : null,
                ObsteBiometriaComentario3 = consulta.NumeroBebes >= 3 ? consulta.ObsteBiometriaComentario3 ?? "-" : null,

                // Cuarto conjunto de propiedades (si aplica)
                ObsteBiometriaRlc4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaRlc4 ?? "-" : null,
                ObsteBiometriaSg4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaSg4 ?? "-" : null,
                ObsteBiometriaDbp4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaDbp4 ?? "-" : null,
                ObsteBiometriaHc4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaHc4 ?? "-" : null,
                ObsteBiometriaAc4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaAc4 ?? "-" : null,
                ObsteBiometriaLf4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaLf4 ?? "-" : null,
                ObsteBiometriaEg4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaEg4 ?? "-" : null,
                ObsteBiometriaFcf4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaFcf4 ?? "-" : null,
                ObsteBiometriaPlacenta4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaPlacenta4 ?? "-" : null,
                ObsteBiometriaGrado4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaGrado4 ?? "-" : null,
                ObsteBiometriaIla4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaIla4 ?? "-" : null,
                ObsteBiometriaMalformaciones4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaMalformaciones4 ?? "-" : null,
                ObsteBiometriaPeso4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaPeso4 ?? "-" : null,
                ObsteBiometriaSexo4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaSexo4 ?? "-" : null,
                ObsteBiometriaFechaParto4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaFechaParto4 ?? "-" : null,
                ObsteBiometriaComentario4 = consulta.NumeroBebes == 4 ? consulta.ObsteBiometriaComentario4 ?? "-" : null,

                #endregion

                #region OBSTETRICIA - ECOGRAFIA ENDOCAVITARIA

                EcografiaEndocavitariaUtero = consulta.EcografiaEndocavitariaUtero ?? "-",
                EcografiaEndocavitariaLongitudinal = consulta.EcografiaEndocavitariaLongitudinal ?? "-",
                EcografiaEndocavitariaTransverso = consulta.EcografiaEndocavitariaTransverso ?? "-",
                EcografiaEndocavitariaEndometrio = consulta.EcografiaEndocavitariaEndometrio ?? "-",
                EcografiaEndocavitariaOvarioDerecho = consulta.EcografiaEndocavitariaOvarioDerecho ?? "-",
                EcografiaEndocavitariaOvarioIzquierdo = consulta.EcografiaEndocavitariaOvarioIzquierdo ?? "-",
                EcografiaEndocavitariaFondoSaco = consulta.EcografiaEndocavitariaFondoSaco ?? "-",
                EcografiaEndocavitariaImpresionClinica = consulta.EcografiaEndocavitariaImpresionClinica ?? "-",
                EcografiaEndocavitariaComentario = consulta.EcografiaEndocavitariaComentario ?? "-",

                #endregion

                #region OBSTETRICIA - EVALUACION OBSTETRICA

                EvaluacionObstetricaAbdomenObstetrico = consulta.EvaluacionObstetricaAbdomenObstetrico ?? "-",
                EvaluacionObstetricaActividadUterina = consulta.EvaluacionObstetricaActividadUterina ?? "-",
                EvaluacionObstetricaAltitud = consulta.EvaluacionObstetricaAltitud ?? "-",
                EvaluacionObstetricaAu = consulta.EvaluacionObstetricaAu ?? "-",
                EvaluacionObstetricaBPorciento = consulta.EvaluacionObstetricaBPorciento ?? "-",
                EvaluacionObstetricaCms = consulta.EvaluacionObstetricaCms ?? "-",
                EvaluacionObstetricaConsistencia = consulta.EvaluacionObstetricaConsistencia ?? "-",
                EvaluacionObstetricaD = consulta.EvaluacionObstetricaD ?? "-",
                EvaluacionObstetricaFcf = consulta.EvaluacionObstetricaFcf ?? "-",
                EvaluacionObstetricaLiquidoAmniotico = consulta.EvaluacionObstetricaLiquidoAmniotico ?? "-",
                EvaluacionObstetricaLiquidoAmnioticoEspecifique = consulta.EvaluacionObstetricaLiquidoAmnioticoEspecifique ?? "-",
                EvaluacionObstetricaMembranasOvulares = consulta.EvaluacionObstetricaMembranasOvulares ?? "-",
                EvaluacionObstetricaMovimientoFetalEspecifique = consulta.EvaluacionObstetricaMovimientoFetalEspecifique ?? "-",
                EvaluacionObstetricaMovimientoFetalPercetible = consulta.EvaluacionObstetricaMovimientoFetalPercetible ?? "-",
                EvaluacionObstetricaOtras = consulta.EvaluacionObstetricaOtras ?? "-",
                EvaluacionObstetricaPelvis = consulta.EvaluacionObstetricaPelvis ?? "-",
                EvaluacionObstetricaPosicionCervix = consulta.EvaluacionObstetricaPosicionCervix ?? "-",
                EvaluacionObstetricaPresentacionLeopold = consulta.EvaluacionObstetricaPresentacionLeopold ?? "-",
                EvaluacionObstetricaTactoVaginal = consulta.EvaluacionObstetricaTactoVaginal ?? "-",
                EvaluacionObstetricaUteroGravio = consulta.EvaluacionObstetricaUteroGravio ?? "-",

                #endregion
            };

            return await RenderPdfAsync("Views/CrearPDF/generarObstetricaPDF.cshtml", model);
        }

        public async Task<IActionResult> generarObstetricaBiometriaPDF(int consultaId)
        {
            // Obtener la consulta y el paciente
            var consulta = _consultasRepository.GetConsulta(consultaId);
            var paciente = consulta?.Citas?.Paciente ?? new Paciente();
            string edadPaciente = CalcularEdad(paciente.FechaNacimiento);

            var (medico, firmaEmpleado, telefonoMedico) = ObtenerMedicoFirma(consulta.Citas.EmpleadoId);

            // Crear el modelo para la vista del PDF
            var model = new ConsultasViewModel
            {
                // Asignar otros datos relevantes para el PDF
                EstablecimientoDireccion = _configuration["EstablecimientoDireccion"],
                EstablecimientoCorreoElectronico = _configuration["EstablecimientoCorreoElectronico"],
                EstablecimientoTelefono = _configuration["EstablecimientoTelefono"],
                EstablecimientoImagenLogo = _configuration["ImagenLogoBase64"],

                // Asignar otros detalles del paciente y la consulta
                PacienteNombre = paciente.Nombre ?? "-",
                PacienteEdad = edadPaciente,
                MedicoNombre = medico.NombreYApellidos ?? "Médico no especificado",
                FirmaEmpleado = firmaEmpleado,
                MedicoCelular = telefonoMedico, // Agregando el teléfono al modelo

                NumeroBebes = consulta.NumeroBebes.HasValue ? consulta.NumeroBebes.Value : 0,

                EcografiaObstetricaFeto = consulta.EcografiaObstetricaFeto ?? "-",
                EcografiaObstetricaSituacion = consulta.EcografiaObstetricaSituacion ?? "-",
                EcografiaObstetricaEstado = consulta.EcografiaObstetricaEstado ?? "-",
                EcografiaObstetricaPresentacion = consulta.EcografiaObstetricaPresentacion ?? "-",
                EcografiaObstetricaPosicion = consulta.EcografiaObstetricaPosicion ?? "-",
                EcografiaObstetricaDorso = consulta.EcografiaObstetricaDorso ?? "-"

            };

            // Asignar solo las propiedades correspondientes a la ronda 1 (sin sufijo)
            model.ObsteBiometriaRlc = consulta.ObsteBiometriaRlc ?? "-";
            model.ObsteBiometriaSg = consulta.ObsteBiometriaSg ?? "-";
            model.ObsteBiometriaDbp = consulta.ObsteBiometriaDbp ?? "-";
            model.ObsteBiometriaHc = consulta.ObsteBiometriaHc ?? "-";
            model.ObsteBiometriaAc = consulta.ObsteBiometriaAc ?? "-";
            model.ObsteBiometriaLf = consulta.ObsteBiometriaLf ?? "-";
            model.ObsteBiometriaEg = consulta.ObsteBiometriaEg ?? "-";
            model.ObsteBiometriaFcf = consulta.ObsteBiometriaFcf ?? "-";
            model.ObsteBiometriaPlacenta = consulta.ObsteBiometriaPlacenta ?? "-";
            model.ObsteBiometriaGrado = consulta.ObsteBiometriaGrado ?? "-";
            model.ObsteBiometriaIla = consulta.ObsteBiometriaIla ?? "-";
            model.ObsteBiometriaMalformaciones = consulta.ObsteBiometriaMalformaciones ?? "-";
            model.ObsteBiometriaPeso = consulta.ObsteBiometriaPeso ?? "-";
            model.ObsteBiometriaSexo = consulta.ObsteBiometriaSexo ?? "-";
            model.ObsteBiometriaFechaParto = consulta.ObsteBiometriaFechaParto ?? "-";
            model.ObsteBiometriaComentario = consulta.ObsteBiometriaComentario ?? "-";


            return await RenderPdfAsync("Views/CrearPDF/generarObstetricaBiometriaPDF.cshtml", model);
        }

        public async Task<IActionResult> generarObstetricaBiometriaPDF2(int consultaId)
        {
            // Obtener la consulta y el paciente
            var consulta = _consultasRepository.GetConsulta(consultaId);
            var paciente = consulta?.Citas?.Paciente ?? new Paciente();
            string edadPaciente = CalcularEdad(paciente.FechaNacimiento);

            var (medico, firmaEmpleado, telefonoMedico) = ObtenerMedicoFirma(consulta.Citas.EmpleadoId);

            // Crear el modelo para la vista del PDF
            var model = new ConsultasViewModel
            {
                // Asignar otros datos relevantes para el PDF
                EstablecimientoDireccion = _configuration["EstablecimientoDireccion"],
                EstablecimientoCorreoElectronico = _configuration["EstablecimientoCorreoElectronico"],
                EstablecimientoTelefono = _configuration["EstablecimientoTelefono"],
                EstablecimientoImagenLogo = _configuration["ImagenLogoBase64"],

                // Asignar otros detalles del paciente y la consulta
                PacienteNombre = paciente.Nombre ?? "-",
                PacienteEdad = edadPaciente,
                MedicoNombre = medico.NombreYApellidos ?? "Médico no especificado",
                FirmaEmpleado = firmaEmpleado,
                MedicoCelular = telefonoMedico, // Agregando el teléfono al modelo

                NumeroBebes = consulta.NumeroBebes.HasValue ? consulta.NumeroBebes.Value : 0,
                EcografiaObstetricaFeto = consulta.EcografiaObstetricaFeto ?? "-",
                EcografiaObstetricaSituacion = consulta.EcografiaObstetricaSituacion ?? "-",
                EcografiaObstetricaEstado = consulta.EcografiaObstetricaEstado ?? "-",
                EcografiaObstetricaPresentacion = consulta.EcografiaObstetricaPresentacion ?? "-",
                EcografiaObstetricaPosicion = consulta.EcografiaObstetricaPosicion ?? "-",
                EcografiaObstetricaDorso = consulta.EcografiaObstetricaDorso ?? "-"
            };

            // Asignar solo las propiedades correspondientes a la ronda 2 (con sufijo "2")
            model.ObsteBiometriaRlc2 = consulta.ObsteBiometriaRlc2 ?? "-";
            model.ObsteBiometriaSg2 = consulta.ObsteBiometriaSg2 ?? "-";
            model.ObsteBiometriaDbp2 = consulta.ObsteBiometriaDbp2 ?? "-";
            model.ObsteBiometriaHc2 = consulta.ObsteBiometriaHc2 ?? "-";
            model.ObsteBiometriaAc2 = consulta.ObsteBiometriaAc2 ?? "-";
            model.ObsteBiometriaLf2 = consulta.ObsteBiometriaLf2 ?? "-";
            model.ObsteBiometriaEg2 = consulta.ObsteBiometriaEg2 ?? "-";
            model.ObsteBiometriaFcf2 = consulta.ObsteBiometriaFcf2 ?? "-";
            model.ObsteBiometriaPlacenta2 = consulta.ObsteBiometriaPlacenta2 ?? "-";
            model.ObsteBiometriaGrado2 = consulta.ObsteBiometriaGrado2 ?? "-";
            model.ObsteBiometriaIla2 = consulta.ObsteBiometriaIla2 ?? "-";
            model.ObsteBiometriaMalformaciones2 = consulta.ObsteBiometriaMalformaciones2 ?? "-";
            model.ObsteBiometriaPeso2 = consulta.ObsteBiometriaPeso2 ?? "-";
            model.ObsteBiometriaSexo2 = consulta.ObsteBiometriaSexo2 ?? "-";
            model.ObsteBiometriaFechaParto2 = consulta.ObsteBiometriaFechaParto2 ?? "-";
            model.ObsteBiometriaComentario2 = consulta.ObsteBiometriaComentario2 ?? "-";

            return await RenderPdfAsync("Views/CrearPDF/generarObstetricaBiometriaPDF2.cshtml", model);
        }


        public async Task<IActionResult> generarObstetricaBiometriaPDF3(int consultaId)
        {
            // Obtener la consulta y el paciente
            var consulta = _consultasRepository.GetConsulta(consultaId);
            var paciente = consulta?.Citas?.Paciente ?? new Paciente();
            string edadPaciente = CalcularEdad(paciente.FechaNacimiento);

            var (medico, firmaEmpleado, telefonoMedico) = ObtenerMedicoFirma(consulta.Citas.EmpleadoId);

            // Crear el modelo para la vista del PDF
            var model = new ConsultasViewModel
            {
                // Asignar otros datos relevantes para el PDF
                EstablecimientoDireccion = _configuration["EstablecimientoDireccion"],
                EstablecimientoCorreoElectronico = _configuration["EstablecimientoCorreoElectronico"],
                EstablecimientoTelefono = _configuration["EstablecimientoTelefono"],
                EstablecimientoImagenLogo = _configuration["ImagenLogoBase64"],

                // Asignar otros detalles del paciente y la consulta
                PacienteNombre = paciente.Nombre ?? "-",
                PacienteEdad = edadPaciente,
                MedicoNombre = medico.NombreYApellidos ?? "Médico no especificado",
                FirmaEmpleado = firmaEmpleado,
                MedicoCelular = telefonoMedico, // Agregando el teléfono al modelo

                NumeroBebes = consulta.NumeroBebes.HasValue ? consulta.NumeroBebes.Value : 0,
                EcografiaObstetricaFeto = consulta.EcografiaObstetricaFeto ?? "-",
                EcografiaObstetricaSituacion = consulta.EcografiaObstetricaSituacion ?? "-",
                EcografiaObstetricaEstado = consulta.EcografiaObstetricaEstado ?? "-",
                EcografiaObstetricaPresentacion = consulta.EcografiaObstetricaPresentacion ?? "-",
                EcografiaObstetricaPosicion = consulta.EcografiaObstetricaPosicion ?? "-",
                EcografiaObstetricaDorso = consulta.EcografiaObstetricaDorso ?? "-"
            };

            // Asignar solo las propiedades correspondientes a la ronda 3 (con sufijo "3")
            model.ObsteBiometriaRlc3 = consulta.ObsteBiometriaRlc3 ?? "-";
            model.ObsteBiometriaSg3 = consulta.ObsteBiometriaSg3 ?? "-";
            model.ObsteBiometriaDbp3 = consulta.ObsteBiometriaDbp3 ?? "-";
            model.ObsteBiometriaHc3 = consulta.ObsteBiometriaHc3 ?? "-";
            model.ObsteBiometriaAc3 = consulta.ObsteBiometriaAc3 ?? "-";
            model.ObsteBiometriaLf3 = consulta.ObsteBiometriaLf3 ?? "-";
            model.ObsteBiometriaEg3 = consulta.ObsteBiometriaEg3 ?? "-";
            model.ObsteBiometriaFcf3 = consulta.ObsteBiometriaFcf3 ?? "-";
            model.ObsteBiometriaPlacenta3 = consulta.ObsteBiometriaPlacenta3 ?? "-";
            model.ObsteBiometriaGrado3 = consulta.ObsteBiometriaGrado3 ?? "-";
            model.ObsteBiometriaIla3 = consulta.ObsteBiometriaIla3 ?? "-";
            model.ObsteBiometriaMalformaciones3 = consulta.ObsteBiometriaMalformaciones3 ?? "-";
            model.ObsteBiometriaPeso3 = consulta.ObsteBiometriaPeso3 ?? "-";
            model.ObsteBiometriaSexo3 = consulta.ObsteBiometriaSexo3 ?? "-";
            model.ObsteBiometriaFechaParto3 = consulta.ObsteBiometriaFechaParto3 ?? "-";
            model.ObsteBiometriaComentario3 = consulta.ObsteBiometriaComentario3 ?? "-";

            return await RenderPdfAsync("Views/CrearPDF/generarObstetricaBiometriaPDF3.cshtml", model);
        }

        public async Task<IActionResult> generarObstetricaBiometriaPDF4(int consultaId)
        {
            // Obtener la consulta y el paciente
            var consulta = _consultasRepository.GetConsulta(consultaId);
            var paciente = consulta?.Citas?.Paciente ?? new Paciente();
            string edadPaciente = CalcularEdad(paciente.FechaNacimiento);

            var (medico, firmaEmpleado, telefonoMedico) = ObtenerMedicoFirma(consulta.Citas.EmpleadoId);

            // Crear el modelo para la vista del PDF
            var model = new ConsultasViewModel
            {
                // Asignar otros datos relevantes para el PDF
                EstablecimientoDireccion = _configuration["EstablecimientoDireccion"],
                EstablecimientoCorreoElectronico = _configuration["EstablecimientoCorreoElectronico"],
                EstablecimientoTelefono = _configuration["EstablecimientoTelefono"],
                EstablecimientoImagenLogo = _configuration["ImagenLogoBase64"],

                // Asignar otros detalles del paciente y la consulta
                PacienteNombre = paciente.Nombre ?? "-",
                PacienteEdad = edadPaciente,
                MedicoNombre = medico.NombreYApellidos ?? "Médico no especificado",
                FirmaEmpleado = firmaEmpleado,
                MedicoCelular = telefonoMedico, // Agregando el teléfono al modelo

                NumeroBebes = consulta.NumeroBebes.HasValue ? consulta.NumeroBebes.Value : 0,
                EcografiaObstetricaFeto = consulta.EcografiaObstetricaFeto ?? "-",
                EcografiaObstetricaSituacion = consulta.EcografiaObstetricaSituacion ?? "-",
                EcografiaObstetricaEstado = consulta.EcografiaObstetricaEstado ?? "-",
                EcografiaObstetricaPresentacion = consulta.EcografiaObstetricaPresentacion ?? "-",
                EcografiaObstetricaPosicion = consulta.EcografiaObstetricaPosicion ?? "-",
                EcografiaObstetricaDorso = consulta.EcografiaObstetricaDorso ?? "-"
            };

            // Asignar solo las propiedades correspondientes a la ronda 4 (con sufijo "4")
            model.ObsteBiometriaRlc4 = consulta.ObsteBiometriaRlc4 ?? "-";
            model.ObsteBiometriaSg4 = consulta.ObsteBiometriaSg4 ?? "-";
            model.ObsteBiometriaDbp4 = consulta.ObsteBiometriaDbp4 ?? "-";
            model.ObsteBiometriaHc4 = consulta.ObsteBiometriaHc4 ?? "-";
            model.ObsteBiometriaAc4 = consulta.ObsteBiometriaAc4 ?? "-";
            model.ObsteBiometriaLf4 = consulta.ObsteBiometriaLf4 ?? "-";
            model.ObsteBiometriaEg4 = consulta.ObsteBiometriaEg4 ?? "-";
            model.ObsteBiometriaFcf4 = consulta.ObsteBiometriaFcf4 ?? "-";
            model.ObsteBiometriaPlacenta4 = consulta.ObsteBiometriaPlacenta4 ?? "-";
            model.ObsteBiometriaGrado4 = consulta.ObsteBiometriaGrado4 ?? "-";
            model.ObsteBiometriaIla4 = consulta.ObsteBiometriaIla4 ?? "-";
            model.ObsteBiometriaMalformaciones4 = consulta.ObsteBiometriaMalformaciones4 ?? "-";
            model.ObsteBiometriaPeso4 = consulta.ObsteBiometriaPeso4 ?? "-";
            model.ObsteBiometriaSexo4 = consulta.ObsteBiometriaSexo4 ?? "-";
            model.ObsteBiometriaFechaParto4 = consulta.ObsteBiometriaFechaParto4 ?? "-";
            model.ObsteBiometriaComentario4 = consulta.ObsteBiometriaComentario4 ?? "-";

            return await RenderPdfAsync("Views/CrearPDF/generarObstetricaBiometriaPDF4.cshtml", model);
        }




        public async Task<IActionResult> generarEndocavitarioPdf(int consultaId)
        {
            var consulta = _consultasRepository.GetConsulta(consultaId);
            var paciente = consulta?.Citas?.Paciente ?? new Paciente();
            string edadPaciente = CalcularEdad(paciente.FechaNacimiento);

            var (medico, firmaEmpleado, telefonoMedico) = ObtenerMedicoFirma(consulta.Citas.EmpleadoId);

            var model = new ConsultasViewModel
            {
                #region DATOS ENCABEZADO PDF

                EstablecimientoDireccion = _configuration["EstablecimientoDireccion"],
                EstablecimientoCorreoElectronico = _configuration["EstablecimientoCorreoElectronico"],
                EstablecimientoTelefono = _configuration["EstablecimientoTelefono"],
                EstablecimientoImagenLogo = _configuration["ImagenLogoBase64"],


                MedicoNombre = medico.NombreYApellidos ?? "Médico no especificado",
                FirmaEmpleado = firmaEmpleado,
                MedicoCelular = telefonoMedico, // Agregando el teléfono al modelo
                PacienteNombre = paciente.Nombre ?? "-",
                PacienteEdad = edadPaciente,

                #endregion

                #region OBSTETRICIA - ECOGRAFIA ENDOCAVITARIA

                EcografiaEndocavitariaUtero = consulta.EcografiaEndocavitariaUtero ?? "-",
                EcografiaEndocavitariaLongitudinal = consulta.EcografiaEndocavitariaLongitudinal ?? "-",
                EcografiaEndocavitariaTransverso = consulta.EcografiaEndocavitariaTransverso ?? "-",
                EcografiaEndocavitariaEndometrio = consulta.EcografiaEndocavitariaEndometrio ?? "-",
                EcografiaEndocavitariaOvarioDerecho = consulta.EcografiaEndocavitariaOvarioDerecho ?? "-",
                EcografiaEndocavitariaOvarioIzquierdo = consulta.EcografiaEndocavitariaOvarioIzquierdo ?? "-",
                EcografiaEndocavitariaFondoSaco = consulta.EcografiaEndocavitariaFondoSaco ?? "-",
                EcografiaEndocavitariaImpresionClinica = consulta.EcografiaEndocavitariaImpresionClinica ?? "-",
                EcografiaEndocavitariaComentario = consulta.EcografiaEndocavitariaComentario ?? "-",

                #endregion

                #region OBSTETRICIA - EVALUACION OBSTETRICA

                EvaluacionObstetricaAbdomenObstetrico = consulta.EvaluacionObstetricaAbdomenObstetrico ?? "-",
                EvaluacionObstetricaActividadUterina = consulta.EvaluacionObstetricaActividadUterina ?? "-",
                EvaluacionObstetricaAltitud = consulta.EvaluacionObstetricaAltitud ?? "-",
                EvaluacionObstetricaAu = consulta.EvaluacionObstetricaAu ?? "-",
                EvaluacionObstetricaBPorciento = consulta.EvaluacionObstetricaBPorciento ?? "-",
                EvaluacionObstetricaCms = consulta.EvaluacionObstetricaCms ?? "-",
                EvaluacionObstetricaConsistencia = consulta.EvaluacionObstetricaConsistencia ?? "-",
                EvaluacionObstetricaD = consulta.EvaluacionObstetricaD ?? "-",
                EvaluacionObstetricaFcf = consulta.EvaluacionObstetricaFcf ?? "-",
                EvaluacionObstetricaLiquidoAmniotico = consulta.EvaluacionObstetricaLiquidoAmniotico ?? "-",
                EvaluacionObstetricaLiquidoAmnioticoEspecifique = consulta.EvaluacionObstetricaLiquidoAmnioticoEspecifique ?? "-",
                EvaluacionObstetricaMembranasOvulares = consulta.EvaluacionObstetricaMembranasOvulares ?? "-",
                EvaluacionObstetricaMovimientoFetalEspecifique = consulta.EvaluacionObstetricaMovimientoFetalEspecifique ?? "-",
                EvaluacionObstetricaMovimientoFetalPercetible = consulta.EvaluacionObstetricaMovimientoFetalPercetible ?? "-",
                EvaluacionObstetricaOtras = consulta.EvaluacionObstetricaOtras ?? "-",
                EvaluacionObstetricaPelvis = consulta.EvaluacionObstetricaPelvis ?? "-",
                EvaluacionObstetricaPosicionCervix = consulta.EvaluacionObstetricaPosicionCervix ?? "-",
                EvaluacionObstetricaPresentacionLeopold = consulta.EvaluacionObstetricaPresentacionLeopold ?? "-",
                EvaluacionObstetricaTactoVaginal = consulta.EvaluacionObstetricaTactoVaginal ?? "-",
                EvaluacionObstetricaUteroGravio = consulta.EvaluacionObstetricaUteroGravio ?? "-",

                #endregion
            };

            return await RenderPdfAsync("Views/CrearPDF/generarEndocavitarioPDF.cshtml", model);
        }




        public IActionResult Resumen(int id)
        {
            var examen = _laboratorioClinico.GetExamenRealizado(id);

            var model = new RealizarExamenLabClinicoViewModel()
            {
                Examen = examen
            };

            return new ViewAsPdf("Resumen", model);
        }
        public IActionResult CotizacionesListaPdf()
        {

            var cotizaciones = _cotizacionRepository.GetList();



            return new ViewAsPdf("CotizacionesListaPdf", cotizaciones);
        }

        public async Task<IActionResult> HospitalizacionInformeGeneralPDF(int hospitalizacionId)
        {
            if (hospitalizacionId <= 0)
            {
                return BadRequest("Debe indicar hospitalizacionId en la URL.");
            }

            var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId);
            if (hospitalizacion == null)
            {
                TempData["Message"] = "La hospitalizacion no existe";
                return RedirectToAction("Index", "Home");
            }

            var model = new HospitalizacionInformeGeneralPDFVM
            {
                Paciente = hospitalizacion.Paciente ?? new Paciente()
            };

            model.EstablecimientoCorreoElectronico = _configuration["EstablecimientoCorreoElectronico"];
            model.EstablecimientoTelefono = _configuration["EstablecimientoTelefono"];
            model.EstablecimientoImagenLogo = _configuration["ImagenLogoBase64"];
            model.EstablecimientoImagenFirma = _configuration["EstablecimientoImagenFirma"];
            model.EstablecimientoDireccion = _configuration["EstablecimientoDireccion"];
            return await RenderPdfAsync(
                "Views/CrearPDF/HospitalizacionInformeGeneralPDF.cshtml",
                model);
        }
        public async Task<IActionResult> HospitalizacionInformeDetalladoPDF(int hospitalizacionId)
        {
            if (hospitalizacionId <= 0)
            {
                return BadRequest("Debe indicar hospitalizacionId en la URL.");
            }

            var hospitalizacion = _hospitalizacionRepository.Get(
                hospitalizacionId,
                includeMedicamentos: true,
                includeServicios: true,
                includeExamenes: true,
                includePaquetes: true);

            if (hospitalizacion == null)
            {
                TempData["Message"] = "La hospitalizacion no existe";
                return RedirectToAction("Index", "Home");
            }

            return await RenderPdfAsync(
                "Views/CrearPDF/HospitalizacionInformeDetalladoPDF.cshtml",
                hospitalizacion);
        }

        public async Task<IActionResult> HospitalizacionInformeHospitalizacionPDF(int hospitalizacionId)
        {
            if (hospitalizacionId <= 0)
            {
                return BadRequest("Debe indicar hospitalizacionId en la URL.");
            }

            var hospitalizacion = _hospitalizacionRepository.Get(
                hospitalizacionId,
                includeMedicamentos: true,
                includeServicios: true,
                includeExamenes: true,
                includePaquetes: true,
                includeConsultas: true,
                includeOrdenesMedicas: true
            );

            if (hospitalizacion == null)
            {
                TempData["Message"] = "La hospitalizacion no existe";
                return RedirectToAction("Index", "Home");
            }

            var archivos = new List<PacienteArchivoVM>();
            if (!string.IsNullOrWhiteSpace(hospitalizacion.UrlArchivoConsentimiento))
            {
                archivos.Add(new PacienteArchivoVM
                {
                    ArchivoId = hospitalizacion.Id,
                    ArchivoFecha = hospitalizacion.FechaInicio.ToString("yyyy-MM-dd"),
                    ArchivoNombre = "Archivo de Consentimiento",
                    ArchivoUrl = hospitalizacion.UrlArchivoConsentimiento
                });
            }

            var pacienteArchivos = await _context.PacienteArchivos
                .Where(a => a.PacienteId == hospitalizacion.PacienteId)
                .ToListAsync();

            foreach (var archivo in pacienteArchivos)
            {
                archivos.Add(new PacienteArchivoVM
                {
                    ArchivoId = archivo.Id,
                    ArchivoFecha = "-",
                    ArchivoNombre = archivo.NombreArchivo ?? "-",
                    ArchivoUrl = archivo.UrlArchivo
                });
            }

            ViewBag.ArchivosPaciente = archivos;
            ViewBag.InformeSecciones = await ConstruirInformeHospitalizacionSeccionesAsync(hospitalizacion);
            return await RenderPdfAsync(
                "Views/CrearPDF/HospitalizacionInformeHospitalizacionPDF.cshtml",
                hospitalizacion);
        }

        public async Task<IActionResult> ExpedientePacientePDF(int hospitalizacionId)
        {
            var hospitalizacion = _hospitalizacionRepository.Get(
                hospitalizacionId,
                includeMedicamentos: true,
                includeServicios: true,
                includeExamenes: true,
                includePaquetes: true,
                includeConsultas: true,
                includeOrdenesMedicas: true
            );

            if (hospitalizacion == null)
                return NotFound("No se encontró la hospitalización solicitada.");

            var contentRoot = ContentRoot;
            var medico = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                hospitalizacion, _empleadoRepository, contentRoot);

            ViewBag.MedicoTratanteNombre = medico.Nombre;
            ViewBag.MedicoTratanteEspecialidad = medico.Especialidad;
            ViewBag.MedicoTratanteColegiado = medico.Colegiado;
            ViewBag.CodigoSeguro = hospitalizacion.Consultas?.FirstOrDefault()?.Citas?.CodigoDeCita ?? "-";
            ViewBag.MotivoIngreso = hospitalizacion.Consultas?.FirstOrDefault()?.Citas?.Motivo ?? "-";

            return await RenderPdfAsync
                ("Views/CrearPDF/ExpedientePacientePDF.cshtml", hospitalizacion);
        }

        public async Task<IActionResult> ExpedienteCompletoPDF(int hospitalizacionId, int? citaId = null)
        {
            var webRoot = Path.Combine(ContentRoot, "wwwroot");
            DevelopmentDemoSeedGuard.EnsureExpedientePdfAssets(_webHostEnvironment, webRoot);
            DevelopmentDemoSeedGuard.EnsureExpedienteHospitalizacion(_webHostEnvironment, _context, hospitalizacionId);

            var hospitalizacion = _hospitalizacionRepository.Get(
                hospitalizacionId,
                includeMedicamentos: true,
                includeServicios: true,
                includeExamenes: true,
                includePaquetes: true,
                includeConsultas: true,
                includeOrdenesMedicas: true
            );

            if (hospitalizacion == null)
                return NotFound("No se encontró la hospitalización solicitada.");

            if (hospitalizacion.Consultas == null || !hospitalizacion.Consultas.Any())
            {
                Consulta consultaHosp = _consultasRepository.GetConsultaPorHospitalizacion(hospitalizacionId);

                if (consultaHosp == null && citaId.HasValue && citaId.Value > 0)
                    consultaHosp = _consultasRepository.GetConsultaPorCita(citaId.Value);

                if (consultaHosp == null)
                {
                    var citaRecienteId = await _context.Citass
                        .Where(c => c.PacienteId == hospitalizacion.PacienteId && !c.Eliminado)
                        .OrderByDescending(c => c.Id)
                        .Select(c => c.Id)
                        .FirstOrDefaultAsync();

                    if (citaRecienteId > 0)
                        consultaHosp = _consultasRepository.GetConsultaPorCita(citaRecienteId);
                }

                if (consultaHosp == null)
                    consultaHosp = _consultasRepository.GetUltimaConsultaPaciente(hospitalizacion.PacienteId);

                if (consultaHosp != null)
                    hospitalizacion.Consultas = new List<Consulta> { consultaHosp };
            }

            var contentRoot = ContentRoot;
            var cirujanoFallback = _hospitalizacionRepository.GetNotasOperatoriasByHospitalizacion(hospitalizacionId)
                ?.OrderByDescending(n => n.FechaRegistro)
                .Select(n => n.Cirujano)
                .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));

            var consentimiento = ResolverConsentimientoHospitalizacion(hospitalizacion)
                ?? new ConsentimientoHospiVM
                {
                    NombrePaciente = hospitalizacion.Paciente?.Nombre ?? "-",
                    NombreCompleto = hospitalizacion.Paciente?.Nombre ?? "-"
                };

            var medico = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                hospitalizacion,
                _empleadoRepository,
                contentRoot,
                _citasRepository,
                citaId,
                cirujanoFallback,
                consentimiento?.NombreMedicoTratante);

            var pacienteExp = hospitalizacion.Paciente;
            var edadExp = pacienteExp?.FechaNacimiento != null
                ? ((int)((DateTime.Now - pacienteExp.FechaNacimiento.Value).TotalDays / 365.25)).ToString()
                : "-";

            var notasEntities = _hospitalizacionRepository.GetNotasMedicasByHospitalizacion(hospitalizacionId)
                ?.OrderBy(n => n.FechaRegistro)
                .ToList() ?? new List<NotaMedica2>();

            var notasEnfermeriaEntities = await _context.NotaEnfermeria2
                .Include(n => n.User).ThenInclude(u => u.Persona)
                .Where(n => n.HospitalizacionId == hospitalizacionId)
                .OrderBy(n => n.FechaRegistro)
                .ToListAsync();

            var userIdsAutorizacion = notasEntities
                .Where(n => n.Autorizado && !string.IsNullOrWhiteSpace(n.UsuarioAutoriza))
                .Select(n => n.UsuarioAutoriza)
                .Concat(hospitalizacion.OrdenesMedicas?
                    .Where(o => o.Autorizado && !string.IsNullOrWhiteSpace(o.UsuarioAutoriza))
                    .Select(o => o.UsuarioAutoriza) ?? Enumerable.Empty<string>())
                .Concat(notasEnfermeriaEntities
                    .Where(n => !string.IsNullOrWhiteSpace(n.UsuarioFirmaId))
                    .Select(n => n.UsuarioFirmaId))
                .Distinct()
                .ToList();

            var usuariosAutorizadores = await ObtenerUsuariosAutorizadoresAsync(userIdsAutorizacion);

            var notasEvolucion = notasEntities.Select(n =>
                {
                    User autorizador = null;
                    if (n.Autorizado && !string.IsNullOrWhiteSpace(n.UsuarioAutoriza))
                        usuariosAutorizadores.TryGetValue(n.UsuarioAutoriza, out autorizador);

                    var notaVm = new NotaMedica2ViewModel
                    {
                        Id = n.Id,
                        Diagnostico = n.Diagnostico ?? "",
                        HistoriaProblema = n.HistoriaProblema ?? "",
                        Sintomas = n.Sintomas ?? "",
                        FechaRegistro = n.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                        PacienteNombre = hospitalizacion.Paciente?.Nombre ?? "-",
                        PacienteEdad = edadExp,
                        PacienteSexoText = hospitalizacion.Paciente?.Sexo?.DescripcionSexo ?? hospitalizacion.Paciente?.sexoText ?? "-",
                        Autorizado = n.Autorizado,
                        TipoNota = n.TipoNota
                    };

                    PdfReportHelper.CompletarProfesionalNotaMedicaPdf(
                        notaVm,
                        n.Profesional,
                        medico,
                        n.Autorizado,
                        n.UsuarioAutoriza,
                        autorizador == null ? null : new Dictionary<string, User> { { autorizador.Id, autorizador } },
                        _empleadoRepository,
                        contentRoot);

                    return notaVm;
                }).ToList();

            var notasEnfermeria = notasEnfermeriaEntities.Select(n =>
            {
                User firmanteUser = null;
                if (!string.IsNullOrWhiteSpace(n.UsuarioFirmaId))
                    usuariosAutorizadores.TryGetValue(n.UsuarioFirmaId, out firmanteUser);

                var firmadoPor = n.Firmado
                    ? PdfReportHelper.ObtenerNombreEmpleadoPorUser(firmanteUser ?? n.User, _empleadoRepository, _userRepository)
                    : null;

                return new NotaEnfermeriaPdfViewModel
                {
                    Id = n.Id,
                    Diagnostico = n.Diagnostico ?? "",
                    FechaRegistro = n.FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss"),
                    Profesional = n.User?.Persona?.NombreYApellidos ?? "Enfermero(a) no especificado",
                    Firmado = n.Firmado,
                    FechaFirma = n.FechaFirma.HasValue ? n.FechaFirma.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                    FirmadoPor = firmadoPor,
                    PacienteNombre = hospitalizacion.Paciente?.Nombre ?? "-",
                    HospitalizacionId = hospitalizacionId,
                    EstablecimientoDireccion = _configuration["EstablecimientoDireccion"],
                    EstablecimientoTelefono = _configuration["EstablecimientoTelefono"],
                    EstablecimientoCorreo = _configuration["EstablecimientoCorreoElectronico"],
                    ImagenLogoBase64 = _configuration["ImagenLogoBase64"],
                    FirmaBase64 = n.Firmado && !string.IsNullOrEmpty(n.FirmaRuta)
                        ? PdfReportHelper.ObtenerFirmaBase64Local(n.FirmaRuta, contentRoot)
                        : null,
                    TipoNota = n.TipoNota
                };
            }).ToList() ?? new List<NotaEnfermeriaPdfViewModel>();

            var ordenes = hospitalizacion.OrdenesMedicas?
                .OrderBy(o => o.FechaHora)
                .Select(o =>
                {
                    var profesionalUser = PdfReportHelper.ResolverUsuarioProfesionalTexto(o.Profesional, _userRepository);
                    var registradoPor = PdfReportHelper.ResolverNombreProfesionalTexto(
                        o.Profesional,
                        _userRepository,
                        _empleadoRepository);
                    var firmante = PdfReportHelper.ResolverFirmanteClinico(
                        o.Autorizado,
                        o.UsuarioAutoriza,
                        profesionalUser,
                        usuariosAutorizadores,
                        _empleadoRepository,
                        medico.FirmaBase64,
                        contentRoot,
                        registradoPor);
                    return new OrdenMedicaViewModel
                    {
                        FechaHora = o.FechaHora.ToString("dd/MM/yyyy HH:mm"),
                        Profesional = registradoPor,
                        Descripcion = PdfReportHelper.NormalizarHtmlParaPdf(o.Descripcion ?? ""),
                        Realizada = o.Realizada ? "Sí" : "No",
                        Autorizado = o.Autorizado,
                        EmpleadoText = medico.Nombre,
                        ColegioEmpleado = medico.Colegiado,
                        FirmaBase64 = firmante.FirmaBase64,
                        NombreFirmante = firmante.NombreFirmante,
                        AutorizadoPor = firmante.AutorizadoPor
                    };
                }).ToList() ?? new List<OrdenMedicaViewModel>();

            var notasOperatoriasRaw = _hospitalizacionRepository.GetNotasOperatoriasByHospitalizacion(hospitalizacionId)
                ?.OrderBy(n => n.FechaRegistro)
                .ToList();
            var anestesistaNotaOp = notasOperatoriasRaw?
                .OrderByDescending(n => n.FechaRegistro)
                .FirstOrDefault()
                ?.Anestesista;

            var notasOperatorias = notasOperatoriasRaw
                ?.Select(n => MapNotaOperatoriaToPdfVm(n, contentRoot, medico))
                .Where(n => n != null)
                .ToList() ?? new List<NotaOperatoriaVM>();

            var documentos = await _context.PacienteArchivos
                .Where(a => a.PacienteId == hospitalizacion.PacienteId)
                .Select(a => new PacienteArchivoVM
                {
                    ArchivoId = a.Id,
                    ArchivoNombre = a.NombreArchivo ?? "-",
                    ArchivoUrl = a.UrlArchivo
                }).ToListAsync();

            var documentosHospitalizacion = await _context.DocumentosHospitalizacion
                .Where(d => !d.Eliminado
                    && (d.HospitalizacionId == hospitalizacionId || d.PacienteId == hospitalizacion.PacienteId))
                .Select(d => new PacienteArchivoVM
                {
                    ArchivoId = d.Id,
                    ArchivoNombre = d.NombreArchivo ?? "-",
                    ArchivoUrl = d.RutaArchivo
                }).ToListAsync();

            foreach (var docHosp in documentosHospitalizacion)
            {
                if (!documentos.Any(d => string.Equals(d.ArchivoUrl, docHosp.ArchivoUrl, StringComparison.OrdinalIgnoreCase)))
                    documentos.Add(docHosp);
            }

            var cuestionarioPreAnestesico = _cuestionarioPreAnestesicoService
                .GetByHospitalizacionId(hospitalizacionId)?
                .OrderByDescending(c => c.FechaRegistro)
                .FirstOrDefault();

            var autorizacionAnestesia = PdfReportHelper.BuildAutorizacionAnestesia(
                hospitalizacion,
                consentimiento,
                _citasRepository,
                _empleadoRepository,
                contentRoot,
                hospitalizacion.Consultas?.FirstOrDefault()?.Citas?.Id,
                anestesistaNotaOp);

            var listasChequeo = await _context.ListasChequeo
                .Where(l => l.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(l => l.FechaRegistro)
                .ToListAsync();

            var signosVitales = await MapSignosVitalesHospitalizacionPdfAsync(hospitalizacionId, hospitalizacion);

            var historialMedsExp = _medicamentoNoControladoRepository
                .GetHistorialByHospitalizacion(hospitalizacionId)
                .Where(m => !m.Eliminado)
                .ToList();
            var ultimaFechaRegistroExp = historialMedsExp.FirstOrDefault()?.FechaRegistro;
            var medicamentosControlados = historialMedsExp
                .Where(m => ultimaFechaRegistroExp == null || m.FechaRegistro == ultimaFechaRegistroExp)
                .Select(m => new MedicamentoNoControladoPdfVM
                {
                    ProductoNombre = m.ProductoNombre,
                    UnidadesIniciales = m.UnidadesIniciales,
                    UnidadesExtra = m.UnidadesExtra,
                    Utilizado = m.Utilizado,
                    Descartado = m.Descartado,
                    Retornadas = m.Retornadas,
                    FechaProcedimiento = m.FechaProcedimiento ?? m.FechaRegistro
                })
                .ToList();

            var registroAnestesia = _registroAnestesiaService.GetByHospitalizacionId(hospitalizacionId);

            PdfReportHelper.CompletarConsentimientoPdfVm(
                consentimiento,
                hospitalizacion,
                _citasRepository,
                _empleadoRepository,
                contentRoot,
                hospitalizacion.Consultas?.FirstOrDefault()?.Citas?.Id,
                medicamentosControlados,
                anestesistaNotaOp);

            if (consentimiento == null && medicamentosControlados.Any())
            {
                consentimiento = new ConsentimientoHospiVM
                {
                    NombrePaciente = hospitalizacion.Paciente?.Nombre ?? "-",
                    NombreCompleto = hospitalizacion.Paciente?.Nombre ?? "-",
                    MedicamentosNoControlados = medicamentosControlados
                };
            }

            PdfReportHelper.AsignarFirmaFarmaciaMedicamentos(
                consentimiento,
                _configuration["EstablecimientoImagenFirma"],
                contentRoot);

            if (consentimiento != null && string.IsNullOrEmpty(consentimiento.FirmaMedicoBase64))
                consentimiento.FirmaMedicoBase64 = medico.FirmaBase64;
            if (consentimiento != null && string.IsNullOrEmpty(consentimiento.UrlFirmaMedico))
                consentimiento.UrlFirmaMedico = medico.FirmaBase64;
            if (consentimiento != null && string.IsNullOrWhiteSpace(consentimiento.NombreMedicoTratante))
                consentimiento.NombreMedicoTratante = medico.Nombre;
            if (consentimiento != null && string.IsNullOrWhiteSpace(consentimiento.ColegiadoMedico))
                consentimiento.ColegiadoMedico = medico.Colegiado;

            medico = PdfReportHelper.ComplementarMedicoTratante(
                medico,
                consentimiento,
                cuestionarioPreAnestesico,
                autorizacionAnestesia,
                _empleadoRepository,
                contentRoot);

            ExpedienteClinicalPdfEnricher.CompletarCuestionario(cuestionarioPreAnestesico, hospitalizacion, medico.Nombre);
            foreach (var lista in listasChequeo)
                ExpedienteClinicalPdfEnricher.CompletarListaChequeo(lista, hospitalizacion, medico.Nombre);
            ExpedienteClinicalPdfEnricher.EnriquecerConsentimientoRadiologia(
                consentimiento, hospitalizacion, cuestionarioPreAnestesico);

            PdfReportHelper.AplicarMedicoTratanteEnNotas(notasEvolucion, medico.Nombre, medico.Colegiado, medico.FirmaBase64);
            PdfReportHelper.AplicarMedicoTratanteEnOrdenes(ordenes, medico.Nombre, medico.Colegiado);

            if (consentimiento != null)
            {
                if (string.IsNullOrWhiteSpace(consentimiento.NombreMedicoTratante) || consentimiento.NombreMedicoTratante == "-")
                    consentimiento.NombreMedicoTratante = medico.Nombre;
                if (string.IsNullOrWhiteSpace(consentimiento.ColegiadoMedico) || consentimiento.ColegiadoMedico == "-")
                    consentimiento.ColegiadoMedico = medico.Colegiado;

                PdfReportHelper.CompletarDatosAnestesista(
                    consentimiento,
                    autorizacionAnestesia,
                    _empleadoRepository,
                    contentRoot,
                    anestesistaNotaOp,
                    medico.Nombre,
                    _citasRepository,
                    hospitalizacion.Consultas?.FirstOrDefault()?.Citas?.Id
                        ?? hospitalizacion.Consultas?.FirstOrDefault()?.CitasId,
                    hospitalizacion);

                PdfReportHelper.SincronizarAutorizacionAnestesiaDesdeConsentimiento(
                    autorizacionAnestesia,
                    consentimiento);
            }

            if (consentimiento != null)
            {
                PdfReportHelper.CompletarFirmasConsentimientoSerSalud(
                    consentimiento,
                    medico.FirmaBase64,
                    contentRoot,
                    _configuration["EstablecimientoImagenFirma"],
                    _configuration["EstablecimientoFirmaRepresentante"],
                    string.Equals(_webHostEnvironment.EnvironmentName, Environments.Development, StringComparison.OrdinalIgnoreCase));
            }

            if (autorizacionAnestesia != null)
            {
                if (string.IsNullOrWhiteSpace(autorizacionAnestesia.NombreMedicoTratante))
                    autorizacionAnestesia.NombreMedicoTratante = medico.Nombre;
                if (string.IsNullOrWhiteSpace(autorizacionAnestesia.ColegiadoMedico))
                    autorizacionAnestesia.ColegiadoMedico = medico.Colegiado;
                if (string.IsNullOrEmpty(autorizacionAnestesia.FirmaMedicoBase64))
                    autorizacionAnestesia.FirmaMedicoBase64 = medico.FirmaBase64;
            }

            double? pesoCtx = cuestionarioPreAnestesico?.Peso;
            double? estCtx = cuestionarioPreAnestesico?.Estatura;
            if (!pesoCtx.HasValue && double.TryParse(pacienteExp?.Peso?.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var pesoPac))
                pesoCtx = pesoPac;
            if (!estCtx.HasValue && double.TryParse(pacienteExp?.Talla?.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var estPac))
                estCtx = estPac;
            string imcCtx = "-";
            if (pesoCtx.HasValue && estCtx.HasValue && estCtx.Value > 0)
                imcCtx = (pesoCtx.Value / (estCtx.Value * estCtx.Value)).ToString("0.00");

            var alergiasCtx = cuestionarioPreAnestesico == null
                ? "-"
                : string.IsNullOrWhiteSpace(cuestionarioPreAnestesico.PA_Alergia)
                    ? "-"
                    : $"{cuestionarioPreAnestesico.PA_Alergia} {cuestionarioPreAnestesico.PA_AlergiaCual}".Trim();

            var contexto = new ExpedienteHospitalizacionContextoVm
            {
                FechaInicio = hospitalizacion.FechaInicio.ToString("dd/MM/yyyy HH:mm"),
                FechaFin = hospitalizacion.FechaFin.ToString("dd/MM/yyyy HH:mm"),
                Habitacion = hospitalizacion.Habitacion?.NombreNumeroHabitacion ?? consentimiento?.NumeroHabitacion ?? "-",
                CategoriaHabitacion = hospitalizacion.CategoriaHabitacionTarifa?.NombreTarifa ?? hospitalizacion.Habitacion?.CategoriaHabitacion?.NombreCategoria ?? "-",
                Observaciones = hospitalizacion.Observaciones ?? "",
                Procedimiento = cuestionarioPreAnestesico?.ProcedimientoProgramado
                    ?? autorizacionAnestesia?.Procedimiento
                    ?? consentimiento?.TratamientoMedico
                    ?? consentimiento?.ProcedimientoProgramado
                    ?? "-",
                MedicoTratante = medico.Nombre,
                ColegiadoMedico = medico.Colegiado,
                Anestesiologo = autorizacionAnestesia?.NombreAnestesista ?? "-",
                HoraIngreso = consentimiento?.HoraIngreso ?? hospitalizacion.FechaInicio.ToString("dd/MM/yyyy HH:mm"),
                ResponsableCuenta = consentimiento?.NombreResponsable ?? "-",
                DpiPaciente = consentimiento?.DPI ?? pacienteExp?.Dpi ?? "-",
                EdadPaciente = consentimiento?.Edad ?? edadExp,
                TipoSangre = consentimiento?.TipoSangre ?? pacienteExp?.TipoDeSangre ?? "-",
                Seguro = string.IsNullOrWhiteSpace(consentimiento?.Aseguradora)
                    ? (consentimiento?.PoseeSeguroMedico ?? "-")
                    : $"{consentimiento.PoseeSeguroMedico} — {consentimiento.Aseguradora}",
                Peso = pesoCtx?.ToString("0.##") ?? pacienteExp?.Peso ?? "-",
                Estatura = estCtx?.ToString("0.##") ?? pacienteExp?.Talla ?? "-",
                Imc = imcCtx,
                MedicamentosActuales = cuestionarioPreAnestesico?.AI_Medicamentos ?? "-",
                Alergias = alergiasCtx,
                ComentariosClinicos = cuestionarioPreAnestesico?.AI_Comentarios ?? "",
                UrlArchivoConsentimiento = hospitalizacion.UrlArchivoConsentimiento ?? ""
            };

            var notasEvoAsignadas = PdfReportHelper.AsignarNotasEvolucionExpediente(notasEvolucion);
            var notasEnfAsignadas = PdfReportHelper.AsignarNotasEnfermeriaExpediente(notasEnfermeria);

            if (autorizacionAnestesia != null && consentimiento != null)
                PdfReportHelper.SincronizarAutorizacionAnestesiaDesdeConsentimiento(autorizacionAnestesia, consentimiento);

            var anestesistaExpediente = PdfReportHelper.ResolverAnestesistaParaPdf(
                hospitalizacion,
                consentimiento,
                autorizacionAnestesia,
                _empleadoRepository,
                _citasRepository,
                contentRoot,
                anestesistaNotaOp,
                medico.Nombre,
                citaId ?? hospitalizacion.Consultas?.FirstOrDefault()?.Citas?.Id
                    ?? hospitalizacion.Consultas?.FirstOrDefault()?.CitasId);

            var registroAnestesiaPdf = RegistroAnestesiaPdfHelper.Build(
                hospitalizacionId,
                hospitalizacion,
                registroAnestesia,
                consentimiento,
                cuestionarioPreAnestesico,
                autorizacionAnestesia,
                anestesistaExpediente.Nombre,
                anestesistaExpediente.FirmaBase64);

            if (autorizacionAnestesia != null && consentimiento != null)
                PdfReportHelper.SincronizarAutorizacionAnestesiaDesdeConsentimiento(autorizacionAnestesia, consentimiento);

            var documentosEmbebidos = PdfReportHelper.ResolverDocumentosEmbebidos(
                documentos.Select(d => (d.ArchivoNombre, d.ArchivoUrl)),
                contentRoot);

            var documentoConsentimientoEmbebido = !string.IsNullOrWhiteSpace(contexto.UrlArchivoConsentimiento)
                ? PdfReportHelper.ResolverDocumentoEmbebido(
                    contexto.UrlArchivoConsentimiento,
                    "Consentimiento firmado",
                    contentRoot)
                : null;

            var historialMedicamentos = MedicamentoHistorialPdfBuilder.Build(_context, hospitalizacionId);

            var model = new ExpedienteCompletoViewModel
            {
                HospitalizacionId = hospitalizacionId,
                PacienteId = hospitalizacion.PacienteId,
                HabitacionId = hospitalizacion.HabitacionId,
                CitaId = citaId ?? hospitalizacion.Consultas?.FirstOrDefault()?.Citas?.Id
                    ?? hospitalizacion.Consultas?.FirstOrDefault()?.CitasId,
                PacienteNombre = hospitalizacion.Paciente?.Nombre ?? "-",
                MedicoTratanteNombre = medico.Nombre,
                MedicoTratanteEspecialidad = medico.Especialidad,
                MedicoTratanteColegiado = medico.Colegiado,
                MedicoTratanteFirmaBase64 = medico.FirmaBase64,
                Consentimiento = consentimiento,
                CuestionarioPreAnestesico = cuestionarioPreAnestesico,
                NotasEvolucion = notasEvolucion,
                NotasEnfermeria = notasEnfermeria,
                OrdenesMedicas = ordenes,
                NotasOperatorias = notasOperatorias,
                Documentos = documentos,
                DocumentosEmbebidos = documentosEmbebidos,
                DocumentoConsentimientoEmbebido = documentoConsentimientoEmbebido,
                AutorizacionAnestesia = autorizacionAnestesia,
                ListasChequeo = listasChequeo,
                SignosVitales = signosVitales,
                MedicamentosControlados = medicamentosControlados,
                HistorialMedicamentos = historialMedicamentos,
                RegistroAnestesia = registroAnestesia,
                RegistroAnestesiaPdf = registroAnestesiaPdf,
                Contexto = contexto,
                NotaIngresoEvolucion = notasEvoAsignadas.Ingreso,
                NotaIngresoEnfermeria = notasEnfAsignadas.Ingreso,
                NotaTrasladoEvolucion = notasEvoAsignadas.Traslado,
                NotaTrasladoEnfermeria = notasEnfAsignadas.Traslado,
                NotaRecepcionEvolucion = notasEvoAsignadas.Recepcion,
                NotaRecepcionEnfermeria = notasEnfAsignadas.Recepcion,
                NotasEgresoEvolucion = notasEvoAsignadas.Egreso,
                NotasEgresoEnfermeria = notasEnfAsignadas.Egreso
            };

            var pdfOptions = new ConvertOptions
            {
                PageMargins = new Margins(10, 10, 10, 10),
                PageOrientation = Orientation.Portrait
            };

            return await RenderPdfAsync(
                "Views/CrearPDF/ExpedienteCompletoPDF.cshtml", model, pdfOptions);
        }

        public async Task<IActionResult> MedicamentosControladosHospiPDF(int hospitalizacionId)
        {
            var hospitalizacion = _hospitalizacionRepository.Get(
                hospitalizacionId,
                includeConsultas: true);

            if (hospitalizacion == null)
                return NotFound("No se encontró la hospitalización solicitada.");

            var contentRoot = ContentRoot;
            var consentimiento = ResolverConsentimientoHospitalizacion(hospitalizacion)
                ?? new ConsentimientoHospiVM
                {
                    NombrePaciente = hospitalizacion.Paciente?.Nombre ?? "-",
                    NombreCompleto = hospitalizacion.Paciente?.Nombre ?? "-"
                };

            var historialMeds = _medicamentoNoControladoRepository
                .GetHistorialByHospitalizacion(hospitalizacionId)
                .Where(m => !m.Eliminado)
                .ToList();
            var ultimaFechaRegistro = historialMeds.FirstOrDefault()?.FechaRegistro;
            var medicamentosControlados = historialMeds
                .Where(m => ultimaFechaRegistro == null || m.FechaRegistro == ultimaFechaRegistro)
                .Select(m => new MedicamentoNoControladoPdfVM
                {
                    ProductoNombre = m.ProductoNombre,
                    UnidadesIniciales = m.UnidadesIniciales,
                    UnidadesExtra = m.UnidadesExtra,
                    Utilizado = m.Utilizado,
                    Descartado = m.Descartado,
                    Retornadas = m.Retornadas,
                    FechaProcedimiento = m.FechaProcedimiento ?? m.FechaRegistro
                })
                .ToList();

            var anestesistaNotaOp = _hospitalizacionRepository.GetNotasOperatoriasByHospitalizacion(hospitalizacionId)
                ?.OrderByDescending(n => n.FechaRegistro)
                .FirstOrDefault()
                ?.Anestesista;

            PdfReportHelper.CompletarConsentimientoPdfVm(
                consentimiento,
                hospitalizacion,
                _citasRepository,
                _empleadoRepository,
                contentRoot,
                hospitalizacion.Consultas?.FirstOrDefault()?.Citas?.Id,
                medicamentosControlados,
                anestesistaNotaOp);

            PdfReportHelper.AsignarFirmaFarmaciaMedicamentos(
                consentimiento,
                _configuration["EstablecimientoImagenFirma"],
                contentRoot);

            var medOptions = new ConvertOptions
            {
                PageMargins = new Margins(10, 10, 10, 10),
                PageOrientation = Orientation.Portrait
            };

            return await RenderPdfAsync(
                "Views/CrearPDF/MedicamentosContrladosHospiPDF.cshtml", consentimiento, medOptions);
        }

        public async Task<IActionResult> CuestionarioPreAnestesicoPDF(int hospitalizacionId)
        {
            DevelopmentDemoSeedGuard.EnsureExpedienteHospitalizacion(_webHostEnvironment, _context, hospitalizacionId);

            var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId, includeConsultas: true);
            var cuestionario = _cuestionarioPreAnestesicoService
                .GetByHospitalizacionId(hospitalizacionId)?
                .OrderByDescending(c => c.FechaRegistro)
                .FirstOrDefault();

            if (cuestionario == null)
                return NotFound("No se encontró cuestionario pre-anestésico para esta hospitalización.");

            var medicoNombre = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                hospitalizacion, _empleadoRepository, ContentRoot, _citasRepository).Nombre;
            ExpedienteClinicalPdfEnricher.CompletarCuestionario(cuestionario, hospitalizacion, medicoNombre);

            var cuestOptions = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait
            };

            return await RenderPdfAsync(
                "Views/CrearPDF/CuestionarioPreAnestesicoPDF.cshtml", cuestionario, cuestOptions);
        }

        public async Task<IActionResult> ListaChequeoPDF(int hospitalizacionId)
        {
            DevelopmentDemoSeedGuard.EnsureExpedienteHospitalizacion(_webHostEnvironment, _context, hospitalizacionId);

            var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId, includeConsultas: true);
            if (hospitalizacion == null)
                return NotFound("Hospitalización no encontrada.");

            var listas = await _context.ListasChequeo
                .Where(l => l.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(l => l.FechaRegistro)
                .ToListAsync();

            if (!listas.Any())
                return NotFound("No hay listas de chequeo registradas para esta hospitalización.");

            var medicoNombre = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                hospitalizacion, _empleadoRepository, ContentRoot, _citasRepository).Nombre;
            foreach (var lista in listas)
                ExpedienteClinicalPdfEnricher.CompletarListaChequeo(lista, hospitalizacion, medicoNombre);

            ViewBag.PacienteNombre = hospitalizacion.Paciente?.Nombre ?? "-";
            ViewBag.HospitalizacionId = hospitalizacionId;

            _generatePdf.SetConvertOptions(new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait
            });

            return await RenderPdfAsync("Views/CrearPDF/ListaChequeoPDF.cshtml", listas);
        }

        public async Task<IActionResult> OrdenesMedicasHospPDF(int hospitalizacionId)
        {
            var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId, includeConsultas: true);
            if (hospitalizacion == null)
                return NotFound("Hospitalización no encontrada.");

            var ordenes = await _context.OrdenesMedicas
                .Where(o => o.HospitalizacionId == hospitalizacionId)
                .OrderBy(o => o.FechaHora)
                .ToListAsync();

            if (ordenes == null || !ordenes.Any())
                return NotFound("No hay órdenes médicas para esta hospitalización.");

            var contentRoot = ContentRoot;
            var medicoTratante = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                hospitalizacion, _empleadoRepository, contentRoot);
            var paciente = hospitalizacion.Paciente;
            var empleadoTextCita = hospitalizacion.Consultas?
                .FirstOrDefault()?.Citas?.EmpleadoText ?? "Sin asignar";

            var autorizadorIds = ordenes
                .Where(o => o.Autorizado && !string.IsNullOrWhiteSpace(o.UsuarioAutoriza))
                .Select(o => o.UsuarioAutoriza)
                .Distinct()
                .ToList();
            var autorizadores = await ObtenerUsuariosAutorizadoresAsync(autorizadorIds);

            var model = new OrdenesMedicasListaPdfViewModel
            {
                PacienteNombre = paciente?.Nombre ?? "-",
                PacienteEdad = CalcularEdad(paciente?.FechaNacimiento),
                PacienteSexoText = paciente?.sexoText ?? "-",
                EmpleadoText = medicoTratante.Nombre,
                ColegioEmpleado = medicoTratante.Colegiado,
                Ordenes = ordenes.Select(o =>
                {
                    var profesionalUser = PdfReportHelper.ResolverUsuarioProfesionalTexto(o.Profesional, _userRepository);
                    var registradoPor = PdfReportHelper.ResolverNombreProfesionalTexto(
                        o.Profesional,
                        _userRepository,
                        _empleadoRepository);
                    User autorizador = null;
                    if (o.Autorizado && !string.IsNullOrWhiteSpace(o.UsuarioAutoriza))
                        autorizadores.TryGetValue(o.UsuarioAutoriza, out autorizador);

                    var firmante = PdfReportHelper.ResolverFirmanteClinico(
                        o.Autorizado,
                        o.UsuarioAutoriza,
                        profesionalUser ?? autorizador,
                        autorizador == null ? null : new Dictionary<string, User> { { autorizador.Id, autorizador } },
                        _empleadoRepository,
                        medicoTratante.FirmaBase64,
                        contentRoot,
                        registradoPor);

                    var firmaOrden = firmante.FirmaBase64;
                    if (string.IsNullOrEmpty(firmaOrden) && !string.IsNullOrWhiteSpace(registradoPor))
                        firmaOrden = PdfReportHelper.ObtenerFirmaEmpleadoPorNombre(registradoPor, _empleadoRepository, contentRoot);

                    var nombreMedico = PdfReportHelper.ResolverNombreMedicoPdf(empleadoTextCita, registradoPor, medicoTratante.Nombre);
                    var colegiado = PdfReportHelper.ObtenerColegiadoPorNombre(
                        nombreMedico, _empleadoRepository, medicoTratante.Colegiado);

                    return new OrdenMedicaViewModel
                    {
                        FechaHora = o.FechaHora.ToString("dd/MM/yyyy hh:mm tt"),
                        Profesional = registradoPor,
                        Descripcion = PdfReportHelper.NormalizarHtmlParaPdf(o.Descripcion ?? ""),
                        PacienteNombre = paciente?.Nombre ?? "-",
                        PacienteEdad = CalcularEdad(paciente?.FechaNacimiento),
                        PacienteSexoText = paciente?.sexoText ?? "-",
                        EmpleadoText = nombreMedico,
                        ColegioEmpleado = colegiado,
                        Realizada = o.Realizada ? "Sí" : "No",
                        Autorizado = o.Autorizado,
                        FirmaBase64 = firmaOrden,
                        NombreFirmante = firmante.NombreFirmante,
                        AutorizadoPor = firmante.AutorizadoPor
                    };
                }).ToList()
            };

            _generatePdf.SetConvertOptions(new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait
            });

            return await RenderPdfAsync("Views/CrearPDF/HospitalizacionOrdenesMedicasPDF.cshtml", model);
        }

        public async Task<IActionResult> DocumentosCargadosHospPDF(int hospitalizacionId)
        {
            var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId);
            if (hospitalizacion == null)
                return NotFound("Hospitalización no encontrada.");

            var contentRoot = ContentRoot;
            var documentos = await _context.PacienteArchivos
                .Where(a => a.PacienteId == hospitalizacion.PacienteId)
                .Select(a => new PacienteArchivoVM
                {
                    ArchivoId = a.Id,
                    ArchivoNombre = a.NombreArchivo ?? "-",
                    ArchivoUrl = a.UrlArchivo
                }).ToListAsync();

            var documentosHospitalizacion = await _context.DocumentosHospitalizacion
                .Where(d => !d.Eliminado
                    && (d.HospitalizacionId == hospitalizacionId || d.PacienteId == hospitalizacion.PacienteId))
                .Select(d => new PacienteArchivoVM
                {
                    ArchivoId = d.Id,
                    ArchivoNombre = d.NombreArchivo ?? "-",
                    ArchivoUrl = d.RutaArchivo
                }).ToListAsync();

            foreach (var docHosp in documentosHospitalizacion)
            {
                if (!documentos.Any(d => string.Equals(d.ArchivoUrl, docHosp.ArchivoUrl, StringComparison.OrdinalIgnoreCase)))
                    documentos.Add(docHosp);
            }

            if (!documentos.Any() && string.IsNullOrWhiteSpace(hospitalizacion.UrlArchivoConsentimiento))
                return NotFound("No hay documentos cargados para esta hospitalización.");

            var documentosEmbebidos = PdfReportHelper.ResolverDocumentosEmbebidos(
                documentos.Select(d => (d.ArchivoNombre, d.ArchivoUrl)),
                contentRoot);

            DocumentoEmbebidoVm consentimientoEmbebido = null;
            if (!string.IsNullOrWhiteSpace(hospitalizacion.UrlArchivoConsentimiento))
            {
                consentimientoEmbebido = PdfReportHelper.ResolverDocumentoEmbebido(
                    hospitalizacion.UrlArchivoConsentimiento,
                    "Consentimiento firmado",
                    contentRoot);
            }

            var model = new DocumentosCargadosPdfViewModel
            {
                HospitalizacionId = hospitalizacionId,
                PacienteNombre = hospitalizacion.Paciente?.Nombre ?? "-",
                DocumentosEmbebidos = documentosEmbebidos,
                DocumentoConsentimientoEmbebido = consentimientoEmbebido
            };

            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait
            };

            return await RenderPdfAsync("Views/CrearPDF/DocumentosCargadosHospPDF.cshtml", model, options);
        }

        public async Task<IActionResult> SignosVitalesHospPDF(int hospitalizacionId)
        {
            var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId);
            if (hospitalizacion == null)
                return NotFound("Hospitalización no encontrada.");

            var signos = await MapSignosVitalesHospitalizacionPdfAsync(hospitalizacionId, hospitalizacion);

            if (!signos.Any())
                return NotFound("No hay registros de signos vitales para esta hospitalización.");

            ViewBag.PacienteNombre = hospitalizacion.Paciente?.Nombre ?? "-";
            ViewBag.HospitalizacionId = hospitalizacionId;

            var signosOptions = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Landscape
            };

            return await RenderPdfAsync(
                "Views/CrearPDF/SignosVitalesHospPDF.cshtml", signos, signosOptions);
        }

        public async Task<IActionResult> RegistroAnestesiaPDF(int hospitalizacionId)
        {
            DevelopmentDemoSeedGuard.EnsureExpedienteHospitalizacion(_webHostEnvironment, _context, hospitalizacionId);

            var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId, includeConsultas: true);
            if (hospitalizacion == null)
                return NotFound("Hospitalización no encontrada.");

            var contentRoot = ContentRoot;
            var consentimiento = ResolverConsentimientoHospitalizacion(hospitalizacion)
                ?? new ConsentimientoHospiVM
                {
                    NombrePaciente = hospitalizacion.Paciente?.Nombre ?? "-",
                    NombreCompleto = hospitalizacion.Paciente?.Nombre ?? "-"
                };
            var cuestionario = _cuestionarioPreAnestesicoService
                .GetByHospitalizacionId(hospitalizacionId)?
                .OrderByDescending(c => c.FechaRegistro)
                .FirstOrDefault();

            var anestesistaNotaOp = _hospitalizacionRepository.GetNotasOperatoriasByHospitalizacion(hospitalizacionId)
                ?.OrderByDescending(n => n.FechaRegistro)
                .FirstOrDefault()
                ?.Anestesista;

            var medicoTratante = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                hospitalizacion, _empleadoRepository, contentRoot, _citasRepository);

            var citaId = hospitalizacion.Consultas?.FirstOrDefault()?.Citas?.Id
                ?? hospitalizacion.Consultas?.FirstOrDefault()?.CitasId;

            PdfReportHelper.CompletarConsentimientoPdfVm(
                consentimiento,
                hospitalizacion,
                _citasRepository,
                _empleadoRepository,
                contentRoot,
                citaId,
                null,
                anestesistaNotaOp);

            var anest = PdfReportHelper.BuildAutorizacionAnestesia(
                hospitalizacion,
                consentimiento,
                _citasRepository,
                _empleadoRepository,
                contentRoot,
                citaId,
                anestesistaNotaOp);

            var anestesistaResuelto = PdfReportHelper.ResolverAnestesistaParaPdf(
                hospitalizacion,
                consentimiento,
                anest,
                _empleadoRepository,
                _citasRepository,
                contentRoot,
                anestesistaNotaOp,
                medicoTratante.Nombre,
                citaId);

            var registroGuardado = _registroAnestesiaService.GetByHospitalizacionId(hospitalizacionId);

            var model = RegistroAnestesiaPdfHelper.Build(
                hospitalizacionId,
                hospitalizacion,
                registroGuardado,
                consentimiento,
                cuestionario,
                anest,
                anestesistaResuelto.Nombre,
                anestesistaResuelto.FirmaBase64);

            _generatePdf.SetConvertOptions(new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait
            });

            return await RenderPdfAsync("Views/CrearPDF/RegistroAnestesiaPDF.cshtml", model);
        }

        public IActionResult DetalleCotizacionListaPdf()
        {

            var cotizaciones = _cotizacionRepository.GetList();

            return new ViewAsPdf("DetalleCotizacionListaPdf", cotizaciones);
        }


        // public async Task<IActionResult> HistoricoProductosPDF(string productosIds)
        // {
        //     var listaProductosIds = new List<int>();
        //     var ids = productosIds.Split(',');
        //     if (ids != null)
        //     {
        //         foreach (var id in ids)
        //         {
        //             listaProductosIds.Add(Convert.ToInt32(id));
        //         }
        //     }
        //     var historico = _productosService.GetHistoricoProductos(DateTime.Now, DateTime.Now, listaProductosIds);
        //     var model = new HistoricoProductosPDFVM
        //     {
        //         Movimientos = historico
        //     };
        //     _generatePdf.SetConvertOptions(new ConvertOptions
        //     {
        //         PageOrientation = Orientation.Landscape
        //     });
        //     return await RenderPdfAsync("Views/CrearPDF/HistoricoProductosPDF.cshtml", model);
        // }
        public async Task<IActionResult> HistoricoProductosPDF(
    DateTime? fechaInicio,
    DateTime? fechaFin,
    string? tiposProducto,
    string? ambientes,
    string? bodegas,
    string? productos
)
        {
            // 1) Traer histórico (sin filtro por productosIds, porque en la vista filtras por nombre)
            var historico = _productosService.GetHistoricoProductos(fechaInicio, fechaFin, new List<int>());

            // 2) Aplicar filtros de nombres igual que KO
            var filtrado = AplicarFiltrosHistorico(historico, tiposProducto, ambientes, bodegas, productos);

            var vm = new HistoricoProductosPDFVM
            {
                Movimientos = filtrado
            };

            _generatePdf.SetConvertOptions(new ConvertOptions
            {
                PageOrientation = Orientation.Landscape,
            });
            return await RenderPdfAsync("Views/CrearPDF/HistoricoProductosPDF.cshtml", vm);
        }

        public async Task<IActionResult> HistoricoProductosPDFNacional(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        string? tiposProducto,
        string? ambientes,
        string? bodegas,
        string? productos
        )
        {
            // 1) Traer histórico (sin filtro por productosIds, porque en la vista filtras por nombre)
            var historico = _productosService.GetHistoricoProductosNacional(fechaInicio, fechaFin, new List<int>());

            // 2) Aplicar filtros de nombres igual que KO
            var filtrado = AplicarFiltrosHistoricoNacional(historico, tiposProducto, ambientes, bodegas, productos);

            var vm = new HistoricoProductosPDFVM
            {
                MovimientosNacional = filtrado
            };

            _generatePdf.SetConvertOptions(new ConvertOptions
            {
                PageOrientation = Orientation.Landscape,
            });
            return await RenderPdfAsync("Views/CrearPDF/HistoricoProductosPDFNacional.cshtml", vm);
        }

        private static List<MovimientoProductoViewModel> AplicarFiltrosHistorico(
            List<MovimientoProductoViewModel> historico,
            string? tiposProducto,
            string? ambientes,
            string? bodegas,
            string? productos
        )
        {
            if (historico == null || historico.Count == 0)
                return historico ?? new List<MovimientoProductoViewModel>();

            static string Norm(string? s) => (s ?? "").Trim().ToLowerInvariant();

            static HashSet<string> Parse(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return new HashSet<string>();
                return s.Split('|')
                        .Select(Norm)
                        .Where(x => x.Length > 0)
                        .ToHashSet();
            }

            var tiposSet = Parse(tiposProducto);
            var ambSet = Parse(ambientes);
            var bodSet = Parse(bodegas);
            var prodSet = Parse(productos);

            IEnumerable<MovimientoProductoViewModel> q = historico;

            if (tiposSet.Count > 0) q = q.Where(x => tiposSet.Contains(Norm(x.TipoProductoNombre)));
            if (ambSet.Count > 0) q = q.Where(x => ambSet.Contains(Norm(x.AmbienteNombre)));
            if (bodSet.Count > 0) q = q.Where(x => bodSet.Contains(Norm(x.BodegaNombre)));
            if (prodSet.Count > 0) q = q.Where(x => prodSet.Contains(Norm(x.Medicamento)));

            return q.ToList();
        }


        private static List<MovimientoProductoNacionalViewModel> AplicarFiltrosHistoricoNacional(
            List<MovimientoProductoNacionalViewModel> historico,
            string? tiposProducto,
            string? ambientes,
            string? bodegas,
            string? productos
        )
        {
            if (historico == null || historico.Count == 0)
                return historico ?? new List<MovimientoProductoNacionalViewModel>();

            static string Norm(string? s) => (s ?? "").Trim().ToLowerInvariant();

            static HashSet<string> Parse(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return new HashSet<string>();
                return s.Split('|')
                        .Select(Norm)
                        .Where(x => x.Length > 0)
                        .ToHashSet();
            }

            var tiposSet = Parse(tiposProducto);
            var ambSet = Parse(ambientes);
            var bodSet = Parse(bodegas);
            var prodSet = Parse(productos);

            IEnumerable<MovimientoProductoNacionalViewModel> q = historico;

            if (tiposSet.Count > 0) q = q.Where(x => tiposSet.Contains(Norm(x.TipoProductoNombre)));
            if (ambSet.Count > 0) q = q.Where(x => ambSet.Contains(Norm(x.AmbienteNombre)));
            if (bodSet.Count > 0) q = q.Where(x => bodSet.Contains(Norm(x.BodegaNombre)));
            if (prodSet.Count > 0) q = q.Where(x => prodSet.Contains(Norm(x.Medicamento)));

            return q.ToList();
        }

        #region PACIENTES
        public async Task<IActionResult> CarneVacunacionJovenes(int idPaciente)
        {

            var vacunas = _pacientesRepository.GetVacunasPaciente(idPaciente);
            var paciente = _pacientesRepository.GetPacientePorId(idPaciente);
            var model = new CarneVacunacionJovenViewModel();
            model.VacunasPaciente = vacunas;
            model.Paciente = paciente;

            return await RenderPdfAsync("Views/CrearPDF/CarneVacunacionJovenes.cshtml", model);
        }
        public async Task<IActionResult> PacientesExpedientePDF(int pacienteId)
        {
            var paciente = _pacientesRepository.Get(pacienteId);

            return await RenderPdfAsync("Views/CrearPDF/PacientesExpedientePDF.cshtml", paciente);
        }
        public async Task<IActionResult> PacientesPresupuestoDentalPDF(int presupuestoId)
        {
            var presupuesto = _pacientesRepository.GetPresupuestoDental(presupuestoId);

            return await RenderPdfAsync
                ("Views/CrearPDF/PacientesPresupuestoDentalPDF.cshtml", presupuesto);
        }

        public IActionResult PacientesRetiradosPDF()
        {
            var pacientesInactivos = _pacientesRepository.GetPacientesInactivos();

            //return View(pacientesInactivos);
            return new ViewAsPdf("PacientesRetiradosPDF", pacientesInactivos);
        }


        public async Task<IActionResult> AmbulanciaPDF(int id)
        {
            var ambulancia = await _context.Ambulancias.FirstOrDefaultAsync(a => a.Id == id);
            if (ambulancia == null)
            {
                return BadRequest("No se encontró el registro de ambulancia.");
            }

            var options = new ConvertOptions
            {
                PageOrientation = Wkhtmltopdf.NetCore.Options.Orientation.Portrait,
                PageMargins = { Top = 10, Bottom = 10, Left = 10, Right = 10 }
            };

            _generatePdf.SetConvertOptions(options);

            return await RenderPdfAsync("Views/CrearPDF/AmbulanciaPDF.cshtml", ambulancia);
        }
        public async Task<IActionResult> PacientesCompromisoMembresiaPDF(int? pacienteId)
        {
            if (pacienteId == null)
            {
                TempData["Message"] = "Solicitud err�nea";
                return RedirectToAction("Lista", "Pacientes");
            }
            var paciente = _pacientesRepository.Get((int)pacienteId);
            if (paciente == null)
            {
                TempData["Message"] = "El paciente consultado no existe";
                return RedirectToAction("Lista", "Pacientes");
            }
            if (!(paciente.TieneMembresia ?? false))
            {
                TempData["Message"] = "Este paciente no tiene membres�a";
                return RedirectToAction("Lista", "Pacientes");
            }

            return await RenderPdfAsync("PacientesCompromisoMembresiaPDF", paciente);
        }
        #endregion
        public async Task<IActionResult> CitasPorFechasPdf(string fecha, int? sucursalId, int? empleadoId, int? especialidadId)
        {
            var fechas = fecha.Split('-');
            var formato = "dd/MM/yyyy HH:mm";

            if (!DateTime.TryParseExact(fechas[0].Trim(), formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var inicio) ||
                !DateTime.TryParseExact(fechas[1].Trim(), formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fin))
            {
                return BadRequest("Formato de fecha inválido");
            }

            fin = fin.AddDays(1);

            var citasPorFecha = (List<Citas>)_citasRepository.CitasPorFechas(inicio, fin, sucursalId, empleadoId, especialidadId);

            if (sucursalId != null)
            {
                citasPorFecha = citasPorFecha
                    .Where(a => a.SucursalId != null && a.SucursalId == sucursalId)
                    .ToList();
            }
            if (empleadoId != null)
            {
                citasPorFecha = citasPorFecha
                    .Where(a => a.EmpleadoId != null && a.EmpleadoId == empleadoId)
                    .ToList();
            }
            if (especialidadId != null)
            {
                citasPorFecha = citasPorFecha
                    .Where(a => a.EspecialidadId != null && a.EspecialidadId == especialidadId)
                    .ToList();
            }

            var u = _userRepository.GetDisplayName(_userManager.GetUserId(HttpContext.User));

            var model = new ReporteCitasViewModel()
            {
                Citas = citasPorFecha,
                Usuario = u
            };

            return await RenderPdfAsync("Views/Cita/ReporteCitasCalendarioPDF.cshtml", model);
        }


        public async Task<IActionResult> generarPdfExamenesLaboratorio(int examenLaboratorioId, int? pacienteId, string TipoPDF)
        {
            var examenLaboratorio = _laboratorioClinico.GetExamenLab(examenLaboratorioId);
            if (examenLaboratorio == null)
                return NotFound("Examen de laboratorio no encontrado.");
            var pacienteAsociado = pacienteId.HasValue
                ? _pacientesRepository.GetPacientePorId(pacienteId.Value)
                : null;

            var paciente = new Paciente();
            if (pacienteAsociado != null)
            {
                paciente = pacienteAsociado;
            }
            var model = new ExamenLabClinicoViewModel
            {
                Id = examenLaboratorio.Id,
                NombreExamen = examenLaboratorio.NombreExamen,
                Advertencias = examenLaboratorio.Advertencias,
                Instrucciones = examenLaboratorio.Instrucciones,
                Indicaciones = examenLaboratorio.Indicaciones,
                DeclaracionConsentimiento = examenLaboratorio.DeclaracionConsentimiento,
                Preguntas = examenLaboratorio.ExamenLabClinicosPreguntas
                .Where(p => !p.Eliminado)
                   .Select(p => new ExamenLabClinicoPreguntasViewModel
                   {

                       Id = p.Id,
                       Pregunta = p.Pregunta,
                       Detalles = p.Detalles,
                       Respuesta = p.Respuesta

                   })
                   .ToList(),
                TipoPDF = TipoPDF,
                Paciente = paciente
            };
            var options = new ConvertOptions
            {
                PageOrientation = Wkhtmltopdf.NetCore.Options.Orientation.Landscape,

            };



            _generatePdf.SetConvertOptions(options);

            return await RenderPdfAsync("Views/CrearPDF/generarPdfExamenesLaboratorio.cshtml", model);
        }

        public async Task<IActionResult> GenerarPdfLaboratorioClinicoExamenesClinicos(string fechaInicial, string fechaFinal, string estado,
            string medicoReferido, string usuarioSolicitaRequest, string usuarioIngresoRequest)
        {
            var laboratorio = _laboratorioClinico.GetAllExamenesRealizados();
            List<ExamenesRealizadosViewModel> modeloLista = new List<ExamenesRealizadosViewModel>();
            foreach (var item in laboratorio)
            {
                var paciente = item.Paciente == null ? "" : item.Paciente.Nombre;
                var NombreEstado = item.EstadoExamen == null ? "// sin asignar" : item.EstadoExamen.Nombre;
                var medico = item.Medicos == null ? "// sin referencia" : item.Medicos.Nombres;

                var usuarioSolicita = await _userManager.FindByIdAsync(item.UsuarioSolicita);
                var usuarioIngreso = await _userManager.FindByIdAsync(item.UsuarioIngresa);

                var usuarioSolicitaText = usuarioSolicita == null ? "// sin asignar" : usuarioSolicita.Email;
                var usuarioIngresoText = usuarioIngreso == null ? "// sin asignar" : usuarioIngreso.Email;

                var objeto = new ExamenesRealizadosViewModel()
                {
                    ExamenNumero = item.Id,
                    Paciente = paciente,
                    FechaRealizacion = item.FechaRealizacion,
                    MedicoReferido = medico,
                    ClinicaReferida = item.ClinicaReferida,
                    Estado = NombreEstado,
                    UsuarioSolicita = usuarioSolicitaText,
                    UsuarioIngreso = usuarioIngresoText,
                };

                modeloLista.Add(objeto);

            }

            if (!string.IsNullOrEmpty(fechaInicial) && !string.IsNullOrEmpty(fechaFinal))
            {
                DateTime fechaInicio = DateTime.Parse(fechaInicial);
                DateTime fechaFin = DateTime.Parse(fechaFinal);
                modeloLista = modeloLista.Where(x => x.FechaRealizacion.Date >= fechaInicio && x.FechaRealizacion.Date <= fechaFin).ToList();

            }

            if (!string.IsNullOrEmpty(medicoReferido))
            {
                //Ajustar el medico referido

                modeloLista = modeloLista.Where(x =>
                    x.MedicoReferido != null &&
                    x.MedicoReferido.ToLower().Trim() == medicoReferido.ToLower().Trim()).ToList();


            }

            if (!string.IsNullOrEmpty(estado))
            {
                modeloLista = modeloLista.Where(x =>
                    x.Estado != null &&
                    x.Estado.ToLower().Trim() == estado.ToLower().Trim()).ToList();
            }
            if (!string.IsNullOrEmpty(usuarioSolicitaRequest))
            {
                //Ajustar usuario
                modeloLista = modeloLista.Where(x =>
                    x.UsuarioSolicita != null &&
                    x.UsuarioSolicita.ToLower().Trim() == usuarioSolicitaRequest.ToLower().Trim()).ToList();
            }

            if (!string.IsNullOrEmpty(usuarioIngresoRequest))
            {
                //Ajustar usuario
                modeloLista = modeloLista.Where(x =>
                    x.UsuarioIngreso != null &&
                    x.UsuarioIngreso.ToLower().Trim() == usuarioIngresoRequest.ToLower().Trim()).ToList();
            }

            return await RenderPdfAsync("Views/CrearPDF/LaboratorioClinicoExamenesClinicosPDF.cshtml", modeloLista);

        }

        public async Task<IActionResult> GenerarPdfCompraOrdenes(string fechaInicial, string fechaFinal, int comprobante,
            string proveedor, string vendedor)
        {

            var data = _compraRepository.PaginacionOrdenesCompra(null, null, null, 25, fechaInicial, fechaFinal, comprobante, proveedor, vendedor);

            return await RenderPdfAsync("Views/CrearPDF/CompraPDF.cshtml", data);
        }

        public async Task<IActionResult> GenerarPdfCompraCompras(string fechaInicial, string fechaFinal, string comprobante, string proveedor, string vendedor,
            int numeroCompra)
        {

            var data = _compraRepository.PaginacionCompras(null, null, null, 1000, fechaInicial, fechaFinal, comprobante, proveedor, vendedor, numeroCompra);

            return await RenderPdfAsync("Views/CrearPDF/ComprasPDF.cshtml", data);
        }
        public async Task<IActionResult> GenerarPdfVentasClinica(string fechaInicial, string fechaFinal, int numeroVenta,
            string comprobante, int formaPago, string origenVenta)
        {
            var lista = _ventaRepository.PaginacionVentasClinica(null, null, null, 25, fechaInicial, fechaFinal, numeroVenta, comprobante, formaPago, origenVenta);
            return await RenderPdfAsync("Views/CrearPDF/VentasPDF.cshtml", lista);
        }

        public async Task<IActionResult> GenerarPdfInventarioProductos(int? tipoProductoId, int? grupoTerapeuticoId, int? bodegaId, int? sucursalId, int? ambienteId)
        {
            var data = _productosService.GetInventarioBySp(tipoProductoId, grupoTerapeuticoId, bodegaId, sucursalId, ambienteId);
            InventarioViewModel viewModel = new InventarioViewModel()
            {
                ProductosInventario = data,
                Precios = _precioRepository.GetList().ToList()
            };
            return await RenderPdfAsync("Views/CrearPDF/InventarioProductosPdf.cshtml", viewModel);
        }

        public async Task<IActionResult> GenerarPDFAdmisionPacientes(int? id)
        {
            if (id == null) return StatusCode(400);

            var cita = _citasRepository.GetCita((int)id);

            if (cita == null) return StatusCode(404);

            var serviciosAgregados = new List<CitaServicioAgregadoViewModel>();
            var serviciosBd = _citasRepository.GetServiciosCita(cita.Id);
            if (serviciosBd != null)
            {
                foreach (var servicio in serviciosBd)
                {
                    var servicioAgregado = new CitaServicioAgregadoViewModel
                    {
                        Id = servicio.Id,
                        ServicioId = servicio.ServicioId,
                        ServicioDuracionHoras = servicio.Servicio != null
                        ? servicio.Servicio.DuracionHoras ?? 0
                        : 0,
                        Cantidad = servicio.Cantidad ?? 1,
                        ServicioNombre = servicio.Servicio.NombreServicio,
                        ServicioDuracionMinutos = servicio.Servicio != null
                        ? servicio.Servicio.DuracionMinutos ?? 15
                        : 15,
                        Nuevo = false,
                        PrecioValor = servicio.PrecioValor ?? 0,
                        PrecioValorCubiertoSeguro = servicio.PrecioValorCubiertoSeguro,
                        PrecioValorCopago = servicio.PrecioValorCopago,
                        PrecioNombre = servicio.Precio != null ? servicio.Precio.NombrePrecio : "N/A"
                    };
                    serviciosAgregados.Add(servicioAgregado);
                }
            }

            var examenesAgregados = new List<CitaExamenesAgregadosViewModel>();
            var ExamenesLabAgregadosCitaBd = _citasRepository.GetCita(cita.Id)
                .CitasExamenes
                .ToList();
            if (ExamenesLabAgregadosCitaBd != null)
            {
                foreach (var examen in ExamenesLabAgregadosCitaBd)
                {
                    var examenAgregado = new CitaExamenesAgregadosViewModel
                    {
                        ExamenId = examen.ExamenLabClinico.Id,
                        ExamenNombre = examen.ExamenLabClinico.NombreExamen,
                        Cantidad = 1,
                        PrecioId = (int)examen.PrecioId,
                        PrecioValor = (int)(examen.PrecioValor ?? 0),
                        PrecioValorCubiertoSeguro = examen.PrecioValorCubiertoSeguro,
                        PrecioValorCopago = examen.PrecioValorCopago,
                        Nuevo = false
                    };
                    examenesAgregados.Add(examenAgregado);
                }
            }

            var departamentoId = cita.Paciente.DepartamentoId;
            var municipioId = cita.Paciente.MunicipioId;
            var departamentosBd = _pacientesRepository.GetListDepartamentos();
            var municipiosBd = _pacientesRepository.GetListMunicipios(cita.Paciente.DepartamentoId ?? 10);
            var nombreDepartamento = departamentosBd.FirstOrDefault(d => d.Id == departamentoId)?.NombreDepartamento;
            var nombreMunicipio = municipiosBd.FirstOrDefault(m => m.Id == municipioId)?.NombreMunicipio;


            var model = new CitaViewModel()
            {
                CitaId = cita.Id,
                PacienteId = cita.PacienteId,
                FechaHora = (DateTime)cita.FechaInicio,
                PacienteNombre = cita.Paciente.Nombre,
                DepartamentoId = cita.Paciente.DepartamentoId,
                MunicipioId = cita.Paciente.MunicipioId,
                CategoriaHabitacionId = cita.CategoriaHabitacionId,
                HabitacionId = cita.HabitacionId,
                nombrePacienteSeleccionado = cita.Paciente.Nombre,
                FechaNacimiento = cita.Paciente.FechaNacimiento,
                Direccion = cita.Paciente.Direccion,
                PacienteEdad = cita.Paciente.Edad,
                EstadoCita = cita.EstadoCita,
                CitaTipoAtencion = cita.CitaTipoAtencion,
                NombreEncargado = cita.NombreEncargado,
                DPIEncargado = cita.DPIEncargado,
                NivelPrioridadCita = cita.NivelPrioridadCita,
                dpiPacienteSeleccionado = cita.Paciente.Dpi,
                SexoId = cita.Paciente.SexoId,
                Telefono = cita.Paciente.Telefono,
                no_IGGS = cita.Paciente.no_IGGS,
                EtniaPaciente = cita.Paciente.EtniaPaciente,
                OrigenPaciente = cita.Paciente.OrigenPaciente,

                AcompananteNombre = cita.AcompananteNombre,
                AcompananteRelacion = cita.AcompananteRelacion,
                AcompananteFechaNacimiento = cita.AcompananteFechaNacimiento,
                AcompananteEdad = cita.AcompananteEdad,
                AcompananteDPI = cita.AcompananteDPI,
                AcompananteDireccion = cita.AcompananteDireccion,
                AcompananteTelefono = cita.AcompananteTelefono,
                AcompananteCorreo = cita.AcompananteCorreo,
                AcompananteOcupacion = cita.AcompananteOcupacion,
                AcompananteEmpresa = cita.AcompananteEmpresa,
                AcompananteTelefonoEmpresa = cita.AcompananteTelefonoEmpresa,
                AcompananteDireccionEmpresa = cita.AcompananteDireccionEmpresa,
                AcompananteTipoIdentificacion = cita.AcompananteTipoIdentificacion,
                AcompananteFechaIngreso = cita.AcompananteFechaIngreso,
                AcompananteAntiguedad = cita.AcompananteAntiguedad,

                DepartamentoNombre = nombreDepartamento,
                MunicipioNombre = nombreMunicipio,

                ResponsableNit = cita.ResponsableNit,
                ResponsableNombre = cita.ResponsableNombre,
                ResponsableDireccion = cita.ResponsableDireccion,
                ResponsableCorreo = cita.ResponsableCorreo,
                ResponsableTelefono = cita.ResponsableTelefono,
                ResponsableDPI = cita.ResponsableDPI,
                ResponsablePasaporte = cita.ResponsablePasaporte,

                // Nuevos campos - Datos del padre
                NombrePadre = cita.NombrePadre,
                FechaNacimientoPadre = cita.FechaNacimientoPadre,
                EdadPadre = cita.EdadPadre,
                DPIPadre = cita.DPIPadre,
                DireccionPadre = cita.DireccionPadre,
                TelefonoPadre = cita.TelefonoPadre,
                CorreoPadre = cita.CorreoPadre,
                OcupacionPadre = cita.OcupacionPadre,
                EmpresaPadre = cita.EmpresaPadre,
                TelefonoEmpresaPadre = cita.TelefonoEmpresaPadre,
                DireccionEmpresaPadre = cita.DireccionEmpresaPadre,

                // Nuevos campos - Datos de la madre
                NombreMadre = cita.NombreMadre,
                FechaNacimientoMadre = cita.FechaNacimientoMadre,
                EdadMadre = cita.EdadMadre,
                DPIMadre = cita.DPIMadre,
                DireccionMadre = cita.DireccionMadre,
                TelefonoMadre = cita.TelefonoMadre,
                CorreoMadre = cita.CorreoMadre,
                OcupacionMadre = cita.OcupacionMadre,
                EmpresaMadre = cita.EmpresaMadre,
                TelefonoEmpresaMadre = cita.TelefonoEmpresaMadre,
                DireccionEmpresaMadre = cita.DireccionEmpresaMadre,

                EspecialidadId = cita.EspecialidadId,
                SucursalId = cita.SucursalId,
                EmpleadoId = cita.EmpleadoId,
                NumeroTurno = cita.NumeroTurno,
                Motivo = cita.Motivo,

                Servicios = serviciosAgregados,
                Examenes = examenesAgregados,

                CodigoCita = cita.CodigoDeCita,
                CodigoAutorizacion = cita.CodigoAutorizacion,
                Dia = ((DateTime)cita.FechaInicio).ToString("dd/MM/yyyy"),
                Hora = ((DateTime)cita.FechaInicio).ToString("HH:mm"),
            };

            // var options = new System.Text.Json.JsonSerializerOptions
            // {
            //     WriteIndented = true
            // };
            // string modelJson = System.Text.Json.JsonSerializer.Serialize(model, options);
            // Console.WriteLine("Contenido del view model:");
            // Console.WriteLine(modelJson);

            model.Init(_citasRepository, _pacientesRepository, _empleadoRepository, _servicioRepository, _sucursalRepository);
            return await RenderPdfAsync("Views/CrearPDF/AdmisionPacientePDF.cshtml", model);
        }

        // Método para convertir URL de imagen a Base64
        private async Task<string> ConvertImageUrlToBase64(HttpContext httpContext, string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                // Console.WriteLine("La URL de la imagen es nula o vacía.");
                return string.Empty;
            }

            try
            {
                // Verificar si la URL es relativa
                if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                {
                    // Construir la URL base dinámica desde HttpContext
                    var request = httpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";

                    // Concatenar para construir la URL absoluta
                    imageUrl = new Uri(new Uri(baseUrl), imageUrl).ToString();
                }

                // Usar HttpClient con un HttpClientHandler personalizado para deshabilitar la validación SSL
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    // Deshabilitar validación SSL (NO SE DEBE USAR EN PRODUCCIÓN)
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        // Console.WriteLine($"Descargando imagen desde: {imageUrl}");
                        var imageBytes = await client.GetByteArrayAsync(imageUrl);
                        return Convert.ToBase64String(imageBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error al convertir la imagen {imageUrl} a Base64: {ex.Message}");
                return string.Empty;
            }
        }

        // public async Task<IActionResult> GenerarPDFConsentimientoHospi(int idPaciente, int idHabitacion, string idHospi = null, int ConsultaId = 0, int CitaId = 0)
        // {

        //     var consentimiento = _consentimientoHospiService.GetConsentimientoByPacienteAndHabitacion(idPaciente, idHabitacion);
        //     // consentimiento = _consentimientoHospiService.GetConsentimientoByPacienteHabitacionAndHospitalizacion(idPaciente, idHabitacion, idHospi);

        //     string firmaPacienteBase64 = await ConvertImageUrlToBase64(HttpContext, consentimiento?.URLFirmaPaciente ?? "");
        //     string firmaResponsableBase64 = await ConvertImageUrlToBase64(HttpContext, consentimiento?.URLFirmaResponsable ?? "");
        //     string firmaNotariaBase64 = await ConvertImageUrlToBase64(HttpContext, consentimiento?.URLFirmaNotaria ?? "");
        //     string firmaRepresentanteBase64 = await ConvertImageUrlToBase64(HttpContext, consentimiento?.URLFirmaRepresentanteNaranjo ?? "");

        //     var model = new ConsentimientoHospiVM()
        //     {
        //         // Relación con Paciente
        //         PacienteId = consentimiento?.PacienteId ?? 0, // Si consentimiento es null, asignamos 0
        //         NombrePaciente = consentimiento?.NombrePaciente ?? "", // Si consentimiento es null, asignamos cadena vacía

        //         // Relación con Habitacion
        //         HabitacionId = consentimiento?.HabitacionId ?? 0, // Si consentimiento es null, asignamos 0
        //         NumeroHabitacion = consentimiento?.NumeroHabitacion ?? "", // Si consentimiento es null, asignamos cadena vacía

        //         // Datos de la Hospitalización
        //         HospitalizacionId = consentimiento?.HospitalizacionId ?? "", // Si consentimiento es null, asignamos cadena vacía

        //         // Datos del Paciente
        //         HoraIngreso = consentimiento?.HoraIngreso ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         NumeroPaciente = consentimiento?.NumeroPaciente ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         NombreCompleto = consentimiento?.NombreCompleto ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         EstadoCivil = consentimiento?.EstadoCivil ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         DPI = consentimiento?.DPI ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         FechaNacimiento = consentimiento?.FechaNacimiento ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         Edad = consentimiento?.Edad ?? "", // Calculado automáticamente en el front
        //         Nacionalidad = consentimiento?.Nacionalidad ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         Direccion = consentimiento?.Direccion ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         Celular = consentimiento?.Celular ?? "", // Si celular es null, asignamos cadena vacia
        //         Email = consentimiento?.Email ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         TipoSangre = consentimiento?.TipoSangre ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         Genero = consentimiento?.Genero ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         Religion = consentimiento?.Religion ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         Ocupacion = consentimiento?.Ocupacion ?? "", // Si consentimiento es null, asignamos cadena vacía

        //         // Información del seguro médico
        //         PoseeSeguroMedico = consentimiento?.PoseeSeguroMedico ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         Aseguradora = consentimiento?.Aseguradora ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         TipoPoliza = consentimiento?.TipoPoliza ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         NombreEmpresa = consentimiento?.NombreEmpresa ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         FormularioPreAutorizacion = consentimiento?.FormularioPreAutorizacion ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         TratamientoMedico = consentimiento?.TratamientoMedico ?? "", // Si consentimiento es null, asignamos cadena vacía

        //         // Datos del Responsable de la Cuenta
        //         NombreResponsable = consentimiento?.NombreResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         DPIResponsable = consentimiento?.DPIResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         EdadResponsable = consentimiento?.EdadResponsable ?? "", // No se especifica de donde obtener este valor
        //         DireccionResponsable = consentimiento?.DireccionResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         CelularResponsable = consentimiento?.CelularResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         EmailResponsable = consentimiento?.EmailResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         NITResponsable = consentimiento?.NITResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         NombreFacturacion = consentimiento?.NombreFacturacion ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         NacionalidadResponsable = consentimiento?.NacionalidadResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         OcupacionResponsable = consentimiento?.OcupacionResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía

        //         // Contacto de Emergencia
        //         NombreContactoEmergencia = consentimiento?.NombreContactoEmergencia ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         CelularContactoEmergencia = consentimiento?.CelularContactoEmergencia ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         ParentescoContactoEmergencia = consentimiento?.ParentescoContactoEmergencia ?? "", // Si consentimiento es null, asignamos cadena vacía

        //         // Información Adicional
        //         HospitalProporcionoMedico = consentimiento?.HospitalProporcionoMedico ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         MedicoAfiliado = consentimiento?.MedicoAfiliado ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         NombreMedicoTratante = consentimiento?.NombreMedicoTratante ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         RecetaMedica = consentimiento?.RecetaMedica ?? "", // Si consentimiento es null, asignamos cadena vacía

        //         // Firmas y nombres de quienes firman
        //         NombreNotaria = consentimiento?.NombreNotaria ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         NombreRepresentanteNarajo = consentimiento?.NombreRepresentanteNarajo ?? "", // Si consentimiento es null, asignamos cadena vacía
        //         URLFirmaPaciente = firmaPacienteBase64, // Si consentimiento es null, asignamos cadena vacía
        //         URLFirmaResponsable = firmaResponsableBase64, // Si consentimiento es null, asignamos cadena vacía
        //         URLFirmaNotaria = firmaNotariaBase64, // Si consentimiento es null, asignamos cadena vacía
        //         URLFirmaRepresentanteNaranjo = firmaRepresentanteBase64 // Si consentimiento es null, asignamos cadena vacía
        //     };

        //     // Console.WriteLine($"Generando PDF para PacienteId: {idPaciente}, HospitalizacionId: {idHabitacion}");
        //     return await RenderPdfAsync("Views/CrearPDF/ConsentimientoHospiPDF.cshtml", model);
        // }

        public async Task<IActionResult> GenerarPDFConsentimientoHospi(int idPaciente, int idHabitacion, string idHospi = null, int? citaId = null)
        {
            ConsentimientoHospiVM consentimiento = null;

            if (!string.IsNullOrEmpty(idHospi) && idHospi != "0")
            {
                consentimiento = _consentimientoHospiService.GetConsentimientoByPacienteHabitacionAndHospitalizacion(idPaciente, idHabitacion, idHospi);
            }

            for (int intento = 0; intento < 5 && consentimiento == null; intento++)
            {
                consentimiento = _consentimientoHospiService.GetConsentimientoByPacienteAndHabitacion(idPaciente, idHabitacion);
                if (consentimiento != null) break;
                await Task.Delay(500);
            }

            if (consentimiento == null && !string.IsNullOrEmpty(idHospi) && idHospi != "0")
            {
                consentimiento = _consentimientoHospiService.GetConsentimientoByPacienteAndHospitalizacion(idPaciente, idHospi);
            }

            if (consentimiento == null)
            {
                consentimiento = _consentimientoHospiService.GetLatestConsentimientoByPaciente(idPaciente);
            }

            if (consentimiento == null)
            {
                return BadRequest($"No se encontró el consentimiento para paciente {idPaciente} y habitación {idHabitacion}. Intente generar el PDF nuevamente.");
            }

            // --- NUEVO MÉTODO CORRECTO PARA OBTENER LAS FIRMAS DESDE LA CARPETA LOCAL ---
            string directorio = ContentRoot;


            string ObtenerFirmaBase64Local(string rutaFirma)
            {
                if (string.IsNullOrEmpty(rutaFirma)) return "";

                string rutaNormalizada = rutaFirma.Replace("\\", "/");

                // Quitar cualquier combinación de "../" o "./" del inicio
                // para que la ruta siempre quede dentro de wwwroot
                while (rutaNormalizada.StartsWith("../") || rutaNormalizada.StartsWith("./") ||
                       rutaNormalizada.StartsWith("/..") || rutaNormalizada.StartsWith("/."))
                    rutaNormalizada = rutaNormalizada.TrimStart('.').TrimStart('/');

                rutaNormalizada = rutaNormalizada.TrimStart('/');

                string rutaFinal = Path.Combine(directorio, "wwwroot", rutaNormalizada);


                if (System.IO.File.Exists(rutaFinal))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(rutaFinal);
                    return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
                }

                return "";
            }

            // Convertimos todas las firmas existentes usando la lectura local
            string firmaPacienteBase64 = ObtenerFirmaBase64Local(consentimiento?.URLFirmaPaciente);
            string firmaResponsableBase64 = ObtenerFirmaBase64Local(consentimiento?.URLFirmaResponsable);
            string firmaNotariaBase64 = ObtenerFirmaBase64Local(consentimiento?.URLFirmaNotaria);
            string firmaRepresentanteBase64 = ObtenerFirmaBase64Local(consentimiento?.URLFirmaRepresentanteNaranjo);

            // --- 1. LÓGICA PARA BUSCAR DATOS Y FIRMA DEL MÉDICO ---
            string nombreMedico = consentimiento?.NombreMedicoTratante ?? "";
            string especialidadMedico = "";
            string colegiadoMedico = "";
            string firmaMedicoBase64 = "";
            string procedimientoProgramado = "";

            int citaIdResuelto = 0;

            // 1. CitaId desde el query param de la URL
            if (citaId.HasValue && citaId.Value > 0)
            {
                citaIdResuelto = citaId.Value;
            }
            // 2. CitaId guardado en el consentimiento
            else if (consentimiento?.CitaId != null && consentimiento.CitaId.Value > 0)
            {
                citaIdResuelto = consentimiento.CitaId.Value;
            }
            // 3. Buscar via Consulta vinculada a la hospitalización
            else if (!string.IsNullOrEmpty(consentimiento?.HospitalizacionId)
                     && consentimiento.HospitalizacionId != "No se especifica"
                     && int.TryParse(consentimiento.HospitalizacionId, out int hospiIdInt))
            {
                var consulta = _context.Consultas
                    .FirstOrDefault(c => c.HospitalizacionId == hospiIdInt);
                if (consulta?.CitasId != null && consulta.CitasId.Value > 0)
                    citaIdResuelto = consulta.CitasId.Value;
            }

            // 4. Fallback: cita más reciente del paciente
            if (citaIdResuelto == 0)
            {
                var citaReciente = _context.Set<Citas>()
                    .Where(c => c.PacienteId == idPaciente)
                    .OrderByDescending(c => c.Id)
                    .Select(c => new { c.Id })
                    .FirstOrDefault();
                if (citaReciente != null)
                    citaIdResuelto = citaReciente.Id;
            }


            if (citaIdResuelto > 0)
            {
                var cita = _citasRepository.GetCita(citaIdResuelto);
                if (cita != null)
                {
                    procedimientoProgramado = cita.Procedimiento ?? "";

                    if (cita.Empleado != null)
                    {
                        nombreMedico = cita.Empleado.NombreYApellidos;
                        colegiadoMedico = cita.Empleado.Colegiado ?? "";
                        especialidadMedico = cita.EspecialidadText != "N/A" ? cita.EspecialidadText : "";
                        firmaMedicoBase64 = ObtenerFirmaBase64Local(cita.Empleado.FirmaEmpleado);
                    }
                }
            }
            // ----------------------------------------------------
            string dpiPaciente = consentimiento?.DPI ?? "";
            var pacienteRecord = _pacientesRepository.Get(idPaciente);
            if (string.IsNullOrWhiteSpace(dpiPaciente))
                dpiPaciente = pacienteRecord?.Dpi ?? "";

            string dpiResponsable = consentimiento?.DPIResponsable ?? "";

            var tipoSangrePdf = HospitalizacionPacienteVitalsHelper.FirstNonEmptyValue(
                consentimiento?.TipoSangre,
                pacienteRecord?.TipoDeSangre);
            var nombreCompletoPdf = HospitalizacionPacienteVitalsHelper.FirstNonEmptyValue(
                consentimiento?.NombreCompleto,
                pacienteRecord?.Nombre);
            var nombrePacientePdf = HospitalizacionPacienteVitalsHelper.FirstNonEmptyValue(
                consentimiento?.NombrePaciente,
                consentimiento?.NombreCompleto,
                pacienteRecord?.Nombre);

            var model = new ConsentimientoHospiVM()
            {
                // Relación con Paciente
                PacienteId = consentimiento?.PacienteId ?? idPaciente,
                NombrePaciente = nombrePacientePdf ?? "",
                HabitacionId = consentimiento?.HabitacionId ?? idHabitacion,
                NumeroHabitacion = consentimiento?.NumeroHabitacion ?? "",
                HospitalizacionId = consentimiento?.HospitalizacionId ?? "",

                // Datos del Paciente
                HoraIngreso = consentimiento?.HoraIngreso ?? "",
                NumeroPaciente = consentimiento?.NumeroPaciente ?? "",
                NombreCompleto = nombreCompletoPdf ?? "",
                EstadoCivil = consentimiento?.EstadoCivil ?? "",
                DPI = dpiPaciente,
                FechaNacimiento = consentimiento?.FechaNacimiento ?? "",
                Edad = consentimiento?.Edad ?? "",
                Nacionalidad = consentimiento?.Nacionalidad ?? "",
                Direccion = consentimiento?.Direccion ?? "",
                Celular = consentimiento?.Celular ?? "",
                Email = consentimiento?.Email ?? "",
                TipoSangre = tipoSangrePdf ?? "",
                Genero = consentimiento?.Genero ?? "",
                Religion = consentimiento?.Religion ?? "",
                Ocupacion = consentimiento?.Ocupacion ?? "",

                // Información del seguro médico
                PoseeSeguroMedico = consentimiento?.PoseeSeguroMedico ?? "",
                Aseguradora = consentimiento?.Aseguradora ?? "",
                TipoPoliza = consentimiento?.TipoPoliza ?? "",
                NombreEmpresa = consentimiento?.NombreEmpresa ?? "",
                FormularioPreAutorizacion = consentimiento?.FormularioPreAutorizacion ?? "",
                TratamientoMedico = consentimiento?.TratamientoMedico ?? "",

                // Datos del Responsable
                NombreResponsable = consentimiento?.NombreResponsable ?? "",
                DPIResponsable = dpiResponsable,
                EdadResponsable = consentimiento?.EdadResponsable ?? "",
                DireccionResponsable = consentimiento?.DireccionResponsable ?? "",
                CelularResponsable = consentimiento?.CelularResponsable ?? "",
                EmailResponsable = consentimiento?.EmailResponsable ?? "",
                NITResponsable = consentimiento?.NITResponsable ?? "",
                NombreFacturacion = consentimiento?.NombreFacturacion ?? "",
                NacionalidadResponsable = consentimiento?.NacionalidadResponsable ?? "",
                OcupacionResponsable = consentimiento?.OcupacionResponsable ?? "",

                // Contacto de Emergencia
                ContactosEmergencia = consentimiento?.ContactosEmergencia?
                .Select(c => new ContactoEmergenciaVM
                {
                    Nombre = c.Nombre,
                    Telefono = c.Telefono,
                    Parentesco = c.Parentesco
                }).ToList() ?? new List<ContactoEmergenciaVM>(),

                // Información Adicional
                HospitalProporcionoMedico = consentimiento?.HospitalProporcionoMedico ?? "",
                MedicoAfiliado = consentimiento?.MedicoAfiliado ?? "",
                RecetaMedica = consentimiento?.RecetaMedica ?? "",

                // --- 2. PASAR LOS NUEVOS DATOS AL MODELO ---
                NombreMedicoTratante = nombreMedico,
                EspecialidadMedico = especialidadMedico,
                ColegiadoMedico = colegiadoMedico,
                UrlFirmaMedico = firmaMedicoBase64,
                ProcedimientoProgramado = procedimientoProgramado,

                CitaId = consentimiento?.CitaId,
                ConsultaId = consentimiento?.ConsultaId,

                // Firmas Generales
                NombreNotaria = consentimiento?.NombreNotaria ?? "",
                NombreRepresentanteNarajo = consentimiento?.NombreRepresentanteNarajo ?? "",
                URLFirmaPaciente = firmaPacienteBase64,
                URLFirmaResponsable = firmaResponsableBase64,
                URLFirmaNotaria = firmaNotariaBase64,
                URLFirmaRepresentanteNaranjo = firmaRepresentanteBase64,
                FirmaPacienteBase64 = firmaPacienteBase64,
                FirmaResponsableBase64 = firmaResponsableBase64,
                FirmaNotariaBase64 = firmaNotariaBase64,
                FirmaRepresentanteBase64 = firmaRepresentanteBase64,
                FirmaMedicoBase64 = firmaMedicoBase64
            };

            // return await RenderPdfAsync("Views/CrearPDF/ConsentimientoHospiPDF.cshtml", model);
            // 1. Leer la variable Cliente desde el appsettings.json
            string clienteApp = _configuration["Cliente"];

            // 2. Definir la ruta de la vista por defecto
            string rutaVistaPDF = "Views/CrearPDF/ConsentimientoHospiPDF_G.cshtml";

            // 3. Evaluar el cliente y asignar la vista correspondiente
            switch (clienteApp)
            {
                case "SS":
                    rutaVistaPDF = "Views/CrearPDF/ConsentimientoHospiPDF_SS.cshtml";
                    break;
                case "HSC":
                    rutaVistaPDF = "Views/CrearPDF/ConsentimientoHospiPDF_HSC.cshtml";
                    break;
                    // El default ya está cubierto por la asignación inicial (_G.cshtml)
            }

            // 4. Retornar el PDF usando la vista dinámica
            if (string.Equals(clienteApp, "SS", StringComparison.OrdinalIgnoreCase))
            {
                PdfReportHelper.CompletarFirmasConsentimientoSerSalud(
                    model,
                    firmaMedicoBase64,
                    directorio,
                    _configuration["EstablecimientoImagenFirma"],
                    _configuration["EstablecimientoFirmaRepresentante"],
                    string.Equals(_webHostEnvironment.EnvironmentName, Environments.Development, StringComparison.OrdinalIgnoreCase));
            }

            return await RenderPdfAsync(rutaVistaPDF, model);
        }

        public async Task<IActionResult> GenerarPDFConsentimientoHospiHospi(int idPaciente, int idHabitacion, string idHospi = null)
        {

            var consentimiento = _consentimientoHospiService.GetConsentimientoByPacienteHabitacionAndHospitalizacion(idPaciente, idHabitacion, idHospi);

            string firmaPacienteBase64 = await ConvertImageUrlToBase64(HttpContext, consentimiento?.URLFirmaPaciente ?? "");
            string firmaResponsableBase64 = await ConvertImageUrlToBase64(HttpContext, consentimiento?.URLFirmaResponsable ?? "");
            string firmaNotariaBase64 = await ConvertImageUrlToBase64(HttpContext, consentimiento?.URLFirmaNotaria ?? "");
            string firmaRepresentanteBase64 = await ConvertImageUrlToBase64(HttpContext, consentimiento?.URLFirmaRepresentanteNaranjo ?? "");

            var model = new ConsentimientoHospiVM()
            {
                // Relación con Paciente
                PacienteId = consentimiento?.PacienteId ?? 0, // Si consentimiento es null, asignamos 0
                NombrePaciente = consentimiento?.NombrePaciente ?? "", // Si consentimiento es null, asignamos cadena vacía

                // Relación con Habitacion
                HabitacionId = consentimiento?.HabitacionId ?? 0, // Si consentimiento es null, asignamos 0
                NumeroHabitacion = consentimiento?.NumeroHabitacion ?? "", // Si consentimiento es null, asignamos cadena vacía

                // Datos de la Hospitalización
                HospitalizacionId = consentimiento?.HospitalizacionId ?? "", // Si consentimiento es null, asignamos cadena vacía

                // Datos del Paciente
                HoraIngreso = consentimiento?.HoraIngreso ?? "", // Si consentimiento es null, asignamos cadena vacía
                NumeroPaciente = consentimiento?.NumeroPaciente ?? "", // Si consentimiento es null, asignamos cadena vacía
                NombreCompleto = consentimiento?.NombreCompleto ?? "", // Si consentimiento es null, asignamos cadena vacía
                EstadoCivil = consentimiento?.EstadoCivil ?? "", // Si consentimiento es null, asignamos cadena vacía
                DPI = consentimiento?.DPI ?? "", // Si consentimiento es null, asignamos cadena vacía
                FechaNacimiento = consentimiento?.FechaNacimiento ?? "", // Si consentimiento es null, asignamos cadena vacía
                Edad = consentimiento?.Edad ?? "", // Calculado automáticamente en el front
                Nacionalidad = consentimiento?.Nacionalidad ?? "", // Si consentimiento es null, asignamos cadena vacía
                Direccion = consentimiento?.Direccion ?? "", // Si consentimiento es null, asignamos cadena vacía
                Celular = consentimiento?.Celular ?? "", // Si celular es null, asignamos cadena vacia
                Email = consentimiento?.Email ?? "", // Si consentimiento es null, asignamos cadena vacía
                TipoSangre = consentimiento?.TipoSangre ?? "", // Si consentimiento es null, asignamos cadena vacía
                Genero = consentimiento?.Genero ?? "", // Si consentimiento es null, asignamos cadena vacía
                Religion = consentimiento?.Religion ?? "", // Si consentimiento es null, asignamos cadena vacía
                Ocupacion = consentimiento?.Ocupacion ?? "", // Si consentimiento es null, asignamos cadena vacía

                // Información del seguro médico
                PoseeSeguroMedico = consentimiento?.PoseeSeguroMedico ?? "", // Si consentimiento es null, asignamos cadena vacía
                Aseguradora = consentimiento?.Aseguradora ?? "", // Si consentimiento es null, asignamos cadena vacía
                TipoPoliza = consentimiento?.TipoPoliza ?? "", // Si consentimiento es null, asignamos cadena vacía
                NombreEmpresa = consentimiento?.NombreEmpresa ?? "", // Si consentimiento es null, asignamos cadena vacía
                FormularioPreAutorizacion = consentimiento?.FormularioPreAutorizacion ?? "", // Si consentimiento es null, asignamos cadena vacía
                TratamientoMedico = consentimiento?.TratamientoMedico ?? "", // Si consentimiento es null, asignamos cadena vacía

                // Datos del Responsable de la Cuenta
                NombreResponsable = consentimiento?.NombreResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
                DPIResponsable = consentimiento?.DPIResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
                EdadResponsable = consentimiento?.EdadResponsable ?? "", // No se especifica de donde obtener este valor
                DireccionResponsable = consentimiento?.DireccionResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
                CelularResponsable = consentimiento?.CelularResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
                EmailResponsable = consentimiento?.EmailResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
                NITResponsable = consentimiento?.NITResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
                NombreFacturacion = consentimiento?.NombreFacturacion ?? "", // Si consentimiento es null, asignamos cadena vacía
                NacionalidadResponsable = consentimiento?.NacionalidadResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía
                OcupacionResponsable = consentimiento?.OcupacionResponsable ?? "", // Si consentimiento es null, asignamos cadena vacía

                // Contacto de Emergencia
                ContactosEmergencia = consentimiento?.ContactosEmergencia?
                .Select(c => new ContactoEmergenciaVM
                {
                    Nombre = c.Nombre,
                    Telefono = c.Telefono,
                    Parentesco = c.Parentesco
                }).ToList() ?? new List<ContactoEmergenciaVM>(),

                // Información Adicional
                HospitalProporcionoMedico = consentimiento?.HospitalProporcionoMedico ?? "", // Si consentimiento es null, asignamos cadena vacía
                MedicoAfiliado = consentimiento?.MedicoAfiliado ?? "", // Si consentimiento es null, asignamos cadena vacía
                NombreMedicoTratante = consentimiento?.NombreMedicoTratante ?? "", // Si consentimiento es null, asignamos cadena vacía
                RecetaMedica = consentimiento?.RecetaMedica ?? "", // Si consentimiento es null, asignamos cadena vacía

                // Firmas y nombres de quienes firman
                NombreNotaria = consentimiento?.NombreNotaria ?? "", // Si consentimiento es null, asignamos cadena vacía
                NombreRepresentanteNarajo = consentimiento?.NombreRepresentanteNarajo ?? "", // Si consentimiento es null, asignamos cadena vacía
                URLFirmaPaciente = firmaPacienteBase64, // Si consentimiento es null, asignamos cadena vacía
                URLFirmaResponsable = firmaResponsableBase64, // Si consentimiento es null, asignamos cadena vacía
                URLFirmaNotaria = firmaNotariaBase64, // Si consentimiento es null, asignamos cadena vacía
                URLFirmaRepresentanteNaranjo = firmaRepresentanteBase64 // Si consentimiento es null, asignamos cadena vacía
            };

            // Console.WriteLine($"Generando PDF para PacienteId: {idPaciente}, HospitalizacionId: {idHabitacion}");
            return await RenderPdfAsync("Views/CrearPDF/ConsentimientoHospiPDF.cshtml", model);
        }

        // [HttpPost]
        // public async Task<IActionResult> ProcesarEstadoCuenta([FromBody] EstadoCuentaViewModel estadoCuenta)
        // {
        //     // 1. Validar el modelo
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest("Datos del estado de cuenta inválidos.");
        //     }

        //     // 2. Imprimir el Estado de Cuenta completo en consola como un objeto JSON
        //     string estadoCuentaJson = JsonConvert.SerializeObject(estadoCuenta, Formatting.Indented);
        //     // Console.WriteLine("=== Datos del Estado de Cuenta ===");
        //     // Console.WriteLine(estadoCuentaJson);

        //     // Opcional: Devolver una vista vacía o un resultado simple
        //     return await RenderPdfAsync("Views/CrearPDF/EstadoDeCuentaHopsPDF.cshtml", estadoCuenta);
        // }



        [HttpPost]
        public async Task<IActionResult> ProcesarEstadoCuenta()
        {
            try
            {
                Request.EnableBuffering();

                string rawBody;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    rawBody = await reader.ReadToEndAsync();
                }
                Request.Body.Position = 0;

                if (string.IsNullOrWhiteSpace(rawBody))
                    return BadRequest("No se recibieron datos.");

                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<EstadoCuentaViewModel>(rawBody);
                if (model == null)
                    return BadRequest("Formato inválido.");

                int hospitalizacionId = model.Hospitalizacion?.HospitalizacionId ?? 0;
                if (hospitalizacionId == 0 && Request.Query.TryGetValue("id", out var idQuery))
                    int.TryParse(idQuery, out hospitalizacionId);

                if (hospitalizacionId == 0 && !string.IsNullOrWhiteSpace(rawBody))
                {
                    var jObj = Newtonsoft.Json.Linq.JObject.Parse(rawBody);
                    var hospToken = jObj.SelectToken("Hospitalizacion.HospitalizacionId");
                    if (hospToken != null)
                        hospitalizacionId = hospToken.Value<int>();
                }

                if (model.Hospitalizacion == null)
                    model.Hospitalizacion = new HospitalizacionEstadoCuentaViewModel();
                model.Hospitalizacion.HospitalizacionId = hospitalizacionId;

                if (hospitalizacionId > 0)
                {
                    // Paquetes
                    var paquetesAsignados = _hospitalizacionRepository.GetPaquetesAgregados(hospitalizacionId);
                    model.Paquetes = paquetesAsignados?
                        .Where(p => p.PaqueteHospitalizacion != null)
                        .Select(p => new PaqueteViewModel
                        {
                            Fecha = p.FechaHora,
                            Nombre = p.PaqueteHospitalizacion.NombrePaquete ?? "Sin nombre",
                            Cantidad = 1,
                            PrecioUnitario = p.PaqueteHospitalizacion.Precio ?? 0,
                            Subtotal = (p.PaqueteHospitalizacion.Precio ?? 0) * 1
                        }).ToList() ?? new List<PaqueteViewModel>();

                    // ── Gastos administrativos ──────────────────────────────────────
                    var gastosAdmin = _hospitalizacionRepository.GetGastosAdministrativos(hospitalizacionId);
                    model.GastosAdministrativos = gastosAdmin?
                        .Select(g => new GastoAdministrativoViewModel
                        {
                            Fecha = g.FechaHora,
                            PorcentajeAplicado = g.PorcentajeAplicado,
                            Monto = g.Monto
                        }).ToList() ?? new List<GastoAdministrativoViewModel>();
                    // ───────────────────────────────────────────────────────────────
                }
                else
                {
                    model.Paquetes = new List<PaqueteViewModel>();
                    model.GastosAdministrativos = new List<GastoAdministrativoViewModel>(); // ← nuevo
                }

                return await RenderPdfAsync("Views/CrearPDF/EstadoDeCuentaHopsPDF.cshtml", model);
            }
            catch
            {
                return StatusCode(500, "Error interno al generar el estado de cuenta.");
            }
        }



        public async Task<IActionResult> ProcesarEstadoCuentaVentaHospital(int id)
        {
            try
            {
                Console.WriteLine($"[EstadoCuenta] ▶ VentaId={id}");
                if (id <= 0) return BadRequest("Id de venta inválido.");

                var venta = _ventaRepository.Get(id);
                if (venta == null) return NotFound($"No se encontró la venta Id={id}.");

                // 1. Obtener HospitalizacionId (misma lógica que ya funciona)
                int hospitalizacionId = 0;
                var detalleCaja = _context.DetalleCajas
                    .Include(d => d.CuentaPorCobrar)
                        .ThenInclude(c => c.DetallesCuentaPorCobrar)
                    .FirstOrDefault(d => d.VentaId == venta.Id && d.CuentaPorCobrarId != null);
                if (detalleCaja?.CuentaPorCobrar?.DetallesCuentaPorCobrar != null)
                {
                    hospitalizacionId = detalleCaja.CuentaPorCobrar.DetallesCuentaPorCobrar
                        .Where(d => d.HospitalizacionId != null)
                        .Max(d => (int?)d.HospitalizacionId) ?? 0;
                }
                if (hospitalizacionId <= 0 && venta.PacienteId != null)
                {
                    var cuentaPaciente = _context.CuentasPorCobrar
                        .Include(c => c.DetallesCuentaPorCobrar)
                        .Where(c => c.PacienteId == venta.PacienteId)
                        .OrderByDescending(c => c.Id)
                        .FirstOrDefault();
                    if (cuentaPaciente?.DetallesCuentaPorCobrar != null)
                    {
                        hospitalizacionId = cuentaPaciente.DetallesCuentaPorCobrar
                            .Where(d => d.HospitalizacionId != null)
                            .Max(d => (int?)d.HospitalizacionId) ?? 0;
                    }
                }
                if (hospitalizacionId <= 0 && venta.PacienteId != null)
                {
                    hospitalizacionId = _context.Hospitalizaciones
                        .Where(h => h.PacienteId == venta.PacienteId)
                        .OrderByDescending(h => h.Id)
                        .Select(h => h.Id)
                        .FirstOrDefault();
                }
                if (hospitalizacionId <= 0)
                    return BadRequest($"No se encontró hospitalización para la venta {id}.");

                // 2. Cargar hospitalización con todas las relaciones necesarias (incluyendo detalles de exámenes)
                var hospitalizacion = _context.Hospitalizaciones
                    .Include(h => h.Paciente)
                    .Include(h => h.Habitacion)
                    .Include(h => h.CategoriaHabitacionTarifa)
                        .ThenInclude(ct => ct.CategoriaHabitacion)
                    .Include(h => h.HospitalizacionesProductos)
                        .ThenInclude(p => p.Producto)
                    .Include(h => h.HospitalizacionesProductos)
                        .ThenInclude(p => p.HospitalizacionesProductosAplicaciones)
                    .Include(h => h.HospitalizacionesServicios)
                        .ThenInclude(s => s.Servicio)
                    .Include(h => h.HospitalizacionesExamenes)
                        .ThenInclude(e => e.Examen)
                            .ThenInclude(ex => ex.DetalleExamenes)   // ← Importante para obtener el precio
                    .Include(h => h.HospitalizacionesPaquetesHospitalizacion)
                        .ThenInclude(pq => pq.PaqueteHospitalizacion)
                    .FirstOrDefault(h => h.Id == hospitalizacionId);

                if (hospitalizacion == null)
                    return NotFound($"No se encontró la hospitalización con Id {hospitalizacionId}.");

                hospitalizacion.HospitalizacionInsumosDirectos = _context.HospitalizacionInsumosDirectos
                    .Include(i => i.Producto)
                    .Include(i => i.Aplicaciones)
                    .Where(i => i.HospitalizacionId == hospitalizacionId && !i.Eliminado)
                    .ToList();

                // 3. Construir modelo (usando alias fmModels)
                var model = new fmModels.EstadoCuentaViewModel();

                // ----- Paciente -----
                var paciente = hospitalizacion.Paciente ?? new Paciente();
                model.Paciente = new fmModels.PacienteEstadoCuentaViewModel
                {
                    Nombre = paciente.Nombre ?? "-",
                    Telefono = paciente.Telefono ?? "-",
                    Celular = paciente.Celular ?? "-",
                    Direccion = paciente.Direccion ?? "-",
                    FechaNacimiento = paciente.FechaNacimiento?.ToString("dd/MM/yyyy") ?? "-",
                    Edad = CalcularEdadEnAños(paciente.FechaNacimiento),
                    Sexo = paciente.SexoId == 1 ? "Masculino" : "Femenino",
                    Nit = venta.Nit ?? "-",
                    Dpi = paciente.Dpi ?? "-"
                };

                // ----- Responsable -----
                model.Responsable = new fmModels.ResponsableViewModel
                {
                    Nombre = venta.Clientes?.Nombre ?? venta.Nombres ?? "-",
                    Nit = venta.Nit ?? "-",
                    Direccion = venta.Direccion ?? "-",
                    // Correo = venta.Clientes?.Correo ?? "-"
                };

                // ----- Habitación -----
                var habitacionEnt = hospitalizacion.Habitacion;
                var categoriaTarifa = hospitalizacion.CategoriaHabitacionTarifa;
                model.Habitacion = new fmModels.HabitacionViewModel
                {
                    Numero = habitacionEnt?.NombreNumeroHabitacion ?? "-",   // Ajusta si es NumeroHabitacion
                    Categoria = categoriaTarifa?.CategoriaHabitacion?.NombreCategoria ?? "-",
                    Tarifa = categoriaTarifa?.ValorTarifa ?? 0m,
                    Seguro = ""
                };

                // ----- Productos -----
                var productosList = new List<fmModels.ProductoViewModel>();

                // 3.1 Estadía
                if (categoriaTarifa != null)
                {
                    var fechaIni = hospitalizacion.FechaInicio.Date;
                    var fechaFin = DateTime.Now.Date;
                    var noches = (fechaIni == fechaFin) ? 1 : (fechaFin - fechaIni).Days;
                    if (noches < 1) noches = 1;
                    var tarifa = Convert.ToDecimal(categoriaTarifa.ValorTarifa);
                    var subtotal = Math.Round(tarifa * noches, 2);

                    productosList.Add(new fmModels.ProductoViewModel
                    {
                        Fecha = hospitalizacion.FechaInicio.ToString("dd/MM/yyyy"),
                        Tipo = "Habitación",
                        Item = $"Estadía en habitación ({noches} noche(s))",
                        Cantidad = noches,
                        PrecioUnitario = tarifa,
                        Subtotal = subtotal,
                        DescPct = 0m,
                        Cargo = 0m
                    });
                }

                // 3.2 Medicamentos / insumos aplicados (clasificados por tipo de producto)
                if (hospitalizacion.HospitalizacionesProductos != null)
                {
                    foreach (var prod in hospitalizacion.HospitalizacionesProductos)
                    {
                        var aplicadas = HospitalizacionCargosHelper
                            .AplicacionesVigentes(prod.HospitalizacionesProductosAplicaciones)
                            .ToList();
                        if (!aplicadas.Any()) continue;

                        var cantidad = aplicadas.Sum(a => a.Cantidad);
                        if (cantidad <= 0) continue;

                        var subtotal = Math.Round(cantidad * prod.PrecioValor, 2);
                        productosList.Add(new fmModels.ProductoViewModel
                        {
                            Fecha = HospitalizacionCargosHelper.FormatearFechaAplicacionProducto(
                                aplicadas,
                                prod.FechaHoraAplicacionManual),
                            Tipo = HospitalizacionCargosHelper.ObtenerTipoProductoCuenta(prod.Producto?.TipoProductoId),
                            Item = prod.Producto?.NombreProducto ?? "Producto",
                            Cantidad = (int)cantidad,
                            PrecioUnitario = Math.Round(prod.PrecioValor, 2),
                            Subtotal = subtotal,
                            DescPct = 0m,
                            Cargo = 0m
                        });
                    }
                }

                // 3.2b Control de insumos directos (misma clasificación por tipo de producto)
                if (hospitalizacion.HospitalizacionInsumosDirectos != null)
                {
                    foreach (var insumo in hospitalizacion.HospitalizacionInsumosDirectos.Where(i => !i.Eliminado))
                    {
                        var aplicadas = HospitalizacionCargosHelper
                            .AplicacionesVigentesInsumoDirecto(insumo.Aplicaciones)
                            .ToList();
                        if (!aplicadas.Any()) continue;

                        var cantidad = aplicadas.Sum(a => a.Cantidad);
                        if (cantidad <= 0) continue;

                        var subtotal = Math.Round(aplicadas.Sum(a => a.Cantidad * insumo.PrecioValor), 2);

                        productosList.Add(new fmModels.ProductoViewModel
                        {
                            Fecha = HospitalizacionCargosHelper.FormatearFechaAplicacionInsumoDirecto(
                                aplicadas,
                                insumo.FechaHoraAplicacionManual),
                            Tipo = HospitalizacionCargosHelper.ObtenerTipoProductoCuenta(insumo.Producto?.TipoProductoId),
                            Item = insumo.Producto?.NombreProducto ?? "Producto",
                            Cantidad = (int)cantidad,
                            PrecioUnitario = Math.Round(insumo.PrecioValor, 2),
                            Subtotal = subtotal,
                            DescPct = 0m,
                            Cargo = 0m
                        });
                    }
                }

                // 3.3 Servicios clínicos
                if (hospitalizacion.HospitalizacionesServicios != null)
                {
                    foreach (var svc in hospitalizacion.HospitalizacionesServicios.Where(s => !s.Eliminado))
                    {
                        decimal precioSvc = ObtenerPrecioUnitarioHospitalizacionServicio(svc);
                        var subtotal = Math.Round(svc.Cantidad * precioSvc, 2);
                        productosList.Add(new fmModels.ProductoViewModel
                        {
                            Fecha = svc.FechaAplicacionFormateada ?? DateTime.Now.ToString("dd/MM/yyyy"),
                            Tipo = "Servicios",
                            Item = svc.Servicio?.NombreServicio ?? "Servicio clínico",
                            Cantidad = (int)Math.Round(svc.Cantidad, 0),
                            PrecioUnitario = Math.Round(precioSvc, 2),
                            Subtotal = subtotal,
                            DescPct = 0m,
                            Cargo = 0m
                        });
                    }
                }

                // 3.4 Exámenes (CORREGIDO según tus modelos)
                if (hospitalizacion.HospitalizacionesExamenes != null)
                {
                    foreach (var exam in hospitalizacion.HospitalizacionesExamenes.Where(e => !e.Eliminado))
                    {
                        var examenRel = exam.Examen;  // ← Navegación correcta
                        if (examenRel == null) continue;

                        var nombreExamen = examenRel.NumeroOrden ?? "Examen de laboratorio";

                        // Calcular precio sumando los PrecioValor de los detalles del examen
                        var precio = examenRel.DetalleExamenes?.Sum(d => d.PrecioValor) ?? 0m;

                        // Cantidad: si tu entidad HospitalizacionExamen tiene una propiedad Cantidad, úsala.
                        // Si no, asumimos 1 (porque cada registro representa una solicitud del examen completo)
                        var cantidad = 1;  // Cambia a exam.Cantidad si existe
                                           // Si existe la propiedad Cantidad en HospitalizacionExamen, descomenta:
                                           // var cantidad = exam.Cantidad;

                        var subtotal = precio * cantidad;

                        // Fecha: probablemente existe exam.FechaHora
                        string fechaStr = exam.FechaHora.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString("dd/MM/yyyy");

                        productosList.Add(new fmModels.ProductoViewModel
                        {
                            Fecha = fechaStr,
                            Tipo = "Exámenes",
                            Item = nombreExamen,
                            Cantidad = cantidad,
                            PrecioUnitario = precio,
                            Subtotal = subtotal,
                            DescPct = 0m,
                            Cargo = 0m
                        });
                    }
                }

                model.Productos = productosList;

                // ----- Honorarios (desde venta) -----
                var honorariosList = new List<fmModels.HonorarioViewModel>();
                if (venta.DetalleVenta != null)
                {
                    foreach (var item in venta.DetalleVenta.Where(d => d.Servicio != null && d.Servicio.NombreServicio.Contains("Honorario")))
                    {
                        honorariosList.Add(new fmModels.HonorarioViewModel
                        {
                            Medico = item.Servicio.NombreServicio,
                            Monto = item.Total
                        });
                    }
                }
                model.Honorarios = honorariosList;

                // ----- Exclusiones (vacío) -----
                model.Exclusiones = new List<fmModels.ExclusionViewModel>();

                // ----- Paquetes -----
                var paquetesAsignados = hospitalizacion.HospitalizacionesPaquetesHospitalizacion?
                    .Where(p => p.PaqueteHospitalizacion != null && !p.Eliminado)
                    .Select(p => new fmModels.PaqueteViewModel
                    {
                        Fecha = p.FechaHora,
                        Nombre = p.PaqueteHospitalizacion.NombrePaquete ?? "Sin nombre",
                        Cantidad = 1,
                        PrecioUnitario = p.PaqueteHospitalizacion.Precio ?? 0m,
                        Subtotal = (p.PaqueteHospitalizacion.Precio ?? 0m) * 1
                    }).ToList() ?? new List<fmModels.PaqueteViewModel>();
                model.Paquetes = paquetesAsignados;

                // ----- Gastos administrativos -----
                var gastosAdmin = _hospitalizacionRepository.GetGastosAdministrativos(hospitalizacionId);
                model.GastosAdministrativos = gastosAdmin?.Select(g => new GastoAdministrativoViewModel
                {
                    Fecha = g.FechaHora,
                    PorcentajeAplicado = g.PorcentajeAplicado,
                    Monto = g.Monto
                }).ToList() ?? new List<GastoAdministrativoViewModel>();

                // ----- Hospitalizacion (extra) -----
                model.Hospitalizacion = new fmModels.HospitalizacionEstadoCuentaViewModel
                {
                    FechaInicioHospitalizacion = hospitalizacion.FechaInicio.ToString("dd/MM/yyyy"),
                    MedicoResponsable = "",
                    NombreSeguro = "",
                    HospitalizacionId = hospitalizacionId
                };

                // ----- Totales y Pagos -----
                decimal totalCuenta = productosList.Sum(p => p.Subtotal) +
                                      model.Paquetes.Sum(p => p.Subtotal) +
                                      model.GastosAdministrativos.Sum(g => g.Monto) +
                                      honorariosList.Sum(h => h.Monto);

                decimal pagoPaciente = venta.Pagos?.Where(p => p.FormaPago?.NombreFormaPago != "Seguro").Sum(p => p.Monto) ?? 0;
                decimal pagoSeguro = venta.Pagos?.Where(p => p.FormaPago?.NombreFormaPago == "Seguro").Sum(p => p.Monto) ?? 0;
                if (pagoPaciente == 0 && pagoSeguro == 0 && totalCuenta > 0) pagoPaciente = totalCuenta;

                model.Pagos = new fmModels.PagosViewModel
                {
                    TotalCuenta = totalCuenta,
                    TotalAseguradora = 0,
                    TotalNoElegibles = 0,
                    Deducibles = 0,
                    Coaseguro = 0,
                    Copago = 0,
                    IVA = 0,
                    PagoPaciente = pagoPaciente,
                    PagoSeguro = pagoSeguro
                };

                model.DescuentoGlobal = 0;

                return await RenderPdfAsync("Views/CrearPDF/EstadoDeCuentaHopsPDF.cshtml", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EstadoCuenta] ERROR: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Calcula la edad en años a partir de una fecha de nacimiento.
        /// </summary>
        private int CalcularEdadEnAños(DateTime? fechaNacimiento)
        {
            if (!fechaNacimiento.HasValue) return 0;
            var today = DateTime.Today;
            var age = today.Year - fechaNacimiento.Value.Year;
            if (fechaNacimiento.Value.Date > today.AddYears(-age)) age--;
            return age;
        }



        public async Task<IActionResult> generarPDFRequisicionDespacho(int requisicionId)
        {
            var requisicion = await _requisicionRepository.GetByIdAsync(requisicionId);
            if (requisicion == null)
                return NotFound($"No se encontró la requisición con Id = {requisicionId}.");

            // var user = await _userManager.GetUserAsync(HttpContext.User);
            // if (user == null)
            //     return Unauthorized();

            // Empleado empleado = null;
            // if (user.EmpleadoId.HasValue)
            // {
            //     // IEmpleado.Get(...) es síncrono en este proyecto
            //     empleado = _empleadoRepository.Get(user.EmpleadoId.Value, includeRelatedEntities: true);
            // }

            // var nombreSolicitante = !string.IsNullOrWhiteSpace(empleado?.NombreYApellidos)
            //     ? empleado.NombreYApellidos
            //     : requisicion.SolicitanteNombre;

            string emailBusqueda = requisicion.SolicitanteNombre?.Trim();

            var usuario = _userRepository.Get(emailBusqueda);

            if (usuario == null)
            {
                throw new Exception($"No se encontró el usuario con el email: '{emailBusqueda}'");
            }

            var empleado = _empleadoRepository.Get(usuario.EmpleadoId.Value);


            var model = new RequisicionViewModel
            {
                RequisicionId = requisicion.Id,
                Direccion = requisicion.Direccion,
                Departamento = requisicion.Departamento,
                UnidadSeccion = requisicion.UnidadSeccion,
                Otros = requisicion.Otros,
                NumeroRequisicion = requisicion.NumeroRequisicion,
                NombreBodega = requisicion.BodegaOrigen?.NombreBodega,
                FechaSolicitud = requisicion.FechaSolicitud,
                // ✅ Aquí está el cambio solicitado:
                NombreSolicitante = empleado.NombreYApellidos,
                JefaturaNombre = requisicion.JefaturaNombre,
                GerenciaNombre = requisicion.GerenciaNombre,
                NombreEncargadoAlmacen = requisicion.EncargadoAlmacenNombre,
            };


            string directorio = ContentRoot;

            string rutaRelativa = requisicion.FirmaSolicitante.TrimStart('/');
            string rutaFinal = Path.Combine(directorio, "wwwroot", rutaRelativa);
            byte[] bytes = System.IO.File.ReadAllBytes(rutaFinal);
            model.FirmaSolicitante = $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";

            if (!string.IsNullOrEmpty(requisicion.VOBOJefatura))
            {
                string rutaRelativa2 = requisicion.VOBOJefatura.TrimStart('/');
                string rutaFinal2 = Path.Combine(directorio, "wwwroot", rutaRelativa2);

                if (System.IO.File.Exists(rutaFinal2))
                {
                    byte[] bytes2 = System.IO.File.ReadAllBytes(rutaFinal2);
                    model.VOBOJefatura = $"data:image/png;base64,{Convert.ToBase64String(bytes2)}";
                }
            }

            if (!string.IsNullOrEmpty(requisicion.AutorizacionGerencia))
            {
                string rutaRelativa3 = requisicion.AutorizacionGerencia.TrimStart('/');
                string rutaFinal3 = Path.Combine(directorio, "wwwroot", rutaRelativa3);

                if (System.IO.File.Exists(rutaFinal3))
                {
                    byte[] bytes3 = System.IO.File.ReadAllBytes(rutaFinal3);
                    model.AutorizacionGerencia = $"data:image/png;base64,{Convert.ToBase64String(bytes3)}";
                }
            }

            if (!string.IsNullOrEmpty(requisicion.AutorizacionAlmacen))
            {
                string rutaRelativa4 = requisicion.AutorizacionAlmacen.TrimStart('/');
                string rutaFinal4 = Path.Combine(directorio, "wwwroot", rutaRelativa4);

                if (System.IO.File.Exists(rutaFinal4))
                {
                    byte[] bytes4 = System.IO.File.ReadAllBytes(rutaFinal4);
                    model.AutorizacionAlmacen = $"data:image/png;base64,{Convert.ToBase64String(bytes4)}";
                }
            }

            if (requisicion.RequisionDetalles != null && requisicion.RequisionDetalles.Any())
            {
                model.Productos = requisicion.RequisionDetalles.Select(d =>
                {
                    var inv = d.ProductoInventario;
                    var prod = inv?.Producto;

                    return new TrasladoProductoViewModel
                    {
                        ProductoNombre = prod != null
                            ? $"{prod.CodigoReferencia} / {prod.NombreProducto}"
                            : "-",

                        UnidadMedidaVentaNombre = inv?.UnidadMedidaVenta != null
                            ? inv.UnidadMedidaVenta.Nombre
                            : "-",

                        CantidadTrasladada = d.CantidadSolicitada,

                        CantidadDespachada = d.CantidadDespachada
                    };
                }).ToList();
            }

            var options = new ConvertOptions
            {
                PageOrientation = Wkhtmltopdf.NetCore.Options.Orientation.Portrait
            };

            _generatePdf.SetConvertOptions(options);

            ViewData["NombreBodega"] = requisicion.BodegaOrigen?.NombreBodega;

            return await RenderPdfAsync("Views/CrearPDF/generarPDFRequisicionDespacho.cshtml", model);
        }




        [HttpGet]
        public async Task<IActionResult> generarPDFDevolucion(int id)
        {
            if (id <= 0) return BadRequest("ID de devolución no válido.");

            var devolucion = await _devolucionRepository.GetByIdAsync(id);

            if (devolucion == null) return NotFound($"No se encontró la devolución con ID: {id}");

            string directorio = ContentRoot;

            if (!string.IsNullOrEmpty(devolucion.FirmaSolicitante))
            {
                string rutaRelativa = devolucion.FirmaSolicitante.TrimStart('/');
                string rutaFinal = Path.Combine(directorio, "wwwroot", rutaRelativa);
                if (System.IO.File.Exists(rutaFinal))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(rutaFinal);
                    devolucion.FirmaSolicitante = $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
                }
            }

            if (!string.IsNullOrEmpty(devolucion.FirmaAutorizacion))
            {
                string rutaRelativa = devolucion.FirmaAutorizacion.TrimStart('/');
                string rutaFinal = Path.Combine(directorio, "wwwroot", rutaRelativa);
                if (System.IO.File.Exists(rutaFinal))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(rutaFinal);
                    devolucion.FirmaAutorizacion = $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
                }
            }

            decimal montoTotal = 0;
            if (devolucion.Detalles != null)
            {
                foreach (var detalle in devolucion.Detalles)
                {
                    var precio = detalle.ProductoInventario?.PrecioCosto ?? 0;
                    montoTotal += detalle.CantidadDevuelta * precio;
                }
            }

            ViewBag.MontoTotal = montoTotal;

            var options = new ConvertOptions
            {
                PageOrientation = Wkhtmltopdf.NetCore.Options.Orientation.Portrait,
                PageMargins = new Wkhtmltopdf.NetCore.Options.Margins(10, 10, 10, 10)
            };
            _generatePdf.SetConvertOptions(options);

            return await RenderPdfAsync("Views/CrearPDF/generarPDFDevolucion.cshtml", devolucion);
        }

        public async Task<IActionResult> GenerarPDFHospiReports(int idPaciente, int idHabitacion, string idHospi = null, int report = 0, int? citaId = null)
        {
            var consentimiento = _consentimientoHospiService.GetConsentimientoByPacienteHabitacionAndHospitalizacion(idPaciente, idHabitacion, idHospi);

            // --- Función local para leer imágenes a Base64 ---
            string directorio = ContentRoot;
            string ObtenerFirmaBase64Local(string rutaFirma)
            {
                if (string.IsNullOrEmpty(rutaFirma)) return "";
                string rutaNormalizada = rutaFirma.Replace("\\", "/").TrimStart('/');

                while (rutaNormalizada.StartsWith("../") || rutaNormalizada.StartsWith("./"))
                    rutaNormalizada = rutaNormalizada.Substring(rutaNormalizada.IndexOf('/') + 1);

                string rutaFinal = Path.Combine(directorio, "wwwroot", rutaNormalizada);
                if (System.IO.File.Exists(rutaFinal))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(rutaFinal);
                    return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
                }
                return "";
            }

            // Firmas generales
            string firmaPacienteBase64 = ObtenerFirmaBase64Local(consentimiento?.URLFirmaPaciente);
            string firmaResponsableBase64 = ObtenerFirmaBase64Local(consentimiento?.URLFirmaResponsable);
            string firmaNotariaBase64 = ObtenerFirmaBase64Local(consentimiento?.URLFirmaNotaria);
            string firmaRepresentanteBase64 = ObtenerFirmaBase64Local(consentimiento?.URLFirmaRepresentanteNaranjo);

            // --- LÓGICA UNIFICADA PARA BUSCAR DATOS Y FIRMAS DEL MÉDICO (BASADO EN ConsentimientoHospi) ---
            string nombreMedico = consentimiento?.NombreMedicoTratante ?? "";
            string especialidadMedico = "";
            string colegiadoMedico = "";
            string firmaMedicoBase64 = "";
            string procedimientoProgramado = "";

            string fechaAdmision = consentimiento?.HoraIngreso ?? DateTime.Now.ToString("dd/MM/yyyy");
            string nombre1erAyudante = "", col1erAyudante = "";
            string nombre2doAyudante = "", col2doAyudante = "";
            string nombreAnestesista = "", colAnestesista = "";
            string firmaAnestesistaBase64 = "";

            int citaIdResuelto = 0;

            // 1. CitaId desde el query param de la URL
            if (citaId.HasValue && citaId.Value > 0)
            {
                citaIdResuelto = citaId.Value;
            }
            // 2. CitaId guardado en el consentimiento
            else if (consentimiento?.CitaId != null && consentimiento.CitaId.Value > 0)
            {
                citaIdResuelto = consentimiento.CitaId.Value;
            }
            // 3. Buscar via Consulta vinculada a la hospitalización
            else if (!string.IsNullOrEmpty(idHospi) && idHospi != "No se especifica" && int.TryParse(idHospi, out int hospiIdInt))
            {
                var consulta = _context.Consultas.FirstOrDefault(c => c.HospitalizacionId == hospiIdInt);
                if (consulta?.CitasId != null) citaIdResuelto = consulta.CitasId.Value;
            }
            // 4. Fallback: cita más reciente del paciente
            if (citaIdResuelto == 0)
            {
                var citaReciente = _context.Citass.Where(c => c.PacienteId == idPaciente).OrderByDescending(c => c.Id).FirstOrDefault();
                if (citaReciente != null) citaIdResuelto = citaReciente.Id;
            }

            if (citaIdResuelto > 0)
            {
                var cita = _citasRepository.GetCita(citaIdResuelto);
                if (cita != null)
                {
                    fechaAdmision = cita.FechaInicio?.ToString("dd/MM/yyyy") ?? fechaAdmision;
                    procedimientoProgramado = cita.Procedimiento ?? "";

                    // Médico Principal
                    if (cita.Empleado != null)
                    {
                        nombreMedico = cita.Empleado.NombreYApellidos;
                        colegiadoMedico = cita.Empleado.Colegiado ?? "";
                        especialidadMedico = cita.EspecialidadText != "N/A" ? cita.EspecialidadText : "";
                        firmaMedicoBase64 = ObtenerFirmaBase64Local(cita.Empleado.FirmaEmpleado);
                    }

                    void ResolverMedicoSecundario(string datoCita, out string nombre, out string colegiado, out string firma)
                    {
                        PdfReportHelper.ResolverMedicoSecundarioCita(
                            datoCita,
                            _empleadoRepository,
                            out nombre,
                            out colegiado,
                            out firma,
                            directorio);
                    }

                    ResolverMedicoSecundario(cita.PrimerAyudante, out nombre1erAyudante, out col1erAyudante, out _);
                    ResolverMedicoSecundario(cita.SegundoAyudante, out nombre2doAyudante, out col2doAyudante, out _);

                    if (cita.AnestesistaId is > 0)
                    {
                        var anestesista = _empleadoRepository.Get(cita.AnestesistaId.Value, false);
                        if (anestesista != null)
                        {
                            nombreAnestesista = anestesista.NombreYApellidos;
                            colAnestesista = anestesista.Colegiado ?? "";
                            firmaAnestesistaBase64 = ObtenerFirmaBase64Local(anestesista.FirmaEmpleado);
                        }
                    }
                    else
                    {
                        ResolverMedicoSecundario(cita.Anestesista, out nombreAnestesista, out colAnestesista, out firmaAnestesistaBase64);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(nombreAnestesista)
                && !string.IsNullOrEmpty(idHospi)
                && int.TryParse(idHospi, out int hospiIdAnest))
            {
                var anestNota = _hospitalizacionRepository.GetNotasOperatoriasByHospitalizacion(hospiIdAnest)
                    ?.OrderByDescending(n => n.FechaRegistro)
                    .FirstOrDefault()
                    ?.Anestesista;
                if (!string.IsNullOrWhiteSpace(anestNota)
                    && !anestNota.Equals("Anestesiólogo asignado", StringComparison.OrdinalIgnoreCase))
                {
                    PdfReportHelper.ResolverMedicoSecundarioCita(
                        anestNota,
                        _empleadoRepository,
                        out nombreAnestesista,
                        out colAnestesista,
                        out firmaAnestesistaBase64,
                        directorio);
                }
            }

            var medicamentosNoControlados = new List<MedicamentoNoControladoPdfVM>();
            if (!string.IsNullOrEmpty(idHospi) && int.TryParse(idHospi, out int hospiIdParaMeds))
            {
                var historialMedsConsent = _medicamentoNoControladoRepository
                    .GetHistorialByHospitalizacion(hospiIdParaMeds)
                    .Where(m => !m.Eliminado)
                    .ToList();
                var ultimaFechaMeds = historialMedsConsent.FirstOrDefault()?.FechaRegistro;
                medicamentosNoControlados = historialMedsConsent
                    .Where(m => ultimaFechaMeds == null || m.FechaRegistro == ultimaFechaMeds)
                    .Select(m => new MedicamentoNoControladoPdfVM
                    {
                        ProductoNombre = m.ProductoNombre,
                        UnidadesIniciales = m.UnidadesIniciales,
                        UnidadesExtra = m.UnidadesExtra,
                        Utilizado = m.Utilizado,
                        Descartado = m.Descartado,
                        Retornadas = m.Retornadas,
                        FechaProcedimiento = m.FechaProcedimiento ?? m.FechaRegistro
                    })
                    .ToList();
            }

            string nombrePaciente = consentimiento?.NombrePaciente;
            if (string.IsNullOrEmpty(nombrePaciente) && idPaciente > 0)
            {
                var pacienteDb = _context.Pacientes.FirstOrDefault(p => p.Id == idPaciente);
                if (pacienteDb != null)
                {
                    nombrePaciente = pacienteDb.Nombre;
                }
            }

            var model = new ConsentimientoHospiVM()
            {
                // Datos del Paciente y Hospitalización
                PacienteId = consentimiento?.PacienteId ?? idPaciente,
                NombrePaciente = nombrePaciente ?? "",
                HabitacionId = consentimiento?.HabitacionId ?? 0,
                NumeroHabitacion = consentimiento?.NumeroHabitacion ?? "",
                HospitalizacionId = consentimiento?.HospitalizacionId ?? "",
                HoraIngreso = consentimiento?.HoraIngreso ?? "",
                NumeroPaciente = consentimiento?.NumeroPaciente ?? "",
                NombreCompleto = consentimiento?.NombreCompleto ?? "",
                EstadoCivil = consentimiento?.EstadoCivil ?? "",
                DPI = consentimiento?.DPI ?? "",
                FechaNacimiento = consentimiento?.FechaNacimiento ?? "",
                Edad = consentimiento?.Edad ?? "",
                Nacionalidad = consentimiento?.Nacionalidad ?? "",
                Direccion = consentimiento?.Direccion ?? "",
                Celular = consentimiento?.Celular ?? "",
                Email = consentimiento?.Email ?? "",
                TipoSangre = consentimiento?.TipoSangre ?? "",
                Genero = consentimiento?.Genero ?? "",
                Religion = consentimiento?.Religion ?? "",
                Ocupacion = consentimiento?.Ocupacion ?? "",

                // Seguro y Responsable
                PoseeSeguroMedico = consentimiento?.PoseeSeguroMedico ?? "",
                Aseguradora = consentimiento?.Aseguradora ?? "",
                TipoPoliza = consentimiento?.TipoPoliza ?? "",
                NombreEmpresa = consentimiento?.NombreEmpresa ?? "",
                FormularioPreAutorizacion = consentimiento?.FormularioPreAutorizacion ?? "",
                TratamientoMedico = consentimiento?.TratamientoMedico ?? "",
                NombreResponsable = consentimiento?.NombreResponsable ?? "",
                DPIResponsable = consentimiento?.DPIResponsable ?? "",
                EdadResponsable = consentimiento?.EdadResponsable ?? "",
                DireccionResponsable = consentimiento?.DireccionResponsable ?? "",
                CelularResponsable = consentimiento?.CelularResponsable ?? "",
                EmailResponsable = consentimiento?.EmailResponsable ?? "",
                NITResponsable = consentimiento?.NITResponsable ?? "",
                NombreFacturacion = consentimiento?.NombreFacturacion ?? "",
                NacionalidadResponsable = consentimiento?.NacionalidadResponsable ?? "",
                OcupacionResponsable = consentimiento?.OcupacionResponsable ?? "",

                // Emergencia y Médicos
                ContactosEmergencia = consentimiento?.ContactosEmergencia?
                    .Select(c => new ContactoEmergenciaVM
                    {
                        Nombre = c.Nombre,
                        Telefono = c.Telefono,
                        Parentesco = c.Parentesco
                    }).ToList() ?? new List<ContactoEmergenciaVM>(),
                HospitalProporcionoMedico = consentimiento?.HospitalProporcionoMedico ?? "",
                MedicoAfiliado = consentimiento?.MedicoAfiliado ?? "",
                RecetaMedica = consentimiento?.RecetaMedica ?? "",

                // --- DATOS MÉDICOS UNIFICADOS ---
                NombreMedicoTratante = nombreMedico,
                EspecialidadMedico = especialidadMedico,
                ColegiadoMedico = colegiadoMedico,
                UrlFirmaMedico = firmaMedicoBase64,
                ProcedimientoProgramado = procedimientoProgramado,
                FechaAdmision = fechaAdmision,
                NombrePrimerAyudante = nombre1erAyudante,
                ColegiadoPrimerAyudante = col1erAyudante,
                NombreSegundoAyudante = nombre2doAyudante,
                ColegiadoSegundoAyudante = col2doAyudante,
                NombreAnestesista = nombreAnestesista,
                ColegiadoAnestesista = colAnestesista,
                UrlFirmaAnestesista = firmaAnestesistaBase64,

                // Firmas
                NombreNotaria = consentimiento?.NombreNotaria ?? "",
                NombreRepresentanteNarajo = consentimiento?.NombreRepresentanteNarajo ?? "",
                URLFirmaPaciente = firmaPacienteBase64,
                URLFirmaResponsable = firmaResponsableBase64,
                URLFirmaNotaria = firmaNotariaBase64,
                URLFirmaRepresentanteNaranjo = firmaRepresentanteBase64,

                MedicamentosNoControlados = medicamentosNoControlados,
            };

            PdfReportHelper.AsignarFirmaFarmaciaMedicamentos(
                model,
                _configuration["EstablecimientoImagenFirma"],
                directorio);

            if (report == 2 || report == 3)
            {
                Hospitalizacion hospitalizacionReport = null;
                string anestNotaReport = null;
                if (!string.IsNullOrEmpty(idHospi) && int.TryParse(idHospi, out int hospiIdReport3))
                {
                    hospitalizacionReport = _hospitalizacionRepository.Get(hospiIdReport3, includeConsultas: true);
                    anestNotaReport = _hospitalizacionRepository.GetNotasOperatoriasByHospitalizacion(hospiIdReport3)
                        ?.OrderByDescending(n => n.FechaRegistro)
                        .FirstOrDefault()
                        ?.Anestesista;
                }

                PdfReportHelper.CompletarConsentimientoPdfVm(
                    model,
                    hospitalizacionReport,
                    _citasRepository,
                    _empleadoRepository,
                    directorio,
                    citaIdResuelto > 0 ? citaIdResuelto : null,
                    medicamentosNoControlados,
                    anestNotaReport);
            }

            // Selección de Vista
            if (report == 1) return await RenderPdfAsync("Views/CrearPDF/AutorizacionSalaOperacionesHospiPDF.cshtml", model);
            else if (report == 2) return await RenderPdfAsync("Views/CrearPDF/MedicoAnestesiaHospiPDF.cshtml", model);
            else if (report == 3) return await RenderPdfAsync("Views/CrearPDF/MedicamentosContrladosHospiPDF.cshtml", model);
            else return await RenderPdfAsync("Views/CrearPDF/ConsentimientoHospiPDF.cshtml", model);
        }




        [Authorize]
        public async Task<IActionResult> ReporteVentasGeneralPDF(
            string desde, string hasta, int ambienteId = 0, int empleadoId = 0,
            string modoDetalle = "monto", bool incluirConsolidadoItems = true,
            bool incluirConsolidadoMedicos = false)
        {
            // ── 1. Parsear fechas ────────────────────────────────────────────────
            if (!DateTime.TryParse(desde, out var fechaDesde) ||
                !DateTime.TryParse(hasta, out var fechaHasta))
                return BadRequest("Formato de fecha inválido. Use yyyy-MM-dd.");

            try
            {
            var fechaHastaFin = fechaHasta.Date.AddDays(1).AddSeconds(-1);

            // modoDetalle: monto = montos por categoría (sin detalle línea a línea en cada venta)
            //              descripcion = descripción de ítems e insumos por venta
            // Los checkboxes controlan consolidados globales (categorías / médicos / especialidades)
            bool modoDescripcion = string.Equals(modoDetalle, "descripcion", StringComparison.OrdinalIgnoreCase);
            modoDetalle = modoDescripcion ? "descripcion" : "monto";

            // ── 2. Consultar ventas (sin navegaciones pesadas inicialmente) ─────
            List<Venta> ventasBd = empleadoId > 0
                ? _ventaRepository.GetListadoFechaEmpleado(fechaDesde.Date, fechaHastaFin, empleadoId)
                : _ventaRepository.GetListadoFecha(fechaDesde.Date, fechaHastaFin);

            ventasBd = ventasBd.Where(v => v.Eliminado != true).ToList();

            if (ambienteId > 0)
                ventasBd = ventasBd.Where(v => v.AmbienteId == ambienteId).ToList();

            // ── 3. Completar nombres de paciente (ventas ya traen Detalle, Pagos, Empleado) ──
            var pacienteIds = ventasBd
                .Where(v => (v.Clientes == null || string.IsNullOrWhiteSpace(v.Clientes.Nombre)) && v.PacienteId != null)
                .Select(v => v.PacienteId!.Value)
                .Distinct()
                .ToList();

            if (pacienteIds.Count > 0)
            {
                var pacientesPorId = await _context.Pacientes.AsNoTracking()
                    .Where(p => pacienteIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                foreach (var v in ventasBd)
                {
                    if ((v.Clientes == null || string.IsNullOrWhiteSpace(v.Clientes.Nombre))
                        && v.PacienteId != null
                        && pacientesPorId.TryGetValue(v.PacienteId.Value, out var paciente))
                    {
                        v.Clientes ??= new Clientes();
                        v.Clientes.Nombre = paciente.Nombre;
                        v.Clientes.Nit = v.Nit ?? "CF";
                        v.Clientes.Direccion = v.Direccion ?? "N/A";
                    }
                }
            }

            var hospitalizacionPorVenta = PrecargarHospitalizacionIdPorVentas(ventasBd);
            var costoProductoCache = PrecargarCostosProductos(ventasBd);

            var ventaIds = ventasBd.Select(v => v.Id).ToList();
            var ingresosCajaPorVenta = ventaIds.Count == 0
                ? new Dictionary<int, decimal>()
                : _context.DetalleCajas.AsNoTracking()
                    .Where(d => d.VentaId != null && ventaIds.Contains(d.VentaId.Value))
                    .GroupBy(d => d.VentaId!.Value)
                    .ToDictionary(g => g.Key, g => g.Sum(d => d.Ingreso));

            // ── 4. Nombre del empleado filtrado ──────────────────────────────────
            string empleadoNombreFiltro = null;
            if (empleadoId > 0)
                empleadoNombreFiltro = _empleadoRepository.Get(empleadoId, false)?.NombreYApellidos;

            // ── 5. Usuario que genera el reporte ─────────────────────────────────
            var userActual = await _userManager.GetUserAsync(User);
            string usuarioGenera = userActual?.UserName ?? "Sistema";

            // ── 6. Mapa de ambientes (solo los que tienen datos) ────────────────
            var mapaAmbientes = new Dictionary<int, string>
    {
        { (int)AmbienteEnum.Farmacia,    "Farmacia"    },
        { (int)AmbienteEnum.Clinica,     "Clínica"     },
        { (int)AmbienteEnum.Hospital,    "Hospital"    },
        { (int)AmbienteEnum.Laboratorio, "Laboratorio" },
    };
            if (ambienteId > 0 && mapaAmbientes.ContainsKey(ambienteId))
                mapaAmbientes = new Dictionary<int, string> { { ambienteId, mapaAmbientes[ambienteId] } };

            // ── 7. Diccionario para acumular items consolidados (con costo unitario y total) ──
            var itemsConsolidadosDict = new Dictionary<string, (decimal cantidad, decimal total, decimal costoTotal, decimal? costoUnitario)>(StringComparer.OrdinalIgnoreCase);
            var ambienteCostos = new Dictionary<int, (decimal costoTotal, decimal cantidad)>();
            var ambienteCategoriaMontos = new Dictionary<int, (decimal productos, decimal servicios, decimal examenes, decimal hospital)>();
            var montosHospitalPorEspecialidad = new Dictionary<string, (decimal productos, decimal servicios, decimal examenes, decimal hospital)>(StringComparer.OrdinalIgnoreCase);

            void AcumularMontoAmbiente(int ambienteIdItem, string categoria, decimal total)
            {
                if (ambienteIdItem <= 0 || total == 0) return;
                categoria = NormalizarCategoriaConsolidado(categoria);
                if (!ambienteCategoriaMontos.TryGetValue(ambienteIdItem, out var montos))
                    montos = (0, 0, 0, 0);

                if (categoria == "Producto")
                    montos.productos += total;
                else if (categoria == "Servicio")
                    montos.servicios += total;
                else if (categoria == "Examen")
                    montos.examenes += total;
                else
                    montos.hospital += total;

                ambienteCategoriaMontos[ambienteIdItem] = montos;
            }

            void AgregarItem(string categoria, string nombre, decimal cantidad, decimal total, decimal? costoUnitario, int ambienteIdItem)
            {
                if (string.IsNullOrWhiteSpace(nombre)) nombre = "No especificado";
                categoria = NormalizarCategoriaConsolidado(categoria);

                decimal cu = costoUnitario ?? 0;
                AcumularMontoAmbiente(ambienteIdItem, categoria, total);

                if (incluirConsolidadoItems)
                {
                    var key = $"{categoria}|{nombre}";
                    if (!itemsConsolidadosDict.TryGetValue(key, out var item))
                    {
                        item = (0, 0, 0, costoUnitario > 0 ? costoUnitario : null);
                        itemsConsolidadosDict[key] = item;
                    }
                    item.cantidad += cantidad;
                    item.total += total;
                    item.costoTotal += cantidad * cu;
                    if (costoUnitario.HasValue && costoUnitario.Value > 0 && !item.costoUnitario.HasValue)
                        item.costoUnitario = costoUnitario;
                    itemsConsolidadosDict[key] = item;
                }

                if (ambienteIdItem > 0 && cu > 0)
                {
                    if (ambienteCostos.TryGetValue(ambienteIdItem, out var ac))
                        ambienteCostos[ambienteIdItem] = (ac.costoTotal + cantidad * cu, ac.cantidad + cantidad);
                    else
                        ambienteCostos[ambienteIdItem] = (cantidad * cu, cantidad);
                }
            }

            // ── 7b. Consolidado hospital (insumos aplicados, paquetes, estadía) — independiente de ventas en caja
            bool incluirHospitalEnReporte = ambienteId == 0 || ambienteId == (int)AmbienteEnum.Hospital;
            if (incluirConsolidadoItems && incluirHospitalEnReporte)
            {
                ConsolidarHospitalizacionesEnPeriodo(
                    ventasBd, fechaDesde.Date, fechaHastaFin, AgregarItem, ambienteCostos,
                    (int)AmbienteEnum.Hospital, montosHospitalPorEspecialidad, hospitalizacionPorVenta, costoProductoCache);
            }

            // ── 8. Construir bloques por ambiente ────────────────────────────────
            var ambientesVM = new List<ReporteVentasAmbienteViewModel>();

            foreach (var kvp in mapaAmbientes)
            {
                var idAmb = kvp.Key;
                var nomAmb = kvp.Value;

                var ventasAmb = ventasBd
                    .Where(v => v.AmbienteId == idAmb)
                    .OrderBy(v => v.FechaVenta)
                    .ToList();

                bool tieneMontosHospital = idAmb == (int)AmbienteEnum.Hospital
                    && ambienteCategoriaMontos.TryGetValue(idAmb, out var mh)
                    && (mh.productos + mh.servicios + mh.examenes + mh.hospital) > 0;

                if (!ventasAmb.Any() && !tieneMontosHospital) continue;

                var items = new List<ReporteVentaItemViewModel>();

                foreach (var v in ventasAmb)
                {
                    bool esHospitalizacion = v.AmbienteId == (int)AmbienteEnum.Hospital
                                             || (v.TipoVenta?.IndexOf("hospitalizacion", StringComparison.OrdinalIgnoreCase) >= 0);

                    ReporteVentaItemViewModel item;

                    if (esHospitalizacion)
                    {
                        // ── OBTENER HOSPITALIZACION ID ──
                        int? hospitalizacionId = ObtenerHospitalizacionIdPorVenta(v, hospitalizacionPorVenta);

                        if (hospitalizacionId != null)
                        {
                            var hosp = _hospitalizacionRepository.Get(
                                hospitalizacionId.Value,
                                includeMedicamentos: true,
                                includeServicios: true,
                                includeExamenes: true,
                                includePaquetes: true,
                                includeConsultas: true,
                                includeOrdenesMedicas: false);

                            if (hosp != null)
                            {
                                // ---- Estadía ----
                                var fechaFinEstadia = DateTime.Now.Date;
                                var noches = (hosp.FechaInicio.Date == fechaFinEstadia) ? 1 : (fechaFinEstadia - hosp.FechaInicio.Date).Days;
                                if (noches < 1) noches = 1;
                                decimal tarifaBase = hosp.CategoriaHabitacionTarifa?.ValorTarifa ?? 0;
                                decimal subtotalEstadia = tarifaBase * noches;
                                var cambios = _hospitalizacionRepository.GetCambiosHabitacion(hosp.Id);
                                foreach (var cambio in cambios)
                                    subtotalEstadia += cambio.ValorTarifa * cambio.Dias;

                                // ---- Productos (medicamentos/insumos aplicados) con costo unitario ----
                                decimal subProductos = 0m, cantProductos = 0m;
                                var listadoProductos = new List<ReporteVentaDetalleItemViewModel>();
                                bool hospEnPeriodoVenta = hosp.FechaInicio <= fechaHastaFin && hosp.FechaFin >= fechaDesde.Date;
                                foreach (var prod in hosp.HospitalizacionesProductos ?? Enumerable.Empty<HospitalizacionProducto>())
                                {
                                    var aplicadas = prod.HospitalizacionesProductosAplicaciones?
                                        .Where(a => a.Aplicado && !a.Eliminado
                                            && FechaAplicacionEnRango(a, fechaDesde.Date, fechaHastaFin, hospEnPeriodoVenta, tieneVentaEnPeriodo: true))
                                        .ToList() ?? new List<HospitalizacionProductoAplicacion>();
                                    if (!aplicadas.Any()) continue;
                                    decimal cantidad = aplicadas.Sum(a => a.Cantidad);
                                    decimal subtotalProd = cantidad * prod.PrecioValor;
                                    subProductos += subtotalProd;
                                    cantProductos += cantidad;

                                    decimal? costoUnitario = ObtenerCostoUnitarioProducto(prod.ProductoId, prod.Producto, costoProductoCache);

                                    listadoProductos.Add(CrearDetalleItemVenta(
                                        prod.Producto?.NombreProducto ?? "Producto",
                                        "Producto",
                                        cantidad,
                                        prod.PrecioValor,
                                        subtotalProd,
                                        0,
                                        costoUnitario));

                                }

                                var detInsumosDirectos = ConstruirDetalleInsumosDirectosHospitalizacion(
                                    hosp.HospitalizacionInsumosDirectos,
                                    fechaDesde.Date,
                                    fechaHastaFin,
                                    hospEnPeriodoVenta,
                                    tieneVentaEnPeriodo: true,
                                    costoProductoCache);
                                subProductos += detInsumosDirectos.subProductos;
                                cantProductos += detInsumosDirectos.cantProductos;
                                listadoProductos.AddRange(detInsumosDirectos.lineas);

                                // ---- Servicios aplicados (misma lógica que facturación: !Eliminado, sin exigir Aplicado) ----
                                decimal subServicios = 0m, cantServicios = 0m;
                                decimal subExamenes = 0m, cantExamenes = 0m;
                                decimal subHospitalDesdeServicios = 0m;
                                var listadoServicios = new List<ReporteVentaDetalleItemViewModel>();
                                foreach (var svc in hosp.HospitalizacionesServicios ?? Enumerable.Empty<HospitalizacionServicio>())
                                {
                                    if (!ServicioHospitalizacionEnRango(svc, fechaDesde.Date, fechaHastaFin, hospEnPeriodoVenta, tieneVentaEnPeriodo: true))
                                        continue;

                                    decimal precioSvc = ObtenerPrecioUnitarioHospitalizacionServicio(svc);
                                    decimal subtotalSvc = Math.Round(svc.Cantidad * precioSvc, 2);
                                    string categoriaSvc = CategoriaMontosHospitalizacionServicio(svc);
                                    if (categoriaSvc == "Servicio")
                                    {
                                        subServicios += subtotalSvc;
                                        cantServicios += svc.Cantidad;
                                    }
                                    else if (categoriaSvc == "Examen")
                                    {
                                        subExamenes += subtotalSvc;
                                        cantExamenes += svc.Cantidad;
                                    }
                                    else
                                        subHospitalDesdeServicios += subtotalSvc;

                                    string categoriaDetalle = categoriaSvc == "Servicio" ? "Servicio" : categoriaSvc;
                                    listadoServicios.Add(CrearDetalleItemVenta(
                                        svc.Servicio?.NombreServicio ?? "Servicio",
                                        categoriaDetalle,
                                        svc.Cantidad,
                                        precioSvc,
                                        subtotalSvc,
                                        0,
                                        ObtenerCostoUnitarioServicio(svc.ServicioId, svc.Servicio)));
                                    if (modoDetalle == "descripcion" && categoriaSvc == "Servicio")
                                        listadoServicios.AddRange(ObtenerDetalleInsumosServicio(svc.ServicioId, svc.Cantidad));
                                }

                                // ---- Exámenes (con categoría real) ----
                                var listadoExamenes = new List<ReporteVentaDetalleItemViewModel>();
                                foreach (var examHosp in hosp.HospitalizacionesExamenes ?? Enumerable.Empty<HospitalizacionExamen>())
                                {
                                    if (examHosp.Examen?.DetalleExamenes != null)
                                    {
                                        foreach (var det in examHosp.Examen.DetalleExamenes)
                                        {
                                            string nombreCategoria = "Examen";
                                            var labClinico = det.ExamenLabClinico;
                                            if (labClinico?.CategoriaLabClinico != null)
                                                nombreCategoria = labClinico.CategoriaLabClinico.Nombre;
                                            else if (labClinico != null)
                                                nombreCategoria = "Examen";

                                            subExamenes += det.PrecioValor;
                                            cantExamenes += det.Cantidad;
                                            decimal? costoExam = ObtenerCostoUnitarioExamen(labClinico?.Id, labClinico);
                                            listadoExamenes.Add(CrearDetalleItemVenta(
                                                labClinico?.NombreExamen ?? "Examen",
                                                nombreCategoria,
                                                det.Cantidad,
                                                det.PrecioValor,
                                                det.PrecioValor,
                                                0,
                                                costoExam));

                                        }
                                    }
                                }

                                // ---- Paquetes ----
                                decimal subPaquetes = 0m;
                                var listadoPaquetes = new List<ReporteVentaDetalleItemViewModel>();
                                foreach (var paq in hosp.HospitalizacionesPaquetesHospitalizacion ?? Enumerable.Empty<HospitalizacionPaqueteHospitalizacion>())
                                {
                                    if (!paq.Eliminado && paq.PaqueteHospitalizacion != null)
                                    {
                                        decimal precioPaq = paq.PaqueteHospitalizacion.Precio ?? 0;
                                        subPaquetes += precioPaq;

                                        decimal? costoPaq = ObtenerCostoPaqueteHospitalizacion(paq.PaqueteHospitalizacion);

                                        listadoPaquetes.Add(CrearDetalleItemVenta(
                                            $"Paquete: {paq.PaqueteHospitalizacion.NombrePaquete}",
                                            "Hospital",
                                            1,
                                            precioPaq,
                                            precioPaq,
                                            0,
                                            costoPaq));
                                    }
                                }

                                // ---- Gastos administrativos ----
                                decimal subGastosAdmin = _hospitalizacionRepository.GetGastosAdministrativos(hosp.Id)?.Sum(g => g.Monto) ?? 0;

                                // ---- Honorarios ----
                                decimal subHonorarios = _context.HospitalizacionHonorarios.Where(h => h.HospitalizacionId == hosp.Id).Sum(h => h.Monto);

                                // ---- Ambulancias ----
                                decimal subAmbulancias = _context.Ambulancias.Where(a => a.HospitalizacionId == hosp.Id).Sum(a => a.Precio);

                                // ---- Total categoría Hospital ----
                                decimal subHospital = subtotalEstadia + subPaquetes + subGastosAdmin + subHonorarios + subAmbulancias + subHospitalDesdeServicios;
                                decimal totalDescuento = 0m;
                                decimal totalNeto = subProductos + subServicios + subExamenes + subHospital - totalDescuento;

                                // Detalle interno (especialidad/consolidados); insumos solo en modo descripción
                                var detalleItems = new List<ReporteVentaDetalleItemViewModel>
                                {
                                    CrearDetalleItemVenta(
                                        $"Estadía en habitación ({noches} noche(s))",
                                        "Hospital",
                                        noches,
                                        tarifaBase,
                                        subtotalEstadia)
                                };
                                foreach (var cambio in cambios)
                                    detalleItems.Add(CrearDetalleItemVenta(
                                        "Cambio de habitación",
                                        "Hospital",
                                        cambio.Dias,
                                        cambio.ValorTarifa,
                                        cambio.ValorTarifa * cambio.Dias));
                                detalleItems.AddRange(listadoProductos);
                                detalleItems.AddRange(listadoServicios);
                                detalleItems.AddRange(listadoExamenes);
                                detalleItems.AddRange(listadoPaquetes);
                                if (subGastosAdmin > 0)
                                    detalleItems.Add(CrearDetalleItemVenta(
                                        "Gastos administrativos",
                                        "Hospital",
                                        1,
                                        subGastosAdmin,
                                        subGastosAdmin));
                                if (subHonorarios > 0)
                                    detalleItems.Add(CrearDetalleItemVenta(
                                        "Honorarios médicos",
                                        "Hospital",
                                        1,
                                        subHonorarios,
                                        subHonorarios));
                                if (subAmbulancias > 0)
                                    detalleItems.Add(CrearDetalleItemVenta(
                                        "Ambulancia",
                                        "Hospital",
                                        1,
                                        subAmbulancias,
                                        subAmbulancias));

                                var medicosAsignados = ObtenerMedicosDeHospitalizacion(hosp);

                                item = new ReporteVentaItemViewModel
                                {
                                    VentaId = v.Id,
                                    FechaVenta = v.FechaVenta,
                                    NumeroComprobante = v.NoComprobante ?? v.UuidFel ?? $"#{v.Id}",
                                    ClienteNombre = v.Clientes?.Nombre ?? v.Nombres ?? "Consumidor Final",
                                    ClienteNit = v.Clientes?.Nit ?? v.Nit ?? "CF",
                                    EmpleadoNombre = v.Empleado?.NombreYApellidos ?? "",
                                    TipoVenta = v.TipoVenta ?? "",
                                    Origen = v.Origen ?? "",
                                    MontoTotal = totalNeto,
                                    TotalDescuento = totalDescuento,
                                    SubtotalProductos = Math.Round(subProductos, 2),
                                    SubtotalServicios = Math.Round(subServicios, 2),
                                    SubtotalExamenes = Math.Round(subExamenes, 2),
                                    SubtotalHospital = Math.Round(subHospital, 2),
                                    FormasPagoTexto = v.Pagos?.Select(p => p.FormaPago?.NombreFormaPago ?? "–").ToList() ?? new List<string>(),
                                    DetalleItems = detalleItems,
                                    MedicosAsignados = medicosAsignados,
                                    CantidadItemsProductos = cantProductos,
                                    CantidadItemsServicios = cantServicios,
                                    CantidadItemsExamenes = cantExamenes,
                                    CantidadItemsHospital = noches + (hosp.HospitalizacionesPaquetesHospitalizacion?.Count ?? 0),
                                    // Sala de operaciones: la habitación pertenece a categoría SO (ej. id 13)
                                    EsSalaOperaciones = hosp.Habitacion?.CategoriaHabitacion?.NombreCategoria
                                                           ?.IndexOf("operacion", StringComparison.OrdinalIgnoreCase) >= 0
                                                        || hosp.Habitacion?.CategoriaHabitacion?.NombreCategoria
                                                           ?.IndexOf("quirofano", StringComparison.OrdinalIgnoreCase) >= 0
                                                        || hosp.Habitacion?.CategoriaHabitacion?.NombreCategoria
                                                           ?.IndexOf("quirófano", StringComparison.OrdinalIgnoreCase) >= 0,
                                };
                            }
                            else
                            {
                                item = ConstruirItemDesdeDetalleVenta(v, modoDetalle, AgregarItem, _productoRepository, idAmb, ingresosCajaPorVenta);
                            }
                        }
                        else
                        {
                            item = ConstruirItemDesdeDetalleVenta(v, modoDetalle, AgregarItem, _productoRepository, idAmb, ingresosCajaPorVenta);
                        }
                    }
                    else
                    {
                        item = ConstruirItemDesdeDetalleVenta(v, modoDetalle, AgregarItem, _productoRepository, idAmb, ingresosCajaPorVenta);
                    }

                    AplicarFallbackMontoCobradoSiVacio(item, v, idAmb, ingresosCajaPorVenta, AgregarItem);

                    // Agregar médicos para ventas normales (Clínica, etc.)
                    if (item.MedicosAsignados == null || !item.MedicosAsignados.Any())
                        item.MedicosAsignados = ObtenerMedicosDeVenta(v);

                    var espDesdeDetalle = InferirEspecialidadDesdeDetalleItems(item.DetalleItems);
                    if (!string.IsNullOrWhiteSpace(espDesdeDetalle) && item.MedicosAsignados != null)
                    {
                        foreach (var med in item.MedicosAsignados.Where(m => string.IsNullOrWhiteSpace(m.Especialidad)))
                            med.Especialidad = espDesdeDetalle;
                    }

                    item.DetalleItemsPdf = modoDescripcion
                        ? (item.DetalleItems ?? new List<ReporteVentaDetalleItemViewModel>()).ToList()
                        : new List<ReporteVentaDetalleItemViewModel>();

                    items.Add(item);
                }

                ambienteCategoriaMontos.TryGetValue(idAmb, out var montosAmb);
                decimal vProd = items.Sum(i => i.SubtotalProductos);
                decimal vServ = items.Sum(i => i.SubtotalServicios);
                decimal vExam = items.Sum(i => i.SubtotalExamenes);
                decimal vHosp = items.Sum(i => i.SubtotalHospital);

                // Resumen por forma de pago del ambiente
                var resumenFP = ventasAmb
                    .SelectMany(v => v.Pagos ?? new List<Pagos>())
                    .GroupBy(p => p.FormaPago?.NombreFormaPago ?? "Sin especificar")
                    .Select(g => new ResumenFormaPagoViewModel
                    {
                        FormaPago = g.Key,
                        Total = g.Sum(p => Convert.ToDecimal(p.Monto))
                    })
                    .OrderByDescending(r => r.Total)
                    .ToList();

                ambientesVM.Add(new ReporteVentasAmbienteViewModel
                {
                    AmbienteId = idAmb,
                    AmbienteNombre = nomAmb,
                    Ventas = items,
                    AdicionalProductos = Math.Max(0, montosAmb.productos - vProd),
                    AdicionalServicios = Math.Max(0, montosAmb.servicios - vServ),
                    AdicionalExamenes = Math.Max(0, montosAmb.examenes - vExam),
                    AdicionalHospital = Math.Max(0, montosAmb.hospital - vHosp),
                    ResumenFormasPago = resumenFP,
                    TotalCobrado = ventasAmb
                        .SelectMany(v => v.Pagos ?? new List<Pagos>())
                        .Sum(p => Convert.ToDecimal(p.Monto)),
                    TotalCosto = ambienteCostos.TryGetValue(idAmb, out var costoAmb) ? costoAmb.costoTotal : 0,
                    TotalCantidadConCosto = ambienteCostos.TryGetValue(idAmb, out var cantAmb) ? cantAmb.cantidad : 0,
                });
            }

            if (incluirHospitalEnReporte
                && !ambientesVM.Any(a => a.AmbienteId == (int)AmbienteEnum.Hospital)
                && ambienteCategoriaMontos.TryGetValue((int)AmbienteEnum.Hospital, out var montosHospSolo)
                && (montosHospSolo.productos + montosHospSolo.servicios + montosHospSolo.examenes + montosHospSolo.hospital) > 0)
            {
                ambientesVM.Add(new ReporteVentasAmbienteViewModel
                {
                    AmbienteId = (int)AmbienteEnum.Hospital,
                    AmbienteNombre = "Hospital",
                    Ventas = new List<ReporteVentaItemViewModel>(),
                    AdicionalProductos = montosHospSolo.productos,
                    AdicionalServicios = montosHospSolo.servicios,
                    AdicionalExamenes = montosHospSolo.examenes,
                    AdicionalHospital = montosHospSolo.hospital,
                    TotalCosto = ambienteCostos.TryGetValue((int)AmbienteEnum.Hospital, out var ch) ? ch.costoTotal : 0,
                    TotalCantidadConCosto = ambienteCostos.TryGetValue((int)AmbienteEnum.Hospital, out var cantH) ? cantH.cantidad : 0,
                });
                ambientesVM = ambientesVM.OrderBy(a => a.AmbienteId).ToList();
            }

            // ── 9. Consolidado de médicos y especialidades ───────────────────────
            var consolidadoMedicos = incluirConsolidadoMedicos && modoDescripcion
                ? CalcularConsolidadoMedicos(ambientesVM)
                : new List<ConsolidadoMedicoViewModel>();

            var consolidadoEspecialidades = incluirConsolidadoMedicos
                ? CalcularConsolidadoEspecialidades(ambientesVM, montosHospitalPorEspecialidad)
                : new List<ConsolidadoEspecialidadViewModel>();

            // ── 10. Resumen global de formas de pago ─────────────────────────────
            var totalGlobalFormasPago = ventasBd
                .Where(v => ambientesVM.Select(a => a.AmbienteId).Contains(v.AmbienteId ?? 0))
                .SelectMany(v => v.Pagos ?? new List<Pagos>())
                .GroupBy(p => p.FormaPago?.NombreFormaPago ?? "Sin especificar")
                .Select(g => new ResumenFormaPagoViewModel
                {
                    FormaPago = g.Key,
                    Total = g.Sum(p => Convert.ToDecimal(p.Monto))
                })
                .OrderByDescending(r => r.Total)
                .ToList();

            var model = new ReporteVentasGeneralViewModel
            {
                Desde = fechaDesde.Date,
                Hasta = fechaHasta.Date,
                UsuarioGenera = usuarioGenera,
                FechaGeneracion = DateTime.Now,
                AmbienteIdFiltro = ambienteId > 0 ? ambienteId : (int?)null,
                EmpleadoIdFiltro = empleadoId > 0 ? empleadoId : (int?)null,
                EmpleadoNombre = empleadoNombreFiltro,
                Ambientes = ambientesVM,
                MostrarDetalle = modoDescripcion,
                MostrarResumenCobro = !modoDescripcion,
                MostrarColumnasCostoResultado = incluirConsolidadoItems,
                MostrarCostoEnConsolidadoItems = incluirConsolidadoItems,
                MostrarCostoGananciaEnResumenAmbiente = incluirConsolidadoItems,
                MostrarPrecioCostoUnitarioResumen = modoDescripcion,
                MostrarConsolidadoItems = incluirConsolidadoItems,
                ModoReporteTexto = "Desglose por ambiente y categoría de ítem",
                TotalGlobalFormasPago = totalGlobalFormasPago,
                TotalGlobalCobrado = totalGlobalFormasPago.Sum(f => f.Total),
                ConsolidadoMedicos = consolidadoMedicos,
                MostrarConsolidadoMedicos = incluirConsolidadoMedicos && modoDescripcion,
                ConsolidadoEspecialidades = consolidadoEspecialidades,
                MostrarConsolidadoEspecialidades = incluirConsolidadoMedicos,
                ItemsConsolidados = incluirConsolidadoItems
                    ? itemsConsolidadosDict.Select(kvp => new ItemConsolidadoViewModel
                    {
                        Categoria = kvp.Key.Split('|')[0],
                        Nombre = kvp.Key.Split('|')[1],
                        Cantidad = kvp.Value.cantidad,
                        Total = kvp.Value.total,
                        CostoUnitario = kvp.Value.costoUnitario,
                        CostoTotal = kvp.Value.costoTotal > 0
                            ? kvp.Value.costoTotal
                            : kvp.Value.cantidad * (kvp.Value.costoUnitario ?? 0)
                    }).OrderBy(i => i.Categoria).ThenBy(i => i.Nombre).ToList()
                    : new List<ItemConsolidadoViewModel>()
            };

            // ── 11. Opciones de página A4 Portrait ────────────────────────────────
            var ventasOptions = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 12 },
                PageOrientation = Wkhtmltopdf.NetCore.Options.Orientation.Portrait,
            };

            return await RenderPdfAsync(
                "Views/CrearPDF/ReporteVentasGeneralPDF.cshtml", model, ventasOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ReporteVentasGeneralPDF desde={Desde} hasta={Hasta}", desde, hasta);
                return new ContentResult
                {
                    StatusCode = 503,
                    ContentType = "text/plain; charset=utf-8",
                    Content = PdfGenerationHelper.GetFriendlyErrorMessage(ex)
                };
            }
        }

        // ── Consolidado por médico tratante (no recepcionista de la venta) ─────
        private List<ConsolidadoMedicoViewModel> CalcularConsolidadoMedicos(List<ReporteVentasAmbienteViewModel> ambientesVM)
        {
            var dictMedicos = new Dictionary<string, ConsolidadoMedicoViewModel>(StringComparer.OrdinalIgnoreCase);
            foreach (var amb in ambientesVM)
            {
                foreach (var vItem in amb.Ventas ?? Enumerable.Empty<ReporteVentaItemViewModel>())
                {
                    if (vItem == null) continue;
                    string medicoTratante = ObtenerNombreMedicoTratante(vItem);
                    if (string.IsNullOrWhiteSpace(medicoTratante)) continue;

                    if (!dictMedicos.TryGetValue(medicoTratante, out var row))
                    {
                        row = new ConsolidadoMedicoViewModel { MedicoNombre = medicoTratante };
                        dictMedicos[medicoTratante] = row;
                    }

                    row.CantidadVentas++;
                    row.TotalProductos += vItem.SubtotalProductos;
                    row.TotalServicios += vItem.SubtotalServicios;
                    row.TotalExamenes += vItem.SubtotalExamenes;
                    row.TotalHospital += vItem.SubtotalHospital;
                    row.TotalDescuentos += vItem.TotalDescuento;
                    row.TotalNeto += vItem.MontoNeto;
                    row.CantidadProductos += vItem.CantidadItemsProductos;
                    row.CantidadServicios += vItem.CantidadItemsServicios;
                    row.CantidadExamenes += vItem.CantidadItemsExamenes;
                    row.CantidadHospital += vItem.CantidadItemsHospital;

                    if (amb.AmbienteId == (int)AmbienteEnum.Hospital ||
                        (vItem.TipoVenta?.IndexOf("hospitalizacion", StringComparison.OrdinalIgnoreCase) >= 0))
                        row.CantidadHospitalizaciones++;
                    else
                        row.CantidadCitas++;

                    if (vItem.EsSalaOperaciones)
                        row.CantidadSalaOperaciones++;
                    else if (amb.AmbienteId != (int)AmbienteEnum.Hospital)
                        row.CantidadConsultaExterna++;
                }
            }
            return dictMedicos.Values
                .Where(m => m.TotalNeto > 0)
                .OrderByDescending(m => m.TotalNeto).ToList();
        }

        private List<ConsolidadoEspecialidadViewModel> CalcularConsolidadoEspecialidades(
            List<ReporteVentasAmbienteViewModel> ambientesVM,
            Dictionary<string, (decimal productos, decimal servicios, decimal examenes, decimal hospital)> montosHospitalPorEspecialidad = null)
        {
            var dict = new Dictionary<string, ConsolidadoEspecialidadViewModel>(StringComparer.OrdinalIgnoreCase);
            foreach (var amb in ambientesVM)
            {
                bool esHospital = amb.AmbienteId == (int)AmbienteEnum.Hospital;
                foreach (var vItem in amb.Ventas ?? Enumerable.Empty<ReporteVentaItemViewModel>())
                {
                    if (vItem == null) continue;
                    string espFallback = ResolverEspecialidadVenta(vItem);
                    var lineas = vItem.DetalleItems?
                        .Where(d => d.Categoria != "Insumo")
                        .ToList();

                    if (lineas != null && lineas.Any())
                    {
                        var especialidadesProcesadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        foreach (var linea in lineas)
                        {
                            string esp = MapearEspecialidadDesdeNombreItem(linea.Descripcion) ?? espFallback;
                            decimal prod = 0, serv = 0, exam = 0, hosp = 0;
                            switch (linea.Categoria)
                            {
                                case "Producto": prod = linea.Subtotal; break;
                                case "Servicio": serv = linea.Subtotal; break;
                                case "Hospital": hosp = linea.Subtotal; break;
                                default:
                                    if (linea.Categoria.Contains("Examen", StringComparison.OrdinalIgnoreCase))
                                        exam = linea.Subtotal;
                                    else
                                        serv = linea.Subtotal;
                                    break;
                            }
                            bool contarVenta = especialidadesProcesadas.Add(esp ?? "Sin especialidad");
                            int cantLinea = (int)Math.Max(1, Math.Round(linea.Cantidad));
                            AcumularEspecialidad(dict, esp, prod, serv, exam, hosp, 0, linea.Subtotal,
                                esHospital, vItem, linea.Categoria, contarVenta, cantLinea);
                        }
                    }
                    else
                    {
                        AcumularEspecialidad(dict, espFallback,
                            vItem.SubtotalProductos, vItem.SubtotalServicios, vItem.SubtotalExamenes,
                            vItem.SubtotalHospital, vItem.TotalDescuento, vItem.MontoNeto,
                            esHospital, vItem, null, true);
                    }
                }
            }

            var ambHospital = ambientesVM.FirstOrDefault(a => a.AmbienteId == (int)AmbienteEnum.Hospital);
            if (ambHospital != null
                && (ambHospital.AdicionalProductos + ambHospital.AdicionalServicios + ambHospital.AdicionalExamenes + ambHospital.AdicionalHospital) > 0)
                AgregarAdicionalesHospitalEspecialidad(dict, ambHospital, montosHospitalPorEspecialidad);

            return dict.Values.OrderByDescending(e => e.TotalNeto).ToList();
        }

        private static void AgregarAdicionalesHospitalEspecialidad(
            Dictionary<string, ConsolidadoEspecialidadViewModel> dict,
            ReporteVentasAmbienteViewModel ambHospital,
            Dictionary<string, (decimal productos, decimal servicios, decimal examenes, decimal hospital)> montosPorEsp)
        {
            if (ambHospital == null) return;

            void Distribuir(
                decimal adicional,
                Func<(decimal productos, decimal servicios, decimal examenes, decimal hospital), decimal> selector,
                string categoria)
            {
                if (adicional <= 0) return;

                decimal totalRef = montosPorEsp?.Values.Sum(selector) ?? 0;
                if (totalRef <= 0 || montosPorEsp == null || !montosPorEsp.Any())
                {
                    decimal prod = categoria == "Producto" ? adicional : 0;
                    decimal serv = categoria == "Servicio" ? adicional : 0;
                    decimal exam = categoria == "Examen" ? adicional : 0;
                    decimal hosp = categoria == "Hospital" ? adicional : 0;
                    string esp = (ambHospital.Ventas ?? new List<ReporteVentaItemViewModel>())
                        .Select(ResolverEspecialidadVenta)
                        .FirstOrDefault(e => !string.IsNullOrWhiteSpace(e) && !string.Equals(e, "Sin especialidad", StringComparison.OrdinalIgnoreCase))
                        ?? "Sin especialidad";
                    AcumularEspecialidad(dict, esp, prod, serv, exam, hosp, 0, adicional,
                        true, null, categoria, false);
                    if (categoria != "Producto" && dict.TryGetValue(esp, out var rowSin))
                    {
                        rowSin.CantidadHospitalizaciones++;
                        rowSin.MontoHospitalizacion += adicional;
                    }
                    return;
                }

                foreach (var kvp in montosPorEsp.Where(k => selector(k.Value) > 0))
                {
                    decimal share = Math.Round(adicional * (selector(kvp.Value) / totalRef), 2);
                    if (share <= 0) continue;
                    decimal prod = categoria == "Producto" ? share : 0;
                    decimal serv = categoria == "Servicio" ? share : 0;
                    decimal exam = categoria == "Examen" ? share : 0;
                    decimal hosp = categoria == "Hospital" ? share : 0;
                    AcumularEspecialidad(dict, kvp.Key, prod, serv, exam, hosp, 0, share,
                        true, null, categoria, false);
                    if (categoria != "Producto" && dict.TryGetValue(kvp.Key, out var row))
                    {
                        row.CantidadHospitalizaciones++;
                        row.MontoHospitalizacion += share;
                    }
                }
            }

            Distribuir(ambHospital.AdicionalProductos, m => m.productos, "Producto");
            Distribuir(ambHospital.AdicionalServicios, m => m.servicios, "Servicio");
            Distribuir(ambHospital.AdicionalExamenes, m => m.examenes, "Examen");
            Distribuir(ambHospital.AdicionalHospital, m => m.hospital, "Hospital");
        }

        private static void AcumularEspecialidad(
            Dictionary<string, ConsolidadoEspecialidadViewModel> dict,
            string especialidad,
            decimal productos, decimal servicios, decimal examenes, decimal hospital,
            decimal descuentos, decimal neto,
            bool esHospitalAmbiente, ReporteVentaItemViewModel vItem, string categoriaLinea,
            bool contarVenta = true, int cantidadLinea = 1)
        {
            if (string.IsNullOrWhiteSpace(especialidad)) especialidad = "Sin especialidad";
            if (!dict.TryGetValue(especialidad, out var row))
            {
                row = new ConsolidadoEspecialidadViewModel { EspecialidadNombre = especialidad };
                dict[especialidad] = row;
            }

            if (contarVenta)
                row.CantidadVentas++;
            row.TotalProductos += productos;
            row.TotalServicios += servicios;
            row.TotalExamenes += examenes;
            row.TotalHospital += hospital;
            row.TotalDescuentos += descuentos;
            row.TotalNeto += neto;

            if (vItem == null)
                return;

            bool esHosp = esHospitalAmbiente
                || (vItem.TipoVenta?.IndexOf("hospitalizacion", StringComparison.OrdinalIgnoreCase) >= 0);

            if (categoriaLinea == null)
            {
                if (vItem.EsSalaOperaciones)
                {
                    row.CantidadSalaOperaciones++;
                    row.MontoSalaOperaciones += neto;
                }
                else if (esHosp)
                {
                    row.CantidadHospitalizaciones++;
                    row.MontoHospitalizacion += neto;
                }
                else
                {
                    row.CantidadConsultaExterna++;
                    row.MontoConsultaExterna += neto;
                    if (vItem.SubtotalServicios > 0)
                    {
                        row.CantidadCitas++;
                        row.MontoCitas += vItem.SubtotalServicios;
                    }
                }
                if (vItem.SubtotalExamenes > 0)
                    row.CantidadExamenes += (int)Math.Max(1, vItem.CantidadItemsExamenes);
                return;
            }

            if (vItem.EsSalaOperaciones)
            {
                row.CantidadSalaOperaciones++;
                row.MontoSalaOperaciones += neto;
            }
            else if (categoriaLinea == "Hospital" || (esHosp && hospital > 0))
            {
                row.CantidadHospitalizaciones++;
                row.MontoHospitalizacion += hospital > 0 ? hospital : neto;
            }
            else if (categoriaLinea.Contains("Examen", StringComparison.OrdinalIgnoreCase))
            {
                row.CantidadExamenes += cantidadLinea;
                if (!esHosp)
                {
                    row.CantidadConsultaExterna++;
                    row.MontoConsultaExterna += examenes > 0 ? examenes : neto;
                }
            }
            else if (categoriaLinea == "Servicio")
            {
                row.CantidadCitas++;
                row.MontoCitas += servicios > 0 ? servicios : neto;
                if (!esHosp)
                {
                    row.CantidadConsultaExterna++;
                    row.MontoConsultaExterna += servicios > 0 ? servicios : neto;
                }
            }
            else if (categoriaLinea == "Producto" && !esHosp)
            {
                row.CantidadConsultaExterna++;
                row.MontoConsultaExterna += productos > 0 ? productos : neto;
            }
            else if (categoriaLinea == "Producto" && esHosp && productos > 0)
            {
                row.CantidadHospitalizaciones++;
                row.MontoHospitalizacion += productos;
            }
        }

        private static string ObtenerNombreMedicoTratante(ReporteVentaItemViewModel vItem)
        {
            if (vItem == null) return null;
            var tratante = vItem.MedicosAsignados?
                .FirstOrDefault(m => string.Equals(m.Rol, "Tratante", StringComparison.OrdinalIgnoreCase)
                                  && !string.IsNullOrWhiteSpace(m.Nombre));
            if (tratante != null) return tratante.Nombre.Trim();

            var asignado = vItem.MedicosAsignados?
                .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.Nombre));
            return asignado?.Nombre?.Trim();
        }

        private static string ResolverEspecialidadVenta(ReporteVentaItemViewModel vItem)
        {
            if (vItem == null) return "Sin especialidad";
            var tratante = vItem.MedicosAsignados?
                .FirstOrDefault(m => string.Equals(m.Rol, "Tratante", StringComparison.OrdinalIgnoreCase)
                                  && !string.IsNullOrWhiteSpace(m.Especialidad));
            if (!string.IsNullOrWhiteSpace(tratante?.Especialidad))
                return tratante.Especialidad.Trim();

            var espMedico = vItem.MedicosAsignados?
                .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.Especialidad));
            if (!string.IsNullOrWhiteSpace(espMedico?.Especialidad))
                return espMedico.Especialidad.Trim();

            var desdeItems = InferirEspecialidadDesdeDetalleItems(vItem.DetalleItems);
            if (!string.IsNullOrWhiteSpace(desdeItems))
                return desdeItems;

            return "Sin especialidad";
        }

        private static string InferirEspecialidadDesdeDetalleItems(IEnumerable<ReporteVentaDetalleItemViewModel> items)
        {
            if (items == null) return null;
            foreach (var item in items.Where(i => i.Categoria != "Insumo" && i.Categoria != "Producto"))
            {
                var esp = MapearEspecialidadDesdeNombreItem(item.Descripcion);
                if (!string.IsNullOrWhiteSpace(esp))
                    return esp;
            }
            foreach (var item in items.Where(i => i.Categoria == "Servicio"))
            {
                var esp = MapearEspecialidadDesdeNombreItem(item.Descripcion);
                if (!string.IsNullOrWhiteSpace(esp))
                    return esp;
            }
            return null;
        }

        private static string NormalizarCategoriaConsolidado(string categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria)) return "Servicio";
            if (categoria.Contains("Examen", StringComparison.OrdinalIgnoreCase)
                || categoria.Contains("Lab", StringComparison.OrdinalIgnoreCase))
                return "Examen";
            if (categoria.Equals("Paquete", StringComparison.OrdinalIgnoreCase)
                || categoria.Contains("Paquete", StringComparison.OrdinalIgnoreCase))
                return "Paquete";
            if (categoria.Equals("Dieta", StringComparison.OrdinalIgnoreCase)
                || categoria.Contains("Dieta", StringComparison.OrdinalIgnoreCase)
                || categoria.Contains("Nutric", StringComparison.OrdinalIgnoreCase))
                return "Dieta";
            if (categoria.Equals("Hospital", StringComparison.OrdinalIgnoreCase))
                return "Hospital";
            if (categoria.Equals("Producto", StringComparison.OrdinalIgnoreCase))
                return "Producto";
            if (categoria.Equals("Servicio", StringComparison.OrdinalIgnoreCase))
                return "Servicio";
            return categoria;
        }

        private static string ClasificarCategoriaServicio(string nombreServicio, string categoriaServicio)
        {
            var nombre = (nombreServicio ?? "").ToUpperInvariant();
            var categoria = (categoriaServicio ?? "").ToUpperInvariant();
            if (nombre.Contains("PAQUETE") || categoria.Contains("PAQUETE"))
                return "Paquete";
            if (nombre.Contains("DIETA") || categoria.Contains("DIETA") || categoria.Contains("NUTRIC"))
                return "Dieta";
            return "Servicio";
        }

        private static string MapearEspecialidadDesdeNombreItem(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;

            var esp = ExtraerEspecialidadDeNombre(nombre);
            if (!string.IsNullOrWhiteSpace(esp))
                return NormalizarNombreEspecialidad(esp);

            var prefijoNumerado = System.Text.RegularExpressions.Regex.Match(
                nombre.Trim(),
                @"^\d+\s*-\s*(.+)$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (prefijoNumerado.Success)
            {
                var espNumerada = MapearEspecialidadDesdeNombreItem(prefijoNumerado.Groups[1].Value.Trim());
                if (!string.IsNullOrWhiteSpace(espNumerada))
                    return espNumerada;
            }

            var u = nombre.ToUpperInvariant();
            if (u.Contains("GINECOL")) return "Ginecología y Obstetricia";
            if (u.Contains("PEDIATR")) return "Pediatría";
            if (u.Contains("TRAUMATOL")) return "Traumatología";
            if (u.Contains("OFTALMOL") || u.Contains("OPTOMETR") || u.Contains("OPTÓMETR")) return "Oftalmología";
            if (u.Contains("NEUMOL")) return "Neumología";
            if (u.Contains("RADIOL")) return "Radiología";
            if (u.Contains("CARDIOL")) return "Cardiología";
            if (u.Contains("NUTRIC")) return "Nutrición";
            if (u.Contains("MEDICINA GENERAL") || u.Contains("CONSULTA MEDICA") || u.Contains("MEDICA GENERAL"))
                return "Medicina general";
            if (u.StartsWith("CONSULTA") || u.Contains(" RE-CONSULTA"))
                return "Medicina general";
            return null;
        }

        private static string NormalizarNombreEspecialidad(string esp)
        {
            if (string.IsNullOrWhiteSpace(esp)) return esp;
            if (esp.Equals("Ginecologia", StringComparison.OrdinalIgnoreCase)) return "Ginecología y Obstetricia";
            if (esp.Equals("Traumatologia", StringComparison.OrdinalIgnoreCase)) return "Traumatología";
            if (esp.Equals("Oftalmologia", StringComparison.OrdinalIgnoreCase)) return "Oftalmología";
            if (esp.Equals("Neumología", StringComparison.OrdinalIgnoreCase) || esp.Equals("Neumologia", StringComparison.OrdinalIgnoreCase))
                return "Neumología";
            return esp.Trim();
        }

        private static string ExtraerEspecialidadDeNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;
            var idx = nombre.IndexOf(" - ", StringComparison.Ordinal);
            if (idx <= 0) return null;
            var parte = nombre[..idx].Trim();
            return parte.Length > 2 ? parte : null;
        }

        private Empleado ObtenerEmpleadoConEspecialidad(int empleadoId) =>
            _context.Empleados
                .Include(e => e.Especialidad)
                .AsNoTracking()
                .FirstOrDefault(e => e.Id == empleadoId);

        private Citas ObtenerCitaRelacionadaVenta(Venta v)
        {
            if (v.PacienteId == null) return null;
            var fecha = v.FechaVenta.Date;
            var fechaFin = fecha.AddDays(1);

            IQueryable<Citas> baseQuery = _context.Citass
                .Include(c => c.Empleado).ThenInclude(e => e.Especialidad)
                .Include(c => c.Especialidad)
                .Include(c => c.Servicio)
                .Include(c => c.CitasServicios)
                .AsNoTracking()
                .Where(c => c.PacienteId == v.PacienteId && !c.Eliminado);

            // 0) Orden de examen / ingreso con consulta vinculada
            if (v.ExamenId != null)
            {
                var examenOrden = _context.Examenes
                    .AsNoTracking()
                    .Where(e => e.Id == v.ExamenId)
                    .Select(e => new { e.ConsultaId, e.PacienteId })
                    .FirstOrDefault();

                if (examenOrden?.ConsultaId != null)
                {
                    var citaIngreso = _context.Consultas
                        .Include(c => c.Citas).ThenInclude(c => c.Empleado).ThenInclude(e => e.Especialidad)
                        .Include(c => c.Citas).ThenInclude(c => c.Especialidad)
                        .Include(c => c.Citas).ThenInclude(c => c.Servicio)
                        .AsNoTracking()
                        .Where(c => c.Id == examenOrden.ConsultaId)
                        .Select(c => c.Citas)
                        .FirstOrDefault();
                    if (citaIngreso != null) return citaIngreso;
                }
            }

            // 1) Cita del mismo día vinculada por servicio facturado
            var servicioIds = v.DetalleVenta?
                .Where(d => d.ServicioId != null)
                .Select(d => d.ServicioId.Value)
                .Distinct()
                .ToList() ?? new List<int>();

            // 1b) Consulta del paciente con servicios facturados en la venta
            if (servicioIds.Any())
            {
                var citaIdConsultaServicio = _context.ConsultasServicios
                    .AsNoTracking()
                    .Where(cs => servicioIds.Contains(cs.ServicioId))
                    .Join(_context.Consultas.AsNoTracking(),
                        cs => cs.ConsultaId,
                        c => c.Id,
                        (cs, c) => c)
                    .Where(c => c.CitasId != null
                             && c.FechaYHoraInicioConsulta >= fecha
                             && c.FechaYHoraInicioConsulta < fechaFin)
                    .Join(_context.Citass.AsNoTracking(),
                        c => c.CitasId,
                        cita => cita.Id,
                        (c, cita) => cita)
                    .Where(cita => cita.PacienteId == v.PacienteId && !cita.Eliminado)
                    .OrderByDescending(cita => cita.Id)
                    .Select(cita => cita.Id)
                    .FirstOrDefault();

                if (citaIdConsultaServicio > 0)
                {
                    var citaDesdeConsulta = baseQuery.FirstOrDefault(c => c.Id == citaIdConsultaServicio);
                    if (citaDesdeConsulta != null) return citaDesdeConsulta;
                }
            }

            if (servicioIds.Any())
            {
                var citaPorServicio = baseQuery
                    .Where(c => c.FechaInicio.HasValue
                             && c.FechaInicio.Value >= fecha
                             && c.FechaInicio.Value < fechaFin
                             && (servicioIds.Contains(c.ServicioId ?? 0)
                                 || c.CitasServicios.Any(cs => servicioIds.Contains(cs.ServicioId))))
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();
                if (citaPorServicio != null) return citaPorServicio;
            }

            // 2) Cita pagada el mismo día (consulta clínica)
            var citaMismoDia = baseQuery
                .Where(c => c.FechaInicio.HasValue && c.FechaInicio.Value >= fecha && c.FechaInicio.Value < fechaFin)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();
            if (citaMismoDia != null) return citaMismoDia;

            // 3) Consulta del paciente en la fecha de la venta
            var citaConsulta = _context.Consultas
                .Include(c => c.Citas).ThenInclude(c => c.Empleado).ThenInclude(e => e.Especialidad)
                .Include(c => c.Citas).ThenInclude(c => c.Especialidad)
                .Include(c => c.Citas).ThenInclude(c => c.Servicio)
                .AsNoTracking()
                .Where(c => c.Citas != null
                         && c.Citas.PacienteId == v.PacienteId
                         && c.FechaYHoraInicioConsulta >= fecha
                         && c.FechaYHoraInicioConsulta < fechaFin)
                .OrderByDescending(c => c.Id)
                .Select(c => c.Citas)
                .FirstOrDefault();
            if (citaConsulta != null) return citaConsulta;

            // 4) Venta de laboratorio vinculada a orden de examen
            if (v.ExamenId != null)
            {
                var citaExamen = _context.Examenes
                    .Include(e => e.Paciente)
                    .AsNoTracking()
                    .Where(e => e.Id == v.ExamenId && e.PacienteId != null)
                    .Select(e => e.PacienteId)
                    .FirstOrDefault();

                if (citaExamen != null)
                {
                    var citaLab = baseQuery
                        .Where(c => c.FechaInicio.HasValue
                                 && c.FechaInicio.Value >= fecha.AddDays(-7)
                                 && c.FechaInicio.Value < fechaFin)
                        .OrderByDescending(c => c.FechaInicio)
                        .FirstOrDefault();
                    if (citaLab != null) return citaLab;
                }
            }

            // 5) Hospitalización activa del paciente
            var hosp = _context.Hospitalizaciones
                .AsNoTracking()
                .Where(h => h.PacienteId == v.PacienteId
                         && h.FechaInicio.Date <= fecha
                         && (!h.Finalizada || h.FechaFin.Date >= fecha))
                .OrderByDescending(h => h.Id)
                .Select(h => h.Id)
                .FirstOrDefault();

            if (hosp > 0)
            {
                var citaHospi = baseQuery
                    .Where(c => c.CitaTipoAtencion != null
                             && c.CitaTipoAtencion.Contains("Hospital"))
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();
                if (citaHospi != null) return citaHospi;
            }

            // 6) Cita más reciente del paciente (±7 días)
            return baseQuery
                .Where(c => c.FechaInicio.HasValue
                         && c.FechaInicio.Value >= fecha.AddDays(-7)
                         && c.FechaInicio.Value < fechaFin.AddDays(7))
                .OrderByDescending(c => c.FechaInicio)
                .FirstOrDefault();
        }

        private string ResolverEspecialidadEmpleado(Empleado emp, Citas cita, Servicio servicio = null)
        {
            if (!string.IsNullOrWhiteSpace(cita?.Especialidad?.NombreEspecialidad))
                return cita.Especialidad.NombreEspecialidad.Trim();
            if (!string.IsNullOrWhiteSpace(emp?.Especialidad?.NombreEspecialidad))
                return emp.Especialidad.NombreEspecialidad.Trim();
            return ResolverEspecialidadServicio(servicio ?? cita?.Servicio);
        }

        private static string ResolverEspecialidadServicio(Servicio servicio)
        {
            if (servicio == null) return null;
            var esp = MapearEspecialidadDesdeNombreItem(servicio.NombreServicio);
            if (!string.IsNullOrWhiteSpace(esp)) return esp;
            return servicio.CategoriaServicio?.NombreCategoria?.Trim();
        }

        private List<ReporteVentaDetalleItemViewModel> ObtenerDetalleInsumosServicio(int? servicioId, decimal cantidadServicio)
        {
            var resultado = new List<ReporteVentaDetalleItemViewModel>();
            if (servicioId == null || cantidadServicio <= 0) return resultado;

            var insumos = _context.ServiciosInsumos
                .Include(si => si.Producto)
                .AsNoTracking()
                .Where(si => si.ServicioId == servicioId.Value && !si.Eliminado)
                .ToList();

            foreach (var insumo in insumos)
            {
                decimal cantidad = insumo.CantidadUtilizada * cantidadServicio;
                if (cantidad <= 0) continue;

                decimal? costoUnitario = null;
                if (insumo.Producto?.PrecioCosto > 0)
                    costoUnitario = insumo.Producto.PrecioCosto;
                else if (insumo.ProductoId > 0)
                {
                    var prod = _productoRepository.GetProdutoById(insumo.ProductoId);
                    if (prod?.PrecioCosto > 0) costoUnitario = prod.PrecioCosto;
                }

                resultado.Add(CrearDetalleItemVenta(
                    $"↳ Insumo: {insumo.Producto?.NombreProducto ?? "Insumo"}",
                    "Insumo",
                    cantidad,
                    costoUnitario ?? 0,
                    cantidad * (costoUnitario ?? 0),
                    0,
                    costoUnitario));
            }

            return resultado;
        }

        private decimal? ObtenerCostoUnitarioExamen(int? examenLabClinicoId, ExamenLabClinico nav)
        {
            if (nav?.PrecioCosto > 0) return nav.PrecioCosto;
            if (examenLabClinicoId.HasValue)
            {
                var exam = _context.ExamenLabClinicos
                    .Where(e => e.Id == examenLabClinicoId.Value)
                    .Select(e => (decimal?)e.PrecioCosto)
                    .FirstOrDefault();
                if (exam.HasValue && exam.Value > 0) return exam;
            }
            return null;
        }

        private decimal? ObtenerCostoUnitarioServicio(int? servicioId, Servicio nav)
        {
            if (servicioId == null) return null;
            var insumos = _context.ServiciosInsumos
                .Include(si => si.Producto)
                .Where(si => si.ServicioId == servicioId.Value && !si.Eliminado)
                .ToList();
            if (insumos.Any())
            {
                decimal total = insumos.Sum(si =>
                {
                    decimal pc = si.Producto?.PrecioCosto ?? 0;
                    if (pc <= 0 && si.ProductoId > 0)
                    {
                        var p = _productoRepository.GetProdutoById(si.ProductoId);
                        pc = p?.PrecioCosto ?? 0;
                    }
                    return pc * si.CantidadUtilizada;
                });
                if (total > 0) return total;
            }
            return null;
        }

        private static ReporteVentaDetalleItemViewModel CrearDetalleItemVenta(
            string descripcion,
            string categoria,
            decimal cantidad,
            decimal precioUnit,
            decimal subtotal,
            decimal descuento = 0,
            decimal? costoUnitario = null)
        {
            decimal costoTotal = Math.Round(cantidad * (costoUnitario ?? 0), 2);
            return new ReporteVentaDetalleItemViewModel
            {
                Descripcion = descripcion,
                Categoria = categoria,
                Cantidad = cantidad,
                PrecioUnit = precioUnit,
                Subtotal = subtotal,
                Descuento = descuento,
                CostoUnitario = costoUnitario,
                CostoTotal = costoTotal,
                Ganancia = Math.Round(subtotal - costoTotal, 2)
            };
        }



        // ────────────────────────────────────────────────────────────
        // Consolidado hospital: insumos aplicados, paquetes, servicios (una vez por hospitalización)
        // ────────────────────────────────────────────────────────────
        private void ConsolidarHospitalizacionesEnPeriodo(
            List<Venta> ventasBd,
            DateTime desde,
            DateTime hastaFin,
            Action<string, string, decimal, decimal, decimal?, int> agregarItem,
            Dictionary<int, (decimal costoTotal, decimal cantidad)> ambienteCostos,
            int ambienteId,
            Dictionary<string, (decimal productos, decimal servicios, decimal examenes, decimal hospital)> montosHospitalPorEspecialidad = null,
            Dictionary<int, int?> hospitalizacionPorVenta = null,
            Dictionary<int, decimal?> costoProductoCache = null)
        {
            var hospIds = ObtenerHospitalizacionIdsEnPeriodo(ventasBd, desde, hastaFin, hospitalizacionPorVenta);
            foreach (var hospId in hospIds)
                AgregarConsolidadoDesdeHospitalizacion(hospId, ventasBd, desde, hastaFin, agregarItem, ambienteCostos, ambienteId, montosHospitalPorEspecialidad, hospitalizacionPorVenta, costoProductoCache);
        }

        private HashSet<int> ObtenerHospitalizacionIdsEnPeriodo(List<Venta> ventasBd, DateTime desde, DateTime hastaFin, Dictionary<int, int?> hospitalizacionPorVenta = null)
        {
            var ids = new HashSet<int>();

            foreach (var v in ventasBd.Where(v => v.AmbienteId == (int)AmbienteEnum.Hospital))
            {
                var id = ObtenerHospitalizacionIdPorVenta(v, hospitalizacionPorVenta);
                if (id != null) ids.Add(id.Value);
            }

            var idsPorAplicacion = _context.HospitalizacionesProductosAplicaciones
                .AsNoTracking()
                .Where(a => a.Aplicado && !a.Eliminado)
                .Where(a =>
                    (a.FechaHoraAplicacionManual != null
                        && a.FechaHoraAplicacionManual >= desde
                        && a.FechaHoraAplicacionManual <= hastaFin)
                    || (a.FechaHoraAplicacionManual == null
                        && a.FechaHoraAplicacion != null
                        && a.FechaHoraAplicacion >= desde
                        && a.FechaHoraAplicacion <= hastaFin))
                .Join(_context.HospitalizacionesProductos.AsNoTracking().Where(hp => !hp.Eliminado),
                    a => a.HospitalizacionProductoId,
                    hp => hp.Id,
                    (a, hp) => hp.HospitalizacionId)
                .Distinct();
            foreach (var id in idsPorAplicacion) ids.Add(id);

            var idsPorInsumoDirecto = _context.HospitalizacionInsumosDirectosAplicaciones
                .AsNoTracking()
                .Where(a => a.Aplicado)
                .Where(a => a.FechaHoraAplicacion != null
                    && a.FechaHoraAplicacion >= desde
                    && a.FechaHoraAplicacion <= hastaFin)
                .Join(_context.HospitalizacionInsumosDirectos.AsNoTracking().Where(i => !i.Eliminado),
                    a => a.HospitalizacionInsumoDirectoId,
                    i => i.Id,
                    (a, i) => i.HospitalizacionId)
                .Distinct();
            foreach (var id in idsPorInsumoDirecto) ids.Add(id);

            var idsPorEstadia = _context.Hospitalizaciones
                .AsNoTracking()
                .Where(h => !h.Eliminada
                         && h.FechaInicio <= hastaFin
                         && h.FechaFin >= desde)
                .Select(h => h.Id);
            foreach (var id in idsPorEstadia) ids.Add(id);

            return ids;
        }

        private void AgregarConsolidadoDesdeHospitalizacion(
            int hospitalizacionId,
            List<Venta> ventasBd,
            DateTime desde,
            DateTime hastaFin,
            Action<string, string, decimal, decimal, decimal?, int> agregarItem,
            Dictionary<int, (decimal costoTotal, decimal cantidad)> ambienteCostos,
            int ambienteId,
            Dictionary<string, (decimal productos, decimal servicios, decimal examenes, decimal hospital)> montosHospitalPorEspecialidad = null,
            Dictionary<int, int?> hospitalizacionPorVenta = null,
            Dictionary<int, decimal?> costoProductoCache = null)
        {
            var hosp = _hospitalizacionRepository.Get(
                hospitalizacionId,
                includeMedicamentos: true,
                includeServicios: true,
                includeExamenes: true,
                includePaquetes: true,
                includeConsultas: false,
                includeOrdenesMedicas: false);
            if (hosp == null) return;

            bool hospEnPeriodo = hosp.FechaInicio <= hastaFin && hosp.FechaFin >= desde;
            bool tieneVentaEnPeriodo = ventasBd.Any(v =>
                v.AmbienteId == (int)AmbienteEnum.Hospital
                && ObtenerHospitalizacionIdPorVenta(v, hospitalizacionPorVenta) == hospitalizacionId);

            string espHospital = hosp.Especialidad?.NombreEspecialidad ?? "Sin especialidad";
            void RegistrarMontoHospital(string categoria, decimal subtotal)
            {
                if (montosHospitalPorEspecialidad == null || subtotal <= 0) return;
                if (!montosHospitalPorEspecialidad.TryGetValue(espHospital, out var montos))
                    montos = (0, 0, 0, 0);
                categoria = NormalizarCategoriaConsolidado(categoria);
                if (categoria == "Producto")
                    montos.productos += subtotal;
                else if (categoria == "Servicio")
                    montos.servicios += subtotal;
                else if (categoria == "Examen")
                    montos.examenes += subtotal;
                else
                    montos.hospital += subtotal;
                montosHospitalPorEspecialidad[espHospital] = montos;
            }

            decimal subtotalEstadia = CalcularSubtotalEstadiaEnPeriodo(hosp, desde, hastaFin, out int nochesEstadia);
            if (subtotalEstadia > 0)
            {
                agregarItem("Hospital", $"Estadía en habitación ({nochesEstadia} noche(s))", nochesEstadia, subtotalEstadia, 0m, ambienteId);
                RegistrarMontoHospital("Hospital", subtotalEstadia);
            }

            var cambiosHabitacion = _hospitalizacionRepository.GetCambiosHabitacion(hosp.Id);
            foreach (var cambio in cambiosHabitacion)
            {
                var fechaCambio = cambio.FechaCambio;
                if (fechaCambio < desde || fechaCambio > hastaFin) continue;
                decimal montoCambio = cambio.ValorTarifa * cambio.Dias;
                if (montoCambio > 0)
                {
                    agregarItem("Hospital", "Cambio de habitación", cambio.Dias, montoCambio, 0m, ambienteId);
                    RegistrarMontoHospital("Hospital", montoCambio);
                }
            }

            foreach (var prod in hosp.HospitalizacionesProductos ?? Enumerable.Empty<HospitalizacionProducto>())
            {
                var aplicadas = prod.HospitalizacionesProductosAplicaciones?
                    .Where(a => a.Aplicado && !a.Eliminado
                        && FechaAplicacionEnRango(a, desde, hastaFin, hospEnPeriodo, tieneVentaEnPeriodo))
                    .ToList() ?? new List<HospitalizacionProductoAplicacion>();
                if (!aplicadas.Any()) continue;

                decimal cantidad = aplicadas.Sum(a => a.Cantidad);
                decimal subtotalProd = cantidad * prod.PrecioValor;
                decimal? costoUnitario = ObtenerCostoUnitarioProducto(prod.ProductoId, prod.Producto, costoProductoCache);
                string nombre = prod.Producto?.NombreProducto ?? "Producto";
                agregarItem("Producto", nombre, cantidad, subtotalProd, costoUnitario, ambienteId);
                RegistrarMontoHospital("Producto", subtotalProd);
            }

            AgregarProductosConsolidadoInsumosDirectos(
                hosp.HospitalizacionInsumosDirectos,
                desde,
                hastaFin,
                hospEnPeriodo,
                tieneVentaEnPeriodo,
                agregarItem,
                ambienteId,
                subtotal => RegistrarMontoHospital("Producto", subtotal),
                costoProductoCache);

            foreach (var svc in hosp.HospitalizacionesServicios ?? Enumerable.Empty<HospitalizacionServicio>())
            {
                if (!ServicioHospitalizacionEnRango(svc, desde, hastaFin, hospEnPeriodo, tieneVentaEnPeriodo))
                    continue;

                decimal precioSvc = ObtenerPrecioUnitarioHospitalizacionServicio(svc);
                decimal subtotalSvc = Math.Round(svc.Cantidad * precioSvc, 2);
                string categoriaSvc = CategoriaMontosHospitalizacionServicio(svc);
                decimal? costoSvc = ObtenerCostoUnitarioServicio(svc.ServicioId, svc.Servicio);
                agregarItem(categoriaSvc, svc.Servicio?.NombreServicio ?? "Servicio", svc.Cantidad, subtotalSvc, costoSvc, ambienteId);
                RegistrarMontoHospital(categoriaSvc, subtotalSvc);
            }

            foreach (var examHosp in hosp.HospitalizacionesExamenes ?? Enumerable.Empty<HospitalizacionExamen>())
            {
                if (examHosp.Examen?.DetalleExamenes == null) continue;
                foreach (var det in examHosp.Examen.DetalleExamenes)
                {
                    var labClinico = det.ExamenLabClinico;
                    decimal? costoExam = ObtenerCostoUnitarioExamen(labClinico?.Id, labClinico);
                    if (!hospEnPeriodo && !tieneVentaEnPeriodo) continue;
                    agregarItem("Examen", labClinico?.NombreExamen ?? "Examen", det.Cantidad, det.PrecioValor, costoExam, ambienteId);
                    RegistrarMontoHospital("Examen", det.PrecioValor);
                }
            }

            foreach (var paq in hosp.HospitalizacionesPaquetesHospitalizacion ?? Enumerable.Empty<HospitalizacionPaqueteHospitalizacion>())
            {
                if (paq.Eliminado || paq.PaqueteHospitalizacion == null) continue;
                if (!FechaEnRango(paq.FechaHora, desde, hastaFin) && !hospEnPeriodo && !tieneVentaEnPeriodo) continue;

                decimal precioPaq = paq.PaqueteHospitalizacion.Precio ?? 0;
                decimal? costoPaq = ObtenerCostoPaqueteHospitalizacion(paq.PaqueteHospitalizacion);
                agregarItem("Paquete", paq.PaqueteHospitalizacion.NombrePaquete, 1, precioPaq, costoPaq, ambienteId);
                RegistrarMontoHospital("Paquete", precioPaq);
            }

            if (hospEnPeriodo || tieneVentaEnPeriodo)
            {
                decimal subGastosAdmin = _hospitalizacionRepository.GetGastosAdministrativos(hosp.Id)?.Sum(g => g.Monto) ?? 0;
                if (subGastosAdmin > 0)
                {
                    agregarItem("Hospital", "Gastos administrativos", 1, subGastosAdmin, 0m, ambienteId);
                    RegistrarMontoHospital("Hospital", subGastosAdmin);
                }

                decimal subHonorarios = _context.HospitalizacionHonorarios
                    .Where(h => h.HospitalizacionId == hosp.Id).Sum(h => h.Monto);
                if (subHonorarios > 0)
                {
                    agregarItem("Hospital", "Honorarios médicos", 1, subHonorarios, 0m, ambienteId);
                    RegistrarMontoHospital("Hospital", subHonorarios);
                }

                decimal subAmbulancias = _context.Ambulancias
                    .Where(a => a.HospitalizacionId == hosp.Id).Sum(a => a.Precio);
                if (subAmbulancias > 0)
                {
                    agregarItem("Hospital", "Ambulancia", 1, subAmbulancias, 0m, ambienteId);
                    RegistrarMontoHospital("Hospital", subAmbulancias);
                }
            }
        }

        private static decimal CalcularSubtotalEstadiaEnPeriodo(
            Hospitalizacion hosp, DateTime desde, DateTime hastaFin, out int noches)
        {
            noches = 0;
            if (hosp == null) return 0;

            var finHosp = hosp.Finalizada ? hosp.FechaFin.Date : DateTime.Now.Date;
            if (finHosp > hastaFin.Date) finHosp = hastaFin.Date;
            var inicioCobro = hosp.FechaInicio.Date > desde.Date ? hosp.FechaInicio.Date : desde.Date;
            var finCobro = finHosp;

            if (finCobro < inicioCobro) return 0;

            noches = (finCobro - inicioCobro).Days;
            if (noches < 1) noches = 1;

            decimal tarifaBase = hosp.CategoriaHabitacionTarifa?.ValorTarifa ?? 0;
            return tarifaBase * noches;
        }

        private static bool FechaEnRango(DateTime fecha, DateTime desde, DateTime hastaFin)
            => fecha >= desde && fecha <= hastaFin;

        private static bool FechaAplicacionEnRango(
            HospitalizacionProductoAplicacion a,
            DateTime desde,
            DateTime hastaFin,
            bool hospEnPeriodo = false,
            bool tieneVentaEnPeriodo = false)
        {
            if (hospEnPeriodo || tieneVentaEnPeriodo) return true;
            var f = a.FechaHoraAplicacionManual ?? a.FechaHoraAplicacion;
            return f.HasValue && FechaEnRango(f.Value, desde, hastaFin);
        }

        /// <summary>Precio unitario de un servicio en hospitalización (Precio o PrecioServicio.Valor).</summary>
        private static decimal ObtenerPrecioUnitarioHospitalizacionServicio(HospitalizacionServicio svc)
        {
            if (svc == null) return 0m;
            if (svc.Precio > 0) return svc.Precio;
            if (svc.PrecioServicio?.Valor > 0) return svc.PrecioServicio.Valor;
            return 0m;
        }

        /// <summary>Dieta y servicios clínicos van a columna Servicios; paquetes a Hospital.</summary>
        private static string CategoriaMontosHospitalizacionServicio(HospitalizacionServicio svc)
        {
            var cat = ClasificarCategoriaServicio(
                svc?.Servicio?.NombreServicio,
                svc?.Servicio?.CategoriaServicio?.NombreCategoria);
            return cat == "Dieta" ? "Servicio" : cat;
        }

        /// <summary>
        /// Incluye servicios de hospitalización en el reporte (misma regla que facturación/recibos: no eliminados).
        /// </summary>
        private static bool ServicioHospitalizacionEnRango(
            HospitalizacionServicio svc,
            DateTime desde,
            DateTime hastaFin,
            bool hospEnPeriodo,
            bool tieneVentaEnPeriodo)
        {
            if (svc == null || svc.Eliminado) return false;
            if (hospEnPeriodo || tieneVentaEnPeriodo) return true;
            if (svc.FechaHoraAplicacion.HasValue)
                return FechaEnRango(svc.FechaHoraAplicacion.Value, desde, hastaFin);
            return false;
        }

        private static bool FechaInsumoDirectoAplicacionEnRango(
            HospitalizacionInsumoDirectoAplicacion a,
            DateTime desde,
            DateTime hastaFin,
            bool hospEnPeriodo,
            bool tieneVentaEnPeriodo)
        {
            if (hospEnPeriodo || tieneVentaEnPeriodo) return true;
            if (a.FechaHoraAplicacion.HasValue)
                return FechaEnRango(a.FechaHoraAplicacion.Value, desde, hastaFin);
            return false;
        }

        private void AgregarProductosConsolidadoInsumosDirectos(
            IEnumerable<HospitalizacionInsumoDirecto> insumosDirectos,
            DateTime desde,
            DateTime hastaFin,
            bool hospEnPeriodo,
            bool tieneVentaEnPeriodo,
            Action<string, string, decimal, decimal, decimal?, int> agregarItem,
            int ambienteId,
            Action<decimal> registrarProductoHospital = null,
            Dictionary<int, decimal?> costoProductoCache = null)
        {
            foreach (var insumo in insumosDirectos ?? Enumerable.Empty<HospitalizacionInsumoDirecto>())
            {
                if (insumo.Eliminado) continue;

                var aplicadas = insumo.Aplicaciones?
                    .Where(a => a.Aplicado && FechaInsumoDirectoAplicacionEnRango(a, desde, hastaFin, hospEnPeriodo, tieneVentaEnPeriodo))
                    .ToList() ?? new List<HospitalizacionInsumoDirectoAplicacion>();
                if (!aplicadas.Any()) continue;

                decimal cantidad = aplicadas.Sum(a => a.Cantidad);
                decimal subtotalProd = aplicadas.Sum(a => a.Cantidad * insumo.PrecioValor);
                decimal? costoUnitario = ObtenerCostoUnitarioProducto(insumo.ProductoId, insumo.Producto, costoProductoCache);
                string nombre = insumo.Producto?.NombreProducto ?? "Insumo/Medicamento";
                agregarItem("Producto", nombre, cantidad, subtotalProd, costoUnitario, ambienteId);
                registrarProductoHospital?.Invoke(subtotalProd);
            }
        }

        private (decimal subProductos, decimal cantProductos, List<ReporteVentaDetalleItemViewModel> lineas)
            ConstruirDetalleInsumosDirectosHospitalizacion(
                IEnumerable<HospitalizacionInsumoDirecto> insumosDirectos,
                DateTime desde,
                DateTime hastaFin,
                bool hospEnPeriodo,
                bool tieneVentaEnPeriodo,
                Dictionary<int, decimal?> costoProductoCache = null)
        {
            var lineas = new List<ReporteVentaDetalleItemViewModel>();
            decimal subProductos = 0m;
            decimal cantProductos = 0m;

            foreach (var insumo in insumosDirectos ?? Enumerable.Empty<HospitalizacionInsumoDirecto>())
            {
                if (insumo.Eliminado) continue;

                var aplicadas = insumo.Aplicaciones?
                    .Where(a => a.Aplicado && FechaInsumoDirectoAplicacionEnRango(a, desde, hastaFin, hospEnPeriodo, tieneVentaEnPeriodo))
                    .ToList() ?? new List<HospitalizacionInsumoDirectoAplicacion>();
                if (!aplicadas.Any()) continue;

                decimal cantidad = aplicadas.Sum(a => a.Cantidad);
                decimal subtotalProd = aplicadas.Sum(a => a.Cantidad * insumo.PrecioValor);
                subProductos += subtotalProd;
                cantProductos += cantidad;

                decimal? costoUnitario = ObtenerCostoUnitarioProducto(insumo.ProductoId, insumo.Producto, costoProductoCache);
                lineas.Add(CrearDetalleItemVenta(
                    insumo.Producto?.NombreProducto ?? "Insumo/Medicamento",
                    "Producto",
                    cantidad,
                    insumo.PrecioValor,
                    subtotalProd,
                    0,
                    costoUnitario));
            }

            return (subProductos, cantProductos, lineas);
        }

        private decimal? ObtenerCostoUnitarioProducto(int productoId, Producto nav, Dictionary<int, decimal?> costoProductoCache = null)
        {
            if (productoId > 0 && costoProductoCache != null
                && costoProductoCache.TryGetValue(productoId, out var cached))
                return cached;

            if (productoId > 0)
            {
                var invRec = _context.ProductosInventario
                    .AsNoTracking()
                    .Where(i => i.ProductoId == productoId && !i.Eliminado)
                    .OrderByDescending(i => i.Id)
                    .Select(i => (decimal?)i.PrecioCosto)
                    .FirstOrDefault();
                if (invRec.HasValue && invRec.Value > 0) return invRec.Value;
            }
            if (nav?.PrecioCosto > 0) return nav.PrecioCosto;
            if (productoId > 0)
            {
                var prodBd = _productoRepository.GetProdutoById(productoId);
                if (prodBd?.PrecioCosto > 0) return prodBd.PrecioCosto;
            }
            return null;
        }

        private decimal? ObtenerCostoPaqueteHospitalizacion(PaqueteHospitalizacion paq)
        {
            if (paq == null) return null;
            if (paq.PrecioCosto > 0) return paq.PrecioCosto;

            var detalles = paq.DetallePaquetesHospitalizacion?
                .Where(d => !d.Eliminado)
                .ToList();
            if (detalles != null && detalles.Any())
            {
                decimal suma = detalles.Sum(d => d.PrecioCosto * d.Cantidad);
                if (suma > 0) return suma;
            }

            var sumaBd = _context.DetallePaqueteHospitalizacion
                .AsNoTracking()
                .Where(d => d.PaqueteHospitalizacionId == paq.Id && !d.Eliminado)
                .Sum(d => (decimal?)(d.PrecioCosto * d.Cantidad));
            return sumaBd > 0 ? sumaBd : null;
        }

        // ────────────────────────────────────────────────────────────
        // Método auxiliar para obtener el ID de hospitalización a partir de una venta
        // ────────────────────────────────────────────────────────────
        private int? ObtenerHospitalizacionIdPorVenta(Venta venta, Dictionary<int, int?> hospitalizacionPorVenta = null)
        {
            if (venta == null) return null;
            if (hospitalizacionPorVenta != null
                && hospitalizacionPorVenta.TryGetValue(venta.Id, out var cached))
                return cached;

            // 1. Intentar por DetalleCaja (vínculo con CuentaPorCobrar)
            var detalleCaja = _context.DetalleCajas
                .Include(dc => dc.CuentaPorCobrar)
                    .ThenInclude(cpc => cpc.DetallesCuentaPorCobrar)
                .FirstOrDefault(dc => dc.VentaId == venta.Id && dc.CuentaPorCobrarId != null);
            if (detalleCaja?.CuentaPorCobrar?.DetallesCuentaPorCobrar != null)
            {
                var hospId = detalleCaja.CuentaPorCobrar.DetallesCuentaPorCobrar
                    .Where(d => d.HospitalizacionId != null)
                    .Select(d => d.HospitalizacionId)
                    .FirstOrDefault();
                if (hospId != null) return hospId;
            }

            // 2. Cuenta por cobrar del paciente con hospitalización en la fecha de la venta
            if (venta.PacienteId != null)
            {
                var hospIdCpc = (
                    from d in _context.DetallesCuentaPorCobrar.AsNoTracking()
                    join cpc in _context.CuentasPorCobrar.AsNoTracking() on d.CuentaPorCobrarId equals cpc.Id
                    join h in _context.Hospitalizaciones.AsNoTracking() on d.HospitalizacionId equals h.Id
                    where d.HospitalizacionId != null
                       && cpc.PacienteId == venta.PacienteId
                       && !h.Eliminada
                       && h.FechaInicio <= venta.FechaVenta
                       && h.FechaFin >= venta.FechaVenta
                    orderby h.Id descending
                    select (int?)h.Id
                ).FirstOrDefault();
                if (hospIdCpc != null) return hospIdCpc;
            }

            // 3. Hospitalización del paciente en la fecha de la venta (activa o finalizada)
            if (venta.PacienteId != null)
            {
                var fechaVenta = venta.FechaVenta;
                var hospId = _context.Hospitalizaciones
                    .AsNoTracking()
                    .Where(h => h.PacienteId == venta.PacienteId
                             && !h.Eliminada
                             && h.FechaInicio <= fechaVenta
                             && (h.FechaFin >= fechaVenta
                                 || !h.Finalizada
                                 || (h.FechaHoraFinalizada != null && h.FechaHoraFinalizada >= fechaVenta)))
                    .OrderByDescending(h => h.Id)
                    .Select(h => (int?)h.Id)
                    .FirstOrDefault();
                if (hospId != null) return hospId;
            }

            return null;
        }

        /// <summary>Resuelve hospitalización por venta en lote (evita N+1 en reportes).</summary>
        private Dictionary<int, int?> PrecargarHospitalizacionIdPorVentas(List<Venta> ventasBd)
        {
            var resultado = new Dictionary<int, int?>();
            if (ventasBd == null || ventasBd.Count == 0) return resultado;

            var ventaIds = ventasBd.Select(v => v.Id).ToList();

            var detallesCaja = _context.DetalleCajas.AsNoTracking()
                .Where(dc => dc.VentaId != null && ventaIds.Contains(dc.VentaId.Value) && dc.CuentaPorCobrarId != null)
                .Select(dc => new { dc.VentaId, dc.CuentaPorCobrarId })
                .ToList();

            var cpcIds = detallesCaja
                .Where(d => d.CuentaPorCobrarId != null)
                .Select(d => d.CuentaPorCobrarId!.Value)
                .Distinct()
                .ToList();

            var hospPorCpc = cpcIds.Count == 0
                ? new Dictionary<int, int?>()
                : _context.DetallesCuentaPorCobrar.AsNoTracking()
                    .Where(d => cpcIds.Contains(d.CuentaPorCobrarId)
                        && d.HospitalizacionId != null)
                    .GroupBy(d => d.CuentaPorCobrarId)
                    .ToDictionary(g => g.Key, g => g.Select(d => d.HospitalizacionId).FirstOrDefault());

            foreach (var dc in detallesCaja)
            {
                if (dc.VentaId == null || dc.CuentaPorCobrarId == null) continue;
                if (hospPorCpc.TryGetValue(dc.CuentaPorCobrarId.Value, out var hospId) && hospId != null)
                    resultado[dc.VentaId.Value] = hospId;
            }

            var pendientes = ventasBd
                .Where(v => !resultado.ContainsKey(v.Id) && v.PacienteId != null)
                .ToList();
            if (pendientes.Count == 0) return resultado;

            var pacienteIds = pendientes.Select(v => v.PacienteId!.Value).Distinct().ToList();
            var hospitalizaciones = _context.Hospitalizaciones.AsNoTracking()
                .Where(h => pacienteIds.Contains(h.PacienteId) && !h.Eliminada)
                .OrderByDescending(h => h.Id)
                .Select(h => new
                {
                    h.Id,
                    h.PacienteId,
                    h.FechaInicio,
                    h.FechaFin,
                    h.Finalizada,
                    h.FechaHoraFinalizada
                })
                .ToList();

            foreach (var v in pendientes)
            {
                var fechaVenta = v.FechaVenta;
                var hospId = hospitalizaciones
                    .Where(h => h.PacienteId == v.PacienteId
                        && h.FechaInicio <= fechaVenta
                        && (h.FechaFin >= fechaVenta
                            || !h.Finalizada
                            || (h.FechaHoraFinalizada != null && h.FechaHoraFinalizada >= fechaVenta)))
                    .Select(h => (int?)h.Id)
                    .FirstOrDefault();
                resultado[v.Id] = hospId;
            }

            foreach (var v in ventasBd)
            {
                if (!resultado.ContainsKey(v.Id))
                    resultado[v.Id] = null;
            }

            return resultado;
        }

        /// <summary>Precarga costos de productos usados en ventas (evita consultas repetidas).</summary>
        private Dictionary<int, decimal?> PrecargarCostosProductos(List<Venta> ventasBd)
        {
            var productoIds = ventasBd?
                .SelectMany(v => v.DetalleVenta ?? Enumerable.Empty<DetalleVenta>())
                .Where(d => d.ProductoId.HasValue && d.ProductoId.Value > 0)
                .Select(d => d.ProductoId!.Value)
                .Distinct()
                .ToList() ?? new List<int>();

            if (productoIds.Count == 0) return new Dictionary<int, decimal?>();

            var costosInventario = _context.ProductosInventario.AsNoTracking()
                .Where(i => productoIds.Contains(i.ProductoId) && !i.Eliminado)
                .GroupBy(i => i.ProductoId)
                .Select(g => new
                {
                    ProductoId = g.Key,
                    PrecioCosto = g.OrderByDescending(i => i.Id).Select(i => (decimal?)i.PrecioCosto).FirstOrDefault()
                })
                .ToList()
                .ToDictionary(x => x.ProductoId, x => x.PrecioCosto);

            var faltantes = productoIds.Where(id => !costosInventario.ContainsKey(id) || !(costosInventario[id] > 0)).ToList();
            var costosProducto = faltantes.Count == 0
                ? new Dictionary<int, decimal?>()
                : _context.Productos.AsNoTracking()
                    .Where(p => faltantes.Contains(p.Id) && p.PrecioCosto > 0)
                    .ToDictionary(p => p.Id, p => (decimal?)p.PrecioCosto);

            var cache = new Dictionary<int, decimal?>();
            foreach (var id in productoIds)
            {
                if (costosInventario.TryGetValue(id, out var inv) && inv > 0)
                    cache[id] = inv;
                else if (costosProducto.TryGetValue(id, out var prod))
                    cache[id] = prod;
                else
                    cache[id] = null;
            }

            return cache;
        }

        // ────────────────────────────────────────────────────────────
        // Obtener médicos asociados a una hospitalización (tratante + secundarios)
        // Fuente 1: Consultas cargadas en hosp.Consultas → Citas
        // Fuente 2 (fallback): Cita más reciente del paciente en BD (CitaTipoAtencion = Hospitalización)
        // ────────────────────────────────────────────────────────────
        private List<ReporteMedicoViewModel> ObtenerMedicosDeHospitalizacion(Hospitalizacion hosp)
        {
            var medicos = new List<ReporteMedicoViewModel>();
            if (hosp == null) return medicos;

            // Helper local para agregar sin duplicados
            void AgregarMedico(string nombre, string especialidad, string rol)
            {
                if (string.IsNullOrWhiteSpace(nombre)) return;
                nombre = nombre.Trim();
                if (!medicos.Any(m => m.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                    medicos.Add(new ReporteMedicoViewModel { Rol = rol, Nombre = nombre, Especialidad = especialidad ?? "" });
            }

            string especialidadHospi = null;
            if (hosp.EspecialidadId != null)
            {
                especialidadHospi = _context.Especialidad
                    .AsNoTracking()
                    .Where(e => e.Id == hosp.EspecialidadId)
                    .Select(e => e.NombreEspecialidad)
                    .FirstOrDefault();
            }

            // ── Fuente 1: Consultas incluidas en el objeto hosp ──────────────────────────
            if (hosp.Consultas != null)
            {
                foreach (var consulta in hosp.Consultas)
                {
                    var cita = consulta.Citas;
                    if (cita == null) continue;

                    if (cita.EmpleadoId.HasValue)
                    {
                        var emp = ObtenerEmpleadoConEspecialidad(cita.EmpleadoId.Value);
                        if (emp != null)
                        {
                            var esp = ResolverEspecialidadEmpleado(emp, cita) ?? especialidadHospi;
                            AgregarMedico(emp.NombreYApellidos, esp, "Tratante");
                        }
                    }

                    if (cita.MedicosSecundarios != null)
                    {
                        foreach (var medId in cita.MedicosSecundarios)
                        {
                            var emp = ObtenerEmpleadoConEspecialidad(medId);
                            if (emp != null)
                                AgregarMedico(emp.NombreYApellidos, ResolverEspecialidadEmpleado(emp, cita) ?? especialidadHospi, "Secundario");
                        }
                    }
                }
            }

            // ── Fuente 2 (fallback): Buscar la cita de hospitalización del paciente en BD ─
            if (!medicos.Any() && hosp.PacienteId > 0)
            {
                var citaHospi = _context.Citass
                    .Include(c => c.Especialidad)
                    .AsNoTracking()
                    .Where(c => c.PacienteId == hosp.PacienteId
                             && !c.Eliminado
                             && c.CitaTipoAtencion == "Hospitalización")
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();

                if (citaHospi != null)
                {
                    if (citaHospi.EmpleadoId.HasValue)
                    {
                        var emp = ObtenerEmpleadoConEspecialidad(citaHospi.EmpleadoId.Value);
                        if (emp != null)
                            AgregarMedico(emp.NombreYApellidos, ResolverEspecialidadEmpleado(emp, citaHospi) ?? especialidadHospi, "Tratante");
                    }

                    if (citaHospi.MedicosSecundarios != null)
                    {
                        foreach (var medId in citaHospi.MedicosSecundarios)
                        {
                            var emp = ObtenerEmpleadoConEspecialidad(medId);
                            if (emp != null)
                                AgregarMedico(emp.NombreYApellidos, ResolverEspecialidadEmpleado(emp, citaHospi) ?? especialidadHospi, "Secundario");
                        }
                    }
                }
            }

            if (!medicos.Any() && !string.IsNullOrWhiteSpace(especialidadHospi))
                AgregarMedico("Médico de ingreso", especialidadHospi, "Ingreso");

            return medicos;
        }

        // ────────────────────────────────────────────────────────────
        // Obtener médicos asociados a una venta (cualquier ambiente)
        // Caso 1: Origen = Consulta/Cita → usar EmpleadoId de la venta
        // Caso 2: Inferir desde consulta del paciente en la misma fecha
        // Caso 3: Venta de Hospital sin consulta → buscar por hospitalización activa del paciente
        // ────────────────────────────────────────────────────────────
        private List<ReporteMedicoViewModel> ObtenerMedicosDeVenta(Venta v)
        {
            var medicos = new List<ReporteMedicoViewModel>();

            void AgregarMedico(string nombre, string especialidad, string rol)
            {
                if (string.IsNullOrWhiteSpace(nombre)) return;
                nombre = nombre.Trim();
                if (!medicos.Any(m => m.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                    medicos.Add(new ReporteMedicoViewModel { Rol = rol, Nombre = nombre, Especialidad = especialidad ?? "" });
            }

            // Cita del paciente en la fecha de la venta (médico tratante real, no el cajero)
            var cita = ObtenerCitaRelacionadaVenta(v);
            if (cita != null)
            {
                if (cita.EmpleadoId.HasValue)
                {
                    var emp = cita.Empleado ?? ObtenerEmpleadoConEspecialidad(cita.EmpleadoId.Value);
                    if (emp != null)
                        AgregarMedico(emp.NombreYApellidos, ResolverEspecialidadEmpleado(emp, cita), "Tratante");
                }

                if (cita.MedicosSecundarios != null)
                {
                    foreach (var medId in cita.MedicosSecundarios)
                    {
                        var emp = ObtenerEmpleadoConEspecialidad(medId);
                        if (emp != null)
                            AgregarMedico(emp.NombreYApellidos, ResolverEspecialidadEmpleado(emp, cita), "Secundario");
                    }
                }
            }

            // Consulta clínica vinculada (ingresos / exámenes con especialidad de la cita)
            if (!medicos.Any() && v.PacienteId != null)
            {
                var consulta = _context.Consultas
                    .Include(c => c.Citas).ThenInclude(c => c.Empleado).ThenInclude(e => e.Especialidad)
                    .Include(c => c.Citas).ThenInclude(c => c.Especialidad)
                    .AsNoTracking()
                    .Where(c => c.Citas.PacienteId == v.PacienteId
                             && c.FechaYHoraInicioConsulta.Date == v.FechaVenta.Date)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();

                if (consulta?.Citas != null)
                {
                    var citaConsulta = consulta.Citas;
                    if (citaConsulta.Empleado != null)
                        AgregarMedico(citaConsulta.Empleado.NombreYApellidos,
                            ResolverEspecialidadEmpleado(citaConsulta.Empleado, citaConsulta), "Tratante");
                    else if (citaConsulta.EmpleadoId.HasValue)
                    {
                        var emp = ObtenerEmpleadoConEspecialidad(citaConsulta.EmpleadoId.Value);
                        if (emp != null)
                            AgregarMedico(emp.NombreYApellidos, ResolverEspecialidadEmpleado(emp, citaConsulta), "Tratante");
                    }

                    if (citaConsulta.MedicosSecundarios != null)
                    {
                        foreach (var medId in citaConsulta.MedicosSecundarios)
                        {
                            var emp = ObtenerEmpleadoConEspecialidad(medId);
                            if (emp != null)
                                AgregarMedico(emp.NombreYApellidos, ResolverEspecialidadEmpleado(emp, citaConsulta), "Secundario");
                        }
                    }
                }
            }

            // Venta originada desde orden de examen — médico de ingreso solo si no hay tratante
            if (v.ExamenId != null && !medicos.Any(m => string.Equals(m.Rol, "Tratante", StringComparison.OrdinalIgnoreCase)))
            {
                var examen = _context.Examenes
                    .Include(e => e.Empleado).ThenInclude(em => em.Especialidad)
                    .AsNoTracking()
                    .FirstOrDefault(e => e.Id == v.ExamenId);
                if (examen?.Empleado != null)
                {
                    string espExamen = examen.Empleado.Especialidad?.NombreEspecialidad;
                    if (string.IsNullOrWhiteSpace(espExamen) && v.DetalleVenta != null)
                    {
                        foreach (var d in v.DetalleVenta)
                        {
                            espExamen = MapearEspecialidadDesdeNombreItem(
                                d.Servicio?.NombreServicio ?? d.ExamenLabClinico?.NombreExamen);
                            if (!string.IsNullOrWhiteSpace(espExamen)) break;
                        }
                    }
                    AgregarMedico(examen.Empleado.NombreYApellidos, espExamen, "Ingreso");
                }
            }

            // Hospitalización activa del paciente
            if (!medicos.Any() && v.PacienteId != null && v.AmbienteId == (int)AmbienteEnum.Hospital)
            {
                var hospActiva = _context.Hospitalizaciones
                    .Include(h => h.Consultas).ThenInclude(c => c.Citas)
                    .Include(h => h.Especialidad)
                    .AsNoTracking()
                    .Where(h => h.PacienteId == v.PacienteId
                             && h.FechaInicio.Date <= v.FechaVenta.Date)
                    .OrderByDescending(h => h.Id)
                    .FirstOrDefault();

                if (hospActiva != null)
                    medicos.AddRange(ObtenerMedicosDeHospitalizacion(hospActiva));
            }

            // Especialidad inferida del detalle (ej. "Ginecologia - Consulta", "2-CONSULTA MEDICA GENERAL")
            if (medicos.Any(m => string.IsNullOrWhiteSpace(m.Especialidad)) && v.DetalleVenta != null)
            {
                string espDetalle = null;
                foreach (var d in v.DetalleVenta)
                {
                    var nombre = d.Servicio?.NombreServicio ?? d.ExamenLabClinico?.NombreExamen;
                    espDetalle = MapearEspecialidadDesdeNombreItem(nombre);
                    if (!string.IsNullOrWhiteSpace(espDetalle)) break;
                }
                if (!string.IsNullOrWhiteSpace(espDetalle))
                {
                    foreach (var med in medicos.Where(m => string.IsNullOrWhiteSpace(m.Especialidad)))
                        med.Especialidad = espDetalle;
                }
            }

            return medicos;
        }

        // ────────────────────────────────────────────────────────────
        // Construir ítem desde detalle de venta (productos, servicios, exámenes)
        // ────────────────────────────────────────────────────────────
        private static decimal CalcularLineaNetaDetalle(DetalleVenta d)
        {
            if (d == null) return 0m;
            if (d.Subtotal > 0) return Convert.ToDecimal(d.Subtotal);
            if (d.Total > 0) return Convert.ToDecimal(d.Total);
            decimal bruto = Convert.ToDecimal(d.Precio) * Convert.ToDecimal(d.Cantidad);
            decimal desc = Convert.ToDecimal(d.Descuento);
            decimal neto = bruto - desc;
            return neto > 0 ? neto : (bruto > 0 ? bruto : 0m);
        }

        private static decimal ObtenerMontoCobradoVenta(
            Venta v,
            IReadOnlyDictionary<int, decimal> ingresosCajaPorVentaId = null)
        {
            decimal monto = v.Pagos?
                .Where(p => !p.Eliminado)
                .Sum(p => p.Monto) ?? 0m;
            if (monto <= 0 && v.Pagos != null && v.Pagos.Any())
                monto = v.Pagos.Sum(p => p.Monto);

            if (monto <= 0 && v.MontoPago > 0)
                monto = v.MontoPago;

            if (monto <= 0
                && ingresosCajaPorVentaId != null
                && ingresosCajaPorVentaId.TryGetValue(v.Id, out var ingresoCaja)
                && ingresoCaja > 0)
                monto = ingresoCaja;

            return monto;
        }

        private ReporteVentaItemViewModel ConstruirItemDesdeDetalleVenta(
            Venta v,
            string modoDetalle,
            Action<string, string, decimal, decimal, decimal?, int> agregarItem,
            IProducto productoRepo,
            int ambienteId,
            IReadOnlyDictionary<int, decimal> ingresosCajaPorVentaId = null)
        {
            decimal subProductos = 0m, subServicios = 0m, subExamenes = 0m, subHospital = 0m;
            decimal totalDesc = 0m, cantProductos = 0m, cantServicios = 0m, cantExamenes = 0m, cantHospital = 0m;
            var detalleItems = new List<ReporteVentaDetalleItemViewModel>();

            if (v.DetalleVenta != null)
            {
                foreach (var d in v.DetalleVenta)
                {
                    decimal lineaNeta = CalcularLineaNetaDetalle(d);
                    decimal cantidad = Convert.ToDecimal(d.Cantidad);
                    totalDesc += Convert.ToDecimal(d.Descuento);

                    string categoria;
                    decimal? costoUnitario = null;

                    if (d.BienOServicio == "B" && d.ProductoId != null)
                    {
                        categoria = "Producto";
                        subProductos += lineaNeta;
                        cantProductos += cantidad;

                        if (d.ProductoId.HasValue)
                        {
                            var invRec = _context.ProductosInventario
                                .Where(i => i.ProductoId == d.ProductoId.Value && !i.Eliminado)
                                .OrderByDescending(i => i.Id)
                                .Select(i => (decimal?)i.PrecioCosto)
                                .FirstOrDefault();
                            if (invRec.HasValue && invRec.Value > 0)
                                costoUnitario = invRec.Value;
                        }
                        if (!costoUnitario.HasValue || costoUnitario.Value == 0)
                        {
                            if (d.Producto?.PrecioCosto > 0)
                                costoUnitario = d.Producto.PrecioCosto;
                            else if (d.ProductoId.HasValue)
                            {
                                var prod = productoRepo.GetProdutoById(d.ProductoId.Value);
                                if (prod?.PrecioCosto > 0) costoUnitario = prod.PrecioCosto;
                            }
                        }
                    }
                    else if (d.BienOServicio == "S" && d.ExamenLabClinicoId != null)
                    {
                        categoria = d.ExamenLabClinico?.CategoriaLabClinico?.Nombre ?? "Examen";
                        subExamenes += lineaNeta;
                        cantExamenes += cantidad;
                        costoUnitario = ObtenerCostoUnitarioExamen(d.ExamenLabClinicoId, d.ExamenLabClinico);
                    }
                    else if (d.BienOServicio == "S" && d.ServicioId != null)
                    {
                        categoria = ClasificarCategoriaServicio(
                            d.Servicio?.NombreServicio,
                            d.Servicio?.CategoriaServicio?.NombreCategoria);
                        subServicios += lineaNeta;
                        cantServicios += cantidad;
                        costoUnitario = ObtenerCostoUnitarioServicio(d.ServicioId, d.Servicio);
                    }
                    else
                    {
                        categoria = d.BienOServicio == "B" ? "Producto" : "Servicio";
                        subServicios += lineaNeta;
                        cantServicios += cantidad;
                    }

                    string nombre = d.Producto?.NombreProducto ?? d.Servicio?.NombreServicio ?? d.ExamenLabClinico?.NombreExamen ?? categoria;
                    string categoriaConsolidado = d.ExamenLabClinicoId != null ? "Examen" : categoria;
                    agregarItem(categoriaConsolidado, nombre, cantidad, lineaNeta, costoUnitario, ambienteId);

                    detalleItems.Add(CrearDetalleItemVenta(
                        nombre,
                        categoria,
                        cantidad,
                        cantidad > 0 ? lineaNeta / cantidad : lineaNeta,
                        lineaNeta,
                        Convert.ToDecimal(d.Descuento),
                        costoUnitario));

                    if (modoDetalle == "descripcion" && d.BienOServicio == "S" && d.ServicioId != null)
                        detalleItems.AddRange(ObtenerDetalleInsumosServicio(d.ServicioId, cantidad));
                }
            }

            CompletarMontosVentaSinDetalle(
                v, ambienteId, ref subProductos, ref subServicios, ref subExamenes, ref subHospital,
                ref cantProductos, ref cantServicios, ref cantExamenes, ref cantHospital,
                detalleItems, agregarItem, ingresosCajaPorVentaId);

            decimal totalBruto = subProductos + subServicios + subExamenes + subHospital + totalDesc;

            return new ReporteVentaItemViewModel
            {
                VentaId = v.Id,
                FechaVenta = v.FechaVenta,
                NumeroComprobante = v.NoComprobante ?? v.UuidFel ?? $"#{v.Id}",
                ClienteNombre = v.Clientes?.Nombre ?? v.Nombres ?? "Consumidor Final",
                ClienteNit = v.Clientes?.Nit ?? v.Nit ?? "CF",
                EmpleadoNombre = v.Empleado?.NombreYApellidos ?? "",
                TipoVenta = v.TipoVenta ?? "",
                Origen = v.Origen ?? "",
                MontoTotal = Math.Round(totalBruto, 2),
                TotalDescuento = totalDesc,
                SubtotalProductos = Math.Round(subProductos, 2),
                SubtotalServicios = Math.Round(subServicios, 2),
                SubtotalExamenes = Math.Round(subExamenes, 2),
                SubtotalHospital = Math.Round(subHospital, 2),
                FormasPagoTexto = v.Pagos?.Select(p => p.FormaPago?.NombreFormaPago ?? "–").ToList() ?? new List<string>(),
                DetalleItems = detalleItems,
                MedicosAsignados = new List<ReporteMedicoViewModel>(),
                CantidadItemsProductos = cantProductos,
                CantidadItemsServicios = cantServicios,
                CantidadItemsExamenes = cantExamenes,
                CantidadItemsHospital = cantHospital,
            };
        }

        /// <summary>
        /// Ventas de caja sin líneas en DetalleVentas (pagos de cita, abonos, etc.): usa monto cobrado y cita/examen vinculado.
        /// </summary>
        private void AplicarFallbackMontoCobradoSiVacio(
            ReporteVentaItemViewModel item,
            Venta v,
            int ambienteId,
            IReadOnlyDictionary<int, decimal> ingresosCajaPorVentaId,
            Action<string, string, decimal, decimal, decimal?, int> agregarItem)
        {
            if (item == null || v == null) return;
            if (item.SubtotalProductos + item.SubtotalServicios + item.SubtotalExamenes + item.SubtotalHospital > 0)
                return;

            decimal subProductos = item.SubtotalProductos;
            decimal subServicios = item.SubtotalServicios;
            decimal subExamenes = item.SubtotalExamenes;
            decimal subHospital = item.SubtotalHospital;
            decimal cantProductos = item.CantidadItemsProductos;
            decimal cantServicios = item.CantidadItemsServicios;
            decimal cantExamenes = item.CantidadItemsExamenes;
            decimal cantHospital = item.CantidadItemsHospital;
            var detalleItems = item.DetalleItems?.ToList() ?? new List<ReporteVentaDetalleItemViewModel>();

            CompletarMontosVentaSinDetalle(
                v, ambienteId, ref subProductos, ref subServicios, ref subExamenes, ref subHospital,
                ref cantProductos, ref cantServicios, ref cantExamenes, ref cantHospital,
                detalleItems, agregarItem, ingresosCajaPorVentaId);

            if (subProductos + subServicios + subExamenes + subHospital <= 0)
                return;

            item.SubtotalProductos = Math.Round(subProductos, 2);
            item.SubtotalServicios = Math.Round(subServicios, 2);
            item.SubtotalExamenes = Math.Round(subExamenes, 2);
            item.SubtotalHospital = Math.Round(subHospital, 2);
            item.CantidadItemsProductos = cantProductos;
            item.CantidadItemsServicios = cantServicios;
            item.CantidadItemsExamenes = cantExamenes;
            item.CantidadItemsHospital = cantHospital;
            item.DetalleItems = detalleItems;
            item.MontoTotal = Math.Round(
                subProductos + subServicios + subExamenes + subHospital + item.TotalDescuento, 2);
        }

        private void CompletarMontosVentaSinDetalle(
            Venta v,
            int ambienteId,
            ref decimal subProductos,
            ref decimal subServicios,
            ref decimal subExamenes,
            ref decimal subHospital,
            ref decimal cantProductos,
            ref decimal cantServicios,
            ref decimal cantExamenes,
            ref decimal cantHospital,
            List<ReporteVentaDetalleItemViewModel> detalleItems,
            Action<string, string, decimal, decimal, decimal?, int> agregarItem,
            IReadOnlyDictionary<int, decimal> ingresosCajaPorVentaId = null)
        {
            if (subProductos + subServicios + subExamenes + subHospital > 0)
                return;

            decimal montoCobrado = ObtenerMontoCobradoVenta(v, ingresosCajaPorVentaId);
            if (montoCobrado <= 0)
                return;

            int amb = v.AmbienteId ?? ambienteId;
            string nombre = "Pago registrado";
            string categoria = "Servicio";
            string categoriaConsolidado = "Servicio";
            decimal? costoUnitario = null;

            if (v.ExamenId != null)
            {
                var examen = _context.Examenes
                    .AsNoTracking()
                    .Include(e => e.DetalleExamenes).ThenInclude(d => d.ExamenLabClinico)
                    .FirstOrDefault(e => e.Id == v.ExamenId);

                if (examen?.DetalleExamenes != null && examen.DetalleExamenes.Any())
                {
                    var primer = examen.DetalleExamenes.First();
                    nombre = primer.ExamenLabClinico?.NombreExamen ?? "Examen de laboratorio";
                    categoria = primer.ExamenLabClinico?.CategoriaLabClinico?.Nombre ?? "Examen";
                    categoriaConsolidado = "Examen";
                    costoUnitario = ObtenerCostoUnitarioExamen(primer.ExamenLabClinicoId, primer.ExamenLabClinico);
                    subExamenes = montoCobrado;
                    cantExamenes = 1;
                }
            }

            if (subProductos + subServicios + subExamenes + subHospital == 0)
            {
                var cita = ObtenerCitaRelacionadaVenta(v);
                if (cita != null)
                {
                    var svcCita = cita.CitasServicios?
                        .Where(s => !s.Eliminado && s.Servicio != null)
                        .FirstOrDefault();
                    if (svcCita?.Servicio != null)
                        nombre = svcCita.Servicio.NombreServicio;
                    else if (cita.Servicio != null)
                        nombre = cita.Servicio.NombreServicio;
                }

                if (amb == (int)AmbienteEnum.Farmacia)
                {
                    categoria = "Producto";
                    categoriaConsolidado = "Producto";
                    subProductos = montoCobrado;
                    cantProductos = 1;
                }
                else if (amb == (int)AmbienteEnum.Laboratorio || v.ExamenId != null)
                {
                    categoria = "Examen";
                    categoriaConsolidado = "Examen";
                    subExamenes = montoCobrado;
                    cantExamenes = 1;
                }
                else if (amb == (int)AmbienteEnum.Hospital
                    || (v.TipoVenta?.IndexOf("hospitalizacion", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    categoria = "Hospital";
                    categoriaConsolidado = "Hospital";
                    subHospital = montoCobrado;
                    cantHospital = 1;
                }
                else
                {
                    categoria = "Servicio";
                    categoriaConsolidado = "Servicio";
                    subServicios = montoCobrado;
                    cantServicios = 1;
                }
            }

            agregarItem(categoriaConsolidado, nombre, 1, montoCobrado, costoUnitario, ambienteId);
            detalleItems.Add(CrearDetalleItemVenta(nombre, categoria, 1, montoCobrado, montoCobrado, 0, costoUnitario));
        }





        // ────────────────────────────────────────────────────────────
        // IMPRESIÓN DE NOTAS DE ENFERMERÍA (INDIVIDUAL Y TODAS)
        // ────────────────────────────────────────────────────────────

        public async Task<IActionResult> NotaEnfermeriaPDF(int id)
        {
            if (id == 0) return StatusCode(404);

            var nota = await _context.NotaEnfermeria2
                .Include(n => n.User)
                    .ThenInclude(u => u.Persona)
                .Include(n => n.Hospitalizacion)
                    .ThenInclude(h => h.Paciente)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (nota == null) return StatusCode(400);

            // Obtener el nombre del profesional que escribió la nota
            string nombreProfesional = PdfReportHelper.ObtenerNombreEmpleadoPorUser(nota.User, _empleadoRepository, _userRepository);

            User usuarioFirma = null;
            if (!string.IsNullOrWhiteSpace(nota.UsuarioFirmaId))
            {
                usuarioFirma = await _context.Users.AsNoTracking()
                    .Include(u => u.Persona)
                    .FirstOrDefaultAsync(u => u.Id == nota.UsuarioFirmaId);
            }

            string firmadoPor = nota.Firmado
                ? PdfReportHelper.ObtenerNombreEmpleadoPorUser(usuarioFirma ?? nota.User, _empleadoRepository, _userRepository)
                : null;

            // Obtener la firma si está firmada y convertir a base64
            string firmaBase64 = null;
            if (nota.Firmado && !string.IsNullOrEmpty(nota.FirmaRuta))
            {
                string directorio = ContentRoot;
                string rutaRelativa = nota.FirmaRuta.TrimStart('/');
                string rutaFinal = Path.Combine(directorio, "wwwroot", rutaRelativa);
                if (System.IO.File.Exists(rutaFinal))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(rutaFinal);
                    firmaBase64 = $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
                }
            }

            var model = new NotaEnfermeriaPdfViewModel
            {
                Id = nota.Id,
                Diagnostico = nota.Diagnostico ?? "",
                FechaRegistro = nota.FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss"),
                Profesional = nombreProfesional,
                Firmado = nota.Firmado,
                FirmaBase64 = firmaBase64,
                FechaFirma = nota.FechaFirma?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                FirmadoPor = firmadoPor,
                PacienteNombre = nota.Hospitalizacion?.Paciente?.Nombre ?? "No especificado",
                HospitalizacionId = nota.HospitalizacionId,
                // Datos del establecimiento desde configuración
                EstablecimientoDireccion = _configuration["EstablecimientoDireccion"],
                EstablecimientoTelefono = _configuration["EstablecimientoTelefono"],
                EstablecimientoCorreo = _configuration["EstablecimientoCorreoElectronico"],
                ImagenLogoBase64 = _configuration["ImagenLogoBase64"]
            };

            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };
            _generatePdf.SetConvertOptions(options);

            return await RenderPdfAsync("Views/CrearPDF/NotaEnfermeriaPDF.cshtml", model);
        }

        public async Task<IActionResult> NotasEnfermeriaAllPDF(int hospitalizacionId)
        {
            if (hospitalizacionId == 0) return StatusCode(400);

            var notas = await _context.NotaEnfermeria2
                .Include(n => n.User)
                    .ThenInclude(u => u.Persona)
                .Include(n => n.Hospitalizacion)
                    .ThenInclude(h => h.Paciente)
                .Where(n => n.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(n => n.FechaRegistro)
                .ToListAsync();

            if (notas == null || !notas.Any())
                return NotFound("No hay notas de enfermería para esta hospitalización.");

            var paciente = notas.First().Hospitalizacion?.Paciente;
            var listaNotas = new List<NotaEnfermeriaPdfViewModel>();

            foreach (var nota in notas)
            {
                string nombreProfesional = PdfReportHelper.ObtenerNombreEmpleadoPorUser(nota.User, _empleadoRepository, _userRepository);
                User usuarioFirma = null;
                if (!string.IsNullOrWhiteSpace(nota.UsuarioFirmaId))
                {
                    usuarioFirma = await _context.Users.AsNoTracking()
                        .Include(u => u.Persona)
                        .FirstOrDefaultAsync(u => u.Id == nota.UsuarioFirmaId);
                }
                string firmadoPor = nota.Firmado
                    ? PdfReportHelper.ObtenerNombreEmpleadoPorUser(usuarioFirma ?? nota.User, _empleadoRepository, _userRepository)
                    : null;
                string firmaBase64 = null;
                if (nota.Firmado && !string.IsNullOrEmpty(nota.FirmaRuta))
                {
                    string directorio = ContentRoot;
                    string rutaRelativa = nota.FirmaRuta.TrimStart('/');
                    string rutaFinal = Path.Combine(directorio, "wwwroot", rutaRelativa);
                    if (System.IO.File.Exists(rutaFinal))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(rutaFinal);
                        firmaBase64 = $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
                    }
                }

                listaNotas.Add(new NotaEnfermeriaPdfViewModel
                {
                    Id = nota.Id,
                    Diagnostico = nota.Diagnostico ?? "",
                    FechaRegistro = nota.FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss"),
                    Profesional = nombreProfesional,
                    Firmado = nota.Firmado,
                    FirmaBase64 = firmaBase64,
                    FechaFirma = nota.FechaFirma?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                    FirmadoPor = firmadoPor,
                    PacienteNombre = nota.Hospitalizacion?.Paciente?.Nombre ?? "No especificado",
                    HospitalizacionId = nota.HospitalizacionId,
                    EstablecimientoDireccion = _configuration["EstablecimientoDireccion"],
                    EstablecimientoTelefono = _configuration["EstablecimientoTelefono"],
                    EstablecimientoCorreo = _configuration["EstablecimientoCorreoElectronico"],
                    ImagenLogoBase64 = _configuration["ImagenLogoBase64"]
                });
            }

            var model = new NotasEnfermeriaAllPdfViewModel
            {
                HospitalizacionId = hospitalizacionId,
                PacienteNombre = paciente?.Nombre ?? "No especificado",
                FechaGeneracion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Notas = listaNotas,
                EstablecimientoDireccion = _configuration["EstablecimientoDireccion"],
                EstablecimientoTelefono = _configuration["EstablecimientoTelefono"],
                EstablecimientoCorreo = _configuration["EstablecimientoCorreoElectronico"],
                ImagenLogoBase64 = _configuration["ImagenLogoBase64"]
            };

            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };
            _generatePdf.SetConvertOptions(options);

            return await RenderPdfAsync("Views/CrearPDF/NotasEnfermeriaAllPDF.cshtml", model);
        }

        private async Task<HospitalizacionInformePdfSectionsVm> ConstruirInformeHospitalizacionSeccionesAsync(
            Hospitalizacion hospitalizacion)
        {
            var hospitalizacionId = hospitalizacion.Id;
            var contentRoot = ContentRoot;
            var medico = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                hospitalizacion, _empleadoRepository, contentRoot);

            var secciones = new HospitalizacionInformePdfSectionsVm
            {
                SignosVitales = await MapSignosVitalesHospitalizacionPdfAsync(hospitalizacionId, hospitalizacion)
            };

            var glucometriaDetalles = await _context.DetalleControlGlucometria2
                .AsNoTracking()
                .Include(x => x.ControlGlucometria2)
                .Include(x => x.User).ThenInclude(u => u.Persona)
                .Include(x => x.Profesional).ThenInclude(u => u.Persona)
                .Where(x => x.ControlGlucometria2.HospitalizacionId == hospitalizacionId && !x.Eliminado)
                .OrderByDescending(x => x.ControlGlucometria2.FechaHora)
                .ToListAsync();

            var glucometriaUserIds = glucometriaDetalles
                .Where(x => x.ControlGlucometria2.Autorizado && !string.IsNullOrWhiteSpace(x.ControlGlucometria2.UsuarioAutoriza))
                .Select(x => x.ControlGlucometria2.UsuarioAutoriza)
                .Distinct()
                .ToList();
            var glucometriaAutorizadores = await ObtenerUsuariosAutorizadoresAsync(glucometriaUserIds);

            secciones.Glucometria = glucometriaDetalles.Select(x =>
            {
                var ctrl = x.ControlGlucometria2;
                var firmante = PdfReportHelper.ResolverFirmanteClinico(
                    ctrl.Autorizado,
                    ctrl.UsuarioAutoriza,
                    x.Profesional,
                    glucometriaAutorizadores,
                    _empleadoRepository,
                    medico.FirmaBase64,
                    contentRoot,
                    PdfReportHelper.ObtenerNombreEmpleadoPorUser(x.Profesional, _empleadoRepository, _userRepository));

                return new InformeGlucometriaPdfRow
                {
                    FechaHora = ctrl.FechaHora.ToString("dd/MM/yyyy HH:mm"),
                    GMT = ctrl.GMT ?? "-",
                    InsulinaNombre = ctrl.Insulina ?? "-",
                    Unidades = ctrl.Unidades ?? "-",
                    FirmaTexto = !string.IsNullOrWhiteSpace(firmante.NombreFirmante) ? firmante.NombreFirmante : (ctrl.Firma ?? "-"),
                    FirmaBase64 = firmante.FirmaBase64,
                    Aplicado = x.Aplicado,
                    FechaHoraAplicacion = x.FechaAplicacion?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                    NombrePersonaAplica = PdfReportHelper.ObtenerNombreEmpleadoPorUser(x.User, _empleadoRepository, _userRepository),
                    NombreProfesional = PdfReportHelper.ObtenerNombreEmpleadoPorUser(x.Profesional, _empleadoRepository, _userRepository),
                    Autorizado = ctrl.Autorizado,
                    AutorizadoPor = firmante.AutorizadoPor
                };
            }).ToList();

            var ingestas = await _context.IngestaExcreta2
                .AsNoTracking()
                .Include(x => x.User).ThenInclude(u => u.Persona)
                .Where(x => x.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(x => x.FechaRegistro)
                .ToListAsync();

            secciones.IngestasExcretas = ingestas.Select(nota => new IngestaExcretaViewModel
            {
                FechaRegistro = nota.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                IngestaIV = nota.IngestaIV,
                IngestaPO = nota.IngestaPO,
                TotalIngesta = nota.TotalIngesta,
                Excreta = nota.Excreta,
                Heces = nota.Heces,
                CuantasHoras = nota.CuantasHoras,
                Enfermeria = PdfReportHelper.ObtenerNombreEmpleadoPorUser(nota.User, _empleadoRepository, _userRepository)
            }).ToList();

            var notasMedicas = _hospitalizacionRepository.GetNotasMedicasByHospitalizacion(hospitalizacionId)
                ?.OrderByDescending(n => n.FechaRegistro)
                .ToList() ?? new List<NotaMedica2>();

            var notaAutorizadorIds = notasMedicas
                .Where(n => n.Autorizado && !string.IsNullOrWhiteSpace(n.UsuarioAutoriza))
                .Select(n => n.UsuarioAutoriza)
                .Distinct()
                .ToList();
            var notaAutorizadores = await ObtenerUsuariosAutorizadoresAsync(notaAutorizadorIds);

            secciones.NotasEvolucion = notasMedicas.Select(n =>
            {
                var registradoPor = n.Profesional?.Persona?.NombreYApellidos ?? n.Profesional?.UserName ?? "-";
                var firmante = PdfReportHelper.ResolverFirmanteClinico(
                    n.Autorizado,
                    n.UsuarioAutoriza,
                    n.Profesional,
                    notaAutorizadores,
                    _empleadoRepository,
                    medico.FirmaBase64,
                    contentRoot,
                    registradoPor);

                return new InformeNotaSimplePdfRow
                {
                    FechaRegistro = n.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                    Profesional = registradoPor,
                    Descripcion = PdfReportHelper.TextoPlanoDesdeHtml(n.Diagnostico),
                    Firmado = n.Autorizado,
                    FirmadoPor = firmante.AutorizadoPor ?? firmante.NombreFirmante,
                    FirmaBase64 = firmante.FirmaBase64
                };
            }).ToList();

            var productosAplicacion = _hospitalizacionRepository.GetProductosAplicacion(hospitalizacionId);
            var userIds = productosAplicacion
                .SelectMany(p => new[] { p.UsuarioAplica, p.UsuarioCreaId })
                .Where(id => id != null)
                .Distinct()
                .ToList();
            var usuarios = _userRepository.GetByIds(userIds).ToDictionary(u => u.Id, u => u);
            var empleadoIds = usuarios.Values
                .Select(u => u.EmpleadoId)
                .Where(id => id != null)
                .Distinct()
                .Select(id => (int)id)
                .ToList();
            var empleados = _empleadoRepository.GetByIds(empleadoIds).ToDictionary(e => e.Id, e => e);

            foreach (var productoAplicacion in productosAplicacion)
            {
                if (!productoAplicacion.Aplicado
                    || productoAplicacion.HospitalizacionProducto?.Eliminado == true)
                    continue;

                string personaAplica = "-";
                if (productoAplicacion.UsuarioAplica != null && usuarios.TryGetValue(productoAplicacion.UsuarioAplica, out var userAplica))
                {
                    personaAplica = userAplica.EmpleadoId != null && empleados.TryGetValue((int)userAplica.EmpleadoId, out var empAplica)
                        ? empAplica.NombreYApellidos
                        : PdfReportHelper.ObtenerNombreEmpleadoPorUser(userAplica, _empleadoRepository, _userRepository);
                }

                string personaCrea = "-";
                if (usuarios.TryGetValue(productoAplicacion.UsuarioCreaId, out var userCrea))
                {
                    personaCrea = userCrea.EmpleadoId != null && empleados.TryGetValue((int)userCrea.EmpleadoId, out var empCrea)
                        ? empCrea.NombreYApellidos
                        : PdfReportHelper.ObtenerNombreEmpleadoPorUser(userCrea, _empleadoRepository, _userRepository);
                }

                secciones.Aplicaciones.Add(new InformeAplicacionPdfRow
                {
                    TipoElemento = "Medicamento",
                    Nombre = productoAplicacion.HospitalizacionProducto?.Producto?.NombreProducto ?? "-",
                    Indicaciones = productoAplicacion.HospitalizacionProducto?.Indicaciones ?? "-",
                    Cantidad = productoAplicacion.Cantidad,
                    UnidadMedidaVentaNombre = productoAplicacion.HospitalizacionProducto?.UnidadMedidaVenta?.Nombre ?? "-",
                    OrdenMedicaNumero = "-",
                    PersonaCrea = personaCrea,
                    Aplicado = productoAplicacion.Aplicado,
                    FechaHoraAplicacion = productoAplicacion.Aplicado && productoAplicacion.FechaHoraAplicacion.HasValue
                        ? productoAplicacion.FechaHoraAplicacion.Value.ToString("dd/MM/yyyy HH:mm")
                        : "-",
                    PersonaAplica = personaAplica
                });
            }

            if (hospitalizacion.HospitalizacionesServicios != null)
            {
                foreach (var servicio in hospitalizacion.HospitalizacionesServicios.Where(s => !s.Eliminado && s.Aplicado))
                {
                    string personaAplica = "-";
                    if (!string.IsNullOrWhiteSpace(servicio.UsuarioAplica))
                    {
                        var userAplica = _userRepository.GetbyId(servicio.UsuarioAplica);
                        personaAplica = PdfReportHelper.ObtenerNombreEmpleadoPorUser(userAplica, _empleadoRepository, _userRepository);
                    }

                    secciones.Aplicaciones.Add(new InformeAplicacionPdfRow
                    {
                        TipoElemento = "Servicio",
                        Nombre = servicio.Servicio?.NombreServicio ?? "-",
                        Cantidad = servicio.Cantidad,
                        PersonaCrea = "-",
                        Aplicado = servicio.Aplicado,
                        FechaHoraAplicacion = servicio.FechaHoraAplicacion?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                        PersonaAplica = personaAplica
                    });
                }
            }

            var notasEnfermeria = await _context.NotaEnfermeria2
                .AsNoTracking()
                .Include(n => n.User).ThenInclude(u => u.Persona)
                .Where(n => n.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(n => n.FechaRegistro)
                .ToListAsync();

            var firmantesNotas = await ObtenerUsuariosAutorizadoresAsync(
                notasEnfermeria.Where(n => !string.IsNullOrWhiteSpace(n.UsuarioFirmaId)).Select(n => n.UsuarioFirmaId));

            secciones.NotasEnfermeria = notasEnfermeria.Select(n =>
            {
                var registradoPor = PdfReportHelper.ObtenerNombreEmpleadoPorUser(n.User, _empleadoRepository, _userRepository);
                User firmanteUser = null;
                if (!string.IsNullOrWhiteSpace(n.UsuarioFirmaId))
                    firmantesNotas.TryGetValue(n.UsuarioFirmaId, out firmanteUser);

                var firmadoPor = n.Firmado
                    ? PdfReportHelper.ObtenerNombreEmpleadoPorUser(firmanteUser ?? n.User, _empleadoRepository, _userRepository)
                    : null;

                return new InformeNotaSimplePdfRow
                {
                    FechaRegistro = n.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                    Profesional = registradoPor,
                    Descripcion = PdfReportHelper.TextoPlanoDesdeHtml(n.Diagnostico),
                    Firmado = n.Firmado,
                    FirmadoPor = firmadoPor,
                    FirmaBase64 = n.Firmado && !string.IsNullOrEmpty(n.FirmaRuta)
                        ? PdfReportHelper.ObtenerFirmaBase64Local(n.FirmaRuta, contentRoot)
                        : null
                };
            }).ToList();

            secciones.Examenes = hospitalizacion.HospitalizacionesExamenes?
                .Where(e => !e.Eliminado)
                .OrderByDescending(e => e.FechaHora)
                .Select(e => new InformeExamenPdfRow
                {
                    FechaHora = e.FechaHora.ToString("dd/MM/yyyy HH:mm"),
                    Nombre = e.Examen?.DetalleExamenes != null && e.Examen.DetalleExamenes.Any()
                        ? string.Join(", ", e.Examen.DetalleExamenes.Select(d => d.ExamenLabClinico?.NombreExamen ?? "-"))
                        : (e.Examen?.NumeroOrden ?? "-")
                }).ToList() ?? new List<InformeExamenPdfRow>();

            return secciones;
        }

        private ConsentimientoHospiVM ResolverConsentimientoHospitalizacion(Hospitalizacion hospitalizacion)
        {
            if (hospitalizacion == null)
                return null;

            var hospiIdStr = hospitalizacion.Id.ToString();
            var consentimiento = _consentimientoHospiService.GetConsentimientoByPacienteHabitacionAndHospitalizacion(
                    hospitalizacion.PacienteId, hospitalizacion.HabitacionId, hospiIdStr)
                ?? _consentimientoHospiService.GetConsentimientoByPacienteAndHospitalizacion(
                    hospitalizacion.PacienteId, hospiIdStr)
                ?? _consentimientoHospiService.GetConsentimientoByPacienteAndHabitacion(
                    hospitalizacion.PacienteId, hospitalizacion.HabitacionId)
                ?? _consentimientoHospiService.GetLatestConsentimientoByPaciente(hospitalizacion.PacienteId);

            if (consentimiento != null && string.IsNullOrWhiteSpace(consentimiento.HospitalizacionId))
            {
                _consentimientoHospiService.UpdateHospitalizacionId(
                    hospitalizacion.PacienteId,
                    hospitalizacion.HabitacionId,
                    hospiIdStr);
                consentimiento.HospitalizacionId = hospiIdStr;
            }

            return consentimiento;
        }

        private async Task<Dictionary<string, User>> ObtenerUsuariosAutorizadoresAsync(IEnumerable<string> userIds)
        {
            var ids = userIds?
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList() ?? new List<string>();

            if (!ids.Any())
                return new Dictionary<string, User>();

            return await _context.Users.AsNoTracking()
                .Where(u => ids.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);
        }

        private async Task<List<SignosVitalesHospPdfRow>> MapSignosVitalesHospitalizacionPdfAsync(
            int hospitalizacionId,
            Hospitalizacion hospitalizacion = null)
        {
            var examenes = _hospitalizacionRepository.GetExamenesFisicosHosp(hospitalizacionId);
            var contentRoot = ContentRoot;
            hospitalizacion ??= _hospitalizacionRepository.Get(hospitalizacionId);
            var medico = PdfReportHelper.ResolverMedicoTratanteHospitalizacion(
                hospitalizacion, _empleadoRepository, contentRoot);

            var autorizadorIds = examenes
                .Where(e => e.Autorizado && !string.IsNullOrWhiteSpace(e.UsuarioAutoriza))
                .Select(e => e.UsuarioAutoriza);
            var autorizadores = await ObtenerUsuariosAutorizadoresAsync(autorizadorIds);

            return PdfReportHelper.MapSignosVitalesHosp(
                examenes,
                _userRepository,
                _empleadoRepository,
                contentRoot,
                medico.FirmaBase64,
                autorizadores);
        }


    }

}