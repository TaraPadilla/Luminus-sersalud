using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace sistema.Models
{
    public class RecetaMenuCrudViewModel
    {
        public int Id { get; set; }

        public int? CategoriaId { get; set; }
        public string Nombre { get; set; }
        public string Ingredientes { get; set; }

        // Persistido en BD (int flags)
        public int DiasSemana { get; set; }

        // Para binding de checkboxes
        public List<int> DiasSeleccionados { get; set; } = new List<int>();

        // Catálogo
        public List<SelectListItem> Categorias { get; set; } = new List<SelectListItem>();
    }
}
