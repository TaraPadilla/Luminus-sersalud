using System;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;
using Microsoft.AspNetCore.Authorization;
using Database.Shared.Models;
using sistema.Json;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Wkhtmltopdf.NetCore;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using Database.Shared.Enumeraciones;
using DocumentFormat.OpenXml.EMMA;
using Database.Shared.IRepository;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using sistema.UtilidadesEmailWp.Services.IService;
using Microsoft.Extensions.Configuration;
using farmamest.Utilidades;


namespace sistema.Controllers
{
    [Authorize]
    public class CitaController : Controller
    {
        private readonly ICitas _citasRepository = null;
        private readonly IConsultas _consultaRepository = null;
        // private readonly ICliente _clienteRepository = null;
        private readonly IEmpleado _empleadoRepository = null;
        private readonly IServicio _servicioRepository = null;
        private readonly IUser _userRepository = null;
        private readonly UserManager<User> _userManager = null;
        private readonly IGeneratePdf _generatePdf;
        private readonly IPacientes _pacienteRepository = null;
        private readonly ICuentasPorCobrar _cuentaPorCobrarRepository = null;
        private readonly ISucursal _sucursalRepository = null;
        private readonly ISeguro _seguroRepository = null;

        private readonly IHabitacion _habitacionRepository = null;
        private readonly IWhatsAppService _whatsAppService;
        private readonly IConfiguration _configuration;

        public CitaController(
            ICitas citasRepository,
            IPacientes pacientesRepository,
            IEmpleado empleadoRepository,
            ICuentasPorCobrar cuentaPorCobrarRepository,
            IServicio servicioRepository,
            UserManager<User> userManager,
            IUser userRepositry,
            IGeneratePdf generatePDF,
            ISucursal sucursalRepository,
            IConsultas consultaRepository,
            ISeguro seguroRepository,
             IHabitacion habitacionRepository,
             IWhatsAppService whatsAppService,
             IConfiguration configuration
            )
        {
            _citasRepository = citasRepository;
            _pacienteRepository = pacientesRepository;
            _cuentaPorCobrarRepository = cuentaPorCobrarRepository;
            _empleadoRepository = empleadoRepository;
            _servicioRepository = servicioRepository;
            _userManager = userManager;
            _userRepository = userRepositry;
            _generatePdf = generatePDF;
            _sucursalRepository = sucursalRepository;
            _consultaRepository = consultaRepository;
            _seguroRepository = seguroRepository;
            _habitacionRepository = habitacionRepository;
            _whatsAppService = whatsAppService;
            _configuration = configuration;


        }

        // public IActionResult Index()
        // {
        //     var lista = _citasRepository.GetList();

        //     var model = new CitasSchedulerViewModel()
        //     {
        //         CitasList = lista,
        //     };

        //     return View(model);
        // }


        // public JsonResult NuevaCita([FromBody]CitasSchedulerViewModel model)
        // {
        //     if(ModelState.IsValid)
        //     {
        //         var cita = new Cita()
        //         {
        //             FechaInicio = model.Cita.FechaInicio,
        //             FechaFinal = model.Cita.FechaFinal,
        //         };

        //         _citasRepository.Add(cita);
        //         return Json(cita.Id);
        //     }
        //    return new JsonErrorResult(new { message = "Ha ocurrido un error de servidor. Por favor intente más tarde." });
        // }

        public IActionResult NuevaCitaAgendarOtraConMismaFecha(string fecha)
        {
            var fechaYHora = Convert.ToDateTime(fecha);

            var model = new CitaBaseViewModel()
            {
                HoraYFecha = fechaYHora
            };

            return View(model);
        }


        public JsonResult EliminarCita(int? id, DateTime fecha)
        {
            if (id == null) return new JsonErrorResult(new { message = "Error 400" });

            var cita = _citasRepository.GetCita((int)id);

            if (cita == null) return new JsonErrorResult(new { message = "Error 404" });

            cita.Eliminado = true;

            _citasRepository.Update(cita);
            return Json(fecha.ToString("MM/dd/yyyy"));
        }

        public JsonResult functionterminarTurno(int? id, DateTime fecha)
        {
            if (id == null) return new JsonErrorResult(new { message = "Error 400" });

            var cita = _citasRepository.GetCita((int)id);

            if (cita == null) return new JsonErrorResult(new { message = "Error 404" });

            cita.EstadoTurno = "FINALIZADO";

            _citasRepository.Update(cita);

            return Json(fecha.ToString("MM/dd/yyyy"));
        }


        // public JsonResult MoverCita (int? id, DateTime fechaInicio, DateTime fechaFinal)
        // {
        //     if(id == null) return new JsonErrorResult(new { message = "Error 400" });

        //     var cita = _citasRepository.GetCita((int)id);

        //     if (cita == null) return new JsonErrorResult(new { message = "Error 404" });

        //     cita.FechaInicio = fechaInicio;
        //     cita.FechaFinal = fechaFinal;

        //     _citasRepository.Update(cita);
        //     return Json(new object());
        // }

        // public JsonResult ResizeCita(int? id, DateTime fechaInicio, DateTime fechaFinal)
        // {
        //     if(id == null) return new JsonErrorResult(new { message = "Error 400" });

        //     var cita = _citasRepository.GetCita((int)id);

        //     if (cita == null) return new JsonErrorResult(new { message = "Error 404" });

        //     cita.FechaInicio = fechaInicio;
        //     cita.FechaFinal = fechaFinal;

        //     _citasRepository.Update(cita);
        //     return Json(new object());
        // }

        //public IActionResult Lista(string sortOrder, string buscar, string currentFilter, int? pageNumber, int? sucursalId, int? empleadoId, int? especialidadId)
        //{
        //    if (buscar != null)
        //    {
        //        pageNumber = 1;
        //    }
        //    else
        //    {
        //        buscar = currentFilter;
        //    }
        //    if (sucursalId != null) { }

        //    ViewData["CurrentFilter"] = buscar;

        //    var lista = _citasRepository.PaginacionCitas(sortOrder, buscar, pageNumber, 35, empleadoId);

        //    return View(lista);
        //}

        public IActionResult Lista(string buscar, int? sucursalId, int? empleadoId, int? especialidadId/*, string currentFilter*/)
        {
            var b = "";

            if (buscar == null)
            {
                b = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                b = Convert.ToDateTime(buscar).ToString("yyyy-MM-dd");
            }

            var fechaConvertida = Convert.ToDateTime(b);
            var citaPorFecha = _citasRepository.CitasListaPorFecha(fechaConvertida);
            if (sucursalId != null)
            {
                citaPorFecha = citaPorFecha
                    .Where(a => a.SucursalId != null && a.SucursalId == sucursalId)
                    .ToList();
            }
            if (empleadoId != null)
            {
                citaPorFecha = citaPorFecha
                    .Where(a => a.EmpleadoId != null && a.EmpleadoId == empleadoId)
                    .ToList();
            }
            if (especialidadId != null)
            {
                citaPorFecha = citaPorFecha
                    .Where(a => a.EspecialidadId != null && a.EspecialidadId == especialidadId)
                    .ToList();
            }

            ViewData["currentFilter"] = b;

            var model = new CalendarioLinealViewModel()
            {
                Fecha = b,
                Citas = citaPorFecha,
                FechaBloqueada = false
            };

            var fechaBloqueada = _citasRepository.GetFechaBloqueada(fechaConvertida);
            if (fechaBloqueada != null)
            {
                model.FechaBloqueada = true;
            }

            model.Init(_sucursalRepository, _empleadoRepository, _citasRepository);

            return View(model);
        }

        public IActionResult NoAsistidas(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _citasRepository.PaginacionCitasNoAsistidas(sortOrder, buscar, pageNumber, 35);

            return View(lista);
        }
        public IActionResult Turnos(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _citasRepository.PaginacionTurnos(sortOrder, buscar, pageNumber, 35);

            return View(lista);
        }
        public IActionResult CitasFinalizadas(string sortOrder, string buscar, string currentFilter, int? pageNumber)
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

            var lista = _citasRepository.PaginacionCitasFinalizadas(sortOrder, buscar, pageNumber, 35);

