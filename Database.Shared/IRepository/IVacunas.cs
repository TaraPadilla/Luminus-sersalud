using Database.Shared.Models;
using Database.Shared.Paginacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface IVacunas
    {
		PaginacionList<Vacuna> PaginacionVacunas(string sortOrder, string searchString, int? pageNumber, int pageSize);
        Vacuna GetVacuna(int id, bool includeRelatedEntities = true);

        void Update(Vacuna vacuna, bool saveChanges = true);

        void Add(Vacuna vacuna, bool saveChanges = true);
    }
}
