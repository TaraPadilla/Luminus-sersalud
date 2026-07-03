using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.IRepository
{
    public interface IHospitalizacionProductoObservacion
    {
        // Añadir una nueva observación
        HospitalizacionProductoObservacion Add(HospitalizacionProductoObservacion observacion);

        // Modificar una observación existente
        void Update(HospitalizacionProductoObservacion observacion);

        // Eliminar (lógicamente) una observación
        void Delete(int id);

        // Obtener observaciones por el ID de HospitalizacionProductoAplicacion
        List<HospitalizacionProductoObservacion> GetByHospitalizacionProductoAplicacionId(int hospitalizacionProductoAplicacionId);
    }
}
