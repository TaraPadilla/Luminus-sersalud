using System;
using System.ComponentModel.DataAnnotations;

namespace sistema.Models
{
    public class Requisicion
    {
        public int RequisicionId { get; set; }

        // ===== Encabezado (maqueta) =====
        [MaxLength(35)]
        public string? Direccion { get; set; }

        [MaxLength(60)]
        public string? Departamento { get; set; }

        [MaxLength(60)]
        public string? UnidadSeccion { get; set; }

        [MaxLength(60)]
        public string? Otros { get; set; }

        public int? NumeroRequisicion { get; set; } // correlativo (6)
        public int? NumeroOrden { get; set; }       // (4)

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        // ===== Ubicaciones (como tu vista actual) =====
        public int BodegaOrigenId { get; set; }
        public int BodegaDestinoId { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        // ===== Firmas / nombres (fase 1: texto) =====
        [MaxLength(60)]
        public string? SolicitanteNombre { get; set; }

        [MaxLength(60)]
        public string? JefaturaNombre { get; set; }

        [MaxLength(60)]
        public string? GerenciaNombre { get; set; }

        [MaxLength(60)]
        public string? EncargadoAlmacenNombre { get; set; }

        [MaxLength(60)]
        public string? RecibeNombre { get; set; }

        // ===== Flujo =====
        public RequisicionEstado Estado { get; set; } = RequisicionEstado.Borrador;

        // ===== Auditoría simple =====
        public DateTime CreadoEn { get; set; } = DateTime.Now;

        [MaxLength(60)]
        public string? CreadoPor { get; set; }
    }
}
