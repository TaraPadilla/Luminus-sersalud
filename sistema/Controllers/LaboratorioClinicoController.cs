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
using sistema.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Wkhtmltopdf.NetCore;
using Database.Shared.Paginacion;
using System.Text.Json;
using Database.Shared.Enumeraciones;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using static sistema.Models.ExamenLabClinicoViewModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using farmamest.Models;
using DocumentFormat.OpenXml.Office2013.Excel;
using sistema.Service.IService;
using sistema.Service;
using farmamest.Service.IService;
//using Newtonsoft.Json;

namespace sistema.Controllers
{
    [Authorize]
    public class LaboratorioClinicoController : Controller
    {
        private readonly ILaboratorioClinico _laboratorioClinico = null;
        private readonly IEmpleado _empleadosDoctoresRepository = null;
        private readonly IPacientes _pacientesRepository = null;
        private readonly UserManager<User> _userManager = null;
        private readonly IEnvio _envioRepository = null;
        private readonly IPrecios _preciosrepository = null;
        private readonly ISucursal _sucursalRepository = null;
        private readonly IPacientes _pacienteRepository = null;
        private readonly IEmpleado _empleadoRepository = null;
        private readonly IUser _usuarioRepository = null;
        private readonly IProducto _productosRepository = null;
        private readonly IEstadoExamen _estadoExamenRepository = null;
        private readonly ICategoriaGeneralLabClinicoService _categoriaGeneralLabClinicoService = null;

        //Servicios (logica)
        private readonly IProductosService _productosService = null;

        public LaboratorioClinicoController(ILaboratorioClinico laboratorioClinico,
        UserManager<User> userManager,
        IPacientes pacientesRepository,
        ISucursal sucursalRepository,
        IEmpleado empleadoDoctoresRepository,
        IEnvio envioRepository,
        IPrecios preciosRepository,
        IEmpleado empleadoRepository,
        IPacientes pacienteRepository,
        IUser usuarioRepository,
        IProducto productosRepository,
        ICategoriaGeneralLabClinicoService categoriaGeneralLabClinicoService,
        IEstadoExamen estadoExamenRepository,
        //Servicios (logica)
        IProductosService productosService
        )
        {
            _laboratorioClinico = laboratorioClinico;
            _userManager = userManager;
            _pacientesRepository = pacientesRepository;
            _empleadosDoctoresRepository = empleadoDoctoresRepository;
            _envioRepository = envioRepository;
            _preciosrepository = preciosRepository;
            _sucursalRepository = sucursalRepository;
            _empleadoRepository = empleadoRepository;
            _pacienteRepository = pacienteRepository;
            _usuarioRepository = usuarioRepository;
            _productosRepository = productosRepository;
            _estadoExamenRepository = estadoExamenRepository;

            //Servicios (logica)
            _categoriaGeneralLabClinicoService = categoriaGeneralLabClinicoService;
            _productosService = productosService;
        }

        public IActionResult ListaCategorias(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _laboratorioClinico.PaginacionCategoriasLab(sortOrder, buscar, pageNumber, 30);

            return View(lista);
        }



        //public IActionResult VentasLabLista(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        //{
        //    if (buscar != null)
        //    {
        //        pageNumber = 1;
        //    }
        //    else
        //    {
        //        buscar = currentFilter;
        //    }

        //    ViewData["CurrentFilter"] = buscar;

        //    var lista = _laboratorioClinico.PaginacionVentasLab(sortOrder, buscar, pageNumber, 30);

        //    return View(lista);
        //}

        public IActionResult ListaExamenes(ExamenesLabClinicoViewModel viewModel)
        {
            if (viewModel.buscar != null)
            {
                viewModel.pageNumber = 1;
            }
            else
            {
                viewModel.buscar = viewModel.currentFilter;
            }

            ViewData["CurrentFilter"] = viewModel.buscar;
            viewModel.Init(_laboratorioClinico);

            viewModel.nombreExamenes = _laboratorioClinico.PaginacionExamenClinicoLab(null, viewModel.buscar, viewModel.pageNumber, 30, viewModel.catexamenId);

            return View(viewModel);
        }

