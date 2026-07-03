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
using Database.Shared.Enumeraciones;

namespace sistema.Controllers
{
    [Authorize]
    public class CotizacionController : Controller
    {

        private readonly ICotizacion _cotizacionRepository = null;
        private readonly IProducto _productoRepository = null;
        private readonly IServicio _servicioRepository = null;
        private readonly IVenta _ventaRepository = null;
        //private readonly IVentaServicio _ventaServicioRepository = null;
        private readonly ICliente _clienteRepository = null;
        private readonly IEmpleado _empleadoRepository = null;
        private readonly ICaja _cajaRepository = null;
        //private readonly ICajaClinica _cajaClinicaRepository = null;



        public CotizacionController(ICotizacion cotizacionRepository, IProducto productoRepository, IVenta ventaRepository
        , ICliente clienteRepository, IEmpleado empleadoRepository, ICaja cajaRepository,
         //IVentaServicio ventaServicioRepository,
         IServicio servicioRepository
            //ICajaClinica cajaClinicaRepository
            )
        {
            _cotizacionRepository = cotizacionRepository;
            _productoRepository = productoRepository;
            _ventaRepository = ventaRepository;
            _clienteRepository = clienteRepository;
            _empleadoRepository = empleadoRepository;
            _cajaRepository = cajaRepository;
            //_ventaServicioRepository = ventaServicioRepository;
            _servicioRepository = servicioRepository;
            //_cajaClinicaRepository = cajaClinicaRepository;
        }

        public IActionResult Nuevo()
        {
            var model = new CotizacionBaseViewModel() { };

            model.Init(_productoRepository);

            return View(model);
        }

        public JsonResult GuardarCotizacion([FromBody] ViewModelCotizacion det)
        {
            if (det.encabezado.Empleado == "")
            {

                TempData["Message"] = "¡Codigo de empleado incorrecto.!";
                return Json("");
            }

            var empleado = _empleadoRepository.Get(Convert.ToInt16(det.encabezado.Empleado));

            if (empleado == null)
            {

                TempData["Message"] = "¡Codigo de empleado incorrecto.!";
                return Json("");
            }

            if (ModelState.IsValid)
            {
                if (det.encabezado.NoComprobante == "")
                {
                    var nuevaCotizacion = new Cotizacion()
                    {
                        Nit = det.encabezado.Nit,
                        Direccion = det.encabezado.Direccion,
                        FechaCotizacion = DateTime.Now,
                        FechaValida = DateTime.Now,
                        Cliente = det.encabezado.Cliente,
                        Empleado = det.encabezado.Empleado
                    };

                    foreach (var item in det.detalle)
                    {

                        var produ = _productoRepository.Get(Convert.ToInt32(item.Producto));

                        if (produ == null)
                        {
                            var detalle = new DetalleCotizacion()
                            {
                                Cotizacion = nuevaCotizacion,
                                Cantidad = Convert.ToInt32(item.Cantidad),
                                Precio = Convert.ToDecimal(item.Precio),
                                Descuento = Convert.ToDecimal(item.Descuento),
                                Subtotal = Convert.ToDecimal(item.Subtotal),
                                Total = Convert.ToDecimal(item.Total),
                                ServicioId = Convert.ToInt32(item.Producto),

                            };
                            _cotizacionRepository.Add(detalle, false);

                        }
                        else
                        {
                            var detalle = new DetalleCotizacion()
                            {
                                Cotizacion = nuevaCotizacion,
                                Cantidad = Convert.ToInt32(item.Cantidad),
                                Precio = Convert.ToDecimal(item.Precio),
                                Descuento = Convert.ToDecimal(item.Descuento),
                                Subtotal = Convert.ToDecimal(item.Subtotal),
                                Total = Convert.ToDecimal(item.Total),
                                ProductoId = Convert.ToInt32(item.Producto),
                            };

                            _cotizacionRepository.Add(detalle, false);

                        }

                    }

                    _cotizacionRepository.saveChanges();

                    TempData["Message"] = "¡La Cotizacion se ha guardado con exito.!";

                    return Json(nuevaCotizacion.Id);

                }
                else
                {




                    var nuevaCotizacion = new Cotizacion()
                    {
                        Nit = det.encabezado.Nit,
                        Direccion = det.encabezado.Direccion,
                        FechaCotizacion = DateTime.Now,
                        FechaValida = DateTime.Now.AddDays(Convert.ToInt16(det.encabezado.NoComprobante)),
                        Cliente = det.encabezado.Cliente

                    };

                    foreach (var item in det.detalle)
                    {


                        var produ = _productoRepository.Get(Convert.ToInt32(item.Producto));

                        if (produ == null)
                        {
                            var detalle = new DetalleCotizacion()
                            {
                                Cotizacion = nuevaCotizacion,
                                Cantidad = Convert.ToInt32(item.Cantidad),
                                Precio = Convert.ToDecimal(item.Precio),
                                Descuento = Convert.ToDecimal(item.Descuento),
                                Subtotal = Convert.ToDecimal(item.Subtotal),
                                Total = Convert.ToDecimal(item.Total),
                                ServicioId = Convert.ToInt32(item.Producto),

                            };

                            _cotizacionRepository.Add(detalle, false);
                        }
                        else
                        {
                            var detalle = new DetalleCotizacion()
                            {
                                Cotizacion = nuevaCotizacion,
                                Cantidad = Convert.ToInt32(item.Cantidad),
                                Precio = Convert.ToDecimal(item.Precio),
                                Descuento = Convert.ToDecimal(item.Descuento),
                                Subtotal = Convert.ToDecimal(item.Subtotal),
                                Total = Convert.ToDecimal(item.Total),
                                ProductoId = Convert.ToInt32(item.Producto),

                            };

                            _cotizacionRepository.Add(detalle, false);
                        }





                    }

                    _cotizacionRepository.saveChanges();

                    TempData["Message"] = "¡La Cotizacion se ha guardado con exito.!";

                    return Json(nuevaCotizacion.Id);

                }

            }

            return Json("Hubo un error interno.");
        }

