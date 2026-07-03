using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class TurnoEnfermeriaRepository : ITurnoEnfermeria
    {
        private readonly Context _context;

        public TurnoEnfermeriaRepository(Context context)
        {
            _context = context;
        }

        // Método para añadir o crear un turno de enfermería
        public void AddTurnoEnfermeria(TurnoEnfermeria entity)
        {
            _context.TurnoEnfermeria.Add(entity); // Añadir el nuevo turno
            _context.SaveChanges(); // Guardar cambios en la base de datos
        }

        // Método para obtener todos los turnos de enfermería
        public List<TurnoEnfermeria> GetTurnoEnfermeriaList()
        {
            return _context.TurnoEnfermeria
                .Include(x => x.Hospitalizacion) // Incluir datos de hospitalización si es necesario
                .Include(x => x.User) // Incluir el usuario (enfermera)
                    .ThenInclude(x => x.Persona) // Incluir persona asociada al usuario
                .ToList(); // Retornar la lista de turnos
        }

        // Método para obtener los turnos de enfermería de un hospital específico
        public List<TurnoEnfermeria> GetTurnosByHospitalizacionId(int hospitalizacionId)
        {
            return _context.TurnoEnfermeria
                .Where(x => x.HospitalizacionId == hospitalizacionId) // Filtrar por hospitalización
                .Include(x => x.Hospitalizacion) // Incluir datos de hospitalización
                .Include(x => x.User) // Incluir usuario
                    .ThenInclude(x => x.Persona) // Incluir la persona asociada al usuario
                .OrderBy(x => x.FechaRegistro) // Ordenar por fecha de registro, de más reciente a más antiguo
                .ToList(); // Retornar la lista de turnos filtrados y ordenados
        }


        // Método para marcar un turno como firmado
        public void MarkTurnoAsFirmado(int turnoId)
        {
            var turno = _context.TurnoEnfermeria.Find(turnoId); // Buscar el turno por su ID

            if (turno != null)
            {
                turno.Firmado = true; // Marcar el turno como firmado
                _context.SaveChanges(); // Guardar cambios en la base de datos
            }
        }
    }
}
