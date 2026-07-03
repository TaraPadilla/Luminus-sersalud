using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace sistema.Models
{
    public class RecetaViewModel
    {
        public int Id { get; set; }

        public string NombreReceta { get; set; } = string.Empty;
        public string Ingredientes { get; set; } = string.Empty;

        // OJO: estos campos hoy se usan en hospitalización (controller tiene endpoints JSON).
        public string Cantidad { get; set; } = string.Empty;
        public int CantidadAplicada { get; set; }
        public string Indicaciones { get; set; } = string.Empty;

        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; set; }

        // Categoría REAL de la receta (persistencia). No reemplaza aún al filtro para no romper UI existente.
        public int? CategoriaId { get; set; }

        // UI (filtro para desplegable dependiente de menús; NO necesariamente la relación persistida)
        public int? CategoriaFiltroId { get; set; }
        public int? MenuSeleccionadoId { get; set; }

        // Relación real Receta -> Menús (múltiples)
        public List<int> MenuIds { get; set; } = new List<int>();

        // Catálogos (para render)
        public List<SelectListItem> Categorias { get; set; } = new List<SelectListItem>();
        public List<MenuOptionVm> Menus { get; set; } = new List<MenuOptionVm>();

        public class MenuOptionVm
        {
            public int Id { get; set; }
            public int CategoriaId { get; set; }
            public string Nombre { get; set; } = string.Empty;
        }
    }
}
