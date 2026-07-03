using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class PresupuestoDentalDetalle
    {
        public int Id { get; set; }
        public int PresupuestoDentalId { get; set; }
        public PresupuestoDental PresupuestoDental { get; set; }
        public string Codigo { get; set; }
        public string NombreServicio { get; set; }
        public string Diente { get; set; }
        public string Precio { get; set; }
        public string Valor { get; set; }
    }
}
