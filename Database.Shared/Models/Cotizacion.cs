using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Database.Shared.Models
{
    public class Cotizacion
    {

       public Cotizacion()
         {
             DetalleCotizacion = new List<DetalleCotizacion>();
         }   

         public int Id { get; set; }

         public string Cliente { get; set; }

         //[Required(ErrorMessage = "* Este campo es obligatorio.")]
         public string Empleado { get; set; }

         public string NoComprobante {get; set;}

         public string Nit {get; set;}

         public string Nombres {get; set;}

         public string Direccion {get; set;}

         public int? EnvioId {get;set;}

         public DateTime FechaCotizacion {get;set;}

         public DateTime FechaValida {get;set;}

         public bool Eliminado {get; set;}

         public bool Confirmado {get; set;}

         public ICollection<DetalleCotizacion> DetalleCotizacion {get;set;}

    }
}