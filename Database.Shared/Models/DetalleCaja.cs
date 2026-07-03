using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class DetalleCaja
    {

        public int Id { get; set; }
        public int? VentaId { get; set; }
        public Venta Venta { get; set; }
        public int? CompraId { get; set; }
        public Compra Compra { get; set; }
        public int? CuentaPorCobrarId { get; set; }
        public CuentaPorCobrar CuentaPorCobrar { get; set; }
        public int? CuentaPorCobrarPagoId { get; set; }
        public Pagos CuentaPorCobrarPago { get; set; }
        //public int? VentaServicioId { get; set; }
        //public VentaServicio VentaServicio { get; set; }
        public int CajaId { get; set; }
        public Caja Caja { get; set; }
        public string Descripcion { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Gasto { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Ingreso { get; set; }
        public DateTime Fecha { get; set; }

        public int? BancoId { get; set; }
        public Banco Banco { get; set; }
        public string NumeroComprabante { get; set; }
        public int? CuentaId { get; set; }
        public Cuentas Cuenta { get; set; }
        public int? CuentaContableId { get; set; }
        public CuentaContable CuentaContable { get; set; }
    }
}