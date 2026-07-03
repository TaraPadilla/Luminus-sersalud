using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IEspecialidad
    {
        public List<Especialidad> GetAll();
        Especialidad Add(Especialidad especialidad);

        void Update(Especialidad especialidad);

        void Delete(int id);

        public PaginacionList<Especialidad> PaginacionEspecialidades(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public Especialidad Get(int id, bool includeRelatedEntities = true);
    }
}
