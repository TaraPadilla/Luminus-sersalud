using Database.Shared.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;

namespace farmamest.Controllers
{
    public class ConsentimientoHospiController : Controller
    {
        private readonly IConsentimientoHospiService _consentimientoHospiService;

        public ConsentimientoHospiController(IConsentimientoHospiService consentimientoHospiService)
        {
            _consentimientoHospiService = consentimientoHospiService;
        }

        [HttpPost]
        public IActionResult Nuevo(ConsentimientoHospi consentimiento)
        {
            try
            {
                _consentimientoHospiService.AddConsentimiento(consentimiento);
                return Json(new { exitoso = true, mensaje = "Consentimiento de hospitalización agregado con éxito" });
            }
            catch (Exception)
            {
                return Json(new { exitoso = false, mensaje = "Error al agregar el consentimiento de hospitalización" });
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
                var vm = _consentimientoHospiService
                    .GetConsentimientoByPacienteAndHabitacion(pacienteId, habitacionId);

                if (vm == null)
                {
                    var nuevo = new ConsentimientoHospi
                    {
                        PacienteId = pacienteId,
                        HabitacionId = habitacionId,
                        HospitalizacionId = "No se especifica",
                        URLFirmaPaciente = urlFirmaPaciente ?? "",
                        URLFirmaResponsable = urlFirmaResponsable ?? "",
                    };
                    _consentimientoHospiService.AddConsentimiento(nuevo);
                    return JsonSerializer.Serialize(new { exitoso = true, mensaje = "Consentimiento creado y firmas actualizadas" });
                }

                var entidad = new ConsentimientoHospi
                {
                    Id = vm.Id,
                    PacienteId = vm.PacienteId,
                    HabitacionId = vm.HabitacionId,
                    HospitalizacionId = vm.HospitalizacionId,
                    URLFirmaPaciente = !string.IsNullOrEmpty(urlFirmaPaciente)
                                            ? urlFirmaPaciente
                                            : vm.URLFirmaPaciente,
                    URLFirmaResponsable = !string.IsNullOrEmpty(urlFirmaResponsable)
                                            ? urlFirmaResponsable
                                            : vm.URLFirmaResponsable,
                };
                _consentimientoHospiService.UpdateConsentimiento(entidad);

                return JsonSerializer.Serialize(new { exitoso = true, mensaje = "Firmas actualizadas correctamente" });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, mensaje = "Error al actualizar las firmas: " + ex.Message });
            }
        }
    }
}