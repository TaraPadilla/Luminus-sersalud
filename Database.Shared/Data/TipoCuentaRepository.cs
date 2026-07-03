using Database.Shared.IRepository;
using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class TipoCuentaRepository : ITipoCuenta
    {
        private readonly Context _context;

        public TipoCuentaRepository(Context context)
        {
            _context = context;
        }

        public List<TipoCuenta> GetAll()
        {
            return _context.TipoCuenta.ToList();
        }
    }
}
