using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface IIngestaExcretaService
    {
        public List<IngestaExcretaViewModel> GetListByHospitalizacionId(int hospitalicacionId);
        public void Add(IngestaExcreta2 entity);

        IngestaExcreta2 GetById(int id);
        void Update(IngestaExcreta2 entity);
    }
}
