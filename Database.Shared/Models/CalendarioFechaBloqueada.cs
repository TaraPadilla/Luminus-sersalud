using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class CalendarioFechaBloqueada
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string UsuarioBloquea { get; set; }
        public int? EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }
        public string Motivo { get; set;}
        public bool Bloqueada { get; set; }
        public bool Eliminada { get; set; }
    }
}
