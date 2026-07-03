using Database.Shared.IRepository;
using Database.Shared.Models;
using Database.Shared.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;

namespace Database.Shared.Data
{
    public class CotizacionRepository : ICotizacion
    {
        private readonly Context _context = null;

        public CotizacionRepository(Context context)
        {
            _context = context;
        }

        public void Add(DetalleCotizacion detalle, bool saveChanges = true)
        {
            _context.DetalleCotizaciones.Add(detalle);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public Cotizacion Get(int id, bool includeRelatedEntities = true)
        {

            return _context.Cotizaciones
                .Include(a => a.DetalleCotizacion).ThenInclude(a => a.Producto)
               .Include(a => a.DetalleCotizacion).ThenInclude(a => a.Servicio)
               .Where(a => a.Id == id)
               .SingleOrDefault();
        }

        public List<DetalleCotizacion> GetDetalle(int id, bool includeRelatedEntities = true)
        {

            var detalle = _context.DetalleCotizaciones.AsQueryable();

            return detalle.Where(x => x.Cotizacion.Id == id).ToList();


        }
        public List<Cotizacion> GetList() => _context.Cotizaciones.Include(a => a.DetalleCotizacion).Where(x => x.Eliminado == false).ToList();

        public PaginacionList<Cotizacion> PaginacionCotizacionNoConfirmadas(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var cotizaciones = _context.Cotizaciones.AsQueryable();


            if (!string.IsNullOrEmpty(searchString))
            {
                // cotizaciones = cotizaciones.Where(s => s.Cliente.Contains(searchString) || s.Id.ToString().Contains(searchString));
            }

            return PaginacionList<Cotizacion>.CreateAsyncc(cotizaciones
            .Include(a => a.DetalleCotizacion)
            .OrderByDescending(a => a.FechaCotizacion)
            .Where(a => a.Confirmado == false)
            , pageNumber ?? 1, pageSize);
        }

        public PaginacionList<Cotizacion> PaginacionCotizacionConfirmadas(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var cotizaciones = _context.Cotizaciones.AsQueryable();


            if (!string.IsNullOrEmpty(searchString))
            {
                // cotizaciones = cotizaciones.Where(s => s.Cliente.Contains(searchString) || s.Id.ToString().Contains(searchString));
            }

            return PaginacionList<Cotizacion>.CreateAsyncc(cotizaciones
            .Include(a => a.DetalleCotizacion)
            .OrderByDescending(a => a.FechaCotizacion)
            .Where(a => a.Confirmado == true)
            , pageNumber ?? 1, pageSize);
        }

        public void Delete(int id, bool savechanges = true)
        {
            var set = _context.Set<DetalleCotizacion>();
            var entity = set.Find(id);
            set.Remove(entity);

            if (true)
            {
                _context.SaveChanges();

            }

        }

        public void DeleteCoti(int id, bool savechanges = true)
        {
            var set = _context.Set<Cotizacion>();
            var entity = set.Find(id);
            set.Remove(entity);

            if (true)
            {
                _context.SaveChanges();

            }

        }

        public void Update(Cotizacion model, bool saveChanges = true)
        {

            _context.Entry(model).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }


        public void saveChanges()
        {

            _context.SaveChanges();
        }

    }

}
