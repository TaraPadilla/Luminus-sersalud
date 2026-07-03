using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Database.Shared.Models;
using Database.Shared.IRepository;
using farmamest.Helpers;
using farmamest.Service.IService;

namespace farmamest.Controllers
{
    public class ListaChequeoController : Controller
    {
        private readonly IListaChequeoService _listaChequeoService;
        private readonly UserManager<User> _userManager;

        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            ReferenceHandler     = ReferenceHandler.IgnoreCycles
        };

        private static readonly JsonSerializerOptions _jsonInputOpts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public ListaChequeoController(
            IListaChequeoService listaChequeoService,
            UserManager<User> userManager)
        {
            _listaChequeoService = listaChequeoService;
            _userManager         = userManager;
        }

        [HttpPost]
        public async Task<string> AgregarListaChequeo()
        {
            try
            {
                var model = await HttpRequestJsonHelper.LeerCuerpoJsonAsync<ListaChequeoInputVM>(
                    Request, this, _jsonInputOpts);
                if (model == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos inválidos." }, _jsonOpts);

                if (model.HospitalizacionId == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "HospitalizacionId inválido." }, _jsonOpts);

                var nueva = new ListaChequeo
                {
                    HospitalizacionId   = model.HospitalizacionId,
                    FechaRegistro       = DateTime.Now,
                    UserId              = _userManager.GetUserId(HttpContext.User),

                    // Encabezado
                    NombrePaciente      = model.NombrePaciente,
                    ApellidoPaciente    = model.ApellidoPaciente,
                    FechaNacimiento     = model.FechaNacimiento,
                    FechaChequeo        = model.FechaChequeo,
                    HoraChequeo         = model.HoraChequeo,
                    MedicoTratante      = model.MedicoTratante,

                    // ENTRADA — Paciente
                    CI_NombreConfirma        = model.CI_NombreConfirma,
                    CI_ApellidoConfirma      = model.CI_ApellidoConfirma,
                    CI_FechaNacConfirma      = model.CI_FechaNacConfirma,
                    CI_Consentimiento        = model.CI_Consentimiento,
                    CI_Operacion             = model.CI_Operacion,
                    CI_LadoOperar            = model.CI_LadoOperar,
                    CI_SitioMarcado          = model.CI_SitioMarcado,
                    CI_Alergia               = model.CI_Alergia,

                    // ENTRADA — Anestesiólogo
                    CI_EvalPreanestesica     = model.CI_EvalPreanestesica,
                    CI_AccesoIV              = model.CI_AccesoIV,
                    CI_EquipoAnestesia       = model.CI_EquipoAnestesia,
                    CI_Medicamentos          = model.CI_Medicamentos,
                    CI_Oximetro              = model.CI_Oximetro,
                    CI_EquipoAspiracion      = model.CI_EquipoAspiracion,
                    CI_ViaAerea              = model.CI_ViaAerea,

                    // PAUSA — Cirujano
                    CP_Presentacion          = model.CP_Presentacion,
                    CP_NombrePacienteCirujano = model.CP_NombrePacienteCirujano,
                    CP_ApellidoPacienteCirujano = model.CP_ApellidoPacienteCirujano,
                    CP_FechaNacCirujano      = model.CP_FechaNacCirujano,
                    CP_NombreCirugia         = model.CP_NombreCirugia,
                    CP_EventosCriticos       = model.CP_EventosCriticos,
                    CP_TiempoDuracion        = model.CP_TiempoDuracion,
                    CP_ImagenesDiagnosticas  = model.CP_ImagenesDiagnosticas,
                    CP_PerdidaSangre         = model.CP_PerdidaSangre,

                    // PAUSA — Instrumentista
                    CP_Esterilidad           = model.CP_Esterilidad,
                    CP_MaterialesAdicionales = model.CP_MaterialesAdicionales,

                    // PAUSA — Anestesiólogo
                    CP_EventosCriticosAnest  = model.CP_EventosCriticosAnest,
                    CP_ProfilaxisAntibiotica = model.CP_ProfilaxisAntibiotica,
                    CP_Tromboprofilaxis      = model.CP_Tromboprofilaxis,
                    CP_ManejoDolor           = model.CP_ManejoDolor,

                    // SALIDA — Enfermera
                    CS_NombreOperacion       = model.CS_NombreOperacion,
                    CS_NombreEnfermera       = model.CS_NombreEnfermera,
                    CS_RecuentoCompleto      = model.CS_RecuentoCompleto,
                    CS_EtiquetadoMuestras    = model.CS_EtiquetadoMuestras,

                    // SALIDA — Recuperación
                    CS_RepasoPostOp          = model.CS_RepasoPostOp,
                    CS_PorQue                = model.CS_PorQue,
                    CS_Traslado              = model.CS_Traslado,
                    CS_Complicaciones        = model.CS_Complicaciones,
                    CS_ServicioNumCama       = model.CS_ServicioNumCama,
                };

                _listaChequeoService.Add(nueva);

                return JsonSerializer.Serialize(new { exitoso = true, resultado = true }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso  = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                }, _jsonOpts);
            }
        }

        [HttpPost]
        public string ObtenerListasChequeo(int idHospitalizacion)
        {
            try
            {
                var result = _listaChequeoService.GetByHospitalizacionId(idHospitalizacion);
                return JsonSerializer.Serialize(new { exitoso = true, resultado = result }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    mensaje = "Error al obtener las listas de chequeo: " + ex.Message
                }, _jsonOpts);
            }
        }

        [HttpPost]
        public async Task<string> ActualizarListaChequeo()
        {
            try
            {
                var model = await HttpRequestJsonHelper.LeerCuerpoJsonAsync<ListaChequeoInputVM>(
                    Request, this, _jsonInputOpts);
                if (model == null || model.Id == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Id inválido." }, _jsonOpts);

                var actualizada = new ListaChequeo
                {
                    Id                = model.Id,
                    HospitalizacionId = model.HospitalizacionId,

                    // Encabezado
                    NombrePaciente    = model.NombrePaciente,
                    ApellidoPaciente  = model.ApellidoPaciente,
                    FechaNacimiento   = model.FechaNacimiento,
                    FechaChequeo      = model.FechaChequeo,
                    HoraChequeo       = model.HoraChequeo,
                    MedicoTratante    = model.MedicoTratante,

                    // ENTRADA — Paciente
                    CI_NombreConfirma   = model.CI_NombreConfirma,
                    CI_ApellidoConfirma = model.CI_ApellidoConfirma,
                    CI_FechaNacConfirma = model.CI_FechaNacConfirma,
                    CI_Consentimiento   = model.CI_Consentimiento,
                    CI_Operacion        = model.CI_Operacion,
                    CI_LadoOperar       = model.CI_LadoOperar,
                    CI_SitioMarcado     = model.CI_SitioMarcado,
                    CI_Alergia          = model.CI_Alergia,

                    // ENTRADA — Anestesiólogo
                    CI_EvalPreanestesica = model.CI_EvalPreanestesica,
                    CI_AccesoIV          = model.CI_AccesoIV,
                    CI_EquipoAnestesia   = model.CI_EquipoAnestesia,
                    CI_Medicamentos      = model.CI_Medicamentos,
                    CI_Oximetro          = model.CI_Oximetro,
                    CI_EquipoAspiracion  = model.CI_EquipoAspiracion,
                    CI_ViaAerea          = model.CI_ViaAerea,

                    // PAUSA — Cirujano
                    CP_Presentacion           = model.CP_Presentacion,
                    CP_NombrePacienteCirujano = model.CP_NombrePacienteCirujano,
                    CP_ApellidoPacienteCirujano = model.CP_ApellidoPacienteCirujano,
                    CP_FechaNacCirujano      = model.CP_FechaNacCirujano,
                    CP_NombreCirugia          = model.CP_NombreCirugia,
                    CP_EventosCriticos        = model.CP_EventosCriticos,
                    CP_TiempoDuracion         = model.CP_TiempoDuracion,
                    CP_ImagenesDiagnosticas   = model.CP_ImagenesDiagnosticas,
                    CP_PerdidaSangre          = model.CP_PerdidaSangre,

                    // PAUSA — Instrumentista
                    CP_Esterilidad           = model.CP_Esterilidad,
                    CP_MaterialesAdicionales = model.CP_MaterialesAdicionales,

                    // PAUSA — Anestesiólogo
                    CP_EventosCriticosAnest  = model.CP_EventosCriticosAnest,
                    CP_ProfilaxisAntibiotica = model.CP_ProfilaxisAntibiotica,
                    CP_Tromboprofilaxis      = model.CP_Tromboprofilaxis,
                    CP_ManejoDolor           = model.CP_ManejoDolor,

                    // SALIDA — Enfermera
                    CS_NombreOperacion    = model.CS_NombreOperacion,
                    CS_NombreEnfermera    = model.CS_NombreEnfermera,
                    CS_RecuentoCompleto   = model.CS_RecuentoCompleto,
                    CS_EtiquetadoMuestras = model.CS_EtiquetadoMuestras,

                    // SALIDA — Recuperación
                    CS_RepasoPostOp    = model.CS_RepasoPostOp,
                    CS_PorQue          = model.CS_PorQue,
                    CS_Traslado        = model.CS_Traslado,
                    CS_Complicaciones  = model.CS_Complicaciones,
                    CS_ServicioNumCama = model.CS_ServicioNumCama,
                };

                _listaChequeoService.Actualizar(actualizada);

                return JsonSerializer.Serialize(new { exitoso = true }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso   = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                }, _jsonOpts);
            }
        }
    }

    // ── INPUT VM ──────────────────────────────────────────────────────────────
    public class ListaChequeoInputVM
    {
        public int Id             { get; set; } // 0 = nuevo, >0 = editar
        public int HospitalizacionId { get; set; }

        // Encabezado
        public string   NombrePaciente   { get; set; }
        public string   ApellidoPaciente { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public DateTime? FechaChequeo    { get; set; }
        public string   HoraChequeo      { get; set; }
        public string   MedicoTratante   { get; set; }

        // ENTRADA — Paciente
        public string CI_NombreConfirma   { get; set; }
        public string CI_ApellidoConfirma { get; set; }
        public DateTime? CI_FechaNacConfirma { get; set; }
        public string CI_Consentimiento   { get; set; }   // "SI" | "NO"
        public string CI_Operacion        { get; set; }
        public string CI_LadoOperar       { get; set; }   // "IZQUIERDO" | "DERECHO" | "NO APLICA"
        public string CI_SitioMarcado     { get; set; }   // "SI" | "NO" | "NO APLICA"
        public string CI_Alergia          { get; set; }   // "SI" | "NO"

        // ENTRADA — Anestesiólogo
        public string CI_EvalPreanestesica { get; set; }
        public string CI_AccesoIV          { get; set; }
        public string CI_EquipoAnestesia   { get; set; }
        public string CI_Medicamentos      { get; set; }
        public string CI_Oximetro          { get; set; }
        public string CI_EquipoAspiracion  { get; set; }
        public string CI_ViaAerea          { get; set; }

        // PAUSA — Cirujano
        public string CP_Presentacion           { get; set; }
        public string CP_NombrePacienteCirujano { get; set; }
        public string CP_ApellidoPacienteCirujano { get; set; }
        public DateTime? CP_FechaNacCirujano   { get; set; }
        public string CP_NombreCirugia          { get; set; }
        public string CP_EventosCriticos        { get; set; }
        public string CP_TiempoDuracion         { get; set; }
        public string CP_ImagenesDiagnosticas   { get; set; }
        public string CP_PerdidaSangre          { get; set; }  // "> 500 cc" | "> 7 ml/kg niños"

        // PAUSA — Instrumentista
        public string CP_Esterilidad           { get; set; }
        public string CP_MaterialesAdicionales { get; set; }

        // PAUSA — Anestesiólogo
        public string CP_EventosCriticosAnest  { get; set; }
        public string CP_ProfilaxisAntibiotica { get; set; }
        public string CP_Tromboprofilaxis      { get; set; }
        public string CP_ManejoDolor           { get; set; }

        // SALIDA — Enfermera
        public string CS_NombreOperacion    { get; set; }
        public string CS_NombreEnfermera    { get; set; }
        public string CS_RecuentoCompleto   { get; set; }
        public string CS_EtiquetadoMuestras { get; set; }

        // SALIDA — Recuperación
        public string CS_RepasoPostOp    { get; set; }
        public string CS_PorQue          { get; set; }
        public string CS_Traslado        { get; set; }  // "Cuidado intensivo" | "Encamamiento" | etc.
        public string CS_Complicaciones  { get; set; }
        public string CS_ServicioNumCama { get; set; }
    }
}