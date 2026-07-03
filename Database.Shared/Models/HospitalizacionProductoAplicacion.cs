using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class HospitalizacionProductoAplicacion
    {
        public int Id { get; set; }
        public int HospitalizacionProductoId { get; set; }
        public HospitalizacionProducto HospitalizacionProducto { get; set; }
        public int Cantidad { get; set; }
        public bool Aplicado { get; set; }
        public DateTime? FechaHoraAplicacion { get; set; }
        public DateTime? FechaHoraAplicacionManual { get; set; }

        public string UsuarioAplica { get; set; }
        public string UsuarioCreaId { get; set; }
        public User UsuarioCrea { get; set; }
        public bool Eliminado { get; set; }
        public string FechaAplicacionFormateada => FechaHoraAplicacion?.ToString("dd MMM yyyy") ?? "Sin fecha";
        public string FechaAplicacionManualFormateada => FechaHoraAplicacionManual?.ToString("dd MMM yyyy") ?? "Sin fecha";
    }
}
