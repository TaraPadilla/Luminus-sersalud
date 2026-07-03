using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using sistema.Models;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using sistema.Json;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Wkhtmltopdf.NetCore;
using Microsoft.AspNetCore.Identity;
using System.IO.Packaging;
using DocumentFormat.OpenXml;
using OfficeOpenXml;
using IronXL;
using System.Linq;
using System.Globalization;
using System.Text.Json;
using System.Collections.Generic;
using Database.Shared.Enumeraciones;
using DocumentFormat.OpenXml.Office2010.Excel;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using DocumentFormat.OpenXml.Office.CustomUI;
using farmamest.Service.IService;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Database.Shared.Dto;

namespace sistema.Controllers
{
    [Authorize(Roles = "Administrador,Seguros,Auxiliar Farmacia,Encargado Bodega Farmacia,Coordinador Compras,Coordinador Financiero,Direccion Medicos,Coordinador Contable,Coordinador Farmacia, Recepcion Diurno, Encargado de Kardex")]
    public class ProductosController : Controller
    {
        private readonly IAmbiente _ambienteRepository = null;
        private readonly IProducto _productosRepository = null;
        private readonly IBodega _bodegaRepository = null;
        private readonly IDespegablesProducto _categoryRepository = null;
        private readonly ISucursal _sucursalRepository = null;
        private readonly IPacientes _pacientesRepository = null;
        private readonly IPrecios _preciosRepository = null;
        private readonly ICliente _clienteRepository = null;
        private readonly IGeneratePdf _generatePdf;
        private readonly UserManager<User> _userManager;
        private readonly IUser _userRepository;
        private IWebHostEnvironment _hostEnvironment;
        private readonly ICompra _compraRepository;
        private readonly ISeguro _seguroRepository;

        private readonly IServicio _servicioRepository;

        private readonly ILaboratorioClinico _laboratorioClinicoRepository;

        private readonly IProductosService _productosService;
        private readonly Database.Shared.Context _context;


        public ProductosController(IDespegablesProducto categoriaRepository, IProducto productosRepository, ICliente clienteRepository,
        IWebHostEnvironment hostEnvironment,
        ISucursal sucursalRepository,
        IAmbiente ambienteRepository,
        IPrecios preciosRepository,
        ISeguro seguroRepository,
        IServicio servicioRepository,
        IBodega bodegaRepository,
        ILaboratorioClinico laboratorioClinicoRepository,
        IGeneratePdf generatePdf, UserManager<User> userManager, IUser userRepository
        , IPacientes pacienteRepository, ICompra compraRepository, Database.Shared.Context context,
        IProductosService productosService)
        {
            _categoryRepository = categoriaRepository;
            _productosRepository = productosRepository;
            _bodegaRepository = bodegaRepository;
            _ambienteRepository = ambienteRepository;
            _preciosRepository = preciosRepository;
            _clienteRepository = clienteRepository;
            _hostEnvironment = hostEnvironment;
            _generatePdf = generatePdf;
            _userManager = userManager;
            _userRepository = userRepository;
            _sucursalRepository = sucursalRepository;
            _pacientesRepository = pacienteRepository;
            _compraRepository = compraRepository;
            _context = context;
            _productosService = productosService;
            _seguroRepository = seguroRepository;
            _servicioRepository = servicioRepository;
            _laboratorioClinicoRepository = laboratorioClinicoRepository;
        }

