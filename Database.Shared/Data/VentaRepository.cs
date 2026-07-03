using Database.Shared.IRepository;
using Database.Shared.Models;
using Database.Shared.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;
using Database.Shared.Enumeraciones;

namespace Database.Shared.Data
{
    public class VentaRepository : IVenta
    {

        private readonly Context _context = null;
        public VentaRepository(Context context)
        {
            _context = context;
        }

        public void Add(DetalleVenta detalle, bool saveChanges = true)
        {
            _context.Add(detalle);
            if (saveChanges)
            {

                _context.SaveChanges();
            }
        }
        public void Add(VentaPerdida ventaPerdida)
        {
            _context.VentasPerdidas.Add(ventaPerdida);
            _context.SaveChanges();
        }

        public List<Venta> GetList() => _context.Ventas.Include(a => a.Paciente)
        .OrderByDescending(a => a.Id)
        .Where(x => x.Eliminado == false)
        .ToList();

        public List<Venta> GetListado() => _context.Ventas.Include(a => a.Paciente).Include(a => a.DetalleVenta).ThenInclude(a => a.Producto).Include(a => a.Empleado).ToList();

        public List<Venta> GetListadoFecha(DateTime inicio, DateTime final) => _context.Ventas
        .Include(a => a.Clientes)
        .Include(a => a.DetalleVenta).ThenInclude(a => a.Producto)
        .Include(a => a.Empleado)
        .Include(a => a.Pagos).ThenInclude(a => a.FormaPago)
        .Where(a => a.FechaVenta <= final && a.FechaVenta >= inicio)
        .Where(a => a.Eliminado == false)
        .OrderByDescending(a => a.FechaVenta)
        .ToList();

        public List<Venta> GetListadoFechaEmpleado(DateTime inicio, DateTime final, int? id) => _context.Ventas
       .Include(a => a.Clientes).Include(a => a.DetalleVenta)
       .ThenInclude(a => a.Producto).Include(a => a.Empleado)
       .Include(a => a.Pagos).ThenInclude(a => a.FormaPago)
       .Where(a => a.FechaVenta <= final && a.FechaVenta >= inicio)
       .Where(a => a.Eliminado == false)
       .Where(a => a.EmpleadoId == id)
       .OrderByDescending(a => a.FechaVenta)
       .ToList();

        public List<Venta> GetListadoFechaFarmacia(DateTime inicio, DateTime final) => _context.Ventas
                  .Include(a => a.Clientes)
                  .Include(a => a.Paciente)
                  .Include(a => a.DetalleVenta)
                      .ThenInclude(a => a.Producto)
                          .ThenInclude(p => p.ProductosInventario)
                              .ThenInclude(pi => pi.ProductosInventarioPrecios)
                                  .ThenInclude(pin => pin.Precio)
                  .Include(a => a.DetalleVenta)
                      .ThenInclude(a => a.Producto)
                          .ThenInclude(p => p.Categoria)
                  .Include(a => a.Empleado)
                  .Include(a => a.Pagos)
                      .ThenInclude(a => a.FormaPago)
                  .Where(a => a.FechaVenta <= final && a.FechaVenta >= inicio)
                  .Where(a => a.Eliminado == false)
                  .OrderByDescending(a => a.Id)
                  .ToList();

        public List<Venta> GetListadoFechaEmpleadoFarmacia(DateTime inicio, DateTime final, int? id) => _context.Ventas
        .Include(a => a.Clientes).Include(a => a.DetalleVenta)
        .ThenInclude(a => a.Producto).Include(a => a.Empleado)
        .Include(a => a.Pagos).ThenInclude(a => a.FormaPago)
        .Where(a => a.FechaVenta <= final && a.FechaVenta >= inicio)
        .Where(a => a.Eliminado == false)
        .Where(a => a.EmpleadoId == id)
        .OrderByDescending(a => a.Id)
        .ToList();





