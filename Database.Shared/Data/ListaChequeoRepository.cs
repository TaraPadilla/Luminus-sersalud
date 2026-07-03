using Database.Shared.IRepository;
using Database.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace Database.Shared.Data
{
    public class ListaChequeoRepository : IListaChequeo
    {
        private readonly Context _context;

        public ListaChequeoRepository(Context context)
        {
            _context = context;
        }

        public void Add(ListaChequeo listaChequeo)
        {
            _context.ListasChequeo.Add(listaChequeo);
            _context.SaveChanges();
        }

        public IEnumerable<ListaChequeo> GetByHospitalizacionId(int hospitalizacionId)
        {
            return _context.ListasChequeo
                .Where(l => l.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(l => l.FechaRegistro)
                .ToList();
        }

        public void Actualizar(ListaChequeo listaChequeo)
        {
            var existente = _context.ListasChequeo.Find(listaChequeo.Id);
            if (existente == null)
                throw new System.Exception($"Lista de chequeo con Id {listaChequeo.Id} no encontrada.");

            // Encabezado
            existente.FechaChequeo      = listaChequeo.FechaChequeo;
            existente.HoraChequeo       = listaChequeo.HoraChequeo;
            existente.MedicoTratante    = listaChequeo.MedicoTratante;
            existente.NombrePaciente    = listaChequeo.NombrePaciente;
            existente.ApellidoPaciente  = listaChequeo.ApellidoPaciente;
            existente.FechaNacimiento   = listaChequeo.FechaNacimiento;

            // ENTRADA — Paciente
            existente.CI_NombreConfirma   = listaChequeo.CI_NombreConfirma;
            existente.CI_ApellidoConfirma = listaChequeo.CI_ApellidoConfirma;
            existente.CI_FechaNacConfirma = listaChequeo.CI_FechaNacConfirma;
            existente.CI_Consentimiento   = listaChequeo.CI_Consentimiento;
            existente.CI_Operacion        = listaChequeo.CI_Operacion;
            existente.CI_LadoOperar       = listaChequeo.CI_LadoOperar;
            existente.CI_SitioMarcado     = listaChequeo.CI_SitioMarcado;
            existente.CI_Alergia          = listaChequeo.CI_Alergia;

            // ENTRADA — Anestesiólogo
            existente.CI_EvalPreanestesica = listaChequeo.CI_EvalPreanestesica;
            existente.CI_AccesoIV          = listaChequeo.CI_AccesoIV;
            existente.CI_EquipoAnestesia   = listaChequeo.CI_EquipoAnestesia;
            existente.CI_Medicamentos      = listaChequeo.CI_Medicamentos;
            existente.CI_Oximetro          = listaChequeo.CI_Oximetro;
            existente.CI_EquipoAspiracion  = listaChequeo.CI_EquipoAspiracion;
            existente.CI_ViaAerea          = listaChequeo.CI_ViaAerea;

            // PAUSA — Cirujano
            existente.CP_Presentacion           = listaChequeo.CP_Presentacion;
            existente.CP_NombrePacienteCirujano = listaChequeo.CP_NombrePacienteCirujano;
            existente.CP_ApellidoPacienteCirujano = listaChequeo.CP_ApellidoPacienteCirujano;
            existente.CP_FechaNacCirujano       = listaChequeo.CP_FechaNacCirujano;
            existente.CP_NombreCirugia          = listaChequeo.CP_NombreCirugia;
            existente.CP_EventosCriticos        = listaChequeo.CP_EventosCriticos;
            existente.CP_TiempoDuracion         = listaChequeo.CP_TiempoDuracion;
            existente.CP_ImagenesDiagnosticas   = listaChequeo.CP_ImagenesDiagnosticas;
            existente.CP_PerdidaSangre          = listaChequeo.CP_PerdidaSangre;

            // PAUSA — Instrumentista
            existente.CP_Esterilidad           = listaChequeo.CP_Esterilidad;
            existente.CP_MaterialesAdicionales = listaChequeo.CP_MaterialesAdicionales;

            // PAUSA — Anestesiólogo
            existente.CP_EventosCriticosAnest  = listaChequeo.CP_EventosCriticosAnest;
            existente.CP_ProfilaxisAntibiotica = listaChequeo.CP_ProfilaxisAntibiotica;
            existente.CP_Tromboprofilaxis      = listaChequeo.CP_Tromboprofilaxis;
            existente.CP_ManejoDolor           = listaChequeo.CP_ManejoDolor;

            // SALIDA — Enfermera
            existente.CS_NombreOperacion    = listaChequeo.CS_NombreOperacion;
            existente.CS_NombreEnfermera    = listaChequeo.CS_NombreEnfermera;
            existente.CS_RecuentoCompleto   = listaChequeo.CS_RecuentoCompleto;
            existente.CS_EtiquetadoMuestras = listaChequeo.CS_EtiquetadoMuestras;

            // SALIDA — Recuperación
            existente.CS_RepasoPostOp    = listaChequeo.CS_RepasoPostOp;
            existente.CS_PorQue          = listaChequeo.CS_PorQue;
            existente.CS_Traslado        = listaChequeo.CS_Traslado;
            existente.CS_Complicaciones  = listaChequeo.CS_Complicaciones;
            existente.CS_ServicioNumCama = listaChequeo.CS_ServicioNumCama;

            _context.SaveChanges();
        }
    }
}