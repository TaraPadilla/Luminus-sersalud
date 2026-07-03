using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class NotaMedica2Repository : INotaMedica2
    {
        private readonly Context _context;

        public NotaMedica2Repository(Context context)
        {
            _context = context;
        }

        public void Add(NotaMedica2 entity)
        {
            _context.NotaMedica2.Add(entity);
            _context.SaveChanges();
        }

        public List<NotaMedica2> GetAllByIdHospitalizacion(int idHospitalizacion)
        {
            var data = _context.NotaMedica2
                .Include(x => x.Hospitalizacion)
                .Include(x => x.Profesional)
                    .ThenInclude(x => x.Persona)

                .Where(x => x.HospitalizacionId == idHospitalizacion)
                                .OrderBy(x => x.FechaRegistro) // Ordenar por fecha de registro de forma descendente
                .ToList();
            return data;
        }


        public NotaMedica2 GetById(int id)
        {
            return _context.NotaMedica2.FirstOrDefault(x => x.Id == id);
        }

        public void Update(NotaMedica2 entity)
        {
            _context.NotaMedica2.Update(entity);
            _context.SaveChanges();
        }
    }
}
