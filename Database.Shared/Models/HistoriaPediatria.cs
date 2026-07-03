using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class HistoriaPediatria
    {
        public int Id { get; set; }
        public string HistoriaProblema { get; set; }
        public string Sintomas { get; set; }
        public string HistoriaEnfermedadActual { get; set; }
        public string Diagnostico { get; set; }
        public string ImpresionClinica { get; set; }
        public string Comentario { get; set; }
        public ICollection<Consulta> Consultas { get; set; }

    }
}