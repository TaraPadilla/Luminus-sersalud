using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class MovimientoProductoNacional
    {
        public int Id { get; set; }
        public int ProductoInventarioId { get; set; }
        public ProductoInventario ProductoInventario { get; set; }
        public int? TipoMovimientoProductoId { get; set; }
        public TipoMovimientoProducto TipoMovimientoProducto { get; set; }
        //public int TipoProducto { get; set; }
        //public string Ambiente { get; set; }
        //public string Bodega { get; set; }
        //public string Producto { get; set; }
        public DateTime Fecha { get; set; }
        public string DescripcionMovimiento { get; set; }
        //public string Medicamento { get; set; }
        //public string Equivalencia { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }
        /// <summary>
        /// Precio de venta del producto en ese movimiento
        /// Es el precio UNITARIO
        /// </summary>
        public decimal PrecioUnitario { get; set; }
        /// <summary>
        /// Corresponde al precioUnitario * Cantidad
        /// </summary>
        public decimal MontoTotal { get; set; }
        public string UsuarioRealizaId { get; set; }

        public string UsuarioEntregaId { get; set; }

        public User UsuarioRealiza { get; set; }

        public User UsuarioEntrega { get; set; }

        public string ProveedorBodegaCliente { get; set; } //En revision
        /// <summary>
        /// Es el stock final que queda de ese lote de producto
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoActual { get; set; }

    }
}
