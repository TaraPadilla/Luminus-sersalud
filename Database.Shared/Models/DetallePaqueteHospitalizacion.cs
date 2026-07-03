using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class DetallePaqueteHospitalizacion
    {
        public int Id { get; set; }
        public int PaqueteHospitalizacionId { get; set; }
        public PaqueteHospitalizacion PaqueteHospitalizacion { get; set; }
        public int? ServicioId { get; set; }
        public Servicio Servicio { get; set; }
        public int? ServicioPrecioId { get; set; }
        public ServicioPrecio ServicioPrecio { get; set; }
        public int? ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int? ProductoInventarioPrecioId { get; set; }
        public ProductoInventarioPrecio ProductoInventarioPrecio { get; set; }
        public int? PrecioId { get; set; }
        public Precio Precio { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public int? LaboratorioId { get; set; }
        public ExamenLabClinico Laboratorio { get; set; }
        public int? LaboratorioPrecioId { get; set; }
        public ExamenLabClinicoPrecio LaboratorioPrecio { get; set; }
        public UnidadMedidaVenta UnidadMedidaVenta { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCosto { get; set; }

        public decimal PrecioValor { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DescuentoPorcentaje { get; set; }
        public bool Eliminado { get; set; }
    }
}