using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;

namespace Database.Shared.IRepository
{
    public interface IPacientes
    {
        public Paciente Add(Paciente Paciente, bool saveChanges = true);
        public PacienteHistorial AddHistorial(PacienteHistorial historial);
        public List<PacienteHistorial> GetHistorial(int pacienteId);
        public PresupuestoDental AddPresupuestoDental(PresupuestoDental presupuestoDental);
        public PresupuestoDental GetPresupuestoDental(int presupuestoId);
        public List<TipoPatologia> GetTipoPatologias();
        public List<PatologiaPaciente> GetAntecedentesFamiliaresPaciente(int pacienteId);
        public List<PersonaSeguro> GetPersonasSeguro(int pacienteId);
        public List<Vacuna> GetVacunas();
        public List<VacunaPaciente> GetVacunasPaciente(int idPaciente);
        public List<VacunaPaciente> GetVacunasAlertaPaciente();
        public List<AntecedentePersonal> GetAntecedentesPersonales();
        public List<PacienteAntecedentePersonal> GetAntecedentesPersonalesPaciente(int idPaciente);
        public List<PreguntaRegistro> GetPreguntasRegistro();
        public List<PacientePreguntaRegistro> GetPreguntasRegistroPaciente(int idPaciente);
        public void UpdateVacuna(VacunaPaciente vacunaPaciente);
        public void UpdateAntecedenteFamiliar(PatologiaPaciente antecedenteFamiliar);
        public void AddSeguimientoNutricional(PacienteSeguimientoNutricional seguimiento);
        public void UpdateSeguimientoNutricional(PacienteSeguimientoNutricional seguimiento);
        public void AddResultadoExamenLaboratorio(PacienteResultadoExamenLaboratorio resultado);
        public void UpdateResultadoExamenLaboratorio(PacienteResultadoExamenLaboratorio resultado);
        public List<PacienteSeguimientoNutricional> GetSeguimientosNutricionalesPaciente(int idPaciente);
        public List<PacienteRangoSaludable> GetRangosSaludablesPaciente(int pacienteId);
        public List<Paciente> GetList();
       
        public List<Paciente> GetPacientesInactivos();
        List<EstadoExamen> GetListEstadosExamen();
        public void Update(Paciente model, bool saveChanges = true);
        public Paciente Get(int pacienteId, bool includeRelatedEntities = true);
        public Paciente GetPacientePorNombre(string nombre);
        public Paciente GetPacientePorId(int id);
        public Paciente GetPacientePorDPI(string Dpi);
        
        public IList<Sexo> GetSexosList();
        public IList<TipoPaciente> GetTiposPaciente();
        public IList<SeguroEpss> GetSegurosEpssList();
        public int LastPatientGenerate();
        public PacienteFaseTratamiento GetFaseTratamientoActual(int pacienteId);
        public IList<FaseTratamiento> GetFasesTratamiento();
        public IList<PacienteFaseTratamiento> GetFasesTratamientoPacientes();
        public IList<Hospitalizacion> GetHospitalizaciones(int pacienteId);

		public void AddVacuna(VacunaPaciente vacunaNueva, bool saveChanges = true);

		public List<Departamento> GetListDepartamentos();
        public List<Municipio> GetListMunicipios(int departametoId);

        public List<AlergiaRaraPaciente> GetAlergiasPaciente(int idPaciente);
        public List<Paciente> Buscar(string terminoBusquedaNombre, string terminoBusquedaDpi);

        public List<ConsultaArchivo> GetArchivosConsultas(int pacienteId);
        List<Consulta> GetConsultas(int pacienteId);
    }
}