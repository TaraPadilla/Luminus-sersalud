using System.Collections.Generic;
using Database.Shared.Models;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface ICategoriaGasto
    {
        public List<CategoriaGasto> ListarCategorias();

        public void Add(CategoriaGasto categoria, bool saveChanges = true);

        public CategoriaGasto Get(int id, bool includeRelatedEntities = true);

        public void Update(CategoriaGasto categoria, bool saveChanges = true);

        public PaginacionList<CategoriaGasto> PaginacionCategoria(string sortOrder, string searchString, int? pageNumber, int pageSize);


    }
}