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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using sistema.Json;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Database.Shared.Enumeraciones;
using farmamest.Service.IService;
using farmamest.Models;

namespace sistema.Controllers
{
    [Authorize]
    public class HospitalizacionPaquetesController : Controller
    {
        private readonly IHospitalizacion _hospitalizacionRepository = null;
        private readonly IServicio _servicioRepository = null;
        private readonly IProducto _productosRepository = null;
        private readonly IPrecios _preciosRepository = null;

        //Servicios
        private readonly IProductosService _productosService = null;
        private readonly IServiciosService _serviciosService = null;
        private readonly IHospitalizacionService _hospitalizacionService = null;

        public HospitalizacionPaquetesController(
            IHospitalizacion hospitalizacionRepository,
            IServicio servicioRepository,
            IPrecios preciosRepository,
            IProducto productosRepository,
            //Servicios
            IProductosService productosService,
            IServiciosService serviciosService,
            IHospitalizacionService hospitalizacionService
            )
        {
            _hospitalizacionRepository = hospitalizacionRepository;
            _servicioRepository = servicioRepository;
            _preciosRepository = preciosRepository;
            _productosRepository = productosRepository;

            //Servicios
            _productosService = productosService;
            _serviciosService = serviciosService;
            _hospitalizacionService = hospitalizacionService;
        }

