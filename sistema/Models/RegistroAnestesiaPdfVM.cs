using System;

namespace sistema.Models
{
    public class RegistroAnestesiaPdfVM
    {
        public int HospitalizacionId { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteSexo { get; set; }
        public string EdadPaciente { get; set; }
        public string PacientePeso { get; set; }
        public string PacienteTalla { get; set; }
        public string Procedimiento { get; set; }
        public string MedicoAnestesista { get; set; }
        public string UrlFirmaAnestesista { get; set; }
        public DateTime FechaImpresion { get; set; }
        public bool TieneRegistroGuardado { get; set; }
        public string DatosJson { get; set; }
        public DateTime? FechaRegistroGuardado { get; set; }
    }
}
