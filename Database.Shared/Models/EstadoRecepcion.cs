using System.Collections.Generic;

namespace Database.Shared.Models
{

    public class EstadoRecepcion{

        public EstadoRecepcion()
        {
            Recepciones = new List<Recepcion>();
        }
        
        public int Id {get; set;}
        public string Estado {get; set;}
        public ICollection<Recepcion> Recepciones {get;set;}

    }
    
}