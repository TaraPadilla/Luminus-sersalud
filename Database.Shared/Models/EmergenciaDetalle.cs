using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class EmergenciaDetalle
    {

        public int Id { get; set; }
        public int? ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public UnidadMedidaVenta UnidadMedidaVenta { get; set; }
        public int? PrecioId { get; set; }
        public Precio Precio { get; set; }
        public int? ServicioId { get; set; }
        public Servicio Servicio { get; set; }
        public int? ExamenLabClinicoId { get; set; }
        public ExamenLabClinico ExamenLabClinico { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioValor { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DescuentoPorcentaje { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Descuento { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        public int EmergenciaId { get; set; }
        public Emergencia Emergencia { get; set; }
        public bool Eliminado { get; set; }
    }
}