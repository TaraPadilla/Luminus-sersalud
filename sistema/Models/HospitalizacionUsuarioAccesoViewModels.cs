using DocumentFormat.OpenXml.Math;

namespace farmamest.Models
{
    public class HospitalizacionUsuarioAccesoViewModels
    {
        public int Id { get; set; }
        public string UserNombre { get; set; }
        public string UserEmail { get; set; }
        public string UserId { get; set; }
        public int HospitalizacionId { get; set; }

        //Permisos
        public bool AutTabEnfermeria { get; set; }
        public bool AutTabActualizarEstadia { get; set; }
        public bool AutTabNotaEnfermeria { get; set; }
        public bool AutTabNotaEvolucion { get; set; }
        public bool AutTabControlGlucometria { get; set; }
        public bool AutTabDietas { get; set; }
        public bool AutTabIncretaExcreta { get; set; }
        public bool AutTabNotaMedica { get; set; }
        public bool AutTabPagos { get; set; }
        public bool AutTabSignosVitales { get; set; }
    }
}
