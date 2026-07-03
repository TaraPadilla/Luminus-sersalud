using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class HospitalizacionExamen
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public int HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }
        public int ExamenId { get; set; }
        public Examen Examen { get; set; }
        public bool Eliminado { get; set; }
        public int? ExamenLabClinicoPrecioId { get; set; }
        public ExamenLabClinicoPrecio ExamenLabClinicoPrecio { get; set; }
        public string FechaAplicacionFormateada => FechaHora.ToString("dd MMM yyyy") ?? "Sin fecha";
    }
}
