using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IRuta
    {
        public void Add(Ruta ruta, bool saveChanges = true);
        public List<Ruta> GetList();
        public Ruta Get(int id, bool includeRelatedEntities = true);

        public void Update(Ruta ruta, bool saveChanges = true);

        public void SaveChanges();

         public PaginacionList<Ruta> PaginacionRutas(string sortOrder, string searchString, int? pageNumber, int pageSize);
    }
}