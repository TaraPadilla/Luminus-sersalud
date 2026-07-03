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

namespace sistema.Controllers
{
    [Authorize]
    public class ServicioController : Controller
    {
        private readonly ISucursal _sucursalRepository = null;
        private readonly IServicio _servicioRepository = null;
        private readonly IProducto _productosRepository = null;
        private readonly IPrecios _preciosRepository = null;


        // private string _dir;

        public ServicioController(
            ISucursal sucursalRepository,
            IServicio servicioRepository,
            IPrecios preciosRepository,
            IProducto productosRepository
            )
        {
            _sucursalRepository = sucursalRepository;
            _servicioRepository = servicioRepository;
            _preciosRepository = preciosRepository;
            _productosRepository = productosRepository;
        }

        public IActionResult Lista()
        {
            var lista = _servicioRepository.GetListaServicios().ToList();

            return View(lista);
        }
        [HttpPost]
        public string ConsultarInsumosExistentes()
        {
            try
            {
                var insumosExistentes = new List<InsumoExistenteServicioBaseViewModel>();
                var resultado = _productosRepository.GetProductos((int)TipoBodegaEnum.Clinica, 2)
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
            // var cargarCategorias = _categoryRepository.ListarCategorias();
            var modelo = new ServicioBaseViewModel();
            return View(modelo);
        }

        [HttpPost]
        public string Nuevo(ServicioBaseViewModel model)
        {
            try
            {
                var servicio = new Servicio
                {
                    CodigoInterno = model.CodigoInterno,
                    NombreServicio = model.NombreServicio,
                    Descripcion = model.Descripcion,
                    DuracionHoras = model.DuracionHoras,
                    DuracionMinutos = model.DuracionMinutos,
                    PrecioId = model.PrecioMostrarId,
                    Eliminado = false
                };

                //Insumos utilizados
                if (model.InsumosUtilizados != null && model.InsumosUtilizados.Count > 0)
                {
                    foreach (var insumo in model.InsumosUtilizados)
                    {
                        var insumoServicio = new ServicioInsumo
                        {
                            ProductoId = insumo.ProductoId,
                            UnidadMedidaVentaId = insumo.UnidadMedidaVentaId,
                            CantidadUtilizada = insumo.CantidadUtilizada
                        };
                        servicio.ServiciosInsumos.Add(insumoServicio);
                    }
                }

                //Precios
                if (model.Precios != null)
                {
                    foreach (var precio in model.Precios)
                    {
                        servicio.ServiciosPrecios.Add(new ServicioPrecio
                        {
                            PrecioId = precio.PrecioId,
                            Valor = precio.PrecioValor,
                            Activar = precio.Activar
                        });
                    }
                }
                //Sucursales
                if (model.Sucursales != null)
                {
                    foreach (var sucursal in model.Sucursales)
                    {
                        servicio.SucursalServicios.Add(new SucursalServicio
                        {
                            SucursalId = sucursal.SucursalId,
                            Activar = sucursal.Activar
                        });
                    }
                }

                _servicioRepository.Add(servicio);
                TempData["Message"] = "¡Servicio registrado!";

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    ServicioId = servicio.Id
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al guardar servicio. " + ex.Message
                });
            }
        }
        public IActionResult Modificar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var servicio = _servicioRepository.Get((int)id);

            if (servicio == null)
            {
                return StatusCode(404);
            }

            var modelo = new ServicioBaseViewModel
            {
                Id = servicio.Id,
                CodigoInterno = servicio.CodigoInterno,
                NombreServicio = servicio.NombreServicio,
                Descripcion = servicio.Descripcion,
                DuracionHoras = servicio.DuracionHoras ?? 0,
                DuracionMinutos = servicio.DuracionMinutos ?? 0,
                PrecioMostrarId = servicio.PrecioId
            };

