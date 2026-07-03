using Database.Shared.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace sistema.Models
{
    public class HospitalizacionHospitalizarTarifaViewModel
    {
        public int TarifaId { get; set; }
        public string NombreTarifa { get; set; }
        public decimal ValorTarifa { get; set; }
        public bool Lunes { get; set; }
        public bool Martes { get; set; }
        public bool Miercoles { get; set; }
        public bool Jueves { get; set; }
        public bool Viernes { get; set; }
        public bool Sabado { get; set; }
        public bool Domingo { get; set; }
        public bool FechaEspecial { get; set; }
        public string FechaTarifa { get; set; }
    }
}
