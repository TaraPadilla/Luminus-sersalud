using System;

namespace Database.Shared.Models
{
    public class RecetaMenuRelacion
    {
        public int RecetaId { get; set; }
        public int MenuId { get; set; }
        public DateTime FechaHoraCreada { get; set; }
    }
}
