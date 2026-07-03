using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface INotaEnfermeria2Service
    {
        public List<NotaEnfermeria2VM> GetNotaEnfermeriaListByHospitalizacionId(int hospitalicacionId);
        public void AddNotaEnfermeria(NotaEnfermeria2 entity);

    }
}
