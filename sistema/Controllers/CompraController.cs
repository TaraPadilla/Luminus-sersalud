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
using Database.Shared.Enumeraciones;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using farmamest.Models;
using System.Text.Json.Serialization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using sistema.Service.IService;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Globalization;

namespace sistema.Controllers
{
    [Authorize]
    public class CompraController : Controller
    {
        //Configuraciones
        private readonly IConfiguracionSistema _configuracionesSistema = null;
        //Repositorios
        private readonly ICompra _compraRepository = null;
        private readonly ISucursal _sucursalRepository = null;
        private readonly IBodega _bodegasRepository = null;
        private readonly IProveedor _proveedorRepository = null;
        private readonly IProducto _productoRepository = null;
        private readonly ICaja _cajaRepository = null;
        private readonly IEmpleado _empleadoRepository = null;
        private readonly IPrecios _preciosRepository = null;
        private readonly IGeneratePdf _generatePdf = null;
        private readonly IDespegablesProducto _categoriaRepository = null;
        private readonly UserManager<User> _userManager = null;
        private readonly IUser _userRepository = null;
        private readonly ITipoCompra _tipoCompraRepository = null;
        //Servicios
        private readonly IComprasService _comprasService = null;

        public CompraController
            (
            //Configuraciones
            IConfiguracionSistema configuracionesSistema,
            //Repositorios
            ICompra compraRepository,
            ISucursal sucursalRepository,
            IBodega bodegasRepository,
            IProveedor proveedorRepository,
            IProducto productoRepository,
            ICaja cajaRepository,
            IPrecios preciosRepository,
            IEmpleado empleadoRepository,
            IGeneratePdf generatepdf,
            IDespegablesProducto categoriaRepository,
             IUser userRepository,
            UserManager<User> userManager,
            ITipoCompra tipoCompraRepository,
            //Servicios
            IComprasService comprasService
            )

        {
            //Configuraciones
            _configuracionesSistema = configuracionesSistema;
            //Repositorios
            _compraRepository = compraRepository;
            _sucursalRepository = sucursalRepository;
            _bodegasRepository = bodegasRepository;
            _proveedorRepository = proveedorRepository;
            _productoRepository = productoRepository;
            _cajaRepository = cajaRepository;
            _preciosRepository = preciosRepository;
            _empleadoRepository = empleadoRepository;
            _generatePdf = generatepdf;
            _categoriaRepository = categoriaRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _tipoCompraRepository = tipoCompraRepository;
            //Servicios
            _comprasService = comprasService;
        }
        private List<ProductoEquivalenciaViewModel> CreateProductoEquivalenciaViewModels(IEnumerable<ProductoEquivalencia> productoEquivalencias)
        {
            var source = productoEquivalencias ?? Enumerable.Empty<ProductoEquivalencia>();
            return source.Select(prodEquivalencia => new ProductoEquivalenciaViewModel
            {
                Id = prodEquivalencia.Id,
                CantidadEquivalente = prodEquivalencia.CantidadEquivalenteDestino,
                ProductoId = prodEquivalencia.ProductoId,
                UnidadMedidaCompraId = prodEquivalencia.UnidadMedidaCompra?.Id ?? 0,
                UnidadMedidaCompraNombre = prodEquivalencia.UnidadMedidaCompra?.Nombre ?? "-",
                UnidadMedidaVentaId = prodEquivalencia.UnidadMedidaVenta?.Id ?? 0,
                UnidadMedidaVentaNombre = prodEquivalencia.UnidadMedidaVenta?.Nombre ?? "-"
            }).ToList();
        }
        private UltimaCompraProductoViewModel CreateUltimaCompraProductoViewModel(ProductoInventario item, DetalleCompra compra, List<ProductoEquivalenciaViewModel> productoEquivalenciaViewModels)
        {
            return new UltimaCompraProductoViewModel
            {
                ProductoId = item.ProductoId,
                CodigoReferencia = item.Producto.CodigoReferencia,
                NombreProducto = item.Producto.NombreProducto,
                UnidadCompra = item.UnidadMedidaCompra?.Nombre ?? "-",
                UnidadVenta = item.UnidadMedidaVenta?.Nombre ?? "-",
                Precio = compra?.Precio ?? 0,
                ProveedorId = compra?.Compra.ProveedorId ?? 0,
                TipoCompra = compra?.Compra.TipoCompraId ?? 0,
                ProductoEquivalencias = productoEquivalenciaViewModels
            };
        }
        [HttpGet]
        public string ObtenerProductos(int? tipoCompraId, int? proveedorId)
        {
            var productosStockBajo = _productoRepository.GetProductosInventarioStockBajo();
            List<UltimaCompraProductoViewModel> listUltimaCompra = new List<UltimaCompraProductoViewModel>();
            if (productosStockBajo != null)
            {
                foreach (var productoStockBajo in productosStockBajo)
                {
                    var productoId = productoStockBajo.ProductoId;
                    DetalleCompra compra = null;
                    if (productoStockBajo.Compra != null
                        && productoStockBajo.Compra.DetalleCompras != null
                        && productoStockBajo.Compra.DetalleCompras.Count > 0)
                    {
                        compra = productoStockBajo.Compra.DetalleCompras
                            .Where(a => a.ProductoId == productoId)
                            .FirstOrDefault();
                    }
                    if (compra?.Compra != null
                        && (proveedorId == null || compra.Compra.ProveedorId == proveedorId)
                        && (tipoCompraId == null || compra.Compra.TipoCompraId == tipoCompraId))
                    {
                        List<ProductoEquivalenciaViewModel> productoEquivalenciaViewModels =
                            CreateProductoEquivalenciaViewModels(productoStockBajo.Producto.ProductoEquivalencias);
                        listUltimaCompra.Add(CreateUltimaCompraProductoViewModel(productoStockBajo, compra, productoEquivalenciaViewModels));
                    }
                }
            }
            return JsonSerializer.Serialize(new
            {
                Exitoso = true,
                Resultado = listUltimaCompra
            }, options: new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true // Esto es opcional, solo hace que el JSON sea más fácil de leer
            });
        }

        [HttpGet]
        public IActionResult Nuevo(int? ambienteId, int? tipoDocumentoId, int? TipoCompraId, int? ProveedorId)
        {
            var proveedores = _proveedorRepository.GetList();
            var tipoCompra = _tipoCompraRepository.GetList();

            var userName = _userManager.GetUserName(HttpContext.User);
            var user = _userRepository.Get(userName);
            if (user == null)
            {
                TempData["Message"] = "No se pudo identificar el usuario actual.";
                return RedirectToAction("Index", "Home");
            }

            // Inicializar lista de productos
            List<ProductoCompraViewModel> productos = new List<ProductoCompraViewModel>();
            if (TempData["Productos"] != null)
            {
                productos = System.Text.Json.JsonSerializer.Deserialize<List<ProductoCompraViewModel>>(TempData["Productos"].ToString());
                TempData.Keep("Productos");
            }

            // Recuperar diccionario de proveedores principales por producto
            Dictionary<string, string> proveedorPrincipalPorItem = new();
            if (TempData["ProveedorPrincipalPorItem"] != null)
            {
                proveedorPrincipalPorItem = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(TempData["ProveedorPrincipalPorItem"].ToString());
                TempData.Keep("ProveedorPrincipalPorItem");
            }

            // Asignar proveedor principal a cada producto
            foreach (var p in productos)
            {
                var key = p.ProductoId.ToString();
                if (proveedorPrincipalPorItem.TryGetValue(key, out var proveedor))
                {
                    p.ProveedorPrincipal = proveedor;
                }
            }

            // Proveedores disponibles para selección (sin marcar uno como preseleccionado)
            ViewBag.Proveedor = new SelectList(proveedores, "Id", "Nombre");
            ViewBag.TipoCompra = new SelectList(tipoCompra, "Id", "Tipo");

            // Armar el modelo final
            var modelo = new CompraBaseViewModel
            {
                CompraAmbienteId = ambienteId,
                ConfigProrrateoHabilitado = _configuracionesSistema.GetProrrateoHabilitado(),
                EncabezadoAmbienteId = ambienteId,
                AmbienteBloqueado = ambienteId != null,
                EncabezadoTipoDocumentoId = tipoDocumentoId,
                TipoDocumentoBloqueado = tipoDocumentoId != null,
                EncabezadoEmpleadoId = user.EmpleadoId ?? 1,
                Productos = productos
            };

            if (modelo.ConfigProrrateoHabilitado)
            {
                modelo.ConfigProrrateoHabilitado =
                    tipoDocumentoId != null
                    && tipoDocumentoId == (int)CompraTipoDocumentoEnum.Compra;
            }

            modelo.Init(_proveedorRepository, _compraRepository, _sucursalRepository);

            return View(modelo);
        }

