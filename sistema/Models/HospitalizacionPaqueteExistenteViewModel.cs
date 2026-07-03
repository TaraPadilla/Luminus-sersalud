using System.Collections.Generic;

namespace sistema.Models
{
    public class HospitalizacionPaqueteExistenteViewModel
    {
        public int PaqueteId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string NombreMostrar
        {
            get
            {
                return $"{Codigo} - {Nombre}";
            }
        }
    }
}
