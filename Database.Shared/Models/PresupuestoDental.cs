using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class PresupuestoDental
    {
        public PresupuestoDental()
        {
            PresupuestosDentalesDetalles = new List<PresupuestoDentalDetalle>();
        }
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public ICollection<PresupuestoDentalDetalle> PresupuestosDentalesDetalles { get; set; }
    }
}
