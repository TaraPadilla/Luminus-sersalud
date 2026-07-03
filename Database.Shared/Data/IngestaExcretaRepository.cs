using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class IngestaExcretaRepository : IIngestaExcreta
    {
        private readonly Context _context;

        public IngestaExcretaRepository(Context context)
        {
            _context = context;
        }

        public void Add(IngestaExcreta2 entity)
        {

            _context.IngestaExcreta2.Add(entity);
            _context.SaveChanges();

        }

        public List<IngestaExcreta2> GetListByHospitalizacionId(int hospitalizacionId)
        {
            return _context.IngestaExcreta2
                .Include(x => x.User)
                    .ThenInclude(x => x.Persona)
                .Where(x => x.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(x => x.FechaRegistro) // Ordenar por fecha de registro de forma descendente
                .ToList();
        }


        public IngestaExcreta2 Get(int id)
        {
            return _context.IngestaExcreta2
                .FirstOrDefault(x => x.Id == id);
        }

        public void Update(IngestaExcreta2 entity)
        {
            _context.IngestaExcreta2.Update(entity);
            _context.SaveChanges();
        }
    }
}
