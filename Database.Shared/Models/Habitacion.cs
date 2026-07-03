using System;
using System.Collections.Generic;
using System.Text;
using Database.Shared.Models;  


namespace Database.Shared.Models
{
    public class Habitacion
    {
        public Habitacion()
        {
            Hospitalizaciones = new List<Hospitalizacion>();
        }
        public int Id { get; set; }
        public string NombreNumeroHabitacion { get; set; }
        public int CategoriaHabitacionId { get; set; }  
        public CategoriaHabitacion CategoriaHabitacion { get; set; }
        public int EstadoHabitacionId { get; set; }
        public EstadoHabitacion EstadoHabitacion { get; set; }
        public int NumeroCamas { get; set; }
        public int CapacidadPersonas { get; set; }
        public bool Eliminada { get; set; }
        public ICollection<Hospitalizacion> Hospitalizaciones { get; set; }
        public string HabitacionCategoria { get; set; }
        // public EstadoHabitacionEnum EstadoHabitacion { get; set; }

    }
}
