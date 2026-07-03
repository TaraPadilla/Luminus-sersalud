using Database.Shared.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface IDevolucionMedicamentoService
    {
        void AddDevolucionMedicamento(DevolucionMedicamento devolucion);
        DevolucionMedicamento GetDevolucionMedicamentoById(int id);
        List<DevolucionMedicamento> GetAllDevolucionesMedicamento(int? hospitalizacionId = null);
        List<DevolucionMedicamento> GetDevolucionesByHospitalizacionId(int hospitalizacionId);
        void UpdateDevolucionMedicamento(DevolucionMedicamento devolucion);
        void DeleteDevolucionMedicamento(int id);
    }
}
