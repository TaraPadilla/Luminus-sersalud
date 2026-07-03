using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Database.Shared.Data
{
    public class ConsultasValoracionInicialEnfermeriaRepository : IConsultasValoracionInicialEnfermeria
    {
        private readonly Context _context;

        public ConsultasValoracionInicialEnfermeriaRepository(Context context)
        {
            _context = context;
        }

        public void Add(ConsultasValoracionInicialEnfermeria consulta, bool saveChanges = true)
        {
            _context.Set<ConsultasValoracionInicialEnfermeria>().Add(consulta);
            if (saveChanges) _context.SaveChanges();
        }

        public long AddConsulta(ConsultasValoracionInicialEnfermeria consulta)
        {
            _context.Set<ConsultasValoracionInicialEnfermeria>().Add(consulta);
            _context.SaveChanges();
            return consulta.Id;
        }

        public void Update(ConsultasValoracionInicialEnfermeria consulta, bool saveChanges = true)
        {
            _context.Entry(consulta).State = EntityState.Modified;
            if (saveChanges) _context.SaveChanges();
        }

        public ConsultasValoracionInicialEnfermeria GetConsulta(long id)
        {
            return _context.Set<ConsultasValoracionInicialEnfermeria>()
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);
        }

        public ConsultasValoracionInicialEnfermeria GetConsulta(int consultaId)
        {
            return _context.Set<ConsultasValoracionInicialEnfermeria>()
                .AsNoTracking()
                .FirstOrDefault(c => c.ConsultaId == consultaId);
        }

        public ConsultasValoracionInicialEnfermeria GetConsultaByPaciente(int pacienteId)
        {
            return _context.Set<ConsultasValoracionInicialEnfermeria>()
                .AsNoTracking()
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.Fecha)   // última por Fecha
                .FirstOrDefault();
        }

        public IEnumerable<ConsultasValoracionInicialEnfermeria> GetConsultasByPaciente(int pacienteId)
        {
            return _context.Set<ConsultasValoracionInicialEnfermeria>()
                .AsNoTracking()
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.Fecha)
                .ToList();
        }

        public IList<ConsultasValoracionInicialEnfermeria> ListaConsultas()
        {
            return _context.Set<ConsultasValoracionInicialEnfermeria>()
                .AsNoTracking()
                .ToList();
        }
    }
}
