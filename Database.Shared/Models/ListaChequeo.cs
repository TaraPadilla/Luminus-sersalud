using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class ListaChequeo
    {
        [Key]
        public int Id { get; set; }

        public int HospitalizacionId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UserId { get; set; }

        // ── Encabezado ────────────────────────────────────────────────────────
        public string    NombrePaciente   { get; set; }
        public string    ApellidoPaciente { get; set; }
        public DateTime? FechaNacimiento  { get; set; }
        public DateTime? FechaChequeo     { get; set; }
        public string    HoraChequeo      { get; set; }
        public string    MedicoTratante   { get; set; }

        // ── ENTRADA: El paciente despierto confirma ───────────────────────────
        public string    CI_NombreConfirma    { get; set; }
        public string    CI_ApellidoConfirma  { get; set; }
        public DateTime? CI_FechaNacConfirma  { get; set; }
        public string    CI_Consentimiento    { get; set; }
        public string    CI_Operacion         { get; set; }
        public string    CI_LadoOperar        { get; set; }
        public string    CI_SitioMarcado      { get; set; }
        public string    CI_Alergia           { get; set; }

        // ── ENTRADA: El anestesiólogo confirma ───────────────────────────────
        public string CI_EvalPreanestesica  { get; set; }
        public string CI_AccesoIV           { get; set; }
        public string CI_EquipoAnestesia    { get; set; }
        public string CI_Medicamentos       { get; set; }
        public string CI_Oximetro           { get; set; }
        public string CI_EquipoAspiracion   { get; set; }
        public string CI_ViaAerea           { get; set; }

        // ── PAUSA: El cirujano confirma ───────────────────────────────────────
        public string CP_Presentacion            { get; set; }
        public string CP_NombrePacienteCirujano  { get; set; }
        public string CP_ApellidoPacienteCirujano { get; set; }
        public DateTime? CP_FechaNacCirujano      { get; set; }
        public string CP_NombreCirugia           { get; set; }
        public string CP_EventosCriticos         { get; set; }
        public string CP_TiempoDuracion          { get; set; }
        public string CP_ImagenesDiagnosticas    { get; set; }
        public string CP_PerdidaSangre           { get; set; }

        // ── PAUSA: La instrumentista confirma ────────────────────────────────
        public string CP_Esterilidad            { get; set; }
        public string CP_MaterialesAdicionales  { get; set; }

        // ── PAUSA: El anestesiólogo confirma ─────────────────────────────────
        public string CP_EventosCriticosAnest   { get; set; }
        public string CP_ProfilaxisAntibiotica  { get; set; }
        public string CP_Tromboprofilaxis        { get; set; }
        public string CP_ManejoDolor             { get; set; }

        // ── SALIDA: La enfermera confirma ─────────────────────────────────────
        public string CS_NombreOperacion     { get; set; }
        public string CS_NombreEnfermera     { get; set; }
        public string CS_RecuentoCompleto    { get; set; }
        public string CS_EtiquetadoMuestras  { get; set; }

        // ── SALIDA: Recuperación ──────────────────────────────────────────────
        public string CS_RepasoPostOp    { get; set; }
        public string CS_PorQue          { get; set; }
        public string CS_Traslado        { get; set; }
        public string CS_Complicaciones  { get; set; }
        public string CS_ServicioNumCama { get; set; }

        // ── Navegación ────────────────────────────────────────────────────────
        [ForeignKey("HospitalizacionId")]
        public virtual Hospitalizacion Hospitalizacion { get; set; }
    }
}