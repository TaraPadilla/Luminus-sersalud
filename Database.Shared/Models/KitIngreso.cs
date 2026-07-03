using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class KitIngreso
    {
        public int Id { get; set; }
        public int? HospitalizacionId { get; set; } // null = global
        public DateTime FechaRegistro { get; set; }
        public string UserId { get; set; }
        public string NombrePaciente { get; set; }
        public string Medico { get; set; }
        public string Procedimiento { get; set; }
        public string Responsable { get; set; }
        public DateTime? FechaKit { get; set; }
        public string NombreKit { get; set; }
        public bool EsGlobal { get; set; } = true; // siempre true para kits globales

        public virtual ICollection<KitIngresoDetalle> Detalles { get; set; }
    }

    public class KitIngresoDetalle
    {
        public int Id { get; set; }
        public int KitIngresoId { get; set; }
        public virtual KitIngreso KitIngreso { get; set; }

        public int ProductoId { get; set; }
        public string ProductoCodigo { get; set; }
        public string ProductoNombre { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public int? PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorSubtotal { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal DescuentoValor { get; set; }
        public decimal ValorTotal { get; set; }
        public bool Eliminado { get; set; }

        // Relación con consumos
        public virtual ICollection<HospitalizacionKitConsumo> Consumos { get; set; }
    }

    public class HospitalizacionKitConsumo
    {
        public int Id { get; set; }
        public int HospitalizacionId { get; set; }
        public virtual Hospitalizacion Hospitalizacion { get; set; }
        public int KitIngresoDetalleId { get; set; }
        public virtual KitIngresoDetalle KitIngresoDetalle { get; set; }
        public decimal Utilizado { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}