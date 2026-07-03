using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class CitasServicio
    {
        public int Id { get; set; }
        public int CitasId { get; set; }
        public Citas Citas { get; set; }
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }
        public int? Cantidad { get; set; }
        public int? PrecioId { get; set; }
        public Precio Precio { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PrecioValor { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioValorCubiertoSeguro { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioValorCopago { get; set; }
        public bool Eliminado { get; set; }
    }
}