using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class CuentaContable
    {
        public int Id { get; set; }
        public string NombreCuenta { get; set; }
        public string Especificaciones { get; set; }
        public int BancoId { get; set; }
        public Banco Banco { get; set; }

        public int? CuentaId { get; set; }
        public Cuentas Cuenta { get; set; }
        public ICollection<CuentaContableNomenclatura> Nomenclatura { get; set; }
        public int CategoriaCuentaId { get; set; }
        public CategoriasCuentaContable CategoriaCuenta { get; set; }
        public bool Eliminado { get; set; }
    }
}
