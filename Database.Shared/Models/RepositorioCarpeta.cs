using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class RepositorioCarpeta
    {
        public int Id { get; set; }
        public string NombreCarpeta { get; set; }
        public ICollection<RepositorioArchivo> RepositorioArchivos { get; set; }
    }
}
