using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Wkhtmltopdf.NetCore;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Database.Shared;
using Sistema.Services.WebAuthn;
using sistema.Models;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Database.Shared.Enumeraciones;
using farmamest.Models;
using sistema.Service.IService;
using Microsoft.AspNetCore.Mvc.Rendering;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;

namespace sistema.Controllers
{
    [Authorize]
    public class HospitalizacionController : Controller
    {
        private readonly IPacientes _pacientesRepository = null;
        private readonly IServicio _serviciosRepository = null;
        private readonly ISeguro _seguroRepository;

        private readonly IEmergencias _emergenciasRepo = null;
        private readonly IConsultas _consultasRepository = null;
        private readonly ICuentasPorCobrar _cuentasPorCobrarRepository = null;
        private readonly IProducto _productoRepository = null;
        private readonly ICitas _citasRepository = null;

        private readonly IEmpleado _empleadoRepository = null;
        private readonly IHabitacion _habitacionRepository = null;
        private readonly IHospitalizacion _hospitalizacionRepository = null;
        private readonly ILaboratorioClinico _laboratorioClinicoRepository = null;
        private readonly IUser _userRepository = null;
        private readonly UserManager<User> _userManager;
        private readonly IDetallePaqueteHospitalizacion _detallePaqueteHospitalizacion;
        private readonly IHospitalizacionDetallePaqueteHospitalizacion _hospitalizacionDetallePaqueteHospitalizacion;
        private readonly IEspecialidad _especialidadRepository;
        private readonly IHospitalizacionUsuarioAccesoService _hospitalizacionUsuarioAcceso;
        private readonly ICaja _cajaRepository;
        private readonly INotaOperatoria _notaOperatoriaRepository;
        private readonly IWebAuthnService _webAuthn;
        private readonly Context _db;

        //Servicio (logica de negocio)
        private readonly IPacientesService _pacientesService = null;
        private readonly IProductosService _productosService = null;
        private readonly IHospitalizacionService _hospitalizacionService = null;
        private readonly IEmergenciaService _emergenciaService = null;
        private readonly IFilesService _filesService = null;
        private readonly IProducto _productosRepository = null;

        private readonly IHospitalizacionDocumentoRepository _hospitalizacionDocumentoRepository;

        private readonly IWebHostEnvironment _env;
        private readonly IMedicamentoNotificacionService _medicamentoNotificacionService;

        private readonly IConfiguration _configuration;


        CultureInfo culture = new CultureInfo("es-GT");

        public HospitalizacionController(
            IPacientes pacientesRepository,
            IServicio serviciosRepository,
            IEmergencias emergenciasRepo,
            IProducto productoRepository,
            ICitas citasRepository,
            ISeguro seguroRepository,
            IConsultas consultasRepository,
            ICuentasPorCobrar cuentasPorCobrarRepository,
            IEmpleado empleadoRepository,
            IHabitacion habitacionRepository,
            IHospitalizacion hospitalizacionRepository,
            IUser userRepository,
            ILaboratorioClinico laboratorioClinicoRepository,
            UserManager<User> userManager,
            IDetallePaqueteHospitalizacion detallePaqueteHospitalizacion,
            IHospitalizacionDetallePaqueteHospitalizacion hospitalizacionDetallePaqueteHospitalizacion,
            ICaja cajaRepository,
            //Servicio (logica de negocio)
            IPacientesService pacientesService,
            IProductosService productosService,
            IEspecialidad especialidadRepository,
            IHospitalizacionUsuarioAccesoService hospitalizacionUsuarioAcceso,
            IHospitalizacionService hospitalizacionService,
            IFilesService filesService,
            IEmergenciaService emergenciaService,
            IProducto productosRepository,
            IWebAuthnService webAuthn,
            INotaOperatoria notaOperatoriaRepository,
            IHospitalizacionDocumentoRepository hospitalizacionDocumentoRepository,
            IWebHostEnvironment env,
            Context db,
             IMedicamentoNotificacionService medicamentoNotificacionService,
              IConfiguration configuration
            )
        {
            _emergenciasRepo = emergenciasRepo;
            _pacientesRepository = pacientesRepository;
            _hospitalizacionRepository = hospitalizacionRepository;
            _cuentasPorCobrarRepository = cuentasPorCobrarRepository;
            _consultasRepository = consultasRepository;
            _serviciosRepository = serviciosRepository;
            _citasRepository = citasRepository;
            _productoRepository = productoRepository;
            _habitacionRepository = habitacionRepository;
            _userRepository = userRepository;
            _empleadoRepository = empleadoRepository;
            _laboratorioClinicoRepository = laboratorioClinicoRepository;
            _userManager = userManager;
            _detallePaqueteHospitalizacion = detallePaqueteHospitalizacion;
            _hospitalizacionDetallePaqueteHospitalizacion = hospitalizacionDetallePaqueteHospitalizacion;
            _especialidadRepository = especialidadRepository;
            _hospitalizacionUsuarioAcceso = hospitalizacionUsuarioAcceso;
            _cajaRepository = cajaRepository;
            _seguroRepository = seguroRepository;
            //Servicio (logica de negocio)
            _pacientesService = pacientesService;
            _productosService = productosService;
            _hospitalizacionService = hospitalizacionService;
            _emergenciaService = emergenciaService;
            _filesService = filesService;
            _productosRepository = productosRepository;
            _webAuthn = webAuthn;
            _notaOperatoriaRepository = notaOperatoriaRepository;
            _hospitalizacionDocumentoRepository = hospitalizacionDocumentoRepository;
            _env = env;
            _db = db;
            _medicamentoNotificacionService = medicamentoNotificacionService;
            _configuration = configuration;

        }

        public IActionResult Habitaciones()
        {
            var habitacionesConsultadas = new List<HospitalizacionHabitacionViewModel>();
            var habitaciones = _habitacionRepository.GetHabitaciones();
            if (habitaciones != null)
            {
                foreach (var habitacion in habitaciones)
                {
                    var ocupante = "-";
                    if (habitacion.EstadoHabitacionId == (int)EstadoHabitacionEnum.Ocupada)
                    {
                        var paciente = _habitacionRepository.GetPacienteOcupante(habitacion.Id);
                        ocupante = paciente != null ? paciente.Nombre : "-";
                    }
                    int? hospitalizacionId = null;
                    var hospitalizacionActualId = _habitacionRepository.GetHospitalizacionActual(habitacion.Id) == null ? 0 : _habitacionRepository.GetHospitalizacionActual(habitacion.Id).Id;

                    if (habitacion.EstadoHabitacionId == (int)EstadoHabitacionEnum.Ocupada)
                    {
                        hospitalizacionId = hospitalizacionActualId;
                    }
                    habitacionesConsultadas.Add(new HospitalizacionHabitacionViewModel
                    {
                        HabitacionId = habitacion.Id,
                        HospitalizacionId = hospitalizacionId,
                        HabitacionNombre = habitacion.NombreNumeroHabitacion,
                        HabitacionCategoria = habitacion.CategoriaHabitacion.NombreCategoria,
                        HabitacionEstadoId = habitacion.EstadoHabitacionId,
                        HabitacionEstado = habitacion.EstadoHabitacion.NombreEstado,
                        HabitacionOcupante = ocupante,
                        HabitacionNumeroCamas = habitacion.NumeroCamas,
                        HabitacionCapacidadPersonas = habitacion.CapacidadPersonas
                    });
                }
            }
            return View(habitacionesConsultadas);

        }
        public IActionResult Lista()
        {
            return View();
        }
        public IActionResult ListaFinalizadas()
        {
            return View();
        }
        public IActionResult ListaEnCurso()
        {
            return View();
        }
        [HttpPost]


