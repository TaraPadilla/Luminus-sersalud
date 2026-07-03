using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class HospitalizacionUsuarioAccesoRepository : IHospitalizacionUsuarioAcceso
    {
        private readonly Context _context;

        public HospitalizacionUsuarioAccesoRepository(Context context)
        {
            _context = context;
        }

        public void Add(HospitalizacionUsuarioAcceso entity)
        {
            _context.HospitalizacionUsuarioAcceso.Add(entity);
            _context.SaveChanges();
        }

        public List<HospitalizacionUsuarioAcceso> GetHospitalizacionUsuarioAccesosByIdHospitalizacion(int hospitalizacionId)
        {
            return _context.HospitalizacionUsuarioAcceso
                .Include(x => x.User)
                    .ThenInclude(x => x.Persona)
                .Where(x => x.HospitalizacionId == hospitalizacionId
                && x.Eliminado == false).ToList();
        }
        public void Update(HospitalizacionUsuarioAcceso usuarioAcceso)
        {
            _context.Entry(usuarioAcceso).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public void Delete(HospitalizacionUsuarioAcceso entity)
        {
            _context.HospitalizacionUsuarioAcceso.Update(entity);
            _context.SaveChanges();
        }
        public HospitalizacionUsuarioAcceso GetById(int id)
        {
            return _context.HospitalizacionUsuarioAcceso.Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
