using System.Collections.Generic;
using System.Linq;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;
using Database.Shared.Enumeraciones;

namespace Database.Shared.Data
{

    public class CajaRepository : ICaja
    {
        private readonly Context _context = null;
        public CajaRepository(Context context)
        {
            _context = context;
        }

        public IList<Caja> ListarCajas()
        {
            return _context.Cajas
            .Include(x => x.DetalleCajas)
                .ThenInclude(x => x.Banco)
            .Include(x => x.DetalleCajas)
                .ThenInclude(x => x.Cuenta)
            .Include(x => x.DetalleCajas)
                .ThenInclude(x => x.CuentaContable)

            .Include(x => x.TipoBodega)
            .Include(x => x.Ambiente)
            .Include(x => x.Sucursal)
            .Include(a => a.ResponsableApertura).ThenInclude(a => a.Persona)
            .Include(a => a.ResponsableCierre).ThenInclude(a => a.Persona)
            .OrderByDescending(a => a.FechaApertura).ToList();
        }

        public List<Caja> GetListadoFecha(DateTime inicio, DateTime final, int ambienteId)
        {
            var cajas = _context.Cajas
            .Include(x => x.DetalleCajas)
            .Include(a => a.DetalleCajas).ThenInclude(a => a.Venta).ThenInclude(a => a.Paciente)
            // .Include(a => a.DetalleCajas).ThenInclude(a => a.Compra).ThenInclude(a => a.Proveedor)
            // .Include(a => a.DetalleCajas).ThenInclude(a => a.VentaServicio).ThenInclude(a => a.Paciente)
            .Include(a => a.ResponsableApertura).ThenInclude(a => a.Persona)
            .Include(a => a.ResponsableCierre).ThenInclude(a => a.Persona)
            .Include(a => a.Sucursal)
            .Include(a => a.Ambiente)
            .Where(a => a.FechaApertura <= final && a.FechaApertura >= inicio)
            .OrderByDescending(a => a.FechaApertura)
            .ToList();

            if (ambienteId == (int)AmbienteEnum.Global)
            {
                return cajas;
            }
            else
            {
                var cajasAmbiente = new List<Caja>();
                if (cajas != null)
                {
                    foreach (var caja in cajas)
                    {
                        if (caja.AmbienteId == ambienteId)
                        {
                            cajasAmbiente.Add(caja);
                        }
                    }
                }
                return cajasAmbiente;
            }

        }



        public void Add(Caja caja, bool saveChanges = true)
        {
            _context.Cajas.Add(caja);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Update(Caja caja, bool saveChanges = true)
        {

            _context.Entry(caja).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }

        }

        public PaginacionList<Caja> PaginacionCategoria(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var caja = _context.Cajas.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if (!string.IsNullOrEmpty(searchString))
            {
                // categoria = categoria.Where(s => s..Contains public DetalleCaja GetDetalleCajaVentaServicio(int id)(searchString));
            }


            return PaginacionList<Caja>.CreateAsyncc(caja, pageNumber ?? 1, pageSize);
        }
        public List<DetalleCaja> GetDetallesCaja(int annio)
        {
            return _context.DetalleCajas
                .Include(a => a.Caja)
                .Where(a => a.Fecha.Year == annio)
                .ToList();
        }
        public Caja GetCajaAbierta()
        {
            return _context.Cajas.Where(a => a.EstadoCaja == true).FirstOrDefault();
        }
        public Caja GetCajaAbierta(int ambienteId)
        {
            return _context.Cajas
                .Where(a => a.EstadoCaja == true
                && a.AmbienteId == ambienteId)
                .FirstOrDefault();
        }

        public Caja GetCajaAbiertaById(int Id)
        {
            return _context.Cajas.Where(a => a.EstadoCaja == true && a.Id == Id).FirstOrDefault();
        }

