using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface ISucursal
    {
        public void Add(Sucursal sucursal);
        public List<Sucursal> GetList();
        public void Update(Sucursal sucursal);
        public Sucursal Get(int id);
        public void Delete(int sucursalId);
    }
}