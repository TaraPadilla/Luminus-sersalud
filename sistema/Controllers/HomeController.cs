using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using farmamest.Models;
using Database.Shared.IRepository;
using Database.Shared.Data;
using Database.Shared.Models;
using Wkhtmltopdf.NetCore;
using Microsoft.AspNetCore.Authorization;
using Database.Shared.Enumeraciones;
using sistema.Models;
using System.Text.Json;
using DocumentFormat.OpenXml.Bibliography;
using sistema.Service.IService;

namespace sistema.Controllers

{

    [Authorize]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICitas _citasRepository = null;
        private readonly IPacientes _pacientesRepository = null;
        private readonly IConsultas _consultasRepository = null;
        private readonly IHabitacion _habitacionRepository = null;
        private readonly IPersonas _personasRepository = null;

        //Servicio (logica de negocio)
        private readonly IHomeService _homeService = null;


        public HomeController(
            ILogger<HomeController> logger,
            IPacientes pacientesRepository,
            IHabitacion habitacionRepository,
            IConsultas consultasRepository,
            ICitas citasRepository,
            IPersonas personasRepository,
            //Servicio (logica de negocio)
            IHomeService homeService
            )
        {
            _logger = logger;
            _citasRepository = citasRepository;
            _pacientesRepository = pacientesRepository;
            _habitacionRepository = habitacionRepository;
            _consultasRepository = consultasRepository;
            _personasRepository = personasRepository;
            //Servicio (logica de negocio)
            _homeService = homeService;
        }
        public IActionResult Index()
        {
            //var citas = _citasRepository.CitasListado();
            //var citasConsultadas = new HomeCitasViewModel()
            //{
            //    citas = citas
            //};
            //return View(citasConsultadas);
            // return await _generatePdf.GetPdf("Views/Home/Index.cshtml", "Hello World");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public JsonResult ConsultarCitas()
        {
            try
            {

                var citas = _citasRepository.CitasListado();

                var citasConsultadas = new List<HomeCitasViewModel>();

                if (citas != null && citas.Count > 0)
                {
                    //foreach (var cita in citas)
                    //{
                    //    citasConsultadas.Add(new HomeCitasViewModel
                    //    {
                    //        FechaInicio = ((DateTime)cita.FechaInicio).ToString("MM/dd/yyyy"),
                    //        Hora = ((DateTime)cita.FechaInicio).ToString("HH:mm"),
                    //        Sucursal = cita.Sucursal.NombreSucursal,
                    //        Empleado = cita.Empleado.NombreYApellidos,
                    //        Paciente = cita.Paciente.Nombre,
                    //        NumeroTurno = cita.NumeroTurno,
                    //        Servicios = (List<Servicio>)cita.CitasServicios


                    //    });


                    //}
                }


                return Json(new { Exitoso = true, Resultado = citasConsultadas });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al consultar citas" + ex.Message });
            }
        }

        //COnsultar todas las vacunas del pacientes que son ara mañana
        [HttpPost]
        public string ConsultarCitasProximas()
        {
            try
            {
                var citasProximas = new List<CitaViewModel>();
                var citasdiasdespues = _citasRepository.GetListCitasProximas();


                if (citasdiasdespues != null)
                {
                    foreach (var citadiasdespues in citasdiasdespues)
                    {

                        citasProximas.Add(new CitaViewModel
                        {
                            FechaInicio = ((DateTime)citadiasdespues.FechaInicio).ToString("MM/dd/yyyy"),
                            Hora = ((DateTime)citadiasdespues.FechaInicio).ToString("HH:mm"),
                            EspecialidadText = citadiasdespues.EspecialidadText,
                            EmpleadoText = citadiasdespues.EmpleadoText,
                            servicios = citadiasdespues.CitasServicios.ToList(),
                            PacienteNombre = citadiasdespues.Paciente != null ? citadiasdespues.Paciente.Nombre : "-",
                            Telefono = citadiasdespues.Paciente != null ? citadiasdespues.Paciente.Telefono : "-",
                            EstadoCita = citadiasdespues.EstadoCita,
                            PersonText = citadiasdespues.PersonText,
                            EsMenorDeEdadText = citadiasdespues.EsMenorDeEdadText,
                            NumeroTurno = citadiasdespues.NumeroTurno
                        });
                    }
                    ;
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = citasProximas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar citasCaleNdario. " + ex.Message
                });
            }
        }

