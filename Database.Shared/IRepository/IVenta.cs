using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;
using System;

namespace Database.Shared.IRepository
{
    public interface IVenta
    {
        public List<User> GetUsersRole(string rolename);
        public Empleado GetEmpleadoUser(string id);

        public List<Venta> GetVentasFechas(DateTime fecha1, DateTime fecha2);
        public void Add(DetalleVenta detalle, bool saveChanges = true);
        public void Add(VentaPerdida ventaPerdida);

        public void saveChanges();

        public List<Venta> GetList();

        public List<Venta> GetListado();

        public List<DetalleVenta> GetListadoDetalles();

        public List<Venta> GetListadoFecha(DateTime inicio, DateTime final);
        public List<Venta> GetListadoFechaEmpleado(DateTime inicio, DateTime final, int? id);



        List<Venta> GetListadoFechaFarmacia(DateTime inicio, DateTime final);
        List<Venta> GetListadoFechaEmpleadoFarmacia(DateTime inicio, DateTime final, int? id);
        List<Venta> GetListadoFechaClinica(DateTime inicio, DateTime final);
        List<Venta> GetListadoFechaEmpleadoClinica(DateTime inicio, DateTime final, int? id);

        List<Venta> GetListadoFechaLaboratorio(DateTime inicio, DateTime final);
        List<Venta> GetListadoFechaEmpleadoLaboratorio(DateTime inicio, DateTime final, int? empleadoId);

        public Venta Get(int id, bool includeRelatedEntities = true);
        public Venta GetExamen(int exmaneId, bool includeRelatedEntities = true);
        public List<DetalleVenta> GetDetalle(int id, bool includeRelatedEntities = true);

        public void Delete(int id, bool savechanges = true);

        public void Update(Venta model, bool saveChanges = true);

        public PaginacionList<Venta> PaginacionVentasFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize);

        public PaginacionList<Venta> PaginacionVentasClinica(string sortOrder, string searchString, int? pageNumber, int pageSize, string fechaInicial, string fechaFinal, int? numeroVenta, string comprobante, int? formaPago, string origenVenta);
        public PaginacionList<Venta> PaginacionVentasHospital(string sortOrder, string searchString, int? pageNumber, int pageSize, string fechaInicial, string fechaFinal, int? numeroVenta, string comprobante, int? formaPago);

        public PaginacionList<Venta> PaginacionVentasLaboratorio(string sortOrder, string searchString, int? pageNumber, int pageSize, string fechaInicial, string fechaFinal, int? numeroVenta, string comprobante, int? formaPago);
        // public void Update(Venta model, bool saveChanges = true);

        //  public Venta Get(int id, bool includeRelatedEntities = true);
        IEnumerable<Pagos> GetPagosByVentaId(int ventaId);

    }
}