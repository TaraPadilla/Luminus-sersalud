using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class CuestionarioPreAnestesico
    {
        [Key]
        public int Id { get; set; }

        public int      HospitalizacionId { get; set; }
        public DateTime FechaRegistro     { get; set; }
        public string   UserId            { get; set; }

        // ── Datos del paciente ────────────────────────────────────────────────
        public string    NombreCompleto      { get; set; }
        public string    RegistroMedico      { get; set; }
        public string    Edad                { get; set; }
        public DateTime? FechaCuestionario   { get; set; }
        public double?  Peso                { get; set; }
        public double?  Estatura            { get; set; }
        public DateTime? FechaUltimaRegla    { get; set; }
        public DateTime? FechaProcedimiento  { get; set; }
        public string    ProcedimientoProgramado { get; set; }
        public string    Cirujano            { get; set; }

        // ── Antecedentes médicos — columna izquierda ──────────────────────────
        public string PA_Alergia         { get; set; }   // SI | NO
        public string PA_AlergiaCual     { get; set; }   // texto libre
        public string PA_Fuma            { get; set; }
        public string PA_FumaCuanto      { get; set; }
        public string PA_Drogas          { get; set; }
        public string PA_DrogasCuales    { get; set; }
        public string PA_Alcohol         { get; set; }
        public string PA_AlcoholCuanto   { get; set; }
        public string PA_Embarazo        { get; set; }
        public string PA_EmbarazoCual        { get; set; }
        public string PA_Transfusion     { get; set; }
        public string PA_TransfusionCual { get; set; }
        public string PA_Asma            { get; set; }
        public string PA_AsmaCual        { get; set; }
        public string PA_Pulmones        { get; set; }
        public string PA_PulmonesCual    { get; set; }
        public string PA_Corazon         { get; set; }
        public string PA_CorazonCual     { get; set; }

        // ── Antecedentes médicos — columna derecha ────────────────────────────
        public string PA_AtaqueCardiaco     { get; set; }
        public string PA_AtaqueCardiacoCual { get; set; }
        public string PA_Angina             { get; set; }
        public string PA_AnginaCual         { get; set; }
        public string PA_Soplo              { get; set; }
        public string PA_SoploCual          { get; set; }
        public string PA_Presion            { get; set; }
        public string PA_PresionCual        { get; set; }
        public string PA_Higado             { get; set; }
        public string PA_HigadoCual         { get; set; }
        public string PA_Rinones            { get; set; }
        public string PA_RinonesCual        { get; set; }
        public string PA_Diabetes           { get; set; }
        public string PA_DiabetesCual       { get; set; }
        public string PA_Epilepsia          { get; set; }
        public string PA_EpilepsiaCual      { get; set; }
        public string PA_Derrame            { get; set; }
        public string PA_DerrameCual        { get; set; }
        public string PA_Tiroides           { get; set; }
        public string PA_TiroidesCual       { get; set; }
        public string PA_Anestesico         { get; set; }
        public string PA_AnestesicoCual     { get; set; }
        public string PA_AceptaTransfusion  { get; set; }
        public string PA_AceptaTransfusionCual { get; set; }

        // ── Información adicional ─────────────────────────────────────────────
        public string AI_Medicamentos    { get; set; }   // textarea
        public string AI_Actividad       { get; set; }   // SI | NO
        public string AI_ActividadDetalle { get; set; }  // textarea
        public string AI_OperacionesPrevias { get; set; } // textarea
        public string AI_Comentarios     { get; set; }   // textarea

        // ── Navegación ────────────────────────────────────────────────────────
        [ForeignKey("HospitalizacionId")]
        public virtual Hospitalizacion Hospitalizacion { get; set; }
    }
}