        public Caja GetCaja(int id, bool includeRelatedEntities = true)
        {
            var caja = _context.Cajas.AsQueryable();

            if (includeRelatedEntities)
            {
                caja = caja
                .Include(a => a.Sucursal)
                .Include(a => a.Ambiente)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Venta).ThenInclude(a => a.Clientes)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Venta).ThenInclude(a => a.Empleado)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Venta).ThenInclude(a => a.DetalleVenta)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Venta).ThenInclude(a => a.Pagos).ThenInclude(a => a.FormaPago)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Compra).ThenInclude(a => a.Proveedor)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Compra).ThenInclude(a => a.Empleado)
                //.Include(a => a.DetalleCajas).ThenInclude(a => a.VentaServicio).ThenInclude(a => a.Paciente)
                //.Include(a => a.DetalleCajas).ThenInclude(a => a.VentaServicio).ThenInclude(a => a.Empleado)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.CuentaContable)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Banco)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Cuenta)
                .Include(a => a.ResponsableApertura).ThenInclude(a => a.Persona)
                .Include(a => a.ResponsableCierre).ThenInclude(a => a.Persona);
            }

            return caja.Where(a => a.Id == id).SingleOrDefault();
        }


        public List<Caja> GetSubcajas(DateTime fechaApertura, DateTime? fechaCierre, int? sucursalId, int cajaGlobalId)
        {
            //Las subcajas son:
            //1. Las que se abrieron antes de la "fechaApertura" y se encuentran abiertas. NOTA: Si se encuentra cerrada
            //y la fechaCierre que viene como parametro viene NULA tambien aplica como SUBCAJA
            //2. Las que se abrieron entre la fecha de apertura y la fecha de cierre
            //3. Las que se cerraron entre la fecha de apertura y la fecha de cierre
            var subcajas = _context.Cajas
                .Where(a =>
                (a.FechaApertura <= fechaApertura && a.EstadoCaja) ||
                (a.FechaApertura >= fechaApertura && a.FechaApertura <= fechaCierre) ||
                (a.FechaApertura >= fechaApertura && a.EstadoCaja) ||
                (a.FechaApertura >= fechaApertura && !a.EstadoCaja && fechaCierre == null) ||
                (a.FechaCierre >= fechaApertura && a.FechaCierre <= fechaCierre)
                )
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Venta)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Venta).ThenInclude(a => a.Clientes)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Venta).ThenInclude(a => a.Empleado)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Venta).ThenInclude(a => a.Pagos)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Venta).ThenInclude(a => a.Pagos).ThenInclude(a => a.FormaPago)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Compra)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Compra).ThenInclude(a => a.Proveedor)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Compra).ThenInclude(a => a.Empleado)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Banco)
                .Include(a => a.DetalleCajas).ThenInclude(a => a.Cuenta)
                .Include(a => a.Ambiente)
                .Include(a => a.Sucursal)
                .ToList();

            //Se excluye la caja GLOBAL
            if (subcajas != null)
            {
                subcajas = subcajas.Where(a => a.Id != cajaGlobalId).ToList();
            }

            //Se filtra por SUCURSAL
            if (sucursalId != null)
            {
                //Si la sucursal trae un ID solamente
                //se consultan las cajas de dicha sucursal
                subcajas = subcajas.Where(a => a.SucursalId == sucursalId).ToList();
            }

            //Se ordena por FECHA en forma descendente
            if (subcajas != null)
            {
                subcajas = subcajas.OrderByDescending(a => a.FechaApertura).ToList();
            }

            return subcajas;
        }


        // para detalles

        public void Add(DetalleCaja detalle, bool saveChanges = true)
        {
            _context.DetalleCajas.Add(detalle);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public DetalleCaja GetDetalleCaja(int id)
        {
            return _context.DetalleCajas.Where(a => a.VentaId == id).SingleOrDefault();
        }

        //public DetalleCaja GetDetalleCajaVentaServicio(int id)
        //{
        //    return _context.DetalleCajas.Where(a => a.VentaServicioId == id).SingleOrDefault();
        //}


        public void GetDetalleCajaPorVentaId(int id, bool savechanges = true)
        {
            var set = _context.Set<DetalleCaja>();
            var entity = set.Find(id);
            set.Remove(entity);

            if (true)
            {
                _context.SaveChanges();
            }
        }

        public void DeleteDetalleCaja(int id, bool saveChanges = true)
        {
            var set = _context.Set<DetalleCaja>();
            var entity = set.Find(id);
            set.Remove(entity);

            if (saveChanges)
            {
                _context.SaveChanges();

            }
        }
        public void ReabrirCaja(int cajaId)
        {
            var caja = _context.Cajas.Where(a => a.Id == cajaId).FirstOrDefault();
            if (caja != null)
            {
                caja.EstadoCaja = true;
            }
            _context.Entry<Caja>(caja).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}