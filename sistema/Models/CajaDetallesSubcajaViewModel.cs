using System;

namespace sistema.Models
{
    public class CajaDetallesSubcajaViewModel
    {
        public int Id { get; set; }
        public bool EstadoCaja { get; set; }
        public int? SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public int? AmbienteId { get; set; }
        public string AmbienteNombre { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string ResponsableApertura { get; set; }
        public string ResponsableCierre { get; set; }
        public decimal MontoApertura { get; set; }
        public decimal Ingresos { get; set; }
        public decimal Gastos { get; set; }
        public decimal TotalCierre { get; set; }
    }
}
