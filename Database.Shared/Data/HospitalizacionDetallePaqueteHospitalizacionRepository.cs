using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class HospitalizacionDetallePaqueteHospitalizacionRepository : IHospitalizacionDetallePaqueteHospitalizacion
    {
        private readonly Context _context;

        public HospitalizacionDetallePaqueteHospitalizacionRepository(Context context)
        {
            _context = context;
        }

        public HospitalizacionDetallePaqueteHospitalizacion GetById(int id)
        {
            return _context.HospitalizacionDetallePaqueteHospitalizacion
                .Include(a => a.HospitalizacionPaqueteHospitalizacion)
                .Where(x => x.Id == id).FirstOrDefault();
        }

        public IEnumerable<HospitalizacionDetallePaqueteHospitalizacion> GetAll()
        {
            return _context.HospitalizacionDetallePaqueteHospitalizacion.ToList();
        }
        public void Update(HospitalizacionDetallePaqueteHospitalizacion entity)
        {
            _context.HospitalizacionDetallePaqueteHospitalizacion.Update(entity);
            _context.SaveChanges();
        }
    }
}
