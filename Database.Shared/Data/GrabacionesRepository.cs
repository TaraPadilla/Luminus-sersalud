using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;

namespace Database.Shared.Data
{
    public class GrabacionesRepository : IGrabaciones
    {
        private readonly Context _context = null;

        public GrabacionesRepository(Context context)
        {
            _context = context;
        }

        public IList<Grabacion> GetList()
        {
            return _context.Grabaciones
            .Where(a => a.Eliminada == false)
            .ToList();
        }

        public void Add(Grabacion model)
        {
            _context.Grabaciones.Add(model);
            _context.SaveChanges();
        }

        public Grabacion Get(int id)
        {
            return _context.Grabaciones
            .Where(a => a.Id == id).SingleOrDefault();
        }

        public void Update(Grabacion model)
        {
            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public void Delete(int grabacionId)

        {
            var grabacion = _context.Grabaciones
                .Where(g => g.Id == grabacionId)
                .FirstOrDefault();
            grabacion.Eliminada = true;
            _context.SaveChanges();
        }
    }
}