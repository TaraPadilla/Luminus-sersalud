using System;

namespace sistema.Models
{
    public class DetalleEmergenciaViewModel
    {
        public int? ProductId { get; set; }
        public int? ServicioId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioValor { get; set; }
        public decimal Descuento { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public int EmergencialId { get; set; }
        public int? ExamenLabClinicId { get; set; }
        public bool Eliminado { get; set; }
        public int Preciold { get; set; }
        public int? UnidadMedidaVentad { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
    }
}