using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class TipoMovimientoProducto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Eliminado { get; set; }
    }
}
