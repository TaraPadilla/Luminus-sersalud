using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Database.Shared.Data
{
    public class ConsultasSueroterapiaRepository : IConsultasSueroterapia
    {
        private readonly Context _context;

        public ConsultasSueroterapiaRepository(Context context)
        {
            _context = context;
        }

        public void Add(ConsultasSueroterapia consulta, bool saveChanges = true)
        {
            _context.Set<ConsultasSueroterapia>().Add(consulta);
            if (saveChanges) _context.SaveChanges();
        }

        public long AddConsulta(ConsultasSueroterapia consulta)
        {
            _context.Set<ConsultasSueroterapia>().Add(consulta);
            _context.SaveChanges();
            return consulta.Id;
        }

        public void Update(ConsultasSueroterapia consulta, bool saveChanges = true)
        {
            _context.Entry(consulta).State = EntityState.Modified;
            if (saveChanges) _context.SaveChanges();
        }

        public ConsultasSueroterapia GetConsulta(long id)
        {
            return _context.Set<ConsultasSueroterapia>()
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);
        }

        public ConsultasSueroterapia GetConsulta(int consultaId)
        {
            return _context.Set<ConsultasSueroterapia>()
                .AsNoTracking()
                .FirstOrDefault(c => c.ConsultaId == consultaId);
        }

        public ConsultasSueroterapia GetConsultaByPaciente(int pacienteId)
        {
            return _context.Set<ConsultasSueroterapia>()
                .AsNoTracking()
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.Fecha)   // última por Fecha
                .FirstOrDefault();
        }

        public IEnumerable<ConsultasSueroterapia> GetConsultasByPaciente(int pacienteId)
        {
            return _context.Set<ConsultasSueroterapia>()
                .AsNoTracking()
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.Fecha)
                .ToList();
        }

        public IList<ConsultasSueroterapia> ListaConsultas()
        {
            return _context.Set<ConsultasSueroterapia>()
                .AsNoTracking()
                .ToList();
        }
    }
}
