using System;

namespace farmamest.Models
{
    public class KitIngresoInputVM
    {
        public int HospitalizacionId { get; set; }
        public string NombrePaciente { get; set; }

        public string NombreKit { get; set; }

        public string Medico { get; set; }
        public string Procedimiento { get; set; }
        public string Responsable { get; set; }
        public DateTime? FechaKit { get; set; }
    }

    public class KitIngresoDetalleInputVM
    {
        public int Id { get; set; }
        public int KitIngresoId { get; set; }
        public int ProductoId { get; set; }
        public string ProductoCodigo { get; set; }
        public string ProductoNombre { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public int? PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorSubtotal { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal DescuentoValor { get; set; }
        public decimal ValorTotal { get; set; }

        public decimal Utilizado { get; set; }
            public string NombreKit { get; set; } 
    }
}