            return View(lista);
        }
        public JsonResult FinalizarCita(int? id, DateTime fecha)
        {
            if (id == null) return new JsonErrorResult(new { message = "Error 400" });

            var cita = _citasRepository.GetCita((int)id);

            if (cita == null) return new JsonErrorResult(new { message = "Error 404" });

            cita.Finalizada = true;
            cita.EstadoCita = "asistida";

            _citasRepository.Update(cita);

            return Json(fecha.ToString("MM/dd/yyyy"));
        }
        [HttpPost]
        public string ConsultarServiciosAgregadosCita(int citaId)
        {
            try
            {
                var serviciosAgregados = new List<CitaServicioAgregadoViewModel>();
                var serviciosBd = _citasRepository.GetServiciosCita(citaId);
                if (serviciosBd != null)
                {
                    foreach (var servicio in serviciosBd)
                    {
                        var servicioAgregado = new CitaServicioAgregadoViewModel
                        {
                            Id = servicio.Id,
                            ServicioId = servicio.ServicioId,
                            ServicioDuracionHoras = servicio.Servicio != null
                            ? servicio.Servicio.DuracionHoras ?? 0
                            : 0,
                            Cantidad = servicio.Cantidad ?? 1,
                            ServicioNombre = servicio.Servicio.NombreServicio,
                            ServicioDuracionMinutos = servicio.Servicio != null
                            ? servicio.Servicio.DuracionMinutos ?? 15
                            : 15,
                            Nuevo = false,
                            PrecioValor = servicio.PrecioValor ?? 0,
                            PrecioValorCubiertoSeguro = servicio.PrecioValorCubiertoSeguro,
                            PrecioValorCopago = servicio.PrecioValorCopago,
                            PrecioNombre = servicio.Precio != null ? servicio.Precio.NombrePrecio : "N/A"
                        };
                        serviciosAgregados.Add(servicioAgregado);
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = serviciosAgregados
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios agregados. " + ex.InnerException.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarExamenesAgregadosCita(int citaId)//Consutltamos los examenes agregados en una cita agendada en calendario
        {
            try
            {
                var examenesAgregados = new List<CitaExamenesAgregadosViewModel>();
                var ExamenesLabAgregadosCitaBd = _citasRepository.GetCita(citaId)
                    .CitasExamenes
                    .ToList();
                if (ExamenesLabAgregadosCitaBd != null)
                {
                    foreach (var examen in ExamenesLabAgregadosCitaBd)
                    {
                        var examenAgregado = new CitaExamenesAgregadosViewModel
                        {

                            ExamenId = examen.ExamenLabClinico.Id,
                            ExamenNombre = examen.ExamenLabClinico.NombreExamen,
                            Cantidad = 1,
                            PrecioId = (int)examen.PrecioId,
                            PrecioValor = (int)(examen.PrecioValor ?? 0),
                            PrecioValorCubiertoSeguro = examen.PrecioValorCubiertoSeguro,
                            PrecioValorCopago = examen.PrecioValorCopago,
                            Nuevo = false
                        };
                        examenesAgregados.Add(examenAgregado);
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = examenesAgregados
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios agregados. " + ex.InnerException.Message
                });
            }
        }
        [HttpGet]
        public JsonResult GetDatosCita(int citaId)
        {
            try
            {
                var cita = _citasRepository.GetCita(citaId);
                if (cita == null)
                    return Json(new { exitoso = false, mensaje = "Cita no encontrada" });

                var cirujano = "";
                if (cita.EmpleadoId.HasValue)
                {
                    var empleado = _empleadoRepository.Get(cita.EmpleadoId.Value);
                    if (empleado != null)
                        cirujano = empleado.NombreYApellidos;
                }

                return Json(new
                {
                    exitoso = true,
                    procedimiento = cita.Procedimiento ?? "",
                    cirujano = cirujano,
                    cirujanoNombre = cirujano,
                    cirujanoId = cita.EmpleadoId,
                    primerAyudante = cita.PrimerAyudante ?? "",
                    segundoAyudante = cita.SegundoAyudante ?? "",
                    anestesista = cita.Anestesista ?? "",
                    instrumentista = cita.Instrumentista ?? "",
                    circulante = cita.Circulante ?? ""
                });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetEmpleadosPorTipo(
            string especialidadNombre = null,
            string unidadNombre = null,
            string especialidadId = null,
            string unidadOrgId = null)
        {
            try
            {
                IEnumerable<Empleado> empleadosQuery = _empleadoRepository.GetEmpleadosConDetalles();

                if (!string.IsNullOrWhiteSpace(especialidadId) && int.TryParse(especialidadId, out var espId))
                {
                    empleadosQuery = empleadosQuery.Where(e => e.EspecialidadId == espId);
                }
                else if (!string.IsNullOrWhiteSpace(especialidadNombre))
                {
                    string busqueda = especialidadNombre.Trim().ToLower();

                    empleadosQuery = empleadosQuery.Where(e =>
                        e.Especialidad != null &&
                        e.Especialidad.NombreEspecialidad != null &&
                        e.Especialidad.NombreEspecialidad.ToLower().Contains(busqueda));
                }

                if (!string.IsNullOrWhiteSpace(unidadOrgId) && int.TryParse(unidadOrgId, out var uniId))
                {
                    empleadosQuery = empleadosQuery.Where(e => e.UnidadOrgId == uniId);
                }
                else if (!string.IsNullOrWhiteSpace(unidadNombre))
                {
                    string busquedaUnidad = unidadNombre.Trim().ToLower();

                    empleadosQuery = empleadosQuery.Where(e =>
                        e.UnidadOrg != null &&
                        e.UnidadOrg.DepartamentoOrg != null &&
                        e.UnidadOrg.DepartamentoOrg.Unidades.Any(u =>
                            u.Nombre != null &&
                            u.Nombre.ToLower().Contains(busquedaUnidad))
                    );
                }

                var resultado = empleadosQuery
                    .Where(e => !string.IsNullOrWhiteSpace(e.NombreYApellidos))
                    .OrderBy(e => e.Nombre)
                    .Select(e => new
                    {
                        id = e.Id,
                        nombre = e.NombreYApellidos,
                        especialidadDetalle = e.Especialidad != null ? e.Especialidad.NombreEspecialidad : "",
                        unidadDetalle = e.UnidadOrg != null ? e.UnidadOrg.Nombre : ""
                    })
                    .ToList();

                return Json(resultado);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error interno", mensaje = ex.Message });
            }
        }


        [HttpPost]
        public string ConsultarServiciosExistentes()
        {
            try
            {
                var serviciosExistentes = new List<CitaServicioExistenteViewModel>();
                var serviciosBd = _servicioRepository.GetListaServicios();
                if (serviciosBd != null)
                {
                    foreach (var servicio in serviciosBd)
                    {
                        var servicioExistente = new CitaServicioExistenteViewModel
                        {
                            ServicioId = servicio.Id,
                            ServicioCodigo = servicio.CodigoInterno,
                            ServicioNombre = servicio.NombreServicio,
                            ServicioDuracionHoras = servicio.DuracionHoras ?? 0,
                            ServicioDuracionMinutos = servicio.DuracionMinutos ?? 0
                        };
                        serviciosExistentes.Add(servicioExistente);
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = serviciosExistentes
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios. " + (ex.InnerException?.Message ?? ex.Message)
                });
            }
        }
        [HttpPost]
        public string ConsultarPreciosServicio(int servicioId)
        {
            try
            {
                // Obtener los precios del servicio desde el repositorio
                var preciosServicio = _servicioRepository
                    .GetPreciosServicio(servicioId)
                    .OrderBy(a => a.PrecioId)
                    .Select(precio => new CitaServicioExistentePrecioViewModel
                    {
                        PrecioId = precio.PrecioId,
                        PrecioNombre = precio.Precio.NombrePrecio,
                        PrecioNombreValor = precio.Precio.NombrePrecio + " - " + precio.Valor,
                        PrecioValor = precio.Valor
                    })
                    .ToList();

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
        public string ConsultarDepartamentosExistentes()
        {
            try
            {
                var departamentosExistentes = new List<DepartamentosViewModel>();
                var departamentosBd = _pacienteRepository.GetListDepartamentos();
                if (departamentosBd != null)
                {
                    foreach (var departamento in departamentosBd)
                    {
                        var departamentoExistente = new DepartamentosViewModel
                        {
                            Id = departamento.Id,
                            NombreDepartamento = departamento.NombreDepartamento,

                        };
                        departamentosExistentes.Add(departamentoExistente);
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = departamentosExistentes
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios. " + ex.InnerException.Message
                });
            }
        }
        public string ConsultarMunicipiosExistentes(int departamentoId)
        {
            try
            {
                var municipiosExistentes = new List<MunicipiosViewModel>();
                var municipiosBd = _pacienteRepository.GetListMunicipios(departamentoId);
                if (municipiosBd != null)
                {
                    foreach (var municipio in municipiosBd)
                    {
                        var municipioExistente = new MunicipiosViewModel
                        {
                            Id = municipio.Id,
                            NombreMunicipio = municipio.NombreMunicipio,
                            DepartamentoId = municipio.DepartamentoId

                        };
                        municipiosExistentes.Add(municipioExistente);
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = municipiosExistentes
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar servicios. " + ex.InnerException.Message
                });
            }
        }

        //En proceso de ser eliminado ya que se ha optimizado el codigo en Agendar cita
        //[HttpPost]
        //public string ConsultarDepartamentoMunicipoPaciente(int citaId)
        //{
        //    try
        //    {
        //        var examenesAgregados = new List<CitaExamenesAgregadosViewModel>();
        //        var departamentoMunicipoPacienteBd = _citasRepository.GetCita(citaId)
        //            .Paciente;
        //        var departamento = new DepartamentosViewModel
        //        {

        //            Id = departamentoMunicipoPacienteBd.DepartamentoId,
        //            NombreDepartamento = departamentoMunicipoPacienteBd.Departamento.NombreDepartamento
        //        };
        //        var municipio = new MunicipiosViewModel
        //        {
        //            //Id = departamentoMunicipoPacienteBd.MunicipioId,
        //            //DepartamentoId = departamentoMunicipoPacienteBd.Municipio.DepartamentoId,
        //            //NombreMunicipio = departamentoMunicipoPacienteBd.Municipio.NombreMunicipio
        //            Id = departamentoMunicipoPacienteBd.Municipio?.Id ?? 0, // Asigna 0 si Municipio es nulo
        //            DepartamentoId = departamentoMunicipoPacienteBd.Municipio?.DepartamentoId ?? 0, // Asigna 0 si Municipio es nulo
        //            NombreMunicipio = departamentoMunicipoPacienteBd.Municipio?.NombreMunicipio // Asigna null si Municipio es nulo

        //        };





        //        return JsonSerializer.Serialize(new
        //        {
        //            Exitoso = true,
        //            Departamento = departamento,
        //            Municipio = municipio
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonSerializer.Serialize(new
        //        {
        //            Exitoso = false,
        //            Mensaje = "Error de servidor al consultar servicios agregados. " + ex.InnerException.Message
        //        });
        //    }
        //}
        public IActionResult GetTurnoEspecialidad(int id)
        {
            var turno = _citasRepository.GetTurnoEspecialidadCita(id);
            var noTurno = turno.Count() + 1;
            var Especialidad = _citasRepository.GetEspecilidad(id);
            var CodigoEspecialidad = "";
            if (Especialidad != null)
            {
                CodigoEspecialidad = Especialidad.Codigo ?? "";
            }
            if (turno != null)
            {
                return Json(new { existe = true, turnoactual = noTurno, codEspecialidad = CodigoEspecialidad });
            }
            else
            {
                return Json(new { existe = false });
            }
        }
        public IActionResult AgendarCita(string fecha)
        {
            var model = new CitaViewModel()
            {
                FechaHora = Convert.ToDateTime(fecha)
            };

            model.Init(_citasRepository, _pacienteRepository, _empleadoRepository, _servicioRepository, _sucursalRepository);
            return View(model);
        }

        // public IActionResult AgendarCitaHospi(string fecha)
        // {
        //     var fechaHora = DateTime.ParseExact(fecha, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
        //     fechaHora = DateTime.SpecifyKind(fechaHora, DateTimeKind.Unspecified);

        //     var model = new CitaViewModel { FechaHora = fechaHora };
        //     model.Init(_citasRepository, _pacienteRepository, _empleadoRepository, _servicioRepository, _sucursalRepository);
        //     return View(model);
        // }

        public IActionResult AgendarCitaHospi(string fecha, string modo = "admision")
        {
            var model = new CitaViewModel()
            {
                FechaHora = Convert.ToDateTime(fecha)
            };

            // "calendario" = viene del CalendarioLineal con modo=hospitalizacion
            // "admision"   = viene de admisiones directas
            ViewData["OrigenFormulario"] = modo == "hospitalizacion" ? "calendario" : "admision";

            model.Init(_citasRepository, _pacienteRepository, _empleadoRepository, _servicioRepository, _sucursalRepository);
            return View(model);
        }


        [HttpPost]
        public async Task<string> AgendarCitaHospi(CitaViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var esMenor = model.PacienteEdad != null && model.PacienteEdad < 18 ? true : false;

                string dpiAsignado = string.IsNullOrEmpty(model.dpiPacienteSeleccionado)
                    ? _citasRepository.ObtenerSiguienteDpiFicticio()
                    : model.dpiPacienteSeleccionado;

                Paciente paciente;
                if (model.PacienteId == null)
                {
                    // Si el PacienteId es nulo, se crea un nuevo paciente
                    paciente = new Paciente
                    {
                        Nombre = model.PacienteNombre,
                        EstadoPacienteId = (int)EstadoPacienteEnum.Activo,
                        TipoPacienteId = (int)TipoPacienteEnum.Nuevo,
                        SexoId = model.SexoId,
                        Edad = model.PacienteEdad,
                        Dpi = dpiAsignado,
                        FechaNacimiento = model.FechaNacimiento,
                        FechaRegistro = model.FechaHora,
                        Telefono = model.Telefono,
                        Email = model.Email,
                        IgssNumeroAfiliacion = model.no_IGGS,
                        EtniaPaciente = model.EtniaPaciente,
                        OrigenPaciente = model.OrigenPaciente,
                        ReligionPaciente = model.ReligionPaciente,
                        DepartamentoId = model.DepartamentoId,
                        MunicipioId = model.MunicipioId,
                        Direccion = model.Direccion,
                        NombreEncargado = model.NombreEncargado,
                        DPIEncargado = model.DPIEncargado,

                        // 🔽 Nuevos campos
                        ResponsableNit = model.ResponsableNit,
                        ResponsableNombre = model.ResponsableNombre,
                        ResponsableDireccion = model.ResponsableDireccion,
                        ResponsableCorreo = model.ResponsableCorreo,
                        ResponsableTelefono = model.ResponsableTelefono,
                        ResponsableDPI = model.ResponsableDPI,
                        ResponsablePasaporte = model.ResponsablePasaporte,
                        // 🔼 Nuevos campos
                        ResponsableNacionalidad = model.ResponsableNacionalidad,
                        ResponsableOcupacion = model.ResponsableOcupacion,

                        NombrePadre = model.NombrePadre,
                        FechaNacimientoPadre = model.FechaNacimientoPadre,
                        EdadPadre = model.EdadPadre,
                        DPIPadre = model.DPIPadre,
                        DireccionPadre = model.DireccionPadre,
                        TelefonoPadre = model.TelefonoPadre,
                        CorreoPadre = model.CorreoPadre,
                        OcupacionPadre = model.OcupacionPadre,
                        EmpresaPadre = model.EmpresaPadre,
                        TelefonoEmpresaPadre = model.TelefonoEmpresaPadre,
                        DireccionEmpresaPadre = model.DireccionEmpresaPadre,

                        NombreMadre = model.NombreMadre,
                        FechaNacimientoMadre = model.FechaNacimientoMadre,
                        EdadMadre = model.EdadMadre,
                        DPIMadre = model.DPIMadre,
                        DireccionMadre = model.DireccionMadre,
                        TelefonoMadre = model.TelefonoMadre,
                        CorreoMadre = model.CorreoMadre,
                        OcupacionMadre = model.OcupacionMadre,
                        EmpresaMadre = model.EmpresaMadre,
                        TelefonoEmpresaMadre = model.TelefonoEmpresaMadre,
                        DireccionEmpresaMadre = model.DireccionEmpresaMadre,

                        AcompananteNombre = model.AcompananteNombre,
                        AcompananteRelacion = model.AcompananteRelacion,
                        AcompananteTelefono = model.AcompananteTelefono,
                        AcompananteDPI = model.AcompananteDPI,
                        AcompananteDireccion = model.AcompananteDireccion,
                        AcompananteCorreo = model.AcompananteCorreo,
                        AcompananteOcupacion = model.AcompananteOcupacion,
                        AcompananteEmpresa = model.AcompananteEmpresa,
                        AcompananteTelefonoEmpresa = model.AcompananteTelefonoEmpresa,
                        AcompananteDireccionEmpresa = model.AcompananteDireccionEmpresa,
                        AcompananteTipoIdentificacion = model.AcompananteTipoIdentificacion,
                        AcompananteFechaNacimiento = model.AcompananteFechaNacimiento,
                        AcompananteEdad = model.AcompananteEdad,
                        AcompananteFechaIngreso = model.AcompananteFechaIngreso,
                        AcompananteAntiguedad = model.AcompananteAntiguedad
                    };

                    _pacienteRepository.Add(paciente); // Guardar el nuevo paciente en la base de datos
                }
                else
                {
                    // Si el PacienteId no es nulo, se actualiza el paciente existente
                    paciente = _pacienteRepository.Get((int)model.PacienteId);
                    if (paciente != null)
                    {
                        paciente.Dpi = model.dpiPacienteSeleccionado;
                        paciente.Telefono = model.Telefono;
                        paciente.Email = model.Email;
                        paciente.FechaNacimiento = model.FechaNacimiento;
                        paciente.Edad = model.PacienteEdad;
                        paciente.Direccion = model.Direccion;
                        paciente.DepartamentoId = model.DepartamentoId;
                        paciente.MunicipioId = model.MunicipioId;
                        paciente.OrigenPaciente = model.OrigenPaciente;
                        paciente.ReligionPaciente = model.ReligionPaciente;
                        paciente.EtniaPaciente = model.EtniaPaciente;
                        paciente.SexoId = model.SexoId;
                        paciente.Nombre = model.PacienteNombre;
                        paciente.NombreEncargado = model.NombreEncargado;
                        paciente.DPIEncargado = model.DPIEncargado;

                        // Nuevos campos
                        paciente.ResponsableNit = model.ResponsableNit;
                        paciente.ResponsableNombre = model.ResponsableNombre;
                        paciente.ResponsableDireccion = model.ResponsableDireccion;
                        paciente.ResponsableCorreo = model.ResponsableCorreo;
                        paciente.ResponsableTelefono = model.ResponsableTelefono;
                        paciente.ResponsableDPI = model.ResponsableDPI;
                        paciente.ResponsablePasaporte = model.ResponsablePasaporte;
                        // 🔼 Nuevos campos
                        paciente.ResponsableNacionalidad = model.ResponsableNacionalidad;
                        paciente.ResponsableOcupacion = model.ResponsableOcupacion;

                        paciente.NombrePadre = model.NombrePadre;
                        paciente.FechaNacimientoPadre = model.FechaNacimientoPadre;
                        paciente.EdadPadre = model.EdadPadre;
                        paciente.DPIPadre = model.DPIPadre;
                        paciente.DireccionPadre = model.DireccionPadre;
                        paciente.TelefonoPadre = model.TelefonoPadre;
                        paciente.CorreoPadre = model.CorreoPadre;
                        paciente.OcupacionPadre = model.OcupacionPadre;
                        paciente.EmpresaPadre = model.EmpresaPadre;
                        paciente.TelefonoEmpresaPadre = model.TelefonoEmpresaPadre;
                        paciente.DireccionEmpresaPadre = model.DireccionEmpresaPadre;

                        paciente.NombreMadre = model.NombreMadre;
                        paciente.FechaNacimientoMadre = model.FechaNacimientoMadre;
                        paciente.EdadMadre = model.EdadMadre;
                        paciente.DPIMadre = model.DPIMadre;
                        paciente.DireccionMadre = model.DireccionMadre;
                        paciente.TelefonoMadre = model.TelefonoMadre;
                        paciente.CorreoMadre = model.CorreoMadre;
                        paciente.OcupacionMadre = model.OcupacionMadre;
                        paciente.EmpresaMadre = model.EmpresaMadre;
                        paciente.TelefonoEmpresaMadre = model.TelefonoEmpresaMadre;
                        paciente.DireccionEmpresaMadre = model.DireccionEmpresaMadre;

                        paciente.AcompananteNombre = model.AcompananteNombre;
                        paciente.AcompananteRelacion = model.AcompananteRelacion;
                        paciente.AcompananteTelefono = model.AcompananteTelefono;
                        paciente.AcompananteDPI = model.AcompananteDPI;
                        paciente.AcompananteDireccion = model.AcompananteDireccion;
                        paciente.AcompananteCorreo = model.AcompananteCorreo;
                        paciente.AcompananteOcupacion = model.AcompananteOcupacion;
                        paciente.AcompananteEmpresa = model.AcompananteEmpresa;
                        paciente.AcompananteTelefonoEmpresa = model.AcompananteTelefonoEmpresa;
                        paciente.AcompananteDireccionEmpresa = model.AcompananteDireccionEmpresa;
                        paciente.AcompananteTipoIdentificacion = model.AcompananteTipoIdentificacion;
                        paciente.AcompananteFechaNacimiento = model.AcompananteFechaNacimiento;
                        paciente.AcompananteEdad = model.AcompananteEdad;
                        paciente.AcompananteFechaIngreso = model.AcompananteFechaIngreso;
                        paciente.AcompananteAntiguedad = model.AcompananteAntiguedad;

                        _pacienteRepository.Update(paciente);
                    }
                }

                // ✅ Normalizar duración UNA sola vez y reutilizar en validación y guardado
                var inicioConflicto = model.FechaHora;
                var totalMinutos = (model.DuracionTotalHoras * 60) + model.DuracionTotalMinutos;
                if (totalMinutos <= 0) totalMinutos = 1;
                var finConflicto = model.FechaHora.AddMinutes(totalMinutos);

                // Validación obligatoria de conflicto de agenda (EmpleadoId + rango)
                if (model.EmpleadoId.HasValue)
                {
                    var hayConflicto = _citasRepository.ExisteConflictoEmpleado(
                        model.EmpleadoId.Value,
                        inicioConflicto,
                        finConflicto,
                        model.CitaId);

                    if (hayConflicto)
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = "El profesional seleccionado ya tiene una cita asignada para la fecha y hora seleccionada. Por favor seleccione otro médico u elija otra fecha y horario."
                        });
                    }
                }

                // Crear la cita asociada al paciente
                var nuevaCita = new Citas()
                {
                    EspecialidadId = model.EspecialidadId,
                    HabitacionId = model.HabitacionId,
                    CategoriaHabitacionId = model.CategoriaHabitacionId,

                    Paciente = paciente,
                    SucursalId = model.SucursalId,
                    Bloqueada = false,
                    Eliminado = false,
                    Finalizada = false,
                    CitaTipoAtencion = model.CitaTipoAtencion,
                    EmpleadoId = model.EmpleadoId, // Médico asignado
                    User = user,
                    FechaInicio = model.FechaHora,
                    FechaFinal = finConflicto,
                    Motivo = model.Motivo,
                    Edad = model.PacienteEdad,
                    NombreEncargado = model.NombreEncargado,
                    DPIEncargado = model.DPIEncargado,
                    EstadoCita = "normal",
                    EsMenorDeEdad = esMenor,
                    CodigoAutorizacion = model.CodigoAutorizacion,
                    CodigoDeCita = model.CodigoCita ?? "SIN SEGURO",
                    EstadoTurno = model.NumeroTurno != null ? "ACTIVO" : null,
                    FechaHoraInicioTurno = model.NumeroTurno != null ? model.FechaHora : (DateTime?)null,
                    NumeroTurno = model.NumeroTurno,
                    NivelPrioridadCita = model.NivelPrioridadCita,

                    ResponsableNit = model.ResponsableNit,
                    ResponsableNombre = model.ResponsableNombre,
                    ResponsableDireccion = model.ResponsableDireccion,
                    ResponsableCorreo = model.ResponsableCorreo,
                    ResponsableTelefono = model.ResponsableTelefono,
                    ResponsablePasaporte = model.ResponsablePasaporte,
                    ResponsableDPI = model.ResponsableDPI,
                    ResponsableNacionalidad = model.ResponsableNacionalidad,
                    ResponsableOcupacion = model.ResponsableOcupacion,

                    NombrePadre = model.NombrePadre,
                    FechaNacimientoPadre = model.FechaNacimientoPadre,
                    EdadPadre = model.EdadPadre,
                    DPIPadre = model.DPIPadre,
                    DireccionPadre = model.DireccionPadre,
                    TelefonoPadre = model.TelefonoPadre,
                    CorreoPadre = model.CorreoPadre,
                    OcupacionPadre = model.OcupacionPadre,
                    EmpresaPadre = model.EmpresaPadre,
                    TelefonoEmpresaPadre = model.TelefonoEmpresaPadre,
                    DireccionEmpresaPadre = model.DireccionEmpresaPadre,

                    NombreMadre = model.NombreMadre,
                    FechaNacimientoMadre = model.FechaNacimientoMadre,
                    EdadMadre = model.EdadMadre,
                    DPIMadre = model.DPIMadre,
                    DireccionMadre = model.DireccionMadre,
                    TelefonoMadre = model.TelefonoMadre,
                    CorreoMadre = model.CorreoMadre,
                    OcupacionMadre = model.OcupacionMadre,
                    EmpresaMadre = model.EmpresaMadre,
                    TelefonoEmpresaMadre = model.TelefonoEmpresaMadre,
                    DireccionEmpresaMadre = model.DireccionEmpresaMadre,

                    AcompananteNombre = model.AcompananteNombre,
                    AcompananteRelacion = model.AcompananteRelacion,
                    AcompananteTelefono = model.AcompananteTelefono,
                    AcompananteDPI = model.AcompananteDPI,
                    AcompananteDireccion = model.AcompananteDireccion,
                    AcompananteCorreo = model.AcompananteCorreo,
                    AcompananteOcupacion = model.AcompananteOcupacion,
                    AcompananteEmpresa = model.AcompananteEmpresa,
                    AcompananteTelefonoEmpresa = model.AcompananteTelefonoEmpresa,
                    AcompananteDireccionEmpresa = model.AcompananteDireccionEmpresa,
                    AcompananteTipoIdentificacion = model.AcompananteTipoIdentificacion,
                    AcompananteFechaNacimiento = model.AcompananteFechaNacimiento,
                    AcompananteEdad = model.AcompananteEdad,
                    AcompananteFechaIngreso = model.AcompananteFechaIngreso,
                    AcompananteAntiguedad = model.AcompananteAntiguedad,

                    // Campos de Sala de Operaciones (solo calendario hospitalización)
                    Anestesista = model.Anestesista,
                    PrimerAyudante = model.PrimerAyudante,
                    SegundoAyudante = model.SegundoAyudante,
                    Instrumentista = model.Instrumentista,
                    Circulante = model.Circulante,
                    Procedimiento = model.Procedimiento,

                    AnestesistaId = model.AnestesistaId,
                    PrimerAyudanteId = model.PrimerAyudanteId,
                    SegundoAyudanteId = model.SegundoAyudanteId,
                    InstrumentistaId = model.InstrumentistaId,
                    CirculanteId = model.CirculanteId
                };

                // Añadir servicios a la cita
                if (model.Servicios != null)
                {
                    foreach (var servicio in model.Servicios)
                    {
                        if (servicio.Nuevo)
                        {
                            nuevaCita.CitasServicios.Add(new CitasServicio
                            {
                                ServicioId = servicio.ServicioId,
                                PrecioId = servicio.PrecioId,
                                PrecioValor = servicio.PrecioValor,
                                PrecioValorCopago = servicio.PrecioValorCopago,
                                PrecioValorCubiertoSeguro = servicio.PrecioValorCubiertoSeguro
                            });
                        }
                    }
                }

                // Añadir exámenes a la cita
                if (model.Examenes != null)
                {
                    if (nuevaCita.CitasExamenes == null)
                    {
                        nuevaCita.CitasExamenes = new List<CitasExamenes>();
                    }
                    foreach (var examen in model.Examenes)
                    {
                        if (examen.Nuevo)
                        {
                            nuevaCita.CitasExamenes.Add(new CitasExamenes
                            {
                                ExamenLabClinicoId = examen.ExamenId,
                                PrecioId = examen.PrecioId,
                                PrecioValor = examen.PrecioValor,
                                PrecioValorCopago = examen.PrecioValorCopago,
                                PrecioValorCubiertoSeguro = examen.PrecioValorCubiertoSeguro
                            });
                        }
                    }
                }

                _citasRepository.Add(nuevaCita);

                if (!string.IsNullOrWhiteSpace(paciente.Telefono))
                {
                    var medicoNombre = model.EmpleadoId.HasValue
                        ? _empleadoRepository.Get(model.EmpleadoId.Value, false)?.NombreYApellidos ?? "su médico"
                        : "su médico";
                    var msgCita = $"Confirmación de cita (Hospitalización)\nPaciente: {paciente.Nombre}\nFecha: {model.FechaHora:dd/MM/yyyy HH:mm}\nMédico: {medicoNombre}";
                    await _whatsAppService.SendTextMessageAsync(paciente.Telefono, msgCita);
                }

                TempData["Message"] = "¡La cita se ha agendado con éxito!";

                // ✅ Return exitoso para cubrir todas las rutas (evita CS0161)
                // return JsonSerializer.Serialize(new
                // {
                //     Exitoso = true,
                //     CitaId = nuevaCita.Id,
                //     Fecha = model.FechaHora.ToString("yyyy/MM/dd")
                // });
                bool esSalaOperaciones = model.CategoriaHabitacionId == 13;

                string redirectUrl = null;
                if (esSalaOperaciones)
                {
                    redirectUrl = Url.Action("CalendarioLineal", "Cita", new
                    {
                        buscar = model.FechaHora.ToString("yyyy-MM-dd"),
                        modo = "hospitalizacion"
                    });
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    CitaId = nuevaCita.Id,
                    Fecha = model.FechaHora.ToString("yyyy/MM/dd"),
                    EsSalaOperaciones = esSalaOperaciones,
                    RedirectUrl = redirectUrl
                });

            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor. " + ex.InnerException?.Message ?? ex.Message
                });
            }
        }



        // [HttpGet]
        // public IActionResult GetHabitacionesByCategoria(int categoriaId)
        // {
        //     var habitaciones = _citasRepository.GetHabitacionesList(categoriaId); // Suponiendo que tienes un repositorio para obtener las habitaciones
        //     var result = habitaciones.Select(h => new { h.Id, h.NombreNumeroHabitacion }).ToList(); // Devuelves el ID y Nombre de la habitación
        //     return Json(result);
        // }


        [HttpGet]
        public IActionResult GetHabitacionesByCategoria(int categoriaId, DateTime fechaInicio, DateTime fechaFin, int? citaId = null)
        {
            //Obtener todas las habitaciones de la categoría (ignorando estado físico)
            var habitaciones = _habitacionRepository.GetHabitacionesPorCategoriaParaAgenda(categoriaId);

            // Obtener los IDs de habitaciones que ya tienen una cita en el rango (excluyendo la actual si es edición)
            var citasEnRango = _citasRepository.GetAll()
                .Where(c => c.HabitacionId != null
                            && !c.Eliminado
                            && c.EstadoCita != "cancelada"
                            && c.FechaInicio < fechaFin
                            && c.FechaFinal > fechaInicio
                            && c.Id != citaId)
                .Select(c => c.HabitacionId.Value)
                .Distinct()
                .ToList();

            // Filtrar las habitaciones que NO están en la lista de ocupadas
            var disponibles = habitaciones
                .Where(h => !citasEnRango.Contains(h.Id))
                .Select(h => new { h.Id, h.NombreNumeroHabitacion })
                .ToList();

            return Json(disponibles);
        }


        [HttpPost]
        public string ConsultarSeguros()
        {
            try
            {
                // Console.WriteLine("Antes del .GetAll()");
                // Obtener todos los seguros utilizando el repositorio
                var seguros = _seguroRepository.GetAll();
                // Console.WriteLine("CitasVM - Seguros Count: " + seguros.Count());
                // Proyectar solo el ID y el nombre de cada seguro
                var result = seguros.Select(x => new
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                });

                // Retornar la respuesta en formato JSON
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = result
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar seguros. " + ex.Message
                });
            }
        }

        int? CalcularEdad(DateTime? fechaNacimiento)
        {
            if (fechaNacimiento == null)
                return null;

            var today = DateTime.Today;
            var edad = today.Year - fechaNacimiento.Value.Year;

            if (fechaNacimiento > today.AddYears(-edad))
                edad--;

            return edad;
        }

        [HttpPost]
        public async Task<string> AgendarCita(CitaViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var esMenor = model.PacienteEdad != null && model.PacienteEdad < 18 ? true : false;

                Paciente paciente;
                if (model.PacienteId == null)
                {
                    // Si el PacienteId es nulo, se crea un nuevo paciente
                    paciente = new Paciente
                    {
                        Nombre = model.PacienteNombre,
                        EstadoPacienteId = (int)EstadoPacienteEnum.Activo,
                        TipoPacienteId = (int)TipoPacienteEnum.Nuevo,
                        SexoId = model.SexoId,
                        Edad = CalcularEdad(model.FechaNacimiento),
                        Dpi = model.dpiPacienteSeleccionado,
                        FechaNacimiento = model.FechaNacimiento,
                        FechaRegistro = model.FechaHora,
                        Telefono = model.Telefono,
                        Email = model.Email,
                        IgssNumeroAfiliacion = model.no_IGGS,
                        EtniaPaciente = model.EtniaPaciente,
                        OrigenPaciente = model.OrigenPaciente,
                        DepartamentoId = model.DepartamentoId,
                        MunicipioId = model.MunicipioId,
                        Direccion = model.Direccion,
                        NombreEncargado = model.NombreEncargado,
                        DPIEncargado = model.DPIEncargado
                    };

                    _pacienteRepository.Add(paciente); // Guardar el nuevo paciente en la base de datos
                }
                else
                {
                    // Si el PacienteId no es nulo, se actualiza el paciente existente
                    paciente = _pacienteRepository.Get((int)model.PacienteId);
                    if (paciente != null)
                    {
                        paciente.Dpi = model.dpiPacienteSeleccionado;
                        paciente.Telefono = model.Telefono;
                        paciente.Email = model.Email;
                        paciente.FechaNacimiento = model.FechaNacimiento;
                        paciente.Edad = CalcularEdad(model.FechaNacimiento);  // Calcula la edad aquí
                        paciente.Direccion = model.Direccion;
                        paciente.DepartamentoId = model.DepartamentoId;
                        paciente.MunicipioId = model.MunicipioId;
                        paciente.OrigenPaciente = model.OrigenPaciente;
                        paciente.EtniaPaciente = model.EtniaPaciente;
                        paciente.SexoId = model.SexoId;
                        paciente.Nombre = model.PacienteNombre;
                        paciente.NombreEncargado = model.NombreEncargado;
                        paciente.DPIEncargado = model.DPIEncargado;

                        _pacienteRepository.Update(paciente); // Actualizar el paciente en la base de datos
                    }
                }

                // ✅ Normalizar duración UNA sola vez y reutilizar en validación y guardado
                var inicioConflicto = model.FechaHora;
                var totalMinutos = (model.DuracionTotalHoras * 60) + model.DuracionTotalMinutos;
                if (totalMinutos <= 0) totalMinutos = 1;
                var finConflicto = model.FechaHora.AddMinutes(totalMinutos);

                // Validación obligatoria de conflicto de agenda (EmpleadoId + rango)
                if (model.EmpleadoId.HasValue)
                {
                    var hayConflicto = _citasRepository.ExisteConflictoEmpleado(
                        model.EmpleadoId.Value,
                        inicioConflicto,
                        finConflicto,
                        model.CitaId);

                    if (hayConflicto)
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = "El profesional seleccionado ya tiene una cita asignada para la fecha y hora seleccionada. Por favor seleccione otro médico u elija otra fecha y horario."
                        });
                    }
                }

                // Crear la cita asociada al paciente
                var nuevaCita = new Citas()
                {
                    EspecialidadId = model.EspecialidadId,
                    Paciente = paciente,
                    SucursalId = model.SucursalId,
                    Bloqueada = false,
                    Eliminado = false,
                    Finalizada = false,
                    CitaTipoAtencion = model.CitaTipoAtencion,
                    EmpleadoId = model.EmpleadoId, // Médico asignado
                    User = user,
                    FechaInicio = model.FechaHora,
                    FechaFinal = finConflicto, // ✅ antes era AddMinutes(model.Duracion...), ahora usa el mismo fin validado
                    ContadorCitaAgendada = model.FechaHora,

                    Motivo = model.Motivo,
                    Edad = model.PacienteEdad,
                    NombreEncargado = model.NombreEncargado,
                    DPIEncargado = model.DPIEncargado,
                    EstadoCita = "normal",
                    EsMenorDeEdad = esMenor,
                    CodigoAutorizacion = model.CodigoAutorizacion,
                    CodigoDeCita = model.CodigoCita,
                    EstadoTurno = model.NumeroTurno != null ? "ACTIVO" : null,
                    FechaHoraInicioTurno = model.NumeroTurno != null ? model.FechaHora : (DateTime?)null,
                    NumeroTurno = model.NumeroTurno,
                    NivelPrioridadCita = model.NivelPrioridadCita,
                };

                // Añadir servicios a la cita
                if (model.Servicios != null)
                {
                    foreach (var servicio in model.Servicios)
                    {
                        if (servicio.Nuevo)
                        {
                            nuevaCita.CitasServicios.Add(new CitasServicio
                            {
                                ServicioId = servicio.ServicioId,
                                PrecioId = servicio.PrecioId,
                                PrecioValor = servicio.PrecioValor,
                                PrecioValorCopago = servicio.PrecioValorCopago,
                                PrecioValorCubiertoSeguro = servicio.PrecioValorCubiertoSeguro
                            });
                        }
                    }
                }

                // Añadir exámenes a la cita
                if (model.Examenes != null)
                {
                    if (nuevaCita.CitasExamenes == null)
                    {
                        nuevaCita.CitasExamenes = new List<CitasExamenes>();
                    }
                    foreach (var examen in model.Examenes)
                    {
                        if (examen.Nuevo)
                        {
                            nuevaCita.CitasExamenes.Add(new CitasExamenes
                            {
                                ExamenLabClinicoId = examen.ExamenId,
                                PrecioId = examen.PrecioId,
                                PrecioValor = examen.PrecioValor,
                                PrecioValorCopago = examen.PrecioValorCopago,
                                PrecioValorCubiertoSeguro = examen.PrecioValorCubiertoSeguro
                            });
                        }
                    }
                }

                _citasRepository.Add(nuevaCita);

                if (!string.IsNullOrWhiteSpace(paciente.Telefono))
                {
                    var medicoNombre = model.EmpleadoId.HasValue
                        ? _empleadoRepository.Get(model.EmpleadoId.Value, false)?.NombreYApellidos ?? "su médico"
                        : "su médico";
                    var msgCita = $"Confirmación de cita\nPaciente: {paciente.Nombre}\nFecha: {model.FechaHora:dd/MM/yyyy HH:mm}\nMédico: {medicoNombre}";
                    await _whatsAppService.SendTextMessageAsync(paciente.Telefono, msgCita);
                }

                TempData["Message"] = "¡La cita se ha agendado con éxito!";

                // return JsonSerializer.Serialize(new
                // {
                //     Exitoso = true,
                //     Fecha = model.FechaHora.ToString("yyyy/MM/dd")
                // });

                var redirectUrl = Url.Action("CalendarioLineal", "Cita", new
                {
                    buscar = model.FechaHora.ToString("yyyy-MM-dd"),
                    modo = "consulta"
                });

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Fecha = model.FechaHora.ToString("yyyy/MM/dd"),
                    RedirectUrl = redirectUrl   // ← nueva propiedad
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor. " + ex.InnerException?.Message ?? ex.Message
                });
            }
        }


        public IActionResult EditarCita(int? id)
        {
            if (id == null) return StatusCode(400);

            var cita = _citasRepository.GetCita((int)id);

            if (cita == null) return StatusCode(404);


            bool esSalaOperaciones = cita.HabitacionId != null ||
                         !string.IsNullOrEmpty(cita.Procedimiento) ||
                         !string.IsNullOrEmpty(cita.Anestesista) ||
                         !string.IsNullOrEmpty(cita.PrimerAyudante);

            ViewData["OrigenFormulario"] = esSalaOperaciones ? "calendario" : "admision";

            var model = new CitaViewModel()
            {
                CitaId = cita.Id,
                PacienteId = cita.PacienteId,
                FechaHora = (DateTime)cita.FechaInicio,
                PacienteNombre = cita.Paciente.Nombre,
                nombrePacienteSeleccionado = cita.Paciente.Nombre,
                FechaNacimiento = cita.Paciente.FechaNacimiento,
                PacienteEdad = cita.Paciente.Edad,
                SucursalId = cita.SucursalId,
                EmpleadoId = cita.EmpleadoId,
                EstadoCita = cita.EstadoCita,
                CitaTipoAtencion = cita.CitaTipoAtencion,
                NombreEncargado = cita.NombreEncargado,
                DPIEncargado = cita.DPIEncargado,
                NivelPrioridadCita = cita.NivelPrioridadCita,
                dpiPacienteSeleccionado = cita.Paciente.Dpi,
                Telefono = cita.Paciente.Telefono,
                Email = cita.Paciente.Email,
                SexoId = cita.Paciente.SexoId,
                Direccion = cita.Paciente.Direccion,
                DepartamentoId = cita.Paciente.DepartamentoId,
                MunicipioId = cita.Paciente.MunicipioId,
                no_IGGS = cita.Paciente.no_IGGS,
                EtniaPaciente = cita.Paciente.EtniaPaciente,
                OrigenPaciente = cita.Paciente.OrigenPaciente,
                EspecialidadId = cita.EspecialidadId,
                NumeroTurno = cita.NumeroTurno,
                Motivo = cita.Motivo,
                CodigoCita = cita.CodigoDeCita,
                CodigoAutorizacion = cita.CodigoAutorizacion,
                Dia = ((DateTime)cita.FechaInicio).ToString("dd/MM/yyyy"),
                Hora = ((DateTime)cita.FechaInicio).ToString("HH:mm"),

                Procedimiento = cita.Procedimiento,

                Anestesista = cita.Anestesista,
                AnestesistaId = cita.AnestesistaId,

                PrimerAyudante = cita.PrimerAyudante,
                PrimerAyudanteId = cita.PrimerAyudanteId,

                SegundoAyudante = cita.SegundoAyudante,
                SegundoAyudanteId = cita.SegundoAyudanteId,

                Instrumentista = cita.Instrumentista,
                InstrumentistaId = cita.InstrumentistaId,

                Circulante = cita.Circulante,
                CirculanteId = cita.CirculanteId,

                HabitacionId = cita.HabitacionId,
                CategoriaHabitacionId = cita.CategoriaHabitacionId,

                //Cita = cita,
                //HoraYFecha = cita.FechaInicio ?? DateTime.Now
            };

            if (cita.EmpleadoId != null)
            {
                var empCirujano = _empleadoRepository.Get(cita.EmpleadoId.Value);
                if (empCirujano != null)
                    model.EmpleadoText = empCirujano.NombreYApellidos;
            }
            else if (!string.IsNullOrWhiteSpace(cita.EmpleadoText))
            {
                model.EmpleadoText = cita.EmpleadoText;
            }

            var categoriaId = cita.CategoriaHabitacionId;
            if (categoriaId.HasValue)
            {
                var habitaciones = _habitacionRepository.GetHabitacionesPorCategoriaParaAgenda(categoriaId.Value);
                var fechaInicio = cita.FechaInicio ?? DateTime.Now;
                var fechaFin = cita.FechaFinal ?? fechaInicio.AddHours(1);
                var citasEnRango = _citasRepository.GetAll()
                    .Where(c => c.HabitacionId != null && !c.Eliminado && c.EstadoCita != "cancelada"
                                && c.FechaInicio < fechaFin && c.FechaFinal > fechaInicio && c.Id != cita.Id)
                    .Select(c => c.HabitacionId.Value).Distinct().ToList();
                var disponibles = habitaciones.Where(h => !citasEnRango.Contains(h.Id) || h.Id == cita.HabitacionId)
                    .Select(h => new { h.Id, h.NombreNumeroHabitacion }).ToList();
                model.HabitacionesSelectList = new SelectList(disponibles, "Id", "NombreNumeroHabitacion", cita.HabitacionId);
            }
            else
            {
                model.HabitacionesSelectList = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            model.Init(_citasRepository, _pacienteRepository, _empleadoRepository, _servicioRepository, _sucursalRepository);
            return View(model);
        }
        [HttpPost]
        public JsonResult BuscarAgenda(string CitaId, string Dia, string Hora)
        {
            var Id = Int32.Parse(CitaId);
            var FechaHora = Dia + " " + Hora;
            DateTime Fecha = DateTime.Parse(FechaHora);
            var citasFecha = _citasRepository.CitasPorFechaHora(Fecha);
            var cantidadeCitas = _citasRepository.CitasPorFechaHora(Fecha).Where(a => a.Id != Id).Count();


            if (cantidadeCitas == 0)
            {
                return Json(new { Exitoso = true, Fecha = Fecha });
            }
            else
            {

                return Json(new { Exitoso = false });

            }
            //return Json(pacientebuscado);
        }

        [HttpPost]
        public async Task<string> EditarCita(CitaViewModel viewModel)
        {
            try
            {
                Console.WriteLine(JsonSerializer.Serialize(viewModel));

                // Validaciones iniciales con mensajes específicos
                if (viewModel == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Error: El modelo de datos (viewModel) es nulo"
                    });
                }

                if (viewModel.CitaId == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Error: CitaId es requerido y no puede ser nulo"
                    });
                }

                // Validación y parsing de fecha (SE MANTIENE TAL CUAL para no afectar lógica existente)
                DateTime fechaHora;
                if (viewModel.Dia != null)
                {
                    if (string.IsNullOrEmpty(viewModel.Hora))
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = "Error: La Hora es requerida cuando se especifica el Día"
                        });
                    }

                    if (!DateTime.TryParse(viewModel.Dia + " " + viewModel.Hora, out fechaHora))
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = $"Error: No se puede convertir la fecha/hora. Día: '{viewModel.Dia}', Hora: '{viewModel.Hora}'"
                        });
                    }
                    viewModel.FechaHora = fechaHora;
                }
                else if (viewModel.FechaHora == default(DateTime))
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Error: Debe especificar una FechaHora válida o proporcionar Dia y Hora"
                    });
                }

                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Error: No se pudo obtener el usuario actual"
                    });
                }

                var esMenor = viewModel.PacienteEdad != null && viewModel.PacienteEdad < 18 ? true : false;

                // Manejo de paciente con validaciones específicas
                Paciente paciente;
                if (viewModel.PacienteId == null)
                {
                    // Validaciones para nuevo paciente
                    if (string.IsNullOrEmpty(viewModel.PacienteNombre))
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = "Error: PacienteNombre es requerido para un nuevo paciente"
                        });
                    }

                    if (viewModel.SexoId == null)
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = "Error: SexoId es requerido para un nuevo paciente"
                        });
                    }

                    paciente = new Paciente
                    {
                        Nombre = viewModel.PacienteNombre,
                        EstadoPacienteId = (int)EstadoPacienteEnum.Activo,
                        TipoPacienteId = (int)TipoPacienteEnum.Nuevo,
                        SexoId = viewModel.SexoId.Value,
                        Edad = viewModel.PacienteEdad,
                        Dpi = viewModel.dpiPacienteSeleccionado,
                        FechaNacimiento = viewModel.FechaNacimiento,
                        FechaRegistro = viewModel.FechaHora,
                        Telefono = viewModel.Telefono,
                        Email = viewModel.Email,
                        IgssNumeroAfiliacion = viewModel.no_IGGS,
                        EtniaPaciente = viewModel.EtniaPaciente,
                        OrigenPaciente = viewModel.OrigenPaciente,
                        DepartamentoId = viewModel.DepartamentoId,
                        MunicipioId = viewModel.MunicipioId,
                        Direccion = viewModel.Direccion
                    };
                }
                else
                {
                    paciente = _pacienteRepository.Get(viewModel.PacienteId.Value);
                    if (paciente == null)
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = $"Error: No se encontró el paciente con ID: {viewModel.PacienteId.Value}"
                        });
                    }

                    // Actualización de paciente existente
                    paciente.Dpi = viewModel.dpiPacienteSeleccionado;
                    paciente.Telefono = viewModel.Telefono;
                    paciente.Email = viewModel.Email;
                    paciente.FechaNacimiento = viewModel.FechaNacimiento;
                    paciente.Edad = viewModel.PacienteEdad;
                    paciente.Direccion = viewModel.Direccion;
                    paciente.DepartamentoId = viewModel.DepartamentoId;
                    paciente.MunicipioId = viewModel.MunicipioId;
                    paciente.OrigenPaciente = viewModel.OrigenPaciente;
                    paciente.EtniaPaciente = viewModel.EtniaPaciente;

                    if (viewModel.SexoId.HasValue)
                    {
                        paciente.SexoId = viewModel.SexoId.Value;
                    }

                    paciente.Nombre = viewModel.PacienteNombre;
                }

                // Obtener y validar la cita
                var cita = _citasRepository.GetCita(viewModel.CitaId.Value);
                if (cita == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"Error: No se encontró la cita con ID: {viewModel.CitaId.Value}"
                    });
                }

                // =========================
                // CAMBIO CLAVE: preservar fecha/hora agendada ORIGINAL
                // =========================
                var fechaInicioOriginal = cita.FechaInicio;
                var fechaFinalOriginal = cita.FechaFinal;

                // Validaciones de campos requeridos para la cita (ajustadas según tipos reales)
                if (viewModel.EmpleadoId == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Error: EmpleadoId es requerido"
                    });
                }

                if (viewModel.SucursalId == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Error: SucursalId es requerido"
                    });
                }

                if (viewModel.EspecialidadId == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Error: EspecialidadId es requerido"
                    });
                }

                // Actualización de datos de CITA
                cita.EmpleadoId = viewModel.EmpleadoId.Value;
                cita.SucursalId = viewModel.SucursalId.Value;
                cita.Paciente = paciente;

                // =========================
                // CAMBIO CLAVE: NO TOCAR FECHA/HORA DE LA CITA
                // Se eliminan estas asignaciones para no alterar agenda:
                // cita.FechaInicio = viewModel.FechaHora;
                // cita.FechaFinal = viewModel.FechaHora.AddMinutes(duracionTotal);
                // =========================

                // (Se mantiene el cálculo de duración porque puede ser usado en otras validaciones futuras,
                //  pero ya NO se aplica a FechaFinal.)
                var duracionTotal = 0;
                if (viewModel.DuracionTotalHoras > 0)
                    duracionTotal += viewModel.DuracionTotalHoras * 60;
                if (viewModel.DuracionTotalMinutos > 0)
                    duracionTotal += viewModel.DuracionTotalMinutos;

                // Reforzar: dejar intactas las fechas originales (blindaje final)
                cita.FechaInicio = fechaInicioOriginal;
                cita.FechaFinal = fechaFinalOriginal;

                cita.NombreEncargado = viewModel.NombreEncargado;
                cita.DPIEncargado = viewModel.DPIEncargado;
                cita.EsMenorDeEdad = esMenor;
                cita.EstadoCita = viewModel.EstadoCita;
                cita.Motivo = viewModel.Motivo;
                cita.EspecialidadId = viewModel.EspecialidadId.Value;
                cita.NivelPrioridadCita = viewModel.NivelPrioridadCita;
                cita.CitaTipoAtencion = viewModel.CitaTipoAtencion;
                cita.CodigoDeCita = viewModel.CodigoCita;
                cita.CodigoAutorizacion = viewModel.CodigoAutorizacion;


                cita.Procedimiento = viewModel.Procedimiento;

                cita.Anestesista = viewModel.Anestesista;
                cita.AnestesistaId = viewModel.AnestesistaId;

                cita.PrimerAyudante = viewModel.PrimerAyudante;
                cita.PrimerAyudanteId = viewModel.PrimerAyudanteId;

                cita.SegundoAyudante = viewModel.SegundoAyudante;
                cita.SegundoAyudanteId = viewModel.SegundoAyudanteId;

                cita.Instrumentista = viewModel.Instrumentista;
                cita.InstrumentistaId = viewModel.InstrumentistaId;

                cita.Circulante = viewModel.Circulante;
                cita.CirculanteId = viewModel.CirculanteId;

                cita.HabitacionId = viewModel.HabitacionId;

                // Manejo de Servicios
                if (cita.CitasServicios != null)
                {
                    foreach (var servicio in cita.CitasServicios)
                    {
                        servicio.Eliminado = true;
                    }
                }
                else
                {
                    cita.CitasServicios = new List<CitasServicio>();
                }

                if (viewModel.Servicios != null)
                {
                    foreach (var servicio in viewModel.Servicios)
                    {
                        try
                        {
                            if (servicio.Nuevo)
                            {
                                var citaServicio = new CitasServicio
                                {
                                    ServicioId = servicio.ServicioId,
                                    Cantidad = servicio.Cantidad,
                                    PrecioId = servicio.PrecioId,
                                    PrecioValor = servicio.PrecioValor,
                                    PrecioValorCubiertoSeguro = servicio.PrecioValorCubiertoSeguro,
                                    PrecioValorCopago = servicio.PrecioValorCopago
                                };
                                cita.CitasServicios.Add(citaServicio);
                            }
                            else
                            {
                                var servicioExistente = cita.CitasServicios
                                    .Where(a => a.Id == servicio.Id)
                                    .FirstOrDefault();

                                if (servicioExistente == null)
                                {
                                    return JsonSerializer.Serialize(new
                                    {
                                        Exitoso = false,
                                        Mensaje = $"Error: No se encontró el servicio con ID: {servicio.Id}"
                                    });
                                }

                                servicioExistente.Eliminado = false;
                                servicioExistente.PrecioValor = servicio.PrecioValor;
                                servicioExistente.PrecioValorCubiertoSeguro = servicio.PrecioValorCubiertoSeguro;
                                servicioExistente.PrecioValorCopago = servicio.PrecioValorCopago;
                                _citasRepository.Update(servicioExistente);
                            }
                        }
                        catch (Exception exServicio)
                        {
                            return JsonSerializer.Serialize(new
                            {
                                Exitoso = false,
                                Mensaje = $"Error procesando servicio - ServicioId: {servicio?.ServicioId}, Id: {servicio?.Id}, Nuevo: {servicio?.Nuevo}",
                                ErrorDetalle = exServicio.Message
                            });
                        }
                    }
                }

                // Manejo de Exámenes
                var listaExamenes = new List<CitasExamenes>();

                if (viewModel.Examenes != null)
                {
                    foreach (var item in viewModel.Examenes)
                    {
                        try
                        {
                            if (item.Nuevo)
                            {
                                var examenCita = new CitasExamenes
                                {
                                    ExamenLabClinicoId = item.ExamenId,
                                    Cantidad = item.Cantidad,
                                    PrecioId = item.PrecioId,
                                    PrecioValor = item.PrecioValor,
                                    Eliminado = false,
                                    PrecioValorCubiertoSeguro = item.PrecioValorCubiertoSeguro,
                                    PrecioValorCopago = item.PrecioValorCopago
                                };
                                listaExamenes.Add(examenCita);
                            }
                            else
                            {
                                var examenExistente = cita.CitasExamenes?
                                    .Where(a => a.ExamenLabClinicoId == item.ExamenId)
                                    .FirstOrDefault();

                                if (examenExistente == null)
                                {
                                    return JsonSerializer.Serialize(new
                                    {
                                        Exitoso = false,
                                        Mensaje = $"Error: No se encontró el examen con ID: {item.ExamenId}"
                                    });
                                }

                                examenExistente.Eliminado = false;
                                examenExistente.PrecioValorCubiertoSeguro = item.PrecioValorCubiertoSeguro;
                                examenExistente.PrecioValorCopago = item.PrecioValorCopago;
                                examenExistente.PrecioValor = item.PrecioValor;
                                listaExamenes.Add(examenExistente);
                            }
                        }
                        catch (Exception exExamen)
                        {
                            return JsonSerializer.Serialize(new
                            {
                                Exitoso = false,
                                Mensaje = $"Error procesando examen - ExamenId: {item?.ExamenId}, Nuevo: {item?.Nuevo}",
                                ErrorDetalle = exExamen.Message
                            });
                        }
                    }
                    cita.CitasExamenes = listaExamenes;
                }

                _citasRepository.Update(cita);
                TempData["Message"] = "¡La cita se ha modificado con éxito!";

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Fecha = fechaInicioOriginal.HasValue
        ? fechaInicioOriginal.Value.ToString("yyyy-MM-dd HH:mm")
        : null
                });

            }
            catch (InvalidOperationException ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error: Operación inválida - posiblemente un valor requerido es nulo",
                    ErrorDetallado = ex.Message,
                    TipoError = "InvalidOperationException"
                });
            }
            catch (NullReferenceException ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error: Referencia nula encontrada",
                    ErrorDetallado = ex.Message,
                    TipoError = "NullReferenceException",
                    StackTrace = ex.StackTrace
                });
            }
            catch (Exception ex)
            {
                // Log más detallado del error
                var errorDetallado = $"Error en EditarCita: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorDetallado += $" | Inner Exception: {ex.InnerException.Message}";
                }

                // Información sobre el estado del viewModel
                var viewModelInfo = viewModel != null ?
                    $" | CitaId: {viewModel.CitaId}, PacienteId: {viewModel.PacienteId}, EmpleadoId: {viewModel.EmpleadoId}, SucursalId: {viewModel.SucursalId}" :
                    " | ViewModel es null";

                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al modificar cita.",
                    ErrorDetallado = errorDetallado + viewModelInfo,
                    TipoError = ex.GetType().Name,
                    StackTrace = ex.StackTrace
                });
            }
        }

        //public async Task<IActionResult> BloquearFecha(string fecha,string motivo)
        //{
        //    var user = await _userManager.GetUserAsync(HttpContext.User);

        //    var nuevaCita = new Citas()
        //    {
        //        User = user,
        //        FechaInicio = Convert.ToDateTime(fecha),
        //        FechaFinal = Convert.ToDateTime(fecha),
        //        Motivo = motivo,
        //        Bloqueada = true
        //    };

        //    _citasRepository.Add(nuevaCita);
        //    TempData["Message"] = "¡La fecha se ha bloqueado con éxito.!";
        //    return RedirectToAction("CalendarioLineal", new { buscar = Convert.ToDateTime(fecha).ToString("yyyy-MM-dd") });

        //}


        [HttpGet]
        public JsonResult GetHabitacionesParaEdicion(int citaId)
        {
            try
            {
                var cita = _citasRepository.GetCita(citaId);
                if (cita == null)
                    return Json(new { exitoso = false, mensaje = "Cita no encontrada" });

                if (cita.CategoriaHabitacionId == null)
                    return Json(new { exitoso = true, habitaciones = new List<object>(), habitacionActualId = (int?)null });

                var categoriaId = cita.CategoriaHabitacionId.Value;
                var habitacionActualId = cita.HabitacionId;

                var fechaInicio = cita.FechaInicio ?? DateTime.Now;
                var fechaFin = cita.FechaFinal ?? fechaInicio.AddHours(1);

                var habitaciones = _habitacionRepository.GetHabitacionesPorCategoriaParaAgenda(categoriaId);

                var citasEnRango = _citasRepository.GetAll()
                    .Where(c => c.HabitacionId != null
                                && !c.Eliminado
                                && c.EstadoCita != "cancelada"
                                && c.FechaInicio < fechaFin
                                && c.FechaFinal > fechaInicio
                                && c.Id != citaId)
                    .Select(c => c.HabitacionId.Value)
                    .Distinct()
                    .ToList();

                var resultado = habitaciones
                    .Select(h => new
                    {
                        id = h.Id,
                        nombre = h.NombreNumeroHabitacion,
                        disponible = !citasEnRango.Contains(h.Id) || h.Id == habitacionActualId
                    })
                    .Where(h => h.disponible)
                    .ToList();

                return Json(new { exitoso = true, habitaciones = resultado, habitacionActualId = habitacionActualId });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<string> BloquearFecha(string fecha, string motivo)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            try
            {
                var nuevaCita = new Citas()
                {
                    User = user,
                    FechaInicio = Convert.ToDateTime(fecha),
                    FechaFinal = Convert.ToDateTime(fecha),
                    Motivo = motivo,
                    Bloqueada = true
                };

                _citasRepository.Add(nuevaCita);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true
                }
                 );

            }
            catch (Exception ex)
            {

                return JsonSerializer.Serialize(
                    new
                    {
                        Exitoso = false,
                        Mensaje = "Error de servidor al bloquear dia." + ex.Message

                    }
                  );


            }



        }

        [HttpPost]
        public string ConsultarPacientes()
        {
            try
            {
                var pacientes = _pacienteRepository.GetList();

                var result = pacientes.Select(x => new
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Dpi = x.Dpi,
                    NombreConDpi = x.PacienteWithDPI,
                    FechaNacimiento = x.FechaNacimiento != null ? ((DateTime)x.FechaNacimiento).ToString("yyyy-MM-dd") : null,
                    Telefono = x.Telefono,
                    Email = x.Email,
                    SexoId = x.SexoId,
                    Direccion = x.Direccion,
                    EtniaPaciente = x.EtniaPaciente,
                    OrigenPaciente = x.OrigenPaciente,
                    DepartamentoId = x.DepartamentoId,
                    MunicipioId = x.MunicipioId,
                    IgssNumeroAfiliacion = x.IgssNumeroAfiliacion
                });

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
                    Mensaje = "Error al consultar pacientes. " + ex.Message
                });
            }
        }
        public IActionResult DesbloquearFecha(int citaId)
        {
            var cita = _citasRepository.GetCita(citaId);
            cita.Bloqueada = false;
            cita.Eliminado = true;

            _citasRepository.Update(cita);
            TempData["Message"] = "¡La fecha se ha desbloqueado con éxito.!";
            return RedirectToAction("CalendarioLineal");

        }
        [HttpPost]
        public string BloquearDia(DateTime dia, string motivo)
        {
            try
            {


                var fechaBloqueada = new CalendarioFechaBloqueada
                {
                    Fecha = dia,
                    UsuarioBloquea = _userManager.GetUserId(HttpContext.User),
                    Motivo = motivo
                };
                _citasRepository.AddFechaBloqueada(fechaBloqueada);
                TempData["Message"] = "El día ha sido bloqueado";
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al bloquear dia. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string DesbloquearDia(DateTime dia)
        {
            try
            {
                _citasRepository.DeleteFechaBloqueada(dia);
                TempData["Message"] = "El día ha sido desbloqueado";
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
                    Mensaje = "Error de servidor al desbloquear dia. " + ex.Message
                });
            }
        }

        public IActionResult CancelarCita(int? id)
        {
            if (id == null) return StatusCode(400);

            var cita = _citasRepository.GetCita((int)id);

            if (cita == null) return StatusCode(404);

            var model = new CitasCancelViewModel()
            {
                Cita = cita,
                HoraYFecha = cita.FechaInicio ?? DateTime.Now
            };

            model.Init(_citasRepository, _pacienteRepository, _empleadoRepository, _servicioRepository);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CancelarCita(CitasCancelViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var cita = _citasRepository.GetCita(viewModel.Cita.Id);
            if (cita == null)
                return NotFound();

            // 1) Cambias el estado
            cita.EstadoCita = "cancelada";
            // 2) Y además la "eliminas" lógicamente:
            cita.Eliminado = true;

            _citasRepository.Update(cita);

            // 3) Aplicas la penalización si la hay
            if (viewModel.CargoPenalizacion > 0)
            {
                var cuenta = _cuentaPorCobrarRepository
                    .GetUltimaCuentaPendientePaciente((int)viewModel.Cita.PacienteId);
                cuenta.Valor += viewModel.CargoPenalizacion;
                cuenta.Observaciones +=
                    $". Cargo por penalización de cita cancelada el día {DateTime.Now}";
                _cuentaPorCobrarRepository.Update(cuenta);
            }

            TempData["Message"] = "¡La cita se ha cancelado con éxito!";
            return RedirectToAction("Lista");
        }


        public IActionResult NoAsistio(int? citaId, DateTime fecha)
        {
            if (citaId == null) return StatusCode(400);

            var cita = _citasRepository.GetCita((int)citaId);

            if (cita == null) return StatusCode(404);

            cita.EstadoCita = "No asistida";

            _citasRepository.Update(cita);
            TempData["Message"] = "¡La cita se ha movido a No asistidas con exito.!";

            return RedirectToAction("CalendarioLineal", new { buscar = fecha.ToString("MM/dd/yyyy") });
        }
        public IActionResult NoAsistioL(int? citaId)
        {
            if (citaId == null) return StatusCode(400);

            var cita = _citasRepository.GetCita((int)citaId);

            if (cita == null) return StatusCode(404);

            cita.EstadoCita = "No asistida";

            _citasRepository.Update(cita);
            TempData["Message"] = "¡La cita se ha movido a No asistidas con exito.!";

            return RedirectToAction("Lista");
        }
        public IActionResult ReprogramarCita(int? citaId, DateTime fecha)
        {
            if (citaId == null) return StatusCode(400);

            var cita = _citasRepository.GetCita((int)citaId);

            if (cita == null) return StatusCode(404);

            var model = new CitasEditViewModel()
            {
                Cita = cita,
                HoraYFecha = fecha
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ReprogramarCita(CitasEditViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                var cita = _citasRepository.GetCita(viewModel.Cita.Id);
                cita.FechaInicio = viewModel.Cita.FechaInicio;
                cita.FechaFinal = viewModel.Cita.FechaInicio;
                cita.EstadoCita = "normal";

                _citasRepository.Update(cita);
                TempData["Message"] = "¡La cita se ha reprogramado con exito.!";
                return RedirectToAction("ReprogramarCita", new
                {
                    citaId = viewModel.Cita.Id,
                    fecha =
                    ((DateTime)viewModel.Cita.FechaInicio).ToString("MM/dd/yyyy")
                });

            }

            return View(viewModel);
        }

        public async Task<IActionResult> CitasListadoNormales(string currentFilter)
        {
            var citas = _citasRepository.CitasNormales(currentFilter);

            var u = _userRepository.GetDisplayName(_userManager.GetUserId(HttpContext.User));

            var model = new ReporteCitasViewModel()
            {
                Citas = citas,
                Usuario = u
            };

            return await _generatePdf.GetPdf("Views/Cita/CitasListadoNormales.cshtml", model);
        }


        public IActionResult CalendarioLineal(string buscar, string buscar2, int? sucursalId, int? empleadoId, int? especialidadId, int? servicioId, string pacienteNombre, string modo, int? habitacionId)
        {
            // Normalizar modo; SerSalud (SS) siempre hospitalización
            modo = ClienteConfigHelper.ResolverModoCalendario(_configuration, modo);
            bool esHospitalizacion = modo == "hospitalizacion";

            ViewData["Modo"] = modo;
            ViewData["PacienteNombre"] = pacienteNombre;

            DateTime fechaInicial;
            DateTime fechaFinal;

            // Determinar fechas (si vienen vacías => búsqueda global)
            var sinFechas = string.IsNullOrWhiteSpace(buscar) && string.IsNullOrWhiteSpace(buscar2);

            if (sinFechas)
            {
                // Estas no se usarán para filtrar en BD (se enviará null/null),
                // pero se necesitan para lógica de UI y bloqueo (mostramos el calendario en el día actual).
                fechaInicial = DateTime.Today;
                fechaFinal = DateTime.Today;
            }
            else
            {
                // Si viene solo una, la otra se asume igual a la que exista (mismo día)
                if (!DateTime.TryParse(buscar, out fechaInicial))
                    fechaInicial = DateTime.Today;

                if (!DateTime.TryParse(buscar2, out fechaFinal))
                    fechaFinal = fechaInicial;
            }

            // Strings para ViewData/UI
            string b;
            string b2;

            if (sinFechas)
            {
                // UI vacía = "todas las fechas"
                b = string.Empty;
                b2 = string.Empty;
            }
            else
            {
                b = fechaInicial.ToString("yyyy-MM-dd");
                b2 = fechaFinal.ToString("yyyy-MM-dd");
            }

            // Fechas para BD (null/null si no se seleccionaron)
            DateTime? fi = sinFechas ? (DateTime?)null : fechaInicial;
            DateTime? ff = sinFechas ? (DateTime?)null : fechaFinal;

            // var citaPorFecha = _citasRepository.CitasCalendarioLineal(
            //     fi,
            //     ff,
            //     sucursalId,
            //     empleadoId,
            //     especialidadId,
            //     servicioId,
            //     pacienteNombre);

            var citaPorFecha = _citasRepository.CitasCalendarioLineal(
                fi, ff, sucursalId, empleadoId, especialidadId, servicioId, null);

            // Aplicar filtro de paciente con búsqueda parcial e insensible a mayúsculas
            if (!string.IsNullOrWhiteSpace(pacienteNombre))
            {
                citaPorFecha = citaPorFecha
                    .Where(c => c.Paciente != null && c.Paciente.Nombre != null &&
                                c.Paciente.Nombre.Contains(pacienteNombre, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }


            // Filtrar por modo: consulta = sin habitación, hospitalización = con habitación
            if (esHospitalizacion)
                citaPorFecha = citaPorFecha.Where(c => c.HabitacionId != null).ToList();
            else
                citaPorFecha = citaPorFecha.Where(c => c.HabitacionId == null).ToList();

            if (habitacionId.HasValue && habitacionId.Value > 0)
            {
                citaPorFecha = citaPorFecha.Where(c => c.HabitacionId == habitacionId.Value).ToList();
            }

            // Contadores (mantengo tu lógica actual usada en la vista)
            var citasNoPagadas = citaPorFecha.Count(c => !c.Eliminado);
            var citasEnEspera = citaPorFecha.Count(c => c.EstadoCita == "En espera");
            var citasFinalizadas = citaPorFecha.Count(c => c.EstadoCita == "Finalizada");

            ViewData["CurrentFilter"] = b;
            ViewData["CurrentFilter2"] = b2;
            ViewData["EmpleadoId"] = empleadoId;
            ViewData["EspecialidadId"] = especialidadId;
            ViewData["ServicioId"] = servicioId;

            var model = new CalendarioLinealViewModel()
            {
                // El calendario siempre necesita un día para pintar filas.
                // Si no hay fechas, se pinta el día actual.
                Fecha = sinFechas ? DateTime.Today.ToString("yyyy-MM-dd") : b,
                Citas = citaPorFecha,
                FechaBloqueada = false,
                CitasNoPagadas = citasNoPagadas,
                CitasEnEspera = citasEnEspera,
                CitasFinalizadas = citasFinalizadas
            };

            // Bloqueo: evaluamos el día que se está mostrando en el calendario (hoy si sinFechas)
            var fechaBloqueada = _citasRepository.GetFechaBloqueada(fechaInicial.Date);
            if (fechaBloqueada != null)
            {
                model.FechaBloqueada = true;
                model.Motivo = fechaBloqueada.Motivo;
            }

            // ============================================================
            // Obtener los IDs de las categorías de Sala de Operaciones
            // ============================================================
            var todasCategorias = _habitacionRepository.GetCategorias(includeTarifas: false);
            var idsSalaOperaciones = todasCategorias
                .Where(c => c.NombreCategoria.Contains("Sala de Operaciones") ||
                            c.NombreCategoria.Contains("Sala de Operación"))
                .Select(c => c.Id)
                .ToList();

            if (!idsSalaOperaciones.Any())
            {
                idsSalaOperaciones.Add(-1);
            }

            var todasHabitaciones = _habitacionRepository.GetHabitaciones();

            foreach (var h in todasHabitaciones)
            {
                if (h.CategoriaHabitacion == null && h.CategoriaHabitacionId != 0)
                {
                    h.CategoriaHabitacion = todasCategorias.FirstOrDefault(c => c.Id == h.CategoriaHabitacionId);
                }
            }

            IEnumerable<Habitacion> habitacionesFiltradas;

            if (esHospitalizacion)
            {
                habitacionesFiltradas = todasHabitaciones
                    .Where(h => idsSalaOperaciones.Contains(h.CategoriaHabitacionId));
            }
            else
            {
                habitacionesFiltradas = todasHabitaciones
                    .Where(h => !idsSalaOperaciones.Contains(h.CategoriaHabitacionId));
            }

            var habitacionesSelectList = habitacionesFiltradas.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = $"{h.NombreNumeroHabitacion} - {(h.CategoriaHabitacion != null ? h.CategoriaHabitacion.NombreCategoria : "Sin categoría")}"
            }).ToList();

            habitacionesSelectList.Insert(0, new SelectListItem { Value = "", Text = "Todas" });
            model.HabitacionesSelectList = habitacionesSelectList;
            model.HabitacionId = habitacionId;

            model.Init(_sucursalRepository, _empleadoRepository, _citasRepository);

            return View(model);
        }

        public IActionResult CalendarioLinealCpl(string buscar, int? sucursalId, int? empleadoId, int? especialidadId/*, string currentFilter*/)
        {
            var b = "";

            if (buscar == null)
            {
                b = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                b = Convert.ToDateTime(buscar).ToString("yyyy-MM-dd");
            }

            var fechaConvertida = Convert.ToDateTime(b);
            var citaPorFecha = _citasRepository.CitasPorFecha(fechaConvertida);
            if (sucursalId != null)
            {
                citaPorFecha = citaPorFecha
                    .Where(a => a.SucursalId != null && a.SucursalId == sucursalId)
                    .ToList();
            }
            if (empleadoId != null)
            {
                citaPorFecha = citaPorFecha
                    .Where(a => a.EmpleadoId != null && a.EmpleadoId == empleadoId)
                    .ToList();
            }
            if (especialidadId != null)
            {
                citaPorFecha = citaPorFecha
                    .Where(a => a.EspecialidadId != null && a.EspecialidadId == especialidadId)
                    .ToList();
            }

            ViewData["currentFilter"] = b;

            var model = new CalendarioLinealViewModel()
            {
                Fecha = b,
                Citas = citaPorFecha,
                FechaBloqueada = false
            };

            var fechaBloqueada = _citasRepository.GetFechaBloqueada(fechaConvertida);
            if (fechaBloqueada != null)
            {
                model.FechaBloqueada = true;
                model.Motivo = fechaBloqueada.Motivo;
            }



            model.Init(_sucursalRepository, _empleadoRepository, _citasRepository);

            return View(model);
        }

        public IActionResult ReporteCitasFechas(string buscar, int? sucursalId, int? empleadoId, int? especialidadId)
        {
            var b = "";

            if (buscar == null)
            {
                b = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                b = Convert.ToDateTime(buscar).ToString("yyyy-MM-dd");
            }

            var fechaConvertida = Convert.ToDateTime(b);
            var citaPorFecha = _citasRepository.CitasPorFecha(fechaConvertida);
            if (sucursalId != null)
            {
                citaPorFecha = citaPorFecha
                    .Where(a => a.SucursalId != null && a.SucursalId == sucursalId)
                    .ToList();
            }
            if (empleadoId != null)
            {
                citaPorFecha = citaPorFecha
                    .Where(a => a.EmpleadoId != null && a.EmpleadoId == empleadoId)
                    .ToList();
            }
            if (especialidadId != null)
            {
                citaPorFecha = citaPorFecha
                    .Where(a => a.EspecialidadId != null && a.EspecialidadId == especialidadId)
                    .ToList();
            }

            ViewData["currentFilter"] = b;

            var model = new CalendarioLinealViewModel()
            {
                Fecha = b,
                Citas = citaPorFecha,
                FechaBloqueada = false
            };

            var fechaBloqueada = _citasRepository.GetFechaBloqueada(fechaConvertida);
            if (fechaBloqueada != null)
            {
                model.FechaBloqueada = true;
            }

            model.Init(_sucursalRepository, _empleadoRepository, _citasRepository);

            return View(model);
        }

        //CitasListadoFecha
        public async Task<IActionResult> CitasListadoFecha(string fecha, int? sucursalId, int? empleadoId, int? especialidadId)
        {
            var citas = new List<Citas>();

            citas = (List<Citas>)_citasRepository.CitasPorFecha(Convert.ToDateTime(fecha));
            //return Json (citas);

            if (sucursalId != null)
            {
                citas = citas
                    .Where(a => a.SucursalId != null && a.SucursalId == sucursalId)
                    .ToList();
            }
            if (empleadoId != null)
            {
                citas = citas
                    .Where(a => a.EmpleadoId != null && a.EmpleadoId == empleadoId)
                    .ToList();
            }
            if (especialidadId != null)
            {
                citas = citas
                    .Where(a => a.EspecialidadId != null && a.EspecialidadId == especialidadId)
                    .ToList();
            }

            var u = _userRepository.GetDisplayName(_userManager.GetUserId(HttpContext.User));

            var model = new ReporteCitasViewModel()
            {
                Citas = citas,
                Usuario = u

            };



            return await _generatePdf.GetPdf("Views/Cita/ReporteCitasCalendarioPDF.cshtml", model);

        }

        // [HttpGet]
        // public JsonResult ObtenerContadores(string buscar, int? sucursalId, int? empleadoId, int? especialidadId, int? servicioId)
        // {
        //     var b = buscar == null ? DateTime.Now.ToString("yyyy-MM-dd") : Convert.ToDateTime(buscar).ToString("yyyy-MM-dd");
        //     var fechaConvertida = Convert.ToDateTime(b);
        //     var citaPorFecha = _citasRepository.CitasPorFecha(fechaConvertida);

        //     // Apply filters
        //     if (sucursalId != null)
        //     {
        //         citaPorFecha = citaPorFecha.Where(a => a.SucursalId != null && a.SucursalId == sucursalId).ToList();
        //     }

        //     if (empleadoId != null)
        //     {
        //         citaPorFecha = citaPorFecha.Where(a => a.EmpleadoId != null && a.EmpleadoId == empleadoId).ToList();
        //     }

        //     if (especialidadId != null)
        //     {
        //         citaPorFecha = citaPorFecha.Where(a => a.EspecialidadId != null && a.EspecialidadId == especialidadId).ToList();
        //     }

        //     if (servicioId != null)
        //     {
        //         citaPorFecha = citaPorFecha
        //             .Where(a => a.CitasServicios != null && a.CitasServicios.Any(s => s.ServicioId == servicioId && !s.Eliminado))
        //             .ToList();
        //     }


        //     // Counters
        //     var citasNoPagadas = citaPorFecha.Count(c => c.EstadoCita == "normal");
        //     var citasEnEspera = citaPorFecha.Count(c => c.EstadoCita == "En espera");
        //     var citasFinalizadas = citaPorFecha.Count(c => c.EstadoCita == "Finalizada");

        //     return Json(new
        //     {
        //         CitasNoPagadas = citasNoPagadas,
        //         CitasEnEspera = citasEnEspera,
        //         CitasFinalizadas = citasFinalizadas
        //     });
        // }
        [HttpGet]
        public JsonResult ObtenerContadores(
            string buscar,
            string buscar2,
            int? sucursalId,
            int? empleadoId,
            int? especialidadId,
            int? servicioId,
            string pacienteNombre,
            string modo)
        {
            modo = ClienteConfigHelper.ResolverModoCalendario(_configuration, modo);
            bool esHospitalizacion = modo == "hospitalizacion";

            DateTime temp;

            DateTime? fi = DateTime.TryParse(buscar, out temp) ? temp : (DateTime?)null;
            DateTime? ff = DateTime.TryParse(buscar2, out temp) ? temp : (DateTime?)null;

            // var citaPorFecha = _citasRepository.CitasCalendarioLineal(
            //     fi,
            //     ff,
            //     sucursalId,
            //     empleadoId,
            //     especialidadId,
            //     servicioId,
            //     pacienteNombre);


            var citaPorFecha = _citasRepository.CitasCalendarioLineal(
            fi, ff, sucursalId, empleadoId, especialidadId, servicioId, null);

            // Aplicar filtro de paciente con búsqueda parcial e insensible a mayúsculas
            if (!string.IsNullOrWhiteSpace(pacienteNombre))
            {
                citaPorFecha = citaPorFecha
                    .Where(c => c.Paciente != null && c.Paciente.Nombre != null &&
                                c.Paciente.Nombre.Contains(pacienteNombre, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filtrar por modo: consulta = sin habitación, hospitalización = con habitación
            if (esHospitalizacion)
                citaPorFecha = citaPorFecha.Where(c => c.HabitacionId != null).ToList();
            else
                citaPorFecha = citaPorFecha.Where(c => c.HabitacionId == null).ToList();

            // Contadores (alineados con CalendarioLineal)
            var citasNoPagadas = citaPorFecha.Count(c => !c.Eliminado);
            var citasEnEspera = citaPorFecha.Count(c => c.EstadoCita == "En espera");
            var citasFinalizadas = citaPorFecha.Count(c => c.EstadoCita == "Finalizada");

            return Json(new
            {
                CitasNoPagadas = citasNoPagadas,
                CitasEnEspera = citasEnEspera,
                CitasFinalizadas = citasFinalizadas
            });
        }


        [HttpPost]
        public JsonResult ValidarDisponibilidadEmpleado(int empleadoId, string fechaHora, int duracionHoras, int duracionMinutos, int? citaId)
        {
            // Validación liviana para soporte de UI (el guardado también valida)
            if (empleadoId <= 0) return Json(new { exitoso = true });
            if (string.IsNullOrWhiteSpace(fechaHora)) return Json(new { exitoso = true });

            if (!TryParseFechaHora(fechaHora, out var inicio))
                return Json(new { exitoso = false, mensaje = "Formato de fecha/hora inválido." });

            var totalMinutos = (duracionHoras * 60) + duracionMinutos;
            if (totalMinutos <= 0) totalMinutos = 1;

            var fin = inicio.AddMinutes(totalMinutos);
            var hayConflicto = _citasRepository.ExisteConflictoEmpleado(empleadoId, inicio, fin, citaId);

            if (!hayConflicto) return Json(new { exitoso = true });

            return Json(new
            {
                exitoso = false,
                mensaje = "El profesional seleccionado ya tiene una cita asignada para la fecha y hora seleccionada. Por favor seleccione otro médico u elija otra fecha y horario."
            });
        }

        private static bool TryParseFechaHora(string fechaHora, out DateTime resultado)
        {
            if (DateTime.TryParse(fechaHora, out resultado))
                return true;

            var formatos = new[]
            {
                "MM/dd/yyyy hh:mm tt",
                "MM/dd/yyyy h:mm tt",
                "yyyy-MM-dd HH:mm",
                "yyyy/MM/dd HH:mm",
                "dd/MM/yyyy HH:mm",
                "dd/MM/yyyy H:mm"
            };

            return DateTime.TryParseExact(
                fechaHora,
                formatos,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out resultado);
        }

        [HttpGet]
        public JsonResult ObtenerDatosMedicoPorCita(int citaId = 0, int consultaId = 0)
        {
            try
            {
                Citas cita = null;
                if (citaId > 0)
                    cita = _citasRepository.GetCita(citaId);
                else if (consultaId > 0)
                {
                    var consulta = _consultaRepository.GetConsulta(consultaId);
                    if (consulta?.CitasId != null && consulta.CitasId.Value > 0)
                        cita = _citasRepository.GetCita(consulta.CitasId.Value);
                    else if (consulta?.Citas != null)
                        cita = consulta.Citas;
                }

                if (cita == null)
                    return Json(new { exitoso = false, mensaje = "Cita no encontrada" });

                var nombreMedico = "No suministrado";
                var especialidad = "";
                var urlFirma = "";
                var colegiado = "";

                if (cita.Empleado != null)
                {
                    nombreMedico = cita.Empleado.NombreYApellidos;
                    urlFirma = cita.Empleado.FirmaEmpleado ?? "";
                    colegiado = cita.Empleado.Colegiado ?? "";
                }

                if (cita.EspecialidadText != "N/A")
                    especialidad = cita.EspecialidadText;

                return Json(new
                {
                    exitoso = true,
                    nombreMedico = nombreMedico,
                    especialidad = especialidad,
                    urlFirma = urlFirma,
                    colegiado = colegiado
                });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = ex.Message });
            }
        }


        [HttpGet]
        public JsonResult GetEspecialidadByEmpleado(int empleadoId)
        {
            try
            {
                var empleado = _empleadoRepository.GetEmpleadosConDetalles()
                    .FirstOrDefault(e => e.Id == empleadoId);

                if (empleado == null)
                    return Json(new { exitoso = false, mensaje = "Empleado no encontrado" });

                return Json(new { exitoso = true, especialidadId = empleado.EspecialidadId });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = ex.Message });
            }
        }
    }
}
