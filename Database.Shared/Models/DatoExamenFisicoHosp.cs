using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class DatoExamenFisicoHosp
    {
        public DatoExamenFisicoHosp()
        {
            ExamenesFisicosHospDatos = new List<ExamenFisicoHospDato>();
        }
        public int Id { get; set; }
        public string NombreDato { get; set; }
        public ICollection<ExamenFisicoHospDato> ExamenesFisicosHospDatos { get; set; }
    }
}
