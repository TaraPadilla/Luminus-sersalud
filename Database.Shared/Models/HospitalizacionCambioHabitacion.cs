using System;

namespace Database.Shared.Models
{
    public class HospitalizacionCambioHabitacion
    {
        public int Id { get; set; }
        public int HospitalizacionId { get; set; }
        public int HabitacionId { get; set; }
        public string Tarifa { get; set; }
        public decimal ValorTarifa { get; set; }
        public int Dias { get; set; }

        public DateTime FechaCambio { get; set; }

        public virtual Hospitalizacion Hospitalizacion { get; set; }
        public virtual Habitacion Habitacion { get; set; }
    }
}
