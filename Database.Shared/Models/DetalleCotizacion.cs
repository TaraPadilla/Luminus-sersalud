using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class DetalleCotizacion
    {
        
        public int Id {get;set;}
        public int? ProductoId {get; set;}
        public int? ServicioId {get; set;}
        public int Cantidad {get;set;}
        [Column(TypeName="decimal(18,2)")]
        public decimal Precio {get;set;}
        [Column(TypeName="decimal(18,2)")]
        public decimal Descuento {get;set;}
        [Column(TypeName="decimal(18,2)")]
        public decimal Subtotal{get;set;}
        [Column(TypeName="decimal(18,2)")]
        public decimal Total {get;set;}

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public int CotizacionId {get;set;}

        public Producto Producto {get;set;}

        public Servicio Servicio {get;set;}
        public Cotizacion Cotizacion {get;set;}

       
    }
}