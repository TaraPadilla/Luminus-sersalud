using Database.Shared.Models;
using Database.Shared.IRepository;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Database.Shared.Paginacion;


namespace Database.Shared.Data
{
    public class RutaRepository : IRuta
    {

        private readonly Context _context = null;

        public RutaRepository(Context context)
        {
            _context = context;
        }

        public void Add(Ruta ruta, bool saveChanges = true)
        {
            _context.Rutas.Add(ruta);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }
        public List<Ruta> GetList() => _context.Rutas.Where(x => x.Eliminado == false).ToList();



        // SELECT *FROM PRODUCTOS WHERE ID = <id>
        public Ruta Get(int id, bool includeRelatedEntities = true)
        {
            var rutas = _context.Rutas.AsQueryable();


            return rutas
               .Where(a => a.Id == id)
               .SingleOrDefault();


        }

        public PaginacionList<Ruta> PaginacionRutas(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var rutas = _context.Rutas.AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if (!string.IsNullOrEmpty(searchString))
            {
                rutas = rutas.Where(s => s.Id.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
    
                default:
                    rutas = rutas.OrderBy(s => s.Id);
                    break;
            }

            return PaginacionList<Ruta>.CreateAsyncc(rutas.Where(a => a.Eliminado == false), pageNumber ?? 1, pageSize);
        }

        public void Update(Ruta model, bool saveChanges = true)
        {

            _context.Entry(model).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }

        }


        public void SaveChanges()
        {
            _context.SaveChanges();
        }


    }
}