        public IActionResult Lista()
        {
            var lista = _hospitalizacionRepository.GetPaquetesHospitalizacion();

            return View(lista);
        }
        [HttpPost]
        public string ConsultarInsumosExistentes()
        {
            try
            {
                var insumosExistentes = new List<InsumoExistenteServicioBaseViewModel>();
                var resultado = _productosRepository.GetProductos((int)TipoBodegaEnum.Clinica, null)
                    .ToList();
                if (resultado != null)
                {
                    foreach (var insumo in resultado)
                    {
                        insumosExistentes.Add(new InsumoExistenteServicioBaseViewModel
                        {
                            ProductoId = insumo.Id,
                            ProductoNombre = insumo.NombreProducto
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = insumosExistentes
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar insumos. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarProductosExistentes(int? BodegaId)
        {
            try
            {

                // var ambiente = (BodegaId == 14) ? (int)AmbienteEnum.Global : (int)AmbienteEnum.Hospital;
                // Console.WriteLine("BodegaId: " + BodegaId);
                var inventario = _productosService.GetInventario(null, BodegaId);

                if (!inventario.Any())
                {
                    return JsonSerializer.Serialize(new { Exitoso = true, Resultado = Array.Empty<object>() });
                }

                // Obtener los IDs únicos de los productos en el inventario
                var productoIds = inventario.Select(i => i.ProductoId).Distinct().ToList();

                // Consultar productos y crear el diccionario ProductoId -> TipoProductoId
                var productoTipoMap = _productosRepository
                    .GetProductosPorIds(productoIds)
                    .ToDictionary(p => p.Id, p => p.TipoProductoId);

                // Agregar TipoProductoId al inventario
                var inventarioConTipoProducto = inventario.Select(item => new
                {
                    item.Item,
                    item.ProductoId,
                    item.ProductoInventarioId,
                    item.ProductoNombre,
                    item.ProductoCodigo,
                    item.ProductoDescripcion,
                    item.ProductoActivoConcentracion,
                    item.ProductoImagen,
                    item.Stock,
                    item.BodegaId,
                    item.BodegaNombre,
                    item.UnidadMedidaVentaId,
                    item.UnidadMedidaVentaNombre,
                    item.PrecioId,
                    item.PrecioNombre,
                    item.PrecioValor,
                    item.PrecioCompra,
                    TipoProductoId = productoTipoMap.TryGetValue(item.ProductoId, out var tipoProductoId) ? tipoProductoId : (int?)null
                });

                // Devolver la respuesta como JSON optimizado
                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = inventarioConTipoProducto });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = $"Error al consultar inventario: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public string ConsultarServiciosExistentes()
        {
            try
            {
                var servicios = _serviciosService.GetServicios();

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = servicios
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar servicios. " + ex.Message
                });
            }
        }
        [HttpPost]
        public JsonResult ConsultarUnidadesVentaInsumo(int productoId)
        {
            try
            {
                var producto = _productosRepository.GetProductos(null, null)
                    .Where(p => p.Id == productoId)
                    .FirstOrDefault();

                return Json(new { Exitoso = true, Resultado = producto.ProductoEquivalencias });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al consultar unidades de venta. " + ex.Message });
            }
        }
        [HttpPost]
        public string ConsultarPreciosExistentes()
        {
            try
            {
                var preciosExistentes = new List<PrecioServicioBaseViewModel>();
                var preciosBd = _preciosRepository.GetList().ToList();
                if (preciosBd != null)
                {
                    foreach (var precio in preciosBd)
                    {
                        preciosExistentes.Add(new PrecioServicioBaseViewModel
                        {
                            Activar = true,
                            PrecioId = precio.Id,
                            PrecioNombre = precio.NombrePrecio
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = preciosExistentes
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar precios. " + ex.Message
                });
            }
        }
        public IActionResult Nuevo()
        {
            var modelo = new HospitalizacionPaqueteViewModel();
            return View(modelo);
        }

        [HttpPost]
        public string Nuevo(HospitalizacionPaqueteViewModel model)
        {
            try
            {
                var fechaHora = DateTime.Now;
                var paquete = new PaqueteHospitalizacion
                {
                    CodigoInterno = model.CodigoInterno,
                    NombrePaquete = model.Nombre,
                    Descripcion = model.Descripcion,
                    FechaHoraCreacion = fechaHora,
                    Precio = model.PrecioPaquete,
                    PrecioCosto = model.PrecioCosto,
                    Eliminado = false
                };

                //Agregar productos
                if (model.Productos != null)
                {
                    foreach (var producto in model.Productos)
                    {
                        paquete.DetallePaquetesHospitalizacion.Add(new DetallePaqueteHospitalizacion
                        {
                            ProductoId = producto.ProductoId,
                            Cantidad = producto.Cantidad,
                            UnidadMedidaVentaId = producto.UnidadMedidaVentaId,
                            PrecioValor = producto.PrecioValor,
                            PrecioCosto = producto.PrecioCompra,
                            PrecioId = producto.ProductoPrecioId
                        });
                    }
                }
                //Agregar servicios
                if (model.Servicios != null)
                {
                    foreach (var servicio in model.Servicios)
                    {
                        paquete.DetallePaquetesHospitalizacion.Add(new DetallePaqueteHospitalizacion
                        {
                            ServicioId = servicio.ServicioId,
                            Cantidad = servicio.Cantidad,
                            PrecioValor = servicio.PrecioValor,
                            PrecioId = servicio.PrecioId
                        });
                    }
                }


                //Agregar laboratorios
                if (model.Laboratorios != null)
                {
                    foreach (var labs in model.Laboratorios)
                    {
                        paquete.DetallePaquetesHospitalizacion.Add(new DetallePaqueteHospitalizacion
                        {
                            LaboratorioId = labs.Id,
                            Cantidad = labs.Cantidad,
                            DescuentoPorcentaje = labs.DescuentoPorcentaje,
                            LaboratorioPrecioId = labs.PrecioId,
                            PrecioValor = labs.PrecioValor
                        });
                    }
                }

                _hospitalizacionRepository.AddPaqueteHospitalizacion(paquete);

                TempData["Message"] = "¡El paquete se ha registrado con exito.!";

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
                    Mensaje = "Error de servidor al registrar paquete. " + ex.Message
                });
            }
        }
        public IActionResult Modificar(int? paqueteId)
        {
            if (paqueteId == null)
            {
                TempData["Message"] = "Error de navegacion: Request is incorrect";
                return RedirectToAction("Lista");
            }

            var paquete = _hospitalizacionRepository.GetPaqueteHospitalizacion((int)paqueteId);

            if (paquete == null)
            {
                TempData["Message"] = "El paquete no existe";
                return RedirectToAction("Lista");
            }

            var modelo = new HospitalizacionPaqueteViewModel
            {
                Id = paquete.Id,
                CodigoInterno = paquete.CodigoInterno,
                Nombre = paquete.NombrePaquete,
                Descripcion = paquete.Descripcion,
                PrecioPaquete = paquete.Precio ?? 0
            };
            //los elementos se cargan mediante llamado ajax desde JS

            return View(modelo);
        }
        public IActionResult Detalles(int? paqueteId)
        {
            if (paqueteId == null)
            {
                TempData["Message"] = "Error de navegacion: Request is incorrect";
                return RedirectToAction("Lista");
            }

            var paquete = _hospitalizacionRepository.GetPaqueteHospitalizacion((int)paqueteId);
            if (paquete != null)
            {
                paquete.DetallePaquetesHospitalizacion = paquete.DetallePaquetesHospitalizacion
                    .Where(a => !a.Eliminado)
                    .ToList();
            }

            if (paquete == null)
            {
                TempData["Message"] = "El paquete no existe";
                return RedirectToAction("Lista");
            }


            return View(paquete);
        }

        [HttpPost]
        public string Modificar(HospitalizacionPaqueteViewModel model)
        {
            try
            {
                var paquete = _hospitalizacionRepository.GetPaqueteHospitalizacion((int)model.Id);

                paquete.CodigoInterno = model.CodigoInterno;
                paquete.NombrePaquete = model.Nombre;
                paquete.Descripcion = model.Descripcion;
                paquete.Precio = model.PrecioPaquete;

                #region Productos

                if (model.Productos != null)
                {
                    foreach (var producto in model.Productos)
                    {
                        if (producto.Nuevo)
                        {
                            paquete.DetallePaquetesHospitalizacion.Add(new DetallePaqueteHospitalizacion
                            {
                                Cantidad = producto.Cantidad,
                                PrecioId = producto.ProductoPrecioId,
                                PrecioCosto = producto.PrecioCompra,
                                DescuentoPorcentaje = producto.DescuentoPorcentaje,
                                PrecioValor = producto.PrecioValor,
                                ProductoId = producto.ProductoId,
                                UnidadMedidaVentaId = producto.UnidadMedidaVentaId
                            });
                        }
                        else
                        {
                            var detalle = paquete.DetallePaquetesHospitalizacion
                                .FirstOrDefault(a => a.Id == producto.DetallePaqueteId);
                            if (detalle != null)
                            {
                                detalle.Cantidad = producto.Cantidad;
                                detalle.DescuentoPorcentaje = producto.DescuentoPorcentaje;
                                detalle.PrecioValor = producto.PrecioValor;
                                detalle.PrecioCosto = producto.PrecioCompra;
                                detalle.Eliminado = producto.Eliminado;
                            }
                        }
                    }
                }

                #endregion

                #region Servicios

                if (model.Servicios != null)
                {
                    foreach (var servicio in model.Servicios)
                    {
                        if (servicio.Nuevo)
                        {
                            paquete.DetallePaquetesHospitalizacion.Add(new DetallePaqueteHospitalizacion
                            {
                                Cantidad = servicio.Cantidad,
                                PrecioId = servicio.PrecioId,
                                DescuentoPorcentaje = servicio.DescuentoPorcentaje,
                                PrecioValor = servicio.PrecioValor,
                                ServicioId = servicio.ServicioId
                            });
                        }
                        else
                        {
                            var detalle = paquete.DetallePaquetesHospitalizacion
                                .FirstOrDefault(a => a.Id == servicio.DetallePaqueteId);
                            if (detalle != null)
                            {
                                detalle.Cantidad = servicio.Cantidad;
                                detalle.DescuentoPorcentaje = servicio.DescuentoPorcentaje;
                                detalle.PrecioValor = servicio.PrecioValor;
                                detalle.Eliminado = servicio.Eliminado;
                            }
                        }
                    }
                }

                #endregion

                #region Laboratorios

                if (model.Laboratorios != null)
                {
                    foreach (var laboratorio in model.Laboratorios)
                    {
                        if (laboratorio.Nuevo)
                        {
                            paquete.DetallePaquetesHospitalizacion.Add(new DetallePaqueteHospitalizacion
                            {
                                Cantidad = laboratorio.Cantidad,
                                LaboratorioPrecioId = laboratorio.PrecioId,
                                DescuentoPorcentaje = laboratorio.DescuentoPorcentaje,
                                PrecioValor = laboratorio.PrecioValor,
                                LaboratorioId = laboratorio.Id
                            });
                        }
                        else
                        {
                            var detalle = paquete.DetallePaquetesHospitalizacion
                                .FirstOrDefault(a => a.Id == laboratorio.DetallePaqueteId);
                            if (detalle != null)
                            {
                                detalle.Cantidad = laboratorio.Cantidad;
                                detalle.DescuentoPorcentaje = laboratorio.DescuentoPorcentaje;
                                detalle.PrecioValor = laboratorio.PrecioValor;
                                detalle.Eliminado = laboratorio.Eliminado;
                            }
                        }
                    }
                }

                #endregion

                paquete.PrecioCosto = paquete.DetallePaquetesHospitalizacion
                    .Where(d => !d.Eliminado)
                    .Sum(d => d.PrecioCosto * d.Cantidad);

                _hospitalizacionRepository.UpdatePaqueteHospitalizacion(paquete);

                TempData["Message"] = "¡El paquete se ha modificado con éxito!";
                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al modificar paquete. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarInsumosServicio(int servicioId)
        {
            try
            {
                var insumosServicio = new List<InsumoServicioBaseViewModel>();
                var insumosBd = _servicioRepository.GetInsumosServicio(servicioId);

                if (insumosBd != null)
                {
                    foreach (var insumo in insumosBd)
                    {
                        insumosServicio.Add(new InsumoServicioBaseViewModel
                        {
                            Id = insumo.Id,
                            ProductoId = insumo.ProductoId,
                            ProductoNombre = insumo.Producto.NombreProducto,
                            CantidadUtilizada = insumo.CantidadUtilizada,
                            UnidadMedidaVentaId = insumo.UnidadMedidaVentaId,
                            UnidadMedidaVentaNombre = insumo.UnidadMedidaVenta.Nombre,
                            Nuevo = false
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = insumosServicio
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Error de servidor al consultar insumos. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarPreciosServicio(int servicioId)
        {
            try
            {
                var preciosServicio = new List<PrecioServicioBaseViewModel>();
                var preciosBd = _servicioRepository.GetPreciosServicio(servicioId);

                var preciosExistentes = _preciosRepository.GetList();

                if (preciosBd == null)
                {
                    if (preciosExistentes != null)
                    {
                        foreach (var precioExistente in preciosExistentes)
                        {
                            preciosServicio.Add(new PrecioServicioBaseViewModel
                            {
                                PrecioId = precioExistente.Id,
                                PrecioNombre = precioExistente.NombrePrecio,
                                Activar = true
                            });
                        }
                    }
                }
                else
                {
                    if (preciosExistentes != null)
                    {
                        foreach (var precioExistente in preciosExistentes)
                        {
                            var precioRegistradoServicio = preciosBd
                                .Where(a => a.PrecioId == precioExistente.Id)
                                .FirstOrDefault();
                            if (precioRegistradoServicio == null)
                            {
                                preciosServicio.Add(new PrecioServicioBaseViewModel
                                {
                                    PrecioId = precioExistente.Id,
                                    PrecioNombre = precioExistente.NombrePrecio,
                                    Activar = true
                                });
                            }
                            else
                            {
                                preciosServicio.Add(new PrecioServicioBaseViewModel
                                {
                                    PrecioId = precioRegistradoServicio.PrecioId,
                                    PrecioNombre = precioRegistradoServicio.Precio.NombrePrecio,
                                    PrecioValor = precioRegistradoServicio.Valor,
                                    Activar = precioRegistradoServicio.Activar
                                });
                            }
                        }
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
                    Mensaje = "Error de servidor al consultar precios. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarElementosPaquete(int paqueteId)
        {
            try
            {
                var paquete = _hospitalizacionRepository.GetPaqueteHospitalizacion(paqueteId);

                var productos = new List<HospitalizacionPaqueteProductoAgregadoViewModel>();
                var laboratorios = new List<HospitalizacionPaqueteLaboratorioAgregadoViewModel>();
                var servicios = new List<HospitalizacionPaqueteServicioAgregadoViewModel>();

                if (paquete != null)
                {
                    if (paquete.DetallePaquetesHospitalizacion != null)
                    {
                        var detallesActivos = paquete.DetallePaquetesHospitalizacion
    .Where(d => !d.Eliminado)
    .ToList();
                        foreach (var detalle in detallesActivos)
                        {
                            //Servicios
                            if (detalle.ServicioId != null)
                            {
                                var precio = detalle.ServicioPrecio ?? new ServicioPrecio();
                                servicios.Add(new HospitalizacionPaqueteServicioAgregadoViewModel
                                {
                                    DetallePaqueteId = detalle.Id,
                                    Cantidad = Convert.ToInt32(detalle.Cantidad),
                                    ServicioId = (int)detalle.ServicioId,
                                    ServicioCodigo = detalle.Servicio.CodigoInterno,
                                    ServicioNombre = detalle.Servicio.NombreServicio,
                                    PrecioId = detalle.ServicioPrecioId,
                                    Nuevo = false,
                                    Precio = precio.Precio != null ? precio.Precio.NombrePrecio : "-",
                                    PrecioValor = detalle.PrecioValor,
                                    DescuentoPorcentaje = detalle.DescuentoPorcentaje ?? 0,
                                    Eliminado = detalle.Eliminado
                                });
                            }
                            //Productos
                            if (detalle.ProductoId != null)
                            {
                                var unidadVenta = detalle.UnidadMedidaVenta ?? new UnidadMedidaVenta();
                                var precio = detalle.Precio ?? new Precio();
                                var subtotal = detalle.Cantidad * detalle.PrecioValor;
                                productos.Add(new HospitalizacionPaqueteProductoAgregadoViewModel
                                {
                                    DetallePaqueteId = detalle.Id,
                                    Cantidad = Convert.ToInt32(detalle.Cantidad),
                                    ProductoId = (int)detalle.ProductoId,
                                    ProductoCodigo = detalle.Producto.CodigoReferencia,
                                    DescuentoPorcentaje = detalle.DescuentoPorcentaje ?? 0,
                                    Subtotal = subtotal,
                                    UnidadMedidaVentaId = detalle.UnidadMedidaVentaId,
                                    ValorTotal = subtotal - (subtotal * (detalle.DescuentoPorcentaje ?? 0 / 100)),
                                    ProductoNombre = detalle.Producto.NombreProducto,
                                    ProductoPrecioId = detalle.PrecioId,
                                    Nuevo = false,
                                    Precio = precio != null ? precio.NombrePrecio : "-",
                                    PrecioCompra = detalle.PrecioCosto,
                                    UnidadMedidaVentaNombre = unidadVenta.Nombre,
                                    PrecioValor = detalle.PrecioValor,
                                    Eliminado = detalle.Eliminado
                                });
                            }
                            //Laboratorios
                            if (detalle.LaboratorioId != null)
                            {
                                var precio = detalle.LaboratorioPrecio ?? new ExamenLabClinicoPrecio();
                                laboratorios.Add(new HospitalizacionPaqueteLaboratorioAgregadoViewModel
                                {
                                    DetallePaqueteId = detalle.Id,
                                    Cantidad = Convert.ToInt32(detalle.Cantidad),
                                    Id = (int)detalle.LaboratorioId,
                                    NombreExamen = detalle.Laboratorio.NombreExamen,
                                    PrecioId = detalle.LaboratorioPrecioId,
                                    Nuevo = false,
                                    Precio = precio.Precio != null ? precio.Precio.NombrePrecio : "-",
                                    PrecioValor = detalle.PrecioValor,
                                    PrecioCompra = 0,
                                    DescuentoPorcentaje = detalle.DescuentoPorcentaje ?? 0,
                                    Eliminado = detalle.Eliminado
                                });
                            }
                        }
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Productos = productos,
                    Laboratorios = laboratorios,
                    Servicios = servicios
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar elementos de paquete. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string Eliminar(int? paqueteId)
        {
            if (paqueteId == null)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de navegacion"
                });
            }

            var paquete = _hospitalizacionRepository.GetPaqueteHospitalizacion((int)paqueteId);

            if (paquete == null)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Paquete no encontrado"
                });
            }

            _hospitalizacionRepository.DeletePaqueteHospitalizacion((int)paqueteId);

            TempData["Message"] = "¡El paquete se ha eliminado con exito.!";

            return JsonSerializer.Serialize(new
            {
                Exitoso = true
            });
        }
        public JsonResult RetornarServicios(string id)
        {
            var servicioBuscado = _servicioRepository.Get(Convert.ToInt16(id));

            if (servicioBuscado == null)
            {
                return new JsonErrorResult(new { message = "" });
            }

            var objetos = new { id = servicioBuscado.Id, nombreServicio = servicioBuscado.NombreServicio, precio = servicioBuscado.Precio };

            return Json(objetos);
        }
        public JsonResult RetornarServiciosPorNombre(string nombre)
        {
            var servicioBuscado = _servicioRepository.GetNombre(nombre);

            if (servicioBuscado == null)
            {
                return new JsonErrorResult(new { message = "" });
            }

            return Json(servicioBuscado);
        }
        [HttpPost]
        public string ConsultarPacientes()
        {
            try
            {
                var pacientes = _hospitalizacionService.GetPacientesExistentes();
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = pacientes
                });
            }
            catch
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false
                });
            }
        }
    }
}