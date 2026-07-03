using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class NotaEvolucionRepository : INotaEvolucion
    {
        private readonly Context _context;

        public NotaEvolucionRepository(Context context)
        {
            _context = context;
        }

        public void AddNotaEvolucion(NotaEvolucion entity)
        {

            _context.NotaEvolucion.Add(entity);
            _context.SaveChanges();

        }

        public List<NotaEvolucion> GetNotaEvolucionListByHospitalizacionId(int hospitalizacionId)
        {
            return _context.NotaEvolucion
                .Include(x => x.User)
                    .ThenInclude(x => x.Persona)
                .Where(x => x.HospitalizacionId == hospitalizacionId)
                .OrderBy(x => x.FechaRegistro) // Ordenar por fecha de registro de forma descendente
                .ToList();
        }
    }
}
