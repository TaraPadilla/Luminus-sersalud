using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class CategoriaCompra
    {

        public CategoriaCompra()
        {

        }
        public int Id { get; set; }
        public string NombreCategoria { get; set; }
        public bool Eliminada {get; set;}
    }
}