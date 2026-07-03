using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public class ProductoEquivalenciaRepository : IProductoEquivalencia
    {
        private readonly Context _context = null;
        public ProductoEquivalenciaRepository(Context context)
        {
            _context = context;
        }

        public List<ProductoEquivalencia> GetAll()
        {
            return _context.ProductosEquivalencias
                .Include(x => x.UnidadMedidaVenta)
                .Include(x => x.Producto)
                .Where(x => !x.Eliminada).ToList();
        }

        public List<ProductoEquivalencia> GetByIdProducto(int productoId)
        {
            return _context.ProductosEquivalencias.Where(x => !x.Eliminada && x.ProductoId == productoId).ToList();
        }
    }
}
