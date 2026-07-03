using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IAmbiente
    {
        public List<Ambiente> GetList();
    }
}