using Database.Shared.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface ISolicitudMedicamentoNacionalService
    {
        void AddSolicitudMedicamento(SolicitudMedicamentoNacional solicitud);
        SolicitudMedicamentoNacional GetSolicitudMedicamentoById(int id);
        List<SolicitudMedicamentoNacional> GetAllSolicitudesMedicamento(int? hospitalizacionId = null);
        List<SolicitudMedicamentoNacional> GetSolicitudesByHospitalizacionId(int hospitalizacionId);
        void UpdateSolicitudMedicamento(SolicitudMedicamentoNacional solicitud);
        void DeleteSolicitudMedicamento(int id);
        List<CategoriaHabitacion> GetAllCategoriasHabitacion();
    }
}
