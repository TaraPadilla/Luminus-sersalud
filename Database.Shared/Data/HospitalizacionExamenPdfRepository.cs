using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Database.Shared.Data
{
    public class HospitalizacionExamenPdfRepository : IHospitalizacionExamenPdf
    {
        private readonly Context _context;

        public HospitalizacionExamenPdfRepository(Context context)
        {
            _context = context;
        }

        public void Add(HospitalizacionExamenPdf entity)
        {
            _context.HospitalizacionExamenPdf.Add(entity);
            _context.SaveChanges();
        }

        public void Update(HospitalizacionExamenPdf entity)
        {
            _context.HospitalizacionExamenPdf.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _context.HospitalizacionExamenPdf.Find(id);
            if (entity != null)
            {
                _context.HospitalizacionExamenPdf.Remove(entity);
                _context.SaveChanges();
            }
        }

        public HospitalizacionExamenPdf GetById(int id)
        {
            return _context.HospitalizacionExamenPdf
                .Include(x => x.Hospitalizacion)
                .Include(x => x.Examen)
                .FirstOrDefault(x => x.Id == id && !x.Eliminado);
        }

        public List<HospitalizacionExamenPdf> GetAll()
        {
            return _context.HospitalizacionExamenPdf
                .Include(x => x.Hospitalizacion)
                .Include(x => x.Examen)
                .Where(x => !x.Eliminado)
                .ToList();
        }

        public List<HospitalizacionExamenPdf> GetByHospitalizacionAndExamen(int hospitalizacionId, int examenId)
        {
            return _context.HospitalizacionExamenPdf
                .Include(x => x.Hospitalizacion)
                .Include(x => x.Examen)
                .Where(x => x.HospitalizacionId == hospitalizacionId && x.ExamenId == examenId && !x.Eliminado)
                .ToList();
        }

        public List<HospitalizacionExamenPdf> GetByHospitalizacionId(int hospitalizacionId)
        {
            return _context.HospitalizacionExamenPdf
                .Include(x => x.Hospitalizacion)
                .Include(x => x.Examen)
                .Where(x => x.HospitalizacionId == hospitalizacionId && !x.Eliminado)
                .ToList();
        }
        
        public void MarkAsDeleted(int id)
        {
            var entity = _context.HospitalizacionExamenPdf.Find(id);
            if (entity != null)
            {
                entity.Eliminado = true;
                _context.HospitalizacionExamenPdf.Update(entity);
                _context.SaveChanges();
            }
        }
    }
}
