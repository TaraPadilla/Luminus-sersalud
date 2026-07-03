using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.IRepository
{
    public interface IConsentimientoHospi
    {
        // Método para agregar un nuevo registro de ConsentimientoHospi
        void AddConsentimiento(ConsentimientoHospi consentimiento);

        // Método para obtener un consentimiento por ID de paciente y habitación
        ConsentimientoHospi GetConsentimientoByPacienteAndHabitacion(int pacienteId, int habitacionId);

        // Método para obtener un consentimiento por ID de paciente, habitación y hospitalización
        ConsentimientoHospi GetConsentimientoByPacienteHabitacionAndHospitalizacion(int pacienteId, int habitacionId, string hospitalizacionId);

        // Método para actualizar el HospitalizacionId utilizando PacienteId y HabitacionId
        bool UpdateHospitalizacionId(int pacienteId, int habitacionId, string newHospitalizacionId);

        void UpdateConsentimiento(ConsentimientoHospi consentimiento);

    }
}
