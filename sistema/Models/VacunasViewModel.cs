using Database.Shared.Models;
using Database.Shared.Paginacion;

namespace sistema.Models
{
    public class VacunasViewModel
    {
        public PaginacionList<Vacuna> nombreVacunas { get; set; }
        public string buscar { get; set; }
        public string currentFilter { get; set; }
        public int? pageNumber { get; set; }
    }
}
