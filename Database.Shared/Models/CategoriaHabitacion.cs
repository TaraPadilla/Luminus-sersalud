using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class CategoriaHabitacion
    {
        public CategoriaHabitacion()
        {
            CategoriaHabitacionTarifas = new List<CategoriaHabitacionTarifa>();
            Habitaciones = new List<Habitacion>();
        }
        public int Id { get; set; }
        public string NombreCategoria { get; set; }
        public bool Eliminada { get; set; }
        public ICollection<CategoriaHabitacionTarifa> CategoriaHabitacionTarifas { get; set; }
        public ICollection<Habitacion> Habitaciones { get; set; }
    }
}
