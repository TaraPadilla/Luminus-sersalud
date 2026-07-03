using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class HospitalizacionServicio
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }
        public int HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }
        public bool Eliminado { get; set; }
        public int? PrecioServicioId { get; set; }
        public ServicioPrecio PrecioServicio { get; set; }
        public bool Aplicado { get; set; }
        public DateTime? FechaHoraAplicacion { get; set; }
        public string UsuarioAplica { get; set; }
        public string UsuarioCreaId { get; set; }
        public User UsuarioCrea { get; set; }
        public string FechaAplicacionFormateada => FechaHoraAplicacion?.ToString("dd MMM yyyy") ?? "Sin fecha";
    }
}
