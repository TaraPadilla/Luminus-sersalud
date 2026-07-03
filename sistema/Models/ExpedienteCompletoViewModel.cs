using System.Collections.Generic;
using Database.Shared.Models;
using farmamest.Models;
using sistema.Models;

namespace farmamest.Models
{
    public class ExpedienteCompletoViewModel
    {
        public int HospitalizacionId { get; set; }
        public int PacienteId { get; set; }
        public int HabitacionId { get; set; }
        public int? CitaId { get; set; }

        public string PacienteNombre { get; set; }
        public string MedicoTratanteNombre { get; set; }
        public string MedicoTratanteEspecialidad { get; set; }
        public string MedicoTratanteColegiado { get; set; }
        public string MedicoTratanteFirmaBase64 { get; set; }

        public ConsentimientoHospiVM Consentimiento { get; set; }
        public CuestionarioPreAnestesico CuestionarioPreAnestesico { get; set; }
        public List<NotaMedica2ViewModel> NotasEvolucion { get; set; } = new();
        public List<NotaEnfermeriaPdfViewModel> NotasEnfermeria { get; set; } = new();
        public List<OrdenMedicaViewModel> OrdenesMedicas { get; set; } = new();
        public List<NotaOperatoriaVM> NotasOperatorias { get; set; } = new();
        public List<PacienteArchivoVM> Documentos { get; set; } = new();
        public List<DocumentoEmbebidoVm> DocumentosEmbebidos { get; set; } = new();
        public DocumentoEmbebidoVm DocumentoConsentimientoEmbebido { get; set; }

        public AutorizacionAnestesiaPdfVM AutorizacionAnestesia { get; set; }
        public List<ListaChequeo> ListasChequeo { get; set; } = new();
        public List<SignosVitalesHospPdfRow> SignosVitales { get; set; } = new();
        public List<MedicamentoNoControladoPdfVM> MedicamentosControlados { get; set; } = new();
        public List<HistorialMedicamentoPdfRow> HistorialMedicamentos { get; set; } = new();
        public RegistroAnestesia RegistroAnestesia { get; set; }
        public RegistroAnestesiaPdfVM RegistroAnestesiaPdf { get; set; }

        public NotaMedica2ViewModel NotaIngresoEvolucion { get; set; }
        public NotaEnfermeriaPdfViewModel NotaIngresoEnfermeria { get; set; }
        public NotaMedica2ViewModel NotaTrasladoEvolucion { get; set; }
        public NotaEnfermeriaPdfViewModel NotaTrasladoEnfermeria { get; set; }
        public NotaMedica2ViewModel NotaRecepcionEvolucion { get; set; }
        public NotaEnfermeriaPdfViewModel NotaRecepcionEnfermeria { get; set; }
        public List<NotaMedica2ViewModel> NotasEgresoEvolucion { get; set; } = new();
        public List<NotaEnfermeriaPdfViewModel> NotasEgresoEnfermeria { get; set; } = new();

        public ExpedienteHospitalizacionContextoVm Contexto { get; set; }
    }
}
