using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Database.Shared.Models
{
    public class OrdenCompra
    {
        public OrdenCompra()
        {
            DetalleOrdenesCompra = new List<DetalleOrdenCompra>();
        }
        public int Id { get; set; }
        public string NumeroComprobante { get; set; }
        public int? SucursalId { get; set; }
        public Sucursal Sucursal { get; set; }
        public int TipoCompraId { get; set; }
        public TipoCompra TipoCompra { get; set; }
        public int ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; }
        public int? EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }
        public string NombreVendedor { get; set; }
        public DateTime? FechaEstimadaRecepcion { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public DateTime Fecha { get; set; }
        public bool Eliminado { get; set; }
        public bool Recibida { get; set; }
        public string Observaciones { get; set; }
        public int DiasCredito { get; set; } = 0;


        public ICollection<DetalleOrdenCompra> DetalleOrdenesCompra { get; set; }
    }
}