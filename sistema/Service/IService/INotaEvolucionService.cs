using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface INotaEvolucionService
    {
        public List<NotaEvolucionVM> GetNotaEvolucionListByHospitalizacionId(int hospitalicacionId);
        public void AddNotaEvolucion(NotaEvolucion entity);

    }
}
