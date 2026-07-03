using Database.Shared.IRepository;
using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class TipoProductoRepository : ITipoProducto
    {
        private readonly Context _context = null;

        public TipoProductoRepository(Context context)
        {
            _context = context;
        }
        public TipoProducto Get(int id)
        {
            return _context.TipoProductos.Where(x => x.Id == id).FirstOrDefault();

        }
    }
}