        public string ConsultarListaHospitalizaciones(int? status = null)
        {
            try
            {
                Console.WriteLine($"Status recibido: {status}");

                var resultado = _hospitalizacionService.GetListaHospitalizaciones();

                if (status == 1)
                {
                    resultado = resultado
                        .Where(h => h.HospitalizacionEstado == "En curso")
                        .ToList();
                }
                else if (status == 2)
                {
                    resultado = resultado
                        .Where(h => h.HospitalizacionEstado == "Finalizada")
                        .ToList();
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = resultado
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





        [HttpPost]
        public string ConsultarEspecialidades()
        {
            try
            {
                var resultado = _especialidadRepository.GetAll();

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
                    Mensaje = "Error al validar existencia de paciente. " + ex.Message
                });
            }
        }

        public IActionResult Hospitalizar(int? habitacionId, int? consultaId = null, int? emergenciaId = null, int? citaId = null)
        {
            // Si no viene habitacionId pero sí citaId, intentar resolverlo desde la cita
            if (!habitacionId.HasValue && citaId.HasValue)
            {
                var citaParaHabitacion = _citasRepository.GetCita(citaId.Value);
                if (citaParaHabitacion?.HabitacionId != null)
                {
                    habitacionId = citaParaHabitacion.HabitacionId;
                }
            }

            // Validación básica del parámetro
            if (!habitacionId.HasValue)
            {
                TempData["Message"] = "Error de servidor: habitación no especificada.";
                TempData["MessageType"] = "error";
                return RedirectToAction("Habitaciones");
            }

            // Obtener habitación
            var habitacion = _habitacionRepository.Get(habitacionId.Value);

            if (habitacion == null)
            {
                TempData["Message"] = $"La habitación seleccionada (ID {habitacionId.Value}) no existe.";
                TempData["MessageType"] = "error";
                return RedirectToAction("Habitaciones");
            }

            // Protección frente a navegación no cargada / nula
            var categoria = habitacion.CategoriaHabitacion;
            var estado = habitacion.EstadoHabitacion;

            if (categoria == null || estado == null)
            {
                TempData["Message"] = "La habitación seleccionada no tiene configurada su categoría y/o estado.";
                TempData["MessageType"] = "error";
                return RedirectToAction("Habitaciones");
            }

            var model = new HospitalizacionHospitalizarViewModel
            {
                HabitacionId = habitacion.Id,
                HabitacionNumeroNombre = habitacion.NombreNumeroHabitacion,
                HabitacionCategoriaId = habitacion.CategoriaHabitacionId,
                HabitacionCategoria = categoria.NombreCategoria,
                HabitacionEstadoId = habitacion.EstadoHabitacionId,
                HabitacionEstado = estado.NombreEstado,
            };

            // --- Precarga de elementos de consulta ---
            if (consultaId != null)
            {
                var consulta = _consultasRepository.GetConsulta((int)consultaId);
                if (consulta != null)
                {
                    var cita = consulta.Citas ?? new Citas();

                    // Tomar especialidad real de la cita si existe; si no, fallback a Medicina General (1)
                    var especialidadId = cita.EspecialidadId;
                    if (especialidadId == null || especialidadId == 0)
                    {
                        especialidadId = 1;
                    }
                    model.EspecialidadId = especialidadId;

                    model.PacienteId = cita.PacienteId;
                    model.ConsultaId = consultaId;
                    model.Consulta = consulta;
                    model.CodigoSeguro = cita.CodigoDeCita;
                }
            }

            // --- Precarga desde cita (cuando NO hay consulta) ---
            if (consultaId == null && citaId != null)
            {
                var cita = _citasRepository.GetCita(citaId.Value);
                if (cita != null)
                {
                    // OJO: no inventamos consulta
                    var especialidadId = cita.EspecialidadId;
                    if (especialidadId == null || especialidadId == 0)
                    {
                        especialidadId = 1;
                    }
                    model.EspecialidadId = especialidadId;

                    model.PacienteId = cita.PacienteId;
                    model.CodigoSeguro = cita.CodigoDeCita;
                }
            }

            // --- Precarga de elementos de emergencia ---
            if (emergenciaId != null)
            {
                var emergenciaBd = _emergenciaService.Get((int)emergenciaId, true);
                if (emergenciaBd != null)
                {
                    model.PacienteId = emergenciaBd.PacienteId;
                    model.EmergenciaId = emergenciaId;
                }
            }

            model.CitaId = citaId;

            // Inicializar combos
            model.Init(_pacientesRepository);

            return View(model);
        }

        public IActionResult CrearFormCuestionarioPreAnestesico(int HospitalizacionId, int PacienteId)
        {
            var paciente = _pacientesRepository.Get(PacienteId);
            var hospitalizacion = _hospitalizacionRepository.Get(HospitalizacionId);

            var model = new CuestionarioPreAnestesicoViewModel
            {
                // Paciente = paciente,
                // Hospitalizacion = hospitalizacion,
                // PacienteId = PacienteId,
                // HospitalizacionId = HospitalizacionId,
            };

            return View(model);
        }

        public IActionResult CargarCuestionarioPreAnestesico(int pacienteId, int habitacionId, int? citaId = null)
        {
            var paciente = _pacientesRepository.Get(pacienteId);

            var model = new CuestionarioPreAnestesicoViewModel
            {
                pacienteNombre = paciente.Nombre,
                pacienteRegistro = paciente.Id.ToString(),
                pacienteEdad = paciente.Edad.ToString(),
            };

            return View("CrearFormCuestionarioPreAnestesico", model);
        }

        public IActionResult CargarListaChequeo(int pacienteId, int habitacionId, int? citaId = null)
        {
            var paciente = _pacientesRepository.Get(pacienteId);
            var pacienteNombre = paciente != null ? paciente.Nombre : "Paciente no encontrado";
            var FechaNacimiento = paciente != null ? paciente.FechaNacimiento : (DateTime?)null;

            var model = new HospitalizacionHospitalizarViewModel
            {
                PacienteId = pacienteId,
                HabitacionId = habitacionId,
                CitaId = citaId,
                PacienteNombre = pacienteNombre,
                PacienteFechaNacimiento = FechaNacimiento,
            };

            return View("CrearFormListaChequeo", model);
        }




        // [HttpPost]
        // public async Task<string> Hospitalizar(HospitalizacionHospitalizarViewModel model)
        // {
        //     try
        //     {
        //         var user = await _userManager.GetUserAsync(HttpContext.User);

        //         #region Especialidad

        //         // Normalización: si viene null o 0, asignar Medicina General (Id = 1)
        //         // Esto evita violación de FK cuando el frontend envía 0 (no null).
        //         if (!model.EspecialidadId.HasValue || model.EspecialidadId.Value == 0)
        //         {
        //             model.EspecialidadId = 1;
        //         }

        //         #endregion

        //         #region Paciente

        //         Paciente paciente;
        //         if (model.PacienteId == null)
        //         {
        //             paciente = new Paciente
        //             {
        //                 FechaRegistro = DateTime.Today,
        //                 Nombre = model.PacienteNombre,
        //                 TipoPacienteId = (int)TipoPacienteEnum.Nuevo,
        //                 EstadoPacienteId = (int)EstadoPacienteEnum.Activo,
        //                 Dpi = model.PacienteDpi,
        //                 Telefono = model.PacienteTelefono,
        //                 FechaNacimiento = model.PacienteFechaNacimiento
        //             };
        //             paciente = _pacientesRepository.Add(paciente);
        //             model.PacienteId = paciente.Id;
        //         }
        //         else
        //         {
        //             paciente = _pacientesRepository.Get((int)model.PacienteId);
        //             paciente.Dpi = model.PacienteDpi;
        //             paciente.Telefono = model.PacienteTelefono;
        //             paciente.FechaNacimiento = model.PacienteFechaNacimiento;
        //             _pacientesRepository.Update(paciente);
        //         }

        //         #endregion

        //         var fechas = model.Periodo.Split("-");
        //         var fechaInicio = Convert.ToDateTime(fechas[0], culture);
        //         var fechaFin = Convert.ToDateTime(fechas[1], culture);

        //         // Variables para la cita y consulta
        //         Citas citaCreada = null;
        //         Consulta consultaCreada = null;

        //         #region CREAR CITA Y CONSULTA SI VIENE DE EMERGENCIA

        //         if (model.EmergenciaId != null && model.EmergenciaId != 0 && model.ConsultaId == null)
        //         {
        //             // Obtener datos de la emergencia
        //             var emergencia = _emergenciasRepo.Get((int)model.EmergenciaId);

        //             // Crear nueva cita basada en la emergencia
        //             citaCreada = new Citas()
        //             {
        //                 EspecialidadId = model.EspecialidadId,
        //                 HabitacionId = model.HabitacionId,
        //                 CategoriaHabitacionId = model.HabitacionCategoriaId,
        //                 Paciente = paciente,
        //                 PacienteId = paciente.Id,
        //                 SucursalId = 6, // Usar sucursal de emergencia o default
        //                 Bloqueada = false,
        //                 Eliminado = false,
        //                 Finalizada = false,
        //                 CitaTipoAtencion = "Hospitalización",
        //                 EmpleadoId = user.EmpleadoId,
        //                 User = user,
        //                 FechaInicio = fechaInicio,
        //                 FechaFinal = fechaInicio.AddHours(1), // Duración default de 1 hora
        //                 Motivo = "Hospitalización por emergencia",
        //                 Edad = paciente.Edad,
        //                 NombreEncargado = paciente.NombreEncargado,
        //                 DPIEncargado = paciente.DPIEncargado,
        //                 EstadoCita = "normal",
        //                 EsMenorDeEdad = paciente.Edad != null && paciente.Edad < 18,
        //                 CodigoAutorizacion = "",
        //                 CodigoDeCita = model.CodigoSeguro ?? "SIN SEGURO",
        //                 EstadoTurno = null,
        //                 FechaHoraInicioTurno = null,
        //                 NumeroTurno = null,
        //                 NivelPrioridadCita = "ALTA", // Prioridad alta por venir de emergencia

        //                 // Datos del responsable si existen
        //                 ResponsableNit = paciente.ResponsableNit,
        //                 ResponsableNombre = paciente.ResponsableNombre,
        //                 ResponsableDireccion = paciente.ResponsableDireccion,
        //                 ResponsableCorreo = paciente.ResponsableCorreo,
        //                 ResponsableTelefono = paciente.ResponsableTelefono,
        //                 ResponsablePasaporte = paciente.ResponsablePasaporte,
        //                 ResponsableDPI = paciente.ResponsableDPI,

        //                 // Datos de padres si existen
        //                 NombrePadre = paciente.NombrePadre,
        //                 FechaNacimientoPadre = paciente.FechaNacimientoPadre,
        //                 EdadPadre = paciente.EdadPadre,
        //                 DPIPadre = paciente.DPIPadre,
        //                 DireccionPadre = paciente.DireccionPadre,
        //                 TelefonoPadre = paciente.TelefonoPadre,
        //                 CorreoPadre = paciente.CorreoPadre,
        //                 OcupacionPadre = paciente.OcupacionPadre,
        //                 EmpresaPadre = paciente.EmpresaPadre,
        //                 TelefonoEmpresaPadre = paciente.TelefonoEmpresaPadre,
        //                 DireccionEmpresaPadre = paciente.DireccionEmpresaPadre,

        //                 // Datos de madre si existen
        //                 NombreMadre = paciente.NombreMadre,
        //                 FechaNacimientoMadre = paciente.FechaNacimientoMadre,
        //                 EdadMadre = paciente.EdadMadre,
        //                 DPIMadre = paciente.DPIMadre,
        //                 DireccionMadre = paciente.DireccionMadre,
        //                 TelefonoMadre = paciente.TelefonoMadre,
        //                 CorreoMadre = paciente.CorreoMadre,
        //                 OcupacionMadre = paciente.OcupacionMadre,
        //                 EmpresaMadre = paciente.EmpresaMadre,
        //                 TelefonoEmpresaMadre = paciente.TelefonoEmpresaMadre,
        //                 DireccionEmpresaMadre = paciente.DireccionEmpresaMadre,

        //                 // Datos del acompañante si existen
        //                 AcompananteNombre = paciente.AcompananteNombre,
        //                 AcompananteRelacion = paciente.AcompananteRelacion,
        //                 AcompananteTelefono = paciente.AcompananteTelefono,
        //                 AcompananteDPI = paciente.AcompananteDPI,
        //                 AcompananteDireccion = paciente.AcompananteDireccion,
        //                 AcompananteCorreo = paciente.AcompananteCorreo,
        //                 AcompananteOcupacion = paciente.AcompananteOcupacion,
        //                 AcompananteEmpresa = paciente.AcompananteEmpresa,
        //                 AcompananteTelefonoEmpresa = paciente.AcompananteTelefonoEmpresa,
        //                 AcompananteDireccionEmpresa = paciente.AcompananteDireccionEmpresa,
        //                 AcompananteTipoIdentificacion = paciente.AcompananteTipoIdentificacion,
        //                 AcompananteFechaNacimiento = paciente.AcompananteFechaNacimiento,
        //                 AcompananteEdad = paciente.AcompananteEdad,
        //                 AcompananteFechaIngreso = paciente.AcompananteFechaIngreso,
        //                 AcompananteAntiguedad = paciente.AcompananteAntiguedad
        //             };

        //             _citasRepository.Add(citaCreada);

        //             // Crear nueva consulta basada en la cita recién creada
        //             consultaCreada = new Consulta
        //             {
        //                 CitasId = citaCreada.Id,
        //                 HistoriaId = null,
        //                 HistoriaPediatriaId = null,
        //                 ExamenFisicoId = null,
        //                 ExamenFisicoPediatriaId = null,
        //                 ConsultaExamenFisicoGinecologiaId = null,
        //                 ConsultaAntPatologicosGinecologiaId = null,
        //                 ConsultaAntNoPatologicosGinecologiaId = null,
        //                 ConsultaAntNoPatologicosObstetriciaId = null,
        //                 ObservacionesAdicionales = "Consulta generada por hospitalización desde emergencia",
        //                 CostoConsulta = 0,
        //                 ConsultaMotivo = "Hospitalización por emergencia",
        //                 FechaYHoraInicioConsulta = fechaInicio,
        //                 FechaProximaConsulta = null,
        //                 ProximaCitaAgendada = false,
        //                 Archivado = false,
        //                 EstadoPagoConsultaId = (int)EstadoPagoConsultaEnum.Pendiente,
        //                 TipoConsulta = "Hospitalización",
        //                 TipoReferencia = "Emergencia",
        //                 MedicoReferido = null,

        //                 // Datos familiares copiados del paciente
        //                 NombrePadre = paciente.NombrePadre,
        //                 FechaNacimientoPadre = paciente.FechaNacimientoPadre,
        //                 EdadPadre = paciente.EdadPadre,
        //                 DPIPadre = paciente.DPIPadre,
        //                 DireccionPadre = paciente.DireccionPadre,
        //                 TelefonoPadre = paciente.TelefonoPadre,
        //                 CorreoPadre = paciente.CorreoPadre,
        //                 OcupacionPadre = paciente.OcupacionPadre,
        //                 EmpresaPadre = paciente.EmpresaPadre,
        //                 TelefonoEmpresaPadre = paciente.TelefonoEmpresaPadre,
        //                 DireccionEmpresaPadre = paciente.DireccionEmpresaPadre,

        //                 NombreMadre = paciente.NombreMadre,
        //                 FechaNacimientoMadre = paciente.FechaNacimientoMadre,
        //                 EdadMadre = paciente.EdadMadre,
        //                 DPIMadre = paciente.DPIMadre,
        //                 DireccionMadre = paciente.DireccionMadre,
        //                 TelefonoMadre = paciente.TelefonoMadre,
        //                 CorreoMadre = paciente.CorreoMadre,
        //                 OcupacionMadre = paciente.OcupacionMadre,
        //                 EmpresaMadre = paciente.EmpresaMadre,
        //                 TelefonoEmpresaMadre = paciente.TelefonoEmpresaMadre,
        //                 DireccionEmpresaMadre = paciente.DireccionEmpresaMadre,

        //                 AcompananteNombre = paciente.AcompananteNombre,
        //                 AcompananteDPI = paciente.AcompananteDPI,
        //                 AcompananteTelefono = paciente.AcompananteTelefono,

        //                 GinecologiaConsultaMotivo = "N/A",
        //                 ResponsableNit = paciente.ResponsableNit,
        //                 ResponsableNombre = paciente.ResponsableNombre,
        //                 ResponsableDireccion = paciente.ResponsableDireccion,
        //                 ResponsableCorreo = paciente.ResponsableCorreo
        //             };

        //             _consultasRepository.Add(consultaCreada);
        //             // Actualizar el modelo para usar la consulta recién creada
        //             model.ConsultaId = consultaCreada.Id;
        //         }

        //         #endregion

        //         var hospitalizacion = new Hospitalizacion
        //         {
        //             HabitacionId = model.HabitacionId,
        //             PacienteId = (int)model.PacienteId,
        //             FechaInicio = fechaInicio,
        //             FechaFin = fechaFin,
        //             Eliminada = false,
        //             CategoriaHabitacionTarifaId = model.TarifaId,
        //             Finalizada = false,
        //             Pagada = false,
        //             Observaciones = model.Observaciones,
        //             // Ya viene normalizado (null/0 => 1). Se mantiene estructura original.
        //             EspecialidadId = model.EspecialidadId ?? 1,
        //             UrlArchivoConsentimiento = model.UrlArchivoConsentimiento
        //         };
        //         hospitalizacion = _hospitalizacionRepository.Add(hospitalizacion);


        //         // Copiar servicios de la cita a la hospitalización (si existe una cita asociada)
        //         if (model.CitaId.HasValue && model.CitaId > 0)
        //         {
        //             var cita = _citasRepository.GetCita(model.CitaId.Value);
        //             if (cita != null && cita.CitasServicios != null)
        //             {
        //                 var userActual = await _userManager.GetUserAsync(HttpContext.User);

        //                 foreach (var servicioCita in cita.CitasServicios.Where(s => !s.Eliminado))
        //                 {
        //                     // Buscar el registro ServicioPrecio que coincida con ServicioId y PrecioId
        //                     var servicioPrecio = await _db.ServiciosPrecios
        //                         .FirstOrDefaultAsync(sp => sp.ServicioId == servicioCita.ServicioId
        //                                                 && sp.PrecioId == servicioCita.PrecioId);

        //                     // Si no existe, créalo con los datos de la cita
        //                     if (servicioPrecio == null)
        //                     {
        //                         servicioPrecio = new ServicioPrecio
        //                         {
        //                             ServicioId = servicioCita.ServicioId,
        //                             PrecioId = (int)servicioCita.PrecioId,
        //                             Valor = servicioCita.PrecioValor ?? 0,
        //                             Activar = true
        //                         };
        //                         _db.ServiciosPrecios.Add(servicioPrecio);
        //                         await _db.SaveChangesAsync(); // Guardar para obtener el Id generado
        //                     }

        //                     // Crear el detalle de servicio para la hospitalización
        //                     var nuevoServicio = new HospitalizacionServicio
        //                     {
        //                         HospitalizacionId = hospitalizacion.Id,
        //                         ServicioId = servicioCita.ServicioId,
        //                         Cantidad = servicioCita.Cantidad ?? 1,
        //                         Precio = servicioCita.PrecioValor ?? 0,
        //                         PrecioServicioId = servicioPrecio.Id,   // ✅ Id válido
        //                         UsuarioCreaId = userActual.Id,
        //                         FechaHoraAplicacion = DateTime.Now,
        //                         Aplicado = false,
        //                         Eliminado = false
        //                     };

        //                     _db.HospitalizacionesServicios.Add(nuevoServicio);
        //                 }

        //                 // Guardar todos los servicios de una vez
        //                 await _db.SaveChangesAsync();
        //             }
        //         }



        //         #region HABITACION - REGISTRAR HABITACION COMO OCUPADA

        //         var habitacion = _habitacionRepository.Get(model.HabitacionId);
        //         habitacion.EstadoHabitacionId = (int)EstadoHabitacionEnum.Ocupada;
        //         _habitacionRepository.Update(habitacion);

        //         #endregion

        //         #region REGISTRAR CONSULTA COMO HOSPITALIZADA

        //         if (model.ConsultaId != null && model.ConsultaId != 0)
        //         {
        //             var consulta = _consultasRepository.GetConsulta((int)model.ConsultaId, false);
        //             if (consulta != null)
        //             {
        //                 consulta.Hospitalizado = true;
        //                 consulta.HospitalizacionId = hospitalizacion.Id;
        //                 _consultasRepository.Update(consulta);
        //             }
        //         }

        //         #endregion

        //         #region MARCAR CITA COMO HOSPITALIZADA (oculta el botón en el calendario)

        //         if (model.CitaId != null && model.CitaId > 0)
        //         {
        //             var citaIdVal = (int)model.CitaId;
        //             var citaOrigen = _citasRepository.GetCita(citaIdVal);
        //             if (citaOrigen != null)
        //             {
        //                 citaOrigen.EstadoCita = "asistida";
        //                 citaOrigen.Finalizada = true;

        //                 // Migrar equipo quirurgico de Citas -> NotaOperatoria
        //                 var hayEquipo = !string.IsNullOrEmpty(citaOrigen.Anestesista)
        //                              || !string.IsNullOrEmpty(citaOrigen.PrimerAyudante)
        //                              || !string.IsNullOrEmpty(citaOrigen.Instrumentista)
        //                              || !string.IsNullOrEmpty(citaOrigen.Circulante);
        //                 if (hayEquipo)
        //                 {
        //                     var cirujanoNombre = "";
        //                     if (citaOrigen.EmpleadoId.HasValue)
        //                     {
        //                         var empCirujano = _empleadoRepository.Get(citaOrigen.EmpleadoId.Value);
        //                         cirujanoNombre = empCirujano?.NombreYApellidos ?? "";
        //                     }
        //                     var notaEquipo = new NotaOperatoria
        //                     {
        //                         HospitalizacionId = hospitalizacion.Id,
        //                         FechaRegistro = DateTime.Now,
        //                         UserId = user.Id,
        //                         FechaOperacion = null,
        //                         Cirujano = cirujanoNombre,
        //                         PrimerAyudante = citaOrigen.PrimerAyudante ?? "",
        //                         SegundoAyudante = citaOrigen.SegundoAyudante ?? "",
        //                         Anestesista = citaOrigen.Anestesista ?? "",
        //                         Instrumentista = citaOrigen.Instrumentista ?? "",
        //                         Circulante = citaOrigen.Circulante ?? "",
        //                         OperacionEfectuada = citaOrigen.Procedimiento ?? "",
        //                         DiagnosticoPreOperatorio = "",
        //                         DiagnosticoPostOperatorio = "",
        //                         HallazgosTransOperatorios = ""
        //                     };
        //                     _notaOperatoriaRepository.AddNotaOperatoria(notaEquipo);

        //                     // Limpiar equipo quirurgico de Citas (ya migrado)
        //                     citaOrigen.Anestesista = null;
        //                     citaOrigen.PrimerAyudante = null;
        //                     citaOrigen.SegundoAyudante = null;
        //                     citaOrigen.Instrumentista = null;
        //                     citaOrigen.Circulante = null;
        //                 }

        //                 _citasRepository.Update(citaOrigen);
        //             }
        //         }

        //         #endregion

        //         #region REGISTRAR EMERGENCIA COMO INGRESADA

        //         if (model.EmergenciaId != null && model.EmergenciaId != 0)
        //         {
        //             var emergencia = _emergenciasRepo.Get((int)model.EmergenciaId);
        //             if (emergencia != null)
        //             {
        //                 emergencia.Ingresada = true;
        //                 emergencia.HospitalizacionId = hospitalizacion.Id;
        //                 _emergenciasRepo.Update(emergencia);
        //             }
        //         }

        //         #endregion

        //         #region AGREGAR HOSPITALIZACION A CUENTA POR COBRAR

        //         var nuevoDetalleCuenta = new DetalleCuentaPorCobrar
        //         {
        //             HospitalizacionId = hospitalizacion.Id,
        //             Descripcion = "Hospitalizacion"
        //         };
        //         var cuenta = _cuentasPorCobrarRepository
        //             .GetUltimaCuentaPendientePaciente((int)model.PacienteId);
        //         if (cuenta == null)
        //         {
        //             var noches = (fechaFin - fechaInicio).Days;
        //             var nuevaCuenta = new CuentaPorCobrar
        //             {
        //                 PacienteId = model.PacienteId,
        //                 Pagada = false,
        //                 Valor = (model.TarifaValor * noches) == 0 ? model.TarifaValor : (model.TarifaValor * noches),
        //                 Eliminada = false
        //             };
        //             cuenta = _cuentasPorCobrarRepository.Add(nuevaCuenta);
        //         }
        //         cuenta.DetallesCuentaPorCobrar.Add(nuevoDetalleCuenta);
        //         _cuentasPorCobrarRepository.Update(cuenta);

        //         #endregion

        //         return JsonSerializer.Serialize(new
        //         {
        //             Exitoso = true,
        //             HospitalizacionId = hospitalizacion.Id,
        //             CitaId = citaCreada?.Id,
        //             ConsultaId = consultaCreada?.Id
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return JsonSerializer.Serialize(new
        //         {
        //             Exitoso = false,
        //             Mensaje = "Error al registrar hospitalizacion. " + ex.InnerException?.Message ?? ex.Message
        //         });
        //     }
        // }


        [HttpPost]
        public async Task<string> Hospitalizar(HospitalizacionHospitalizarViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                #region Especialidad

                // Normalización: si viene null o 0, asignar Medicina General (Id = 1)
                // Esto evita violación de FK cuando el frontend envía 0 (no null).
                if (!model.EspecialidadId.HasValue || model.EspecialidadId.Value == 0)
                {
                    model.EspecialidadId = 1;
                }

                #endregion

                #region Paciente

                Paciente paciente;
                if (model.PacienteId == null)
                {
                    paciente = new Paciente
                    {
                        FechaRegistro = DateTime.Today,
                        Nombre = model.PacienteNombre,
                        TipoPacienteId = (int)TipoPacienteEnum.Nuevo,
                        EstadoPacienteId = (int)EstadoPacienteEnum.Activo,
                        Dpi = model.PacienteDpi,
                        Telefono = model.PacienteTelefono,
                        FechaNacimiento = model.PacienteFechaNacimiento
                    };
                    paciente = _pacientesRepository.Add(paciente);
                    model.PacienteId = paciente.Id;
                }
                else
                {
                    paciente = _pacientesRepository.Get((int)model.PacienteId);
                    paciente.Dpi = model.PacienteDpi;
                    paciente.Telefono = model.PacienteTelefono;
                    paciente.FechaNacimiento = model.PacienteFechaNacimiento;
                    _pacientesRepository.Update(paciente);
                }

                #endregion

                var fechas = model.Periodo.Split("-");
                var fechaInicio = Convert.ToDateTime(fechas[0], culture);
                var fechaFin = Convert.ToDateTime(fechas[1], culture);

                // Variables para la cita y consulta
                Citas citaCreada = null;
                Consulta consultaCreada = null;

                #region CREAR CITA Y CONSULTA SI VIENE DE EMERGENCIA

                if (model.EmergenciaId != null && model.EmergenciaId != 0 && model.ConsultaId == null)
                {
                    // Obtener datos de la emergencia
                    var emergencia = _emergenciasRepo.Get((int)model.EmergenciaId);

                    // Crear nueva cita basada en la emergencia
                    citaCreada = new Citas()
                    {
                        EspecialidadId = model.EspecialidadId,
                        HabitacionId = model.HabitacionId,
                        CategoriaHabitacionId = model.HabitacionCategoriaId,
                        Paciente = paciente,
                        PacienteId = paciente.Id,
                        SucursalId = 6, // Usar sucursal de emergencia o default
                        Bloqueada = false,
                        Eliminado = false,
                        Finalizada = false,
                        CitaTipoAtencion = "Hospitalización",
                        EmpleadoId = user.EmpleadoId,
                        User = user,
                        FechaInicio = fechaInicio,
                        FechaFinal = fechaInicio.AddHours(1), // Duración default de 1 hora
                        Motivo = "Hospitalización por emergencia",
                        Edad = paciente.Edad,
                        NombreEncargado = paciente.NombreEncargado,
                        DPIEncargado = paciente.DPIEncargado,
                        EstadoCita = "normal",
                        EsMenorDeEdad = paciente.Edad != null && paciente.Edad < 18,
                        CodigoAutorizacion = "",
                        CodigoDeCita = model.CodigoSeguro ?? "SIN SEGURO",
                        EstadoTurno = null,
                        FechaHoraInicioTurno = null,
                        NumeroTurno = null,
                        NivelPrioridadCita = "ALTA",

                        // Datos del responsable si existen
                        ResponsableNit = paciente.ResponsableNit,
                        ResponsableNombre = paciente.ResponsableNombre,
                        ResponsableDireccion = paciente.ResponsableDireccion,
                        ResponsableCorreo = paciente.ResponsableCorreo,
                        ResponsableTelefono = paciente.ResponsableTelefono,
                        ResponsablePasaporte = paciente.ResponsablePasaporte,
                        ResponsableDPI = paciente.ResponsableDPI,

                        // Datos de padres si existen
                        NombrePadre = paciente.NombrePadre,
                        FechaNacimientoPadre = paciente.FechaNacimientoPadre,
                        EdadPadre = paciente.EdadPadre,
                        DPIPadre = paciente.DPIPadre,
                        DireccionPadre = paciente.DireccionPadre,
                        TelefonoPadre = paciente.TelefonoPadre,
                        CorreoPadre = paciente.CorreoPadre,
                        OcupacionPadre = paciente.OcupacionPadre,
                        EmpresaPadre = paciente.EmpresaPadre,
                        TelefonoEmpresaPadre = paciente.TelefonoEmpresaPadre,
                        DireccionEmpresaPadre = paciente.DireccionEmpresaPadre,

                        // Datos de madre si existen
                        NombreMadre = paciente.NombreMadre,
                        FechaNacimientoMadre = paciente.FechaNacimientoMadre,
                        EdadMadre = paciente.EdadMadre,
                        DPIMadre = paciente.DPIMadre,
                        DireccionMadre = paciente.DireccionMadre,
                        TelefonoMadre = paciente.TelefonoMadre,
                        CorreoMadre = paciente.CorreoMadre,
                        OcupacionMadre = paciente.OcupacionMadre,
                        EmpresaMadre = paciente.EmpresaMadre,
                        TelefonoEmpresaMadre = paciente.TelefonoEmpresaMadre,
                        DireccionEmpresaMadre = paciente.DireccionEmpresaMadre,

                        // Datos del acompañante si existen
                        AcompananteNombre = paciente.AcompananteNombre,
                        AcompananteRelacion = paciente.AcompananteRelacion,
                        AcompananteTelefono = paciente.AcompananteTelefono,
                        AcompananteDPI = paciente.AcompananteDPI,
                        AcompananteDireccion = paciente.AcompananteDireccion,
                        AcompananteCorreo = paciente.AcompananteCorreo,
                        AcompananteOcupacion = paciente.AcompananteOcupacion,
                        AcompananteEmpresa = paciente.AcompananteEmpresa,
                        AcompananteTelefonoEmpresa = paciente.AcompananteTelefonoEmpresa,
                        AcompananteDireccionEmpresa = paciente.AcompananteDireccionEmpresa,
                        AcompananteTipoIdentificacion = paciente.AcompananteTipoIdentificacion,
                        AcompananteFechaNacimiento = paciente.AcompananteFechaNacimiento,
                        AcompananteEdad = paciente.AcompananteEdad,
                        AcompananteFechaIngreso = paciente.AcompananteFechaIngreso,
                        AcompananteAntiguedad = paciente.AcompananteAntiguedad
                    };

                    _citasRepository.Add(citaCreada);

                    // Crear nueva consulta basada en la cita recién creada
                    consultaCreada = new Consulta
                    {
                        CitasId = citaCreada.Id,
                        HistoriaId = null,
                        HistoriaPediatriaId = null,
                        ExamenFisicoId = null,
                        ExamenFisicoPediatriaId = null,
                        ConsultaExamenFisicoGinecologiaId = null,
                        ConsultaAntPatologicosGinecologiaId = null,
                        ConsultaAntNoPatologicosGinecologiaId = null,
                        ConsultaAntNoPatologicosObstetriciaId = null,
                        ObservacionesAdicionales = "Consulta generada por hospitalización desde emergencia",
                        CostoConsulta = 0,
                        ConsultaMotivo = "Hospitalización por emergencia",
                        FechaYHoraInicioConsulta = fechaInicio,
                        FechaProximaConsulta = null,
                        ProximaCitaAgendada = false,
                        Archivado = false,
                        EstadoPagoConsultaId = (int)EstadoPagoConsultaEnum.Pendiente,
                        TipoConsulta = "Hospitalización",
                        TipoReferencia = "Emergencia",
                        MedicoReferido = null,

                        // Datos familiares copiados del paciente
                        NombrePadre = paciente.NombrePadre,
                        FechaNacimientoPadre = paciente.FechaNacimientoPadre,
                        EdadPadre = paciente.EdadPadre,
                        DPIPadre = paciente.DPIPadre,
                        DireccionPadre = paciente.DireccionPadre,
                        TelefonoPadre = paciente.TelefonoPadre,
                        CorreoPadre = paciente.CorreoPadre,
                        OcupacionPadre = paciente.OcupacionPadre,
                        EmpresaPadre = paciente.EmpresaPadre,
                        TelefonoEmpresaPadre = paciente.TelefonoEmpresaPadre,
                        DireccionEmpresaPadre = paciente.DireccionEmpresaPadre,

                        NombreMadre = paciente.NombreMadre,
                        FechaNacimientoMadre = paciente.FechaNacimientoMadre,
                        EdadMadre = paciente.EdadMadre,
                        DPIMadre = paciente.DPIMadre,
                        DireccionMadre = paciente.DireccionMadre,
                        TelefonoMadre = paciente.TelefonoMadre,
                        CorreoMadre = paciente.CorreoMadre,
                        OcupacionMadre = paciente.OcupacionMadre,
                        EmpresaMadre = paciente.EmpresaMadre,
                        TelefonoEmpresaMadre = paciente.TelefonoEmpresaMadre,
                        DireccionEmpresaMadre = paciente.DireccionEmpresaMadre,

                        AcompananteNombre = paciente.AcompananteNombre,
                        AcompananteDPI = paciente.AcompananteDPI,
                        AcompananteTelefono = paciente.AcompananteTelefono,

                        GinecologiaConsultaMotivo = "N/A",
                        ResponsableNit = paciente.ResponsableNit,
                        ResponsableNombre = paciente.ResponsableNombre,
                        ResponsableDireccion = paciente.ResponsableDireccion,
                        ResponsableCorreo = paciente.ResponsableCorreo
                    };

                    _consultasRepository.Add(consultaCreada);
                    // Actualizar el modelo para usar la consulta recién creada
                    model.ConsultaId = consultaCreada.Id;
                }

                #endregion

                var hospitalizacion = new Hospitalizacion
                {
                    HabitacionId = model.HabitacionId,
                    PacienteId = (int)model.PacienteId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Eliminada = false,
                    CategoriaHabitacionTarifaId = model.TarifaId,
                    Finalizada = false,
                    Pagada = false,
                    Observaciones = model.Observaciones,
                    EspecialidadId = model.EspecialidadId ?? 1,
                    UrlArchivoConsentimiento = model.UrlArchivoConsentimiento
                };
                hospitalizacion = _hospitalizacionRepository.Add(hospitalizacion);


                // ============================================================
                // COPIAR SERVICIOS DESDE LA CITA (Sala de Operaciones)
                // ============================================================
                if (model.CitaId.HasValue && model.CitaId > 0)
                {
                    var cita = _citasRepository.GetCita(model.CitaId.Value);
                    if (cita?.CitasServicios != null)
                    {
                        foreach (var servicioCita in cita.CitasServicios.Where(s => !s.Eliminado))
                        {
                            var servicioPrecio = await _db.ServiciosPrecios
                                .FirstOrDefaultAsync(sp => sp.ServicioId == servicioCita.ServicioId
                                                        && sp.PrecioId == servicioCita.PrecioId);

                            if (servicioPrecio == null)
                            {
                                servicioPrecio = new ServicioPrecio
                                {
                                    ServicioId = servicioCita.ServicioId,
                                    PrecioId = (int)servicioCita.PrecioId,
                                    Valor = servicioCita.PrecioValor ?? 0,
                                    Activar = true
                                };
                                _db.ServiciosPrecios.Add(servicioPrecio);
                                await _db.SaveChangesAsync();
                            }

                            _db.HospitalizacionesServicios.Add(new HospitalizacionServicio
                            {
                                HospitalizacionId = hospitalizacion.Id,
                                ServicioId = servicioCita.ServicioId,
                                Cantidad = servicioCita.Cantidad ?? 1,
                                Precio = servicioCita.PrecioValor ?? 0,
                                PrecioServicioId = servicioPrecio.Id,
                                UsuarioCreaId = user.Id,
                                FechaHoraAplicacion = DateTime.Now,
                                Aplicado = false,
                                Eliminado = false
                            });
                        }
                        await _db.SaveChangesAsync();
                    }
                }

                // ============================================================
                // COPIAR SERVICIOS DESDE CONSULTA EXTERNA
                // ============================================================
                if (model.ConsultaId.HasValue && model.ConsultaId > 0)
                {
                    // Usar los métodos del repositorio que ya cargan correctamente los datos
                    var serviciosConsulta = _consultasRepository.GetServiciosAgregados(model.ConsultaId.Value);
                    var examenesConsulta = _consultasRepository.GetExamenesAgregadosConsulta(model.ConsultaId.Value);

                    // --- Servicios desde consulta externa ---
                    if (serviciosConsulta?.Any() == true)
                    {
                        foreach (var svc in serviciosConsulta)
                        {
                            // Obtener el precio adecuado según el código de seguro
                            var preciosServicio = _serviciosRepository.GetPrecioServicioById(svc.ServicioId);
                            string codigoSeguro = model.CodigoSeguro ?? "Sin seguro";
                            var precioSeleccionado = preciosServicio
                                .FirstOrDefault(p => string.Equals(p.Precio.NombrePrecio, codigoSeguro, StringComparison.OrdinalIgnoreCase))
                                ?? preciosServicio
                                .FirstOrDefault(p => string.Equals(p.Precio.NombrePrecio, "Sin seguro", StringComparison.OrdinalIgnoreCase));

                            decimal precioValor = precioSeleccionado?.Valor ?? 0;
                            int precioId = precioSeleccionado?.PrecioId ?? 1;

                            // Buscar o crear ServicioPrecio
                            var servicioPrecio = await _db.ServiciosPrecios
                                .FirstOrDefaultAsync(sp => sp.ServicioId == svc.ServicioId && sp.PrecioId == precioId);
                            if (servicioPrecio == null)
                            {
                                servicioPrecio = new ServicioPrecio
                                {
                                    ServicioId = svc.ServicioId,
                                    PrecioId = precioId,
                                    Valor = precioValor,
                                    Activar = true
                                };
                                _db.ServiciosPrecios.Add(servicioPrecio);
                                await _db.SaveChangesAsync();
                            }

                            // Agregar a la hospitalización
                            _db.HospitalizacionesServicios.Add(new HospitalizacionServicio
                            {
                                HospitalizacionId = hospitalizacion.Id,
                                ServicioId = svc.ServicioId,
                                Cantidad = svc.Cantidad,  // ya es int
                                Precio = precioValor,
                                PrecioServicioId = servicioPrecio.Id,
                                UsuarioCreaId = user.Id,
                                FechaHoraAplicacion = DateTime.Now,
                                Aplicado = false,
                                Eliminado = false
                            });
                        }
                        await _db.SaveChangesAsync();
                    }

                    // --- Exámenes desde consulta externa ---
                    if (examenesConsulta?.Any() == true)
                    {
                        foreach (var exam in examenesConsulta)
                        {
                            // Crear el objeto Examen
                            var nuevoExamen = new Examen
                            {
                                EstadoExamenId = (int)EstadoExamenEnum.Solictiado,
                                FechaRealizacion = DateTime.Now,
                                PacienteId = (int)model.PacienteId,
                                UsuarioSolicita = user.Id,
                                Eliminado = false,
                                DetalleExamenes = new List<DetalleExamen>()
                            };

                            // Obtener el precio según el código de seguro
                            var preciosExamen = _laboratorioClinicoRepository.GetPreciosExamen(exam.ExamenLabClinicoId ?? 0);
                            string codigoSeguro = model.CodigoSeguro ?? "Sin seguro";
                            var precioExamenSeleccionado = preciosExamen
                                .FirstOrDefault(p => string.Equals(p.Precio.NombrePrecio, codigoSeguro, StringComparison.OrdinalIgnoreCase))
                                ?? preciosExamen
                                .FirstOrDefault(p => string.Equals(p.Precio.NombrePrecio, "Sin seguro", StringComparison.OrdinalIgnoreCase));

                            decimal valorExamen = precioExamenSeleccionado?.PrecioValor ?? 0;
                            int? precioIdExamen = precioExamenSeleccionado?.PrecioId;

                            // Crear el detalle
                            nuevoExamen.DetalleExamenes.Add(new DetalleExamen
                            {
                                ExamenLabClinicoId = exam.ExamenLabClinicoId ?? 0,   // corregido
                                Cantidad = exam.Cantidad,
                                PrecioId = precioIdExamen ?? 1,
                                PrecioValor = valorExamen,
                                Descuento = 0,
                                Subtotal = valorExamen,
                                Total = valorExamen,
                            });

                            // Agregar a la hospitalización
                            _db.HospitalizacionesExamenes.Add(new HospitalizacionExamen
                            {
                                HospitalizacionId = hospitalizacion.Id,
                                FechaHora = DateTime.Now,
                                Examen = nuevoExamen,
                                Eliminado = false
                            });
                        }
                        await _db.SaveChangesAsync();
                    }
                }


                #region HABITACION - REGISTRAR HABITACION COMO OCUPADA

                var habitacion = _habitacionRepository.Get(model.HabitacionId);
                habitacion.EstadoHabitacionId = (int)EstadoHabitacionEnum.Ocupada;
                _habitacionRepository.Update(habitacion);

                #endregion

                #region REGISTRAR CONSULTA COMO HOSPITALIZADA

                if (model.ConsultaId != null && model.ConsultaId != 0)
                {
                    var consulta = _consultasRepository.GetConsulta((int)model.ConsultaId, false);
                    if (consulta != null)
                    {
                        consulta.Hospitalizado = true;
                        consulta.HospitalizacionId = hospitalizacion.Id;
                        _consultasRepository.Update(consulta);
                    }
                }

                #endregion

                #region MARCAR CITA COMO HOSPITALIZADA (oculta el botón en el calendario)

                if (model.CitaId != null && model.CitaId > 0)
                {
                    var citaIdVal = (int)model.CitaId;
                    var citaOrigen = _citasRepository.GetCita(citaIdVal);
                    if (citaOrigen != null)
                    {
                        citaOrigen.EstadoCita = "asistida";
                        citaOrigen.Finalizada = true;

                        // Migrar equipo quirurgico de Citas -> NotaOperatoria
                        var hayEquipo = !string.IsNullOrEmpty(citaOrigen.Anestesista)
                                     || !string.IsNullOrEmpty(citaOrigen.PrimerAyudante)
                                     || !string.IsNullOrEmpty(citaOrigen.Instrumentista)
                                     || !string.IsNullOrEmpty(citaOrigen.Circulante);
                        if (hayEquipo)
                        {
                            var cirujanoNombre = "";
                            if (citaOrigen.EmpleadoId.HasValue)
                            {
                                var empCirujano = _empleadoRepository.Get(citaOrigen.EmpleadoId.Value);
                                cirujanoNombre = empCirujano?.NombreYApellidos ?? "";
                            }
                            var notaEquipo = new NotaOperatoria
                            {
                                HospitalizacionId = hospitalizacion.Id,
                                FechaRegistro = DateTime.Now,
                                UserId = user.Id,
                                FechaOperacion = null,
                                Cirujano = cirujanoNombre,
                                PrimerAyudante = citaOrigen.PrimerAyudante ?? "",
                                SegundoAyudante = citaOrigen.SegundoAyudante ?? "",
                                Anestesista = citaOrigen.Anestesista ?? "",
                                Instrumentista = citaOrigen.Instrumentista ?? "",
                                Circulante = citaOrigen.Circulante ?? "",
                                OperacionEfectuada = citaOrigen.Procedimiento ?? "",
                                DiagnosticoPreOperatorio = "",
                                DiagnosticoPostOperatorio = "",
                                HallazgosTransOperatorios = ""
                            };
                            _notaOperatoriaRepository.AddNotaOperatoria(notaEquipo);

                            // Limpiar equipo quirurgico de Citas (ya migrado)
                            citaOrigen.Anestesista = null;
                            citaOrigen.PrimerAyudante = null;
                            citaOrigen.SegundoAyudante = null;
                            citaOrigen.Instrumentista = null;
                            citaOrigen.Circulante = null;
                        }

                        _citasRepository.Update(citaOrigen);
                    }
                }

                #endregion

                #region REGISTRAR EMERGENCIA COMO INGRESADA

                if (model.EmergenciaId != null && model.EmergenciaId != 0)
                {
                    var emergencia = _emergenciasRepo.Get((int)model.EmergenciaId);
                    if (emergencia != null)
                    {
                        emergencia.Ingresada = true;
                        emergencia.HospitalizacionId = hospitalizacion.Id;
                        _emergenciasRepo.Update(emergencia);
                    }
                }

                #endregion

                #region AGREGAR HOSPITALIZACION A CUENTA POR COBRAR

                var nuevoDetalleCuenta = new DetalleCuentaPorCobrar
                {
                    HospitalizacionId = hospitalizacion.Id,
                    Descripcion = "Hospitalizacion"
                };
                var cuenta = _cuentasPorCobrarRepository
                    .GetUltimaCuentaPendientePaciente((int)model.PacienteId);
                if (cuenta == null)
                {
                    var noches = (fechaFin - fechaInicio).Days;
                    var nuevaCuenta = new CuentaPorCobrar
                    {
                        PacienteId = model.PacienteId,
                        Pagada = false,
                        Valor = (model.TarifaValor * noches) == 0 ? model.TarifaValor : (model.TarifaValor * noches),
                        Eliminada = false
                    };
                    cuenta = _cuentasPorCobrarRepository.Add(nuevaCuenta);
                }
                cuenta.DetallesCuentaPorCobrar.Add(nuevoDetalleCuenta);
                _cuentasPorCobrarRepository.Update(cuenta);

                #endregion


                #region RECOPILAR CORREOS DEL EQUIPO PARA NOTIFICACIÓN

                var emailPaciente = "";
                var emailCirujano = "";
                var emailPrimerAyudante = "";
                var emailSegundoAyudante = "";
                var emailAnestesista = "";
                var emailInstrumentista = "";
                var emailCirculante = "";

                try
                {
                    if (model.PacienteId.HasValue)
                    {
                        emailPaciente = await _db.Pacientes
                            .Where(p => p.Id == model.PacienteId.Value)
                            .Select(p => p.Email)
                            .FirstOrDefaultAsync() ?? "";
                    }

                    if (model.CitaId.HasValue && model.CitaId > 0)
                    {
                        var citaParaEmail = _citasRepository.GetCita(model.CitaId.Value);
                        if (citaParaEmail != null)
                        {
                            if (citaParaEmail.EmpleadoId.HasValue)
                            {
                                var emp = _empleadoRepository.Get(citaParaEmail.EmpleadoId.Value);
                                emailCirujano = emp?.Email ?? "";
                            }
                            if (citaParaEmail.PrimerAyudanteId.HasValue)
                            {
                                var emp = _empleadoRepository.Get(citaParaEmail.PrimerAyudanteId.Value);
                                emailPrimerAyudante = emp?.Email ?? "";
                            }
                            if (citaParaEmail.SegundoAyudanteId.HasValue)
                            {
                                var emp = _empleadoRepository.Get(citaParaEmail.SegundoAyudanteId.Value);
                                emailSegundoAyudante = emp?.Email ?? "";
                            }
                            if (citaParaEmail.AnestesistaId.HasValue)
                            {
                                var emp = _empleadoRepository.Get(citaParaEmail.AnestesistaId.Value);
                                emailAnestesista = emp?.Email ?? "";
                            }
                            if (citaParaEmail.InstrumentistaId.HasValue)
                            {
                                var emp = _empleadoRepository.Get(citaParaEmail.InstrumentistaId.Value);
                                emailInstrumentista = emp?.Email ?? "";
                            }
                            if (citaParaEmail.CirculanteId.HasValue)
                            {
                                var emp = _empleadoRepository.Get(citaParaEmail.CirculanteId.Value);
                                emailCirculante = emp?.Email ?? "";
                            }
                        }
                    }
                }
                catch (Exception exEmail)
                {
                    Console.WriteLine($"Advertencia al recopilar correos: {exEmail.Message}");
                }

                #endregion



                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    HospitalizacionId = hospitalizacion.Id,
                    CitaId = citaCreada?.Id ?? model.CitaId,
                    ConsultaId = consultaCreada?.Id,
                    EmailPaciente = emailPaciente,
                    EmailCirujano = emailCirujano,
                    EmailPrimerAyudante = emailPrimerAyudante,
                    EmailSegundoAyudante = emailSegundoAyudante,
                    EmailAnestesista = emailAnestesista,
                    EmailInstrumentista = emailInstrumentista,
                    EmailCirculante = emailCirculante
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al registrar hospitalizacion. " + ex.InnerException?.Message ?? ex.Message
                });
            }
        }



