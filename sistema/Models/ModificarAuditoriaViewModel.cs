using System.Collections.Generic;

namespace farmamest.Models
{
    public class ModificarAuditoriaViewModel
    {
        public int AuditoriaId { get; set; }
        public Dictionary<int, int> Productos { get; set; }
    }
}