            return View(modelo);
        }
        [HttpPost]
        public string Modificar(ServicioBaseViewModel model)
        {
            try
            {
                var servicio = _servicioRepository.Get((int)model.Id);

                servicio.CodigoInterno = model.CodigoInterno;
                servicio.NombreServicio = model.NombreServicio;
                servicio.Descripcion = model.Descripcion;
                servicio.DuracionHoras = model.DuracionHoras;
                servicio.DuracionMinutos = model.DuracionMinutos;
                servicio.PrecioId = model.PrecioMostrarId;

                _servicioRepository.Update(servicio, false);

                //Precios
                if (model.Precios != null)
                {
                    foreach (var precio in model.Precios)
                    {
                        var precioBd = servicio.ServiciosPrecios.Where(a => a.PrecioId == precio.PrecioId)
                            .FirstOrDefault();
                        if (precioBd == null)
                        {
                            servicio.ServiciosPrecios.Add(new ServicioPrecio
                            {
                                PrecioId = precio.PrecioId,
                                Valor = precio.PrecioValor,
                                Activar = precio.Activar
                            });
                            _servicioRepository.Update(servicio, false);
                        }
                        else
                        {
                            precioBd.Valor = precio.PrecioValor;
                            precioBd.Activar = precio.Activar;

                            _servicioRepository.UpdatePrecio(precioBd);
                        }

                    }
                }

                //Insumos
                foreach (var insumoBd in servicio.ServiciosInsumos)
                {
                    insumoBd.Eliminado = true;
                }
                if (model.InsumosUtilizados != null)
                {
                    foreach (var insumoModel in model.InsumosUtilizados)
                    {
                        if (insumoModel.Nuevo)
                        {
                            servicio.ServiciosInsumos.Add(new ServicioInsumo
                            {
                                ProductoId = insumoModel.ProductoId,
                                UnidadMedidaVentaId = insumoModel.UnidadMedidaVentaId,
                                CantidadUtilizada = insumoModel.CantidadUtilizada,
                                Eliminado = false
                            });
                            _servicioRepository.Update(servicio, false);
                        }
                        else
                        {
                            var insumoEditado = servicio.ServiciosInsumos
                                .Where(a => a.Id == (int)insumoModel.Id)
                                .FirstOrDefault();
                            insumoEditado.CantidadUtilizada = insumoModel.CantidadUtilizada;
                            insumoEditado.Eliminado = false;
                            _servicioRepository.UpdateInsumo(insumoEditado, false);
                        }
                    }
                }

                _servicioRepository.SaveChanges();

                TempData["Message"] = "¡El servicio se ha modificado con exito.!";
                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al modificar servicio. " + ex.Message
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
                    Exitoso = true,
                    Mensaje = "Error de servidor al consultar precios. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarSucursalesExistentes()
        {
            try
            {
                var sucursalesExistentes = new List<ServicioSucursalViewModel>();
                var sucursalesBd = _sucursalRepository.GetList().ToList();
                if (sucursalesBd != null)
                {
                    foreach (var sucursal in sucursalesBd)
                    {
                        sucursalesExistentes.Add(new ServicioSucursalViewModel
                        {
                            Activar = true,
                            SucursalId = sucursal.Id,
                            SucursalNombre = sucursal.NombreSucursal
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = sucursalesExistentes
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar sucursales. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarSucursalesServicio(int servicioId)
        {
            try
            {
                var sucursalesServicio = new List<ServicioSucursalViewModel>();
                var sucursalesBd = _servicioRepository.GetSucursalesServicio(servicioId);

                var sucursalesExistentes = _sucursalRepository.GetList();

                if (sucursalesBd == null)
                {
                    if (sucursalesExistentes != null)
                    {
                        foreach (var sucursalExistente in sucursalesExistentes)
                        {
                            sucursalesServicio.Add(new ServicioSucursalViewModel
                            {
                                SucursalId = sucursalExistente.Id,
                                SucursalNombre = sucursalExistente.NombreSucursal,
                                Activar = true
                            });
                        }
                    }
                }
                else
                {
                    if (sucursalesExistentes != null)
                    {
                        foreach (var sucursalExistente in sucursalesExistentes)
                        {
                            var sucursalRegistradaServicio = sucursalesBd
                                .Where(a => a.SucursalId == sucursalExistente.Id)
                                .FirstOrDefault();
                            if (sucursalRegistradaServicio == null)
                            {
                                sucursalesServicio.Add(new ServicioSucursalViewModel
                                {
                                    SucursalId = sucursalExistente.Id,
                                    SucursalNombre = sucursalExistente.NombreSucursal,
                                    Activar = true
                                });
                            }
                            else
                            {
                                sucursalesServicio.Add(new ServicioSucursalViewModel
                                {
                                    SucursalId = sucursalRegistradaServicio.SucursalId,
                                    SucursalNombre = sucursalRegistradaServicio.Sucursal.NombreSucursal,
                                    Activar = sucursalRegistradaServicio.Activar
                                });
                            }
                        }
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = sucursalesServicio
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Error de servidor al consultar sucursales. " + ex.Message
                });
            }
        }

        public IActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _servicioRepository.Get((int)id);

            if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado = true;

            _servicioRepository.Update(model);
            TempData["Message"] = "¡El Servicio se ha eliminado con exito.!";

            return RedirectToAction("Lista");
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

        #region Categorias
        public IActionResult CategoriaNueva()
        {
            return View();
        }
        [HttpPost]
        public string CategoriaNueva(ServicioCategoriaViewModel model)
        {
            try
            {
                var categoria = new CategoriaServicio
                {
                    NombreCategoria = model.NombreCategoria,
                    Eliminada = false
                };
                _servicioRepository.AddCategoria(categoria);


                TempData["Message"] = "Categoría registrada";
                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al registrar categoría. " + ex.Message
                });
            }
        }
        public IActionResult CategoriaModificar(int? categoriaId)
        {
            return View();
        }
        public IActionResult CategoriasLista()
        {
            return View();
        }
        //[HttpPost]
        //public string CategoriaNueva(ServicioCategoriaViewModel model)
        //{
        //    try
        //    {
        //        var categoria = new CategoriaServicio
        //        {
        //            NombreCategoria = model.NombreCategoria,
        //            Eliminada = false
        //        };
        //        _servicioRepository.AddCategoria(categoria);


        //        TempData["Message"] = "Categoría registrada";
        //        return JsonSerializer.Serialize(new { Exitoso = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonSerializer.Serialize(new
        //        {
        //            Exitoso = false,
        //            Mensaje = "Error de servidor al registrar categoría. " + ex.Message
        //        });
        //    }
        //}
        #endregion

    }
}