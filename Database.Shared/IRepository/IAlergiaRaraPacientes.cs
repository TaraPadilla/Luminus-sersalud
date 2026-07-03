using Database.Shared.Models;
using Database.Shared.Paginacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface IAlergiaRaraPacientes
    {
        public List<AlergiaRara> GetAlergias();
        public List<AlergiaRaraPaciente> GetAlergiasPaciente(int idPaciente);

        public void AddAlergiasPacientes(AlergiaRaraPaciente alergiaRaraPaciente, bool saveChanges = true);

        public void AddAlergiaRara(AlergiaRara alergiaRara, bool saveChanges = true);

    }
}
