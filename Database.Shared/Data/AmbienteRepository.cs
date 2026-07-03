
using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;

namespace Database.Shared.Data
{
    public class AmbienteRepository : IAmbiente
    {
        private readonly Context _context = null;
        public AmbienteRepository(Context context)
        {
            _context = context;
        }
        public List<Ambiente> GetList()
        {
            return _context.Ambientes.ToList();
        }
    }
}