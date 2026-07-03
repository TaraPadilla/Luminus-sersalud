using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class CuentaContableTrazabilidad
    {

        public int Id { get; set; }
        public int CuentaContableId { get; set; }
        public CuentaContable CuentaContable { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
