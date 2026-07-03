using Database.Shared.IRepository;
using Database.Shared.Models;
using Database.Shared.Paginacion;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
	public class VacunasRepository : IVacunas
    {
        private readonly Context _context = null;

        public VacunasRepository(Context context)
        {
            _context = context;
        }

        //Metodo creado para listar la paginacion de las vacinas
        public PaginacionList<Vacuna> PaginacionVacunas(string sortOrder, string searchString, int? pageNumber, int pageSize)
        {
            var vacunas = _context.Vacunas
            .AsQueryable();


            // para hacer la busqueda, el IsNullOrEmpty verifica que no este vacia la cadena entrante
            if (!string.IsNullOrEmpty(searchString))
            {
               vacunas = vacunas
                .Where(s => s.Nombre.Contains(searchString));
            }

            return PaginacionList<Vacuna>.CreateAsyncc(vacunas
            .Where(x => x.Eliminado == false)
            .OrderBy(a => a.Id),
            pageNumber ?? 1, pageSize);
        }

        //Metodo creado para obtener una vacuna pos su id
        public Vacuna GetVacuna(int id, bool includeRelatedEntities = true)
        {
            return _context.Vacunas
                .Where(a => a.Id == id).FirstOrDefault();
        }


        public void Update(Vacuna vacuna, bool saveChanges = true)
        {
            _context.Entry(vacuna).State = EntityState.Modified;

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        public void Add(Vacuna vacuna, bool saveChanges = true)
        {
            _context.Vacunas.Add(vacuna);

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }


    }
}
