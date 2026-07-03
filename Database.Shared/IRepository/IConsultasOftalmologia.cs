// Database.Shared/IRepository/IConsultasOftalmologia.cs
using System.Collections.Generic;
using Database.Shared.Models;

namespace Database.Shared.IRepository
{
    public interface IConsultasOftalmologia
    {
        // Crear
        void Add(ConsultasOftalmologia model, bool saveChanges = true);

        // Crear y devolver Id (siguiendo el patrón de IConsultas.AddConsulta)
        long AddConsulta(ConsultasOftalmologia model);

        // Actualizar
        void Update(ConsultasOftalmologia model, bool saveChanges = true);

        // Leer por Id (PK)
        ConsultasOftalmologia GetConsulta(long id);

        // Leer por ConsultaId (FK al módulo de consultas)
        ConsultasOftalmologia GetConsulta(int consultaId);

        // Leer la última consulta por PacienteId (más reciente por Fecha)
        ConsultasOftalmologia GetConsultaByPaciente(int pacienteId);

        // Leer el histórico por PacienteId (todas, ordenadas por Fecha desc)
        IEnumerable<ConsultasOftalmologia> GetConsultasByPaciente(int pacienteId);

        // Listado simple (sin tracking en implementación)
        IList<ConsultasOftalmologia> ListaConsultas();
    }
}