      [HttpPost]
public string ConsultarAlertaVacunas(int idPaciente)
{
    try
    {
        var alertasVacunas = new List<VacunaPacienteViewModel>();
        var vacunasPacientes = _pacientesRepository.GetVacunasPaciente(idPaciente);

        if (vacunasPacientes != null)
        {
            foreach (var vacunaPaciente in vacunasPacientes)
            {
                alertasVacunas.Add(new VacunaPacienteViewModel
                {
                    Id = vacunaPaciente.Id,
                    NombreVacuna = vacunaPaciente.Vacuna.Nombre,
                    NombrePaciente = vacunaPaciente.Paciente.Nombre,
                    Telefono = vacunaPaciente.Paciente.Telefono,
                    Email = vacunaPaciente.Paciente.Email,
                    Primera = vacunaPaciente.Primera,
                    Segunda = vacunaPaciente.Segunda,
                    Tercera = vacunaPaciente.Tercera,
                    PrimerRefuerzo = vacunaPaciente.PrimerRefuerzo,
                    SegundoRefuerzo = vacunaPaciente.SegundoRefuerzo
                });
            }
        }
        return JsonSerializer.Serialize(new
        {
            Exitoso = true,
            Resultado = alertasVacunas
        });
    }
    catch (Exception ex)
    {
        return JsonSerializer.Serialize(new
        {
            Exitoso = false,
            Mensaje = "Error al consultar habitaciones ocupadas. " + ex.Message
        });
    }
}
        [HttpPost]
        public JsonResult ConsultarPacientesCumpleannios()
        {
            try
            {
                var pacientes = _pacientesRepository.GetList().Where(p => p.FechaNacimiento != null
                    && Convert.ToDateTime(p.FechaNacimiento).Month >= DateTime.Today.Month).ToList();

                var pacientesCumpleannios = new List<Paciente>();

                if (pacientes != null && pacientes.Count > 0)
                {
                    foreach (var paciente in pacientes)
                    {
                        if (Convert.ToDateTime(paciente.FechaNacimiento).Month >
                            DateTime.Today.Month)
                        {
                            pacientesCumpleannios.Add(paciente);
                        }
                        else
                        {
                            if (Convert.ToDateTime(paciente.FechaNacimiento).Day >=
                                DateTime.Today.Day)
                            {
                                pacientesCumpleannios.Add(paciente);
                            }
                        }
                    }
                }

                return Json(new { Exitoso = true, Resultado = pacientesCumpleannios });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al consultar cumpleaños de pacientes" + ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ConsultarPacientesAplicablesMembresia()
        {
            try
            {
                var consultas = _consultasRepository.ListaConsultas()
                    .Where(c => c.FaseTratamientoId == (int)FaseTratamientoEnum.Mantenimiento1
                        && (DateTime.Today - c.FechaYHoraInicioConsulta).TotalDays >= 365)
                    .ToList();
                var pacientes = new List<Paciente>();

                foreach (var consulta in consultas)
                {
                    pacientes.Add(consulta.Citas.Paciente);
                }

                var pacientesAplicables = pacientes.Distinct().ToList();

                return Json(new { Exitoso = true, Resultado = pacientesAplicables });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar pacientes aplicables para membresía. "
                    + ex.Message
                });
            }
        }
        [HttpPost]
        public JsonResult ConsultarPacientesRetiradosContactar()
        {
            try
            {
                //var pacientesContactar = new List<HomePacientesContactarViewModel>();
                //var pacientesRetirados = _pacientesRepository.GetList()
                //    .Where(p => p.EstadoPacienteId == (int)EstadoPacienteEnum.Inactivo)
                //    .ToList();
                //foreach (var paciente in pacientesRetirados)
                //{
                //    var historial = _pacientesRepository.GetHistorial(paciente.Id)
                //        .OrderByDescending(h => h.Fecha)
                //        .FirstOrDefault();
                //    if (historial != null
                //        && historial.AccionPacienteId == (int)AccionPacienteEnum.Retiro
                //        && (bool)historial.VolverAContactar)
                //    {
                //        var fechaRetiro = historial.Fecha == null ? "" :
                //                Convert.ToDateTime(historial.Fecha).ToString("yyyy/MM/dd");
                //        var fechaContacto = historial.FechaContacto == null ? "" :
                //                Convert.ToDateTime(historial.FechaContacto).ToString("yyyy/MM/dd");
                //        pacientesContactar.Add(new HomePacientesContactarViewModel
                //        {
                //            PacienteId = paciente.Id,
                //            PacienteNombre = paciente.Nombre,
                //            FechaRetiro = fechaRetiro,
                //            FechaContacto = fechaContacto
                //        });
                //    }
                //}

                var pacientesContactar = _personasRepository.GetPersonas()
                    .Where(x => x.TipificacionComunicacion.NombreTipificacion.ToLower().Trim() == "Recontactado".ToLower().Trim()).ToList();

                return Json(new { Exitoso = true, Resultado = pacientesContactar });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al consultar pacientes por contactar. " + ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ConsultarPacientesAniversario()
        {
            try
            {
                var pacientesAniversario = new List<HomePacientesAniversarioViewModel>();
                var fasesTratamiento = _pacientesRepository.GetFasesTratamientoPacientes()
                    .Where(f => f.FechaInicioFase.Date == DateTime.Today.AddYears(-1).Date);

                foreach (var fase in fasesTratamiento)
                {
                    pacientesAniversario.Add(new HomePacientesAniversarioViewModel
                    {
                        PacienteId = fase.Paciente.Id,
                        PacienteNombre = fase.Paciente.Nombre,
                        FaseNombre = fase.FaseTratamiento.NombreFase,
                        FaseFechaInicio = fase.FechaInicioFase.ToString("yyyy/MM/dd")
                    });
                }
                return Json(new { Exitoso = true, Resultado = pacientesAniversario });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al consultar aniversario de pacientes. " + ex.Message });
            }
        }

        [HttpPost]
        public string ConsultarReconsultas()
        {
            try
            {
                var reconsultas = new List<HomeCitasViewModel>();

                var reconsultasResultado = _citasRepository.GetList().Where(a => a.Reconsulta).ToList();

                if (reconsultasResultado != null)
                {
                    foreach (var reconsulta in reconsultasResultado)
                    {

                        reconsultas.Add(new HomeCitasViewModel
                        {
                            FechaInicio = (reconsulta.FechaInicio != null ? reconsulta.FechaInicio.ToString() : "-"),
                            Paciente = reconsulta.Paciente.Nombre,
                            Empleado = reconsulta.PersonText,
                            Telefono = reconsulta.Paciente.Telefono,
                            Especialidad = reconsulta.EspecialidadText
                        });
                    }
                    ;
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = reconsultas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar reconsultas. " + ex.Message
                });
            }
        }

        #region Metodos TAB Clinica

        [HttpPost]
        public string ConsultarExamenesFinalizados()
        {
            try
            {
                var listaExamenesFinalizados = _homeService.GetExamenesFinalizados();

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaExamenesFinalizados
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar Examenes Finalizados " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarClinicaInventarioStockMinimo()
        {
            try
            {
                var listaStockMinimo = _homeService.GetProductosStockMinimo((int)TipoBodegaEnum.Clinica);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaStockMinimo
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos CLINICA Stock minimo. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarClinicaInventarioProximosVencer()
        {
            try
            {
                var listaProximosVencer = _homeService.GetProductosProximosVencer((int)TipoBodegaEnum.Clinica);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaProximosVencer
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos CLINICA Proximos vencer. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarClinicaInventarioVencidos()
        {
            try
            {
                var listaVencidos = _homeService.GetProductosVencidos((int)TipoBodegaEnum.Clinica);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaVencidos
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos CLINICA Vencidos. " + ex.Message
                });
            }
        }

        #endregion

        #region Metodos TAB Farmacia

        [HttpPost]
        public string ConsultarFarmaciaInventarioStockMinimo()
        {
            try
            {
                var listaStockMinimo = _homeService.GetProductosStockMinimo((int)TipoBodegaEnum.Farmacia);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaStockMinimo
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos FARMACIA Stock minimo. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarFarmaciaInventarioProximosVencer()
        {
            try
            {
                var listaProximosVencer = _homeService.GetProductosProximosVencer((int)TipoBodegaEnum.Farmacia);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaProximosVencer
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos FARMACIA Proximos vencer. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarFarmaciaInventarioVencidos()
        {
            try
            {
                var listaVencidos = _homeService.GetProductosVencidos((int)TipoBodegaEnum.Farmacia);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaVencidos
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos FARMACIA Vencidos. " + ex.Message
                });
            }
        }

        #endregion

        #region Metodos TAB Hospital
        [HttpPost]
        public string ConsultarHabitacionesOcupadas()
        {
            try
            {
                var habitacionesOcupadas = new List<HomeHabitacionesOcupadasViewModel>();
                var habitaciones = _habitacionRepository.GetHabitaciones()
                    .Where(a => a.EstadoHabitacionId == (int)EstadoHabitacionEnum.Ocupada);
                if (habitaciones != null)
                {
                    foreach (var habitacion in habitaciones)
                    {
                        var ocupante = "";
                        var paciente = _habitacionRepository.GetPacienteOcupante(habitacion.Id);
                        ocupante = paciente != null ? paciente.Nombre : "-";

                        var medico = "";
                        var codigoCita = "";

                        var hospitalizacionActualId = _habitacionRepository.GetHospitalizacionActual(habitacion.Id) == null ? 0 : _habitacionRepository.GetHospitalizacionActual(habitacion.Id).Id;
                        var hospitalizacionActual = _habitacionRepository.GetHospitalizacionActual(habitacion.Id);

                        medico = hospitalizacionActual.Consultas?
                            .FirstOrDefault()?.Citas?.Empleado?.NombreYApellidos;

                        codigoCita = hospitalizacionActual.Consultas?
                            .FirstOrDefault()?.Citas.CodigoDeCita;

                        habitacionesOcupadas.Add(new HomeHabitacionesOcupadasViewModel
                        {
                            HospitalizacionId = hospitalizacionActualId,
                            HabitacionId = habitacion.Id,
                            HabitacionNumeroNombre = habitacion.NombreNumeroHabitacion,
                            HabitacionCategoria = habitacion.CategoriaHabitacion.NombreCategoria,
                            Paciente = ocupante,
                            MedicoAsignado = medico,
                            CodigoCita = codigoCita
                        });
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = habitacionesOcupadas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar habitaciones ocupadas. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarHospitalInventarioStockMinimo()
        {
            try
            {
                var listaStockMinimo = _homeService.GetProductosStockMinimo((int)TipoBodegaEnum.Hospital);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaStockMinimo
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos HOSPITAL Stock minimo. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarHospitalInventarioProximosVencer()
        {
            try
            {
                var listaProximosVencer = _homeService.GetProductosProximosVencer((int)TipoBodegaEnum.Hospital);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaProximosVencer
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos HOSPITAL Proximos vencer. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarHospitalInventarioVencidos()
        {
            try
            {
                var listaVencidos = _homeService.GetProductosVencidos((int)TipoBodegaEnum.Hospital);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaVencidos
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos HOSPITAL Vencidos. " + ex.Message
                });
            }
        }

        #endregion

        #region Metodos TAB Laboratorio
        [HttpPost]
        public string ConsultarExamenesSolicitados()
        {
            try
            {
                var listaExamenesSolicitados = _homeService.GetExamenesSolicitados();

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaExamenesSolicitados
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar Examenes solicitados " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarLaboratorioInventarioStockMinimo()
        {
            try
            {
                var listaStockMinimo = _homeService.GetProductosStockMinimo((int)TipoBodegaEnum.Laboratorio);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaStockMinimo
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos LABORATORIO Stock minimo. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarLaboratorioInventarioProximosVencer()
        {
            try
            {
                var listaProximosVencer = _homeService.GetProductosProximosVencer((int)TipoBodegaEnum.Laboratorio);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaProximosVencer
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos LABORATORIO Proximos vencer. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarLaboratorioInventarioVencidos()
        {
            try
            {
                var listaVencidos = _homeService.GetProductosVencidos((int)TipoBodegaEnum.Laboratorio);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaVencidos
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos LABORATORIO Vencidos. " + ex.Message
                });
            }
        }

        #endregion

        #region Metodos TAB Compras

        [HttpPost]
        public string ConsultarCuentasPorPagar()
        {
            try
            {
                var listaCuentasPorPagar = _homeService.GetCuentasPorPagar();

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaCuentasPorPagar
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar Cuentas por pagar. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarComprasStockMinimo()
        {
            try
            {
                var listaComprasStockMinimo = _homeService.GetComprasStockMinimo();

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = listaComprasStockMinimo
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos con Stock minimo. " + ex.Message
                });
            }
        }

        #endregion
   

    }
}