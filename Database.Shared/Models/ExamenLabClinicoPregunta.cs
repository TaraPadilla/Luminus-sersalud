using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class ExamenLabClinicoPregunta
    {
        public int Id { get; set; }
        public int ExamenLabClinicoId { get; set; }
        public ExamenLabClinico ExamenLabClinico { get; set; }
        public string Pregunta { get; set; }

        public string Detalles { get; set; }
        public bool Respuesta { get; set; }
        public bool Eliminado { get; set; }
    }
}