        [HttpPost]
        public IActionResult RecibirProductos([FromBody] CompraBaseViewModel model)
        {
            if (model == null || model.Productos == null || !model.Productos.Any())
                return BadRequest("No se recibieron productos");

            if (model.ProveedorPrincipalPorItem == null || !model.ProveedorPrincipalPorItem.Any())
                return BadRequest("No se recibieron proveedores principales por producto.");

            TempData["Productos"] = System.Text.Json.JsonSerializer.Serialize(model.Productos);
            TempData["ProveedorPrincipalPorItem"] = System.Text.Json.JsonSerializer.Serialize(model.ProveedorPrincipalPorItem);

            return RedirectToAction("Nuevo", new
            {
                ambienteId = model.CompraAmbienteId ?? model.EncabezadoAmbienteId ?? (int)AmbienteEnum.Bodega,
                tipoDocumentoId = model.EncabezadoTipoDocumentoId ?? (int)CompraTipoDocumentoEnum.Compra,
                TipoCompraId = model.EncabezadoTipoCompraId > 0 ? model.EncabezadoTipoCompraId : (int?)null,
                ProveedorId = (int?)null
            });
        }


        [HttpPost]
        public string ConsultarProductosRegistrados(int? laboratorioId, int? ambienteId = null)
        {
            try
            {
                // ambienteId = 6;
                var listaProductosRegistrados = new List<object>();
                var productos = new List<Producto>();
                if (laboratorioId == null)
                    productos = _productoRepository.GetProductos(ambienteId);
                else
                    productos = (List<Producto>)_productoRepository.GetProductosLaboratorio((int)laboratorioId);

                foreach (var producto in productos)
                {
                    var ambiente = producto.Ambiente ?? new Ambiente();
                    var productoRegistrado = new
                    {
                        Id = producto.Id,
                        ProductoCodigo = producto.CodigoReferencia,
                        NombreProducto = producto.NombreProducto,
                        NombreTipoProductoTipoBodega = producto.NombreProducto
                        + " - Activo y concentracion: "
                        + producto.ActivoYConcentracion
                        + " ("
                        + (producto.TipoProducto?.NombreTipoProducto ?? "-")
                        + " - "
                        + ambiente.NombreAmbiente
                        + ")"
                    };
                    listaProductosRegistrados.Add(productoRegistrado);
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    ProductosRegistrados = listaProductosRegistrados
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos. " + ex.Message
                });
            }
        }
        private Tuple<string, string> ConsultarProveedores(int IdProducto)
        {
            var data = (_compraRepository.GetDetalleOrdenCompraByIdProducto(IdProducto))
                .Where(x => (x.OrdenCompra.Fecha - DateTime.Now).TotalDays <= 30).ToList();
            string minPrecio = "-";
            string minCredito = "-";
            if (data != null && data.Count != 0)
            {
                minPrecio = (data.OrderBy(x => x.Precio).FirstOrDefault()).OrdenCompra.Proveedor.Nombre;

                minCredito = ((data.Select(x => x.OrdenCompra))
                    .Where(x => x.TipoCompraId == 2)).OrderBy(x => x.DiasCredito).FirstOrDefault() == null ? "-" :
                    ((data.Select(x => x.OrdenCompra))
                    .Where(x => x.TipoCompraId == 2)).OrderBy(x => x.DiasCredito).FirstOrDefault().Proveedor.Nombre;
            }
            return Tuple.Create(minPrecio, minCredito);
        }
        [HttpPost]
        public string ConsultarUnidadesCompra(int productoId)
        {
            try
            {
                var equivalencias = _productoRepository.GetEquivalenciasProducto(productoId);

                List<object> result = new List<object>();

                foreach (var item in equivalencias)
                {
                    result.Add(new
                    {
                        Id = item.Id,
                        UnidadMedidaCompraId = item.UnidadMedidaCompraId,
                        UnidadMedidaCompra = item.UnidadMedidaCompra.Nombre,
                        UnidadMedidaVentaId = item.UnidadMedidaVentaId,
                        UnidadMedidaVenta = item.UnidadMedidaVenta.Nombre,
                        CantidadEquivalente = item.CantidadEquivalenteDestino,
                        Abreviatura = item.UnidadMedidaCompra.Abreviatura
                    });
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = result
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Resultado = "Error en el servidor " + ex.Message
                });
            }

        }
        [HttpPost]
        public string ConsultarUnidadesUbicaciones(int productoId)
        {
            try
            {
                var equivalencias = _productoRepository.GetEquivalenciasProducto(productoId);

                List<object> result = new List<object>();

                foreach (var item in equivalencias)
                {
                    result.Add(new
                    {
                        Id = item.Id,
                        UnidadMedidaCompraId = item.UnidadMedidaCompraId,
                        UnidadMedidaCompra = item.UnidadMedidaCompra.Nombre,
                        Abreviatura = item.UnidadMedidaCompra.Abreviatura
                    });
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = result
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Resultado = "Error en el servidor " + ex.Message
                });
            }

        }
        [HttpPost]
        public string ConsultarUnidadesCompraProducto(int productoId, int? bodegaId)
        {
            try
            {
                var proveedores = ConsultarProveedores(productoId);

                //meter lo proveedores en este metodo
                var listaUnidades = new List<object>();
                var equivalencias = _productoRepository.GetEquivalenciasProducto(productoId);
                var idsUnidadesCompra = equivalencias
                    .Select(a => a.UnidadMedidaCompraId)
                    .Distinct().ToList();

                var getActualProductoSeleccionado = _productoRepository.GetProductoByIdAndBodegaId(productoId, bodegaId); //busco si un producto con determinada bodega cuenta con stock 
                var stockActualProductoSeleccionado = getActualProductoSeleccionado != null ? getActualProductoSeleccionado.Stock : 0;//asigno el valor del stock actual de lo contrario le envio 0

                var ultimoPrecio = _compraRepository.GetUltimoPrecioCompraProducto(productoId) == null ? (decimal)0 : _compraRepository.GetUltimoPrecioCompraProducto(productoId).Precio;

                foreach (int? id in idsUnidadesCompra)
                {
                    var equivalencia = equivalencias
                        .Where(a => a.UnidadMedidaCompraId == id)
                        .FirstOrDefault();
                    if (equivalencia?.UnidadMedidaCompra == null)
                        continue;
                    var unidadBd = new UnidadMedidaCompraViewModel
                    {
                        Id = id,
                        NombreUnidad = equivalencia.UnidadMedidaCompra.Nombre ?? "-",
                        Abreviatura = equivalencia.UnidadMedidaCompra.Abreviatura ?? "-"
                    };
                    listaUnidades.Add(unidadBd);
                }

                #region LOTES EXISTENTES
                var lotesProducto = new List<ProductoLoteExistenteVM>();
                var productosInventarioBd = _productoRepository.GetLotesProducto(productoId);
                if (productosInventarioBd != null)
                {
                    foreach (var productoInventario in productosInventarioBd)
                    {
                        var bodega = productoInventario.Bodega ?? new Bodega();
                        var compra = productoInventario.Compra ?? new Compra();
                        var proveedor = compra.Proveedor ?? new Proveedor();
                        var fechaVencimiento = "-";
                        if (productoInventario.FechaVencimientoArticuloCompra != null)
                            fechaVencimiento = ((DateTime)productoInventario.FechaVencimientoArticuloCompra).ToString("dd-MM-yyyy");
                        var fechaRecepcionLote = "-";
                        if (productoInventario.FechaRecepcionLote != null)
                            fechaRecepcionLote = ((DateTime)productoInventario.FechaRecepcionLote).ToString("dd-MM-yyyy");
                        //Precio de compra
                        decimal precioCompra = 0;
                        if (compra.DetalleCompras != null)
                        {
                            var registroCompra = compra.DetalleCompras
                                .Where(a => a.ProductoId == productoInventario.ProductoId)
                                .FirstOrDefault();
                            if (registroCompra != null)
                                precioCompra = registroCompra.Precio;
                        }
                        lotesProducto.Add(new ProductoLoteExistenteVM
                        {
                            ProductoId = productoInventario.ProductoId,
                            FechaVencimiento = fechaVencimiento,
                            Stock = productoInventario.Stock,
                            PrecioCompra = precioCompra,
                            ProveedorNombre = proveedor.Nombre,
                            Lote = productoInventario.Lote,
                            FechaRecepcionLote = fechaRecepcionLote,
                            BodegaNombre = bodega.NombreBodega
                        });
                    }
                }
                #endregion

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    UnidadesCompra = listaUnidades,
                    stock = stockActualProductoSeleccionado, //envio esta variable con el stock del producto si cuenta con uno , de  lo contrario es 0
                    ProveedorSugeridoPrecio = proveedores.Item1,
                    ProveedorSugeridoCredito = proveedores.Item2,
                    UltimoPrecioCompra = ultimoPrecio,
                    LotesExistentes = lotesProducto
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar unidades. " + ex.Message
                });
            }
        }
        private List<object> MapPreciosProducto(List<ProductoInventarioPrecio> list)
        {
            var ListaObjeto = new List<object>();
            if (list != null)
            {
                foreach (var item in list)
                {

                    ListaObjeto.Add(new
                    {
                        IdProductoInventario = item.ProductoInventarioId,
                        PrecioId = item.PrecioId,
                        Valor = item.Valor
                    });

                }
            }
            return ListaObjeto;
        }
        [HttpPost]
        public string ConsultarUnidadesVentaProducto(int productoId, int unidadCompraId, int bodegaId)
        {
            try
            {
                var listaUnidades = new List<object>();
                var equivalencias = _productoRepository.GetEquivalenciasProducto(productoId)
                    .Where(p => p.UnidadMedidaCompraId == unidadCompraId);
                var producto = _productoRepository.Get(productoId);


                var productoByIdAndBodegaId = _productoRepository.GetProductoByIdAndBodegaId(productoId, bodegaId);
                var preciosProducto = productoByIdAndBodegaId?.ProductosInventarioPrecios?.Where(x => !x.Precio.Eliminado).ToList();


                foreach (var equivalencia in equivalencias)
                {
                    var unidadBd = new UnidadMedidaVentaCompraViewModel
                    {
                        Id = (int)equivalencia.UnidadMedidaVentaId,
                        NombreUnidad = $"{equivalencia.UnidadMedidaVenta.Nombre} ({equivalencia.UnidadMedidaVenta.Abreviatura})",
                        Equivalencia = $"1 {equivalencia.UnidadMedidaCompra.Nombre} =" +
                        $" {equivalencia.CantidadEquivalenteDestino} {equivalencia.UnidadMedidaVenta.Nombre}",
                        CantidadEquivalenteDestino = equivalencia.CantidadEquivalenteDestino
                    };
                    listaUnidades.Add(unidadBd);
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    UnidadesVenta = listaUnidades,
                    ProductoPrecios = MapPreciosProducto(preciosProducto)
                }/*,options: options*/);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar unidades. " + ex.Message
                });
            }
        }
        public class ProductoPrecioSolicitudDto
        {
            public int ProductoId { get; set; }
        }

        [HttpPost]
        public JsonResult ObtenerUltimosPreciosPorProveedor(string proveedor, [FromBody] List<ProductoPrecioSolicitudDto> productos)
        {
            try
            {
                var precios = new Dictionary<string, decimal>();
                if (string.IsNullOrWhiteSpace(proveedor) || productos == null || productos.Count == 0)
                    return Json(new { Exitoso = true, Precios = precios });

                var proveedorBd = _proveedorRepository.GetProveedorPorNombre(proveedor.Trim());
                foreach (var item in productos.Where(p => p.ProductoId > 0))
                {
                    var detalles = _compraRepository.GetDetalleOrdenCompraByIdProducto(item.ProductoId) ?? new List<DetalleOrdenCompra>();
                    var ultimo = detalles
                        .Where(d => d.OrdenCompra != null
                                    && (proveedorBd == null || d.OrdenCompra.ProveedorId == proveedorBd.Id))
                        .OrderByDescending(d => d.OrdenCompra.Fecha)
                        .FirstOrDefault();

                    if (ultimo != null)
                        precios[item.ProductoId.ToString()] = ultimo.Precio;
                }

                return Json(new { Exitoso = true, Precios = precios });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al consultar precios del proveedor. " + ex.Message });
            }
        }

        [HttpPost]
        public string ConsultarInfoProveedor(string proveedorNombre)
        {
            try
            {
                //int? politicaDevolucionDias = null;
                //int? creditoDias = null;
                var infoProveedor = new ProveedorBaseViewModel
                {
                    ProveedorNombre = "-",
                    ProveedorPoliticasDevolucion = null,
                    ProveedorDiasCredito = null
                };
                if (proveedorNombre != null)
                {
                    var proveedor = _proveedorRepository.GetProveedorPorNombre(proveedorNombre);
                    if (proveedor != null)
                    {
                        infoProveedor.ProveedorNombre = proveedor.Nombre;
                        infoProveedor.ProveedorPoliticasDevolucion = proveedor.PoliticasDevolucion;
                        infoProveedor.ProveedorDiasCredito = proveedor.DiasCredito;
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = infoProveedor
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar informacion de proveedor. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarPreciosExistentes()
        {
            try
            {
                var listaPrecios = new List<CompraPrecioExistenteViewModel>();
                var preciosBd = _preciosRepository.GetList();
                if (preciosBd != null)
                {
                    foreach (var precio in preciosBd)
                    {
                        listaPrecios.Add(new CompraPrecioExistenteViewModel
                        {
                            PrecioId = precio.Id,
                            PrecioNombre = precio.NombrePrecio
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaPrecios
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar precios. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarLaboratorios()
        {
            try
            {
                var listaLaboratorios = new List<object>();
                var laboratorios = _categoriaRepository.ListaLaboratorioProducto();
                foreach (var laboratorio in laboratorios)
                {
                    var laboratorioBd = new
                    {
                        Id = laboratorio.Id,
                        NombreLaboratorioProducto = laboratorio.NombreLaboratorioProducto
                    };
                    listaLaboratorios.Add(laboratorioBd);
                }


                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Laboratorios = listaLaboratorios
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar laboratorios. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarBodegasExistentes()
        {
            try
            {
                var listaBodegas = new List<CompraBodegaExistenteViewModel>();
                var bodegasBd = _bodegasRepository.GetList();
                if (bodegasBd != null)
                {
                    foreach (var bodega in bodegasBd)
                    {
                        listaBodegas.Add(new CompraBodegaExistenteViewModel
                        {
                            BodegaId = bodega.Id,
                            SucursalId = bodega.SucursalId,
                            AmbienteId = bodega.AmbienteId,
                            BodegaNombre = bodega.NombreBodega
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaBodegas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar precios. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarSucursalesExistentes()
        {
            try
            {
                var listaSucursales = new List<CompraSucursalExistenteViewModel>();
                var sucursalesBd = _sucursalRepository.GetList();
                if (sucursalesBd != null)
                {
                    foreach (var sucursal in sucursalesBd)
                    {
                        listaSucursales.Add(new CompraSucursalExistenteViewModel
                        {
                            SucursalId = sucursal.Id,
                            SucursalNombre = sucursal.NombreSucursal
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaSucursales
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar sucursales. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string GuardarCompra([FromBody] CompraBaseViewModel model)
        {
            try
            {
                var fechaHora = DateTime.Now;

                #region Proveedor

                Proveedor proveedor;
                var proveedorExistente = _proveedorRepository.GetProveedorPorNombre(model.EncabezadoProveedor);

                if (proveedorExistente == null)
                {
                    Console.WriteLine($"⚠️ Proveedor '{model.EncabezadoProveedor}' no encontrado, se creará uno nuevo.");

                    if (string.IsNullOrWhiteSpace(model.EncabezadoProveedor))
                    {
                        Console.WriteLine("❌ ERROR: El proveedor no tiene nombre.");
                        return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "El proveedor no tiene nombre." });
                    }

                    proveedor = new Proveedor()
                    {
                        Nombre = model.EncabezadoProveedor,
                        BancoId = 1
                    };
                }
                else
                {
                    proveedor = proveedorExistente;
                }

                #endregion

                var ordenCompraRecibida = model.EncabezadoTipoDocumentoId != null
                                          && (int)model.EncabezadoTipoDocumentoId != (int)CompraTipoDocumentoEnum.OrdenCompra;

                DateTime? fechaLimite = null;
                if (!string.IsNullOrWhiteSpace(model.EncabezadoFechaLimite))
                {
                    fechaLimite = Convert.ToDateTime(model.EncabezadoFechaLimite);
                }

                DateTime? fechaRecepcionFactura = null;
                if (!string.IsNullOrWhiteSpace(model.EncabezadoFechaRecepcion))
                {
                    fechaRecepcionFactura = Convert.ToDateTime(model.EncabezadoFechaRecepcion);
                }

                var nuevaCompra = new Compra()
                {
                    Proveedor = proveedor,
                    TipoBodegaId = model.EncabezadoTipoBodegaId,
                    DiasCredito = model.DiasCredito,
                    NoComprobante = model.EncabezadoNoComprobante,
                    SucursalId = model.EncabezadoSucursalId,
                    EmpleadoId = model.EncabezadoEmpleadoId,
                    FechaLimite = fechaLimite,
                    FechaCompra = fechaHora,
                    Observaciones = model.EncabezadoObservacion,
                    // Estos dos siguen fijos por ahora, lo vemos en un paso aparte
                    TipoCompraId = model.EncabezadoTipoCompraId > 0 ? model.EncabezadoTipoCompraId : 1,
                    CompraTipoDocumentoId = model.EncabezadoTipoDocumentoId ?? (int)CompraTipoDocumentoEnum.OrdenCompra,
                    OrdenCompraRecibida = ordenCompraRecibida,
                    FechaRecepcion = fechaRecepcionFactura,
                    DetalleCompras = new List<DetalleCompra>()
                };

                Console.WriteLine($"🛒 Creando detalles de compra... {model.ProductosComprados?.Count} productos recibidos");

                if (model.ProductosComprados != null)
                {
                    foreach (var item in model.ProductosComprados)
                    {
                        Console.WriteLine($"🔹 Procesando producto ID: {item.ProductoId}, Cantidad: {item.Cantidad}, Precio: {item.PrecioCompra}");

                        if (item.Nuevo)
                        {
                            DateTime? fechaVencimiento = null;
                            if (!string.IsNullOrWhiteSpace(item.FechaVencimiento))
                            {
                                fechaVencimiento = Convert.ToDateTime(item.FechaVencimiento);
                            }

                            var detalleCompra = new DetalleCompra()
                            {
                                ProductoId = Convert.ToInt32(item.ProductoId),
                                // Asegurar ID 1 por defecto
                                UnidadMedidaCompraId = item.UnidadMedidaCompraId > 0 ? item.UnidadMedidaCompraId : 1,
                                Cantidad = Convert.ToInt32(item.Cantidad),
                                Lote = item.Lote,
                                FechaVencimiento = fechaVencimiento,
                                Precio = Convert.ToDecimal(item.PrecioCompra),
                                PrecioTotal = Convert.ToDecimal(item.Cantidad * item.PrecioCompra),
                                DetalleComprasUbicaciones = new List<DetalleCompraUbicacion>()
                            };

                            // ===========================
                            // POLÍTICA DEVOLUCIÓN (DEFECTUOSO)
                            // ===========================
                            detalleCompra.ManejaPoliticaDevolucion = false;
                            detalleCompra.ManejaPoliticaDevolucionProveedor = false;
                            detalleCompra.ManejaPoliticaDevolucionPersonalizada = false;
                            detalleCompra.PoliticaDevolucionPersonalizadaDias = null;

                            if (!string.IsNullOrWhiteSpace(item.PoliticaDevolucionProducto))
                            {
                                switch (item.PoliticaDevolucionProducto)
                                {
                                    case "radio-politica-proveedor":
                                        detalleCompra.ManejaPoliticaDevolucion = true;
                                        detalleCompra.ManejaPoliticaDevolucionProveedor = true;
                                        break;

                                    case "radio-politica-personalizada":
                                        detalleCompra.ManejaPoliticaDevolucion = true;
                                        detalleCompra.ManejaPoliticaDevolucionPersonalizada = true;
                                        detalleCompra.PoliticaDevolucionPersonalizadaDias =
                                            item.PoliticaDevolucionPersonalizadaDias;
                                        break;

                                    // "radio-politica-no-maneja" u otro valor:
                                    default:
                                        // se deja todo en false / null
                                        break;
                                }
                            }

                            // ===========================
                            // POLÍTICA DEVOLUCIÓN (VENCIMIENTO)
                            // ===========================
                            detalleCompra.ManejaPoliticaDevolucionVencimiento = false;
                            detalleCompra.ManejaPoliticaDevolucionVencimientoProveedor = false;
                            detalleCompra.ManejaPoliticaDevolucionVencimientoPersonalizada = false;
                            detalleCompra.PoliticaDevolucionVencimientoPersonalizadaDias = null;

                            if (!string.IsNullOrWhiteSpace(item.PoliticaDevolucionVencimientoProducto))
                            {
                                switch (item.PoliticaDevolucionVencimientoProducto)
                                {
                                    case "radio-politica-vencimiento-proveedor":
                                        detalleCompra.ManejaPoliticaDevolucionVencimiento = true;
                                        detalleCompra.ManejaPoliticaDevolucionVencimientoProveedor = true;
                                        break;

                                    case "radio-politica-vencimiento-personalizada":
                                        detalleCompra.ManejaPoliticaDevolucionVencimiento = true;
                                        detalleCompra.ManejaPoliticaDevolucionVencimientoPersonalizada = true;
                                        detalleCompra.PoliticaDevolucionVencimientoPersonalizadaDias =
                                            item.PoliticaDevolucionVencimientoPersonalizadaDias;
                                        break;

                                    // "radio-politica-vencimiento-no-maneja" u otro valor:
                                    default:
                                        // se deja todo en false / null
                                        break;
                                }
                            }

                            // ===========================
                            // CRÉDITO
                            // ===========================
                            detalleCompra.ManejaCredito = false;
                            detalleCompra.ManejaCreditoProveedor = false;
                            detalleCompra.ManejaCreditoPersonalizado = false;
                            detalleCompra.CreditoPersonalizadoDias = null;

                            if (!string.IsNullOrWhiteSpace(item.CreditoProducto))
                            {
                                switch (item.CreditoProducto)
                                {
                                    case "radio-credito-proveedor":
                                        detalleCompra.ManejaCredito = true;
                                        detalleCompra.ManejaCreditoProveedor = true;
                                        break;

                                    case "radio-credito-personalizado":
                                        detalleCompra.ManejaCredito = true;
                                        detalleCompra.ManejaCreditoPersonalizado = true;
                                        detalleCompra.CreditoPersonalizadoDias = item.CreditoPersonalizadoDias;
                                        break;

                                    // "radio-credito-no-maneja" u otro valor:
                                    default:
                                        // se deja todo en false / null
                                        break;
                                }
                            }

                            Console.WriteLine($"✅ Detalle creado para producto {item.ProductoId} con total: {detalleCompra.PrecioTotal}");

                            nuevaCompra.DetalleCompras.Add(detalleCompra);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ No se encontraron productos en la compra.");
                }

                Console.WriteLine("💾 Guardando compra en la base de datos...");
                _compraRepository.Add(nuevaCompra);

                if (model.EncabezadoTipoDocumentoId != null &&
                    (int)model.EncabezadoTipoDocumentoId == (int)CompraTipoDocumentoEnum.Compra)
                {
                    Console.WriteLine("📦 Agregando productos a inventario...");
                    _comprasService.AgregarProductosAInventario(
                        nuevaCompra,
                        _userManager.GetUserId(HttpContext.User));
                }

                #region Caja

                if (model.EncabezadoTipoDocumentoId != null &&
                    (int)model.EncabezadoTipoDocumentoId == (int)CompraTipoDocumentoEnum.Compra)
                {
                    Console.WriteLine("🔍 Buscando caja abierta...");
                    var cajaAbierta = _cajaRepository.ListarCajas()
                        .FirstOrDefault(a => a.SucursalId == model.EncabezadoSucursalId
                                          && a.AmbienteId == model.EncabezadoAmbienteId
                                          && a.EstadoCaja);

                    if (cajaAbierta == null)
                    {
                        Console.WriteLine("❌ ERROR: No hay cajas abiertas.");
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = "No hay cajas abiertas para esta sucursal ni este ambiente"
                        });
                    }

                    var detalleCaja = new DetalleCaja
                    {
                        CompraId = nuevaCompra.Id,
                        CajaId = cajaAbierta.Id,
                        Descripcion = "Registro de compra",
                        Gasto = model.ValorTotalCompra,
                        Fecha = fechaHora
                    };

                    Console.WriteLine($"✅ Registrando gasto en caja: {detalleCaja.Gasto}");
                    _cajaRepository.Add(detalleCaja);
                }

                #endregion

                Console.WriteLine("✅ Compra guardada exitosamente.");

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    IsOrden = model.EncabezadoTipoDocumentoId == (int)CompraTipoDocumentoEnum.OrdenCompra,
                    IsCompra = model.EncabezadoTipoDocumentoId == (int)CompraTipoDocumentoEnum.Compra
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR en GuardarCompra: {ex.Message}");
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Ha ocurrido un error de servidor al registrar compra. " + ex.Message
                });
            }
        }

        public IActionResult ListaPeticiones(string sortOrder, string buscar, string currentFilter, int? pageNumber,
     string fechaInicial, string fechaFinal, int? comprobante, string proveedor, string vendedor)
        {
            ViewData["CurrentSort"] = sortOrder;
            // ViewData["ApellidoSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Apellido_desc" : "";
            // ViewData["NombreSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Nombre_desc" : "";

            if (buscar != null)
            {
                pageNumber = 1;
            }
            else
            {
                buscar = currentFilter;
            }

            var proveedorList = _compraRepository.GetAll().Select(x => x.Proveedor == null ? null : x.Proveedor.Nombre).Distinct().ToList();
            proveedorList.RemoveAll(x => x == null);

            var provList = proveedorList.Select(x => new
            {
                Nombre = x
            });

            ViewBag.Proveedor = new SelectList(provList, "Nombre", "Nombre");

            var vendedorList = (_compraRepository.GetAll()).Select(x => x.Empleado == null ? null : x.Empleado.Nombre).Distinct().ToList();
            vendedorList.RemoveAll(x => x == null);

            var vendList = vendedorList.Select(x => new
            {
                Nombre = x
            });
            ViewBag.Vendedor = new SelectList(vendList, "Nombre", "Nombre");

            ViewData["CurrentFilter"] = buscar;

            var lista = _compraRepository.PaginacionOrdenesCompra(sortOrder, buscar, pageNumber, 10, fechaInicial, fechaFinal, comprobante, proveedor, vendedor);
            //var lista = _compraRepository.PaginacionComprasPeticion(sortOrder, buscar, pageNumber, 10);
            return View(lista);
        }

        [HttpPost]
        public JsonResult ListaComprados()
        {
            try
            {
                var data = _compraRepository.ListaComprados();
                data = data.OrderByDescending(x => x.Id).ToList();
                List<object> resultado = new List<object>();

                foreach (var item in data)
                {
                    resultado.Add(new
                    {
                        Id = item.Id,
                        FechaCompra = item.FechaCompra.AddDays(item.Proveedor.DiasCredito).ToString("dd/MM/yyyy"),
                        NombreProveedor = item.Proveedor.Nombre,
                        NoComprobante = item.NoComprobante,
                        Empleado = item.Empleado == null ? "Admin" : item.Empleado.Nombre,
                        FechaRecepcion = item.FechaRecepcion?.ToString("dd/MM/yyyy") ?? "-",
                        FechaLimite = item.FechaLimite?.ToString("dd/MM/yyyy") ?? "-",
                        PrecioTotal = item.DetalleCompras.Sum(a => a.PrecioTotal)
                    });
                }

                return Json(new
                {
                    Exitoso = true,
                    Resultado = resultado
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar la tabla de proveedores. " + ex.Message
                });
            }
        }
        public IActionResult ListaComprados(string sortOrder, string buscar, string currentFilter, int? pageNumber,
            string fechaInicial, string fechaFinal, string comprobante, string proveedor, string vendedor,
            int numeroCompra)
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

            var lista = _compraRepository.PaginacionCompras(sortOrder, buscar, pageNumber, 10, fechaInicial, fechaFinal, comprobante,
                proveedor, vendedor, numeroCompra);

            var proveedorList = (lista.Select(x => x.Proveedor == null ? null : x.Proveedor.Nombre).Distinct().ToList());
            proveedorList.RemoveAll(x => x == null);

            var provList = proveedorList.Select(x => new
            {
                Nombre = x
            });

            ViewBag.Proveedor = new SelectList(provList, "Nombre", "Nombre");


            var vendedorList = (lista.Select(x => x.Empleado == null ? null : x.Empleado.Nombre).Distinct().ToList());
            vendedorList.RemoveAll(x => x == null);

            var vendList = vendedorList.Select(x => new
            {
                Nombre = x
            });
            ViewBag.Vendedor = new SelectList(vendList, "Nombre", "Nombre");

            return View(lista);
        }

        [HttpGet]
        public IActionResult Modificar(int compraId)
        {
            var compra = _compraRepository.Get(compraId);
            if (compra == null) return StatusCode(404);

            // Proveedores para el combo (mismo ViewBag que en Nuevo, pero con seleccionado)
            var proveedores = _proveedorRepository.GetList();
            ViewBag.Proveedor = new SelectList(
                proveedores,
                "Id",
                "Nombre",
                compra.ProveedorId);

            // Tipo de compra para el combo (por si la vista lo usa igual que Nuevo)
            var tipoCompra = _tipoCompraRepository.GetList();
            ViewBag.TipoCompra = new SelectList(
                tipoCompra,
                "Id",
                "Tipo",
                compra.TipoCompraId);

            // Fechas en formato yyyy-MM-dd para <input type="date">
            string fechaLimite = compra.FechaLimite?.ToString("yyyy-MM-dd");
            string fechaRecepcion = compra.FechaRecepcion?.ToString("yyyy-MM-dd");

            // Ambiente: aquí NO lo invento; si Compra no tiene AmbienteId,
            // lo dejamos en null para que JS se comporte igual que antes.
            int? ambienteId = null;

            var modelo = new CompraBaseViewModel
            {
                CompraId = compra.Id,
                CompraAmbienteId = compra.SucursalId,
                EncabezadoAmbienteId = compra.SucursalId,
                AmbienteBloqueado = compra.SucursalId != null,

                EncabezadoNoComprobante = compra.NoComprobante,
                OrdenCompraRecibida = compra.OrdenCompraRecibida,
                EncabezadoEmpleadoId = compra.EmpleadoId,

                EncabezadoFechaLimite = fechaLimite,
                EncabezadoFechaRecepcion = fechaRecepcion,

                EncabezadoProveedor = compra.Proveedor?.Nombre,
                EncabezadoTipoCompraId = compra.TipoCompraId,

                // Usamos el tipo de documento real si existe, si no,
                // dejamos OrdenCompra (como lo tenías).
                EncabezadoTipoDocumentoId = compra.CompraTipoDocumentoId
                                            ?? (int)CompraTipoDocumentoEnum.OrdenCompra,
                TipoDocumentoBloqueado = true,

                EncabezadoObservacion = compra.Observaciones,
                EncabezadoSucursalId = compra.SucursalId,
                DiasCredito = compra.DiasCredito ?? 0,

                // Configuración de prorrateo igual que en Nuevo
                ConfigProrrateoHabilitado = _configuracionesSistema.GetProrrateoHabilitado()
            };

            if (modelo.ConfigProrrateoHabilitado)
            {
                modelo.ConfigProrrateoHabilitado =
                    modelo.EncabezadoTipoDocumentoId == (int)CompraTipoDocumentoEnum.Compra;
            }

            modelo.Init(_proveedorRepository, _compraRepository, _sucursalRepository);

            return View(modelo);
        }


        [HttpPost]
        public string Modificar([FromBody] CompraBaseViewModel model)
        {
            try
            {
                if (model == null || model.CompraId == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Modelo inválido o sin ID de compra."
                    });
                }

                var compra = _compraRepository.Get(model.CompraId.Value);

                if (compra == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "La compra a modificar no existe."
                    });
                }

                Console.WriteLine($"🛠 Modificando compra ID: {compra.Id}");

                #region PROVEEDOR (MISMA LÓGICA QUE GUARDARCOMPRA)

                Proveedor proveedor;
                var proveedorExistente = _proveedorRepository.GetProveedorPorNombre(model.EncabezadoProveedor);

                if (proveedorExistente == null)
                {
                    Console.WriteLine($"⚠️ Proveedor '{model.EncabezadoProveedor}' no encontrado, se creará uno nuevo.");

                    if (string.IsNullOrWhiteSpace(model.EncabezadoProveedor))
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = "El proveedor no tiene nombre."
                        });
                    }

                    proveedor = new Proveedor()
                    {
                        Nombre = model.EncabezadoProveedor,
                        BancoId = 1
                    };
                }
                else
                {
                    proveedor = proveedorExistente;
                }

                #endregion

                var ordenCompraRecibida = model.EncabezadoTipoDocumentoId != null
                                          && (int)model.EncabezadoTipoDocumentoId != (int)CompraTipoDocumentoEnum.OrdenCompra;

                DateTime? fechaLimite = null;
                if (!string.IsNullOrWhiteSpace(model.EncabezadoFechaLimite))
                {
                    fechaLimite = Convert.ToDateTime(model.EncabezadoFechaLimite);
                }

                DateTime? fechaRecepcionFactura = null;
                if (!string.IsNullOrWhiteSpace(model.EncabezadoFechaRecepcion))
                {
                    fechaRecepcionFactura = Convert.ToDateTime(model.EncabezadoFechaRecepcion);
                }



                // ==========================
                // ENCABEZADO: ACTUALIZAR CAMPOS
                // ==========================
                compra.Proveedor = proveedor;
                compra.TipoBodegaId = model.EncabezadoTipoBodegaId;
                compra.DiasCredito = model.DiasCredito;
                compra.NoComprobante = model.EncabezadoNoComprobante;
                compra.SucursalId = model.EncabezadoSucursalId;
                compra.EmpleadoId = model.EncabezadoEmpleadoId;
                compra.FechaLimite = fechaLimite;
                compra.Observaciones = model.EncabezadoObservacion;
                compra.OrdenCompraRecibida = ordenCompraRecibida;
                compra.FechaRecepcion = fechaRecepcionFactura;

                // Si viene un tipo de compra válido, se actualiza
                if (model.EncabezadoTipoCompraId > 0)
                {
                    compra.TipoCompraId = model.EncabezadoTipoCompraId;
                }

                // ==========================
                // DETALLE: REEMPLAZAR TODO
                // ==========================

                if (compra.DetalleCompras == null)
                    compra.DetalleCompras = new List<DetalleCompra>();
                else
                    compra.DetalleCompras.Clear(); // dejamos que EF se encargue de borrar los viejos

                if (model.ProductosComprados != null)
                {
                    Console.WriteLine($"🧾 Productos recibidos en el modelo: {model.ProductosComprados.Count}");

                    foreach (var item in model.ProductosComprados)
                    {
                        Console.WriteLine($"🔹 Producto ID: {item.ProductoId}, Cantidad: {item.Cantidad}, Precio: {item.PrecioCompra}");

                        DateTime? fechaVencimiento = null;
                        if (!string.IsNullOrWhiteSpace(item.FechaVencimiento))
                        {
                            fechaVencimiento = Convert.ToDateTime(item.FechaVencimiento);
                        }

                        var detalleCompra = new DetalleCompra()
                        {
                            ProductoId = Convert.ToInt32(item.ProductoId),
                            UnidadMedidaCompraId = item.UnidadMedidaCompraId > 0 ? item.UnidadMedidaCompraId : 1,
                            Cantidad = Convert.ToInt32(item.Cantidad),
                            Lote = item.Lote,
                            FechaVencimiento = fechaVencimiento,
                            Precio = Convert.ToDecimal(item.PrecioCompra),
                            PrecioTotal = Convert.ToDecimal(item.Cantidad * item.PrecioCompra),
                            DetalleComprasUbicaciones = new List<DetalleCompraUbicacion>()
                        };

                        // ===========================
                        // POLÍTICA DEVOLUCIÓN (DEFECTO)
                        // ===========================
                        detalleCompra.ManejaPoliticaDevolucion = false;
                        detalleCompra.ManejaPoliticaDevolucionProveedor = false;
                        detalleCompra.ManejaPoliticaDevolucionPersonalizada = false;
                        detalleCompra.PoliticaDevolucionPersonalizadaDias = null;

                        if (!string.IsNullOrWhiteSpace(item.PoliticaDevolucionProducto))
                        {
                            switch (item.PoliticaDevolucionProducto)
                            {
                                case "radio-politica-proveedor":
                                    detalleCompra.ManejaPoliticaDevolucion = true;
                                    detalleCompra.ManejaPoliticaDevolucionProveedor = true;
                                    break;

                                case "radio-politica-personalizada":
                                    detalleCompra.ManejaPoliticaDevolucion = true;
                                    detalleCompra.ManejaPoliticaDevolucionPersonalizada = true;
                                    detalleCompra.PoliticaDevolucionPersonalizadaDias =
                                        item.PoliticaDevolucionPersonalizadaDias;
                                    break;
                            }
                        }

                        // ===========================
                        // POLÍTICA DEVOLUCIÓN (VENCIMIENTO)
                        // ===========================
                        detalleCompra.ManejaPoliticaDevolucionVencimiento = false;
                        detalleCompra.ManejaPoliticaDevolucionVencimientoProveedor = false;
                        detalleCompra.ManejaPoliticaDevolucionVencimientoPersonalizada = false;
                        detalleCompra.PoliticaDevolucionVencimientoPersonalizadaDias = null;

                        if (!string.IsNullOrWhiteSpace(item.PoliticaDevolucionVencimientoProducto))
                        {
                            switch (item.PoliticaDevolucionVencimientoProducto)
                            {
                                case "radio-politica-vencimiento-proveedor":
                                    detalleCompra.ManejaPoliticaDevolucionVencimiento = true;
                                    detalleCompra.ManejaPoliticaDevolucionVencimientoProveedor = true;
                                    break;

                                case "radio-politica-vencimiento-personalizada":
                                    detalleCompra.ManejaPoliticaDevolucionVencimiento = true;
                                    detalleCompra.ManejaPoliticaDevolucionVencimientoPersonalizada = true;
                                    detalleCompra.PoliticaDevolucionVencimientoPersonalizadaDias =
                                        item.PoliticaDevolucionVencimientoPersonalizadaDias;
                                    break;
                            }
                        }

                        // ===========================
                        // CRÉDITO
                        // ===========================
                        detalleCompra.ManejaCredito = false;
                        detalleCompra.ManejaCreditoProveedor = false;
                        detalleCompra.ManejaCreditoPersonalizado = false;
                        detalleCompra.CreditoPersonalizadoDias = null;

                        if (!string.IsNullOrWhiteSpace(item.CreditoProducto))
                        {
                            switch (item.CreditoProducto)
                            {
                                case "radio-credito-proveedor":
                                    detalleCompra.ManejaCredito = true;
                                    detalleCompra.ManejaCreditoProveedor = true;
                                    break;

                                case "radio-credito-personalizado":
                                    detalleCompra.ManejaCredito = true;
                                    detalleCompra.ManejaCreditoPersonalizado = true;
                                    detalleCompra.CreditoPersonalizadoDias = item.CreditoPersonalizadoDias;
                                    break;
                            }
                        }

                        compra.DetalleCompras.Add(detalleCompra);
                    }
                }

                Console.WriteLine("💾 Guardando cambios de la compra...");
                _compraRepository.Update(compra);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Orden de compra modificada correctamente."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR en Modificar: {ex.Message}");
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Ha ocurrido un error de servidor al modificar la orden. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarProductosCompra(int compraId)
        {
            try
            {
                var unidadesVenta = new List<CompraUnidadVentaViewModel>();
                var ubicaciones = new List<CompraUbicacionViewModel>();
                var precios = new List<CompraPrecioVentaViewModel>();
                var lotesExistentes = new List<ProductoLoteExistenteVM>();
                var productosComprados = new List<CompraProductoCompradoViewModel>();
                var productosCompraBd = _compraRepository.GetDetalles(compraId);

                int item = 0; // ← contador para la propiedad Item

                if (productosCompraBd != null)
                {
                    foreach (var producto in productosCompraBd)
                    {
                        var productoCompradoVm = new CompraProductoCompradoViewModel
                        {
                            Item = item, // ← asignamos Item para que la vista tenga un índice único
                            Id = producto.Id,
                            FechaVencimiento = producto.FechaVencimiento != null
                                ? ((DateTime)producto.FechaVencimiento).ToString("yyyy-MM-dd")
                                : null,
                            ProductoId = producto.ProductoId,
                            Lote = producto.Lote,
                            ProductoCodigo = producto.Producto.CodigoReferencia,
                            NombreProducto = producto.Producto.NombreProducto,
                            UnidadMedidaCompra = new UnidadMedidaCompraViewModel
                            {
                                Id = producto.UnidadMedidaCompraId,
                                NombreUnidad = producto.UnidadMedidaCompra.Nombre
                            },
                            Cantidad = producto.Cantidad,
                            PrecioCompra = producto.Precio,
                            Total = producto.PrecioTotal,
                            Nuevo = false
                        };

                        // POLITICAS DE DEVOLUCION CAMBIO
                        if (producto.ManejaPoliticaDevolucionProveedor)
                        {
                            productoCompradoVm.PoliticaDevolucionProducto = "radio-politica-proveedor";
                        }
                        else if (producto.ManejaPoliticaDevolucionPersonalizada)
                        {
                            productoCompradoVm.PoliticaDevolucionProducto = "radio-politica-personalizada";
                            productoCompradoVm.PoliticaDevolucionPersonalizadaDias = producto.PoliticaDevolucionPersonalizadaDias;
                        }
                        else
                        {
                            productoCompradoVm.PoliticaDevolucionProducto = "radio-politica-no-maneja";
                        }

                        // POLITICAS DE DEVOLUCION VENCIMIENTO
                        if (producto.ManejaPoliticaDevolucionVencimientoProveedor)
                        {
                            productoCompradoVm.PoliticaDevolucionVencimientoProducto = "radio-politica-vencimiento-proveedor";
                        }
                        else if (producto.ManejaPoliticaDevolucionVencimientoPersonalizada)
                        {
                            productoCompradoVm.PoliticaDevolucionVencimientoProducto = "radio-politica-vencimiento-personalizada";
                            productoCompradoVm.PoliticaDevolucionVencimientoPersonalizadaDias = producto.PoliticaDevolucionVencimientoPersonalizadaDias;
                        }
                        else
                        {
                            productoCompradoVm.PoliticaDevolucionVencimientoProducto = "radio-politica-vencimiento-no-maneja";
                        }

                        // CREDITO
                        if (producto.ManejaCreditoProveedor)
                        {
                            productoCompradoVm.CreditoProducto = "radio-credito-proveedor";
                        }
                        else if (producto.ManejaCreditoPersonalizado)
                        {
                            productoCompradoVm.CreditoProducto = "radio-credito-personalizado";
                            productoCompradoVm.CreditoPersonalizadoDias = producto.CreditoPersonalizadoDias;
                        }
                        else
                        {
                            productoCompradoVm.CreditoProducto = "radio-credito-no-maneja";
                        }

                        // Agregar el elemento a la lista a enviar como resultado
                        productosComprados.Add(productoCompradoVm);

                        #region LOTES EXISTENTES
                        if (producto.Producto.ProductosInventario != null)
                        {
                            // Se obtienen solo los lotes que tengan stock
                            producto.Producto.ProductosInventario = producto.Producto.ProductosInventario
                                .Where(a => a.Stock > 0 && !a.Eliminado)
                                .ToList();

                            foreach (var loteInventario in producto.Producto.ProductosInventario)
                            {
                                var fechaVencimiento = "-";
                                if (loteInventario.FechaVencimientoArticuloCompra != null)
                                    fechaVencimiento = ((DateTime)loteInventario.FechaVencimientoArticuloCompra)
                                        .ToString("dd-MM-yyyy");

                                var fechaRecepcionLote = "-";
                                if (loteInventario.FechaRecepcionLote != null)
                                    fechaRecepcionLote = ((DateTime)loteInventario.FechaRecepcionLote)
                                        .ToString("dd-MM-yyyy");

                                var compra = loteInventario.Compra ?? new Compra();
                                var bodega = loteInventario.Bodega ?? new Bodega();
                                var proveedor = compra.Proveedor ?? new Proveedor();

                                // Precio compra
                                decimal precioCompra = 0;
                                if (compra.DetalleCompras != null)
                                {
                                    var registroCompra = compra.DetalleCompras
                                        .Where(a => a.ProductoId == loteInventario.ProductoId)
                                        .FirstOrDefault();
                                    if (registroCompra != null)
                                        precioCompra = registroCompra.Precio;
                                }

                                lotesExistentes.Add(new ProductoLoteExistenteVM
                                {
                                    FechaVencimiento = fechaVencimiento,
                                    ProductoId = loteInventario.ProductoId,
                                    ProveedorNombre = proveedor.Nombre,
                                    PrecioCompra = precioCompra,
                                    Stock = loteInventario.Stock,
                                    Lote = loteInventario.Lote,
                                    FechaRecepcionLote = fechaRecepcionLote,
                                    BodegaNombre = bodega.NombreBodega
                                });
                            }
                        }
                        #endregion

                        #region UBICACIONES
                        if (producto.DetalleComprasUbicaciones != null)
                        {
                            foreach (var ubicacion in producto.DetalleComprasUbicaciones)
                            {
                                var bodega = ubicacion.Bodega ?? new Bodega();
                                var equivalencia = producto.Producto.ProductoEquivalencias
                                    .Where(a => a.UnidadMedidaVentaId == ubicacion.UnidadMedidaVentaId)
                                    .FirstOrDefault();
                                if (equivalencia == null)
                                    continue;
                                var unidadVenta = ubicacion.UnidadMedidaVenta ?? new UnidadMedidaVenta();

                                ubicaciones.Add(new CompraUbicacionViewModel
                                {
                                    DetalleOrdenCompraId = producto.Id,
                                    BodegaId = (int)ubicacion.BodegaId,
                                    NombreUbicacion = bodega.NombreBodega,
                                    Cantidad = ubicacion.Cantidad,
                                    UnidadMedidaVentaId = (int)ubicacion.UnidadMedidaVentaId,
                                    NombreUnidad = unidadVenta.Nombre,
                                    CantidadEquivalenteDestino = equivalencia.CantidadEquivalenteDestino * ubicacion.Cantidad,
                                    UnidadEquivalencia = $"1 {unidadVenta.Nombre} =" +
                                        $" {equivalencia.CantidadEquivalenteDestino} {equivalencia.UnidadMedidaVenta?.Nombre ?? "-"}"
                                });

                                // Precios
                                if (ubicacion.DetalleCompraUbicacionPrecios != null)
                                {
                                    foreach (var precioBd in ubicacion.DetalleCompraUbicacionPrecios)
                                    {
                                        var precio = precioBd.Precio ?? new Precio();

                                        precios.Add(new CompraPrecioVentaViewModel
                                        {
                                            DetalleOrdenCompraId = producto.Id,
                                            UnidadMedidaVentaId = unidadVenta.Id,
                                            PrecioNombre = precio.NombrePrecio,
                                            PrecioValor = precioBd.PrecioValor,
                                            PrecioId = (int)precioBd.PrecioId
                                        });
                                    }
                                }
                            }
                        }
                        #endregion

                        // Incrementamos el índice para el siguiente producto
                        item++;
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = productosComprados,
                    LotesExistentes = lotesExistentes,
                    Ubicaciones = ubicaciones,
                    Precios = precios
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos de la orden. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string CambiarAComprado(int? ordenCompraId, int? compraId)
        {
            try
            {
                // 1) Prioridad: si viene una orden de compra, se procesa como hasta ahora
                if (ordenCompraId.HasValue)
                {
                    return CambiarOrdenAComprado(ordenCompraId.Value);
                }

                // 2) Si no hay orden, pero sí hay compraId → compra directa
                if (compraId.HasValue)
                {
                    return CambiarCompraDirectaAComprado(compraId.Value);
                }

                // 3) Ninguno de los dos parámetros vino con valor
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Debe proporcionar OrdenCompraId o CompraId para cambiar a COMPRADO."
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al cambiar a comprado. " + ex.Message
                });
            }
        }

        /// <summary>
        /// Flujo original: cambiar una ORDEN de compra a COMPRADO.
        /// </summary>
        private string CambiarOrdenAComprado(int ordenCompraId)
        {
            try
            {
                var ordenCompra = _compraRepository.Get(ordenCompraId);

                if (ordenCompra == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "La orden procesada no existe"
                    });
                }

                var fechaCompra = DateTime.Now;

                _comprasService.AgregarProductosAInventario(
                    ordenCompra,
                    _userManager.GetUserId(HttpContext.User)
                );

                // Marcar la orden como RECIBIDA / COMPRADA
                ordenCompra.OrdenCompraRecibida = true;
                ordenCompra.FechaCompra = fechaCompra;
                _compraRepository.Update(ordenCompra);

                TempData["Message"] = "¡Se registró como COMPRADA la orden!";

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
                    Mensaje = "Error de servidor al cambiar la orden a comprado. " + ex.Message
                });
            }
        }

        /// <summary>
        /// Flujo para COMPRA DIRECTA (sin orden previa).
        /// </summary>
        private string CambiarCompraDirectaAComprado(int compraId)
        {
            try
            {
                var compra = _compraRepository.Get(compraId);

                if (compra == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "La compra procesada no existe"
                    });
                }

                var fechaCompra = DateTime.Now;

                _comprasService.AgregarProductosAInventario(
                    compra,
                    _userManager.GetUserId(HttpContext.User)
                );

                // Ajusta estos flags si tu modelo distingue orden vs compra,
                // por ahora se mantiene la misma lógica que en la orden.
                compra.OrdenCompraRecibida = true;
                compra.FechaCompra = fechaCompra;
                _compraRepository.Update(compra);

                TempData["Message"] = "¡Se registró como COMPRADA la compra!";

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
                    Mensaje = "Error de servidor al cambiar la compra a comprado. " + ex.Message
                });
            }
        }



        public IActionResult ReporteComprasFechas()
        {
            var model = new CompraBaseViewModel();
            model.Init(_empleadoRepository);

            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Reporte(int? CompraId)
        {
            if (CompraId == null) return StatusCode(400);

            var compra = _compraRepository.Get((int)CompraId);

            if (compra == null) return StatusCode(404);

            // var user = _userManager.GetUserAsync(HttpContext.User);
            // var u = _userRepository.GetbyId(user.Result.Id).Persona.Nombre;

            return await _generatePdf.GetPdf("Views/Compra/Reporte.cshtml", compra);
        }

        [HttpPost]
        public IActionResult GenerarOrdenesDesdeCotizacion([FromBody] CompraBaseViewModel model)
        {
            Console.WriteLine("=== [GenerarOrdenesDesdeCotizacion] INICIO ===");

            // 1. Validaciones
            if (model == null)
            {
                return BadRequest(new { Exitoso = false, Mensaje = "El modelo recibido es nulo." });
            }

            if (model.Productos == null || !model.Productos.Any())
            {
                return BadRequest(new { Exitoso = false, Mensaje = "No se recibieron productos." });
            }

            if (model.EncabezadoSucursalId == null || model.EncabezadoAmbienteId == null)
            {
                return BadRequest(new { Exitoso = false, Mensaje = "Faltan datos de Sucursal o Ambiente." });
            }

            // Resolver empleado
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            var empleadoId = model.EncabezadoEmpleadoId ?? (user?.EmpleadoId ?? 1);

            // 2. Normalización de Proveedores (Lógica existente mantenida)
            foreach (var prod in model.Productos)
            {
                if (string.IsNullOrWhiteSpace(prod.ProveedorPrincipal))
                {
                    var key = prod.ProductoId.ToString();
                    if (model.ProveedorPrincipalPorItem != null && model.ProveedorPrincipalPorItem.TryGetValue(key, out var proveedor))
                    {
                        prod.ProveedorPrincipal = proveedor;
                    }
                }
            }

            // Validación de productos huérfanos
            var productosSinProveedor = model.Productos
                .Where(p => string.IsNullOrWhiteSpace(p.ProveedorPrincipal))
                .ToList();

            if (productosSinProveedor.Any())
            {
                return BadRequest(new
                {
                    Exitoso = false,
                    Mensaje = "Existen productos sin proveedor principal definido.",
                    ProductosSinProveedor = productosSinProveedor.Select(p => new { p.ProductoId, p.Codigo, p.Producto })
                });
            }

            // 3. Agrupación y Generación de Órdenes
            var gruposPorProveedor = model.Productos.GroupBy(p => p.ProveedorPrincipal).ToList();
            var fechaHora = DateTime.Now;
            var ordenesCreadas = new List<object>();

            try
            {
                // Idealmente esto debería ir dentro de una transacción de BD (_context.Database.BeginTransaction())
                // para asegurar atomicidad, pero me apego a la lógica actual solicitada.

                foreach (var grupo in gruposPorProveedor)
                {
                    var nombreProveedor = grupo.Key;

                    // Buscar o Crear Proveedor
                    var proveedor = _proveedorRepository.GetProveedorPorNombre(nombreProveedor);
                    if (proveedor == null)
                    {
                        if (string.IsNullOrWhiteSpace(nombreProveedor)) continue; // Skip si nombre inválido

                        proveedor = new Proveedor
                        {
                            Nombre = nombreProveedor,
                            BancoId = 1
                        };
                        _proveedorRepository.Add(proveedor);
                        // Es posible que necesites _context.SaveChanges() aquí si el repo no lo hace auto, 
                        // para obtener el ID del proveedor nuevo. Asumiré que tu Repo lo maneja.
                    }

                    // Parseo de Fechas
                    DateTime? fechaLimite = null;
                    if (DateTime.TryParse(model.EncabezadoFechaLimite, out var f)) fechaLimite = f;

                    DateTime? fechaRecepcionFactura = null;
                    if (DateTime.TryParse(model.EncabezadoFechaRecepcion, out var fr)) fechaRecepcionFactura = fr;

                    // Creación de la Orden
                    var nuevaOrden = new Compra
                    {
                        Proveedor = proveedor,
                        // CORRECCIÓN SOLICITADA: Mapear el Ambiente al TipoBodegaId
                        TipoBodegaId = model.EncabezadoAmbienteId,
                        DiasCredito = model.DiasCredito,
                        NoComprobante = model.EncabezadoNoComprobante,
                        SucursalId = model.EncabezadoSucursalId.Value,
                        EmpleadoId = empleadoId,
                        FechaLimite = fechaLimite,
                        FechaCompra = fechaHora,
                        Observaciones = model.EncabezadoObservacion,
                        TipoCompraId = 1,
                        CompraTipoDocumentoId = 1,
                        OrdenCompraRecibida = false,
                        FechaRecepcion = fechaRecepcionFactura,
                        DetalleCompras = new List<DetalleCompra>()
                    };

                    decimal totalOrden = 0m;

                    foreach (var item in grupo)
                    {
                        var precioTotal = item.Cantidad * item.PrecioCompra;

                        var detalle = new DetalleCompra
                        {
                            ProductoId = item.ProductoId,
                            UnidadMedidaCompraId = 1, // Default según tu lógica anterior
                            Cantidad = item.Cantidad,
                            Precio = item.PrecioCompra,
                            PrecioTotal = precioTotal,
                            DetalleComprasUbicaciones = new List<DetalleCompraUbicacion>(),
                            // Booleanos por defecto explícitos para evitar nulos en BD
                            ManejaPoliticaDevolucion = false,
                            ManejaPoliticaDevolucionProveedor = false,
                            ManejaPoliticaDevolucionPersonalizada = false,
                            ManejaPoliticaDevolucionVencimiento = false,
                            ManejaPoliticaDevolucionVencimientoProveedor = false,
                            ManejaPoliticaDevolucionVencimientoPersonalizada = false,
                            ManejaCredito = false,
                            ManejaCreditoProveedor = false,
                            ManejaCreditoPersonalizado = false
                        };

                        nuevaOrden.DetalleCompras.Add(detalle);
                        totalOrden += precioTotal;
                    }

                    _compraRepository.Add(nuevaOrden);

                    ordenesCreadas.Add(new
                    {
                        Proveedor = nombreProveedor,
                        OrdenCompraId = nuevaOrden.Id, // Asegúrate que tu Repo actualice el ID tras el Add
                        Total = totalOrden,
                        Fecha = nuevaOrden.FechaCompra
                    });
                }

                return Ok(new
                {
                    Exitoso = true,
                    Mensaje = "Órdenes de compra generadas correctamente desde las cotizaciones.",
                    OrdenesCreadas = ordenesCreadas
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error crítico generando órdenes: {ex.Message}");
                return StatusCode(500, new { Exitoso = false, Mensaje = "Error interno del servidor: " + ex.Message });
            }
        }


        

    }
}
