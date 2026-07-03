using System;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Shared.Models;

namespace Database.Shared.Models
{
    public class DetallePrescripcion
    {
        public int Id { get; set; }
        public int Item { get; set; }

        /// <summary>
        /// Contiene el nombre del producto. Se dejo con ese nombre pues
        /// ya el modelo de datos trabajaba sobre ese nombre
        /// </summary>
        public string Medicine { get; set; }
        public int? ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public UnidadMedidaVenta UnidadMedidaVenta { get; set; }
        public int? PrecioId { get; set; }
        public Precio Precio { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioValor { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DescuentoPorcentaje { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DescuentoValor { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorCubiertoSeguro { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorCopago { get; set; }
        /// <summary>
        /// En proceso de ser eliminado pues no deberia ser tipo cadena
        /// </summary>
        public string Qty { get; set; }
        public string Indications { get; set; }
        public int PrescripcionId { get; set; }
        public Prescripcion Prescripcion { get; set; }
        public bool Pagado { get; set; }
        public string Color { get; set; }
        public DateTime FechaPrescripcion { get; set; }
    }
}
