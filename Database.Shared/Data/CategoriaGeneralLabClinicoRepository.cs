using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.Shared.Data
{
    public  class CategoriaGeneralLabClinicoRepository : ICategoriaGeneralLabClinico
    {
        private readonly Context _context = null;
        public CategoriaGeneralLabClinicoRepository(Context context)
        {
            _context = context;
        }

        public List<CategoriaGeneralLabClinico> GetListCategoriasGeneralesLabClinico() {
            return _context.CategoriaGeneralLabClinico
                 .Where(x => x.Eliminado == false)
                 .OrderBy(x => x.Nombre)
                 .ToList();
        
        }

        public void Add(CategoriaGeneralLabClinico categoriaGeneralLabClinico, bool saveChanges = true) {
            _context.CategoriaGeneralLabClinico.Add(categoriaGeneralLabClinico);

            if (saveChanges) {

                _context.SaveChanges();
            }
        }

        public CategoriaGeneralLabClinico GetCategoriaGeneralLab(int id, bool includeRelatedEntities = true)
        {
            return _context.CategoriaGeneralLabClinico.Where(a => a.Id == id).FirstOrDefault();
        }

        public void Update(CategoriaGeneralLabClinico categoriaGeneralLabClinico, bool saveChanges = true)
        {
            EfUpdateHelper.UpdateEntity(_context, categoriaGeneralLabClinico, saveChanges);
        }


    }
}
