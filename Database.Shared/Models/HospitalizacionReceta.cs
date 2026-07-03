using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class HospitalizacionReceta
    {
        public int Id { get; set; }
        public int RecetaId { get; set; }
        public Receta Receta { get; set; }
        public int HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }
        public string Cantidad { get; set; }
        public int CantidadAplicada { get; set; }
        public string Inidicaciones { get; set; }
        public bool Eliminado { get; set; }
        public string UsuarioCreacionId { get; set; }
        public User UsuarioCreacion { get; set; }
        public ICollection<HospitalizacionRecetaDetalle> HospitalizacionRecetaDetalle { get; set; }
    }
}
