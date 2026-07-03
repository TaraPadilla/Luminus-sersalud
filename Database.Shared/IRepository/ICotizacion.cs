using System.Collections.Generic;
using Database.Shared.Models;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface ICotizacion
    {
        public void Add(DetalleCotizacion detalle, bool saveChanges = true);
        public Cotizacion Get(int id, bool includeRelatedEntities = true);
        public List<DetalleCotizacion> GetDetalle(int id, bool includeRelatedEntities = true);

        public PaginacionList<Cotizacion> PaginacionCotizacionNoConfirmadas(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public PaginacionList<Cotizacion> PaginacionCotizacionConfirmadas(string sortOrder, string searchString, int? pageNumber, int pageSize);

        public List<Cotizacion> GetList();

        public void Delete(int id, bool savechanges = true);
        public void DeleteCoti(int id, bool savechanges = true);
        public void Update(Cotizacion model, bool saveChanges = true);
        public void saveChanges();


    }

}