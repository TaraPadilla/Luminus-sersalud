using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class Envio
    {

        public Envio()
        {

            DetalleEnvios = new List<DetalleEnvio>(); 
            Ventas = new List<Venta>();

        }
        public int Id { get; set; }


        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public int RutaId { get; set; }

        public int EstadosEnvioId {get; set;}

        public string NombrePiloto { get; set; }

        public string DireccionExacta { get; set; }

        public string NoComprobante {get; set;}

        public string Nit {get; set;}

        public string Nombres {get; set;}

        public DateTime FechaEntrega {get; set;}

        public DateTime FechaEnvio {get; set;}

        public int EmpleadoId {get; set;}

        public int ClienteId {get; set;}

        public string UserId1 {get; set;}

        public string UserId { get; set; }
        public User User {get; set;}

        public Ruta Ruta { get; set; }

        public EstadosEnvio EstadosEnvio {get;set;}

        public ICollection<DetalleEnvio> DetalleEnvios { get; set; }

         public ICollection<Venta> Ventas { get; set; }


    }
}