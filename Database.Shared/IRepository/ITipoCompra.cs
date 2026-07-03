using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface ITipoCompra
    {
        public List<TipoCompra> GetList();
        public List<TipoCompra> GetListByProveedorId(int proveedorId);

    }
}
