using Database.Shared.Models;
using Database.Shared.Paginacion;
using Microsoft.AspNetCore.Identity;
using sistema.Models;

namespace sistema.Service.IService 
{ 
    public interface ICategoriaGeneralLabClinicoService
    {

        public PaginacionList<CategoriaGeneralLabClinicoViewModel> GetListCategoriasGeneralesLabClinico(string sortOrder, string searchString, int? pageNumber, int pageSize);

        public void Add(CategoriaGeneralLabClinicoViewModel categoriaViewModel);

        public CategoriaGeneralLabClinicoViewModel GetCategoriaGeneralLab(int id);

        public void Update(CategoriaGeneralLabClinicoViewModel categoriaViewModel);
    }
}
