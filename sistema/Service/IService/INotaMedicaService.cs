using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;

namespace farmamest.Service.IService
{
    public interface INotaMedica2Service
    {
        public void Add(NotaMedica2 entity);
        public List<NotaMedica2ViewModel> GetListByIdHospitalizacion(int idHospitalizacion);


        NotaMedica2 GetById(int id);
        void Update(NotaMedica2 entity);
    }
}
