using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface INotaMedica2
    {
        public void Add(NotaMedica2 entity);
        public List<NotaMedica2> GetAllByIdHospitalizacion(int idHospitalizacion);


        NotaMedica2 GetById(int id);
        void Update(NotaMedica2 entity);
    }
}
