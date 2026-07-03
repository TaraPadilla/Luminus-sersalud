using System;
namespace Database.Shared.Models
{
    public class DevolucionListaDTO
    {
        public int Id { get; set; }
        public int NumeroDevolucion { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public int Estado { get; set; }
        public string DepartamentoNombre { get; set; }
        public string UnidadNombre { get; set; }
        public string SolicitanteNombre { get; set; }
        public string BodegaOrigenNombre { get; set; }

        public string AutorizadoPor { get; set; }

    }
}