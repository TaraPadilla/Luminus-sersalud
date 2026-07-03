using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;
using System;


namespace Database.Shared.Data
{
    public class GastoRepository : IGasto
    {

        private readonly Context _context = null;

        public GastoRepository(Context context)
        {
            _context = context;
        }

        public void Add(Gasto gasto, bool saveChanges = true)
        {
            _context.Gastos.Add(gasto);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public List<Gasto> GetList() => _context.Gastos.Include(a => a.CategoriaGasto).Where(x=>x.Eliminado==false).ToList();

        public IList<Gasto> GetListado() => _context.Gastos.Where(x=>x.Eliminado==false).ToList();


        // SELECT *FROM PRODUCTOS WHERE ID = <id>
        public Gasto Get(int id, bool includeRelatedEntities = true)
        {
            var gastos = _context.Gastos.AsQueryable();

            if (includeRelatedEntities)
            {
                gastos = gastos.Include(a => a.CategoriaGasto);
            }

            return gastos
               .Where(a => a.Id == id)
               .SingleOrDefault();


        }

    public void Update(Gasto model, bool saveChanges = true)
    {

     _context.Entry(model).State = EntityState.Modified;

     if(saveChanges)
     {
     _context.SaveChanges();
     }
        
    }

     public PaginacionList<Gasto> PaginacionGastos(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var gastos = _context.Gastos.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if(!string.IsNullOrEmpty(searchString))
            {
                gastos = gastos.Where(s => s.NombreGasto.Contains(searchString) || s.CategoriaGasto.NombreCategoria.Contains(searchString));
            }

            switch(sortOrder)
            {
                case "Nombre_desc":
                gastos = gastos.OrderByDescending(s => s.NombreGasto);
                break;

                default:
                gastos = gastos.OrderBy(s => s.NombreGasto);
                break;
            }

            return PaginacionList<Gasto>.CreateAsyncc(gastos.Where(a => a.Eliminado == false), pageNumber ?? 1, pageSize);
        }


    public Gasto GetPorNombre(string nombre, bool includeRelatedEntities = true)
    {
        return _context.Gastos.Where(a => a.NombreGasto == nombre).Where(a => a.Eliminado == false).SingleOrDefault();
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    
}
}