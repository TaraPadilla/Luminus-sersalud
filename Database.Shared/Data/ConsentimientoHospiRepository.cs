using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Database.Shared.Data
{
    public class ConsentimientoHospiRepository : IConsentimientoHospi 
    {
        private readonly Context _context;

        public ConsentimientoHospiRepository(Context context)
        {
            _context = context;
        }

        public void AddConsentimiento(ConsentimientoHospi consentimiento)
        {
            _context.ConsentimientoHospi.Add(consentimiento);
            _context.SaveChanges();
        }

        public ConsentimientoHospi GetConsentimientoByPacienteAndHabitacion(int pacienteId, int habitacionId)
        {
            return _context.ConsentimientoHospi
                .Include(c => c.ContactosEmergencia)
                .FirstOrDefault(c => c.PacienteId   == pacienteId
                                  && c.HabitacionId == habitacionId);
        }

        public ConsentimientoHospi GetConsentimientoByPacienteHabitacionAndHospitalizacion(
            int pacienteId, int habitacionId, string hospitalizacionId)
        {
            return _context.ConsentimientoHospi
                .FirstOrDefault(c => c.PacienteId        == pacienteId
                                  && c.HabitacionId      == habitacionId
                                  && c.HospitalizacionId == hospitalizacionId);
        }

        public bool UpdateHospitalizacionId(int pacienteId, int habitacionId, string newHospitalizacionId)
        {
            var consentimiento = _context.ConsentimientoHospi
                .FirstOrDefault(c => c.PacienteId   == pacienteId
                                  && c.HabitacionId == habitacionId);

            if (consentimiento == null) return false;

            consentimiento.HospitalizacionId = newHospitalizacionId;
            _context.SaveChanges();
            return true;
        }

        public void UpdateConsentimiento(ConsentimientoHospi consentimiento)
        {
            var existente = _context.ConsentimientoHospi.Find(consentimiento.Id);
            if (existente == null)
                throw new System.Exception(
                    $"ConsentimientoHospi con Id {consentimiento.Id} no encontrado.");

            if (!string.IsNullOrEmpty(consentimiento.URLFirmaPaciente))
                existente.URLFirmaPaciente = consentimiento.URLFirmaPaciente;

            if (!string.IsNullOrEmpty(consentimiento.URLFirmaResponsable))
                existente.URLFirmaResponsable = consentimiento.URLFirmaResponsable;

            // if (!string.IsNullOrEmpty(consentimiento.URLFirmaNotaria))
            //     existente.URLFirmaNotaria = consentimiento.URLFirmaNotaria;

            // if (!string.IsNullOrEmpty(consentimiento.URLFirmaRepresentanteNaranjo))
            //     existente.URLFirmaRepresentanteNaranjo = consentimiento.URLFirmaRepresentanteNaranjo;

            // if (!string.IsNullOrEmpty(consentimiento.HospitalizacionId))
            //     existente.HospitalizacionId = consentimiento.HospitalizacionId;

            _context.SaveChanges();
        }
    }
}