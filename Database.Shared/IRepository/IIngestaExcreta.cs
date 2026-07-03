using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface IIngestaExcreta
    {
        public void Add(IngestaExcreta2 entity);
        public List<IngestaExcreta2> GetListByHospitalizacionId(int hospitalizacionId);

        IngestaExcreta2 Get(int id);
        void Update(IngestaExcreta2 entity);
    }
}
