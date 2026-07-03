using Database.Shared.Models;

namespace farmamest.Models
{
    public class HospitalizacionProductoObservacionVM
    {
        public int Id { get; set; } // Identificador de la observación
        public string Observacion { get; set; } // Texto de la observación
        public string FechaCreacion { get; set; } // Fecha de creación, formateada como string
        public int HospitalizacionProductoAplicacionId { get; set; } // Relación con HospitalizaciónProductoAplicación

        /// <summary>
        /// Usuario que creó la observación
        /// </summary>
        public string UsuarioCreaId { get; set; }
        public string UsuarioCreaNombre { get; set; } // Nombre del usuario que creó la observación
    }
}
