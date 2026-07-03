using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Shared.Enumeraciones;

namespace Database.Shared.Models
{
    public class CuentaPorCobrar
    {
        public CuentaPorCobrar()
        {
            Pagos = new List<Pagos>();
            DetallesCaja = new List<DetalleCaja>();
            //DetallesCajaClinica = new List<DetalleCajaClinica>();
            DetallesCuentaPorCobrar = new List<DetalleCuentaPorCobrar>();
        }
        public int Id { get; set; }
        public string UuidFel { get; set; }

        public int? PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public DateTime? FechaLimitePago { get; set; }
        public decimal? Valor { get; set; }
        public decimal? ValorPagado { get; set; }
        public bool Pagada { get; set; }
        public DateTime? FechaPagoRealizado { get; set; }
        public string Observaciones { get; set; }
        public bool Eliminada { get; set; }
        public ICollection<Pagos> Pagos { get; set; }

        public string NombrePacienteText
        {
            get { return Paciente == null || Paciente.Nombre == null ? "-" : Paciente.Nombre; }
        }
        public string DiasRestantesPagoText
        {
            get { return "Días"; }
        }
        public string PagadaText
        {
            get { return Pagada ? "Sí" : "No"; }
        }

        public ICollection<DetalleCaja> DetallesCaja { get; set; }
        //public ICollection<DetalleCajaClinica> DetallesCajaClinica { get; set; }
        public ICollection<DetalleCuentaPorCobrar> DetallesCuentaPorCobrar { get; set; }

        public EstadosFEL FelEstado { get; set; } = EstadosFEL.NoIniciado;
        public int FelIntentos { get; set; } = 0;
        [MaxLength(1000)]
        public string FelUltimoError { get; set; }
        public DateTime? FelFechaUltimoIntento { get; set; }
        public DateTime? FelFechaEmitida { get; set; }

        public string FelReceptorNit { get; set; }
        public string FelReceptorNombre { get; set; }
        public string FelReceptorDireccion { get; set; }
        public string FelReceptorCorreo { get; set; }
    }
}