using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Database.Shared.Models
{
    public class ConsultaRevisionSistemas
    {
        public int Id { get; set; }
        
        public string AparienciaGeneral { get; set; }
        public string Cabeza { get; set; }
        public string OidosBoca { get; set; }
        public string Cuello { get; set; }
        public string Torax { get; set; }
        public string Abdomen { get; set; }
        public string Genitales { get; set; }
        public string DorsoYExtremidades { get; set; }
        public string Musculoesqueletico { get; set; }
        
        // Nuevos campos añadidos
        public string Neurologico { get; set; }
        public string Cardiovascular { get; set; }
        public string Respiratorio { get; set; }
        public string Gastrointestinal { get; set; }
        public string PielFanera { get; set; }
        public string Genitourinario { get; set; }
    }
}
