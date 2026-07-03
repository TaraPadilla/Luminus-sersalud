using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.IRepository
{
    public interface IDevolucionMedicamentoRepository
    {
        // Agrega una nueva devolución de medicamento.
        void AddDevolucionMedicamento(DevolucionMedicamento devolucion);

        // Obtiene una devolución de medicamento por su ID.
        DevolucionMedicamento GetDevolucionMedicamentoById(int id);

        // Obtiene todas las devoluciones de medicamento, con la opción de filtrar por hospitalización.
        List<DevolucionMedicamento> GetAllDevolucionesMedicamento(int? hospitalizacionId = null);

        // Obtiene las devoluciones de medicamento asociadas a una hospitalización específica.
        List<DevolucionMedicamento> GetDevolucionesByHospitalizacionId(int hospitalizacionId);

        // Actualiza la información de una devolución de medicamento.
        void UpdateDevolucionMedicamento(DevolucionMedicamento devolucion);

        // Elimina una devolución de medicamento por su ID.
        void DeleteDevolucionMedicamento(int id);
    }
}
