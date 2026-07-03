using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class ServicioPrecio
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }
        public int PrecioId { get; set; }
        public Precio Precio { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }
        public bool Activar { get; set; }
    }
}