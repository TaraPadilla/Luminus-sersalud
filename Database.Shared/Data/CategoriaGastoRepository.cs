using System.Collections.Generic;
using System.Linq;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;

namespace Database.Shared.Data
{
    public class CategoriaGastoRepository : ICategoriaGasto
    {
        private readonly Context _context = null;
        public CategoriaGastoRepository(Context context)
        {
            _context = context;
        }

        public List<CategoriaGasto> ListarCategorias()
        {
            return _context.CategoriasGastos
            .Include(a => a.Gastos)
            .OrderBy(a => a.NombreCategoria).Where(x=>x.Eliminado==false).ToList();
        }

        public void Add(CategoriaGasto categoria, bool saveChanges = true)
        {
            _context.CategoriasGastos.Add(categoria);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Update(CategoriaGasto categoria, bool saveChanges = true)
        {

            _context.Entry(categoria).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }

        }

         public PaginacionList<CategoriaGasto> PaginacionCategoria(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var categoria = _context.CategoriasGastos.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if(!string.IsNullOrEmpty(searchString))
            {
                categoria = categoria.Where(s => s.NombreCategoria.Contains(searchString));
            }

            // esto que hice no es buena practica, lo ideal seria hacer un patron por estrategia
            // visitar : https://refactoring.guru/es/design-patterns/strategy
            // asi como lo tengo funciona pero no es tan tan tan recomendado
            // quizas mas adelante lo mejoremos con un patron de estrategia.

            switch(sortOrder)
            {
                case "Nombre_desc":
                categoria = categoria.OrderByDescending(s => s.NombreCategoria);
                break;

                default:
                categoria = categoria.OrderBy(s => s.NombreCategoria);
                break;
            }

            return PaginacionList<CategoriaGasto>.CreateAsyncc(categoria.Where(s=>s.Eliminado==false), pageNumber ?? 1, pageSize);
        }

        public CategoriaGasto Get(int id, bool includeRelatedEntities = true)
        {
            return _context.CategoriasGastos.Where(a => a.Id == id) .SingleOrDefault();
        }

        

    }
}