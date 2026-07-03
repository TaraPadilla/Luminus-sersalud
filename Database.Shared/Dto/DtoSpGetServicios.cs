using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Shared.Dto
{
    public class DtoSpGetServicios
    {
        [Key]
        public int Item { get; set; }
        public int ServicioId { get; set; }
        public string ServicioCodigo { get; set; }
        public string ServicioNombre { get; set; }
        public string ServicioDescripcion { get; set; }
        public int? PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal? PrecioValor { get; set; }
    }
}
