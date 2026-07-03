using System;

namespace Database.Shared.Models
{
    public class CategoriaReceta
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaHoraCreada { get; set; }
    }
}
