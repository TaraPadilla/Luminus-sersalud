using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface INotaEnfermeria2
    {
        public void AddNotaEnfermeria(NotaEnfermeria2 entity);
        public List<NotaEnfermeria2> GetNotaEnfermeriaListByHospitalizacionId(int hospitalizacionId);
    }
}
