// Database.Shared/IRepository/IConsultasPodologia.cs
using System.Collections.Generic;
using Database.Shared.Models;

namespace Database.Shared.IRepository
{
    public interface IConsultasPodologia
    {
        // Crear
        void Add(ConsultasPodologia model, bool saveChanges = true);

        // Crear y devolver Id
        long AddConsulta(ConsultasPodologia model);

        // Actualizar
        void Update(ConsultasPodologia model, bool saveChanges = true);

        // Leer por Id (PK)
        ConsultasPodologia GetConsulta(long id);

        // Leer por ConsultaId (FK al módulo de consultas)
        ConsultasPodologia GetConsulta(int consultaId);

        // Leer la última consulta por PacienteId (más reciente por Fecha)
        ConsultasPodologia GetConsultaByPaciente(int pacienteId);

        // Leer el histórico por PacienteId (todas, ordenadas por Fecha desc)
        IEnumerable<ConsultasPodologia> GetConsultasByPaciente(int pacienteId);


        // Listado simple
        IList<ConsultasPodologia> ListaConsultas();
    }
}
