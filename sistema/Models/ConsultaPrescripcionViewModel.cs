using System;

namespace sistema.Models
{
    public class ConsultaPrescripcionViewModel
    {

        public int Item { get; set; }
        public int? ProductoId { get; set; }
        public string Medicamento { get; set; }
        public string ProductoCodigo { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoIndicaciones { get; set; }
        public int? ProductoPrecioId { get; set; }
        public decimal? PrecioValor { get; set; }
        public string Precio { get; set; }
        public decimal? ValorTotal { get; set; }
        public decimal ValorCubiertoSeguro { get; set; }
        public decimal ValorCopago { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public string Observaciones { get; set; }
        public string Categoria { get; set; }
        public decimal Cantidad { get; set; }
        public bool Nuevo { get; set; }
        public bool Eliminado { get; set; }
        public bool Pagado { get; set; }
        public string Color { get; set; }
        public bool Another { get; set; }
        public int PrescripcionId { get; set; }
        public int ConsultaId { get; set; }
        public string ColorPrescripcion { get; set; }
        public DateTime FechaPrescripcion { get; set; }

    }
}
