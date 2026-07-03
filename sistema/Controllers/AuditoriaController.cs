using Database.Shared;
using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using sistema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Wkhtmltopdf.NetCore;

namespace sistema.Controllers
{
    [Authorize]
    public class AuditoriaController : Controller
    {
        private readonly IAmbiente _ambienteRepository = null;
        private readonly IProducto _productosRepository = null;
        private readonly IBodega _bodegaRepository = null;
        private readonly IDespegablesProducto _categoryRepository = null;
        private readonly ISucursal _sucursalRepository = null;
        private readonly IAuditoria _auditoriaRepository = null;
        private readonly UserManager<User> _userManager;
        private readonly IUser _userRepository;
        private readonly ITipoProducto _tipoProductoRepository;
        private readonly ISeguro _seguroRepository;
        private readonly Context _context; 


        public AuditoriaController(IAmbiente ambienteRepository, IProducto productosRepository, IBodega bodegaRepository,
            IDespegablesProducto categoryRepository, ISucursal sucursalRepository, IAuditoria auditoriaRepository,
            UserManager<User> userManager, IUser userRepository, ITipoProducto tipoProductoRepository, ISeguro seguroRepository, Context context)
        {
            _ambienteRepository = ambienteRepository;
            _productosRepository = productosRepository;
            _bodegaRepository = bodegaRepository;
            _categoryRepository = categoryRepository;
            _sucursalRepository = sucursalRepository;
            _auditoriaRepository = auditoriaRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _tipoProductoRepository = tipoProductoRepository;
            _seguroRepository = seguroRepository;
            _context = context;

        }

        public IActionResult Nuevo(int? BodegaId, int? TipoProductoId)
        {
            var sp = _productosRepository.GetBandejaAuditoriaNuevo(BodegaId, TipoProductoId);

            var viewModel = new InventarioViewModel();
            Bodega nombreBodega = new Bodega();
            string tipoProducto = "";

            if (TipoProductoId != null)
            {
                tipoProducto = _tipoProductoRepository.Get((int)TipoProductoId).NombreTipoProducto;
            }

            viewModel.AuditoriaSP = sp;

            if (BodegaId != null && BodegaId != 0)
            {
                nombreBodega = _bodegaRepository.GetById((int)BodegaId);
            }

            viewModel.currentFilter = viewModel.buscar;
            viewModel.Init(_categoryRepository, _sucursalRepository, _bodegaRepository, _categoryRepository, _ambienteRepository, _seguroRepository);

            viewModel.TodasLasPresentaciones = _categoryRepository.ListarPresentacion();

            viewModel.TotalMedicamentos = sp.Count();
            viewModel.BodegaId = BodegaId;
            viewModel.TipoProductoId = TipoProductoId;
            viewModel.NombreBodega = (nombreBodega == null || (BodegaId == null || BodegaId == 0)) ? "" : nombreBodega.NombreBodega;
            viewModel.NombreTipoProductos = tipoProducto == null ? "" : tipoProducto;

            return View(viewModel);
        }

        public IActionResult Modificar(int AuditoriaId)
        {
            var data = _auditoriaRepository.GetAuditoria(AuditoriaId);
            return View(data);
        }

