// Database.Shared/IRepository/IConsultasValoracionEnfermeria.cs
using System.Collections.Generic;
using Database.Shared.Models;

namespace Database.Shared.IRepository
{
    public interface IConsultasValoracionInicialEnfermeria
    {
        // Crear
        void Add(ConsultasValoracionInicialEnfermeria model, bool saveChanges = true);

        // Crear y devolver Id (siguiendo el patrón de IConsultas.AddConsulta)
        long AddConsulta(ConsultasValoracionInicialEnfermeria model);

        // Actualizar
        void Update(ConsultasValoracionInicialEnfermeria model, bool saveChanges = true);

        // Leer por Id (PK)
        ConsultasValoracionInicialEnfermeria GetConsulta(long id);

        // Leer por ConsultaId (FK al módulo de consultas)
        ConsultasValoracionInicialEnfermeria GetConsulta(int consultaId);

        // Leer la última consulta por PacienteId (más reciente por Fecha)
        ConsultasValoracionInicialEnfermeria GetConsultaByPaciente(int pacienteId);

        // Leer el histórico por PacienteId (todas, ordenadas por Fecha desc)
        IEnumerable<ConsultasValoracionInicialEnfermeria> GetConsultasByPaciente(int pacienteId);

        // Listado simple (sin tracking en implementación)
        IList<ConsultasValoracionInicialEnfermeria> ListaConsultas();
    }
}