        [HttpPost]
        public string ConsultarTarifasHabitacion(int habitacionId, string codigoSeguro)
        {
            try
            {
                var listaTarifas = new List<HospitalizacionHospitalizarTarifaViewModel>();
                var tarifasBd = _habitacionRepository.GetTarifasHabitacion(habitacionId);

                if (tarifasBd != null)
                {
                    // Filtrar tarifas que coincidan con el codigoSeguro
                    foreach (var tarifa in tarifasBd)
                    {
                        if (string.Equals(tarifa.NombreTarifa, codigoSeguro, StringComparison.OrdinalIgnoreCase))
                        {
                            listaTarifas.Add(new HospitalizacionHospitalizarTarifaViewModel
                            {
                                TarifaId = tarifa.Id,
                                NombreTarifa = tarifa.NombreTarifa,
                                Lunes = tarifa.Lunes,
                                Martes = tarifa.Martes,
                                Miercoles = tarifa.Miercoles,
                                Jueves = tarifa.Jueves,
                                Viernes = tarifa.Viernes,
                                Sabado = tarifa.Sabado,
                                Domingo = tarifa.Domingo,
                                FechaEspecial = tarifa.FechaEspecial,
                                FechaTarifa = tarifa.FechaEspecial ?
                                    tarifa.FechaTarifa.ToString("yyyy/MM/dd") :
                                    "-",
                                ValorTarifa = tarifa.ValorTarifa
                            });
                        }
                    }

                    // Si no hay tarifas coincidentes, buscar la tarifa "Sin seguro"
                    if (!listaTarifas.Any())
                    {
                        var tarifaSinSeguro = tarifasBd.FirstOrDefault(t =>
                            string.Equals(t.NombreTarifa, "Sin seguro", StringComparison.OrdinalIgnoreCase));

                        if (tarifaSinSeguro != null)
                        {
                            listaTarifas.Add(new HospitalizacionHospitalizarTarifaViewModel
                            {
                                TarifaId = tarifaSinSeguro.Id,
                                NombreTarifa = tarifaSinSeguro.NombreTarifa,
                                Lunes = tarifaSinSeguro.Lunes,
                                Martes = tarifaSinSeguro.Martes,
                                Miercoles = tarifaSinSeguro.Miercoles,
                                Jueves = tarifaSinSeguro.Jueves,
                                Viernes = tarifaSinSeguro.Viernes,
                                Sabado = tarifaSinSeguro.Sabado,
                                Domingo = tarifaSinSeguro.Domingo,
                                FechaEspecial = tarifaSinSeguro.FechaEspecial,
                                FechaTarifa = tarifaSinSeguro.FechaEspecial ?
                                    tarifaSinSeguro.FechaTarifa.ToString("yyyy/MM/dd") :
                                    "-",
                                ValorTarifa = tarifaSinSeguro.ValorTarifa
                            });
                        }
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaTarifas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar tarifas. " + ex.Message
                });
            }
        }




        public class ActualizarCodigoSeguroRequest
        {
            public int HospitalizacionId { get; set; }

            public int CitaId { get; set; }
            public string NuevoCodigoSeguro { get; set; }
        }

        [HttpPost]
        public IActionResult ActualizarCodigoSeguro([FromBody] ActualizarCodigoSeguroRequest request)
        {
            try
            {
                // Validar si el citaId es válido
                if (request.CitaId <= 0)
                {
                    return Json(new
                    {
                        Exitoso = false,
                        Mensaje = $"El ID de la cita recibido no es válido. Recibido: {request.CitaId}."
                    });
                }

                // Validar si el nuevo código de seguro no es nulo o vacío
                if (string.IsNullOrEmpty(request.NuevoCodigoSeguro))
                {
                    return Json(new
                    {
                        Exitoso = false,
                        Mensaje = "El CódigoSeguro proporcionado está vacío o es inválido."
                    });
                }

                // Obtener la cita usando el repositorio
                var cita = _citasRepository.GetCita(request.CitaId);

                // Validar si la cita existe
                if (cita == null)
                {
                    return Json(new
                    {
                        Exitoso = false,
                        Mensaje = $"No se encontró ninguna cita con el ID ({request.CitaId})."
                    });
                }

                // Actualizar el CódigoSeguro en la cita
                cita.CodigoDeCita = request.NuevoCodigoSeguro;
                _citasRepository.Update(cita);

                // Buscar la hospitalización asociada al paciente de la cita
                var hospitalizacion = _hospitalizacionRepository.GetHospitalizacionById(request.HospitalizacionId);

                if (hospitalizacion != null)
                {
                    // Obtener la habitación asociada a la hospitalización
                    var habitacion = _habitacionRepository.Get(hospitalizacion.HabitacionId);
                    if (habitacion == null)
                    {
                        return Json(new
                        {
                            Exitoso = false,
                            Mensaje = $"No se encontró la habitación asociada a la hospitalización del paciente con ID {cita.PacienteId}."
                        });
                    }

                    // Obtener la tarifa de la habitación basada en la categoría y código de seguro
                    var tarifas = _habitacionRepository.GetTarifasHabitacion(habitacion.Id);
                    var nuevaTarifa = tarifas.FirstOrDefault(t => t.NombreTarifa.Contains(request.NuevoCodigoSeguro));

                    if (nuevaTarifa != null)
                    {
                        // Asignar la nueva tarifa a la hospitalización
                        hospitalizacion.CategoriaHabitacionTarifaId = nuevaTarifa.Id;
                        _hospitalizacionRepository.Update(hospitalizacion);
                    }
                    else
                    {
                        return Json(new
                        {
                            Exitoso = false,
                            Mensaje = $"No se encontró una tarifa de habitación correspondiente al código de seguro '{request.NuevoCodigoSeguro}'."
                        });
                    }
                }

                return Json(new
                {
                    Exitoso = true,
                    Mensaje = $"El CódigoSeguro se actualizó correctamente a '{request.NuevoCodigoSeguro}' para la cita con ID {request.CitaId}, y la tarifa de la habitación también fue actualizada."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = $"Error inesperado al actualizar el CódigoSeguro y la tarifa de la habitación. Detalles: {ex.Message}"
                });
            }
        }

        public class ActualizarMedicoAsignadoRequest
        {
            public int CitaId { get; set; }
            public int EmpleadoId { get; set; }
        }

        [HttpPost]
        public IActionResult ActualizarMedicoAsignado([FromBody] ActualizarMedicoAsignadoRequest request)
        {


            try
            {
                // Validar si el citaId es válido
                if (request.CitaId <= 0)
                {
                    return Json(new
                    {
                        Exitoso = false,
                        Mensaje = $"El ID de la cita recibido no es válido. Recibido: {request.CitaId}."
                    });
                }



                var cita = _citasRepository.GetCita(request.CitaId);

                if (cita == null)
                {
                    return Json(new
                    {
                        Exitoso = false,
                        Mensaje = $"No se encontró ninguna cita con el ID ({request.CitaId})."
                    });
                }

                cita.EmpleadoId = request.EmpleadoId;

                _citasRepository.Update(cita);

                return Json(new
                {
                    Exitoso = true,
                    Mensaje = $"El EmpleadoId se actualizó correctamente a '{request.EmpleadoId}' para la cita con ID {request.CitaId}."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = $"Error inesperado al actualizar el EmpleadoId. Detalles: {ex.Message}"
                });
            }
        }


        public class AgregarMedicoSecundarioRequest
        {
            public int CitaId { get; set; }
            public int EmpleadoId { get; set; }
        }


        [HttpPost]
        public IActionResult AgregarMedicoSecundario([FromBody] AgregarMedicoSecundarioRequest request)
        {
            try
            {

                if (request.CitaId <= 0)
                {
                    return Json(new
                    {
                        Exitoso = false,
                        Mensaje = $"El ID de la cita recibido no es válido. Recibido: {request.CitaId}."
                    });
                }

                var cita = _citasRepository.GetCita(request.CitaId);

                if (cita == null)
                {
                    return Json(new
                    {
                        Exitoso = false,
                        Mensaje = $"No se encontró ninguna cita con el ID ({request.CitaId})."
                    });
                }


                if (cita.MedicosSecundarios == null)
                {
                    cita.MedicosSecundarios = new List<int>();
                }

                // Luego ya es seguro hacer Contains
                if (cita.MedicosSecundarios.Contains(request.EmpleadoId))
                {
                    return Json(new
                    {
                        Exitoso = false,
                        Mensaje = $"El médico con ID {request.EmpleadoId} ya está asignado como médico secundario en esta cita."
                    });
                }

                cita.MedicosSecundarios.Add(request.EmpleadoId);


                _citasRepository.Update(cita);

                return Json(new
                {
                    Exitoso = true,
                    Mensaje = $"El médico secundario con ID {request.EmpleadoId} se ha agregado correctamente a la cita con ID {request.CitaId}."
                });
            }
            catch (Exception ex)
            {
                var Empleado = _empleadoRepository.Get(request.EmpleadoId);
                var nombre = Empleado?.NombreYApellidos ?? "Nombre no encontrado";

                return Json(new
                {
                    Exitoso = true,
                    MedicoId = request.EmpleadoId,
                    Nombre = nombre
                });
            }
        }


        public class QuitarMedicoSecundarioRequest
        {
            public int CitaId { get; set; }
            public int EmpleadoId { get; set; }
        }

        [HttpPost]
        public IActionResult QuitarMedicoSecundario([FromBody] QuitarMedicoSecundarioRequest request)

