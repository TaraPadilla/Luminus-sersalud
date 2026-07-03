using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Database.Shared.Models
{
    public class Recepcion
    {

        public int Id { get; set; }
        public int CompraId { get; set; }
        public int EstadoRecepcionId { get; set; }
        //public int? tipoentidad { get; set; }
        public EstadoRecepcion EstadoRecepcion { get; set; }
        public Compra Compra { get; set; }

    }
}