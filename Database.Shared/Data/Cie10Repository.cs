using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Database.Shared.IRepository;
using Database.Shared.Models;

namespace Database.Shared.Data
{
    public class Cie10Repository : ICie10
    {
        private readonly Context _context;

        public Cie10Repository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cie10>> GetAll()
        {
            return await _context.Set<Cie10>().ToListAsync();
        }

        public async Task<Cie10> GetByID(string codigo)
        {
            return await _context.Set<Cie10>().FirstOrDefaultAsync(c => c.codigo == codigo);
        }
    }
}