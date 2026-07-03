using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class CategoriaHabitacionTarifa
    {
        public CategoriaHabitacionTarifa()
        {
            Hospitalizaciones = new List<Hospitalizacion>();
        }
        public int Id { get; set; }
        public string NombreTarifa { get; set; }
        public int CategoriaHabitacionId { get; set; }
        public CategoriaHabitacion CategoriaHabitacion { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTarifa { get; set; }
        public bool Lunes { get; set; }
        public bool Domingo { get; set; }
        public bool Sabado { get; set; }
        public bool Viernes { get; set; }
        public bool Jueves { get; set; }
        public bool Miercoles { get; set; }
        public bool Martes { get; set; }
        public bool Eliminada { get; set; }
        public DateTime FechaTarifa { get; set; }
        public bool FechaEspecial { get; set; }
        public ICollection<Hospitalizacion> Hospitalizaciones { get; set; }
    }
}
