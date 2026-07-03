using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class AntecedentePersonal
    {
        public AntecedentePersonal()
        {
            PacientesAntecedentesPersonales = new List<PacienteAntecedentePersonal>();
        }
        public int Id { get; set; }
        public string NombreAntecedente { get; set; }
        public ICollection<PacienteAntecedentePersonal> PacientesAntecedentesPersonales { get; set; }
    }
}
