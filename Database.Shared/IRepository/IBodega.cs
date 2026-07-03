using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IBodega
    {
        //public Clientes Add(Clientes Paciente, bool saveChanges = true);
        public List<Bodega> GetList();
        //public PaginacionList<Clientes> PaginacionClientes(string sortOrder, string searchString, int? pageNumber, int pageSize);
        //public void Update(Clientes model, bool saveChanges = true);

        //public Clientes Get(int id, bool includeRelatedEntities = true);

        //public Clientes GetClientePorNombre(string nombre); 

        //public Clientes GetClientePorId(int id);
        public Bodega GetById(int id);    
    }
}