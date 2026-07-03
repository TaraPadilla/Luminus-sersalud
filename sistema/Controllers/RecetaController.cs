using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using sistema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class RecetaController : Controller
    {
        private readonly IRecetas _recetasRepository;
        private readonly IUser _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IHospitalizacion _hopitalizacion;
        private readonly ICuentasPorCobrar _cuentaPorCobrarRepository;

        public RecetaController(
            IRecetas recetasRepository,
            IUser userRepository,
            UserManager<User> userManager,
            IHospitalizacion hopitalizacion,
            ICuentasPorCobrar cuentasPorCobrarRepository)
        {
            _recetasRepository = recetasRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _hopitalizacion = hopitalizacion;
            _cuentaPorCobrarRepository = cuentasPorCobrarRepository;
        }

        // --------------------------
        // Helpers
        // --------------------------
        private void CargarCatalogos(RecetaViewModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var categorias = _recetasRepository.GetCategoriasRecetas()
                .Where(x => !x.Eliminado)
                .OrderBy(x => x.Nombre)
                .ToList();

            model.Categorias = categorias
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                })
                .ToList();

            var menus = _recetasRepository.GetRecetaMenus()
                .Where(x => !x.Eliminado)
                .OrderBy(x => x.Nombre)
                .ToList();

            model.Menus = menus
                .Select(m => new RecetaViewModel.MenuOptionVm
                {
                    Id = m.Id,
                    CategoriaId = m.CategoriaId,
                    Nombre = m.Nombre
                })
                .ToList();

            // Evitar NRE en vista/guardar
            model.MenuIds ??= new List<int>();
        }

        private void ValidarReceta(RecetaViewModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(model.NombreReceta))
                ModelState.AddModelError(nameof(model.NombreReceta), "El nombre es requerido.");

            if (!model.CategoriaId.HasValue || model.CategoriaId.Value <= 0)
                ModelState.AddModelError(nameof(model.CategoriaId), "La categoría es obligatoria.");

            if (model.PrecioCosto < 0)
                ModelState.AddModelError(nameof(model.PrecioCosto), "El precio costo no puede ser negativo.");

            if (model.PrecioVenta < 0)
                ModelState.AddModelError(nameof(model.PrecioVenta), "El precio venta no puede ser negativo.");

            // Validación de consistencia: los menús seleccionados deben pertenecer a la categoría seleccionada
            model.MenuIds ??= new List<int>();

            if (model.CategoriaId.HasValue && model.CategoriaId.Value > 0 && model.MenuIds.Count > 0)
            {
                var categoriaId = model.CategoriaId.Value;

                // Menús válidos para la categoría (y no eliminados)
                var menusValidos = _recetasRepository.GetRecetaMenus()
                    .Where(m => !m.Eliminado && m.CategoriaId == categoriaId)
                    .Select(m => m.Id)
                    .ToHashSet();

                // Detectar ids no válidos
                var invalidos = model.MenuIds.Where(id => !menusValidos.Contains(id)).Distinct().ToList();
                if (invalidos.Count > 0)
                {
                    ModelState.AddModelError(nameof(model.MenuIds),
                        "Hay menús seleccionados que no pertenecen a la categoría elegida.");
                }
            }
        }


        // --------------------------
        // Recetas (MVC estándar)
        // --------------------------
        public IActionResult Lista()
        {
            var recetas = _recetasRepository.GetList()
                .Where(x => x.Eliminado == false)
                .ToList();

            return View(recetas);
        }

        public IActionResult Nuevo()
        {
            var model = new RecetaViewModel();
            CargarCatalogos(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Nuevo(RecetaViewModel model)
        {
            model.MenuIds ??= new List<int>();

            ValidarReceta(model);

            if (!ModelState.IsValid)
            {
                CargarCatalogos(model);
                return View(model);
            }

            var receta = new Receta
            {
                CategoriaId = model.CategoriaId!.Value,

                NombreReceta = model.NombreReceta,
                Ingredientes = model.Ingredientes,
                PrecioCosto = model.PrecioCosto,
                PrecioVenta = model.PrecioVenta,
                FechaHoraCreada = DateTime.Now,
                Eliminado = false
            };

            _recetasRepository.Add(receta);

            // Relación real Receta -> Menús (many-to-many)
            _recetasRepository.ReplaceMenusForReceta(receta.Id, model.MenuIds);

            TempData["Message"] = "La receta se ha creado con éxito";
            return RedirectToAction(nameof(Lista));
        }

        public IActionResult Modificar(int recetaId)
        {
            var receta = _recetasRepository.Get(recetaId);
            if (receta == null || receta.Eliminado)
                return RedirectToAction(nameof(Lista));

            var model = new RecetaViewModel
            {
                Id = receta.Id,
                CategoriaId = receta.CategoriaId,
                NombreReceta = receta.NombreReceta,
                PrecioCosto = receta.PrecioCosto,
                PrecioVenta = receta.PrecioVenta,
                Ingredientes = receta.Ingredientes,
                MenuIds = _recetasRepository.GetMenuIdsByReceta(receta.Id)?.ToList() ?? new List<int>()
            };

            CargarCatalogos(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ModificarReceta(RecetaViewModel model)
        {
            if (model.Id <= 0)
                return RedirectToAction(nameof(Lista));

            model.MenuIds ??= new List<int>();

            ValidarReceta(model);

            if (!ModelState.IsValid)
            {
                CargarCatalogos(model);
                return View("Modificar", model);
            }

            var receta = _recetasRepository.Get(model.Id);
            if (receta == null || receta.Eliminado)
                return RedirectToAction(nameof(Lista));

            receta.CategoriaId = model.CategoriaId!.Value;
            receta.NombreReceta = model.NombreReceta;
            receta.Ingredientes = model.Ingredientes;
            receta.PrecioCosto = model.PrecioCosto;
            receta.PrecioVenta = model.PrecioVenta;

            _recetasRepository.Update(receta);

            // Actualiza relación Receta -> Menús
            _recetasRepository.ReplaceMenusForReceta(receta.Id, model.MenuIds);

            TempData["Message"] = "La receta se ha modificado con éxito";
            return RedirectToAction(nameof(Lista));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarReceta(int recetaId)
        {
            var receta = _recetasRepository.Get(recetaId);
            if (receta != null)
            {
                receta.Eliminado = true;
                _recetasRepository.Update(receta);
                TempData["Message"] = "La receta se ha eliminado con éxito";
            }

            return RedirectToAction(nameof(Lista));
        }

        // --------------------------
        // Hospitalización (JSON) - Se mantiene como estaba (misma ruta y campos actuales)
        // Se mejora: agrega CategoriaNombre y Menus (con Ingredientes) sin romper el consumo actual en KO.
        // --------------------------
        [HttpPost]
        public string ConsultarLista(int hospitalizacionId)
        {
            try
            {
                var recetasBd = _hopitalizacion.GetHospitalizacionRecetaByIdHospitalizacion(hospitalizacionId);

                // Catálogo de categorías: Id -> Nombre (una sola vez, evita repetir en foreach)
                var categoriasDict = _recetasRepository.GetCategoriasRecetas()
                    .Where(c => !c.Eliminado)
                    .ToDictionary(c => c.Id, c => c.Nombre);

                // Catálogo de menús: Id -> Entidad (una sola vez)
                var menusDict = _recetasRepository.GetRecetaMenus()
                    .Where(m => !m.Eliminado)
                    .ToDictionary(m => m.Id, m => m);

                // Resultado: se mantiene estructura JSON con Exitoso/Resultado,
                // y por cada item preserva los campos actuales + agrega CategoriaNombre y Menus.
                var resultado = new List<object>();

                if (recetasBd != null)
                {
                    foreach (var hr in recetasBd)
                    {
                        if (hr?.Receta == null) continue;

                        // Categoría (fallback conservador como en otras listas del proyecto)
                        var categoriaId = hr.Receta.CategoriaId;
                        var categoriaNombre = categoriasDict.TryGetValue(categoriaId, out var nombreCategoria)
                            ? nombreCategoria
                            : $"#{categoriaId}";

                        // Menús asociados a la receta (many-to-many)
                        // OJO: la relación es por Receta.Id (no por HospitalizacionReceta.Id)
                        var menuIds = _recetasRepository.GetMenuIdsByReceta(hr.Receta.Id)?.ToList() ?? new List<int>();

                        var menus = new List<object>();
                        foreach (var menuId in menuIds)
                        {
                            if (!menusDict.TryGetValue(menuId, out var menuEntity)) continue;

                            menus.Add(new
                            {
                                Id = menuEntity.Id,
                                Nombre = menuEntity.Nombre,
                                Ingredientes = menuEntity.Ingredientes
                            });
                        }

                        // IMPORTANTE: Campos actuales se mantienen con los mismos nombres.
                        resultado.Add(new
                        {
                            Id = hr.Id,
                            NombreReceta = hr.Receta.NombreReceta,
                            Indicaciones = hr.Inidicaciones,
                            Ingredientes = hr.Receta.Ingredientes,
                            PrecioVenta = hr.Receta.PrecioVenta,
                            Cantidad = hr.Cantidad,
                            CantidadAplicada = hr.CantidadAplicada,

                            // NUEVOS (no rompen KO actual; KO simplemente los ignorará hasta que la vista los use)
                            CategoriaNombre = categoriaNombre,
                            Menus = menus
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = resultado
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar las recetas. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string ConsultarRecetasAplicadas(int hospitalizacionId)
        {
            try
            {
                var listaRecetasAplicacion = new List<RecetaAplicacionViewModel>();
                var recetasAplicacion = _hopitalizacion.GetHospitalizacionRecetaDetalleByIdHospitalizacion(hospitalizacionId);

                if (recetasAplicacion != null)
                {
                    var grupos = recetasAplicacion
                        .Where(r => r?.HospitalizacionReceta?.Receta != null)
                        .GroupBy(r => r.HospitalizacionRecetaId);

                    foreach (var grupo in grupos)
                    {
                        var receta = grupo.First();
                        var parent = receta.HospitalizacionReceta;
                        var totalDetalles = grupo.Count();
                        var aplicados = grupo.Count(x => x.Aplicado);

                        if (parent.Eliminado && aplicados == 0)
                            continue;

                        var persona = "-";
                        var ultimoAplicado = grupo
                            .Where(x => x.Aplicado && x.UsuarioId != null)
                            .OrderByDescending(x => x.FechaHoraAplicada)
                            .FirstOrDefault();
                        if (ultimoAplicado?.UsuarioId != null)
                        {
                            var user = _userRepository.GetbyId(ultimoAplicado.UsuarioId);
                            persona = user?.NormalizedUserName ?? "-";
                            var display = _userRepository.GetDisplayName(ultimoAplicado.UsuarioId);
                            if (!string.IsNullOrWhiteSpace(display) && display != ultimoAplicado.UsuarioId)
                                persona = display;
                        }

                        var fechaAplicacion = ultimoAplicado?.FechaHoraAplicada?.ToString("dd/MM/yyyy HH:mm") ?? "-";

                        var personaCrea = "-";
                        if (parent.UsuarioCreacionId != null)
                        {
                            var uCrea = _userRepository.GetbyId(parent.UsuarioCreacionId);
                            personaCrea = uCrea?.NormalizedUserName ?? "-";
                            var displayCrea = _userRepository.GetDisplayName(parent.UsuarioCreacionId);
                            if (!string.IsNullOrWhiteSpace(displayCrea) && displayCrea != parent.UsuarioCreacionId)
                                personaCrea = displayCrea;
                        }

                        if (aplicados > 0 || !parent.Eliminado)
                        {
                            listaRecetasAplicacion.Add(new RecetaAplicacionViewModel
                            {
                                IdHospitalizacionReceta = parent.Id,
                                IdReceta = receta.Id,
                                NombreReceta = parent.Receta.NombreReceta,
                                Ingredientes = parent.Receta.Ingredientes,
                                Cantidad = totalDetalles.ToString(),
                                Indicaciones = parent.Inidicaciones,
                                Aplicado = aplicados >= totalDetalles,
                                FechaHoraAplicacion = fechaAplicacion,
                                PersonaAplica = persona,
                                PersonaCrea = personaCrea,
                            });
                        }
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaRecetasAplicacion
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar recetas para aplicacion. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string AgregarReceta(HospitalizacionRecetaViewModel model)
        {
            try
            {
                var receta = new HospitalizacionReceta
                {
                    RecetaId = model.Id,
                    Cantidad = model.Cantidad,
                    CantidadAplicada = 0,
                    Inidicaciones = model.Indicaciones,
                    HospitalizacionId = model.HospitalizacionId,
                    Eliminado = false,
                    UsuarioCreacionId = _userManager.GetUserId(HttpContext.User)
                };

                var cantidad = Convert.ToInt32(model.Cantidad);

                List<HospitalizacionRecetaDetalle> hospitalizacionRecetaDetalles = new List<HospitalizacionRecetaDetalle>();
                for (int i = 0; i < cantidad; i++)
                {
                    hospitalizacionRecetaDetalles.Add(new HospitalizacionRecetaDetalle
                    {
                        Aplicado = false,
                        Eliminado = false,
                        FechaHoraAplicada = null
                    });
                }

                receta.HospitalizacionRecetaDetalle = hospitalizacionRecetaDetalles;

                _hopitalizacion.AddHospitalizacionReceta(receta);

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al registrar la receta. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string EliminarHospitalizacionReceta(int? IdHospitalizacionReceta, int cuentaId)
        {
            try
            {
                if (IdHospitalizacionReceta == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "IdHospitalizacionReceta es requerido."
                    });
                }

                var receta = _hopitalizacion.GetHospitalizacionRecetaById(IdHospitalizacionReceta.Value);

                if (receta != null)
                {
                    receta.Eliminado = true;
                    _hopitalizacion.Update(receta);
                }

                #region Actualizacion de cuenta por cobrar
                if (receta?.Receta != null)
                {
                    var cuenta = _cuentaPorCobrarRepository.Get(cuentaId);
                    if (cuenta != null)
                    {
                        var valorPendiente = cuenta.Valor ?? 0;
                        valorPendiente -= (receta.Receta.PrecioVenta * receta.CantidadAplicada);
                        cuenta.Valor = valorPendiente;

                        _cuentaPorCobrarRepository.Update(cuenta);
                    }
                }
                #endregion

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al eliminar receta formulada. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string AplicarReceta(int IdHospitalizacionReceta, int cuentaId)
        {
            try
            {
                var receta = _hopitalizacion.GetHospitalizacionRecetaDetalleById(IdHospitalizacionReceta);
                if (receta == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se encontró el detalle de receta."
                    });
                }

                receta.Aplicado = true;
                receta.FechaHoraAplicada = DateTime.Now;
                receta.UsuarioId = _userManager.GetUserId(HttpContext.User);

                if (receta.HospitalizacionReceta != null)
                {
                    receta.HospitalizacionReceta.CantidadAplicada++;
                }

                _hopitalizacion.UpdateHospitalicacionRecetaDetalle(receta);

                #region Actualizacion de cuenta por cobrar
                var cuenta = _cuentaPorCobrarRepository.GetCuenta(cuentaId);
                if (cuenta != null && receta.HospitalizacionReceta?.Receta != null)
                {
                    var valorPendiente = cuenta.Valor ?? 0;
                    valorPendiente += receta.HospitalizacionReceta.Receta.PrecioVenta;
                    cuenta.Valor = valorPendiente;
                    _cuentaPorCobrarRepository.Update(cuenta);
                }
                #endregion

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al aplicar la receta. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarRecetasExistentes()
        {
            try
            {
                var listaRecetas = new List<RecetaViewModel>();
                var recetasBd = _recetasRepository.GetList()
                    .Where(x => x.Eliminado == false)
                    .ToList();

                if (recetasBd != null)
                {
                    foreach (var receta in recetasBd)
                    {
                        if (receta == null) continue;

                        var nombre = (receta.NombreReceta ?? "").Trim();
                        if (string.IsNullOrWhiteSpace(nombre)) continue;

                        listaRecetas.Add(new RecetaViewModel
                        {
                            Id = receta.Id,
                            NombreReceta = nombre,
                            Ingredientes = receta.Ingredientes
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaRecetas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar examenes existentes. " + ex.Message
                });
            }
        }
    }
}
