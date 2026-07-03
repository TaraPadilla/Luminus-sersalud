using Database.Shared.Models;

namespace farmamest.Models
{
    public class NotaEnfermeria2VM
    {
        public int Id { get; set; }
        public string Evolucion { get; set; }
        public string Sintomas { get; set; }
        public string Diagnostico { get; set; }
        public string FechaRegistro { get; set; }
        public int HospitalizacionId { get; set; }

        /// <summary>
        /// El user es el mismo profesional, persona que crea la nota de enfermeria
        /// </summary>
        public string UserId { get; set; }
        public string Profesional { get; set; }
        public int TurnoEnfermeriaId { get; set; }
    }
}
