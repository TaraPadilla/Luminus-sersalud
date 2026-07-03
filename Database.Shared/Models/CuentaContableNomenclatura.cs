using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class CuentaContableNomenclatura
    {
        public int Id { get; set; }
        public int CuentaContableId { get; set; }
        public CuentaContable CuentaContable { get; set; }
        public int NomenclaturaId { get;set; }
        public Nomenclatura Nomenclatura { get; set; }
    }
}
