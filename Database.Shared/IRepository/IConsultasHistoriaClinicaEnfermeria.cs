// Database.Shared/IRepository/IConsultasClinicaEnfermeria.cs
using System.Collections.Generic;
using Database.Shared.Models;

namespace Database.Shared.IRepository
{
    public interface IConsultasHistoriaClinicaEnfermeria
    {
        // Crear
        void Add(ConsultasHistoriaClinicaEnfermeria model, bool saveChanges = true);

        // Crear y devolver Id (siguiendo el patrón de IConsultas.AddConsulta)
        long AddConsulta(ConsultasHistoriaClinicaEnfermeria model);

        // Actualizar
        void Update(ConsultasHistoriaClinicaEnfermeria model, bool saveChanges = true);

        // Leer por Id (PK)
        ConsultasHistoriaClinicaEnfermeria GetConsulta(long id);

        // Leer por ConsultaId (FK al módulo de consultas)
        ConsultasHistoriaClinicaEnfermeria GetConsulta(int consultaId);

        // Leer la última consulta por PacienteId (más reciente por Fecha)
        ConsultasHistoriaClinicaEnfermeria GetConsultaByPaciente(int pacienteId);

        // Leer el histórico por PacienteId (todas, ordenadas por Fecha desc)
        IEnumerable<ConsultasHistoriaClinicaEnfermeria> GetConsultasByPaciente(int pacienteId);

        // Listado simple (sin tracking en implementación)
        IList<ConsultasHistoriaClinicaEnfermeria> ListaConsultas();
    }
}
