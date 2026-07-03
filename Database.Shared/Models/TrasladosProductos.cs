using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class TrasladosProductos
    {
        public TrasladosProductos()
        {
            DetalleTrasladoProductos = new List<DetalleTrasladoProductos>();
        }

        public int Id { get; set; }
        public int EstadoTrasladosId { get; set; }
        public DateTime? FechaTraslado { get; set; }
        public string Observaciones { get; set; }
        public bool Eliminado { get; set; }
        public bool ProductosRegresadosAFarmacia { get; set; }

        //Bodegas
        public string TipoTrasladoBodega { get; set; } // bodega a farmacia o bodega a clinica
        public int? BodegaOrigenId { get; set; }
        public Bodega BodegaOrigen { get; set; }
        public int? BodegaDestinoId { get; set; }
        public Bodega BodegaDestino { get; set; }

        public User ResponsableEnviado { get; set; }
        public User ResponsableRecibido { get; set; }
        public EstadoTraslados EstadoTraslados { get; set; }

        public ICollection<DetalleTrasladoProductos> DetalleTrasladoProductos { get; set; }


        public string ResponsableEnviadoText
        {
            get
            {
                return (ResponsableEnviado.Persona == null || ResponsableEnviado == null)
                ? "" : ResponsableEnviado.Persona.Nombre.ToString();
            }
        }

        public string ResponsableRecibidoText
        {
            get
            {
                return (ResponsableRecibido.Persona == null || ResponsableRecibido == null)
                ? "" : ResponsableRecibido.Persona.Nombre.ToString();
            }
        }

        public string EstanRegresados
        {
            get { return ProductosRegresadosAFarmacia ? "Sí" : "No"; }
        }

    }
}