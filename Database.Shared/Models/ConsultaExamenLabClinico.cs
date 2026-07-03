using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class ConsultaExamenLabClinico
    {
        public int Id { get; set; }
        /// <summary>
        /// Fecha en que se agrega el examen a la consulta
        /// </summary>
        public DateTime FechaRegistro { get; set; }
        public int? ConsultaId { get; set; }
        public Consulta Consulta { get; set; }
        public int? ExamenLabClinicoId { get; set; }
        public ExamenLabClinico ExamenLabClinico { get; set; }
        public int? PrecioId { get; set; }
        public Precio Precio { get; set; }
        public decimal PrecioValor { get; set; }
        public int Cantidad { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal PrecioValorCubiertoSeguro { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioValorCopago { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DescuentoPorcentaje { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DescuentoValor { get; set; }
        public bool Pagado { get; set; }
    }
}