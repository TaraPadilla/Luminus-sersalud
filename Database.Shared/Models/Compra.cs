using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class Compra
    {
        public Compra()
        {
            DetalleCompras = new List<DetalleCompra>();
            Recepciones = new List<Recepcion>();
            ProductoInventarios = new List<ProductoInventario>();
        }
        public int Id { get; set; }
        public int TipoCompraId { get; set; }
        public TipoCompra TipoCompra { get; set; }
        public int? DiasCredito { get; set; }
        public int? CompraTipoDocumentoId { get; set; }
        public CompraTipoDocumento CompraTipoDocumento { get; set; }
        public int ProveedorId { get; set; }
        public int? EmpleadoId { get; set; }
        public string NoComprobante { get; set; }
        public string NombreVendedor { get; set; }
        public DateTime? FechaLimite { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public DateTime FechaCompra { get; set; }
        public bool OrdenCompraRecibida { get; set; }
        public bool Estado { get; set; }
        public bool Eliminado { get; set; }
        public int? SucursalId { get; set; }
        public Sucursal Sucursal { get; set; }
        public int? TipoBodegaId { get; set; }
        public TipoBodega TipoBodega { get; set; }
        public Proveedor Proveedor { get; set; }
        public Empleado Empleado { get; set; }
        public string Observaciones { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }
        public ICollection<DetalleCompra> DetalleCompras { get; set; }
        public ICollection<Recepcion> Recepciones { get; set; }
        public ICollection<ProductoInventario> ProductoInventarios { get; set; }
        public string verEstado()
        {
            if (Estado == false)
            {
                return "Peticion";
            }
            return "Comprado";
        }


    }
}