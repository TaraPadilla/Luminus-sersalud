using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Database.Shared.Models
{
    public class ConsultaArchivo
    {
        public int Id { get; set; }
        public int ConsultaId { get; set; }
        public Consulta Consulta { get; set; }
        public string NombreArchivo { get; set; }
        public string UrlArchivo { get; set; }
        public DateTime FechaCarga { get; set; }
        public bool Eliminado { get; set; }
    }
}