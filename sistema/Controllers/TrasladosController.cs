using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using farmamest.Models;
using Database.Shared.IRepository;
using Database.Shared.Data;
using Wkhtmltopdf.NetCore;
using sistema.Models;
using sistema.Json;
using Database.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Database.Shared.Enumeraciones;

namespace sistema.Controllers
{
    [Authorize]
    public class TrasladosController : Controller
    {
        private readonly IProducto _productoRepository = null;
        private readonly IBodega _bodegaRepository = null;
        private readonly ITraslados _trasladosRepository = null;
        private readonly UserManager<User> _userManager = null;
        private readonly IGeneratePdf _generatePdf = null;
        private readonly IUser _userRepository;
        private readonly IPrecios _preciosRepository;

        public TrasladosController(IProducto productoRepository,
            ITraslados trasladosRepository,
            IBodega bodegaRepository,
            IPrecios preciosRepository,
            UserManager<User> userManager,
        IGeneratePdf generatePDF, IUser userRepository)
        {
            _productoRepository = productoRepository;
            _userManager = userManager;
            _trasladosRepository = trasladosRepository;
            _bodegaRepository = bodegaRepository;
            _generatePdf = generatePDF;
            _userRepository = userRepository;
            _preciosRepository = preciosRepository;
        }

        public IActionResult NuevoTraslado()
        {
            var model = new TrasladoViewModel();
            var rolFarmacia = User.IsInRole("Farmacia");
            model.Init(_bodegaRepository, rolFarmacia);
            return View(model);
        }

