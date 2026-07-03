using System;
using System.Collections.Generic;

namespace Database.Shared.Models
{
    public class DevolucionNacional
    {
        public int Id { get; set; }
        public int? NumeroDevolucion { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public string? Direccion { get; set; }
        public string? Departamento { get; set; }
        public string? UnidadSeccion { get; set; }
        public string? Otros { get; set; }
        public string? Observaciones { get; set; }
        public int? BodegaOrigenId { get; set; }
        public string? SolicitanteNombre { get; set; }
        public string? FirmaSolicitante { get; set; }
        public string? AutorizadoPor { get; set; }
        public string? FirmaAutorizacion { get; set; }
        public int Estado { get; set; }
        public DateTime? CreadoEn { get; set; }
        public string? CreadoPor { get; set; }

        public Bodega? BodegaOrigen { get; set; }

        public int? ProveedorId { get; set; }
        public virtual Proveedor Proveedor { get; set; }

        public int? NumeroOficio { get; set; }


        public ICollection<DevolucionNacionalDetalle> Detalles { get; set; }
            = new List<DevolucionNacionalDetalle>();
    }

    public class DevolucionNacionalDetalle
    {
        public int Id { get; set; }
        public int DevolucionNacionalId { get; set; }
        public DevolucionNacional? DevolucionNacional { get; set; }
        public int ProductoInventarioId { get; set; }
        public ProductoInventario? ProductoInventario { get; set; }
        public int CantidadDevuelta { get; set; }
    }
}