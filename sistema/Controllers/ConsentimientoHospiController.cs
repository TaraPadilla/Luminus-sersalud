using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Helpers;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace farmamest.Controllers
{
    public class ConsentimientoHospiController : Controller
    {
        private readonly IConsentimientoHospiService _consentimientoHospiService;
        private readonly IPacientes _pacientesRepository;
        private readonly IHabitacion _habitacionRepository;

        public ConsentimientoHospiController(
            IConsentimientoHospiService consentimientoHospiService,
            IPacientes pacientesRepository,
            IHabitacion habitacionRepository)
        {
            _consentimientoHospiService = consentimientoHospiService;
            _pacientesRepository = pacientesRepository;
            _habitacionRepository = habitacionRepository;
        }

        private static readonly JsonSerializerOptions _jsonInputOpts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        [HttpPost]
        public async Task<IActionResult> Nuevo(ConsentimientoHospi consentimiento)
        {
            try
            {
                consentimiento ??= new ConsentimientoHospi();

                if (consentimiento.PacienteId <= 0 || consentimiento.HabitacionId <= 0)
                {
                    var desdeJson = await HttpRequestJsonHelper.LeerCuerpoJsonAsync<ConsentimientoHospi>(
                        Request, jsonOptions: _jsonInputOpts);
                    if (desdeJson != null && desdeJson.PacienteId > 0 && desdeJson.HabitacionId > 0)
                        consentimiento = desdeJson;
                }

                if (consentimiento.PacienteId <= 0 || consentimiento.HabitacionId <= 0)
                {
                    return Json(new
                    {
                        exitoso = false,
                        mensaje = "Paciente y habitación son obligatorios. Verifique que el formulario se abrió desde el asistente de hospitalización."
                    });
                }

                var paciente = _pacientesRepository.Get(consentimiento.PacienteId, includeRelatedEntities: false);
                if (paciente == null)
                {
                    return Json(new
                    {
                        exitoso = false,
                        mensaje = $"El paciente con ID {consentimiento.PacienteId} no existe en la base de datos. Cierre el formulario y vuelva a abrirlo desde el paso 5 del asistente."
                    });
                }

                var habitacion = _habitacionRepository.Get(consentimiento.HabitacionId);
                if (habitacion == null)
                {
                    return Json(new
                    {
                        exitoso = false,
                        mensaje = $"La habitación con ID {consentimiento.HabitacionId} no existe."
                    });
                }

                if (string.IsNullOrWhiteSpace(consentimiento.HospitalizacionId))
                    consentimiento.HospitalizacionId = "No se especifica";

                if (string.IsNullOrWhiteSpace(consentimiento.NumeroPaciente))
                    consentimiento.NumeroPaciente = consentimiento.PacienteId.ToString();

                _consentimientoHospiService.UpsertConsentimiento(consentimiento);
                return Json(new { exitoso = true, mensaje = "Consentimiento de hospitalización guardado con éxito" });
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException?.Message ?? ex.Message;
                return Json(new { exitoso = false, mensaje = "Error al guardar el consentimiento: " + detalle });
            }
        }

        [HttpGet]
        public string ObtenerPorPacienteYHabitacion(int pacienteId, int habitacionId)
        {
            try
            {
                var consentimiento = _consentimientoHospiService
                    .GetConsentimientoByPacienteAndHabitacion(pacienteId, habitacionId);

                if (consentimiento != null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = true,
                        resultado = consentimiento
                    });
                }
                else
                {
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        mensaje = "Consentimiento no encontrado para este paciente y habitación"
                    });
                }
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    mensaje = "Error al obtener el consentimiento"
                });
            }
        }

        [HttpGet]
        public string ObtenerPorPacienteHabitacionYHospitalizacion(int pacienteId, int habitacionId, string hospitalizacionId)
        {
            try
            {
                var consentimiento = _consentimientoHospiService
                    .GetConsentimientoByPacienteHabitacionAndHospitalizacion(pacienteId, habitacionId, hospitalizacionId);

                if (consentimiento != null)
                    return JsonSerializer.Serialize(new { exitoso = true, resultado = consentimiento });
                else
                    return JsonSerializer.Serialize(new { exitoso = false, mensaje = "Consentimiento no encontrado" });
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new { exitoso = false, mensaje = "Error al obtener el consentimiento" });
            }
        }

        [HttpPost]
        public string ActualizarHospitalizacionId(ConsentimientoHospi consentimiento)
        {
            try
            {
                Console.WriteLine($"ActualizarHospitalizacionId recibido: pacienteId = {consentimiento.PacienteId}, habitacionId = {consentimiento.HabitacionId}, newHospitalizacionId = {consentimiento.HospitalizacionId}");

                bool resultado = _consentimientoHospiService
                    .UpdateHospitalizacionId(consentimiento.PacienteId, consentimiento.HabitacionId, consentimiento.HospitalizacionId);

                if (resultado)
                    return JsonSerializer.Serialize(new { exitoso = true, mensaje = "Hospitalización actualizada con éxito" });
                else
                    return JsonSerializer.Serialize(new { exitoso = false, mensaje = "No se pudo actualizar la hospitalización" });
            }
            catch (Exception)
            {
                return JsonSerializer.Serialize(new { exitoso = false, mensaje = "Error al actualizar la hospitalización" });
            }
        }


        [HttpPost]
        public string ActualizarFirmas(int pacienteId, int habitacionId,
                                       string urlFirmaPaciente = null,
                                       string urlFirmaResponsable = null)
        {
            try
            {
                if (pacienteId <= 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        mensaje = "Debe seleccionar un paciente válido antes de guardar la firma (paso 3)."
                    });
                }

                if (habitacionId <= 0)
                {
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        mensaje = "La habitación no es válida."
                    });
                }

                var paciente = _pacientesRepository.Get(pacienteId, includeRelatedEntities: false);
                if (paciente == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        mensaje = $"El paciente con ID {pacienteId} no existe. Vuelva al paso 3 y seleccione un paciente de la lista."
                    });
                }

                var habitacion = _habitacionRepository.Get(habitacionId);
                if (habitacion == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        mensaje = $"La habitación con ID {habitacionId} no existe."
                    });
                }

                var vm = _consentimientoHospiService
                    .GetConsentimientoByPacienteAndHabitacion(pacienteId, habitacionId);

                if (vm == null)
                {
                    var nuevo = new ConsentimientoHospi
                    {
                        PacienteId = pacienteId,
                        HabitacionId = habitacionId,
                        HospitalizacionId = "No se especifica",
                        NombreCompleto = paciente.Nombre ?? "",
                        NumeroPaciente = paciente.Id.ToString(),
                        NumeroHabitacion = habitacion.NombreNumeroHabitacion ?? "",
                        HoraIngreso = DateTime.Now.ToString("g"),
                        DPI = paciente.Dpi ?? "",
                        FechaNacimiento = paciente.FechaNacimiento?.ToString("dd/MM/yyyy") ?? "",
                        Edad = paciente.Edad?.ToString() ?? "",
                        Direccion = paciente.Direccion ?? "",
                        Celular = paciente.Celular ?? paciente.Telefono ?? "",
                        Email = paciente.Email ?? "",
                        URLFirmaPaciente = urlFirmaPaciente ?? "",
                        URLFirmaResponsable = urlFirmaResponsable ?? "",
                    };
                    _consentimientoHospiService.AddConsentimiento(nuevo);
                    return JsonSerializer.Serialize(new { exitoso = true, mensaje = "Consentimiento creado y firmas actualizadas" });
                }

                _consentimientoHospiService.UpdateFirmas(
                    pacienteId,
                    habitacionId,
                    urlFirmaPaciente,
                    urlFirmaResponsable);

                return JsonSerializer.Serialize(new { exitoso = true, mensaje = "Firmas actualizadas correctamente" });
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException?.Message ?? ex.Message;
                return JsonSerializer.Serialize(new { exitoso = false, mensaje = "Error al actualizar las firmas: " + detalle });
            }
        }

    }
}