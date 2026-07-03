using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface IHospitalizacionProductoObservacionService
    {
        // Añadir una nueva observación
        void Add(HospitalizacionProductoObservacion entity);

        // Modificar una observación existente
        void Update(HospitalizacionProductoObservacion entity);

        // Eliminar (lógicamente) una observación
        void Delete(int id);

        // Obtener observaciones por el ID de HospitalizacionProductoAplicacion
        List<HospitalizacionProductoObservacionVM> GetByHospitalizacionProductoAplicacionId(int hospitalizacionProductoAplicacionId);
    }
}
