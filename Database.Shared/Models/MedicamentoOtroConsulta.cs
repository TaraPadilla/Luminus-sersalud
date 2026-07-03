using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class MedicamentoOtroConsulta
    {
        public int Id { get; set; }
        public int ConsultaId { get; set; }
        public string Nombre { get; set; }
        public string Indicaciones { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaPrescripcion { get; set; }
        public Consulta Consulta { get; set; }
    }

}