using Database.Shared.IRepository;
using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class FormasPagoRepository : IFormasPago
    {
        private readonly Context _context = null;
        public FormasPagoRepository(Context context)
        {
            _context = context;
        }

        public List<FormaPago> GetAll()
        {
            return _context.FormaPagos.ToList();
        }

    }
}
