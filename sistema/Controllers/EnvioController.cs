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
//using System.Web.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace sistema.Controllers
{
    [Authorize]
    public class EnvioController : Controller
    {

        // private readonly UserManager<IdentityUser> _userManager;
        private readonly IEnvio _envioRepository = null;

        private readonly IEmpleado _empleadoRepository = null;

        private readonly IVenta _ventaRepository = null;

        private readonly IRuta _rutaRepository = null;

        private readonly IProducto _productoRepository = null;

        private readonly ICaja _cajaRepository = null;
        private readonly ICliente _clienteRepository = null;

        private readonly IUser _userRepository = null;

        

        public EnvioController(IVenta ventaRepository, IEmpleado empleadoRepository, IEnvio envioRepository, IRuta rutaRepository, IProducto productoRepository
        , ICaja cajaRepository, ICliente clienteRepository, IUser userRepository)
        {
            _ventaRepository = ventaRepository;
            _empleadoRepository = empleadoRepository;
            _envioRepository = envioRepository;
            _rutaRepository = rutaRepository;
            _productoRepository = productoRepository;
            _cajaRepository = cajaRepository;
            _clienteRepository = clienteRepository;
            _userRepository = userRepository;
        }

       

        public IActionResult Lista(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

            var lista = _envioRepository.PaginacionEnvios(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult ListaLiquidados(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

            var lista = _envioRepository.PaginacionEnviosLiquidados(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult ListaRechazados(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

            var lista = _envioRepository.PaginacionEnviosRechazados(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult ListaEnRuta(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

            var lista = _envioRepository.PaginacionEnviosEnRuta(sortOrder, buscar, pageNumber, 10,"");

            return View(lista);
        }

        public IActionResult ListaPedidos(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

            var lista = _envioRepository.PaginacionEnviosPedidos(sortOrder, buscar, pageNumber, 10);

            return View(lista);
        }

        public IActionResult ListaMisPedidos(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

       

            var user = _userRepository.Get(User.Identity.Name);

            var lista = _envioRepository.PaginacionMisPedidos(sortOrder, buscar, pageNumber, 10, user.Id);

            return View(lista);
        }

         public IActionResult ListaMisPedidosEntregados(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

       

            var user = _userRepository.Get(User.Identity.Name);

            var lista = _envioRepository.PaginacionMisPedidosEntregados(sortOrder, buscar, pageNumber, 10, user.Id);

            return View(lista);
        }
        public IActionResult ListaMisPedidosEntregadosAdmin(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

       

           

            var lista = _envioRepository.PaginacionMisPedidosEntregados(sortOrder, buscar, pageNumber, 10,"");

            return View(lista);
        }

         public IActionResult ListaMisPedidosEnRuta(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            ViewData["CurrentFilter"] = buscar;

       

            var user = _userRepository.Get(User.Identity.Name);

            var lista = _envioRepository.PaginacionEnviosEnRuta(sortOrder, buscar, pageNumber, 10, user.Id);

            return View(lista);
        }

        public IActionResult VerPedido(int id)
        {

            var envio = _envioRepository.Get(id, true);

            var modelo = new EnvioBaseViewModel();

            modelo.Envio = envio;

            modelo.Init(_rutaRepository, _productoRepository);
            modelo.Init(_clienteRepository);

            var listausers = _ventaRepository.GetUsersRole("Mensajero");

            modelo.ListaUsuarios = listausers;

            modelo.Init(_envioRepository);

            return View(modelo);
        }

        public IActionResult VerPedidoMensajero(int id)
        {

            var envio = _envioRepository.Get(id, true);

            var empleado = _empleadoRepository.Get(envio.EmpleadoId);

            var modelo = new EnvioBaseViewModel();

            modelo.Envio = envio;
            modelo.Empleado=empleado;

            modelo.Init(_rutaRepository, _productoRepository);
            modelo.Init(_clienteRepository);

            var listausers = _ventaRepository.GetUsersRole("Mensajero");

            modelo.ListaUsuarios = listausers;

            modelo.Init(_envioRepository);

            return View(modelo);
        }

        public IActionResult EnviaraRutaAdmin(int id)
        {

            var envio = _envioRepository.Get(id, true);

            var modelo = new EnvioBaseViewModel();

            modelo.Envio = envio;

            modelo.Envio.EstadosEnvioId = 2;

            _envioRepository.Update(modelo.Envio);

            _envioRepository.saveChanges();

            modelo.Init(_rutaRepository, _productoRepository);

            TempData["Message"] = "¡El pedido se ha ido a ruta.!";

              return RedirectToAction("VerPedido",new {id=id});
        }
         public IActionResult EnviaraRuta(int id)
        {

            var envio = _envioRepository.Get(id, true);

            var modelo = new EnvioBaseViewModel();

            modelo.Envio = envio;

            modelo.Envio.EstadosEnvioId = 2;

            _envioRepository.Update(modelo.Envio);

            _envioRepository.saveChanges();

            modelo.Init(_rutaRepository, _productoRepository);

            TempData["Message"] = "¡El pedido se ha ido a ruta.!";

              //return RedirectToAction("VerPedidoMensajero",new {id=id});

              return RedirectToAction("ListaMisPedidosEnRuta");
        }

        public IActionResult Entregado(int id)
        {

            var envio = _envioRepository.Get(id, true);

            var modelo = new EnvioBaseViewModel();

            modelo.Envio = envio;

            modelo.Envio.EstadosEnvioId = 1002;

            _envioRepository.Update(modelo.Envio);

            _envioRepository.saveChanges();

            modelo.Init(_rutaRepository, _productoRepository);

            TempData["Message"] = "¡El pedido ha sido entregado!";

            return RedirectToAction("ListaMisPedidosEntregados");
        }

        public JsonResult RecibirPedido(int Id, string FormaPago, string Monto) // mandas tambien el metodo de pagoo segun seleccionado como parametro
        {

            var envio = _envioRepository.Get(Id, true);

             if (envio.EstadosEnvioId == 3)
            {
                TempData["Message"] = "¡Este envio ya ha sido recibido.!";
                 return Json("");
            }

            var cliente = _clienteRepository.GetClientePorNombre(envio.Nombres.Trim());

            var cajita = _cajaRepository.ListarCajas();

            if (!cajita.Any(a => a.EstadoCaja == true))
            {
                TempData["Message"] = "¡Error. No hay cajas abiertas. Por favor debe abrir una caja.!";
                return Json("");
            }

            var cajaAbierta = _cajaRepository.GetCajaAbierta();


            if (cliente == null)
            {
                var nuevoCliente = new Paciente()
                {
                    Nombre = envio.Nombres,
                    Direccion = envio.DireccionExacta,
                    Nit = envio.Nit,

                };

                var venta = new Venta()
                {

                    NoComprobante = envio.NoComprobante,
                    Paciente = nuevoCliente,
                    Nit = envio.Nit,
                    Nombres = nuevoCliente.Nombre,
                    Direccion = envio.DireccionExacta,
                    EmpleadoId = envio.EmpleadoId,
                    FechaVenta = DateTime.Now,

                };


             
                var pago = new Pagos()
                {
                 Venta= venta, 
                 FormaPagoId = Convert.ToInt32(FormaPago), 
                 Monto = Convert.ToDecimal(Monto),
                };

                _envioRepository.AddPago(pago,false);

                var nuevoDetalleCaja = new DetalleCaja()
                {
                    Venta = venta,
                    Descripcion = "Venta a cliente: " + nuevoCliente.Nombre + " Con Envio",
                    Ingreso = envio.DetalleEnvios.Sum(a => Convert.ToDecimal(a.Total)),
                    Caja = cajaAbierta
                };

                _cajaRepository.Add(nuevoDetalleCaja, false);

                foreach (var item in envio.DetalleEnvios)
                {
                    var producto = _productoRepository.Get(item.ProductoId);

                    var nuevodetalle = new DetalleVenta()
                    {
                        Venta = venta,
                        Cantidad = Convert.ToInt32(item.Cantidad),
                        Precio = Convert.ToDecimal(item.Precio),
                        Descuento = Convert.ToDecimal(item.Descuento),
                        Subtotal = Convert.ToDecimal(item.Subtotal),
                        Total = Convert.ToDecimal(item.Total),
                        ProductoId = producto.Id,

                    };

                    _ventaRepository.Add(nuevodetalle, false);
                    producto.Stock -= nuevodetalle.Cantidad;
                    _productoRepository.Update(producto, false);

                }

                

                envio.EstadosEnvioId = 3;

                _envioRepository.Update(envio, false);

                _envioRepository.saveChanges();


                TempData["Message"] = "¡El pedido se ha recibido con exito.!";

                // return RedirectToAction("VerPedido",new {id=id});
                return Json("ok");
            }
            else
            {
                var venta = new Venta()
                {

                    //NoComprobante = det.encabezado.NoComprobante,
                    Clientes = cliente,
                    Nit = envio.Nit,
                    Nombres = cliente.Nombre,
                    Direccion = envio.DireccionExacta,
                    EmpleadoId = envio.EmpleadoId,
                    FechaVenta = DateTime.Now,

                };

                var pago = new Pagos()
                {
                    Venta= venta, // metes la venta de arriba porque pago pide una venta.
                    FormaPagoId = Convert.ToInt32(FormaPago), 
                    Monto = Convert.ToDecimal(Monto),
                };

                _envioRepository.AddPago(pago,false);

                var nuevoDetalleCaja = new DetalleCaja()
                {
                    Venta = venta,
                    Descripcion = "Venta a cliente: " + cliente.Nombre + " Con Envio",
                    Ingreso = envio.DetalleEnvios.Sum(a => Convert.ToDecimal(a.Total)),
                    Caja = cajaAbierta
                };

                _cajaRepository.Add(nuevoDetalleCaja, false);

                foreach (var item in envio.DetalleEnvios)
                {
                    var producto = _productoRepository.Get(item.ProductoId);

                    var nuevodetalle = new DetalleVenta()
                    {
                        Venta = venta,
                        Cantidad = Convert.ToInt32(item.Cantidad),
                        Precio = Convert.ToDecimal(item.Precio),
                        Descuento = Convert.ToDecimal(item.Descuento),
                        Subtotal = Convert.ToDecimal(item.Subtotal),
                        Total = Convert.ToDecimal(item.Total),
                        ProductoId = producto.Id,

                    };

                    _ventaRepository.Add(nuevodetalle, false);
                    producto.Stock -= nuevodetalle.Cantidad;
                    _productoRepository.Update(producto, false);

                }

                

                envio.EstadosEnvioId = 3;

                _envioRepository.Update(envio, false);

                _envioRepository.saveChanges();


                TempData["Message"] = "¡El pedido se ha recibido con exito.!";

                // return RedirectToAction("VerPedido",new {id=id});    
                return Json("ok");

            }




         //return Json(Monto);



        }

        public IActionResult RechazarPedido(int id)
        {

            var envio = _envioRepository.Get(id, true);

            var modelo = new EnvioBaseViewModel();

            modelo.Envio = envio;

            modelo.Envio.EstadosEnvioId = 4;

            _envioRepository.Update(modelo.Envio);

            _envioRepository.saveChanges();


            modelo.Init(_rutaRepository, _productoRepository);

            TempData["Message"] = "¡El pedido ha sido rechazado.!";

             return RedirectToAction("VerPedido",new {id=id});
        }

        public JsonResult ModificarEnvio([FromBody] ViewModelVenta2 det)
        {
            var listadoEnBd = _envioRepository.GetDetalle(det.datosenvio.Id);

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
                        _envioRepository.Delete(item.Id, false);
                    }



                }

                //string[] fecha = det.datosenvio.Fecha.Split('-');

                 var user = _userRepository.GetbyId(det.datosenvio.UserId);

                var nuevoEnvio = new Envio()
                {
                    Id = Convert.ToInt32(det.datosenvio.Id),
                   // NombrePiloto = det.datosenvio.NombrePiloto,
                    RutaId = Convert.ToInt32(det.datosenvio.Ruta),
                    FechaEntrega = Convert.ToDateTime(det.datosenvio.Fecha),
                    DireccionExacta = det.datosenvio.DireccionExacta,
                    Nombres = det.datosenvio.Nombre,
                    NoComprobante = det.datosenvio.NoComprobante,
                    Nit = det.datosenvio.Nit,
                    EmpleadoId = Convert.ToInt32(det.datosenvio.EmpleadoId),
                    User = user,
                    FechaEnvio=DateTime.Now,
                    EstadosEnvioId = 1,

                };

                _envioRepository.Update(nuevoEnvio);


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

                TempData["Message"] = "¡El pedido se ha Modificado con exito.!";

                return Json("OK");

            }


            return new JsonErrorResult(new { message = "Ha ocurrido un error de servidor." });
        }







    }


    public class RecibirEnvioBinding
    {
      
        public int Id { get; set; }
        public string FormaPago { get; set; }

        public string Monto {get; set;}

      

    }

}
