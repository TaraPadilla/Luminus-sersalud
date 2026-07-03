using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class TipoGastoProrrateo
    {
        public TipoGastoProrrateo()
        {

        }
        public int Id { get; set; }
        public string NombreTipo { get; set; }
        public bool Eliminado { get; set; }
    }
}