        [HttpPost]
        public string GuardarTraslado(TrasladoViewModel model)
        {
            try
            {
                var user = _userManager.GetUserAsync(HttpContext.User);
                var fechaHora = DateTime.Now;
                //Crear traslado
                var nuevoTraslado = new TrasladosProductos()
                {
                    // en tansito es el inicial
                    EstadoTrasladosId = (int)EstadoTrasladoEnum.EnTransito,
                    FechaTraslado = fechaHora,
                    Observaciones = model.Observaciones,
                    BodegaOrigenId = model.BodegaOrigenId,
                    BodegaDestinoId = model.BodegaDestinoId,
                    ResponsableEnviado = user.Result
                };



                if (model.Productos != null)
                {
                    foreach (var producto in model.Productos)
                    {
                        var nuevoDetalle = new DetalleTrasladoProductos()
                        {
                            ProductoInventarioId = producto.ProductoInventarioId,
                            Cantidad = producto.CantidadTrasladada,
                            FechaTraslado = DateTime.Now,
                            TrasladosProductos = nuevoTraslado
                        };

                        nuevoTraslado.DetalleTrasladoProductos.Add(nuevoDetalle);

                        //Se resta el producto de la bodega actual,
                        //pues su estado pasa a en Transito
                        var productoInventario = _productoRepository
                            .GetRegistroInventarioProducto((int)producto.ProductoInventarioId);
                        productoInventario.Stock -= producto.CantidadTrasladada;
                        _productoRepository.Update(productoInventario);
                    }
                }


                _trasladosRepository.Add(nuevoTraslado);


                return JsonSerializer.Serialize(new
                {
                    Exitoso = true
                });


            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al guardar traslado. " + ex.Message
                });
            }


        }


        [HttpPost]
        public string EditarTraslado(TrasladoViewModel model)
        {
            try
            {
                if (model.TrasladoId == null)
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error: TrasladoId es nulo." });
                }

                var traslado = _trasladosRepository.GetTraslados((int)model.TrasladoId);
                if (traslado == null)
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error: No se encontró el traslado." });
                }

                traslado.Observaciones = model.Observaciones;
                traslado.BodegaOrigenId = model.BodegaOrigenId;
                traslado.BodegaDestinoId = model.BodegaDestinoId;

                // Obtener los IDs de productos que vienen desde la vista
                // Obtener los IDs de productos que vienen desde la vista, ignorando los nulos
                var productosIds = model.Productos?
                    .Select(p => p.DetalleTrasladoId)
                    .Where(id => id.HasValue)  // Filtra los valores no nulos
                    .Select(id => id.Value)    // Convierte de int? a int
                    .ToList() ?? new List<int>(); // Si es null, asigna una lista vacía

                // Marcar como eliminados los productos que no están en la lista actualizada
                var productosParaEliminar = traslado.DetalleTrasladoProductos
                    .Where(p => !productosIds.Contains(p.Id))
                    .ToList();

                foreach (var detalle in productosParaEliminar)
                {
                    detalle.Eliminado = true;

                    if (detalle.ProductoInventarioId == null)
                    {
                        return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error: ProductoInventarioId es nulo." });
                    }

                    // Restaurar stock del inventario
                    var productoInventario = _productoRepository.GetRegistroInventarioProducto((int)detalle.ProductoInventarioId);
                    if (productoInventario == null)
                    {
                        return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error: ProductoInventario no encontrado al restaurar stock." });
                    }

                    productoInventario.Stock += detalle.Cantidad;
                    _productoRepository.UpdateRegistroInventario(productoInventario);
                }

                // Procesar los productos que se quedan o se actualizan
                if (model.Productos != null)
                {
                    foreach (var productoTraslado in model.Productos)
                    {
                        if (productoTraslado.ProductoInventarioId == null)
                        {
                            return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error: ProductoInventarioId es nulo en ProductoTraslado." });
                        }

                        // Buscar si el producto ya existe en el traslado
                        var detalleExistente = traslado.DetalleTrasladoProductos
                            .FirstOrDefault(d => d.Id == productoTraslado.DetalleTrasladoId);

                        if (detalleExistente != null)
                        {
                            // Actualizar el producto existente
                            detalleExistente.Cantidad = productoTraslado.CantidadTrasladada;
                            detalleExistente.Eliminado = false;
                        }
                        else
                        {
                            // Agregar un nuevo producto
                            var nuevoDetalle = new DetalleTrasladoProductos
                            {
                                Cantidad = productoTraslado.CantidadTrasladada,
                                ProductoInventarioId = productoTraslado.ProductoInventarioId,
                                FechaTraslado = DateTime.Now
                            };

                            traslado.DetalleTrasladoProductos.Add(nuevoDetalle);
                        }

                        // Restar stock del inventario
                        var productoInventario = _productoRepository.GetRegistroInventarioProducto((int)productoTraslado.ProductoInventarioId);
                        if (productoInventario == null)
                        {
                            return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error: ProductoInventario no encontrado antes de restar stock." });
                        }

                        productoInventario.Stock -= productoTraslado.CantidadTrasladada;
                        _productoRepository.UpdateRegistroInventario(productoInventario);
                    }
                }

                _trasladosRepository.UpdateTraslado(traslado);

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al editar traslado. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string ConsultarProductosInventario(int bodegaOrigenId)
        {
            try
            {
                var listaProductos = new List<TrasladoProductoDisponibleViewModel>();
                var productosBd = _trasladosRepository
                    .GetProductosDisponiblesTraslado(bodegaOrigenId);
                if (productosBd != null)
                {
                    foreach (var producto in productosBd)
                    {
                        listaProductos.Add(new TrasladoProductoDisponibleViewModel
                        {
                            ProductoInventarioId = producto.Id,
                            ProductoId = producto.ProductoId,
                            ProductoNombre = producto.Producto != null
                            ? producto.Producto.NombreProducto
                            : "-",
                            ProductoCodigo = producto.Producto != null
                            ? producto.Producto.CodigoReferencia
                            : "-",
                            CantidadExistente = producto.Stock,
                            UnidadMedidaVentaId = producto.UnidadMedidaVentaId ?? 0,
                            UnidadMedidaVentaNombre =
                            producto.UnidadMedidaVenta != null
                            ? producto.UnidadMedidaVenta.Nombre
                            : "-",
                            CantidadTrasladada = 0
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaProductos
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos de inventario. "
                    + ex.Message
                });
            }
        }

        [HttpPost]
        public string AceptarTraslado(int? id)
        {
            try
            {
                if (id == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Código de traslado no válido"
                    });
                }

                var traslado = _trasladosRepository.GetTraslados((int)id);

                if (traslado == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No existe registro para este traslado"
                    });
                }

                // Verificar si ya está aceptado
                if (traslado.EstadoTrasladosId == (int)EstadoTrasladoEnum.Aceptado)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Error. Este traslado ya se encuentra registrado como ACEPTADO"
                    });
                }

                var bodegaDestino = _bodegaRepository.GetById((int)traslado.BodegaDestinoId);
                if (bodegaDestino == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se encontró la bodega destino"
                    });
                }
                int? ambienteId = bodegaDestino.AmbienteId;

                // Modificar stock y asignar ambiente al producto
                foreach (var item in traslado.DetalleTrasladoProductos)
                {
                    var producto = _productoRepository.Get(item.ProductoInventario.ProductoId);

                    if (producto != null && ambienteId.HasValue)
                    {
                        producto.AmbienteId = ambienteId.Value;
                        _productoRepository.Update(producto, false);
                    }

                    var productoInventario = _productoRepository.GetValidarProductoInventario(
                        (int)traslado.BodegaDestinoId,
                        item.ProductoInventario.UnidadMedidaVentaId,
                        item.ProductoInventario.ProductoId
                    );

                    if (productoInventario != null)
                    {
                        productoInventario.Stock += item.Cantidad;
                        _productoRepository.UpdateRegistroInventario(productoInventario);
                    }
                    else
                    {
                        var registroProductoInventario = new ProductoInventario
                        {
                            BodegaId = traslado.BodegaDestinoId,
                            ProductoId = item.ProductoInventario.ProductoId,
                            UnidadMedidaVentaId = item.ProductoInventario.UnidadMedidaVentaId,
                            Stock = item.Cantidad,
                            ProductosInventarioPrecios = new List<ProductoInventarioPrecio>()
                        };

                        var precios = _preciosRepository.GetList().ToList();
                        if (precios != null)
                        {
                            foreach (var precio in precios)
                            {
                                registroProductoInventario.ProductosInventarioPrecios.Add(new ProductoInventarioPrecio
                                {
                                    PrecioId = precio.Id,
                                    Valor = 0
                                });
                            }
                        }
                        _productoRepository.AddProductoInventario(registroProductoInventario);
                    }
                }

                // Cambiar el estado del traslado a ACEPTADO
                var user = _userManager.GetUserAsync(HttpContext.User);
                traslado.ResponsableRecibido = user.Result;
                traslado.EstadoTrasladosId = (int)EstadoTrasladoEnum.Aceptado;
                _trasladosRepository.UpdateTraslado(traslado, false);

                _productoRepository.SaveChanges();
                TempData["Message"] = "¡Se ha modificado con éxito!";

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al aceptar traslado. " + ex.Message
                });
            }
        }

        public IActionResult Lista(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _trasladosRepository.PaginacionTraslados(sortOrder, buscar, pageNumber, 20);

            return View(lista);
        }

        public IActionResult EnTransito(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _trasladosRepository.PaginacionEnTransito(sortOrder, buscar, pageNumber, 20);

            return View(lista);
        }

        public IActionResult Aceptados(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _trasladosRepository.PaginacionAceptados(sortOrder, buscar, pageNumber, 20);

            return View(lista);
        }

        public IActionResult Faltantes(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _trasladosRepository.PaginacionFaltantes(sortOrder, buscar, pageNumber, 20);

            return View(lista);
        }

        public IActionResult Cancelados(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _trasladosRepository.PaginacionCancelados(sortOrder, buscar, pageNumber, 20);

            return View(lista);
        }

        public IActionResult ConProblema(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _trasladosRepository.PaginacionConProblema(sortOrder, buscar, pageNumber, 20);

            return View(lista);
        }

        public IActionResult Editar(int? id)
        {
            if (id == null) return StatusCode(404);

            var traslado = _trasladosRepository.GetTraslados((int)id);

            if (traslado == null) return StatusCode(400);

            var model = new TrasladoViewModel()
            {
                BodegaOrigenId = traslado.BodegaOrigenId,
                BodegaDestinoId = traslado.BodegaDestinoId,
                TrasladoId = traslado.Id,
                EstadoTraslado = traslado.EstadoTraslados != null
                ? traslado.EstadoTraslados.DescripcionEstado
                : "N/A"
            };

            var rolFarmacia = User.IsInRole("Farmacia");

            model.Init(_bodegaRepository, rolFarmacia);

            return View(model);
        }

        [HttpPost]
        public string ConsultarProductosTrasladados(int trasladoId)
        {
            try
            {
                var detalleTrasladoBd = _trasladosRepository.GetListDetalleTrasladoProductos(trasladoId);
                var listaProductos = new List<TrasladoProductoViewModel>();
                if (detalleTrasladoBd != null)
                {
                    foreach (var productoTraslado in detalleTrasladoBd)
                    {
                        if (productoTraslado.ProductoInventario != null)
                        {
                            var nombreUnidad = productoTraslado
                                .ProductoInventario.UnidadMedidaVenta != null
                                ? productoTraslado.ProductoInventario.UnidadMedidaVenta.Nombre
                                : "-";
                            listaProductos.Add(new TrasladoProductoViewModel
                            {
                                DetalleTrasladoId = productoTraslado.Id,
                                ProductoCodigo = productoTraslado.ProductoInventario.Producto.CodigoReferencia,
                                ProductoNombre = productoTraslado.ProductoInventario.Producto.NombreProducto,
                                CantidadTrasladada = productoTraslado.Cantidad,
                                ProductoInventarioId = productoTraslado.ProductoInventarioId,
                                UnidadMedidaVentaNombre = nombreUnidad,
                                FechaTraslado = productoTraslado.FechaTraslado,
                                Nuevo = false
                            });
                        }
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaProductos
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar productos. " + ex.Message
                });
            }
        }

        public IActionResult CambiarAEnTransito(int? trasladoId)
        {
            if (trasladoId == null) return StatusCode(400);

            var traslado = _trasladosRepository.GetTraslados((int)trasladoId);

            if (traslado == null) return StatusCode(404);

            if (traslado.EstadoTrasladosId == (int)EstadoTrasladoEnum.Aceptado)
            {
                TempData["Message"] = "¡Error, este registro ya esta registrado como: Aceptado.!";
                return RedirectToAction("Editar", new { id = trasladoId });
            }

            var user = _userManager.GetUserAsync(HttpContext.User);
            traslado.ResponsableRecibido = user.Result;
            traslado.EstadoTrasladosId = (int)EstadoTrasladoEnum.EnTransito;

            _trasladosRepository.UpdateTraslado(traslado);

            TempData["Message"] = "¡Se ha modificado con éxito.!";
            return RedirectToAction("Editar", new { id = trasladoId });
        }

        public IActionResult CambiarAFaltantes(int? trasladoId)
        {
            if (trasladoId == null) return StatusCode(400);

            var traslado = _trasladosRepository.GetTraslados((int)trasladoId);

            if (traslado == null) return StatusCode(404);

            if (traslado.EstadoTrasladosId == (int)EstadoTrasladoEnum.Aceptado)
            {
                TempData["Message"] = "¡Error, este registro ya esta registrado como: Aceptado.!";
                return RedirectToAction("Editar", new { id = trasladoId });
            }

            var user = _userManager.GetUserAsync(HttpContext.User);
            traslado.ResponsableRecibido = user.Result;
            traslado.EstadoTrasladosId = (int)EstadoTrasladoEnum.Faltantes;

            _trasladosRepository.UpdateTraslado(traslado);

            TempData["Message"] = "¡Se ha modificado con éxito.!";
            return RedirectToAction("Editar", new { id = trasladoId });
        }

        public IActionResult CambiarAConProblema(int? trasladoId)
        {
            if (trasladoId == null) return StatusCode(400);

            var traslado = _trasladosRepository.GetTraslados((int)trasladoId);

            if (traslado == null) return StatusCode(404);

            if (traslado.EstadoTrasladosId == (int)EstadoTrasladoEnum.Aceptado)
            {
                TempData["Message"] = "¡Error, este registro ya esta registrado como: Aceptado.!";
                return RedirectToAction("Editar", new { id = trasladoId });
            }

            var user = _userManager.GetUserAsync(HttpContext.User);
            traslado.ResponsableRecibido = user.Result;
            traslado.EstadoTrasladosId = (int)EstadoTrasladoEnum.ConProblema;

            _trasladosRepository.UpdateTraslado(traslado);

            TempData["Message"] = "¡Se ha modificado con éxito.!";
            return RedirectToAction("Editar", new { id = trasladoId });
        }

        public IActionResult CambiarACancelado(int? trasladoId)
        {
            if (trasladoId == null) return StatusCode(400);

            var traslado = _trasladosRepository.GetTraslados((int)trasladoId);

            if (traslado == null) return StatusCode(404);

            if (traslado.EstadoTrasladosId == (int)EstadoTrasladoEnum.Aceptado)
            {
                TempData["Message"] = "¡Error, este registro ya esta registrado como: Aceptado.!";
                return RedirectToAction("Editar", new { id = trasladoId });
            }

            var user = _userManager.GetUserAsync(HttpContext.User);
            traslado.ResponsableRecibido = user.Result;
            traslado.EstadoTrasladosId = (int)EstadoTrasladoEnum.Denegado;

            _trasladosRepository.UpdateTraslado(traslado);

            TempData["Message"] = "¡Se ha modificado con éxito.!";
            return RedirectToAction("Editar", new { id = trasladoId });
        }

        [Authorize(Roles = "Administrador, Supervisor, Vendedor, Mensajero,Desarrollador,Farmacia,Laboratorio,Hospital,Clinica,Urologia,Algologia")]
        public async Task<IActionResult> Reporte(int? trasladoId)
        {
            if (trasladoId == null) return StatusCode(400);

            var traslado = _trasladosRepository.GetTraslados((int)trasladoId);

            if (traslado == null) return StatusCode(404);

            var user = _userManager.GetUserAsync(HttpContext.User);
            var userU = _userRepository.GetbyId(user.Result.Id);
            var u = userU.Persona != null
                ? userU.Persona.Nombre
                : "-";

            var model = new TrasladosBaseViewModel()
            {
                TrasladosProductos = traslado,
            };

            return await _generatePdf.GetPdf("Views/Traslados/Reporte.cshtml", model);
        }

        public async Task<IActionResult> ReporteU(int? trasladoId)
        {
            if (trasladoId == null) return StatusCode(400);

            var traslado = _trasladosRepository.GetTraslados((int)trasladoId);

            if (traslado == null) return StatusCode(404);

            var user = _userManager.GetUserAsync(HttpContext.User);
            var userU = _userRepository.GetbyId(user.Result.Id);
            var u = userU.Persona != null
                ? userU.Persona.Nombre
                : "-";

            var model = new TrasladosBaseViewModel()
            {
                TrasladosProductos = traslado,
            };

            return await _generatePdf.GetPdf("Views/Traslados/ReporteU.cshtml", model);
        }

    }
}