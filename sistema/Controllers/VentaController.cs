using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using sistema.Models;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Http;
using sistema.Json;
using Wkhtmltopdf.NetCore;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Rotativa.AspNetCore;
using System.Text.Json;
using Database.Shared.Enumeraciones;
using Database.Shared.Data;
using farmamest.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentFormat.OpenXml.Presentation;
using farmamest.Service.IService;
using System.Security.Cryptography.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using sistema.Utilidades;

namespace sistema.Controllers
{
    public class VentaController : Controller
    {

        private readonly IVenta _ventaRepository = null;
        private readonly ICliente _clienteRepository = null;
        private readonly IConsultas _consultasRepository = null;

        private readonly IEmergencias _emergenciasRepository = null;

        private readonly IPacientes _pacienteRepository = null;
        private readonly IProducto _productoRepository = null;
        private readonly ILaboratorioClinico _laboratorioRepository = null;
        private readonly IRuta _rutaRepository = null;
        private readonly IEnvio _envioRepository = null;
        private readonly ICaja _cajaRepository = null;
        private readonly IEmpleado _empleadoRepository = null;
        private readonly IServicio _servicioRepository = null;
        private readonly IUser _userRepository = null;
        private readonly ISucursal _sucursalRepository = null;
        private readonly IGeneratePdf _generatePdf;
        private readonly UserManager<User> _userManager = null;
        private readonly IFormasPago _formasPagoRepository;

        #region Servicio (logica)
        private readonly IProductosService _productosService;
        #endregion

        private readonly IConfiguration _configuration;

        public VentaController(IVenta ventaRepository, ICliente clienteRepository, IProducto productoRepository, IRuta rutaRepository,
        IEnvio envioRepository,
        IServicio servicioRepository,
        ILaboratorioClinico laboratorioRepository,
        ICaja cajaRepository,
        IEmpleado empleadoRepository,
        IGeneratePdf generatePdf,
        ISucursal sucursalRepository,
        IUser userRepository,
        IFormasPago formasPagoRepository,
        UserManager<User> userManager,
        IPacientes pacienteRepository,
        IConsultas consultasRepository,
        IEmergencias emergenciasRepository,
        //Servicio (logica)
        IProductosService productosService,
        IConfiguration configuration
        )
        {
            _ventaRepository = ventaRepository;
            _clienteRepository = clienteRepository;
            _productoRepository = productoRepository;
            _rutaRepository = rutaRepository;
            _servicioRepository = servicioRepository;
            _laboratorioRepository = laboratorioRepository;
            _envioRepository = envioRepository;
            _cajaRepository = cajaRepository;
            _sucursalRepository = sucursalRepository;
            _generatePdf = generatePdf;
            _empleadoRepository = empleadoRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _pacienteRepository = pacienteRepository;
            _consultasRepository = consultasRepository;
            _emergenciasRepository = emergenciasRepository;
            _formasPagoRepository = formasPagoRepository;

            //Servicios (logica)
            _productosService = productosService;
            _configuration = configuration;

        }

        public async Task<IActionResult> prueba()
        {
            return await _generatePdf.GetPdf("Views/Venta/prueba.cshtml", "Hello World");
        }


        [Authorize]
        public IActionResult ListaVentasFarmacia(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _ventaRepository.PaginacionVentasFarmacia(sortOrder, buscar, pageNumber, 25);

            return View(lista);
        }



        [Authorize]
        public IActionResult ListaVentasClinica(string sortOrder, string buscar, string currentFilter, int? pageNumber,
            string fechaInicial, string fechaFinal, int numeroVenta, string comprobante, int formaPago, string origenVenta)
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

            ViewBag.FormasPago = new SelectList(ConsultarFormasPago(), "Id", "NombreFormaPago");



            var lista = _ventaRepository.PaginacionVentasClinica(sortOrder, buscar, pageNumber, 25, fechaInicial, fechaFinal,
                numeroVenta, comprobante, formaPago, origenVenta);

            foreach (var item in lista)
            {
                item.Detalles = _ventaRepository.GetDetalle(item.Id).ToList();
            }

            return View(lista);
        }




        [Authorize]
        public IActionResult ListaVentasHospital(string sortOrder, string buscar, string currentFilter, int? pageNumber,
            string fechaInicial, string fechaFinal, int numeroVenta, string comprobante, int formaPago)
        {
            try
            {

                Console.WriteLine("=== ListaVentasHospital EJECUTANDO ===");
                Console.WriteLine($"Parametros: fechaInicial={fechaInicial}, fechaFinal={fechaFinal}, numVenta={numeroVenta}, comp={comprobante}, formaPago={formaPago}");

                if (buscar != null) pageNumber = 1;
                else buscar = currentFilter;

                ViewData["CurrentFilter"] = buscar;
                ViewBag.FormasPago = new SelectList(ConsultarFormasPago(), "Id", "NombreFormaPago");

                var lista = _ventaRepository.PaginacionVentasHospital(sortOrder, buscar, pageNumber, 25,
                    fechaInicial, fechaFinal, numeroVenta, comprobante, formaPago);

                Console.WriteLine($"Cantidad de ventas devueltas: {lista.Count()}");

                if (lista.Count() == 0)
                {
                    Console.WriteLine("No se encontraron ventas para Hospital.");
                }
                else
                {
                    foreach (var item in lista)
                    {
                        Console.WriteLine($"Venta ID: {item.Id}, AmbienteId: {item.AmbienteId}, Fecha: {item.FechaVenta}");
                        // Cargar detalles y pagos para evitar null en la vista
                        item.DetalleVenta = _ventaRepository.GetDetalle(item.Id).ToList();
                        item.Pagos = _ventaRepository.Get(item.Id)?.Pagos?.ToList() ?? new List<Pagos>();
                        Console.WriteLine($"  Detalles: {item.DetalleVenta.Count()}, Pagos: {item.Pagos.Count()}");
                    }
                }

                return View(lista);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR EN CONSTRUCTOR: " + ex.Message);
                throw;
            }
        }

        [Authorize]
        public IActionResult ListaVentasLaboratorio(string sortOrder, string buscar, string currentFilter, int? pageNumber,
           string fechaInicial, string fechaFinal, int numeroVenta, string comprobante, int formaPago)
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

            ViewBag.FormasPago = new SelectList(ConsultarFormasPago(), "Id", "NombreFormaPago");


            var lista = _ventaRepository.PaginacionVentasLaboratorio(sortOrder, buscar, pageNumber, 25, fechaInicial, fechaFinal,
                numeroVenta, comprobante, formaPago);

