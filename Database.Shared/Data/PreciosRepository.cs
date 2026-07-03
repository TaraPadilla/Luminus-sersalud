using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;

namespace Database.Shared.Data
{
    public class PreciosRepository : IPrecios
    {
        private readonly Context _context = null;

        public PreciosRepository(Context context)
        {
            _context = context;
        }

        public IList<Precio> GetList()
        {
            return _context.Precios
            .Where(a => a.Eliminado == false)
            .OrderBy(a => a.Id)
            .ToList();
        }
        public IList<ExamenLabClinicoPrecio> GetPreciosExamenLabClinico(int examenLabClinicoId)
        {
            return _context.ExamenLabClinicosPrecios
                .Include(a => a.Precio)
                .Where(a => a.ExamenLabClinicoId == examenLabClinicoId)
                .ToList();
        }

        public void Add(Precio precio)
        {
            _context.Precios.Add(precio);
            _context.SaveChanges();
        }

        public Precio Get(int precioId)
        {
            return _context.Precios
            .Where(a => a.Id == precioId)
            .SingleOrDefault();
        }
        public Precio GetByName(string nombrePrecio)
        {
            return _context.Precios
                .FirstOrDefault(p => p.NombrePrecio == nombrePrecio && !p.Eliminado);
        }

        public void Update(Precio precio)
        {
            _context.Entry(precio).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public void Delete(int precioId)

        {
            var precio = _context.Precios
                .Where(g => g.Id == precioId)
                .FirstOrDefault();
            precio.Eliminado = true;
            _context.SaveChanges();
        }
        public decimal? ObtenerPrecioPorSeguro(string tipo, string nombre, string seguro)
        {
            var precioSeguro = _context.Precios
                .Where(p => p.NombrePrecio == seguro && !p.Eliminado)
                .Select(p => p.Id)
                .FirstOrDefault();

            if (precioSeguro == 0)
                return null;

            if (tipo == "Producto")
            {
                return _context.ProductosInventarioPrecios
                    .Include(p => p.ProductoInventario)
                    .Where(p => p.ProductoInventario.Producto.NombreProducto == nombre && p.PrecioId == precioSeguro)
                    .Select(p => p.Valor)
                    .FirstOrDefault();
            }
            else if (tipo == "Servicio")
            {
                return _context.ServiciosPrecios
                    .Include(s => s.Servicio)
                    .Where(s => s.Servicio.NombreServicio == nombre && s.PrecioId == precioSeguro)
                    .Select(s => s.Valor)
                    .FirstOrDefault();
            }
            else if (tipo == "Examen")
            {
                return _context.ExamenLabClinicosPrecios
                    .Include(e => e.ExamenLabClinico)
                    .Where(e => e.ExamenLabClinico.NombreExamen == nombre && e.PrecioId == precioSeguro)
                    .Select(e => e.PrecioValor)
                    .FirstOrDefault();
            }

            return null;
        }
    }
}
