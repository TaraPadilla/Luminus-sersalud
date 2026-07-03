using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Database.Shared.Models
{
    public class ConsultaRevisionSistemasPediatria
    {
        public int Id { get; set; }
        public string AparienciaGeneral { get; set; }
        public string Cabeza { get; set; }
        public string OidosBoca { get; set; }
        public string Cuello { get; set; }
        public string Torax { get; set; }
        public string Abdomen { get; set; }
        public string DorsoYExtremidades { get; set; }
        public string Genitales { get; set; }
    }
}