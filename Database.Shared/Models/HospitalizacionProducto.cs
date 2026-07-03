using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class HospitalizacionProducto
    {
        public HospitalizacionProducto()
        {
            HospitalizacionesProductosAplicaciones = new List<HospitalizacionProductoAplicacion>();
        }
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }
        public string Indicaciones { get; set; }
        public string ViaAdministracion { get; set; }
        public string FrecuenciaAdministracion { get; set; }
        public string FechaHoraAplicacionManual { get; set; }
        

        public bool Eliminado { get; set; }
        public int? PrecioProductoId { get; set; }//Campo en proceso de ser eliminado en el futuro
        public ProductoInventarioPrecio PrecioProducto { get; set; } //Campo en proceso de ser eliminado en el futuro
        public int? PrecioId { get; set; }
        public Precio Precio { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioValor { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public UnidadMedidaVenta UnidadMedidaVenta { get; set; }
        public ICollection<HospitalizacionProductoAplicacion> HospitalizacionesProductosAplicaciones { get; set; }
    }
}