        #region Unidades
        [HttpPost]
        public string ConsultarUnidadesCompra()
        {
            try
            {
                var listaUnidades = new List<ProductoUnidadCompraViewModel>();
                var unidadesBd = _productosRepository.GetUnidadesCompra();
                if (unidadesBd != null)
                {
                    foreach (var unidad in unidadesBd)
                    {
                        listaUnidades.Add(new ProductoUnidadCompraViewModel
                        {
                            Id = unidad.Id,
                            Nombre = unidad.Nombre,
                            Abreviatura = unidad.Abreviatura
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaUnidades
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar unidades de compra. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarUnidadesVenta()
        {
            try
            {
                var listaUnidades = new List<ProductoUnidadVentaViewModel>();
                var unidadesBd = _productosRepository.GetUnidadesVenta();
                if (unidadesBd != null)
                {
                    foreach (var unidad in unidadesBd)
                    {
                        listaUnidades.Add(new ProductoUnidadVentaViewModel
                        {
                            Id = unidad.Id,
                            Nombre = unidad.Nombre,
                            Abreviatura = unidad.Abreviatura
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaUnidades
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar unidades de venta. " + ex.Message
                });
            }
        }
        public string AgregarUnidadCompra(string nombreUnidad, string abreviatura)
        {
            try
            {
                var unidad = new UnidadMedidaCompra
                {
                    Nombre = nombreUnidad,
                    Abreviatura = abreviatura
                };
                _productosRepository.AddUnidadCompra(unidad);
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
                    Mensaje = "Error al registrar unidad de compra. " + ex.Message
                });
            }
        }
        public string AgregarUnidadVenta(string nombreUnidad, string abreviatura)
        {
            try
            {
                var unidad = new UnidadMedidaVenta
                {
                    Nombre = nombreUnidad,
                    Abreviatura = abreviatura
                };
                _productosRepository.AddUnidadVenta(unidad);
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
                    Mensaje = "Error al registrar unidad de venta. " + ex.Message
                });
            }
        }
        #endregion

        #region Consulta Categorias, marcas, grupos, presentaciones, vias de administracion, laboratorios

        [HttpPost]
        public string ConsultarCategorias()
        {
            try
            {
                var categorias = _categoryRepository.ListaCategorias().ToList();
                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = categorias });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar categorías. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarGruposT()
        {
            try
            {
                var grupos = _categoryRepository.ListarGrupoT(false).ToList();
                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = grupos });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar grupos. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarMarcas()
        {
            try
            {
                var marcas = _categoryRepository.ListaMarcas().ToList();
                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = marcas });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar marcas. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarPresentaciones()
        {
            try
            {
                var presentaciones = _categoryRepository.ListarPresentacion(false).ToList();
                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = presentaciones });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar presentaciones. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarViasAdministracion()
        {
            try
            {
                var viasAdministracionConsultadas = new List<ProductoViaAdministracionViewModel>();
                var viasBd = _categoryRepository.GetViadmins();
                if (viasBd != null)
                {
                    foreach (var via in viasBd)
                    {
                        viasAdministracionConsultadas.Add(new ProductoViaAdministracionViewModel
                        {
                            Id = via.Id,
                            NombreViadmin = via.NombreViadmin
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = viasAdministracionConsultadas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar vias de administracion. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarLaboratorios()
        {
            try
            {
                var laboratoriosConsultados = new List<ProductoLaboratorioViewModel>();
                var laboratoriosBd = _categoryRepository.ListaLaboratorioProducto();
                if (laboratoriosBd != null)
                {
                    foreach (var lab in laboratoriosBd)
                    {
                        laboratoriosConsultados.Add(new ProductoLaboratorioViewModel
                        {
                            Id = lab.Id,
                            NombreLaboratorioProducto = lab.NombreLaboratorioProducto
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = laboratoriosConsultados
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar laboratorios. " + ex.Message
                });
            }
        }

        #endregion

        public async Task<IActionResult> MedicamentosFarmaciaReporte(int terapeuticoId, string currentFilter)
        {
            var productos = _productosRepository.FiltrarPorBusquedaYTerapeutico(currentFilter, terapeuticoId, 1);
            var user = _userManager.GetUserAsync(HttpContext.User);
            var u = _userRepository.GetbyId(user.Result.Id).Persona.Nombre;

            var model = new ReportesProductosViewModel()
            {
                Productos = productos,
                Usuario = u
            };

            return await _generatePdf.GetPdf("Views/Productos/MedicamentosFarmaciaReporte.cshtml", model);
        }

        public async Task<IActionResult> InsumosFarmaciaReporte(int categoriaId, string currentFilter)
        {
            var productos = _productosRepository.FiltrarPorBusquedaYCategoria(currentFilter, categoriaId, 1);
            var user = _userManager.GetUserAsync(HttpContext.User);
            var us = _userRepository.GetbyId(user.Result.Id);
            string nombre = "";
            if (us.Persona != null)
                nombre = us.Persona.Nombre;

            var model = new ReportesProductosViewModel()
            {
                Productos = productos,
                Usuario = nombre
            };

            return await _generatePdf.GetPdf("Views/Productos/InsumosFarmaciaReporte.cshtml", model);
        }

        public async Task<IActionResult> MedicamentosClinicaReporte(int terapeuticoId, string currentFilter)
        {
            var productos = _productosRepository.FiltrarPorBusquedaYTerapeutico(currentFilter, terapeuticoId, 2);
            var user = _userManager.GetUserAsync(HttpContext.User);
            var u = _userRepository.GetbyId(user.Result.Id).Persona.Nombre;

            var model = new ReportesProductosViewModel()
            {
                Productos = productos,
                Usuario = u
            };

            return await _generatePdf.GetPdf("Views/Productos/MedicamentosClinicaReporte.cshtml", model);
        }

        public async Task<IActionResult> InsumosClinicaReporte(int categoriaId, string currentFilter)
        {
            var productos = _productosRepository.FiltrarPorBusquedaYCategoria(currentFilter, categoriaId, 2);
            var user = _userManager.GetUserAsync(HttpContext.User);
            var u = _userRepository.GetbyId(user.Result.Id).Persona.Nombre;

            var model = new ReportesProductosViewModel()
            {
                Productos = productos,
                Usuario = u
            };

            return await _generatePdf.GetPdf("Views/Productos/InsumosClinicaReporte.cshtml", model);
        }
        public async Task<IActionResult> MedicamentosBodegaReporte(int terapeuticoId, string currentFilter)
        {
            var productos = _productosRepository.FiltrarPorBusquedaYTerapeutico(currentFilter, terapeuticoId, 3);
            var user = _userManager.GetUserAsync(HttpContext.User);
            var u = _userRepository.GetbyId(user.Result.Id).Persona.Nombre;

            var model = new ReportesProductosViewModel()
            {
                Productos = productos,
                Usuario = u
            };

            return await _generatePdf.GetPdf("Views/Productos/MedicamentosBodegaReporte.cshtml", model);
        }

        public async Task<IActionResult> InsumosBodegaReporte(int categoriaId, string currentFilter)
        {
            var productos = _productosRepository.FiltrarPorBusquedaYCategoria(currentFilter, categoriaId, 3);
            var user = _userManager.GetUserAsync(HttpContext.User);
            var us = _userRepository.GetbyId(user.Result.Id);
            string nombre = "";
            if (us.Persona != null)
                nombre = us.Persona.Nombre;

            var model = new ReportesProductosViewModel()
            {
                Productos = productos,
                Usuario = nombre
            };

            return await _generatePdf.GetPdf("Views/Productos/InsumosBodegaReporte.cshtml", model);
        }









        [Authorize]
        public IActionResult ListaFaltantesFarmacia(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _productosRepository.PaginacionProductosFaltantes(sortOrder, buscar, pageNumber, 25);

            return View(lista);
        }

        public async Task<IActionResult> FaltantesFarmacia(string currentFilter)
        {

            var productos = _productosRepository.GetListadoFaltantesFarmacia(currentFilter);
            return await _generatePdf.GetPdf("Views/Productos/FaltantesFarmacia.cshtml", productos);
        }

        [Authorize]
        public IActionResult ListaFaltantesClinica(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _productosRepository.PaginacionProductosFaltantes(sortOrder, buscar, pageNumber, 25);

            return View(lista);
        }

        public async Task<IActionResult> FaltantesClinica(string currentFilter)
        {

            var productos = _productosRepository.GetListadoFaltantesClinica(currentFilter);
            return await _generatePdf.GetPdf("Views/Productos/FaltantesClinica.cshtml", productos);
        }

        [Authorize]
        public IActionResult ListaVencimiento(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _productosRepository.PaginacionProductosVencimiento(sortOrder, buscar, pageNumber, 25);

            return View(lista);
        }


        [HttpPost]
        public IActionResult ModificarLote(int productoId, int productoInventarioId, string lote)
        {
            try
            {
                var productoInventario = _productosRepository.GetRegistroInventarioProducto(productoInventarioId, productoId);

                if (productoInventario != null)
                {
                    productoInventario.Lote = lote;  // Actualizar el número de lote
                    _productosRepository.UpdateRegistroInventario(productoInventario);

                    // Devolviendo un objeto JSON válido
                    return Json(new { Exitoso = true, Mensaje = "Número de lote modificado correctamente." });
                }
                else
                {
                    return Json(new { Exitoso = false, Mensaje = "No se encontró el registro de inventario." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al actualizar el número de lote: " + ex.Message });
            }
        }


        [HttpPost]
        public IActionResult ModificarFechaRecepcionLote(int productoId, int productoInventarioId, DateTime fechaRecepcionLote)
        {
            try
            {
                var productoInventario = _productosRepository.GetRegistroInventarioProducto(productoInventarioId, productoId);

                if (productoInventario != null)
                {
                    productoInventario.FechaRecepcionLote = fechaRecepcionLote;
                    _productosRepository.UpdateRegistroInventario(productoInventario);

                    // Devolviendo un objeto JSON válido
                    return Json(new { Exitoso = true, Mensaje = "Fecha de recepción de lote modificada correctamente." });
                }
                else
                {
                    return Json(new { Exitoso = false, Mensaje = "No se encontró el registro de inventario." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al actualizar la fecha de recepción de lote: " + ex.Message });
            }
        }


        [HttpPost]
        public IActionResult ModificarFechaVencimiento(int productoId, int productoInventarioId, DateTime fechaVencimiento)
        {
            try
            {
                var productoInventario = _productosRepository.GetRegistroInventarioProducto(productoInventarioId, productoId);

                if (productoInventario != null)
                {
                    productoInventario.FechaVencimientoArticuloCompra = fechaVencimiento;
                    _productosRepository.UpdateRegistroInventario(productoInventario);

                    // Devolviendo un objeto JSON válido
                    return Json(new { Exitoso = true, Mensaje = "Fecha de vencimiento modificada correctamente." });
                }
                else
                {
                    return Json(new { Exitoso = false, Mensaje = "No se encontró el registro de inventario." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al actualizar la fecha de vencimiento: " + ex.Message });
            }
        }



        #region Modificar precios
        [HttpPost]
        public string ModificarPrecio(int productoInventarioPrecioId, decimal productoInventarioPrecioValor, int precioId, int productoInventarioId)
        {
            try
            {
                // Console.WriteLine("Iniciando ModificarPrecio");
                // Console.WriteLine($"Parámetros recibidos: productoInventarioPrecioId={productoInventarioPrecioId}, productoInventarioPrecioValor={productoInventarioPrecioValor}, precioId={precioId}, productoInventarioId={productoInventarioId}");

                // Buscar el registro existente en la tabla ProductoInventarioPrecio por ProductoInventarioId y PrecioId
                var productoInventarioPrecio = _productosRepository.GetProductoInventarioPrecioPorInventarioYTipoPrecio(productoInventarioId, precioId);
                string nombrePrecio = _productosRepository.GetNombrePrecio(precioId); // Obtener el nombre del precio
                decimal valorActualizado;

                if (productoInventarioPrecio == null)
                {
                    // Console.WriteLine("Creando nuevo ProductoInventarioPrecio...");
                    ProductoInventarioPrecio nuevoProductoInventarioPrecio = new ProductoInventarioPrecio()
                    {
                        PrecioId = precioId,
                        ProductoInventarioId = productoInventarioId,
                        Valor = productoInventarioPrecioValor
                    };
                    _productosRepository.Add(nuevoProductoInventarioPrecio);
                    // Console.WriteLine("Nuevo ProductoInventarioPrecio agregado correctamente.");
                    valorActualizado = productoInventarioPrecioValor; // El valor recién agregado
                }
                else
                {
                    // Console.WriteLine($"Actualizando ProductoInventarioPrecio existente con ID: {productoInventarioPrecio.Id}");
                    productoInventarioPrecio.Valor = productoInventarioPrecioValor; // Actualización del valor del precio
                    _productosRepository.Update(productoInventarioPrecio);
                    // Console.WriteLine("ProductoInventarioPrecio actualizado correctamente.");
                    valorActualizado = productoInventarioPrecioValor; // El valor actualizado
                }

                // Console.WriteLine("Modificación completada con éxito.");
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    NombrePrecio = nombrePrecio,
                    ValorActualizado = valorActualizado
                });
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error en ModificarPrecio: {ex.Message}");
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al modificar precio. " + ex.Message
                });
            }
        }
        #endregion

        #region Modificar todos Precios
        [HttpPost]
        public string ModificarTodosPrecios(List<Producto> Productos)
        {
            //Recorre la lista de productos
            try
            {
                foreach (var producto in Productos)
                {
                    var productoId = producto.Id;
                    //Recorre los diferentes inventarios del producto
                    foreach (var productoInventario in producto.ProductosInventario)
                    {

                        var productoInventarioId = productoInventario.Id;
                        //Captura los productos inventarios  que solo tienen un precio
                        if (productoInventario.ProductosInventarioPrecios.Count() == 1)
                        {
                            //Buaca los precios existentes
                            var preciosExistentes = _preciosRepository.GetList();

                            if (preciosExistentes != null)
                            {
                                foreach (var precio in preciosExistentes)
                                {
                                    //Agrega todos los precios existentes ecepto el primero
                                    if (precio.Id != 1)
                                    {
                                        productoInventario.ProductosInventarioPrecios.Add(new ProductoInventarioPrecio
                                        {
                                            PrecioId = precio.Id,
                                            Valor = 0
                                        });
                                    }

                                }
                            }
                            _productosRepository.Update(productoInventario);

                        }

                    }

                }

                //var productoInventario = _productosRepository.GetRegistroInventarioProducto((int)productoInventarioId);
                //var productoSeleccionado = _productosRepository.GetEquivalenciasProducto((int)productoId).FirstOrDefault();
                //productoInventario.Stock = stock;
                //productoInventario.UnidadMedidaVenta = productoSeleccionado.UnidadMedidaVenta;

                //var preciosExistentes = _preciosRepository.GetList();
                //if (preciosExistentes != null)
                //{
                //    foreach (var precio in preciosExistentes)
                //    {
                //        productoInventario.ProductosInventarioPrecios.Add(new ProductoInventarioPrecio
                //        {
                //            PrecioId = precio.Id,
                //            Valor = 0
                //        });
                //    }
                //}
                //_productosRepository.Update(productoInventario);


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
                    Mensaje = "Error de servidor al modificar stock. " + ex.Message
                });
            }
        }
        #endregion



        #region Modificar stock
        [HttpPost]
        public string ModificarStock(int? productoId, int? productoInventarioId,
            decimal stock, bool crearRegistroInventario, int ambienteId)
        {
            try
            {
                if (productoId == null || (productoInventarioId == null && !crearRegistroInventario))
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "El ID del producto o inventario es nulo."
                    });
                }

                if (crearRegistroInventario)
                {
                    var bodegas = _bodegaRepository.GetList()
                        .Where(a => a.AmbienteId == ambienteId)
                        .ToList();

                    // Obtener todas las equivalencias del producto
                    var equivalenciasProducto = _productosRepository.GetEquivalenciasProducto((int)productoId).ToList();

                    if (equivalenciasProducto == null || equivalenciasProducto.Count == 0)
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = "No es posible modificar el stock, ya que no existen equivalencias del producto."
                        });
                    }

                    var preciosExistentes = _preciosRepository.GetList();
                    var producto = _productosRepository.Get((int)productoId);
                    producto.Stock = stock;

                    if (bodegas.Count > 0)
                    {
                        foreach (var bodega in bodegas)
                        {
                            // Crear un registro de inventario por cada equivalencia del producto
                            foreach (var equivalencia in equivalenciasProducto)
                            {
                                var registroInventario = new ProductoInventario
                                {
                                    ProductoId = (int)productoId,
                                    Stock = stock,
                                    BodegaId = bodega.Id,
                                    UnidadMedidaVentaId = equivalencia.UnidadMedidaVentaId
                                };

                                if (preciosExistentes != null)
                                {
                                    foreach (var precio in preciosExistentes)
                                    {
                                        registroInventario.ProductosInventarioPrecios.Add(new ProductoInventarioPrecio
                                        {
                                            PrecioId = precio.Id,
                                            Valor = 0
                                        });
                                    }
                                }
                                _productosRepository.AddProductoInventario(registroInventario);
                            }
                        }
                    }
                    else
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = "No hay bodegas creadas en este ambiente."
                        });
                    }
                    _productosRepository.Update(producto);
                }
                else
                {
                    // Aquí, productoInventarioId no puede ser null
                    var productoInventario = _productosRepository.GetRegistroInventarioProducto((int)productoInventarioId);

                    if (productoInventario == null)
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = "Inventario del producto no encontrado."
                        });
                    }

                    // Si el stock es 0, eliminar el registro
                    if (stock == 0)
                    {
                        _productosRepository.EliminarRegistroInventario(productoInventario);
                    }
                    else
                    {
                        var productoSeleccionado = _productosRepository.GetEquivalenciasProducto((int)productoId).FirstOrDefault();
                        if (productoSeleccionado != null)
                        {
                            productoInventario.Stock = stock;
                            productoInventario.UnidadMedidaVenta = productoSeleccionado.UnidadMedidaVenta;
                            _productosRepository.Update(productoInventario);
                        }
                        else
                        {
                            return JsonSerializer.Serialize(new
                            {
                                Exitoso = false,
                                Mensaje = "Equivalencia del producto no encontrada."
                            });
                        }
                    }
                }

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
                    Mensaje = "Error de servidor al modificar stock. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string ModificarStockMinimo(int? productoId, int? productoInventarioId, decimal stockMinimo, bool crearRegistroInventario, int ambienteId)
        {
            try
            {
                // Log para verificar el valor del stock mínimo
                Console.WriteLine($"Stock mínimo recibido: {stockMinimo}");

                var productoInventario = _productosRepository.GetRegistroInventarioProducto((int)productoInventarioId);

                if (productoInventario != null)
                {
                    // Actualizar el stock mínimo en la base de datos
                    productoInventario.StockMinimo = stockMinimo;
                    _productosRepository.Update(productoInventario);
                }

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al modificar stock mínimo. " + ex.Message
                });
            }
        }

        #endregion

        //private List<object> ProveedorSugerido()
        //{

        //    var data = _compraRepository.GetAllDetalle();

        //    var dato = (data
        //        .GroupBy(x => x.ProductoId)
        //        .Select(a => a.OrderByDescending(x => x.Compra.FechaCompra).First())
        //        .ToList()).Select(x => new
        //        {
        //            ProductoId = x.ProductoId,
        //            NombreProveedor = x.Compra.Proveedor.Nombre
        //        }).ToList();


        //    return dato;
        //}

        #region Inventario

        private void CrearRegistroInventario(int productoId, int ambienteId, decimal stock, decimal precioNormal, decimal precioInterno, decimal precioVIP, decimal stockMinimo, DateTime? fechaRecepcionLote, DateTime? fechaVencimiento, string lote)
        {
            var bodegas = _bodegaRepository.GetList().Where(a => a.AmbienteId == ambienteId).ToList();
            var equivalenciasProducto = _productosRepository.GetEquivalenciasProducto(productoId).ToList();

            if (bodegas.Count > 0)
            {
                foreach (var equivalencia in equivalenciasProducto)
                {
                    foreach (var bodega in bodegas)
                    {
                        // Crear el registro de inventario solo una vez para cada combinación de ProductoId, BodegaId y UnidadMedidaVentaId
                        var registroInventario = new ProductoInventario
                        {
                            ProductoId = productoId,
                            Stock = stock,
                            BodegaId = bodega.Id,
                            UnidadMedidaVentaId = equivalencia.UnidadMedidaVentaId,
                            StockMinimo = stockMinimo, // Añadir el stock mínimo
                            FechaRecepcionLote = fechaRecepcionLote, // Asignar fecha de recepción de lote
                            FechaVencimientoArticuloCompra = fechaVencimiento, // Asignar fecha de vencimiento
                            Lote = lote, // Asignar el lote
                            ProductosInventarioPrecios = new List<ProductoInventarioPrecio>
                    {
                        new ProductoInventarioPrecio
                        {
                            PrecioId = 1, // ID para Precio Normal
                            Valor = precioNormal
                        },
                        new ProductoInventarioPrecio
                        {
                            PrecioId = 2, // ID para Precio Interno
                            Valor = precioInterno
                        },
                        new ProductoInventarioPrecio
                        {
                            PrecioId = 3, // ID para Precio VIP
                            Valor = precioVIP
                        }
                    }
                        };

                        // Agregar el registro de inventario con los precios y stock mínimo
                        _productosRepository.AddProductoInventario(registroInventario);
                    }
                }

            }
        }

        [HttpPost]
        public string ReiniciarStock(int productoId, int ambienteId, decimal precioNormal, decimal precioInterno, decimal precioVIP, decimal stockMinimo, DateTime? fechaRecepcionLote, DateTime? fechaVencimiento, string lote)
        {
            try
            {
                // Obtener todas las equivalencias del producto
                var equivalenciasProducto = _productosRepository.GetEquivalenciasProducto(productoId).ToList();

                if (equivalenciasProducto == null || equivalenciasProducto.Count == 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No es posible reiniciar el stock, ya que no existen equivalencias del producto."
                    });
                }

                // Obtener todos los registros de inventario de este producto
                var registrosInventario = _productosRepository.GetStocks(productoId, null);

                // Eliminar los registros de ventas relacionados antes de eliminar el inventario
                foreach (var registro in registrosInventario)
                {
                    // Obtener los registros de DetalleVentas relacionados al producto en inventario
                    var detallesVentas = _productosRepository.GetDetalleVentasByProductoInventarioId(registro.Id);

                    // Eliminar los registros de ventas relacionados
                    foreach (var detalleVenta in detallesVentas)
                    {
                        _productosRepository.EliminarDetalleVenta(detalleVenta);
                    }

                    // Ahora eliminar el registro de inventario
                    _productosRepository.EliminarRegistroInventario(registro);
                }

                // Crear nuevos registros de inventario con stock de 1, el stock mínimo, las fechas, y el lote
                CrearRegistroInventario(productoId, ambienteId, 1, precioNormal, precioInterno, precioVIP, stockMinimo, fechaRecepcionLote, fechaVencimiento, lote);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Stock reiniciado exitosamente."
                });
            }
            catch (DbUpdateException ex)
            {
                // Capturar la excepción interna para más detalles
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al reiniciar el stock: " + innerException
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al reiniciar el stock: " + ex.Message
                });
            }
        }

        [Authorize]
        public IActionResult Inventario(InventarioViewModel model)
        {
            var viewModel = new InventarioViewModel
            {
                AmbienteId = model.AmbienteId,
                TipoProductoId = model.TipoProductoId,
                BodegaId = model.BodegaId,
                SeguroId = model.SeguroId
            };

            // Inicializa los datos necesarios
            viewModel.Init(_categoryRepository, _sucursalRepository, _bodegaRepository, _categoryRepository, _ambienteRepository, _seguroRepository);
            viewModel.TodasLasPresentaciones = _categoryRepository.ListarPresentacion();

            // Obtiene los productos según los parámetros de tipo de producto, ambiente, bodega, etc.
            var productosSP = _productosService.GetInventarioBySp(model.TipoProductoId, model.terapeuticoId, model.BodegaId, model.sucursalId, model.AmbienteId);

            // Obtener precios disponibles
            viewModel.Precios = _preciosRepository.GetList().ToList();

            // Asignación de productos al viewModel
            viewModel.ProductosInventario = productosSP;
            viewModel.TotalMedicamentos = viewModel.ProductosInventario.Count();

            // Asignación del nombre del ambiente según el AmbienteId
            switch (model.AmbienteId)
            {
                case (int)AmbienteEnum.Clinica:
                    viewModel.NombreBodega = "Clinica";
                    break;
                case (int)AmbienteEnum.Farmacia:
                    viewModel.NombreBodega = "Farmacia";
                    break;
                case (int)AmbienteEnum.Hospital:
                    viewModel.NombreBodega = "Hospital";
                    break;
                case (int)AmbienteEnum.Laboratorio:
                    viewModel.NombreBodega = "Laboratorio";
                    break;
                case (int)AmbienteEnum.Bodega:
                    viewModel.NombreBodega = "Bodega";
                    break;
                default:
                    viewModel.NombreBodega = "Inventario";
                    break;
            }

            // Asignación del nombre del tipo de producto según el TipoProductoId
            switch (model.TipoProductoId)
            {
                case (int)TipoProductoEnum.InsumosMedicos:
                    viewModel.NombreTipoProductos = "Insumos medicos";
                    break;
                case (int)TipoProductoEnum.Medicamentos:
                    viewModel.NombreTipoProductos = "Medicamentos";
                    break;
                case (int)TipoProductoEnum.EquiposMedicos:
                    viewModel.NombreTipoProductos = "Equipos medicos";
                    break;
                case (int)TipoProductoEnum.Suministros:
                    viewModel.NombreTipoProductos = "Suministros";
                    break;
                default:
                    viewModel.NombreTipoProductos = "Inventario";
                    break;
            }

            // Devuelve el ViewModel a la vista
            return View(viewModel);
        }

        [Authorize]
        public IActionResult InventarioProductoNuevo(int tipoBodega, int tipoProducto, int ambienteId)
        {
            var tipoBodegaNombre = "";
            var tipoProductoNombre = "";

            switch (tipoBodega)
            {
                case (int)TipoBodegaEnum.Bodega:
                    tipoBodegaNombre = "Bodega";
                    break;
                case (int)TipoBodegaEnum.Farmacia:
                    tipoBodegaNombre = "Farmacia";
                    break;
                case (int)TipoBodegaEnum.Clinica:
                    tipoBodegaNombre = "Clinica";
                    break;
                case (int)TipoBodegaEnum.Laboratorio:
                    tipoBodegaNombre = "Laboratorio";
                    break;
                default:
                    tipoBodegaNombre = "Inventario";
                    break;
            }

            switch (tipoProducto)
            {
                case (int)TipoProductoEnum.InsumosMedicos:
                    tipoProductoNombre = "Insumos medicos";
                    break;
                case (int)TipoProductoEnum.Medicamentos:
                    tipoProductoNombre = "Medicamentos";
                    break;
                default:
                    tipoProductoNombre = "Inventario";
                    break;
            }

            ambienteId = 6;
            var modelo = new InventarioProductoViewModel
            {
                TipoBodegaId = tipoBodega,
                TipoProductoId = tipoProducto,
                TipoBodegaNombre = tipoBodegaNombre,
                TipoProductoNombre = tipoProductoNombre,
                AmbienteId = ambienteId
            };

            modelo.Init(_categoryRepository);

            return View(modelo);
        }

        [Authorize]
        [HttpPost]
        public string InventarioProductoNuevo(InventarioProductoViewModel model)
        {
            // Revisar este metodo si es necesario al crear un producto nuevo, crear un nuevo producto inventario
            try
            {
                // Parche rápido: todos los productos nuevos se registran en el ambiente Global (Id = 6)
                const int globalAmbienteId = 6;
                model.AmbienteId = globalAmbienteId;

                var productoExistente = _productosRepository.GetByName(model.Nombre);
                if (productoExistente != null &&
                    productoExistente.TipoProductoId == model.TipoProductoId)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Ya existe un producto en esta bodega con nombre " + model.Nombre
                    });
                }

                var producto = new Producto
                {
                    TipoProductoId = model.TipoProductoId,
                    NombreProducto = model.Nombre,
                    CodigoReferencia = model.CodigoReferencia,
                    CategoriaId = model.CategoriaId,
                    MarcaId = model.MarcaId,
                    GrupoTProductoId = model.GrupoId,
                    PresentacionProductoId = model.PresentacionId,
                    PresentacionProductoId2 = model.PresentacionId2,
                    PresentacionProductoId3 = model.PresentacionId3,
                    PresentacionProductoId4 = model.PresentacionId4,
                    PresentacionProductoId5 = model.PresentacionId5,

                    ViadminId = model.ViadminId,
                    LaboratorioProductoId = model.LaboratorioId,
                    ActivoYConcentracion = model.ActivoConcentracion,
                    Imagen = model.UrlImagen,
                    Descripcion = model.Descripcion,
                    //TipoBodegaId = model.TipoBodegaId,
                    Ubicacion = model.Ubicacion,
                    AmbienteId = globalAmbienteId // garantizamos que siempre sea 6
                };

                if (model.Equivalencias != null && model.Equivalencias.Count > 0)
                {
                    foreach (var equivalenciaAgregada in model.Equivalencias)
                    {
                        var equivalenciaProducto = new ProductoEquivalencia
                        {
                            UnidadMedidaCompraId = equivalenciaAgregada.UnidadMedidaCompraId,
                            UnidadMedidaVentaId = equivalenciaAgregada.UnidadMedidaVentaId,
                            CantidadEquivalenteDestino = equivalenciaAgregada.CantidadEquivalente,
                            Eliminada = false
                        };

                        producto.ProductoEquivalencias.Add(equivalenciaProducto);
                    }
                }

                _productosRepository.Add(producto);

                TempData["Message"] = "¡Producto registrado!";

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al registrar producto. " + ex.Message
                });
            }
        }

        public IActionResult InventarioProductoModificar(int? id, int ambienteId, int tipoProductoId)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var producto = _productosRepository.Get((int)id);

            if (producto == null)
            {
                return StatusCode(404);
            }

            var tipoBodegaNombre = "";
            var tipoProductoNombre = "";

            switch (producto.TipoBodegaId)
            {
                case (int)TipoBodegaEnum.Bodega:
                    tipoBodegaNombre = "Bodega";
                    break;
                case (int)TipoBodegaEnum.Farmacia:
                    tipoBodegaNombre = "Farmacia";
                    break;
                case (int)TipoBodegaEnum.Clinica:
                    tipoBodegaNombre = "Clinica";
                    break;
                case (int)TipoBodegaEnum.Laboratorio:
                    tipoBodegaNombre = "Laboratorio";
                    break;
                default:
                    tipoBodegaNombre = "Inventario";
                    break;
            }

            switch (producto.TipoProductoId)
            {
                case (int)TipoProductoEnum.InsumosMedicos:
                    tipoProductoNombre = "Insumos medicos";
                    break;
                case (int)TipoProductoEnum.Medicamentos:
                    tipoProductoNombre = "Medicamentos";
                    break;
                default:
                    tipoProductoNombre = "Inventario";
                    break;
            }

            var modelo = new InventarioProductoViewModel()
            {
                Id = producto.Id,
                CodigoReferencia = producto.CodigoReferencia,
                CategoriaId = producto.CategoriaId,
                GrupoId = producto.GrupoTProductoId,
                MarcaId = producto.MarcaId,
                PresentacionId = producto.PresentacionProductoId,
                PresentacionId2 = producto.PresentacionProductoId2,
                PresentacionId3 = producto.PresentacionProductoId3,
                PresentacionId4 = producto.PresentacionProductoId4,
                PresentacionId5 = producto.PresentacionProductoId5,

                ViadminId = producto.ViadminId,
                LaboratorioId = producto.LaboratorioProductoId,
                Nombre = producto.NombreProducto,
                ActivoConcentracion = producto.ActivoYConcentracion,
                UrlImagen = producto.Imagen,
                Descripcion = producto.Descripcion,
                TipoProductoId = producto.TipoProductoId,
                TipoProductoNombre = tipoProductoNombre,
                TipoBodegaId = producto.TipoBodegaId,
                TipoBodegaNombre = tipoBodegaNombre,
                AmbienteId = ambienteId,
                Ubicacion = producto.Ubicacion
            };

            modelo.Init(_categoryRepository);

            return View(modelo);
        }

        [Authorize]
        [HttpPost]
        public string InventarioProductoModificar(InventarioProductoViewModel model)
        {
            try
            {
                // Paso 1: Obtener el producto existente
                var producto = _productosRepository.Get((int)model.Id);

                // Paso 2: Actualizar campos básicos del producto
                producto.NombreProducto = model.Nombre;
                producto.CodigoReferencia = model.CodigoReferencia;
                producto.CategoriaId = model.CategoriaId;
                producto.MarcaId = model.MarcaId;
                producto.GrupoTProductoId = model.GrupoId;
                producto.PresentacionProductoId = model.PresentacionId;
                producto.PresentacionProductoId2 = model.PresentacionId2;
                producto.PresentacionProductoId3 = model.PresentacionId3;
                producto.PresentacionProductoId4 = model.PresentacionId4;
                producto.PresentacionProductoId5 = model.PresentacionId5;
                producto.ViadminId = model.ViadminId;
                producto.LaboratorioProductoId = model.LaboratorioId;
                producto.ActivoYConcentracion = model.ActivoConcentracion;
                producto.Imagen = model.UrlImagen;
                producto.Descripcion = model.Descripcion;
                producto.Ubicacion = model.Ubicacion;

                // Paso 3: Eliminar todas las equivalencias existentes asociadas al producto
                if (producto.ProductoEquivalencias != null && producto.ProductoEquivalencias.Count > 0)
                {
                    foreach (var equivalenciaBd in producto.ProductoEquivalencias)
                    {
                        equivalenciaBd.Eliminada = true;
                    }
                }

                // Paso 4: Agregar equivalencias nuevas del formulario
                if (model.Equivalencias != null && model.Equivalencias.Count > 0)
                {
                    foreach (var equivalenciaNueva in model.Equivalencias)
                    {
                        // Crear una nueva equivalencia y asociarla al producto
                        var equivalenciaAgregada = new ProductoEquivalencia
                        {
                            UnidadMedidaCompraId = equivalenciaNueva.UnidadMedidaCompraId,
                            UnidadMedidaVentaId = equivalenciaNueva.UnidadMedidaVentaId,
                            CantidadEquivalenteDestino = equivalenciaNueva.CantidadEquivalente,
                            Eliminada = false // Asegurar que la nueva equivalencia no esté marcada como eliminada
                        };
                        producto.ProductoEquivalencias.Add(equivalenciaAgregada);
                    }
                }

                // Paso 5: Actualizar el producto en el repositorio
                _productosRepository.Update(producto);

                // Paso 6: Mensaje de confirmación de éxito
                TempData["Message"] = "¡Cambios guardados!";

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al modificar producto. " + ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost]
        public string InventarioProductoEliminarRegistroInventario(int? registroId)
        {
            if (registroId == null)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de peticion"
                });
            }

            var model = _productosRepository.GetRegistroInventarioProducto((int)registroId);

            if (model == null)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Producto no existente"
                });
            }

            model.Eliminado = true;
            _productosRepository.Update(model);

            TempData["Message"] = "¡Registro eliminado!";
            return JsonSerializer.Serialize(new
            {
                Exitoso = true
            });
        }

        #endregion



        #region Consulta de equivalencias


        [HttpPost]
        public string ConsultarEquivalencias(int? productoId)
        {
            try
            {
                var listaEquivalencias = new List<ProductoEquivalenciaViewModel>();

                var equivalenciasProducto = _productosRepository.GetEquivalenciasProducto((int)productoId);
                if (equivalenciasProducto != null || equivalenciasProducto.Count > 0)
                {
                    foreach (var equivalencia in equivalenciasProducto)
                    {
                        var equivalenciaProducto = new ProductoEquivalenciaViewModel
                        {
                            Id = equivalencia.Id,
                            ProductoId = equivalencia.ProductoId,
                            CantidadEquivalente = equivalencia.CantidadEquivalenteDestino,
                            UnidadMedidaCompraId = (int)equivalencia.UnidadMedidaCompraId,
                            UnidadMedidaCompraNombre = equivalencia.UnidadMedidaCompra.Nombre + " (" +
                            equivalencia.UnidadMedidaCompra.Abreviatura + ")",
                            UnidadMedidaVentaId = (int)equivalencia.UnidadMedidaVentaId,
                            UnidadMedidaVentaNombre = equivalencia.UnidadMedidaVenta.Nombre + " (" +
                            equivalencia.UnidadMedidaVenta.Abreviatura + ")",
                        };
                        listaEquivalencias.Add(equivalenciaProducto);
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaEquivalencias
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar equivalencias. " + ex.Message
                });
            }
        }


        #endregion




        [Authorize]
        public JsonResult RetornarProducto(string codigo)
        {
            // para depurar
            var productoBuscadoList = _productosRepository.GetPorNumeroDeReferenciaList(codigo);

            var productoBuscado = _productosRepository.GetPorNumeroDeReferencia(codigo);

            if (productoBuscado == null)
            {
                return new JsonErrorResult(new { message = "" });
            }

            return Json(productoBuscado);
        }

        [Authorize]
        public JsonResult RetornarProductoClinica(string codigo)
        {
            var productoBuscado = _productosRepository.GetPorNumeroDeReferenciaClinica(codigo);

            if (productoBuscado == null)
            {
                return new JsonErrorResult(new { message = "" });
            }

            return Json(productoBuscado);
        }

        [Authorize]
        public JsonResult RetornarProductoById(int id)
        {
            var productoBuscado = _productosRepository.GetProdutoById(id);

            if (productoBuscado == null)
            {
                return new JsonErrorResult(new { message = "" });
            }

            return Json(productoBuscado);
        }


        [Authorize]
        public JsonResult RetornarProductoCotizacion(string codigo)
        {
            var productoBuscado = _productosRepository.GetPorNumeroDeReferenciayNombre(codigo, false);

            if (productoBuscado == null)
            {
                return new JsonErrorResult(new { message = "" });
            }


            return Json(productoBuscado);
        }

        [Authorize]
        public JsonResult RetornarProductoLista()
        {
            var productos = _productosRepository.GetListParaCotizacion();
            return Json(productos);
        }

        [Authorize]
        public IActionResult Importar()
        {
            return View();
        }


        [Authorize]
        public IActionResult HistoricoProductos()
        {

            //var movimientos = _context.MovimientosProducto.AsQueryable();


            //if (!string.IsNullOrEmpty(fecha))
            //{
            //    var fechas = fecha.Split(" - ");
            //    if (fechas.Length == 2)
            //    {
            //        DateTime fechaInicial = DateTime.Parse(fechas[0]);
            //        DateTime fechaFinal = DateTime.Parse(fechas[1]);
            //        movimientos = movimientos.Where(m => m.Fecha >= fechaInicial && m.Fecha <= fechaFinal);
            //    }
            //}

            //if (tipoProductoId.HasValue)
            //{
            //    movimientos = movimientos.Where(m => m.TipoProducto == tipoProductoId.Value);
            //}

            //if (!string.IsNullOrEmpty(ambiente))
            //{
            //    movimientos= movimientos.Where(m => m.Ambiente.Contains(ambiente));
            //}

            //if (!string.IsNullOrEmpty(bodega))
            //{
            //    movimientos = movimientos.Where(m => m.Bodega.Contains(bodega));
            //}
            //if (!string.IsNullOrEmpty(producto))
            //{
            //    movimientos = movimientos.Where(m => m.Producto.Contains(producto));
            //}
            //var movimientosProduct = await movimientos.ToListAsync();
            var viewModel = new MovimientoProductoViewModel
            {
                //MovimientosProducto = movimientosProduct,
                TipoProductoSelectList = new SelectList(_context.TipoProductos.ToList(), "Id", "NombreTipoProducto")
            };

            return View(viewModel);


        }

        [Authorize]
        public IActionResult HistoricoProductosNacional()
        {

            //var movimientos = _context.MovimientosProducto.AsQueryable();


            //if (!string.IsNullOrEmpty(fecha))
            //{
            //    var fechas = fecha.Split(" - ");
            //    if (fechas.Length == 2)
            //    {
            //        DateTime fechaInicial = DateTime.Parse(fechas[0]);
            //        DateTime fechaFinal = DateTime.Parse(fechas[1]);
            //        movimientos = movimientos.Where(m => m.Fecha >= fechaInicial && m.Fecha <= fechaFinal);
            //    }
            //}

            //if (tipoProductoId.HasValue)
            //{
            //    movimientos = movimientos.Where(m => m.TipoProducto == tipoProductoId.Value);
            //}

            //if (!string.IsNullOrEmpty(ambiente))
            //{
            //    movimientos= movimientos.Where(m => m.Ambiente.Contains(ambiente));
            //}

            //if (!string.IsNullOrEmpty(bodega))
            //{
            //    movimientos = movimientos.Where(m => m.Bodega.Contains(bodega));
            //}
            //if (!string.IsNullOrEmpty(producto))
            //{
            //    movimientos = movimientos.Where(m => m.Producto.Contains(producto));
            //}
            //var movimientosProduct = await movimientos.ToListAsync();
            var viewModel = new MovimientoProductoNacionalViewModel
            {
                //MovimientosProducto = movimientosProduct,
                TipoProductoSelectList = new SelectList(_context.TipoProductos.ToList(), "Id", "NombreTipoProducto")
            };

            Console.WriteLine(viewModel + " !!!");

            return View(viewModel);


        }


        // [HttpPost]
        // public string ConsultarHistoricoProductos(DateTime? fechaInicio = null, DateTime? fechaFin = null, List<int> productosIds = null)
        // {
        //     try
        //     {
        //         var historico = _productosService.GetHistoricoProductos(DateTime.Now, DateTime.Now, productosIds);
        //         return JsonSerializer.Serialize(new
        //         {
        //             Exitoso = true,
        //             Resultado = historico
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return JsonSerializer.Serialize(new
        //         {
        //             Exitoso = false,
        //             Mensaje = "Error al consultar historico de productos. " + ex.Message
        //         });
        //     }
        // }

        [HttpPost]
        public string ConsultarHistoricoProductos(DateTime? fechaInicio = null, DateTime? fechaFin = null, List<int> productosIds = null)
        {
            try
            {
                var historico = _productosService.GetHistoricoProductos(fechaInicio, fechaFin, productosIds);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = historico
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar historico de productos. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarHistoricoProductosNacional(DateTime? fechaInicio = null, DateTime? fechaFin = null, List<int> productosIds = null)
        {
            try
            {
                var historico = _productosService.GetHistoricoProductosNacional(fechaInicio, fechaFin, productosIds);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = historico
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar historico de productos nacional. " + ex.Message
                });
            }
        }



        public IActionResult HistoricoProductosExcel(
      DateTime? fechaInicio = null,
      DateTime? fechaFin = null,
      List<int> productosIds = null,
      string tiposProducto = null,
      string ambientes = null,
      string bodegas = null,
      string productos = null
  )
        {
            // 1) Consulta según parámetros reales (no “hoy”)
            var historico = _productosService.GetHistoricoProductos(fechaInicio, fechaFin, productosIds);

            // 2) Aplicar filtros igual que la vista (KO)
            historico = AplicarFiltrosPorNombre(historico, tiposProducto, ambientes, bodegas, productos);

            var row = 1;

            using (var workboook = new XLWorkbook())
            {
                var worksheet = workboook.Worksheets.Add("Historico de productos");

                // Encabezados
                worksheet.Cell(row, 1).Value = "Fecha";
                worksheet.Cell(row, 2).Value = "Ambiente";
                worksheet.Cell(row, 3).Value = "Bodega";
                worksheet.Cell(row, 4).Value = "Tipo de movimiento";
                worksheet.Cell(row, 5).Value = "Descripcion de movimiento";
                worksheet.Cell(row, 6).Value = "Tipo de producto";
                worksheet.Cell(row, 7).Value = "Producto";
                worksheet.Cell(row, 8).Value = "Lote";
                worksheet.Cell(row, 9).Value = "Unidad";
                worksheet.Cell(row, 10).Value = "Cantidad";
                worksheet.Cell(row, 11).Value = "Saldo final";
                worksheet.Cell(row, 12).Value = "Facturado";

                // Datos
                foreach (var movimiento in historico)
                {
                    row++;
                    worksheet.Cell(row, 1).Value = movimiento.Fecha;            // DateTime
                    worksheet.Cell(row, 2).Value = movimiento.AmbienteNombre;
                    worksheet.Cell(row, 3).Value = movimiento.BodegaNombre;
                    worksheet.Cell(row, 4).Value = movimiento.TipoMovimientoNombre;
                    worksheet.Cell(row, 5).Value = movimiento.DescripcionMovimiento;
                    worksheet.Cell(row, 6).Value = movimiento.TipoProductoNombre;
                    worksheet.Cell(row, 7).Value = movimiento.Medicamento;
                    worksheet.Cell(row, 8).Value = movimiento.Lote;
                    worksheet.Cell(row, 9).Value = movimiento.UnidadNombre;
                    worksheet.Cell(row, 10).Value = movimiento.Cantidad;
                    worksheet.Cell(row, 11).Value = movimiento.SaldoActual;
                    worksheet.Cell(row, 12).Value = "-";
                }

                // (Opcional, pero ayuda) Formato de fecha igual a la vista
                worksheet.Column(1).Style.DateFormat.Format = "dd-MM-yyyy HH:mm";

                // (Opcional) Ajustar ancho a contenido
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workboook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Historico de productos.xlsx"
                    );
                }
            }
        }

        private static List<MovimientoProductoViewModel> AplicarFiltrosPorNombre(
            List<MovimientoProductoViewModel> historico,
            string tiposProducto,
            string ambientes,
            string bodegas,
            string productos
        )
        {
            if (historico == null || historico.Count == 0)
                return historico ?? new List<MovimientoProductoViewModel>();

            static string Norm(string s) => (s ?? string.Empty).Trim().ToLowerInvariant();

            static HashSet<string> Parse(string s)
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

        [Authorize]
        public IActionResult ExportarProductos()
        {
            var productos = _productosRepository.GetList();

            using (var workboook = new XLWorkbook())
            {
                var worksheet = workboook.Worksheets.Add("Productos");
                var row = 1;
                worksheet.Cell(row, 1).Value = "CodigoReferencia";
                worksheet.Cell(row, 2).Value = "NombreProducto";
                worksheet.Cell(row, 3).Value = "Descripcion";
                worksheet.Cell(row, 4).Value = "CategoriaId";
                worksheet.Cell(row, 5).Value = "PrecioCosto";
                worksheet.Cell(row, 6).Value = "PrecioVenta";
                worksheet.Cell(row, 7).Value = "PrecioDeFardo";
                worksheet.Cell(row, 8).Value = "PrecioClienteEspecial";
                worksheet.Cell(row, 9).Value = "PrecioCuentasClave";
                worksheet.Cell(row, 10).Value = "PrecioModificable";
                worksheet.Cell(row, 11).Value = "PrecioMayorista";
                worksheet.Cell(row, 12).Value = "StockIncial";
                worksheet.Cell(row, 13).Value = "Stock";
                worksheet.Cell(row, 14).Value = "FechaVencimiento";


                foreach (var prod in productos)
                {
                    row++;
                    worksheet.Cell(row, 1).Value = prod.CodigoReferencia; ;
                    worksheet.Cell(row, 2).Value = prod.NombreProducto;
                    worksheet.Cell(row, 3).Value = prod.Descripcion;
                    worksheet.Cell(row, 4).Value = prod.ViadminId;
                    worksheet.Cell(row, 5).Value = prod.PrecioCosto;
                    worksheet.Cell(row, 6).Value = prod.Precio;
                    worksheet.Cell(row, 7).Value = prod.Precio_3;
                    worksheet.Cell(row, 8).Value = prod.Precio_4;
                    worksheet.Cell(row, 9).Value = prod.Precio_5;
                    worksheet.Cell(row, 10).Value = prod.Precio_6;
                    worksheet.Cell(row, 11).Value = prod.Precio_2;
                    worksheet.Cell(row, 12).Value = prod.StockInical;
                    worksheet.Cell(row, 13).Value = prod.Stock;
                    worksheet.Cell(row, 14).Value = prod.FechaVencimiento;

                }

                using (var stream = new MemoryStream())
                {
                    workboook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officefocument.spreadsheet",
                    "Productos.xlsx"
                    );

                }
            }
        }

        [HttpPost]
        public string GetLotesExistentesInventario()
        {
            try
            {
                var lotes = _productosRepository.GetLotes();
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = lotes
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar lotes existentes. " + ex.Message
                });
            }
        }


        [Authorize]
        public IActionResult ExportarProductosInventario(InventarioViewModel model)
        {
            var viewModel = new InventarioViewModel
            {
                AmbienteId = model.AmbienteId,
                TipoProductoId = model.TipoProductoId,
                BodegaId = model.BodegaId
            };
            var productos = _productosRepository.GetInventarioProductos(
                null,
                model.TipoProductoId,
                null,
                model.terapeuticoId,
                model.sucursalId,
                model.BodegaId,
                model.AmbienteId
                );

            using (var workboook = new XLWorkbook())
            {
                var worksheet = workboook.Worksheets.Add("Productos");
                var row = 1;
                worksheet.Cell(row, 1).Value = "CodigoReferencia";
                worksheet.Cell(row, 2).Value = "NombreProducto";
                worksheet.Cell(row, 3).Value = "Descripcion";
                worksheet.Cell(row, 4).Value = "CategoriaId";
                worksheet.Cell(row, 5).Value = "PrecioCosto";
                worksheet.Cell(row, 6).Value = "PrecioVenta";
                //worksheet.Cell(row, 7).Value = "PrecioDeFardo";
                //worksheet.Cell(row, 8).Value = "PrecioClienteEspecial";
                //worksheet.Cell(row, 9).Value = "PrecioCuentasClave";
                //worksheet.Cell(row, 10).Value = "PrecioModificable";
                //worksheet.Cell(row, 11).Value = "PrecioMayorista";
                worksheet.Cell(row, 12).Value = "StockIncial";
                worksheet.Cell(row, 13).Value = "Stock";
                worksheet.Cell(row, 14).Value = "FechaVencimiento";


                foreach (var prod in productos)
                {
                    row++;
                    worksheet.Cell(row, 1).Value = prod.CodigoReferencia; ;
                    worksheet.Cell(row, 2).Value = prod.NombreProducto;
                    worksheet.Cell(row, 3).Value = prod.Descripcion;
                    worksheet.Cell(row, 4).Value = prod.ViadminId;
                    worksheet.Cell(row, 5).Value = prod.PrecioCosto;
                    worksheet.Cell(row, 6).Value = prod.Precio;
                    //worksheet.Cell(row, 7).Value = prod.Precio_3;
                    //worksheet.Cell(row, 8).Value = prod.Precio_4;
                    //worksheet.Cell(row, 9).Value = prod.Precio_5;
                    //worksheet.Cell(row, 10).Value = prod.Precio_6;
                    //worksheet.Cell(row, 11).Value = prod.Precio_2;
                    worksheet.Cell(row, 12).Value = prod.StockInical;
                    worksheet.Cell(row, 13).Value = prod.Stock;
                    worksheet.Cell(row, 14).Value = prod.FechaVencimiento;

                }

                using (var stream = new MemoryStream())
                {
                    workboook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officefocument.spreadsheet",
                    "Productos.xlsx"
                    );

                }
            }
        }


        public IActionResult VencidosFarmacia(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _productosRepository.PaginacionVencidosFarmacia(sortOrder, buscar, pageNumber, 35);

            return View(lista);
        }

        public IActionResult ProximosAVencerFarmacia(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _productosRepository.PaginacionProximosAVencerFarmacia(sortOrder, buscar, pageNumber, 35);

            return View(lista);
        }


        public IActionResult VencidosClinica(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _productosRepository.PaginacionVencidosClinica(sortOrder, buscar, pageNumber, 25);

            return View(lista);
        }

        public IActionResult ProximosAVencerClinica(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _productosRepository.PaginacionProximosAVencerClinica(sortOrder, buscar, pageNumber, 25);

            return View(lista);
        }

        public IActionResult ImportarDesdeExcel(InventarioViewModel model)
        {

            var viewModel = new InventarioViewModel
            {
                AmbienteId = model.AmbienteId,
                TipoProductoId = model.TipoProductoId,
                BodegaId = model.BodegaId
            };

            viewModel.Init(_categoryRepository, _sucursalRepository, _bodegaRepository, _categoryRepository, _ambienteRepository, _seguroRepository);


            return View(viewModel);
        }

        public IActionResult ImportarDesdeExcelSeguros(SegurosPreciosViewModel model)
        {

            var viewModel = new SegurosPreciosViewModel
            {
                // FechaEdicion = DateTime.Now,
                TipoPrecio = 0,
                TipoItem = 0
            };
            viewModel.Init(_seguroRepository);
            return View(viewModel);
        }

        //          public string ImportarInventarioExcel(int? BodegaId, int? TipoProductoId, IFormFile excel)
        //          {
        //              int row = 0;
        //              var codigoReferencia = "";
        //              string columna = "";
        //              try
        //              {
        //                  if (BodegaId == null)

        //                  {
        //                      return JsonSerializer.Serialize(new
        //                      {
        //                          Exitoso = false,
        //                          Mensaje = "No hay ninguna bodega seleccionada"
        //                      });
        //                  }
        //                  //Leer el archivo excel
        //                  var wokrbook = new XLWorkbook(excel.OpenReadStream());

        //                  var sheet = wokrbook.Worksheet(1);

        //                  var sheets = wokrbook.Worksheets.Count();
        //                  if (sheets > 1)
        //                  {
        //                      return JsonSerializer.Serialize(new
        //                      {
        //                          Exitoso = false,
        //                          Mensaje = "Debe cargar un excel con una unica Hoja"
        //                      });
        //                  }

        //                  #region Seleccion de AMBIENTE
        //                  int? ambienteId = null;
        //                  var bodega = _bodegaRepository.GetById((int)BodegaId);
        //                  if (bodega != null)
        //                  {
        //                      ambienteId = bodega.AmbienteId;
        //                  }
        //                  #endregion

        //                  var listProducto = new List<Producto>();
        //                  List<ProductoInventario> listProductoIventario = new List<ProductoInventario>();
        //                  var numeroFilas = sheet.LastRowUsed().RowNumber();
        //                  //foreach (var item in sheet.LastRowUsed().RowNumber())
        //                  for (row = 2; row <= numeroFilas; row++)
        //                  {
        //                      var item = sheet.Row(row);
        //                      //if (i > 9)
        //                      //{
        //                      columna = "Codigo referencia";
        //                      codigoReferencia = item.Cell(1).GetValue<string>();
        //                      columna = "Fecha de ingreso";
        //                      var fechaIngreso = item.Cell(2).GetValue<string>();
        //                      columna = "Nombre";
        //                      var nombre = item.Cell(3).GetValue<string>();
        //                      columna = "Descripcion";
        //                      var descripcion = item.Cell(4).GetValue<string>();
        //                      columna = "Activo Concentracion";
        //                      var activoConcentracion = item.Cell(5).GetValue<string>();
        //                      columna = "Casa medica";
        //                      var casaMedica = item.Cell(6).GetValue<string>();
        //                      columna = "Proveedor";
        //                      var proveedor = item.Cell(7).GetValue<string>();
        //                      columna = "Via de administración";
        //                      var viaAdministracion = item.Cell(8).GetValue<string>();
        //                      columna = "Stock por unidad";
        //                      var stockPorUnidad = string.IsNullOrEmpty(item.Cell(9).GetValue<string>()) == true ? 0 : item.Cell(9).GetValue<int>();
        //                      columna = "Stock minimo";
        //                      var stockMinimo = string.IsNullOrEmpty(item.Cell(10).GetValue<string>()) == true ? 0 : item.Cell(10).GetValue<int>();
        //                      columna = "Fecha de vencimiento";
        //                      var fechaVencimiento = item.Cell(11).GetValue<string>();
        //                      columna = "Unidad por compra";
        //                      var unidadPorCompra = item.Cell(12).GetValue<string>();
        //                      columna = "Unidad por venta";
        //                      var unidadPorVenta = item.Cell(13).GetValue<string>();
        //                      columna = "Equivalencia";
        //                      var equivalencia = string.IsNullOrEmpty(item.Cell(14).GetValue<string>()) == true ? 0 : item.Cell(14).GetValue<decimal>();
        //                      columna = "Precio de compra";
        //                      var precioCompra = item.Cell(15).GetValue<string>();
        //                      columna = "Precio PUBLICO";
        //                      var precioPublico = item.Cell(16).GetValue<string>();
        //                      columna = "Precio INTERNO";
        //                      var precioInterno = item.Cell(17).GetValue<string>();
        //                      columna = "Precio de VIP";
        //                      var precioVip = item.Cell(18).GetValue<string>();
        //                      columna = "Ubicacion";
        //                      var ubicacion = item.Cell(19).GetValue<string>();
        //                      columna = "Lote";
        //                      var lote = item.Cell(20).GetValue<string>();
        //                      columna = "Facturado";
        //                      var facturado = item.Cell(21).GetValue<string>();
        //                      columna = "Politicas de devolucion DEFECTUOSO";
        //                      var politicasDevolucion = item.Cell(22).GetValue<string>();
        //                      columna = "Politicas de devolucion VENCIMIENTO";
        //                      var politicasDevolucionVencimiento = item.Cell(23).GetValue<string>();
        //                      var presentacionProductoNombre = descripcion;


        //                      var laboratorioProducto = new LaboratorioProducto()
        //                      {
        //                          NombreLaboratorioProducto = casaMedica.Trim().ToUpper(),
        //                          Eliminado = false
        //                      };
        //                      var viadmin = new Viadmin()
        //                      {
        //                          NombreViadmin = viaAdministracion.Trim().ToUpper(),
        //                          Eliminado = false
        //                      };
        //                      var unidadMedidaVenta = new UnidadMedidaVenta()
        //                      {
        //                          Nombre = unidadPorVenta.Trim().ToUpper(),
        //                          Abreviatura = unidadPorVenta.Substring(0, 3).ToUpper().Trim()
        //                      };
        //                      var unidadMedidaCompra = new UnidadMedidaCompra()
        //                      {
        //                          Nombre = unidadPorCompra.Trim().ToUpper(),
        //                          Abreviatura = unidadPorCompra.Substring(0, 3).ToUpper().Trim()
        //                      };

        //                      var presentacionProducto = new PresentacionProducto()
        //                      {
        //                          PresentProducto = presentacionProductoNombre,
        //                          Eliminado = false
        //                      };

        //                      var productosExistentes = _productosRepository.GetList();

        //                      #region Objeto PRODUCTO
        //                      Producto producto;

        //                      if (productosExistentes.Any(a => a.NombreProducto == nombre))
        //                      {
        //                          producto = productosExistentes.Where(a => a.NombreProducto == nombre).FirstOrDefault();
        //                      }
        //                      else
        //                      {
        //                          producto = new Producto();
        //                          //producto.TipoBodegaId = _bodegaRepository.GetById((int)BodegaId).TipoBodegaId;
        //                          producto.ViadminId = _productosRepository.ExistOrAddViaAdmin(viadmin);
        //                          producto.TipoProductoId = TipoProductoId;
        //                          producto.GrupoTProductoId = null;
        //                          producto.PresentacionProductoId = _productosRepository.ExistOrAddPresentacionProducto(presentacionProducto);
        //                          producto.LaboratorioProductoId = _productosRepository.ExistOrAddLaboratorioProducto(laboratorioProducto);
        //                          producto.MarcaId = null;
        //                          producto.AmbienteId = ambienteId;
        //                          producto.CategoriaId = null;
        //                          producto.GrupoId = null;
        //                          producto.NombreProducto = nombre;
        //                          producto.CodigoReferencia = codigoReferencia;
        //                          producto.Imagen = "";
        //                          producto.Descripcion = descripcion;
        //                          producto.ActivoYConcentracion = activoConcentracion;
        //                          producto.Dosis = "";
        //                          //producto.FechaVencimiento = fechaVencimiento == "" ? new DateTime(1, 1, 0001) : Convert.ToDateTime(fechaVencimiento);
        //                          producto.Eliminado = false;
        //                          producto.Ubicacion = ubicacion;
        //                          if (producto.ProductosInventario == null)
        //                          {
        //                              producto.ProductosInventario = new List<ProductoInventario>();
        //                          }
        //                          productosExistentes.Add(producto);
        //                      }

        //                      #endregion




        //                      #region ProductoInventario
        //                      var precioCostoConvert = precioCompra != null ?
        //                          Convert.ToDecimal(precioCompra.Replace('.', ',').Replace('Q', ' '))
        //                          : 0;
        //                      DateTime? fechaRecLote;
        //                      if (fechaIngreso == "")
        //                          fechaRecLote = null;
        //                      else
        //                          fechaRecLote = Convert.ToDateTime(fechaIngreso);
        //                      producto.ProductosInventario.Add(new ProductoInventario
        //                      {
        //                          UnidadMedidaVentaId = _productosRepository.ExistOrAddUnidadMedidaVenta(unidadMedidaVenta),
        //                          UnidadMedidaCompraId = _productosRepository.ExistOrAddUnidadMedidaCompra(unidadMedidaCompra),
        //                          BodegaId = BodegaId,
        //                          Stock = stockPorUnidad,
        //                          Facturado = (facturado != null && facturado == "SI"),

        //                          PoliticaDevolucionPersonalizadaDias = int.TryParse(politicasDevolucion, out int diasDevolucion) 
        //                              ? diasDevolucion 
        //                              : 0,

        //                          ManejaPoliticaDevolucionPersonalizada = int.TryParse(politicasDevolucion, out _),

        //                          Lote = lote,
        //                          StockMinimo = stockMinimo,
        //                          Eliminado = false,
        //                          FechaRecepcionLote = fechaRecLote,
        //                          PrecioCosto = precioCostoConvert,

        //                          FechaVencimientoArticuloCompra = string.IsNullOrEmpty(fechaVencimiento) 
        //                              ? new DateTime(1, 1, 1) 
        //                              : Convert.ToDateTime(fechaVencimiento)
        //                      });


        //                      #endregion

        //                      #region PRODUCTO EQUIVALENCIAS
        //                      producto.ProductoEquivalencias.Add(new ProductoEquivalencia
        //                      {
        //                          CantidadEquivalenteDestino = equivalencia,
        //                          UnidadMedidaCompraId = _productosRepository.ExistOrAddUnidadMedidaCompra(unidadMedidaCompra),
        //                          UnidadMedidaVentaId = _productosRepository.ExistOrAddUnidadMedidaVenta(unidadMedidaVenta),
        //                          Eliminada = false
        //                      });
        //                      #endregion

        //                      _productosRepository.Add(producto);

        //                      #region PRECIOS

        //                      //var listProductoInventarioPrecios = new List<ProductoInventarioPrecio>();

        //                      var precioPublicoConvert = precioPublico != null ?
        //                          Convert.ToDecimal(precioPublico.Replace('.', ',').Replace('Q', ' '))
        //                          : 0;
        //                      var precioInternoConvert = precioInterno != null ?
        //                          Convert.ToDecimal(precioInterno.Replace('.', ',').Replace('Q', ' '))
        //                          : 0;
        //                      var precioVipConvert = precioVip != null ?
        //                          Convert.ToDecimal(precioVip.Replace('.', ',').Replace('Q', ' '))
        //                          : 0;
        //                      var precio1 = ProductoInventarioPrecios(precioPublicoConvert, 1);
        //                      var precio2 = ProductoInventarioPrecios(precioInternoConvert, 2);
        //                      var precio3 = ProductoInventarioPrecios(precioVipConvert, 3);

        //                      if (precio1 != null)
        //                      {
        //                          precio1.ProductoInventarioId = producto.ProductosInventario.FirstOrDefault().Id;
        //                          _productosRepository.Add(precio1);
        //                      }
        //                      if (precio2 != null)
        //                      {
        //                          precio2.ProductoInventarioId = producto.ProductosInventario.FirstOrDefault().Id;
        //                          _productosRepository.Add(precio2);
        //                      }
        //                      if (precio3 != null)
        //                      {
        //                          precio3.ProductoInventarioId = producto.ProductosInventario.FirstOrDefault().Id;
        //                          _productosRepository.Add(precio3);
        //                      }
        //                      #endregion
        //                  }

        //                  var viewModel = new InventarioViewModel
        //                  {
        //                      //AmbienteId = model.AmbienteId,
        //                      TipoProductoId = TipoProductoId,
        //                      BodegaId = BodegaId
        //                  };

        //                  viewModel.currentFilter = viewModel.buscar;
        //                  viewModel.Init(_categoryRepository, _sucursalRepository, _bodegaRepository, _categoryRepository, _ambienteRepository);
        //                  TempData["Message"] = "¡Inventario cargado correctamente!";

        //                  return JsonSerializer.Serialize(new { Exitoso = true });
        //              }
        //              catch (FormatException ex)
        //              {
        //                  return JsonSerializer.Serialize(new
        //                  {
        //                      Exitoso = false,
        //                      Mensaje = "Error de servidor al registrar producto con codigo de referencia " + codigoReferencia + " en la fila # " + row + ". " +
        //                          " Esta intentando agregar un valor con un tipo de dato diferente al permitido en la columna " + columna
        //                  });
        //              }
        //              catch (TimeoutException)
        //              {
        //                  return JsonSerializer.Serialize(new
        //                  {
        //                      Exitoso = false,
        //                      Mensaje = "Error de servidor, se ha excedio el tiempo de espera."
        //                  });
        //              }
        //              catch (Exception ex)
        //              {
        //                  return JsonSerializer.Serialize(new
        //                  {
        //                      Exitoso = false,
        //                      Mensaje = "Error de servidor al registrar producto con codigo de referencia "
        //                      + codigoReferencia + " en la fila # " + row + ". " + ex.Message
        //                  });
        //              }
        //              finally
        //              {
        //                  var data = excel.OpenReadStream();
        //                  data.Dispose();
        //              }
        //          }
        //  
        // [HttpPost]
        // public string ImportarInventarioExcel(int? BodegaId, int? TipoProductoId, IFormFile excel)
        // {
        //     int row = 0;
        //     var codigoReferencia = "";
        //     string columna = "";

        //     try
        //     {
        //         if (BodegaId == null)
        //         {
        //             return JsonSerializer.Serialize(new
        //             {
        //                 Exitoso = false,
        //                 Mensaje = "No hay ninguna bodega seleccionada"
        //             });
        //         }

        //         var wokrbook = new XLWorkbook(excel.OpenReadStream());
        //         var sheet = wokrbook.Worksheet(1);

        //         var sheets = wokrbook.Worksheets.Count();
        //         if (sheets > 1)
        //         {
        //             return JsonSerializer.Serialize(new
        //             {
        //                 Exitoso = false,
        //                 Mensaje = "Debe cargar un excel con una unica Hoja"
        //             });
        //         }

        //         #region Helpers locales (validaciones y parsing)
        //         string GetStr(IXLRow r, int col, string field, bool required = false)
        //         {
        //             columna = field;
        //             var val = r.Cell(col).GetValue<string>() ?? "";
        //             val = val.Trim();
        //             if (required && string.IsNullOrWhiteSpace(val))
        //                 throw new Exception($"El campo '{field}' está vacío.");
        //             return val;
        //         }

        //         int GetInt(IXLRow r, int col, string field, bool required = false, int defaultValue = 0)
        //         {
        //             columna = field;
        //             var s = r.Cell(col).GetValue<string>() ?? "";
        //             s = s.Trim();
        //             if (string.IsNullOrWhiteSpace(s))
        //             {
        //                 if (required) throw new Exception($"El campo '{field}' está vacío.");
        //                 return defaultValue;
        //             }
        //             if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n))
        //                 throw new Exception($"El campo '{field}' debe ser un entero válido. Valor: '{s}'.");
        //             return n;
        //         }

        //         decimal GetDec(IXLRow r, int col, string field, bool required = false, decimal defaultValue = 0m)
        //         {
        //             columna = field;
        //             var s = r.Cell(col).GetValue<string>() ?? "";
        //             s = s.Trim().Replace("Q", "", StringComparison.OrdinalIgnoreCase).Replace(" ", "");
        //             s = s.Replace(',', '.'); // normaliza coma a punto

        //             if (string.IsNullOrWhiteSpace(s))
        //             {
        //                 if (required) throw new Exception($"El campo '{field}' está vacío.");
        //                 return defaultValue;
        //             }

        //             if (!decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var d))
        //                 throw new Exception($"El campo '{field}' debe ser numérico. Valor: '{s}'.");
        //             return d;
        //         }

        //         DateTime? GetDate(IXLRow r, int col, string field, bool required = false)
        //         {
        //             columna = field;
        //             var s = r.Cell(col).GetValue<string>() ?? "";
        //             s = s.Trim();
        //             if (string.IsNullOrWhiteSpace(s))
        //             {
        //                 if (required) throw new Exception($"El campo '{field}' está vacío.");
        //                 return null;
        //             }

        //             if (r.Cell(col).TryGetValue<DateTime>(out var dtxl))
        //                 return dtxl;

        //             if (DateTime.TryParse(s, CultureInfo.GetCultureInfo("es-GT"), DateTimeStyles.None, out var dt))
        //                 return dt;
        //             if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
        //                 return dt;

        //             throw new Exception($"El campo '{field}' no tiene una fecha válida. Valor: '{s}'.");
        //         }

        //         string Abrev3(string s, string field)
        //         {
        //             columna = field;
        //             s = (s ?? "").Trim();
        //             if (string.IsNullOrWhiteSpace(s))
        //                 throw new Exception($"El campo '{field}' está vacío.");
        //             var len = Math.Min(3, s.Length);
        //             return s.Substring(0, len).ToUpperInvariant();
        //         }

        //         string UpperOrEmpty(string s) => (s ?? "").Trim().ToUpperInvariant();
        //         #endregion

        //         #region Seleccion de AMBIENTE
        //         int? ambienteId = null;
        //         var bodega = _bodegaRepository.GetById((int)BodegaId);
        //         if (bodega != null)
        //         {
        //             ambienteId = bodega.AmbienteId;
        //         }
        //         #endregion

        //         var numeroFilas = sheet.LastRowUsed()?.RowNumber() ?? 1;
        //         if (numeroFilas < 2)
        //         {
        //             return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "El archivo no contiene filas de datos." });
        //         }

        //         for (row = 2; row <= numeroFilas; row++)
        //         {
        //             var item = sheet.Row(row);

        //             // Corta el bucle si la fila está vacía (columna 1 vacía)
        //             var col1 = (item.Cell(1).GetValue<string>() ?? "").Trim();
        //             //if (string.IsNullOrEmpty(col1)) break;

        //             // Lectura y validaciones por columna

        //             // Campos STRING requeridos → usar "-" si vienen vacíos
        //             codigoReferencia = GetStr(item, 1, "Codigo referencia", required: false);
        //             if (string.IsNullOrWhiteSpace(codigoReferencia))
        //                 codigoReferencia = "-";

        //             var fechaIngreso = GetStr(item, 2, "Fecha de ingreso", required: false);

        //             var nombre = GetStr(item, 3, "Nombre", required: false);
        //             if (string.IsNullOrWhiteSpace(nombre))
        //                 nombre = "-";

        //             var descripcion = GetStr(item, 4, "Descripcion", required: false);
        //             if (string.IsNullOrWhiteSpace(descripcion))
        //                 descripcion = "-";

        //             var activoConcentracion = GetStr(item, 5, "Activo Concentracion", required: false);
        //             var casaMedica = GetStr(item, 6, "Casa medica", required: false);
        //             var proveedor = GetStr(item, 7, "Proveedor", required: false);
        //             var viaAdministracion = GetStr(item, 8, "Via de administración", required: false);

        //             // Campos NUMÉRICOS → ya tienen default 0
        //             var stockPorUnidad = GetInt(item, 9, "Stock por unidad", required: false, defaultValue: 0);
        //             var stockMinimo = GetInt(item, 10, "Stock minimo", required: false, defaultValue: 0);

        //             var fechaVencimiento = GetStr(item, 11, "Fecha de vencimiento", required: false);

        //             // Más STRING requeridos → usar "-"
        //             var unidadPorCompra = GetStr(item, 12, "Unidad por compra", required: false);
        //             if (string.IsNullOrWhiteSpace(unidadPorCompra))
        //                 unidadPorCompra = "-";

        //             var unidadPorVenta = GetStr(item, 13, "Unidad por venta", required: false);
        //             if (string.IsNullOrWhiteSpace(unidadPorVenta))
        //                 unidadPorVenta = "-";

        //             // Decimales → default 0m
        //             var equivalencia = GetDec(item, 14, "Equivalencia", required: false, defaultValue: 0m);

        //             // Precios vienen como string, los tratamos como NUMÉRICOS → "0" si vacío
        //             var precioCompra = GetStr(item, 15, "Precio de compra", required: false);
        //             if (string.IsNullOrWhiteSpace(precioCompra))
        //                 precioCompra = "0";

        //             var precioPublico = GetStr(item, 16, "Precio PUBLICO", required: false);
        //             if (string.IsNullOrWhiteSpace(precioPublico))
        //                 precioPublico = "0";

        //             var precioInterno = GetStr(item, 17, "Precio INTERNO", required: false);
        //             if (string.IsNullOrWhiteSpace(precioInterno))
        //                 precioInterno = "0";

        //             var precioVip = GetStr(item, 18, "Precio de VIP", required: false);
        //             if (string.IsNullOrWhiteSpace(precioVip))
        //                 precioVip = "0";

        //             var ubicacion = GetStr(item, 19, "Ubicacion", required: false);
        //             var lote = GetStr(item, 20, "Lote", required: false);
        //             var facturado = GetStr(item, 21, "Facturado", required: false);
        //             var politicasDevolucion = GetStr(item, 22, "Politicas de devolucion DEFECTUOSO", required: false);
        //             var politicasDevolucionVencimiento = GetStr(item, 23, "Politicas de devolucion VENCIMIENTO", required: false);


        //             // Instancias auxiliares
        //             var laboratorioProducto = new LaboratorioProducto()
        //             {
        //                 NombreLaboratorioProducto = UpperOrEmpty(casaMedica),
        //                 Eliminado = false
        //             };
        //             var viadmin = new Viadmin()
        //             {
        //                 NombreViadmin = UpperOrEmpty(viaAdministracion),
        //                 Eliminado = false
        //             };
        //             var unidadMedidaVenta = new UnidadMedidaVenta()
        //             {
        //                 Nombre = UpperOrEmpty(unidadPorVenta),
        //                 Abreviatura = Abrev3(unidadPorVenta, "Unidad por venta (abreviatura)")
        //             };
        //             var unidadMedidaCompra = new UnidadMedidaCompra()
        //             {
        //                 Nombre = UpperOrEmpty(unidadPorCompra),
        //                 Abreviatura = Abrev3(unidadPorCompra, "Unidad por compra (abreviatura)")
        //             };
        //             var presentacionProductoNombre = descripcion;
        //             var presentacionProducto = new PresentacionProducto()
        //             {
        //                 PresentProducto = presentacionProductoNombre,
        //                 Eliminado = false
        //             };

        //             var productosExistentes = _productosRepository.GetList();

        //             #region Objeto PRODUCTO
        //             Producto producto;
        //             if (productosExistentes.Any(a => a.NombreProducto == nombre))
        //             {
        //                 producto = productosExistentes.First(a => a.NombreProducto == nombre);
        //                 // **Fix NRE**: asegura colecciones si el producto ya existe
        //                 producto.ProductosInventario ??= new List<ProductoInventario>();
        //                 producto.ProductoEquivalencias ??= new List<ProductoEquivalencia>();
        //             }
        //             else
        //             {
        //                 producto = new Producto
        //                 {
        //                     ViadminId = _productosRepository.ExistOrAddViaAdmin(viadmin),
        //                     TipoProductoId = TipoProductoId,
        //                     GrupoTProductoId = null,
        //                     PresentacionProductoId = _productosRepository.ExistOrAddPresentacionProducto(presentacionProducto),
        //                     LaboratorioProductoId = _productosRepository.ExistOrAddLaboratorioProducto(laboratorioProducto),
        //                     MarcaId = null,
        //                     AmbienteId = ambienteId,
        //                     CategoriaId = null,
        //                     GrupoId = null,
        //                     NombreProducto = nombre,
        //                     CodigoReferencia = codigoReferencia,
        //                     Imagen = "",
        //                     Descripcion = descripcion,
        //                     ActivoYConcentracion = activoConcentracion,
        //                     Dosis = "",
        //                     Eliminado = false,
        //                     Ubicacion = ubicacion,
        //                     ProductosInventario = new List<ProductoInventario>(),
        //                     ProductoEquivalencias = new List<ProductoEquivalencia>()
        //                 };
        //                 productosExistentes.Add(producto);
        //             }
        //             #endregion

        //             #region ProductoInventario (crear y agregar)
        //             var precioCostoConvert = string.IsNullOrWhiteSpace(precioCompra)
        //                 ? 0m
        //                 : GetDec(item, 15, "Precio de compra", required: false, defaultValue: 0m);

        //             DateTime? fechaRecLote = string.IsNullOrWhiteSpace(fechaIngreso)
        //                 ? (DateTime?)null
        //                 : GetDate(item, 2, "Fecha de ingreso", required: false);

        //             var fechaVencCompra = string.IsNullOrWhiteSpace(fechaVencimiento)
        //                 ? new DateTime(1, 1, 1)
        //                 : GetDate(item, 11, "Fecha de vencimiento", required: false) ?? new DateTime(1, 1, 1);

        //             var inventario = new ProductoInventario
        //             {
        //                 UnidadMedidaVentaId = _productosRepository.ExistOrAddUnidadMedidaVenta(unidadMedidaVenta),
        //                 UnidadMedidaCompraId = _productosRepository.ExistOrAddUnidadMedidaCompra(unidadMedidaCompra),
        //                 BodegaId = BodegaId,
        //                 Stock = stockPorUnidad,
        //                 Facturado = (!string.IsNullOrWhiteSpace(facturado) && facturado.Trim().ToUpperInvariant() == "SI"),
        //                 PoliticaDevolucionPersonalizadaDias = int.TryParse(politicasDevolucion, out int diasDevolucion) ? diasDevolucion : 0,
        //                 ManejaPoliticaDevolucionPersonalizada = int.TryParse(politicasDevolucion, out _),
        //                 Lote = lote,
        //                 StockMinimo = stockMinimo,
        //                 Eliminado = false,
        //                 FechaRecepcionLote = fechaRecLote,
        //                 PrecioCosto = precioCostoConvert,
        //                 FechaVencimientoArticuloCompra = fechaVencCompra
        //             };

        //             producto.ProductosInventario.Add(inventario);
        //             #endregion

        //             #region PRODUCTO EQUIVALENCIAS
        //             producto.ProductoEquivalencias.Add(new ProductoEquivalencia
        //             {
        //                 CantidadEquivalenteDestino = equivalencia,
        //                 UnidadMedidaCompraId = _productosRepository.ExistOrAddUnidadMedidaCompra(unidadMedidaCompra),
        //                 UnidadMedidaVentaId = _productosRepository.ExistOrAddUnidadMedidaVenta(unidadMedidaVenta),
        //                 Eliminada = false
        //             });
        //             #endregion

        //             // Guardar producto (según tu repo, Add probablemente persiste; si no, haz SaveChanges aquí)
        //             _productosRepository.Add(producto);

        //             #region PRECIOS (solo si ya tengo Id de inventario)
        //             var inv = producto.ProductosInventario.LastOrDefault();
        //             if (inv == null || inv.Id <= 0)
        //             {
        //                 // Si tu repositorio NO guarda inmediatamente, descomenta la siguiente línea.
        //                 // _productosRepository.SaveChanges();
        //                 // Reintenta obtener el Id:
        //                 inv = producto.ProductosInventario.LastOrDefault();
        //             }

        //             if (inv != null && inv.Id > 0)
        //             {
        //                 var precioPublicoConvert = string.IsNullOrWhiteSpace(precioPublico) ? 0m : GetDec(item, 16, "Precio PUBLICO", required: false, defaultValue: 0m);
        //                 var precioInternoConvert = string.IsNullOrWhiteSpace(precioInterno) ? 0m : GetDec(item, 17, "Precio INTERNO", required: false, defaultValue: 0m);
        //                 var precioVipConvert = string.IsNullOrWhiteSpace(precioVip) ? 0m : GetDec(item, 18, "Precio de VIP", required: false, defaultValue: 0m);

        //                 var precio1 = ProductoInventarioPrecios(precioPublicoConvert, 1);
        //                 var precio2 = ProductoInventarioPrecios(precioInternoConvert, 2);
        //                 var precio3 = ProductoInventarioPrecios(precioVipConvert, 3);

        //                 if (precio1 != null) { precio1.ProductoInventarioId = inv.Id; _productosRepository.Add(precio1); }
        //                 if (precio2 != null) { precio2.ProductoInventarioId = inv.Id; _productosRepository.Add(precio2); }
        //                 if (precio3 != null) { precio3.ProductoInventarioId = inv.Id; _productosRepository.Add(precio3); }
        //             }
        //             else
        //             {
        //                 throw new Exception("No se pudo obtener el Id del inventario para asignar precios.");
        //             }
        //             #endregion
        //         }

        //         // (Opcional) mensaje de éxito
        //         TempData["Message"] = "¡Inventario cargado correctamente!";
        //         return JsonSerializer.Serialize(new { Exitoso = true });
        //     }
        //     catch (FormatException)
        //     {
        //         return JsonSerializer.Serialize(new
        //         {
        //             Exitoso = false,
        //             Mensaje = "Error de servidor al registrar producto con codigo de referencia " + codigoReferencia + " en la fila # " + row + ". " +
        //                       "Está intentando agregar un valor con un tipo de dato diferente al permitido en la columna " + columna + "."
        //         });
        //     }
        //     catch (TimeoutException)
        //     {
        //         return JsonSerializer.Serialize(new
        //         {
        //             Exitoso = false,
        //             Mensaje = "Error de servidor, se ha excedio el tiempo de espera."
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return JsonSerializer.Serialize(new
        //         {
        //             Exitoso = false,
        //             Mensaje = "Error de servidor al registrar producto con codigo de referencia " + codigoReferencia +
        //                       " en la fila # " + row + ". " + ex.Message
        //         });
        //     }
        //     finally
        //     {
        //         var data = excel.OpenReadStream();
        //         data.Dispose();
        //     }
        // }
        [HttpPost]
        public string ImportarInventarioExcel(int? BodegaId, int? TipoProductoId, IFormFile excel)
        {
            try
            {
                if (BodegaId == null)
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "No hay ninguna bodega seleccionada" });
                }

                if (excel == null || excel.Length == 0)
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "El archivo Excel es inválido o está vacío." });
                }

                // 1. Validar Anti-Duplicidad de Archivo (SHA-256)
                string hashArchivo = CalcularHashArchivo(excel);
                var archivoYaProcesado = _context.HistorialImportacionExcel
                    .FirstOrDefault(h => h.HashArchivo == hashArchivo);

                if (archivoYaProcesado != null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"Este archivo exacto ya fue importado exitosamente el {archivoYaProcesado.FechaImportacion:dd/MM/yyyy HH:mm}. No se permiten duplicados."
                    });
                }

                // 2. Preparar el DTO para la Session y Guardar Excel Temporalmente
                string sessionKey = Guid.NewGuid().ToString();
                var previsualizacionDto = new PrevisualizacionInventarioDto
                {
                    SessionKey = sessionKey,
                    HashArchivo = hashArchivo,
                    BodegaId = BodegaId.Value,
                    TipoProductoId = TipoProductoId ?? 0,
                    TieneErrores = false
                };

                // Guardamos el archivo físicamente para poder leer las 24 columnas en el POST de Confirmación
                var tempPath = Path.Combine(_hostEnvironment.WebRootPath, "temp");
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                var filePath = Path.Combine(tempPath, $"{sessionKey}.xlsx");

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    excel.CopyTo(fileStream);
                }

                // 3. Procesar el Excel (Leemos desde el temporal)
                using var workbook = new XLWorkbook(filePath);
                var sheet = workbook.Worksheet(1);

                if (workbook.Worksheets.Count() > 1)
                {
                    System.IO.File.Delete(filePath); // Limpiamos si hay error
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Debe cargar un excel con una única Hoja" });
                }

                var numeroFilas = sheet.LastRowUsed()?.RowNumber() ?? 1;
                if (numeroFilas < 2)
                {
                    System.IO.File.Delete(filePath); // Limpiamos si hay error
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "El archivo no contiene filas de datos." });
                }

                // Cargar productos existentes para validación de Lotes y Existencias
                var productosExistentes = _productosRepository.GetList();

                for (int row = 2; row <= numeroFilas; row++)
                {
                    var item = sheet.Row(row);
                    var codigoReferencia = (item.Cell(1).GetValue<string>() ?? "").Trim();

                    if (string.IsNullOrWhiteSpace(codigoReferencia)) continue;

                    var nombreProducto = (item.Cell(3).GetValue<string>() ?? "").Trim();
                    var lote = (item.Cell(20).GetValue<string>() ?? "").Trim();
                    var stockStr = (item.Cell(9).GetValue<string>() ?? "").Trim();
                    decimal stockASumar = string.IsNullOrWhiteSpace(stockStr) ? 0m : Convert.ToDecimal(stockStr);

                    // Regla de Negocio: Lote Referencial si viene vacío
                    if (string.IsNullOrWhiteSpace(lote) || lote == "-")
                    {
                        lote = $"LOTE-REF-{DateTime.Now:yyyyMMdd}-{codigoReferencia}";
                    }

                    // Validar existencia en Base de Datos
                    var productoBd = productosExistentes.FirstOrDefault(p => p.CodigoReferencia == codigoReferencia || p.NombreProducto == nombreProducto);
                    bool productoExiste = productoBd != null;
                    bool loteExiste = false;

                    if (productoExiste && productoBd.ProductosInventario != null)
                    {
                        // Si el producto existe, revisamos si este lote en esta bodega específica ya está registrado
                        loteExiste = productoBd.ProductosInventario.Any(inv => inv.Lote == lote && inv.BodegaId == BodegaId.Value && !inv.Eliminado);
                    }

                    // Llenar la fila del DTO para la previsualización
                    var filaDto = new FilaImportacionDto
                    {
                        FilaExcel = row,
                        CodigoReferencia = codigoReferencia,
                        NombreProducto = nombreProducto,
                        Lote = lote,
                        StockASumar = stockASumar,
                        ProductoExisteEnBd = productoExiste,
                        LoteExisteEnBd = loteExiste,
                        EsFilaValida = true
                    };

                    previsualizacionDto.Filas.Add(filaDto);
                }

                // 4. Guardar temporalmente en Session
                var jsonDto = JsonSerializer.Serialize(previsualizacionDto);
                HttpContext.Session.SetString($"ImportInv_{sessionKey}", jsonDto);

                // Retornar el objeto Data directamente a la vista para Knockout.js
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Data = previsualizacionDto
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = $"Error al procesar el archivo Excel: {ex.Message}"
                });
            }
        }
        [HttpPost]
        public string ImportarServiciosSeguros(int SeguroId, IFormFile excel)
        {
            int row = 0;
            string columna = "";

            try
            {
                var workbook = new XLWorkbook(excel.OpenReadStream());
                var sheet = workbook.Worksheet(1);

                // Obtener seguro por ID
                var seguro = _seguroRepository.GetSeguro(SeguroId);
                if (seguro == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"No se encontró un seguro con el ID '{SeguroId}'."
                    });
                }

                string nombreSeguro = seguro.Nombre;


                var nombreSeguroExcel = sheet.Cell("B1").GetValue<string>().Trim();

                // Validar nombre del seguro en Excel
                if (string.IsNullOrEmpty(nombreSeguroExcel) || nombreSeguroExcel != nombreSeguro)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"El nombre del seguro en el archivo ('{nombreSeguroExcel}') no coincide con el registrado ('{nombreSeguro}')."
                    });
                }


                // Obtener el precio relacionado con el seguro
                var precio = _preciosRepository.GetByName(nombreSeguro);
                if (precio == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"No se encontró un precio asociado al seguro '{nombreSeguro}'."
                    });
                }
                int precioId = precio.Id;

                // Lista para almacenar los servicios nuevos
                var servicios = new List<Servicio>();

                // Obtener el número de filas con datos en el Excel
                var numeroFilas = sheet.LastRowUsed().RowNumber();

                // Procesar filas desde la fila 2
                for (row = 3; row <= numeroFilas; row++)
                {
                    var item = sheet.Row(row);

                    // Leer valores de las columnas
                    columna = "CODIGO";
                    var codigoServicio = item.Cell(1).GetValue<string>().Trim();

                    columna = "NOMBRE";
                    var nombreServicio = item.Cell(2).GetValue<string>().Trim();

                    columna = "PRECIO";
                    var valorPrecio = item.Cell(3).GetValue<decimal>();

                    // Buscar si el servicio ya existe por su nombre
                    var servicioExistente = _servicioRepository.GetServicioPrecioSeguro(codigoServicio);
                    if (servicioExistente != null)
                    {
                        servicioExistente.NombreServicio = nombreServicio;
                        _servicioRepository.Update(servicioExistente);

                        var precioExistente = servicioExistente.ServiciosPrecios.FirstOrDefault(p => p.PrecioId == precioId);
                        if (precioExistente != null)
                        {
                            // Actualizar precio existente
                            Console.WriteLine($"El servicio '{nombreServicio}' ya tiene un precio registrado para el seguro '{nombreSeguro}', actualizando...");
                            precioExistente.Valor = valorPrecio;
                            _servicioRepository.UpdatePrecio(precioExistente);
                        }
                        else
                        {
                            // Si no tiene precio, se agrega uno nuevo
                            Console.WriteLine($"El servicio '{nombreServicio}' no tenía un precio para el seguro '{nombreSeguro}', agregando...");
                            servicioExistente.ServiciosPrecios.Add(new ServicioPrecio
                            {
                                PrecioId = precioId,
                                Valor = valorPrecio,
                                Activar = true
                            });
                            _servicioRepository.Update(servicioExistente);
                        }
                    }
                    else
                    {
                        // Crear un nuevo servicio si no existe
                        Console.WriteLine($"Creando nuevo servicio '{nombreServicio}'...");
                        var nuevoServicio = new Servicio
                        {
                            CodigoInterno = codigoServicio,
                            NombreServicio = nombreServicio,
                            PrecioId = precioId,
                            Eliminado = false,
                            ServiciosPrecios = new List<ServicioPrecio>
                    {
                        new ServicioPrecio
                        {
                            PrecioId = precioId,
                            Valor = valorPrecio,
                            Activar = true
                        }
                    }
                        };

                        servicios.Add(nuevoServicio);
                    }
                }

                // Guardar los nuevos servicios en la base de datos
                foreach (var servicio in servicios)
                {
                    _servicioRepository.Add(servicio);
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "¡Servicios cargados correctamente!"
                });
            }
            catch (FormatException)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = $"Error en el formato de la columna {columna} en la fila #{row}."
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = $"Error en la fila #{row}: {ex.Message}"
                });
            }
            finally
            {
                var data = excel.OpenReadStream();
                data.Dispose();
            }
        }

        [HttpPost]
        public string ImportarProductosSeguros(int SeguroId, int TipoProductoId, IFormFile excel)
        {
            int row = 0;
            string columna = "";

            try
            {
                var workbook = new XLWorkbook(excel.OpenReadStream());
                var sheet = workbook.Worksheet(1);

                // Obtener seguro por ID
                var seguro = _seguroRepository.GetSeguro(SeguroId);
                if (seguro == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"No se encontró un seguro con el ID '{SeguroId}'."
                    });
                }

                string nombreSeguro = seguro.Nombre;


                var nombreSeguroExcel = sheet.Cell("B1").GetValue<string>().Trim();

                // Validar nombre del seguro en Excel
                if (string.IsNullOrEmpty(nombreSeguroExcel) || nombreSeguroExcel != nombreSeguro)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"El nombre del seguro en el archivo ('{nombreSeguroExcel}') no coincide con el registrado ('{nombreSeguro}')."
                    });
                }

                // Obtener el precio relacionado con el seguro
                var precio = _preciosRepository.GetByName(nombreSeguro);
                if (precio == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"No se encontró un precio asociado al seguro '{nombreSeguro}'."
                    });
                }
                int precioId = precio.Id;

                // Lista para almacenar los productos
                var productos = new List<Producto>();

                // Obtener el número de filas con datos en el Excel
                var numeroFilas = sheet.LastRowUsed().RowNumber();

                // Procesar filas desde la fila 2
                for (row = 3; row <= numeroFilas; row++)
                {
                    var item = sheet.Row(row);

                    // Leer valores de las columnas
                    columna = "CODIGO";
                    var codigoProducto = item.Cell(1).GetValue<string>().Trim();

                    columna = "NOMBRE";
                    var nombreProducto = item.Cell(2).GetValue<string>().Trim();

                    columna = "PRECIO";
                    var valorPrecio = item.Cell(3).GetValue<decimal>();

                    // Buscar si el producto ya existe por su código
                    var productoExistente = _productosRepository.GetByCodigo(codigoProducto); // Método para buscar producto
                    if (productoExistente != null)
                    {
                        // Actualizar producto existente
                        productoExistente.NombreProducto = nombreProducto;

                        // Verificar o agregar precio
                        var productoInventario = productoExistente.ProductosInventario.FirstOrDefault();
                        if (productoInventario == null)
                        {
                            productoInventario = new ProductoInventario
                            {
                                ProductoId = productoExistente.Id,
                                Stock = 0,
                                PrecioCosto = 0,
                                ProductosInventarioPrecios = new List<ProductoInventarioPrecio>()
                            };
                            productoExistente.ProductosInventario.Add(productoInventario);
                        }

                        var precioExistente = productoInventario.ProductosInventarioPrecios.FirstOrDefault(p => p.PrecioId == precioId);
                        if (precioExistente != null)
                        {
                            precioExistente.Valor = valorPrecio;
                        }
                        else
                        {
                            productoInventario.ProductosInventarioPrecios.Add(new ProductoInventarioPrecio
                            {
                                PrecioId = precioId,
                                Valor = valorPrecio,
                                Eliminado = false
                            });
                        }

                        // Agregar equivalencias genéricas si no existen
                        if (!productoExistente.ProductoEquivalencias.Any())
                        {
                            productoExistente.ProductoEquivalencias.Add(new ProductoEquivalencia
                            {
                                UnidadMedidaVentaId = 1, // ID genérico
                                UnidadMedidaCompraId = 1, // ID genérico
                                CantidadEquivalenteDestino = 1, // Valor genérico
                                Eliminada = false
                            });
                        }

                        _productosRepository.Update(productoExistente); // Actualizar producto en la base de datos
                    }
                    else
                    {
                        // Crear un nuevo producto si no existe
                        var nuevoProducto = new Producto
                        {
                            CodigoReferencia = codigoProducto,
                            NombreProducto = nombreProducto,
                            Precio_5 = valorPrecio,
                            Eliminado = false,
                            AmbienteId = 6,
                            TipoProductoId = TipoProductoId,
                            Stock = 0,
                            ProductosInventario = new List<ProductoInventario>
                    {
                        new ProductoInventario
                        {
                            Stock = 1,
                            PrecioCosto = 0,
                            BodegaId = 14,
                            UnidadMedidaVentaId = 1,
                            UnidadMedidaCompraId = 1,
                            ProductosInventarioPrecios = new List<ProductoInventarioPrecio>
                            {
                                new ProductoInventarioPrecio
                                {
                                    PrecioId = precioId,
                                    Valor = valorPrecio,
                                    Eliminado = false
                                }
                            }
                        }
                    },
                            ProductoEquivalencias = new List<ProductoEquivalencia>
                    {
                        new ProductoEquivalencia
                        {
                            UnidadMedidaVentaId = 1, // ID genérico
                            UnidadMedidaCompraId = 1, // ID genérico
                            CantidadEquivalenteDestino = 1, // Valor genérico
                            Eliminada = false
                        }
                    }
                        };

                        productos.Add(nuevoProducto);
                    }
                }

                // Guardar los nuevos productos en la base de datos
                foreach (var producto in productos)
                {
                    _productosRepository.Add(producto);
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "¡Medicamentos cargados correctamente!"
                });
            }
            catch (FormatException)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = $"Error en el formato de la columna {columna} en la fila #{row}."
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = $"Error en la fila #{row}: {ex.Message}"
                });
            }
            finally
            {
                var data = excel.OpenReadStream();
                data.Dispose();
            }
        }


        [HttpPost]
        public string ImportarExamenesSeguros(int SeguroId, IFormFile excel)
        {
            int row = 0;
            string columna = "";

            try
            {


                var workbook = new XLWorkbook(excel.OpenReadStream());
                var sheet = workbook.Worksheet(1);

                var seguro = _seguroRepository.GetSeguro(SeguroId);
                if (seguro == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"No se encontró un seguro con el ID '{SeguroId}'."
                    });
                }

                string nombreSeguro = seguro.Nombre;


                var nombreSeguroExcel = sheet.Cell("B1").GetValue<string>().Trim();

                // Validar nombre del seguro en Excel
                if (string.IsNullOrEmpty(nombreSeguroExcel) || nombreSeguroExcel != nombreSeguro)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"El nombre del seguro en el archivo ('{nombreSeguroExcel}') no coincide con el registrado ('{nombreSeguro}')."
                    });
                }

                var precio = _preciosRepository.GetByName(nombreSeguro);
                if (precio == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"No se encontró un precio con el nombre '{nombreSeguro}'."
                    });
                }
                int precioId = precio.Id;

                // Lista para almacenar los exámenes
                var examenes = new List<ExamenLabClinico>();

                // Obtener el número de filas con datos
                var numeroFilas = sheet.LastRowUsed().RowNumber();

                // Procesar filas de exámenes desde la fila 2
                for (row = 3; row <= numeroFilas; row++)
                {
                    var item = sheet.Row(row);

                    // Leer valores de las columnas
                    columna = "CODIGO";
                    var codigoExamen = item.Cell(1).GetValue<string>().Trim();

                    columna = "NOMBRE";
                    var nombreExamen = item.Cell(2).GetValue<string>().Trim();

                    columna = "PRECIO";
                    var precioExamen = item.Cell(3).GetValue<decimal>();

                    // Buscar si el examen ya existe
                    var examenExistente = _laboratorioClinicoRepository.GetByCodigo(codigoExamen); // Método para buscar el examen
                    if (examenExistente != null)
                    {
                        // Editar el examen existente
                        examenExistente.NombreExamen = nombreExamen;
                        examenExistente.UltimaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        // Editar o agregar precio
                        var precioExistente = examenExistente.ExamenLabClinicosPrecios.FirstOrDefault(p => p.PrecioId == precioId);
                        if (precioExistente != null)
                        {
                            precioExistente.PrecioValor = precioExamen;
                        }
                        else
                        {
                            examenExistente.ExamenLabClinicosPrecios.Add(new ExamenLabClinicoPrecio
                            {
                                PrecioId = precioId,
                                PrecioValor = precioExamen
                            });
                        }

                        _laboratorioClinicoRepository.Update(examenExistente); // Actualizar en la base de datos
                    }
                    else
                    {
                        // Crear un nuevo examen si no existe
                        var nuevoExamen = new ExamenLabClinico
                        {
                            CodigoInterno = codigoExamen,
                            NombreExamen = nombreExamen,
                            FechaCreacion = DateTime.Now,
                            CategoriaLabClinicoId = 1, // Valor estático
                            UltimaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Eliminado = false,
                            Activo = true,
                            ExamenLabClinicosPrecios = new List<ExamenLabClinicoPrecio>
                    {
                        new ExamenLabClinicoPrecio
                        {
                            PrecioId = precioId,
                            PrecioValor = precioExamen
                        }
                    }
                        };

                        examenes.Add(nuevoExamen);
                    }
                }

                // Guardar los nuevos exámenes en la base de datos
                foreach (var examen in examenes)
                {
                    _laboratorioClinicoRepository.Add(examen);
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "¡Exámenes cargados correctamente!"
                });
            }
            catch (FormatException)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = $"Error en el formato de la columna {columna} en la fila #{row}."
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = $"Error en la fila #{row}: {ex.Message}"
                });
            }
            finally
            {
                var data = excel.OpenReadStream();
                data.Dispose();
            }
        }


        private ProductoInventarioPrecio ProductoInventarioPrecios(decimal precio, int PrecioId)
        {
            if (precio != 0)
            {
                ProductoInventarioPrecio productoInventarioPrecio = new ProductoInventarioPrecio();
                productoInventarioPrecio.PrecioId = PrecioId;
                productoInventarioPrecio.Valor = Convert.ToDecimal(precio.ToString("F2"));
                return productoInventarioPrecio;
            }
            else
            {
                ProductoInventarioPrecio productoInventarioPrecio = null;
                return productoInventarioPrecio;
            }
        }

        private int ExisteProducto(Producto producto)
        {
            var productoExistente = _productosRepository.GetByName(producto.NombreProducto);
            if (productoExistente != null)
            {
                return productoExistente.Id;
            }
            else
            {
                return 0;
            }


        }

        [HttpPost]
        public string CargarInventarioExcelToBD()
        {
            try
            {

                TempData["Message"] = "¡Producto registrado!";

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al registrar producto. " + ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult ImportarDesdeExcel(IFormFile file)
        {
            IronXL.License.LicenseKey = "IRONXL.RAPH123007.31044-196C95EBA4-HQKBRQ-NRCMJN4U2ESS-V3WHT3FOV3EE-C2BP7NANSYLY-GQSSALBCK5T6-QTJFDBQ2D6TB-NB7IY5-T6HG27VPP5WEUA-DEPLOYMENT.TRIAL-ZRJCLZ.TRIAL.EXPIRES.09.APR.2022";

            if (file != null)
            {
                var rootFolder = _hostEnvironment.WebRootPath;
                var filename = "meds.xlsx";
                var filepath = Path.Combine(rootFolder, filename);

                var workbook = WorkBook.Load(filepath);
                var sheet = workbook.WorkSheets[0];
                string val = sheet.Rows[2].Columns[2].Value.ToString();

                for (var y = 2; y <= 121; y++)
                {
                    var cells = sheet[$"A{y}:M{y}"].ToList();

                    var producto = _productosRepository.GetByName(cells[1].StringValue, false);

                    if (producto != null) continue; // si se repite se salta y no se lee lo que hay abajo

                    // el laboratorio es un texto, asi que si no existe hay que crearlo
                    // var laboratorio = _categoryRepository.GetLaboratorioPorNombre(cells[5].StringValue);
                    Viadmin Viadmin = null;
                    PresentacionProducto presentacion = null;
                    GrupoTProducto GrupoT = null;
                    LaboratorioProducto lab = null;
                    //DateTime? fech = null;
                    int stock = 0;
                    decimal PrecioNormal = 0;
                    //decimal PrecioFamiliar = 0;
                    decimal PrecioCosto = 0;

                    // pendiente
                    // if(laboratorio == null)
                    // {
                    //     var nuevoLab = new LaboratorioProducto()
                    //     {
                    //         NombreLaboratorioProducto = cells[5].StringValue
                    //     };                 

                    //     _categoryRepository.Add(nuevoLab, false);
                    //     cells[5].StringValue = nuevoLab.Id.ToString();
                    // }

                    // precio normal
                    if (string.IsNullOrEmpty(cells[7].StringValue))
                    {
                        cells[8].StringValue = "0.00";
                    }

                    // // precio plan familiar
                    // if(string.IsNullOrEmpty(cells[9].StringValue))
                    // {
                    //     cells[9].StringValue = "0.00";
                    // }

                    // precio costo
                    if (string.IsNullOrEmpty(cells[8].StringValue))
                    {
                        cells[10].StringValue = "0.00";
                    }

                    // if(string.IsNullOrEmpty(cells[11].StringValue))
                    // {
                    //     fech = null;
                    // }
                    // else 
                    // {
                    //     var fecha = cells[11].StringValue;
                    //     var partesFecha = fecha.Split('-','.',',',' ').ToList();
                    //     partesFecha.Insert(1,"01");
                    //     var fechaNueva = string.Join("-", partesFecha);

                    //     fech = Convert.ToDateTime(fechaNueva);
                    // }

                    // // viAdmin
                    // if(!string.IsNullOrEmpty(cells[2].StringValue))
                    // {
                    //     Viadmin = Convert.ToInt32(cells[2].StringValue);
                    // }

                    var viadminn = _categoryRepository.GetViadminByname(cells[2].StringValue);

                    if (viadminn == null)
                    {
                        var nuevaViadmin = new Viadmin()
                        {
                            NombreViadmin = cells[2].StringValue
                        };

                        _categoryRepository.Add(nuevaViadmin);

                        Viadmin = nuevaViadmin;
                    }
                    else
                    {
                        Viadmin = viadminn;
                    }

                    // // Presentacion
                    // if(!string.IsNullOrEmpty(cells[3].StringValue))
                    // {
                    //     Presentacion = Convert.ToInt32(cells[3].StringValue);
                    // }

                    var pres = _categoryRepository.GetPresentacionProductoByName(cells[3].StringValue);

                    if (pres == null)
                    {
                        var nuevapres = new PresentacionProducto()
                        {
                            PresentProducto = cells[3].StringValue
                        };

                        _categoryRepository.Add(nuevapres);

                        presentacion = nuevapres;
                    }
                    else
                    {
                        presentacion = pres;
                    }

                    // // GrupoTerapeutico
                    // if(!string.IsNullOrEmpty(cells[4].StringValue))
                    // {
                    //     GrupoT = Convert.ToInt32(cells[4].StringValue);
                    // }

                    var grupot = _categoryRepository.GetGrupoTProductoByName(cells[4].StringValue);

                    if (grupot == null)
                    {
                        var nuevotrupot = new GrupoTProducto()
                        {
                            NombreGrupoT = cells[4].StringValue
                        };

                        _categoryRepository.Add(nuevotrupot);

                        GrupoT = nuevotrupot;
                    }
                    else
                    {
                        GrupoT = grupot;
                    }

                    // // Laboratorio
                    // if(!string.IsNullOrEmpty(cells[5].StringValue))
                    // {
                    //     lab = Convert.ToInt32(cells[5].StringValue);
                    // }

                    var labs = _categoryRepository.GetLaboratorioProductoByName(cells[5].StringValue);

                    if (labs == null)
                    {
                        var nuevolabs = new LaboratorioProducto()
                        {
                            NombreLaboratorioProducto = cells[5].StringValue
                        };

                        _categoryRepository.Add(nuevolabs);

                        lab = nuevolabs;
                    }
                    else
                    {
                        lab = labs;
                    }

                    // stock
                    if (!string.IsNullOrEmpty(cells[6].StringValue))
                    {
                        stock = Convert.ToInt32(cells[6].StringValue);
                    }

                    // precio normal
                    if (!string.IsNullOrEmpty(cells[7].StringValue))
                    {
                        PrecioNormal = Convert.ToDecimal(cells[7].StringValue);
                    }

                    // // precio familiar
                    // if(!string.IsNullOrEmpty(cells[9].StringValue))
                    // {
                    //     PrecioFamiliar = Convert.ToDecimal(cells[9].StringValue);
                    // }

                    // precio costo
                    if (!string.IsNullOrEmpty(cells[8].StringValue))
                    {
                        PrecioCosto = Convert.ToDecimal(cells[8].StringValue);
                    }

                    var medicamentoFarmacia = new Producto()
                    {
                        CodigoReferencia = cells[0].StringValue,
                        NombreProducto = cells[1].StringValue,
                        Viadmin = Viadmin,
                        PresentacionProducto = presentacion,
                        GrupoTProducto = GrupoT,
                        LaboratorioProducto = lab,
                        Stock = stock,
                        ActivoYConcentracion = cells[9].StringValue,
                        Dosis = cells[10] == null ? "" : cells[10].StringValue,
                        Precio_5 = PrecioNormal,
                        PrecioCosto = PrecioCosto,
                        TipoBodegaId = 2,
                        TipoProductoId = 10,
                    };

                    _productosRepository.Add(medicamentoFarmacia, false);
                }

                _productosRepository.SaveChanges();
                TempData["Message"] = "OK";
                return View();


            }

            return View();
        }

        [HttpPost]
        //public async Task<FileResult> exportarInventarioExcel()
        //{
        //    //var producto = _productosRepository.GetInventarioProductos
        //    return View();
        //}
        public IActionResult ImportarDesdeExcelPacientes()
        {
            IronXL.License.LicenseKey = "IRONXL.RAPH123007.31044-196C95EBA4-HQKBRQ-NRCMJN4U2ESS-V3WHT3FOV3EE-C2BP7NANSYLY-GQSSALBCK5T6-QTJFDBQ2D6TB-NB7IY5-T6HG27VPP5WEUA-DEPLOYMENT.TRIAL-ZRJCLZ.TRIAL.EXPIRES.09.APR.2022";

            bool is_licensed = IronXL.License.IsLicensed;
            ViewBag.Licencia = is_licensed;

            return View();
        }

        [HttpPost]
        public IActionResult ImportarDesdeExcelPacientes(IFormFile file)
        {
            IronXL.License.LicenseKey = "IRONXL.RAPH123007.31044-196C95EBA4-HQKBRQ-NRCMJN4U2ESS-V3WHT3FOV3EE-C2BP7NANSYLY-GQSSALBCK5T6-QTJFDBQ2D6TB-NB7IY5-T6HG27VPP5WEUA-DEPLOYMENT.TRIAL-ZRJCLZ.TRIAL.EXPIRES.09.APR.2022";

            if (file != null)
            {
                var rootFolder = _hostEnvironment.WebRootPath;
                var filename = "pacientes.xlsx";
                var filepath = Path.Combine(rootFolder, filename);

                var workbook = WorkBook.Load(filepath);
                var sheet = workbook.WorkSheets[0];
                string val = sheet.Rows[2].Columns[2].Value.ToString();

                for (var y = 2; y <= 24; y++)
                {
                    var cells = sheet[$"A{y}:H{y}"].ToList();

                    // var producto = _productosRepository.GetByName(cells[1].StringValue, false);

                    // if(producto != null) continue; // si se repite se salta y no se lee lo que hay abajo

                    // el laboratorio es un texto, asi que si no existe hay que crearlo
                    // var laboratorio = _categoryRepository.GetLaboratorioPorNombre(cells[5].StringValue);
                    // Viadmin Viadmin = null;
                    // PresentacionProducto presentacion = null;
                    // GrupoTProducto GrupoT = null;
                    // LaboratorioProducto lab = null;
                    // DateTime? fech = null;
                    // int stock = 0;
                    // decimal PrecioNormal = 0;
                    // decimal PrecioFamiliar = 0;
                    // decimal PrecioCosto = 0; 

                    // pendiente
                    // if(laboratorio == null)
                    // {
                    //     var nuevoLab = new LaboratorioProducto()
                    //     {
                    //         NombreLaboratorioProducto = cells[5].StringValue
                    //     };                 

                    //     _categoryRepository.Add(nuevoLab, false);
                    //     cells[5].StringValue = nuevoLab.Id.ToString();
                    // }

                    // precio normal
                    // if(string.IsNullOrEmpty(cells[7].StringValue))
                    // {
                    //     cells[8].StringValue = "0.00";
                    // }

                    // // precio plan familiar
                    // if(string.IsNullOrEmpty(cells[9].StringValue))
                    // {
                    //     cells[9].StringValue = "0.00";
                    // }

                    // precio costo
                    // if(string.IsNullOrEmpty(cells[8].StringValue))
                    // {
                    //     cells[10].StringValue = "0.00";
                    // }

                    // if(string.IsNullOrEmpty(cells[11].StringValue))
                    // {
                    //     fech = null;
                    // }
                    // else 
                    // {
                    //     var fecha = cells[11].StringValue;
                    //     var partesFecha = fecha.Split('-','.',',',' ').ToList();
                    //     partesFecha.Insert(1,"01");
                    //     var fechaNueva = string.Join("-", partesFecha);

                    //     fech = Convert.ToDateTime(fechaNueva);
                    // }

                    // // viAdmin
                    // if(!string.IsNullOrEmpty(cells[2].StringValue))
                    // {
                    //     Viadmin = Convert.ToInt32(cells[2].StringValue);
                    // }

                    // var viadminn = _categoryRepository.GetViadminByname(cells[2].StringValue);

                    // if(viadminn == null)
                    // {
                    //     var nuevaViadmin = new Viadmin()
                    //     {
                    //         NombreViadmin = cells[2].StringValue
                    //     };

                    //     _categoryRepository.Add(nuevaViadmin);

                    //     Viadmin = nuevaViadmin;
                    // }
                    // else {
                    //     Viadmin = viadminn;
                    // }

                    // // Presentacion
                    // if(!string.IsNullOrEmpty(cells[3].StringValue))
                    // {
                    //     Presentacion = Convert.ToInt32(cells[3].StringValue);
                    // }

                    // var pres = _categoryRepository.GetPresentacionProductoByName(cells[3].StringValue);

                    // if(pres == null)
                    // {
                    //     var nuevapres = new PresentacionProducto()
                    //     {
                    //         PresentProducto = cells[3].StringValue
                    //     };

                    //     _categoryRepository.Add(nuevapres);

                    //     presentacion = nuevapres;
                    // }
                    // else {
                    //     presentacion = pres;
                    // }

                    // // GrupoTerapeutico
                    // if(!string.IsNullOrEmpty(cells[4].StringValue))
                    // {
                    //     GrupoT = Convert.ToInt32(cells[4].StringValue);
                    // }

                    // var grupot = _categoryRepository.GetGrupoTProductoByName(cells[4].StringValue);

                    // if(grupot == null)
                    // {
                    //     var nuevotrupot = new GrupoTProducto()
                    //     {
                    //         NombreGrupoT = cells[4].StringValue
                    //     };

                    //     _categoryRepository.Add(nuevotrupot);

                    //     GrupoT = nuevotrupot;
                    // }
                    // else {
                    //     GrupoT = grupot;
                    // }

                    // // Laboratorio
                    // if(!string.IsNullOrEmpty(cells[5].StringValue))
                    // {
                    //     lab = Convert.ToInt32(cells[5].StringValue);
                    // }

                    // var labs = _categoryRepository.GetLaboratorioProductoByName(cells[5].StringValue);

                    // if(labs == null)
                    // {
                    //     var nuevolabs = new LaboratorioProducto()
                    //     {
                    //         NombreLaboratorioProducto = cells[5].StringValue
                    //     };

                    //     _categoryRepository.Add(nuevolabs);

                    //     lab = nuevolabs;
                    // }
                    // else {
                    //     lab = labs;
                    // }

                    // // stock
                    // if(!string.IsNullOrEmpty(cells[6].StringValue))
                    // {
                    //     stock = Convert.ToInt32(cells[6].StringValue);
                    // }

                    // precio normal
                    // if(!string.IsNullOrEmpty(cells[7].StringValue))
                    // {
                    //     PrecioNormal = Convert.ToDecimal(cells[7].StringValue);
                    // }

                    // // precio familiar
                    // if(!string.IsNullOrEmpty(cells[9].StringValue))
                    // {
                    //     PrecioFamiliar = Convert.ToDecimal(cells[9].StringValue);
                    // }

                    // precio costo
                    // if(!string.IsNullOrEmpty(cells[8].StringValue))
                    // {
                    //     PrecioCosto = Convert.ToDecimal(cells[8].StringValue);
                    // }

                    var nuevoPaciente = new Paciente()
                    {
                        Nombre = cells[0].StringValue,
                        Celular = cells[2].StringValue,
                        Nit = cells[3].StringValue,
                        Direccion = cells[4].StringValue,
                        SexoId = cells[6].StringValue == "F" ? 2 : 1,
                    };

                    _pacientesRepository.Add(nuevoPaciente, false);
                }

                _productosRepository.SaveChanges();
                TempData["Message"] = "OK";
                return View();


            }

            return View();
        }



        public IActionResult ImportarDesdeExcelInsumos()
        {
            IronXL.License.LicenseKey = "IRONXL.RAPH123007.31044-196C95EBA4-HQKBRQ-NRCMJN4U2ESS-V3WHT3FOV3EE-C2BP7NANSYLY-GQSSALBCK5T6-QTJFDBQ2D6TB-NB7IY5-T6HG27VPP5WEUA-DEPLOYMENT.TRIAL-ZRJCLZ.TRIAL.EXPIRES.09.APR.2022";

            bool is_licensed = IronXL.License.IsLicensed;
            ViewBag.Licencia = is_licensed;



            return View();
        }

        [HttpPost]
        public IActionResult ImportarDesdeExcelInsumos(IFormFile file)
        {
            IronXL.License.LicenseKey = "IRONXL.RAPH123007.31044-196C95EBA4-HQKBRQ-NRCMJN4U2ESS-V3WHT3FOV3EE-C2BP7NANSYLY-GQSSALBCK5T6-QTJFDBQ2D6TB-NB7IY5-T6HG27VPP5WEUA-DEPLOYMENT.TRIAL-ZRJCLZ.TRIAL.EXPIRES.09.APR.2022";

            if (file != null)
            {
                var rootFolder = _hostEnvironment.WebRootPath;
                var filename = "invprods.xlsx";
                var filepath = Path.Combine(rootFolder, filename);

                var workbook = WorkBook.Load(filepath);
                var sheet = workbook.WorkSheets[0];
                string val = sheet.Rows[2].Columns[2].Value.ToString();

                for (var y = 2; y <= 121; y++)
                {
                    var cells = sheet[$"A{y}:M{y}"].ToList();

                    // el laboratorio es un texto, asi que si no existe hay que crearlo
                    // var laboratorio = _categoryRepository.GetLaboratorioPorNombre(cells[5].StringValue);
                    Categoria categoria = null;
                    PresentacionProducto presentacion = null;
                    Grupo grupo = null;
                    int stock = 0;
                    Marca marca = null;
                    decimal precioNormal = 0;


                    // precio normal
                    if (string.IsNullOrEmpty(cells[7].StringValue))
                    {
                        cells[7].StringValue = "0.00";
                    }


                    // // Categoria
                    // if(!string.IsNullOrEmpty(cells[2].StringValue))
                    // {
                    //     categoria = Convert.ToInt32(cells[2].StringValue);
                    // }

                    // chequear si existe la categoria, sino crearla.
                    var cat = _categoryRepository.GetCategoriaByName(cells[2].StringValue);

                    if (cat == null)
                    {
                        var nuevacat = new Categoria()
                        {
                            NombreCategoria = cells[2].StringValue
                        };

                        _categoryRepository.Add(nuevacat);

                        categoria = nuevacat;
                    }
                    else
                    {
                        categoria = cat;
                    }


                    // // Presentacion
                    // if(!string.IsNullOrEmpty(cells[5].StringValue))
                    // {
                    //     presentacion = Convert.ToInt32(cells[5].StringValue);
                    // }

                    var pres = _categoryRepository.GetPresentacionProductoByName(cells[5].StringValue);

                    if (pres == null)
                    {
                        var nuevapres = new PresentacionProducto()
                        {
                            PresentProducto = cells[5].StringValue
                        };

                        _categoryRepository.Add(nuevapres);

                        presentacion = nuevapres;
                    }
                    else
                    {
                        presentacion = pres;
                    }

                    // // grupo
                    // if(!string.IsNullOrEmpty(cells[4].StringValue))
                    // {
                    //     grupo = Convert.ToInt32(cells[4].StringValue);
                    // }

                    var grup = _categoryRepository.GetGrupoByName(cells[4].StringValue);

                    if (pres == null)
                    {
                        var nuevogrupo = new Grupo()
                        {
                            NombreGrupo = cells[4].StringValue
                        };

                        _categoryRepository.Add(nuevogrupo);

                        grupo = nuevogrupo;
                    }
                    else
                    {
                        grupo = grup;
                    }


                    // stock
                    if (!string.IsNullOrEmpty(cells[6].StringValue))
                    {
                        stock = Convert.ToInt32(cells[6].StringValue);
                    }

                    // // marca
                    // if(!string.IsNullOrEmpty(cells[3].StringValue))
                    // {
                    //     marca = Convert.ToInt32(cells[3].StringValue);
                    // }

                    var marc = _categoryRepository.GetMarcaByName(cells[3].StringValue);

                    if (marc == null)
                    {
                        var nuevaMarca = new Marca()
                        {
                            NombreMarca = cells[3].StringValue
                        };

                        _categoryRepository.Add(nuevaMarca);

                        marca = nuevaMarca;
                    }
                    else
                    {
                        marca = marc;
                    }

                    // precio normal
                    if (!string.IsNullOrEmpty(cells[7].StringValue))
                    {
                        precioNormal = Convert.ToDecimal(cells[7].StringValue);
                    }

                    var nuevoInsumo = new Producto()
                    {
                        CodigoReferencia = cells[0].StringValue,
                        NombreProducto = cells[1].StringValue,
                        Categoria = categoria,
                        PresentacionProducto = presentacion,
                        Grupo = grupo,
                        Stock = stock,
                        Marca = marca,
                        Precio_5 = precioNormal,
                        TipoBodegaId = 2,
                        TipoProductoId = 11,
                    };

                    _productosRepository.Add(nuevoInsumo, false);
                }

                _productosRepository.SaveChanges();
                TempData["Message"] = "OK";
                return View();


            }

            return View();
        }

        [HttpPost]
        public JsonResult GetProductosLaboratorio(int? laboratorioId)
        {
            if (laboratorioId == null)
                return Json(_productosRepository.GetProductosLaboratorio());
            return Json(_productosRepository.GetProductosLaboratorio((int)laboratorioId));
        }

        // temporal eliminar pls
        public IActionResult ImportarMedicamentosFarmaciaABodega()
        {
            var listaProductosFarmacia = _productosRepository.GetMedicamentosFarmaciaList();
            // crear nuevo objeto para insertarlo a bodega

            foreach (var item in listaProductosFarmacia)
            {
                var nuevoProducto = new Producto()
                {
                    CodigoReferencia = item.CodigoReferencia,
                    NombreProducto = item.NombreProducto,
                    ViadminId = item.ViadminId,
                    PresentacionProductoId = item.PresentacionProductoId,
                    GrupoTProductoId = item.GrupoTProductoId,
                    LaboratorioProductoId = item.LaboratorioProductoId,
                    Stock = 0,
                    ActivoYConcentracion = item.ActivoYConcentracion,
                    Precio_5 = item.Precio_5,
                    Precio_6 = item.Precio_6,

                    PrecioCosto = item.PrecioCosto,
                    FechaVencimiento = item.FechaVencimiento,
                    TipoBodegaId = 3, // a bodega
                    TipoProductoId = 10, // medicamento
                    Eliminado = false
                };

                _productosRepository.Add(nuevoProducto, false);
            }

            _productosRepository.SaveChanges();

            TempData["Message"] = "OK";
            return View();
        }



        [Authorize]
        public IActionResult ListaFaltantesLaboratorio(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _productosRepository.PaginacionProductosFaltantesLab(sortOrder, buscar, pageNumber, 25);

            return View(lista);
        }

        public IActionResult VencidosLaboratorio(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _productosRepository.PaginacionVencidosLab(sortOrder, buscar, pageNumber, 25);

            return View(lista);
        }


        public IActionResult ProximosAVencerLaboratorio(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _productosRepository.PaginacionProximosAVencerLab(sortOrder, buscar, pageNumber, 25);

            return View(lista);
        }

        public async Task<IActionResult> FaltantesLab(string currentFilter)
        {

            var productos = _productosRepository.GetListadoFaltantesLab(currentFilter);
            return await _generatePdf.GetPdf("Views/Productos/FaltantesLaboratorio.cshtml", productos);
        }

        public IActionResult ModificarMedicamentosVencidos(int id)
        {

            try
            {
                if (id == 0)
                {
                    return BadRequest("Este producto no" +
                        "" +
                        " tiene inventarios");
                }

                var productoInventario = _productosRepository.GetStocksVencido(id);


                var tipoBodegaNombre = "";
                var tipoProductoNombre = "";

                switch (productoInventario.Bodega.TipoBodegaId)
                {
                    case (int)TipoBodegaEnum.Bodega:
                        tipoBodegaNombre = "Bodega";
                        break;
                    case (int)TipoBodegaEnum.Farmacia:
                        tipoBodegaNombre = "Farmacia";
                        break;
                    case (int)TipoBodegaEnum.Clinica:
                        tipoBodegaNombre = "Clinica";
                        break;
                    case (int)TipoBodegaEnum.Laboratorio:
                        tipoBodegaNombre = "Laboratorio";
                        break;
                    default:
                        tipoBodegaNombre = "Inventario";
                        break;
                }

                switch (productoInventario.Producto.TipoProductoId)
                {
                    case (int)TipoProductoEnum.InsumosMedicos:
                        tipoProductoNombre = "Insumos medicos";
                        break;
                    case (int)TipoProductoEnum.Medicamentos:
                        tipoProductoNombre = "Medicamentos";
                        break;
                    default:
                        tipoProductoNombre = "Inventario";
                        break;
                }

                var modelo = new InventarioProductoViewModel()
                {
                    Id = productoInventario.Id,
                    CodigoReferencia = productoInventario.Producto.CodigoReferencia,
                    CategoriaId = productoInventario.Producto.CategoriaId,
                    GrupoId = productoInventario.Producto.GrupoTProductoId,
                    MarcaId = productoInventario.Producto.MarcaId,
                    PresentacionId = productoInventario.Producto.PresentacionProductoId,
                    ViadminId = productoInventario.Producto.ViadminId,
                    LaboratorioId = productoInventario.Producto.LaboratorioProductoId,
                    Nombre = productoInventario.Producto.NombreProducto,
                    ActivoConcentracion = productoInventario.Producto.ActivoYConcentracion,
                    UrlImagen = productoInventario.Producto.Imagen,
                    Descripcion = productoInventario.Producto.Descripcion,
                    TipoProductoId = productoInventario.Producto.TipoProductoId,
                    FechaVencimiento = productoInventario.FechaVencimientoArticuloCompra,
                    Stock = productoInventario.Stock,
                    TipoProductoNombre = tipoProductoNombre,
                    TipoBodegaId = productoInventario.Bodega.TipoBodegaId,
                    TipoBodegaNombre = tipoBodegaNombre
                };

                modelo.Init(_categoryRepository);

                return View(modelo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ha ocurrido un error: {ex.Message}");
            }


        }

        [HttpPost]
        public string InventarioProductoModificarVencidos(int idProdictoInventario, DateTime fechaVencimiento)
        {
            try
            {
                var productoInventario = _productosRepository.GetStocksVencido(idProdictoInventario);

                productoInventario.FechaVencimientoArticuloCompra = fechaVencimiento;


                _productosRepository.Update(productoInventario);
                TempData["Message"] = "¡Cambios guardados!";

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al modificar producto. " + ex.Message
                });
            }
        }

        public IActionResult EliminarMedicamentosVencidos(int id, int? bodegaId)
        {

            try
            {
                var productoInventario = _productosRepository.GetStocksVencido(id);

                if (productoInventario != null)
                {
                    productoInventario.Eliminado = true;
                    _productosRepository.Update(productoInventario);
                    TempData["Message"] = "¡Stock del producto elimiminado correctamente!";

                    switch (bodegaId)
                    {
                        case 1:
                            return RedirectToAction("vencidosFarmacia", "Productos");
                        case 2:
                            return RedirectToAction("vencidosClinica", "Productos");
                        case 4:
                            return RedirectToAction("vencidosLaboratorio", "Productos");
                        default:
                            // Manejar el caso por defecto aquí, como redireccionar a una página de error
                            return RedirectToAction("vencidosFarmacia", "Productos");
                    }
                    // Redirige a la acción "vencidosFarmacia" del controlador "NombreControlador"
                    //return RedirectToAction("vencidosFarmacia", "Productos");
                }
                else
                {
                    TempData["Message"] = "No se encontró el producto en el inventario.";
                    return RedirectToAction("vencidosFarmacia", "Productos"); // Otra acción a la que desees redirigir
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                TempData["Error"] = "Ocurrió un error al eliminar el producto. " + ex.Message;
                return RedirectToAction("vencidosFarmacia", "Productos"); // Otra acción a la que desees redirigir
            }
        }



        #region ImportarExcel Laboratorio

        // GET: muestra el formulario
        public IActionResult ImportarDesdeLaboratorioClinico()
        {
            Console.WriteLine("[GET] ImportarDesdeLaboratorioClinico -> Render vista.");
            return View();
        }

        // POST (1): solo “arma” en memoria desde el Excel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ImportarDesdeLaboratorioClinico(IFormFile excel)
        {
            Console.WriteLine("[POST-ARMADO] Inicio.");
            try
            {
                if (excel is null || excel.Length == 0)
                {
                    Console.WriteLine("[POST-ARMADO] Archivo vacío o nulo.");
                    ModelState.AddModelError(string.Empty, "Debe seleccionar un archivo Excel válido.");
                    return View(); // misma vista
                }

                var resultado = new Dictionary<string, List<ItemCatalogo>>(StringComparer.OrdinalIgnoreCase);
                var correlativoPorCatalogo = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                using var wb = new XLWorkbook(excel.OpenReadStream());

                // Procesar hoja LABORATORIO (si existe)
                ProcesarHojaLaboratorio(wb, resultado, correlativoPorCatalogo);

                // Procesar hoja RX (si existe)
                ProcesarHojaRx(wb, resultado, correlativoPorCatalogo);

                // Procesar hoja ULTRASONIDO (si existe)
                ProcesarHojaUltrasonido(wb, resultado, correlativoPorCatalogo);

                if (resultado.Count == 0)
                {
                    Console.WriteLine("[POST-ARMADO] No se encontraron datos válidos en hojas 'LABORATORIO', 'RX' ni 'ULTRASONIDO'.");
                    ModelState.AddModelError(string.Empty, "El archivo no contiene datos válidos en las hojas 'LABORATORIO', 'RX' o 'ULTRASONIDO'.");
                    return View();
                }

                // Resumen global
                var resumenGlobal = new
                {
                    Catalogos = resultado.Count,
                    TotalItems = resultado.Values.Sum(l => l.Count)
                };

                // Resumen por hoja (LAB / RX / ULTRASONIDO)
                var resumenPorHoja = resultado
                    .SelectMany(kvp => kvp.Value.Select(item => new { item.Hoja, Catalogo = kvp.Key }))
                    .GroupBy(x => x.Hoja ?? "DESCONOCIDA")
                    .Select(g => new
                    {
                        Hoja = g.Key,
                        Catalogos = g.Select(x => x.Catalogo).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                        TotalItems = g.Count()
                    })
                    .OrderBy(x => x.Hoja)
                    .ToList();

                ViewData["ResumenImportacion"] = resumenGlobal;
                ViewData["ResumenImportacionPorHoja"] = resumenPorHoja;

                Console.WriteLine($"[POST-ARMADO] Resumen global -> Catálogos: {resumenGlobal.Catalogos}, Ítems: {resumenGlobal.TotalItems}");
                foreach (var r in resumenPorHoja)
                {
                    Console.WriteLine($"[POST-ARMADO] Hoja={r.Hoja} Catálogos={r.Catalogos} Ítems={r.TotalItems}");
                }

                // Guardar en Session (no TempData) para evitar 431 por cookies grandes
                var json = System.Text.Json.JsonSerializer.Serialize(resultado);
                HttpContext.Session.SetString("ImportLab_ResultJson", json);
                Console.WriteLine($"[POST-ARMADO] JSON guardado en Session (len={json.Length}).");

                return View(); // misma vista
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[POST-ARMADO][ERROR] {ex}");
                ModelState.AddModelError(string.Empty, $"Error procesando el archivo: {ex.Message}");
                return View();
            }
        }

        // POST (2): inserta en BD usando lo armado en Session
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertarDesdeLaboratorioClinico()
        {
            Console.WriteLine("[POST-INSERTAR] Inicio.");
            try
            {
                var json = HttpContext.Session.GetString("ImportLab_ResultJson");
                if (string.IsNullOrWhiteSpace(json))
                {
                    Console.WriteLine("[POST-INSERTAR] Session vacía: ImportLab_ResultJson.");
                    ModelState.AddModelError(string.Empty, "No hay datos armados para insertar. Vuelve a cargar el Excel.");
                    return View("ImportarDesdeLaboratorioClinico");
                }

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var armado = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<ItemCatalogo>>>(json, options)
                             ?? new Dictionary<string, List<ItemCatalogo>>(StringComparer.OrdinalIgnoreCase);

                string NormalizaNombre(string s) => NormalizaTexto(s ?? string.Empty).ToUpperInvariant();

                Console.WriteLine($"[POST-INSERTAR] Catálogos en armado: {armado.Count}");

                // Cargar categorías existentes (Id + nombre normalizado)
                var categoriasDb = _context.CategoriaLabClinicos
                    .Select(c => new { c.Id, c.Nombre })
                    .AsEnumerable()
                    .Select(c => new { c.Id, NombreNorm = NormalizaNombre(c.Nombre) })
                    .ToList();

                var mapNombreNormToId = categoriasDb
                    .GroupBy(c => c.NombreNorm)
                    .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

                int categoriasCreadas = 0;
                int examenesInsertados = 0;
                int duplicadosSaltados = 0;

                using var tx = _context.Database.BeginTransaction();

                // Resolver/crear categorías necesarias
                foreach (var kvp in armado)
                {
                    var nombreCatRaw = kvp.Key ?? string.Empty;
                    var nombreCatNorm = NormalizaNombre(nombreCatRaw);

                    if (!mapNombreNormToId.TryGetValue(nombreCatNorm, out int catId))
                    {
                        var nuevaCat = new CategoriaLabClinico
                        {
                            Nombre = nombreCatRaw?.Trim(),
                            Estado = null,
                            FechaCreacion = DateTime.Now,
                            UltimoUsuarioModificado = null,
                            Eliminado = false,
                            Activo = true
                        };
                        _context.CategoriaLabClinicos.Add(nuevaCat);
                        _context.SaveChanges();

                        catId = nuevaCat.Id;
                        mapNombreNormToId[nombreCatNorm] = catId;
                        categoriasCreadas++;
                        Console.WriteLine($"[POST-INSERTAR] Categoría creada: '{nombreCatRaw}' -> Id={catId}");
                    }
                }

                // Duplicados por nombre+categoría (política: omitir)
                var nombresExistentesPorCat = _context.ExamenLabClinicos
                    .Select(e => new { e.CategoriaLabClinicoId, e.NombreExamen })
                    .AsEnumerable()
                    .GroupBy(e => e.CategoriaLabClinicoId)
                    .ToDictionary(
                        g => g.Key,
                        g => new HashSet<string>(
                                g.Select(x => NormalizaNombre(x.NombreExamen)),
                                StringComparer.OrdinalIgnoreCase)
                    );

                // Correlativo base por categoría (máximo N de CodigoInterno)
                var correlativoBasePorCat = new Dictionary<int, int>();
                foreach (var catId in mapNombreNormToId.Values.Distinct())
                {
                    var maxN = _context.ExamenLabClinicos
                        .Where(e => e.CategoriaLabClinicoId == catId && e.CodigoInterno != null)
                        .Select(e => e.CodigoInterno)
                        .AsEnumerable()
                        .Select(cod =>
                        {
                            var idx = cod.LastIndexOf('-');
                            if (idx < 0) return 0;
                            var seg = cod[(idx + 1)..].Trim();
                            return int.TryParse(seg, out var n) ? n : 0;
                        })
                        .DefaultIfEmpty(0)
                        .Max();

                    correlativoBasePorCat[catId] = maxN;
                    Console.WriteLine($"[POST-INSERTAR] CatId={catId} correlativo base={maxN}");
                }

                var nuevos = new List<ExamenLabClinico>();
                var estudiosInsertados = new List<EstudioInsertado>(); // para trazabilidad por pestaña

                foreach (var (nombreCatRaw, items) in armado)
                {
                    var nombreCatNorm = NormalizaNombre(nombreCatRaw);
                    var catId = mapNombreNormToId[nombreCatNorm];

                    if (!nombresExistentesPorCat.TryGetValue(catId, out var nombresCat))
                    {
                        nombresCat = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        nombresExistentesPorCat[catId] = nombresCat;
                    }

                    var corr = correlativoBasePorCat[catId];

                    foreach (var it in items)
                    {
                        var nombreNorm = NormalizaNombre(it.Estudio);
                        if (nombresCat.Contains(nombreNorm))
                        {
                            duplicadosSaltados++;
                            continue;
                        }

                        corr++;

                        // Generar código interno y limpiarlo (solo alfanumérico y '-')
                        var codigoRaw = GeneraCodigo(nombreCatRaw, it.Estudio, corr);
                        var codigoLimpio = Regex.Replace(codigoRaw ?? string.Empty, @"[^A-Za-z0-9\-]", string.Empty);
                        if (string.IsNullOrWhiteSpace(codigoLimpio))
                        {
                            codigoLimpio = $"X-XX-{corr}";
                        }

                        // Normalizar precio: si viene 0, se fuerza a 1 (tanto en Examen como en tabla de precios)
                        var precioBase = it.Precio <= 0 ? 1m : it.Precio;

                        var entidad = new ExamenLabClinico
                        {
                            CategoriaLabClinicoId = catId,
                            CategoriaGeneralLabClinicoId = null,
                            CodigoInterno = codigoLimpio,
                            NombreExamen = it.Estudio?.Trim(),

                            // LAB: Precio = PRECIO (normalizado), PrecioB=0, PrecioC=0
                            // RX / US: Precio = PRECIO, PrecioC = PRECIO EMERGENCIA (Anio2024)
                            Precio = precioBase,
                            PrecioB = it.SetentayCinco,
                            PrecioC = it.Anio2024,
                            PrecioCosto = it.Costo,

                            Indicaciones = it.TipoMuestra?.Trim(),
                            TipoDeExamen = null,
                            FechaCreacion = DateTime.Now,
                            UltimaModificacion = null,
                            Activo = true,
                            Eliminado = false
                        };

                        nuevos.Add(entidad);
                        nombresCat.Add(nombreNorm);

                        estudiosInsertados.Add(new EstudioInsertado
                        {
                            Hoja = it.Hoja,
                            Catalogo = nombreCatRaw,
                            Estudio = it.Estudio
                        });
                    }

                    correlativoBasePorCat[catId] = corr;
                    Console.WriteLine($"[POST-INSERTAR] CatId={catId} nuevos={nuevos.Count(x => x.CategoriaLabClinicoId == catId)} corrFinal={corr}");
                }

                if (nuevos.Count > 0)
                {
                    // Insertar exámenes
                    _context.ExamenLabClinicos.AddRange(nuevos);
                    _context.SaveChanges();
                    examenesInsertados = nuevos.Count;

                    // Ahora crear registros en ExamenLabClinicosPrecios
                    const int PrecioNormalId = 1;  // NORMAL
                    const int PrecioPrivadoId = 27; // PRIVADO (emergencia / sin seguro)

                    var preciosNuevos = new List<ExamenLabClinicoPrecio>();

                    foreach (var examen in nuevos)
                    {
                        // Precio normal: todos los exámenes deben tenerlo
                        var precioNormal = examen.Precio <= 0 ? 1m : examen.Precio;

                        // Aseguramos que el campo del examen quede también normalizado
                        examen.Precio = precioNormal;

                        preciosNuevos.Add(new ExamenLabClinicoPrecio
                        {
                            ExamenLabClinicoId = examen.Id,
                            PrecioId = PrecioNormalId,
                            PrecioValor = precioNormal
                        });

                        // Precio emergencia solo si hay valor (> 0) → RX / ULTRASONIDO
                        if (examen.PrecioC > 0)
                        {
                            preciosNuevos.Add(new ExamenLabClinicoPrecio
                            {
                                ExamenLabClinicoId = examen.Id,
                                PrecioId = PrecioPrivadoId,
                                PrecioValor = examen.PrecioC
                            });
                        }
                    }

                    if (preciosNuevos.Count > 0)
                    {
                        _context.ExamenLabClinicosPrecios.AddRange(preciosNuevos);
                        _context.SaveChanges();
                    }
                }

                tx.Commit();

                // Resumen global de inserción
                ViewData["ResumenInsercion"] = new
                {
                    CategoriasCreadas = categoriasCreadas,
                    ExamenesInsertados = examenesInsertados,
                    DuplicadosSaltados = duplicadosSaltados
                };

                // Resumen por hoja de lo que SÍ se insertó
                var resumenInsercionPorHoja = estudiosInsertados
                    .GroupBy(x => x.Hoja ?? "DESCONOCIDA")
                    .Select(g => new
                    {
                        Hoja = g.Key,
                        Catalogos = g.Select(x => x.Catalogo).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                        ExamenesInsertados = g.Count()
                    })
                    .OrderBy(x => x.Hoja)
                    .ToList();

                ViewData["ResumenInsercionPorHoja"] = resumenInsercionPorHoja;
                ViewData["EstudiosInsertados"] = estudiosInsertados;

                // Limpiar la Session para evitar reinserciones accidentales
                HttpContext.Session.Remove("ImportLab_ResultJson");

                Console.WriteLine($"[POST-INSERTAR] OK -> Creadas:{categoriasCreadas} Insertadas:{examenesInsertados} Duplicados:{duplicadosSaltados}");
                foreach (var r in resumenInsercionPorHoja)
                {
                    Console.WriteLine($"[POST-INSERTAR] Hoja={r.Hoja} Catálogos={r.Catalogos} Exámenes={r.ExamenesInsertados}");
                }

                return View("ImportarDesdeLaboratorioClinico");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[POST-INSERTAR][ERROR] {ex}");
                ModelState.AddModelError(string.Empty, $"Error en la inserción: {ex.Message}");
                return View("ImportarDesdeLaboratorioClinico");
            }
        }

        // ====== Tipos/Helpers locales (solo para esta importación) ======
        private sealed class ItemCatalogo
        {
            public string Hoja { get; set; }          // LABORATORIO / RX / ULTRASONIDO
            public string Catalogo { get; set; }      // Nombre del catálogo (título amarillo)
            public string CodigoGenerado { get; set; }
            public string Estudio { get; set; }
            public string TipoMuestra { get; set; }
            public decimal Costo { get; set; }
            public decimal SetentayCinco { get; set; }
            public decimal Anio2024 { get; set; }     // En RX/US = PRECIO EMERGENCIA
            public decimal Precio { get; set; }       // Precio normal (LAB, RX, US)
        }

        // Para trazabilidad de lo que se insertó finalmente
        private sealed class EstudioInsertado
        {
            public string Hoja { get; set; }      // LABORATORIO / RX / ULTRASONIDO
            public string Catalogo { get; set; }  // Nombre del catálogo
            public string Estudio { get; set; }   // Nombre del examen
        }

        /// <summary>
        /// Procesa la hoja LABORATORIO y llena el diccionario resultado.
        /// LAB:
        /// - Costo -> Costo
        /// - 75%   -> NO se usa (SetentayCinco = 0)
        /// - 2024  -> NO se usa (Anio2024 = 0)
        /// - PRECIO -> Precio
        /// </summary>
        private void ProcesarHojaLaboratorio(
            XLWorkbook wb,
            Dictionary<string, List<ItemCatalogo>> resultado,
            Dictionary<string, int> correlativoPorCatalogo)
        {
            var ws = wb.Worksheets.FirstOrDefault(s =>
                string.Equals(s.Name?.Trim(), "LABORATORIO", StringComparison.OrdinalIgnoreCase));

            if (ws is null)
            {
                Console.WriteLine("[POST-ARMADO] Hoja 'LABORATORIO' no encontrada. Se omite.");
                return;
            }

            string catalogoActual = null;

            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            Console.WriteLine($"[POST-ARMADO] [LAB] Última fila usada: {lastRow}");

            for (int r = 1; r <= lastRow; r++)
            {
                var col1 = (ws.Cell(r, 1).GetValue<string>() ?? "").Trim(); // Estudio o título de catálogo
                var col2 = (ws.Cell(r, 2).GetValue<string>() ?? "").Trim(); // Tipo de muestra

                // Encabezado
                if (col1.Equals("Estudio", StringComparison.OrdinalIgnoreCase) &&
                    col2.StartsWith("Tipo", StringComparison.OrdinalIgnoreCase))
                    continue;

                // ¿Es fila de catálogo?
                if (EsFilaCatalogo(ws, r))
                {
                    catalogoActual = NormalizaTexto(col1);
                    if (!resultado.ContainsKey(catalogoActual))
                        resultado[catalogoActual] = new List<ItemCatalogo>();
                    if (!correlativoPorCatalogo.ContainsKey(catalogoActual))
                        correlativoPorCatalogo[catalogoActual] = 0;

                    Console.WriteLine($"[POST-ARMADO] [LAB] Catálogo detectado: {catalogoActual} (fila {r})");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(catalogoActual)) continue;
                if (string.IsNullOrWhiteSpace(col1) || string.IsNullOrWhiteSpace(col2)) continue;

                var estudio = NormalizaTexto(col1);
                var tipoMuestra = col2;

                var costo = LeeDecimal(ws.Cell(r, 3));    // Costo
                var precio = LeeDecimal(ws.Cell(r, 6));   // PRECIO

                correlativoPorCatalogo[catalogoActual]++;
                var correlativo = correlativoPorCatalogo[catalogoActual];
                var codigo = GeneraCodigo(catalogoActual, estudio, correlativo);

                resultado[catalogoActual].Add(new ItemCatalogo
                {
                    Hoja = "LABORATORIO",
                    Catalogo = catalogoActual,
                    CodigoGenerado = codigo,
                    Estudio = estudio,
                    TipoMuestra = tipoMuestra,
                    Costo = costo,
                    SetentayCinco = 0m,   // 75% no se almacena
                    Anio2024 = 0m,        // 2024 no se almacena
                    Precio = precio       // PRECIO
                });
            }
        }

        /// <summary>
        /// Procesa la hoja RX y llena el diccionario resultado.
        /// RX:
        /// - Costo             -> Costo
        /// - 75%               -> NO se usa (SetentayCinco = 0)
        /// - P.E COSTO         -> NO se usa por ahora
        /// - PRECIO            -> Precio
        /// - PRECIO EMERGENCIA -> Anio2024 (PrecioC)
        /// </summary>
        private void ProcesarHojaRx(
            XLWorkbook wb,
            Dictionary<string, List<ItemCatalogo>> resultado,
            Dictionary<string, int> correlativoPorCatalogo)
        {
            var ws = wb.Worksheets.FirstOrDefault(s =>
                string.Equals(s.Name?.Trim(), "RX", StringComparison.OrdinalIgnoreCase));

            if (ws is null)
            {
                Console.WriteLine("[POST-ARMADO] Hoja 'RX' no encontrada. Se omite.");
                return;
            }

            string catalogoActual = null;

            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            Console.WriteLine($"[POST-ARMADO] [RX] Última fila usada: {lastRow}");

            for (int r = 1; r <= lastRow; r++)
            {
                var col1 = (ws.Cell(r, 1).GetValue<string>() ?? "").Trim(); // Estudio o título de catálogo
                var col2 = (ws.Cell(r, 2).GetValue<string>() ?? "").Trim(); // Tipo de muestra

                // Encabezado
                if (col1.Equals("Estudio", StringComparison.OrdinalIgnoreCase) &&
                    col2.StartsWith("Tipo", StringComparison.OrdinalIgnoreCase))
                    continue;

                // ¿Es fila de catálogo?
                if (EsFilaCatalogo(ws, r))
                {
                    catalogoActual = NormalizaTexto(col1);
                    if (!resultado.ContainsKey(catalogoActual))
                        resultado[catalogoActual] = new List<ItemCatalogo>();
                    if (!correlativoPorCatalogo.ContainsKey(catalogoActual))
                        correlativoPorCatalogo[catalogoActual] = 0;

                    Console.WriteLine($"[POST-ARMADO] [RX] Catálogo detectado: {catalogoActual} (fila {r})");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(catalogoActual)) continue;

                // En RX solo exigimos que haya Estudio (col1). Tipo de muestra (col2) puede venir vacío.
                if (string.IsNullOrWhiteSpace(col1)) continue;

                var estudio = NormalizaTexto(col1);
                var tipoMuestra = col2; // puede ser vacío

                var costo = LeeDecimal(ws.Cell(r, 3));                // Costo
                var precio = LeeDecimal(ws.Cell(r, 6));               // PRECIO
                var precioEmergencia = LeeDecimal(ws.Cell(r, 7));     // PRECIO EMERGENCIA -> PrecioC

                correlativoPorCatalogo[catalogoActual]++;
                var correlativo = correlativoPorCatalogo[catalogoActual];
                var codigo = GeneraCodigo(catalogoActual, estudio, correlativo);

                resultado[catalogoActual].Add(new ItemCatalogo
                {
                    Hoja = "RX",
                    Catalogo = catalogoActual,
                    CodigoGenerado = codigo,
                    Estudio = estudio,
                    TipoMuestra = tipoMuestra,
                    Costo = costo,
                    SetentayCinco = 0m,             // 75% no se almacena
                    Anio2024 = precioEmergencia,    // PRECIO EMERGENCIA -> PrecioC
                    Precio = precio                 // PRECIO
                });
            }
        }

        /// <summary>
        /// Procesa la hoja ULTRASONIDO y llena el diccionario resultado.
        /// Estructura igual a RX:
        /// - Costo             -> Costo
        /// - 75%               -> NO se usa (SetentayCinco = 0)
        /// - P.E COSTO         -> NO se usa por ahora
        /// - PRECIO            -> Precio
        /// - PRECIO EMERGENCIA -> Anio2024 (PrecioC)
        /// </summary>
        private void ProcesarHojaUltrasonido(
            XLWorkbook wb,
            Dictionary<string, List<ItemCatalogo>> resultado,
            Dictionary<string, int> correlativoPorCatalogo)
        {
            var ws = wb.Worksheets.FirstOrDefault(s =>
                string.Equals(s.Name?.Trim(), "ULTRASONIDO", StringComparison.OrdinalIgnoreCase));

            if (ws is null)
            {
                Console.WriteLine("[POST-ARMADO] Hoja 'ULTRASONIDO' no encontrada. Se omite.");
                return;
            }

            string catalogoActual = null;

            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            Console.WriteLine($"[POST-ARMADO] [US] Última fila usada: {lastRow}");

            for (int r = 1; r <= lastRow; r++)
            {
                var col1 = (ws.Cell(r, 1).GetValue<string>() ?? "").Trim(); // Estudio o título de catálogo
                var col2 = (ws.Cell(r, 2).GetValue<string>() ?? "").Trim(); // Tipo de muestra

                // Encabezado
                if (col1.Equals("Estudio", StringComparison.OrdinalIgnoreCase) &&
                    col2.StartsWith("Tipo", StringComparison.OrdinalIgnoreCase))
                    continue;

                // ¿Es fila de catálogo?
                if (EsFilaCatalogo(ws, r))
                {
                    catalogoActual = NormalizaTexto(col1);
                    if (!resultado.ContainsKey(catalogoActual))
                        resultado[catalogoActual] = new List<ItemCatalogo>();
                    if (!correlativoPorCatalogo.ContainsKey(catalogoActual))
                        correlativoPorCatalogo[catalogoActual] = 0;

                    Console.WriteLine($"[POST-ARMADO] [US] Catálogo detectado: {catalogoActual} (fila {r})");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(catalogoActual)) continue;

                // En ULTRASONIDO solo exigimos que haya Estudio (col1). Tipo de muestra (col2) puede venir vacío.
                if (string.IsNullOrWhiteSpace(col1)) continue;

                var estudio = NormalizaTexto(col1);
                var tipoMuestra = col2; // puede ser vacío

                var costo = LeeDecimal(ws.Cell(r, 3));                // Costo
                var precio = LeeDecimal(ws.Cell(r, 6));               // PRECIO
                var precioEmergencia = LeeDecimal(ws.Cell(r, 7));     // PRECIO EMERGENCIA -> PrecioC

                correlativoPorCatalogo[catalogoActual]++;
                var correlativo = correlativoPorCatalogo[catalogoActual];
                var codigo = GeneraCodigo(catalogoActual, estudio, correlativo);

                resultado[catalogoActual].Add(new ItemCatalogo
                {
                    Hoja = "ULTRASONIDO",
                    Catalogo = catalogoActual,
                    CodigoGenerado = codigo,
                    Estudio = estudio,
                    TipoMuestra = tipoMuestra,
                    Costo = costo,
                    SetentayCinco = 0m,
                    Anio2024 = precioEmergencia,   // PRECIO EMERGENCIA -> PrecioC
                    Precio = precio                // PRECIO
                });
            }
        }

        private static bool EsFilaCatalogo(IXLWorksheet ws, int r)
        {
            // Catálogo: col1 con texto (no "Estudio") y col2..col6 vacías
            string c1 = (ws.Cell(r, 1).GetValue<string>() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(c1)) return false;
            if (c1.Equals("Estudio", StringComparison.OrdinalIgnoreCase)) return false;

            bool c2 = string.IsNullOrWhiteSpace((ws.Cell(r, 2).GetValue<string>() ?? "").Trim());
            bool c3 = string.IsNullOrWhiteSpace((ws.Cell(r, 3).GetValue<string>() ?? "").Trim());
            bool c4 = string.IsNullOrWhiteSpace((ws.Cell(r, 4).GetValue<string>() ?? "").Trim());
            bool c5 = string.IsNullOrWhiteSpace((ws.Cell(r, 5).GetValue<string>() ?? "").Trim());
            bool c6 = string.IsNullOrWhiteSpace((ws.Cell(r, 6).GetValue<string>() ?? "").Trim());

            return c2 && c3 && c4 && c5 && c6;
        }

        private static decimal LeeDecimal(IXLCell cell)
        {
            // Admite valores como "Q 97,50" / "97.50" / "97,50"
            var s = (cell.GetValue<string>() ?? "").Trim();
            if (string.IsNullOrEmpty(s))
                return 0m;

            s = s.Replace("Q", "", StringComparison.OrdinalIgnoreCase)
                 .Replace(" ", "")
                 .Replace(",", ".");

            return decimal.TryParse(
                s,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var d) ? d : 0m;
        }

        private static string NormalizaTexto(string s)
        {
            s = (s ?? "").Trim();
            if (s.Length == 0) return s;

            // quitar tildes
            s = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC).Trim();
        }

        private static string GeneraCodigo(string catalogo, string estudio, int correlativo)
        {
            // Inicial del catálogo
            var iniCat = string.IsNullOrWhiteSpace(catalogo)
                ? "X"
                : catalogo.Substring(0, 1).ToUpperInvariant();

            // Iniciales del estudio (ignorando stopwords) → 2 letras
            var stop = new HashSet<string>(
                new[] { "DE", "DEL", "LA", "EL", "LOS", "LAS", "POR", "PARA", "EN", "Y", "A", "CON", "SIN" },
                StringComparer.OrdinalIgnoreCase);

            var tokens = (estudio ?? "")
                .Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(t => !stop.Contains(t))
                .ToList();

            string iniEstudio;
            if (tokens.Count >= 2)
                iniEstudio = (tokens[0][0].ToString() + tokens[1][0].ToString()).ToUpperInvariant();
            else if (tokens.Count == 1)
                iniEstudio = tokens[0].Substring(0, Math.Min(2, tokens[0].Length)).ToUpperInvariant();
            else
                iniEstudio = "XX";

            // Construimos el código base; el sanitizado final se hace fuera (Regex)
            return $"{iniCat}-{iniEstudio}-{correlativo}";
        }
        #endregion

        #region ImportarExcel Servicios (Banco de Sangre / Procedimientos)

        // GET: muestra el formulario para Servicios
        public IActionResult ImportarServiciosBancoProcedimientos()
        {
            Console.WriteLine("[SERV-GET] ImportarServiciosBancoProcedimientos -> Render vista.");
            // Usamos la misma vista que para exámenes
            return View("ImportarDesdeLaboratorioClinico");
        }

        // POST (1): solo arma en memoria desde el Excel (BANCO DE SANGRE y PROCEDIMIENTOS)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ImportarServiciosBancoProcedimientos(IFormFile excel)
        {
            Console.WriteLine("[SERV-POST-ARMADO] Inicio.");

            try
            {
                if (excel is null || excel.Length == 0)
                {
                    Console.WriteLine("[SERV-POST-ARMADO] Archivo vacío o nulo.");
                    ModelState.AddModelError(string.Empty, "Debe seleccionar un archivo Excel válido.");
                    return View("ImportarDesdeLaboratorioClinico"); // misma vista
                }

                var items = new List<ItemServicio>();

                using var wb = new XLWorkbook(excel.OpenReadStream());

                // Procesar hoja BANCO DE SANGRE (si existe)
                ProcesarHojaBancoDeSangre(wb, items);

                // Procesar hoja PROCEDIMIENTOS (si existe)
                ProcesarHojaProcedimientos(wb, items);

                if (items.Count == 0)
                {
                    Console.WriteLine("[SERV-POST-ARMADO] No se encontraron datos válidos en hojas 'BANCO DE SANGRE' ni 'PROCEDIMIENTOS'.");
                    ModelState.AddModelError(string.Empty, "El archivo no contiene datos válidos en las hojas 'BANCO DE SANGRE' o 'PROCEDIMIENTOS'.");
                    return View("ImportarDesdeLaboratorioClinico");
                }

                // Resumen global
                var resumenGlobal = new
                {
                    TotalServicios = items.Count,
                    HojasConDatos = items.Select(i => i.Hoja).Distinct(StringComparer.OrdinalIgnoreCase).Count()
                };

                // Resumen por hoja
                var resumenPorHoja = items
                    .GroupBy(x => x.Hoja ?? "DESCONOCIDA")
                    .Select(g => new
                    {
                        Hoja = g.Key,
                        Servicios = g.Count()
                    })
                    .OrderBy(x => x.Hoja)
                    .ToList();

                // Claves específicas para servicios (para no chocar con exámenes)
                ViewData["ResumenImportacionServicios"] = resumenGlobal;
                ViewData["ResumenImportacionPorHojaServicios"] = resumenPorHoja;

                Console.WriteLine($"[SERV-POST-ARMADO] Resumen global -> Hojas con datos: {resumenGlobal.HojasConDatos}, Servicios: {resumenGlobal.TotalServicios}");
                foreach (var r in resumenPorHoja)
                {
                    Console.WriteLine($"[SERV-POST-ARMADO] Hoja={r.Hoja} Servicios={r.Servicios}");
                }

                // Guardar en Session (no TempData) para evitar 431
                var json = System.Text.Json.JsonSerializer.Serialize(items);
                HttpContext.Session.SetString("ImportServ_ResultJson", json);
                Console.WriteLine($"[SERV-POST-ARMADO] JSON de servicios guardado en Session (len={json.Length}).");

                return View("ImportarDesdeLaboratorioClinico"); // misma vista
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERV-POST-ARMADO][ERROR] {ex}");
                ModelState.AddModelError(string.Empty, $"Error procesando el archivo: {ex.Message}");
                return View("ImportarDesdeLaboratorioClinico");
            }
        }

        // POST (2): inserta en BD usando lo armado en Session
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertarServiciosBancoProcedimientos()
        {
            Console.WriteLine("[SERV-POST-INSERTAR] Inicio.");

            try
            {
                var json = HttpContext.Session.GetString("ImportServ_ResultJson");
                if (string.IsNullOrWhiteSpace(json))
                {
                    Console.WriteLine("[SERV-POST-INSERTAR] Session vacía: ImportServ_ResultJson.");
                    ModelState.AddModelError(string.Empty, "No hay datos armados para insertar (Servicios). Vuelve a cargar el Excel.");
                    return View("ImportarDesdeLaboratorioClinico");
                }

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var armado = System.Text.Json.JsonSerializer.Deserialize<List<ItemServicio>>(json, options)
                             ?? new List<ItemServicio>();

                if (armado.Count == 0)
                {
                    Console.WriteLine("[SERV-POST-INSERTAR] Lista de servicios armados vacía.");
                    ModelState.AddModelError(string.Empty, "No hay servicios válidos para insertar. Vuelve a cargar el Excel.");
                    return View("ImportarDesdeLaboratorioClinico");
                }

                string NormalizaNombre(string s) => NormalizaTexto(s ?? string.Empty).ToUpperInvariant();

                Console.WriteLine($"[SERV-POST-INSERTAR] Servicios armados: {armado.Count}");

                // Cargar servicios existentes (Id + nombre normalizado + CodigoInterno para correlativo)
                var serviciosDb = _context.Servicios
                    .Select(s => new { s.Id, s.NombreServicio, s.CodigoInterno })
                    .AsEnumerable()
                    .Select(s => new
                    {
                        s.Id,
                        NombreNorm = NormalizaNombre(s.NombreServicio),
                        s.CodigoInterno
                    })
                    .ToList();

                var nombresExistentes = new HashSet<string>(
                    serviciosDb.Select(s => s.NombreNorm),
                    StringComparer.OrdinalIgnoreCase);

                // Correlativo base para CodigoInterno (numérico)
                var baseCodigoInterno = serviciosDb
                    .Select(s =>
                    {
                        if (int.TryParse(s.CodigoInterno, out var n))
                            return n;
                        return 0;
                    })
                    .DefaultIfEmpty(0)
                    .Max();

                Console.WriteLine($"[SERV-POST-INSERTAR] CodigoInterno base (máximo existente) = {baseCodigoInterno}");

                // Cargar tipos de Precio desde tabla Precio (para mapear NORMAL / PRIVADO / SIN SEGURO)
                var preciosCatalogo = _context.Precios
                    .Where(p => !p.Eliminado)
                    .ToList();

                int? precioNormalId = preciosCatalogo
                    .FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.NombrePrecio) &&
                                         p.NombrePrecio.Trim().ToUpper().Contains("NORMAL"))?.Id;

                int? precioPrivadoId = preciosCatalogo
                    .FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.NombrePrecio) &&
                                         p.NombrePrecio.Trim().ToUpper().Contains("PRIVADO"))?.Id;

                int? precioSinSeguroId = preciosCatalogo
                    .FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.NombrePrecio) &&
                                         p.NombrePrecio.Trim().ToUpper().Contains("SIN SEGURO"))?.Id;

                Console.WriteLine($"[SERV-POST-INSERTAR] PrecioId NORMAL={precioNormalId}, PRIVADO={precioPrivadoId}, SIN SEGURO={precioSinSeguroId}");

                if (precioNormalId is null)
                {
                    ModelState.AddModelError(string.Empty, "No se encontró en la tabla PRECIOS un registro para 'NORMAL'.");
                    return View("ImportarDesdeLaboratorioClinico");
                }

                if (precioPrivadoId is null)
                {
                    ModelState.AddModelError(string.Empty, "No se encontró en la tabla PRECIOS un registro para 'PRIVADO' (requerido para 75% y HONORARIOS).");
                    return View("ImportarDesdeLaboratorioClinico");
                }

                if (precioSinSeguroId is null)
                {
                    ModelState.AddModelError(string.Empty, "No se encontró en la tabla PRECIOS un registro para 'SIN SEGURO' (requerido para CORTESÍA).");
                    return View("ImportarDesdeLaboratorioClinico");
                }

                int serviciosInsertados = 0;
                int duplicadosSaltados = 0;
                int preciosCreados = 0;

                var nuevosServicios = new List<Servicio>();
                var nuevosServiciosPrecios = new List<ServicioPrecio>();
                var serviciosInsertadosDetalle = new List<ServicioInsertado>();

                using var tx = _context.Database.BeginTransaction();

                int correlativo = baseCodigoInterno;

                foreach (var it in armado)
                {
                    var nombreNorm = NormalizaNombre(it.NombreServicio);

                    if (nombresExistentes.Contains(nombreNorm))
                    {
                        duplicadosSaltados++;
                        Console.WriteLine($"[SERV-POST-INSERTAR] Duplicado omitido: '{it.NombreServicio}' (Hoja={it.Hoja})");
                        continue;
                    }

                    correlativo++;
                    var codigoInterno = correlativo.ToString();

                    // Según lo acordado: Precio_2, Precio_3, Precio_4 se dejan en 1 de momento.
                    var servicio = new Servicio
                    {
                        CodigoInterno = codigoInterno,
                        CategoriaServicioId = null,
                        NombreServicio = it.NombreServicio?.Trim(),
                        Precio_2 = 1m,
                        Precio_3 = 1m,
                        Precio_4 = 1m,
                        Descripcion = null,
                        DuracionHoras = 0,
                        DuracionMinutos = 0,
                        // Usamos NORMAL como precio "principal" de referencia
                        PrecioId = precioNormalId.Value,
                        Eliminado = false
                    };

                    nuevosServicios.Add(servicio);
                    nombresExistentes.Add(nombreNorm);
                    serviciosInsertados++;

                    serviciosInsertadosDetalle.Add(new ServicioInsertado
                    {
                        Hoja = it.Hoja,
                        NombreServicio = it.NombreServicio,
                        CodigoInterno = codigoInterno,
                        Costo = it.Costo,
                        PrecioNormal = it.PrecioNormal,
                        Precio75 = it.Precio75,
                        PrecioCortesia = it.PrecioCortesia,
                        PrecioHonorarios = it.PrecioHonorarios
                    });

                    // ----- Crear precios asociados en ServiciosPrecios -----
                    // Siempre creamos el precio NORMAL si hay valor
                    if (it.PrecioNormal > 0)
                    {
                        nuevosServiciosPrecios.Add(new ServicioPrecio
                        {
                            Servicio = servicio,
                            PrecioId = precioNormalId.Value,
                            Valor = it.PrecioNormal,
                            Activar = true
                        });
                        preciosCreados++;
                    }

                    if (it.Hoja == "BANCO DE SANGRE")
                    {
                        // 75% -> PRIVADO
                        if (it.Precio75 > 0)
                        {
                            nuevosServiciosPrecios.Add(new ServicioPrecio
                            {
                                Servicio = servicio,
                                PrecioId = precioPrivadoId.Value,
                                Valor = it.Precio75,
                                Activar = true
                            });
                            preciosCreados++;
                        }
                    }
                    else if (it.Hoja == "PROCEDIMIENTOS")
                    {
                        // CORTESÍA -> SIN SEGURO
                        if (it.PrecioCortesia > 0)
                        {
                            nuevosServiciosPrecios.Add(new ServicioPrecio
                            {
                                Servicio = servicio,
                                PrecioId = precioSinSeguroId.Value,
                                Valor = it.PrecioCortesia,
                                Activar = true
                            });
                            preciosCreados++;
                        }

                        // HONORARIOS -> PRIVADO
                        if (it.PrecioHonorarios > 0)
                        {
                            nuevosServiciosPrecios.Add(new ServicioPrecio
                            {
                                Servicio = servicio,
                                PrecioId = precioPrivadoId.Value,
                                Valor = it.PrecioHonorarios,
                                Activar = true
                            });
                            preciosCreados++;
                        }
                    }
                }

                if (nuevosServicios.Count > 0)
                {
                    Console.WriteLine($"[SERV-POST-INSERTAR] Inserción: nuevos Servicios={nuevosServicios.Count}, nuevos ServiciosPrecios={nuevosServiciosPrecios.Count}");

                    _context.Servicios.AddRange(nuevosServicios);
                    _context.ServiciosPrecios.AddRange(nuevosServiciosPrecios);
                    _context.SaveChanges();
                }

                tx.Commit();

                // Resumen global de inserción (clave específica para servicios)
                ViewData["ResumenInsercionServicios"] = new
                {
                    ServiciosInsertados = serviciosInsertados,
                    DuplicadosSaltados = duplicadosSaltados,
                    PreciosCreados = preciosCreados
                };

                // Resumen por hoja de lo que sí se insertó
                var resumenInsercionPorHoja = serviciosInsertadosDetalle
                    .GroupBy(x => x.Hoja ?? "DESCONOCIDA")
                    .Select(g => new
                    {
                        Hoja = g.Key,
                        ServiciosInsertados = g.Count()
                    })
                    .OrderBy(x => x.Hoja)
                    .ToList();

                ViewData["ResumenInsercionPorHojaServicios"] = resumenInsercionPorHoja;
                ViewData["ServiciosInsertados"] = serviciosInsertadosDetalle;

                // Limpiar Session para evitar reinserciones accidentales
                HttpContext.Session.Remove("ImportServ_ResultJson");

                Console.WriteLine($"[SERV-POST-INSERTAR] OK -> ServiciosInsertados:{serviciosInsertados} Duplicados:{duplicadosSaltados} PreciosCreados:{preciosCreados}");
                foreach (var r in resumenInsercionPorHoja)
                {
                    Console.WriteLine($"[SERV-POST-INSERTAR] Hoja={r.Hoja} ServiciosInsertados={r.ServiciosInsertados}");
                }

                return View("ImportarDesdeLaboratorioClinico");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERV-POST-INSERTAR][ERROR] {ex}");
                ModelState.AddModelError(string.Empty, $"Error en la inserción de Servicios: {ex.Message}");
                return View("ImportarDesdeLaboratorioClinico");
            }
        }

        // ====== Tipos/Helpers locales (solo para esta importación de Servicios) ======
        private sealed class ItemServicio
        {
            public string Hoja { get; set; }              // BANCO DE SANGRE / PROCEDIMIENTOS
            public string NombreServicio { get; set; }    // Nombre del Servicio
            public decimal Costo { get; set; }           // COSTO del Excel (solo referencia)
            public decimal PrecioNormal { get; set; }    // PRECIO
            public decimal Precio75 { get; set; }        // 0.75 (BANCO DE SANGRE)
            public decimal PrecioCortesia { get; set; }  // CORTESIA (PROCEDIMIENTOS)
            public decimal PrecioHonorarios { get; set; }// HONORARIOS (PROCEDIMIENTOS)
        }

        // Para trazabilidad en la vista
        private sealed class ServicioInsertado
        {
            public string Hoja { get; set; }             // BANCO DE SANGRE / PROCEDIMIENTOS
            public string NombreServicio { get; set; }
            public string CodigoInterno { get; set; }
            public decimal Costo { get; set; }
            public decimal PrecioNormal { get; set; }
            public decimal Precio75 { get; set; }
            public decimal PrecioCortesia { get; set; }
            public decimal PrecioHonorarios { get; set; }
        }

        /// <summary>
        /// Procesa la hoja BANCO DE SANGRE y llena la lista de ItemServicio.
        /// BANCO DE SANGRE:
        /// - Col1: NOMBRE DEL SERVICIO
        /// - Col2: COSTO
        /// - Col3: PRECIO  (Normal)
        /// - Col4: 0.75   (75% privado)
        /// </summary>
        private void ProcesarHojaBancoDeSangre(
            XLWorkbook wb,
            List<ItemServicio> items)
        {
            var ws = wb.Worksheets.FirstOrDefault(s =>
                string.Equals(s.Name?.Trim(), "BANCO DE SANGRE", StringComparison.OrdinalIgnoreCase));

            if (ws is null)
            {
                Console.WriteLine("[SERV-POST-ARMADO] Hoja 'BANCO DE SANGRE' no encontrada. Se omite.");
                return;
            }

            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            Console.WriteLine($"[SERV-POST-ARMADO] [BANCO] Última fila usada: {lastRow}");

            for (int r = 1; r <= lastRow; r++)
            {
                var col1 = (ws.Cell(r, 1).GetValue<string>() ?? "").Trim();

                if (string.IsNullOrWhiteSpace(col1))
                    continue;

                // Encabezado
                if (col1.Equals("NOMBRE DEL SERVICIO", StringComparison.OrdinalIgnoreCase))
                    continue;

                var nombreServicio = col1;
                var costo = LeeDecimal(ws.Cell(r, 2));
                var precio = LeeDecimal(ws.Cell(r, 3));
                var precio75 = LeeDecimal(ws.Cell(r, 4));

                items.Add(new ItemServicio
                {
                    Hoja = "BANCO DE SANGRE",
                    NombreServicio = nombreServicio,
                    Costo = costo,
                    PrecioNormal = precio,
                    Precio75 = precio75,
                    PrecioCortesia = 0m,
                    PrecioHonorarios = 0m
                });

                Console.WriteLine($"[SERV-POST-ARMADO][BANCO] Fila {r}: Servicio='{nombreServicio}', Costo={costo}, Precio={precio}, 75%={precio75}");
            }
        }

        /// <summary>
        /// Procesa la hoja PROCEDIMIENTOS y llena la lista de ItemServicio.
        /// PROCEDIMIENTOS:
        /// - Col1: NOMBRE DEL SERVICIO
        /// - Col2: COSTO
        /// - Col3: PRECIO        (Normal)
        /// - Col4: Cortesia      (Sin seguro)
        /// - Col5: Honorarios    (Privado)
        /// </summary>
        private void ProcesarHojaProcedimientos(
            XLWorkbook wb,
            List<ItemServicio> items)
        {
            var ws = wb.Worksheets.FirstOrDefault(s =>
                string.Equals(s.Name?.Trim(), "PROCEDIMIENTOS", StringComparison.OrdinalIgnoreCase));

            if (ws is null)
            {
                Console.WriteLine("[SERV-POST-ARMADO] Hoja 'PROCEDIMIENTOS' no encontrada. Se omite.");
                return;
            }

            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            Console.WriteLine($"[SERV-POST-ARMADO] [PROC] Última fila usada: {lastRow}");

            for (int r = 1; r <= lastRow; r++)
            {
                var col1 = (ws.Cell(r, 1).GetValue<string>() ?? "").Trim();

                if (string.IsNullOrWhiteSpace(col1))
                    continue;

                // Encabezado
                if (col1.StartsWith("NOMBRE DEL SERVICIO", StringComparison.OrdinalIgnoreCase))
                    continue;

                var nombreServicio = col1;
                var costo = LeeDecimal(ws.Cell(r, 2));
                var precio = LeeDecimal(ws.Cell(r, 3));
                var cortesia = LeeDecimal(ws.Cell(r, 4));
                var honorarios = LeeDecimal(ws.Cell(r, 5));

                items.Add(new ItemServicio
                {
                    Hoja = "PROCEDIMIENTOS",
                    NombreServicio = nombreServicio,
                    Costo = costo,
                    PrecioNormal = precio,
                    Precio75 = 0m,
                    PrecioCortesia = cortesia,
                    PrecioHonorarios = honorarios
                });

                Console.WriteLine($"[SERV-POST-ARMADO][PROC] Fila {r}: Servicio='{nombreServicio}', Costo={costo}, Precio={precio}, Cortesia={cortesia}, Honorarios={honorarios}");
            }
        }

        #endregion
        private string CalcularHashArchivo(IFormFile archivo)
        {
            using var sha256 = SHA256.Create();
            using var stream = archivo.OpenReadStream();
            var hashBytes = sha256.ComputeHash(stream);
            // Convertimos a string hexadecimal
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        [HttpGet]
        public IActionResult PrevisualizarImportacion(string sessionKey)
        {
            if (string.IsNullOrEmpty(sessionKey))
            {
                return RedirectToAction("ImportarDesdeExcel");
            }

            var jsonDto = HttpContext.Session.GetString($"ImportInv_{sessionKey}");

            if (string.IsNullOrEmpty(jsonDto))
            {
                TempData["Message"] = "La sesión de importación expiró o el archivo no es válido. Por favor intente nuevamente.";
                return RedirectToAction("ImportarDesdeExcel");
            }

            var dto = JsonSerializer.Deserialize<PrevisualizacionInventarioDto>(jsonDto);

            return View(dto);
        }

        [HttpPost]
        public string ConfirmarYGuardarInventario(string sessionKey)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionKey))
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Llave de sesión no válida." });
                }

                // 1. Recuperar el contexto de la sesión
                var jsonDto = HttpContext.Session.GetString($"ImportInv_{sessionKey}");
                if (string.IsNullOrEmpty(jsonDto))
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "La sesión expiró o es inválida. Vuelva a cargar el archivo." });
                }

                var dto = JsonSerializer.Deserialize<PrevisualizacionInventarioDto>(jsonDto);

                // 2. Ubicar el archivo físico temporal
                var tempPath = Path.Combine(_hostEnvironment.WebRootPath, "temp");
                var filePath = Path.Combine(tempPath, $"{sessionKey}.xlsx");

                if (!System.IO.File.Exists(filePath))
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "No se encontró el archivo temporal en el servidor." });
                }

                // 3. Iniciar Transacción (Todo o Nada)
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    using var workbook = new XLWorkbook(filePath);
                    var sheet = workbook.Worksheet(1);
                    var numeroFilas = sheet.LastRowUsed()?.RowNumber() ?? 1;

                    var productosExistentes = _productosRepository.GetList();

                    #region Helpers locales (validaciones y parsing)
                    string GetStr(IXLRow r, int col, string field, bool required = false)
                    {
                        var val = r.Cell(col).GetValue<string>() ?? "";
                        val = val.Trim();
                        if (required && string.IsNullOrWhiteSpace(val)) throw new Exception($"El campo '{field}' está vacío.");
                        return val;
                    }

                    int GetInt(IXLRow r, int col, string field, bool required = false, int defaultValue = 0)
                    {
                        var s = (r.Cell(col).GetValue<string>() ?? "").Trim();
                        if (string.IsNullOrWhiteSpace(s)) return required ? throw new Exception($"El campo '{field}' está vacío.") : defaultValue;
                        if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)) throw new Exception($"El campo '{field}' debe ser entero.");
                        return n;
                    }

                    decimal GetDec(IXLRow r, int col, string field, bool required = false, decimal defaultValue = 0m)
                    {
                        var s = (r.Cell(col).GetValue<string>() ?? "").Trim().Replace("Q", "", StringComparison.OrdinalIgnoreCase).Replace(" ", "").Replace(',', '.');
                        if (string.IsNullOrWhiteSpace(s)) return required ? throw new Exception($"El campo '{field}' está vacío.") : defaultValue;
                        if (!decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var d)) throw new Exception($"El campo '{field}' debe ser numérico.");
                        return d;
                    }

                    DateTime? GetDate(IXLRow r, int col, string field, bool required = false)
                    {
                        var s = (r.Cell(col).GetValue<string>() ?? "").Trim();
                        if (string.IsNullOrWhiteSpace(s)) return required ? throw new Exception($"El campo '{field}' está vacío.") : (DateTime?)null;
                        if (r.Cell(col).TryGetValue<DateTime>(out var dtxl)) return dtxl;
                        if (DateTime.TryParse(s, CultureInfo.GetCultureInfo("es-GT"), DateTimeStyles.None, out var dt)) return dt;
                        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt;
                        throw new Exception($"El campo '{field}' no tiene una fecha válida.");
                    }

                    string Abrev3(string s, string field)
                    {
                        s = (s ?? "").Trim();
                        if (string.IsNullOrWhiteSpace(s)) throw new Exception($"El campo '{field}' está vacío.");
                        return s.Substring(0, Math.Min(3, s.Length)).ToUpperInvariant();
                    }

                    string UpperOrEmpty(string s) => (s ?? "").Trim().ToUpperInvariant();
                    #endregion

                    for (int row = 2; row <= numeroFilas; row++)
                    {
                        var item = sheet.Row(row);
                        var codigoReferencia = GetStr(item, 1, "Codigo referencia", required: false);

                        if (string.IsNullOrWhiteSpace(codigoReferencia)) continue;

                        var fechaIngreso = GetStr(item, 2, "Fecha de ingreso", required: false);
                        var nombre = GetStr(item, 3, "Nombre", required: false);
                        if (string.IsNullOrWhiteSpace(nombre)) nombre = "-";

                        var descripcion = GetStr(item, 4, "Descripcion", required: false);
                        if (string.IsNullOrWhiteSpace(descripcion)) descripcion = "-";

                        var activoConcentracion = GetStr(item, 5, "Activo Concentracion", required: false);
                        var casaMedica = GetStr(item, 6, "Casa medica", required: false);
                        var proveedor = GetStr(item, 7, "Proveedor", required: false);
                        var viaAdministracion = GetStr(item, 8, "Via de administración", required: false);

                        var stockPorUnidad = GetInt(item, 9, "Stock por unidad", required: false, defaultValue: 0);
                        var stockMinimo = GetInt(item, 10, "Stock minimo", required: false, defaultValue: 0);
                        var fechaVencimiento = GetStr(item, 11, "Fecha de vencimiento", required: false);

                        var unidadPorCompra = GetStr(item, 12, "Unidad por compra", required: false);
                        if (string.IsNullOrWhiteSpace(unidadPorCompra)) unidadPorCompra = "-";

                        var unidadPorVenta = GetStr(item, 13, "Unidad por venta", required: false);
                        if (string.IsNullOrWhiteSpace(unidadPorVenta)) unidadPorVenta = "-";

                        var equivalencia = GetDec(item, 14, "Equivalencia", required: false, defaultValue: 0m);

                        var precioCompra = GetStr(item, 15, "Precio de compra", required: false);
                        var precioPublico = GetStr(item, 16, "Precio PUBLICO", required: false);
                        var precioInterno = GetStr(item, 17, "Precio INTERNO", required: false);
                        var precioVip = GetStr(item, 18, "Precio de VIP", required: false);

                        var ubicacion = GetStr(item, 19, "Ubicacion", required: false);
                        var lote = GetStr(item, 20, "Lote", required: false);
                        var facturado = GetStr(item, 21, "Facturado", required: false);
                        var politicasDevolucion = GetStr(item, 22, "Politicas de devolucion DEFECTUOSO", required: false);
                        var politicasDevolucionVencimiento = GetStr(item, 23, "Politicas de devolucion VENCIMIENTO", required: false);

                        // Re-aplicar regla del lote referencial
                        if (string.IsNullOrWhiteSpace(lote) || lote == "-")
                        {
                            lote = $"LOTE-REF-{DateTime.Now:yyyyMMdd}-{codigoReferencia}";
                        }

                        // Dependencias
                        var laboratorioProducto = new LaboratorioProducto { NombreLaboratorioProducto = UpperOrEmpty(casaMedica), Eliminado = false };
                        var viadmin = new Viadmin { NombreViadmin = UpperOrEmpty(viaAdministracion), Eliminado = false };
                        var unidadMedidaVenta = new UnidadMedidaVenta { Nombre = UpperOrEmpty(unidadPorVenta), Abreviatura = Abrev3(unidadPorVenta, "Unidad Venta") };
                        var unidadMedidaCompra = new UnidadMedidaCompra { Nombre = UpperOrEmpty(unidadPorCompra), Abreviatura = Abrev3(unidadPorCompra, "Unidad Compra") };
                        var presentacionProducto = new PresentacionProducto { PresentProducto = descripcion, Eliminado = false };

                        Producto producto;
                        if (productosExistentes.Any(a => a.NombreProducto == nombre))
                        {
                            producto = productosExistentes.First(a => a.NombreProducto == nombre);
                            producto.ProductosInventario ??= new List<ProductoInventario>();
                            producto.ProductoEquivalencias ??= new List<ProductoEquivalencia>();
                        }
                        else
                        {
                            producto = new Producto
                            {
                                ViadminId = _productosRepository.ExistOrAddViaAdmin(viadmin),
                                TipoProductoId = dto.TipoProductoId,
                                PresentacionProductoId = _productosRepository.ExistOrAddPresentacionProducto(presentacionProducto),
                                LaboratorioProductoId = _productosRepository.ExistOrAddLaboratorioProducto(laboratorioProducto),
                                AmbienteId = _bodegaRepository.GetById(dto.BodegaId)?.AmbienteId,
                                NombreProducto = nombre,
                                CodigoReferencia = codigoReferencia,
                                Imagen = "",
                                Descripcion = descripcion,
                                ActivoYConcentracion = activoConcentracion,
                                Dosis = "",
                                Eliminado = false,
                                Ubicacion = ubicacion,
                                ProductosInventario = new List<ProductoInventario>(),
                                ProductoEquivalencias = new List<ProductoEquivalencia>()
                            };
                            productosExistentes.Add(producto);
                        }

                        var precioCostoConvert = string.IsNullOrWhiteSpace(precioCompra) ? 0m : GetDec(item, 15, "Precio de compra", required: false, defaultValue: 0m);
                        DateTime? fechaRecLote = string.IsNullOrWhiteSpace(fechaIngreso) ? (DateTime?)null : GetDate(item, 2, "Fecha de ingreso", required: false);
                        var fechaVencCompra = string.IsNullOrWhiteSpace(fechaVencimiento) ? new DateTime(1, 1, 1) : GetDate(item, 11, "Fecha de vencimiento", required: false) ?? new DateTime(1, 1, 1);

                        // Validamos si el lote ya existe en esta bodega para este producto
                        var inventarioExistente = producto.ProductosInventario.FirstOrDefault(inv => inv.Lote == lote && inv.BodegaId == dto.BodegaId && !inv.Eliminado);

                        if (inventarioExistente != null)
                        {
                            // Si existe, SUMAMOS al stock (Regla #3 confirmada)
                            inventarioExistente.Stock += stockPorUnidad;
                            _productosRepository.Update(producto);
                        }
                        else
                        {
                            // Si no existe, CREAMOS un nuevo registro de inventario
                            var nuevoInventario = new ProductoInventario
                            {
                                UnidadMedidaVentaId = _productosRepository.ExistOrAddUnidadMedidaVenta(unidadMedidaVenta),
                                UnidadMedidaCompraId = _productosRepository.ExistOrAddUnidadMedidaCompra(unidadMedidaCompra),
                                BodegaId = dto.BodegaId,
                                Stock = stockPorUnidad,
                                Facturado = (!string.IsNullOrWhiteSpace(facturado) && facturado.Trim().ToUpperInvariant() == "SI"),
                                PoliticaDevolucionPersonalizadaDias = int.TryParse(politicasDevolucion, out int diasDevolucion) ? diasDevolucion : 0,
                                ManejaPoliticaDevolucionPersonalizada = int.TryParse(politicasDevolucion, out _),
                                Lote = lote,
                                StockMinimo = stockMinimo,
                                Eliminado = false,
                                FechaRecepcionLote = fechaRecLote,
                                PrecioCosto = precioCostoConvert,
                                FechaVencimientoArticuloCompra = fechaVencCompra
                            };

                            producto.ProductosInventario.Add(nuevoInventario);

                            // Aseguramos que la equivalencia exista
                            if (!producto.ProductoEquivalencias.Any(pe => pe.UnidadMedidaCompraId == nuevoInventario.UnidadMedidaCompraId && pe.UnidadMedidaVentaId == nuevoInventario.UnidadMedidaVentaId))
                            {
                                producto.ProductoEquivalencias.Add(new ProductoEquivalencia
                                {
                                    CantidadEquivalenteDestino = equivalencia,
                                    UnidadMedidaCompraId = nuevoInventario.UnidadMedidaCompraId,
                                    UnidadMedidaVentaId = nuevoInventario.UnidadMedidaVentaId,
                                    Eliminada = false
                                });
                            }

                            _productosRepository.Add(producto);

                            // Forzamos el guardado para obtener el ID de inventario necesario para los precios
                            _context.SaveChanges();

                            var invId = nuevoInventario.Id;

                            var precioPublicoConvert = string.IsNullOrWhiteSpace(precioPublico) ? 0m : GetDec(item, 16, "Precio PUBLICO", required: false, defaultValue: 0m);
                            var precioInternoConvert = string.IsNullOrWhiteSpace(precioInterno) ? 0m : GetDec(item, 17, "Precio INTERNO", required: false, defaultValue: 0m);
                            var precioVipConvert = string.IsNullOrWhiteSpace(precioVip) ? 0m : GetDec(item, 18, "Precio de VIP", required: false, defaultValue: 0m);

                            var precio1 = ProductoInventarioPrecios(precioPublicoConvert, 1);
                            var precio2 = ProductoInventarioPrecios(precioPublicoConvert, 27);
                            var precio3 = ProductoInventarioPrecios(precioPublicoConvert, 61);

                            if (precio1 != null) { precio1.ProductoInventarioId = invId; _productosRepository.Add(precio1); }
                            if (precio2 != null) { precio2.ProductoInventarioId = invId; _productosRepository.Add(precio2); }
                            if (precio3 != null) { precio3.ProductoInventarioId = invId; _productosRepository.Add(precio3); }
                        }
                    }

                    // 4. Registrar en el Historial para bloquear futuras cargas idénticas
                    _context.HistorialImportacionExcel.Add(new HistorialImportacionExcel
                    {
                        HashArchivo = dto.HashArchivo,
                        NombreArchivo = $"Inventario_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx",
                        FechaImportacion = DateTime.UtcNow // Práctica estándar para evitar choques de Timezone
                    });

                    _context.SaveChanges();
                    transaction.Commit();

                    // 5. Limpiar rastros temporales
                    System.IO.File.Delete(filePath);
                    HttpContext.Session.Remove($"ImportInv_{sessionKey}");

                    return JsonSerializer.Serialize(new { Exitoso = true });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Limpieza en caso de fallo crítico de base de datos
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                    HttpContext.Session.Remove($"ImportInv_{sessionKey}");

                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = $"Error al guardar en Base de Datos. Los cambios han sido revertidos. Detalles: {ex.Message}" });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = $"Error crítico del sistema: {ex.Message}" });
            }
        }
    }

}