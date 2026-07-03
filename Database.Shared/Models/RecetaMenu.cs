using System;

namespace Database.Shared.Models
{
    public class RecetaMenu
    {
        public int Id { get; set; }
        public int CategoriaId { get; set; }
        public string Nombre { get; set; }
        public string Ingredientes { get; set; }
        public int DiasSemana { get; set; } // Flags
        public bool Eliminado { get; set; }
        public DateTime FechaHoraCreada { get; set; }
    }
}
