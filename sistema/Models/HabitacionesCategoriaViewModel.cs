using System.Collections.Generic;

namespace sistema.Models
{
    public class HabitacionesCategoriaViewModel
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; }
        public List<HabitacionesCategoriaTarifaViewModel> Tarifas { get; set; }
    }
    public class HabitacionesCategoriaTarifaViewModel
    {
        public int? Id { get; set; }
        public string Nombre { get; set; }
        public bool Lunes { get; set; }
        public bool Martes { get; set; }
        public bool Miercoles { get; set; }
        public bool Jueves { get; set; }
        public bool Viernes { get; set; }
        public bool Sabado { get; set; }
        public bool Domingo { get; set; }
        public bool FechaEspecial { get; set; }
        public string Fecha { get; set; }
        public decimal Valor { get; set; }
        public bool Nueva { get; set; }
    }

}
