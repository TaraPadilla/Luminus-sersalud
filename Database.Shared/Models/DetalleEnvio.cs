using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Database.Shared.Models
{
    public class DetalleEnvio
    {


        public int Id { get; set; }

        public int EnvioId { get; set; }

        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Descuento { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }


        public Producto Producto { get; set; }
        public Envio Envio { get; set; }


    }
}