using Database.Shared.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface ISolicitudMedicamentoService
    {
        void AddSolicitudMedicamento(SolicitudMedicamento solicitud);
        SolicitudMedicamento GetSolicitudMedicamentoById(int id);
        List<SolicitudMedicamento> GetAllSolicitudesMedicamento(int? hospitalizacionId = null);
        List<SolicitudMedicamento> GetSolicitudesByHospitalizacionId(int hospitalizacionId);
        void UpdateSolicitudMedicamento(SolicitudMedicamento solicitud);
        void DeleteSolicitudMedicamento(int id);
        List<CategoriaHabitacion> GetAllCategoriasHabitacion();
    }
}
