using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IGasto
    {
        public void Add(Gasto producto, bool saveChanges = true);
        public List<Gasto> GetList();

      public IList<Gasto> GetListado();
        
        public Gasto Get(int id, bool includeRelatedEntities = true);

        public Gasto GetPorNombre(string nombre, bool includeRelatedEntities = true);

        public void Update(Gasto gasto, bool saveChanges = true);

        public void SaveChanges();

        public PaginacionList<Gasto> PaginacionGastos(string sortOrder, string searchString, int? pageNumber, int pageSize);

    } 

}