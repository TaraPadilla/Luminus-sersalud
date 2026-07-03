using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;
using System;
using System.Linq;
using Database.Shared.DataBindings;
using Microsoft.EntityFrameworkCore;

namespace Database.Shared.IRepository
{
    public interface IConsultas
    {

        Consulta GetConsultaPorHospitalizacion(int hospitalizacionId);
        Consulta GetConsultaPorCita(int citaId);

        void Add(Consulta consulta, bool saveChanges = true);
        int AddConsulta(Consulta consulta);
        IList<Consulta> ListaConsultas();
        IQueryable<Consulta> QueryConsultasParaListado();
        Consulta GetConsulta(int id, bool relatedEntities = true);
        List<ConsultaCaracteristicaDental> GetCaracteristicasDentales(int? idConsulta);
        List<ConsultaServicio> GetServiciosAgregados(int consultaId);
        List<Archivo> GetArchivos(int consultaId);
        List<Archivo> GetArchivosNew(int pacienteId);

        void DeleteArchivoConsulta(int archivoId);
        Consulta GetUltimaConsultaPaciente(int? idPaciente);
        void Update(Consulta consulta, bool saveChanges = true);
        void Update(ExamenFisico examen, bool saveChanges = true);
        void Update(Prescripcion prescripcion);
        void AddPrescipcion(Prescripcion prescripcion);
        void AddDetallePrescipcion(DetallePrescripcion detallePrescripcion);
        void AddExamenFisico(ExamenFisico examenFisico);
        Prescripcion GetPrescripcion(int prescripcionId);
        Prescripcion GetPrescripcionConsulta(int consultaId, bool includeProducto = false);
        List<Prescripcion> GetPrescripcionesCita(int citaId);
        List<Prescripcion> GetPrescripcionesPaciente(int pacienteId);

        List<ConsultaExamenLabClinico> GetExamenesAgregadosConsulta(int consultaId);
        void UpdateTablePrescription();
        public ExamenLabClinico GetExamenLabClincos(string codigo);
        List<ConsultaExamenArchivo> GetExamenArchivo(int consultaId);
        void AddExamnenArchivo(ConsultaExamenArchivo examenArchivo, bool saveChanges = true);

        void UpdateExamnenArchivo(ConsultaExamenArchivo examenArchivo, bool saveChanges = true);

    }
}