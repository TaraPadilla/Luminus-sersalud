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

namespace sistema.Controllers
{
    [Authorize]
    public class RequisicionController : Controller
    {
        private readonly IProducto _productoRepository;
        private readonly IBodega _bodegaRepository;
        private readonly IRequision _requisionRepository;
        private readonly IUser _usuarioRepository;
        private readonly IEmpleado _empleadoRepository;

        public RequisicionController(
            IProducto productoRepository,
            IBodega bodegaRepository,
            IRequision requisionRepository,
            IUser usuarioRepository,
            IEmpleado empleadoRepository)
        {
            _productoRepository = productoRepository;
            _bodegaRepository = bodegaRepository;
            _requisionRepository = requisionRepository;
            _usuarioRepository = usuarioRepository;
            _empleadoRepository = empleadoRepository;
        }

        [HttpGet]
        public IActionResult Nuevo()
        {
            var model = new RequisicionViewModel();

            // Sin roles por ahora: inicializa como usuario estándar
            model.Init(_bodegaRepository, rolFarmacia: false);

            model.EstadoRequisicion = RequisicionEstado.EnviadoAJefatura;

            return View(model);
        }

        // IMPORTANTE: solo debe existir UNA acción Lista().
        // Esta es la que se consume desde el menú (asp-action="Lista").
        [HttpGet]
        public IActionResult Lista()
        {
            // Por ahora (hasta conectar el repo de listado) devolvemos modelo vacío.
            // Esto permite montar vista + modal sin bloquear compilación ni navegación.
            var model = new RequisicionListaViewModel();
            return View("Lista", model);
        }

        [HttpPost]
        public string ConsultarProductosInventario(int bodegaOrigenId)
        {
            try
            {
                var listaProductos = new List<TrasladoProductoDisponibleViewModel>();

                var productosInventario = _productoRepository
                    .GetInventarioDisponiblePorBodega(bodegaOrigenId);

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
                    Mensaje = "Error al consultar productos de inventario. " + ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<string> GuardarRequisicion(RequisicionViewModel model)
        {
            try
            {

                var username = User.Identity.Name;
                var usuariodb = _usuarioRepository.Get(username);
                var empleado = _empleadoRepository.Get(usuariodb.EmpleadoId.Value);

                if (string.IsNullOrEmpty(empleado.FirmaEmpleado))
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Debe agregar la firma."
                    });
                }

                if (model == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Error: el modelo de requisición es nulo."
                    });
                }

