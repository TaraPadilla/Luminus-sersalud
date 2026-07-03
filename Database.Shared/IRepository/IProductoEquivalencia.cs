using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface IProductoEquivalencia
    {
        public List<ProductoEquivalencia> GetAll();
        public List<ProductoEquivalencia> GetByIdProducto(int productoId);
    }
}
