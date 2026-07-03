using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Database.Shared.Data
{
    public class ConsultasPodologiaRepository : IConsultasPodologia
    {
        private readonly Context _context;

        public ConsultasPodologiaRepository(Context context)
        {
            _context = context;
        }

        public void Add(ConsultasPodologia consulta, bool saveChanges = true)
        {
            _context.Set<ConsultasPodologia>().Add(consulta);
            if (saveChanges) _context.SaveChanges();
        }

        public long AddConsulta(ConsultasPodologia consulta)
        {
            _context.Set<ConsultasPodologia>().Add(consulta);
            _context.SaveChanges();
            return consulta.Id;
        }

        public void Update(ConsultasPodologia consulta, bool saveChanges = true)
        {
            _context.Entry(consulta).State = EntityState.Modified;
            if (saveChanges) _context.SaveChanges();
        }

        public ConsultasPodologia GetConsulta(long id)
        {
            return _context.Set<ConsultasPodologia>()
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);
        }

        public ConsultasPodologia GetConsulta(int consultaId)
        {
            return _context.Set<ConsultasPodologia>()
                .AsNoTracking()
                .FirstOrDefault(c => c.ConsultaId == consultaId);
        }

        public ConsultasPodologia GetConsultaByPaciente(int pacienteId)
        {
            return _context.Set<ConsultasPodologia>()
                .AsNoTracking()
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.Fecha)   // última por Fecha
                .FirstOrDefault();
        }

        public IEnumerable<ConsultasPodologia> GetConsultasByPaciente(int pacienteId)
        {
            return _context.Set<ConsultasPodologia>()
                .AsNoTracking()
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.Fecha)
                .ToList();
        }


        public IList<ConsultasPodologia> ListaConsultas()
        {
            return _context.Set<ConsultasPodologia>()
                .AsNoTracking()
                .ToList();
        }
    }
}