        public IActionResult NuevaCategoria()
        {
            return View(new ModCategoriaListados
            {
                CategoriaLabClinico = new CategoriaLabClinico()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NuevaCategoria(ModCategoriaListados model)
        {
            model ??= new ModCategoriaListados();
            model.CategoriaLabClinico ??= new CategoriaLabClinico();

            if (string.IsNullOrWhiteSpace(model.CategoriaLabClinico.Nombre))
            {
                ModelState.AddModelError("CategoriaLabClinico.Nombre", "Este campo es obligatorio.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Complete el nombre de la categoría.";
                TempData["MessageType"] = "error";
                return View(model);
            }

            try
            {
                model.CategoriaLabClinico.FechaCreacion = DateTime.Now;
                model.CategoriaLabClinico.Activo = true;
                model.CategoriaLabClinico.Eliminado = false;
                _laboratorioClinico.Add(model.CategoriaLabClinico);
                TempData["Message"] = "¡El registro se ha guardado con éxito.!";
                return RedirectToAction("ListaCategorias");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error al guardar la categoría: " + ex.Message;
                TempData["MessageType"] = "error";
                return View(model);
            }
        }

        public IActionResult ModificarCategoriaLab(int? id)
        {
            var categoriaLab = _laboratorioClinico.GetCategoriaLab((int)id);
            var nombreExamenes = _laboratorioClinico.ExamenesLabList((int)id);

            var model = new ModCategoriaListados()
            {
                CategoriaLabClinico = categoriaLab,
                NombresExamenes = nombreExamenes

            };

            return View(model);
        }

        public IActionResult DesactivarOActivarCategoriaLab(int? id)
        {
            var categoriaLab = _laboratorioClinico.GetCategoriaLab((int)id);

            if (categoriaLab.Activo == true)
            {
                categoriaLab.Activo = false;
            }
            else
            {
                categoriaLab.Activo = true;
            }

            _laboratorioClinico.Update(categoriaLab);

            return RedirectToAction("ListaCategorias");
        }

        public IActionResult ActivarODesactivarDatosLab(int? id, int? cat2)
        {
            var datos = _laboratorioClinico.GetDatosExamenLab((int)id);

            if (datos.Activo == true)
            {
                datos.Activo = false;
            }
            else
            {
                datos.Activo = true;
            }

            _laboratorioClinico.Update(datos);

            return RedirectToAction("ModificarExamenLab", new { id = cat2 });


        }




        public IActionResult ActivarODesactivarNombreExamenLab(int? id, int cat2)
        {
            var categoriaLab = _laboratorioClinico.GetExamenLab((int)id);
            if (categoriaLab == null)
                return RedirectToAction("ModificarCategoriaLab", new { id = cat2 });

            if (categoriaLab.Activo == true)
            {
                categoriaLab.Activo = false;
            }
            else
            {
                categoriaLab.Activo = true;
            }

            _laboratorioClinico.Update(categoriaLab);

            return RedirectToAction("ModificarCategoriaLab", new { id = cat2 });

            // return RedirectToAction("ListaCategorias");
        }



        public IActionResult EliminarCategoria(int? id)
        {
            var categoriaLab = _laboratorioClinico.GetCategoriaLab((int)id);
            if (categoriaLab == null)
                return RedirectToAction("ListaCategorias");
            categoriaLab.Eliminado = true;

            _laboratorioClinico.Update(categoriaLab);

            TempData["Message"] = "¡El registro se ha archivado con éxito.!";

            return RedirectToAction("ListaCategorias");
        }

        public IActionResult EliminarDatosLab(int? id, int? cat2)
        {
            var categoriaLab = _laboratorioClinico.GetDatosExamenLab((int)id);
            if (categoriaLab == null)
                return RedirectToAction("ModificarExamenLab", new { id = cat2 });
            categoriaLab.Eliminado = true;

            _laboratorioClinico.Update(categoriaLab);

            TempData["Message"] = "¡El registro se ha archivado con éxito.!";

            return RedirectToAction("ModificarExamenLab", new { id = cat2 });
        }

        public IActionResult EliminarExamen(int? id)
        {
            var examen = _laboratorioClinico.GetExamenLab((int)id);
            if (examen == null)
                return RedirectToAction("ListaExamenes");
            examen.Eliminado = true;

            _laboratorioClinico.Update(examen);

            TempData["Message"] = "¡El registro se ha archivado con éxito.!";

            return RedirectToAction("ListaExamenes");
        }

        public IActionResult EliminarExamen2(int? id, int cat2)
        {
            var examen = _laboratorioClinico.GetExamenLab((int)id);
            if (examen == null)
                return RedirectToAction("ModificarCategoriaLab", new { id = cat2 });
            examen.Eliminado = true;

            _laboratorioClinico.Update(examen);

            TempData["Message"] = "¡El registro se ha archivado con éxito.!";

            // return RedirectToAction("ListaCategorias");
            return RedirectToAction("ModificarCategoriaLab", new { id = cat2 });
        }

        public IActionResult EliminarExamenRealizado(int? id)
        {
            var examen = _laboratorioClinico.GetExamenRealizado((int)id);
            if (examen == null)
                return RedirectToAction("ListaExamenesRealizados");
            examen.Eliminado = true;

            _laboratorioClinico.Update(examen);

            TempData["Message"] = "¡El registro se ha archivado con éxito.!";

            return RedirectToAction("ListaExamenesRealizados");
        }

        //public IActionResult EliminarVentaLab(int? id)
        //{
        //    var venta = _laboratorioClinico.GetVentaLab((int)id);
        //    var examen = _laboratorioClinico.GetExamenRealizado((int)venta.ExamenId);

        //    examen.Eliminado = true;
        //    venta.Eliminado = true;

        //    _laboratorioClinico.Update(examen, false);
        //    _laboratorioClinico.Update(venta, false);

        //    _laboratorioClinico.saveChanges();


        //    TempData["Message"] = "¡El registro se ha archivado con éxito.!";

        //    return RedirectToAction("VentasLabLista");
        //}

        public IActionResult ModificarExamenLab(int? id)
        {
            var lab = _laboratorioClinico.GetExamenLab((int)id);
            var datosLista = _laboratorioClinico.DatosLabList((int)id);

            var model = new ExamenLabClinicoViewModel()
            {
                ExamenLabClinico = lab,
                DatosExamenes = datosLista,

                IdExamen = lab.Id,
                NombreExamen = lab.NombreExamen,
                CodigoInterno = lab.CodigoInterno,
                Indicaciones = lab.Indicaciones,
                TipoExamen = lab.TipoDeExamen,
                CategoriaLabClinicoId = lab.CategoriaLabClinicoId,
                Advertencias = lab.Advertencias,
                Instrucciones = lab.Instrucciones,
                DeclaracionConsentimiento = lab.DeclaracionConsentimiento
            };

            model.Init(_laboratorioClinico);
            return View(model);
        }

        public IActionResult ModificarDatosLab(int? id)
        {
            var dato = _laboratorioClinico.GetDatosExamenLab((int)id);
            var model = new DatosExamenesLabClinicoViewModel()
            {
                Id = dato.Id,
                ExamenLabClinicoId = dato.ExamenLabClinicoId,
                Campos = dato.Campos,
                Tipo = dato.Tipo,
                Resultado = dato.Resultado,
                ValorReferencia = dato.ValorReferencia,
                Indicaciones = dato.Indicaciones
            };
            model.Init(_laboratorioClinico);
            ViewBag.MostrarEstado = true;
            return View(model);
        }


        public IActionResult EditarDetalleExamen(int? id, bool radiologia = false)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var examenBase = _laboratorioClinico.GetExamenRealizado((int)id);
            if (examenBase == null)
            {
                return NotFound();
            }

            var examenExtra = _laboratorioClinico.GetInfoRequeridaEditarDetalleExamen((int)id);

            var hospitalizacionRel = examenExtra?.HospitalizacionesExamenes?.FirstOrDefault();
            var hospitalizacionId = hospitalizacionRel?.HospitalizacionId;

            var citaReciente = examenExtra?.Paciente?.Citas?.OrderByDescending(c => c.Id).FirstOrDefault();

            if (examenBase.DetalleExamenes != null)
            {
                examenBase.DetalleExamenes = examenBase.DetalleExamenes
                    .Where(d =>
                        d.ExamenLabClinico?.CategoriaLabClinico?.Id != null &&
                        (radiologia
                            ? d.ExamenLabClinico.CategoriaLabClinico.Id == 38
                            : d.ExamenLabClinico.CategoriaLabClinico.Id != 38))
                    .ToList();
            }


            var model = new RealizarExamenLabClinicoViewModel()
            {
                Examen = examenBase,
                ExamenId = examenBase.Id,
                HospitalizacionId = hospitalizacionId,
                PacienteNombre = examenBase.Paciente?.Nombre,
                PacienteDPI = examenBase.Paciente?.Dpi,
                PacienteFechaNacimiento = examenBase.Paciente?.FechaNacimiento,
                PacienteEdad = examenBase.Paciente?.Edad,
                NombreHabitacion = hospitalizacionRel?.Hospitalizacion?.Habitacion?.NombreNumeroHabitacion,
                NombreSeguro = citaReciente?.CodigoDeCita,
                NombreMedico = citaReciente?.Empleado?.Nombre ?? examenBase.Empleado?.Nombre
            };

            model.Init(_laboratorioClinico, _empleadosDoctoresRepository, _pacientesRepository, _envioRepository);
            ViewBag.MostrarEstado = true;

            return View(model);
        }


        [HttpPost]
        public IActionResult EditarDetalleExamen(RealizarExamenLabClinicoViewModel viewModel)
        {
            var examen1 = _laboratorioClinico.GetExamenRealizado((int)viewModel.Examen.Id);
            examen1.FechaActualizacion = DateTime.Now;
            examen1.EstadoExamenId = viewModel.Examen.EstadoExamenId;
            // examen1.UsuarioIngresa = _userManager.GetUserId(HttpContext.User);
            _laboratorioClinico.Update(examen1);

            var model1 = new RealizarExamenLabClinicoViewModel()
            {
                Examen = examen1,

            };

            model1.Init(_laboratorioClinico, _empleadosDoctoresRepository, _pacientesRepository, _envioRepository);
            ViewBag.MostrarEstado = true;

            TempData["Message"] = "¡El registro se ha modificado con éxito!";

            return View(model1);
        }

        [HttpPost]
        public async Task<IActionResult> ModificarCategoriaLab(ModCategoriaListados model)
        {
            if (model?.CategoriaLabClinico == null || model.CategoriaLabClinico.Id <= 0)
            {
                TempData["Message"] = "No se pudo identificar la categoría a modificar.";
                return RedirectToAction("ListaCategorias");
            }

            if (!ModelState.IsValid)
            {
                model.NombresExamenes = _laboratorioClinico.ExamenesLabList(model.CategoriaLabClinico.Id);
                return View(model);
            }

            var existing = _laboratorioClinico.GetCategoriaLab(model.CategoriaLabClinico.Id);
            if (existing == null)
            {
                TempData["Message"] = "La categoría no existe o fue eliminada.";
                return RedirectToAction("ListaCategorias");
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            existing.Nombre = model.CategoriaLabClinico.Nombre;
            existing.UltimoUsuarioModificado = user?.UserName ?? user?.Email ?? "-";

            try
            {
                _laboratorioClinico.Update(existing);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Message"] = ex.Message;
                model.NombresExamenes = _laboratorioClinico.ExamenesLabList(model.CategoriaLabClinico.Id);
                return View(model);
            }

            model.CategoriaLabClinico = existing;
            model.NombresExamenes = _laboratorioClinico.ExamenesLabList(existing.Id);
            TempData["Message"] = "¡El registro se ha modificado con éxito!";
            return View(model);
        }



        [HttpPost]
        public async Task<string> ModificarExamenLab(ExamenLabClinicoViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var examen = _laboratorioClinico.GetExamenLab((int)model.IdExamen);
                examen.CodigoInterno = model.CodigoInterno;
                examen.NombreExamen = model.NombreExamen;
                examen.Indicaciones = model.Indicaciones;
                examen.TipoDeExamen = model.TipoExamen;
                examen.CategoriaLabClinicoId = model.CategoriaLabClinicoId;
                examen.UltimaModificacion = user.UserName;
                examen.Instrucciones = model.Instrucciones;
                examen.Advertencias = model.Advertencias;
                examen.DeclaracionConsentimiento = model.DeclaracionConsentimiento;

                //Precios de examen
                if (model.Precios != null)
                {
                    examen.ExamenLabClinicosPrecios.Clear();
                    foreach (var precio in model.Precios)
                    {
                        examen.ExamenLabClinicosPrecios.Add(new ExamenLabClinicoPrecio
                        {
                            Id = precio.Id ?? 0,
                            ExamenLabClinicoId = (int)model.IdExamen,
                            PrecioId = precio.PrecioId,
                            PrecioValor = precio.PrecioValor
                        });
                    }
                }
                //Gauardar las preguntas generadas
                //var ListaPreguntas = new List<ExamenLabClinicoPregunta>();
                examen.ExamenLabClinicosPreguntas.Clear();
                if (model.Preguntas != null)
                {
                    //examen.ExamenLabClinicosPreguntas.Clear();
                    foreach (var pregunta in model.Preguntas)
                    {
                        examen.ExamenLabClinicosPreguntas.Add(new ExamenLabClinicoPregunta
                        {
                            Id = pregunta.Id ?? 0,
                            ExamenLabClinicoId = examen.Id,
                            Pregunta = pregunta.Pregunta,
                            Detalles = "",
                            Respuesta = false,
                            Eliminado = false
                        });

                    }
                }


                if (model.InsumosUtilizados != null && model.InsumosUtilizados.Count > 0)
                {
                    examen.ExamenLabClinicoInsumo.Clear();
                    foreach (var insumo in model.InsumosUtilizados)
                    {
                        var ExamenLabClinicoInsumo = new ExamenLabClinicoInsumo
                        {
                            ProductoId = insumo.ProductoId,
                            UnidadMedidaVentaId = insumo.UnidadMedidaVentaId,
                            CantidadUtilizada = insumo.CantidadUtilizada,
                            ExamenLabClinicoId = examen.Id,
                            PrecioCosto = insumo.PrecioCostoInsumo,
                            Total = insumo.TotalInsumo,
                            Eliminado = false
                        };
                        if (examen.ExamenLabClinicoInsumo == null)
                        {
                            examen.ExamenLabClinicoInsumo = new List<ExamenLabClinicoInsumo>();
                        }
                        examen.ExamenLabClinicoInsumo.Add(ExamenLabClinicoInsumo);
                    }
                }
                _laboratorioClinico.Update(examen);

                TempData["Message"] = "¡El registro se ha modificado con éxito!";

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
                    Mensaje = "Error de servidor al modificar examen. " + ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult ModificarDatosLab(DatosExamenesLabClinico model)
        {
            if (model == null || model.Id <= 0)
            {
                TempData["Message"] = "No se pudo identificar el dato a modificar.";
                return RedirectToAction("ListaCategorias");
            }

            if (!ModelState.IsValid)
            {
                var invalidModel = new DatosExamenesLabClinicoViewModel
                {
                    Id = model.Id,
                    ExamenLabClinicoId = model.ExamenLabClinicoId,
                    Campos = model.Campos,
                    Tipo = model.Tipo,
                    Resultado = model.Resultado,
                    ValorReferencia = model.ValorReferencia,
                    Indicaciones = model.Indicaciones
                };
                invalidModel.Init(_laboratorioClinico);
                ViewBag.MostrarEstado = true;
                return View(invalidModel);
            }

            var existing = _laboratorioClinico.GetDatosExamenLab(model.Id);
            if (existing == null)
            {
                TempData["Message"] = "El dato no existe o fue eliminado.";
                return RedirectToAction("ModificarExamenLab", new { id = model.ExamenLabClinicoId });
            }

            existing.Campos = model.Campos;
            existing.Tipo = model.Tipo;
            existing.Resultado = model.Resultado;
            existing.ValorReferencia = model.ValorReferencia;
            existing.Indicaciones = model.Indicaciones;

            try
            {
                _laboratorioClinico.Update(existing);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("ModificarDatosLab", new { id = model.Id });
            }

            var modelo = new DatosExamenesLabClinicoViewModel()
            {
                Id = existing.Id,
                ExamenLabClinicoId = existing.ExamenLabClinicoId,
                Campos = existing.Campos,
                Tipo = existing.Tipo,
                Resultado = existing.Resultado,
                ValorReferencia = existing.ValorReferencia,
                Indicaciones = existing.Indicaciones
            };

            TempData["Message"] = "¡El registro se ha modificado con éxito!";
            modelo.Init(_laboratorioClinico);
            ViewBag.MostrarEstado = true;
            return View(modelo);
        }





        public IActionResult CrearExamen()
        {
            var model = new ExamenLabClinicoViewModel();
            model.Init(_laboratorioClinico);

            return View(model);
        }

        [HttpPost]
        public string CrearExamen(ExamenLabClinicoViewModel model)
        {
            try
            {
                var examen = new ExamenLabClinico
                {
                    NombreExamen = model.NombreExamen,
                    TipoDeExamen = model.TipoExamen,
                    CategoriaLabClinicoId = model.CategoriaLabClinicoId,
                    Indicaciones = model.Indicaciones,
                    Eliminado = false,
                    Activo = true,
                    CodigoInterno = model.CodigoInterno,
                    FechaCreacion = DateTime.Now,
                    Precio = 0,
                    PrecioB = 0,
                    PrecioC = 0,
                    PrecioCosto = 0,
                    Instrucciones = model.Instrucciones,
                    Advertencias = model.Advertencias,
                    DuracionHoras = model.DuracionHoras,
                    DuracionMinutos = model.DuracionMinutos,
                    DeclaracionConsentimiento = model.DeclaracionConsentimiento
                };

                //Insumos utilizados
                if (model.InsumosUtilizados != null && model.InsumosUtilizados.Count > 0)
                {

                    foreach (var insumo in model.InsumosUtilizados)
                    {
                        var ExamenLabClinicoInsumo = new ExamenLabClinicoInsumo
                        {
                            ProductoId = insumo.ProductoId,
                            UnidadMedidaVentaId = insumo.UnidadMedidaVentaId,
                            CantidadUtilizada = insumo.CantidadUtilizada,
                            ExamenLabClinicoId = examen.Id,
                            PrecioCosto = insumo.PrecioCostoInsumo,
                            Total = insumo.TotalInsumo,
                            Eliminado = false

                        };
                        if (examen.ExamenLabClinicoInsumo == null)
                        {
                            examen.ExamenLabClinicoInsumo = new List<ExamenLabClinicoInsumo>();
                        }
                        examen.ExamenLabClinicoInsumo.Add(ExamenLabClinicoInsumo);
                    }
                }

                //PRecios de examen
                if (model.Precios != null)
                {
                    foreach (var precio in model.Precios)
                    {
                        examen.ExamenLabClinicosPrecios.Add(new ExamenLabClinicoPrecio
                        {
                            PrecioId = precio.PrecioId,
                            PrecioValor = precio.PrecioValor
                        });
                    }
                }

                //Guardar las preguntas creardas
                // var ListaPreguntas = new List<ExamenLabClinicoPregunta>();
                if (model.Preguntas != null)
                {
                    foreach (var pregunta in model.Preguntas)
                    {
                        examen.ExamenLabClinicosPreguntas.Add(new ExamenLabClinicoPregunta
                        {
                            ExamenLabClinicoId = examen.Id,
                            Pregunta = pregunta.Pregunta,
                            Detalles = "",
                            Respuesta = false,
                            Eliminado = false
                        });

                    }


                }
                _laboratorioClinico.Add(examen);
                TempData["Message"] = "El examen ha sido registrado";
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
                    Mensaje = "Error de servidor al crear examen. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarPreciosExistentes()
        {
            try
            {
                var preciosExamen = new List<ExamenLabClinicoPrecioViewModel>();
                var preciosBd = _preciosrepository.GetList();
                if (preciosBd != null)
                {
                    foreach (var precio in preciosBd)
                    {
                        preciosExamen.Add(new ExamenLabClinicoPrecioViewModel
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
                    Resultado = preciosExamen
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
        public string ConsultarPreciosExamen(int examenLabClinicoId)
        {
            try
            {
                var preciosExamen = new List<ExamenLabClinicoPrecioViewModel>();
                var preciosBd = _preciosrepository.GetPreciosExamenLabClinico(examenLabClinicoId);
                if (preciosBd != null)
                {
                    foreach (var precio in preciosBd)
                    {
                        preciosExamen.Add(new ExamenLabClinicoPrecioViewModel
                        {
                            Id = precio.Id,
                            Activar = true,
                            PrecioId = precio.PrecioId,
                            PrecioNombre = precio.Precio.NombrePrecio,
                            PrecioValor = precio.PrecioValor
                        });
                    }
                }

                var preciosExistentesBd = _preciosrepository.GetList().ToList();
                if (preciosExistentesBd != null)
                {
                    foreach (var precio in preciosExistentesBd)
                    {
                        var existe = preciosExamen.Where(a => a.PrecioId == precio.Id).FirstOrDefault()
                            != null;
                        if (!existe)
                        {
                            preciosExamen.Add(new ExamenLabClinicoPrecioViewModel
                            {
                                PrecioId = precio.Id,
                                Activar = true,
                                PrecioNombre = precio.NombrePrecio,
                                PrecioValor = 0
                            });
                        }
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = preciosExamen
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

        //Consultar preguntas de examenes que pertenecen a alguna cita
        [HttpPost]
        public string ConsultarPreguntasExamenCita(int citaId)
        {
            try
            {
                var preguntasExamen = new List<ExamenLabClinicoPreguntasViewModel>();
                var preguntaBd = _laboratorioClinico.GetPreguntasExamenLabClinicoCita(citaId);
                if (preguntaBd != null)
                {
                    foreach (var pregunta in preguntaBd)
                    {
                        preguntasExamen.Add(new ExamenLabClinicoPreguntasViewModel
                        {
                            Id = pregunta.Id,
                            Pregunta = pregunta.Pregunta,
                            Respuesta = pregunta.Respuesta,
                            Detalles = pregunta.Detalles
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = preguntasExamen
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

        //Consultar preguntas de examenes que pertenecen a laguna cita
        [HttpPost]
        public string ConsultarPreguntasExamen(int examenLabClinicoId)
        {
            try
            {
                var preguntasExamen = new List<ExamenLabClinicoPreguntasViewModel>();
                var preguntaBd = _laboratorioClinico.GetPreguntasExamenLabClinico(examenLabClinicoId);
                if (preguntaBd != null)
                {
                    foreach (var pregunta in preguntaBd)
                    {
                        preguntasExamen.Add(new ExamenLabClinicoPreguntasViewModel
                        {
                            Id = pregunta.Id,
                            Pregunta = pregunta.Pregunta,
                            Respuesta = pregunta.Respuesta,
                            Eliminada = pregunta.Eliminado
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = preguntasExamen
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
        //Metodo para modificar la pregunta de un examnen
        [HttpPost]
        public async Task<string> ModificarPreguntasExamen(ExamenLabClinicoPreguntasViewModel data)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var pregunta = _laboratorioClinico.GetPregunta((int)data.Id);
                pregunta.Pregunta = data.Pregunta;
                pregunta.Detalles = data.Detalles;
                pregunta.Respuesta = (bool)data.Respuesta;


                _laboratorioClinico.UpdatePregunta(pregunta);



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
                    Mensaje = "Error de servidor al modificar examen. " + ex.Message
                });
            }
        }

        //En revision para probablemente ser eliminado
        //Metodo para cambiar el estado eliminado de una pregunta
        //[HttpPost]
        //public async Task<string> EliminarPreguntaExamen(ExamenLabClinicoPreguntasViewModel data)
        //{
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);

        //        var pregunta = _laboratorioClinico.GetPregunta((int)data.Id);
        //        pregunta.Pregunta = data.Pregunta;
        //        pregunta.Detalles = data.Detalles;
        //        pregunta.Respuesta = (bool)data.Respuesta;
        //        pregunta.Eliminado = true;


        //        _laboratorioClinico.UpdatePregunta(pregunta);



        //        return JsonSerializer.Serialize(new
        //        {
        //            Exitoso = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonSerializer.Serialize(new
        //        {
        //            Exitoso = false,
        //            Mensaje = "Error de servidor al eliminar el examen. " + ex.Message
        //        });
        //    }
        //}

        [HttpPost]
        public IActionResult CrearExamen2(ExamenLabClinico model, int catid)
        {
            if (ModelState.IsValid)
            {
                model.CategoriaLabClinicoId = catid;
                _laboratorioClinico.Add(model);

                TempData["Message"] = "¡El registro se ha insertado con éxito.!";
                // return RedirectToAction("ListaCategorias");
                return RedirectToAction("ModificarCategoriaLab", new { id = catid });
            }

            // var viewModel = new ExamenLabClinicoViewModel();
            // model.Init(_laboratorioClinico);

            return RedirectToAction("ModificarCategoriaLab", new { id = catid });
        }

        [HttpPost]
        public IActionResult CrearDatosExamenLab(DatosExamenesLabClinico model, int datoid)
        {
            if (ModelState.IsValid)
            {
                model.ExamenLabClinicoId = datoid;
                _laboratorioClinico.Add(model);

                TempData["Message"] = "¡El registro se ha insertado con éxito.!";
                // return RedirectToAction("ListaCategorias");
                return RedirectToAction("ModificarExamenLab", new { id = datoid });
            }

            // var viewModel = new ExamenLabClinicoViewModel();
            // model.Init(_laboratorioClinico);

            return RedirectToAction("ModificarExamenLab", new { id = datoid });
        }

        public IActionResult RealizarExamenClinico()
        {
            var model = new RealizarExamenLabClinicoViewModel();
            ViewBag.MostrarEstado = false;

            model.Init(_laboratorioClinico, _empleadosDoctoresRepository,
            _pacientesRepository, _envioRepository);
            return View(model);
        }

        [HttpPost]
        public IActionResult RealizarExamenClinico(RealizarExamenLabClinicoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // var usuario = _usuarioRepository.Get();

                viewModel.Examen.FechaRealizacion = DateTime.Now;
                viewModel.Examen.EstadoExamenId = 2; // 2 es en proceso
                _laboratorioClinico.Add(viewModel.Examen);

                TempData["Message"] = "¡El registro se ha insertado con éxito.!";
                // return RedirectToAction("ListaCategorias");
                return RedirectToAction("GenerarReporteExamen", "CrearPDF", new { viewModel.Examen.Id });

            }

            var model = new RealizarExamenLabClinicoViewModel();

            model.Init(_laboratorioClinico, _empleadosDoctoresRepository, _pacientesRepository, _envioRepository);
            return View(model);
        }





        // public async Task<IActionResult> ListaExamenesRealizados(
        //     string sortOrder,
        //     string fechaInicial,
        //     string fechaFinal,
        //     string buscar,
        //     string medicoReferido,
        //     string estado,
        //     string usuarioSolicitaRequest,
        //     string usuarioIngresoRequest,
        //     string currentFilter,
        //     int? pageNumber,
        //     string categoriaLabClinico,
        //     string categoriaExcluir) // NUEVO PARÁMETRO
        // {
        //     if (buscar != null)
        //     {
        //         pageNumber = 1;
        //     }
        //     else
        //     {
        //         buscar = currentFilter;
        //     }

        //     ViewData["CurrentFilter"] = buscar;
        //     ViewData["FechaInicial"] = fechaInicial;
        //     ViewData["FechaFinal"] = fechaFinal;
        //     ViewData["Estadol"] = estado;
        //     ViewData["usuarioSolicita"] = usuarioSolicitaRequest;
        //     ViewData["usuarioIngreso"] = usuarioIngresoRequest;
        //     ViewData["categoriaLabClinico"] = categoriaLabClinico;
        //     ViewData["categoriaExcluir"] = categoriaExcluir; // NUEVO ViewData

        //     // Obtener la lista completa de exámenes
        //     var lista = _laboratorioClinico.PaginacionExamenesRealizados(sortOrder, buscar, pageNumber, 2000, null);

        //     // --- Generar el SelectList de Categorías ---
        //     var listaCategorias = lista
        //         .SelectMany(ex => ex.DetalleExamenes)
        //         .Where(de => de.ExamenLabClinico != null &&
        //                      de.ExamenLabClinico.CategoriaLabClinico != null)
        //         .Select(de => de.ExamenLabClinico.CategoriaLabClinico)
        //         .Distinct()
        //         .ToList();

        //     ViewBag.CategoriaLabClinico = new SelectList(listaCategorias, "Id", "Nombre");
        //     ViewBag.CategoriaExcluir = new SelectList(listaCategorias, "Id", "Nombre"); // NUEVO ViewBag

        //     if (!string.IsNullOrEmpty(categoriaLabClinico))
        //     {
        //         if (int.TryParse(categoriaLabClinico, out int categoriaId))
        //         {
        //             lista = lista
        //                 .Where(item => item.DetalleExamenes != null
        //                     && item.DetalleExamenes.Any()
        //                     && item.DetalleExamenes.Any(detalle =>
        //                         detalle.ExamenLabClinico?.CategoriaLabClinico?.Id == categoriaId))
        //                 .ToList();
        //         }
        //     }

        //     if (!string.IsNullOrEmpty(categoriaExcluir))
        //     {
        //         if (int.TryParse(categoriaExcluir, out int categoriaExcluirId))
        //         {
        //             lista = lista
        //                 .Where(item => item.DetalleExamenes == null
        //                     || !item.DetalleExamenes.Any()
        //                     || item.DetalleExamenes.Any(detalle =>
        //                         detalle.ExamenLabClinico?.CategoriaLabClinico?.Id != categoriaExcluirId))
        //                 .ToList();
        //         }
        //     }


        //     // Resto del código original...
        //     ViewBag.Estado = new SelectList(_estadoExamenRepository.GetAll(), "Nombre", "Nombre");
        //     var modeloLista = new List<ExamenesRealizadosViewModel>();

        //     PaginacionList<ExamenesRealizadosViewModel> pagList;

        //     foreach (var item in lista)
        //     {
        //         var paciente = item.Paciente == null ? "" : item.Paciente.Nombre;
        //         var pacienteEmail = item.Paciente == null ? "" : item.Paciente.Email;
        //         var pacienteCelular = item.Paciente == null ? "" : item.Paciente.Celular;
        //         var NombreEstado = item.EstadoExamen == null ? "Sin asignar" : item.EstadoExamen.Nombre;
        //         var medico = item.Medicos == null ? "Sin referencia" : item.Medicos.Nombres;

        //         var usuarioSolicita = await _userManager.FindByIdAsync(item.UsuarioSolicita);
        //         var usuarioIngreso = await _userManager.FindByIdAsync(item.UsuarioIngresa);

        //         var usuarioSolicitaText = usuarioSolicita == null ? "Sin asignar" : usuarioSolicita.Email;
        //         var usuarioIngresoText = usuarioIngreso == null ? "Sin asignar" : usuarioIngreso.Email;

        //         // Inicializamos las variables para almacenar el nombre del examen y el nombre de la categoría
        //         string nombreExamen = "";
        //         string nombreCategoria = "";

        //         // Si existen detalles, tomamos el primero para extraer los datos
        //         if (item.DetalleExamenes != null && item.DetalleExamenes.Any())
        //         {
        //             var primerDetalle = item.DetalleExamenes.FirstOrDefault();
        //             // Se obtiene el nombre del examen desde ExamenLabClinico
        //             nombreExamen = primerDetalle?.ExamenLabClinico?.NombreExamen ?? "";
        //             // Se obtiene el nombre de la categoría del examen
        //             nombreCategoria = primerDetalle?.ExamenLabClinico?.CategoriaLabClinico?.Nombre ?? "";
        //         }

        //         var objeto = new ExamenesRealizadosViewModel()
        //         {
        //             ExamenNumero = item.Id,
        //             Paciente = paciente,
        //             PacienteEmail = pacienteEmail,
        //             PacienteCelular = pacienteCelular,
        //             FechaSolicitud = item.FechaRealizacion,
        //             FechaRealizacion = item.FechaActualizacion,
        //             MedicoReferido = medico,
        //             ClinicaReferida = item.ClinicaReferida,
        //             Estado = NombreEstado,
        //             UsuarioSolicita = usuarioSolicitaText,
        //             UsuarioIngreso = usuarioIngresoText,
        //             NombreExamen = nombreExamen,
        //             CategoriaLabClinico = nombreCategoria
        //         };

        //         modeloLista.Add(objeto);
        //     }

        //     Console.WriteLine(modeloLista.Count + "  ***");
        //     // --- Resto del código original para otros filtros ---

        //     // Lista desplegable de médicos
        //     var listMedicos = (modeloLista.Select(x => x.MedicoReferido))
        //                         .Distinct().ToList();
        //     listMedicos.RemoveAll(x => x == null);
        //     var lisFinalMedicos = listMedicos
        //                         .Select(x => new { Nombres = x })
        //                         .ToList();
        //     ViewBag.MedicoReferido = new SelectList(lisFinalMedicos, "Nombres", "Nombres");

        //     // Lista desplegable de usuarios solicita
        //     var listUsuariosSolicita = ((modeloLista.Select(x => x.UsuarioSolicita))
        //                         .Distinct().ToList())
        //                         .Select(x => new { Nombres = x })
        //                         .ToList();
        //     ViewBag.UsuarioSolicita = new SelectList(listUsuariosSolicita, "Nombres", "Nombres");

        //     // Lista desplegable de usuarios ingreso
        //     var listUsuarioIngreso = ((modeloLista.Select(x => x.UsuarioIngreso))
        //                         .Distinct().ToList())
        //                         .Select(x => new { Nombres = x })
        //                         .ToList();
        //     ViewBag.UsuarioIngreso = new SelectList(listUsuarioIngreso, "Nombres", "Nombres");

        //     // --- Filtros adicionales (por fechas, médico, estado, etc.) ---
        //     // --- Filtros adicionales (por fechas, médico, estado, etc.) ---
        //     string errorFiltroFechas = null;

        //     // Siempre mantenemos lo que vino del usuario para que vuelva a la vista
        //     ViewData["FechaInicial"] = fechaInicial;
        //     ViewData["FechaFinal"] = fechaFinal;

        //     // ¿El usuario escribió algo en al menos uno de los campos?
        //     bool tieneFechaInicial = !string.IsNullOrWhiteSpace(fechaInicial);
        //     bool tieneFechaFinal = !string.IsNullOrWhiteSpace(fechaFinal);

        //     if (tieneFechaInicial || tieneFechaFinal)
        //     {
        //         // CASO 1: Falta una de las dos → NO se aplica filtro, solo mensaje
        //         if (!tieneFechaInicial)
        //         {
        //             errorFiltroFechas = "Debe ingresar la fecha inicial para aplicar el filtro por rango. "
        //                               + "Si desea buscar en un único día, use la misma fecha en ambos campos.";
        //         }
        //         else if (!tieneFechaFinal)
        //         {
        //             errorFiltroFechas = "Debe ingresar la fecha final para aplicar el filtro por rango. "
        //                               + "Si desea buscar en un único día, use la misma fecha en ambos campos.";
        //         }
        //         else
        //         {
        //             // CASO 2: Vienen ambas fechas → intentamos parsear y aplicar BETWEEN
        //             if (DateTime.TryParse(fechaInicial, out var fi) &&
        //                 DateTime.TryParse(fechaFinal, out var ff))
        //             {
        //                 var inicio = fi.Date;
        //                 var fin = ff.Date;

        //                 // IMPORTANTE:
        //                 // Actualmente filtras por FechaRealizacion (que viene de item.FechaActualizacion en el ViewModel).
        //                 // Si quieres filtrar por fecha de solicitud, cambia x.FechaRealizacion por x.FechaSolicitud.
        //                 modeloLista = modeloLista
        //                     .Where(x => x.FechaRealizacion.Date >= inicio
        //                              && x.FechaRealizacion.Date <= fin)
        //                     .ToList();
        //             }
        //             else
        //             {
        //                 errorFiltroFechas = "Las fechas ingresadas no tienen un formato válido. "
        //                                   + "Por favor, seleccione las fechas usando el control de calendario.";
        //             }
        //         }
        //     }

        //     // Pasamos el mensaje (si existe) a la vista
        //     ViewBag.ErrorFiltroFechas = errorFiltroFechas;


        //     if (!string.IsNullOrEmpty(medicoReferido))
        //     {
        //         modeloLista = modeloLista.Where(x =>
        //             x.MedicoReferido != null &&
        //             x.MedicoReferido.ToLower().Trim() == medicoReferido.ToLower().Trim()).ToList();
        //     }

        //     if (!string.IsNullOrEmpty(estado))
        //     {
        //         modeloLista = modeloLista.Where(x =>
        //             x.Estado != null &&
        //             x.Estado.ToLower().Trim() == estado.ToLower().Trim()).ToList();
        //     }

        //     if (!string.IsNullOrEmpty(usuarioSolicitaRequest))
        //     {
        //         modeloLista = modeloLista.Where(x =>
        //             x.UsuarioSolicita != null &&
        //             x.UsuarioSolicita.ToLower().Trim() == usuarioSolicitaRequest.ToLower().Trim()).ToList();
        //     }

        //     if (!string.IsNullOrEmpty(usuarioIngresoRequest))
        //     {
        //         modeloLista = modeloLista.Where(x =>
        //             x.UsuarioIngreso != null &&
        //             x.UsuarioIngreso.ToLower().Trim() == usuarioIngresoRequest.ToLower().Trim()).ToList();
        //     }

        //     pagList = PaginacionList<ExamenesRealizadosViewModel>.CreateAsynccCustom(modeloLista,
        //         pageNumber ?? 1, 50);


        //     Console.WriteLine("THIIIIS");

        //     return View(pagList);
        // }




        [HttpGet]
        public async Task<IActionResult> ListaExamenesRealizados(
            DateTime? fechaInicial,
            DateTime? fechaFinal,
            string buscar,
            string medicoReferido,
            string estado,
            string usuarioSolicitaRequest,
            string usuarioIngresoRequest,
            string categoriaLabClinico,
            string categoriaExcluir)
        {
            ViewData["FechaInicial"] = fechaInicial?.ToString("yyyy-MM-dd");
            ViewData["FechaFinal"] = fechaFinal?.ToString("yyyy-MM-dd");
            ViewData["CurrentFilter"] = buscar;

            var consulta = _laboratorioClinico.ObtenerExamenesQueryable(buscar, null);

            if (fechaInicial.HasValue && fechaFinal.HasValue)
            {
                // Convertir día a UTC
                var inicioLocal = fechaInicial.Value.Date;
                var finLocal = fechaFinal.Value.Date.AddDays(1);

                var inicioUtc = TimeZoneInfo.ConvertTimeToUtc(inicioLocal);
                var finUtc = TimeZoneInfo.ConvertTimeToUtc(finLocal);

                consulta = consulta.Where(x =>
                    x.FechaRealizacion >= inicioUtc &&
                    x.FechaRealizacion < finUtc);
            }

            if (int.TryParse(categoriaLabClinico, out int catId))
            {
                consulta = consulta.Where(x =>
                    x.DetalleExamenes.Any(d =>
                        d.ExamenLabClinico.CategoriaLabClinico.Id == catId));
            }

            if (int.TryParse(categoriaExcluir, out int exId))
            {
                consulta = consulta.Where(x =>
                    !x.DetalleExamenes.Any(d =>
                        d.ExamenLabClinico.CategoriaLabClinico.Id == exId));
            }

            var listaBase = await consulta.ToListAsync();

            var usuariosMap = await _userManager.Users
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            var modeloLista = listaBase.Select(item =>
            {
                var detalle = item.DetalleExamenes?.FirstOrDefault();

                return new ExamenesRealizadosViewModel
                {
                    ExamenNumero = item.Id,
                    Paciente = item.Paciente?.Nombre ?? "N/A",
                    PacienteEmail = item.Paciente?.Email ?? "",
                    PacienteCelular = item.Paciente?.Celular ?? item.Paciente?.Telefono ?? "",
                    FechaRealizacion = item.FechaActualizacion,
                    FechaSolicitud = item.FechaRealizacion,
                    MedicoReferido = item.Medicos?.Nombres ?? "Sin referencia",
                    Estado = item.EstadoExamen?.Nombre ?? "Sin asignar",
                    UsuarioSolicita = usuariosMap
                        .FirstOrDefault(u => u.Id == item.UsuarioSolicita)?.Email ?? "N/A",
                    UsuarioIngreso = usuariosMap
                        .FirstOrDefault(u => u.Id == item.UsuarioIngresa)?.Email ?? "N/A",
                    NombreExamen = detalle?.ExamenLabClinico?.NombreExamen ?? "",
                    CategoriaLabClinico = detalle?.ExamenLabClinico
                        ?.CategoriaLabClinico?.Nombre ?? ""
                };
            }).ToList();

            if (!string.IsNullOrEmpty(medicoReferido))
                modeloLista = modeloLista
                    .Where(x => x.MedicoReferido == medicoReferido)
                    .ToList();

            if (!string.IsNullOrEmpty(estado) && estado != "Todos")
                modeloLista = modeloLista
                    .Where(x => x.Estado == estado)
                    .ToList();

            CargarFiltrosViewBag(modeloLista, listaBase);

            return View(modeloLista);
        }

        private void CargarFiltrosViewBag(
            List<ExamenesRealizadosViewModel> modelo,
            List<Examen> original)
        {
            var categorias = original
                .SelectMany(x => x.DetalleExamenes)
                .Select(d => d.ExamenLabClinico.CategoriaLabClinico)
                .Where(c => c != null)
                .Distinct()
                .ToList();

            ViewBag.CategoriaLabClinico = new SelectList(categorias, "Id", "Nombre");
            ViewBag.CategoriaExcluir = new SelectList(categorias, "Id", "Nombre");
            ViewBag.Estado = new SelectList(
                _estadoExamenRepository.GetAll(), "Nombre", "Nombre");

            var usuariosSolicita = modelo
                .Select(x => x.UsuarioSolicita)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            var usuariosIngreso = modelo
                .Select(x => x.UsuarioIngreso)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ViewBag.UsuarioSolicita = new SelectList(usuariosSolicita);
            ViewBag.UsuarioIngreso = new SelectList(usuariosIngreso);
        }


        //public IActionResult ExamenesEnProceso(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        //{
        //    if (buscar != null)
        //    {
        //        pageNumber = 1;
        //    }
        //    else
        //    {
        //        buscar = currentFilter;
        //    }

        //    ViewData["CurrentFilter"] = buscar;

        //    var lista = _laboratorioClinico.PaginacionExamenesRealizados(sortOrder, buscar, pageNumber, 30, 2);

        //    return View(lista);
        //}

        //public IActionResult ExamenesSolicitados(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        //{
        //    if (buscar != null)
        //    {
        //        pageNumber = 1;
        //    }
        //    else
        //    {
        //        buscar = currentFilter;
        //    }

        //    ViewData["CurrentFilter"] = buscar;

        //    var lista = _laboratorioClinico.PaginacionExamenesRealizados(sortOrder, buscar, pageNumber, 30, 1, true);

        //    return View(lista);
        //}

        //public IActionResult ExamenesCancelados(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        //{
        //    if (buscar != null)
        //    {
        //        pageNumber = 1;
        //    }
        //    else
        //    {
        //        buscar = currentFilter;
        //    }

        //    ViewData["CurrentFilter"] = buscar;

        //    var lista = _laboratorioClinico.PaginacionExamenesRealizados(sortOrder, buscar, pageNumber, 30, 3);

        //    return View(lista);
        //}

        //public IActionResult ExamenesEnRevision(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        //{
        //    if (buscar != null)
        //    {
        //        pageNumber = 1;
        //    }
        //    else
        //    {
        //        buscar = currentFilter;
        //    }

        //    ViewData["CurrentFilter"] = buscar;

        //    var lista = _laboratorioClinico.PaginacionExamenesRealizados(sortOrder, buscar, pageNumber, 30, 4);

        //    return View(lista);
        //}

        //public IActionResult ExamenesFinalizados(string sortOrder, string buscar, string currentFilter, int? pageNumber)
        //{
        //    if (buscar != null)
        //    {
        //        pageNumber = 1;
        //    }
        //    else
        //    {
        //        buscar = currentFilter;
        //    }

        //    ViewData["CurrentFilter"] = buscar;

        //    var lista = _laboratorioClinico.PaginacionExamenesRealizados(sortOrder, buscar, pageNumber, 30, 5);

        //    return View(lista);
        //}

        public JsonResult RetornarExamenes(string id)
        {
            var examenBuscado = _laboratorioClinico.GetExamenLab(Convert.ToInt16(id));

            if (examenBuscado == null)
            {
                return new JsonErrorResult(new { message = "" });
            }

            var objetos = new
            {
                id = examenBuscado.Id,
                nombreServicio = examenBuscado.NombreExamen,
                precio = examenBuscado.Precio,
                precioB = examenBuscado.PrecioB,
                precioC = examenBuscado.PrecioC
            };

            return Json(objetos);
        }

        public JsonResult ObtenerListadoExamenesLab()
        {
            var listado = _laboratorioClinico.ExamenesLabListTodos();

            var list = new List<object>();

            foreach (var item in listado)
            {
                var model = new
                {
                    id = item.Id,
                    nombreServicio = item.NombreExamen,
                    categoria = item.CategoriaLabClinico.Nombre
                };

                list.Add(model);
            }
            ;

            return Json(list);
        }


        //[Authorize(Roles = "Administrador, Supervisor, Vendedor, Mensajero,Desarrollador,Farmacia,Laboratorio,Hospital,Clinica,Urologia,Algologia")]
        //public JsonResult GuardarVentaLab([FromBody] VentaClinicaAddViewModel model)
        //{
        //    // registrar un identificador si es de farm o de clinica

        //    if (ModelState.IsValid)
        //    {
        //        if (string.IsNullOrEmpty(model.encabezado.Nombres))
        //        {
        //            model.encabezado.Nombres = "CF";
        //        }

        //        if (string.IsNullOrEmpty(model.encabezado.Nit))
        //        {
        //            model.encabezado.Nit = "CF";
        //        }

        //        if (string.IsNullOrEmpty(model.encabezado.Direccion))
        //        {
        //            model.encabezado.Direccion = "CF";
        //        }

        //        // cargar lista de todas las cajas
        //        var cajita = _laboratorioClinico.ListarCajas();

        //        // verificar si hay cajas abiertas, una por una, si te acordas del Any()
        //        // otra manera seria hacerlo con una bandera, pero el any te hace el trabajo.
        //        if (!cajita.Any(a => a.EstadoCaja == true))
        //        {
        //            return new JsonErrorResult(new { message = "¡Error. No hay cajas abiertas. Por favor debe abrir una caja.!" });
        //        }

        //        var paciente = _pacienteRepository.Get(model.encabezado.ClienteId);
        //        var cajaAbierta = _laboratorioClinico.GetCajaAbierta();
        //        var empleado = _empleadoRepository.Get(model.encabezado.EmpleadoId);
        //        var medico = _empleadoRepository.GetMedicoByName(model.encabezado.Medico);
        //        var clinica = _empleadoRepository.GetClinicaByName(model.encabezado.Clinica);

        //        var medicoNuevo = new Medicos();
        //        var clinicaNueva = new Clinica();

        //        if (empleado == null)
        //        {
        //            return new JsonErrorResult(new { message = "¡Codigo de empleado incorrecto.!" });
        //        }

        //        if (medico == null)
        //        {
        //            medicoNuevo = new Medicos()
        //            {
        //                Nombres = model.encabezado.Medico
        //            };
        //        }

        //        if (clinica == null)
        //        {
        //            clinicaNueva = new Clinica()
        //            {
        //                NombreClinica = model.encabezado.Clinica
        //            };
        //        }
        //        // _empleadoRepository.Add(nuevoMedico, false);

        //        var examen = new Examen()
        //        {
        //            Paciente = paciente,
        //            EstadoExamenId = 1,
        //            FechaRealizacion = DateTime.Now,
        //            Medicos = medico == null ? medicoNuevo : medico,
        //            Clinicas = clinica == null ? clinicaNueva : clinica,
        //            ClinicaReferida = model.encabezado.ClinicaReferida,
        //            UsuarioSolicita = empleado.Users.FirstOrDefault().Id
        //        };

        //        var nuevaVenta = new VentasLab()
        //        {
        //            // NoComprobante = model.encabezado.NoComprobante,
        //            Nombres = model.encabezado.Nombres,
        //            Nit = model.encabezado.Nit,
        //            Direccion = model.encabezado.Direccion,
        //            // Paciente = paciente,
        //            // FormaPago = model.detalle.encabezado.FormaPago,
        //            FechaVenta = DateTime.Now,
        //            EmpleadoResponsable = empleado.Nombre,
        //            // TipoVenta = "clinica",
        //            MontoPagado = model.encabezado.Monto,
        //            Vuelto = model.encabezado.Vuelto,
        //            Examen = examen
        //        };

        //        var pago = new Pagos()
        //        {
        //            VentaLab = nuevaVenta,
        //            FormaPagoId = Convert.ToInt32(model.encabezado.FormaPago),
        //            Monto = Convert.ToDecimal(model.encabezado.Monto),
        //        };

        //        _envioRepository.AddPago(pago, false);



        //        var nuevoDetalleCaja = new DetalleCajaLab()
        //        {
        //            VentasLab = nuevaVenta,
        //            Descripcion = "Venta de examen: " + paciente.Nombre,
        //            Ingreso = model.encabezado.Monto,
        //            CajaLab = cajaAbierta
        //        };

        //        _laboratorioClinico.Add(nuevoDetalleCaja, false);

        //        foreach (var item in model.detalle)
        //        {

        //            var nuevodetalle = new DetalleExamen()
        //            {
        //                Examen = examen,
        //                Cantidad = item.Cantidad,
        //                PrecioValor = item.Precio,
        //                 = item.,
        //                Subtotal = item.Subtotal,
        //                Total = item.Total,
        //                ExamenLabClinicoId = item.ProductoId,
        //            };

        //            _laboratorioClinico.Add(nuevodetalle, false);

        //            var datos = _laboratorioClinico.DatosLabList((int)item.ProductoId);

        //            foreach (var dato in datos)
        //            {
        //                var newDato = new Resultados()
        //                {
        //                    DatosExamenesLabClinico = dato,
        //                    DetalleExamen = nuevodetalle
        //                };

        //                _laboratorioClinico.Add(newDato, false);

        //            }


        //            //restar al inventario
        //            // var producto = _productoRepository.Get((int)nuevodetalle.ProductoId);
        //            // producto.Stock -= nuevodetalle.Cantidad;
        //            // _productoRepository.Update(producto, false);
        //        }

        //        _laboratorioClinico.saveChanges();

        //        TempData["Message"] = "¡La venta se ha guardado con éxito.!";

        //        return Json(nuevaVenta.Id);

        //    }

        //    return Json("Hubo un error interno.");
        //}


        //public IActionResult Aperturar()
        //{
        //    var model = new CajaLaboratorioBaseViewModel()
        //    {
        //    };

        //    model.Init(_laboratorioClinico, _sucursalRepository);


        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Aperturar(CajaLaboratorioBaseViewModel model)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        // var cajaAbierta = _cajaRepository.GetCajaAbierta();
        //        var cajita = _laboratorioClinico.ListarCajas()
        //            .Where(a => a.SucursalId == model.AperturarSucursalId)
        //            .ToList();

        //        //verificar si no hay cajas abiertas

        //        if (cajita.Any(a => a.EstadoCaja == true))
        //        {
        //            TempData["Message"] = "¡Error. Ya hay una caja abierta en esta sucursal!";
        //            model.Init(_laboratorioClinico, _sucursalRepository);
        //            return RedirectToAction("Aperturar");
        //        }
        //        else
        //        {
        //            var user = await _userManager.GetUserAsync(HttpContext.User);

        //            var nuevaCaja = new CajaLab()
        //            {
        //                MontoApertura = model.CajaLab.MontoApertura,
        //                FechaApertura = DateTime.Now,
        //                SucursalId = model.AperturarSucursalId,
        //                EstadoCaja = true,
        //                ResponsableAperturaLab = user
        //            };

        //            _laboratorioClinico.Add(nuevaCaja);

        //            model.Init(_laboratorioClinico, _sucursalRepository);
        //            return RedirectToAction("Aperturar");
        //        }
        //    }

        //    model.Init(_laboratorioClinico, _sucursalRepository);
        //    return View(model);
        //}


        //public JsonResult GuardarEgreso(string monto, string descripcion)
        //{
        //    if (monto != null && descripcion != null)
        //    {
        //        var cajaAbierta = _laboratorioClinico.GetCajaAbierta();

        //        var nuevoDetalleCaja = new DetalleCajaLab()
        //        {
        //            Fecha = DateTime.Now,
        //            Descripcion = descripcion,
        //            Gasto = Convert.ToInt32(monto),
        //            CajaLab = cajaAbierta,
        //        };

        //        _laboratorioClinico.Add(nuevoDetalleCaja);

        //        TempData["Message"] = "¡El gasto se ha guardado con exito.!";

        //        return Json(nuevoDetalleCaja.CajaLab.Id);
        //    }

        //    return Json("Ha ocurrido un error");
        //}

        //public JsonResult GuardarIngreso(string monto, string descripcion)
        //{

        //    if (monto != null && descripcion != null)
        //    {

        //        var cajaAbierta = _laboratorioClinico.GetCajaAbierta();

        //        var nuevoDetalleCaja = new DetalleCajaLab()
        //        {
        //            Fecha = DateTime.Now,
        //            Descripcion = descripcion,
        //            Ingreso = Convert.ToInt32(monto),
        //            CajaLab = cajaAbierta,
        //        };

        //        _laboratorioClinico.Add(nuevoDetalleCaja);

        //        TempData["Message"] = "¡El ingreso se ha guardado con exito.!";

        //        return Json(nuevoDetalleCaja.CajaLab.Id);


        //    }

        //    return Json("Ha ocurrido un error");

        //}

        //public async Task<IActionResult> Cerrar(CajaLaboratorioBaseViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.GetUserAsync(HttpContext.User);

        //        var caja = _laboratorioClinico.GetCaja(model.CajaLab.Id);
        //        caja.EstadoCaja = false;
        //        caja.FechaCierre = DateTime.Now;
        //        caja.ResponsableCierreLab = user;

        //        _laboratorioClinico.Update(caja);

        //        model.Init(_laboratorioClinico, _sucursalRepository);
        //        return RedirectToAction("Aperturar");
        //    }

        //    model.Init(_laboratorioClinico, _sucursalRepository);
        //    return View(model);
        //}


        //public IActionResult VerDetalle(int? id)
        //{

        //    if (id == null)
        //    {
        //        return BadRequest("request is incorrect");
        //    }

        //    var caja = _laboratorioClinico.GetCaja((int)id);

        //    if (caja == null)
        //    {
        //        return StatusCode(404);
        //    }

        //    var model = new CajaLaboratorioBaseViewModel()
        //    {
        //        CajaLab = caja
        //    };

        //    return View(model);
        //}

        public IActionResult ModificarResultadosExamen(int detalleExamenId)
        {
            var detalle = _laboratorioClinico.GetDetalleExamenLab((int)detalleExamenId);

            var model = new ModificarResultadosExamen()
            {
                DetalleExamen = detalle,
                ExamenId = detalle.ExamenId,
                ExamenLabClinicoId = detalle.ExamenLabClinicoId,
                DatosResultados = new List<Resultados>()
            };

            var resultados = _laboratorioClinico.ResultadosListByDetalleId((int)detalleExamenId);


            if (resultados == null || resultados.Count == 0)
            {
                var datosExamenLabClinico = _laboratorioClinico.DatosLabList(detalle.ExamenLabClinicoId);
                foreach (var dato in datosExamenLabClinico)
                {
                    model.DatosResultados.Add(new Resultados
                    {
                        Id = dato.Id,
                        DetalleExamenId = detalle.Id,
                        ValorResultado = dato.Resultado,
                        DatosExamenesLabClinicoId = dato.Id,
                        DatosExamenesLabClinico = dato
                    });
                }
            }
            else
            {
                model.DatosResultados = resultados;
            }

            model.Init(_laboratorioClinico);

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ModificarResultadosExamen(ModificarResultadosExamen viewModel, string texto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var userText = user == null ? "" : user.Id;
                // /LaboratorioClinico/EditarDetalleExamen/15
                // var detalle = _laboratorioClinico.GetDetalleExamenLab((int) viewModel.Id);

                if (viewModel.DatosResultados != null)
                {
                    foreach (var item in viewModel.DatosResultados)
                    {
                        var resultado = _laboratorioClinico.GetResultadoById(item.Id);
                        if (resultado == null)
                        {
                            var nuevoResultado = new Resultados
                            {
                                DatosExamenesLabClinicoId = item.DatosExamenesLabClinicoId,
                                DetalleExamenId = item.DetalleExamenId,
                                ValorResultado = item.ValorResultado,
                                Huevos = item.Huevos,
                                Larvas = item.Larvas,
                                Quistes = item.Quistes,
                                Trofozoitos = item.Trofozoitos
                            };
                            _laboratorioClinico.Add(nuevoResultado);
                        }
                        else
                        {
                            resultado.ValorResultado = item.ValorResultado;
                            resultado.Huevos = item.Huevos;
                            resultado.Larvas = item.Larvas;
                            resultado.Quistes = item.Quistes;
                            resultado.Trofozoitos = item.Trofozoitos;
                            _laboratorioClinico.Update(resultado, false);
                        }


                    }
                }


                var examen = _laboratorioClinico.GetExamenRealizado((int)viewModel.ExamenId);
                examen.UsuarioIngresa = userText;

                _laboratorioClinico.Update(examen, false);

                // detalle.ExamenId = viewModel.ExamenId;c
                // detalle.ExamenLabClinicoId = viewModel.DetalleExamen.ExamenLabClinicoId;
                // detalle.Resultado = viewModel.DetalleExamen.Resultado;

                // _laboratorioClinico.Update(detalle);
                _laboratorioClinico.saveChanges();
                TempData["Message"] = "¡El registro se ha modificado con éxito.!";
                // return RedirectToAction("ListaCategorias");

                // viewModel.Init(_laboratorioClinico, _empleadosDoctoresRepository,_pacientesRepository, _envioRepository);
                // ViewBag.MostrarEstado = true;

                // viewModel.Init(_laboratorioClinico);

                return RedirectToAction("EditarDetalleExamen", new { id = viewModel.ExamenId });
            }

            return View(viewModel.DetalleExamen);

        }

        public IActionResult ModificarTodosResultadosExamen(int Id, bool radiologia = false)
        {

            var examen = _laboratorioClinico.GetExamenRealizado(Id);

            if (examen.DetalleExamenes != null)
            {
                examen.DetalleExamenes = examen.DetalleExamenes
                    .Where(d =>
                        d.ExamenLabClinico?.CategoriaLabClinico?.Id != null &&
                        (radiologia
                            ? d.ExamenLabClinico.CategoriaLabClinico.Id == 38
                            : d.ExamenLabClinico.CategoriaLabClinico.Id != 38))
                    .ToList();
            }

            var model = new ModificarTodosResultadosViewModel()
            {
                ExamenId = Id,
                DetalleExamenenes = (List<DetalleExamen>)examen.DetalleExamenes,
                DatosResultados = new List<Resultados>()

            };
            ViewBag.MostrarEstado = true;



            foreach (var detalleExamen in examen.DetalleExamenes)
            {
                var resultados = _laboratorioClinico.ResultadosListByDetalleId(detalleExamen.Id);


                if (resultados == null || resultados.Count == 0)
                {
                    var datosExamenLabClinico = _laboratorioClinico.DatosLabList(detalleExamen.ExamenLabClinicoId);
                    foreach (var dato in datosExamenLabClinico)
                    {
                        model.DatosResultados.Add(new Resultados
                        {
                            Id = dato.Id,
                            DetalleExamenId = detalleExamen.Id,
                            ValorResultado = dato.Resultado,
                            DatosExamenesLabClinicoId = dato.Id,
                            DatosExamenesLabClinico = dato
                        });
                    }
                }
                else
                {
                    //model.DatosResultados = resultados;
                    model.DatosResultados.AddRange(resultados);
                }



            }


            return View(model);

        }

        public async Task<IActionResult> ModificarTodosResultadosExamen1(ModificarTodosResultadosViewModel viewModel, string texto)
        {
            Console.WriteLine("dffsgdf");

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var userText = user == null ? "" : user.Id;
                // /LaboratorioClinico/EditarDetalleExamen/15
                // var detalle = _laboratorioClinico.GetDetalleExamenLab((int) viewModel.Id);

                if (viewModel.DatosResultados != null)
                {
                    foreach (var item in viewModel.DatosResultados)
                    {
                        var resultado = _laboratorioClinico.GetResultadoById(item.Id);
                        if (resultado == null)
                        {
                            var nuevoResultado = new Resultados
                            {
                                DatosExamenesLabClinicoId = item.DatosExamenesLabClinicoId,
                                DetalleExamenId = item.DetalleExamenId,
                                ValorResultado = item.ValorResultado,
                                Huevos = item.Huevos,
                                Larvas = item.Larvas,
                                Quistes = item.Quistes,
                                Trofozoitos = item.Trofozoitos
                            };
                            _laboratorioClinico.Add(nuevoResultado);
                        }
                        else
                        {
                            resultado.ValorResultado = item.ValorResultado;
                            resultado.Huevos = item.Huevos;
                            resultado.Larvas = item.Larvas;
                            resultado.Quistes = item.Quistes;
                            resultado.Trofozoitos = item.Trofozoitos;
                            _laboratorioClinico.Update(resultado, false);
                        }


                    }
                }


                var examen = _laboratorioClinico.GetExamenRealizado((int)viewModel.ExamenId);
                examen.UsuarioIngresa = userText;

                _laboratorioClinico.Update(examen, false);

                // detalle.ExamenId = viewModel.ExamenId;c
                // detalle.ExamenLabClinicoId = viewModel.DetalleExamen.ExamenLabClinicoId;
                // detalle.Resultado = viewModel.DetalleExamen.Resultado;

                // _laboratorioClinico.Update(detalle);
                _laboratorioClinico.saveChanges();
                TempData["Message"] = "¡El registro se ha modificado con éxito.!";
                // return RedirectToAction("ListaCategorias");

                // viewModel.Init(_laboratorioClinico, _empleadosDoctoresRepository,_pacientesRepository, _envioRepository);
                // ViewBag.MostrarEstado = true;

                // viewModel.Init(_laboratorioClinico);

                return RedirectToAction("EditarDetalleExamen", new { id = viewModel.ExamenId });
            }

            return View(viewModel.DetalleExamenenes);
        }
        public JsonResult RetornarPaciente(int id)
        {
            var paciente = _pacienteRepository.Get((int)id);

            var model = new
            {
                nombres = paciente.Nombre,
                nit = paciente.Nit,
                direccion = paciente.Direccion,
            };

            return Json(model);
        }

        public string ConsultarInsumosExistentes()
        {
            var bodegafarmaciaid = 1;
            try
            {
                var insumos = _productosService.GetInventario(null, bodegafarmaciaid);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = insumos
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


        //public JsonResult ConsultarUnidadesVentaInsumo(int productoId)
        //{
        //    try
        //    {
        //        var producto = _productosRepository.GetProductos(null, null)
        //            .Where(p => p.Id == productoId)
        //            .FirstOrDefault();

        //        return Json(new { Exitoso = true, Resultado = producto.ProductoEquivalencias });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Exitoso = false, Mensaje = "Error al consultar unidades de venta. " + ex.Message });
        //    }
        //}

        //en revision y proceso de ser eliminado
        //pues esta siendo utilizado desde un JS
        public string ConsultarUnidadesVentaInsumo(int productoId)
        {
            try
            {
                var insumosExistentes = new List<InsumoEquivalenciaUnidadBaseViewModel>();

                var producto = _productosRepository.GetProductos(null, null)
                    .Where(p => p.Id == productoId)
                    .FirstOrDefault();
                if (producto != null)
                {
                    foreach (var insumo in producto.ProductoEquivalencias)
                    {
                        insumosExistentes.Add(new InsumoEquivalenciaUnidadBaseViewModel
                        {
                            Id = insumo.UnidadMedidaVenta.Id,
                            Nombre = insumo.UnidadMedidaVenta.Nombre
                        });
                    }

                }
                var productoInventarioId = _productosRepository.GetProductoInventariobyId(productoId);
                //if (PrecioCosto==null) { PrecioCosto = "0"; }
                //return Json(new { Exitoso = true, Resultado = producto.ProductoEquivalencias });
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = insumosExistentes,
                    PrecioCosto = productoInventarioId.PrecioCosto
                });


            }
            catch (Exception ex)
            {
                //return Json(new { Exitoso = false, Mensaje = "Error al consultar unidades de venta. " + ex.Message });
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar insumos. " + ex.Message
                });
            }
        }


        public string consultarInsumosPreCargadosExamenes(int idExamen)
        {
            try
            {
                var insumosExistentes = new List<InsumosAsignadosExamen>();

                var insumosAsignados = _productosRepository.GetInsumosAsignadosExamenLab(idExamen);
                if (insumosAsignados != null)
                {
                    foreach (var insumo in insumosAsignados)
                    {
                        var unidadVenta = insumo.UnidadMedidaVenta ?? new UnidadMedidaVenta();
                        insumosExistentes.Add(new InsumosAsignadosExamen
                        {
                            Id = insumo.Id,
                            ProductoId = insumo.ProductoId,
                            ProductoNombre = insumo.Producto.NombreProducto,
                            UnidadMedidaVentaId = insumo.UnidadMedidaVentaId,
                            UnidadMedidaVentaNombre = unidadVenta.Nombre ?? "UN",
                            PrecioCostoInsumo = insumo.PrecioCosto,
                            CantidadUtilizada = insumo.CantidadUtilizada,
                            TotalInsumo = insumo.Total,
                            Nuevo = true
                        });
                    }

                }
                //var PrecioCosto = _productosRepository.GetProductoInventariobyId(productoId);
                //if (PrecioCosto==null) { PrecioCosto = "0"; }
                //return Json(new { Exitoso = true, Resultado = producto.ProductoEquivalencias });
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = insumosExistentes,
                    //PrecioCosto = PrecioCosto.precioUnidadVenta
                });


            }
            catch (Exception ex)
            {
                //return Json(new { Exitoso = false, Mensaje = "Error al consultar unidades de venta. " + ex.Message });
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar insumos. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarLaboratorioExistentes()
        {
            try
            {
                var laboratorioExsitente = new List<ExamenLabClinicoViewModel>();
                var laboratorioBd = _laboratorioClinico.GetListExamenesLaboratorio();
                if (laboratorioBd != null)
                {
                    foreach (var lab in laboratorioBd)
                    {
                        laboratorioExsitente.Add(new ExamenLabClinicoViewModel
                        {
                            NombreExamen = lab.NombreExamen,
                            Id = lab.Id,
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = laboratorioExsitente
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar laboratorio producto: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarPreciosLaboratorioExistentes(int Id)
        {
            try
            {
                var laboratorioBd = _laboratorioClinico.GetExamenLab(Id, true);
                var listaDef = new List<object>();
                if (laboratorioBd != null)
                {
                    foreach (var lab in laboratorioBd.ExamenLabClinicosPrecios)
                    {
                        listaDef.Add(new
                        {
                            Id = lab.Id,
                            Precio = lab.PrecioValor,
                            Nombre = lab.Precio.NombrePrecio
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaDef
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar el precio del laboratorio producto: " + ex.Message
                });
            }
        }

        #region Categorias Generales
        public IActionResult ListaCategoriasGenerales(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _categoriaGeneralLabClinicoService.GetListCategoriasGeneralesLabClinico(sortOrder, buscar, pageNumber, 30);

            return View(lista);
        }

        public IActionResult NuevaCategoriaGeneral()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NuevaCategoriaGeneral(CategoriaGeneralLabClinicoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                model.UltimoUsuarioModificado = user.UserName;
                _categoriaGeneralLabClinicoService.Add(model);
                TempData["Message"] = "¡El registro se ha guardado con éxito.!";
                return RedirectToAction("ListaCategoriasGenerales");
            }

            return View(model);
        }

        public IActionResult ModificarCategoriaGeneralLab(int? id)
        {
            var categoriaViewModel = _categoriaGeneralLabClinicoService.GetCategoriaGeneralLab((int)id);
            return View(categoriaViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ModificarCategoriaGeneralLab(CategoriaGeneralLabClinicoViewModel model)
        {
            if (model == null || model.Id <= 0)
            {
                TempData["Message"] = "No se pudo identificar la categoría a modificar.";
                return RedirectToAction("ListaCategoriasGenerales");
            }

            if (!ModelState.IsValid)
                return View(model);

            var existing = _categoriaGeneralLabClinicoService.GetCategoriaGeneralLab(model.Id);
            if (existing == null)
            {
                TempData["Message"] = "La categoría no existe o fue eliminada.";
                return RedirectToAction("ListaCategoriasGenerales");
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            existing.Nombre = model.Nombre;
            existing.UltimoUsuarioModificado = user?.UserName ?? user?.Email ?? "-";
            _categoriaGeneralLabClinicoService.Update(existing);

            TempData["Message"] = "¡El registro se ha modificado con éxito!";
            return View(existing);
        }

        public IActionResult EliminarCategoriaGeneral(int? id)
        {
            var categoriaLab = _categoriaGeneralLabClinicoService.GetCategoriaGeneralLab((int)id);
            categoriaLab.Eliminado = true;
            _categoriaGeneralLabClinicoService.Update(categoriaLab);

            TempData["Message"] = "¡El registro se ha archivado con éxito.!";

            return RedirectToAction("ListaCategoriasGenerales");
        }
        #endregion
    }

    public class Result
    {
        public string texto { get; set; }
        public string id { get; set; }
    }



}
