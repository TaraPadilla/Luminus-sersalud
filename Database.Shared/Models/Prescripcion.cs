using System;
using Database.Shared.Models;
using System.Collections.Generic;

namespace Database.Shared.Models
{
    public class Prescripcion
    {
        public Prescripcion()
        {
            DetallePrescripcion = new List<DetallePrescripcion>();
        }
        public int Id { get; set; }
        public int? ConsultaId { get; set; }
        public Consulta Consulta { get; set; }
        public int? CitasId { get; set; }
        public Citas Citas { get; set; }
        public DateTime NextDate { get; set; }
        public bool Eliminada { get; set; }
        public string Color { get; set; }

        public ICollection<DetallePrescripcion> DetallePrescripcion { get; set; }
    }
}
