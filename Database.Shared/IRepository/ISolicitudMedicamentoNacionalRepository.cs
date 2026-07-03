using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.IRepository
{
    public interface ISolicitudMedicamentoNacionalRepository
    {
        void AddSolicitudMedicamento(SolicitudMedicamentoNacional solicitud);
        SolicitudMedicamentoNacional GetSolicitudMedicamentoById(int id);
        List<SolicitudMedicamentoNacional> GetAllSolicitudesMedicamento(int? hospitalizacionId = null);
        List<SolicitudMedicamentoNacional> GetSolicitudesByHospitalizacionId(int hospitalizacionId);
        void UpdateSolicitudMedicamento(SolicitudMedicamentoNacional solicitud);
        void DeleteSolicitudMedicamento(int id);
        List<CategoriaHabitacion> GetAllCategoriasHabitacion();

        public List<SolicitudMedicamentoNacional> GetList();

    }
}
