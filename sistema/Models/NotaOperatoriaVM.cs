using System;
using System.Collections.Generic;

namespace farmamest.Models
{
    public class NotaOperatoriaVM
    {
        public int Id { get; set; }

        public string Diagnostico { get; set; }

        public string FechaRegistro { get; set; }
        public int HospitalizacionId { get; set; }
        public string UserId { get; set; }

        // Nombre del profesional (User->Persona)
        public string Profesional { get; set; }

        // ── Datos de la operación ──────────────────────────
        public string FechaOperacion { get; set; }
        public string HoraComenzo { get; set; }
        public string HoraTermino { get; set; }

        // ── Personal de quirófano ──────────────────────────
        public string Cirujano { get; set; }
        public string PrimerAyudante { get; set; }
        public string SegundoAyudante { get; set; }
        public string Anestesista { get; set; }
        public string Instrumentista { get; set; }
        public string Circulante { get; set; }

        // ── Diagnósticos y hallazgos ───────────────────────
        public string DiagnosticoPreOperatorio { get; set; }
        public string DiagnosticoPostOperatorio { get; set; }
        public string OperacionEfectuada { get; set; }
        public string HallazgosTransOperatorios { get; set; }

        // ── Firma del cirujano ──────────────────────────────
        public string FirmaRuta { get; set; }
        public string FechaFirma { get; set; }
        public bool Firmado => !string.IsNullOrEmpty(FirmaRuta);

        // Campos auxiliares para PDF
        public string FirmaBase64 { get; set; }
        public string Colegiado { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteSexoText { get; set; }
        public string PacienteEdad { get; set; }
    }

    public class NotaOperatoriaListaViewModel
    {
        public string PacienteNombre { get; set; }
        public string PacienteEdad { get; set; }
        public string PacienteSexoText { get; set; }
        public string EmpleadoText { get; set; }
        public string Colegiado { get; set; }
        public string EmpleadoEspecialidad { get; set; }
        public List<NotaOperatoriaVM> Notas { get; set; } = new();
    }
}