        public IActionResult Confirmadas(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ApellidoSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Apellido_desc" : "";
            ViewData["NombreSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Nombre_desc" : "";

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

            var lista = _cotizacionRepository.PaginacionCotizacionConfirmadas(sortOrder, buscar, pageNumber, 10);
            return View(lista);
        }

        public IActionResult NoConfirmadas(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ApellidoSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Apellido_desc" : "";
            ViewData["NombreSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Nombre_desc" : "";

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

            var lista = _cotizacionRepository.PaginacionCotizacionNoConfirmadas(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult Modificar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var cotizacion = _cotizacionRepository.Get((int)id);


            if (cotizacion == null)
            {
                return StatusCode(404);
            }

            var model = new CotizacionBaseViewModel()
            {
                Cotizacion = cotizacion,
            };
            model.Init(_productoRepository);

            return View(model);
        }




        // public IActionResult ConfirmarCotizacion(int id)
        // {

        //     var cotizacion = _cotizacionRepository.Get(id);

        //     foreach (var item in cotizacion.DetalleCotizacion)
        //     {
        //         var producto = _productoRepository.GetPorNumeroDeReferenciayNombre(item.Producto.Trim());
        //     }

        //     if (cotizacion.Confirmado == true)
        //     {
        //         TempData["Message"] = "¡Esta Cotizacion ya ha sido confirmada.!";
        //         var model2 = new CotizacionBaseViewModel()
        //         {
        //             Cotizacion = cotizacion,
        //         };

        //         return View(model2);
        //     }

        //     var cliente = _clienteRepository.GetClientePorNombre(cotizacion.Cliente);


        //     if (ModelState.IsValid)
        //     {


        //         var cajita = _cajaRepository.ListarCajas();


        //         if (!cajita.Any(a => a.EstadoCaja == true))
        //         {
        //             TempData["Message"] = "¡Error. No hay cajas abiertas. Por favor debe abrir una caja.!";
        //             return Json("");
        //         }

        //         var cajaAbierta = _cajaRepository.GetCajaAbierta();

        //         if (cliente == null)
        //         {
        //             var nuevoCliente = new Cliente()
        //             {
        //                 Nombre = cotizacion.Cliente,
        //                 Direccion = cotizacion.Direccion,
        //                 Nit = cotizacion.Nit,

        //             };



        //             var venta = new Venta()
        //             {

        //                 //NoComprobante = det.encabezado.NoComprobante,
        //                 Cliente = nuevoCliente,
        //                 Nit = cotizacion.Nit,
        //                 Nombres = nuevoCliente.Nombre,
        //                 Direccion = cotizacion.Direccion,
        //                 EmpleadoId = Convert.ToInt32(cotizacion.Empleado),
        //                 FechaVenta = DateTime.Now,
        //             };

        //             var nuevoDetalleCaja = new DetalleCaja()
        //             {
        //                 Venta = venta,
        //                 Descripcion = "Venta a cliente: " + nuevoCliente.Nombre,
        //                 Ingreso = cotizacion.DetalleCotizacion.Sum(a => Convert.ToDecimal(a.Total)),
        //                 Caja = cajaAbierta
        //             };

        //             _cajaRepository.Add(nuevoDetalleCaja, false);

        //             foreach (var item in cotizacion.DetalleCotizacion)
        //             {
        //                 var producto = _productoRepository.GetPorNumeroDeReferenciayNombre(item.Producto.Trim());

        //                 var nuevodetalle = new DetalleVenta()
        //                 {
        //                     Venta = venta,
        //                     Cantidad = Convert.ToInt32(item.Cantidad),
        //                     Precio = Convert.ToDecimal(item.Precio),
        //                     Descuento = Convert.ToDecimal(item.Descuento),
        //                     Subtotal = Convert.ToDecimal(item.Subtotal),
        //                     Total = Convert.ToDecimal(item.Total),
        //                     ProductoId = producto.Id,

        //                 };

        //                 _ventaRepository.Add(nuevodetalle, false);
        //                 producto.Stock -= nuevodetalle.Cantidad;
        //                 _productoRepository.Update(producto, false);

        //             }

        //             _ventaRepository.saveChanges();
        //             cotizacion.Confirmado=true;
        //             TempData["Message"] = "¡La Cotizacion se ha Confirmado con exito.!";
        //             return RedirectToAction("Lista");

        //         }
        //         else
        //         {
        //             var venta = new Venta()
        //             {
        //                 // NoComprobante = det.encabezado.NoComprobante,
        //                 Cliente = cliente,
        //                 Nit = cotizacion.Nit,
        //                 Nombres = cotizacion.Cliente,
        //                 Direccion = cotizacion.Direccion,
        //                 FechaVenta = DateTime.Now,
        //                 EmpleadoId = Convert.ToInt32(cotizacion.Empleado),

        //             };

        //             var nuevoDetalleCaja = new DetalleCaja()
        //             {
        //                 Venta = venta,
        //                 Descripcion = "Venta a cliente: " + cliente.Nombre,
        //                 Ingreso = cotizacion.DetalleCotizacion.Sum(a => Convert.ToDecimal(a.Total)),
        //                 Caja = cajaAbierta
        //             };

        //             _cajaRepository.Add(nuevoDetalleCaja, false);

        //             foreach (var item in cotizacion.DetalleCotizacion)
        //             {
        //                 var producto = _productoRepository.GetPorNumeroDeReferenciayNombre(item.Producto.Trim());

        //                 if(producto == null){


        //                 }

        //                 var nuevodetalle = new DetalleVenta()
        //                 {
        //                     Venta = venta,
        //                     Cantidad = Convert.ToInt32(item.Cantidad),
        //                     Precio = Convert.ToDecimal(item.Precio),
        //                     Descuento = Convert.ToDecimal(item.Descuento),
        //                     Subtotal = Convert.ToDecimal(item.Subtotal),
        //                     Total = Convert.ToDecimal(item.Total),
        //                     ProductoId = producto.Id,

        //                 };

        //                 _ventaRepository.Add(nuevodetalle, false);
        //                 producto.Stock -= nuevodetalle.Cantidad;
        //                 _productoRepository.Update(producto, false);


        //             }

        //             _ventaRepository.saveChanges();
        //             cotizacion.Confirmado=true;
        //             TempData["Message"] = "¡La Cotizacion se ha Confirmado con exito.!";
        //             return RedirectToAction("Lista");

        //         }



        //     }

        //     TempData["Message"] = "¡La Cotizacion no se modifico.!";
        //     //return new JsonErrorResult(new { message = "Ha ocurrido un error de servidor." });  
        //     var model = new CotizacionBaseViewModel()
        //     {
        //         Cotizacion = cotizacion,
        //     };

        //     return View(model);
        // }

        //public IActionResult ConfirmarCotizacion(int? id)
        //{
        //    var cotizacion = _cotizacionRepository.Get((int)id);

        //    if (cotizacion.Confirmado == true)
        //    {
        //        TempData["Message"] = "¡Esta Cotizacion ya ha sido confirmada.!";

        //        var model2 = new CotizacionBaseViewModel()
        //        {
        //            Cotizacion = cotizacion,
        //        };

        //        return View(model2);
        //    }


        //    var cliente = _clienteRepository.GetClientePorNombre(cotizacion.Cliente);

        //    if (ModelState.IsValid)
        //    {
        //        var cajaClinica = _cajaRepository.ListarCajas().Where(a => a.AmbienteId == (int)AmbienteEnum.Clinica).ToList();

        //        if (!cajaClinica.Any(a => a.EstadoCaja == true))
        //        {
        //            TempData["Message"] = "¡Error. No hay cajas abiertas. Por favor debe abrir una caja.!";
        //            return Json("");
        //        }

        //        var cajaAbierta = _cajaRepository.GetCajaAbierta();


        //        if (cliente == null)
        //        {
        //            var nuevoCliente = new Clientes()
        //            {
        //                Nombre = cotizacion.Cliente,
        //                Direccion = cotizacion.Direccion,
        //                Nit = cotizacion.Nit,
        //            };

        //            var venta = new Venta()
        //            {
        //                //NoComprobante = det.encabezado.NoComprobante,
        //                Clientes = nuevoCliente,
        //                Nit = cotizacion.Nit,
        //                Nombres = nuevoCliente.Nombre,
        //                Direccion = cotizacion.Direccion,
        //                EmpleadoId = Convert.ToInt32(cotizacion.Empleado),
        //                FechaVenta = DateTime.Now,
        //                TipoVenta = "clinica"
        //            };

        //            var nuevoDetalleCaja = new DetalleCajaClinica()
        //            {
        //                Venta = venta,
        //                Descripcion = "Venta a cliente: " + nuevoCliente.Nombre,
        //                Ingreso = cotizacion.DetalleCotizacion.Sum(a => Convert.ToDecimal(a.Total)),
        //                CajaClinica = cajaAbierta
        //            };

        //            _cajaClinicaRepository.Add(nuevoDetalleCaja, false);

        //            foreach (var item in cotizacion.DetalleCotizacion)
        //            {
        //                if (item.Producto != null)
        //                {
        //                    var producto = _productoRepository.GetProdutoById(item.Producto.Id);

        //                    var nuevodetalle = new DetalleVenta()
        //                    {
        //                        Venta = venta,
        //                        Cantidad = Convert.ToInt32(item.Cantidad),
        //                        Precio = Convert.ToDecimal(item.Precio),
        //                        Descuento = Convert.ToDecimal(item.Descuento),
        //                        Subtotal = Convert.ToDecimal(item.Subtotal),
        //                        Total = Convert.ToDecimal(item.Total),
        //                        ProductoId = producto.Id,

        //                    };

        //                    _ventaRepository.Add(nuevodetalle, false);
        //                    producto.Stock -= nuevodetalle.Cantidad;
        //                    _productoRepository.Update(producto, false);

        //                }
        //                else if (item.Servicio != null)
        //                {
        //                    var servicio = _servicioRepository.Get(item.Servicio.Id);

        //                    var nuevodetalle = new DetalleVenta()
        //                    {
        //                        Venta = venta,
        //                        Cantidad = Convert.ToInt32(item.Cantidad),
        //                        Precio = Convert.ToDecimal(item.Precio),
        //                        Descuento = Convert.ToDecimal(item.Descuento),
        //                        Subtotal = Convert.ToDecimal(item.Subtotal),
        //                        Total = Convert.ToDecimal(item.Total),
        //                        ServicioId = servicio.Id,

        //                    };
        //                    _ventaRepository.Add(nuevodetalle, false);
        //                }

        //            }

        //            _ventaRepository.saveChanges();
        //            cotizacion.Confirmado = true;
        //            _cotizacionRepository.Update(cotizacion);
        //            TempData["Message"] = "¡La Cotizacion se ha Confirmado con exito.!";
        //            return RedirectToAction("NoConfirmadas");

        //        }
        //        else
        //        {
        //            var venta = new Venta()
        //            {
        //                // NoComprobante = det.encabezado.NoComprobante,
        //                Clientes = cliente,
        //                Nit = cotizacion.Nit,
        //                Nombres = cotizacion.Cliente,
        //                Direccion = cotizacion.Direccion,
        //                FechaVenta = DateTime.Now,
        //                EmpleadoId = Convert.ToInt32(cotizacion.Empleado),
        //                TipoVenta = "clinica"
        //            };

        //            var nuevoDetalleCaja = new DetalleCajaClinica()
        //            {
        //                Venta = venta,
        //                Descripcion = "Venta a cliente: " + cliente.Nombre,
        //                Ingreso = cotizacion.DetalleCotizacion.Sum(a => Convert.ToDecimal(a.Total)),
        //                CajaClinica = cajaAbierta
        //            };

        //            _cajaClinicaRepository.Add(nuevoDetalleCaja, false);

        //            foreach (var item in cotizacion.DetalleCotizacion)
        //            {
        //                if (item.Producto != null)
        //                {
        //                    var producto = _productoRepository.GetProdutoById(item.Producto.Id);

        //                    var nuevodetalle = new DetalleVenta()
        //                    {
        //                        Venta = venta,
        //                        Cantidad = Convert.ToInt32(item.Cantidad),
        //                        Precio = Convert.ToDecimal(item.Precio),
        //                        Descuento = Convert.ToDecimal(item.Descuento),
        //                        Subtotal = Convert.ToDecimal(item.Subtotal),
        //                        Total = Convert.ToDecimal(item.Total),
        //                        ProductoId = producto.Id,

        //                    };

        //                    _ventaRepository.Add(nuevodetalle, false);
        //                    producto.Stock -= nuevodetalle.Cantidad;
        //                    _productoRepository.Update(producto, false);

        //                }
        //                else if (item.Servicio != null)
        //                {
        //                    var servicio = _servicioRepository.Get(item.Servicio.Id);

        //                    var nuevodetalle = new DetalleVenta()
        //                    {
        //                        Venta = venta,
        //                        Cantidad = Convert.ToInt32(item.Cantidad),
        //                        Precio = Convert.ToDecimal(item.Precio),
        //                        Descuento = Convert.ToDecimal(item.Descuento),
        //                        Subtotal = Convert.ToDecimal(item.Subtotal),
        //                        Total = Convert.ToDecimal(item.Total),
        //                        ServicioId = servicio.Id,

        //                    };

        //                    _ventaRepository.Add(nuevodetalle, false);
        //                }
        //            }

        //            _ventaRepository.saveChanges();
        //            cotizacion.Confirmado = true;
        //            _cotizacionRepository.Update(cotizacion);
        //            TempData["Message"] = "¡La Cotizacion se ha Confirmado con exito.!";
        //            return RedirectToAction("NoConfirmadas");

        //        }
        //    }

        //    TempData["Message"] = "¡La Cotizacion no se modifico.!";
        //    var model = new CotizacionBaseViewModel()
        //    {
        //        Cotizacion = cotizacion,
        //    };

        //    return View(model);

        //}


        // public IActionResult ConfirmarCotizacion(int id)
        // {
        //     bool prod = false;
        //     bool serv = false;

        //     var cotizacion = _cotizacionRepository.Get(id);

        //     if (ModelState.IsValid)
        //     {

        //         if (cotizacion.Confirmado == true)
        //         {
        //             TempData["Message"] = "¡Esta Cotización ya ha sido confirmada.!";
        //             var model2 = new CotizacionBaseViewModel()
        //             {
        //                 Cotizacion = cotizacion,
        //             };

        //             model2.Init(_productoRepository);

        //             return View("Modificar", model2);
        //         }

        //         foreach (var item in cotizacion.DetalleCotizacion)
        //         {

        //             if (item.ProductoId != null)
        //             {
        //                 prod = true;
        //             }


        //             if (item.ServicioId != null)
        //             {
        //                 serv = true;
        //             }
        //         }

        //         var cliente = _clienteRepository.GetClientePorNombre(cotizacion.Cliente);
        //         // var cajita = _cajaRepository.ListarCajas();
        //         var cajita = _cajaClinicaRepository.ListarCajas();

        //         if (!cajita.Any(a => a.EstadoCaja == true))
        //         {
        //             TempData["Message"] = "¡Error. No hay cajas abiertas. Por favor debe abrir una caja.!";
        //              return RedirectToAction("Modificar","Cotizacion" ,new {id = (int)id});
        //         }

        //         var cajaAbierta = _cajaClinicaRepository.GetCajaAbierta();

        //         if (cliente == null)
        //         {

        //             if (prod == true && serv == false)
        //             {
        //                 var venta = new Venta()
        //                 {

        //                     //NoComprobante = det.encabezado.NoComprobante,
        //                     Nit = cotizacion.Nit,
        //                     Nombres = cotizacion.Nombres,
        //                     Direccion = cotizacion.Direccion,
        //                     FechaVenta = DateTime.Now,
        //                     EmpleadoId = 1

        //                 };

        //                 var nuevoDetalleCaja = new DetalleCajaClinica()
        //                 {
        //                     Venta = venta,
        //                     Descripcion = "Venta a cliente: " + cotizacion.Nombres,
        //                     Ingreso = cotizacion.DetalleCotizacion.Sum(a => Convert.ToDecimal(a.Total)),
        //                     CajaClinica = cajaAbierta
        //                 };

        //                 _cajaClinicaRepository.Add(nuevoDetalleCaja, false);

        //                 foreach (var item in cotizacion.DetalleCotizacion)
        //                 {
        //                     //var producto = _productoRepository.Get((int)item.ProductoId);

        //                     var nuevodetalle = new DetalleVenta()
        //                     {
        //                         Venta = venta,
        //                         Cantidad = Convert.ToInt32(item.Cantidad),
        //                         Precio = Convert.ToDecimal(item.Precio),
        //                         Descuento = Convert.ToDecimal(item.Descuento),
        //                         Subtotal = Convert.ToDecimal(item.Subtotal),
        //                         Total = Convert.ToDecimal(item.Total),
        //                         Producto = item.Producto,
        //                     };

        //                     _ventaRepository.Add(nuevodetalle, false);
        //                     item.Producto.Stock -= nuevodetalle.Cantidad;
        //                     _productoRepository.Update(item.Producto, false);

        //                 }

        //                 cotizacion.Confirmado = true;
        //                 _cotizacionRepository.Update(cotizacion, false);
        //                 _ventaRepository.saveChanges();

        //                 TempData["Message"] = "¡La Cotizacion se ha Confirmado con exito.!";
        //                 return RedirectToAction("ReciboVentaPdf", "CrearPDF", new { id = venta.Id });

        //             }
        //             else if (prod == false && serv == true)
        //             {

        //                 var nuevaVentaServicio = new VentaServicio()
        //                 {

        //                     Nit = cotizacion.Nit,
        //                     Nombres = cotizacion.Nombres,
        //                     Direccion = cotizacion.Direccion,
        //                     FechaVenta = DateTime.Now,

        //                 };

        //                 var nuevoDetalleCaja = new DetalleCajaClinica()
        //                 {
        //                     VentaServicio = nuevaVentaServicio,
        //                     Descripcion = "Venta de servicio a cliente: " + cotizacion.Nombres,
        //                     Ingreso = cotizacion.DetalleCotizacion.Sum(a => Convert.ToDecimal(a.Total)),
        //                     CajaClinica = cajaAbierta
        //                 };

        //                 _cajaClinicaRepository.Add(nuevoDetalleCaja, false);

        //                 foreach (var item in cotizacion.DetalleCotizacion)
        //                 {
        //                     // var servicio = _servicioRepository.Get((int)item.ServicioId);

        //                     var detalle = new DetalleServicio()
        //                     {
        //                         VentaServicio = nuevaVentaServicio,
        //                         Cantidad = Convert.ToInt32(item.Cantidad),
        //                         Precio = Convert.ToDecimal(item.Precio),
        //                         Total = Convert.ToDecimal(item.Total),
        //                         Servicio = item.Servicio,
        //                     };

        //                     _ventaServicioRepository.Add(detalle, false);

        //                 }
        //                 cotizacion.Confirmado = true;
        //                 _cotizacionRepository.Update(cotizacion, false);
        //                 _ventaServicioRepository.saveChanges();


        //                 TempData["Message"] = "¡La cotización se ha Confirmado con éxito.!";
        //                 return RedirectToAction("ReciboVentaPdf", "CrearPDF", new { id = nuevaVentaServicio.Id });
        //             }

        //             else if (prod == true && serv == true)
        //             {
        //                 var venta = new Venta()
        //                 {

        //                     //NoComprobante = det.encabezado.NoComprobante,
        //                     Nit = cotizacion.Nit,
        //                     Nombres = cotizacion.Nombres,
        //                     Direccion = cotizacion.Direccion,
        //                     FechaVenta = DateTime.Now,
        //                     EmpleadoId = 1

        //                 };

        //                 var nuevaVentaServicio = new VentaServicio()
        //                 {

        //                     Nit = cotizacion.Nit,
        //                     Nombres = cotizacion.Nombres,
        //                     Direccion = cotizacion.Direccion,
        //                     FechaVenta = DateTime.Now,


        //                 };



        //                 foreach (var item in cotizacion.DetalleCotizacion)
        //                 {
        //                     // var producto = _productoRepository.Get((int)item.ProductoId);
        //                     if (item.Producto != null)
        //                     {
        //                         var nuevodetalle = new DetalleVenta()
        //                         {
        //                             Venta = venta,
        //                             Cantidad = Convert.ToInt32(item.Cantidad),
        //                             Precio = Convert.ToDecimal(item.Precio),
        //                             Descuento = Convert.ToDecimal(item.Descuento),
        //                             Subtotal = Convert.ToDecimal(item.Subtotal),
        //                             Total = Convert.ToDecimal(item.Total),
        //                             Producto = item.Producto,

        //                         };

        //                         _ventaRepository.Add(nuevodetalle, false);
        //                         item.Producto.Stock -= nuevodetalle.Cantidad;
        //                         _productoRepository.Update(item.Producto, false);
        //                     }

        //                     //var servicio = _servicioRepository.Get((int)item.ServicioId);
        //                     if (item.Servicio != null)
        //                     {
        //                         var detalle = new DetalleServicio()
        //                         {
        //                             VentaServicio = nuevaVentaServicio,
        //                             Cantidad = Convert.ToInt32(item.Cantidad),
        //                             Precio = Convert.ToDecimal(item.Precio),
        //                             Total = Convert.ToDecimal(item.Total),
        //                             Servicio = item.Servicio,

        //                         };

        //                         _ventaServicioRepository.Add(detalle, false);
        //                     }

        //                 }

        //                 var nuevoDetalleCaja = new DetalleCajaClinica()
        //                 {
        //                     Venta = venta,

        //                     Descripcion = "Venta a cliente: " + cotizacion.Nombres,
        //                     Ingreso = venta.DetalleVenta.Sum(a => Convert.ToDecimal(a.Total)),
        //                     CajaClinica = cajaAbierta
        //                 };

        //                 var nuevoDetalleCaja2 = new DetalleCajaClinica()
        //                 {

        //                     VentaServicio = nuevaVentaServicio,
        //                     Descripcion = "Venta a cliente: " + cotizacion.Nombres,
        //                     Ingreso = nuevaVentaServicio.DetalleServicio.Sum(a => Convert.ToDecimal(a.Total)),
        //                     CajaClinica = cajaAbierta
        //                 };

        //                 _cajaClinicaRepository.Add(nuevoDetalleCaja, false);
        //                 _cajaClinicaRepository.Add(nuevoDetalleCaja2, false);

        //                 cotizacion.Confirmado = true;
        //                 _cotizacionRepository.Update(cotizacion, false);
        //                 _ventaRepository.saveChanges();
        //                 TempData["Message"] = "¡La Cotizacion se ha Confirmado con exito.!";
        //                 return RedirectToAction("ReciboVentaPdf", "CrearPDF", new { id = venta.Id });
        //             }

        //         }
        //         else
        //         {
        //             if (prod == true && serv == false)
        //             {
        //                 var venta = new Venta()
        //                 {
        //                     //NoComprobante = det.encabezado.NoComprobante,
        //                     Nit = cotizacion.Nit,
        //                     Nombres = cliente.Nombre,
        //                     Direccion = cotizacion.Direccion,
        //                     FechaVenta = DateTime.Now,
        //                 };

        //                 var nuevoDetalleCaja = new DetalleCajaClinica()
        //                 {
        //                     Venta = venta,
        //                     Descripcion = "Venta a cliente: " + cliente.Nombre,
        //                     Ingreso = cotizacion.DetalleCotizacion.Sum(a => Convert.ToDecimal(a.Total)),
        //                     CajaClinica = cajaAbierta
        //                 };

        //                 _cajaClinicaRepository.Add(nuevoDetalleCaja, false);

        //                 foreach (var item in cotizacion.DetalleCotizacion)
        //                 {
        //                     //var producto = _productoRepository.Get((int)item.ProductoId);

        //                     var nuevodetalle = new DetalleVenta()
        //                     {
        //                         Venta = venta,
        //                         Cantidad = Convert.ToInt32(item.Cantidad),
        //                         Precio = Convert.ToDecimal(item.Precio),
        //                         Descuento = Convert.ToDecimal(item.Descuento),
        //                         Subtotal = Convert.ToDecimal(item.Subtotal),
        //                         Total = Convert.ToDecimal(item.Total),
        //                         Producto = item.Producto

        //                     };

        //                     _ventaRepository.Add(nuevodetalle, false);
        //                     item.Producto.Stock -= nuevodetalle.Cantidad;
        //                     _productoRepository.Update(item.Producto, false);

        //                 }

        //                 cotizacion.Confirmado = true;
        //                 _cotizacionRepository.Update(cotizacion, false);
        //                 _ventaRepository.saveChanges();
        //                 TempData["Message"] = "¡La Cotizacion se ha Confirmado con exito.!";
        //                 return RedirectToAction("ReciboVentaPdf", "CrearPDF", new { id = venta.Id });
        //             }

        //             else if (prod == false && serv == true)
        //             {
        //                 var nuevaVentaServicio = new VentaServicio()
        //                 {

        //                     Nit = cotizacion.Nit,
        //                     Nombres = cliente.Nombre,
        //                     Direccion = cotizacion.Direccion,
        //                     FechaVenta = DateTime.Now,
        //                     EmpleadoId = 1
        //                 };

        //                 var nuevoDetalleCaja = new DetalleCajaClinica()
        //                 {
        //                     VentaServicio = nuevaVentaServicio,
        //                     Descripcion = "Venta de servicio a cliente: " + cliente.Nombre,
        //                     Ingreso = cotizacion.DetalleCotizacion.Sum(a => Convert.ToDecimal(a.Total)),
        //                     CajaClinica = cajaAbierta
        //                 };

        //                 _cajaClinicaRepository.Add(nuevoDetalleCaja, false);

        //                 foreach (var item in cotizacion.DetalleCotizacion)
        //                 {
        //                     // var servicio =_servicioRepository.Get((int)item.ServicioId);

        //                     var detalle = new DetalleServicio()
        //                     {
        //                         VentaServicio = nuevaVentaServicio,
        //                         Cantidad = Convert.ToInt32(item.Cantidad),
        //                         Precio = Convert.ToDecimal(item.Precio),
        //                         Total = Convert.ToDecimal(item.Total),
        //                         Servicio = item.Servicio,
        //                     };

        //                     _ventaServicioRepository.Add(detalle, false);

        //                 }

        //                 cotizacion.Confirmado = true;
        //                 _cotizacionRepository.Update(cotizacion, false);
        //                 _ventaRepository.saveChanges();

        //                 TempData["Message"] = "¡La cotizacion se ha confirmado con exito.!";
        //                 return RedirectToAction("ReciboVentaPdf", "CrearPDF", new { id = nuevaVentaServicio.Id });
        //             }
        //             else if (prod == true && serv == true)
        //             {
        //                 var venta = new Venta()
        //                 {
        //                     //NoComprobante = det.encabezado.NoComprobante,
        //                     Nit = cotizacion.Nit,
        //                     Nombres = cliente.Nombre,
        //                     Direccion = cotizacion.Direccion,
        //                     FechaVenta = DateTime.Now,
        //                     EmpleadoId = 1

        //                 };

        //                 var nuevaVentaServicio = new VentaServicio()
        //                 {
        //                     Nit = cotizacion.Nit,
        //                     Nombres = cliente.Nombre,
        //                     Direccion = cotizacion.Direccion,
        //                     FechaVenta = DateTime.Now,
        //                     EmpleadoId = 1

        //                 };

        //                 foreach (var item in cotizacion.DetalleCotizacion)
        //                 {
        //                     // var producto = _productoRepository.Get((int)item.ProductoId);
        //                     if (item.Producto != null)
        //                     {
        //                         var nuevodetalle = new DetalleVenta()
        //                         {
        //                             Venta = venta,
        //                             Cantidad = Convert.ToInt32(item.Cantidad),
        //                             Precio = Convert.ToDecimal(item.Precio),
        //                             Descuento = Convert.ToDecimal(item.Descuento),
        //                             Subtotal = Convert.ToDecimal(item.Subtotal),
        //                             Total = Convert.ToDecimal(item.Total),
        //                             Producto = item.Producto,

        //                         };

        //                         _ventaRepository.Add(nuevodetalle, false);
        //                         item.Producto.Stock -= nuevodetalle.Cantidad;
        //                         _productoRepository.Update(item.Producto, false);
        //                     }

        //                     //var servicio = _servicioRepository.Get((int)item.ServicioId);
        //                     if (item.Servicio != null)
        //                     {
        //                         var detalle = new DetalleServicio()
        //                         {
        //                             VentaServicio = nuevaVentaServicio,
        //                             Cantidad = Convert.ToInt32(item.Cantidad),
        //                             Precio = Convert.ToDecimal(item.Precio),
        //                             Total = Convert.ToDecimal(item.Total),
        //                             Servicio = item.Servicio,

        //                         };

        //                         _ventaServicioRepository.Add(detalle, false);
        //                     }

        //                 }

        //                 var nuevoDetalleCaja = new DetalleCajaClinica()
        //                 {
        //                     Venta = venta,

        //                     Descripcion = "Venta a cliente: " + cliente.Nombre,
        //                     Ingreso = venta.DetalleVenta.Sum(a => Convert.ToDecimal(a.Total)),
        //                     CajaClinica = cajaAbierta
        //                 };

        //                 var nuevoDetalleCaja2 = new DetalleCajaClinica()
        //                 {

        //                     VentaServicio = nuevaVentaServicio,
        //                     Descripcion = "Venta a cliente: " + cliente.Nombre,
        //                     Ingreso = nuevaVentaServicio.DetalleServicio.Sum(a => Convert.ToDecimal(a.Total)),
        //                     CajaClinica = cajaAbierta
        //                 };

        //                 _cajaClinicaRepository.Add(nuevoDetalleCaja, false);
        //                 _cajaClinicaRepository.Add(nuevoDetalleCaja2, false);

        //                 cotizacion.Confirmado = true;
        //                 _cotizacionRepository.Update(cotizacion, false);
        //                 _ventaRepository.saveChanges();
        //                 TempData["Message"] = "¡La Cotizacion se ha Confirmado con exito.!";
        //                 return RedirectToAction("ReciboVentaPdf", "CrearPDF", new { id = venta.Id });
        //             }
        //         }

        //     }

        //     TempData["Message"] = "¡La Cotizacion no se modifico.!";
        //     //return new JsonErrorResult(new { message = "Ha ocurrido un error de servidor." });  
        //     var model = new CotizacionBaseViewModel()
        //     {
        //         Cotizacion = cotizacion,
        //     };
        //     model.Init(_productoRepository);

        //     return View(model);
        // }

        public JsonResult ModificarCotizacion([FromBody] ViewModelCotizacion det)
        {
            var listadoEnBd = _cotizacionRepository.GetDetalle(det.encabezado.Id);

            if (det.encabezado.Empleado == "")
            {

                TempData["Message"] = "¡Codigo de empleado incorrecto.!";
                return Json("");
            }


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
                        _cotizacionRepository.Delete(item.Id, false);
                    }



                }

                var cotizacion = _cotizacionRepository.Get(det.encabezado.Id);

                cotizacion.Nit = det.encabezado.Nit;
                cotizacion.Direccion = det.encabezado.Direccion;
                cotizacion.FechaCotizacion = DateTime.Now;
                // cotizacion.FechaValida = DateTime.Now.AddDays(Convert.ToInt16(det.encabezado.NoComprobante));





                _cotizacionRepository.Update(cotizacion);


                foreach (var item in det.detalle)
                {
                    var produ = _productoRepository.Get(Convert.ToInt32(item.Producto));

                    if (produ == null)
                    {
                        var nuevodetalle = new DetalleCotizacion()

                        {
                            Cotizacion = cotizacion,
                            ServicioId = Convert.ToInt32(item.Producto),
                            Cantidad = Convert.ToInt32(item.Cantidad),
                            Precio = Convert.ToDecimal(item.Precio),
                            Descuento = Convert.ToDecimal(item.Descuento),
                            Subtotal = Convert.ToDecimal(item.Subtotal),
                            Total = Convert.ToDecimal(item.Total),
                        };

                        _cotizacionRepository.Add(nuevodetalle, false);
                    }

                    else
                    {

                        var nuevodetalle = new DetalleCotizacion()

                        {
                            Cotizacion = cotizacion,
                            ProductoId = Convert.ToInt32(item.Producto),
                            Cantidad = Convert.ToInt32(item.Cantidad),
                            Precio = Convert.ToDecimal(item.Precio),
                            Descuento = Convert.ToDecimal(item.Descuento),
                            Subtotal = Convert.ToDecimal(item.Subtotal),
                            Total = Convert.ToDecimal(item.Total),
                        };

                        _cotizacionRepository.Add(nuevodetalle, false);

                    }



                }

                _cotizacionRepository.saveChanges();
                TempData["Message"] = "¡La Cotizacion se ha modificado con exito.!";
                return Json("OK");

            }

            TempData["Message"] = "¡La Cotizacion no se modifico.!";
            return new JsonErrorResult(new { message = "Ha ocurrido un error de servidor." });
        }

        public IActionResult Backup()
        {

            return View();
        }

        public IActionResult Delete(int id, string view)
        {

            _cotizacionRepository.DeleteCoti(id);

            return RedirectToAction(view);
        }

    }

    public class DetalleCotizacionBinding
    {
        public string Producto { get; set; }
        public string Cantidad { get; set; }
        public string Precio { get; set; }
        public string Descuento { get; set; }

        public string Subtotal { get; set; }

        public string Total { get; set; }
    }




    public class IdsCotBinding
    {
        public string Ids { get; set; }
    }



    public class EncabezadoCotizacionBinding
    {
        public string NoComprobante { get; set; }

        public string Cliente { get; set; }

        public string Nit { get; set; }

        public string Direccion { get; set; }

        public string Empleado { get; set; }

        public int Id { get; set; }


    }

    public class ViewModelCotizacion
    {
        public List<DetalleCotizacionBinding> detalle { get; set; }
        public List<IdsCotBinding> nuevos { get; set; }
        public EncabezadoCotizacionBinding encabezado { get; set; }
    }

}