        public List<Venta> GetListadoFechaClinica(DateTime inicio, DateTime final)
        {
            return _context.Ventas
                .Include(v => v.Paciente)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.Servicio)
                    .ThenInclude(s => s.ServiciosPrecios)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.Servicio)
                    .ThenInclude(s => s.CategoriaServicio) // Include CategoriaServicio here
                .Include(v => v.Empleado)
                .Include(v => v.Pagos)
                    .ThenInclude(p => p.FormaPago)
                .Where(v => v.FechaVenta >= inicio && v.FechaVenta <= final)
                .Where(v => v.Eliminado == false)
                .Where(v => v.DetalleVenta.Any(d => d.ServicioId != null)) // Only sales with services
                .OrderByDescending(v => v.Id)
                .ToList();
        }


        public List<Venta> GetListadoFechaEmpleadoClinica(DateTime inicio, DateTime final, int? empleadoId)
        {
            return _context.Ventas
                .Include(v => v.Paciente)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.Servicio)
                    .ThenInclude(s => s.ServiciosPrecios)
                .Include(v => v.Empleado)
                .Include(v => v.Pagos)
                    .ThenInclude(p => p.FormaPago)
                .Where(v => v.FechaVenta >= inicio && v.FechaVenta <= final)
                .Where(v => v.Eliminado == false)
                .Where(v => v.EmpleadoId == empleadoId)

                .Where(v => v.DetalleVenta.Any(d => d.ServicioId != null)) // Solo ventas con servicios
                .OrderByDescending(v => v.Id)
                .ToList();
        }


        public List<Venta> GetListadoFechaLaboratorio(DateTime inicio, DateTime final)
        {
            return _context.Ventas
                .Include(v => v.Paciente)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.ExamenLabClinico)
                    .ThenInclude(e => e.ExamenLabClinicosPrecios)
                .Include(v => v.Empleado)
                .Include(v => v.Pagos)
                    .ThenInclude(p => p.FormaPago)
                .Where(v => v.FechaVenta >= inicio && v.FechaVenta <= final)
                .Where(v => v.Eliminado == false)

                .Where(v => v.DetalleVenta.Any(d => d.ExamenLabClinicoId != null)) // Solo ventas con exámenes de laboratorio
                .OrderByDescending(v => v.Id)
                .ToList();
        }

        public List<Venta> GetListadoFechaEmpleadoLaboratorio(DateTime inicio, DateTime final, int? empleadoId)
        {
            return _context.Ventas
                .Include(v => v.Paciente)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.ExamenLabClinico)
                    .ThenInclude(e => e.ExamenLabClinicosPrecios)
                .Include(v => v.Empleado)
                .Include(v => v.Pagos)
                    .ThenInclude(p => p.FormaPago)
                .Where(v => v.FechaVenta >= inicio && v.FechaVenta <= final)
                .Where(v => v.Eliminado == false)
                .Where(v => v.EmpleadoId == empleadoId)

                .Where(v => v.DetalleVenta.Any(d => d.ExamenLabClinicoId != null)) // Solo ventas con exámenes de laboratorio
                .OrderByDescending(v => v.Id)
                .ToList();
        }


        public List<DetalleVenta> GetDetalle(int id, bool includeRelatedEntities = true)
        {

            var detalle = _context.DetalleVentas.AsQueryable();

            if (includeRelatedEntities)
            {
                detalle = detalle.Include(x => x.Producto);
            }

            return detalle.Where(x => x.Venta.Id == id).ToList();


        }

        public List<DetalleVenta> GetListadoDetalles() => _context.DetalleVentas.Include(a => a.Venta).Include(a => a.Producto).ToList();



        public PaginacionList<Venta> PaginacionVentasFarmacia(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var venta = _context.Ventas.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                venta = venta.Where(s => s.NoComprobante.Contains(searchString) || s.Id.ToString().Contains(searchString));
            }


            return PaginacionList<Venta>.CreateAsyncc(venta
            .Include(a => a.Paciente)
            .Include(a => a.DetalleVenta)
            .Include(a => a.Empleado)
            .Include(a => a.Pagos).ThenInclude(a => a.FormaPago)
            .Where(a => a.Eliminado == false && a.AmbienteId == (int)AmbienteEnum.Farmacia)
            .OrderByDescending(a => a.FechaVenta)
            , pageNumber ?? 1, pageSize);
        }

        public PaginacionList<Venta> PaginacionVentasClinica(string sortOrder, string searchString, int? pageNumber, int pageSize,
            string fechaInicial, string fechaFinal, int? numeroVenta, string comprobante, int? formaPago, string origenVenta)
        {
            var venta = _context.Ventas.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                venta = venta.Where(s => s.NoComprobante.Contains(searchString) || s.Id.ToString().Contains(searchString));
            }


            if (!string.IsNullOrEmpty(fechaInicial) && !string.IsNullOrEmpty(fechaFinal))
            {
                DateTime fechaInicio = DateTime.Parse(fechaInicial);
                DateTime fechaFin = DateTime.Parse(fechaFinal);
                venta = venta.Where(x => x.FechaVenta.Date >= fechaInicio && x.FechaVenta.Date <= fechaFin);

            }

            if (numeroVenta != 0 && numeroVenta != null)
            {
                venta = venta.Where(x => x.Id == numeroVenta);

            }

            if (!string.IsNullOrEmpty(comprobante))
            {
                venta = venta.Where(x =>
                    x.NoComprobante != null &&
                    x.NoComprobante.ToLower().Trim() == comprobante.ToLower().Trim());
            }


            if (formaPago != 0 && formaPago != null)
            {
                venta = venta.Where(v => v.Pagos.Any(p => p.FormaPagoId == formaPago));
            }

            if (!string.IsNullOrEmpty(origenVenta))
            {
                if (origenVenta == "CONSULTA")
                {
                    venta = venta.Where(x => string.IsNullOrEmpty(x.Origen) || x.Origen != "EMERGENCIA");
                }
                else if (origenVenta == "EMERGENCIA")
                {
                    venta = venta.Where(x => x.Origen == "EMERGENCIA");
                }
            }

            //if (!string.IsNullOrEmpty(comprobanteFacturado))
            //{
            //    //Ajustar el medico referido

            //    foreach (var item in venta)
            //    {
            //        foreach (var item1 in item.Pagos)
            //        {
            //            venta = venta.Where(x => x. item1.FormaPago.NombreFormaPago);
            //        }

            //    }
            //    venta = venta.Where(x => x.pago. == numeroVenta);

            //}



            return PaginacionList<Venta>.CreateAsyncc(venta
            .Include(a => a.Paciente)
            .Include(a => a.DetalleVenta)
            .Include(a => a.Empleado)
            .Include(a => a.Pagos).ThenInclude(a => a.FormaPago)
            .OrderByDescending(a => a.FechaVenta)
            .Where(a => a.Eliminado == false && a.AmbienteId == (int)AmbienteEnum.Clinica)
            , pageNumber ?? 1, pageSize);
        }

        public PaginacionList<Venta> PaginacionVentasHospital(string sortOrder, string searchString, int? pageNumber, int pageSize,
                   string fechaInicial, string fechaFinal, int? numeroVenta, string comprobante, int? formaPago)
        {
            var venta = _context.Ventas.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                venta = venta.Where(s => s.NoComprobante.Contains(searchString) || s.Id.ToString().Contains(searchString));
            }


            if (!string.IsNullOrEmpty(fechaInicial) && !string.IsNullOrEmpty(fechaFinal))
            {
                DateTime fechaInicio = DateTime.Parse(fechaInicial);
                DateTime fechaFin = DateTime.Parse(fechaFinal);
                venta = venta.Where(x => x.FechaVenta.Date >= fechaInicio && x.FechaVenta.Date <= fechaFin);

            }

            if (numeroVenta != 0 && numeroVenta != null)
            {
                venta = venta.Where(x => x.Id == numeroVenta);

            }

            if (!string.IsNullOrEmpty(comprobante))
            {
                venta = venta.Where(x =>
                    x.NoComprobante != null &&
                    x.NoComprobante.ToLower().Trim() == comprobante.ToLower().Trim());
            }


            if (formaPago != 0 && formaPago != null)
            {
                venta = venta.Where(v => v.Pagos.Any(p => p.FormaPagoId == formaPago));
            }

            //if (!string.IsNullOrEmpty(comprobanteFacturado))
            //{
            //    //Ajustar el medico referido

            //    foreach (var item in venta)
            //    {
            //        foreach (var item1 in item.Pagos)
            //        {
            //            venta = venta.Where(x => x. item1.FormaPago.NombreFormaPago);
            //        }

            //    }
            //    venta = venta.Where(x => x.pago. == numeroVenta);

            //}



            return PaginacionList<Venta>.CreateAsyncc(venta
            .Include(a => a.Paciente)
            .Include(a => a.DetalleVenta)
            .Include(a => a.Empleado)
            .Include(a => a.Pagos).ThenInclude(a => a.FormaPago)
            .OrderByDescending(a => a.FechaVenta)
            .Where(a => a.Eliminado == false && a.AmbienteId == (int)AmbienteEnum.Hospital)
            , pageNumber ?? 1, pageSize);
        }

        public PaginacionList<Venta> PaginacionVentasLaboratorio(string sortOrder, string searchString, int? pageNumber, int pageSize,
         string fechaInicial, string fechaFinal, int? numeroVenta, string comprobante, int? formaPago)
        {
            var venta = _context.Ventas
                .Include(a => a.DetalleVenta).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                venta = venta.Where(s => s.NoComprobante.Contains(searchString) || s.Id.ToString().Contains(searchString));
            }


            if (!string.IsNullOrEmpty(fechaInicial) && !string.IsNullOrEmpty(fechaFinal))
            {
                DateTime fechaInicio = DateTime.Parse(fechaInicial);
                DateTime fechaFin = DateTime.Parse(fechaFinal);
                venta = venta.Where(x => x.FechaVenta.Date >= fechaInicio && x.FechaVenta.Date <= fechaFin);

            }

            if (numeroVenta != 0 && numeroVenta != null)
            {
                venta = venta.Where(x => x.Id == numeroVenta);

            }

            if (!string.IsNullOrEmpty(comprobante))
            {
                venta = venta.Where(x =>
                    x.NoComprobante != null &&
                    x.NoComprobante.ToLower().Trim() == comprobante.ToLower().Trim());
            }


            if (formaPago != 0 && formaPago != null)
            {
                venta = venta.Where(v => v.Pagos.Any(p => p.FormaPagoId == formaPago));
            }

            //if (!string.IsNullOrEmpty(comprobanteFacturado))
            //{
            //    //Ajustar el medico referido

            //    foreach (var item in venta)
            //    {
            //        foreach (var item1 in item.Pagos)
            //        {
            //            venta = venta.Where(x => x. item1.FormaPago.NombreFormaPago);
            //        }

            //    }
            //    venta = venta.Where(x => x.pago. == numeroVenta);

            //}



            return PaginacionList<Venta>.CreateAsyncc(venta
            .Include(a => a.Paciente)
            .Include(a => a.DetalleVenta)
            .Include(a => a.Empleado)
            .Include(a => a.Pagos).ThenInclude(a => a.FormaPago)
            .OrderByDescending(a => a.FechaVenta)
            .Where(a => a.Eliminado == false && a.AmbienteId == (int)AmbienteEnum.Laboratorio)
            , pageNumber ?? 1, pageSize);
        }
        public List<User> GetUsersRole(string rolename)
        {

            var users = (from user in _context.Usuarios
                         join userRole in _context.UserRoles
                         on user.Id equals userRole.UserId
                         join role in _context.Roles
                         on userRole.RoleId equals role.Id
                         where role.Name == rolename
                         select user)
                                   .ToList();

            return users;
        }


        public List<Venta> GetVentasFechas(DateTime fecha1, DateTime fecha2)
        {


            var ventas = _context.Ventas.FromSqlRaw("DECLARE @fecha1  DATETIME, @fecha2  DATETIME;  SELECT   @fecha1 = '20210101' ,@fecha2   = '20210201';  SELECT  DATENAME(MONTH, DATEADD(MONTH, x.number, @fecha1)) AS MonthName FROM Ventas x WHERE   x.type = 'P'  AND  x.number <= DATEDIFF(MONTH, @fecha1, @fecha2)").ToList();


            return ventas;
        }

        public Empleado GetEmpleadoUser(string id)
        {

            var emp = (from empleado in _context.Empleados
                       join user in _context.Usuarios
                       on empleado.Id equals user.EmpleadoId
                       join envio in _context.Envios
                       on user.Id equals envio.UserId1
                       where user.Id == id
                       select empleado
                          ).SingleOrDefault();



            return emp;
        }

        public void saveChanges()
        {
            _context.SaveChanges();
        }

        public Venta Get(int id, bool includeRelatedEntities = true)
        {
            var Venta = _context.Ventas
                .Include(a => a.DetalleVenta).AsQueryable();

            if (includeRelatedEntities)
            {
                Venta = Venta
                .Include(a => a.DetalleVenta).ThenInclude(a => a.Producto)
                .Include(a => a.DetalleVenta).ThenInclude(a => a.Servicio)
                .Include(a => a.DetalleVenta).ThenInclude(a => a.ExamenLabClinico)
                .Include(a => a.Pagos).ThenInclude(a => a.FormaPago)
                .Include(a => a.Empleado)
                .Include(a => a.Paciente);
            }

            return Venta.Where(a => a.Id == id).SingleOrDefault();
        }

        //Obrener una venta de laboratorio  por medio de un examen
        public Venta GetExamen(int exmaneId, bool includeRelatedEntities = true)
        {
            var Venta = _context.Ventas.AsQueryable();

            if (includeRelatedEntities)
            {
                Venta = Venta
                .Include(a => a.DetalleVenta).ThenInclude(a => a.Producto)
                .Include(a => a.DetalleVenta).ThenInclude(a => a.Servicio)
                .Include(a => a.DetalleVenta).ThenInclude(a => a.ExamenLabClinico)
                .Include(a => a.Pagos).ThenInclude(a => a.FormaPago)
                .Include(a => a.Empleado)
                .Include(a => a.Examen)
                .Include(a => a.Paciente);
            }

            return Venta.Where(a => a.ExamenId == exmaneId).FirstOrDefault();
        }

        public void Delete(int id, bool savechanges = true)
        {
            var set = _context.Set<DetalleVenta>();
            var entity = set.Find(id);
            set.Remove(entity);

            if (true)
            {
                _context.SaveChanges();

            }

        }

        public void Update(Venta model, bool saveChanges = true)
        {

            _context.Entry(model).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }


        public IEnumerable<Pagos> GetPagosByVentaId(int ventaId)
        {
            return _context.Pagos.Where(p => p.VentaId == ventaId).ToList();
        }


    }

}



