using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class ConfiguracionSistema
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string NombreEstablecimiento { get; set; }
        public bool ProrrateoHabilitado { get; set; }
    }
}
