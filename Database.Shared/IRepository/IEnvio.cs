using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;
using System;

namespace Database.Shared.IRepository
{
    public interface IEnvio
    {
        public void Add(DetalleEnvio detalle, bool saveChanges = true);
        public void AddPago(Pagos pago, bool saveChanges = true);

        public void saveChanges();

        public List<Envio> GetList();

        public List<FormaPago> GetListPagos();

         public List<Envio> GetListadoFecha(DateTime inicio, DateTime final);

        public Envio Get(int id, bool includeRelatedEntities = true);
        
        public List<DetalleEnvio> GetDetalle(int id, bool includeRelatedEntities = true);

        public void Delete(int id, bool savechanges = true);

         public void Update(Envio model, bool saveChanges = true);

         public PaginacionList<Envio> PaginacionEnvios(string sortOrder, string searchString, int? pageNumber, int pageSize);

         public PaginacionList<Envio> PaginacionEnviosLiquidados(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public PaginacionList<Envio> PaginacionEnviosRechazados(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public PaginacionList<Envio> PaginacionEnviosPedidos(string sortOrder, string searchString, int? pageNumber, int pageSize);
        public PaginacionList<Envio> PaginacionEnviosEnRuta(string sortOrder, string searchString, int? pageNumber, int pageSize, string id);
        public PaginacionList<Envio> PaginacionMisPedidos(string sortOrder, string searchString, int? pageNumber, int pageSize,string id);

        public PaginacionList<Envio> PaginacionMisPedidosEntregados(string sortOrder, string searchString, int? pageNumber, int pageSize,string id);
        

      

    }
}