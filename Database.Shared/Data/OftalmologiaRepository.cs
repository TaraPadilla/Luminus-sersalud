using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Database.Shared.Data
{
    public class ConsultasOftalmologiaRepository : IConsultasOftalmologia
    {
        private readonly Context _context;

        public ConsultasOftalmologiaRepository(Context context)
        {
            _context = context;
        }

        public void Add(ConsultasOftalmologia consulta, bool saveChanges = true)
        {
            _context.Set<ConsultasOftalmologia>().Add(consulta);
            if (saveChanges) _context.SaveChanges();
        }

        public long AddConsulta(ConsultasOftalmologia consulta)
        {
            _context.Set<ConsultasOftalmologia>().Add(consulta);
            _context.SaveChanges();
            return consulta.Id;
        }

        public void Update(ConsultasOftalmologia consulta, bool saveChanges = true)
        {
            _context.Entry(consulta).State = EntityState.Modified;
            if (saveChanges) _context.SaveChanges();
        }

        public ConsultasOftalmologia GetConsulta(long id)
        {
            return _context.Set<ConsultasOftalmologia>()
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);
        }

        public ConsultasOftalmologia GetConsulta(int consultaId)
        {
            return _context.Set<ConsultasOftalmologia>()
                .AsNoTracking()
                .FirstOrDefault(c => c.ConsultaId == consultaId);
        }

        public ConsultasOftalmologia GetConsultaByPaciente(int pacienteId)
        {
            return _context.Set<ConsultasOftalmologia>()
                .AsNoTracking()
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.Fecha)   // última por Fecha
                .FirstOrDefault();
        }

        public IEnumerable<ConsultasOftalmologia> GetConsultasByPaciente(int pacienteId)
        {
            return _context.Set<ConsultasOftalmologia>()
                .AsNoTracking()
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.Fecha)
                .ToList();
        }

        public IList<ConsultasOftalmologia> ListaConsultas()
        {
            return _context.Set<ConsultasOftalmologia>()
                .AsNoTracking()
                .ToList();
        }


    }
}
