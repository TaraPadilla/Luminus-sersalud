using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
                .Where(c => c.PacienteId == pacienteId
                         && c.HabitacionId == habitacionId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();
        }

        public ConsentimientoHospi GetConsentimientoByPacienteHabitacionAndHospitalizacion(
            int pacienteId, int habitacionId, string hospitalizacionId)
        {
            var exact = _context.ConsentimientoHospi
                .Include(c => c.ContactosEmergencia)
                .FirstOrDefault(c => c.PacienteId == pacienteId
                                  && c.HabitacionId == habitacionId
                                  && c.HospitalizacionId == hospitalizacionId);
            if (exact != null)
                return exact;

            return _context.ConsentimientoHospi
                .Include(c => c.ContactosEmergencia)
                .Where(c => c.PacienteId == pacienteId
                         && c.HabitacionId == habitacionId
                         && (c.HospitalizacionId == null || c.HospitalizacionId == ""))
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();
        }

        public ConsentimientoHospi GetConsentimientoByPacienteAndHospitalizacion(int pacienteId, string hospitalizacionId)
        {
            return _context.ConsentimientoHospi
                .Include(c => c.ContactosEmergencia)
                .Where(c => c.PacienteId == pacienteId && c.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();
        }

        public ConsentimientoHospi GetLatestConsentimientoByPaciente(int pacienteId)
        {
            return _context.ConsentimientoHospi
                .Include(c => c.ContactosEmergencia)
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();
        }

        public bool UpdateHospitalizacionId(int pacienteId, int habitacionId, string newHospitalizacionId)
        {
            var consentimiento = _context.ConsentimientoHospi
                .Where(c => c.PacienteId == pacienteId
                         && c.HabitacionId == habitacionId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

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

            _context.SaveChanges();
        }

        public void UpdateFirmas(int pacienteId, int habitacionId, string urlFirmaPaciente, string urlFirmaResponsable)
        {
            var existente = _context.ConsentimientoHospi
                .Where(c => c.PacienteId == pacienteId && c.HabitacionId == habitacionId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            if (existente == null)
                throw new System.InvalidOperationException(
                    $"No existe consentimiento para paciente {pacienteId} y habitación {habitacionId}.");

            if (!string.IsNullOrEmpty(urlFirmaPaciente))
                existente.URLFirmaPaciente = urlFirmaPaciente;

            if (!string.IsNullOrEmpty(urlFirmaResponsable))
                existente.URLFirmaResponsable = urlFirmaResponsable;

            _context.SaveChanges();
        }

        public void UpsertConsentimiento(ConsentimientoHospi consentimiento)
        {
            if (consentimiento.ContactosEmergencia != null)
            {
                consentimiento.ContactosEmergencia = consentimiento.ContactosEmergencia
                    .Where(c => c != null && !string.IsNullOrWhiteSpace(c.Nombre))
                    .ToList();
            }

            var existente = _context.ConsentimientoHospi
                .Include(c => c.ContactosEmergencia)
                .Where(c => c.PacienteId == consentimiento.PacienteId
                         && c.HabitacionId == consentimiento.HabitacionId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            if (existente == null)
            {
                if (consentimiento.ContactosEmergencia == null)
                    consentimiento.ContactosEmergencia = new List<ContactoEmergencia>();

                _context.ConsentimientoHospi.Add(consentimiento);
                _context.SaveChanges();
                return;
            }

            consentimiento.Id = existente.Id;
            consentimiento.PacienteId = existente.PacienteId;
            consentimiento.HabitacionId = existente.HabitacionId;
            _context.Entry(existente).CurrentValues.SetValues(consentimiento);
            _context.Entry(existente).Property(c => c.Id).IsModified = false;

            if (consentimiento.ContactosEmergencia != null && consentimiento.ContactosEmergencia.Any())
            {
                existente.ContactosEmergencia.Clear();

                foreach (var contacto in consentimiento.ContactosEmergencia.Where(c => !string.IsNullOrWhiteSpace(c.Nombre)))
                {
                    existente.ContactosEmergencia.Add(new ContactoEmergencia
                    {
                        Nombre = contacto.Nombre,
                        Telefono = contacto.Telefono,
                        Parentesco = contacto.Parentesco
                    });
                }
            }

            _context.SaveChanges();
        }
    }
}