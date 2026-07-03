using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Database.Shared.Models
{
    public class ExamenFisicoHospDato
    {
        public int Id { get; set; }
        public int ExamenFisicoHospId { get; set; }
        public ExamenFisicoHosp ExamenFisicoHosp { get; set; }
        public int DatoExamenFisicoHospId { get; set; }
        public DatoExamenFisicoHosp DatoExamenFisicoHosp { get; set; }
        public string Valor { get; set; }
    }
}