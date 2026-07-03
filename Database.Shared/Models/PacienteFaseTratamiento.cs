using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class PacienteFaseTratamiento
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public int? FaseTratamientoId { get; set; }
        public FaseTratamiento FaseTratamiento { get; set; }
        public DateTime FechaInicioFase { get; set; }
        public bool FaseFinalizada { get; set; }
        public DateTime? FechaFinalizacionFase { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PesoAlIniciar { get; set; }
    }
}