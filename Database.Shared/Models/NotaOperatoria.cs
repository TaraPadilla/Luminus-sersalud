using System;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Shared.Models;

public class NotaOperatoria
{
    public int Id { get; set; }

    public string Sintomas { get; set; }
    public string Evolucion { get; set; }

    public string Diagnostico { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int HospitalizacionId { get; set; }
    public string UserId { get; set; }

    // ── Datos de la operación ──────────────────────────────
    public DateTime? FechaOperacion { get; set; }
    public string HoraComenzo { get; set; }
    public string HoraTermino { get; set; }

    // ── Personal de quirófano ──────────────────────────────
    public string Cirujano { get; set; }
    public string PrimerAyudante { get; set; }
    public string SegundoAyudante { get; set; }
    public string Anestesista { get; set; }
    public string Instrumentista { get; set; }
    public string Circulante { get; set; }

    // ── Diagnósticos y hallazgos ───────────────────────────
    public string DiagnosticoPreOperatorio { get; set; }
    public string DiagnosticoPostOperatorio { get; set; }
    public string OperacionEfectuada { get; set; }
    public string HallazgosTransOperatorios { get; set; }

    // ── Firma del cirujano ─────────────────────────────────
    public string FirmaRuta { get; set; }
    public DateTime? FechaFirma { get; set; }

    [ForeignKey("HospitalizacionId")]
    public virtual Hospitalizacion Hospitalizacion { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}