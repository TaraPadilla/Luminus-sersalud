using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class HospitalizacionUsuarioAcceso
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }
        public bool Eliminado { get; set; }

        //Campos autorizaciones
        public bool AutorizacionTabEnfermeria { get; set; }
        public bool AutorizacionTabActualizarEstadia { get; set; }
        public bool AutorizacionTabControlGlucometria { get; set; }
        public bool AutorizacionTabNotaEnfermeria { get; set; }
        public bool AutorizacionTabNotaEvolucion { get; set; }
        public bool AutorizacionTabIncretaExcreta { get; set; }
        public bool AutorizacionTabNotaMedica { get; set; }
        public bool AutorizacionTabPagos { get; set; }
        public bool AutorizacionTabSignosVitales { get; set; }
        public bool AutorizacionTabDietas { get; set; }
    }
}
