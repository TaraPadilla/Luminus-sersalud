using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;


namespace Database.Shared.Data
{
    public class SucursalRepository : ISucursal
    {
        private readonly Context _context = null;

        public SucursalRepository(Context context)
        {
            _context = context;
        }
        public void Add(Sucursal sucursal)
        {
            _context.Sucursales.Add(sucursal);

            _context.SaveChanges();
        }
        public List<Sucursal> GetList()
        {
            return _context.Sucursales
               .Where(a => !a.Eliminado)
               .ToList();
        }

        public Sucursal Get(int id)
        {
            return _context.Sucursales
               .Where(a => a.Id == id)
               .FirstOrDefault();
        }
        public void Update(Sucursal sucursal)
        {

            _context.Entry(sucursal).State = EntityState.Modified;

            _context.SaveChanges();
        }
        public void Delete(int sucursalId)
        {
            var sucursal = _context.Sucursales.Where(a => a.Id == sucursalId).FirstOrDefault();
            sucursal.Eliminado = true;

            _context.Entry(sucursal).State = EntityState.Modified;

            _context.SaveChanges();
        }
    }
}