using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class RepositorioArchivo
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; }
        public string Tipo { get; set; }
        public int RepositorioCarpetaId { get; set; }
        public RepositorioCarpeta RepositorioCarpeta { get; set; }
    }
}
