using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class Cuentas
    {
        public int Id { get; set; }
        public string NombreCuenta { get; set; }
        public int BancoId { get; set; }
        public Banco Banco { get; set; }
        public int TipoCuentaId { get; set; }
        public TipoCuenta TipoCuenta { get; set; }
        public string NumeroCuenta { get; set; }
        public bool Eliminado { get; set; }
    }
}
