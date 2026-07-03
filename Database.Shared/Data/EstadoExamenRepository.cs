using Database.Shared.IRepository;
using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class EstadoExamenRepository : IEstadoExamen
    {
        private readonly Context _context = null;

        public EstadoExamenRepository(Context context)
        {

            _context = context;

        }
        public List<EstadoExamen> GetAll()
        {
            return _context.EstadoExamenes.OrderByDescending(x => x.Nombre).ToList();
        }
    }
}
