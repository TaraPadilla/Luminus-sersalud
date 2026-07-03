using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Database.Shared.Models
{
    public class EstadoTraslados
    {
        public EstadoTraslados()
        {
            TrasladosProductos = new List<TrasladosProductos>();
        }

        public int Id { get; set; }
        public string DescripcionEstado {get;set;}
        public ICollection<TrasladosProductos> TrasladosProductos {get;set;}
        
    }
}