                if (!model.BodegaOrigenId.HasValue || !model.BodegaDestinoId.HasValue)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Debe seleccionar Bodega Origen y Bodega Destino."
                    });
                }

                if (model.BodegaOrigenId.Value == model.BodegaDestinoId.Value)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Bodega Origen y Destino no pueden ser la misma."
                    });
                }

                if (model.Productos == null || !model.Productos.Any())
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Debe agregar al menos un producto."
                    });
                }

                // Conversión estricta decimal -> int (sin redondeos silenciosos)
                static bool TryDecimalToInt(decimal value, out int result)
                {
                    result = 0;

                    // no permitir fracciones
                    if (decimal.Truncate(value) != value)
                        return false;

                    // rango de int
                    if (value < int.MinValue || value > int.MaxValue)
                        return false;

                    result = (int)value;
                    return true;
                }

                var detalles = new List<RequisionDetalle>();

                foreach (var p in model.Productos)
                {
                    if (!p.ProductoInventarioId.HasValue || p.ProductoInventarioId.Value <= 0)
                        continue;

                    if (!TryDecimalToInt(p.CantidadTrasladada, out var cantidadSolicitada))
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = $"Cantidad solicitada inválida para el producto '{p.ProductoNombre}'. Debe ser un número entero."
                        });
                    }

                    if (cantidadSolicitada <= 0)
                        continue;

                    int? cantidadDespachada = null;
                    if (p.CantidadDespachada.HasValue)
                    {
                        if (!TryDecimalToInt(p.CantidadDespachada.Value, out var cd))
                        {
                            return JsonSerializer.Serialize(new
                            {
                                Exitoso = false,
                                Mensaje = $"Cantidad despachada inválida para el producto '{p.ProductoNombre}'. Debe ser un número entero."
                            });
                        }

                        cantidadDespachada = cd;
                    }

                    detalles.Add(new RequisionDetalle
                    {
                        ProductoInventarioId = p.ProductoInventarioId.Value,
                        CantidadSolicitada = cantidadSolicitada,
                        CantidadDespachada = cantidadDespachada
                    });
                }

                if (!detalles.Any())
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No hay productos con cantidad válida para guardar."
                    });
                }

                var entidad = new Requision
                {
                    Direccion = model.Direccion,
                    Departamento = model.Departamento,
                    UnidadSeccion = model.UnidadSeccion,
                    Otros = model.Otros,

                    NumeroRequisicion = model.NumeroRequisicion,
                    NumeroOrden = model.NumeroOrden,
                    FechaSolicitud = model.FechaSolicitud ?? DateTime.Now,

                    BodegaOrigenId = model.BodegaOrigenId.Value,
                    BodegaDestinoId = model.BodegaDestinoId.Value,

                    Observaciones = model.Observaciones,

                    SolicitanteNombre = model.NombreSolicitante,
                    JefaturaNombre = model.NombreJefaturaCoordinador,
                    GerenciaNombre = model.NombreGerenteAdministrativo,
                    EncargadoAlmacenNombre = model.NombreEncargadoAlmacen,
                    RecibeNombre = model.NombreReceptorFinal,



                    Estado = (int)model.EstadoRequisicion
                };

                var usuario = User?.Identity?.Name;

                var data = _usuarioRepository.Get(usuario);
                var empleadoDb = _empleadoRepository.Get(data.EmpleadoId.Value);
                entidad.FirmaSolicitante = empleadoDb.FirmaEmpleado;

                var requisionGuardada = await _requisionRepository.CrearAsync(entidad, detalles, usuario);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Requisición guardada correctamente.",
                    RequisicionId = requisionGuardada.Id,
                    NumeroRequisicion = requisionGuardada.NumeroRequisicion,
                    NumeroOrden = requisionGuardada.NumeroOrden,
                    RedirectUrl = Url.Action(nameof(Lista), "Requisicion")
                });
            }
            catch (DbUpdateException dbEx)
            {
                var baseEx = dbEx.GetBaseException();
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de BD al guardar requisición: " + baseEx.Message
                });
            }
            catch (Exception ex)
            {
                var baseEx = ex.GetBaseException();
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al guardar requisición: " + baseEx.Message
                });
            }
        }

        // Detalle para MODAL: devuelve PartialView
        [HttpGet]
        public async Task<IActionResult> DetalleModal(int id)
        {
            if (id <= 0)
                return BadRequest("Id inválido.");

            var requision = await _requisionRepository.GetByIdAsync(id);
            if (requision == null)
                return NotFound();

            var model = new RequisicionViewModel
            {
                RequisicionId = requision.Id,
                EstadoRequisicion = (RequisicionEstado)requision.Estado,

                Direccion = requision.Direccion,
                Departamento = requision.Departamento,
                UnidadSeccion = requision.UnidadSeccion,
                Otros = requision.Otros,

                NumeroRequisicion = requision.NumeroRequisicion,
                NumeroOrden = requision.NumeroOrden,
                FechaSolicitud = requision.FechaSolicitud,

                BodegaOrigenId = requision.BodegaOrigenId,
                BodegaDestinoId = requision.BodegaDestinoId,

                Observaciones = requision.Observaciones,

                NombreSolicitante = requision.SolicitanteNombre,
                NombreJefaturaCoordinador = requision.JefaturaNombre,
                NombreGerenteAdministrativo = requision.GerenciaNombre,
                NombreEncargadoAlmacen = requision.EncargadoAlmacenNombre,
                NombreReceptorFinal = requision.RecibeNombre
            };

            // Carga listas (bodegas) y otros datos auxiliares del VM (manteniendo tu lógica)
            model.Init(_bodegaRepository, rolFarmacia: false);

            // Mapear productos con información completa para el modal (nombre, unidad, stock, etc.)
            if (requision.RequisionDetalles != null && requision.RequisionDetalles.Any())
            {
                model.Productos = requision.RequisionDetalles.Select(d =>
                {
                    var inv = d.ProductoInventario;
                    var prod = inv?.Producto;

                    return new TrasladoProductoViewModel
                    {
                        ProductoInventarioId = d.ProductoInventarioId,
                        ProductoId = inv.ProductoId,

                        // Formato consistente con la tabla original: "Código / Producto"
                        ProductoCodigo = prod?.CodigoReferencia ?? "-",
                        ProductoNombre = prod != null
                            ? $"{prod.CodigoReferencia} / {prod.NombreProducto}"
                            : "-",

                        CantidadExistente = inv?.Stock ?? 0,

                        UnidadMedidaVentaId = inv?.UnidadMedidaVentaId ?? 0,
                        UnidadMedidaVentaNombre = inv?.UnidadMedidaVenta != null
                            ? inv.UnidadMedidaVenta.Nombre
                            : "-",

                        // En tu UI: CantidadTrasladada = solicitada
                        CantidadTrasladada = d.CantidadSolicitada,
                        CantidadDespachada = d.CantidadDespachada
                    };
                }).ToList();
            }

            return PartialView("_DetalleRequisicionModal", model);
        }

        [HttpGet]
        public async Task<IActionResult> Autorizacion(int id, string tipo)
        {
            if (id <= 0)
                return BadRequest("Id inválido.");

            var requision = await _requisionRepository.GetByIdAsync(id);
            if (requision == null)
                return NotFound();

            ViewBag.TipoFirma = tipo;

            var model = new RequisicionViewModel
            {
                RequisicionId = requision.Id,
                EstadoRequisicion = (RequisicionEstado)requision.Estado,

                Direccion = requision.Direccion,
                Departamento = requision.Departamento,
                UnidadSeccion = requision.UnidadSeccion,
                Otros = requision.Otros,

                NumeroRequisicion = requision.NumeroRequisicion,
                NumeroOrden = requision.NumeroOrden,
                FechaSolicitud = requision.FechaSolicitud,

                BodegaOrigenId = requision.BodegaOrigenId,
                BodegaDestinoId = requision.BodegaDestinoId,

                Observaciones = requision.Observaciones,

                NombreSolicitante = requision.SolicitanteNombre,
                NombreJefaturaCoordinador = requision.JefaturaNombre,
                NombreGerenteAdministrativo = requision.GerenciaNombre,
                NombreEncargadoAlmacen = requision.EncargadoAlmacenNombre,
                NombreReceptorFinal = requision.RecibeNombre
            };

            string username = User.Identity.Name;
            var usuario = _usuarioRepository.Get(username);
            var empleadoDb = _empleadoRepository.Get(usuario.EmpleadoId.Value);
            model.NombreJefaturaCoordinador = empleadoDb.NombreYApellidos;
            model.NombreGerenteAdministrativo = empleadoDb.NombreYApellidos;
            model.NombreEncargadoAlmacen = empleadoDb.NombreYApellidos;
            model.EntregadoNombre = username;
            return PartialView("_AutorizacionModal", model);
        }

        // Compatibilidad: si alguien pega /Detalle/{id} redirigimos a Lista
        [HttpGet]
        public IActionResult Detalle(int id)
        {
            return RedirectToAction(nameof(Lista));
        }

        [HttpPost]
        public async Task<IActionResult> ListaData([FromForm] DataTablesRequest request)
        {
            if (request == null)
            {
                return Json(new
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = Array.Empty<object>()
                });
            }

            // DataTables: columna ordenada (index) -> Columns[index].Name (lo configuramos en la vista)
            string? orderColumn = null;
            string? orderDir = null;

            if (request.Order != null && request.Order.Count > 0)
            {
                var ord = request.Order[0];
                orderDir = ord.Dir;

                if (request.Columns != null
                    && ord.Column >= 0
                    && ord.Column < request.Columns.Count)
                {
                    // Usamos Name (mapeo controlado en el repo)
                    orderColumn = request.Columns[ord.Column]?.Name;
                }
            }

            var search = request.Search?.Value;

            var result = await _requisionRepository.ListarDataTableAsync(
                start: request.Start,
                length: request.Length,
                search: search,
                orderColumn: orderColumn,
                orderDir: orderDir
            );

            // Respuesta con nombres EXACTOS que la vista DataTable espera (data: '...')
            var data = result.Items.Select(x => new
            {
                requisicionId = x.RequisicionId,
                numeroRequisicion = x.NumeroRequisicion?.ToString() ?? "-",
                numeroOrden = x.NumeroOrden?.ToString() ?? "-",
                fechaSolicitud = x.FechaSolicitud.HasValue ? DateTime.SpecifyKind(x.FechaSolicitud.Value, DateTimeKind.Local).ToString("dd/MM/yyyy HH:mm") : "-",
                solicitanteNombre = string.IsNullOrWhiteSpace(x.SolicitanteNombre) ? "-" : x.SolicitanteNombre,
                bodegaOrigenNombre = string.IsNullOrWhiteSpace(x.BodegaOrigenNombre) ? "-" : x.BodegaOrigenNombre,
                bodegaDestinoNombre = string.IsNullOrWhiteSpace(x.BodegaDestinoNombre) ? "-" : x.BodegaDestinoNombre,
                departamentoNombre = string.IsNullOrWhiteSpace(x.DepartamentoNombre) ? "-" : x.DepartamentoNombre,
                unidadNombre = string.IsNullOrWhiteSpace(x.UnidadNombre) ? "-" : x.UnidadNombre,
                estado = ((RequisicionEstado)x.Estado).ToString()
            }).ToList();

            return Json(new
            {
                draw = request.Draw,
                recordsTotal = result.RecordsTotal,
                recordsFiltered = result.RecordsFiltered,
                data
            });
        }

        [HttpGet]
        public string ConsultarUltimoRegistro()
        {
            try
            {
                var (ultimoNumeroRequisicion, ultimoNumeroOrden) = _requisionRepository.ObtenerUltimoRegistro();

                if (ultimoNumeroRequisicion == 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = true,
                        Mensaje = "No existen requisiciones registradas actualmente.",
                        Resultado = new
                        {
                            ProximoRequisicion = 1,
                            ProximoNumeroOrden = 1,
                        }
                    });
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Consulta realizada correctamente",
                    Resultado = new
                    {
                        ProximoRequisicion = (ultimoNumeroRequisicion ?? 0) + 1,
                        ProximoNumeroOrden = (ultimoNumeroOrden ?? 0) + 1
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al intentar obtener la última requisición: " + ex.Message
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
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Usuario no tiene empleado asociado." });
                }

                var empleado = _empleadoRepository.Get(usuario.EmpleadoId.Value);

                if (empleado == null)
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "No se encontró la data del empleado." });
                }

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
                {
                    return Json(new { success = false, message = "No se recibió la información de la firma." });
                }

                string username = User.Identity.Name;
                var usuario = _usuarioRepository.Get(username);
                var empleadoDb = _empleadoRepository.Get(usuario.EmpleadoId.Value);

                if (empleadoDb == null)
                {
                    return Json(new { success = false, message = "Empleado no encontrado." });
                }

                var carpetaFirmas = "Firmas";
                var pathRaiz = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", carpetaFirmas);
                if (!Directory.Exists(pathRaiz)) Directory.CreateDirectory(pathRaiz);

                if (!string.IsNullOrEmpty(empleadoDb.FirmaEmpleado))
                {
                    try
                    {
                        var nombreArchivoViejo = Path.GetFileName(empleadoDb.FirmaEmpleado);
                        var rutaViejaFull = Path.Combine(pathRaiz, nombreArchivoViejo);
                        if (System.IO.File.Exists(rutaViejaFull))
                        {
                            System.IO.File.Delete(rutaViejaFull);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                string nombreLimpio = (empleadoDb.Nombre ?? "Empleado").Replace(" ", "_");
                string sanitizedFileName = $"{nombreLimpio}_Firma_{Guid.NewGuid()}.png";

                var base64Data = firmaBase64.Contains(",") ? firmaBase64.Split(',')[1] : firmaBase64;
                byte[] imageBytes = Convert.FromBase64String(base64Data);

                var filePath = Path.Combine(pathRaiz, sanitizedFileName);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                empleadoDb.FirmaEmpleado = $"/{carpetaFirmas}/{sanitizedFileName}";
                _empleadoRepository.Update(empleadoDb);

                return Json(new
                {
                    success = true,
                    message = "Firma actualizada exitosamente.",
                    rutaFirma = empleadoDb.FirmaEmpleado
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarAutorizacionFirma(int id, string firmaBase64, string tipoFirma, string nombreGerencia, string nombreJefatura, string nombreAlmacen, string nombreEntregado)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(firmaBase64))
                    return Json(new { success = false, message = "No se recibió la firma." });

                // 1. OBTENER LA CLASE DESDE LA BD
                var requisicion = await _requisionRepository.GetByIdAsync(id);

                if (requisicion == null)
                    return Json(new { success = false, message = "Requisición no encontrada." });

                // 2. MANEJO DE ARCHIVO FÍSICO
                string tipoLimpio = string.Concat(tipoFirma.Where(char.IsLetterOrDigit)).ToLower();
                var carpeta = "FirmasAutorizaciones";
                var pathRaiz = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", carpeta);
                if (!Directory.Exists(pathRaiz)) Directory.CreateDirectory(pathRaiz);

                // Limpiar firmas anteriores del mismo tipo
                foreach (var f in Directory.GetFiles(pathRaiz, $"{tipoLimpio}_{id}_*")) { System.IO.File.Delete(f); }

                string nombreArchivo = $"{tipoLimpio}_{id}_{Guid.NewGuid().ToString().Substring(0, 8)}.png";
                string rutaWeb = $"/{carpeta}/{nombreArchivo}";

                var base64Data = firmaBase64.Contains(",") ? firmaBase64.Split(',')[1] : firmaBase64;
                await System.IO.File.WriteAllBytesAsync(Path.Combine(pathRaiz, nombreArchivo), Convert.FromBase64String(base64Data));

                var tipoFirmaLimpia = tipoFirma.ToLower().Trim();

                switch (tipoFirmaLimpia)
                {
                    case "vobo":
                        requisicion.VOBOJefatura = rutaWeb; // Propiedad de la clase
                        requisicion.Estado = 3;             // Cambiamos el estado en la clase
                        requisicion.JefaturaNombre = nombreJefatura;
                        break;

                    case "autorizacion":
                        requisicion.AutorizacionGerencia = rutaWeb; // Propiedad de la clase
                        requisicion.Estado = 4;                      // Cambiamos el estado en la clase
                        requisicion.GerenciaNombre = nombreGerencia;
                        break;

                    case "autorizacionbodega":
                        // CORRECCIÓN: NO setear Estado=6 aquí.
                        // La entrega a Kardex (estado 6 + inventario + movimientos + historial) la ejecuta ProcesarEntregaAKardexAsync.
                        requisicion.AutorizacionAlmacen = rutaWeb;
                        requisicion.EncargadoAlmacenNombre = nombreAlmacen;
                        requisicion.EntregadoNombre = nombreEntregado;
                        break;

                    default:
                        // Limpieza del archivo recién creado ante error de tipo
                        var fullBad = Path.Combine(pathRaiz, nombreArchivo);
                        if (System.IO.File.Exists(fullBad))
                            System.IO.File.Delete(fullBad);

                        return Json(new { success = false, message = "Tipo de firma no reconocido." });
                }

                // 3. Persistencia:
                // - Para VOBO y Autorización: se mantiene Update() como siempre
                // - Para Bodega: se ejecuta proceso transaccional (inventario + kardex + estado + historial)
                if (tipoFirmaLimpia == "autorizacionbodega")
                {
                    string username = User.Identity.Name;
                    var usuario = _usuarioRepository.Get(requisicion.SolicitanteNombre.Trim());

                    var usuario2 = _usuarioRepository.Get(requisicion.EntregadoNombre.Trim());


                    if (usuario == null)
                    {
                        var fullPath = Path.Combine(pathRaiz, nombreArchivo);
                        if (System.IO.File.Exists(fullPath))
                            System.IO.File.Delete(fullPath);

                        return Json(new { success = false, message = "Usuario no encontrado." });
                    }

                    var (exitoso, mensaje) = await _requisionRepository.ProcesarEntregaAKardexAsync(
                        requisionId: id,
                        usuarioId: usuario.Id,
                        rutaFirmaAlmacen: rutaWeb,
                        nombreAlmacen: nombreAlmacen,
                        usuarioId2: usuario2.Id
                    );

                    if (!exitoso)
                    {
                        // Si falla kardex/inventario, borramos la firma recién guardada
                        var fullPath = Path.Combine(pathRaiz, nombreArchivo);
                        if (System.IO.File.Exists(fullPath))
                            System.IO.File.Delete(fullPath);

                        return Json(new { success = false, message = mensaje });
                    }
                }
                else
                {
                    // Mantener funcionalidad original
                    _requisionRepository.Update(requisicion);
                }

                return Json(new
                {
                    success = true,
                    message = "Requisición actualizada correctamente.",
                    ruta = rutaWeb
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GuardarDespachoMasivo([FromBody] List<RequisionDetalle> items)
        {
            try
            {
                if (items == null || !items.Any())
                {
                    return BadRequest(new { Exitoso = false, Mensaje = "No se recibieron datos." });
                }

                if (items.Any(i => i.CantidadDespachada < 0))
                {
                    return BadRequest(new { Exitoso = false, Mensaje = "Las cantidades no pueden ser negativas." });
                }

                bool resultado = await _requisionRepository.ActualizarCantidadesDespachoPorProductoAsync(items);

                if (resultado)
                {
                    return Ok(new { Exitoso = true, Mensaje = "Despacho guardado exitosamente." });
                }

                return StatusCode(500, new { Exitoso = false, Mensaje = "No se pudieron aplicar los cambios en el inventario." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = "Error: " + ex.GetBaseException().Message });
            }
        }
    }
}