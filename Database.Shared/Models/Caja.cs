using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class Caja
    {
        public Caja()
        {
            DetalleCajas = new List<DetalleCaja>();
        }

        public int Id { get; set; }
        public int? EmpleadoId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoApertura { get; set; }
        public string NombrePersonalizado { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public int? SucursalId { get; set; }
        public int? TipoBodegaId { get; set; }
        public TipoBodega TipoBodega { get; set; }
        public int? AmbienteId { get; set; }
        public Ambiente Ambiente { get; set; }
        public Sucursal Sucursal { get; set; }
        public bool EstadoCaja { get; set; }
        public User ResponsableApertura { get; set; }
        public User ResponsableCierre { get; set; }
        public ICollection<DetalleCaja> DetalleCajas { get; set; }

        public string ResponsableAperturaText
        {
            get
            {
                return (ResponsableApertura.Persona == null || ResponsableApertura == null)
                ? "Admin" : ResponsableApertura.Persona.NombreYApellidos.ToString();
            }
        }

        public string ResponsableCierreText
        {
            get
            {
                return (ResponsableCierre.Persona == null || ResponsableCierre == null)
                ? "Admin" : ResponsableCierre.Persona.NombreYApellidos.ToString();
            }
        }
    }
}