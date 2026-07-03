using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class NotaEnfermeria2Repository : INotaEnfermeria2
    {
        private readonly Context _context;

        public NotaEnfermeria2Repository(Context context)
        {
            _context = context;
        }

        public void AddNotaEnfermeria(NotaEnfermeria2 entity)
        {

            _context.NotaEnfermeria2.Add(entity);
            _context.SaveChanges();

        }

        public List<NotaEnfermeria2> GetNotaEnfermeriaListByHospitalizacionId(int hospitalizacionId)
        {
            return _context.NotaEnfermeria2
                .Include(x => x.User)
                    .ThenInclude(x => x.Persona)
                .Where(x => x.HospitalizacionId == hospitalizacionId)
                .OrderBy(x => x.FechaRegistro) // Ordenar por fecha de registro de forma descendente
                .ToList();
        }

    }
}
