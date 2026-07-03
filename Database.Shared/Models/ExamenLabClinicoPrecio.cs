using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class ExamenLabClinicoPrecio
    {
        public int Id { get; set; }
        public int ExamenLabClinicoId { get; set; }
        public ExamenLabClinico ExamenLabClinico { get; set; }
        public int PrecioId { get; set; }
        public Precio Precio { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioValor { get; set; }
    }
}