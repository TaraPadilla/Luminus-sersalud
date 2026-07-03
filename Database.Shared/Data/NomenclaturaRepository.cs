using Database.Shared.IRepository;
using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class NomenclaturaRepository : INomenclatura
    {
        private readonly Context _context;

        public NomenclaturaRepository(Context context)
        {
            _context = context;
        }

        public List<Nomenclatura> GetAll()
        {
            return _context.Nomenclaturas.ToList();
        }
    }
}
