using System;

namespace Database.Shared.Models
{
    public class RequisionListaItemDto
    {
        public int RequisicionId { get; set; }

        public int? NumeroRequisicion { get; set; }
        public int? NumeroOrden { get; set; }

        public DateTime? FechaSolicitud { get; set; }

        public string SolicitanteNombre { get; set; }

        public string BodegaOrigenNombre { get; set; }
        public string BodegaDestinoNombre { get; set; }

        public string DepartamentoNombre { get; set; }

        public string UnidadNombre { get; set; }

        public int Estado { get; set; }
    }
}
