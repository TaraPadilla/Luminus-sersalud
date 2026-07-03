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
using sistema.Helpers;
using sistema.Models;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Database.Shared.Enumeraciones;
using farmamest.Models;
using sistema.Service.IService;
using Microsoft.AspNetCore.Mvc.Rendering;
using farmamest.Service.IService;
using farmamest.Service;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace sistema.Controllers
{
    [Authorize]
    public class IntensivosController : Controller
    {
        private readonly IPacientes _pacientesRepository = null;
        private readonly IServicio _serviciosRepository = null;
        private readonly IEmergencias _emergenciasRepo = null;
        private readonly IConsultas _consultasRepository = null;
        private readonly ICuentasPorCobrar _cuentasPorCobrarRepository = null;
        private readonly IProducto _productoRepository = null;
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

        //Servicio (logica de negocio)
        private readonly IPacientesService _pacientesService = null;
        private readonly IProductosService _productosService = null;
        private readonly IHospitalizacionService _hospitalizacionService = null;
        private readonly IEmergenciaService _emergenciaService = null;
        private readonly IFilesService _filesService = null;



        CultureInfo culture = new CultureInfo("es-GT");

        public IntensivosController(
            IPacientes pacientesRepository,
            IServicio serviciosRepository,
            IEmergencias emergenciasRepo,
            IProducto productoRepository,
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
            IEmergenciaService emergenciaService
            )
        {
            _emergenciasRepo = emergenciasRepo;
            _pacientesRepository = pacientesRepository;
            _hospitalizacionRepository = hospitalizacionRepository;
            _cuentasPorCobrarRepository = cuentasPorCobrarRepository;
            _consultasRepository = consultasRepository;
            _serviciosRepository = serviciosRepository;
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
            //Servicio (logica de negocio)
            _pacientesService = pacientesService;
            _productosService = productosService;
            _hospitalizacionService = hospitalizacionService;
            _emergenciaService = emergenciaService;
            _filesService = filesService;
        }

        // public IActionResult Habitaciones()
        // {
        //     var habitacionesConsultadas = new List<HospitalizacionHabitacionViewModel>();
        //     var habitaciones = _habitacionRepository.GetHabitaciones();
        //     if (habitaciones != null)
        //     {
        //         foreach (var habitacion in habitaciones)
        //         {
        //             var ocupante = "-";
        //             if (habitacion.EstadoHabitacionId == (int)EstadoHabitacionEnum.Ocupada)
        //             {
        //                 var paciente = _habitacionRepository.GetPacienteOcupante(habitacion.Id);
        //                 ocupante = paciente != null ? paciente.Nombre : "-";
        //             }
        //             int? hospitalizacionId = null;
        //             var hospitalizacionActualId = _habitacionRepository.GetHospitalizacionActual(habitacion.Id) == null ? 0 : _habitacionRepository.GetHospitalizacionActual(habitacion.Id).Id;

        //             if (habitacion.EstadoHabitacionId == (int)EstadoHabitacionEnum.Ocupada)
        //             {
        //                 hospitalizacionId = hospitalizacionActualId;
        //             }
        //             habitacionesConsultadas.Add(new HospitalizacionHabitacionViewModel
        //             {
        //                 HabitacionId = habitacion.Id,
        //                 HospitalizacionId = hospitalizacionId,
        //                 HabitacionNombre = habitacion.NombreNumeroHabitacion,
        //                 HabitacionCategoria = habitacion.CategoriaHabitacion.NombreCategoria,
        //                 HabitacionEstadoId = habitacion.EstadoHabitacionId,
        //                 HabitacionEstado = habitacion.EstadoHabitacion.NombreEstado,
        //                 HabitacionOcupante = ocupante,
        //                 HabitacionNumeroCamas = habitacion.NumeroCamas,
        //                 HabitacionCapacidadPersonas = habitacion.CapacidadPersonas
        //             });
        //         }
        //     }
        //     return View(habitacionesConsultadas);

        // }
        public IActionResult HabitacionesIntensivos()
        {
            var habitacionesConsultadas = new List<HospitalizacionHabitacionViewModel>();
            var habitaciones = _habitacionRepository.GetHabitaciones();
            if (habitaciones != null)
            {
                foreach (var habitacion in habitaciones)
                {
                    habitacionesConsultadas.Add(HospitalizacionHabitacionHelper.CrearViewModel(habitacion, _habitacionRepository));
                }
            }
            // return View("~/Views/Operaciones/HabitacionesOperaciones.cshtml");
            return View(habitacionesConsultadas);
        }
        public IActionResult Lista()
        {
            return View();
        }
        [HttpPost]
        public string ConsultarListaHospitalizaciones()
        {
            try
            {
                var resultado = _hospitalizacionService.GetListaHospitalizaciones();
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
        public IActionResult Hospitalizar(int? habitacionId, int? consultaId = null, int? emergenciaId = null)
        {
            //Revisar aqui la especialidad
            if (habitacionId == null)
            {
                TempData["Message"] = "Error de servidor";
                return RedirectToAction("HabitacionesIntensivos");
            }
            var habitacion = _habitacionRepository.Get((int)habitacionId);
            var model = new HospitalizacionHospitalizarViewModel
            {
                HabitacionId = (int)habitacionId,
                HabitacionNumeroNombre = habitacion.NombreNumeroHabitacion,
                HabitacionCategoriaId = habitacion.CategoriaHabitacionId,
                HabitacionCategoria = habitacion.CategoriaHabitacion.NombreCategoria,
                HabitacionEstadoId = habitacion.EstadoHabitacionId,
                HabitacionEstado = habitacion.EstadoHabitacion.NombreEstado
            };



            //Precarga de elementos de consulta
            if (consultaId != null)
            {
                var consulta = _consultasRepository.GetConsulta((int)consultaId);
                if (consulta != null)
                {
                    var cita = consulta.Citas ?? new Citas();
                    model.EspecialidadId = cita.EspecialidadId;
                    model.PacienteId = cita.PacienteId;
                    model.ConsultaId = consultaId;
                    model.Consulta = consulta;
                }
            }
            //Precarga de elementos de emergencia
            if (emergenciaId != null)
            {
                var emergenciaBd = _emergenciaService.Get((int)emergenciaId, true);
                if (emergenciaBd != null)
                {
                    model.PacienteId = emergenciaBd.PacienteId;
                    model.EmergenciaId = emergenciaId;
                }
            }

            model.Init(_pacientesRepository);
            return View(model);
        }

        [HttpPost]
        public string Hospitalizar(HospitalizacionHospitalizarViewModel model)
        {
            try
            {
                #region Especialidad

                if (model.EspecialidadId == null)
                {
                    var especialidad = new Especialidad
                    {
                        NombreEspecialidad = model.EspecialidadNombre
                    };
                    especialidad = _especialidadRepository.Add(especialidad);
                    model.EspecialidadId = especialidad.Id;
                }

                #endregion

                #region Paciente

                if (model.PacienteId == null)
                {
                    var paciente = new Paciente
                    {
                        FechaRegistro = DateTime.Today,
                        Nombre = model.PacienteNombre,
                        TipoPacienteId = (int)TipoPacienteEnum.Nuevo,
                        EstadoPacienteId = (int)EstadoPacienteEnum.Activo
                    };
                    paciente = _pacientesRepository.Add(paciente);
                    model.PacienteId = paciente.Id;
                }
                else
                {
                    var paciente = _pacientesRepository.Get((int)model.PacienteId);
                    paciente.Dpi = model.PacienteDpi;
                    paciente.Telefono = model.PacienteTelefono;
                    paciente.FechaNacimiento = model.PacienteFechaNacimiento;
                    _pacientesRepository.Update(paciente);
                }

                #endregion

                var fechas = model.Periodo.Split("-");
                var fechaInicio = Convert.ToDateTime(fechas[0], culture);
                var fechaFin = Convert.ToDateTime(fechas[1], culture);

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
                    EspecialidadId = (int)model.EspecialidadId,
                    UrlArchivoConsentimiento = model.UrlArchivoConsentimiento // Guardamos la URL del consentimiento

                };
                hospitalizacion = _hospitalizacionRepository.Add(hospitalizacion);

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


                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    HospitalizacionId = hospitalizacion.Id
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al registrar hospitalizacion. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarTarifasHabitacion(int habitacionId)
        {
            try
            {
                var listaTarifas = new List<HospitalizacionHospitalizarTarifaViewModel>();
                var tarifasBd = _habitacionRepository.GetTarifasHabitacion(habitacionId);
                if (tarifasBd != null)
                {
                    foreach (var tarifa in tarifasBd)
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
                            tarifa.FechaTarifa.ToString("yyyy/MM/dd")
                            : "-",
                            ValorTarifa = tarifa.ValorTarifa
                        });
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
        public IActionResult Detalles(int? hospitalizacionId)
        {
            var hospitalizacion = _hospitalizacionRepository.Get((int)hospitalizacionId);
            if (hospitalizacion == null)
            {
                TempData["Message"] = "La hospitalizacion no existe";
                TempData["MessageType"] = "error";

                return RedirectToAction("HabitacionesIntensivos");
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



            //Autorizaciones y permisos
            var autorizacionTabEnfermeria = true;
            var autorizacionTabActualizarEstadia = true;
            var autorizacionTabDietas = true;
            var autorizacionTabPagos = true;
            var autorizacionTabSignosVitales = true;
            var autorizacionTabNotaMedica = true;
            var autorizacionTabNotaEnfermeria = true;
            var autorizacionTabControlGlucometria = true;
            var autorizacionTabIncretaExcreta = true;
            if (!User.IsInRole("Administrador")
                && !User.IsInRole("Enfermeria")
                && !User.IsInRole("Medico General")
                && !User.IsInRole("Farmacia")
                && !User.IsInRole("Medico Interno"))
            {
                var registroAutorizacionUsuario = hospitalizacion.HospitalizacionUsuariosAcceso
               .Where(a => a.UserId == usuarioActual)
               .FirstOrDefault();
                if (registroAutorizacionUsuario == null)
                {
                    TempData["Message"] = "El usuario no tiene acceso para ver los detalles de la hospitalizacion";
                    TempData["MessageType"] = "error";

                    return RedirectToAction("HabitacionesIntensivos");
                }

                autorizacionTabEnfermeria = registroAutorizacionUsuario.AutorizacionTabEnfermeria;
                autorizacionTabActualizarEstadia = registroAutorizacionUsuario.AutorizacionTabActualizarEstadia;
                autorizacionTabNotaMedica = registroAutorizacionUsuario.AutorizacionTabNotaMedica;
                autorizacionTabSignosVitales = registroAutorizacionUsuario.AutorizacionTabSignosVitales;
                autorizacionTabNotaEnfermeria = registroAutorizacionUsuario.AutorizacionTabNotaEnfermeria;
                autorizacionTabControlGlucometria = registroAutorizacionUsuario.AutorizacionTabControlGlucometria;
                autorizacionTabIncretaExcreta = registroAutorizacionUsuario.AutorizacionTabIncretaExcreta;
                autorizacionTabPagos = registroAutorizacionUsuario.AutorizacionTabPagos;
                autorizacionTabDietas = registroAutorizacionUsuario.AutorizacionTabDietas;
            }

            // Cargar la consulta relacionada con la hospitalización
            var consulta = _consultasRepository.GetConsultaPorHospitalizacion((int)hospitalizacionId);

            // Si la consulta es nula, inicializamos los submodelos para evitar errores en la vista
            if (consulta != null)
            {
                consulta.ExamenFisico = consulta.ExamenFisico ?? new ExamenFisico();
                consulta.ConsultaRevisionSistemas = consulta.ConsultaRevisionSistemas ?? new ConsultaRevisionSistemas();
                consulta.Historia = consulta.Historia ?? new Historia();
            }

            var model = new HospitalizacionDetallesViewModel
            {
                CuentaId = cuenta.Id,
                HospitalizacionId = (int)hospitalizacionId,
                HabitacionId = hospitalizacion.HabitacionId,
                UrlArchivoConsentimiento = hospitalizacion.UrlArchivoConsentimiento,// Pasar la URL al ViewModel

                //Ajustar aqui el nombre de la especialidad

                //EspecialidadNombre va quemado mientras se termina maquetaci�n
                EspecialidadNombre = hospitalizacion.Especialidad == null ? "-" : hospitalizacion.Especialidad.NombreEspecialidad,
                Consulta = consulta, // Asignar la consulta cargada al ViewModel

                PacienteId = hospitalizacion.PacienteId,
                PacienteNombre = hospitalizacion.Paciente.Nombre,
                PacienteEstadoId = (int)hospitalizacion.Paciente.EstadoPacienteId,
                PacienteEstado = hospitalizacion.Paciente.EstadoPaciente.NombreEstado,
                PacienteTelefono = hospitalizacion.Paciente.Telefono,
                PacienteCelular = hospitalizacion.Paciente.Celular,
                PacienteSexo = hospitalizacion.Paciente.sexoText,
                PacienteTipoSangre = hospitalizacion.Paciente.TipoDeSangre,
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

                //Autorizaciones permisos
                AutorizacionTabEnfermeria = autorizacionTabEnfermeria,
                AutorizacionTabActualizarEstadia = autorizacionTabActualizarEstadia,
                AutorizacionTabDietas = autorizacionTabDietas,
                AutorizacionTabIncretaExcreta = autorizacionTabIncretaExcreta,
                AutorizacionTabControlGlucometria = autorizacionTabControlGlucometria,
                AutorizacionTabNotaEnfermeria = autorizacionTabNotaEnfermeria,
                AutorizacionTabNotaMedica = autorizacionTabNotaMedica,
                AutorizacionTabPagos = autorizacionTabPagos,
                AutorizacionTabSignosVitales = autorizacionTabSignosVitales
            };
            model.Init(_cuentasPorCobrarRepository);
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
                    UsuarioCreaId = _userManager.GetUserId(HttpContext.User)
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
        public string ConsultarPrecioServicio(int servicioId)
        {
            try
            {
                var data = _serviciosRepository.GetPrecioServicioById(servicioId);

                var result = new List<object>();

                result.Add(data.Select(x => new
                {
                    PrecioServicioId = x.Id,
                    NombrePrecio = x.Precio.NombrePrecio,
                    Valor = x.Valor,
                    PrecioMostrar = x.Precio.NombrePrecio + " Q. " + x.Valor.ToString("0.00") // Formatear con dos decimales

                }));

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
                MedicamentoAplicacionHelper.RegistrarProductoConAplicaciones(
                    _hospitalizacionRepository, model, _userManager.GetUserId(HttpContext.User));
                return JsonSerializer.Serialize(new { Exitoso = true });
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
        public string AgregarExamen(HospitalizacionAgregarExamenViewModel model)
        {
            try
            {
                var examen = new Examen
                {
                    EstadoExamenId = (int)EstadoExamenEnum.Solictiado,
                    FechaRealizacion = DateTime.Now,
                    PacienteId = model.PacienteId,
                    UsuarioSolicita = _userManager.GetUserId(HttpContext.User),
                    Eliminado = false
                };
                var examenLabClinico = _laboratorioClinicoRepository
                    .GetExamenLab(model.ExamenLabClinicoId, false);
                examen.DetalleExamenes.Add(new DetalleExamen
                {
                    ExamenLabClinicoId = model.ExamenLabClinicoId,
                    Cantidad = 1,
                    PrecioValor = examenLabClinico.Precio,
                    Descuento = 0,
                    Subtotal = examenLabClinico.Precio,
                    Total = examenLabClinico.Precio
                });

                _hospitalizacionRepository.AddExamen(new HospitalizacionExamen
                {
                    FechaHora = Convert.ToDateTime(DateTime.Now, culture),
                    HospitalizacionId = model.HospitalizacionId,
                    Examen = examen,
                    ExamenLabClinicoPrecioId = model.ExamenLabClinicoPrecioId
                });

                #region Actualizar cuenta por cobrar

                var cuenta = _cuentasPorCobrarRepository.Get(model.CuentaId);
                if (cuenta != null)
                {
                    var valorCuenta = cuenta.Valor ?? 0;
                    valorCuenta += examenLabClinico.Precio;
                    cuenta.Valor = valorCuenta;
                    _cuentasPorCobrarRepository.Update(cuenta);
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
                    Mensaje = "Error al agregar examen. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarPrecioExamen(int examenId)
        {
            try
            {
                var data = _laboratorioClinicoRepository.GetPreciosExamen(examenId);

                var result = data.Select(x => new
                {
                    ExamenLabClinicoId = x.ExamenLabClinicoId,
                    ExamenLabClinicoPrecioId = x.Id,
                    PrecioNombre = x.Precio.NombrePrecio,
                    Valor = x.PrecioValor,
                    PrecioMostrar = x.Precio.NombrePrecio + " Q. " + x.PrecioValor.ToString("0.00") // Formatear con dos decimales
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
                    Mensaje = "Error al agregar servicio. " + ex.Message
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
                    Telefono = x.Telefono
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
                var hospitalizacionExamen = _hospitalizacionRepository
                    .GetHospitalizacionExamen(hospitalizacionExamenId);
                hospitalizacionExamen.Eliminado = true;
                _hospitalizacionRepository.Update(hospitalizacionExamen);
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
                    Mensaje = "Error al eliminar examen. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string AgregarPaquete(HospitalizacionAgregarPaqueteViewModel model)
        {
            try
            {
                var dataDetalle = _detallePaqueteHospitalizacion.GetByIdPaqueteHospitalizacion(model.PaqueteId);
                var listHospDetalleHosp = new List<HospitalizacionDetallePaqueteHospitalizacion>();
                foreach (var detalle in dataDetalle)
                {
                    for (int i = 0; i < detalle.Cantidad; i++)
                    {



                        //Aca se debe agregar los laboratorios
                        if (detalle.LaboratorioId != null && !detalle.Eliminado)
                        {
                            var examen = new Examen
                            {
                                EstadoExamenId = (int)EstadoExamenEnum.Solictiado,
                                FechaRealizacion = DateTime.Now,
                                PacienteId = model.PacienteId,
                                UsuarioSolicita = _userManager.GetUserId(HttpContext.User),
                                Eliminado = false
                            };
                            var examenLabClinico = _laboratorioClinicoRepository
                                .GetExamenLab((int)detalle.LaboratorioId, false);
                            var detalleExamen = new DetalleExamen
                            {
                                ExamenLabClinicoId = (int)detalle.LaboratorioId,
                                Cantidad = 1,
                                PrecioValor = detalle.LaboratorioPrecio.PrecioValor,
                                Descuento = 0,
                                Subtotal = detalle.LaboratorioPrecio.PrecioValor,
                                Total = detalle.LaboratorioPrecio.PrecioValor
                            };
                            examen.DetalleExamenes.Add(detalleExamen);

                            var laboratorioPrecio = detalle.LaboratorioPrecio ?? new ExamenLabClinicoPrecio();
                            _laboratorioClinicoRepository.Add(examen);

                            var productoInventarioPrecio = detalle.ProductoInventarioPrecio ?? new ProductoInventarioPrecio();

                            listHospDetalleHosp.Add(new HospitalizacionDetallePaqueteHospitalizacion
                            {
                                FechaHora = DateTime.Now,
                                Aplicacion = false,
                                UsuarioAplicacionId = null,
                                FechaHoraAplicada = null,
                                ExamenId = examen.Id,
                                Eliminado = false,
                                ServicioId = detalle.ServicioId,
                                PrecioProducto = productoInventarioPrecio.Valor,
                                LaboratorioId = detalle.LaboratorioId,
                                LaboratorioPrecioId = detalle.LaboratorioPrecioId,
                                ProductoId = detalle.ProductoId,
                                UnidadMedidaVentaId = detalle.UnidadMedidaVentaId
                            });
                        }

                        if (detalle.LaboratorioId == null && !detalle.Eliminado)
                        {
                            var productoInventarioPrecio = detalle.ProductoInventarioPrecio ?? new ProductoInventarioPrecio();
                            listHospDetalleHosp.Add(new HospitalizacionDetallePaqueteHospitalizacion
                            {
                                FechaHora = DateTime.Now,
                                Aplicacion = false,
                                UsuarioAplicacionId = null,
                                FechaHoraAplicada = null,
                                Eliminado = false,
                                ServicioId = detalle.ServicioId,
                                PrecioProducto = productoInventarioPrecio.Valor,
                                LaboratorioId = detalle.LaboratorioId,
                                LaboratorioPrecioId = detalle.LaboratorioPrecioId,
                                ProductoId = detalle.ProductoId,
                                UnidadMedidaVentaId = detalle.UnidadMedidaVentaId
                            });
                        }

                    }
                }

                if (dataDetalle != null && dataDetalle.Count > 0)
                {
                    var paquete = dataDetalle.FirstOrDefault()
                        .PaqueteHospitalizacion;

                    #region Actualizar cuenta por cobrar

                    var cuenta = _cuentasPorCobrarRepository.Get(model.CuentaId);
                    if (cuenta != null)
                    {
                        var valorCuenta = cuenta.Valor ?? 0;
                        valorCuenta += paquete.Precio ?? 0;
                        cuenta.Valor = valorCuenta;
                        _cuentasPorCobrarRepository.Update(cuenta);
                    }

                    #endregion
                }

                _hospitalizacionRepository.AddHospitalizacionPaqueteHospitalizacion(new HospitalizacionPaqueteHospitalizacion
                {
                    FechaHora = Convert.ToDateTime(DateTime.Now, culture),
                    HospitalizacionId = model.HospitalizacionId,
                    PaqueteHospitalizacionId = model.PaqueteId,
                    Eliminado = false,
                    HospitalizacionDetallePaqueteHospitalizacion = listHospDetalleHosp
                });

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Paquete agregado exitosamente"
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al agregar paquete. " + ex.Message
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
        public string AgregarExamenFisicoHospitalizacion(HospitalizacionAgregarExamenFisicoViewModel model)
        {
            try
            {
                var fechaHora = DateTime.Now;
                var datos = new List<ExamenFisicoHospDato>();
                if (model.DatosExamen != null)
                {
                    foreach (var dato in model.DatosExamen)
                    {
                        datos.Add(new ExamenFisicoHospDato
                        {
                            DatoExamenFisicoHospId = dato.DatoExamenFisicoHospId,
                            Valor = dato.ValorDato
                        });
                    }
                }
                _hospitalizacionRepository.AddExamenFisicoHosp(new ExamenFisicoHosp
                {
                    FechaHora = fechaHora,
                    HospitalizacionId = model.HospitalizacionId,
                    Observaciones = model.Observaciones,
                    UsuarioToma = _userManager.GetUserId(HttpContext.User),
                    ExamenesFisicosHospDatos = datos
                });
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
                    Mensaje = "Error al agregar examen fisico. " + ex.Message
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

                var habitacionOrigen = _habitacionRepository.Get(hospitalizacion.HabitacionId);
                habitacionOrigen.EstadoHabitacionId = (int)EstadoHabitacionEnum.Disponible;
                _habitacionRepository.Update(habitacionOrigen);

                hospitalizacion.HabitacionId = habitacionId;
                _hospitalizacionRepository.Update(hospitalizacion);


                var habitacionDestino = _habitacionRepository.Get(habitacionId);
                habitacionDestino.EstadoHabitacionId = (int)EstadoHabitacionEnum.Ocupada;
                _habitacionRepository.Update(habitacionDestino);


                TempData["Message"] = "Se ha realizado el cambio de habitacion";

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
                    Mensaje = "Error al cambiar de habitacion. " + ex.Message
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
                        var fechaInicio = new DateTime(
                            registro.FechaInicio.Year,
                            registro.FechaInicio.Month,
                            registro.FechaInicio.Day
                            );
                        var fechaFin = new DateTime(
                            registro.FechaFin.Year,
                            registro.FechaFin.Month,
                            registro.FechaFin.Day
                            );
                        var noches = 1;
                        if (fechaInicio != fechaFin)
                        {
                            noches = (fechaFin - fechaInicio).Days;
                        }
                        var precioHospitalizacion =
                            registro.CategoriaHabitacionTarifa.ValorTarifa * noches;
                        registrosHospitalizacion.Add(new HospitalizacionRegistroHospitalizacionViewModel
                        {
                            Id = registro.Id,
                            FechaInicio = registro.FechaInicio.ToString(),
                            FechaFin = registro.FechaFin.ToString(),
                            NumeroNoches = noches,
                            HabitacionId = registro.HabitacionId,
                            HabitacionNumeroNombre = registro.Habitacion.NombreNumeroHabitacion,
                            HabitacionCategoria = registro.Habitacion.CategoriaHabitacion.NombreCategoria,
                            HabitacionTarifa = registro.CategoriaHabitacionTarifa?.NombreTarifa == null ? "-" : registro.CategoriaHabitacionTarifa?.NombreTarifa,
                            HabitacionValorNoche = registro.CategoriaHabitacionTarifa.ValorTarifa.ToString(),
                            Precio = precioHospitalizacion.ToString(),
                            HabitacionNumeroCamas = registro.Habitacion.NumeroCamas.ToString(),
                            Observaciones = registro.Observaciones
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = registrosHospitalizacion
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar registros de hospitalizacion. " + ex.Message
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
                if (paquetesBd != null)
                {
                    foreach (var paquete in paquetesBd)
                    {
                        var listaPaquetesServicios = new List<HospitalizacionPaqueteDetallePaqueteServicioViewModel>();
                        var listaPaquetesProductos = new List<HospitalizacionPaqueteDetallePaqueteProductoViewModel>();
                        var listaPaquetesLaboratorios = new List<HospitalizacionPaqueteDetallePaqueteLaboratorioViewModel>();

                        foreach (var item in paquete.PaqueteHospitalizacion.DetallePaquetesHospitalizacion)
                        {
                            if (item.LaboratorioId == null && item.ProductoId == null && item.ServicioId != null)
                            {
                                ServicioPrecio servicioPrecio = new ServicioPrecio();
                                if (item.ServicioPrecioId == null)
                                {
                                    servicioPrecio = null;
                                }
                                else
                                {
                                    servicioPrecio = _hospitalizacionRepository.GetServicioPrecioById((int)item.ServicioPrecioId);
                                }

                                listaPaquetesServicios.Add(new HospitalizacionPaqueteDetallePaqueteServicioViewModel
                                {
                                    Id = item.ServicioId,
                                    Nombre = item.Servicio.NombreServicio,
                                    Cantidad = item.Cantidad,
                                    Codigo = item.Servicio.CodigoInterno,
                                    Precio = servicioPrecio == null ? 0 : servicioPrecio.Valor,
                                });
                            }
                            if (item.LaboratorioId == null && item.ProductoId != null && item.ServicioId == null)
                            {
                                listaPaquetesProductos.Add(new HospitalizacionPaqueteDetallePaqueteProductoViewModel
                                {
                                    Id = item.ProductoId,
                                    Nombre = item.Producto.NombreProducto,
                                    Cantidad = item.Cantidad,
                                    Codigo = item.Producto.CodigoReferencia,
                                    Precio = item.ProductoInventarioPrecio == null ? 0 : item.ProductoInventarioPrecio.Valor
                                });
                            }
                            if (item.LaboratorioId != null && item.ProductoId == null && item.ServicioId == null)
                            {
                                listaPaquetesLaboratorios.Add(new HospitalizacionPaqueteDetallePaqueteLaboratorioViewModel
                                {
                                    Id = item.LaboratorioId,
                                    Nombre = item.Laboratorio.NombreExamen,
                                    Cantidad = item.Cantidad,
                                    Codigo = item.Laboratorio.Id.ToString(),
                                    Precio = item.LaboratorioPrecio.PrecioValor
                                });
                            }

                        }
                        listaPaquetes.Add(new HospitalizacionPaqueteAgregadoViewModel
                        {
                            Id = paquete.Id,
                            PaqueteId = paquete.PaqueteHospitalizacionId,
                            Codigo = paquete.PaqueteHospitalizacion.CodigoInterno,
                            Nombre = paquete.PaqueteHospitalizacion.NombrePaquete,
                            FechaHora = paquete.FechaHora.ToString(),
                            Servicios = listaPaquetesServicios,
                            Productos = listaPaquetesProductos,
                            Laboratorios = listaPaquetesLaboratorios,
                            Precio = paquete.PaqueteHospitalizacion.Precio ?? 0
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
        public string ConsultarMedicamentosHospitalizacion(int hospitalizacionId)
        {
            try
            {
                var listaProductos = new List<HospitalizacionProductoViewModel>();
                var productos = _hospitalizacionRepository.GetProductos(hospitalizacionId);
                if (productos != null)
                {
                    foreach (var producto in productos)
                    {
                        var hospitalizacionProductosAplicados
                            = producto.HospitalizacionesProductosAplicaciones
                            .Where(a => a.Aplicado).ToList()
                            ?? new List<HospitalizacionProductoAplicacion>();
                        listaProductos.Add(new HospitalizacionProductoViewModel
                        {
                            Id = producto.Id,
                            Nombre = producto.Producto.NombreProducto,
                            Cantidad = producto.Cantidad,
                            CantidadAplicada = hospitalizacionProductosAplicados.Sum(a => a.Cantidad),
                            Precio = producto.PrecioValor,
                            Indicaciones = producto.Indicaciones,
                            Subtotal =
                            producto.PrecioValor
                            * hospitalizacionProductosAplicados
                            .Where(a => a.Aplicado && !a.Eliminado).Sum(a => a.Cantidad)
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
                    Mensaje = "Error al consultar medicamentos. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarProductosAplicacionHospitalizacion(int hospitalizacionId)
        {
            try
            {
                var listaProductosAplicacion = new List<HospitalizacionProductoAplicacionViewModel>();
                var productosAplicacion = _hospitalizacionRepository
                    .GetProductosAplicacion(hospitalizacionId);
                if (productosAplicacion != null)
                {
                    foreach (var productoAplicacion in productosAplicacion)
                    {

                        if (productoAplicacion.Aplicado)
                        {
                            var persona = "-";
                            if (productoAplicacion.UsuarioAplica != null)
                            {
                                var empleadoId = _userRepository.GetbyId(productoAplicacion.UsuarioAplica)?.EmpleadoId;

                                if (empleadoId != null)
                                {
                                    persona = _empleadoRepository.Get((int)empleadoId).NombreYApellidos;
                                }
                                else
                                {
                                    persona = "Admin";
                                }
                            }
                            var fechaAplicacion = "-";
                            if (productoAplicacion.Aplicado)
                            {
                                fechaAplicacion = productoAplicacion.FechaHoraAplicacion.ToString();
                            }
                            var unidadVenta = productoAplicacion.HospitalizacionProducto
                                .UnidadMedidaVenta ?? new UnidadMedidaVenta();
                            listaProductosAplicacion.Add(new HospitalizacionProductoAplicacionViewModel
                            {
                                Id = productoAplicacion.Id,
                                Nombre = productoAplicacion.HospitalizacionProducto.Producto.NombreProducto,
                                Cantidad = productoAplicacion.Cantidad,
                                UnidadMedidaVentaNombre = unidadVenta.Nombre,
                                Indicaciones = productoAplicacion.HospitalizacionProducto.Indicaciones,
                                Aplicado = productoAplicacion.Aplicado,
                                FechaHoraAplicacion = fechaAplicacion,
                                PersonaAplica = persona,
                                PersonaCrea = _userRepository.GetUserNameOrDefault(productoAplicacion.UsuarioCreaId),
                            });
                        }
                        else if (!productoAplicacion.HospitalizacionProducto.Eliminado && !productoAplicacion.Aplicado)
                        {
                            var persona = "-";
                            if (productoAplicacion.UsuarioAplica != null)
                            {
                                var empleadoId = _userRepository.GetbyId(productoAplicacion.UsuarioAplica)?.EmpleadoId;

                                if (empleadoId != null)
                                {
                                    persona = _empleadoRepository.Get((int)empleadoId).NombreYApellidos;
                                }
                                else
                                {
                                    persona = "Admin";
                                }
                            }
                            var fechaAplicacion = "-";
                            if (productoAplicacion.Aplicado)
                            {
                                fechaAplicacion = productoAplicacion.FechaHoraAplicacion.ToString();
                            }
                            listaProductosAplicacion.Add(new HospitalizacionProductoAplicacionViewModel
                            {
                                Id = productoAplicacion.Id,
                                Nombre = productoAplicacion.HospitalizacionProducto.Producto.NombreProducto,
                                Cantidad = productoAplicacion.Cantidad,
                                Indicaciones = productoAplicacion.HospitalizacionProducto.Indicaciones,
                                Aplicado = productoAplicacion.Aplicado,
                                FechaHoraAplicacion = fechaAplicacion,
                                PersonaAplica = persona,
                                PersonaCrea = _userRepository.GetUserNameOrDefault(productoAplicacion.UsuarioCreaId),
                            });
                        }

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
            try
            {
                var listaExamenes = new List<HospitalizacionExamenViewModel>();
                var examenes = _hospitalizacionRepository.GetExamenes(hospitalizacionId);
                if (examenes != null)
                {
                    foreach (var examen in examenes)
                    {

                        var detalleExamen = examen.Examen?.DetalleExamenes?.FirstOrDefault();
                        var examenNombre = detalleExamen?.ExamenLabClinico?.NombreExamen ?? "-";
                        var precioExamenValor = examen.ExamenLabClinicoPrecio?.PrecioValor ?? 0;

                        listaExamenes.Add(new HospitalizacionExamenViewModel
                        {
                            Id = examen.Id,
                            ExamenId = examen.ExamenId,
                            FechaHora = examen.FechaHora.ToString(),
                            DetalleExamenId = detalleExamen?.Id ?? 0,
                            Nombre = examenNombre,
                            Precio = precioExamenValor,

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
                    Mensaje = "Error al consultar examenes. " + ex.Message
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
                        var datosExamen = "";
                        if (examen.ExamenesFisicosHospDatos != null)
                        {
                            foreach (var dato in examen.ExamenesFisicosHospDatos)
                            {
                                datosExamen += $"<b>{dato.DatoExamenFisicoHosp.NombreDato}: </b>" +
                                    $"{dato.Valor}<br/>";
                            }
                        }
                        listaExamenesFisicos.Add(new HospitalizacionExamenFisicoViewModel
                        {
                            FechaHora = examen.FechaHora.ToString(),
                            Persona = persona,
                            Observaciones = examen.Observaciones,
                            Datos = datosExamen
                        });
                    }
                }
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

                //Se actualiza la cuenta por cobrar
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
                    Exitoso = true
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

                #region Actualizar cuenta por cobrar

                var cuenta = _cuentasPorCobrarRepository.Get(cuentaId);
                if (cuenta != null)
                {
                    var valorCuenta = cuenta.Valor ?? 0;
                    valorCuenta += hospitalizacionServicio.Precio;
                    cuenta.Valor = valorCuenta;
                    _cuentasPorCobrarRepository.Update(cuenta);
                }

                #endregion

                //Pendiente el descuento de productos en inventario relacionados con el servicio

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Mensaje = "Servicio aplicado exitosamente"
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

                if (hospitalizacion.Pagada)
                {
                    hospitalizacion.Finalizada = true;
                    hospitalizacion.FechaHoraFinalizada = fechaHora;
                    _hospitalizacionRepository.Update(hospitalizacion);

                    var habitacion = _habitacionRepository.Get(hospitalizacion.HabitacionId);
                    habitacion.EstadoHabitacionId = (int)EstadoHabitacionEnum.Disponible;
                    _habitacionRepository.Update(habitacion);

                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = true,
                        Pagada = true
                    });
                }
                else
                {
                    var cuenta = _cuentasPorCobrarRepository
                        .GetUltimaCuentaPendientePaciente(hospitalizacion.PacienteId);

                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = true,
                        Pagada = false,
                        CuentaId = cuenta.Id
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al finalizar hospitalizaci�n. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarDetallePaquetesHospitalizacionAplicados(int hospitalizacionId)
        {
            try
            {

                var listaProductosAplicacion = new List<HospitalizacionDetallePaqueteAplicacionViewModel>();
                var productosAplicacion = _hospitalizacionRepository.GetHospitalizacionPaqueteByIdHospitalizacion(hospitalizacionId);


                if (productosAplicacion != null)
                {
                    foreach (var producto in productosAplicacion)
                    {

                        foreach (var productoDetalle in producto.HospitalizacionDetallePaqueteHospitalizacion)
                        {
                            //revisar el tema de la persona como es la relacion
                            var persona = "-";
                            if (productoDetalle.UsuarioAplicacionId != null)
                            {

                                persona = _userRepository.GetUserNameOrDefault(productoDetalle.UsuarioAplicacionId);

                            }
                            var fechaAplicacion = "-";

                            if (productoDetalle.FechaHoraAplicada != null)
                            {
                                fechaAplicacion = productoDetalle.FechaHoraAplicada.ToString();
                            }

                            //Productos
                            if (productoDetalle.ServicioId == null && productoDetalle.LaboratorioId == null)
                            {
                                if (productoDetalle.Aplicacion)
                                {

                                    listaProductosAplicacion.Add(new HospitalizacionDetallePaqueteAplicacionViewModel
                                    {
                                        Tipo = "Producto",
                                        Id = productoDetalle.Id,
                                        Nombre = productoDetalle.Producto.NombreProducto,
                                        Descripcion = productoDetalle.Producto.Descripcion,
                                        Codigo = productoDetalle.Producto.CodigoReferencia,
                                        Aplicado = productoDetalle.Aplicacion,
                                        FechaHoraAplicacion = fechaAplicacion,
                                        Persona = persona,
                                    });
                                }
                                else if (!producto.Eliminado && !productoDetalle.Aplicacion)
                                {

                                    listaProductosAplicacion.Add(new HospitalizacionDetallePaqueteAplicacionViewModel
                                    {
                                        Tipo = "Producto",
                                        Id = productoDetalle.Id,
                                        Nombre = productoDetalle.Producto.NombreProducto,
                                        Descripcion = productoDetalle.Producto.Descripcion,
                                        Codigo = productoDetalle.Producto.CodigoReferencia,
                                        Aplicado = productoDetalle.Aplicacion,
                                        FechaHoraAplicacion = fechaAplicacion,
                                        Persona = persona,
                                    });
                                }
                            }
                            //Laboratorio
                            if (productoDetalle.LaboratorioId != null)
                            {
                                if (!producto.Eliminado)
                                {
                                    var examen = productoDetalle.Examen ?? new Examen();
                                    int? detalleId = null;
                                    if (examen.DetalleExamenes != null && examen.DetalleExamenes.Count() > 0)
                                    {
                                        detalleId = examen.DetalleExamenes.FirstOrDefault().Id;
                                    }
                                    listaProductosAplicacion.Add(new HospitalizacionDetallePaqueteAplicacionViewModel
                                    {
                                        Tipo = "Laboratorio",
                                        Id = productoDetalle.Id,
                                        ExamenId = productoDetalle.ExamenId,
                                        DetalleExamenId = detalleId,
                                        Nombre = productoDetalle.Laboratorio.NombreExamen,
                                        Descripcion = productoDetalle.Laboratorio.TipoDeExamen,
                                        Codigo = productoDetalle.Laboratorio.CodigoInterno,
                                        Aplicado = productoDetalle.Aplicacion,
                                        FechaHoraAplicacion = fechaAplicacion,
                                        Persona = persona,
                                    });
                                }

                            }
                            //Servicio
                            if (productoDetalle.ProductoId == null && productoDetalle.LaboratorioId == null)
                            {
                                if (productoDetalle.Aplicacion)
                                {
                                    listaProductosAplicacion.Add(new HospitalizacionDetallePaqueteAplicacionViewModel
                                    {
                                        Tipo = "Servicio",
                                        Id = productoDetalle.Id,
                                        Nombre = productoDetalle.Servicio.NombreServicio,
                                        Descripcion = productoDetalle.Servicio.Descripcion,
                                        Codigo = productoDetalle.Servicio.CodigoInterno,
                                        Aplicado = productoDetalle.Aplicacion,
                                        FechaHoraAplicacion = fechaAplicacion,
                                        Persona = persona,
                                    });
                                }
                                else if (!producto.Eliminado && !productoDetalle.Aplicacion)
                                {
                                    listaProductosAplicacion.Add(new HospitalizacionDetallePaqueteAplicacionViewModel
                                    {
                                        Tipo = "Servicio",
                                        Id = productoDetalle.Id,
                                        Nombre = productoDetalle.Servicio.NombreServicio,
                                        Descripcion = productoDetalle.Servicio.Descripcion,
                                        Codigo = productoDetalle.Servicio.CodigoInterno,
                                        Aplicado = productoDetalle.Aplicacion,
                                        FechaHoraAplicacion = fechaAplicacion,
                                        Persona = persona,
                                    });
                                }

                            }
                        }


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
                    Mensaje = "Error al consultar los paquetes. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string AplicarDetallePaqueteHospitalizacion(int Id)
        {
            try
            {
                var data = _hospitalizacionDetallePaqueteHospitalizacion.GetById(Id);

                data.Aplicacion = true;
                data.UsuarioAplicacionId = _userManager.GetUserId(HttpContext.User);
                data.FechaHoraAplicada = DateTime.Now;
                _hospitalizacionDetallePaqueteHospitalizacion.Update(data);

                //Se hace el descuento del inventario en caso de que sea un producto
                if (data.ProductoId != null)
                {
                    _productosService.RealizarDescuentoInventario((int)data.ProductoId, data.UnidadMedidaVentaId, 1);
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
                    Mensaje = "Error al aplicar el paquete. " + ex.Message
                });
            }

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



    }
}