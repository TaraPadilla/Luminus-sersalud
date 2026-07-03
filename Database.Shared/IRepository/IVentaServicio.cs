//using Database.Shared.Models;
//using System.Collections.Generic;
//using Database.Shared.Paginacion;
//using System;

//namespace Database.Shared.IRepository
//{
//    public interface IVentaServicio
//    {
//        public void Add(DetalleServicio detalle, bool saveChanges = true);

//        public void saveChanges();

//        public List<VentaServicio> GetList();
        
//        public List<VentaServicio> GetListadoFecha(DateTime inicio, DateTime final);
        
//        public List<VentaServicio> GetListadoFechaEmpleado(DateTime inicio, DateTime final, int? id);

//        public VentaServicio Get(int id, bool includeRelatedEntities = true);
        
//        public List<DetalleServicio> GetDetalle(int id, bool includeRelatedEntities = true);

//        public void Delete(int id, bool savechanges = true);

//         public void Update(VentaServicio model, bool saveChanges = true);

//         public PaginacionList<VentaServicio> PaginacionVentaServicios(string sortOrder, string searchString, int? pageNumber, int pageSize);
//        // public void Update(Venta model, bool saveChanges = true);

//        //  public Venta Get(int id, bool includeRelatedEntities = true);

//    }
//}