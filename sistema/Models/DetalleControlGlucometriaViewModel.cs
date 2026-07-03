using Database.Shared.Models;

namespace farmamest.Models
{
    public class DetalleControlGlucometria2ViewModel
    {
        public int Id { get; set; }
        public string FechaHora { get; set; }
        public string GMT { get; set; }
        public string Insulina { get; set; }
        public string Unidades { get; set; }
        public string Medicamento { get; set; }
        public decimal Dosis { get; set; }
        public string Firma { get; set; }
        public string PersonaAplicaId { get; set; }
        public string NombrePersonaAplica { get; set; }
        public string FechaHoraAplicacion { get; set; }
        public bool Aplicado { get; set; }
        public string ProfesionalId { get; set; }
        public string NombreProfesional { get; set; }
        public int HospitalizacionId { get; set; }

        public bool Autorizado { get; set; }
        public string UsuarioAutoriza { get; set; }
        public string FechaAutorizacion { get; set; }
    }
}
