using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Database.Shared.Repository
{
    public class HospitalizacionDocumentoRepository : IHospitalizacionDocumentoRepository
    {
        private readonly Context _context;

        public HospitalizacionDocumentoRepository(Context context)
        {
            _context = context;
        }

        public DocumentosHospitalizacion GetById(int id)
        {
            return _context.DocumentosHospitalizacion
                .FirstOrDefault(d => d.Id == id && !d.Eliminado);
        }

        public IEnumerable<DocumentosHospitalizacion> GetByHospitalizacionId(int hospitalizacionId)
        {
            return _context.DocumentosHospitalizacion
                .Where(d => d.HospitalizacionId == hospitalizacionId && !d.Eliminado)
                .OrderByDescending(d => d.FechaSubida)
                .ToList();
        }

        public IEnumerable<DocumentosHospitalizacion> GetByPacienteId(int pacienteId)
        {
            return _context.DocumentosHospitalizacion
                .Where(d => d.PacienteId == pacienteId && !d.Eliminado)
                .OrderByDescending(d => d.FechaSubida)
                .ToList();
        }

        public void Add(DocumentosHospitalizacion documento)
        {
            _context.DocumentosHospitalizacion.Add(documento);
        }

        public void Update(DocumentosHospitalizacion documento)
        {
            _context.DocumentosHospitalizacion.Update(documento);
        }

        public void Delete(int id)
        {
            var doc = GetById(id);
            if (doc != null)
            {
                doc.Eliminado = true;
                Update(doc);
            }
        }

        public bool Exists(int id)
        {
            return _context.DocumentosHospitalizacion.Any(d => d.Id == id && !d.Eliminado);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}