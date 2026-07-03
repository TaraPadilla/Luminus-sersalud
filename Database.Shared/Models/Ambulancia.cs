using System;

namespace Database.Shared.Models
{
    public class Ambulancia
    {
        public int Id { get; set; }

        public string ServicioSolicitado { get; set; }
        public DateTime FechaHoraSolicitud { get; set; }

        public string NombrePaciente { get; set; }
        public string Cama { get; set; }

        public decimal Precio { get; set; }

        public int? HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }

        public bool EsTraslado { get; set; }
        public bool EsEgreso { get; set; }
        public string DireccionTraslado { get; set; }

        public string FormaConduccion { get; set; } // Camilla, Silla de ruedas, Ambulatorio

        public string TipoViaje { get; set; } // "Privado" o "IGSS"

        public DateTime? HoraSalidaAmbulancia { get; set; }
        public DateTime? HoraEntradaAmbulancia { get; set; }
        public string AmbulanciaUnidad { get; set; } // 1, 2, otro

        public string NombrePiloto { get; set; }

        public string ExamenConsulta { get; set; }

        public string AfiliacionIGSS { get; set; }
        public DateTime? HoraExamen { get; set; }


        public bool EnfermeraAcompania { get; set; }
        public bool Oxigeno { get; set; }

        public int CantidadOxigeno { get; set; }

        public string Observaciones { get; set; }

    }

}
