using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.IRepository
{
    public interface ISolicitudMedicamentoRepository
    {
        void AddSolicitudMedicamento(SolicitudMedicamento solicitud);
        SolicitudMedicamento GetSolicitudMedicamentoById(int id);
        List<SolicitudMedicamento> GetAllSolicitudesMedicamento(int? hospitalizacionId = null);
        List<SolicitudMedicamento> GetSolicitudesByHospitalizacionId(int hospitalizacionId);
        void UpdateSolicitudMedicamento(SolicitudMedicamento solicitud);
        void DeleteSolicitudMedicamento(int id);
        List<CategoriaHabitacion> GetAllCategoriasHabitacion();

        public List<SolicitudMedicamento> GetList();

    }
}
