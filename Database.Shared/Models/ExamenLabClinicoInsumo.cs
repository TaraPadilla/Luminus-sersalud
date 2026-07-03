using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class ExamenLabClinicoInsumo
    {
        public int Id { get; set; }
        public int ExamenLabClinicoId { get; set; }
        public ExamenLabClinico ExamenLabClinicos { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public UnidadMedidaVenta UnidadMedidaVenta { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadUtilizada { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal Total { get; set; }
        public bool Eliminado { get; set; }
    }
}