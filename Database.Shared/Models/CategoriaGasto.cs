using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class CategoriaGasto
    {

        public CategoriaGasto()
        {

            Gastos = new List<Gasto>();

        }
        public int Id { get; set; }


        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string NombreCategoria { get; set; }

        public bool Eliminado {get; set;}

        public ICollection<Gasto> Gastos { get; set; }
    }
}