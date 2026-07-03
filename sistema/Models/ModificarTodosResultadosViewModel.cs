using Database.Shared.Models;
using sistema.Models;
using System.Collections.Generic;

namespace sistema.Models
{
    public class ModificarTodosResultadosViewModel
    {
        public int? ExamenId { get; set; }

        public List<DetalleExamen> DetalleExamenenes { get; set; }
        public List<Resultados> DatosResultados { get; set; }

    }
}
