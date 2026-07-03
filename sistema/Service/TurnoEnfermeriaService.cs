using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace farmamest.Service
{
    public class TurnoEnfermeriaService : ITurnoEnfermeriaService
    {
        private readonly ITurnoEnfermeria _turnoEnfermeria;

        public TurnoEnfermeriaService(ITurnoEnfermeria turnoEnfermeria)
        {
            _turnoEnfermeria = turnoEnfermeria;
        }

        // Método para añadir un turno de enfermería
        public void AddTurnoEnfermeria(TurnoEnfermeria entity)
        {
            _turnoEnfermeria.AddTurnoEnfermeria(entity);
        }

        // Método para obtener todos los turnos de enfermería
        public List<TurnoEnfermeriaVM> GetTurnoEnfermeriaList()
        {
            var data = _turnoEnfermeria.GetTurnoEnfermeriaList();

            var result = data.Select(turno => new TurnoEnfermeriaVM
            {
                Id = turno.Id,
                FechaRegistro = turno.FechaRegistro.ToString("dd/MM/yyyy HH:mm"), // Formato de fecha con hora
                NumeroTurno = turno.NumeroTurno,
                NombreTurno = turno.NombreTurno,
                HospitalizacionId = turno.HospitalizacionId,
                Firmado = turno.Firmado,
                UserId = turno.UserId,
                Profesional = turno.User?.Persona != null ? $"{turno.User.Persona.NombreYApellidos}" : "-"
            }).ToList();

            return result;
        }

        // Método para obtener los turnos por hospitalización
        public List<TurnoEnfermeriaVM> GetTurnosByHospitalizacionId(int hospitalizacionId)
        {
            var data = _turnoEnfermeria.GetTurnosByHospitalizacionId(hospitalizacionId);

            var result = data.Select(turno => new TurnoEnfermeriaVM
            {
                Id = turno.Id,
                FechaRegistro = turno.FechaRegistro.ToString("dd/MM/yyyy HH:mm"), // Formato de fecha con hora
                NumeroTurno = turno.NumeroTurno,
                NombreTurno = turno.NombreTurno,
                HospitalizacionId = turno.HospitalizacionId,
                Firmado = turno.Firmado,
                UserId = turno.UserId,
                Profesional = turno.User?.Persona != null ? $"{turno.User.Persona.NombreYApellidos}" : "-"
            }).ToList();

            return result;
        }

        // Método para marcar un turno como firmado
        public void MarkTurnoAsFirmado(int turnoId)
        {
            _turnoEnfermeria.MarkTurnoAsFirmado(turnoId);
        }
    }
}
