using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Shared.Dto
{
    public class DtoSpGetVentasProductoAnnio
    {
        [Key]
        public int Item { get; set; }
        public int Annio { get; set; }
        public int Mes { get; set; }
        public int ProductoId { get; set; }
        public int ProductoCantidad { get; set; }
        public string ProductoNombre { get; set; }
        public string Ambiente { get; set; }
    }
}
