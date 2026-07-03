using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Database.Shared.IRepository;
using sistema.Models;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using sistema.Models.DataTables;
using System.IO;
using sistema.Helpers;

namespace sistema.Controllers
{
    [Authorize]
    public class DevolucionController : Controller
    {
        private readonly IProducto _productoRepository;
        private readonly IBodega _bodegaRepository;
        private readonly IRequision _requisionRepository;
        private readonly IDevolucionNacional _devolucionRepository;
        private readonly IUser _usuarioRepository;
        private readonly IEmpleado _empleadoRepository;

        private readonly IProveedor _proveedorRepository;


        public DevolucionController(
            IProducto productoRepository,
            IBodega bodegaRepository,
            IRequision requisionRepository,
            IDevolucionNacional devolucionRepository,
            IUser usuarioRepository,
            IEmpleado empleadoRepository,
            IProveedor proveedorRepository)
        {
            _productoRepository = productoRepository;
            _bodegaRepository = bodegaRepository;
            _requisionRepository = requisionRepository;
            _devolucionRepository = devolucionRepository;
            _usuarioRepository = usuarioRepository;
            _empleadoRepository = empleadoRepository;
            _proveedorRepository = proveedorRepository;

        }

        private bool TryGetEmpleadoActual(out Empleado empleado, out string errorMessage)
        {
            empleado = null;
            errorMessage = null;

            var username = User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "No se pudo identificar al usuario en sesi?n.";
                return false;
            }

            var usuario = _usuarioRepository.Get(username);
            if (usuario?.EmpleadoId == null)
            {
                errorMessage = "Su usuario no tiene un empleado asociado. Contacte al administrador para vincular su cuenta.";
                return false;
            }

            empleado = _empleadoRepository.Get(usuario.EmpleadoId.Value);
            if (empleado == null)
            {
                errorMessage = "No se encontr? el registro del empleado asociado a su usuario.";
                return false;
            }

            return true;
        }

        [HttpGet]
        public IActionResult Nuevo()
        {
            var model = new DevolucionViewModel();
            model.Init(_bodegaRepository, rolFarmacia: false);
            model.EstadoRequisicion = RequisicionEstado.EnviadoAJefatura;
            return View(model);
        }
        [HttpGet]
        public IActionResult Lista()
        {

            return View();
        }

        [HttpPost]
        public string ConsultarProductosInventario(int bodegaOrigenId)
        {
            try
            {
                var listaProductos = new List<TrasladoProductoDisponibleViewModel>();
                var productosInventario = _productoRepository.GetInventarioDisponiblePorBodega(bodegaOrigenId);

                if (productosInventario != null)
                {
                    foreach (var producto in productosInventario)
                    {
                        listaProductos.Add(new TrasladoProductoDisponibleViewModel
                        {
                            ProductoInventarioId = producto.Id,
                            ProductoId = producto.ProductoId,
                            ProductoNombre = producto.Producto != null ? producto.Producto.NombreProducto : "-",
                            ProductoCodigo = producto.Producto != null ? producto.Producto.CodigoReferencia : "-",
                            CantidadExistente = producto.Stock,
                            UnidadMedidaVentaId = producto.UnidadMedidaVentaId ?? 0,
                            UnidadMedidaVentaNombre = producto.UnidadMedidaVenta != null ? producto.UnidadMedidaVenta.Nombre : "-",
                            CantidadTrasladada = 0
                        });
                    }
                }

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaProductos });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos de inventario. " + ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Guardar(DevolucionViewModel entidad)
        {
            try
            {
                string username = User.Identity?.Name;

                var usuario = _usuarioRepository.Get(username);

                if (usuario?.EmpleadoId == null)
                {
                    return Content(JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Usuario no tiene empleado asociado."
                    }), "application/json");
                }

                var empleado = _empleadoRepository.Get(usuario.EmpleadoId.Value);

