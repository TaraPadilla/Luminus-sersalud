using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface ICategoriaGeneralLabClinico
    {
        public List<CategoriaGeneralLabClinico> GetListCategoriasGeneralesLabClinico();

        public void Add(CategoriaGeneralLabClinico categoriaGeneralLabClinico, bool saveChanges = true);

        public CategoriaGeneralLabClinico GetCategoriaGeneralLab(int id, bool includeRelatedEntities = true);

        public void Update(CategoriaGeneralLabClinico categoriaGeneralLabClinico, bool saveChanges = true);
    }
}
