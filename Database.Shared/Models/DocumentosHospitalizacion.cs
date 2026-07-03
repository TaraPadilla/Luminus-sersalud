using System;

namespace Database.Shared.Models
{
    public class DocumentosHospitalizacion
    {
        public int Id { get; set; }
        public int HospitalizacionId { get; set; }
        public int PacienteId { get; set; }
        public string NombreArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public DateTime FechaSubida { get; set; }
        public string Extension { get; set; }
        public long Tamano { get; set; } // bytes
        public bool Eliminado { get; set; } = false;

        // Relaciones
        public virtual Hospitalizacion Hospitalizacion { get; set; }
        public virtual Paciente Paciente { get; set; }

        public bool Autorizado { get; set; }
        public string UsuarioAutoriza { get; set; }
        public DateTime? FechaAutorizacion { get; set; }
    }
}