                if (empleado == null)
                {
                    return Content(JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Empleado no encontrado."
                    }), "application/json");
                }

                string firmaFinal = entidad.FirmaSolicitante;

                if (string.IsNullOrWhiteSpace(firmaFinal))
                {
                    firmaFinal = empleado.FirmaEmpleado;
                }

                var modeloBD = new DevolucionNacional
                {
                    Direccion = entidad.Direccion,
                    Departamento = entidad.Departamento,
                    UnidadSeccion = entidad.UnidadSeccion,
                    Otros = entidad.Otros,
                    Observaciones = entidad.Observaciones,
                    BodegaOrigenId = entidad.BodegaOrigenId,
                    ProveedorId = entidad.ProveedorId,
                    FirmaSolicitante = firmaFinal,
                    SolicitanteNombre = empleado.NombreYApellidos,
                    Estado = DevolucionEstado.Borrador,
                    NumeroOficio      = entidad.NumeroOficio
                };

                if (entidad.Productos != null && entidad.Productos.Any())
                {
                    foreach (var p in entidad.Productos)
                    {
                        if (!p.ProductoInventarioId.HasValue || p.ProductoInventarioId <= 0 || p.CantidadTrasladada <= 0)
                            continue;
                        modeloBD.Detalles.Add(new DevolucionNacionalDetalle
                        {
                            ProductoInventarioId = p.ProductoInventarioId.Value,
                            CantidadDevuelta = (int)p.CantidadTrasladada,
                        });
                    }
                }

                var resultado = await _devolucionRepository.CrearAsync(modeloBD, username);

                return Content(JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    DevolucionId = resultado.Id
                }), "application/json");
            }
            catch (Exception ex)
            {
                return Content(JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al guardar la devoluci?n. " + ex.Message
                }), "application/json");
            }
        }



        [HttpGet]
        public async Task<IActionResult> DetalleModal(int id)
        {
            var devolucion = await _devolucionRepository.GetByIdAsync(id);

            if (devolucion == null)
                return Content("<div class='alert alert-danger'>No se encontr? la devoluci?n</div>");

            return PartialView("_DetalleDevolucionPartial", devolucion);
        }


        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var devolucion = await _devolucionRepository.GetByIdAsync(id);

            if (devolucion == null)
            {
                return NotFound();
            }

            return View("Detalle", devolucion);
        }

        [HttpPost]
        public async Task<IActionResult> ListaData()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 10;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            try
            {
                var data = await _devolucionRepository.GetAllAsync();
                int total = data.Count;

                var rows = data.Skip(skip).Take(pageSize).Select(x => new
                {
                    id = x.Id,
                    numeroDevolucion = x.NumeroDevolucion,
                    departamentoNombre = DataTablesJsonHelper.DtStr(x.DepartamentoNombre),
                    unidadNombre = DataTablesJsonHelper.DtStr(x.UnidadNombre),
                    fechaSolicitud = x.FechaSolicitud,
                    solicitanteNombre = DataTablesJsonHelper.DtStr(x.SolicitanteNombre),
                    bodegaOrigenNombre = DataTablesJsonHelper.DtStr(x.BodegaOrigenNombre),
                    autorizadoPor = DataTablesJsonHelper.DtStr(x.AutorizadoPor),
                    estadoNombre = DevolucionEstado.ObtenerNombre(x.Estado)
                }).ToList();

                return DataTablesJsonHelper.Ok(new
                {
                    draw,
                    recordsFiltered = total,
                    recordsTotal = total,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return DataTablesJsonHelper.Ok(new
                {
                    draw,
                    recordsFiltered = 0,
                    recordsTotal = 0,
                    data = Array.Empty<object>(),
                    error = "Error al cargar devoluciones. " + ex.Message
                });
            }
        }

        [HttpGet]
        public string ConsultarUltimoRegistro()
        {
            try
            {
                var ultimoNumero = _devolucionRepository.ObtenerUltimoRegistro();
                var proximo = (ultimoNumero ?? 0) + 1;

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Consulta realizada correctamente",
                    Resultado = new
                    {
                        ProximoDevolucion = proximo,
                        NumeroOficio = proximo.ToString("D3")
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al intentar obtener la ?ltima devoluci?n: " + ex.Message
                });
            }
        }



        [HttpGet]
        public string ConsultarProveedores()
        {
            try
            {
                var proveedores = _proveedorRepository.GetList();

                var resultado = proveedores.Select(p => new
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Telefono = p.Telefono_1,
                    Direccion = p.Direccion
                }).ToList();

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Consulta realizada correctamente",
                    Resultado = resultado
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al intentar obtener proveedores: " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarDataUsuario()
        {
            try
            {
                string username = User.Identity.Name;
                var usuario = _usuarioRepository.Get(username);

                if (usuario?.EmpleadoId == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Usuario no tiene empleado asociado." });

                var empleado = _empleadoRepository.Get(usuario.EmpleadoId.Value);

                if (empleado == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "No se encontr? la data del empleado." });

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Consulta realizada correctamente",
                    Data = new
                    {
                        UnidadNombre = empleado.UnidadOrg?.Nombre ?? "Sin Unidad",
                        DepartamentoNombre = empleado.UnidadOrg?.DepartamentoOrg?.Nombre ?? "Sin Departamento",
                        EmpleadoId = empleado.Id,
                        Firma = empleado.FirmaEmpleado
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al intentar obtener la data del usuario: " + ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarFirmaUsuario(string firmaBase64)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(firmaBase64))
                    return Json(new { success = false, message = "No se recibi? la informaci?n de la firma." });

                if (!TryGetEmpleadoActual(out var empleadoDb, out var errorEmpleado))
                    return Json(new { success = false, message = errorEmpleado });

                var carpetaFirmas = "Firmas";
                var pathRaiz = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", carpetaFirmas);
                if (!Directory.Exists(pathRaiz)) Directory.CreateDirectory(pathRaiz);

                if (!string.IsNullOrEmpty(empleadoDb.FirmaEmpleado))
                {
                    try
                    {
                        var nombreArchivoViejo = Path.GetFileName(empleadoDb.FirmaEmpleado);
                        var rutaViejaFull = Path.Combine(pathRaiz, nombreArchivoViejo);
                        if (System.IO.File.Exists(rutaViejaFull)) System.IO.File.Delete(rutaViejaFull);
                    }
                    catch { }
                }

                string nombreLimpio = (empleadoDb.Nombre ?? "Empleado").Replace(" ", "_");
                string sanitizedFileName = $"{nombreLimpio}_Firma_{Guid.NewGuid()}.png";
                var base64Data = firmaBase64.Contains(",") ? firmaBase64.Split(',')[1] : firmaBase64;
                byte[] imageBytes = Convert.FromBase64String(base64Data);

                await System.IO.File.WriteAllBytesAsync(Path.Combine(pathRaiz, sanitizedFileName), imageBytes);

                empleadoDb.FirmaEmpleado = $"/{carpetaFirmas}/{sanitizedFileName}";
                _empleadoRepository.Update(empleadoDb);

                return Json(new { success = true, message = "Firma actualizada exitosamente.", rutaFirma = empleadoDb.FirmaEmpleado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno: " + ex.Message });
            }
        }



        [HttpGet]
        public async Task<IActionResult> AutorizacionModal(int id, string tipo)
        {
            var devolucion = await _devolucionRepository.GetByIdAsync(id);

            if (devolucion == null)
                return Content("<div class='alert alert-danger'>No se encontr? la devoluci?n.</div>");

            ViewBag.TipoFirma = tipo;
            return PartialView("_AutorizacionDevolucionModal", devolucion);
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarAutorizacionFirma(int id, string firmaBase64, string tipoFirma, string nombreJefatura, string nombreGerencia, string nombreAlmacen)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(firmaBase64))
                    return Json(new { success = false, message = "No se recibi? la firma." });

                var devolucion = await _devolucionRepository.GetByIdAsync(id);

                if (devolucion == null)
                    return Json(new { success = false, message = "Devoluci?n no encontrada." });

                if (tipoFirma.ToLower().Trim() != "autorizacion")
                    return Json(new { success = false, message = "Tipo de firma no reconocido." });

                string rutaWeb;

                bool esRuta = firmaBase64.StartsWith("/") || firmaBase64.StartsWith("http");
                if (esRuta)
                {
                    rutaWeb = firmaBase64;
                }
                else
                {
                    string tipoLimpio = string.Concat(tipoFirma.Where(char.IsLetterOrDigit)).ToLower();
                    var carpeta = "FirmasDevolucion";
                    var pathRaiz = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", carpeta);
                    if (!Directory.Exists(pathRaiz)) Directory.CreateDirectory(pathRaiz);

                    foreach (var f in Directory.GetFiles(pathRaiz, $"{tipoLimpio}_{id}_*"))
                        System.IO.File.Delete(f);

                    string nombreArchivo = $"{tipoLimpio}_{id}_{Guid.NewGuid().ToString()[..8]}.png";
                    rutaWeb = $"/{carpeta}/{nombreArchivo}";

                    var base64Data = firmaBase64.Contains(",") ? firmaBase64.Split(',')[1] : firmaBase64;
                    await System.IO.File.WriteAllBytesAsync(
                        Path.Combine(pathRaiz, nombreArchivo),
                        Convert.FromBase64String(base64Data));
                }

                if (!TryGetEmpleadoActual(out var empleado, out var errorEmpleado))
                    return Json(new { success = false, message = errorEmpleado });

                devolucion.FirmaAutorizacion = rutaWeb;
                devolucion.AutorizadoPor = empleado.NombreYApellidos;
                int nuevoEstado = DevolucionEstado.Autorizado;

                await _devolucionRepository.ActualizarFirmaAsync(devolucion);
                await _devolucionRepository.CambiarEstadoAsync(id, nuevoEstado, User.Identity?.Name, null);

                return Json(new { success = true, message = "Devoluci?n actualizada correctamente.", ruta = rutaWeb });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


        [HttpPost]
        public IActionResult GuardarDespachoMasivo([FromBody] List<RequisionDetalle> items)
        {
            return BadRequest(new { Exitoso = false, Mensaje = "Use el endpoint /Requisicion/GuardarDespachoMasivo para despachos de requisición." });
        }
    }
}