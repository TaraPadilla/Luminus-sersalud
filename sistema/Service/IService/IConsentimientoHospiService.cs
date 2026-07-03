using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface IConsentimientoHospiService
    {
        // Método para agregar un nuevo consentimiento
        void AddConsentimiento(ConsentimientoHospi consentimiento);

        // Método para obtener un consentimiento por el ID del paciente y el ID de la habitación
        ConsentimientoHospiVM GetConsentimientoByPacienteAndHabitacion(int pacienteId, int habitacionId);

        // Método para obtener un consentimiento por el ID del paciente, el ID de la habitación y el ID de la hospitalización
        ConsentimientoHospiVM GetConsentimientoByPacienteHabitacionAndHospitalizacion(int pacienteId, int habitacionId, string hospitalizacionId);

        ConsentimientoHospiVM GetConsentimientoByPacienteAndHospitalizacion(int pacienteId, string hospitalizacionId);

        ConsentimientoHospiVM GetLatestConsentimientoByPaciente(int pacienteId);

        // Método para actualizar el HospitalizacionId utilizando PacienteId y HabitacionId
        bool UpdateHospitalizacionId(int pacienteId, int habitacionId, string newHospitalizacionId);

        void UpdateConsentimiento(ConsentimientoHospi consentimiento);

        void UpdateFirmas(int pacienteId, int habitacionId, string urlFirmaPaciente, string urlFirmaResponsable);

        void UpsertConsentimiento(ConsentimientoHospi consentimiento);

    }
}
