using System;

namespace sistema.Models
{
    public class RequisicionListaItemViewModel
    {
        public int RequisicionId { get; set; }

        public int? NumeroRequisicion { get; set; }
        public int? NumeroOrden { get; set; }

        public DateTime? FechaSolicitud { get; set; }

        public string SolicitanteNombre { get; set; }

        // Texto listo para mostrar en la tabla (evita lógica de joins en la vista)
        public string BodegaOrigenNombre { get; set; }
        public string BodegaDestinoNombre { get; set; }

        public RequisicionEstado EstadoRequisicion { get; set; }
    }
}
