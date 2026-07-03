using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IProveedor
    {
        public void Add(Proveedor empleado, bool saveChanges = true);
        public List<Proveedor> GetList();

        public List<Banco> ListarBancos();
        public void Update(Proveedor model, bool saveChanges = true);

        public Proveedor Get(int id, bool includeRelatedEntities = true);

        public PaginacionList<Proveedor> PaginacionProveedores(string sortOrder, string searchString, int? pageNumber, int pageSize);

        public Proveedor GetProveedorPorNombre(string nombre);
        public Proveedor GetByNombre(string nombre, bool includeRelatedEntities = true);


    }
}