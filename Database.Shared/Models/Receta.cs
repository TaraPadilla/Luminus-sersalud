using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class Receta
    {
        public int Id { get; set; }
        public string NombreReceta { get; set; }
        public string Ingredientes { get; set; }
        public DateTime FechaHoraCreada { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCosto { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioVenta { get; set; }
        public bool Eliminado { get; set; }
        public int CategoriaId { get; set; }
    }
}
