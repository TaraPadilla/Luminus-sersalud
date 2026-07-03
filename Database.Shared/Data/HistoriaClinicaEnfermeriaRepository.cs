using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Database.Shared.Data
{
    public class ConsultasHistoriaClinicaEnfermeriaRepository : IConsultasHistoriaClinicaEnfermeria
    {
        private readonly Context _context;

        public ConsultasHistoriaClinicaEnfermeriaRepository(Context context)
        {
            _context = context;
        }

        public void Add(ConsultasHistoriaClinicaEnfermeria consulta, bool saveChanges = true)
        {
            _context.Set<ConsultasHistoriaClinicaEnfermeria>().Add(consulta);
            if (saveChanges) _context.SaveChanges();
        }

        public long AddConsulta(ConsultasHistoriaClinicaEnfermeria consulta)
        {
            _context.Set<ConsultasHistoriaClinicaEnfermeria>().Add(consulta);
            _context.SaveChanges();
            return consulta.Id; // <- igual que en Oftalmología
        }

        public void Update(ConsultasHistoriaClinicaEnfermeria consulta, bool saveChanges = true)
        {
            _context.Entry(consulta).State = EntityState.Modified;
            if (saveChanges) _context.SaveChanges();
        }

        public ConsultasHistoriaClinicaEnfermeria GetConsulta(long id)
        {
            return _context.Set<ConsultasHistoriaClinicaEnfermeria>()
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);
        }

        public ConsultasHistoriaClinicaEnfermeria GetConsulta(int consultaId)
        {
            return _context.Set<ConsultasHistoriaClinicaEnfermeria>()
                .AsNoTracking()
                .FirstOrDefault(c => c.ConsultaId == consultaId);
        }

        public ConsultasHistoriaClinicaEnfermeria GetConsultaByPaciente(int pacienteId)
        {
            return _context.Set<ConsultasHistoriaClinicaEnfermeria>()
                .AsNoTracking()
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.Fecha)   // última por Fecha
                .FirstOrDefault();
        }

        public IEnumerable<ConsultasHistoriaClinicaEnfermeria> GetConsultasByPaciente(int pacienteId)
        {
            return _context.Set<ConsultasHistoriaClinicaEnfermeria>()
                .AsNoTracking()
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.Fecha)
                .ToList();
        }

        public IList<ConsultasHistoriaClinicaEnfermeria> ListaConsultas()
        {
            return _context.Set<ConsultasHistoriaClinicaEnfermeria>()
                .AsNoTracking()
                .ToList();
        }
    }
}
