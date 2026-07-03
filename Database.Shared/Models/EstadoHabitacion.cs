using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class EstadoHabitacion
    {
        public EstadoHabitacion()
        {
            Habitaciones = new List<Habitacion>();
        }
        public int Id { get; set; }
        public string NombreEstado { get; set; }
        public ICollection<Habitacion> Habitaciones { get; set; }
    }
}
