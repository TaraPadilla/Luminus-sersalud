using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class CuentasTrazabilidad
    {
        public int Id { get; set; }
        public int CuentaId { get; set; }
        public Cuentas Cuenta { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
