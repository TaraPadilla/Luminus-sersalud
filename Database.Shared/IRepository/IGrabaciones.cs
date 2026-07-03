using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;
using System;
using System.Linq;
using Database.Shared.DataBindings;

namespace Database.Shared.IRepository
{
    public interface IGrabaciones
    {
        IList<Grabacion> GetList();
        void Add(Grabacion model);
        void Update(Grabacion model);
        Grabacion Get(int id);
        void Delete(int grabacionId);
    }
}