        {
            try
            {
                var cita = _citasRepository.GetCita(request.CitaId);

                if (cita == null)
                {
                    return Json(new { exitoso = false, Mensaje = "Cita no encontrada" });
                }


                if (!cita.MedicosSecundarios.Contains(request.EmpleadoId))
                {
                    return Json(new { exitoso = false, Mensaje = "El médico no es un médico secundario de esta cita." });
                }

                cita.MedicosSecundarios.Remove(request.EmpleadoId);

                _citasRepository.Update(cita);

                return Json(new { exitoso = true });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, Mensaje = ex.Message });
            }
        }



        // public IActionResult Detalles(int? hospitalizacionId, int? citaId = null)
        // {
        //     if (!hospitalizacionId.HasValue)
        //     {
        //         TempData["Message"] = "La hospitalizacion no existe";
        //         TempData["MessageType"] = "error";
        //         return RedirectToAction("Habitaciones");
        //     }

        //     var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId.Value);
        //     if (hospitalizacion == null)
        //     {
        //         TempData["Message"] = "La hospitalizacion no existe";
        //         TempData["MessageType"] = "error";

        //         return RedirectToAction("Habitaciones");
        //     }

        //     var usuarioActual = _userManager.GetUserId(HttpContext.User);

        //     var cuenta = _cuentasPorCobrarRepository
        //         .GetUltimaCuentaPendientePaciente(hospitalizacion.PacienteId);

        //     if (cuenta == null)
        //     {
        //         cuenta = new CuentaPorCobrar
        //         {
        //             PacienteId = hospitalizacion.PacienteId,
        //             Eliminada = false,
        //             Pagada = false,
        //             Valor = 0
        //         };
        //         cuenta = _cuentasPorCobrarRepository.Add(cuenta);
        //     }

        //     //Autorizaciones y permisos
        //     var autorizacionTabEnfermeria = true;
        //     var autorizacionTabActualizarEstadia = true;
        //     var autorizacionTabDietas = true;
        //     var autorizacionTabPagos = true;
        //     var autorizacionTabSignosVitales = true;
        //     var autorizacionTabNotaMedica = true;
        //     var autorizacionTabNotaEnfermeria = true;
        //     var autorizacionTabNotaEvolucion = true;
        //     var autorizacionTabControlGlucometria = true;
        //     var autorizacionTabIncretaExcreta = true;

        //     if (!User.IsInRole("Administrador")
        //         && !User.IsInRole("Enfermeria")
        //         && !User.IsInRole("Medico General")
        //         && !User.IsInRole("Farmacia")
        //         && !User.IsInRole("Medico Interno")
        //         && !User.IsInRole("Coordinador Laboratorio")
        //         && !User.IsInRole("Tecnico laboratorio")
        //         && !User.IsInRole("Admisiones")
        //         && !User.IsInRole("Recepcion Diurno")
        //         && !User.IsInRole("Recepcion Nocturno")
        //         && !User.IsInRole("Recepcion"))
        //     {
        //         var registroAutorizacionUsuario = hospitalizacion.HospitalizacionUsuariosAcceso
        //             .Where(a => a.UserId == usuarioActual)
        //             .FirstOrDefault();

        //         if (registroAutorizacionUsuario == null)
        //         {
        //             TempData["Message"] = "El usuario no tiene acceso para ver los detalles de la hospitalizacion";
        //             TempData["MessageType"] = "error";

        //             return RedirectToAction("Habitaciones");
        //         }

        //         autorizacionTabEnfermeria = registroAutorizacionUsuario.AutorizacionTabEnfermeria;
        //         autorizacionTabActualizarEstadia = registroAutorizacionUsuario.AutorizacionTabActualizarEstadia;
        //         autorizacionTabNotaMedica = registroAutorizacionUsuario.AutorizacionTabNotaMedica;
        //         autorizacionTabSignosVitales = registroAutorizacionUsuario.AutorizacionTabSignosVitales;
        //         autorizacionTabNotaEnfermeria = registroAutorizacionUsuario.AutorizacionTabNotaEnfermeria;
        //         autorizacionTabNotaEvolucion = registroAutorizacionUsuario.AutorizacionTabNotaEvolucion;
        //         autorizacionTabControlGlucometria = registroAutorizacionUsuario.AutorizacionTabControlGlucometria;
        //         autorizacionTabIncretaExcreta = registroAutorizacionUsuario.AutorizacionTabIncretaExcreta;
        //         autorizacionTabPagos = registroAutorizacionUsuario.AutorizacionTabPagos;
        //         autorizacionTabDietas = registroAutorizacionUsuario.AutorizacionTabDietas;
        //     }

        //     // Cargar la consulta relacionada con la hospitalización
        //     var consulta = _consultasRepository.GetConsultaPorHospitalizacion(hospitalizacionId.Value);

        //     // Si la consulta es nula, inicializamos los submodelos para evitar errores en la vista
        //     if (consulta != null)
        //     {
        //         consulta.ExamenFisico = consulta.ExamenFisico ?? new ExamenFisico();
        //         consulta.ConsultaRevisionSistemas = consulta.ConsultaRevisionSistemas ?? new ConsultaRevisionSistemas();
        //         consulta.Historia = consulta.Historia ?? new Historia();
        //     }

        //     var pacienteIdPDF = hospitalizacion.PacienteId;
        //     var habitacionIdPDF = hospitalizacion.HabitacionId;

        //     // CORRECCIÓN: consulta puede ser null; usar citaId si viene
        //     var citaIdPDF = 0;
        //     if (consulta?.CitasId != null && consulta.CitasId.Value > 0)
        //     {
        //         citaIdPDF = consulta.CitasId.Value;
        //     }
        //     else if (citaId.HasValue && citaId.Value > 0)
        //     {
        //         citaIdPDF = citaId.Value;
        //     }

        //     // CORRECCIÓN: evitar NullReference si no hay cita o no hay médicos secundarios
        //     var preMedicosSecundarios = (citaIdPDF > 0) ? _citasRepository.GetCita(citaIdPDF) : null;
        //     var medicosSecundarios = preMedicosSecundarios?.MedicosSecundarios;

        //     // Suponiendo que tienes un repositorio para obtener los médicos por ID
        //     var medicoSecundariosDto = _empleadoRepository.GetMedicosSecundariosPorIds(medicosSecundarios);





        //     // Conversión de FechaNacimiento a string, con formato deseado (por ejemplo: "dd/MM/yyyy")
        //     string fechaNacimientoStr = hospitalizacion.Paciente.FechaNacimiento.HasValue
        //         ? hospitalizacion.Paciente.FechaNacimiento.Value.ToString("dd/MM/yyyy")
        //         : "No especificada";

        //     // Cálculo de la edad a partir de FechaNacimiento
        //     int? edadCalculada = null;
        //     if (hospitalizacion.Paciente.FechaNacimiento.HasValue)
        //     {
        //         DateTime fechaNacimiento = hospitalizacion.Paciente.FechaNacimiento.Value;
        //         DateTime hoy = DateTime.Today;
        //         edadCalculada = hoy.Year - fechaNacimiento.Year;

        //         // Ajustar si aún no se ha cumplido el cumpleaños en el año actual
        //         if (fechaNacimiento > hoy.AddYears(-edadCalculada.Value))
        //         {
        //             edadCalculada--;
        //         }
        //     }

        //     // Leer cita directamente para médico asignado y equipo quirúrgico
        //     // (cuando viene del calendario no hay consulta, pero sí cita)
        //     // GetCita busca incluyendo citas finalizadas para no perder el medico asignado
        //     var citaParaEquipo = (citaIdPDF > 0) ? _citasRepository.GetCita(citaIdPDF) : null;
        //     var medicoAsignadoNombre = "";
        //     if (citaParaEquipo?.EmpleadoId != null)
        //     {
        //         var empAsignado = _empleadoRepository.Get(citaParaEquipo.EmpleadoId.Value);
        //         if (empAsignado != null)
        //             medicoAsignadoNombre = empAsignado.NombreYApellidos;
        //     }
        //     // Fallback 1: EmpleadoText de la consulta
        //     if (string.IsNullOrEmpty(medicoAsignadoNombre))
        //         medicoAsignadoNombre = consulta?.Citas?.EmpleadoText ?? "";
        //     // Fallback 2: Cirujano de la ultima NotaOperatoria
        //     if (string.IsNullOrEmpty(medicoAsignadoNombre))
        //     {
        //         var ultimaNota = _notaOperatoriaRepository
        //             .GetNotaOperatoriaListByHospitalizacionId(hospitalizacionId.Value)
        //             .FirstOrDefault();
        //         if (!string.IsNullOrEmpty(ultimaNota?.Cirujano))
        //             medicoAsignadoNombre = ultimaNota.Cirujano;
        //     }

        //     var model = new HospitalizacionDetallesViewModel
        //     {
        //         // CORRECCIÓN: consulta puede ser null
        //         CodigoSeguro = consulta?.Citas?.CodigoDeCita ?? citaParaEquipo?.CodigoDeCita,
        //         MedicoAsignado = medicoAsignadoNombre,
        //         CitaAnestesista = citaParaEquipo?.Anestesista ?? "",
        //         CitaPrimerAyudante = citaParaEquipo?.PrimerAyudante ?? "",
        //         CitaSegundoAyudante = citaParaEquipo?.SegundoAyudante ?? "",
        //         CitaInstrumentista = citaParaEquipo?.Instrumentista ?? "",
        //         CitaCirculante = citaParaEquipo?.Circulante ?? "",
        //         CitaProcedimiento = citaParaEquipo?.Procedimiento ?? "",

        //         FechaNacimientoPaciente = fechaNacimientoStr,
        //         PacienteTalla = hospitalizacion.Paciente.Talla,
        //         PacientePeso = hospitalizacion.Paciente.Peso,
        //         EdadPaciente = edadCalculada,
        //         CuentaId = cuenta.Id,
        //         HospitalizacionId = hospitalizacionId.Value,
        //         HabitacionId = hospitalizacion.HabitacionId,
        //         CategoriaHabitacionNombre = hospitalizacion.Habitacion.CategoriaHabitacion.NombreCategoria,
        //         UrlArchivoConsentimiento = hospitalizacion.UrlArchivoConsentimiento,
        //         EspecialidadNombre = "Medicina General",
        //         Consulta = consulta,
        //         PacienteId = hospitalizacion.PacienteId,
        //         PacienteNombre = hospitalizacion.Paciente.Nombre,
        //         PacienteEstadoId = (int)hospitalizacion.Paciente.EstadoPacienteId,
        //         PacienteEstado = hospitalizacion.Paciente.EstadoPaciente.NombreEstado,
        //         PacienteTelefono = hospitalizacion.Paciente.Telefono,
        //         PacienteCelular = hospitalizacion.Paciente.Celular,
        //         PacienteSexo = hospitalizacion.Paciente.sexoText,
        //         PacienteTipoSangre = hospitalizacion.Paciente.TipoDeSangre,
        //         MedicosSecundarios = medicoSecundariosDto,

        //         TipoPaciente = hospitalizacion.Paciente.TipoPaciente != null
        //             ? hospitalizacion.Paciente.TipoPaciente.NombreTipo
        //             : "-",
        //         FechaInicial = hospitalizacion.FechaInicio,
        //         FechaFinal = hospitalizacion.FechaFin,
        //         NumeroCamas = hospitalizacion.Habitacion.NumeroCamas,
        //         NumeroNombreHabitacion = hospitalizacion.Habitacion.NombreNumeroHabitacion,
        //         Pagada = hospitalizacion.Pagada,
        //         HospitalizacionFinalizada = hospitalizacion.Finalizada,
        //         Observaciones = hospitalizacion.Observaciones,

        //         // Autorizaciones permisos (si es necesario)
        //         AutorizacionTabEnfermeria = autorizacionTabEnfermeria,
        //         AutorizacionTabActualizarEstadia = autorizacionTabActualizarEstadia,
        //         AutorizacionTabDietas = autorizacionTabDietas,
        //         AutorizacionTabIncretaExcreta = autorizacionTabIncretaExcreta,
        //         AutorizacionTabControlGlucometria = autorizacionTabControlGlucometria,
        //         AutorizacionTabNotaEnfermeria = autorizacionTabNotaEnfermeria,
        //         AutorizacionTabNotaEvolucion = autorizacionTabNotaEvolucion,
        //         AutorizacionTabNotaMedica = autorizacionTabNotaMedica,
        //         AutorizacionTabPagos = autorizacionTabPagos,
        //         AutorizacionTabSignosVitales = autorizacionTabSignosVitales,

        //         PacienteIdPDF = pacienteIdPDF,
        //         HabitacionIdPDF = habitacionIdPDF,
        //         CitaIdPDF = citaIdPDF,
        //     };

        //     model.AsignarBodegaId(); // Aquí es donde asignas el valor de BodegaId
        //     model.Init(_cuentasPorCobrarRepository, _seguroRepository, _empleadoRepository);
        //     return View(model);
        // }



        public IActionResult Detalles(int? hospitalizacionId, int? citaId = null)
        {
            if (!hospitalizacionId.HasValue)
            {
                TempData["Message"] = "La hospitalizacion no existe";
                TempData["MessageType"] = "error";
                return RedirectToAction("Habitaciones");
            }

            var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId.Value);
            if (hospitalizacion == null)
            {
                TempData["Message"] = "La hospitalizacion no existe";
                TempData["MessageType"] = "error";
                return RedirectToAction("Habitaciones");
            }

            var usuarioActual = _userManager.GetUserId(HttpContext.User);

            var cuenta = _cuentasPorCobrarRepository
                .GetUltimaCuentaPendientePaciente(hospitalizacion.PacienteId);

            if (cuenta == null)
            {
                cuenta = new CuentaPorCobrar
                {
                    PacienteId = hospitalizacion.PacienteId,
                    Eliminada = false,
                    Pagada = false,
                    Valor = 0
                };
                cuenta = _cuentasPorCobrarRepository.Add(cuenta);
            }

            // Autorizaciones y permisos
            var autorizacionTabEnfermeria = true;
            var autorizacionTabActualizarEstadia = true;
            var autorizacionTabDietas = true;
            var autorizacionTabPagos = true;
            var autorizacionTabSignosVitales = true;
            var autorizacionTabNotaMedica = true;
            var autorizacionTabNotaEnfermeria = true;
            var autorizacionTabNotaEvolucion = true;
            var autorizacionTabControlGlucometria = true;
            var autorizacionTabIncretaExcreta = true;

            if (!User.IsInRole("Administrador")
                && !User.IsInRole("Enfermeria")
                && !User.IsInRole("Medico General")
                && !User.IsInRole("Farmacia")
                && !User.IsInRole("Medico Interno")
                && !User.IsInRole("Medico Externo")
                && !User.IsInRole("Coordinador Laboratorio")
                && !User.IsInRole("Tecnico laboratorio")
                && !User.IsInRole("Admisiones")
                && !User.IsInRole("Recepcion Diurno")
                && !User.IsInRole("Recepcion Nocturno")
                && !User.IsInRole("Recepcion"))
            {
                var registroAutorizacionUsuario = hospitalizacion.HospitalizacionUsuariosAcceso
                    .Where(a => a.UserId == usuarioActual)
                    .FirstOrDefault();

                if (registroAutorizacionUsuario == null)
                {
                    TempData["Message"] = "El usuario no tiene acceso para ver los detalles de la hospitalizacion *****";
                    TempData["MessageType"] = "error";
                    return RedirectToAction("Habitaciones");
                }

                autorizacionTabEnfermeria = registroAutorizacionUsuario.AutorizacionTabEnfermeria;
                autorizacionTabActualizarEstadia = registroAutorizacionUsuario.AutorizacionTabActualizarEstadia;
                autorizacionTabNotaMedica = registroAutorizacionUsuario.AutorizacionTabNotaMedica;
                autorizacionTabSignosVitales = registroAutorizacionUsuario.AutorizacionTabSignosVitales;
                autorizacionTabNotaEnfermeria = registroAutorizacionUsuario.AutorizacionTabNotaEnfermeria;
                autorizacionTabNotaEvolucion = registroAutorizacionUsuario.AutorizacionTabNotaEvolucion;
                autorizacionTabControlGlucometria = registroAutorizacionUsuario.AutorizacionTabControlGlucometria;
                autorizacionTabIncretaExcreta = registroAutorizacionUsuario.AutorizacionTabIncretaExcreta;
                autorizacionTabPagos = registroAutorizacionUsuario.AutorizacionTabPagos;
                autorizacionTabDietas = registroAutorizacionUsuario.AutorizacionTabDietas;
            }

            // Cargar la consulta relacionada con la hospitalización
            var consulta = _consultasRepository.GetConsultaPorHospitalizacion(hospitalizacionId.Value);

            if (consulta != null)
            {
                consulta.ExamenFisico = consulta.ExamenFisico ?? new ExamenFisico();
                consulta.ConsultaRevisionSistemas = consulta.ConsultaRevisionSistemas ?? new ConsultaRevisionSistemas();
                consulta.Historia = consulta.Historia ?? new Historia();
            }

            var pacienteIdPDF = hospitalizacion.PacienteId;
            var habitacionIdPDF = hospitalizacion.HabitacionId;

            // Obtener el ID de la cita
            var citaIdPDF = 0;
            if (consulta?.CitasId != null && consulta.CitasId.Value > 0)
            {
                // Caso principal: la consulta tiene el CitasId cargado (Sala de Operaciones)
                citaIdPDF = consulta.CitasId.Value;
            }
            else if (consulta?.Citas?.Id != null && consulta.Citas.Id > 0)
            {
                // Fallback Consultas Externas: CitasId es null pero la propiedad de
                // navegación Citas sí está cargada en memoria (EF lazy/eager load).
                citaIdPDF = consulta.Citas.Id;
            }
            else if (citaId.HasValue && citaId.Value > 0)
            {
                // Caso: citaId viene explícito en la URL (?citaId=123)
                citaIdPDF = citaId.Value;
            }

            var citaParaEquipo = (citaIdPDF > 0) ? _citasRepository.GetCita(citaIdPDF) : null;

            // ── Fallback final para Consultas Externas sin citaIdPDF ──────────────────────
            // Si aún no tenemos la cita, buscamos la más reciente del paciente en BD.
            // La entidad Citas no tiene HabitacionId, así que filtramos solo por paciente.
            if (citaParaEquipo == null)
            {
                citaParaEquipo = _db.Citass
                    .Where(c => c.PacienteId == hospitalizacion.PacienteId && !c.Eliminado)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();

                if (citaParaEquipo != null)
                    citaIdPDF = citaParaEquipo.Id;
            }

            // ── Obtener la última NotaOperatoria (fuente de verdad para el equipo quirúrgico
            //    ya que al hospitalizar se migra el equipo de la Cita hacia la NotaOperatoria
            //    y se borra de la Cita — por eso hay que buscarlo aquí también).
            var ultimaNotaOp = _notaOperatoriaRepository
                .GetNotaOperatoriaListByHospitalizacionId(hospitalizacionId.Value)
                .FirstOrDefault();

            // ── Obtener IDs de médicos secundarios desde la cita ──────────────────────────
            var idsMedicosSecundarios = citaParaEquipo?.MedicosSecundarios ?? new List<int>();
            var medicoSecundariosDto = _empleadoRepository.GetMedicosSecundariosPorIds(idsMedicosSecundarios) ?? new List<MedicoSecundarioDtoHospi>();

            // ── Incluir el Primer Ayudante en "Médicos Secundarios" ───────────────────────
            // Fuente 1: campo PrimerAyudante de la Cita (si aún no fue migrado)
            // Fuente 2: campo PrimerAyudante de la NotaOperatoria (tras la migración al hospitalizar)
            var primerAyudanteNombre = citaParaEquipo?.PrimerAyudante;
            if (string.IsNullOrWhiteSpace(primerAyudanteNombre))
                primerAyudanteNombre = ultimaNotaOp?.PrimerAyudante;

            if (!string.IsNullOrWhiteSpace(primerAyudanteNombre))
            {
                var yaExiste = medicoSecundariosDto.Any(m => m.NombreCompleto == primerAyudanteNombre);
                if (!yaExiste)
                {
                    medicoSecundariosDto.Add(new MedicoSecundarioDtoHospi
                    {
                        Id = 0,
                        NombreCompleto = primerAyudanteNombre
                    });
                }
            }

            // ── Obtener nombre del médico asignado (cirujano) ─────────────────────────────
            // Fuente 1: empleado vinculado a la cita (Sala de Operaciones)
            // Fuente 2: EmpleadoText de la consulta (Consulta Externa con cita vinculada)
            // Fuente 3: Cirujano de la última NotaOperatoria (Consulta Externa sin cita o fallback final)
            var medicoAsignadoNombre = "";
            if (citaParaEquipo?.EmpleadoId != null)
            {
                var empAsignado = _empleadoRepository.Get(citaParaEquipo.EmpleadoId.Value);
                if (empAsignado != null)
                    medicoAsignadoNombre = empAsignado.NombreYApellidos;
            }
            if (string.IsNullOrEmpty(medicoAsignadoNombre))
                medicoAsignadoNombre = consulta?.Citas?.EmpleadoText ?? "";
            if (string.IsNullOrEmpty(medicoAsignadoNombre))
            {
                // Fuente 3: Cirujano de la última NotaOperatoria
                // (cubre Consultas Externas sin cita vinculada y Sala de Operaciones
                //  cuando la cita ya fue archivada o no tiene EmpleadoId)
                if (!string.IsNullOrEmpty(ultimaNotaOp?.Cirujano))
                    medicoAsignadoNombre = ultimaNotaOp.Cirujano;
            }

            // Conversión de fecha y edad
            string fechaNacimientoStr = hospitalizacion.Paciente.FechaNacimiento.HasValue
                ? hospitalizacion.Paciente.FechaNacimiento.Value.ToString("dd/MM/yyyy")
                : "No especificada";

            int? edadCalculada = null;
            if (hospitalizacion.Paciente.FechaNacimiento.HasValue)
            {
                DateTime fechaNacimiento = hospitalizacion.Paciente.FechaNacimiento.Value;
                DateTime hoy = DateTime.Today;
                edadCalculada = hoy.Year - fechaNacimiento.Year;
                if (fechaNacimiento > hoy.AddYears(-edadCalculada.Value))
                    edadCalculada--;
            }

            var model = new HospitalizacionDetallesViewModel(_configuration)
            {
                CodigoSeguro = consulta?.Citas?.CodigoDeCita ?? citaParaEquipo?.CodigoDeCita,
                MedicoAsignado = medicoAsignadoNombre,
                CitaAnestesista = citaParaEquipo?.Anestesista ?? ultimaNotaOp?.Anestesista ?? "",
                // PrimerAyudante: buscar primero en la cita, luego en la NotaOperatoria
                // (al hospitalizar se migra de Cita → NotaOperatoria y se borra de la Cita)
                CitaPrimerAyudante = primerAyudanteNombre ?? "",
                CitaSegundoAyudante = citaParaEquipo?.SegundoAyudante ?? ultimaNotaOp?.SegundoAyudante ?? "",
                CitaInstrumentista = citaParaEquipo?.Instrumentista ?? ultimaNotaOp?.Instrumentista ?? "",
                CitaCirculante = citaParaEquipo?.Circulante ?? ultimaNotaOp?.Circulante ?? "",
                CitaProcedimiento = citaParaEquipo?.Procedimiento ?? "",

                FechaNacimientoPaciente = fechaNacimientoStr,
                PacienteTalla = hospitalizacion.Paciente.Talla,
                PacientePeso = hospitalizacion.Paciente.Peso,
                EdadPaciente = edadCalculada,
                CuentaId = cuenta.Id,
                HospitalizacionId = hospitalizacionId.Value,
                HabitacionId = hospitalizacion.HabitacionId,
                CategoriaHabitacionNombre = hospitalizacion.Habitacion.CategoriaHabitacion.NombreCategoria,
                UrlArchivoConsentimiento = hospitalizacion.UrlArchivoConsentimiento,
                EspecialidadNombre = "Medicina General",
                Consulta = consulta,
                PacienteId = hospitalizacion.PacienteId,
                PacienteNombre = hospitalizacion.Paciente.Nombre,
                PacienteEstadoId = (int)hospitalizacion.Paciente.EstadoPacienteId,
                PacienteEstado = hospitalizacion.Paciente.EstadoPaciente.NombreEstado,
                PacienteTelefono = hospitalizacion.Paciente.Telefono,
                PacienteCelular = hospitalizacion.Paciente.Celular,
                PacienteSexo = hospitalizacion.Paciente.sexoText,
                PacienteTipoSangre = hospitalizacion.Paciente.TipoDeSangre,
                MedicosSecundarios = medicoSecundariosDto, // Aquí asignas la lista ya procesada

                TipoPaciente = hospitalizacion.Paciente.TipoPaciente != null
                    ? hospitalizacion.Paciente.TipoPaciente.NombreTipo
                    : "-",
                FechaInicial = hospitalizacion.FechaInicio,
                FechaFinal = hospitalizacion.FechaFin,
                NumeroCamas = hospitalizacion.Habitacion.NumeroCamas,
                NumeroNombreHabitacion = hospitalizacion.Habitacion.NombreNumeroHabitacion,
                Pagada = hospitalizacion.Pagada,
                HospitalizacionFinalizada = hospitalizacion.Finalizada,
                Observaciones = hospitalizacion.Observaciones,

                AutorizacionTabEnfermeria = autorizacionTabEnfermeria,
                AutorizacionTabActualizarEstadia = autorizacionTabActualizarEstadia,
                AutorizacionTabDietas = autorizacionTabDietas,
                AutorizacionTabIncretaExcreta = autorizacionTabIncretaExcreta,
                AutorizacionTabControlGlucometria = autorizacionTabControlGlucometria,
                AutorizacionTabNotaEnfermeria = autorizacionTabNotaEnfermeria,
                AutorizacionTabNotaEvolucion = autorizacionTabNotaEvolucion,
                AutorizacionTabNotaMedica = autorizacionTabNotaMedica,
                AutorizacionTabPagos = autorizacionTabPagos,
                AutorizacionTabSignosVitales = autorizacionTabSignosVitales,

                PacienteIdPDF = pacienteIdPDF,
                HabitacionIdPDF = habitacionIdPDF,
                CitaIdPDF = citaIdPDF
            };

            model.AsignarBodegaId();
            model.Init(_cuentasPorCobrarRepository, _seguroRepository, _empleadoRepository);

            return View(model);
        }


        [HttpPost]
        public string ConsultarPaquetesExistentes()
        {
            try
            {
                var listaPaquetes = new List<HospitalizacionPaqueteExistenteViewModel>();
                var paquetesBd = _hospitalizacionRepository.GetPaquetesHospitalizacion();
                if (paquetesBd != null)
                {
                    foreach (var paquete in paquetesBd)
                    {
                        listaPaquetes.Add(new HospitalizacionPaqueteExistenteViewModel
                        {
                            PaqueteId = paquete.Id,
                            Codigo = paquete.CodigoInterno,
                            Descripcion = paquete.Descripcion,
                            Nombre = paquete.NombrePaquete
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaPaquetes
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar paquetes. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarServiciosExistentes()
        {
            try
            {
                var listaServicios = new List<HospitalizacionServicioExistenteViewModel>();
                var serviciosBd = _serviciosRepository.GetListaServicios();
                if (serviciosBd != null)
                {
                    foreach (var servicio in serviciosBd)
                    {
                        listaServicios.Add(new HospitalizacionServicioExistenteViewModel
                        {
                            ServicioId = servicio.Id,
                            Nombre = servicio.NombreServicio,
                            //Precio = servicio.Precio
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaServicios
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
        public string ConsultarMedicamentosExistentes()
        {
            try
            {
                var listaMedicamentos = new List<HospitalizacionMedicamentoExistenteViewModel>();
                //var medicamentosBd = _productoRepository.GetList()
                //    .Where(p => p.TipoBodegaId == (int)TipoBodegaEnum.Clinica
                //    && p.TipoProductoId == (int)TipoProductoEnum.Medicamentos)
                //    .ToList();

                var sucursal = _productoRepository.GetSucursalIdByTipoBodega((int)TipoBodegaEnum.Clinica); //buscamos la sucursal ID SEGUN EL AMBIENTE BODEGA 

                var medicamentosBd = _productoRepository.GetInventarioProductos
                   (null, null, null, null, null, null, (int)AmbienteEnum.Hospital);

                if (medicamentosBd != null)
                {
                    foreach (var medicamento in medicamentosBd)
                    {
                        listaMedicamentos.Add(new HospitalizacionMedicamentoExistenteViewModel
                        {
                            ProductoId = medicamento.Id,
                            Nombre = medicamento.NombreProducto
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaMedicamentos
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar medicamentos. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarExamenesExistentes()
        {
            try
            {
                var listaExamenes = new List<HospitalizacionExamenExistenteViewModel>();
                var examenesBd = _laboratorioClinicoRepository.GetListExamenesLaboratorio()
                    .ToList();
                if (examenesBd != null)
                {
                    foreach (var examen in examenesBd)
                    {
                        listaExamenes.Add(new HospitalizacionExamenExistenteViewModel
                        {
                            ExamenLabClinicoId = examen.Id,
                            Nombre = examen.NombreExamen,
                            Precio = examen.Precio
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaExamenes
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

        [HttpPost]
        public string ConsultarDatosExamenFisico()
        {
            try
            {
                var listaDatos = new List<HospitalizacionDatoExamenFisicoExistenteViewModel>();
                var datosBd = _hospitalizacionRepository.GetDatosExamenFisicoHosp();
                if (datosBd != null)
                {
                    foreach (var dato in datosBd)
                    {
                        listaDatos.Add(new HospitalizacionDatoExamenFisicoExistenteViewModel
                        {
                            DatoExamenFisicoHospId = dato.Id,
                            NombreDato = dato.NombreDato
                        });
                    }
                }
                // Console.WriteLine("Contenido de listaDatos: " + JsonSerializer.Serialize(listaDatos));
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaDatos
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar datos de examen fisico. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarHabitacionesDisponiblesCambio()
        {
            try
            {
                var habitacionesDisponibles = new List<HospitalizacionHabitacionDisponibleCambioViewModel>();
                var habitaciones = _habitacionRepository.GetHabitaciones()
                    .Where(a => a.EstadoHabitacionId == (int)EstadoHabitacionEnum.Disponible);
                if (habitaciones != null)
                {
                    foreach (var habitacion in habitaciones)
                    {
                        habitacionesDisponibles.Add(new HospitalizacionHabitacionDisponibleCambioViewModel
                        {
                            HabitacionId = habitacion.Id,
                            HabitacionNombre = habitacion.NombreNumeroHabitacion,
                            HabitacionCategoria = habitacion.CategoriaHabitacion.NombreCategoria,
                            HabitacionNumeroCamas = habitacion.NumeroCamas,
                            HabitacionCapacidadPersonas = habitacion.CapacidadPersonas
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = habitacionesDisponibles
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar habitaciones disponibles para hospitalizacion. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string AgregarServicio(HospitalizacionAgregarServicioViewModel model)
        {
            try
            {
                _hospitalizacionRepository.AddServicio(new HospitalizacionServicio
                {
                    HospitalizacionId = model.HospitalizacionId,
                    ServicioId = model.ServicioId,
                    PrecioServicioId = model.PrecioServicioId,
                    Cantidad = model.Cantidad,
                    UsuarioCreaId = _userManager.GetUserId(HttpContext.User),
                    FechaHoraAplicacion = DateTime.Now
                });

                //Agregar registros de aplicacion de servicios

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
                    Mensaje = "Error al agregar servicio. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarPrecioServicio(int servicioId, string codigoSeguro)
        {
            try
            {
                var data = _serviciosRepository.GetPrecioServicioById(servicioId) ?? new List<ServicioPrecio>();

                // Normalización básica (misma intención que OrdinalIgnoreCase + evita null/espacios)
                static string Norm(string s) => (s ?? string.Empty).Trim();

                var codigo = Norm(codigoSeguro);

                // 1) Base: solo activos
                var activos = data.Where(x => x.Activar).ToList();

                // 2) Regla del usuario: solo precios con valor > 0
                var conValor = activos.Where(x => x.Valor > 0).ToList();

                // Si no hay precios con valor > 0, no hay nada que mostrar.
                if (!conValor.Any())
                {
                    return JsonSerializer.Serialize(new { Exitoso = true, Resultado = new object[0] });
                }

                // 3) Intento 1: por codigoSeguro (solo si viene informado)
                var result = new List<object>();

                if (!string.IsNullOrWhiteSpace(codigo))
                {
                    result = conValor
                        .Where(x => string.Equals(Norm(x.Precio?.NombrePrecio), codigo, StringComparison.OrdinalIgnoreCase))
                        .Select(x => new
                        {
                            PrecioServicioId = x.Id,
                            NombrePrecio = x.Precio.NombrePrecio,
                            Valor = x.Valor,
                            PrecioMostrar = x.Precio.NombrePrecio + " Q. " + x.Valor.ToString("0.00")
                        })
                        .Cast<object>()
                        .ToList();
                }

                // 4) Fallback: SIN SEGURO -> NORMAL
                if (!result.Any())
                {
                    // NOTA: en tu tabla Precios está "SIN SEGURO" y "NORMAL" en mayúsculas.
                    // Comparación case-insensitive por seguridad.
                    var sinSeguro = conValor.FirstOrDefault(x =>
                        string.Equals(Norm(x.Precio?.NombrePrecio), "SIN SEGURO", StringComparison.OrdinalIgnoreCase));

                    if (sinSeguro != null)
                    {
                        result.Add(new
                        {
                            PrecioServicioId = sinSeguro.Id,
                            NombrePrecio = sinSeguro.Precio.NombrePrecio,
                            Valor = sinSeguro.Valor,
                            PrecioMostrar = sinSeguro.Precio.NombrePrecio + " Q. " + sinSeguro.Valor.ToString("0.00")
                        });
                    }

                    var normal = conValor.FirstOrDefault(x =>
                        string.Equals(Norm(x.Precio?.NombrePrecio), "NORMAL", StringComparison.OrdinalIgnoreCase));

                    if (normal != null && (sinSeguro == null || normal.Id != sinSeguro.Id))
                    {
                        result.Add(new
                        {
                            PrecioServicioId = normal.Id,
                            NombrePrecio = normal.Precio.NombrePrecio,
                            Valor = normal.Valor,
                            PrecioMostrar = normal.Precio.NombrePrecio + " Q. " + normal.Valor.ToString("0.00")
                        });
                    }
                }

                // 5) Si aun así no hay nada, devolvemos vacío
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
                    Mensaje = "Error al agregar servicio. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string EliminarServicio(int hospitalizacionServicioId)
        {
            try
            {
                var hospitalizacionServicio = _hospitalizacionRepository
                    .GetHospitalizacionServicio(hospitalizacionServicioId);
                hospitalizacionServicio.Eliminado = true;
                _hospitalizacionRepository.Update(hospitalizacionServicio);
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
                    Mensaje = "Error al eliminar servicio. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string AgregarMedicamento(HospitalizacionAgregarMedicamentoViewModel model)
        {
            try
            {
                var hospitalizacionProducto = _hospitalizacionRepository.AddMedicamento(new HospitalizacionProducto
                {
                    HospitalizacionId = model.HospitalizacionId,
                    ProductoId = model.ProductoId,
                    UnidadMedidaVentaId = model.UnidadMedidaVentaId,
                    PrecioId = model.PrecioId,
                    PrecioValor = model.Precio,
                    Cantidad = model.Cantidad,
                    Indicaciones = model.Indicaciones,
                    ViaAdministracion = model.ViaAdministracion,
                    FrecuenciaAdministracion = model.FrecuenciaAdministracion,
                    FechaHoraAplicacionManual = model.FechaHoraAplicacionManual
                    //PrecioProductoId = model.IdProductoPrecioInventario
                });


                for (var i = 1; i <= hospitalizacionProducto.Cantidad; i++)
                {
                    _hospitalizacionRepository.AddProductoAplicacion(new HospitalizacionProductoAplicacion
                    {
                        HospitalizacionProductoId = hospitalizacionProducto.Id,
                        Cantidad = 1,
                        Aplicado = false,
                        UsuarioCreaId = _userManager.GetUserId(HttpContext.User)
                    });
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
                    Mensaje = "Error al agregar medicamento. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string EliminarMedicamento(int hospitalizacionMedicamentoId)
        {
            try
            {
                var hospitalizacionMedicamento = _hospitalizacionRepository
                    .GetHospitalizacionMedicamento(hospitalizacionMedicamentoId);
                hospitalizacionMedicamento.Eliminado = true;
                _hospitalizacionRepository.Update(hospitalizacionMedicamento);
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
                    Mensaje = "Error al eliminar medicamento. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarMedicamentoPrecio(int productoId)
        {
            try
            {
                var data = _productoRepository.GetProductoInventarioPrecioByIdProducto(productoId);

                var result = data.Select(x => new
                {
                    IdProductoPrecioInventario = x.Id,
                    ProductoId = x.ProductoInventario.ProductoId,
                    NombrePrecio = x.Precio.NombrePrecio,
                    Valor = x.Valor,
                    PrecioMostrar = x.Precio.NombrePrecio + " Q. " + x.Valor.ToString("0.00") // Formatear con dos decimales
                });

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = result
                });

            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar los precios del medicamento"
                });
            }
        }


        [HttpPost]
        public string AgregarExamen([FromBody] HospitalizacionAgregarExamenViewModel model)
        {
            try
            {
                if (model == null || model.Examenes == null || !model.Examenes.Any())
                {
                    Console.WriteLine("❌ Error: No se han recibido exámenes.");
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Debe proporcionar al menos un examen."
                    });
                }

                var usuarioSolicita = _userManager.GetUserId(HttpContext.User);
                if (string.IsNullOrWhiteSpace(usuarioSolicita))
                {
                    Console.WriteLine("❌ No se pudo identificar al usuario.");
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se pudo identificar al usuario que solicita el examen."
                    });
                }

                var codigoSeguro = (model.CodigoSeguro ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(codigoSeguro))
                {
                    codigoSeguro = "Sin seguro";
                }

                var cuenta = _cuentasPorCobrarRepository.Get(model.CuentaId);
                if (cuenta == null)
                {
                    Console.WriteLine("Cuenta por cobrar no encontrada.");
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Cuenta por cobrar no encontrada."
                    });
                }

                decimal totalPrecio = 0m;
                int examenesAgregados = 0;

                foreach (var examenItem in model.Examenes)
                {
                    var examenLabClinico = _laboratorioClinicoRepository
                        .GetExamenLab(examenItem.ExamenLabClinicoId, false);

                    if (examenLabClinico == null)
                    {
                        Console.WriteLine($"❌ ExamenLabClinicoId {examenItem.ExamenLabClinicoId} no encontrado. Se omite.");
                        continue;
                    }

                    var precios = _laboratorioClinicoRepository
                        .GetPreciosExamen(examenItem.ExamenLabClinicoId)
                        ?? new List<ExamenLabClinicoPrecio>();

                    ExamenLabClinicoPrecio precioExamen = null;

                    // 1) Buscar por codigoSeguro (case-insensitive)
                    precioExamen = precios.FirstOrDefault(p =>
                        p?.Precio != null &&
                        !string.IsNullOrWhiteSpace(p.Precio.NombrePrecio) &&
                        string.Equals(p.Precio.NombrePrecio.Trim(), codigoSeguro, StringComparison.OrdinalIgnoreCase));

                    // 2) Fallback: Sin seguro
                    if (precioExamen == null)
                    {
                        precioExamen = precios.FirstOrDefault(p =>
                            p?.Precio != null &&
                            !string.IsNullOrWhiteSpace(p.Precio.NombrePrecio) &&
                            string.Equals(p.Precio.NombrePrecio.Trim(), "Sin seguro", StringComparison.OrdinalIgnoreCase));
                    }

                    // 3) Fallback: primer precio disponible con PrecioId=1
                    if (precioExamen == null)
                    {
                        precioExamen = precios.FirstOrDefault(p => p != null && p.PrecioId == 1);
                    }

                    // 4) Sin precio registrado → valor 0
                    if (precioExamen == null)
                    {
                        Console.WriteLine($"⚠️ Sin precio para examen {examenItem.ExamenLabClinicoId}. Valor=0.");
                        precioExamen = new ExamenLabClinicoPrecio { PrecioId = 1, PrecioValor = 0m };
                    }

                    var valor = precioExamen.PrecioValor;

                    var examenNuevo = new Examen
                    {
                        EstadoExamenId = (int)EstadoExamenEnum.Solictiado,
                        FechaRealizacion = DateTime.Now,
                        PacienteId = model.PacienteId,
                        UsuarioSolicita = usuarioSolicita,
                        Eliminado = false,
                        DetalleExamenes = new List<DetalleExamen>
                {
                    new DetalleExamen
                    {
                        ExamenLabClinicoId = examenItem.ExamenLabClinicoId,
                        Cantidad = 1,
                        PrecioId = precioExamen.PrecioId,
                        PrecioValor = valor,
                        Descuento = 0,
                        Subtotal = valor,
                        Total = valor,
                        Observacion = examenItem.Observacion
                    }
                }
                    };

                    _hospitalizacionRepository.AddExamen(new HospitalizacionExamen
                    {
                        FechaHora = DateTime.Now,
                        HospitalizacionId = model.HospitalizacionId,
                        Examen = examenNuevo
                    });

                    totalPrecio += valor;
                    examenesAgregados++;

                    Console.WriteLine($"Examen {examenItem.ExamenLabClinicoId} agregado como HospitalizacionExamen independiente. Valor: {valor}");
                }

                if (examenesAgregados == 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se pudieron agregar exámenes válidos."
                    });
                }

                // Actualizar cuenta por cobrar con el total de todos los exámenes
                cuenta.Valor = (cuenta.Valor ?? 0m) + totalPrecio;
                _cuentasPorCobrarRepository.Update(cuenta);

                Console.WriteLine($"{examenesAgregados} examen(es) registrado(s). Total acumulado: {totalPrecio}");

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en AgregarExamen: {ex.Message}");
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al agregar examen. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarPrecioExamen(int examenId, string codigoSeguro)
        {
            try
            {
                var codigo = (codigoSeguro ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(codigo))
                    codigo = "Sin seguro";

                var data = _laboratorioClinicoRepository.GetPreciosExamen(examenId)
                           ?? new List<ExamenLabClinicoPrecio>();

                var result = data
                    .Where(x =>
                        x?.Precio != null &&
                        !string.IsNullOrWhiteSpace(x.Precio.NombrePrecio) &&
                        string.Equals(x.Precio.NombrePrecio.Trim(), codigo, StringComparison.OrdinalIgnoreCase))
                    .Select(x => new
                    {
                        ExamenLabClinicoId = x.ExamenLabClinicoId,
                        ExamenLabClinicoPrecioId = x.Id,
                        PrecioNombre = x.Precio.NombrePrecio,
                        Valor = x.PrecioValor,
                        PrecioMostrar = x.Precio.NombrePrecio + " Q. " + x.PrecioValor.ToString("0.00")
                    })
                    .ToList();

                if (!result.Any())
                {
                    var precioSinSeguro = data.FirstOrDefault(x =>
                        x?.Precio != null &&
                        !string.IsNullOrWhiteSpace(x.Precio.NombrePrecio) &&
                        string.Equals(x.Precio.NombrePrecio.Trim(), "Sin seguro", StringComparison.OrdinalIgnoreCase));

                    if (precioSinSeguro != null)
                    {
                        result.Add(new
                        {
                            ExamenLabClinicoId = precioSinSeguro.ExamenLabClinicoId,
                            ExamenLabClinicoPrecioId = precioSinSeguro.Id,
                            PrecioNombre = precioSinSeguro.Precio.NombrePrecio,
                            Valor = precioSinSeguro.PrecioValor,
                            PrecioMostrar = precioSinSeguro.Precio.NombrePrecio + " Q. " + precioSinSeguro.PrecioValor.ToString("0.00")
                        });
                    }
                }

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = result });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar precios del examen. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string ConsultarPacientes()
        {

            try
            {
                var lista = _pacientesRepository.GetList();

                var result = lista.Select(x => new
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Dpi = x.Dpi,
                    NombreConDpi = x.PacienteWithDPI,
                    FechaNacimiento = x.FechaNacimiento != null ? ((DateTime)x.FechaNacimiento).ToString("yyyy-MM-dd") : null,
                    Telefono = x.Telefono,
                    Correo = x.Email
                });

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = result });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al consultar los pacientes. " + ex.Message });
            }
        }
        [HttpPost]
        public string EliminarExamen(int hospitalizacionExamenId)
        {
            try
            {
                if (hospitalizacionExamenId <= 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "ID de examen inválido."
                    });
                }

                var hospitalizacionExamen = _hospitalizacionRepository
                    .GetHospitalizacionExamen(hospitalizacionExamenId);

                if (hospitalizacionExamen == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"No se encontró el examen con Id {hospitalizacionExamenId}. " +
                                  "Es posible que ya haya sido eliminado o que el Id sea incorrecto."
                    });
                }

                hospitalizacionExamen.Eliminado = true;
                _hospitalizacionRepository.Update(hospitalizacionExamen);

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al eliminar examen. " + ex.Message
                });
            }
        }




        [HttpPost]
        public string AgregarPaquete(HospitalizacionAgregarPaqueteViewModel model)
        {
            try
            {
                // 1. Validar modelo de entrada
                if (model == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Modelo inválido." });

                if (model.PaqueteId <= 0)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "ID de paquete inválido." });

                if (model.HospitalizacionId <= 0)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "ID de hospitalización inválido." });

                if (model.PacienteId <= 0)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "ID de paciente inválido." });

                if (model.CuentaId <= 0)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "ID de cuenta inválido." });

                // 2. Obtener detalles del paquete
                var dataDetalle = _detallePaqueteHospitalizacion.GetByIdPaqueteHospitalizacion(model.PaqueteId);
                if (dataDetalle == null || !dataDetalle.Any())
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"El paquete con ID {model.PaqueteId} no tiene detalles configurados o no existe."
                    });
                }

                // 3. Obtener PaqueteHospitalizacion desde el primer detalle
                var primerDetalle = dataDetalle.FirstOrDefault();
                if (primerDetalle?.PaqueteHospitalizacion == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se pudo obtener la información del paquete (PaqueteHospitalizacion no cargado)."
                    });
                }
                var paquete = primerDetalle.PaqueteHospitalizacion;

                // 4. Construir lista de HospitalizacionDetallePaqueteHospitalizacion
                var listHospDetalleHosp = new List<HospitalizacionDetallePaqueteHospitalizacion>();

                foreach (var detalle in dataDetalle)
                {
                    if (detalle.Eliminado) continue;

                    for (int i = 0; i < detalle.Cantidad; i++)
                    {
                        // ══════════════════════
                        // CASO 1: LABORATORIO
                        // ══════════════════════
                        if (detalle.LaboratorioId != null)
                        {
                            decimal precioLab = detalle.PrecioValor;

                            var examen = new Examen
                            {
                                EstadoExamenId = (int)EstadoExamenEnum.Solictiado,
                                FechaRealizacion = DateTime.Now,
                                PacienteId = model.PacienteId,
                                UsuarioSolicita = _userManager.GetUserId(HttpContext.User),
                                Eliminado = false
                            };
                            var detalleExamen = new DetalleExamen
                            {
                                ExamenLabClinicoId = (int)detalle.LaboratorioId,
                                Cantidad = 1,
                                PrecioValor = precioLab,
                                Descuento = 0,
                                Subtotal = precioLab,
                                Total = precioLab
                            };
                            examen.DetalleExamenes.Add(detalleExamen);
                            _laboratorioClinicoRepository.Add(examen);

                            listHospDetalleHosp.Add(new HospitalizacionDetallePaqueteHospitalizacion
                            {
                                FechaHora = DateTime.Now,
                                Aplicacion = false,
                                UsuarioAplicacionId = null,
                                FechaHoraAplicada = null,
                                ExamenId = examen.Id,
                                Eliminado = false,
                                ServicioId = null,
                                ProductoId = null,
                                LaboratorioId = detalle.LaboratorioId,
                                LaboratorioPrecioId = detalle.LaboratorioPrecioId,
                                PrecioProducto = precioLab,
                                UnidadMedidaVentaId = null
                            });
                        }
                        // ══════════════════════
                        // CASO 2: SERVICIO
                        // ══════════════════════
                        else if (detalle.ServicioId != null && detalle.ProductoId == null)
                        {
                            listHospDetalleHosp.Add(new HospitalizacionDetallePaqueteHospitalizacion
                            {
                                FechaHora = DateTime.Now,
                                Aplicacion = false,
                                UsuarioAplicacionId = null,
                                FechaHoraAplicada = null,
                                Eliminado = false,
                                ServicioId = detalle.ServicioId,
                                ProductoId = null,
                                LaboratorioId = null,
                                LaboratorioPrecioId = null,
                                PrecioProducto = detalle.PrecioValor,
                                UnidadMedidaVentaId = null
                            });
                        }
                        // ══════════════════════
                        // CASO 3: PRODUCTO
                        // ══════════════════════
                        else if (detalle.ProductoId != null)
                        {
                            listHospDetalleHosp.Add(new HospitalizacionDetallePaqueteHospitalizacion
                            {
                                FechaHora = DateTime.Now,
                                Aplicacion = false,
                                UsuarioAplicacionId = null,
                                FechaHoraAplicada = null,
                                Eliminado = false,
                                ServicioId = null,
                                ProductoId = detalle.ProductoId,
                                LaboratorioId = null,
                                LaboratorioPrecioId = null,
                                PrecioProducto = detalle.PrecioValor,
                                UnidadMedidaVentaId = detalle.UnidadMedidaVentaId
                            });
                        }
                    }
                }

                // 5. Guardar el encabezado del paquete en la hospitalización
                var hospitalizacionPaquete = new HospitalizacionPaqueteHospitalizacion
                {
                    FechaHora = DateTime.Now,
                    HospitalizacionId = model.HospitalizacionId,
                    PaqueteHospitalizacionId = model.PaqueteId,
                    Eliminado = false,
                    HospitalizacionDetallePaqueteHospitalizacion = listHospDetalleHosp
                };
                _hospitalizacionRepository.AddHospitalizacionPaqueteHospitalizacion(hospitalizacionPaquete);

                // 6. Actualizar cuenta por cobrar (si existe)
                var cuenta = _cuentasPorCobrarRepository.Get(model.CuentaId);
                if (cuenta == null)
                {
                    // Si no hay cuenta, la creamos (opcional, depende de tu lógica)
                    // Por ahora solo mostramos advertencia pero no fallamos
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = true,
                        Mensaje = "Paquete agregado, pero no se actualizó la cuenta porque no existe."
                    });
                }

                // Asegurar que paquete.Precio no sea null (usar ?? 0)
                decimal valorPaquete = paquete.Precio ?? 0;
                cuenta.Valor = (cuenta.Valor ?? 0) + valorPaquete;
                _cuentasPorCobrarRepository.Update(cuenta);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Paquete agregado exitosamente"
                });
            }
            catch (Exception ex)
            {
                // 🔥 Capturar el error completo para depuración
                var innerMessage = ex.InnerException?.Message ?? "";
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = $"Error al agregar paquete. {ex.Message} {innerMessage}"
                });
            }
        }



        [HttpPost]
        public string EliminarPaquete(int hospitalizacionPaqueteId)
        {
            try
            {
                var hospitalizacionPaquete = _hospitalizacionRepository
                    .GetHospitalizacionPaqueteHospitalizacion(hospitalizacionPaqueteId);
                hospitalizacionPaquete.Eliminado = true;
                _hospitalizacionRepository.Update(hospitalizacionPaquete);
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
                    Mensaje = "Error al eliminar paquete. " + ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<string> AgregarExamenFisicoHospitalizacion()
        {
            try
            {
                var opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                };

                string rawBody;
                using (var reader = new StreamReader(Request.Body))
                    rawBody = await reader.ReadToEndAsync();

                if (string.IsNullOrEmpty(rawBody))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Body vacío." });

                using var doc = JsonDocument.Parse(rawBody);
                var root = doc.RootElement;

                // ── Extraer entity ────────────────────────────────────────────
                if (!root.TryGetProperty("entity", out var entityElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos del examen no encontrados." });

                var model = JsonSerializer.Deserialize<HospitalizacionAgregarExamenFisicoViewModel>(
                    entityElement.GetRawText(), opts);

                if (model == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos del examen inválidos." });

                if (model.HospitalizacionId == 0)
                {
                    if (root.TryGetProperty("hospitalizacionId", out var hospFallback))
                        model.HospitalizacionId = hospFallback.GetInt32();
                }

                if (model.HospitalizacionId == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "HospitalizacionId inválido." });

                // ── Guardar sin autorización (estado Pendiente) ───────────────
                var hospitalizacion = _hospitalizacionRepository.Get((int)model.HospitalizacionId);
                var fechaHora = DateTime.Now;
                var datos = new List<ExamenFisicoHospDato>();

                if (model.DatosExamen != null)
                {
                    foreach (var dato in model.DatosExamen)
                    {
                        if (dato.NombreDato == "Peso")
                            hospitalizacion.Paciente.Peso = dato.ValorDato;

                        if (dato.NombreDato == "Estatura")
                            hospitalizacion.Paciente.Talla = dato.ValorDato;

                        datos.Add(new ExamenFisicoHospDato
                        {
                            DatoExamenFisicoHospId = dato.DatoExamenFisicoHospId,
                            Valor = dato.ValorDato
                        });
                    }
                }

                var nuevoExamen = new ExamenFisicoHosp
                {
                    FechaHora = fechaHora,
                    HospitalizacionId = model.HospitalizacionId,
                    Observaciones = model.Observaciones,
                    UsuarioToma = _userManager.GetUserId(HttpContext.User),
                    Autorizado = false,
                    ExamenesFisicosHospDatos = datos
                };

                _hospitalizacionRepository.AddExamenFisicoHosp(nuevoExamen);

                return JsonSerializer.Serialize(new { exitoso = true, examenId = nuevoExamen.Id });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                });
            }
        }

        [HttpPost]
        public async Task<string> AutorizarExamenFisicoHospitalizacion()
        {
            try
            {
                var opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                };

                string rawBody;
                using (var reader = new StreamReader(Request.Body))
                    rawBody = await reader.ReadToEndAsync();

                if (string.IsNullOrEmpty(rawBody))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Body vacío." });

                using var doc = JsonDocument.Parse(rawBody);
                var root = doc.RootElement;

                // ── Paso 1: Extraer examenId ──────────────────────────────────
                if (!root.TryGetProperty("examenId", out var examenIdElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "ExamenId no encontrado." });

                var examenId = examenIdElement.GetInt32();

                // ── Paso 2: Extraer huellaPayload ─────────────────────────────
                if (!root.TryGetProperty("huellaPayload", out var huellaElement))
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Payload de huella no encontrado." });

                var huellaPayload = JsonSerializer.Deserialize<WebAuthnAssertionPayload>(
                    huellaElement.GetRawText(), opts);

                if (huellaPayload == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Payload de huella inválido." });

                // ── Paso 3: Identificar al autorizador por su credentialId ─────
                var credIdString = Convert.ToBase64String(
                    Convert.FromBase64String(
                        huellaPayload.RawId.Replace('-', '+').Replace('_', '/')
                        + new string('=', (4 - huellaPayload.RawId.Length % 4) % 4)
                    )
                );

                var credencial = await _db.WebAuthnCredentials
                    .FirstOrDefaultAsync(c => c.DescriptorId == credIdString);

                if (credencial == null)
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        resultado = "La credencial presentada no está registrada en el sistema."
                    });

                var authorizerId = credencial.UserId;

                // ── Paso 4: Verificar la firma criptográfica ──────────────────
                var verificacion = await _webAuthn.CompleteVerifyAsync(authorizerId, huellaPayload);

                if (!verificacion.Success)
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        resultado = verificacion.UserMessage,
                        errorCode = verificacion.ErrorCode?.ToString()
                    });

                // ── Paso 5: Validar que el autorizador tiene permiso ──────────
                var examen = await _db.ExamenesFisicosHosp
                    .FirstOrDefaultAsync(e => e.Id == examenId);

                if (examen == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Examen no encontrado." });

                var authorizerUser = await _userManager.FindByIdAsync(authorizerId);
                var esAdmin = await _userManager.IsInRoleAsync(authorizerUser, "Administrador");

                if (!esAdmin)
                {
                    var citasId = await _db.Consultas
                        .Where(c => c.HospitalizacionId == examen.HospitalizacionId)
                        .Select(c => c.CitasId)
                        .FirstOrDefaultAsync();

                    if (citasId == null)
                        return JsonSerializer.Serialize(new
                        {
                            exitoso = false,
                            resultado = "No se encontró la consulta de esta hospitalización."
                        });

                    var cita = await _db.Citass
                        .Where(c => c.Id == citasId.Value)
                        .Select(c => new { c.EmpleadoId, c.MedicosSecundarios })
                        .FirstOrDefaultAsync();

                    if (cita == null)
                        return JsonSerializer.Serialize(new
                        {
                            exitoso = false,
                            resultado = "No se encontró la cita de esta hospitalización."
                        });

                    var empleadosAutorizados = new List<int>();
                    if (cita.EmpleadoId.HasValue)
                        empleadosAutorizados.Add(cita.EmpleadoId.Value);
                    if (cita.MedicosSecundarios != null)
                        empleadosAutorizados.AddRange(cita.MedicosSecundarios);

                    var userIdsAutorizados = await _db.Usuarios
                        .Where(u => u.EmpleadoId != null
                                 && empleadosAutorizados.Contains(u.EmpleadoId.Value))
                        .Select(u => u.Id)
                        .ToListAsync();

                    if (!userIdsAutorizados.Contains(authorizerId))
                        return JsonSerializer.Serialize(new
                        {
                            exitoso = false,
                            resultado = "El autorizador no es el médico asignado ni un médico secundario de esta hospitalización."
                        });
                }

                // ── Paso 6: Marcar como autorizado ───────────────────────────
                examen.Autorizado = true;
                examen.UsuarioAutoriza = authorizerId;
                examen.FechaAutorizacion = DateTime.Now;
                await _db.SaveChangesAsync();

                return JsonSerializer.Serialize(new { exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                });
            }
        }

        [HttpPost]
        public string AgregarDeposito(HospitalizacionAgregarDepositoViewModel model)
        {
            try
            {


                var cajaAbierta = _cajaRepository.GetCajaAbierta((int)AmbienteEnum.Hospital);
                if (cajaAbierta == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No hay ninguna caja abierta para el ambiente de Hospital"
                    });
                }

                var pago = new Pagos
                {
                    FechaHora = DateTime.Now,
                    CuentaPorCobrarId = model.CuentaId,
                    FormaPagoId = model.FormaPagoId,
                    MonedaId = model.MonedaId,
                    Monto = model.Valor
                };
                _cuentasPorCobrarRepository.AddPago(pago);

                var detalleCaja = new DetalleCaja
                {
                    CuentaPorCobrarId = model.CuentaId,
                    CuentaPorCobrarPagoId = pago.Id,
                    Ingreso = model.Valor,
                    CajaId = cajaAbierta.Id,
                    Fecha = DateTime.Now,
                    Descripcion = "Deposito en cuenta por cobrar: " + model.CuentaId
                };
                _cajaRepository.Add(detalleCaja);

                #region Actualizacion de valor de cuenta por cobrar

                var cuenta = _cuentasPorCobrarRepository.Get(model.CuentaId);
                if (cuenta != null)
                {
                    var valorCuenta = cuenta.Valor ?? 0;
                    valorCuenta -= model.Valor;
                    cuenta.Valor = valorCuenta;
                    _cuentasPorCobrarRepository.Update(cuenta);
                }

                #endregion

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Deposito agregado"
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al agregar deposito. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string EliminarDeposito(int pagoId)
        {
            try
            {
                var pago = _cuentasPorCobrarRepository
                    .GetPago(pagoId);
                if (pago != null)
                {
                    var cuenta = pago.CuentaPorCobrar;
                    if (cuenta != null)
                    {
                        var valorCuenta = cuenta.Valor ?? 0;
                        valorCuenta += pago.Monto;
                        pago.CuentaPorCobrar.Valor = valorCuenta;
                    }
                }
                pago.Eliminado = true;
                _cuentasPorCobrarRepository.Update(pago);
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
                    Mensaje = "Error al eliminar deposito. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string CambiarHabitacion(int hospitalizacionId, int habitacionId)
        {
            try
            {
                var fechaHora = DateTime.Now;

                var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId, false);

                if (hospitalizacion == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se encontró la hospitalización."
                    });
                }

                var habitacionOrigen = _habitacionRepository.Get(hospitalizacion.HabitacionId);
                if (habitacionOrigen != null)
                {
                    habitacionOrigen.EstadoHabitacionId = (int)EstadoHabitacionEnum.Disponible;
                    _habitacionRepository.Update(habitacionOrigen);
                }

                var habitacionDestino = _habitacionRepository.Get(habitacionId);
                if (habitacionDestino == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se encontró la nueva habitación."
                    });
                }


                var habitacion = _habitacionRepository.Get(hospitalizacion.HabitacionId);



                // Registrar el historial de cambio
                var cambio = new HospitalizacionCambioHabitacion
                {
                    HospitalizacionId = hospitalizacion.Id,
                    HabitacionId = hospitalizacion.HabitacionId,
                    FechaCambio = fechaHora,
                    Dias = hospitalizacion.NochesInt,
                    ValorTarifa = hospitalizacion.ValorNocheDecimal

                };
                _hospitalizacionRepository.AddCambioHabitacion(cambio);

                // Actualizar la hospitalización con la nueva habitación
                hospitalizacion.HabitacionId = habitacionId;
                _hospitalizacionRepository.Update(hospitalizacion);

                habitacionDestino.EstadoHabitacionId = (int)EstadoHabitacionEnum.Ocupada;
                _habitacionRepository.Update(habitacionDestino);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Se ha realizado el cambio de habitación y registrado en el historial."
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al cambiar de habitación. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string ConsultarRegistrosHospitalizacion(int pacienteId)
        {
            try
            {
                var registrosHospitalizacion = new List<HospitalizacionRegistroHospitalizacionViewModel>();
                var registrosBd = _pacientesRepository.GetHospitalizaciones(pacienteId)
                    .Where(b => !b.Pagada && !b.Finalizada)
                    .ToList();

                if (registrosBd != null)
                {
                    foreach (var registro in registrosBd)
                    {
                        var fechaInicio = registro.FechaInicio.Date;
                        // var fechaFin = registro.FechaFin.Date;
                        var fechaFin = DateTime.Now.Date;

                        var noches = (fechaInicio == fechaFin) ? 1 : (fechaFin - fechaInicio).Days;
                        var precioHospitalizacion = registro.CategoriaHabitacionTarifa?.ValorTarifa * noches ?? 0; // Manejar valores nulos

                        registrosHospitalizacion.Add(new HospitalizacionRegistroHospitalizacionViewModel
                        {
                            Id = registro.Id,
                            FechaInicio = registro.FechaInicio.ToString(),
                            FechaFin = registro.FechaFin.ToString(),
                            NumeroNoches = noches,
                            HabitacionId = registro.HabitacionId,
                            HabitacionNumeroNombre = registro.Habitacion?.NombreNumeroHabitacion ?? "Desconocida",
                            HabitacionCategoria = registro.Habitacion?.CategoriaHabitacion?.NombreCategoria ?? "Sin categoría",
                            HabitacionTarifa = registro.CategoriaHabitacionTarifa?.NombreTarifa ?? "-",
                            HabitacionValorNoche = registro.CategoriaHabitacionTarifa?.ValorTarifa.ToString() ?? "0",
                            Precio = precioHospitalizacion.ToString(),
                            HabitacionNumeroCamas = registro.Habitacion?.NumeroCamas.ToString() ?? "0",
                            Observaciones = registro.Observaciones
                        });
                    }
                }

                var cambiosHabitacion = new List<dynamic>();

                if (registrosBd.Any())
                {
                    cambiosHabitacion = _hospitalizacionRepository.GetCambiosHabitacion(registrosBd.First().Id)
                        .Select(c =>
                        {
                            var habitacion = _habitacionRepository.Get(c.HabitacionId);

                            return new
                            {
                                FechaCambio = c.FechaCambio.ToString("yyyy-MM-dd HH:mm"),
                                Dias = c.Dias,
                                Habitacion = habitacion?.NombreNumeroHabitacion ?? "Desconocida",
                                Categoria = habitacion?.CategoriaHabitacion?.NombreCategoria ?? "Sin categoría",
                                Tarifa = habitacion?.CategoriaHabitacion?.CategoriaHabitacionTarifas?.FirstOrDefault()?.NombreTarifa ?? "Sin tarifa",
                                ValorTarifa = c.ValorTarifa,
                                ValorTotal = c.Dias * c.ValorTarifa
                            };
                        })
                        .ToList<dynamic>();
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = registrosHospitalizacion,
                    Historial = cambiosHabitacion
                });
            }
            catch (Exception ex)
            {
                // Mostrar error en consola
                Console.WriteLine("Error en ConsultarRegistrosHospitalizacion:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar registros de hospitalización. Revisa la consola para más detalles."
                });
            }
        }




        [HttpPost]
        public string ConsultarPaquetesHospitalizacion(int hospitalizacionId)
        {
            try
            {
                var listaPaquetes = new List<HospitalizacionPaqueteAgregadoViewModel>();
                var paquetesBd = _hospitalizacionRepository.GetPaquetesAgregados(hospitalizacionId);

                if (paquetesBd == null)
                    return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaPaquetes });

                foreach (var paquete in paquetesBd)
                {
                    // Validar que el paquete tenga la relación cargada
                    if (paquete.PaqueteHospitalizacion == null)
                        continue;

                    var detalles = paquete.PaqueteHospitalizacion.DetallePaquetesHospitalizacion;
                    if (detalles == null)
                        continue;

                    var servicios = new List<HospitalizacionPaqueteDetallePaqueteServicioViewModel>();
                    var productos = new List<HospitalizacionPaqueteDetallePaqueteProductoViewModel>();
                    var laboratorios = new List<HospitalizacionPaqueteDetallePaqueteLaboratorioViewModel>();

                    foreach (var item in detalles.Where(d => !d.Eliminado))
                    // foreach (var item in detalles)
                    {
                        // Servicio
                        if (item.LaboratorioId == null && item.ProductoId == null && item.ServicioId != null)
                        {
                            if (item.Servicio != null)
                            {
                                decimal precio = item.PrecioValor;
                                servicios.Add(new HospitalizacionPaqueteDetallePaqueteServicioViewModel
                                {
                                    Id = item.ServicioId.Value,
                                    Nombre = item.Servicio.NombreServicio,
                                    Cantidad = item.Cantidad,
                                    Codigo = item.Servicio.CodigoInterno,
                                    Precio = precio
                                });
                            }
                        }
                        // Producto
                        else if (item.LaboratorioId == null && item.ProductoId != null && item.ServicioId == null)
                        {
                            if (item.Producto != null)
                            {
                                decimal precio = item.PrecioValor;
                                productos.Add(new HospitalizacionPaqueteDetallePaqueteProductoViewModel
                                {
                                    Id = item.ProductoId.Value,
                                    Nombre = item.Producto.NombreProducto,
                                    Cantidad = item.Cantidad,
                                    Codigo = item.Producto.CodigoReferencia,
                                    Precio = precio
                                });
                            }
                        }
                        // Laboratorio
                        else if (item.LaboratorioId != null && item.ProductoId == null && item.ServicioId == null)
                        {
                            if (item.Laboratorio != null)
                            {
                                decimal precio = item.PrecioValor;
                                laboratorios.Add(new HospitalizacionPaqueteDetallePaqueteLaboratorioViewModel
                                {
                                    Id = item.LaboratorioId.Value,
                                    Nombre = item.Laboratorio.NombreExamen,
                                    Cantidad = item.Cantidad,
                                    Codigo = item.Laboratorio.Id.ToString(),
                                    Precio = precio
                                });
                            }
                        }
                    }

                    listaPaquetes.Add(new HospitalizacionPaqueteAgregadoViewModel
                    {
                        Id = paquete.Id,
                        PaqueteId = paquete.PaqueteHospitalizacionId,
                        Codigo = paquete.PaqueteHospitalizacion.CodigoInterno,
                        Nombre = paquete.PaqueteHospitalizacion.NombrePaquete,
                        FechaHora = paquete.FechaHora.ToString(),
                        Servicios = servicios,
                        Productos = productos,
                        Laboratorios = laboratorios,
                        Precio = paquete.PaqueteHospitalizacion.Precio ?? 0
                    });
                }

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaPaquetes });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar paquetes agregados. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarServiciosHospitalizacion(int hospitalizacionId)
        {
            try
            {
                var servicios = _hospitalizacionService.GetServiciosHospitalizacion(hospitalizacionId);

                // Ordenar: los más recientes primero, y los sin fecha al final
                var serviciosOrdenados = servicios
                    .OrderByDescending(s => DateTime.TryParse(s.FechaHoraAplicacion, out var fecha) ? fecha : DateTime.MinValue)
                    .ThenBy(s => string.IsNullOrEmpty(s.FechaHoraAplicacion)); // Los sin fecha al final

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = serviciosOrdenados
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


        [HttpGet]
        [Route("Hospitalizacion/ObtenerEmergenciaId/{hospitalizacionId}")]
        public string ObtenerEmergenciaId(int hospitalizacionId)
        {
            try
            {
                int emergenciaId = _emergenciaService.GetEmergenciaIdByHospitalizacion(hospitalizacionId);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    EmergenciaId = emergenciaId
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, EmergenciaId = 0, Mensaje = ex.Message });
            }
        }

        [HttpGet]
        [Route("Hospitalizacion/ConsultarServiciosEmergencias/{hospitalizacionId}")]
        public string ConsultarServiciosEmergencias(int hospitalizacionId)
        {
            try
            {
                int emergenciaId = _emergenciaService.GetEmergenciaIdByHospitalizacion(hospitalizacionId);

                var emergenciaDetalle = _emergenciaService.Get(emergenciaId, includePaciente: true, includeElementos: true);

                if (emergenciaDetalle == null)
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Detalle no encontrado" });
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Paciente = emergenciaDetalle.PacienteNombre,
                    Resultado = new
                    {
                        Productos = emergenciaDetalle.Productos ?? new List<EmergenciaProductoAgregadoViewModel>(),
                        Servicios = emergenciaDetalle.Servicios ?? new List<EmergenciaServicioAgregadoViewModel>(),
                        Examenes = emergenciaDetalle.Examenes ?? new List<EmergenciaExamenAgregadoViewModel>(),
                        Observaciones = emergenciaDetalle.Observaciones
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
            }
        }
        [HttpPost]
        public string ConsultarMedicamentosHospitalizacion(int hospitalizacionId)
        {
            try
            {
                // Console.WriteLine($"[DEBUG] Iniciando consulta de medicamentos para hospitalizacionId: {hospitalizacionId}");

                var listaProductos = new List<HospitalizacionProductoViewModel>();

                var productos = _hospitalizacionRepository.GetProductos(hospitalizacionId);
                // Console.WriteLine($"[DEBUG] Se obtuvieron {productos?.Count() ?? 0} productos.");

                if (productos != null && productos.Any())
                {
                    var productoIds = productos.Select(p => p.ProductoId).Distinct().ToList();
                    // Console.WriteLine($"[DEBUG] Se obtuvieron {productoIds.Count} IDs únicos de productos.");

                    var productosConTipo = _productosRepository.GetProductosPorIds(productoIds);
                    // Console.WriteLine($"[DEBUG] Se obtuvieron {productosConTipo?.Count() ?? 0} productos con tipo.");

                    var productoTipoMap = productosConTipo.ToDictionary(p => p.Id, p => p.TipoProductoId);
                    // Console.WriteLine($"[DEBUG] Mapeo de tipo de producto creado con {productoTipoMap.Count} elementos.");

                    foreach (var producto in productos)
                    {
                        var hospitalizacionProductosAplicados =
                            producto.HospitalizacionesProductosAplicaciones
                            .Where(a => a.Aplicado).ToList()
                            ?? new List<HospitalizacionProductoAplicacion>();

                        // Console.WriteLine($"[DEBUG] ProductoId: {producto.ProductoId} - Aplicaciones: {hospitalizacionProductosAplicados.Count}");

                        var cantidadAplicada = hospitalizacionProductosAplicados.Sum(a => a.Cantidad);
                        var subtotal = producto.PrecioValor * hospitalizacionProductosAplicados.Where(a => a.Aplicado && !a.Eliminado).Sum(a => a.Cantidad);

                        listaProductos.Add(new HospitalizacionProductoViewModel
                        {
                            Id = producto.Id,
                            ProductoId = producto.ProductoId.ToString(),
                            Nombre = producto.Producto.NombreProducto,
                            Cantidad = producto.Cantidad,
                            CantidadAplicada = cantidadAplicada,
                            Precio = producto.PrecioValor,
                            Indicaciones = producto.Indicaciones,
                            ViaAdministracion = producto.ViaAdministracion,
                            FrecuenciaAdministracion = producto.FrecuenciaAdministracion,
                            Subtotal = subtotal,
                            TipoProductoId = productoTipoMap.ContainsKey(producto.ProductoId)
                                            ? productoTipoMap[producto.ProductoId]
                                            : (int?)null
                        });

                        // Console.WriteLine($"[DEBUG] Agregado producto: {producto.Producto.NombreProducto}, Subtotal: {subtotal}");
                    }
                }
                else
                {
                    // Console.WriteLine("[DEBUG] No se obtuvieron productos para la hospitalizacion.");
                }

                var result = JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaProductos
                });

                // Console.WriteLine($"[DEBUG] Consulta finalizada. Total productos procesados: {listaProductos.Count}");
                return result;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"[ERROR] Error al consultar medicamentos: {ex.Message}");
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar medicamentos. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarProductosAplicacionHospitalizacion(int hospitalizacionId, int tipoProductoId)
        {
            try
            {
                // Se obtiene la lista de productos filtrando por hospitalización y tipo de producto.
                // var productosAplicacion = _hospitalizacionRepository
                //     .GetProductosAplicacion(hospitalizacionId)
                //     .Where(p => p.HospitalizacionProducto.Producto.TipoProductoId == tipoProductoId)
                //     .ToList();
                var productosAplicacion = _hospitalizacionRepository
    .GetProductosAplicacion(hospitalizacionId)
    .Where(p => p.HospitalizacionProducto.Producto.TipoProductoId == tipoProductoId)
    .OrderByDescending(p => p.Id)  // Los más recientes primero
    .ToList();

                // Si no se encuentran registros, se retorna la respuesta vacía.
                if (productosAplicacion == null || !productosAplicacion.Any())
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = true,
                        Resultado = new List<HospitalizacionProductoAplicacionViewModel>()
                    });
                }

                // 1. Extraer todos los user IDs involucrados (UsuarioAplica y UsuarioCreaId)
                var userIds = productosAplicacion
                    .SelectMany(p => new[] { p.UsuarioAplica, p.UsuarioCreaId })
                    .Where(id => id != null)
                    .Distinct()
                    .ToList();

                // 2. Obtener todos los usuarios en una sola consulta y organizarlos en un diccionario.
                var usuarios = _userRepository.GetByIds(userIds)
                    .ToDictionary(u => u.Id, u => u);

                // 3. Extraer todos los EmpleadoIds de los usuarios obtenidos (evitando duplicados)
                var empleadoIds = usuarios.Values
                    .Select(u => u.EmpleadoId)
                    .Where(id => id != null)
                    .Distinct()
                    .Select(id => (int)id)
                    .ToList();

                // 4. Obtener todos los empleados en una sola consulta.
                var empleados = _empleadoRepository.GetByIds(empleadoIds)
                    .ToDictionary(e => e.Id, e => e);

                // Lista para armar el ViewModel final.
                var listaProductosAplicacion = new List<HospitalizacionProductoAplicacionViewModel>();

                foreach (var productoAplicacion in productosAplicacion)
                {
                    // Resolver el nombre de la persona que aplica utilizando los diccionarios precargados.
                    string persona = "-";
                    if (productoAplicacion.UsuarioAplica != null && usuarios.ContainsKey(productoAplicacion.UsuarioAplica))
                    {
                        var empleadoId = usuarios[productoAplicacion.UsuarioAplica].EmpleadoId;
                        if (empleadoId != null && empleados.ContainsKey((int)empleadoId))
                        {
                            persona = empleados[(int)empleadoId].NombreYApellidos;
                        }
                        else
                        {
                            persona = "Admin";
                        }
                    }

                    string fechaAplicacion = productoAplicacion.Aplicado && productoAplicacion.FechaHoraAplicacion.HasValue
          ? productoAplicacion.FechaHoraAplicacion.Value.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("es-GT"))
          : "-";

                    string fechaAplicacionManualOriginal = productoAplicacion.HospitalizacionProducto.FechaHoraAplicacionManual;

                    string fechaAplicacionManual = DateTime.TryParse(fechaAplicacionManualOriginal, out DateTime fechaConvertida)
                        ? fechaConvertida.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("es-GT"))
                        : "-";

                    // Resolver el usuario que creó (PersonaCrea)
                    string personaCrea = "-";
                    if (usuarios.ContainsKey(productoAplicacion.UsuarioCreaId))
                    {
                        personaCrea = usuarios[productoAplicacion.UsuarioCreaId].NormalizedUserName;
                    }

                    // Solo se procesan los productos que han sido aplicados o que no han sido eliminados y no aplicados.
                    if (productoAplicacion.Aplicado || (!productoAplicacion.HospitalizacionProducto.Eliminado && !productoAplicacion.Aplicado))
                    {
                        // Se arma el ViewModel; en el caso de aplicado se incluye la unidad de medida de venta.
                        var viewModel = new HospitalizacionProductoAplicacionViewModel
                        {
                            Id = productoAplicacion.Id,
                            ProductoId = productoAplicacion.HospitalizacionProducto.ProductoId,
                            Nombre = productoAplicacion.HospitalizacionProducto.Producto.NombreProducto,
                            Cantidad = productoAplicacion.Cantidad,
                            Indicaciones = productoAplicacion.HospitalizacionProducto.Indicaciones,
                            ViaAdministracion = productoAplicacion.HospitalizacionProducto.ViaAdministracion,
                            FrecuenciaAdministracion = productoAplicacion.HospitalizacionProducto.FrecuenciaAdministracion,
                            Aplicado = productoAplicacion.Aplicado,
                            FechaHoraAplicacion = fechaAplicacion,
                            FechaHoraAplicacionManual = fechaAplicacionManual,
                            PersonaAplica = persona,
                            PersonaCrea = personaCrea,
                        };


                        // Si el producto está aplicado, se agrega el nombre de la unidad de medida de venta.
                        if (productoAplicacion.Aplicado)
                        {
                            viewModel.UnidadMedidaVentaNombre = productoAplicacion.HospitalizacionProducto.UnidadMedidaVenta != null
                                ? productoAplicacion.HospitalizacionProducto.UnidadMedidaVenta.Nombre
                                : "-";
                        }

                        listaProductosAplicacion.Add(viewModel);
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaProductosAplicacion
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar medicamentos para aplicacion. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarExamenesHospitalizacion(int hospitalizacionId)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("🧪 ConsultarExamenesHospitalizacion INVOCADO");
            Console.WriteLine($"➡ hospitalizacionId recibido: {hospitalizacionId}");
            Console.WriteLine("=================================================");

            try
            {
                var listaExamenes = new List<HospitalizacionExamenViewModel>();

                Console.WriteLine("📡 Llamando a _hospitalizacionRepository.GetExamenes(...)");
                var examenes = _hospitalizacionRepository.GetExamenes(hospitalizacionId);

                if (examenes == null)
                {
                    Console.WriteLine("⚠ _hospitalizacionRepository.GetExamenes devolvió NULL");
                }
                else
                {
                    Console.WriteLine($"✅ Repositorio devolvió {examenes.Count()} registros");

                    foreach (var examen in examenes)
                    {
                        Console.WriteLine("---------------------------------------------");
                        Console.WriteLine($"🧩 HospitalizacionExamen Id: {examen.Id}");
                        Console.WriteLine($"   ExamenId: {examen.ExamenId}");
                        Console.WriteLine($"   FechaHora: {examen.FechaHora}");

                        if (examen.Examen == null)
                        {
                            Console.WriteLine("❌ examen.Examen es NULL");
                            continue;
                        }

                        Console.WriteLine($"   ✔ Examen existe | Examen.Id: {examen.Examen.Id}");

                        if (examen.Examen.DetalleExamenes != null && examen.Examen.DetalleExamenes.Any())
                        {
                            Console.WriteLine($"   🔬 DetalleExamenes encontrados: {examen.Examen.DetalleExamenes.Count}");

                            foreach (var detalleExamen in examen.Examen.DetalleExamenes)
                            {
                                Console.WriteLine($"      ➤ DetalleExamenId: {detalleExamen.Id}");

                                if (detalleExamen.ExamenLabClinico == null)
                                {
                                    Console.WriteLine("      ❌ ExamenLabClinico es NULL");
                                }
                                else
                                {
                                    Console.WriteLine($"      🧾 NombreExamen: {detalleExamen.ExamenLabClinico.NombreExamen}");
                                }

                                Console.WriteLine($"      💰 PrecioValor: {detalleExamen.PrecioValor}");

                                listaExamenes.Add(new HospitalizacionExamenViewModel
                                {
                                    Id = examen.Id,
                                    ExamenId = examen.ExamenId,
                                    FechaHora = examen.FechaHora != DateTime.MinValue
                                        ? examen.FechaHora.ToString("dd/MM/yyyy HH:mm:ss")
                                        : "-",
                                    DetalleExamenId = detalleExamen.Id,
                                    // Nombre = detalleExamen.ExamenLabClinico?.NombreExamen ?? "-",
                                    // Precio = detalleExamen.PrecioValor
                                    Nombre = string.Join(", ", examen.Examen.DetalleExamenes
                .Select(d => d.ExamenLabClinico?.NombreExamen ?? "-")),
                                    Precio = examen.Examen.DetalleExamenes.Sum(d => d.PrecioValor),
                                });
                            }
                        }
                        else
                        {
                            Console.WriteLine("⚠ Examen SIN DetalleExamenes");

                            listaExamenes.Add(new HospitalizacionExamenViewModel
                            {
                                Id = examen.Id,
                                ExamenId = examen.ExamenId,
                                FechaHora = examen.FechaHora != DateTime.MinValue
                                    ? examen.FechaHora.ToString("dd/MM/yyyy HH:mm:ss")
                                    : "-",
                                DetalleExamenId = 0,
                                Nombre = "-",
                                Precio = 0
                            });
                        }
                    }
                }

                Console.WriteLine("📊 Total registros generados para la vista: " + listaExamenes.Count);

                var examenesOrdenados = listaExamenes
                    .OrderByDescending(e => DateTime.TryParse(e.FechaHora, out var fecha) ? fecha : DateTime.MinValue)
                    .ThenBy(e => e.FechaHora == "-")
                    .ToList();

                Console.WriteLine("📤 Retornando resultado OK");
                Console.WriteLine("=================================================");

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = examenesOrdenados
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ EXCEPCIÓN en ConsultarExamenesHospitalizacion");
                Console.WriteLine(ex.ToString());

                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar exámenes. " + ex.Message
                });
            }
        }



        [HttpPost]
        public string ConsultarExamenesFisicosHospitalizacion(int hospitalizacionId)
        {
            try
            {
                var listaExamenesFisicos = new List<HospitalizacionExamenFisicoViewModel>();
                var examenesFisicosBd = _hospitalizacionRepository
                    .GetExamenesFisicosHosp(hospitalizacionId)
                    .OrderByDescending(a => a.FechaHora)
                    .ToList();

                if (examenesFisicosBd != null)
                {
                    foreach (var examen in examenesFisicosBd)
                    {
                        var persona = "-";
                        var usuario = _userRepository.GetbyId(examen.UsuarioToma);
                        if (usuario != null && usuario.EmpleadoId != null)
                        {
                            persona = _empleadoRepository.Get((int)usuario.EmpleadoId).NombreYApellidos;
                        }

                        // Lista para almacenar los datos estructurados
                        var datosExamen = new List<object>();

                        if (examen.ExamenesFisicosHospDatos != null)
                        {
                            foreach (var dato in examen.ExamenesFisicosHospDatos)
                            {
                                if (dato.DatoExamenFisicoHosp.NombreDato == "Presion arterial brazo derecho")
                                {
                                    // Ignorar este dato
                                    continue;
                                }

                                // Modificar el nombre si es "Presion arterial brazo izquierdo"
                                var nombreDato = dato.DatoExamenFisicoHosp.NombreDato == "Presion arterial brazo izquierdo"
                                    ? "Presion arterial"
                                    : dato.DatoExamenFisicoHosp.NombreDato;

                                // Agregar el nombre y valor como un objeto clave-valor
                                datosExamen.Add(new
                                {
                                    Nombre = nombreDato,
                                    Valor = dato.Valor
                                });
                            }
                        }

                        listaExamenesFisicos.Add(new HospitalizacionExamenFisicoViewModel
                        {
                            Id = examen.Id,
                            FechaHora = examen.FechaHora.ToString(),
                            Persona = persona,
                            Observaciones = examen.Observaciones,
                            Datos = JsonSerializer.Serialize(datosExamen), // Aquí asignamos los datos estructurados
                            Autorizado = examen.Autorizado
                        });
                    }
                }

                // Devolver la respuesta en formato JSON puro
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaExamenesFisicos
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar examenes fisicos. " + ex.Message
                });

            }

        }


        [HttpPost]
        public string ConsultarDepositosHospitalizacion(int cuentaId)
        {
            try
            {
                var listaDepositos = new List<HospitalizacionDepositoViewModel>();
                var depositosBd = _cuentasPorCobrarRepository.GetPagos(cuentaId);
                if (depositosBd != null)
                {
                    foreach (var deposito in depositosBd)
                    {
                        var fechaHora = deposito.FechaHora == null ? "-"
                            : Convert.ToDateTime(deposito.FechaHora).ToString();
                        listaDepositos.Add(new HospitalizacionDepositoViewModel
                        {
                            Id = deposito.Id,
                            FechaHora = fechaHora,
                            FormaPago = deposito.FormaPago.NombreFormaPago,
                            Moneda = deposito.Moneda.NombreMoneda,
                            Monto = deposito.Monto.ToString()
                        }); ;
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaDepositos
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar depositos. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string AplicarProducto(int hospitalizacionProductoAplicacionId, int cuentaId)
        {
            try
            {
                //Se registra la aplicacion del producto
                var hospitalizacionProductoAplicacion = _hospitalizacionRepository
                    .GetProductoAplicacion(hospitalizacionProductoAplicacionId);
                hospitalizacionProductoAplicacion.Aplicado = true;
                hospitalizacionProductoAplicacion.FechaHoraAplicacion = DateTime.Now;
                hospitalizacionProductoAplicacion.UsuarioAplica = _userManager.GetUserId(HttpContext.User);
                _hospitalizacionRepository.UpdateProductoAplicacion(hospitalizacionProductoAplicacion);

                //Se actualiza la cuenta por cobrarCodigoSeguro
                var cuenta = _cuentasPorCobrarRepository.GetCuenta(cuentaId);
                if (cuenta != null)
                {
                    var valorPendiente = cuenta.Valor ?? 0;
                    valorPendiente += hospitalizacionProductoAplicacion
                        .HospitalizacionProducto
                        .PrecioValor;
                    cuenta.Valor = valorPendiente;
                    _cuentasPorCobrarRepository.Update(cuenta);
                }

                //Se realiza descuento en inventario
                _productosService.RealizarDescuentoInventario
                    (hospitalizacionProductoAplicacion.HospitalizacionProducto.ProductoId,
                    hospitalizacionProductoAplicacion.HospitalizacionProducto.UnidadMedidaVentaId,
                    hospitalizacionProductoAplicacion.Cantidad);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    ProductoAplicado = new
                    {
                        hospitalizacionProductoAplicacionId,
                        Aplicado = true,
                        FechaHoraAplicacion = DateTime.Now,
                        UsuarioAplica = _userManager.GetUserId(HttpContext.User),
                        Subtotal = hospitalizacionProductoAplicacion.HospitalizacionProducto.PrecioValor * hospitalizacionProductoAplicacion.Cantidad  // ← AGREGAR

                    }
                });

            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al aplicar producto. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string AplicarServicio(int hospitalizacionServicioId, int cuentaId)
        {
            try
            {
                var hospitalizacionServicio = _hospitalizacionRepository
                    .GetServicioHospitalizacion(hospitalizacionServicioId);

                hospitalizacionServicio.Aplicado = true;
                hospitalizacionServicio.FechaHoraAplicacion = DateTime.Now;
                hospitalizacionServicio.UsuarioAplica = _userManager.GetUserId(HttpContext.User);
                _hospitalizacionRepository.Update(hospitalizacionServicio);

                // Actualizar cuenta por cobrar
                var cuenta = _cuentasPorCobrarRepository.Get(cuentaId);
                if (cuenta != null)
                {
                    var valorCuenta = cuenta.Valor ?? 0;
                    valorCuenta += hospitalizacionServicio.Precio;
                    cuenta.Valor = valorCuenta;
                    _cuentasPorCobrarRepository.Update(cuenta);
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Servicio aplicado exitosamente",
                    ServicioAplicado = new
                    {
                        servicioId = hospitalizacionServicio.Id,
                        FechaHoraAplicacion = hospitalizacionServicio.FechaHoraAplicacion,
                        UsuarioAplica = hospitalizacionServicio.UsuarioAplica
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al aplicar producto. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ActualizarEstadia(int hospitalizacionId, string periodo)
        {
            try
            {
                var fechas = periodo.Split("-");
                var fechaInicio = Convert.ToDateTime(fechas[0], culture);
                var fechaFin = Convert.ToDateTime(fechas[1], culture);

                var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId);
                //Se utiliza para la actualizacion
                //de valor en cuenta por cobrar y se toma aca antes de que se modifique
                //las fechas de la hospitalizacion
                var cantidadAnteriorNoches =
                    (hospitalizacion.FechaFin - hospitalizacion.FechaInicio).Days;
                hospitalizacion.FechaInicio = fechaInicio;
                hospitalizacion.FechaFin = fechaFin;
                _hospitalizacionRepository.Update(hospitalizacion);

                #region Actualizacion de cuenta por cobrar

                var tarifa = hospitalizacion.CategoriaHabitacionTarifa;
                if (cantidadAnteriorNoches == 0)
                {
                    cantidadAnteriorNoches = 1;
                }
                var cantidadNuevaNoches = (fechaFin - fechaInicio).Days;
                if (cantidadNuevaNoches == 0)
                {
                    cantidadNuevaNoches = 1;
                }
                decimal valorTarifa = 0;
                if (tarifa != null)
                {

                    valorTarifa = tarifa.ValorTarifa;
                }

                var valorAnterior = valorTarifa * cantidadAnteriorNoches;
                var valorNuevo = valorTarifa * cantidadNuevaNoches;

                var detalleCuenta = _hospitalizacionRepository.GetDetalleCuenta(hospitalizacionId);
                if (detalleCuenta != null && detalleCuenta.CuentaPorCobrar != null)
                {
                    var valorCuenta = detalleCuenta.CuentaPorCobrar.Valor ?? 0;
                    valorCuenta -= valorAnterior;
                    valorCuenta += valorNuevo;
                    detalleCuenta.CuentaPorCobrar.Valor = valorCuenta;
                    _cuentasPorCobrarRepository.Update(detalleCuenta);
                }

                #endregion

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
                    Mensaje = "Error al actualizar estadia. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string FinalizarHospitalizacion(int hospitalizacionId)
        {
            try
            {
                var fechaHora = DateTime.Now;
                var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId, false);

                // Marcar la hospitalización como finalizada
                hospitalizacion.Finalizada = true;
                hospitalizacion.FechaHoraFinalizada = fechaHora;
                _hospitalizacionRepository.Update(hospitalizacion);

                // Liberar la habitación
                var habitacion = _habitacionRepository.Get(hospitalizacion.HabitacionId);
                habitacion.EstadoHabitacionId = (int)EstadoHabitacionEnum.Disponible;
                _habitacionRepository.Update(habitacion);

                // Verificar si hay cuentas pendientes
                var cuentaPendiente = _cuentasPorCobrarRepository
                    .GetUltimaCuentaPendientePaciente(hospitalizacion.PacienteId);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Pagada = hospitalizacion.Pagada,
                    CuentaId = cuentaPendiente?.Id
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al finalizar hospitalización. " + ex.Message
                });
            }
        }




        [HttpPost]
        public IActionResult AplicarDetallePaqueteHospitalizacion(int Id, int Cantidad)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                // 1. Obtener el detalle de referencia (sin tracking)
                var referencia = _db.HospitalizacionDetallePaqueteHospitalizacion
                    .AsNoTracking()
                    .FirstOrDefault(d => d.Id == Id);

                if (referencia == null)
                    return Content(JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "El elemento no existe." }), "application/json");

                // 2. Buscar pendientes del mismo tipo DENTRO DEL MISMO PAQUETE
                var pendientes = _db.HospitalizacionDetallePaqueteHospitalizacion
                    .Where(d => !d.Aplicacion && !d.Eliminado
                             && d.HospitalizacionPaqueteHospitalizacionId == referencia.HospitalizacionPaqueteHospitalizacionId
                             && (
                                 (referencia.ProductoId != null && d.ProductoId == referencia.ProductoId) ||
                                 (referencia.ServicioId != null && d.ServicioId == referencia.ServicioId) ||
                                 (referencia.LaboratorioId != null && d.LaboratorioId == referencia.LaboratorioId)
                                ))
                    .OrderBy(d => d.Id)
                    .Take(Cantidad)
                    .ToList();

                if (!pendientes.Any())
                    return Content(JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "No hay unidades pendientes de este tipo." }), "application/json");

                string usuarioId = _userManager.GetUserId(HttpContext.User);
                DateTime ahora = DateTime.Now;
                const int bodegaHospId = (int)AmbienteEnum.Hospital;

                foreach (var item in pendientes)
                {
                    item.Aplicacion = true;
                    item.UsuarioAplicacionId = usuarioId;
                    item.FechaHoraAplicada = ahora;

                    // 3. Descontar inventario SOLO para PRODUCTOS
                    if (item.ProductoId != null)
                    {
                        int productoId = item.ProductoId.Value;
                        int unidadVenta = item.UnidadMedidaVentaId ?? 1;

                        var inventario =
                            _db.ProductosInventario.FirstOrDefault(pi =>
                                pi.ProductoId == productoId &&
                                pi.UnidadMedidaVentaId == unidadVenta &&
                                pi.BodegaId == bodegaHospId)
                            ?? _db.ProductosInventario.FirstOrDefault(pi =>
                                pi.ProductoId == productoId &&
                                pi.BodegaId == bodegaHospId)
                            ?? _db.ProductosInventario.FirstOrDefault(pi =>
                                pi.ProductoId == productoId &&
                                pi.UnidadMedidaVentaId == unidadVenta)
                            ?? _db.ProductosInventario.FirstOrDefault(pi =>
                                pi.ProductoId == productoId);

                        if (inventario == null)
                        {
                            transaction.Rollback();
                            return Content(JsonSerializer.Serialize(new
                            {
                                Exitoso = false,
                                Mensaje = $"No hay inventario registrado para el producto ID {productoId}. Contacte al administrador."
                            }), "application/json");
                        }

                        if (inventario.Stock < 1)
                        {
                            transaction.Rollback();
                            return Content(JsonSerializer.Serialize(new
                            {
                                Exitoso = false,
                                Mensaje = $"Stock insuficiente para el producto '{item.ProductoId}'. Disponible: {inventario.Stock}"
                            }), "application/json");
                        }

                        inventario.Stock -= 1;
                    }
                }

                // 4. Guardar estado + descuentos de inventario en un solo SaveChanges
                int guardados = _db.SaveChanges();
                if (guardados == 0)
                {
                    transaction.Rollback();
                    return Content(JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "No se pudo guardar ningún cambio." }), "application/json");
                }

                transaction.Commit();

                string persona = _userRepository.GetbyId(usuarioId)?.NormalizedUserName ?? User.Identity?.Name ?? "-";
                string fechaStr = ahora.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("es-GT"));

                return Content(JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Aplicados = pendientes.Count,
                    FechaHoraAplicacion = fechaStr,
                    Persona = persona
                }), "application/json");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Content(JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error interno: " + ex.Message }), "application/json");
            }
        }


        [HttpPost]
        public string ConsultarDetallePaquetesHospitalizacionAplicados(int hospitalizacionId)
        {
            try
            {
                var cultura = new CultureInfo("es-GT");

                var productosAplicacion = _hospitalizacionRepository
                    .GetHospitalizacionPaqueteByIdHospitalizacion(hospitalizacionId);

                if (productosAplicacion == null || !productosAplicacion.Any())
                    return JsonSerializer.Serialize(new { Exitoso = true, Resultado = new List<object>() });

                var paquetesActivos = productosAplicacion
                    .Where(p => !p.Eliminado)
                    .ToList();

                if (!paquetesActivos.Any())
                    return JsonSerializer.Serialize(new { Exitoso = true, Resultado = new List<object>() });

                var todosLosDetalles = paquetesActivos
                    .SelectMany(p => p.HospitalizacionDetallePaqueteHospitalizacion
                                      .Where(d => !d.Eliminado))
                    .ToList();

                var productosIds = todosLosDetalles.Where(d => d.ProductoId != null).Select(d => d.ProductoId.Value).Distinct().ToList();
                var serviciosIds = todosLosDetalles.Where(d => d.ServicioId != null).Select(d => d.ServicioId.Value).Distinct().ToList();
                var laboratoriosIds = todosLosDetalles.Where(d => d.LaboratorioId != null).Select(d => d.LaboratorioId.Value).Distinct().ToList();

                var productos = _db.Productos
                    .Where(p => productosIds.Contains(p.Id))
                    .ToDictionary(p => p.Id);

                var servicios = _db.Servicios
                    .Where(s => serviciosIds.Contains(s.Id))
                    .ToDictionary(s => s.Id);

                var laboratorios = _db.ExamenLabClinicos
                    .Where(l => laboratoriosIds.Contains(l.Id))
                    .ToDictionary(l => l.Id);

                var resultado = new List<object>();

                // ── PRODUCTOS ──────────────────────────────────────────────────────
                var gruposProducto = todosLosDetalles
                    .Where(d => d.ProductoId != null)
                    .GroupBy(d => new { d.ProductoId, d.HospitalizacionPaqueteHospitalizacionId });

                foreach (var g in gruposProducto)
                {
                    var primero = g.First();
                    var total = g.Count();
                    var aplicados = g.Count(x => x.Aplicacion);
                    var ultimo = g.Where(x => x.Aplicacion)
                                     .OrderByDescending(x => x.FechaHoraAplicada)
                                     .FirstOrDefault();
                    var pendiente = g.OrderBy(x => x.Id)
                                     .FirstOrDefault(x => !x.Aplicacion);

                    productos.TryGetValue(primero.ProductoId.Value, out var prod);

                    resultado.Add(new
                    {
                        Id = pendiente?.Id ?? primero.Id,   // para APLICAR
                        IdAplicado = ultimo?.Id,                     // para DEVOLVER
                        Tipo = "Producto",
                        Codigo = prod?.CodigoReferencia ?? "-",
                        Nombre = prod?.NombreProducto ?? "-",
                        Descripcion = prod?.Descripcion ?? "-",
                        Cantidad = total,
                        CantidadAplicados = aplicados,
                        Aplicado = aplicados >= total,
                        FechaHoraAplicacion = ultimo?.FechaHoraAplicada.HasValue == true
                                                    ? ultimo.FechaHoraAplicada.Value.ToString("dd/MM/yyyy hh:mm:ss tt", cultura)
                                                    : "-",
                        Persona = ObtenerPersona(ultimo?.UsuarioAplicacionId),
                        ExamenId = (int?)null,
                        DetalleExamenId = (int?)null,
                        PaqueteHospitalizacionId = primero.HospitalizacionPaqueteHospitalizacionId,
                    });
                }

                // ── SERVICIOS ──────────────────────────────────────────────────────
                var gruposServicio = todosLosDetalles
                    .Where(d => d.ServicioId != null && d.ProductoId == null)
                    .GroupBy(d => new { d.ServicioId, d.HospitalizacionPaqueteHospitalizacionId });

                foreach (var g in gruposServicio)
                {
                    var primero = g.First();
                    var total = g.Count();
                    var aplicados = g.Count(x => x.Aplicacion);
                    var ultimo = g.Where(x => x.Aplicacion)
                                     .OrderByDescending(x => x.FechaHoraAplicada)
                                     .FirstOrDefault();
                    var pendiente = g.OrderBy(x => x.Id)
                                     .FirstOrDefault(x => !x.Aplicacion);

                    servicios.TryGetValue(primero.ServicioId.Value, out var serv);

                    resultado.Add(new
                    {
                        Id = pendiente?.Id ?? primero.Id,
                        IdAplicado = ultimo?.Id,
                        Tipo = "Servicio",
                        Codigo = serv?.CodigoInterno ?? "-",
                        Nombre = serv?.NombreServicio ?? "-",
                        Descripcion = serv?.Descripcion ?? "-",
                        Cantidad = total,
                        CantidadAplicados = aplicados,
                        Aplicado = aplicados >= total,
                        FechaHoraAplicacion = ultimo?.FechaHoraAplicada.HasValue == true
                                                    ? ultimo.FechaHoraAplicada.Value.ToString("dd/MM/yyyy hh:mm:ss tt", cultura)
                                                    : "-",
                        Persona = ObtenerPersona(ultimo?.UsuarioAplicacionId),
                        ExamenId = (int?)null,
                        DetalleExamenId = (int?)null,
                        PaqueteHospitalizacionId = primero.HospitalizacionPaqueteHospitalizacionId,
                    });
                }

                // ── LABORATORIOS ───────────────────────────────────────────────────
                var gruposLaboratorio = todosLosDetalles
                    .Where(d => d.LaboratorioId != null)
                    .GroupBy(d => new { d.LaboratorioId, d.HospitalizacionPaqueteHospitalizacionId });

                foreach (var g in gruposLaboratorio)
                {
                    var primero = g.First();
                    var total = g.Count();
                    var aplicados = g.Count(x => x.Aplicacion);
                    var ultimo = g.Where(x => x.Aplicacion)
                                     .OrderByDescending(x => x.FechaHoraAplicada)
                                     .FirstOrDefault();
                    var pendiente = g.OrderBy(x => x.Id)
                                     .FirstOrDefault(x => !x.Aplicacion);

                    laboratorios.TryGetValue(primero.LaboratorioId.Value, out var lab);

                    resultado.Add(new
                    {
                        Id = pendiente?.Id ?? primero.Id,
                        IdAplicado = ultimo?.Id,
                        Tipo = "Laboratorio",
                        Codigo = lab?.CodigoInterno ?? "-",
                        Nombre = lab?.NombreExamen ?? "-",
                        Descripcion = lab?.TipoDeExamen ?? "-",
                        Cantidad = total,
                        CantidadAplicados = aplicados,
                        Aplicado = aplicados >= total,
                        FechaHoraAplicacion = ultimo?.FechaHoraAplicada.HasValue == true
                                                    ? ultimo.FechaHoraAplicada.Value.ToString("dd/MM/yyyy hh:mm:ss tt", cultura)
                                                    : "-",
                        Persona = ObtenerPersona(ultimo?.UsuarioAplicacionId),
                        ExamenId = primero.ExamenId,
                        DetalleExamenId = (int?)null,
                        PaqueteHospitalizacionId = primero.HospitalizacionPaqueteHospitalizacionId,
                    });
                }

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = resultado });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar los paquetes. " + ex.Message
                });
            }
        }
        private string ObtenerPersona(string usuarioId)
        {
            if (string.IsNullOrEmpty(usuarioId)) return "-";
            try { return _userRepository.GetbyId(usuarioId)?.NormalizedUserName ?? "-"; }
            catch { return "-"; }
        }



        [HttpPost]
        public string ConsultarArchivosPaciente(int hospitalizacionId)
        {
            try
            {
                // Obtener la hospitalización utilizando el hospitalizacionId
                var hospitalizacion = _hospitalizacionRepository.GetHospitalizacionById(hospitalizacionId);

                if (hospitalizacion == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se encontró la hospitalización."
                    });
                }

                // Crear un objeto con la URL del archivo de consentimiento
                var archivos = new List<PacienteArchivoVM>
                {
                    new PacienteArchivoVM
                    {
                        ArchivoId = hospitalizacion.Id,
                        ArchivoFecha = hospitalizacion.FechaInicio.ToString("yyyy-MM-dd"),
                        ArchivoNombre = "Archivo de Consentimiento",
                        ArchivoUrl = hospitalizacion.UrlArchivoConsentimiento // Usar la URL de consentimiento
                    }
                };

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = archivos
                });
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar archivos: " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarArchivosAutorizaciones(int hospitalizacionId)
        {
            try
            {
                // Obtener la información de la hospitalización por hospitalizacionId
                var hospitalizacion = _hospitalizacionRepository.GetHospitalizacionById(hospitalizacionId);

                if (hospitalizacion == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se encontró la hospitalización."
                    });
                }

                var autorizacionesPath = Path.Combine("wwwroot", "Autorizaciones");

                // Crear la carpeta si no existe
                if (!Directory.Exists(autorizacionesPath))
                {
                    Directory.CreateDirectory(autorizacionesPath);
                }

                // Simular una lista de archivos relacionados con la hospitalización
                var archivos = new List<ArchivoAutorizacionVM>
                {
                    new ArchivoAutorizacionVM
                    {
                        ArchivoId = hospitalizacion.Id,
                        ArchivoNombre = "Archivo Autorización " + hospitalizacion.Id,
                        ArchivoUrl = $"/Autorizaciones/{hospitalizacion}" // Asegúrate de que hospitalizacion.NombreArchivo exista
                    }
                };

                // Retornar resultado exitoso
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = archivos
                });
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar archivos: " + ex.Message
                });
            }
        }


        [HttpPost]
        public string ConsultarHistorialHospitalizacionesPaciente(int pacienteId)
        {
            try
            {
                var historial = _pacientesService.GetHistorialHospitalizaciones(pacienteId);


                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = historial
                });

            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar historial de hospitalizaciones"
                });
            }
        }
        [HttpPost]
        public string ConsultarHistorialConsultasPaciente(int pacienteId)
        {
            try
            {
                var historial = _pacientesService.GetHistorialConsultas(pacienteId);


                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = historial
                });

            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar historial de paciente"
                });
            }
        }

        [HttpPost]
        public async Task<string> UploadConsentimientoFirmado(IFormFile file, string nuevoNombreArchivo = null)
        {
            try
            {
                // Si no se proporciona un nuevo nombre, usamos el nombre original del archivo
                var extensionArchivo = Path.GetExtension(file.FileName);
                var nombreArchivoFinal = string.IsNullOrEmpty(nuevoNombreArchivo)
                    ? Path.GetFileName(file.FileName) // Usar el nombre original si no se da un nombre nuevo
                    : (Path.HasExtension(nuevoNombreArchivo) ? nuevoNombreArchivo : $"{nuevoNombreArchivo}{extensionArchivo}"); // Si ya tiene la extensión no se agrega

                // Crear un archivo temporal con el nombre que queremos usar
                var archivoRenombrado = new FormFile(file.OpenReadStream(), 0, file.Length, file.Name, nombreArchivoFinal);

                // Subir el archivo usando el servicio sin modificar
                var resultadoSubirArchivo = await _filesService.UploadFile(archivoRenombrado, "hospitalizacion-consentimientos");

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = resultadoSubirArchivo
                });
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al subir consentimiento"
                });
            }
        }


        [HttpPost]
        public string HospitalizacionHistorialHabitacion(int hospitalizacionId)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null, // Mantiene Exitoso/Resultado/Mensaje tal cual
                WriteIndented = false
            };

            try
            {
                if (hospitalizacionId <= 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "El hospitalizacionId no es válido.",
                        Resultado = Array.Empty<object>()
                    }, jsonOptions);
                }

                var cambios = _hospitalizacionRepository.GetCambiosHabitacion(hospitalizacionId);

                var cambiosLista = (cambios ?? Enumerable.Empty<HospitalizacionCambioHabitacion>())
                    .OrderByDescending(c => c.FechaCambio) // más nuevo -> más viejo
                    .ToList();

                var resultado = new List<object>(cambiosLista.Count);

                foreach (var c in cambiosLista)
                {
                    var habitacion = (c.HabitacionId > 0)
                        ? _habitacionRepository.Get(c.HabitacionId)
                        : null;

                    var nombreHabitacion = habitacion?.NombreNumeroHabitacion ?? string.Empty;
                    var nombreCategoria = habitacion?.CategoriaHabitacion?.NombreCategoria ?? string.Empty;

                    var valorTotal = c.Dias * c.ValorTarifa;

                    resultado.Add(new
                    {
                        c.Id,
                        c.HabitacionId,
                        Habitacion = nombreHabitacion,
                        Categoria = nombreCategoria,
                        c.FechaCambio,
                        c.Dias,
                        Tarifa = c.Tarifa ?? string.Empty,
                        c.ValorTarifa,
                        ValorTotal = valorTotal
                    });
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = string.Empty,
                    Resultado = resultado
                }, jsonOptions);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = ex.Message,
                    Resultado = Array.Empty<object>()
                }, jsonOptions);
            }
        }


        [HttpPost]
        public string EliminarCambioHabitacion(int cambioHabitacionId)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                WriteIndented = false
            };

            try
            {
                if (cambioHabitacionId <= 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "El ID del cambio de habitación no es válido."
                    }, jsonOptions);
                }

                var cambio = _hospitalizacionRepository.GetCambioHabitacion(cambioHabitacionId);
                if (cambio == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se encontró el registro de cambio de habitación."
                    }, jsonOptions);
                }

                _hospitalizacionRepository.DeleteCambioHabitacion(cambioHabitacionId);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Registro eliminado correctamente."
                }, jsonOptions);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = ex.Message
                }, jsonOptions);
            }
        }

        [HttpPost]
        public string ModificarTarifaCambioHabitacion(int cambioHabitacionId, decimal nuevaTarifa, int nuevosDias)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                WriteIndented = false
            };

            try
            {
                if (cambioHabitacionId <= 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "El ID del cambio de habitación no es válido."
                    }, jsonOptions);
                }

                if (nuevaTarifa < 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "La tarifa no puede ser negativa."
                    }, jsonOptions);
                }

                if (nuevosDias <= 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "El número de días debe ser mayor a cero."
                    }, jsonOptions);
                }

                var cambio = _hospitalizacionRepository.GetCambioHabitacion(cambioHabitacionId);
                if (cambio == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se encontró el registro de cambio de habitación."
                    }, jsonOptions);
                }

                cambio.ValorTarifa = nuevaTarifa;
                cambio.Dias = nuevosDias;
                _hospitalizacionRepository.UpdateCambioHabitacion(cambio);

                var nuevoTotal = nuevaTarifa * nuevosDias;

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Tarifa actualizada correctamente.",
                    ValorTotal = nuevoTotal
                }, jsonOptions);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = ex.Message
                }, jsonOptions);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubirDocumento(List<IFormFile> archivos, int hospitalizacionId)
        {
            try
            {
                if (archivos == null || archivos.Count == 0)
                    return Json(new { exitoso = false, mensaje = "No se seleccionó ningún archivo." });

                var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId);
                if (hospitalizacion == null)
                    return Json(new { exitoso = false, mensaje = "Hospitalización no encontrada." });

                var baseFolder = Path.Combine(_env.WebRootPath, "hospitalizacion");
                var pacienteFolder = Path.Combine(baseFolder, hospitalizacion.PacienteId.ToString());

                if (!Directory.Exists(pacienteFolder))
                    Directory.CreateDirectory(pacienteFolder);

                foreach (var archivo in archivos)
                {
                    if (archivo.Length == 0) continue;

                    var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
                    var nombreUnico = $"{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(pacienteFolder, nombreUnico);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await archivo.CopyToAsync(stream);
                    }

                    var rutaRelativa = $"hospitalizacion/{hospitalizacion.PacienteId}/{nombreUnico}";

                    var documento = new DocumentosHospitalizacion
                    {
                        HospitalizacionId = hospitalizacionId,
                        PacienteId = hospitalizacion.PacienteId,
                        NombreArchivo = archivo.FileName,
                        RutaArchivo = rutaRelativa,
                        FechaSubida = DateTime.Now,
                        Extension = extension,
                        Tamano = archivo.Length,
                        Eliminado = false,
                        Autorizado = false,
                        UsuarioAutoriza = null,
                        FechaAutorizacion = null
                    };

                    _hospitalizacionDocumentoRepository.Add(documento);
                }

                _hospitalizacionDocumentoRepository.SaveChanges();

                return Json(new { exitoso = true, mensaje = $"{archivos.Count} archivos subidos correctamente (pendientes de autorización)." });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = "Error al subir documentos: " + ex.Message });
            }
        }




        [HttpPost]
        public async Task<IActionResult> AutorizarDocumento([FromBody] AutorizarDocumentoRequest request)
        {
            try
            {
                // 1. Validar request básico
                if (request == null || request.DocumentoId <= 0 || string.IsNullOrWhiteSpace(request.HuellaPayload))
                    return Json(new { exitoso = false, mensaje = "Solicitud inválida." });

                // 2. Verificar huella digital
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var huellaPayload = JsonSerializer.Deserialize<WebAuthnAssertionPayload>(request.HuellaPayload, opts);
                if (huellaPayload == null)
                    return Json(new { exitoso = false, mensaje = "Payload de huella inválido." });

                var credIdString = Convert.ToBase64String(
                    Convert.FromBase64String(
                        huellaPayload.RawId.Replace('-', '+').Replace('_', '/')
                        + new string('=', (4 - huellaPayload.RawId.Length % 4) % 4)
                    ));
                var credencial = await _db.WebAuthnCredentials.FirstOrDefaultAsync(c => c.DescriptorId == credIdString);
                if (credencial == null)
                    return Json(new { exitoso = false, mensaje = "Credencial no registrada." });

                var authorizerId = credencial.UserId;

                // 3. Verificar la firma (WebAuthn)
                var verificacion = await _webAuthn.CompleteVerifyAsync(authorizerId, huellaPayload);
                if (!verificacion.Success)
                    return Json(new { exitoso = false, mensaje = verificacion.UserMessage });

                // 4. Obtener el documento
                var documento = _hospitalizacionDocumentoRepository.GetById(request.DocumentoId);
                if (documento == null)
                    return Json(new { exitoso = false, mensaje = "Documento no encontrado." });

                // 5. Validar que el documento tenga HospitalizacionId (si es int, siempre tiene valor)
                if (documento.HospitalizacionId == 0)
                    return Json(new { exitoso = false, mensaje = "El documento no está asociado a una hospitalización." });

                // 6. Obtener el usuario que autoriza (authorizerId)
                var authorizerUser = await _userManager.FindByIdAsync(authorizerId);
                if (authorizerUser == null)
                    return Json(new { exitoso = false, mensaje = "Usuario no encontrado." });

                // 7. Validar permisos: Administrador o médico asignado
                var esAdmin = await _userManager.IsInRoleAsync(authorizerUser, "Administrador");
                if (!esAdmin)
                {
                    // Obtener CitasId desde Consulta asociada a la hospitalización (puede ser null)
                    var citasId = await _db.Consultas
                        .Where(c => c.HospitalizacionId == documento.HospitalizacionId)
                        .Select(c => c.CitasId)
                        .FirstOrDefaultAsync();

                    if (citasId == null || citasId == 0)
                        return Json(new { exitoso = false, mensaje = "No se encontró la consulta asociada." });

                    // Obtener médico asignado y médicos secundarios
                    var cita = await _db.Citass
                        .Where(c => c.Id == citasId)
                        .Select(c => new { c.EmpleadoId, c.MedicosSecundarios })
                        .FirstOrDefaultAsync();

                    if (cita == null)
                        return Json(new { exitoso = false, mensaje = "No se encontró la cita." });

                    var empleadosAutorizados = new List<int>();
                    if (cita.EmpleadoId.HasValue)
                        empleadosAutorizados.Add(cita.EmpleadoId.Value);
                    if (cita.MedicosSecundarios != null)
                        empleadosAutorizados.AddRange(cita.MedicosSecundarios);

                    var userIdsAutorizados = await _db.Usuarios
                        .Where(u => u.EmpleadoId != null && empleadosAutorizados.Contains(u.EmpleadoId.Value))
                        .Select(u => u.Id)
                        .ToListAsync();

                    if (!userIdsAutorizados.Contains(authorizerId))
                        return Json(new { exitoso = false, mensaje = "Usuario no autorizado para autorizar este documento." });
                }

                // 8. Actualizar estado del documento
                documento.Autorizado = true;
                documento.UsuarioAutoriza = authorizerId;
                documento.FechaAutorizacion = DateTime.Now;
                _hospitalizacionDocumentoRepository.Update(documento);
                _hospitalizacionDocumentoRepository.SaveChanges();
                return Json(new { exitoso = true, mensaje = "Documento autorizado correctamente." });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error en AutorizarDocumento");
                return Json(new { exitoso = false, mensaje = ex.Message });
            }
        }

        public class AutorizarDocumentoRequest
        {
            public int DocumentoId { get; set; }
            public string HuellaPayload { get; set; }
        }

        private string GetMimeType(string extension)
        {
            return extension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }

        [HttpGet]
        public IActionResult DescargarDocumento(int id)
        {
            var documento = _hospitalizacionDocumentoRepository.GetById(id);
            if (documento == null)
                return NotFound();

            var rutaFisica = Path.Combine(_env.WebRootPath, documento.RutaArchivo.Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(rutaFisica))
                return NotFound();

            var mimeType = GetMimeType(documento.Extension);
            var stream = System.IO.File.OpenRead(rutaFisica);

            return File(stream, mimeType, documento.NombreArchivo, enableRangeProcessing: true);
        }

        [HttpGet]
        public IActionResult ListarDocumentos(int hospitalizacionId)
        {
            try
            {
                var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId);
                if (hospitalizacion == null)
                    return Json(new { exitoso = false, mensaje = "Hospitalización no encontrada." });

                var documentosHosp = _hospitalizacionDocumentoRepository.GetByHospitalizacionId(hospitalizacionId);
                var documentosPaciente = _hospitalizacionDocumentoRepository.GetByPacienteId(hospitalizacion.PacienteId);

                var todos = documentosHosp.Concat(documentosPaciente)
                    .GroupBy(d => d.Id)
                    .Select(g => g.First())
                    .OrderByDescending(d => d.FechaSubida);

                var resultado = todos.Select(d => new
                {
                    d.Id,
                    d.NombreArchivo,
                    d.RutaArchivo,
                    d.FechaSubida,
                    d.Extension,
                    TamanoFormateado = FormatearTamano(d.Tamano),
                    Usuario = "Sistema",
                    UrlDescarga = Url.Action("DescargarDocumento", "Hospitalizacion", new { id = d.Id }),
                    Autorizado = d.Autorizado,
                    UsuarioAutoriza = d.UsuarioAutoriza,
                    FechaAutorizacion = d.FechaAutorizacion?.ToString("yyyy-MM-dd HH:mm:ss")
                });

                return Json(new { exitoso = true, documentos = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult EliminarDocumento(int id)
        {
            try
            {
                var documento = _hospitalizacionDocumentoRepository.GetById(id);
                if (documento == null)
                    return Json(new { exitoso = false, mensaje = "Documento no encontrado." });

                _hospitalizacionDocumentoRepository.Delete(id);
                _hospitalizacionDocumentoRepository.SaveChanges();

                var rutaFisica = Path.Combine(_env.WebRootPath, documento.RutaArchivo.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(rutaFisica))
                    System.IO.File.Delete(rutaFisica);

                return Json(new { exitoso = true, mensaje = "Documento eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = ex.Message });
            }
        }

        private string FormatearTamano(long bytes)
        {
            string[] sufijos = { "B", "KB", "MB", "GB", "TB" };
            int indice = 0;
            double tamaño = bytes;
            while (tamaño >= 1024 && indice < sufijos.Length - 1)
            {
                tamaño /= 1024;
                indice++;
            }
            return $"{tamaño:0.##} {sufijos[indice]}";
        }






        [HttpPost]
        public async Task<string> AgregarInsumoDirecto(HospitalizacionAgregarMedicamentoViewModel model)
        {
            try
            {
                var usuarioId = _userManager.GetUserId(HttpContext.User);
                var producto = _productoRepository.GetProdutoById(model.ProductoId);
                string nombreProducto = producto?.NombreProducto ?? "Producto";

                // 1. Guardar insumo directo
                var insumo = new HospitalizacionInsumoDirecto
                {
                    HospitalizacionId = model.HospitalizacionId,
                    ProductoId = model.ProductoId,
                    UnidadMedidaVentaId = model.UnidadMedidaVentaId > 0 ? model.UnidadMedidaVentaId : 1,
                    PrecioId = model.PrecioId,
                    PrecioValor = model.Precio,
                    Cantidad = model.Cantidad,
                    Indicaciones = model.Indicaciones ?? "",
                    ViaAdministracion = model.ViaAdministracion ?? "",
                    FrecuenciaAdministracion = model.FrecuenciaAdministracion ?? "",
                    FechaHoraAplicacionManual = model.FechaHoraAplicacionManual,
                    UsuarioCreaId = usuarioId,
                    FechaCreacion = DateTime.Now,
                    Eliminado = false
                };

                _db.HospitalizacionInsumosDirectos.Add(insumo);
                await _db.SaveChangesAsync();

                for (int i = 0; i < insumo.Cantidad; i++)
                {
                    _db.HospitalizacionInsumosDirectosAplicaciones.Add(
                        new HospitalizacionInsumoDirectoAplicacion
                        {
                            HospitalizacionInsumoDirectoId = insumo.Id,
                            Cantidad = 1,
                            Aplicado = false,
                            UsuarioCreaId = usuarioId
                        });
                }
                await _db.SaveChangesAsync();

                // 2. Enviar notificaciones (con la URL base)
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                await _medicamentoNotificacionService.ProgramarNotificacionesAsync(
                    model.HospitalizacionId,
                    nombreProducto,
                    model.Cantidad,
                    model.Indicaciones ?? "",
                    model.ViaAdministracion ?? "",
                    model.FrecuenciaAdministracion ?? "",
                    model.FechaHoraAplicacionManual,
                    baseUrl,
                    usuarioId);

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al agregar insumo directo. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string ConsultarInsumosDirectosAplicacion(int hospitalizacionId)
        {
            try
            {
                var aplicaciones = _db.HospitalizacionInsumosDirectosAplicaciones
                    .Include(a => a.HospitalizacionInsumoDirecto)
                        .ThenInclude(i => i.Producto)
                    .Include(a => a.HospitalizacionInsumoDirecto)
                        .ThenInclude(i => i.UnidadMedidaVenta)
                    .Where(a =>
                        a.HospitalizacionInsumoDirecto.HospitalizacionId == hospitalizacionId &&
                        !a.HospitalizacionInsumoDirecto.Eliminado)
                    .OrderByDescending(a => a.Id)
                    .ToList();

                if (!aplicaciones.Any())
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = true,
                        Resultado = new List<HospitalizacionInsumoDirectoAplicacionViewModel>()
                    });
                }

                // Resolver nombres de usuario en batch
                var userIds = aplicaciones
                    .SelectMany(a => new[] { a.UsuarioCreaId, a.UsuarioAplica })
                    .Where(id => id != null)
                    .Distinct()
                    .ToList();

                var usuarios = _userRepository.GetByIds(userIds)
                    .ToDictionary(u => u.Id, u => u);

                var empleadoIds = usuarios.Values
                    .Select(u => u.EmpleadoId)
                    .Where(id => id != null)
                    .Distinct()
                    .Select(id => (int)id)
                    .ToList();

                var empleados = _empleadoRepository.GetByIds(empleadoIds)
                    .ToDictionary(e => e.Id, e => e);

                string ResolverNombre(string userId)
                {
                    if (userId == null || !usuarios.ContainsKey(userId)) return "-";
                    var empId = usuarios[userId].EmpleadoId;
                    if (empId == null) return "Admin";
                    return empleados.ContainsKey((int)empId)
                        ? empleados[(int)empId].NombreYApellidos
                        : "Admin";
                }

                var lista = new List<HospitalizacionInsumoDirectoAplicacionViewModel>();

                foreach (var a in aplicaciones)
                {
                    // Omitir ya aplicados del listado de pendientes (igual que el flujo original)
                    if (a.Aplicado) continue;

                    string fechaAplicacion = a.Aplicado && a.FechaHoraAplicacion.HasValue
                        ? a.FechaHoraAplicacion.Value.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("es-GT"))
                        : "-";

                    string fechaManual = DateTime.TryParse(
    a.HospitalizacionInsumoDirecto.FechaHoraAplicacionManual,
    out DateTime fechaConvertida)
    ? fechaConvertida.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("es-GT"))
    : "-";

                    lista.Add(new HospitalizacionInsumoDirectoAplicacionViewModel
                    {
                        Id = a.Id,
                        ProductoId = a.HospitalizacionInsumoDirecto.ProductoId,
                        Nombre = a.HospitalizacionInsumoDirecto.Producto?.NombreProducto ?? "-",
                        Cantidad = a.Cantidad,
                        Indicaciones = a.HospitalizacionInsumoDirecto.Indicaciones,
                        ViaAdministracion = a.HospitalizacionInsumoDirecto.ViaAdministracion,
                        FrecuenciaAdministracion = a.HospitalizacionInsumoDirecto.FrecuenciaAdministracion,
                        Aplicado = a.Aplicado,
                        FechaHoraAplicacion = fechaAplicacion,
                        FechaHoraAplicacionManual = fechaManual,
                        PersonaAplica = ResolverNombre(a.UsuarioAplica),
                        PersonaCrea = ResolverNombre(a.UsuarioCreaId),
                        UnidadMedidaVentaNombre = a.HospitalizacionInsumoDirecto.UnidadMedidaVenta?.Nombre ?? "-"
                    });
                }

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = lista });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar insumos directos. " + ex.Message
                });
            }
        }


        // ────────────────────────────────────────────────────────────
        // INSUMOS DIRECTOS — Aplicar
        // POST /Hospitalizacion/AplicarInsumoDirecto
        // ────────────────────────────────────────────────────────────
        // [HttpPost]
        // public string AplicarInsumoDirecto(int insumoDirectoAplicacionId, int cuentaId)
        // {
        //     try
        //     {
        //         var aplicacion = _db.HospitalizacionInsumosDirectosAplicaciones
        //             .Include(a => a.HospitalizacionInsumoDirecto)
        //             .FirstOrDefault(a => a.Id == insumoDirectoAplicacionId);

        //         if (aplicacion == null)
        //             return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Registro no encontrado." });

        //         aplicacion.Aplicado = true;
        //         aplicacion.FechaHoraAplicacion = DateTime.Now;
        //         aplicacion.UsuarioAplica = _userManager.GetUserId(HttpContext.User);
        //         _db.SaveChanges();

        //         // Actualizar cuenta por cobrar
        //         var cuenta = _cuentasPorCobrarRepository.GetCuenta(cuentaId);
        //         if (cuenta != null)
        //         {
        //             cuenta.Valor = (cuenta.Valor ?? 0) + aplicacion.HospitalizacionInsumoDirecto.PrecioValor;
        //             _cuentasPorCobrarRepository.Update(cuenta);
        //         }


        //         Console.WriteLine($"[DEBUG] AplicarInsumoDirecto: ProductoId={ aplicacion.HospitalizacionInsumoDirecto.ProductoId}, UnidadMedidaVentaId={ aplicacion.HospitalizacionInsumoDirecto.UnidadMedidaVentaId}, Cantidad={ aplicacion.HospitalizacionInsumoDirecto.Cantidad}");
        //         // Descontar inventario
        //         _productosService.RealizarDescuentoInventario(
        //             aplicacion.HospitalizacionInsumoDirecto.ProductoId,
        //             aplicacion.HospitalizacionInsumoDirecto.UnidadMedidaVentaId,
        //             aplicacion.Cantidad);

        //         return JsonSerializer.Serialize(new
        //         {
        //             Exitoso = true,
        //             ProductoAplicado = new
        //             {
        //                 insumoDirectoAplicacionId,
        //                 Aplicado = true,
        //                 FechaHoraAplicacion = DateTime.Now,
        //                 UsuarioAplica = aplicacion.UsuarioAplica,
        //                 Subtotal = aplicacion.HospitalizacionInsumoDirecto.PrecioValor * aplicacion.Cantidad  // ← AGREGAR

        //             }
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return JsonSerializer.Serialize(new
        //         {
        //             Exitoso = false,
        //             Mensaje = "Error al aplicar insumo directo. " + ex.Message
        //         });
        //     }
        // }




        [HttpPost]
        public string AplicarInsumoDirecto(int insumoDirectoAplicacionId, int cuentaId)
        {
            try
            {
                var aplicacion = _db.HospitalizacionInsumosDirectosAplicaciones
                    .Include(a => a.HospitalizacionInsumoDirecto)
                    .FirstOrDefault(a => a.Id == insumoDirectoAplicacionId);

                if (aplicacion == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Registro no encontrado." });

                aplicacion.Aplicado = true;
                aplicacion.FechaHoraAplicacion = DateTime.Now;
                aplicacion.UsuarioAplica = _userManager.GetUserId(HttpContext.User);
                _db.SaveChanges();

                // Actualizar cuenta por cobrar
                var cuenta = _cuentasPorCobrarRepository.GetCuenta(cuentaId);
                if (cuenta != null)
                {
                    cuenta.Valor = (cuenta.Valor ?? 0) + aplicacion.HospitalizacionInsumoDirecto.PrecioValor;
                    _cuentasPorCobrarRepository.Update(cuenta);
                }

                // --- DESCUENTO MANUAL DE INVENTARIO con Bodega prioritaria (Hospitalización) ---
                int productoId = aplicacion.HospitalizacionInsumoDirecto.ProductoId;
                int unidadVenta = aplicacion.HospitalizacionInsumoDirecto.UnidadMedidaVentaId;
                int cantidad = aplicacion.Cantidad;

                // 1. Definir la bodega objetivo para Hospitalización (Id fijo 8 según tu tabla)
                const int bodegaHospitalizacionId = 8;

                // 2. Buscar inventario en bodega de Hospitalización con la misma unidad de medida
                var inventario = _db.ProductosInventario
                    .Where(pi => pi.ProductoId == productoId
                              && pi.UnidadMedidaVentaId == unidadVenta
                              && pi.BodegaId == bodegaHospitalizacionId)
                    .FirstOrDefault();

                // 3. Si no existe, buscar cualquier inventario del producto en bodega Hospitalización (sin importar unidad)
                if (inventario == null)
                {
                    inventario = _db.ProductosInventario
                        .Where(pi => pi.ProductoId == productoId && pi.BodegaId == bodegaHospitalizacionId)
                        .FirstOrDefault();
                    if (inventario != null)
                        Console.WriteLine($"[INFO] Se usó inventario en bodega Hospitalización pero con unidad diferente (Id:{inventario.UnidadMedidaVentaId})");
                }

                // 4. Fallback: buscar en la bodega "Central" (Id=14) o cualquier otra si no hay en Hospitalización
                if (inventario == null)
                {
                    const int bodegaCentralId = 14;
                    inventario = _db.ProductosInventario
                        .Where(pi => pi.ProductoId == productoId && pi.UnidadMedidaVentaId == unidadVenta && pi.BodegaId == bodegaCentralId)
                        .FirstOrDefault();

                    if (inventario == null)
                    {
                        inventario = _db.ProductosInventario
                            .Where(pi => pi.ProductoId == productoId)
                            .FirstOrDefault();
                        if (inventario != null)
                            Console.WriteLine($"[WARN] No se encontró inventario en bodega Hospitalización ({bodegaHospitalizacionId}) ni Central ({bodegaCentralId}) para producto {productoId}. Se usó bodega {inventario.BodegaId} como fallback.");
                    }
                    else
                    {
                        Console.WriteLine($"[WARN] No hay inventario en bodega Hospitalización para producto {productoId}, se usó bodega Central (Id={bodegaCentralId})");
                    }
                }

                if (inventario != null)
                {
                    if (inventario.Stock >= cantidad)
                    {
                        inventario.Stock -= cantidad;
                        _db.SaveChanges();
                        Console.WriteLine($"[INFO] Descuento manual exitoso para producto {productoId} en bodega {inventario.BodegaId}. Nuevo stock: {inventario.Stock}");
                    }
                    else
                    {
                        Console.WriteLine($"[ERROR] Stock insuficiente para producto {productoId} en bodega {inventario.BodegaId}: {inventario.Stock} < {cantidad}");
                        // Opcional: lanzar excepción o notificar al usuario
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = $"Stock insuficiente en bodega {inventario.BodegaId}. Disponible: {inventario.Stock}, Requerido: {cantidad}"
                        });
                    }
                }
                else
                {
                    Console.WriteLine($"[ERROR] No se encontró ningún registro de inventario para el producto {productoId}. No se pudo descontar stock.");
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"No hay inventario registrado para el producto ID {productoId}. Contacte al administrador."
                    });
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    ProductoAplicado = new
                    {
                        insumoDirectoAplicacionId,
                        Aplicado = true,
                        FechaHoraAplicacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("es-GT")),
                        UsuarioAplica = aplicacion.UsuarioAplica,
                        Subtotal = aplicacion.HospitalizacionInsumoDirecto.PrecioValor * aplicacion.Cantidad
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al aplicar insumo directo. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarTotalInsumosDirectosAplicados(int hospitalizacionId)
        {
            try
            {
                var total = _db.HospitalizacionInsumosDirectosAplicaciones
                    .Include(a => a.HospitalizacionInsumoDirecto)
                    .Where(a =>
                        a.HospitalizacionInsumoDirecto.HospitalizacionId == hospitalizacionId &&
                        !a.HospitalizacionInsumoDirecto.Eliminado &&
                        a.Aplicado)
                    .Sum(a => a.HospitalizacionInsumoDirecto.PrecioValor * a.Cantidad);

                return JsonSerializer.Serialize(new { Exitoso = true, Total = total });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
            }
        }


        [HttpPost]
        public string ConsultarHistorialMedicamentosHospitalizacion(int hospitalizacionId)
        {
            try
            {
                var resultado = new List<object>();

                // ── 1. Control de Medicamentos ────────────────────────────────────────
                var medicamentos = _db.HospitalizacionesProductos
                    .Include(p => p.Producto)
                    .Include(p => p.HospitalizacionesProductosAplicaciones)
                    .Where(p => p.HospitalizacionId == hospitalizacionId && !p.Eliminado)
                    .ToList();

                foreach (var med in medicamentos)
                {
                    var apps = med.HospitalizacionesProductosAplicaciones.ToList();
                    bool aplicado = apps.Any(a => a.Aplicado);

                    var fechaApp = apps
                        .Where(a => a.Aplicado && a.FechaHoraAplicacion != null)
                        .OrderByDescending(a => a.FechaHoraAplicacion)
                        .FirstOrDefault()?.FechaHoraAplicacion;

                    resultado.Add(new
                    {
                        Id = 0,  // los medicamentos no tienen eliminación desde este modal
                        Origen = "Control de Medicamentos",
                        Nombre = med.Producto?.NombreProducto ?? "-",
                        CantidadTotal = apps.Sum(a => a.Cantidad),
                        Cantidad = apps.Where(a => a.Aplicado).Sum(a => a.Cantidad),
                        Indicaciones = med.Indicaciones ?? "-",
                        Via = med.ViaAdministracion ?? "-",
                        Frecuencia = med.FrecuenciaAdministracion ?? "-",
                        Aplicado = aplicado,
                        FechaAplicacion = fechaApp.Value.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("es-GT")),


                        FechaRegistro = DateTime.TryParse(med.FechaHoraAplicacionManual, out DateTime fReg)
    ? fReg.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("es-GT"))
    : "-"


                    });
                }  // ← cierra foreach medicamentos correctamente

                // ── 2. Control de Insumos Directos (una fila por cada aplicación, SIN AGRUPAR) ──
                var aplicacionesInsumos = _db.HospitalizacionInsumosDirectosAplicaciones
                    .Include(a => a.HospitalizacionInsumoDirecto)
                        .ThenInclude(i => i.Producto)
                    .Where(a => a.HospitalizacionInsumoDirecto.HospitalizacionId == hospitalizacionId &&
                                !a.HospitalizacionInsumoDirecto.Eliminado)
                    .ToList();

                foreach (var app in aplicacionesInsumos)
                {
                    var insumo = app.HospitalizacionInsumoDirecto;
                    resultado.Add(new
                    {
                        Id = app.Id,   // ← ID de la aplicación (útil para eliminar)
                        Origen = "Control de Insumos",
                        Nombre = insumo.Producto?.NombreProducto ?? "-",
                        CantidadTotal = 1,   // cada aplicación es de 1 unidad
                        Cantidad = app.Aplicado ? 1 : 0,
                        Indicaciones = insumo.Indicaciones ?? "-",
                        Via = insumo.ViaAdministracion ?? "-",
                        Frecuencia = insumo.FrecuenciaAdministracion ?? "-",
                        Aplicado = app.Aplicado,
                        FechaAplicacion = app.FechaHoraAplicacion.HasValue
                            ? app.FechaHoraAplicacion.Value.ToString("dd/MM/yyyy HH:mm", new CultureInfo("es-GT"))
                            : "-",
                        FechaRegistro = DateTime.TryParse(insumo.FechaHoraAplicacionManual, out DateTime fManual)
    ? fManual.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("es-GT"))
    : "-",
                        MotivoDevolucion = app.MotivoDevolucion ?? "-"

                    });
                }

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = resultado });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar historial: " + ex.Message
                });
            }
        }

        [HttpPost]
        public string EliminarInsumoDirecto(int hospitalizacionInsumoDirectoId)
        {
            try
            {
                var insumo = _db.HospitalizacionInsumosDirectos
                    .FirstOrDefault(i => i.Id == hospitalizacionInsumoDirectoId);

                if (insumo == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Insumo no encontrado." });

                insumo.Eliminado = true;
                _db.SaveChanges();

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al eliminar insumo. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string EliminarInsumoDirectoAplicacion(int aplicacionId)
        {
            try
            {
                var aplicacion = _db.HospitalizacionInsumosDirectosAplicaciones
                    .FirstOrDefault(a => a.Id == aplicacionId);
                if (aplicacion == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Aplicación no encontrada." });

                // Eliminar físicamente la aplicación
                _db.HospitalizacionInsumosDirectosAplicaciones.Remove(aplicacion);

                // Opcional: si el insumo padre ya no tiene más aplicaciones (pendientes o aplicadas)
                // se podría marcar como Eliminado, pero no es necesario para el historial.
                _db.SaveChanges();

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al eliminar insumo. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string DevolverInsumoAplicadoAPendiente(int aplicacionId, string motivoDevolucion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(motivoDevolucion))
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Debe seleccionar un motivo de devolución." });

                var aplicacion = _db.HospitalizacionInsumosDirectosAplicaciones
                    .Include(a => a.HospitalizacionInsumoDirecto)
                    .FirstOrDefault(a => a.Id == aplicacionId);

                if (aplicacion == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Aplicación no encontrada." });

                if (!aplicacion.Aplicado)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Este insumo ya está en estado Pendiente." });

                // Revertir estado
                aplicacion.Aplicado = false;
                aplicacion.FechaHoraAplicacion = null;
                aplicacion.UsuarioAplica = null;
                aplicacion.MotivoDevolucion = motivoDevolucion.Trim();

                // Devolver stock al inventario
                var insumo = aplicacion.HospitalizacionInsumoDirecto;
                var productoId = insumo.ProductoId;
                var unidadVenta = insumo.UnidadMedidaVentaId;
                var cantidad = aplicacion.Cantidad;

                const int bodegaHospitalizacionId = (int)AmbienteEnum.Hospital;
                var inventario = _db.ProductosInventario
                    .FirstOrDefault(pi => pi.ProductoId == productoId
                                       && pi.UnidadMedidaVentaId == unidadVenta
                                       && pi.BodegaId == bodegaHospitalizacionId)
                    ?? _db.ProductosInventario
                        .FirstOrDefault(pi => pi.ProductoId == productoId);

                if (inventario != null)
                    inventario.Stock += cantidad;

                _db.SaveChanges();

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al devolver insumo a pendiente. " + ex.Message
                });
            }
        }



        [HttpPost]
        public string EliminarDetallePaqueteHospitalizacion(int detalleId)
        {
            try
            {
                var detalle = _db.HospitalizacionDetallePaqueteHospitalizacion
                    .FirstOrDefault(d => d.Id == detalleId);

                if (detalle == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Elemento no encontrado." });

                if (detalle.Aplicacion)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "No se puede eliminar un elemento ya aplicado. Use la opción Devolver." });

                detalle.Eliminado = true;
                _db.SaveChanges();

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al eliminar el elemento. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string DevolverDetallePaqueteHospitalizacion(int detalleId, string motivoDevolucion)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrWhiteSpace(motivoDevolucion))
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Debe indicar un motivo de devolución." });

                var detalle = _db.HospitalizacionDetallePaqueteHospitalizacion
                    .FirstOrDefault(d => d.Id == detalleId);

                if (detalle == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Elemento no encontrado." });

                if (!detalle.Aplicacion)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Este elemento ya está en estado Pendiente." });

                // Revertir aplicación
                detalle.Aplicacion = false;
                detalle.FechaHoraAplicada = null;
                detalle.UsuarioAplicacionId = null;
                detalle.MotivoDevolucion = motivoDevolucion.Trim();

                if (detalle.ProductoId != null)
                {
                    int productoId = detalle.ProductoId.Value;
                    int unidadVenta = detalle.UnidadMedidaVentaId ?? 1;
                    const int bodegaHospId = (int)AmbienteEnum.Hospital;

                    var inventario =
                        _db.ProductosInventario.FirstOrDefault(pi =>
                            pi.ProductoId == productoId &&
                            pi.UnidadMedidaVentaId == unidadVenta &&
                            pi.BodegaId == bodegaHospId)
                        ?? _db.ProductosInventario.FirstOrDefault(pi =>
                            pi.ProductoId == productoId &&
                            pi.BodegaId == bodegaHospId)
                        ?? _db.ProductosInventario.FirstOrDefault(pi =>
                            pi.ProductoId == productoId &&
                            pi.UnidadMedidaVentaId == unidadVenta)
                        ?? _db.ProductosInventario.FirstOrDefault(pi =>
                            pi.ProductoId == productoId);

                    if (inventario != null)
                        inventario.Stock += 1; // cada detalle representa 1 unidad

                }

                _db.SaveChanges();
                transaction.Commit();

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al devolver el elemento. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string AgregarProductoAPaqueteHospitalizacion(
        int hospitalizacionPaqueteId,
        int productoId,
        int unidadMedidaVentaId,
        int cantidad,
        decimal precioProducto,
        string indicaciones = null)
        {
            try
            {
                if (cantidad <= 0)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "La cantidad debe ser mayor a 0." });

                var paqueteCabecera = _db.HospitalizacionesPaquetesHospitalizacion
                    .FirstOrDefault(p => p.Id == hospitalizacionPaqueteId && !p.Eliminado);

                if (paqueteCabecera == null)
                {
                    // Log opcional
                    Console.WriteLine($"Paquete ID {hospitalizacionPaqueteId} no encontrado en HospitalizacionesPaquetesHospitalizacion");
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Paquete no encontrado o eliminado." });
                }

                // Agregar una fila por cada unidad
                for (int i = 0; i < cantidad; i++)
                {
                    var nuevoDetalle = new HospitalizacionDetallePaqueteHospitalizacion
                    {
                        HospitalizacionPaqueteHospitalizacionId = paqueteCabecera.Id,
                        FechaHora = DateTime.Now,
                        Aplicacion = false,
                        Eliminado = false,
                        ProductoId = productoId,
                        ServicioId = null,
                        LaboratorioId = null,
                        LaboratorioPrecioId = null,
                        UnidadMedidaVentaId = unidadMedidaVentaId,
                        PrecioProducto = precioProducto,
                        UsuarioAplicacionId = null,
                        FechaHoraAplicada = null,
                        MotivoDevolucion = null
                    };
                    _db.HospitalizacionDetallePaqueteHospitalizacion.Add(nuevoDetalle);
                }

                _db.SaveChanges();

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = $"{cantidad} unidad(es) agregada(s) al paquete."
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al agregar el producto al paquete. " + ex.Message
                });
            }
        }


        [HttpPost]
        public string ObtenerTarifasPorHabitacion([FromBody] int habitacionId)
        {
            try
            {
                if (habitacionId <= 0)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "ID de habitación inválido." });

                var tarifas = _habitacionRepository.GetTarifasHabitacion(habitacionId);
                var resultado = tarifas.Select(t => new { t.Id, t.NombreTarifa, t.ValorTarifa });
                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = resultado });
            }
            catch (Exception ex)
            {
                // Log the full exception
                Console.WriteLine(ex.ToString());
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
            }
        }

        [HttpPost]
        public string ActualizarTarifaHospitalizacion([FromBody] ActualizarTarifaRequest request)
        {
            try
            {
                var hospitalizacion = _hospitalizacionRepository.Get(request.hospitalizacionId, false, false, false);
                if (hospitalizacion == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Hospitalización no encontrada" });

                var nuevaTarifa = _habitacionRepository.GetTarifaById(request.nuevaTarifaId);
                if (nuevaTarifa == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Tarifa no válida" });

                decimal tarifaAnteriorValor = hospitalizacion.CategoriaHabitacionTarifa?.ValorTarifa ?? 0;
                decimal nuevaTarifaValor = nuevaTarifa.ValorTarifa;

                hospitalizacion.CategoriaHabitacionTarifaId = nuevaTarifa.Id;

                // Recalcular el valor de la cuenta por cobrar asociada
                var detalleCuenta = _hospitalizacionRepository.GetDetalleCuenta(request.hospitalizacionId);
                if (detalleCuenta?.CuentaPorCobrar != null)
                {
                    var noches = (hospitalizacion.FechaFin - hospitalizacion.FechaInicio).Days;
                    if (noches == 0) noches = 1;
                    decimal diferencia = (nuevaTarifaValor - tarifaAnteriorValor) * noches;
                    detalleCuenta.CuentaPorCobrar.Valor = (detalleCuenta.CuentaPorCobrar.Valor ?? 0) + diferencia;
                    _cuentasPorCobrarRepository.Update(detalleCuenta.CuentaPorCobrar);
                }

                _hospitalizacionRepository.Update(hospitalizacion);
                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
            }
        }

        public class ActualizarTarifaRequest
        {
            public int hospitalizacionId { get; set; }
            public int nuevaTarifaId { get; set; }
        }


    }
}