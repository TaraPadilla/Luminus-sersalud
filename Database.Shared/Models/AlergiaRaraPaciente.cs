using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class AlergiaRaraPaciente
    {
        public int? Id { get; set; }

        public string Estado { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public int AlergiaRaraId { get; set; }
        public AlergiaRara AlergiaRara { get; set; }


    }
}
