using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class HospitalizacionProductoObservacion
    {
        public int Id { get; set; } 
        public int HospitalizacionProductoAplicacionId { get; set; } 
        public HospitalizacionProductoAplicacion HospitalizacionProductoAplicacion { get; set; } 
        public string Observacion { get; set; } 
        public DateTime FechaCreacion { get; set; } 
        public string UsuarioCreaId { get; set; }
        public bool Eliminado { get; set; }
    }
}