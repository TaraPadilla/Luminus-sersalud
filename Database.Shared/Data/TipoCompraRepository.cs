using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class TipoCompraRepository : ITipoCompra
    {
        private readonly Context _context = null;
        public TipoCompraRepository(Context context)
        {
            _context = context;
        }
        public List<TipoCompra> GetList()
        {
            return _context.TipoCompra.ToList();
        }

        public List<TipoCompra> GetListByProveedorId(int proveedorId)
        {
            var data = _context.TipoCompra
                .Include(x => x.TipoCompraProveedores).ToList();

            var datos = ((data.SelectMany(x => x.TipoCompraProveedores)).Where(x => x.ProveedorId == proveedorId)).Select(x => x.TipoCompra).ToList();
            return datos;

        }
    }
}