            return View(lista);
        }
        [Authorize]

        public IActionResult ListaCuentas(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _ventaRepository.PaginacionVentasFarmacia(sortOrder, buscar, pageNumber, 25);

            return View(lista);
        }



        [Authorize]
        public IActionResult ListaPagos(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _ventaRepository.PaginacionVentasClinica(sortOrder, buscar, pageNumber, 25, null, null, null, null, null, null);

            return View(lista);
        }

        [Authorize]        // vista para reportar la utilidad por fechas
        public IActionResult ReporteFechas()
        {
            var modelo = new VentaBaseViewModel(0);
            return View(modelo);
        }



        [Authorize]        // vista para reportar las ventas por fechas, pdf y excel
        public IActionResult ReporteVentasFechas()
        {
            var modelo = new VentaBaseViewModel(0);
            modelo.Init(_empleadoRepository);
            return View(modelo);
        }

        [Authorize]
        public IActionResult ReporteEnviosFechas()
        {
            var modelo = new VentaBaseViewModel(0);
            modelo.Init(_empleadoRepository);
            return View(modelo);
        }


        [Authorize]
        public JsonResult BusquedaProductos(string codigo)
        {
            var len = codigo.Length;

            if (codigo.Length < 3)
            {
                return new JsonErrorResult(new { message = "Solo se acepta un mínimo 3 letras en delante." });
            }
            var buscado = _productoRepository.BuscarPorNombreYReferenciaBusquedaAjax(codigo);
            return Json(buscado);
        }

        [Authorize]
        public JsonResult BusquedaProductosClinica(string codigo)
        {
            if (ModelState.IsValid)
            {
                if (codigo == null || codigo.Length < 3)
                {
                    return new JsonErrorResult(new { message = "Solo se acepta un mínimo 3 letras en delante." });
                }

                var buscado = _productoRepository.BuscarPorNombreYReferenciaBusquedaAjaxClinica(codigo);
                return Json(buscado);
            }

            return new JsonErrorResult(new { message = "Ha ocurrido un error de servidor." });

        }

        [Authorize]
        public JsonResult GuardarEnvio([FromBody] ViewModelVenta2 det)
        {

            if (ModelState.IsValid)
            {
                if (det.datosenvio.EmpleadoId == "")
                {

                    TempData["Message"] = "¡Codigo de empleado incorrecto.!";
                    return Json("");
                }

                var empleado = _empleadoRepository.Get(Convert.ToInt32(det.datosenvio.EmpleadoId));

                if (empleado == null)
                {

                    TempData["Message"] = "¡Codigo de empleado incorrecto.!";
                    return Json("");
                }

                // string[] fecha = det.datosenvio.Fecha.Split('-');

                // var piloto = _ventaRepository.GetEmpleadoUser(det.datosenvio.UserId);
                var user = _userRepository.GetbyId(det.datosenvio.UserId);

                var nuevoEnvio = new Envio()
                {
                    // NombrePiloto = piloto.Nombre,
                    RutaId = Convert.ToInt32(det.datosenvio.Ruta),
                    FechaEntrega = Convert.ToDateTime(det.datosenvio.Fecha),
                    DireccionExacta = det.datosenvio.DireccionExacta,
                    Nombres = det.datosenvio.Nombre,
                    NoComprobante = det.datosenvio.NoComprobante,
                    Nit = det.datosenvio.Nit,
                    EmpleadoId = Convert.ToInt32(det.datosenvio.EmpleadoId),
                    UserId2 = user?.Id,
                    User = user,
                    FechaEnvio = DateTime.Now,
                    EstadosEnvioId = 1,

                };


                foreach (var item in det.detalle)
                {

                    var detalle = new DetalleEnvio()
                    {
                        Envio = nuevoEnvio,
                        Cantidad = Convert.ToInt32(item.Cantidad),
                        Precio = Convert.ToDecimal(item.Precio),
                        Descuento = Convert.ToDecimal(item.Descuento),
                        Subtotal = Convert.ToDecimal(item.Subtotal),
                        Total = Convert.ToDecimal(item.Total),
                        ProductoId = Convert.ToInt32(item.ProductoId),

                    };

                    _envioRepository.Add(detalle, false);


                }

                _envioRepository.saveChanges();

                TempData["Message"] = "¡El pedido se ha guardado con exito.!";

                return Json("Ok");



            }

            return Json("Hubo un error interno.");
        }


        [Authorize]
        public IActionResult Modificar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var Venta = _ventaRepository.Get((int)id);

            if (Venta == null)
            {
                return StatusCode(404);
            }

            var model = new VentaBaseViewModel(0)
            {
                Venta = Venta,
            };

            model.Init(_clienteRepository, _pacienteRepository);

            return View(model);
        }



        //[Authorize(Roles = "Administrador, Supervisor, Vendedor, Mensajero,Desarrollador,Farmacia,Laboratorio,Hospital,Clinica,Urologia,Algologia")]
        [Authorize]
        public JsonResult ModificarVenta([FromBody] ViewModelVenta2 det)
        {
            var listadoEnBd = _ventaRepository.GetDetalle(det.encabezado.Id);

            if (ModelState.IsValid)
            {
                foreach (var item in listadoEnBd)
                {
                    bool flag = false;

                    foreach (var ins in det.nuevos)
                    {
                        if (item.Id == Convert.ToInt32(ins.Ids))
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (!flag)
                    {
                        _ventaRepository.Delete(item.Id, false);
                    }



                }

                var venta = new Venta()
                {
                    Id = det.encabezado.Id,
                    NoComprobante = det.encabezado.NoComprobante,
                    PacienteId = Convert.ToInt32(det.encabezado.ClienteId),
                    Nit = det.encabezado.Nit,
                    Nombres = det.encabezado.Nombre,
                    Direccion = det.encabezado.Direccion,
                    EmpleadoId = det.encabezado.EmpleadoId,

                };

                _ventaRepository.Update(venta);


                foreach (var item in det.detalle)
                {

                    var nuevodetalle = new DetalleVenta()

                    {
                        VentaId = Convert.ToInt32(det.encabezado.Id),
                        ProductoId = Convert.ToInt32(item.ProductoId),
                        Cantidad = Convert.ToInt32(item.Cantidad),
                        Precio = Convert.ToDecimal(item.Precio),
                        Descuento = Convert.ToDecimal(item.Descuento),
                        Subtotal = Convert.ToDecimal(item.Subtotal),
                        Total = Convert.ToDecimal(item.Total),
                    };

                    _ventaRepository.Add(nuevodetalle, false);

                }

                _ventaRepository.saveChanges();

                return Json("OK");

            }


            return new JsonErrorResult(new { message = "Ha ocurrido un error de servidor." });
        }

        //[Authorize(Roles = "Administrador, Supervisor, Vendedor, Mensajero,Desarrollador,Farmacia,Laboratorio,Hospital,Clinica,Urologia,Algologia")]
        [Authorize]
        public IActionResult ExportarVentas()
        {
            var ventas = _ventaRepository.GetListado().OrderByDescending(a => a.FechaVenta);

            using (var workboook = new XLWorkbook())
            {
                var worksheet = workboook.Worksheets.Add("Ventas");
                var row = 1;
                worksheet.Cell(row, 1).Value = "Venta #";
                worksheet.Cell(row, 2).Value = "Cliente";
                worksheet.Cell(row, 3).Value = "NoComprobante";
                worksheet.Cell(row, 4).Value = "Nit";
                worksheet.Cell(row, 5).Value = "Nombres";
                worksheet.Cell(row, 6).Value = "Direccion";
                worksheet.Cell(row, 7).Value = "FechaVenta";
                worksheet.Cell(row, 8).Value = "Subtotal";
                worksheet.Cell(row, 9).Value = "Descuento";
                worksheet.Cell(row, 10).Value = "Total";



                foreach (var ven in ventas)
                {
                    row++;
                    worksheet.Cell(row, 1).Value = ven.Id; ;
                    worksheet.Cell(row, 2).Value = ven.Paciente.Nombre;
                    worksheet.Cell(row, 3).Value = ven.NoComprobante;
                    worksheet.Cell(row, 4).Value = ven.Nit;
                    worksheet.Cell(row, 5).Value = ven.Nombres;
                    worksheet.Cell(row, 6).Value = ven.Direccion;
                    worksheet.Cell(row, 7).Value = ven.FechaVenta;
                    worksheet.Cell(row, 8).Value = ven.DetalleVenta.Sum(a => a.Subtotal);
                    worksheet.Cell(row, 9).Value = ven.DetalleVenta.Sum(a => a.Descuento);
                    worksheet.Cell(row, 10).Value = ven.DetalleVenta.Sum(a => a.Total);


                }

                using (var stream = new MemoryStream())
                {
                    workboook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officefocument.spreadsheet",
                    "Ventas.xlsx"
                    );

                }



            }

        }

        //[Authorize(Roles = "Administrador, Supervisor, Vendedor, Mensajero,Desarrollador,Farmacia,Laboratorio,Hospital,Clinica,Urologia,Algologia")]
        public IActionResult ExportarDetalleVentas()
        {
            var detalles = _ventaRepository.GetListadoDetalles();

            using (var workboook = new XLWorkbook())
            {
                var worksheet = workboook.Worksheets.Add("Ventas");
                var row = 1;
                worksheet.Cell(row, 1).Value = "CodigoProducto";
                worksheet.Cell(row, 2).Value = "NombreProducto";
                worksheet.Cell(row, 3).Value = "Cantidad";
                worksheet.Cell(row, 4).Value = "Precio";
                worksheet.Cell(row, 5).Value = "Descuento";
                worksheet.Cell(row, 6).Value = "Subtotal";
                worksheet.Cell(row, 7).Value = "Total";
                worksheet.Cell(row, 8).Value = "VentaId";


                foreach (var det in detalles)
                {
                    row++;
                    worksheet.Cell(row, 1).Value = det.Producto.CodigoReferencia;
                    worksheet.Cell(row, 2).Value = det.Producto.NombreProducto;
                    worksheet.Cell(row, 3).Value = det.Cantidad;
                    worksheet.Cell(row, 4).Value = det.Precio;
                    worksheet.Cell(row, 5).Value = det.Descuento;
                    worksheet.Cell(row, 6).Value = det.Subtotal;
                    worksheet.Cell(row, 7).Value = det.Total;
                    worksheet.Cell(row, 8).Value = det.VentaId;


                }

                using (var stream = new MemoryStream())
                {
                    workboook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officefocument.spreadsheet",
                    "DetalleVentas.xlsx"
                    );

                }



            }

        }

        // public IActionResult VerGraficaFecha()
        //  {



        //     return View();
        // }

        // public IActionResult GraficaVentas(string mesdesde, string meshasta)
        //  {

        //     if(mesdesde == null || meshasta == null)
        //     {

        //         return View();
        //     }

        //     var fechahoy= DateTime.Now;
        //     var anio = fechahoy.Year;

        //    var desde = mesdesde+"/"+"01/"+anio;
        //     var hasta = meshasta+"/"+"01/"+anio;

        //       //var fechas = fecha.Split('-');
        //       var ventas = new List<Venta>();

        //     // ventas = _ventaRepository.GetVentasFechas(Convert.ToDateTime(desde), Convert.ToDateTime(hasta).AddDays(1));

        //      ventas = _ventaRepository.GetListadoFecha(Convert.ToDateTime(desde), Convert.ToDateTime(hasta).AddDays(1));
        //         //return Json (ventas);


        //     return View(ventas);
        //     //return Json(ventas);

        // }

        [Authorize]
        //[Authorize(Roles = "Administrador, Supervisor, Vendedor, Mensajero,Desarrollador,Farmacia,Laboratorio,Hospital,Clinica,Urologia,Algologia")]
        public IActionResult AnularVenta(int? Id, string returnAction)
        {
            if (Id == null) return BadRequest("request is incorrect");

            var venta = _ventaRepository.Get((int)Id);

            if (venta == null) return StatusCode(404);

            venta.Eliminado = true; // cambiamos el estado a falso, practicamente es anulado.
            _ventaRepository.Update(venta, false);

            // regresar los productos a inventario y sumarlos
            foreach (var item in venta.DetalleVenta)
            {
                if (item.Producto != null)
                {
                    var producto = _productoRepository.Get(item.Producto.Id);
                    producto.Stock += item.Cantidad;
                    _productoRepository.Update(producto, false);
                }
            }

            // Eliminar el registro en caja, sin importar el contexto
            var detallecaja = _cajaRepository.GetDetalleCaja((int)Id);
            if (detallecaja != null)
                _cajaRepository.GetDetalleCajaPorVentaId(detallecaja.Id, false);

            // Redirección dinámica basada en el returnAction
            return RedirectToAction(returnAction);
        }


        //[Authorize(Roles = "Administrador, Supervisor, Vendedor, Mensajero,Desarrollador,Farmacia,Laboratorio,Hospital,Clinica,Urologia,Algologia")]
        //[Authorize]
        //public IActionResult AnularVentaClinica(int? Id)
        //{
        //    if (Id == null) return BadRequest("request is incorrect");

        //    var venta = _ventaRepository.Get((int)Id);

        //    if (venta == null) return StatusCode(404);


        //    venta.Eliminado = true; // cambiamos el estado a falso, practicamente es anulado.
        //    _ventaRepository.Update(venta, false);

        //    // regresar los productos a inventario y sumarlos
        //    foreach (var item in venta.DetalleVenta)
        //    {
        //        if (item.Producto != null)
        //        {
        //            var producto = _productoRepository.Get(item.Producto.Id);
        //            producto.Stock += item.Cantidad;
        //            _productoRepository.Update(producto, false);
        //        }

        //    }

        // tambien debemos de eliminar el registro en caja.
        //var detallecajaclinica = _cajaClinicaRepository.GetDetalleCaja((int)Id);
        //_cajaClinicaRepository.GetDetalleCajaPorVentaIdC(detallecajaclinica.Id, false);

        //    var detallecajaclinica = _cajaRepository.GetDetalleCaja((int)Id);
        //    _cajaRepository.GetDetalleCajaPorVentaId(detallecajaclinica.Id, false);
        //    return RedirectToAction("ListaVentasClinica");
        //}


        public async Task<IActionResult> Reporte(string fecha, int? empleadoid)
        {
            var fechas = fecha.Split('-');
            var ventas = new List<Venta>();

            if (empleadoid == null)
            {

                ventas = _ventaRepository.GetListadoFecha(Convert.ToDateTime(fechas[0]), Convert.ToDateTime(fechas[1]).AddDays(1));
            }
            else
            {
                ventas = _ventaRepository.GetListadoFechaEmpleado(Convert.ToDateTime(fechas[0]), Convert.ToDateTime(fechas[1]).AddDays(1), empleadoid);
            }

            var model = new ReportesVentasViewModel()
            {
                Ventas = ventas,
                Desde = fechas[0],
                Hasta = fechas[1],
            };

            ViewBag.Desde = "2";
            return await _generatePdf.GetPdf("Views/Venta/Reporte.cshtml", model);
        }


        #region Nueva venta unificada
        [HttpPost]
        public string BuscarProductosNombre(string nombre)
        {
            try
            {
                if (nombre == null || nombre.Length < 3)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Solo se acpeta un minimo de 3 letras en adelante"
                    });
                }

                var productosConsultados = new List<VentaUnificadaProductoExistenteViewModel>();
                var productosBd = _productoRepository.BuscarProductosNombre(nombre);
                if (productosBd != null)
                {
                    foreach (var producto in productosBd)
                    {
                        if (producto.ProductosInventarioPrecios != null || producto.ProductosInventarioPrecios.Count > 0)
                        {
                            foreach (var precio in producto.ProductosInventarioPrecios)
                            {
                                productosConsultados.Add(new VentaUnificadaProductoExistenteViewModel
                                {
                                    ProductoInventarioId = producto.Id,
                                    ProductoId = producto.ProductoId,
                                    PrecioId = precio.PrecioId,
                                    PrecioNombre = precio.Precio.NombrePrecio,
                                    PrecioValor = precio.Valor,
                                    ProductoCodigo = producto.Producto.CodigoReferencia,
                                    ProductoNombre = producto.Producto.NombreProducto,
                                    ProductoActivoConcentracion = producto.Producto.ActivoYConcentracion,
                                    ProductoPresentacion = producto.Producto.PresentacionProducto.PresentProducto,
                                    ProductoGrupoTerapeutico = producto.Producto.GrupoTProducto.NombreGrupoT,
                                    ProductoViaAdministracion = producto.Producto.Viadmin.NombreViadmin,
                                    ProductoLaboratorio = producto.Producto.LaboratorioProducto.NombreLaboratorioProducto,
                                    ProductoImagen = producto.Producto.Imagen,
                                    ProductoStock = producto.Stock,
                                    UnidadMedidaVentaId = producto.UnidadMedidaVenta == null ? 0 : producto.UnidadMedidaVentaId,
                                    UnidadMedidaVentaNombre = producto.UnidadMedidaVenta == null ? "-" : producto.UnidadMedidaVenta.Nombre
                                });
                            }
                        }
                    }
                }

                if (productosConsultados.Count() == 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se encontraron coincidencias"
                    });
                }


                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = productosConsultados
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error del servidor al buscar producto: " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarProductosExistentes(int? ambienteId)
        {
            try
            {
                string clienteNombre = _configuration["Cliente"] ?? "";
                bool esHSC = clienteNombre.Equals("HSC", StringComparison.OrdinalIgnoreCase);

                int? ambienteParametro = null;
                int? bodegaParametro = null;

                switch (ambienteId)
                {
                    case 1:
                        if (esHSC)
                        {
                            ambienteParametro = null;  // Productos son Global (AmbienteId=6), no filtrar
                            bodegaParametro = 1;       // Bodega Farmacia
                        }
                        else
                        {
                            ambienteParametro = 1;
                            bodegaParametro = null;
                        }
                        break;
                    case 3:
                        bodegaParametro = 10; // Bodega Emergencia
                        break;
                }

                var inventario = _productosService.GetInventario(ambienteParametro, bodegaParametro);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = inventario
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos existentes. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarProductosExistentesActivoConcentracion(string ProductoActivoConcentracion, bool isClinica, bool isFarmacia,
          bool isLaboratorio, int? sucursalId)
        {
            try
            {
                int? ambienteId = null;
                if (isClinica)
                {
                    ambienteId = (int)AmbienteEnum.Clinica;
                }
                if (isFarmacia)
                {
                    ambienteId = (int)AmbienteEnum.Farmacia;
                }
                if (isLaboratorio)
                {
                    ambienteId = (int)AmbienteEnum.Laboratorio;
                }

                var productosExistentes = new List<VentaUnificadaProductoExistenteViewModel>();



                var productosBd = _productoRepository.GetInventarioProductos
                   (null, null, null, null, sucursalId, null, ambienteId)
                   .Where(p => p.ActivoYConcentracion == ProductoActivoConcentracion);

                if (productosBd != null)
                {
                    foreach (var producto in productosBd)
                    {
                        if (producto.ProductosInventario != null
                            && producto.ProductosInventario.Count() > 0)
                        {
                            foreach (var registroInventario in producto.ProductosInventario)
                            {
                                if (registroInventario.Stock > 0)
                                {
                                    string unidadNombre = registroInventario.UnidadMedidaVenta != null
                                        ? registroInventario.UnidadMedidaVenta.Nombre
                                        : "-";
                                    productosExistentes.Add(new VentaUnificadaProductoExistenteViewModel
                                    {
                                        ProductoId = producto.Id,
                                        ProductoInventarioId = registroInventario.Id,
                                        ProductoCodigo = producto.CodigoReferencia,
                                        UnidadMedidaVentaId = registroInventario.UnidadMedidaVentaId,
                                        UnidadMedidaVentaNombre = unidadNombre,
                                        ProductoStock = registroInventario.Stock,
                                        ProductoNombre = producto.NombreProducto,
                                        ProductoActivoConcentracion = producto.ActivoYConcentracion
                                    });

                                }
                            }
                        }
                    }
                }


                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = productosExistentes
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar productos: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarPreciosProducto(int productoInventarioId)
        {
            try
            {
                var preciosProducto = new List<VentaUnificadaPrecioViewModel>();
                var preciosBd = _productoRepository
                    .ConsultarPreciosProductoInventario(productoInventarioId);
                if (preciosBd != null)
                {
                    foreach (var precio in preciosBd)
                    {
                        preciosProducto.Add(new VentaUnificadaPrecioViewModel
                        {
                            PrecioId = precio.PrecioId,
                            PrecioNombre = precio.Precio.NombrePrecio,
                            PrecioValor = precio.Valor
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = preciosProducto
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar precios: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarServiciosExistentes()
        {
            try
            {
                var serviciosExistentes = new List<VentaUnificadaServicioExistenteViewModel>();
                var serviciosBd = _servicioRepository.GetListaServicios();
                if (serviciosBd != null)
                {
                    foreach (var servicio in serviciosBd)
                    {
                        serviciosExistentes.Add(new VentaUnificadaServicioExistenteViewModel
                        {
                            ServicioId = servicio.Id,
                            ServicioCodigo = servicio.CodigoInterno,
                            ServicioNombre = servicio.NombreServicio
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = serviciosExistentes
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarPreciosServicio(int servicioId)
        {
            try
            {
                var preciosServicio = new List<VentaUnificadaPrecioViewModel>();
                var preciosBd = _servicioRepository
                    .GetPreciosServicio(servicioId);
                if (preciosBd != null)
                {
                    foreach (var precio in preciosBd)
                    {
                        preciosServicio.Add(new VentaUnificadaPrecioViewModel
                        {
                            PrecioId = precio.PrecioId,
                            PrecioNombre = precio.Precio.NombrePrecio,
                            PrecioValor = precio.Valor
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = preciosServicio
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar precios: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarExamenesExistentes()
        {
            try
            {
                Console.WriteLine("[COEX][Examenes][Backend] ConsultarExamenesExistentes iniciado");
                //var examenesExistentes = new List<VentaUnificadaExamenExistenteViewModel>();
                var examenesExistentes = _laboratorioRepository.GetListExamenesLaboratorioSP();
                Console.WriteLine($"[COEX][Examenes][Backend] Examenes enviados: {examenesExistentes?.Count ?? 0}");
                if (examenesExistentes != null && examenesExistentes.Any())
                {
                    var primero = examenesExistentes.First();
                    Console.WriteLine($"[COEX][Examenes][Backend] Primer examen: Id={primero.ExamenId}, Codigo={primero.ExamenCodigo}, Nombre={primero.ExamenNombre}");
                }
                //if (examenesBd != null)
                //{
                //    foreach (var examen in examenesBd)
                //    {
                //        examenesExistentes.Add(new VentaUnificadaExamenExistenteViewModel
                //        {
                //            ExamenId = examen.Id,
                //            ExamenCodigo = examen.CodigoInterno,
                //            ExamenNombre = examen.NombreExamen
                //        });
                //    }
                //}w
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = examenesExistentes
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[COEX][Examenes][Backend] Error: {ex}");
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar examenes: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarPreciosExamen(int examenLabClinicoId, string fecha)
        {
            try
            {
                //Se captura la hora de la fecha en la que se agendo la cita
                var fechaHora = Convert.ToDateTime(fecha);
                int hora = fechaHora.Hour;
                int minutos = fechaHora.Minute;
                //Los Identificadores de precios que no deben salir en la lista
                //int idPrecio = 0;
                //int idPrecio2 = 0;

                var preciosExamen = new List<VentaUnificadaPrecioViewModel>();
                //if (hora == 17 && minutos != 0)//Si la hora enque se agendo la cita es mayor a las  17
                //{
                //    idPrecio = 1;
                //    idPrecio2 = 12;
                //}
                //else if (hora >= 7 && hora <= 17)
                //{
                //    idPrecio = 10;
                //    idPrecio2 = 13;
                //}
                //else
                //{

                //    idPrecio = 1;
                //    idPrecio2 = 12;
                //}

                //var preciosBd = _laboratorioRepository.GetPreciosExamen(examenLabClinicoId)
                //    .Where(a => a.PrecioId != idPrecio && a.PrecioId != idPrecio2)
                //    .OrderBy(a => a.PrecioId);
                //    ;

                var preciosBd = _laboratorioRepository.GetPreciosExamen(examenLabClinicoId)
                    .OrderBy(a => a.PrecioId);
                ;

                if (preciosBd != null)
                {
                    foreach (var precio in preciosBd)
                    {
                        preciosExamen.Add(new VentaUnificadaPrecioViewModel
                        {
                            PrecioId = precio.PrecioId,
                            PrecioNombre = precio.Precio.NombrePrecio,
                            PrecioValor = precio.PrecioValor
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = preciosExamen
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar precios: " + ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult NuevaVentaClinica(int? consultaId = null, int? emergenciaId = null)
            => NuevaVentaUnificadaPorTipo("clinica", consultaId, emergenciaId);

        [HttpGet]
        [Authorize]
        public IActionResult NuevaVentaFarmacia(int? consultaId = null, int? emergenciaId = null)
            => NuevaVentaUnificadaPorTipo("farmacia", consultaId, emergenciaId);

        [HttpGet]
        [Authorize]
        public IActionResult NuevaVentaLaboratorio(int? consultaId = null, int? emergenciaId = null)
            => NuevaVentaUnificadaPorTipo("laboratorio", consultaId, emergenciaId);

        [HttpGet]
        [Authorize]
        public IActionResult NuevaVentaUnificada(int? consultaId = null, int? emergenciaId = null)
        {
            if (!TryResolveTipoVentaDesdeRequest(Request.Query, out var tipoVenta, out _, out _, out _, out _, out _))
            {
                TempData["Message"] = "Error de navegación";
                return RedirectToAction("Index", "Home");
            }

            return NuevaVentaUnificadaPorTipo(tipoVenta, consultaId, emergenciaId);
        }

        private IActionResult NuevaVentaUnificadaPorTipo(
            string tipoVenta,
            int? consultaId = null,
            int? emergenciaId = null)
        {
            var model = new VentaUnificadaViewModel();
            model.ApplyTipoVenta(tipoVenta);

            if (!model.IsClinica && !model.IsFarmacia && !model.IsLaboratorio && !model.IsEmergencia && !model.IsHospital)
            {
                TempData["Message"] = "Error de navegación";
                return RedirectToAction("Index", "Home");
            }

            var userName = _userManager.GetUserName(HttpContext.User);
            var user = _userRepository.Get(userName);
            if (user == null)
            {
                TempData["Message"] = "No se encontró el usuario en el sistema.";
                return RedirectToAction("Index", "Home");
            }

            string cliente = "";
            string citaTipoAtencion = "";
            int clienteId = 0;
            int? condigoMedico = 0;

            string ResponsableNit = "";
            string ResponsableNombre = "";
            string ResponsableDireccion = "";
            string ResponsableCorreo = "";
            string ResponsableDPI = "";
            string Origen = "";

            DateTime? FechaNacimientoPaciente = null;

            if (consultaId != null)
            {
                var consulta = _consultasRepository.GetConsulta((int)consultaId);
                if (consulta == null || consulta.Citas == null)
                {
                    TempData["Message"] = "La consulta no existe";
                    return RedirectToAction("Index", "Home");
                }
                condigoMedico = consulta.Citas.EmpleadoId;
                var pacienteConsulta = _pacienteRepository.GetPacientePorId((int)consulta.Citas.PacienteId);
                if (pacienteConsulta == null)
                {
                    TempData["Message"] = "Paciente de la consulta no encontrado";
                    return RedirectToAction("Index", "Home");
                }
                cliente = pacienteConsulta.Nombre;
                clienteId = pacienteConsulta.Id;
                citaTipoAtencion = consulta.Citas.CitaTipoAtencion;
                ResponsableNit = consulta.Citas.ResponsableNit;
                ResponsableNombre = consulta.Citas.ResponsableNombre;
                ResponsableDireccion = consulta.Citas.ResponsableDireccion;
                ResponsableCorreo = consulta.Citas.ResponsableCorreo;
                ResponsableDPI = consulta.Citas.ResponsableDPI;
                FechaNacimientoPaciente = consulta.Citas.Paciente.FechaNacimiento;
                Origen = "";
            }

            if (emergenciaId != null)
            {
                var emergencia = _emergenciasRepository.Get((int)emergenciaId, true, true);
                if (emergencia == null)
                {
                    TempData["Message"] = "La emergencia no existe";
                    return RedirectToAction("Index", "Home");
                }

                condigoMedico = emergencia.EmpleadoId;

                var paciente = _pacienteRepository.GetPacientePorId((int)emergencia.PacienteId);
                if (paciente == null)
                {
                    TempData["Message"] = "Paciente de la emergencia no encontrado";
                    return RedirectToAction("Index", "Home");
                }

                cliente = paciente.Nombre;
                clienteId = paciente.Id;
                FechaNacimientoPaciente = paciente.FechaNacimiento;
                citaTipoAtencion = "EMERGENCIA";
                ResponsableNit = paciente.Nit;
                ResponsableNombre = paciente.ResponsableNombre;
                ResponsableDireccion = paciente.ResponsableDireccion;
                ResponsableCorreo = paciente.ResponsableCorreo;
                ResponsableDPI = paciente.ResponsableDPI;
                Origen = "EMERGENCIA";
            }

            model.ConsultaId = consultaId;
            model.EmergenciaId = emergenciaId;
            model.CitaTipoAtencion = citaTipoAtencion;
            model.CodigoVendedor = user.EmpleadoId;
            model.ResponsableNit = ResponsableNit;
            model.ResponsableNombre = ResponsableNombre;
            model.ResponsableDireccion = ResponsableDireccion;
            model.ResponsableCorreo = ResponsableCorreo;
            model.ResponsableDPI = ResponsableDPI;
            model.PacienteFechaNacimiento = FechaNacimientoPaciente ?? DateTime.MinValue;
            model.Origen = Origen;

            model.Init(_pacienteRepository, _clienteRepository,
                _envioRepository, _empleadoRepository, _sucursalRepository);
            model.Cliente = (consultaId != null || emergenciaId != null) ? cliente : null;
            model.ClienteId = clienteId;
            model.CodigoMedico = condigoMedico;
            return View("NuevaVentaUnificada", model);
        }
        [HttpPost]
        public string NuevaVentaUnificada(VentaUnificadaViewModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.TipoVenta))
                    model.ApplyTipoVenta();

                var fechaHora = DateTime.Now;

                //Empleado
                //var empleado = new Empleado();
                //if (model.CodigoVendedor != null)
                //{
                //    empleado = _empleadoRepository.Get((int)model.CodigoVendedor);
                //}

                #region CLIENTE O PACIENTE
                Paciente paciente = null;

                int EdadCalculada = (model.PacienteFechaNacimiento == DateTime.MinValue)
                            ? 0
                            : DateTime.Today.Year - model.PacienteFechaNacimiento.Year -
                            (DateTime.Today < model.PacienteFechaNacimiento.AddYears(DateTime.Today.Year - model.PacienteFechaNacimiento.Year) ? 1 : 0);


                if (model.ClienteId != null && model.ClienteId > 0)
                {
                    paciente = _pacienteRepository.Get((int)model.ClienteId, false);
                    if (paciente == null)
                    {
                        return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "El paciente seleccionado no existe." });
                    }
                    paciente.FechaNacimiento = model.PacienteFechaNacimiento;
                    paciente.Edad = EdadCalculada;
                    _pacienteRepository.Update(paciente);

                }
                else if (!string.IsNullOrWhiteSpace(model.Cliente))
                {
                    paciente = new Paciente
                    {
                        Nombre = model.Cliente.Trim(),
                        Direccion = model.Direccion,
                        Nit = model.Nit,
                        EstadoPacienteId = (int)EstadoPacienteEnum.Activo,
                        TipoPacienteId = (int)TipoPacienteEnum.Nuevo,
                        FechaNacimiento = model.PacienteFechaNacimiento,
                        Edad = EdadCalculada,
                    };

                }

                var nombreVenta = paciente?.Nombre
                    ?? model.ResponsableNombre
                    ?? model.Cliente
                    ?? "CF";

                #endregion

                #region CAJA GLOBAL Y CAJA LOCAL
                //GLOBAL
                var cajasGlobales = _cajaRepository.ListarCajas()
                    .Where(a => a.SucursalId == model.SucursalId
                                && a.AmbienteId == (int)AmbienteEnum.Global)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Total de cajas globales encontradas: {cajasGlobales.Count}");
                foreach (var caja in cajasGlobales)
                {
                    System.Diagnostics.Debug.WriteLine($"Caja Id: {caja.Id}, EstadoCaja: {caja.EstadoCaja}");
                }

                var cajaGlobal = cajasGlobales.Where(a => a.EstadoCaja).FirstOrDefault();

                //LOCAL (AMBIENTE)
                var cajas = _cajaRepository.ListarCajas()
                        .Where(a => a.SucursalId == model.SucursalId
                        && a.AmbienteId == model.AmbienteId)
                        .ToList();
                if (!cajas.Any(a => a.EstadoCaja))
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "¡Error. No hay cajas abiertas. Por favor debe abrir una caja.!"
                    });
                }
                var cajaLocal = cajas.Where(a => a.EstadoCaja).FirstOrDefault();
                #endregion


                #region Creacion de objeto VENTA

                if (string.IsNullOrEmpty(model.Origen))
                {
                    if (model.EmergenciaId != null) model.Origen = "EMERGENCIA";
                    else if (model.ConsultaId != null) model.Origen = "";
                    else model.Origen = "";
                }

                var venta = new Venta
                {
                    NoComprobante = model.NumeroComprobante,
                    Nombres = nombreVenta,
                    Nit = !string.IsNullOrWhiteSpace(model.Nit)
                        ? model.Nit
                        : (!string.IsNullOrWhiteSpace(model.ResponsableNit)
                            ? model.ResponsableNit
                            : paciente?.Nit),
                    Direccion = !string.IsNullOrWhiteSpace(model.ResponsableDireccion)
                        ? model.ResponsableDireccion
                        : paciente?.Direccion,
                    Correo = !string.IsNullOrWhiteSpace(model.ResponsableCorreo)
                        ? model.ResponsableCorreo
                        : paciente?.Email,
                    ResponsableNombre = model.ResponsableNombre,
                    Paciente = paciente,
                    FechaVenta = fechaHora,
                    EmpleadoId = model.CodigoVendedor,
                    AmbienteId = model.AmbienteId,
                    MontoPago = model.PagoMonto,
                    Vuelto = model.PagoVuelto,
                    UuidFel = model.UuidFel,
                    Origen = model.Origen
                };

                #endregion

                #region PAGOS
                if (model.Pagos != null)
                {
                    foreach (var pago in model.Pagos)
                    {
                        venta.Pagos.Add(new Pagos
                        {
                            FechaHora = fechaHora,
                            FormaPagoId = pago.FormaPagoId,
                            Monto = pago.ValorTotal
                        });
                    }
                }
                if (model.ValorCubiertoSeguro > 0)
                {
                    venta.Pagos.Add(new Pagos
                    {
                        FechaHora = fechaHora,
                        FormaPagoId = (int)FormaPagoEnum.Seguro,
                        Monto = model.ValorCubiertoSeguro
                    });
                }
                #endregion

                #region PRODUCTOS
                if (model.Productos != null)
                {
                    foreach (var productoVenta in model.Productos)
                    {
                        if (productoVenta.VentaPerdida)
                        {
                            //Si es venta perdida se registra en una tabla especial que lleva el control
                            //de las ventas que se pierden por no existir dicho producto en el sistema
                            var ventaPerdida = new VentaPerdida
                            {
                                FechaRegistro = fechaHora,
                                ProductoNombre = productoVenta.ProductoNombre,
                                Descripcion = "Producto no manejado por el establecimiento"
                            };
                            _ventaRepository.Add(ventaPerdida);
                        }
                        else
                        {
                            var registroProductoInventario = _productoRepository
                                .GetRegistroInventarioProducto(0, productoVenta.ProductoId);


                            //Tambien se registra una venta perdida cuando el Stock esta en 0
                            //o hay un Stock bajo
                            if (registroProductoInventario == null || registroProductoInventario.Stock < productoVenta.Cantidad)
                            {
                                var ventaPerdida = new VentaPerdida
                                {
                                    FechaRegistro = fechaHora,
                                    ProductoId = productoVenta.ProductoId,
                                    Descripcion = "Stock bajo"
                                };
                                _ventaRepository.Add(ventaPerdida);
                            }
                            //Se agrega el detalle de la venta SI y SOLO si hay stock
                            if (registroProductoInventario != null)
                            {
                                var nuevodetalle = new DetalleVenta
                                {
                                    Venta = venta,
                                    Cantidad = productoVenta.Cantidad,
                                    Precio = productoVenta.PrecioValor,
                                    Descuento = productoVenta.DescuentoValor,
                                    Subtotal = Convert.ToDecimal(productoVenta.Subtotal),
                                    Total = Convert.ToDecimal(productoVenta.PrecioValor * productoVenta.Cantidad),
                                    ProductoId = productoVenta.ProductoId,
                                    //ProductoInventarioId = productoVenta.ProductoInventarioId,
                                    BienOServicio = "B",
                                    Cargo = productoVenta.Recargo,
                                    DescuentoPorcentaje = productoVenta.DescuentoProductoPorcentaje,
                                    UsuarioAutorizaModificacion = productoVenta.UsuarioAutoriza
                                };
                                venta.DetalleVenta.Add(nuevodetalle);

                                //ACTUALIZAR INVENTARIO
                                registroProductoInventario.Stock -= productoVenta.Cantidad;
                                if (registroProductoInventario.Stock < 0)
                                {
                                    registroProductoInventario.Stock = 0;
                                }
                                _productoRepository.UpdateRegistroInventario(registroProductoInventario, false);

                                //Agregar historico de movimiento
                                var movimiento = new MovimientoProducto
                                {
                                    UsuarioRealizaId = _userManager.GetUserId(HttpContext.User),
                                    ProductoInventarioId = registroProductoInventario.Id,
                                    Fecha = fechaHora,
                                    PrecioUnitario = productoVenta.PrecioValor,
                                    MontoTotal = nuevodetalle.Total,
                                    Cantidad = nuevodetalle.Cantidad,
                                    DescripcionMovimiento = "Venta de producto",
                                    SaldoActual = registroProductoInventario.Stock,
                                    TipoMovimientoProductoId = (int)TipoMovimientoProductoEnum.SalidaVenta
                                };
                                _productoRepository.Add(movimiento);
                            }
                        }
                    }
                }
                #endregion

                #region SERVICIOS
                if (model.Servicios != null)
                {
                    foreach (var servicioVenta in model.Servicios)
                    {

                        var nuevodetalle = new DetalleVenta
                        {
                            Venta = venta,
                            Cantidad = servicioVenta.Cantidad,
                            Precio = servicioVenta.ValorUnitario,
                            Descuento = servicioVenta.DescuentoValor,
                            Subtotal = servicioVenta.ValorSubtotal,
                            Total = servicioVenta.ValorUnitario * servicioVenta.Cantidad,
                            ServicioId = servicioVenta.ServicioId,
                            BienOServicio = "S",
                            Cargo = servicioVenta.Recargo,
                            DescuentoPorcentaje = servicioVenta.DescuentoServicioPorcentaje,
                            UsuarioAutorizaModificacion = servicioVenta.UsuarioAutoriza
                        };
                        venta.DetalleVenta.Add(nuevodetalle);

                        //Actualizar inventario
                        _servicioRepository.ActualizarInventarioVentaServicio(servicioVenta.ServicioId);
                    }
                }
                #endregion

                #region EXAMENES DE LABORATORIO
                var examen = new Examen();
                if (model.Examenes != null)
                {
                    //Medico
                    var medico = _empleadoRepository.GetMedicoByName(model.Medico);
                    if (medico == null)
                    {
                        medico = new Medicos()
                        {
                            Nombres = model.Medico
                        };
                    }

                    //Clinica
                    var clinica = _empleadoRepository.GetClinicaByName(model.Clinica);
                    if (clinica == null)
                    {
                        clinica = new Clinica()
                        {
                            NombreClinica = model.Clinica
                        };
                    }

                    examen = new Examen()
                    {
                        Paciente = paciente,
                        EstadoExamenId = 1,
                        FechaRealizacion = fechaHora,
                        Medicos = medico,
                        Clinicas = clinica,
                        UsuarioSolicita = _userManager.GetUserId(HttpContext.User),
                        ClinicaReferida = clinica.NombreClinica,
                        EmpleadoId = model.CodigoVendedor
                    };
                    foreach (var examenAgregado in model.Examenes)
                    {
                        //DEtalle de ventas
                        var nuevodetalleVenta = new DetalleVenta
                        {
                            Venta = venta,
                            Cantidad = examenAgregado.Cantidad,
                            Precio = examenAgregado.ValorUnitario,
                            Descuento = examenAgregado.DescuentoValor,
                            Subtotal = examenAgregado.ValorSubtotal,
                            Total = examenAgregado.ValorUnitario * examenAgregado.Cantidad,
                            ExamenLabClinicoId = examenAgregado.ExamenId,
                            BienOServicio = "S",
                            Cargo = examenAgregado.Recargo,
                            DescuentoPorcentaje = examenAgregado.DescuentoExamenPorcentaje,
                            UsuarioAutorizaModificacion = examenAgregado.UsuarioAutoriza
                        };
                        venta.DetalleVenta.Add(nuevodetalleVenta);
                        //DEtalle de examenes
                        var nuevoDetalle = new DetalleExamen()
                        {
                            Cantidad = examenAgregado.Cantidad,
                            PrecioValor = examenAgregado.ValorUnitario,
                            PrecioId = examenAgregado.PrecioId,
                            Descuento = examenAgregado.DescuentoValor,
                            Subtotal = examenAgregado.ValorSubtotal,
                            Total = examenAgregado.ValorTotal,
                            ExamenLabClinicoId = examenAgregado.ExamenId,
                        };
                        var datos = _laboratorioRepository.DatosLabList(examenAgregado.ExamenId);
                        foreach (var dato in datos)
                        {
                            var newDato = new Resultados
                            {
                                DatosExamenesLabClinico = dato
                            };
                            nuevoDetalle.Resultados.Add(newDato);
                        }

                        examen.DetalleExamenes.Add(nuevoDetalle);

                        _laboratorioRepository.ActualizarInventarioInsumoVentaExamenesLaboratorio(examenAgregado.ExamenId);

                    }
                    _laboratorioRepository.Add(examen);
                }
                #endregion

                #region Registro final de datos de VENTA
                var descripcion = "";
                if (model.AmbienteId == (int)AmbienteEnum.Laboratorio)
                    descripcion = "Venta de examen: " + nombreVenta;
                if (model.AmbienteId == (int)AmbienteEnum.Global)
                {
                    cajaGlobal.DetalleCajas.Add(new DetalleCaja
                    {
                        Venta = venta,
                        Descripcion = descripcion,
                        Ingreso = venta.DetalleVenta.Sum(a => a.Total),
                        Fecha = fechaHora
                    });
                    _cajaRepository.Update(cajaGlobal);
                }
                else
                {
                    cajaLocal.DetalleCajas.Add(new DetalleCaja
                    {
                        Venta = venta,
                        Descripcion = descripcion,
                        Ingreso = venta.DetalleVenta.Sum(a => a.Total),
                        Fecha = fechaHora
                    });
                    _cajaRepository.Update(cajaLocal);
                }
                #endregion


                // if (model.ConsultaId != null)
                // {
                //     var consulta = _consultasRepository.GetConsulta((int)model.ConsultaId, true);

                //     decimal costoServicios = consulta.ConsultasServicios?
                //         .Where(s => !s.Pagado)
                //         .Sum(s => (s.PrecioValor ?? 0) * s.Cantidad) ?? 0;

                //     decimal costoExamenes = consulta.ConsultaExamenesAgregados?
                //         .Where(e => !e.Pagado)
                //         .Sum(e => e.PrecioValor * e.Cantidad) ?? 0;

                //     decimal costoPrescripciones =
                //         consulta.Prescripciones?.FirstOrDefault()?.DetallePrescripcion != null
                //             ? consulta.Prescripciones.First().DetallePrescripcion
                //                 .Where(p => !p.Pagado)
                //                 .Sum(p => p.PrecioValor * p.Cantidad)
                //             : 0;


                //     decimal totalCostos = costoServicios + costoExamenes + costoPrescripciones;
                //     decimal totalPagado = venta.DetalleVenta.Sum(a => a.Total);

                //     var CostoDebug = totalCostos - totalPagado;
                //     consulta.CostoConsulta = CostoDebug;

                //     // Console.WriteLine($"Total costos: {totalCostos}");
                //     // Console.WriteLine($"Total pagado: {totalPagado}");
                //     // Console.WriteLine($"Saldo pendiente: {CostoDebug}");

                //     consulta.EstadoPagoConsultaId = (int)EstadoPagoConsultaEnum.Pagado;

                //     if (consulta.ConsultasServicios != null)
                //         foreach (var servicio in consulta.ConsultasServicios)
                //             servicio.Pagado = true;

                //     if (consulta.ConsultaExamenesAgregados != null)
                //         foreach (var examenConsulta in consulta.ConsultaExamenesAgregados)
                //             examenConsulta.Pagado = true;

                //     if (consulta.Prescripciones?.FirstOrDefault()?.DetallePrescripcion != null)
                //         foreach (var detalle in consulta.Prescripciones.First().DetallePrescripcion)
                //             detalle.Pagado = true;

                //     _consultasRepository.Update(consulta);
                // }



                if (model.ConsultaId != null)
                {
                    var consulta = _consultasRepository.GetConsulta((int)model.ConsultaId, true);
                    if (consulta == null)
                        return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Consulta no encontrada" });

                    if (consulta.ConsultasServicios == null)
                        consulta.ConsultasServicios = new List<ConsultaServicio>();
                    if (consulta.ConsultaExamenesAgregados == null)
                        consulta.ConsultaExamenesAgregados = new List<ConsultaExamenLabClinico>();
                    if (consulta.Prescripciones == null)
                        consulta.Prescripciones = new List<Prescripcion>();

                    // ==================== Procesar SERVICIOS ====================
                    if (model.Servicios != null)
                    {
                        foreach (var servicioVenta in model.Servicios)
                        {
                            if (servicioVenta == null) continue;

                            var existente = consulta.ConsultasServicios
                                .FirstOrDefault(s => s.ServicioId == servicioVenta.ServicioId
                                                  && s.PrecioId == servicioVenta.PrecioId);
                            if (existente != null)
                            {
                                existente.Pagado = true;
                            }
                            else
                            {
                                consulta.ConsultasServicios.Add(new ConsultaServicio
                                {
                                    ConsultaId = consulta.Id,
                                    ServicioId = servicioVenta.ServicioId,
                                    PrecioId = servicioVenta.PrecioId,
                                    Cantidad = servicioVenta.Cantidad,
                                    PrecioValor = servicioVenta.ValorUnitario,
                                    Pagado = true
                                });
                            }
                        }
                    }

                    // ==================== Procesar EXAMENES ====================
                    if (model.Examenes != null)
                    {
                        foreach (var examenVenta in model.Examenes)
                        {
                            if (examenVenta == null) continue;

                            var existente = consulta.ConsultaExamenesAgregados
                                .FirstOrDefault(e => e.ExamenLabClinicoId == examenVenta.ExamenId
                                                  && e.PrecioId == examenVenta.PrecioId);
                            if (existente != null)
                            {
                                existente.Pagado = true;
                            }
                            else
                            {
                                consulta.ConsultaExamenesAgregados.Add(new ConsultaExamenLabClinico
                                {
                                    ConsultaId = consulta.Id,
                                    ExamenLabClinicoId = examenVenta.ExamenId,
                                    PrecioId = examenVenta.PrecioId,
                                    Cantidad = examenVenta.Cantidad,
                                    PrecioValor = examenVenta.ValorUnitario,
                                    Pagado = true,
                                    FechaRegistro = DateTime.Now
                                });
                            }
                        }
                    }

                    // ==================== Procesar PRODUCTOS (receta) ====================
                    if (model.Productos != null)
                    {
                        var productosNoPerdidos = model.Productos
                            .Where(p => p != null && !p.VentaPerdida)
                            .ToList();

                        if (productosNoPerdidos.Any())
                        {
                            var prescripcion = consulta.Prescripciones.FirstOrDefault();
                            if (prescripcion == null)
                            {
                                prescripcion = new Prescripcion
                                {
                                    CitasId = consulta.CitasId,
                                    ConsultaId = consulta.Id,
                                    NextDate = DateTime.Now.AddDays(30),
                                    DetallePrescripcion = new List<DetallePrescripcion>()
                                };
                                consulta.Prescripciones.Add(prescripcion);
                            }

                            if (prescripcion.DetallePrescripcion == null)
                                prescripcion.DetallePrescripcion = new List<DetallePrescripcion>();

                            foreach (var productoVenta in productosNoPerdidos)
                            {
                                var existente = prescripcion.DetallePrescripcion
                                    .FirstOrDefault(d => d.ProductoId == productoVenta.ProductoId
                                                      && d.PrecioId == productoVenta.PrecioId);
                                if (existente != null)
                                {
                                    existente.Pagado = true;
                                }
                                else
                                {
                                    // Validar y corregir el precio del producto
                                    if (productoVenta.PrecioId == null || productoVenta.PrecioId <= 0)
                                    {
                                        var precioDefault = _productoRepository.ObtenerPrecioProductoPorDefecto(productoVenta.ProductoId);
                                        if (precioDefault != null)
                                        {
                                            productoVenta.PrecioId = (int)precioDefault.PrecioId;
                                            if (productoVenta.PrecioValor <= 0)
                                                productoVenta.PrecioValor = precioDefault.Valor;
                                        }
                                        else
                                        {
                                            continue; // Sin precio válido, no se puede agregar
                                        }
                                    }

                                    prescripcion.DetallePrescripcion.Add(new DetallePrescripcion
                                    {
                                        ProductoId = productoVenta.ProductoId,
                                        PrecioId = productoVenta.PrecioId,
                                        Cantidad = productoVenta.Cantidad,
                                        PrecioValor = productoVenta.PrecioValor,
                                        Medicine = productoVenta.ProductoNombre,
                                        Pagado = true,
                                        FechaPrescripcion = DateTime.Now
                                    });
                                }
                            }
                        }
                    }

                    // ==================== Recalcular saldo pendiente ====================
                    decimal pendingServicios = consulta.ConsultasServicios?
                        .Where(s => !s.Pagado)
                        .Sum(s => (s.PrecioValor ?? 0) * s.Cantidad) ?? 0;

                    decimal pendingExamenes = consulta.ConsultaExamenesAgregados?
                        .Where(e => !e.Pagado)
                        .Sum(e => e.PrecioValor * e.Cantidad) ?? 0;

                    decimal pendingProductos = consulta.Prescripciones?
                        .FirstOrDefault()?.DetallePrescripcion
                        .Where(p => !p.Pagado)
                        .Sum(p => p.PrecioValor * p.Cantidad) ?? 0;

                    decimal totalPending = pendingServicios + pendingExamenes + pendingProductos;

                    consulta.CostoConsulta = totalPending;
                    consulta.EstadoPagoConsultaId = totalPending == 0
                        ? (int)EstadoPagoConsultaEnum.Pagado
                        : (int)EstadoPagoConsultaEnum.Pendiente;

                    _consultasRepository.Update(consulta);
                }

                if (model.EmergenciaId != null)
                {
                    var emergencia = _emergenciasRepository.UpdateSoloEstado(model.EmergenciaId.Value, true);
                }

                TempData["Message"] = "¡La venta se ha guardado con éxito!";

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = venta.Id,
                    ExamenId = model.Examenes != null && model.Examenes.Any() && examen.Id > 0 ? examen.Id : (int?)null,
                    AmbienteId = model.AmbienteId
                });

            }
            catch (Exception ex)
            {
                var detalle = ExceptionHelper.ObtenerMensajeRaiz(ex);
                Console.WriteLine(detalle + "   ///");
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al registrar venta: " + detalle
                });
            }
        }

        [HttpPost]
        public string ConsultarServiciosConsulta(int consultaId) //Consulta los servicios agregados 
        {
            try
            {
                var serviciosConsulta = new List<VentaUnificadaServicioAgregadoViewModel>();
                var serviciosBd = _consultasRepository
                    .GetServiciosAgregados(consultaId);
                if (serviciosBd != null)
                {
                    foreach (var servicio in serviciosBd)
                    {
                        //Se comprueba que no este pagado, ya que los pagados no se vuelven a cargar
                        if (!servicio.Pagado)
                        {
                            serviciosConsulta.Add(new VentaUnificadaServicioAgregadoViewModel
                            {
                                ServicioId = servicio.ServicioId,
                                ServicioCodigo = servicio.Servicio.CodigoInterno,
                                ServicioNombre = servicio.Servicio.NombreServicio,
                                PrecioId = servicio.PrecioId,
                                PrecioNombre = servicio.Precio.NombrePrecio,
                                ValorUnitario = servicio.PrecioValor ?? 0,
                                Cantidad = servicio.Cantidad,
                                ValorSubtotal = (servicio.PrecioValor ?? 0) * servicio.Cantidad,
                                ValorTotal = (servicio.PrecioValor ?? 0) * servicio.Cantidad,
                                ValorCubiertoSeguro = servicio.PrecioCubiertoSeguro,
                                ValorCopago = servicio.PrecioCopago,
                                DescuentoPorcentaje = 0,
                                DescuentoValor = 0
                            });
                        }
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = serviciosConsulta
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios: " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarServiciosEmergencias(int emergenciaId) //Consulta los servicios agregados 
        {
            try
            {
                var serviciosConsulta = new List<VentaUnificadaServicioAgregadoViewModel>();
                var serviciosBd = _emergenciasRepository
                    .GetServiciosAgregados(emergenciaId);
                if (serviciosBd != null)
                {
                    foreach (var servicio in serviciosBd)
                    {

                        if (servicio.ServicioId > 0)
                        {

                            serviciosConsulta.Add(new VentaUnificadaServicioAgregadoViewModel
                            {
                                ServicioId = servicio.ServicioId ?? 0,
                                ServicioCodigo = servicio.Servicio != null ? servicio.Servicio.CodigoInterno : string.Empty,
                                ServicioNombre = servicio.Servicio != null ? servicio.Servicio.NombreServicio : string.Empty,
                                PrecioId = servicio.PrecioId,
                                PrecioNombre = servicio.Precio != null ? servicio.Precio.NombrePrecio : string.Empty,
                                ValorUnitario = servicio.PrecioValor,
                                Cantidad = (int)servicio.Cantidad,
                                ValorSubtotal = servicio.PrecioValor * servicio.Cantidad,
                                ValorTotal = servicio.PrecioValor * servicio.Cantidad,
                                //ValorCubiertoSeguro = servicio.PrecioCubiertoSeguro,
                                //ValorCopago = servicio.PrecioCopago,
                                DescuentoPorcentaje = 0,
                                DescuentoValor = 0
                            });

                        }

                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = serviciosConsulta
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios: " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarProductosPrescripcionConsulta(int consultaId)
        {
            try
            {
                var productosConsulta = new List<VentaUnificadaProductoAgregadoViewModel>();
                var prescripcion = _consultasRepository.GetPrescripcionConsulta(consultaId, true);
                if (prescripcion != null && prescripcion.DetallePrescripcion != null)
                {
                    foreach (var elementoPrescripcion in prescripcion.DetallePrescripcion)
                    {
                        //Se verifica que NO este PAGADO, ya que son
                        //los que se cobran en la generacion de la venta
                        if (!elementoPrescripcion.Pagado)
                        {
                            var producto = elementoPrescripcion.Producto ?? new Producto();
                            var precio = elementoPrescripcion.Precio ?? new Precio();
                            productosConsulta.Add(new VentaUnificadaProductoAgregadoViewModel
                            {
                                ProductoId = elementoPrescripcion.ProductoId ?? 0,
                                ProductoNombre = elementoPrescripcion.ProductoId != null
                                ? producto.NombreProducto
                                : elementoPrescripcion.Medicine,
                                ProductoCodigo = producto.CodigoReferencia,
                                Precio = precio.NombrePrecio,
                                Cantidad = Convert.ToInt32(elementoPrescripcion.Cantidad),
                                DescuentoPorcentaje = elementoPrescripcion.DescuentoPorcentaje,
                                DescuentoValor = elementoPrescripcion.DescuentoValor,
                                VentaPerdida = elementoPrescripcion.ProductoId == null,
                                UnidadMedidaVentaId = elementoPrescripcion.UnidadMedidaVentaId ?? 0,
                                PrecioId = elementoPrescripcion.PrecioId ?? 0,
                                PrecioValor = elementoPrescripcion.PrecioValor,
                                Subtotal = elementoPrescripcion.Cantidad * elementoPrescripcion.PrecioValor
                            });
                        }
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = productosConsulta
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios: " + ex.Message
                });
            }
        }


        [HttpPost]
        public string ConsultarProductosPrescripcionEmergencia(int emergenciaId)
        {
            try
            {
                var productosConsulta = new List<VentaUnificadaProductoAgregadoViewModel>();

                var listaPrescripcion = _emergenciasRepository.GetPrescripcionEmergencia(emergenciaId, true);

                Console.WriteLine($"emergenciaId  {emergenciaId}");

                foreach (var item in listaPrescripcion)
                {
                    Console.WriteLine($"ProductoId  {item.ProductoId}");

                }

                if (listaPrescripcion != null && listaPrescripcion.Any())
                {
                    foreach (var elementoPrescripcion in listaPrescripcion)
                    {

                        var producto = elementoPrescripcion.Producto ?? new Producto();
                        var precio = elementoPrescripcion.Precio ?? new Precio();

                        productosConsulta.Add(new VentaUnificadaProductoAgregadoViewModel
                        {
                            ProductoId = elementoPrescripcion.ProductoId ?? 0,
                            ProductoNombre = producto.NombreProducto,
                            ProductoCodigo = producto.CodigoReferencia,
                            Precio = precio.NombrePrecio,
                            Cantidad = Convert.ToInt32(elementoPrescripcion.Cantidad),
                            DescuentoPorcentaje = elementoPrescripcion.DescuentoPorcentaje,
                            VentaPerdida = elementoPrescripcion.ProductoId == null,
                            UnidadMedidaVentaId = elementoPrescripcion.UnidadMedidaVentaId ?? 0,
                            PrecioId = elementoPrescripcion.PrecioId ?? 0,
                            PrecioValor = elementoPrescripcion.PrecioValor,
                            Subtotal = elementoPrescripcion.Cantidad * elementoPrescripcion.PrecioValor,
                            TipoProductoId = (int)(producto.TipoProductoId ?? 0)
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = productosConsulta
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar productos: " + ex.Message
                });
            }
        }

        [HttpPost]
        public string consultarExamenesAgregadosConsulta(int consultaId) //Consulta los Examenes agregados 
        {
            try
            {
                var citaId = _consultasRepository.GetConsulta(consultaId).CitasId;
                var ExamenesConsulta = new List<VentaUnificadaExamenAgregadoViewModel>();
                var examenesBd = _consultasRepository.GetExamenesAgregadosConsulta(consultaId);
                if (examenesBd != null)
                {
                    foreach (var examen in examenesBd)
                    {
                        //Se comprueba que NO este pagadao ya que son los que se cobran
                        if (!examen.Pagado)
                        {
                            ExamenesConsulta.Add(new VentaUnificadaExamenAgregadoViewModel
                            {
                                ExamenId = (int)examen.ExamenLabClinicoId,
                                ExamenCodigo = examen.ExamenLabClinico.CodigoInterno,
                                ExamenNombre = examen.ExamenLabClinico.NombreExamen,
                                PrecioId = examen.Precio.Id,
                                PrecioNombre = examen.Precio.NombrePrecio,
                                ValorUnitario = examen.PrecioValor,
                                Cantidad = 1,
                                ValorSubtotal = examen.PrecioValor * 1,
                                ValorTotal = examen.PrecioValor * 1,
                                ValorCubiertoSeguro = examen.PrecioValorCubiertoSeguro,
                                ValorCopago = examen.PrecioValorCopago,
                                DescuentoPorcentaje = examen.DescuentoPorcentaje,
                                DescuentoValor = examen.DescuentoValor
                            });
                        }
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = ExamenesConsulta
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios: " + ex.Message
                });
            }
        }



        [HttpPost]
        public string consultarExamenesAgregadosEmergencias(int emergenciaId) //Consulta los Examenes agregados 
        {
            try
            {
                var emergencia = _emergenciasRepository.Get(emergenciaId);
                var ExamenesConsulta = new List<VentaUnificadaExamenAgregadoViewModel>();
                var examenesBd = _emergenciasRepository.GetExamenesAgregados(emergenciaId);
                if (examenesBd != null)
                {
                    foreach (var examen in examenesBd)
                    {

                        if (examen.ExamenLabClinicoId > 0)
                        {
                            Console.WriteLine(examen.ExamenLabClinicoId);

                            ExamenesConsulta.Add(new VentaUnificadaExamenAgregadoViewModel
                            {
                                ExamenId = examen.ExamenLabClinicoId ?? 0,
                                ExamenCodigo = examen.ExamenLabClinico?.CodigoInterno ?? "",
                                ExamenNombre = examen.ExamenLabClinico?.NombreExamen ?? "Sin Nombre",
                                PrecioId = examen.Precio?.Id ?? 0,
                                PrecioNombre = examen.Precio?.NombrePrecio ?? "Sin Precio",

                                ValorUnitario = examen.PrecioValor,
                                Cantidad = (int)examen.Cantidad,
                                ValorSubtotal = examen.PrecioValor * 1,
                                ValorTotal = examen.PrecioValor * 1,

                                // Propiedades adicionales (Protección con coalescencia nula a 0)
                                //ValorCubiertoSeguro = examen.PrecioValorCubiertoSeguro ?? 0,
                                //ValorCopago = examen.PrecioValorCopago ?? 0,
                                //DescuentoPorcentaje = examen.DescuentoPorcentaje ?? 0,
                                //DescuentoValor = examen.DescuentoValor ?? 0
                            });

                        }

                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = ExamenesConsulta
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar examenes: " + ex.Message
                });
            }
        }
        private List<FormasPagoViewModel> ConsultarFormasPago()
        {
            var listaFormaPago = new List<FormasPagoViewModel>();

            var data = _formasPagoRepository.GetAll().Where(a => !a.Eliminada);

            if (data != null)
            {
                foreach (var formaPago in data)
                {
                    listaFormaPago.Add(new FormasPagoViewModel
                    {
                        Id = formaPago.Id,
                        NombreFormaPago = formaPago.NombreFormaPago
                    });
                }
            }
            return listaFormaPago;
        }
        [HttpPost]
        public JsonResult ObtenerDatosCliente(int clienteId)
        {
            // Buscar únicamente en pacientes
            var paciente = _pacienteRepository.Get(clienteId, false);

            if (paciente == null)
            {
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = "Paciente no encontrado"
                });
            }

            return Json(new
            {
                Exitoso = true,
                Datos = new
                {
                    Nit = paciente.Nit,
                    Direccion = paciente.Direccion,
                    Correo = paciente.Email,
                    FechaNacimiento = paciente.FechaNacimiento
                }
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult GuardarVentaFarmacia([FromBody] VentaClinicaAddViewModel model)
            => GuardarVentaLegacy(model, (int)AmbienteEnum.Farmacia, retornarExamenId: false);

        [Authorize]
        [HttpPost]
        public IActionResult GuardarVentaClinica([FromBody] VentaClinicaAddViewModel model)
            => GuardarVentaLegacy(model, (int)AmbienteEnum.Clinica, retornarExamenId: false);

        /// <summary>
        /// Endpoint usado por RealizarExamenClinico (botón Listo). Devuelve el Id del examen creado.
        /// </summary>
        [Authorize]
        [HttpPost]
        public IActionResult GuardarVentaLab([FromBody] VentaClinicaAddViewModel model)
            => GuardarVentaLegacy(model, (int)AmbienteEnum.Laboratorio, retornarExamenId: true);

        private IActionResult GuardarVentaLegacy(VentaClinicaAddViewModel model, int ambienteId, bool retornarExamenId)
        {
            try
            {
                if (model?.encabezado == null || model.detalle == null || !model.detalle.Any())
                    return new JsonErrorResult(LegacyVentaError("Datos de venta incompletos."));

                var enc = model.encabezado;
                if (string.IsNullOrWhiteSpace(enc.Nombres)) enc.Nombres = "CF";
                if (string.IsNullOrWhiteSpace(enc.Nit)) enc.Nit = "CF";
                if (string.IsNullOrWhiteSpace(enc.Direccion)) enc.Direccion = "CF";

                var empleado = _empleadoRepository.Get(enc.EmpleadoId);
                if (empleado == null)
                    return new JsonErrorResult(LegacyVentaError("Código de empleado incorrecto."));

                var cajaLocal = _cajaRepository.GetCajaAbierta(ambienteId);
                if (cajaLocal == null)
                    return new JsonErrorResult(LegacyVentaError("No hay cajas abiertas. Por favor abra una caja."));

                var fechaHora = DateTime.Now;
                Paciente paciente = null;
                if (ambienteId == (int)AmbienteEnum.Laboratorio && enc.ClienteId > 0)
                    paciente = _pacienteRepository.Get(enc.ClienteId, false);

                var venta = new Venta
                {
                    NoComprobante = enc.NoComprobante,
                    Nombres = enc.Nombres.Trim(),
                    Nit = enc.Nit,
                    Direccion = enc.Direccion,
                    Paciente = paciente,
                    FechaVenta = fechaHora,
                    EmpleadoId = enc.EmpleadoId,
                    AmbienteId = ambienteId,
                    MontoPago = enc.Monto,
                    Vuelto = enc.Vuelto,
                    TipoVenta = ambienteId switch
                    {
                        (int)AmbienteEnum.Farmacia => "farmacia",
                        (int)AmbienteEnum.Clinica => "clinica",
                        (int)AmbienteEnum.Laboratorio => "laboratorio",
                        _ => null
                    }
                };

                venta.Pagos.Add(new Pagos
                {
                    FechaHora = fechaHora,
                    FormaPagoId = enc.FormaPago,
                    Monto = enc.Monto
                });

                Examen examen = null;
                if (ambienteId == (int)AmbienteEnum.Laboratorio)
                {
                    var medico = _empleadoRepository.GetMedicoByName(enc.Medico);
                    if (medico == null && !string.IsNullOrWhiteSpace(enc.Medico))
                        medico = new Medicos { Nombres = enc.Medico };

                    var clinica = _empleadoRepository.GetClinicaByName(enc.Clinica);
                    if (clinica == null && !string.IsNullOrWhiteSpace(enc.Clinica))
                        clinica = new Clinica { NombreClinica = enc.Clinica };

                    examen = new Examen
                    {
                        Paciente = paciente,
                        EstadoExamenId = 1,
                        FechaRealizacion = fechaHora,
                        Medicos = medico,
                        Clinicas = clinica,
                        ClinicaReferida = enc.ClinicaReferida ?? enc.Clinica,
                        UsuarioSolicita = _userManager.GetUserId(HttpContext.User),
                        EmpleadoId = enc.EmpleadoId
                    };
                }

                var userId = _userManager.GetUserId(HttpContext.User);
                foreach (var item in model.detalle)
                {
                    if (ambienteId == (int)AmbienteEnum.Laboratorio)
                    {
                        var detalleVenta = new DetalleVenta
                        {
                            Venta = venta,
                            Cantidad = item.Cantidad,
                            Precio = item.Precio,
                            Descuento = item.Descuento,
                            Subtotal = item.Subtotal,
                            Total = item.Total,
                            ExamenLabClinicoId = item.ProductoId,
                            BienOServicio = "S",
                            UsuarioAutorizaModificacion = item.UsuarioAutorizaModificacion
                        };
                        venta.DetalleVenta.Add(detalleVenta);

                        var nuevoDetalleExamen = new DetalleExamen
                        {
                            Cantidad = item.Cantidad,
                            PrecioValor = item.Precio,
                            Descuento = item.Descuento,
                            Subtotal = item.Subtotal,
                            Total = item.Total,
                            ExamenLabClinicoId = item.ProductoId
                        };
                        foreach (var dato in _laboratorioRepository.DatosLabList(item.ProductoId))
                        {
                            nuevoDetalleExamen.Resultados.Add(new Resultados { DatosExamenesLabClinico = dato });
                        }
                        examen.DetalleExamenes.Add(nuevoDetalleExamen);
                        _laboratorioRepository.ActualizarInventarioInsumoVentaExamenesLaboratorio(item.ProductoId);
                    }
                    else if (ambienteId == (int)AmbienteEnum.Farmacia
                        || string.Equals(item.BienOServicio, "B", StringComparison.OrdinalIgnoreCase))
                    {
                        var registroInventario = _productoRepository.GetRegistroInventarioProducto(0, item.ProductoId);
                        if (registroInventario == null)
                            return new JsonErrorResult(LegacyVentaError($"Producto {item.ProductoId} sin inventario."));

                        var detalleVenta = new DetalleVenta
                        {
                            Venta = venta,
                            Cantidad = item.Cantidad,
                            Precio = item.Precio,
                            Descuento = item.Descuento,
                            Subtotal = item.Subtotal,
                            Total = item.Total,
                            ProductoId = item.ProductoId,
                            BienOServicio = "B",
                            UsuarioAutorizaModificacion = item.UsuarioAutorizaModificacion
                        };
                        venta.DetalleVenta.Add(detalleVenta);

                        registroInventario.Stock -= item.Cantidad;
                        if (registroInventario.Stock < 0) registroInventario.Stock = 0;
                        _productoRepository.UpdateRegistroInventario(registroInventario, false);
                        _productoRepository.Add(new MovimientoProducto
                        {
                            UsuarioRealizaId = userId,
                            ProductoInventarioId = registroInventario.Id,
                            Fecha = fechaHora,
                            PrecioUnitario = item.Precio,
                            MontoTotal = detalleVenta.Total,
                            Cantidad = detalleVenta.Cantidad,
                            DescripcionMovimiento = "Venta de producto",
                            SaldoActual = registroInventario.Stock,
                            TipoMovimientoProductoId = (int)TipoMovimientoProductoEnum.SalidaVenta
                        });
                    }
                    else
                    {
                        venta.DetalleVenta.Add(new DetalleVenta
                        {
                            Venta = venta,
                            Cantidad = item.Cantidad,
                            Precio = item.Precio,
                            Descuento = item.Descuento,
                            Subtotal = item.Subtotal,
                            Total = item.Total,
                            ServicioId = item.ProductoId,
                            BienOServicio = "S",
                            UsuarioAutorizaModificacion = item.UsuarioAutorizaModificacion
                        });
                        _servicioRepository.ActualizarInventarioVentaServicio(item.ProductoId);
                    }
                }

                if (examen != null)
                {
                    _laboratorioRepository.Add(examen);
                    venta.ExamenId = examen.Id;
                }

                var descripcion = ambienteId == (int)AmbienteEnum.Laboratorio
                    ? "Venta de examen: " + (paciente?.Nombre ?? enc.Nombres)
                    : "Venta " + venta.TipoVenta;

                cajaLocal.DetalleCajas.Add(new DetalleCaja
                {
                    Venta = venta,
                    Descripcion = descripcion,
                    Ingreso = venta.DetalleVenta.Sum(a => a.Total),
                    Fecha = fechaHora
                });
                _cajaRepository.Update(cajaLocal);

                if (retornarExamenId)
                {
                    if (examen == null || examen.Id <= 0)
                        return new JsonErrorResult(LegacyVentaError("No se pudo registrar el examen."));
                    return Json(examen.Id);
                }

                return Json(venta.Id);
            }
            catch (Exception ex)
            {
                return new JsonErrorResult(LegacyVentaError("Error al guardar la venta: " + ExceptionHelper.ObtenerMensajeRaiz(ex)));
            }
        }

        private static object LegacyVentaError(string mensaje) => new { message = mensaje, messsage = mensaje };

        [Authorize]
        public IActionResult ReporteVentasGeneral()
        {
            var empleados = _empleadoRepository.GetList()
                .Where(e => !e.Eliminado)
                .OrderBy(e => e.Nombre)
                .ThenBy(e => e.Apellido)
                .ToList();
            ViewBag.Empleados = Utilidades.EmpleadoSelectListHelper.Crear(empleados);
            return View();
        }

        private static bool TryResolveTipoVentaDesdeRequest(
            IQueryCollection query,
            out string tipoVenta,
            out bool isClinica,
            out bool isFarmacia,
            out bool isLaboratorio,
            out bool isEmergencia,
            out bool isHospital)
        {
            isClinica = false;
            isFarmacia = false;
            isLaboratorio = false;
            isEmergencia = false;
            isHospital = false;
            tipoVenta = ObtenerTipoVentaDesdeQuery(query);

            if (string.IsNullOrWhiteSpace(tipoVenta))
                return false;

            switch (tipoVenta)
            {
                case "clinica":
                    isClinica = true;
                    return true;
                case "farmacia":
                    isFarmacia = true;
                    return true;
                case "laboratorio":
                    isLaboratorio = true;
                    return true;
                case "emergencia":
                    isEmergencia = true;
                    return true;
                case "hospital":
                    isHospital = true;
                    return true;
                default:
                    return false;
            }
        }

        private static string ObtenerTipoVentaDesdeQuery(IQueryCollection query)
        {
            var explicito = query["tipoVenta"].FirstOrDefault()
                ?? query["TipoVenta"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(explicito))
                return explicito.Trim().ToLowerInvariant();

            if (EsQueryVerdadero(query, "isEmergencia"))
                return "emergencia";
            if (EsQueryVerdadero(query, "isHospital"))
                return "hospital";
            if (EsQueryVerdadero(query, "isLaboratorio") || EsQueryVerdadero(query, "islaboratorio"))
                return "laboratorio";
            if (EsQueryVerdadero(query, "isFarmacia"))
                return "farmacia";
            if (EsQueryVerdadero(query, "isClinica"))
                return "clinica";

            return null;
        }

        private static bool EsQueryVerdadero(IQueryCollection query, string key)
        {
            if (!query.TryGetValue(key, out var values) || values.Count == 0)
                return false;

            var val = values[0];
            if (string.IsNullOrEmpty(val))
                return true;

            return val.Equals("true", StringComparison.OrdinalIgnoreCase)
                || val == "1"
                || val == "True";
        }

    }
    #endregion
}


public class DetalleVentaBinding
{
    public string ProductoId { get; set; }
    public string Cantidad { get; set; }
    public string Precio { get; set; }
    public string Descuento { get; set; }

    public string Subtotal { get; set; }

    public string Total { get; set; }
}
public class DatosEnvioBinding
{
    //public string NombrePiloto { get; set; }

    public string Ruta { get; set; }

    public string Fecha { get; set; }

    public string DireccionExacta { get; set; }

    public string NoComprobante { get; set; }

    public string Nit { get; set; }

    public string Nombre { get; set; }

    public string EmpleadoId { get; set; }

    public string UserId { get; set; }

    public int Id { get; set; }

}


public class DatosBinding
{
    public string NoComprobante { get; set; }

    public int EmpleadoId { get; set; }
    public string Nombre { get; set; }

    public string ClienteId { get; set; }

    public string Nit { get; set; }

    public string Direccion { get; set; }

    public string FormaPago { get; set; }

    public int Id { get; set; }

}

public class ViewModelVenta2
{
    public List<DetalleVentaBinding> detalle { get; set; }
    public List<IdsBinding> nuevos { get; set; }
    public DatosBinding encabezado { get; set; }
    public DatosEnvioBinding datosenvio { get; set; }

}




