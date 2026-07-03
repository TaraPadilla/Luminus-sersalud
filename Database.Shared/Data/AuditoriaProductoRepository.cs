using Database.Shared.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Data
{
    public class AuditoriaProductoRepository : IAuditoriaProducto
    {
        private readonly Context _context = null;

        public AuditoriaProductoRepository(Context context)
        {
            _context = context;
        }
    }
}
