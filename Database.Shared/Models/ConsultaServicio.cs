using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class ConsultaServicio
    {
        public int Id { get; set; }
        public int ConsultaId { get; set; }
        public Consulta Consulta { get; set; }
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }
        public int? NumeroDiente { get; set; }
        public int? PrecioId { get; set; }
        public Precio Precio { get; set; }
        public decimal? PrecioValor { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCubiertoSeguro { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCopago { get; set; }
        public int Cantidad { get; set; }
        public bool Pagado { get; set; }
    }
}