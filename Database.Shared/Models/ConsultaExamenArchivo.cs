using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class ConsultaExamenArchivo
    {
        public int Id { get; set; }
        public int ConsultaId { get; set; }
        //public Consulta Consulta { get; set; }
        public string NombreArchivo { get; set; }
        public string UrlArchivo { get; set; }
    }
}