        [HttpPost]
        public string Eliminar(int? auditoriaId)
        {
            try
            {
                _auditoriaRepository.DeleteAuditoria(auditoria: (int)auditoriaId);
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
                    Mensaje = "Error al eliminar esta auditoria. " + ex.Message
                });
            }
        }

        public IActionResult Lista()
        {
            AuditoriaViewModel auditoriasViewModel = new AuditoriaViewModel()
            {
                Auditorias = _auditoriaRepository.GetAllAuditoria()
            };

            return View(auditoriasViewModel);
        }

        public IActionResult Detalles(int? auditoriaId, string codigo, string nombre, string unidadCompra, string unidadVenta)
        {
            if (auditoriaId == null)
            {
                TempData["Message"] = "Error de navegacion: Request is incorrect";
                return RedirectToAction("Lista");
            }

            var auditoria = _auditoriaRepository.GetDetalleAuditoria((int)auditoriaId, codigo, nombre, unidadCompra, unidadVenta);

            var unidadCompraSelect = ((auditoria.Productos.SelectMany(x => x.Producto.ProductosInventario).ToList())).Select(x => x.UnidadMedidaCompra).ToList();
            unidadCompraSelect.RemoveAll(x => x == null);

            var unidadCompraList = unidadCompraSelect
                .Select(x => x.Nombre)
                .Where(nombre => nombre != null)
                .Distinct()
                .Select(nombre => new { Nombre = nombre })
                .ToList();

            ViewBag.UnidadCompra = new SelectList(unidadCompraList, "Nombre", "Nombre");

            var unidadventaselect = ((auditoria.Productos.SelectMany(x => x.Producto.ProductosInventario).ToList())).Select(x => x.UnidadMedidaVenta).ToList();
            unidadventaselect.RemoveAll(x => x == null);

            var unidadVentaList = unidadventaselect
                .Select(x => x.Nombre)
                .Where(nombre => nombre != null)
                .Distinct()
                .Select(nombre => new { Nombre = nombre })
                .ToList();

            ViewBag.UnidadVenta = new SelectList(unidadVentaList, "Nombre", "Nombre");

            if (auditoria == null)
            {
                TempData["Message"] = "El paquete no existe";
                return RedirectToAction("Lista");
            }

            return View(auditoria);
        }

        [HttpPost]
        public IActionResult ModificarStock([FromBody] ModificarAuditoriaViewModel model)
        {
            Auditoria auditoria = _auditoriaRepository.GetAuditoria(model.AuditoriaId);

            foreach (var auditoriaProd in auditoria.Productos)
            {
                foreach (var item in model.Productos)
                {
                    var idProducto = item.Key;
                    var nuevoStock = item.Value;

                    if (auditoriaProd.Id == idProducto)
                    {
                        if (auditoria.ActualizoStock)
                        {
                            foreach (var itemProductoInventario in auditoriaProd.Producto.ProductosInventario)
                            {
                                itemProductoInventario.Stock = nuevoStock;
                            }
                        }
                        auditoriaProd.Stock = nuevoStock;
                    }
                }
            }

            _auditoriaRepository.UpdateAuditoria(auditoria);

            return Ok();
        }



        [HttpPost]
        public IActionResult ActualizarStock([FromBody] List<AuditoriaProductoStock> productos)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                var personaCreacionAuditoria = _userRepository.GetUserNameOrDefault(userId);

                // Filtrar productos con stock ingresado
                productos = productos.Where(a => a.StockIngresado != null).ToList();
                var productosArray = new List<object>();

                foreach (var p in productos)
                {
                    // Obtener el ProductoInventario con tracking
                    var productoInventario = _context.ProductosInventario
                        .FirstOrDefault(pi => pi.Id == p.IdProductoInventario);

                    if (productoInventario == null) continue;

                    int stockActual = (int)productoInventario.Stock;
                    int stockIngresado = p.StockIngresado ?? 0;

                    // VALIDACIÓN: el stock ingresado no puede ser mayor al stock actual
                    if (stockIngresado > stockActual)
                    {
                        transaction.Rollback();
                        return BadRequest(new
                        {
                            Exitoso = false,
                            Mensaje = $"No se puede restar {stockIngresado} unidades del producto '{p.NombreProducto}'. " +
                                      $"Stock actual es {stockActual}. Ingrese un valor menor o igual."
                        });
                    }

                    // RESTAR: nuevo stock = stock actual - stock ingresado
                    int stockFinal = stockActual - stockIngresado;

                    // Actualizar el stock
                    productoInventario.Stock = stockFinal;
                    _context.Entry(productoInventario).Property(x => x.Stock).IsModified = true;

                    // Preparar objeto para auditoría
                    productosArray.Add(new
                    {
                        idproducto = p.IdProducto,
                        stockingresado = stockIngresado,
                        stockfinal = stockFinal,
                        id_producto_inventario = p.IdProductoInventario,
                        codigo_referencia = p.CodigoReferencia,
                        nombre_producto = p.NombreProducto,
                        fecha_vencimiento = p.FechaVencimiento,
                        lote_producto = p.LoteProducto,
                        fecha_recepcion_lote = p.FechaRecepcionLote,
                        precio_costo = p.PrecioCosto
                    });
                }

                _context.SaveChanges();
                _auditoriaRepository.AddAuditoriaSp(productosArray.ToArray(), userId, personaCreacionAuditoria, false);

                transaction.Commit();
                return Ok(new { Exitoso = true, Mensaje = "Auditoría guardada y stock actualizado correctamente." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al actualizar: {ex.Message}" });
            }
        }
        /// <summary>
        /// Guarda la auditoría SIN actualizar el stock en inventario.
        /// Solo registra el conteo auditado para referencia.
        /// </summary>
        [HttpPost]
        public IActionResult NuevoSinActualizarStock([FromBody] List<AuditoriaProductoStock> productos)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var personaCreacionAuditoria = _userRepository.GetUserNameOrDefault(_userManager.GetUserId(HttpContext.User));

            var productosArray = productos.Select(p => new
            {
                idproducto = p.IdProducto,
                stockingresado = p.StockIngresado,
                id_producto_inventario = p.IdProductoInventario,
                codigo_referencia = p.CodigoReferencia,
                nombre_producto = p.NombreProducto,
                fecha_vencimiento = p.FechaVencimiento,
                lote_producto = p.LoteProducto,
                fecha_recepcion_lote = p.FechaRecepcionLote,
                precio_costo = p.PrecioCosto
            }).ToArray();

            _auditoriaRepository.AddAuditoriaSp(productosArray, userId, personaCreacionAuditoria, false);

            return Ok();
        }
    }
}