using System.Collections.Generic;
using Database.Shared.Models;

namespace Database.Shared.IRepository
{
    public interface IConsultasSueroterapia
    {
        // Crear
        void Add(ConsultasSueroterapia model, bool saveChanges = true);

        // Crear y devolver Id (siguiendo el patrón de IConsultas.AddConsulta)
        long AddConsulta(ConsultasSueroterapia model);

        // Actualizar
        void Update(ConsultasSueroterapia model, bool saveChanges = true);

        // Leer por Id (PK)
        ConsultasSueroterapia GetConsulta(long id);

        // Leer por ConsultaId (FK al módulo de consultas)
        ConsultasSueroterapia GetConsulta(int consultaId);

        // Leer la última consulta por PacienteId (más reciente por Fecha)
        ConsultasSueroterapia GetConsultaByPaciente(int pacienteId);

        // Leer el histórico por PacienteId (todas, ordenadas por Fecha desc)
        IEnumerable<ConsultasSueroterapia> GetConsultasByPaciente(int pacienteId);

        // Listado simple (sin tracking en implementación)
        IList<ConsultasSueroterapia> ListaConsultas();
    }
}
