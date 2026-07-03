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
            IMedicamentoNoControladoRepository medicamentoNoControladoRepository



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

        //     return await _generatePdf.GetPdf("Views/CrearPDF/ReciboVentaPdf.cshtml", venta);
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
                                foreach (var prod in hospi.HospitalizacionesProductos
                                                              .Where(p => p.HospitalizacionesProductosAplicaciones != null
                                                                       && p.HospitalizacionesProductosAplicaciones.Any(a => a.Aplicado)))
                                {
                                    var cantidad = prod.HospitalizacionesProductosAplicaciones
                                                       .Where(a => a.Aplicado)
                                                       .Sum(a => a.Cantidad);

                                    if (cantidad <= 0) continue; // ← solo saltar si no hay cantidad real

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

                            // ── 4. Servicios clínicos ────────────────────────────────
                            if (hospi.HospitalizacionesServicios != null)
                            {
                                foreach (var svc in hospi.HospitalizacionesServicios
                                                         .Where(s => !s.Eliminado)) // ✅ sin filtro de Aplicado
                                {
                                    var subtotal = Math.Round(svc.Cantidad * svc.Precio, 2);

                                    detallesGenerados.Add(new DetalleVenta
                                    {
                                        BienOServicio = "S",
                                        Cantidad = (int)Math.Round(svc.Cantidad, 0),
                                        Precio = Math.Round(svc.Precio, 2),
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

            return await _generatePdf.GetPdf("Views/CrearPDF/ReciboVentaPdf.cshtml", venta);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/TurnoPDF.cshtml", cita);
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
            return await _generatePdf.GetPdf("Views/CrearPDF/CotizacionPDF.cshtml", model);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/CuentasPorCobrarReciboPagoPDF.cshtml", venta);
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

        //    return await _generatePdf.GetPdf("Views/CrearPDF/ReciboServicios.cshtml", ventaServicio);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/ProveedoresPdf.cshtml", proveedores);
        }

        public async Task<IActionResult> EmpleadosPdf()
        {

            var empleados = _empleadoRepository.GetList();

            return await _generatePdf.GetPdf("Views/CrearPDF/EmpleadosPdf.cshtml", empleados);
        }

        public async Task<IActionResult> MedicosPdf()
        {

            var medicos = _empleadoRepository.GetList();

            return await _generatePdf.GetPdf("Views/CrearPDF/MedicosPdf.cshtml", medicos);
        }

        public async Task<IActionResult> ProductosPdf()
        {

            var producto = _productoRepository.GetListPdf();

            return await _generatePdf.GetPdf("Views/CrearPDF/ProductosPdf.cshtml", producto);
        }

        public async Task<IActionResult> ClientesPdf()
        {

            var clientes = _clienteRepository.GetList(); // Asegúrate de que esto devuelve List<Clientes>
            return await _generatePdf.GetPdf("Views/CrearPDF/ClientesPdf.cshtml", clientes);

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

            return await _generatePdf.GetPdf("Views/CrearPDF/CajasPdf.cshtml", model);
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

        //public IActionResult EnviosPdf(string fecha)
        //{

        //    var fechas = fecha.Split('-');



        //    var ventas = _ventaServicioRepository.GetListadoFecha(Convert.ToDateTime(fechas[0]), Convert.ToDateTime(fechas[1]).AddDays(1));
        //    //return Json (ventas);


        //    return new ViewAsPdf("VentasServiciosPdf", ventas);
        //}

        public async Task<IActionResult> CategoriasLabClinico()
        {

            var lista = _laboratorioClinico.GetListCategoriasLab();
            return await _generatePdf.GetPdf("Views/CrearPDF/CategoriasLabClinicoPDF.cshtml", lista);

        }

        public async Task<IActionResult> GenerarReporteExamen(int id)
        {
            var lista = _laboratorioClinico.GetExamenRealizado(id);

            return await _generatePdf.GetPdf("Views/CrearPDF/GenerarReporteExamen.cshtml", lista);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/OrdenCompraPDF.cshtml", ordenCompraDetalles);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/PaqueteCotizacionPDF.cshtml", model);
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


            string directorio = Directory.GetCurrentDirectory();
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

            return await _generatePdf.GetPdf("Views/CrearPDF/ConsultaExamenesPDF.cshtml", model);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/generarExamenesSolicitarPdf.cshtml", model);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/generarExamenesSolicitarAPdf.cshtml", model);
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

                return await _generatePdf.GetPdf("Views/CrearPDF/GenerarResultados.cshtml", model);
            }
            catch (Exception ex)
            {
                // Manejar el error
                return StatusCode(500, $"Ocurrió un error: {ex.Message}");
            }
        }


        public IActionResult HospitalizacionNotaMedica2PDF(int notaMedicaId)
        {
            var notaMedica = _hospitalizacionRepository.GetNotaMedica2(notaMedicaId);

            if (notaMedica == null)
            {
                return NotFound("No se encontró la nota médica solicitada.");
            }

            string empleadoText = notaMedica.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.EmpleadoText ?? "Sin asignar";

            string colegioEmpleado = notaMedica?.Hospitalizacion?.Consultas?.FirstOrDefault()?.Citas?.Empleado.Colegiado ?? "No disponible";

            var model = new NotaMedica2ViewModel
            {
                HistoriaProblema = notaMedica?.HistoriaProblema ?? "No disponible",
                Sintomas = notaMedica?.Sintomas ?? "No disponibles",
                Diagnostico = notaMedica?.Diagnostico ?? "No disponible",
                FechaRegistro = notaMedica.FechaRegistro.ToString("dd/MM/yyyy hh:mm: tt"),
                Profesional = notaMedica?.Profesional?.Persona?.Nombre ?? "Sin asignar",
                PacienteNombre = notaMedica?.Hospitalizacion?.Paciente?.Nombre ?? "Sin asignar",
                PacienteEdad = CalcularEdad(notaMedica?.Hospitalizacion?.Paciente?.FechaNacimiento),
                PacienteSexoText = notaMedica?.Hospitalizacion?.Paciente?.sexoText ?? "Sin asignar",
                EmpleadoText = empleadoText,
                ColegioEmpleado = colegioEmpleado
            };

            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };

            _generatePdf.SetConvertOptions(options);
            var pdf = _generatePdf.GetPdf("Views/CrearPDF/HospitalizacionNotaMedicaPDF.cshtml", model).Result;

            return pdf;
        }

        public IActionResult HospitalizacionNotaMedicaPDFGetAll(int hospitalizacionId)
        {
            var notasMedicas = _hospitalizacionRepository.GetNotasMedicasByHospitalizacion(hospitalizacionId);

            if (notasMedicas == null || !notasMedicas.Any())
            {
                return NotFound("No se encontraron notas médicas para la hospitalización solicitada.");
            }

            var paciente = notasMedicas.FirstOrDefault()?.Hospitalizacion?.Paciente;

            var empleadoText = notasMedicas.FirstOrDefault()?.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.EmpleadoText ?? "Sin asignar";

            var empleadoEspecialidad = notasMedicas.FirstOrDefault()?.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.Empleado?.Especialidad?.NombreEspecialidad ?? "Sin asignar";


            var colegioEmpleado = notasMedicas.FirstOrDefault()?.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.Empleado?.Colegiado ?? "No disponible";

            var model = new NotaMedica2ListaViewModel
            {
                PacienteNombre = paciente?.Nombre ?? "Sin asignar",
                PacienteEdad = CalcularEdad(paciente?.FechaNacimiento),
                PacienteSexoText = paciente?.sexoText ?? "Sin asignar",
                EmpleadoText = empleadoText,
                EmpleadoEspecialidad = empleadoEspecialidad,
                ColegioEmpleado = colegioEmpleado,
                NotasMedicas = notasMedicas.Select(nm => new NotaMedica2ViewModel
                {
                    Id = nm.Id,
                    Diagnostico = nm.Diagnostico ?? "No disponible",
                    FechaRegistro = nm.FechaRegistro.ToString("dd/MM/yyyy hh:mm tt"),
                    Profesional = nm.Profesional?.Persona?.NombreYApellidos ?? "Sin asignar"
                }).ToList()
            };

            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };

            _generatePdf.SetConvertOptions(options);
            var pdf = _generatePdf.GetPdf("Views/CrearPDF/HospitalizacionNotasMedicasPDFGetAll.cshtml", model).Result;

            return pdf;
        }

        [HttpGet]
        public IActionResult HospitalizacionNotaOperatoria2PDF(int notaOperatoriaId)
        {
            var notaOperatoria = _hospitalizacionRepository.GetNotaOperatoria(notaOperatoriaId);

            if (notaOperatoria == null)
            {
                return NotFound("No se encontró la nota operatoria solicitada.");
            }

            string empleadoText = notaOperatoria.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.EmpleadoText ?? "Sin asignar";

            string colegioEmpleado = notaOperatoria.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.Empleado?.Colegiado ?? "No disponible";

            string empleadoEspecialidad = notaOperatoria.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.Empleado?.Especialidad?.NombreEspecialidad ?? "Sin asignar";

            string nombreProfesional = "Sin asignar";
            if (notaOperatoria.User != null && notaOperatoria.User.Persona != null)
            {
                nombreProfesional = notaOperatoria.User.Persona.NombreYApellidos;
            }

            var model = new NotaMedica2ViewModel
            {
                Sintomas = notaOperatoria.Sintomas ?? "No disponibles",
                Diagnostico = notaOperatoria.Diagnostico ?? "No disponible",
                Evolucion = notaOperatoria.Evolucion ?? "No disponible",
                FechaRegistro = notaOperatoria.FechaRegistro.ToString("dd/MM/yyyy hh:mm tt"),
                Profesional = nombreProfesional,
                PacienteNombre = notaOperatoria.Hospitalizacion?.Paciente?.Nombre ?? "Sin asignar",
                PacienteEdad = CalcularEdad(notaOperatoria.Hospitalizacion?.Paciente?.FechaNacimiento),
                PacienteSexoText = notaOperatoria.Hospitalizacion?.Paciente?.sexoText ?? "Sin asignar",
                EmpleadoText = empleadoText,
                ColegioEmpleado = colegioEmpleado,
            };

            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };

            _generatePdf.SetConvertOptions(options);
            var pdf = _generatePdf.GetPdf("Views/CrearPDF/HospitalizacionNotaOperatoriaPDF.cshtml", model).Result;
            return pdf;
        }

        [HttpGet]
        public IActionResult HospitalizacionNotaOperatoriaPDFGetAll(int hospitalizacionId)
        {
            var notas = _hospitalizacionRepository.GetNotasOperatoriasByHospitalizacion(hospitalizacionId);

            if (notas == null || !notas.Any())
            {
                return NotFound("No se encontraron notas operatorias para esta hospitalización.");
            }

            var primeraNota = notas.First();
            var paciente = primeraNota.Hospitalizacion?.Paciente;
            var consultaBase = primeraNota.Hospitalizacion?.Consultas?.FirstOrDefault();

            var model = new NotaMedica2ListaViewModel
            {
                PacienteNombre = paciente?.Nombre ?? "Sin asignar",
                PacienteEdad = CalcularEdad(paciente?.FechaNacimiento),
                PacienteSexoText = paciente?.sexoText ?? "Sin asignar",
                EmpleadoText = consultaBase?.Citas?.EmpleadoText ?? "Sin asignar",
                EmpleadoEspecialidad = consultaBase?.Citas?.Empleado?.Especialidad?.NombreEspecialidad ?? "Sin asignar",
                ColegioEmpleado = consultaBase?.Citas?.Empleado?.Colegiado ?? "No disponible",
                NotasMedicas = notas.Select(n =>
                {

                    string nombreProfesional = "Sin asignar";
                    if (n.User != null && n.User.Persona != null)
                    {
                        nombreProfesional = n.User.Persona.NombreYApellidos;
                    }

                    return new NotaMedica2ViewModel
                    {
                        Id = n.Id,
                        Diagnostico = n.Diagnostico ?? "No disponible",
                        FechaRegistro = n.FechaRegistro.ToString("dd/MM/yyyy hh:mm tt"),
                        Profesional = nombreProfesional
                    };
                }).ToList()
            };

            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };

            _generatePdf.SetConvertOptions(options);
            var pdf = _generatePdf.GetPdf("Views/CrearPDF/HospitalizacionNotasOperatoriasPDFGetAll.cshtml", model).Result;
            return pdf;
        }

        public IActionResult HospitalizacionOrdenMedicaPDF(int ordenMedicaId)
        {
            var ordenMedica = _hospitalizacionRepository.GetOrdenMedica(ordenMedicaId);
            if (ordenMedica == null)
            {
                return NotFound("No se encontró la orden médica solicitada.");
            }

            string empleadoText = ordenMedica.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.EmpleadoText ?? "Sin asignar";

            string colegioEmpleado = ordenMedica?.Hospitalizacion?.Consultas?
                .FirstOrDefault()?.Citas?.Empleado.Colegiado ?? "No disponible";

            var model = new OrdenMedicaViewModel
            {
                FechaHora = ordenMedica.FechaHora.ToString("dd/MM/yyyy hh:mm tt"),
                Profesional = ordenMedica?.Profesional ?? "Sin asignar",
                Descripcion = ordenMedica?.Descripcion ?? "No disponible",
                PacienteNombre = ordenMedica?.Hospitalizacion?.Paciente?.Nombre ?? "Sin asignar",
                PacienteEdad = CalcularEdad(ordenMedica?.Hospitalizacion?.Paciente?.FechaNacimiento),
                PacienteSexoText = ordenMedica?.Hospitalizacion?.Paciente?.sexoText ?? "Sin asignar",
                EmpleadoText = empleadoText,
                ColegioEmpleado = colegioEmpleado,
                Realizada = ordenMedica.Realizada ? "Sí" : "No"
            };


            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                PageOrientation = Orientation.Portrait,
            };

            _generatePdf.SetConvertOptions(options);
            var pdf = _generatePdf.GetPdf("Views/CrearPDF/HospitalizacionOrdenMedicaPDF.cshtml", model).Result;

            return pdf;
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

            return await _generatePdf.GetPdf("Views/CrearPDF/PrescripcionPDF.cshtml", model);
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

        //     return await _generatePdf.GetPdf("Views/CrearPDF/PrescripcionPDF.cshtml", model);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/GenerarGinecologiaConsultaPDF.cshtml", model);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/generarObstetricaPDF.cshtml", model);
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


            return await _generatePdf.GetPdf("Views/CrearPDF/generarObstetricaBiometriaPDF.cshtml", model);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/generarObstetricaBiometriaPDF2.cshtml", model);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/generarObstetricaBiometriaPDF3.cshtml", model);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/generarObstetricaBiometriaPDF4.cshtml", model);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/generarEndocavitarioPDF.cshtml", model);
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
            return await _generatePdf.GetPdf
                ("Views/CrearPDF/HospitalizacionInformeGeneralPDF.cshtml", model);
        }
        public async Task<IActionResult> HospitalizacionInformeDetalladoPDF(int hospitalizacionId)
        {
            var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId);
            return await _generatePdf.GetPdf
                ("Views/CrearPDF/HospitalizacionInformeDetalladoPDF.cshtml", hospitalizacion);
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

            return await _generatePdf.GetPdf
                ("Views/CrearPDF/ExpedientePacientePDF.cshtml", hospitalizacion);
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
        //     return await _generatePdf.GetPdf("Views/CrearPDF/HistoricoProductosPDF.cshtml", model);
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
            return await _generatePdf.GetPdf("Views/CrearPDF/HistoricoProductosPDF.cshtml", vm);
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
            return await _generatePdf.GetPdf("Views/CrearPDF/HistoricoProductosPDFNacional.cshtml", vm);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/CarneVacunacionJovenes.cshtml", model);
        }
        public async Task<IActionResult> PacientesExpedientePDF(int pacienteId)
        {
            var paciente = _pacientesRepository.Get(pacienteId);

            return await _generatePdf.GetPdf("Views/CrearPDF/PacientesExpedientePDF.cshtml", paciente);
        }
        public async Task<IActionResult> PacientesPresupuestoDentalPDF(int presupuestoId)
        {
            var presupuesto = _pacientesRepository.GetPresupuestoDental(presupuestoId);

            return await _generatePdf.GetPdf
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

            return await _generatePdf.GetPdf("Views/CrearPDF/AmbulanciaPDF.cshtml", ambulancia);
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

            return await _generatePdf.GetPdf("PacientesCompromisoMembresiaPDF", paciente);
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

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var u = _userRepository.GetbyId(user.Id).Persona.Nombre;

            var model = new ReporteCitasViewModel()
            {
                Citas = citasPorFecha,
                Usuario = u
            };

            return await _generatePdf.GetPdf("Views/Cita/ReporteCitasCalendarioPDF.cshtml", model);
        }


        public async Task<IActionResult> generarPdfExamenesLaboratorio(int examenLaboratorioId, int? pacienteId, string TipoPDF)
        {
            var examenLaboratorio = _laboratorioClinico.GetExamenLab(examenLaboratorioId);
            var pacienteAsociado = _pacientesRepository.GetPacientePorId((int)pacienteId);

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

            return await _generatePdf.GetPdf("Views/CrearPDF/generarPdfExamenesLaboratorio.cshtml", model);
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

            return await _generatePdf.GetPdf("Views/CrearPDF/LaboratorioClinicoExamenesClinicosPDF.cshtml", modeloLista);

        }

        public async Task<IActionResult> GenerarPdfCompraOrdenes(string fechaInicial, string fechaFinal, int comprobante,
            string proveedor, string vendedor)
        {

            var data = _compraRepository.PaginacionOrdenesCompra(null, null, null, 25, fechaInicial, fechaFinal, comprobante, proveedor, vendedor);

            return await _generatePdf.GetPdf("Views/CrearPDF/CompraPDF.cshtml", data);
        }

        public async Task<IActionResult> GenerarPdfCompraCompras(string fechaInicial, string fechaFinal, string comprobante, string proveedor, string vendedor,
            int numeroCompra)
        {

            var data = _compraRepository.PaginacionCompras(null, null, null, 1000, fechaInicial, fechaFinal, comprobante, proveedor, vendedor, numeroCompra);

            return await _generatePdf.GetPdf("Views/CrearPDF/ComprasPDF.cshtml", data);
        }
        public async Task<IActionResult> GenerarPdfVentasClinica(string fechaInicial, string fechaFinal, int numeroVenta,
            string comprobante, int formaPago, string origenVenta)
        {
            var lista = _ventaRepository.PaginacionVentasClinica(null, null, null, 25, fechaInicial, fechaFinal, numeroVenta, comprobante, formaPago, origenVenta);
            return await _generatePdf.GetPdf("Views/CrearPDF/VentasPDF.cshtml", lista);
        }

        public async Task<IActionResult> GenerarPdfInventarioProductos(int? tipoProductoId, int? grupoTerapeuticoId, int? bodegaId, int? sucursalId, int? ambienteId)
        {
            var data = _productosService.GetInventarioBySp(tipoProductoId, grupoTerapeuticoId, bodegaId, sucursalId, ambienteId);
            InventarioViewModel viewModel = new InventarioViewModel()
            {
                ProductosInventario = data,
                Precios = _precioRepository.GetList().ToList()
            };
            return await _generatePdf.GetPdf("Views/CrearPDF/InventarioProductosPdf.cshtml", viewModel);
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
            return await _generatePdf.GetPdf("Views/CrearPDF/AdmisionPacientePDF.cshtml", model);
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
        //     return await _generatePdf.GetPdf("Views/CrearPDF/ConsentimientoHospiPDF.cshtml", model);
        // }

        public async Task<IActionResult> GenerarPDFConsentimientoHospi(int idPaciente, int idHabitacion, string idHospi = null, int? citaId = null)
        {
            ConsentimientoHospiVM consentimiento = null;
            for (int intento = 0; intento < 5; intento++)
            {
                consentimiento = _consentimientoHospiService.GetConsentimientoByPacienteAndHabitacion(idPaciente, idHabitacion);
                if (consentimiento != null) break;
                await Task.Delay(500);
            }

            if (consentimiento == null)
            {
                return BadRequest($"No se encontró el consentimiento para paciente {idPaciente} y habitación {idHabitacion}. Intente generar el PDF nuevamente.");
            }

            // --- NUEVO MÉTODO CORRECTO PARA OBTENER LAS FIRMAS DESDE LA CARPETA LOCAL ---
            string directorio = Directory.GetCurrentDirectory();


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
            if (string.IsNullOrWhiteSpace(dpiPaciente) && (consentimiento?.PacienteId ?? 0) > 0)
            {
                var paciente = _pacientesRepository.Get(consentimiento.PacienteId);
                dpiPaciente = paciente?.Dpi ?? "";
            }

            string dpiResponsable = consentimiento?.DPIResponsable ?? "";

            var model = new ConsentimientoHospiVM()
            {
                // Relación con Paciente
                PacienteId = consentimiento?.PacienteId ?? 0,
                NombrePaciente = consentimiento?.NombrePaciente ?? "",
                HabitacionId = consentimiento?.HabitacionId ?? 0,
                NumeroHabitacion = consentimiento?.NumeroHabitacion ?? "",
                HospitalizacionId = consentimiento?.HospitalizacionId ?? "",

                // Datos del Paciente
                HoraIngreso = consentimiento?.HoraIngreso ?? "",
                NumeroPaciente = consentimiento?.NumeroPaciente ?? "",
                NombreCompleto = consentimiento?.NombreCompleto ?? "",
                EstadoCivil = consentimiento?.EstadoCivil ?? "",
                DPI = dpiPaciente,
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
                URLFirmaRepresentanteNaranjo = firmaRepresentanteBase64
            };

            // return await _generatePdf.GetPdf("Views/CrearPDF/ConsentimientoHospiPDF.cshtml", model);
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
            return await _generatePdf.GetPdf(rutaVistaPDF, model);
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
            return await _generatePdf.GetPdf("Views/CrearPDF/ConsentimientoHospiPDF.cshtml", model);
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
        //     return await _generatePdf.GetPdf("Views/CrearPDF/EstadoDeCuentaHopsPDF.cshtml", estadoCuenta);
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

                return await _generatePdf.GetPdf("Views/CrearPDF/EstadoDeCuentaHopsPDF.cshtml", model);
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

                // 3.2 Medicamentos / insumos aplicados
                if (hospitalizacion.HospitalizacionesProductos != null)
                {
                    foreach (var prod in hospitalizacion.HospitalizacionesProductos
                        .Where(p => p.HospitalizacionesProductosAplicaciones != null &&
                                    p.HospitalizacionesProductosAplicaciones.Any(a => a.Aplicado)))
                    {
                        var cantidad = prod.HospitalizacionesProductosAplicaciones
                            .Where(a => a.Aplicado)
                            .Sum(a => a.Cantidad);
                        if (cantidad <= 0) continue;

                        var subtotal = Math.Round(cantidad * prod.PrecioValor, 2);
                        string fechaStr = prod.FechaHoraAplicacionManual ?? DateTime.Now.ToString("dd/MM/yyyy");
                        productosList.Add(new fmModels.ProductoViewModel
                        {
                            Fecha = fechaStr,
                            Tipo = "Medicamentos",
                            Item = prod.Producto?.NombreProducto ?? "Medicamento/Insumo",
                            Cantidad = (int)cantidad,
                            PrecioUnitario = Math.Round(prod.PrecioValor, 2),
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
                        var subtotal = Math.Round(svc.Cantidad * svc.Precio, 2);
                        productosList.Add(new fmModels.ProductoViewModel
                        {
                            Fecha = svc.FechaAplicacionFormateada ?? DateTime.Now.ToString("dd/MM/yyyy"),
                            Tipo = "Servicios",
                            Item = svc.Servicio?.NombreServicio ?? "Servicio clínico",
                            Cantidad = (int)Math.Round(svc.Cantidad, 0),
                            PrecioUnitario = Math.Round(svc.Precio, 2),
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

                return await _generatePdf.GetPdf("Views/CrearPDF/EstadoDeCuentaHopsPDF.cshtml", model);
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


            string directorio = Directory.GetCurrentDirectory();

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

            return await _generatePdf.GetPdf("Views/CrearPDF/generarPDFRequisicionDespacho.cshtml", model);
        }




        [HttpGet]
        public async Task<IActionResult> generarPDFDevolucion(int id)
        {
            if (id <= 0) return BadRequest("ID de devolución no válido.");

            var devolucion = await _devolucionRepository.GetByIdAsync(id);

            if (devolucion == null) return NotFound($"No se encontró la devolución con ID: {id}");

            string directorio = Directory.GetCurrentDirectory();

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

            return await _generatePdf.GetPdf("Views/CrearPDF/generarPDFDevolucion.cshtml", devolucion);
        }

        public async Task<IActionResult> GenerarPDFHospiReports(int idPaciente, int idHabitacion, string idHospi = null, int report = 0, int? citaId = null)
        {
            var consentimiento = _consentimientoHospiService.GetConsentimientoByPacienteHabitacionAndHospitalizacion(idPaciente, idHabitacion, idHospi);

            // --- Función local para leer imágenes a Base64 ---
            string directorio = Directory.GetCurrentDirectory();
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

                    // Función interna para resolver ayudantes
                    void ResolverMedicoSecundario(string datoCita, out string nombre, out string colegiado)
                    {
                        nombre = datoCita ?? ""; colegiado = "";
                        if (int.TryParse(datoCita, out int idEmp))
                        {
                            var emp = _empleadoRepository.Get(idEmp, false);
                            if (emp != null) { nombre = emp.NombreYApellidos; colegiado = emp.Colegiado ?? ""; }
                        }
                    }

                    ResolverMedicoSecundario(cita.PrimerAyudante, out nombre1erAyudante, out col1erAyudante);
                    ResolverMedicoSecundario(cita.SegundoAyudante, out nombre2doAyudante, out col2doAyudante);
                    ResolverMedicoSecundario(cita.Anestesista, out nombreAnestesista, out colAnestesista);

                    if (int.TryParse(cita.Anestesista, out int idAnestesista))
                    {
                        var anestesista = _empleadoRepository.Get(idAnestesista, false);
                        if (anestesista != null)
                        {
                            firmaAnestesistaBase64 = ObtenerFirmaBase64Local(anestesista.FirmaEmpleado);
                        }
                    }
                }
            }

            var medicamentosNoControlados = new List<MedicamentoNoControladoPdfVM>();
            if (!string.IsNullOrEmpty(idHospi) && int.TryParse(idHospi, out int hospiIdParaMeds))
            {
                medicamentosNoControlados = _medicamentoNoControladoRepository
                    .GetHistorialByHospitalizacion(hospiIdParaMeds)
                    .Where(m => !m.Eliminado)
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

            // Selección de Vista
            if (report == 1) return await _generatePdf.GetPdf("Views/CrearPDF/AutorizacionSalaOperacionesHospiPDF.cshtml", model);
            else if (report == 2) return await _generatePdf.GetPdf("Views/CrearPDF/MedicoAnestesiaHospiPDF.cshtml", model);
            else if (report == 3) return await _generatePdf.GetPdf("Views/CrearPDF/MedicamentosContrladosHospiPDF.cshtml", model);
            else return await _generatePdf.GetPdf("Views/CrearPDF/ConsentimientoHospiPDF.cshtml", model);
        }




        [Authorize]
        public async Task<IActionResult> ReporteVentasGeneralPDF(
            string desde, string hasta, int ambienteId = 0, int empleadoId = 0,
            string modoDetalle = "monto", bool incluirConsolidadoItems = false,
            bool incluirConsolidadoMedicos = false)
        {
            // ── 1. Parsear fechas ────────────────────────────────────────────────
            if (!DateTime.TryParse(desde, out var fechaDesde) ||
                !DateTime.TryParse(hasta, out var fechaHasta))
                return BadRequest("Formato de fecha inválido. Use yyyy-MM-dd.");

            var fechaHastaFin = fechaHasta.Date.AddDays(1).AddSeconds(-1);

            // ── 2. Consultar ventas (sin navegaciones pesadas inicialmente) ─────
            List<Venta> ventasBd = empleadoId > 0
                ? _ventaRepository.GetListadoFechaEmpleado(fechaDesde.Date, fechaHastaFin, empleadoId)
                : _ventaRepository.GetListadoFecha(fechaDesde.Date, fechaHastaFin);

            ventasBd = ventasBd.Where(v => v.Eliminado != true).ToList();

            if (ambienteId > 0)
                ventasBd = ventasBd.Where(v => v.AmbienteId == ambienteId).ToList();

            // ── 3. Cargar información completa de cada venta ─────────────────────
            foreach (var v in ventasBd)
            {
                // Asignar nombre real del paciente si la venta tiene PacienteId pero clientes nulo
                if ((v.Clientes == null || string.IsNullOrWhiteSpace(v.Clientes.Nombre)) && v.PacienteId != null)
                {
                    var paciente = _pacientesRepository.Get(v.PacienteId.Value);
                    if (paciente != null)
                    {
                        if (v.Clientes == null)
                            v.Clientes = new Clientes();
                        v.Clientes.Nombre = paciente.Nombre;
                        v.Clientes.Nit = v.Nit ?? "CF";
                        v.Clientes.Direccion = v.Direccion ?? "N/A";
                    }
                }

                var vCompleta = _ventaRepository.Get(v.Id);
                if (vCompleta != null)
                {
                    v.DetalleVenta = vCompleta.DetalleVenta?.ToList() ?? new List<DetalleVenta>();
                    v.Pagos = vCompleta.Pagos?.ToList() ?? new List<Pagos>();
                    if (v.Empleado == null && vCompleta.Empleado != null)
                        v.Empleado = vCompleta.Empleado;
                    if (v.Clientes == null && vCompleta.Clientes != null)
                        v.Clientes = vCompleta.Clientes;
                }
                else
                {
                    var detalles = _ventaRepository.GetDetalle(v.Id);
                    v.DetalleVenta = detalles?.ToList() ?? new List<DetalleVenta>();
                    var vTemp = _ventaRepository.Get(v.Id);
                    v.Pagos = vTemp?.Pagos?.ToList() ?? new List<Pagos>();
                }
            }

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

            void AgregarItem(string categoria, string nombre, decimal cantidad, decimal total, decimal? costoUnitario = null)
            {
                if (!incluirConsolidadoItems) return;
                if (string.IsNullOrWhiteSpace(nombre)) nombre = "No especificado";
                var key = $"{categoria}|{nombre}";
                if (!itemsConsolidadosDict.TryGetValue(key, out var item))
                {
                    item = (0, 0, 0, costoUnitario);
                    itemsConsolidadosDict[key] = item;
                }
                item.cantidad += cantidad;
                item.total += total;
                if (costoUnitario.HasValue)
                {
                    item.costoTotal += cantidad * costoUnitario.Value;
                    if (!item.costoUnitario.HasValue) item.costoUnitario = costoUnitario;
                }
                itemsConsolidadosDict[key] = item;
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

                if (!ventasAmb.Any()) continue;

                var items = new List<ReporteVentaItemViewModel>();

                foreach (var v in ventasAmb)
                {
                    bool esHospitalizacion = v.AmbienteId == (int)AmbienteEnum.Hospital
                                             || (v.TipoVenta?.IndexOf("hospitalizacion", StringComparison.OrdinalIgnoreCase) >= 0);

                    ReporteVentaItemViewModel item;

                    if (esHospitalizacion)
                    {
                        // ── OBTENER HOSPITALIZACION ID ──
                        int? hospitalizacionId = ObtenerHospitalizacionIdPorVenta(v);

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
                                foreach (var prod in hosp.HospitalizacionesProductos ?? Enumerable.Empty<HospitalizacionProducto>())
                                {
                                    var aplicadas = prod.HospitalizacionesProductosAplicaciones?.Where(a => a.Aplicado).ToList() ?? new List<HospitalizacionProductoAplicacion>();
                                    if (!aplicadas.Any()) continue;
                                    decimal cantidad = aplicadas.Sum(a => a.Cantidad);
                                    decimal subtotalProd = cantidad * prod.PrecioValor;
                                    subProductos += subtotalProd;
                                    cantProductos += cantidad;

                                    // Costo: prioridad → ProductoInventario.PrecioCosto → Producto.PrecioCosto → DetallePaquete.PrecioCosto
                                    decimal? costoUnitario = null;
                                    // 1) Buscar en inventario por ProductoId (precio de compra real del lote)
                                    var invRecord = _context.ProductosInventario
                                        .Where(i => i.ProductoId == prod.ProductoId && !i.Eliminado)
                                        .OrderByDescending(i => i.Id)
                                        .Select(i => (decimal?)i.PrecioCosto)
                                        .FirstOrDefault();
                                    if (invRecord.HasValue && invRecord.Value > 0)
                                        costoUnitario = invRecord.Value;
                                    // 2) Fallback: PrecioCosto del catálogo
                                    else if (prod.Producto?.PrecioCosto > 0)
                                        costoUnitario = prod.Producto.PrecioCosto;
                                    else if (prod.ProductoId > 0)
                                    {
                                        var prodBd = _productoRepository.GetProdutoById(prod.ProductoId);
                                        if (prodBd?.PrecioCosto > 0) costoUnitario = prodBd.PrecioCosto;
                                    }

                                    if (modoDetalle == "descripcion")
                                        listadoProductos.Add(new ReporteVentaDetalleItemViewModel
                                        {
                                            Descripcion = prod.Producto?.NombreProducto ?? "Producto",
                                            Categoria = "Producto",
                                            Cantidad = cantidad,
                                            PrecioUnit = prod.PrecioValor,
                                            Subtotal = subtotalProd,
                                            Descuento = 0
                                        });

                                    AgregarItem("Producto", prod.Producto?.NombreProducto ?? "Producto", cantidad, subtotalProd, costoUnitario);
                                }

                                // ---- Servicios aplicados ----
                                decimal subServicios = 0m, cantServicios = 0m;
                                var listadoServicios = new List<ReporteVentaDetalleItemViewModel>();
                                foreach (var svc in hosp.HospitalizacionesServicios ?? Enumerable.Empty<HospitalizacionServicio>())
                                {
                                    if (svc.Aplicado)
                                    {
                                        decimal subtotalSvc = svc.Cantidad * svc.Precio;
                                        subServicios += subtotalSvc;
                                        cantServicios += svc.Cantidad;

                                        if (modoDetalle == "descripcion")
                                            listadoServicios.Add(new ReporteVentaDetalleItemViewModel
                                            {
                                                Descripcion = svc.Servicio?.NombreServicio ?? "Servicio",
                                                Categoria = "Servicio",
                                                Cantidad = svc.Cantidad,
                                                PrecioUnit = svc.Precio,
                                                Subtotal = subtotalSvc,
                                                Descuento = 0
                                            });

                                        AgregarItem("Servicio", svc.Servicio?.NombreServicio ?? "Servicio", svc.Cantidad, subtotalSvc, null);
                                    }
                                }

                                // ---- Exámenes (con categoría real) ----
                                decimal subExamenes = 0m, cantExamenes = 0m;
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
                                            if (modoDetalle == "descripcion")
                                                listadoExamenes.Add(new ReporteVentaDetalleItemViewModel
                                                {
                                                    Descripcion = labClinico?.NombreExamen ?? "Examen",
                                                    Categoria = nombreCategoria,
                                                    Cantidad = det.Cantidad,
                                                    PrecioUnit = det.PrecioValor,
                                                    Subtotal = det.PrecioValor,
                                                    Descuento = 0
                                                });

                                            AgregarItem(nombreCategoria, labClinico?.NombreExamen ?? "Examen", det.Cantidad, det.PrecioValor, null);
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

                                        // Costo del paquete: suma de PrecioCosto de cada línea de detalle
                                        decimal? costoPaq = paq.PaqueteHospitalizacion.DetallePaquetesHospitalizacion != null
                                            ? paq.PaqueteHospitalizacion.DetallePaquetesHospitalizacion
                                                .Where(d => !d.Eliminado)
                                                .Sum(d => (d.PrecioCosto) * d.Cantidad)
                                            : paq.PaqueteHospitalizacion.PrecioCosto;

                                        if (modoDetalle == "descripcion")
                                            listadoPaquetes.Add(new ReporteVentaDetalleItemViewModel
                                            {
                                                Descripcion = $"Paquete: {paq.PaqueteHospitalizacion.NombrePaquete}",
                                                Categoria = "Hospital",
                                                Cantidad = 1,
                                                PrecioUnit = precioPaq,
                                                Subtotal = precioPaq,
                                                Descuento = 0
                                            });

                                        AgregarItem("Paquete", paq.PaqueteHospitalizacion.NombrePaquete, 1, precioPaq, costoPaq);
                                    }
                                }

                                // ---- Gastos administrativos ----
                                decimal subGastosAdmin = _hospitalizacionRepository.GetGastosAdministrativos(hosp.Id)?.Sum(g => g.Monto) ?? 0;
                                if (subGastosAdmin > 0)
                                    AgregarItem("Hospital", "Gastos administrativos", 1, subGastosAdmin, null);

                                // ---- Honorarios ----
                                decimal subHonorarios = _context.HospitalizacionHonorarios.Where(h => h.HospitalizacionId == hosp.Id).Sum(h => h.Monto);
                                if (subHonorarios > 0)
                                    AgregarItem("Hospital", "Honorarios médicos", 1, subHonorarios, null);

                                // ---- Ambulancias ----
                                decimal subAmbulancias = _context.Ambulancias.Where(a => a.HospitalizacionId == hosp.Id).Sum(a => a.Precio);
                                if (subAmbulancias > 0)
                                    AgregarItem("Hospital", "Ambulancia", 1, subAmbulancias, null);

                                // ---- Total categoría Hospital ----
                                decimal subHospital = subtotalEstadia + subPaquetes + subGastosAdmin + subHonorarios + subAmbulancias;
                                decimal totalDescuento = 0m;
                                decimal totalNeto = subProductos + subServicios + subExamenes + subHospital - totalDescuento;

                                // ---- Detalle de ítems para modoDescripcion ----
                                var detalleItems = new List<ReporteVentaDetalleItemViewModel>();
                                if (modoDetalle == "descripcion")
                                {
                                    detalleItems.Add(new ReporteVentaDetalleItemViewModel
                                    {
                                        Descripcion = $"Estadía en habitación ({noches} noche(s))",
                                        Categoria = "Hospital",
                                        Cantidad = noches,
                                        PrecioUnit = tarifaBase,
                                        Subtotal = subtotalEstadia,
                                        Descuento = 0
                                    });
                                    foreach (var cambio in cambios)
                                        detalleItems.Add(new ReporteVentaDetalleItemViewModel
                                        {
                                            Descripcion = "Cambio de habitación",
                                            Categoria = "Hospital",
                                            Cantidad = cambio.Dias,
                                            PrecioUnit = cambio.ValorTarifa,
                                            Subtotal = cambio.ValorTarifa * cambio.Dias,
                                            Descuento = 0
                                        });
                                    detalleItems.AddRange(listadoProductos);
                                    detalleItems.AddRange(listadoServicios);
                                    detalleItems.AddRange(listadoExamenes);
                                    detalleItems.AddRange(listadoPaquetes);
                                    if (subGastosAdmin > 0)
                                        detalleItems.Add(new ReporteVentaDetalleItemViewModel
                                        {
                                            Descripcion = "Gastos administrativos",
                                            Categoria = "Hospital",
                                            Cantidad = 1,
                                            PrecioUnit = subGastosAdmin,
                                            Subtotal = subGastosAdmin,
                                            Descuento = 0
                                        });
                                    if (subHonorarios > 0)
                                        detalleItems.Add(new ReporteVentaDetalleItemViewModel
                                        {
                                            Descripcion = "Honorarios médicos",
                                            Categoria = "Hospital",
                                            Cantidad = 1,
                                            PrecioUnit = subHonorarios,
                                            Subtotal = subHonorarios,
                                            Descuento = 0
                                        });
                                    if (subAmbulancias > 0)
                                        detalleItems.Add(new ReporteVentaDetalleItemViewModel
                                        {
                                            Descripcion = "Ambulancia",
                                            Categoria = "Hospital",
                                            Cantidad = 1,
                                            PrecioUnit = subAmbulancias,
                                            Subtotal = subAmbulancias,
                                            Descuento = 0
                                        });
                                }

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
                                item = ConstruirItemDesdeDetalleVenta(v, modoDetalle, AgregarItem, _productoRepository);
                            }
                        }
                        else
                        {
                            item = ConstruirItemDesdeDetalleVenta(v, modoDetalle, AgregarItem, _productoRepository);
                        }
                    }
                    else
                    {
                        item = ConstruirItemDesdeDetalleVenta(v, modoDetalle, AgregarItem, _productoRepository);
                    }

                    // Agregar médicos para ventas normales (Clínica, etc.)
                    if (item.MedicosAsignados == null || !item.MedicosAsignados.Any())
                    {
                        var medicosVenta = ObtenerMedicosDeVenta(v);
                        item.MedicosAsignados = medicosVenta;
                    }

                    items.Add(item);
                }

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
                    ResumenFormasPago = resumenFP,
                    // Total efectivamente cobrado en pagos (suma de v.Pagos.Monto)
                    TotalCobrado = ventasAmb
                        .SelectMany(v => v.Pagos ?? new List<Pagos>())
                        .Sum(p => Convert.ToDecimal(p.Monto)),
                });
            }

            // ── 9. Consolidado de médicos ─────────────────────────────────────────
            var consolidadoMedicos = incluirConsolidadoMedicos
                ? CalcularConsolidadoMedicos(ambientesVM)
                : new List<ConsolidadoMedicoViewModel>();

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
                MostrarDetalle = modoDetalle == "descripcion",
                TotalGlobalFormasPago = totalGlobalFormasPago,
                TotalGlobalCobrado = totalGlobalFormasPago.Sum(f => f.Total),
                ConsolidadoMedicos = consolidadoMedicos,
                MostrarConsolidadoMedicos = incluirConsolidadoMedicos,
                ItemsConsolidados = incluirConsolidadoItems
                    ? itemsConsolidadosDict.Select(kvp => new ItemConsolidadoViewModel
                    {
                        Categoria = kvp.Key.Split('|')[0],
                        Nombre = kvp.Key.Split('|')[1],
                        Cantidad = kvp.Value.cantidad,
                        Total = kvp.Value.total,
                        CostoUnitario = kvp.Value.costoUnitario,
                        CostoTotal = kvp.Value.costoTotal
                    }).OrderBy(i => i.Categoria).ThenBy(i => i.Nombre).ToList()
                    : new List<ItemConsolidadoViewModel>()
            };

            // ── 11. Opciones de página A4 Portrait ────────────────────────────────
            var options = new ConvertOptions
            {
                PageMargins = { Top = 10, Left = 10, Right = 10, Bottom = 12 },
                PageOrientation = Wkhtmltopdf.NetCore.Options.Orientation.Portrait,
            };
            _generatePdf.SetConvertOptions(options);

            return await _generatePdf.GetPdf("Views/CrearPDF/ReporteVentasGeneralPDF.cshtml", model);
        }

        // ── Método auxiliar para consolidar médicos (ya lo tenías) ─────────────
        private List<ConsolidadoMedicoViewModel> CalcularConsolidadoMedicos(List<ReporteVentasAmbienteViewModel> ambientesVM)
        {
            var dictMedicos = new Dictionary<string, ConsolidadoMedicoViewModel>(StringComparer.OrdinalIgnoreCase);
            foreach (var amb in ambientesVM)
            {
                foreach (var vItem in amb.Ventas)
                {
                    // Médico primario: el empleado directo de la venta
                    string medicoPrincipal = string.IsNullOrWhiteSpace(vItem.EmpleadoNombre)
                        ? null
                        : vItem.EmpleadoNombre.Trim();

                    // Médicos secundarios: los asignados (distintos al primario)
                    var medicosSecundarios = vItem.MedicosAsignados?
                        .Select(m => m.Nombre?.Trim())
                        .Where(n => !string.IsNullOrWhiteSpace(n) &&
                                    !string.Equals(n, medicoPrincipal, StringComparison.OrdinalIgnoreCase))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList() ?? new List<string>();

                    // Si no hay médico primario, usar el primero de los asignados
                    if (medicoPrincipal == null && medicosSecundarios.Any())
                    {
                        medicoPrincipal = medicosSecundarios.First();
                        medicosSecundarios.RemoveAt(0);
                    }

                    // Asignar importes SOLO al médico primario
                    if (medicoPrincipal != null)
                    {
                        if (!dictMedicos.TryGetValue(medicoPrincipal, out var row))
                        {
                            row = new ConsolidadoMedicoViewModel { MedicoNombre = medicoPrincipal };
                            dictMedicos[medicoPrincipal] = row;
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

                        // Tipo de atención: Hospitalización vs Consulta externa
                        if (amb.AmbienteId == (int)AmbienteEnum.Hospital ||
                            (vItem.TipoVenta?.IndexOf("hospitalizacion", StringComparison.OrdinalIgnoreCase) >= 0))
                            row.CantidadHospitalizaciones++;
                        else
                            row.CantidadCitas++;

                        // Sub-tipo: Sala de Operaciones (la habitación de categoría SO) vs Consulta Externa
                        if (vItem.EsSalaOperaciones)
                            row.CantidadSalaOperaciones++;
                        else if (amb.AmbienteId != (int)AmbienteEnum.Hospital)
                            row.CantidadConsultaExterna++;
                    }

                    // Médicos secundarios: solo sumar el contador de ventas (sin montos para no inflar totales)
                    foreach (var nombreSec in medicosSecundarios)
                    {
                        if (!dictMedicos.TryGetValue(nombreSec, out var rowSec))
                        {
                            rowSec = new ConsolidadoMedicoViewModel { MedicoNombre = nombreSec };
                            dictMedicos[nombreSec] = rowSec;
                        }
                        rowSec.CantidadVentas++;
                        // Los importes NO se duplican en médicos secundarios
                    }
                }
            }
            return dictMedicos.Values
                .Where(m => m.TotalNeto > 0)   // excluir médicos secundarios sin importes
                .OrderByDescending(m => m.TotalNeto).ToList();
        }



        // ────────────────────────────────────────────────────────────
        // Método auxiliar para obtener el ID de hospitalización a partir de una venta
        // ────────────────────────────────────────────────────────────
        private int? ObtenerHospitalizacionIdPorVenta(Venta venta)
        {
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

            // 2. Si no, buscar hospitalización activa del paciente en la fecha de la venta
            if (venta.PacienteId != null)
            {
                var hosp = _context.Hospitalizaciones
                    .Where(h => h.PacienteId == venta.PacienteId
                             && !h.Finalizada
                             && venta.FechaVenta >= h.FechaInicio
                             && venta.FechaVenta <= h.FechaFin)
                    .OrderByDescending(h => h.Id)
                    .FirstOrDefault();
                if (hosp != null) return hosp.Id;
            }
            return null;
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

            // ── Fuente 1: Consultas incluidas en el objeto hosp ──────────────────────────
            if (hosp.Consultas != null)
            {
                foreach (var consulta in hosp.Consultas)
                {
                    var cita = consulta.Citas;
                    if (cita == null) continue;

                    if (cita.EmpleadoId.HasValue)
                    {
                        var emp = _empleadoRepository.Get(cita.EmpleadoId.Value, false);
                        if (emp != null)
                            AgregarMedico(emp.NombreYApellidos, emp.Especialidad?.NombreEspecialidad, "Tratante");
                    }

                    if (cita.MedicosSecundarios != null)
                    {
                        foreach (var medId in cita.MedicosSecundarios)
                        {
                            var emp = _empleadoRepository.Get(medId, false);
                            if (emp != null)
                                AgregarMedico(emp.NombreYApellidos, emp.Especialidad?.NombreEspecialidad, "Secundario");
                        }
                    }
                }
            }

            // ── Fuente 2 (fallback): Buscar la cita de hospitalización del paciente en BD ─
            // Cubre el caso donde hosp.Consultas viene vacío o sin Citas cargadas
            if (!medicos.Any() && hosp.PacienteId > 0)
            {
                var citaHospi = _context.Citass
                    .Where(c => c.PacienteId == hosp.PacienteId
                             && !c.Eliminado
                             && c.CitaTipoAtencion == "Hospitalización")
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();

                if (citaHospi != null)
                {
                    if (citaHospi.EmpleadoId.HasValue)
                    {
                        var emp = _empleadoRepository.Get(citaHospi.EmpleadoId.Value, false);
                        if (emp != null)
                            AgregarMedico(emp.NombreYApellidos, emp.Especialidad?.NombreEspecialidad, "Tratante");
                    }

                    if (citaHospi.MedicosSecundarios != null)
                    {
                        foreach (var medId in citaHospi.MedicosSecundarios)
                        {
                            var emp = _empleadoRepository.Get(medId, false);
                            if (emp != null)
                                AgregarMedico(emp.NombreYApellidos, emp.Especialidad?.NombreEspecialidad, "Secundario");
                        }
                    }
                }
            }

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

            // Caso 1: Origen explícito (se guardó al facturar desde Consulta/Cita)
            if (v.Origen == "Consulta" || v.Origen == "Cita")
            {
                if (v.EmpleadoId != null)
                {
                    var emp = _empleadoRepository.Get(v.EmpleadoId.Value, false);
                    if (emp != null)
                        AgregarMedico(emp.NombreYApellidos, emp.Especialidad?.NombreEspecialidad, "Tratante");
                }

                // Buscar médicos secundarios desde la cita del paciente en esa fecha
                if (v.PacienteId != null)
                {
                    var cita = _context.Citass
                        .Where(c => c.PacienteId == v.PacienteId
                                 && !c.Eliminado
                                 && c.FechaInicio == v.FechaVenta.Date)
                        .OrderByDescending(c => c.Id)
                        .FirstOrDefault();
                    if (cita?.MedicosSecundarios != null)
                    {
                        foreach (var medId in cita.MedicosSecundarios)
                        {
                            var emp = _empleadoRepository.Get(medId, false);
                            if (emp != null)
                                AgregarMedico(emp.NombreYApellidos, emp.Especialidad?.NombreEspecialidad, "Secundario");
                        }
                    }
                }
            }
            // Caso 2: Inferir desde consulta del paciente en la misma fecha de la venta
            else if (v.PacienteId != null)
            {
                var consulta = _context.Consultas
                    .Include(c => c.Citas)
                        .ThenInclude(c => c.Empleado)
                    .Where(c => c.Citas.PacienteId == v.PacienteId
                             && c.FechaYHoraInicioConsulta.Date == v.FechaVenta.Date)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();

                if (consulta?.Citas != null)
                {
                    if (consulta.Citas.Empleado != null)
                        AgregarMedico(consulta.Citas.Empleado.NombreYApellidos,
                                      consulta.Citas.Empleado.Especialidad?.NombreEspecialidad, "Tratante");

                    if (consulta.Citas.MedicosSecundarios != null)
                    {
                        foreach (var medId in consulta.Citas.MedicosSecundarios)
                        {
                            var emp = _empleadoRepository.Get(medId, false);
                            if (emp != null)
                                AgregarMedico(emp.NombreYApellidos, emp.Especialidad?.NombreEspecialidad, "Secundario");
                        }
                    }
                }
            }

            // Caso 3: Venta de Hospital sin consulta vinculada
            // (cuando no se encontró hospitalizacionId por DetalleCaja y se cayó al flujo normal)
            if (!medicos.Any() && v.PacienteId != null && v.AmbienteId == (int)AmbienteEnum.Hospital)
            {
                var hospActiva = _context.Hospitalizaciones
                    .Include(h => h.Consultas).ThenInclude(c => c.Citas)
                    .Where(h => h.PacienteId == v.PacienteId
                             && h.FechaInicio.Date <= v.FechaVenta.Date)
                    .OrderByDescending(h => h.Id)
                    .FirstOrDefault();

                if (hospActiva != null)
                {
                    var medicosHospi = ObtenerMedicosDeHospitalizacion(hospActiva);
                    medicos.AddRange(medicosHospi);
                }
            }

            return medicos;
        }

        // ────────────────────────────────────────────────────────────
        // Construir ítem desde detalle de venta (productos, servicios, exámenes)
        // ────────────────────────────────────────────────────────────
        private ReporteVentaItemViewModel ConstruirItemDesdeDetalleVenta(
            Venta v,
            string modoDetalle,
            Action<string, string, decimal, decimal, decimal?> agregarItem,
            IProducto productoRepo)
        {
            decimal subProductos = 0m, subServicios = 0m, subExamenes = 0m, subHospital = 0m;
            decimal totalDesc = 0m, cantProductos = 0m, cantServicios = 0m, cantExamenes = 0m, cantHospital = 0m;
            var detalleItems = new List<ReporteVentaDetalleItemViewModel>();

            if (v.DetalleVenta != null)
            {
                foreach (var d in v.DetalleVenta)
                {
                    decimal lineaNeta = d.Subtotal > 0 ? Convert.ToDecimal(d.Subtotal) : Convert.ToDecimal(d.Total);
                    decimal cantidad = Convert.ToDecimal(d.Cantidad);
                    totalDesc += Convert.ToDecimal(d.Descuento);

                    string categoria;
                    decimal? costoUnitario = null;

                    if (d.BienOServicio == "B" && d.ProductoId != null)
                    {
                        categoria = "Producto";
                        subProductos += lineaNeta;
                        cantProductos += cantidad;

                        // Obtener costo unitario: prioridad → ProductoInventario → Producto.PrecioCosto
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
                        // Categoría real del examen
                        if (d.ExamenLabClinico?.CategoriaLabClinico != null)
                            categoria = d.ExamenLabClinico.CategoriaLabClinico.Nombre;
                        else if (d.ExamenLabClinico != null)
                            categoria = "Examen";
                        else
                            categoria = "Examen/Lab";
                        subExamenes += lineaNeta;
                        cantExamenes += cantidad;
                    }
                    else if (d.BienOServicio == "S" && d.ServicioId != null)
                    {
                        categoria = "Servicio";
                        subServicios += lineaNeta;
                        cantServicios += cantidad;
                    }
                    else
                    {
                        categoria = d.BienOServicio == "B" ? "Producto" : "Servicio";
                        subServicios += lineaNeta;
                        cantServicios += cantidad;
                    }

                    string nombre = d.Producto?.NombreProducto ?? d.Servicio?.NombreServicio ?? d.ExamenLabClinico?.NombreExamen ?? categoria;
                    agregarItem(categoria, nombre, cantidad, lineaNeta, costoUnitario);

                    if (modoDetalle == "descripcion")
                    {
                        detalleItems.Add(new ReporteVentaDetalleItemViewModel
                        {
                            Descripcion = nombre,
                            Categoria = categoria,
                            Cantidad = cantidad,
                            PrecioUnit = cantidad > 0 ? lineaNeta / cantidad : lineaNeta,
                            Subtotal = lineaNeta,
                            Descuento = Convert.ToDecimal(d.Descuento)
                        });
                    }
                }
            }

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
                MontoTotal = Convert.ToDecimal(v.MontoPago),
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
            string nombreProfesional = nota.User?.Persona?.NombreYApellidos ?? "Enfermero(a) no especificado";

            // Obtener la firma si está firmada y convertir a base64
            string firmaBase64 = null;
            if (nota.Firmado && !string.IsNullOrEmpty(nota.FirmaRuta))
            {
                string directorio = Directory.GetCurrentDirectory();
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

            return await _generatePdf.GetPdf("Views/CrearPDF/NotaEnfermeriaPDF.cshtml", model);
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
                string nombreProfesional = nota.User?.Persona?.NombreYApellidos ?? "Enfermero(a) no especificado";
                string firmaBase64 = null;
                if (nota.Firmado && !string.IsNullOrEmpty(nota.FirmaRuta))
                {
                    string directorio = Directory.GetCurrentDirectory();
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

            return await _generatePdf.GetPdf("Views/CrearPDF/NotasEnfermeriaAllPDF.cshtml", model);
        }


    }

}