using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class PaqueteHospitalizacion
    {
        public PaqueteHospitalizacion()
        {
            DetallePaquetesHospitalizacion = new List<DetallePaqueteHospitalizacion>();
            HospitalizacionesPaquetesHospitalizacion = new List<HospitalizacionPaqueteHospitalizacion>();
        }
        public int Id { get; set; }
        public DateTime? FechaHoraCreacion { get; set; }
        public string CodigoInterno { get; set; }
        public string NombrePaquete { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Precio { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCosto { get; set; }
        public string Descripcion { get; set; }
        public bool Eliminado { get; set; }
        public ICollection<DetallePaqueteHospitalizacion> DetallePaquetesHospitalizacion { get; set; }
        public ICollection<HospitalizacionPaqueteHospitalizacion> HospitalizacionesPaquetesHospitalizacion { get; set; }
    }
}
